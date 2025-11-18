using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueVendorCredits
{
    public class ItemIssueVendorCreditItemManager : CorarlERPDomainServiceBase, IItemIssueVendorCreditItemManager
    {
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueItemRepository;

        public ItemIssueVendorCreditItemManager(IRepository<ItemIssueVendorCreditItem, Guid> itemIssueItemRepository)
        {
            _itemIssueItemRepository = itemIssueItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(ItemIssueVendorCreditItem entity)
        {
            await _itemIssueItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemIssueVendorCreditItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemIssueItemRepository.GetAll() : _itemIssueItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemIssueVendorCreditItem entity)
        {
            await _itemIssueItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemIssueVendorCreditItem entity)
        {
            await _itemIssueItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
