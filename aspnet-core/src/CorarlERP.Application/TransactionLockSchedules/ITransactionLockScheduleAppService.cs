using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.EventBridge.Dto;
using CorarlERP.PermissionLocks.Dto;
using CorarlERP.TransactionLockSchedules.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorarlERP.TransactionLockSchedules
{
    public interface ITransactionLockScheduleAppService : IApplicationService
    {
        Task<TransactionLockScheduleDetailOutput> Create(CreateTransactionLockScheduleInput input);
        Task<PagedResultDto<TransactionLockScheduleDetailOutput>> GetList(GetTransactionLockScheduleListInput input);
        Task<TransactionLockScheduleDetailOutput> GetDetail(EntityDto<long> input);
        Task<TransactionLockScheduleDetailOutput> Update(UpdateTransactionLockScheduleInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task<string> ScheduleLock(SyncDataInput input);


    }
}
