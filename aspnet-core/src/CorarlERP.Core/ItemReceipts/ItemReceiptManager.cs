using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceipts
{
    public class ItemReceiptManager : CorarlERPDomainServiceBase, IItemReceiptManager
    {
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;

        public ItemReceiptManager(IRepository<ItemReceipt, Guid> itemReceiptRepository)
        {
            _itemReceiptRepository = itemReceiptRepository;
        }
        public async Task<IdentityResult> CreateAsync(ItemReceipt entity)
        {
            await _itemReceiptRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemReceipt> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemReceiptRepository.GetAll()
                .Include(u => u.Vendor)
               
                :
                _itemReceiptRepository.GetAll()
                .Include(u => u.Vendor)               
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemReceipt entity)
        {
            await _itemReceiptRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemReceipt entity)
        {
            await _itemReceiptRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
