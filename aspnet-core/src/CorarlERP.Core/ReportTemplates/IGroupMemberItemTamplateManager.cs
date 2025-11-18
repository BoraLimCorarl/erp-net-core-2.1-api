using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReportTemplates
{
   public interface IGroupMemberItemTamplateManager
    {
        Task<GroupMemberItemTamplate> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(GroupMemberItemTamplate @entity);
        Task<IdentityResult> RemoveAsync(GroupMemberItemTamplate @entity);
        Task<IdentityResult> UpdateAsync(GroupMemberItemTamplate @entity);
    }
}
