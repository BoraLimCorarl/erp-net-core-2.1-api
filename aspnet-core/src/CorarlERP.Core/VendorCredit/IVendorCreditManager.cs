using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.VendorCredit
{
    public interface IVendorCreditManager
    {
        Task<VendorCredit> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(VendorCredit @entity);
        Task<IdentityResult> RemoveAsync(VendorCredit @entity);
        Task<IdentityResult> UpdateAsync(VendorCredit @entity);
    }
}
