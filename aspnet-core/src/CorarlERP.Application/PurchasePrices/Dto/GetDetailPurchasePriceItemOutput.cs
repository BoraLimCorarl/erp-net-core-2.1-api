using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.PurchasePrices;
using CorarlERP.Items.Dto;

namespace CorarlERP.PurchasePrices.Dto
{
    [AutoMapFrom(typeof(PurchasePriceItem))]
    public class GetDetailPurchasePriceItemOutput
    {
       // public Guid Id { get; set; }
       

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }
        public List<GetPurchasePriceDetail> GetPriceDetail { get; set; }

    }

    public class GetPurchasePriceDetail
    {
        public Guid Id { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }
}
