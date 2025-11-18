using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.VendorCustomerOpenBalances
{
    public class VendorOpenBalanceManager : CorarlERPDomainServiceBase, IVendorOpenBalanceManager
    {
        private readonly IRepository<VendorOpenBalance, Guid> _vendorOpenBalanceRepository;

        public VendorOpenBalanceManager(IRepository<VendorOpenBalance, Guid> vendorOpenBalanceRepository)
        {
            _vendorOpenBalanceRepository = vendorOpenBalanceRepository;
        }
        public async Task<IdentityResult> CreateAsync(VendorOpenBalance entity)
        {
            await _vendorOpenBalanceRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(VendorOpenBalance entity)
        {
            await _vendorOpenBalanceRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }

    public class CustomerOpenBalanceManager : CorarlERPDomainServiceBase, ICustomerOpenBalanceManager
    {
        private readonly IRepository<CustomerOpenBalance, Guid> _customerOpenBalanceRepository;

        public CustomerOpenBalanceManager(IRepository<CustomerOpenBalance, Guid> customerOpenBalanceRepository)
        {
            _customerOpenBalanceRepository = customerOpenBalanceRepository;
        }
        public async Task<IdentityResult> CreateAsync(CustomerOpenBalance entity)
        {
            await _customerOpenBalanceRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(CustomerOpenBalance entity)
        {
            await _customerOpenBalanceRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
