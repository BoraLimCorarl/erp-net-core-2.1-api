using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
   public interface IRawMaterialItemManager
    {
        Task<RawMaterialItems> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(RawMaterialItems @entity);
        Task<IdentityResult> RemoveAsync(RawMaterialItems @entity);
        Task<IdentityResult> UpdateAsync(RawMaterialItems @entity);
    }
}
