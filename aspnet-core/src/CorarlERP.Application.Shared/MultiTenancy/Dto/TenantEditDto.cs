using Abp.Application.Services.Dto;
using Abp.MultiTenancy;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorarlERP.MultiTenancy.Dto
{
    public class TenantEditDto : EntityDto
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public int? EditionId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }
        public bool UseDefaultAccount { get; set; }
        public bool AutoItemCode { get;  set; }       
        public string Prifix { get;  set; }
        public string ItemCode { get; set; }
        public GetSubscriptionInput Subscription { get; set; }
        public GetSubscriptionInput UpgradeFromSubscription { get; set; }
        //public List<SubscriptionPromotionInput> SubscriptionPromotions { get; set; }
        public Guid? SubScriptionId { get; set; }
        public bool UseBatchNo { get; set; }
        public bool DefaultInventoryReportTemplate { get; set; }
        public bool ProductionSummaryQty { get; set; }
        public bool ProductionSummaryNetWeight { get; set; }
    }

    public class GetSubscriptionDetailOutput
    {

        public string TenantName { get; set; }
        public int? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public Guid? PackageId { get; set; }
        public string Package { get; set; }
        public string EditionName { get; set; }
        public DateTime? Endate { get; set; }
        public DurationType? DurationType { get; set; }
        public bool Unlimited { get; set; }
        public bool IsTrail { get; set; }
        public bool ShowWarning { get; set; }
        public DateTime? SubScriptionEndDate { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal UpgradeDeduction { get; set; }
        public decimal TotalPrice { get; set; }
        public string SubscriptionType { get; set; }
    }

    public class GetSubscriptionDetailInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<int> TenantIds { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "TenantName";
            }

        }
    }
}