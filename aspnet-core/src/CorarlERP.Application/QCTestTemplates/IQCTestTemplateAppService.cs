using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.QCTestTemplates.Dto;
using System.Threading.Tasks;

namespace CorarlERP.QCTestTemplates
{
   public interface IQCTestTemplateAppService : IApplicationService
    {
        Task<QCTestTemplateDetailOutput> Create(CreateQCTestTemplateInput input);
        Task<PagedResultDto<QCTestTemplateDetailOutput>> GetList(GetQCTestTemplateListInput input);
        Task<PagedResultDto<QCTestTemplateDetailOutput>> Find(GetQCTestTemplateListInput input);
        Task<QCTestTemplateDetailOutput> GetDetail(EntityDto<long> input);
        Task<QCTestTemplateDetailOutput> Update(UpdateQCTestTemplateInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
