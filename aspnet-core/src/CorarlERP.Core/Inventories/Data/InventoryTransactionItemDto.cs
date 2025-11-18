using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class InventoryTransactionItemDto
    {
        public int Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid? TransferOrProductionId { get; set; }
        public Guid? TransferItemId { get; set; }
        public Guid JournalId { get; set; }
        public DateTime Date { get; set; }
        public string OrderIndex { get; set; } //{Date:yyyymmdd}-{CreationTimeIndex}-{CeationTime:yyMMddHHmmssfff}
        public JournalType JournalType { get; set; }

        public Guid ItemId { get; set; }
        public Guid InventoryAccountId { get; set; }
        public long LocationId { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineCost { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AvgCost { get; set; }
        public decimal AdjustmentCost { get; set; }
        public bool IsItemReceipt { get; set; }
    }
}
