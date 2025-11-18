using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ItemPrices
{
    public class ItemPriceItemManager : CorarlERPDomainServiceBase, IItemPriceItemManager
    {
        private readonly IRepository<ItemPriceItem, Guid> _itemPriceItemRepository;

        public ItemPriceItemManager(IRepository<ItemPriceItem, Guid> itemPriceItemRepository)
        {
            _itemPriceItemRepository = itemPriceItemRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(ItemPriceItem entity)
        {
            await _itemPriceItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemPriceItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemPriceItemRepository.GetAll()
                .Include(u => u.Item)
                .Include(u=>u.Currency) :
                _itemPriceItemRepository.GetAll()
                .Include(u => u.Item)
                .Include(u => u.Currency).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemPriceItem entity)
        {
            await _itemPriceItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemPriceItem entity)
        {
            await _itemPriceItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

    }
}
