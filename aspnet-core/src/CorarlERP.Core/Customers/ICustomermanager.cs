using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Customers
{
  public interface ICustomerManager : IDomainService
    {
        Task<Customer> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Customer @entity);
        Task<IdentityResult> RemoveAsync(Customer @entity);
        Task<IdentityResult> UpdateAsync(Customer @entity);
        Task<IdentityResult> DisableAsync(Customer @entity);
        Task<IdentityResult> EnableAsync(Customer @entity);
    }
}
