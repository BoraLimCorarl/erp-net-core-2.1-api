using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PhysicalCount
{
    public interface IPhysicalCountItemManager
    {
        Task<PhysicalCountItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PhysicalCountItem @entity);
        Task<IdentityResult> RemoveAsync(PhysicalCountItem @entity);
        Task<IdentityResult> UpdateAsync(PhysicalCountItem @entity);
    }
}
