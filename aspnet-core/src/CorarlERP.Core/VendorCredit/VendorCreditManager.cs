using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.VendorCredit
{
    public class VendorCreditManager : CorarlERPDomainServiceBase, IVendorCreditManager
    {
        private readonly IRepository<VendorCredit, Guid> _vendorCreditRepository;

        public VendorCreditManager(IRepository<VendorCredit, Guid> vendorCreditRepository)
        {
            _vendorCreditRepository = vendorCreditRepository;
        }

        public async Task<IdentityResult> CreateAsync(VendorCredit entity)
        {
            await _vendorCreditRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<VendorCredit> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _vendorCreditRepository.GetAll()               
                .Include(u => u.Vendor)
                :
                _vendorCreditRepository.GetAll()              
                .Include(u => u.Vendor)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(VendorCredit entity)
        {
            await _vendorCreditRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(VendorCredit entity)
        {
            await _vendorCreditRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
