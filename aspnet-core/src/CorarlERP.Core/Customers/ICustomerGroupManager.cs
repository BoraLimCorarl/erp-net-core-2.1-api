using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Customers
{
    public interface ICustomerGroupManager : IDomainService
    {
        Task<CustomerGroup> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(CustomerGroup @entity);
        Task<IdentityResult> RemoveAsync(CustomerGroup @entity);
        Task<IdentityResult> UpdateAsync(CustomerGroup @entity);
    }
}
