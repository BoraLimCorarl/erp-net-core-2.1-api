using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.Withdraws
{
    public interface IWithdrawItemManager
    {
        Task<WithdrawItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(WithdrawItem @entity);
        Task<IdentityResult> RemoveAsync(WithdrawItem @entity);
        Task<IdentityResult> UpdateAsync(WithdrawItem @entity);
    }
}
