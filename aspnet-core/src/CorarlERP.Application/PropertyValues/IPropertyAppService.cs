using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.PropertyValues.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PropertyValues
{
    public interface IPropertyAppService: IApplicationService
    {
        Task Create(CreatePropertyInput input);
        Task<ListResultDto<PropertyDetailOutput>> GetList(GetPropertyListInput input);
        Task<ListResultDto<PropertyOutput>> Find(GetPropertyListInput input);
        Task<PropertyDetailOutput> Update(UpdatePropertyInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task<FileDto> ExportExcel();
        Task<PagedResultDto<PropertyValueDetailOutput>> FindPOSFilterCategories(GetPOSCategoryListInput input);
        Task ImportUpdateProperty(FileDto input);
    }
}
