using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CustomerCredits
{
   public interface ICustomerCreditManager
    {
        Task<CustomerCredit> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(CustomerCredit @entity);
        Task<IdentityResult> RemoveAsync(CustomerCredit @entity);
        Task<IdentityResult> UpdateAsync(CustomerCredit @entity);
    }
}
