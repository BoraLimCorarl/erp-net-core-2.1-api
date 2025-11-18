using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PropertyValues
{
  public  interface IPropertyValueManager: IDomainService
    {
        Task<PropertyValue> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PropertyValue @entity);
        Task<IdentityResult> RemoveAsync(PropertyValue @entity);
        Task<IdentityResult> UpdateAsync(PropertyValue @entity);
        Task<IdentityResult> DisableAsync(PropertyValue @entity);
        Task<IdentityResult> EnableAsync(PropertyValue @entity);
    }
}
