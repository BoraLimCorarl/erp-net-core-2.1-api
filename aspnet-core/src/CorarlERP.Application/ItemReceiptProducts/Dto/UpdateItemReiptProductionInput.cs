using System;

namespace CorarlERP.ItemReceiptProducts.Dto
{
    public class UpdateItemReceiptProductionInput : CreateItemReceiptProductionInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
