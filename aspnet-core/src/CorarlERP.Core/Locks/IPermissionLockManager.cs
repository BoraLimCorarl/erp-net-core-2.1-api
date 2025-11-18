using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.Locks
{
    public interface IPermissionLockManager : IDomainService
    {
        Task<PermissionLock> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PermissionLock @entity);
        Task<IdentityResult> UpdateAsync(PermissionLock @entity);       
        Task<IdentityResult> RemoveAsync(PermissionLock @entity);       
        Task<IdentityResult> EnableAsync(PermissionLock @entity);       
        Task<IdentityResult> DisableAsync(PermissionLock @entity);       
    }
}
