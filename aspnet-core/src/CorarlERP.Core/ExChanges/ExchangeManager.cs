using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ExChanges
{
    public class ExchangeManager : CorarlERPDomainServiceBase, IExchangeManager
    {
        private readonly IRepository<Exchange, Guid> _exchangeRepository;

        public ExchangeManager(IRepository<Exchange, Guid> exchangeRepository)
        {
            _exchangeRepository = exchangeRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(Exchange entity)
        {
            await _exchangeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Exchange> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _exchangeRepository.GetAll()
              : _exchangeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Exchange entity)
        {
            await _exchangeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> UpdateAsync(Exchange entity)
        {
            await _exchangeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
