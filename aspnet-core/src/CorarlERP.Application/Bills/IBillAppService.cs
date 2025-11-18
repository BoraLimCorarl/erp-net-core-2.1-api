using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Bills.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Bills
{
   public interface IBillAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateBillInput input);

       // Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<BillDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<PagedResultDto<BillHeader>> GetList(GetListBillInput input);
        Task<PagedResultDto<getBillListOutput>> GetListBillForPayBill(GetListBillForPaybillInput input);

        Task<PagedResultDto<GetListBillOutput>> Find(GetListBillInput input);

        Task<NullableIdDto<Guid>> Update(UpdateBillInput input);

        Task<PagedResultDto<BillSummaryOutput>> GetBills(GetBillListInput input);

        Task<BillSummaryOutputForGetBillItem> GetBillItems(EntityDto<Guid> input);

        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);

        Task CleanRoundingPaidStatus(CarlEntityDto input);
        Task<FileDto> ExportExcelItemsTamplate();
        Task ImportExcelItems(FileDto input);

        Task MultiDelete(GetListDeleteInput input);
    }
}
