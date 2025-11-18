using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Promotions;
using MailKit.Search;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Subscriptions
{
    [Table("CarlErpSubscriptionCampaignPromotions")]
    public class SubscriptionCampaignPromotion : AuditedEntity<Guid>
    {
        public Guid SubscriptionPromotionId { get; private set; }
        public SubscriptionPromotion SubscriptionPromotion { get; private set; }     
        public Guid CampaignId { get; private set; }
        public PromotionCampaign PromotionCampaign { get; private set; }
      
        public Guid? PromotionId { get; private set; }
        public Promotion Promotion { get; private set; }
        public int EditionId { get; private set; }
        public Edition Edition { get; private set; }
        public int SortOrder { get; private set; }

        public static SubscriptionCampaignPromotion Create(long? userId, Guid subscriptionPromotionId, Guid campaignId, int editionId, int sortOrder, Guid? promotionId)
        {
            return new SubscriptionCampaignPromotion
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                SubscriptionPromotionId = subscriptionPromotionId,
                CampaignId = campaignId,
                EditionId = editionId,
                PromotionId = promotionId
            };
        }

        public void Update(long? userId, Guid subscriptionPromotionId, Guid campaignId, int editionId, int sortOrder, Guid? promotionId)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            SubscriptionPromotionId = subscriptionPromotionId;
            CampaignId = campaignId;
            EditionId = editionId;
            PromotionId = promotionId;
        }
    }
}
