using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceipts
{
   public interface IItemReceiptItemManager
    {
        Task<ItemReceiptItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemReceiptItem @entity);
        Task<IdentityResult> RemoveAsync(ItemReceiptItem @entity);
        Task<IdentityResult> UpdateAsync(ItemReceiptItem @entity);
    }
}
