using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Lots.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Locations.Dto;
using CorarlERP.Locations;
using CorarlERP.UserGroups;
using System;
using CorarlERP.Locks;
using CorarlERP.TransactionLockSchedules.Dto;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using CorarlERP.JobSchedulings;
using Abp.Timing;
using Microsoft.Extensions.Configuration;
using CorarlERP.Configuration;
using CorarlERP.EventBridge.Dto;
using CorarlERP.EventBridge;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Abp.Domain.Uow;
using System.Transactions;

namespace CorarlERP.TransactionLockSchedules
{
    public class TransactionLockScheduleAppService : CorarlERPAppServiceBase, ITransactionLockScheduleAppService
    {
        private readonly ITransactionLockScheduleManager _transactionLockScheduleManager;
        private readonly ITransactionLockScheduleItemManager _transactionLockScheduleItemManager;
        private readonly IRepository<TransactionLockSchedule, long> _transactionLockScheduleRepository;
        private readonly IRepository<TransactionLockSheduleItem, long> _transactionLockScheduleItemRepository;

        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly ITransactionLockJobSchedulingManager _jobSchedulingManager;
        private readonly IAppConfigurationAccessor _configuration;
        private readonly IEventBridgeManager _iEventBridgeManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public TransactionLockScheduleAppService(
            ITransactionLockScheduleManager transactionLockScheduleManager,
            ITransactionLockScheduleItemManager transactionLockScheduleItemManager,
            IRepository<TransactionLockSchedule, long> transactionLockScheduleRepository,
            IRepository<TransactionLockSheduleItem, long> transactionLockScheduleItemRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            ITransactionLockJobSchedulingManager jobSchedulingManager,
            IEventBridgeManager iEventBridgeManager,
            IAppConfigurationAccessor configuration,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _transactionLockScheduleManager = transactionLockScheduleManager;
            _transactionLockScheduleItemManager = transactionLockScheduleItemManager;
            _transactionLockScheduleRepository = transactionLockScheduleRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _transactionLockScheduleItemRepository = transactionLockScheduleItemRepository;
            _jobSchedulingManager = jobSchedulingManager;
            _configuration = configuration;
            _iEventBridgeManager = iEventBridgeManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<TransactionLockScheduleDetailOutput> Create(CreateTransactionLockScheduleInput input)
        {
            this.CheckItems(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = new TransactionLockSchedule();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    @entity = TransactionLockSchedule.Create(tenantId, userId, input.ScheduleType, input.ScheduleTime, input.ScheduleDate, input.DaysBeforeYesterday);
                    CheckErrors(await _transactionLockScheduleManager.CreateAsync(@entity));
                    await CurrentUnitOfWork.SaveChangesAsync();
                    await AddItems(input.TransactionLockScheduleItems, entity);
                }
                await uow.CompleteAsync();
            }
            if (bool.Parse(_configuration.Configuration["AWS:Scheduler:Enable"]))
            {
                await CreateSheduleJobHerperLambda(entity, tenantId, userId);
            }
            else
            {
                await CreateScheduleJobHelper(entity, tenantId, userId);
            }
            return ObjectMapper.Map<TransactionLockScheduleDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _transactionLockScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var id = entity.Id;
            var items = await _transactionLockScheduleItemRepository.GetAll().Where(s => s.TransactionLockSheduleId == input.Id).ToListAsync();
            foreach (var del in items)
            {
                CheckErrors(await _transactionLockScheduleItemManager.RemoveAsync(del));
            }

            CheckErrors(await _transactionLockScheduleManager.RemoveAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();


            if (bool.Parse(_configuration.Configuration["AWS:Scheduler:Enable"]))
            {
                await DeleteScheduleJobLambdaHelper(id);
            }
            else
            {
                await DeleteScheduleJobHelper(id);
            }


        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _transactionLockScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var id = entity.Id;
            CheckErrors(await _transactionLockScheduleManager.DisableAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            if (bool.Parse(_configuration.Configuration["AWS:Scheduler:Enable"]))
            {
                await DeleteScheduleJobLambdaHelper(id);
            }
            else
            {
                await DeleteScheduleJobHelper(id);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task Enable(EntityDto<long> input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _transactionLockScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _transactionLockScheduleManager.EnableAsync(@entity));

            if (bool.Parse(_configuration.Configuration["AWS:Scheduler:Enable"]))
            {
                await CreateSheduleJobHerperLambda(entity, tenantId, userId);
            }
            else
            {
                await CreateScheduleJobHelper(entity, tenantId, userId);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_GetList)]
        public async Task<TransactionLockScheduleDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _transactionLockScheduleManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var items = await _transactionLockScheduleItemRepository.GetAll()
                              .Where(s => s.TransactionLockSheduleId == input.Id)
                              .Select(s => new TransactionLockScheduleItemDto
                              {
                                  Id = s.Id,
                                  TransactionLockType = s.LockTransaction,
                                  TransactionLockTypeName = s.LockTransaction.ToString(),
                              })
                              .ToListAsync();

            var result = ObjectMapper.Map<TransactionLockScheduleDetailOutput>(@entity);
            result.TransactionLockScheduleItems = items;

            return result;
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_GetList)]
        public async Task<PagedResultDto<TransactionLockScheduleDetailOutput>> GetList(GetTransactionLockScheduleListInput input)
        {
            var @query = from s in _transactionLockScheduleRepository
                .GetAll()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .AsNoTracking()

                         join si in _transactionLockScheduleItemRepository.GetAll().AsNoTracking()
                         on s.Id equals si.TransactionLockSheduleId
                         into items

                         select new TransactionLockScheduleDetailOutput
                         {
                             Id = s.Id,
                             ScheduleDate = s.ScheduleDate,
                             DaysBeforeYesterday = s.DaysBeforeYesterday,
                             ScheduleDateName = s.ScheduleDate.ToString(),
                             ScheduleTime = s.ScheduleTime,
                             ScheduleType = s.ScheduleType,
                             ScheduleTypeName = s.ScheduleType.ToString(),
                             TransactionLockScheduleItems = items.Select(i => new TransactionLockScheduleItemDto
                             {
                                 Id = i.Id,
                                 TransactionLockType = i.LockTransaction,
                                 TransactionLockTypeName = i.LockTransaction.ToString()
                             }).ToList(),
                             IsActive = s.IsActive,
                         };

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<TransactionLockScheduleDetailOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task<TransactionLockScheduleDetailOutput> Update(UpdateTransactionLockScheduleInput input)
        {
            this.CheckItems(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _transactionLockScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.ScheduleType, input.ScheduleTime, input.ScheduleDate, input.DaysBeforeYesterday);

            CheckErrors(await _transactionLockScheduleManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            var items = await _transactionLockScheduleItemRepository.GetAll()
                             .Where(s => s.TransactionLockSheduleId == input.Id)
                             .ToListAsync();

            var addItems = input.TransactionLockScheduleItems.Where(s => s.Id == 0).ToList();
            var editItems = input.TransactionLockScheduleItems.Where(s => s.Id > 0).ToList();
            var editIds = editItems.Select(s => s.Id).ToList();
            var deleteItems = items.Where(s => !editIds.Contains(s.Id)).ToList();

            await AddItems(addItems, entity);

            foreach (var item in editItems)
            {
                var editItem = items.FirstOrDefault(s => s.Id == item.Id);

                if (editItem == null) throw new UserFriendlyException(this.L("RecordNotFound"));

                editItem.Update(userId, entity.Id, item.TransactionLockType);

                CheckErrors(await _transactionLockScheduleItemManager.UpdateAsync(editItem));

            }

            foreach (var item in deleteItems)
            {
                CheckErrors(await _transactionLockScheduleItemManager.RemoveAsync(item));
            }

            if (bool.Parse(_configuration.Configuration["AWS:Scheduler:Enable"]))
            {
                await DeleteScheduleJobLambdaHelper(entity.Id);
                await CreateSheduleJobHerperLambda(entity, tenantId, userId);
            }
            else
            {
                await CreateScheduleJobHelper(entity, tenantId, userId);
            }

            return ObjectMapper.Map<TransactionLockScheduleDetailOutput>(@entity);
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Host_Auto_Log_Transacction)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<string> ScheduleLock(SyncDataInput input)
        {
            try
            {              
                await _jobSchedulingManager.ScheduleLock(input.TenantId, input.UserId.Value,Convert.ToInt64(input.JobId));
                return "Successfull";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }


        #region function helper
        private async Task CreateScheduleJobHelper(TransactionLockSchedule entity, int tenantId, long userId)
        {

            var userTimezone = MyTimeZoneInfo();

            await Task.Run(() =>
            {

                if (@entity.ScheduleType == ScheduleType.Daily)
                {
                    var time = entity.ScheduleTime;
                    var minutes = time.Minute;
                    var hours = time.Hour;
                    var dayOfWeek = time.DayOfWeek;

                    RecurringJob.AddOrUpdate($"transaction_lock_{@entity.Id.ToString()}",
                                            () => _jobSchedulingManager.ScheduleLock(tenantId, userId, entity.Id),
                                            Cron.Daily(hours, minutes),
                                            userTimezone);
                }


            });
        }
        private async Task CreateSheduleJobHerperLambda(TransactionLockSchedule entity, int tenantId, long userId)
        {
            var time = entity.ScheduleTime;
            var minutes = time.Minute;
            var hours = time.Hour;
            var monthdayM = new DateTime(2023, 09, 04, hours, minutes, 0);
            TimeZoneInfo tz = base.MyTimeZoneInfo();
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(monthdayM, tz);
            var expression = $"cron({$"{utcTime.Minute} {utcTime.Hour} ? * * *"})";
            var createEvnentBridgeInput = new CreateEventBridgeInput
            {
                Description = "",
                Expression = expression,
                JobId = entity.Id,
                Input = new SyncDataInput
                {
                    JobId = entity.Id,
                    //ScheduleDate = entity.ScheduleDate,
                    // ScheduleTime = entity.ScheduleTime,
                    TenantId = tenantId,
                    //ThowExeption = false,
                    UserId = userId
                },
            };
            await _iEventBridgeManager.CreateEventBridge(createEvnentBridgeInput);
        }
        private async Task DeleteScheduleJobHelper(long id)
        {

            await Task.Run(() =>
            {
                RecurringJob.RemoveIfExists($"transaction_lock_{id.ToString()}"); // this will delete the recurring jobs on the basis of their id
            });
        }

        private async Task DeleteScheduleJobLambdaHelper(long id)
        {
            try
            {
                var input = new DeleteEventBridgeInput { JobId = id };
                await _iEventBridgeManager.DeleteEventBridge(input);
            }
            catch
            {

            }

        }



        private async Task AddItems(ICollection<TransactionLockScheduleItemDto> addItems, TransactionLockSchedule entity)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            foreach (var item in addItems)
            {
                var scheduleItem = TransactionLockSheduleItem.Create(tenantId, userId, entity.Id, item.TransactionLockType);
                CheckErrors(await _transactionLockScheduleItemManager.CreateAsync(scheduleItem));
            }
        }

        private void CheckItems(CreateTransactionLockScheduleInput input)
        {
            if (!input.TransactionLockScheduleItems.Any()) throw new UserFriendlyException(L("PleaseSelect", L("Transaction")));
        }
        #endregion


    }
}
