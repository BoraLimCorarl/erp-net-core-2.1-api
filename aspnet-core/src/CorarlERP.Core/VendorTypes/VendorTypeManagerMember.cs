using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.VendorTypes
{
    public  class VendorTypeMemberManager : CorarlERPDomainServiceBase, IVendorTypeMemberManager
    {
        private readonly IRepository<VendorTypeMember, long> _vendorTypeRepository;

        public VendorTypeMemberManager(IRepository<VendorTypeMember, long> vendorTypeRepository)
        {
            _vendorTypeRepository = vendorTypeRepository;
        }
       
        public async virtual Task<IdentityResult> CreateAsync(VendorTypeMember entity)
        {
            await _vendorTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(VendorTypeMember entity)
        {
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(VendorTypeMember entity)
        {
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<VendorTypeMember> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _vendorTypeRepository.GetAll() : _vendorTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(VendorTypeMember entity)
        {
            await _vendorTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(VendorTypeMember entity)
        {
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
