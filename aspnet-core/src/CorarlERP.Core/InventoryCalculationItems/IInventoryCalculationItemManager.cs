using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryCalculationItems
{
   public interface IInventoryCalculationItemManager
    {
        Task<InventoryCalculationItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(InventoryCalculationItem @entity);
        Task<IdentityResult> RemoveAsync(InventoryCalculationItem @entity);
        Task<IdentityResult> UpdateAsync(InventoryCalculationItem @entity);

        Task<IdentityResult> TrackChangeAsync(int tenantId, long userId, DateTime date, List<Guid> items);
    }
}
