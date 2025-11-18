using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.CustomerTypes
{
    public  class CustomerTypeManager : CorarlERPDomainServiceBase, ICustomerTypeManager
    {
        private readonly IRepository<CustomerType, long> _customerRepository;

        public CustomerTypeManager(IRepository<CustomerType, long> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        private async Task CheckDuplicateClass(CustomerType @entity)
        {
            var @old = await _customerRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.CustomerTypeName.ToLower() == entity.CustomerTypeName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateCustomerName", entity.CustomerTypeName));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(CustomerType entity)
        {
            await CheckDuplicateClass(entity);

            await _customerRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(CustomerType entity)
        {
            @entity.Enable(false);
            await _customerRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(CustomerType entity)
        {
            @entity.Enable(true);
            await _customerRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<CustomerType> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _customerRepository.GetAll() : _customerRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(CustomerType entity)
        {
            await _customerRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(CustomerType entity)
        {
            await CheckDuplicateClass(entity);

            await _customerRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
