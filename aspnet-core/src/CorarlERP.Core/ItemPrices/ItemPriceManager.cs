using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ItemPrices
{
    public class ItemPriceManager : CorarlERPDomainServiceBase, IItemPriceManager
    {
        private readonly IRepository<ItemPrice, Guid> _itemPriceRepository;

        public ItemPriceManager(IRepository<ItemPrice, Guid> itemPriceRepository)
        {
            _itemPriceRepository = itemPriceRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(ItemPrice entity)
        {
            await _itemPriceRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemPrice> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemPriceRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.TransactionTypeSale) :
                _itemPriceRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.TransactionTypeSale).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemPrice entity)
        {
            await _itemPriceRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> UpdateAsync(ItemPrice entity)
        {
            await _itemPriceRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
