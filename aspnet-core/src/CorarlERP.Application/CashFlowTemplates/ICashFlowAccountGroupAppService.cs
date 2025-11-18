using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.CashFlowTemplates.Dto;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CashFlowTemplates
{
    public interface ICashFlowAccountGroupAppService : IApplicationService
    {
        Task Create(CashFlowAccountGroupDto input);
        Task<PagedResultDto<CashFlowAccountGroupDto>> GetList(GetListCashFlowAccountGroupInput input);
        Task<PagedResultDto<CashFlowAccountGroupDto>> Find(GetListCashFlowAccountGroupInput input);
        Task<CashFlowAccountGroupDto> GetDetail(EntityDto<Guid> input);
        Task Update(CashFlowAccountGroupDto input);
        Task Delete(EntityDto<Guid> input);
    }
}
