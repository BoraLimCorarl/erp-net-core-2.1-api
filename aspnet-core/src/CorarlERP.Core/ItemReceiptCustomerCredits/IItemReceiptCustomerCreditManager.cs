using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    public interface IItemReceiptCustomerCreditManager
    {
        Task<ItemReceiptCustomerCredit> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemReceiptCustomerCredit @entity);
        Task<IdentityResult> RemoveAsync(ItemReceiptCustomerCredit @entity);
        Task<IdentityResult> UpdateAsync(ItemReceiptCustomerCredit @entity);
    }
}
