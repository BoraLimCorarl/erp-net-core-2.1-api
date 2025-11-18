using CorarlERP.BatchNos;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Productions.Dto
{
   public class CreateOrUpdateFinishItemInput
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }
        public string Description { get; set; }
        public Guid InventoryAccount { get; set; }
        public decimal Unit { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateTime { get; set; }
        public long ToLotId { get; set; }
        public LotSummaryOutput ToLotDetail { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
