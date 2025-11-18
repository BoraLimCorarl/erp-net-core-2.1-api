using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.VendorCredit.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.VendorCredit
{

    public interface IVendorCreditAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateVendorCreditInput input);

        //Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<VendorCreditDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<NullableIdDto<Guid>> Update(UpdateVendorCreditInput input);

        Task<PagedResultDto<GetListVendorCreditOutput>> Find(ListVendorCreditInput input);

        Task<PagedResultDto<VendorCreditSummaryOutput>> GetVendorCreditHeader(GetVendorCreditInput input);
        Task<VendorCreditDetailOutput> GetVendorCreditItems(GetVendorCreditInputForItem input);

        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);

     //   Task<PagedResultDto<GetListVendorCreditForItemIssueVendorCredit>> GetVendorCreditForItemIssueVendorCredit(GetVendorCreditFromIssueCreditInput input);

    }

}
