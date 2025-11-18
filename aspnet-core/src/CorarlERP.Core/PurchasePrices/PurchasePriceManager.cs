using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PurchasePrices
{
    public class PurchasePriceManager : CorarlERPDomainServiceBase, IPurchasePriceManager
    {
        private readonly IRepository<PurchasePrice, Guid> _purchasePriceRepository;

        public PurchasePriceManager(IRepository<PurchasePrice, Guid> purchasePriceRepository)
        {
            _purchasePriceRepository = purchasePriceRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(PurchasePrice entity)
        {
            await _purchasePriceRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<PurchasePrice> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _purchasePriceRepository.GetAll()
                .Include(u => u.Location):
                _purchasePriceRepository.GetAll()
                .Include(u => u.Location).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(PurchasePrice entity)
        {
            await _purchasePriceRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> UpdateAsync(PurchasePrice entity)
        {
            await _purchasePriceRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
