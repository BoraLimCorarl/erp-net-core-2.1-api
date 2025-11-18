using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.PackageEditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Promotions
{
    public enum CampaignType
    {
        NewRegister = 0,
        Upgrade = 1,
    }


    [Table("CarlErpPromotionCampaigns")]
    public class PromotionCampaign : AuditedEntity<Guid>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool NeverEnd { get; private set; }
        public bool IsSpecificPackage { get; private set; }
        public Guid? PackageId { get; private set; }
        public Package Package { get; private set; }
        public Guid? PromotionId { get; private set; }
        public Promotion Promotion { get; private set; }
        public string Description { get; private set; }
        public bool IsRenewable { get; private set; }
        public bool IsEligibleWithOther {  get; private set; }
        public CampaignType CampaignType { get; private set; }

        public bool IsActive { get; private set; }       
        public void Enable(long userId, bool enable)
        {
            IsActive = enable;
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
        }

        public static PromotionCampaign Create(long userId, string name, CampaignType campaignType, DateTime startDate, DateTime? endDate, bool neverEnd, bool isSpecificPackage, Guid? packageId, Guid? promotionId, string description, bool isRenewable, bool isEligibleWithOther)
        {
            return new PromotionCampaign
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,  
                Name = name,
                CampaignType = campaignType,
                StartDate = startDate,
                EndDate = endDate,
                NeverEnd = neverEnd,
                IsSpecificPackage = isSpecificPackage,
                PackageId = packageId,
                PromotionId = promotionId,
                Description = description,
                IsRenewable = isRenewable,
                IsEligibleWithOther = isEligibleWithOther,
                IsActive = true
            };
        }

        public void Update(long userId, string name, CampaignType campaignType, DateTime startDate, DateTime? endDate, bool neverEnd, bool isSpecificPackage, Guid? packageId, Guid? promotionId, string description, bool isRenewable, bool isEligibleWithOther)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            Name = name;
            CampaignType = campaignType;
            StartDate = startDate;
            EndDate = endDate;
            NeverEnd = neverEnd;
            IsSpecificPackage = isSpecificPackage;
            PackageId = packageId;
            PromotionId = promotionId;
            Description = description;
            IsRenewable = isRenewable;
            IsEligibleWithOther = isEligibleWithOther;
        }

    }
}
