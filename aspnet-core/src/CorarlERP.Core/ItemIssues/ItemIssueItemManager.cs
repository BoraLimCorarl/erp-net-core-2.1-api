using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ItemIssues
{
    public class ItemIssueItemManager : CorarlERPDomainServiceBase, IItemIssueItemManager
    {
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;

        public ItemIssueItemManager(IRepository<ItemIssueItem, Guid> itemIssueItemRepository)
        {
            _itemIssueItemRepository = itemIssueItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(ItemIssueItem entity)
        {
            await _itemIssueItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ItemIssueItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemIssueItemRepository.GetAll()
                :
                _itemIssueItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(ItemIssueItem entity)
        {
            await _itemIssueItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ItemIssueItem entity)
        {
            await _itemIssueItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
