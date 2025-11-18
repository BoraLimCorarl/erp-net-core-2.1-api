using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.ItemReceiptAdjustments.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptAdjustments
{
    public interface IItemReceiptAdjustmentAppService : IApplicationService
    {
        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
        Task<NullableIdDto<Guid>> Create(CreateItemReceiptAdjustmentInput input);
        Task<ItemReceiptAdjustmentDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemReiptAdjustmentInput input);

        Task ImportExcelHasDefaultAccount(FileDto input);
        Task<FileDto> ExportExcelTamplateHasDefaultAccount();
    }
}
