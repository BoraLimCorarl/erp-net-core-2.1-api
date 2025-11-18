using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using CorarlERP.Authorization;
using CorarlERP.MultiCurrencies;
using CorarlERP.MultiCurrencys.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.MultiCurrencys
{
    [AbpAuthorize]
    public class MultiCurrencyAppService : CorarlERPAppServiceBase, IMultiCurrencyAppService
    {
        private readonly IMultiCurrencyManager _currencyManager;
        private readonly IRepository<MultiCurrencies.MultiCurrency, long> _currencyRepository;
        private readonly IDefaultValues _defaultValues;

        public MultiCurrencyAppService(IMultiCurrencyManager currencyManager,
                            IRepository<MultiCurrencies.MultiCurrency, long> currencyRepository,
                            IDefaultValues defaultValues)
        {
            _currencyManager = currencyManager;
            _currencyRepository = currencyRepository;
            _defaultValues = defaultValues;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_MultiCurrencies_Find)]
        public async Task<ListResultDto<MultiCurrencyDetailOutput>> Find(GetMultiCurrencyListInput input)
        {
            var @entities = await _currencyRepository
                 .GetAll()
                 .Include(u => u.Currency)
                 .Select(t => new MultiCurrencyDetailOutput {
                     Code = t.Currency.Code,
                     Id = t.Id,
                     Name = t.Currency.Name,
                     Symbol = t.Currency.Symbol,
                     PluralName = t.Currency.PluralName,
                     CurrencyId = t.CurrencyId,
                 })
                 .AsNoTracking()               
                 .OrderBy(p => p.Code)
                 .ToListAsync();
            return new ListResultDto<MultiCurrencyDetailOutput>(ObjectMapper.Map<List<MultiCurrencyDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_MultiCurrencies_GetList)]
        public async Task<PagedResultDto<MultiCurrencyDetailOutput>> GetList(GetMultiCurrencyListInput input)
        {
            var query = _currencyRepository
                 .GetAll()
                 .Include(u => u.Currency)
                 .Select(t => new MultiCurrencyDetailOutput
                 {
                     Code = t.Currency.Code,
                     Id = t.Id,
                     Name = t.Currency.Name,
                     Symbol = t.Currency.Symbol,
                     PluralName = t.Currency.PluralName
                 })
                 .AsNoTracking();
            var resultCount = await query.CountAsync();
            var @entities = await query.PageBy(input).ToListAsync();
            return new PagedResultDto<MultiCurrencyDetailOutput>(resultCount, ObjectMapper.Map<List<MultiCurrencyDetailOutput>>(@entities));
        }
    }
}
