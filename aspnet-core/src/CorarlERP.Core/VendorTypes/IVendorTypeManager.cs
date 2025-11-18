using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.VendorTypes
{
    public interface IVendorTypeManager : IDomainService
    {
        Task<VendorType> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(VendorType @entity);
        Task<IdentityResult> RemoveAsync(VendorType @entity);
        Task<IdentityResult> UpdateAsync(VendorType @entity);
        Task<IdentityResult> DisableAsync(VendorType @entity);
        Task<IdentityResult> EnableAsync(VendorType @entity);
    }
}
