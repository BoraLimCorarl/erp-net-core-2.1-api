using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.CashFlowTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CashFlowTemplates
{
    public interface ICashFlowTemplateAppService : IApplicationService
    {
        Task<CreateOrUpdateCashFlowTemplateDto> Create(CreateOrUpdateCashFlowTemplateDto input);
        Task<PagedResultDto<CashFlowTemplateListDto>> GetList(GetListCashFlowTemplateInput input);
        Task<PagedResultDto<CashFlowTemplateSummaryDto>> Find(GetListCashFlowTemplateInput input);
        Task<CashFlowTemplateSummaryDto> GetDefaultTemplate();
        Task<CashFlowTemplateDetailDto> GetDetail(EntityDto<Guid> input);
        Task<CreateOrUpdateCashFlowTemplateDto> Update(CreateOrUpdateCashFlowTemplateDto input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task SyncDefaultTemplate();
        Task<GetCashFlowCategoryWithAccountOutput> GetAccountWithDefaultCategory();
        Task<List<CashFlowTemplateAccountDetailDto>> SyncAccounts(EntityDto<Guid?> input);
    }
}
