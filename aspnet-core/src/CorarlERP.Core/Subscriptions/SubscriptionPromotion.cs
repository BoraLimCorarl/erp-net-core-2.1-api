using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Promotions;
using MailKit.Search;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Subscriptions
{
    [Table("CarlErpSubscriptionPromotions")]
    public class SubscriptionPromotion : AuditedEntity<Guid>
    {
        public Guid SubscriptionId { get; private set; }
        public Subscription Subscription { get; private set; }     
        public Guid? CampaignId { get; private set; }
        public PromotionCampaign PromotionCampaign { get; private set; }
        public bool IsRenewable { get; private set; }
        public bool IsEligibleWithOther { get; private set; }
        public Guid? PromotionId { get; private set; }
        public Promotion Promotion { get; private set; }
        public ICollection<SubscriptionCampaignPromotion> SubscriptionCampaignPromotions { get; private set; }
        public bool IsSpecificPackage { get; private set; }

        public static SubscriptionPromotion Create(long? userId, Guid subscriptionId, Guid? campaignId, Guid? promotionId, bool isRenewable,bool isEligibleWithOther, bool isSpecificPackage)
        {
            return new SubscriptionPromotion
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                SubscriptionId = subscriptionId,
                CampaignId = campaignId,
                PromotionId = promotionId,
                IsRenewable = isRenewable,
                IsEligibleWithOther = isEligibleWithOther,
                IsSpecificPackage = isSpecificPackage
            };
        }

        public void Update(long? userId, Guid subscriptionId, Guid? campaignId, Guid? promotionId, bool isRenewable, bool isEligibleWithOther, bool isSpecificPackage)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            SubscriptionId = subscriptionId;
            CampaignId = campaignId;
            PromotionId = promotionId;
            IsRenewable = isRenewable;
            IsEligibleWithOther = isEligibleWithOther;
            IsSpecificPackage = isSpecificPackage;
        }
    }
}
