using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.PurchaseOrders
{
    public  interface IPurchaseOrderItemManager 
    {
        Task<PurchaseOrderItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PurchaseOrderItem @entity);
        Task<IdentityResult> RemoveAsync(PurchaseOrderItem @entity);
        Task<IdentityResult> UpdateAsync(PurchaseOrderItem @entity);
    }
}
