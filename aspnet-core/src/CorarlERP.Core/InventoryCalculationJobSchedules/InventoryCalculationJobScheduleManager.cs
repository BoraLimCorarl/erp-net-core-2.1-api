using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Linq;
using CorarlERP.Inventories;
using Abp.Timing;
using Abp.UI;
using Abp.Extensions;
using Abp.Domain.Services;
using CorarlERP.Items;
using CorarlERP.InventoryCalculationSchedules;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CorarlERP.InventoryCalculationJobSchedules
{
    public class InventoryCalculationJobScheduleManager : CorarlERPDomainServiceBase, IInventoryCalculationJobScheduleManager
    {
        private readonly ICorarlRepository<InventoryCalculationSchedule, Guid> _inventoryCalculationScheduleRepository;
        private readonly ICorarlRepository<InventoryCalculationItem, Guid> _inventoryCalculationItemRepository;
        private readonly ICorarlRepository<Tenant> _tenantRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public InventoryCalculationJobScheduleManager()
        {
            _inventoryCalculationScheduleRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryCalculationSchedule, Guid>>();
            _inventoryCalculationItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryCalculationItem, Guid>>();
            _tenantRepository = IocManager.Instance.Resolve<ICorarlRepository<Tenant>>();
            _inventoryManager = IocManager.Instance.Resolve<IInventoryManager>();
            _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
        }

       
        public async Task CreateAsync(long userId, DateTime date)
        {

            var schedule = await _inventoryCalculationScheduleRepository.GetAll()
                                .Where(s => s.ScheduleTime.Date == date.Date.Date &&
                                            s.ScheduleTime.Hour == date.Hour &&
                                            s.ScheduleTime.Minute == date.Minute)
                                .FirstOrDefaultAsync();


            if (schedule != null) throw new UserFriendlyException(L("DupplicationScheduleTime"));
         
          
            schedule = InventoryCalculationSchedule.Create(userId, date);
            await _inventoryCalculationScheduleRepository.InsertAsync(schedule);

            await CreateOrUpdateScheduleJobHelper(schedule.Id, schedule.ScheduleTime);
        }

        public async Task UpdateAsync(long userId, Guid id, DateTime date)
        {

            var schedule = await _inventoryCalculationScheduleRepository.GetAll()
                                .Where(s => s.Id == id)
                                .FirstOrDefaultAsync();

            if (schedule == null) throw new UserFriendlyException(L("RecordNotFound"));
           
            schedule.Update(userId, date);
            await _inventoryCalculationScheduleRepository.UpdateAsync(schedule);           

            await CreateOrUpdateScheduleJobHelper(schedule.Id, schedule.ScheduleTime);
        }

        public async Task DeleteAsync(Guid scheduleId)
        {           
            var item = await _inventoryCalculationScheduleRepository.GetAll()
                                .Where(s => s.Id == scheduleId)
                                .FirstOrDefaultAsync();

            if (item == null) throw new UserFriendlyException(L("RecordNotFound"));

            await _inventoryCalculationScheduleRepository.DeleteAsync(item);
            await DeleteScheduleJobHelper(scheduleId);
        }

        public async Task EnableAsync(Guid scheduleId)
        {
            var item = await _inventoryCalculationScheduleRepository.GetAll()
                                .Where(s => s.Id == scheduleId)
                                .FirstOrDefaultAsync();

            if (item == null) throw new UserFriendlyException(L("RecordNotFound"));

            item.Enable(true);
            await _inventoryCalculationScheduleRepository.UpdateAsync(item);
            await CreateOrUpdateScheduleJobHelper(scheduleId, item.ScheduleTime);
        }

        public async Task DisableAsync(Guid scheduleId)
        {
            var item = await _inventoryCalculationScheduleRepository.GetAll()
                                .Where(s => s.Id == scheduleId)
                                .FirstOrDefaultAsync();

            if (item == null) throw new UserFriendlyException(L("RecordNotFound"));

            item.Enable(false);
            await _inventoryCalculationScheduleRepository.UpdateAsync(item);
            await DeleteScheduleJobHelper(scheduleId);
        }


        public async Task ExecuteAsync(Guid scheduleId)
        {
            await Task.Run(() => { RecurringJob.Trigger($"{scheduleId}"); });
        }

        protected async Task CreateOrUpdateScheduleJobHelper(Guid scheduleId, DateTime time)
        {
            var userTimezone = MyTimeZoneInfo();
           
            await Task.Run(() =>
            {
                //var scheduleTime = TimeZoneInfo.ConvertTime(time, userTimezone);
                var scheduleTime = time;
                var minutes = scheduleTime.Minute;
                var hours = scheduleTime.Hour;

                RecurringJob.AddOrUpdate($"{scheduleId}", 
                                        () => ScheduleCalculationByTenant(scheduleId), 
                                        Cron.Daily(hours, minutes),
                                        userTimezone);

            });
        }

        private async Task DeleteScheduleJobHelper(Guid scheduleId)
        {
            await Task.Run(() =>
            {
                RecurringJob.RemoveIfExists($"{scheduleId}"); // this will delete the recurring jobs on the basis of their id
            });
        }

        public async Task ScheduleCalculationByTenant(Guid scheduleId)
        {
            //var tenants = new List<Tenant>();
            var scheduleItems = new Dictionary<int, int>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //tenants = await _tenantRepository.GetAll().Where(s => s.IsActive).AsNoTracking().ToListAsync();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
                {
                    scheduleItems = await _inventoryCalculationItemRepository.GetAll()
                                          .AsNoTracking()
                                          .GroupBy(s => s.TenantId)
                                          .ToDictionaryAsync(k => k.Key.Value, v => v.Key.Value);
                }
            }

            if (!scheduleItems.Any()) return;

            var time = DateTime.UtcNow.AddMinutes(1);
            foreach (var t in scheduleItems)
            {
               await CreateOrUpdateScheduleCalculationJobHelper(t.Key, Guid.NewGuid(), time);
            }

        }

        protected async Task CreateOrUpdateScheduleCalculationJobHelper(int tenantId, Guid scheduleId, DateTime time)
        {
            var userTimezone = MyTimeZoneInfo();

            await Task.Run(() =>
            {
                var scheduleTime = TimeZoneInfo.ConvertTime(time, userTimezone);            
                var minutes = scheduleTime.Minute;
                var hours = scheduleTime.Hour;

                RecurringJob.AddOrUpdate($"{scheduleId}",
                                        () => TryCalculation(tenantId, scheduleId),
                                        Cron.Daily(hours, minutes),
                                        userTimezone);

            });
        }

        public async Task TryCalculation(int tenantId, Guid scheduleId)
        {
            var scheduleItems = new List<InventoryCalculationItem>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    scheduleItems = await _inventoryCalculationItemRepository.GetAll()
                                          .AsNoTracking()
                                          .OrderBy(s => s.Date)
                                          .ToListAsync();
                }
            }

            if (scheduleItems == null || !scheduleItems.Any())
            {
                await DeleteScheduleJobHelper(scheduleId);
                return;
            }


            var timeZone = MyTimeZoneInfo();
            var utcNow = DateTime.UtcNow;
            var toDate = TimeZoneInfo.ConvertTime(utcNow, timeZone);

            var itemByDates = scheduleItems.GroupBy(s => s.Date.Date).ToList();
            foreach(var g in itemByDates)
            {
                var itemIds = g.Select(s => s.ItemId).ToList();
                var fromDate = g.Key;

                //take long time execution
                await _inventoryManager.RecalculationCostHelper(tenantId, fromDate, toDate, itemIds);
            };

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _inventoryCalculationItemRepository.BulkDeleteAsync(scheduleItems);
                }
                await uow.CompleteAsync();
            }

            await DeleteScheduleJobHelper(scheduleId);
           
        }
    }
}
