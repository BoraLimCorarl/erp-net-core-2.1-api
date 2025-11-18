using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Vendors
{
    public interface IVendorManager : IDomainService
    {
        Task<Vendor> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Vendor @entity);
        Task<IdentityResult> RemoveAsync(Vendor @entity);
        Task<IdentityResult> UpdateAsync(Vendor @entity);
        Task<IdentityResult> DisableAsync(Vendor @entity);
        Task<IdentityResult> EnableAsync(Vendor @entity);
    }
}
