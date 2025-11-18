using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Exchanges.Dto;

namespace CorarlERP.Exchanges
{
    public interface IExChangeAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateExchangeInput input);
        Task<PagedResultDto<GetListExchangeOutput>> GetList(GetListExchangeInput input);
        Task<ListResultDto<GetExchangeRateDto>> Find(FindExchangeInput input);
        Task<GetExchangeRateDto> GetExchangeRate(GetExchangeInput input);
        Task<GetDetailExchangeOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateExchangeInput input);
        Task Delete(EntityDto<Guid> input);
        Task<CreateOrUpdateExchangeInput> GetExChangeCurrency(GetExchangeInput input);
        Task ApplyRate(EntityDto<Guid> input);
    }
}
