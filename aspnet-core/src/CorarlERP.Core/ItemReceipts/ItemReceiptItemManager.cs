using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceipts
{
   public class ItemReceiptItemManager : CorarlERPDomainServiceBase, IItemReceiptItemManager
    {
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        public ItemReceiptItemManager(IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository)
        {
            _itemReceiptItemRepository = itemReceiptItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(ItemReceiptItem entity)
        {
            await _itemReceiptItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemReceiptItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemReceiptItemRepository.GetAll():_itemReceiptItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemReceiptItem entity)
        {
            await _itemReceiptItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemReceiptItem entity)
        {
            await _itemReceiptItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
