using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Abp.UI;

namespace CorarlERP.Currencies

{
    public class CurrencyManager : CorarlERPDomainServiceBase, ICurrencyManager
    {
        private readonly IRepository<Currency, long> _currencyRepository;

        public CurrencyManager(IRepository<Currency, long> currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<IdentityResult> CreateAsync(Currency entity, bool checkDuplicate = true)
        {
           if (checkDuplicate)
                await CheckDuplicateCurrency(@entity);

            await _currencyRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Currency entity, bool checkDuplicate = true)
        {
            if (checkDuplicate) await CheckDuplicateCurrency(@entity);

            await _currencyRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Currency> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _currencyRepository.GetAll() : _currencyRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }
        private async Task CheckDuplicateCurrency(Currency @entity)
        {
            var @old = await _currencyRepository
                             .GetAll().AsNoTracking()
                             .Where(u => 
                                       u.Id != entity.Id &&
                                       u.Code.ToLower() == entity.Code.ToLower())
                              .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateCurrencyCode", entity.Code));
            }
        }
    }
}
