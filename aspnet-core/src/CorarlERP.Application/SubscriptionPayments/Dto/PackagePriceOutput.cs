using CorarlERP.MultiTenancy;
using CorarlERP.PackageEditions.Dot;
using CorarlERP.Promotions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SubscriptionPayments.Dto
{
    public class EditionPriceOutput
    {
        public Guid PackageId { get; set; }
        public int EditionId { get; set; }
        public string EditionName { get; set; }
        public int SortOrder { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal UpgrageDeduction { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalPrice { get; set; }
        public int DayRemaining { get; set; }         
        public int FreeDayRemaining { get; set; }         
        public SubscriptionType SubscriptionType { get; set; }
        public List<PromotionSummaryDto> FreeExtraMonths { get; set; }
        public List<PromotionSummaryDto> Discounts { get; set; }
        public decimal UpgradeExtraMonth { get; set; }
    }

    public class PackageSubscriptionOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<EditionPriceOutput> Editions { get; set; }
        public List<PackageFeatureDto> Features { get; set; }
        public List<PackageEditionFeatureDto> EditionFeatureValues { get; set; }

    }


    public class SubscriptionPaymentOutput
    {
        public DateTime SubscriptionDate { get; set; }
        public DateTime AffectedDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public DurationType DurationType { get; set; }
        public int EditionId { get; set; }
        public string EditionName { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
