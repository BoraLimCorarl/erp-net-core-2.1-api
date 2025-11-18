using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.QCSamples.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.QCSamples
{
   public interface IQCSampleAppService : IApplicationService
    {
        Task<QCSampleDetailOutput> Create(CreateQCSampleInput input);
        Task<PagedResultDto<QCSampleDetailOutput>> GetList(GetQCSampleListInput input);
        Task<PagedResultDto<QCSampleDetailOutput>> Find(GetQCSampleListInput input);
        Task<QCSampleDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<QCSampleDetailOutput> Update(UpdateQCSampleInput input);
        Task Delete(EntityDto<Guid> input);
    }
}
