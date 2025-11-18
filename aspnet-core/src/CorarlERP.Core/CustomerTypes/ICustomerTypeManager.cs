using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CustomerTypes
{
   public interface ICustomerTypeManager : IDomainService
    {
        Task<CustomerType> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(CustomerType @entity);
        Task<IdentityResult> RemoveAsync(CustomerType @entity);
        Task<IdentityResult> UpdateAsync(CustomerType @entity);
        Task<IdentityResult> DisableAsync(CustomerType @entity);
        Task<IdentityResult> EnableAsync(CustomerType @entity);
    }
}
