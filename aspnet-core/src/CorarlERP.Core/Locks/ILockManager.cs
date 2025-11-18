using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.Locks
{
    public interface ILockManager : IDomainService
    {
        Task<Lock> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Lock @entity);
        Task<IdentityResult> UpdateAsync(Lock @entity);       
    }
}
