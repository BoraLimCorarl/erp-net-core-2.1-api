using CorarlERP.BatchNos;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Productions.Dto
{
    public class CreateOrUpdateRawMaterialItemsInput
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }
        public string Description { get; set; }
        public decimal Unit { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }

        public Guid? RawMaterialId { get; set; }
        public string ProductionOrderNo { get; set; }
        public long? FromLotId { get; set; }
        public LotSummaryOutput FromLotDetail {get;set;}
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get;set; }
    }
}
