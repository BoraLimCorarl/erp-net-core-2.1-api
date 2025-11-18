using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.UserGroups
{
   public interface IUserGroupMemberManager
    {
        Task<UserGroupMember> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(UserGroupMember @entity);
        Task<IdentityResult> RemoveAsync(UserGroupMember @entity);
        Task<IdentityResult> UpdateAsync(UserGroupMember @entity);
    }
}
