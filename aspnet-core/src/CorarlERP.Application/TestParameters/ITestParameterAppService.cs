using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.TestParameters.Dto;
using System.Threading.Tasks;

namespace CorarlERP.TestParameters
{
   public interface ITestParameterAppService : IApplicationService
    {
        Task<TestParameterDetailOutput> Create(CreateTestParameterInput input);
        Task<PagedResultDto<TestParameterDetailOutput>> GetList(GetTestParameterListInput input);
        Task<PagedResultDto<TestParameterDetailOutput>> Find(GetTestParameterListInput input);
        Task<TestParameterDetailOutput> GetDetail(EntityDto<long> input);
        Task<TestParameterDetailOutput> Update(UpdateTestParameterInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
