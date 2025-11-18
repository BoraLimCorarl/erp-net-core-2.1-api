using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Journals
{
   public interface IJournalItemManager
    {
        Task<JournalItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(JournalItem @entity);
        Task<IdentityResult> RemoveAsync(JournalItem @entity);
        Task<IdentityResult> UpdateAsync(JournalItem @entity);
    }
}
