using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Inventories.Data
{
    public class GetAvgCostForIssueData
    {
        public int Index { get; set; }

      
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineCost { get; set; }
        public long? LotId { get; set; }
        public string ItemName { get; set; }
        public Guid? ItemIssueItemId { get; set; }
        public Guid? ItemReceiptItemId { get; set; }

        public Guid InventoryAccountId { get; set; }
        public long ItemTypeId { get; set; }

       
    }
}
