using System;
using System.Collections.Generic;

namespace CorarlERP.PurchasePrices.Dto
{
    public class CreatePurchasePriceItemInput
    {
        //public Guid? Id { get; set; }
        public List<PurchasePriceDetail> Prices { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }

    }

    public class PurchasePriceDetail
    {
        public Guid? Id { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }
}
