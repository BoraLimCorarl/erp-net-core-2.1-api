using CorarlERP.BatchNos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptOthers.Dto
{
   public class CreateOrUpdateItemReceiptItemOtherInput
    {
        public Guid? Id { get; set; }

        public Guid ItemId { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public long? ToLotId { get; set; }

        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
    }
}
