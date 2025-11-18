using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.ItemPrices;
using CorarlERP.Items.Dto;

namespace CorarlERP.ItemPriceAppServices.Dto
{
    [AutoMapFrom(typeof(ItemPriceItem))]
    public class GetDetailItemPriceItemOutput
    {
       // public Guid Id { get; set; }
       

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public List<GetItemPriceDetail> GetPriceDetail { get; set; }

    }

    public class GetItemPriceDetail
    {
        public Guid Id { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }
}
