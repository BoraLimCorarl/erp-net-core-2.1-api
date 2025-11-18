using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Inventories.Data
{
    public class GetAvgCostForIssueOutput
    {
        public decimal Total { get; set; }
        public List<GetAvgCostForIssueData> Items {get;set;}
        public List<RoundingAdjustmentItemOutput> RoundingItems { get; set; }

    }

    public class RoundingAdjustmentItemOutput
    {
        public Guid ItemId { get; set; }
        public decimal AdjustmentBalance { get; set; }
        public Guid InventoryAccountId { get; set; }
    }
}
