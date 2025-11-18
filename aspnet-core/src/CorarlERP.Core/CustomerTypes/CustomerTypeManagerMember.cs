using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.CustomerTypes
{
    public  class CustomerTypeMemberManager : CorarlERPDomainServiceBase, ICustomerTypeMemberManager
    {
        private readonly IRepository<CustomerTypeMember, long> _customerTypeRepository;

        public CustomerTypeMemberManager(IRepository<CustomerTypeMember, long> customerTypeRepository)
        {
            _customerTypeRepository = customerTypeRepository;
        }
       
        public async virtual Task<IdentityResult> CreateAsync(CustomerTypeMember entity)
        {
            await _customerTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(CustomerTypeMember entity)
        {
            await _customerTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(CustomerTypeMember entity)
        {
            await _customerTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<CustomerTypeMember> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _customerTypeRepository.GetAll() : _customerTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(CustomerTypeMember entity)
        {
            await _customerTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(CustomerTypeMember entity)
        {
            await _customerTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
