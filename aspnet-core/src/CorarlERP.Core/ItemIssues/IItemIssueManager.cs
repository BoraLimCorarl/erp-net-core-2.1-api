using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssues
{
   public interface IItemIssueManager
    {
        Task<ItemIssue> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemIssue @entity);
        Task<IdentityResult> RemoveAsync(ItemIssue @entity);
        Task<IdentityResult> UpdateAsync(ItemIssue @entity);
    }
}
