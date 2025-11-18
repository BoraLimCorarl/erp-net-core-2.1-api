using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.CustomerCredits
{
    public class CustomerCreditDetailManagers : CorarlERPDomainServiceBase, ICustomerCreditDetailManager
    {
        private readonly IRepository<CustomerCreditDetail, Guid> _customerCreditDetailRepository;

        public CustomerCreditDetailManagers(IRepository<CustomerCreditDetail, Guid> customerCreditDetailRepository)
        {
            _customerCreditDetailRepository = customerCreditDetailRepository;
        }

        public async Task<IdentityResult> CreateAsync(CustomerCreditDetail entity)
        {
            await _customerCreditDetailRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<CustomerCreditDetail> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _customerCreditDetailRepository.GetAll() :
                 _customerCreditDetailRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(CustomerCreditDetail entity)
        {
            await _customerCreditDetailRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(CustomerCreditDetail entity)
        {
            await _customerCreditDetailRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
