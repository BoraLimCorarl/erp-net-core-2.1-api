using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.VendorTypes
{
    public  class VendorTypeManager : CorarlERPDomainServiceBase, IVendorTypeManager
    {
        private readonly IRepository<VendorType, long> _vendorTypeRepository;

        public VendorTypeManager(IRepository<VendorType, long> vendorTypeRepository)
        {
            _vendorTypeRepository = vendorTypeRepository;
        }
        private async Task CheckDuplicateClass(VendorType @entity)
        {
            var @old = await _vendorTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.VendorTypeName.ToLower() == entity.VendorTypeName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateVendorTypeName", entity.VendorTypeName));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(VendorType entity)
        {
            await CheckDuplicateClass(entity);

            await _vendorTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(VendorType entity)
        {
            @entity.Enable(false);
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(VendorType entity)
        {
            @entity.Enable(true);
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<VendorType> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _vendorTypeRepository.GetAll() : _vendorTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(VendorType entity)
        {
            await _vendorTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(VendorType entity)
        {
            await CheckDuplicateClass(entity);

            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
