using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.VendorCredit
{
    public class VendorCreditDetailManager : CorarlERPDomainServiceBase, IVendorCreditDetailManager
    {
        private readonly IRepository<VendorCreditDetail, Guid> _vendorCreditDetailRepository;

        public VendorCreditDetailManager(IRepository<VendorCreditDetail, Guid> vendorCreditDetailRepository)
        {
            _vendorCreditDetailRepository = vendorCreditDetailRepository;
        }

        public async Task<IdentityResult> CreateAsync(VendorCreditDetail entity)
        {
            await _vendorCreditDetailRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<VendorCreditDetail> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _vendorCreditDetailRepository.GetAll() :
                _vendorCreditDetailRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(VendorCreditDetail entity)
        {
            await _vendorCreditDetailRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(VendorCreditDetail entity)
        {
            await _vendorCreditDetailRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }

}
