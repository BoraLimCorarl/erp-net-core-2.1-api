using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using MailKit.Search;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Promotions
{
    [Table("CarlErpPromotionCampaignEditions")]
    public class PromotionCampaignEdition : AuditedEntity<Guid>
    {
        public Guid PromotionCampaignId { get; private set; }
        public PromotionCampaign PromotionCampaign { get; private set; }
        public int EditionId { get; private set; }
        public Edition Edition { get; private set; }
        public int SortOrder { get; private set; }
        public Guid? PromotionId { get; private set; }
        public Promotion Promotion { get; private set; }


        public static PromotionCampaignEdition Create(long userId, Guid promotionCampaignId, int editionId, int sortOrder, Guid? promotionId)
        {
            return new PromotionCampaignEdition
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PromotionCampaignId = promotionCampaignId,
                EditionId = editionId,
                SortOrder = sortOrder,
                PromotionId = promotionId
            };
        }

        public void Update(long userId, Guid promotionCampaignId, int editionId, int sortOrder, Guid? promotionId)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PromotionCampaignId = promotionCampaignId;
            EditionId = editionId;
            SortOrder = sortOrder;
            PromotionId = promotionId;
        }
    }
}
