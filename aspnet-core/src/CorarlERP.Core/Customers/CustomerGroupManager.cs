using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Customers
{
    public class CustomerGroupManager : CorarlERPDomainServiceBase, ICustomerGroupManager
    {
        private readonly IRepository<CustomerGroup, Guid> _CustomerGroupRepository;

        public CustomerGroupManager(IRepository<CustomerGroup, Guid> CustomerGroupRepository)
        {
            _CustomerGroupRepository = CustomerGroupRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(CustomerGroup entity)
        {
            await _CustomerGroupRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }
        
        public async virtual Task<CustomerGroup> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _CustomerGroupRepository.GetAll()
                .Include(u => u.UserGroup)
                :
                _CustomerGroupRepository.GetAll()
                .Include(u => u.UserGroup)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(CustomerGroup entity)
        {
            await _CustomerGroupRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(CustomerGroup entity)
        {
            await _CustomerGroupRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
