using Abp.Domain.Entities.Auditing;
using CorarlERP.MultiTenancy;
using CorarlERP.Promotions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Promotions.Dot
{

    public class PromotionDto
    {
        public Guid? Id { get; set; }
        public string PromotionName { get; set; }
        public PromotionType PromotionType { get; set; }
        public string PromotionTypeName { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal ExtraMonth { get; set; }
        public bool IsTrial { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetPackagePromotionListDto
    {
        public Guid Id { get; set; }
        public string PromotionName { get; set; }
        public PromotionType PromotionType { get; set; }
        public string PromotionTypeName { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal ExtraMonth { get; set; }
        public bool IsTrial { get; set; }
        public bool IsActive { get; set; }
    }

}
