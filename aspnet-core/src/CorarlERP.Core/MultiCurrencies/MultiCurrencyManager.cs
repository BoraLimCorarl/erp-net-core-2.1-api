using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.MultiCurrencies
{
   public class MultiCurrencyManager : CorarlERPDomainServiceBase, IMultiCurrencyManager
    {
        private readonly IRepository<MultiCurrency, long> _multiCurrencyRepository;

        public MultiCurrencyManager(IRepository<MultiCurrency, long> multiCurrencyRepository)
        {
            _multiCurrencyRepository = multiCurrencyRepository;
        }

        public async Task<IdentityResult> CreateAsync(MultiCurrency entity, bool checkDuplicate = true)
        {
            
            await _multiCurrencyRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(MultiCurrency entity, bool checkDuplicate = true)
        {
           
            await _multiCurrencyRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<MultiCurrency> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _multiCurrencyRepository.GetAll() : _multiCurrencyRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

      
        public async  Task<IdentityResult> RemoveAsync(MultiCurrency entity)
        {
            await _multiCurrencyRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
