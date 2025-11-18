using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.CashFlowTemplates.Dto;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CashFlowTemplates
{
    public interface ICashFlowCategoryAppService : IApplicationService
    {
        Task Create(CashFlowCategoryDto input);
        Task<PagedResultDto<CashFlowCategoryDto>> GetList(GetListCashFlowCategoryInput input);
        Task<PagedResultDto<CashFlowCategoryDto>> Find(GetListCashFlowCategoryInput input);
        Task<CashFlowCategoryDto> GetDetail(EntityDto<Guid> input);
        Task Update(CashFlowCategoryDto input);
        Task Delete(EntityDto<Guid> input);
    }
}
