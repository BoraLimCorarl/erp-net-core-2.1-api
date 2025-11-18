using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.LabTestRequests.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.LabTestRequests
{
   public interface ILabTestRequestAppService : IApplicationService
    {
        Task<LabTestRequestDetailOutput> Create(CreateLabTestRequestInput input);
        Task<PagedResultDto<LabTestRequestDetailOutput>> Find(GetLabTestRequestListInput input);
        Task<PagedResultDto<LabTestRequestDetailOutput>> GetList(GetLabTestRequestListInput input);
        Task<LabTestRequestDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<LabTestRequestDetailOutput> Update(UpdateLabTestRequestInput input);
        Task Delete(EntityDto<Guid> input);
        Task UpdateToSent(EntityDto<Guid> input);
        Task UpdateToPending(EntityDto<Guid> input);
    }
}
