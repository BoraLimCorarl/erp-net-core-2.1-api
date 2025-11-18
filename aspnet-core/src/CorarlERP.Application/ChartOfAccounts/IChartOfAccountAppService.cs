using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ChartOfAccounts
{
    public interface IChartOfAccountAppService: IApplicationService
    {
        Task<ChartAccountDetailOutput> Create(CreateChartAccountInput input);
        Task<PagedResultDto<ChartAccountDetailOutput>> GetList(GetChartAccountListInput input);
        Task<PagedResultDto<ChartAccountDetailOutput>> Find(GetChartAccountListInput input);
        Task<ChartAccountDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<ChartAccountDetailOutput> Update(UpateChartAccountInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);

        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcel();
        Task<FileDto> ExportPDF();
        //FileDto ChartOfAccountExportFromExcel(List<ChartAccountDetailOutput> chartOfAccountListDtos);
    }
}
