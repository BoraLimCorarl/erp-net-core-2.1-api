using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ChartOfAccounts
{
    public interface IAccountTypeManager: IDomainService
    {
        Task<AccountType> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(AccountType @entity);
        Task<IdentityResult> RemoveAsync(AccountType @entity);
        Task<IdentityResult> UpdateAsync(AccountType @entity);
        Task<IdentityResult> DisableAsync(AccountType @entity);
        Task<IdentityResult> EnableAsync(AccountType @entity);
    }
}
