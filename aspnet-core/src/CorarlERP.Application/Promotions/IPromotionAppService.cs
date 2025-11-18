using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Promotions.Dot;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Promotions
{
    public interface IPromotionAppService : IApplicationService
    {
        Task Create(PromotionDto input);
        Task<PromotionDto> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<GetPackagePromotionListDto>> GetList(GetPromotionListInput input);
        Task<ListResultDto<GetPackagePromotionListDto>> Find(GetPromotionListInput input);
        Task Update(PromotionDto input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<GetPromotionOutput> GetDefaultPromotion(GetPromotionInput input);
    }
}
