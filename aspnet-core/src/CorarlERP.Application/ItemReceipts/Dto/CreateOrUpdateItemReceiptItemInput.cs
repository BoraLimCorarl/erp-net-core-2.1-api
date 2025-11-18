using CorarlERP.BatchNos;
using CorarlERP.BatchNos.Dto;
using CorarlERP.PurchaseOrders;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceipts.Dto
{
    public class CreateOrUpdateItemReceiptItemInput
    {
        public Guid? PurchaseOrderId { get; set; }
        public Guid? Id { get; set; }

        public Guid? OrderItemId { get; set; }

        public Guid? BillItemId { get; set; }
    
        public Guid ItemId { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost {get;set;}

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
     
        public long? LotId { get; set; }

        public decimal OriginalQtyFromPurchaseOrder { get; set; }

        public string ItemName { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
    }

   public class ItemReceiptEntity
    {
        public Guid Id { get; set; }
        public Guid? OrderItemId { get; set; }
        public decimal Qty { get; set; }
        public decimal RemainingQty { get; set; }
        public Guid? POId { get; set; }       
    }

    public class ItemIssueEntity
    {
        public Guid Id { get; set; }
        public Guid? OrderItemId { get; set; }
        public decimal Qty { get; set; }
        public decimal RemainingQty { get; set; }
        public Guid? SOId { get; set; }
    }


}
