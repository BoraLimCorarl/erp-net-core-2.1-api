using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.PackageEditions.Dot;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PackageEditions
{
    public interface IPackageAppService : IApplicationService
    {
        Task Create(PackageDto input);
        Task<PackageDto> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<GetPackageListDto>> GetList(GetPackageListInput input);
        Task<ListResultDto<GetPackageListDto>> Find(GetPackageListInput input);
        Task Update(PackageDto input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<ListResultDto<PackageEditionDto>> GetPackageEditions(EntityDto<Guid> input);
    }
}
