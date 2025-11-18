using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Promotions.Dot;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Promotions
{
    public interface IPromotionCampaignAppService : IApplicationService
    {
        Task Create(PromotionCampaignDto input);
        Task<PromotionCampaignDto> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<GetPromotionCampaignListDto>> GetList(GetPromotionCampaignListInput input);
        Task<ListResultDto<GetPromotionCampaignListDto>> Find(FindPromotionCampaignInput input);
        Task Update(PromotionCampaignDto input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<PromotionSummaryDto> GetPromotion(GetCampaignPromotionInput input);
    }
}
