using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    public class ItemReceiptItemCustomerCreditManager : CorarlERPDomainServiceBase, IItemReceiptItemCustomerCreditManager
    {
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptItemRepository;

        public ItemReceiptItemCustomerCreditManager(IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptItemRepository)
        {
            _itemReceiptItemRepository = itemReceiptItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(ItemReceiptItemCustomerCredit entity)
        {
            await _itemReceiptItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemReceiptItemCustomerCredit> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemReceiptItemRepository.GetAll() : _itemReceiptItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemReceiptItemCustomerCredit entity)
        {
            await _itemReceiptItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemReceiptItemCustomerCredit entity)
        {
            await _itemReceiptItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
