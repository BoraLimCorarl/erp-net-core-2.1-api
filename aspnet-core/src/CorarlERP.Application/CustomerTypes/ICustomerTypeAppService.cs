using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.CustomerTypes.Dto;
using System.Threading.Tasks;

namespace CorarlERP.CustomerTypes
{
    public interface ICustomerTypeAppService : IApplicationService
    {
        Task<CustomerTypeDetailOutput> Create(CreateCustomertTypeInput input);
        Task<PagedResultDto<CustomerTypeDetailOutput>> GetList(GetCustomerTypeListInput input);
        Task<PagedResultDto<CustomerTypeDetailOutput>> Find(GetCustomerTypeListInput input);
        Task<CustomerTypeDetailOutput> GetDetail(EntityDto<long> input);
        Task<CustomerTypeDetailOutput> Update(UpdateCustomerTypeInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
