using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using CorarlERP.Locations.Dto;
using CorarlERP.VendorTypes.Dto;

namespace CorarlERP.PurchasePrices.Dto
{
    [AutoMapFrom(typeof(PurchasePrice))]
    public class GetDetailPurchasePriceOutput
    {
        public Guid Id { get; set; }
        public long? LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public bool SpecificVendor { get; set; }

        public List<GetDetailPurchasePriceItemOutput> PurchasePriceItems { get; set; }

    }

    [AutoMapFrom(typeof(PurchasePrice))]
    public class GetSummanryPurchasePriceOutput
    {
        public Guid? Id { get; set; }
        public long? LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }
        public bool SpecificVendor { get; set; }
    }
}
