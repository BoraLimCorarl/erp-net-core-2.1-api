using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class CreateOrUpdatePhysicalItemInput
    {
        public Guid? Id { get; set; }
        public int No { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        public long? LotId { get; set; }
        public Guid? BatchNoId { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal CountQty { get; set; }
        public decimal DiffQty { get; set; }
        public string Description { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total { get; set; }
        public Guid? ItemIssueItemId { get; set; }
        public Guid? ItemReceiptItemId { get; set; }
    }

    public class PhysicalItemDetailDto : CreateOrUpdatePhysicalItemInput
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string LotName { get; set; }
        public string BatchNo { get; set; }
        public bool TrackSerial { get; set; }
    }

    public class CountByItemInput
    {
        public Guid? Id { get; set; }
        public decimal CountQty { get; set; }
        public decimal UnitCost { get; set; }
        public string Description { get; set; }
    }
}
