using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Currencies.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Currencies
{
    public interface ICurrencyAppService : IApplicationService
    {
        Task<PagedResultDto<CurrencyDetailOutput>> GetList(GetCurrencyListInput input);
        Task<ListResultDto<CurrencyDetailOutput>> Find(GetCurrencyListInput input);
        Task Sync();
    }
}
