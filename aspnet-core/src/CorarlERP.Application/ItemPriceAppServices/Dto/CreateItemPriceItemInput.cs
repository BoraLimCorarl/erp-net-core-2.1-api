using System;
using System.Collections.Generic;

namespace CorarlERP.ItemPriceAppServices.Dto
{
    public class CreateItemPriceItemInput
    {
        //public Guid? Id { get; set; }
        public List<ItemPriceDetail> Prices { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }

    }

    public class ItemPriceDetail
    {
        public Guid? Id { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }
}
