using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Items
{
    public interface IItemManager : IDomainService
    {
        Task<Item> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Item @entity);
        Task<IdentityResult> RemoveAsync(Item @entity);
        Task<IdentityResult> UpdateAsync(Item @entity);
        Task<IdentityResult> DisableAsync(Item @entity);
        Task<IdentityResult> EnableAsync(Item @entity);
    }
}
