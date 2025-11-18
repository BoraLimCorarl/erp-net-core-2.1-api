using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.LabTestResults.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.LabTestResults
{
   public interface ILabTestResultAppService : IApplicationService
    {
        Task<LabTestResultDetailOutput> Create(CreateLabTestResultInput input);
        Task<PagedResultDto<LabTestResultDetailOutput>> GetList(GetLabTestResultListInput input);
        Task<LabTestResultDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<LabTestResultDetailOutput> Update(UpdateLabTestResultInput input);
        Task Delete(EntityDto<Guid> input);
    }
}
