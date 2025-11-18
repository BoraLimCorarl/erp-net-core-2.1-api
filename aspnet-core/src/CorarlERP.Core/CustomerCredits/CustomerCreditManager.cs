using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.CustomerCredits
{
    public class CustomerCreditManager : CorarlERPDomainServiceBase, ICustomerCreditManager
    {
        private readonly IRepository<CustomerCredit, Guid> _customerCreditRepository;

        public CustomerCreditManager(IRepository<CustomerCredit, Guid> customerCreditRepository)
        {
            _customerCreditRepository = customerCreditRepository;
        }
        public async Task<IdentityResult> CreateAsync(CustomerCredit entity)
        {
            await _customerCreditRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<CustomerCredit> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _customerCreditRepository.GetAll()
               //.Include(u => u.Location)
               .Include(u => u.Customer)
               :
               _customerCreditRepository.GetAll()
               //.Include(u => u.Location)
               .Include(u => u.Customer)
               .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(CustomerCredit entity)
        {
            await _customerCreditRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(CustomerCredit entity)
        {
            await _customerCreditRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
