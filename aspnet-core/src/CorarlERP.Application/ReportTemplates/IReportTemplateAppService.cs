using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ReportTemplates.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorarlERP.ReportTemplates
{
    public interface IReportTemplateAppService : IApplicationService
    {
        Task<ReportTemplateOutput> Create(CreateTemplate input);
        
        Task<PagedResultDto<GetReportTemplateOutput>> GetList(GetReportTemplateInput input);

        Task<ReportTemplateOutput> Update(UpdateTemplate input);
        Task<ReportTemplateOutput> GetDetail(EntityDto<long> input);
        Task Delete(EntityDto<long> input);


    }
}
