using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.UserGroups
{
   public interface IUserGroupManager : IDomainService
    {
        Task<UserGroup> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(UserGroup @entity);
        Task<IdentityResult> RemoveAsync(UserGroup @entity);
        Task<IdentityResult> UpdateAsync(UserGroup @entity);
        Task<IdentityResult> DisableAsync(UserGroup @entity);
        Task<IdentityResult> EnableAsync(UserGroup @entity);
    }
}
