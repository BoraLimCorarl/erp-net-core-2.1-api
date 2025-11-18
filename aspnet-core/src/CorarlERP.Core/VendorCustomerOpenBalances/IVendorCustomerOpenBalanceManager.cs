using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.VendorCustomerOpenBalances
{
    public interface IVendorOpenBalanceManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(VendorOpenBalance @entity);
        Task<IdentityResult> RemoveAsync(VendorOpenBalance @entity);
    }

    public interface ICustomerOpenBalanceManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(CustomerOpenBalance @entity);
        Task<IdentityResult> RemoveAsync(CustomerOpenBalance @entity);
    }
}
