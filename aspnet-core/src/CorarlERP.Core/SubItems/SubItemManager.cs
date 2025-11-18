using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.SubItems
{
    public class SubItemManager : CorarlERPDomainServiceBase, ISubItemManager
    {
        private readonly IRepository<SubItem, Guid> _subItemRepository;
        public SubItemManager(IRepository<SubItem, Guid> subItemRepository)
        {
            _subItemRepository = subItemRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(SubItem sEntity)
        {
            await CheckDuplicateSubItem(sEntity);
            await _subItemRepository.InsertAsync(sEntity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateSubItem(SubItem @entity)
        {
            var @old = await _subItemRepository.GetAll().AsNoTracking()
                          .Where(u => u.Id == entity.Id &&
                                 u.ParentSubItemId == entity.ParentSubItemId &&
                                 u.ItemId == entity.ItemId)
                          .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateSubItem")); //this sub item is created already
            }
        }

        public async virtual Task<IdentityResult> DisableAsync(SubItem entity)
        {
            @entity.Enable(false);
            await _subItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(SubItem entity)
        {
            @entity.Enable(true);
            await _subItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<SubItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _subItemRepository.GetAll() : _subItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(SubItem entity)
        {
            await _subItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(SubItem entity)
        {
           
            await _subItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        
    }
}
