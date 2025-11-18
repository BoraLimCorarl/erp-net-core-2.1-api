using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PhysicalCounts.Dto
{
    [AutoMapFrom(typeof(ItemReceiptItem))]
    public class ItemReceiptItemPhysicalCountDetailOutput
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public long? LotId { get; set; }
        public LotSummaryOutput LotDetail {get;set;}
        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
