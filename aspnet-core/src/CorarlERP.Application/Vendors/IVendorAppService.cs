using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.Vendors
{
    public interface IVendorAppService: IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateVendorInput input);
        Task<PagedResultDto<VendorGetListOutPut>> GetList(GetVendorListInput input);
        Task<PagedResultDto<VendorGetListOutPut>> Find(GetVendorListInput input);
        Task<VendorDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateVendorInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcel();
        Task<ValidationCountOutput> CheckMaxVendorCount();
    }
}
