using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.MultiCurrencys.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.MultiCurrencys
{
   public interface IMultiCurrencyAppService : IApplicationService
    {
        Task<PagedResultDto<MultiCurrencyDetailOutput>> GetList(GetMultiCurrencyListInput input);
        Task<ListResultDto<MultiCurrencyDetailOutput>> Find(GetMultiCurrencyListInput input);
    }
}
