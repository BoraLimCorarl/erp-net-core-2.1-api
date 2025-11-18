using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    public class ItemReceiptCustomerCreditManager : CorarlERPDomainServiceBase, IItemReceiptCustomerCreditManager
    {
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;

        public ItemReceiptCustomerCreditManager(IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptRepository)
        {
            _itemReceiptCustomerCreditRepository = itemReceiptRepository;
        }
        public async Task<IdentityResult> CreateAsync(ItemReceiptCustomerCredit entity)
        {
            await _itemReceiptCustomerCreditRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemReceiptCustomerCredit> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemReceiptCustomerCreditRepository.GetAll()
                .Include(u => u.Customer)             
                :
                _itemReceiptCustomerCreditRepository.GetAll()
                .Include(u => u.Customer)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemReceiptCustomerCredit entity)
        {
            await _itemReceiptCustomerCreditRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemReceiptCustomerCredit entity)
        {
            await _itemReceiptCustomerCreditRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
