using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.PackageEditions.Dot;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PackageEditions
{
    public interface IFeatureAppService : IApplicationService
    {
        Task Create(FeatureDto input);
        Task<FeatureDto> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<GetFeatureListDto>> GetList(GetFeatureListInput input);
        Task<ListResultDto<GetFeatureListDto>> Find(FindFeatureInput input);
        Task<ListResultDto<GetFeatureListDto>> GetDefaultFeatures();
        Task Update(FeatureDto input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcel();
    }
}
