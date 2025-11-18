using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Customers
{
  public  interface ICustomerContactPersonManager
    {
        Task<CustomerContactPerson> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(CustomerContactPerson @entity);
        Task<IdentityResult> RemoveAsync(CustomerContactPerson @entity);
        Task<IdentityResult> UpdateAsync(CustomerContactPerson @entity);
    }
}
