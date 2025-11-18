using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
  public  class RawMaterialItemManager : CorarlERPDomainServiceBase, IRawMaterialItemManager
    {
        private readonly IRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;

        public RawMaterialItemManager(IRepository<RawMaterialItems, Guid> rawMaterialItemsItemRepository)
        {
            _rawMaterialItemsRepository = rawMaterialItemsItemRepository;
        }
        public async Task<IdentityResult> CreateAsync(RawMaterialItems entity)
        {
            await _rawMaterialItemsRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<RawMaterialItems> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _rawMaterialItemsRepository.GetAll() :
                _rawMaterialItemsRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(RawMaterialItems entity)
        {
            await _rawMaterialItemsRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(RawMaterialItems entity)
        {
            await _rawMaterialItemsRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
