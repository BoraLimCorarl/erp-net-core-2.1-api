using Abp.Domain.Entities.Auditing;
using CorarlERP.Promotions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PackageEditions.Dot
{

    public class PackageDto
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<PackageEditionDto> Editions { get; set; }
        public List<PackageFeatureDto> Features { get; set; }
        public List<PackageEditionFeatureDto> EditionFeatureValues { get; set; }
        //public List<PackageEditionPromotionDto> Discounts { get; set; }
        //public List<PackageEditionPromotionDto> FreeExtraMonths { get; set; }
    }

    public class GetPackageListDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class PackageEditionDto
    {
        public Guid? Id { get; set; }
        public int EditionId { get; set; }
        public string EditionName { get; set; }
        public int SortOrder { get; set; }
        public decimal AnnualPrice { get; set; }
    }

    public class PackageFeatureDto
    {
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; }
        public int SortOrder { get; set; }
    }

    public class PackageEditionFeatureDto
    {
        public Guid? Id { get; set; }
        public int EditionId { get; set; }
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }

    //public class PackageEditionPromotionDto
    //{
    //    public Guid? Id { get; set; }
    //    public int EditionId { get; set; }
    //    public Guid? PromotionId { get; set; }
    //    public string PromotionName { get; set; }
    //}

}
