using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PhysicalCount
{
    public class PhysicalCountItemManager : CorarlERPDomainServiceBase, IPhysicalCountItemManager
    {
        private readonly IRepository<PhysicalCountItem, Guid> _PhysicalCountItemRepository;

        public PhysicalCountItemManager(IRepository<PhysicalCountItem, Guid> ransferOrderItemRepository)
        {
            _PhysicalCountItemRepository = ransferOrderItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(PhysicalCountItem entity)
        {
            await _PhysicalCountItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PhysicalCountItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _PhysicalCountItemRepository.GetAll() :
                _PhysicalCountItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PhysicalCountItem entity)
        {
            await _PhysicalCountItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PhysicalCountItem entity)
        {
            await _PhysicalCountItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
