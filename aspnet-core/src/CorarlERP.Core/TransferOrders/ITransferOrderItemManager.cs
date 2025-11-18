using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.TransferOrders
{
    public interface ITransferOrderItemManager
    {
        Task<TransferOrderItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(TransferOrderItem @entity);
        Task<IdentityResult> RemoveAsync(TransferOrderItem @entity);
        Task<IdentityResult> UpdateAsync(TransferOrderItem @entity);
    }
}
