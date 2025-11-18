using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Withdraws
{
   public interface IWithdrawManager
    {
        Task<Withdraw> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Withdraw @entity);
        Task<IdentityResult> RemoveAsync(Withdraw @entity);
        Task<IdentityResult> UpdateAsync(Withdraw @entity,bool isCheck = true);
    }
}
