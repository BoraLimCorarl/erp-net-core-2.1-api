using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.ChartOfAccounts
{
    public interface IChartOfAccountManager: IDomainService
    {
        Task<ChartOfAccount> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ChartOfAccount @entity);
        Task<IdentityResult> RemoveAsync(ChartOfAccount @entity);
        Task<IdentityResult> UpdateAsync(ChartOfAccount @entity);
        Task<IdentityResult> DisableAsync(ChartOfAccount @entity);
        Task<IdentityResult> EnableAsync(ChartOfAccount @entity);
    }
}
