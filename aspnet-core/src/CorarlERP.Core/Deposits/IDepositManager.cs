using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Deposits
{
    public interface IDepositManager
    {
        Task<Deposit> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Deposit @entity);
        Task<IdentityResult> RemoveAsync(Deposit @entity);
        Task<IdentityResult> UpdateAsync(Deposit @entity, bool isCheck = true);
    }
}
