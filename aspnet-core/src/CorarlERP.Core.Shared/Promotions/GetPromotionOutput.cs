using CorarlERP.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Promotions
{
    public class GetPromotionOutput
    {
        public List<PromotionSummaryDto> RenewablePromotions { get; set; }
        public List<PromotionSummaryDto> Promotions { get; set; }
        public List<PromotionSummaryDto> SpecificPackagePromotions { get; set; }
        
    }

    public class PromotionSummaryDto
    {
        public Guid? Id { get; set; }
        public string PromotionName { get; set; }
        public decimal Value { get; set; }
        public bool IsTrial { get; set; }
        public PromotionType PromotionType { get; set; }
        public bool IsRenewable { get; set; }
        public bool IsEligibleWithOther { get; set; }
        public Guid? CampaignId { get; set; }
        public bool IsSpecificPackage { get; set; }
        public List<SubscriptionCampaignPromotionInput> CampaignEditionPromotions { get; set; }
    }

    public class EditionPromotionSummaryDto : PromotionSummaryDto
    {
        public int EditionId { get; set; }
    }

}
