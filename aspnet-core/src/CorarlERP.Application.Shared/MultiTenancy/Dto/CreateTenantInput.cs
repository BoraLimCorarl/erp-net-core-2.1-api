using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.MultiTenancy;

namespace CorarlERP.MultiTenancy.Dto
{
    public class CreateTenantInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        [Required]       
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpUserBase.MaxPasswordLength)]
        public string AdminPassword { get; set; }

        [MaxLength(AbpTenantBase.MaxConnectionStringLength)]
        public string ConnectionString { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public bool SendActivationEmail { get; set; }

        public int? EditionId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }
        public bool UseDefaultAccount { get; set; }

        public bool AutoItemCode { get;  set; }
        public string Prifix { get;  set; }
        public string ItemCode { get;  set; }
        public GetSubscriptionInput Subscription { get; set; }
        //public List<SubscriptionPromotionInput> SubscriptionPromotions { get; set; }
        public bool UseBatchNo { get; set; }
        public bool DefaultInventoryReportTemplate { get; set; }
        public bool IsTrail { get; set; }
        public bool ShowWarning { get; set; }
        public Guid? SignUpId { get; set; }
    }
    public class GetDebugInput { 
    
        public int? Id { get; set; }
    }
}