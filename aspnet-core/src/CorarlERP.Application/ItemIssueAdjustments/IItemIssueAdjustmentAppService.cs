using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.ItemIssueAdjustments.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueAdjustments
{
   public interface IItemIssueAdjustmentAppService : IApplicationService
    {
        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
        Task<NullableIdDto<Guid>> Create(CreateItemIssueAdjustmentInput input);
        Task<ItemIssueAdjustmentDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemIssueAdjustmentInput input);
        //Task Delete(EntityDto<Guid> input);
        Task<FileDto> ExportExcelTamplateHasDefaultAccount();
        Task ImportExcelHasDefaultAccount(FileDto input);
    }
}
