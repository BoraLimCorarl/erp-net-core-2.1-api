using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PurchasePrices
{
    public class PurchasePriceItemManager : CorarlERPDomainServiceBase, IPurchasePriceItemManager
    {
        private readonly IRepository<PurchasePriceItem, Guid> _purchasePriceItemRepository;

        public PurchasePriceItemManager(IRepository<PurchasePriceItem, Guid> purchasePriceItemRepository)
        {
            _purchasePriceItemRepository = purchasePriceItemRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(PurchasePriceItem entity)
        {
            await _purchasePriceItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<PurchasePriceItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _purchasePriceItemRepository.GetAll()
                .Include(u => u.Item)
                .Include(u => u.Vendor)
                .Include(u=>u.Currency) :
                _purchasePriceItemRepository.GetAll()
                .Include(u => u.Item)
                .Include(u => u.Vendor)
                .Include(u => u.Currency).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(PurchasePriceItem entity)
        {
            await _purchasePriceItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(PurchasePriceItem entity)
        {
            await _purchasePriceItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

    }
}
