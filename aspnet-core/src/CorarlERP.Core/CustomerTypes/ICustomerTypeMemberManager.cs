using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.CustomerTypes
{
    public interface ICustomerTypeMemberManager : IDomainService
    {
        Task<CustomerTypeMember> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(CustomerTypeMember @entity);
        Task<IdentityResult> RemoveAsync(CustomerTypeMember @entity);
        Task<IdentityResult> UpdateAsync(CustomerTypeMember @entity);
        Task<IdentityResult> DisableAsync(CustomerTypeMember @entity);
        Task<IdentityResult> EnableAsync(CustomerTypeMember @entity);
    }
}
