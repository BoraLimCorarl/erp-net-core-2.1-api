using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.DeliverySchedules.Dto
{
   public class DeliveryScheduleItemInput
    {       
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid ItemId { get; set; }
        public decimal Qty { get; set; }
        public string Description { get; set; }      
        public Guid DeliveryScheduleId { get; set; }
        public Guid? SaleOrderItemId { get; set; }
        public Guid? Id { get; set; }
        public Guid? InvoiceItemId { get; set; }
        public Guid? ItemIssueItemId { get; set; }
        public string OrderNumber { get; set; } 
        public decimal OrginalQtyFromSaleOrder { get; set; }
        public decimal Remain { get; set; }

    }
}
