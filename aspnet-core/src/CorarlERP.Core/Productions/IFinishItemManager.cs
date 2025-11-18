using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
   public interface IFinishItemManager
    {
        Task<FinishItems> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(FinishItems @entity);
        Task<IdentityResult> RemoveAsync(FinishItems @entity);
        Task<IdentityResult> UpdateAsync(FinishItems @entity);
    }
}
