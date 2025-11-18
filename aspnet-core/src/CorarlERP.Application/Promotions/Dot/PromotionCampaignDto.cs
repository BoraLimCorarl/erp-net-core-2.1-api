using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using CorarlERP.PackageEditions;
using CorarlERP.Promotions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Promotions.Dot
{

    public class PromotionCampaignDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public CampaignType CampaignType { get; set; }
        public string CampaignTypeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool NeverEnd { get; set; }
        public bool IsRenewable { get; set; }
        public bool IsEligibleWithOther { get; set; }
        public bool IsSpecificPackage { get; set; }
        public Guid? PackageId { get; set; }
        public string PackageName { get; set; }
        public Guid? PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string Description { get; set; }
      
        public bool IsActive { get; set; }
        public List<PromotionCampaignEditionDto> PromotionEditions { get; set; }
    }

    public class GetPromotionCampaignListDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public CampaignType CampaignType { get; set; }
        public string CampaignTypeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool NeverEnd { get; set; }
        public bool IsRenewable { get; set; }
        public bool IsEligibleWithOther { get; set; }
        public bool IsSpecificPackage { get; set; }
        public Guid? PackageId { get; set; }
        public string PackageName { get; set; }
        public Guid? PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class PromotionCampaignEditionDto
    {
        public Guid? Id { get; set; }
        public int EditionId { get; set; }
        public string EditionName { get; set; }
        public int SortOrder { get; set; }
        public Guid? PromotionId { get; set; }
        public string PromotionName { get;  set; }
    }
}
