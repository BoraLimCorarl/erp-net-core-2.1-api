using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.PhysicalCounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PhysicalCounts
{
    public interface IPhysicalCountAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreatePhysicalCountInput input);
        Task<PagedResultDto<PhysicalCountGetListOutput>> GetList(GetPhysicalCountListInput input);
        Task<PhysicalCountDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<PhysicalCountDetailOutput> Update(UpdatePhysicalCountInput input);
        Task<List<PhysicalItemDetailDto>> CalculateStockBalance(CalculatePhysicalCountInput input);
        Task<PhysicalCountDetailOutput> UpdateAndClose(UpdatePhysicalCountInput input);
        Task<FileDto> Print(EntityDto<Guid> input);
        Task<FileDto> ExportPDF(EntityDto<Guid> input);
        Task<FileDto> ExportExcel(EntityDto<Guid> input);
        Task<List<CreateOrUpdatePhysicalItemInput>> ImportExcel(FileDto input);
        Task ChangeSetting(PhysicalCountSettingDto input);
        Task<PhysicalCountSettingDto> GetSetting();

        Task Delete(CarlEntityDto input);
        Task Open(CarlEntityDto input);
        Task Close(CarlEntityDto input);

        Task<ItemIssuePhysicalCountDetailOutput> GetItemIssue(EntityDto<Guid> input);
        Task<ItemReceiptPhysicalCountDetailOutput> GetItemReceipt(EntityDto<Guid> input);
    }
}
