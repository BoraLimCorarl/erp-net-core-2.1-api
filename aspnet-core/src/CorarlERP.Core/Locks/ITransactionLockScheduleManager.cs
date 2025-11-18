using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.Locks
{
    public interface ITransactionLockScheduleManager : IDomainService
    {
        Task<TransactionLockSchedule> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(TransactionLockSchedule @entity);
        Task<IdentityResult> UpdateAsync(TransactionLockSchedule @entity);       
        Task<IdentityResult> RemoveAsync(TransactionLockSchedule @entity);       
        Task<IdentityResult> EnableAsync(TransactionLockSchedule @entity);       
        Task<IdentityResult> DisableAsync(TransactionLockSchedule @entity);       
    }
}
