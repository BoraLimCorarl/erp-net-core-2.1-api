using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using CorarlERP.MultiTenancy.Payments;
using CorarlERP.MultiTenancy.Payments.Dto;

namespace CorarlERP.MultiTenancy.Dto
{
    public class RegisterTenantInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string UserNameName { get; set; }


        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        public string AdminPassword { get; set; }

        [DisableAuditing]
        public string CaptchaResponse { get; set; }

        public SubscriptionStartType SubscriptionStartType { get; set; }

        public SubscriptionPaymentGatewayType? Gateway { get; set; }

        public int? EditionId { get; set; }

        public string PaymentId { get; set; }
        public bool UseDefaultAccount { get; set; }
        public bool AutoItemCode { get; set; }
        public string Prifix { get; set; }
        public string ItemCode { get; set; }
        public GetSubscriptionInput Subscription { get; set; }
        public List<SubscriptionPromotionInput> SubscriptionPromotions { get; set; }
        public bool UseBatchNo { get; set; }
        public bool DefaultInventoryReportTemplate { get; set; }
        public Guid? SignupId { get; set; }
    }
}