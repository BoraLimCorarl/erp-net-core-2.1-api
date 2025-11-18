using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Vendors
{
    public interface  IVendorGroupManager : IDomainService
    {
        Task<VendorGroup> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(VendorGroup @entity);
        Task<IdentityResult> RemoveAsync(VendorGroup @entity);
        Task<IdentityResult> UpdateAsync(VendorGroup @entity);
    }
}
