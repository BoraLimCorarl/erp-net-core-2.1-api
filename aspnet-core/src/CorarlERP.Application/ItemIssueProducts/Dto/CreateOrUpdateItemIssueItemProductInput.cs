using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueProducts
{
   public class CreateOrUpdateItemIssueItemProductInput
    {
      
        public Guid? Id { get; set; }
        
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public string Description { get; set; }
        
        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }

        public Guid? TransferItemId { get; set; }
        public string TransferOrderNo { get; set; }

    }
}
