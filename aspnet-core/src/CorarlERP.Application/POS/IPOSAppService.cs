using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.POS.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.POS
{
    public interface IPOSAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreatePOSInput input);
        Task<FileDto> CreateAndPrint(CreatePOSInput input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<POSDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<POSGetListOutput>> GetList(GetListPOSInput input);
        Task<PagedResultDto<POSGetListOutput>> GetUnpaidInvoice(GetUnpaidPOSInput input);
        Task<string> PayMore(POSPaymoreInput input);
        Task<FileDto> Print(EntityDto<Guid> input);

        Task<string> CreateAndGetHTMLPrintContent(CreatePOSInput input);
        Task<string> GetHTMLPrintContent(EntityDto<Guid> input);

    }
}
