using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceipts
{
    public interface IItemReceiptAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemReceiptInput input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<ItemReceiptDetailOutput>GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<PagedResultDto<GetListItemReceiptOut>> GetList(GetListItemReceiptInput input);

        Task<PagedResultDto<GetListItemReceiptOut>> Find(GetListItemReceiptInput input);

        Task<NullableIdDto<Guid>> Update(UpdateItemReceiptInput input);

        Task<PagedResultDto<ItemReceiptSummaryOutput>> GetItemReceipts(GetItemReceiptInput input);

        Task<ItemReceiptSummaryOutputForItemReceiptItem> GetItemReceiptItems(EntityDto<Guid>input);

        Task<PagedResultDto<ItemReceiptSummaryOutput>> GetItemReceiptForVendorCredits(GetItemReceiptInput input, Guid? exceptId);

        Task<ItemReceiptSummaryOutputForItemReceiptItem> GetItemReceiptItemVendorCredits(EntityDto<Guid> input, Guid? exceptId);

        Task<PagedResultDto<ItemReceiptitemFromVendorCreditOutput>> GetNewItemReceiptForVendorCredits(GetNewItemReceiptInput input, Guid? exceptId);
        //Task<PagedResultDto<GetListInventoryReportOutput>> GetInventoryReport(GetInventoryReportInput input);
    }
}
