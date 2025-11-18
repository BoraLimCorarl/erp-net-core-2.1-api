using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.AccountCycles
{
    public interface IAccountCycleManager : IDomainService
    {
        Task<AccountCycle> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(AccountCycle @entity);
        Task<IdentityResult> RemoveAsync(AccountCycle @entity);
        Task<IdentityResult> UpdateAsync(AccountCycle @entity);
        Task<IdentityResult> DisableAsync(AccountCycle @entity);
        Task<IdentityResult> EnableAsync(AccountCycle @entity);
    }
}
