using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Customers
{
   public interface ICustomerAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateCustomerInput input);
        Task<PagedResultDto<CustomerGetListOutput>> GetList(GetCustomerListInput input);
        Task<PagedResultDto<GetListFindOutput>> Find(GetCustomerListInput input);
        Task<CustomerDetailOutput> FindDefault(long locationId);
        Task<CustomerDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateCustomerInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcel();
        Task<PagedResultDto<CustomerGetListOutput>> GetListPOS(GetCustomerListInput input);
        Task<bool> CheckExist(CheckExistCustomerInput input);
        Task<ValidationCountOutput> CheckMaxCustomerCount();
    }
}
