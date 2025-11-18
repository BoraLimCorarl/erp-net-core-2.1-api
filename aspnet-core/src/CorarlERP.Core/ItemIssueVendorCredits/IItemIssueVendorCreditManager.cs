using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueVendorCredits
{
    public interface IItemIssueVendorCreditManager
    {
        Task<ItemIssueVendorCredit> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemIssueVendorCredit @entity);
        Task<IdentityResult> RemoveAsync(ItemIssueVendorCredit @entity);
        Task<IdentityResult> UpdateAsync(ItemIssueVendorCredit @entity);
    }
}
