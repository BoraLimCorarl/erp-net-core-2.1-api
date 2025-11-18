using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PurchaseOrders
{
    public class PurchaseOrderItemManager : CorarlERPDomainServiceBase, IPurchaseOrderItemManager
    {
        private readonly IRepository<PurchaseOrderItem, Guid> _purchaseOrderItemRepository;
        public PurchaseOrderItemManager(IRepository<PurchaseOrderItem, Guid> purchaseOrderItemRepository)
        {
            _purchaseOrderItemRepository = purchaseOrderItemRepository;
        }
        public async Task<IdentityResult> CreateAsync(PurchaseOrderItem pEntity)
        {
            //await CheckDuplicatePurchaseOrderItem(pEntity);
            await _purchaseOrderItemRepository.InsertAsync(pEntity);
            return IdentityResult.Success;
        }

        public async Task<PurchaseOrderItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _purchaseOrderItemRepository.GetAll() : _purchaseOrderItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PurchaseOrderItem entity)
        {
            await _purchaseOrderItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PurchaseOrderItem entity)
        {
            await _purchaseOrderItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
        private async Task CheckDuplicatePurchaseOrderItem(PurchaseOrderItem @entity)
        {
            var @old = await _purchaseOrderItemRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicatePurchaseOrderItem")); 
            }
        }
    }
}
