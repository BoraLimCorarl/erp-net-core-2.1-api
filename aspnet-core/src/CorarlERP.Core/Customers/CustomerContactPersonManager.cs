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
    public class CustomerContactPersonManager : CorarlERPDomainServiceBase, ICustomerContactPersonManager
    {
        private readonly IRepository<CustomerContactPerson, Guid> _customercontactPersonRepository;
        public CustomerContactPersonManager(IRepository<CustomerContactPerson, Guid> customerContactPersonRepository)
        {
            _customercontactPersonRepository = customerContactPersonRepository;
        }
        private async Task CheckDuplicateContactPerson(CustomerContactPerson @entity)
        {
            var @old = await _customercontactPersonRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateContactPerson")); //this contact person is created already
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(CustomerContactPerson cEntity)
        {
            await CheckDuplicateContactPerson(cEntity);
            await _customercontactPersonRepository.InsertAsync(cEntity);
            return IdentityResult.Success;
        }

        public async virtual Task<CustomerContactPerson> GetAsync(Guid id, bool tracking = true)
        {
            var @query = tracking ? _customercontactPersonRepository.GetAll() : _customercontactPersonRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(CustomerContactPerson entity)
        {
            await _customercontactPersonRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(CustomerContactPerson entity)
        {
            await _customercontactPersonRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
