using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CustomerCredits
{
    public interface ICustomerCreditDetailManager
    {
        Task<CustomerCreditDetail> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(CustomerCreditDetail @entity);
        Task<IdentityResult> RemoveAsync(CustomerCreditDetail @entity);
        Task<IdentityResult> UpdateAsync(CustomerCreditDetail @entity);
    }
}
