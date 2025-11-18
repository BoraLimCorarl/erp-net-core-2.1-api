using CorarlERP.BatchNos;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.TransferOrders.Dto
{
    public class CreateOrUpdateTransferOrderItemInput
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }
        public string Description { get; set; }
        public decimal Unit { get; set; }
        public DateTime CreateTime { get; set; }

        public LotSummaryOutput FromLotDetail {get;set;}
        public long? FromLotId { get; set; }

        public LotSummaryOutput ToLotDetail { get; set; }
        public long? ToLotId { get; set; }
      
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
