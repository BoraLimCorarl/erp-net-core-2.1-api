using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Vendors
{
    public class VendorManager : CorarlERPDomainServiceBase, IVendorManager
    {
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        public VendorManager(IRepository<Vendor, Guid> vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }
        private async Task CheckDuplicateVendor(Vendor @entity)
        {
            var @old = await _vendorRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.VendorCode.ToLower() == entity.VendorCode.ToLower() &&
                                       u.Id !=entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.VendorCode.ToLower() == entity.VendorCode.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateVendorCode", entity.VendorCode));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(Vendor entity)
        {
            await CheckDuplicateVendor(entity);

            await _vendorRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Vendor entity)
        {
            @entity.Enable(false);
            await _vendorRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Vendor entity)
        {
            @entity.Enable(true);
            await _vendorRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Vendor> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _vendorRepository.GetAll()
                .Include (u=>u.ChartOfAccount)
                .Include("VendorType")
                :
                _vendorRepository.GetAll()
                .Include(u => u.ChartOfAccount)
                .Include("VendorType")
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Vendor entity)
        {
            await _vendorRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Vendor entity)
        {
            await CheckDuplicateVendor(entity);
            await _vendorRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
