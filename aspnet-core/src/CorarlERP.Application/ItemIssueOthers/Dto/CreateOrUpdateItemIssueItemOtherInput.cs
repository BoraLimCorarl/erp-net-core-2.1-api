using CorarlERP.BatchNos;
using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueOthers.Dto
{
   public class CreateOrUpdateItemIssueItemOtherInput
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
        public long? FromLotId { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
    }
}
