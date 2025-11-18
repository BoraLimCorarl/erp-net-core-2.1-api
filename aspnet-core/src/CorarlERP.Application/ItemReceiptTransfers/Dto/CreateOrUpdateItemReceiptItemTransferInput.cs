using CorarlERP.BatchNos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptTransfers
{
   public class CreateOrUpdateItemReceiptItemTransferInput
    {
      
        public Guid? Id { get; set; }
        
        public Guid ItemId { get; set; }

        public string Description { get; set; }
        
        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public Guid? TransferOrderItemId { get; set; }
        public long? LotId { get; set; }
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
    }
}
