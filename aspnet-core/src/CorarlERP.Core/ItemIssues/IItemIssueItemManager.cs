using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssues
{
  public interface IItemIssueItemManager
    {
        Task<ItemIssueItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemIssueItem @entity);
        Task<IdentityResult> RemoveAsync(ItemIssueItem @entity);
        Task<IdentityResult> UpdateAsync(ItemIssueItem @entity);
    }
}
