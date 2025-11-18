using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Items
{
    public interface IItemUserGroupManager : IDomainService
    {

        Task<ItemsUserGroup> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemsUserGroup @entity);
        Task<IdentityResult> RemoveAsync(ItemsUserGroup @entity);
        Task<IdentityResult> UpdateAsync(ItemsUserGroup @entity);
    }

}
