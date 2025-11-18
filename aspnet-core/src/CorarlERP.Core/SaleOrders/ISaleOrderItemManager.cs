using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SaleOrders
{
    public interface ISaleOrderItemManager
    {
        Task<SaleOrderItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(SaleOrderItem @entity);
        Task<IdentityResult> RemoveAsync(SaleOrderItem @entity);
        Task<IdentityResult> UpdateAsync(SaleOrderItem @entity);
    }
}
