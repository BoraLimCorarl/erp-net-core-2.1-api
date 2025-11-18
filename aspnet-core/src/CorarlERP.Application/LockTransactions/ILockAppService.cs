using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.LockTransactions.Dto;
using CorarlERP.TransactionLockSchedules.Dto;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.LockTransactions
{
    public interface ILockAppService : IApplicationService
    {
        Task<PagedResultDto<GetListLockOutput>> GetList(GetListLockInput input);
        Task<NullableIdDto<long>> CreateOrUpdate(UpdateLockInput input);
        //Task<GetListLockOutput> CreateOrUpdate(UpdateLockInput input);
        string GenenratePasswork();
        Task<PagedResultDto<GetListLockOutput>> Find(GetListFindLockInput input);

        PagedResultDto<LockTransactionActionOutput> FindLockActions(string filter);
        PagedResultDto<LockTransactionActionOutput> FindLockTransactions(string filter);

        Task AutoLock(TransactionLockScheduleDetailOutput input);

        Task RemoveDuplicateLock();
        Task ClearLock();
    }
}
