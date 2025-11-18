using System;
using System.Collections.Generic;

namespace CorarlERP.PurchasePrices.Dto
{
    public class CreatePurchasePriceInput
    {
        public long? LocationId { get; set; }
        public bool SpecificVendor { get; set; }
        public List<CreatePurchasePriceItemInput> PurchasePriceItems { get; set; }

    }
}
