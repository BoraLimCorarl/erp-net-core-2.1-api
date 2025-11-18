using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Vendors
{
    public class VendorGroupManager : CorarlERPDomainServiceBase, IVendorGroupManager
    {
        private readonly IRepository<VendorGroup, Guid> _vendorGroupRepository;

        public VendorGroupManager(IRepository<VendorGroup, Guid> vendorGroupRepository)
        {
            _vendorGroupRepository = vendorGroupRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(VendorGroup entity)
        {
            await _vendorGroupRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }
        
        public async virtual Task<VendorGroup> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _vendorGroupRepository.GetAll()
                .Include(u => u.UserGroup)
                :
                _vendorGroupRepository.GetAll()
                .Include(u => u.UserGroup)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(VendorGroup entity)
        {
            await _vendorGroupRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(VendorGroup entity)
        {
            await _vendorGroupRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
