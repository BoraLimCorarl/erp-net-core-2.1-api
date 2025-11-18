using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.ItemReceiptOthers.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptOthers
{
   public interface IItemReceiptOtherAppService : IApplicationService
    {
        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
        Task<NullableIdDto<Guid>> Create(CreateItemReceiptOtherInput input);
        Task<ItemReceiptOtherDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemReiptOtherInput input);
        //Task Delete(EntityDto<Guid> input);
        //Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        //Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        //Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<FileDto> ExportExcelTamplateHasAccountDefault();
        Task ImportExcelHasAccountDefault(FileDto input);
       
    }
}
