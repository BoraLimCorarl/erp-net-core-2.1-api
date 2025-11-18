using Abp.Application.Services.Dto;
using CorarlERP.InventoryCalculationSchedules.Dto;
using CorarlERP.Invoices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.Items;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.Domain.Uow;
using Hangfire.Storage;

namespace CorarlERP.InventoryCalculationSchedules
{
    public class InventoryCalculationScheduleAppService : CorarlERPAppServiceBase, IInventoryCalculationScheduleAppService
    {

        private readonly IInventoryCalculationJobScheduleManager _inventoryCalculationJobScheduleManager;
        private readonly ICorarlRepository<InventoryCalculationSchedule, Guid> _inventoryCalculationScheduleRepository;

        public InventoryCalculationScheduleAppService()
        {
            _inventoryCalculationJobScheduleManager = IocManager.Instance.Resolve<IInventoryCalculationJobScheduleManager>();
            _inventoryCalculationScheduleRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryCalculationSchedule, Guid>>();
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule)]
        [UnitOfWork(IsDisabled = true)]
        public async Task Execute(EntityDto<Guid> input)
        {
            await _inventoryCalculationJobScheduleManager.ExecuteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _inventoryCalculationJobScheduleManager.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            await _inventoryCalculationJobScheduleManager.EnableAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            await _inventoryCalculationJobScheduleManager.DisableAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_GetDetail)]
        public async Task<InventoryCalculationScheduleDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var job = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs().FirstOrDefault(s => s.Id == input.Id.ToString());
            if (job == null) throw new UserFriendlyException(L("RecordNotFound"));

            var item = await _inventoryCalculationScheduleRepository.FirstOrDefaultAsync(s => s.Id == input.Id);
            if(item == null) throw new UserFriendlyException(L("RecordNotFound"));

            return new InventoryCalculationScheduleDetailOutput
            {
                Id = item.Id,
                CreatedAt = job.CreatedAt.Value,
                NextExecution = job.NextExecution,
                LastExecution = job.LastExecution,
                LastJobState = job.LastJobState,
                Cron = job.Cron,
                Queue = job.Queue,
                Method = job.Job.Method.Name,
                ScheduleTime = item.ScheduleTime,
                IsActive = item.IsActive
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_GetList)]
        [HttpPost]
        public async Task<PagedResultDto<GetListInventoryCalculationScheduleOutput>> GetList(GetListInventoryCalculationScheduleInput input)
        {
            const string MethodName = "ScheduleCalculationByTenant";

            var jobs = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs()
                      .Where(s => s.Job.Method.Name == MethodName)
                      .ToList();

            var query = from i in _inventoryCalculationScheduleRepository.GetAll()
                                  .Where(s => !input.IsActive.HasValue || input.IsActive == s.IsActive)
                                   .AsNoTracking()
                        join j in jobs
                        on $"{i.Id}" equals j.Id
                        into js from j in js.DefaultIfEmpty()
                        orderby i.ScheduleTime
                        select new GetListInventoryCalculationScheduleOutput
                        {
                            Id = i.Id,
                            CreatedAt = j != null ? j.CreatedAt : (DateTime?) null,
                            NextExecution = j != null ? j.NextExecution : (DateTime?) null,
                            LastExecution = j !=  null ? j.LastExecution : (DateTime?) null,
                            LastJobState = j != null ? j.LastJobState : "",
                            Cron = j != null ? j.Cron : "",
                            Queue = j != null ? j.Queue : "",
                            Method = MethodName,
                            ScheduleTime = i.ScheduleTime,
                            IsActive = i.IsActive
                        };

            var count = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<GetListInventoryCalculationScheduleOutput> { TotalCount = count, Items = items };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_Create)]
        public async Task Create(InventoryCalculationScheduleDto input)
        {
            await _inventoryCalculationJobScheduleManager.CreateAsync(AbpSession.UserId.Value, input.ScheduleTime);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_CalculationSchedule_Edit)]
        public async Task Update(InventoryCalculationScheduleDto input)
        {
            await _inventoryCalculationJobScheduleManager.UpdateAsync(AbpSession.UserId.Value, input.Id, input.ScheduleTime);
        }
    }
}
