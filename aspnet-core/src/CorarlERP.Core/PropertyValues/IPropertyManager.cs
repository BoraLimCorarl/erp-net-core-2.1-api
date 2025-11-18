using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PropertyValues
{
    public interface IPropertyManager: IDomainService
    {
        Task<Property> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Property @entity);
        Task<IdentityResult> RemoveAsync(Property @entity);
        Task<IdentityResult> UpdateAsync(Property @entity);
        Task<IdentityResult> DisableAsync(Property @entity);
        Task<IdentityResult> EnableAsync(Property @entity);
    }
}
