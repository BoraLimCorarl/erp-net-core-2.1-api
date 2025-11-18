using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerTypes.Dto;
using CorarlERP.ItemPrices;
using CorarlERP.Locations.Dto;
using CorarlERP.TransactionTypes.Dto;

namespace CorarlERP.ItemPriceAppServices.Dto
{
    [AutoMapFrom(typeof(ItemPrice))]
    public class GetDetailItemPriceOutput
    {
        public Guid Id { get; set; }
        public long? LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? SaleTypeId { get; set; }
        public TransactionTypeSummaryOutput SaleType { get; set; }

        public long? CustomerTypeId { get; set; }
        public CustomerTypeSummaryOutput CustomerType { get; set; }

        public List<GetDetailItemPriceItemOutput> ItemPriceItems { get; set; }

    }

    [AutoMapFrom(typeof(ItemPrice))]
    public class GetSummanryItemPriceOutput
    {
        public Guid? Id { get; set; }
        public long? LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? SaleTypeId { get; set; }
        public TransactionTypeSummaryOutput SaleType { get; set; }
        public long? CustomerTypeId { get; set; }
        public CustomerTypeSummaryOutput CustomerType { get; set; }

    }
}
