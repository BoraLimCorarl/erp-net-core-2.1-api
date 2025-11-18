using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.VendorTypes
{
    public interface IVendorTypeMemberManager : IDomainService
    {
        Task<VendorTypeMember> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(VendorTypeMember @entity);
        Task<IdentityResult> RemoveAsync(VendorTypeMember @entity);
        Task<IdentityResult> UpdateAsync(VendorTypeMember @entity);
        Task<IdentityResult> DisableAsync(VendorTypeMember @entity);
        Task<IdentityResult> EnableAsync(VendorTypeMember @entity);
    }
}
