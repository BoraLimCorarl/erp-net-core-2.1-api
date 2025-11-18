using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using CorarlERP.Currencies.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Collections.Extensions;
using System.Linq;
using Abp.Linq.Extensions;
using AutoMapper;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using Abp.Authorization;
using System.Linq.Dynamic.Core;

namespace CorarlERP.Currencies
{
    [AbpAuthorize]
    public class CurrencyAppService : CorarlERPAppServiceBase, ICurrencyAppService
    {
        private readonly ICurrencyManager _currencyManager; 
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IDefaultValues _defaultValues;

        public CurrencyAppService(ICurrencyManager currencyManager,
                            IRepository<Currency, long> currencyRepository,
                            IDefaultValues defaultValues)
        {
            _currencyManager = currencyManager;
            _currencyRepository = currencyRepository;
            _defaultValues = defaultValues;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Currencies_Find)]
        public async Task<ListResultDto<CurrencyDetailOutput>> Find(GetCurrencyListInput input)
        {
            var @entities = await _currencyRepository
                 .GetAll()
                 .AsNoTracking()
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Code.ToLower().Contains(input.Filter.ToLower())||
                          p.Name.ToLower().Contains(input.Filter.ToLower())
                 )
                 .OrderBy(p => p.Code)
                 .ToListAsync();
            return new ListResultDto<CurrencyDetailOutput>(ObjectMapper.Map<List<CurrencyDetailOutput>>(@entities));
        }
        [AbpAuthorize(AppPermissions.Pages_Host_Currencies_GetList)]
        public async Task<PagedResultDto<CurrencyDetailOutput>> GetList(GetCurrencyListInput input)
        {
            var query = _currencyRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Code.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Name.ToLower().Contains(input.Filter.ToLower())
                );
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<CurrencyDetailOutput>(resultCount, ObjectMapper.Map<List<CurrencyDetailOutput>>(@entities));         
        }
        [AbpAuthorize(AppPermissions.Pages_Host_Client_Currencies_Sync)]
        public async Task Sync()
        {
            var defaultCurrecies = _defaultValues.Currencies;
            
            var defaultCurrencyCodes = defaultCurrecies.Select(u => u.Code.ToLower());

            if (defaultCurrencyCodes.Distinct().Count() != 
                defaultCurrencyCodes.Count())
            {
                throw new UserFriendlyException(L("DuplicateCurrencyCodes"));
            }


            var existingCurrencies = await _currencyRepository.GetAll().ToListAsync();
            var lookup = existingCurrencies.ToDictionary(u => u.Code);

            foreach (var c in defaultCurrecies)
            {
                if (lookup != null && lookup.ContainsKey(c.Code))
                {
                    //update
                    var @entity = lookup[c.Code];
                    entity.Update(null, c.Code, c.Symbol, c.Name, c.PluralName);
                    CheckErrors(await _currencyManager.UpdateAsync(@entity, false));
                }
                else 
                {
                    //create new
                    var @entity = Currency.Create(null, c.Code, c.Name, c.Symbol, c.PluralName);
                    CheckErrors(await _currencyManager.CreateAsync(@entity, false));
                }
            }

        }
       
    }
}
