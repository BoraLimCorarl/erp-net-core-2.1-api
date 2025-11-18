using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.VendorCredit
{
    public interface IVendorCreditDetailManager
    {
        Task<VendorCreditDetail> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(VendorCreditDetail @entity);
        Task<IdentityResult> RemoveAsync(VendorCreditDetail @entity);
        Task<IdentityResult> UpdateAsync(VendorCreditDetail @entity);
    }
}
