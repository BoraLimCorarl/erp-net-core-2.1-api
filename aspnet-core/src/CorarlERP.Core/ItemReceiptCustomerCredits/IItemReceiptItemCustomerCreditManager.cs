using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    public interface IItemReceiptItemCustomerCreditManager
    {
        Task<ItemReceiptItemCustomerCredit> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemReceiptItemCustomerCredit @entity);
        Task<IdentityResult> RemoveAsync(ItemReceiptItemCustomerCredit @entity);
        Task<IdentityResult> UpdateAsync(ItemReceiptItemCustomerCredit @entity);
    }
}
