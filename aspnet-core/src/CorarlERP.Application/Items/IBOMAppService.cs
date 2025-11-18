using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Items
{
   public  interface IBOMAppService : IApplicationService
    {
        Task<Guid> Create(CreateBomInput input);
        Task<PagedResultDto<GetBomOutput>> GetList(GetListBomInput input);
        Task<GetBomOutput> GetDetail(EntityDto<Guid> input);
        Task<Guid> Update(UpdateBomInput input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task Delete(EntityDto<Guid> input);
        Task<PagedResultDto<GetBomOutput>> Find(GetListBomInput input);
        Task<PagedResultDto<GetListItemForBOMOutput>> FindItemTypeItem(GetListItemForBOMInput input);
        Task<PagedResultDto<GetListItemForBOMOutput>> FindItemForBOM(GetListItemForBOMInput input);
        Task<List<GetBomItemOutput>> GetBomItemByBomId(BomItemInput input);
        Task<PagedResultDto<GetListItemFromBOMOutput>> FindItemItmeTypeMenu(GetListItemForBOMInput input);
        Task<FileDto> ExportExcel(GetListBomExcelInput input);
        Task ImportExcel(FileDto input);
    }
}
