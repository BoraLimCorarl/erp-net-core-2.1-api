using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.PurchaseOrders.Dto
{
   public class CreateOrUpdatePurchaseOrderItemInput
    {
        
        public Guid? Id { get; set; }
        [Required]
        public Guid ItemId { get; set; }                      
        public long TaxId { get; set; }
        public string Description { get; set; }
        public decimal Unit { get; set; }
        public decimal UnitCost { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal Total { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal TaxRate { get; set; }        
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalBillQty { get; set; }

    }
}
