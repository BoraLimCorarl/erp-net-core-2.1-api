using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Customers
{
    public class CustomerManager : CorarlERPDomainServiceBase, ICustomerManager
    {
        private readonly IRepository<Customer, Guid> _customerRepository;

        public CustomerManager(IRepository<Customer, Guid> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        private async Task CheckDuplicateVendor(Customer @entity)
        {
            var @old = await _customerRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.CustomerCode.ToLower() == entity.CustomerCode.ToLower() &&
                                       u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.CustomerCode.ToLower() == entity.CustomerCode.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateCustomerCode", entity.CustomerCode));
            }
        }
        public async Task<IdentityResult> CreateAsync(Customer entity)
        {
            await CheckDuplicateVendor(entity);

            await _customerRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(Customer entity)
        {
            @entity.Enable(false);
            await _customerRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(Customer entity)
        {
            @entity.Enable(true);
            await _customerRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Customer> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _customerRepository.GetAll()
               .Include(u => u.Account)
               .Include (u=>u.CustomerType)
               :
               _customerRepository.GetAll()
               .Include(u => u.Account)
               .Include(u => u.CustomerType)
               .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(Customer entity)
        {
            await _customerRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Customer entity)
        {
            await CheckDuplicateVendor(entity);
            await _customerRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
