using CorarlERP.MultiTenancy.Payments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;


namespace CorarlERP.MultiTenancy
{

    public enum SubscriptionType
    {
        Subscribe =0,
        Renew = 1,
        Upgrade = 2,
    }

    public enum ItemCodeSetting
    {    
        Auto = 1,
        Custom = 0,
    }
    public class GetSubscriptionInput
    {

        public Guid? Id { get; set; }
        public int TenantId { get; set; }
        public int? Duration { get; set; }
        public DateTime? SubscriptionDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public Guid? PackageId { get; set; }
        public int? EditionId { get; set; }
        public DateTime? EndDate { get; set; }
        public DurationType? DurationType { get; set; }
        public bool Unlimited { get; set; }
        public bool IsTrail { get; set; }
        public bool ShowWarning { get; set; }
        public bool IsExp { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal UpgradeDeduction { get; set; }
        public decimal TotalPrice { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public Guid? UpgradeFromSubscriptionId { get; set; }
        public List<SubscriptionPromotionInput> SubscriptionPromotions { get; set; }

    }
    public enum DurationType
    {   
        Year = 1,
        Month = 2,
        Day = 3
    }
    public enum PromotionType
    {
        Discount = 1,
        FreeExtraMonth = 2
    }
    public class SubscriptionPromotionInput
    {
        public Guid? Id { get; set; }
        public Guid SubscriptionId { get;  set; }    
        public Guid? CampaignId { get; set; }
        public Guid? PromotionId { get;  set; }
        public string PromotionName { get; set; }
        public PromotionType? PromotionType { get;  set; }
        public bool IsRenewable { get; set; }
        public bool IsEligibleWithOther { get; set; }
        public decimal Value { get; set; }
        public bool IsTrial { get; set; }
        public bool IsSpecificPackage { get; set; }
        public List<SubscriptionCampaignPromotionInput> CampaignEditionPromotions { get; set; }
    }

    public class SubscriptionCampaignPromotionInput
    {
        public Guid? Id { get; set; }
        public Guid? SubscriptionPromotionId { get; set; }
        public Guid CampaignId { get; set; }
        public int EditionId { get; set; }
        public string EditionName { get; set; }
        public Guid? PromotionId { get; set; }
        public string PromotionName { get; set; }
        public PromotionType PromotionType { get; set; }
        public decimal Value { get; set; }
        public int SortOrder { get; set; }
        public bool IsTrial { get; set; }

    }

}
