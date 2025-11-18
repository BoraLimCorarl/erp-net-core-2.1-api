using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueVendorCredits
{
    public class ItemIssueVendorCreditManager: CorarlERPDomainServiceBase, IItemIssueVendorCreditManager
    {
        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;

        public ItemIssueVendorCreditManager(IRepository<ItemIssueVendorCredit, Guid> itemReceiptRepository)
        {
            _itemIssueVendorCreditRepository = itemReceiptRepository;
        }
        public async Task<IdentityResult> CreateAsync(ItemIssueVendorCredit entity)
        {
            await _itemIssueVendorCreditRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemIssueVendorCredit> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemIssueVendorCreditRepository.GetAll()
                .Include(u => u.Vendor)  
                .Include(u=>u.VendorCredit)
                .Include(u=>u.ItemReceiptPurchase)
                :
                _itemIssueVendorCreditRepository.GetAll()
                .Include(u => u.Vendor)
                .Include(u => u.ItemReceiptPurchase)
                .Include(u => u.VendorCredit)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemIssueVendorCredit entity)
        {
            await _itemIssueVendorCreditRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemIssueVendorCredit entity)
        {
            await _itemIssueVendorCreditRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
