using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Deposits
{
    public interface IDepositItemManager
    {
        Task<DepositItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(DepositItem @entity);
        Task<IdentityResult> RemoveAsync(DepositItem @entity);
        Task<IdentityResult> UpdateAsync(DepositItem @entity);
    }
}
