using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptProducts.Dto
{
    [AutoMapFrom(typeof(ItemReceiptItem))]
    public class ItemReceiptItemProductionDetailOutput
    {
        public Guid Id { get; set; }       
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? FirnishItemId { get; set; }
        public string ProductionOrderNo { get; set; }

        public long? LotId { get; set; }
        public LotSummaryOutput LotDetail { get; set; }

        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
