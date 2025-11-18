using CorarlERP.BatchNos;
using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssues.Dto
{
   public class CreateOrUpdateItemIssueItemInput
    {
        public Guid? SaleOrderId { get; set; }
        public Guid? Id { get; set; }

        public Guid? SaleOrderItemId { get; set; }

        public Guid? InvoiceItemId { get; set; }

        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }
        
        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }

        public Guid PurchaseAccountId { get; set; }
        public long? LotId { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public Guid? KitchenOrderItemAndBOMItemId { get; set; }
        public Guid? DeliveryScheduleItemId { get; set; }
        public Guid? DeliveryScheduleId { get; set; }

    }
}
