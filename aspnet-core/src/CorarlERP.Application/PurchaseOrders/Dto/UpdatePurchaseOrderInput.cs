using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PurchaseOrders.Dto
{
   public class UpdatePurchaseOrderInput : CreatePurchaseOrderInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
