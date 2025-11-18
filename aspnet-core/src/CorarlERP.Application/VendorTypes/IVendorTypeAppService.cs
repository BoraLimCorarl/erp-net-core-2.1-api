using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.VendorTypes.Dto;
using System.Threading.Tasks;

namespace CorarlERP.VendorTypes
{
    public interface IVendorTypeAppService : IApplicationService
    {
        Task<VendorTypeDetailOutput> Create(CreateVendorTypeInput input);
        Task<PagedResultDto<VendorTypeDetailOutput>> GetList(GetVendorTypeListInput input);
        Task<PagedResultDto<VendorTypeDetailOutput>> Find(GetVendorTypeListInput input);
        Task<VendorTypeDetailOutput> GetDetail(EntityDto<long> input);
        Task<VendorTypeDetailOutput> Update(UpdateVendorTypeInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
