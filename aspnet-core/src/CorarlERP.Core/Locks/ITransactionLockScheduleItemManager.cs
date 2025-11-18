using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.Locks
{
    public interface ITransactionLockScheduleItemManager : IDomainService
    {
        Task<TransactionLockSheduleItem> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(TransactionLockSheduleItem @entity);
        Task<IdentityResult> UpdateAsync(TransactionLockSheduleItem @entity);       
        Task<IdentityResult> RemoveAsync(TransactionLockSheduleItem @entity);       
    }
}
