using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueVendorCredits
{
    public interface IItemIssueVendorCreditItemManager
    {
        Task<ItemIssueVendorCreditItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemIssueVendorCreditItem @entity);
        Task<IdentityResult> RemoveAsync(ItemIssueVendorCreditItem @entity);
        Task<IdentityResult> UpdateAsync(ItemIssueVendorCreditItem @entity);
    }
}
