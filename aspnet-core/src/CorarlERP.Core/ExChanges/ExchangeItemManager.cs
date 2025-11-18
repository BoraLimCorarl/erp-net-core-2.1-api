using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ExChanges
{
    public class ExchangeItemManager : CorarlERPDomainServiceBase, IExchangeItemManager
    {
        private readonly IRepository<ExchangeItem, Guid> _exchangeItemRepository;

        public ExchangeItemManager(IRepository<ExchangeItem, Guid> exchangeItemRepository)
        {
            _exchangeItemRepository = exchangeItemRepository;
        }
        public async virtual Task<IdentityResult> CreateAsync(ExchangeItem entity)
        {
            await _exchangeItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ExchangeItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _exchangeItemRepository.GetAll()
                .Include(u=>u.FromCurrency)
                .Include(u=>u.ToCurrency)
              : _exchangeItemRepository.GetAll()
                .Include(u => u.FromCurrency)
                .Include(u => u.ToCurrency).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ExchangeItem entity)
        {
            await _exchangeItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> UpdateAsync(ExchangeItem entity)
        {
            await _exchangeItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
