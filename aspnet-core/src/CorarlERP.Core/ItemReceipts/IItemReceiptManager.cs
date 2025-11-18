using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceipts
{
   public interface IItemReceiptManager
    {
        Task<ItemReceipt> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemReceipt @entity);
        Task<IdentityResult> RemoveAsync(ItemReceipt @entity);
        Task<IdentityResult> UpdateAsync(ItemReceipt @entity);
    }
}
