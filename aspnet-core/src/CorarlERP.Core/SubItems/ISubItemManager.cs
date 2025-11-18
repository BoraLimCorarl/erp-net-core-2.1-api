using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SubItems
{
   public interface ISubItemManager
    {
        Task<SubItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(SubItem @entity);
        Task<IdentityResult> RemoveAsync(SubItem @entity);
        Task<IdentityResult> UpdateAsync(SubItem @entity);
        Task<IdentityResult> DisableAsync(SubItem @entity);
        Task<IdentityResult> EnableAsync(SubItem @entity);
    }
}
