using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.InvoiceTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InvoiceTemplates
{
    public interface IInvoiceTemplateAppService : IApplicationService
    {
        Task<InvoiceTemplateDto> Create(InvoiceTemplateDto input);
        Task<PagedResultDto<InvoiceTemplateDto>> GetList(GetInvoiceTemplateListInput input);
        Task<PagedResultDto<InvoiceTemplateDto>> Find(GetInvoiceTemplateListInput input);
        Task<InvoiceTemplateDto> GetDetail(EntityDto<Guid> input);
        Task<InvoiceTemplateDto> Update(InvoiceTemplateDto input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);

        Task<ListResultDto<DefaultTemplateOutput>> GetDefaultTemplateList(DefaultTemplateInput input);
        Task<ListResultDto<InvoiceTemplateMapDto>> GetTemplateMapList(TemplateMapListInput input);
        Task<InvoiceTemplateMapDto> MapTemplate(InvoiceTemplateMapDto input);
        Task UnMapTemplate(EntityDto<Guid> input);

        Task<InvoiceTemplateResultOutput> GetTemplateHtml(InvoiceTemplateInput input);
        Task<InvoiceTemplateResultOutput> GetDetailTemplateHtml(InvoiceTemplateInput input);
        Task<InvoiceTemplateResultOutput> GetPaymentSummaryTemplateHtml(InvoiceTemplateInput input);

        Task<ListResultDto<VarriableDto>> GetTemplateVarriables();
    }
}
