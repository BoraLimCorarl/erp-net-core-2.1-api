using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SubItems.Dto
{
   public class CreateSubItemInput
    {
        public Guid? Id { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public Guid ItemId { get; set; }
    }

    public class CreateSubItemInputExportExcel
    {
        public Guid? Id { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }

        public Guid ParentItemId { get; set; }
        public ItemSummaryOutput ParentItem { get; set; }
    }
    public class LotOutputExport
    {

       public string ItemName { get; set; } 
       public string LotName { get; set; }
        public string ItemCode { get; set; }
        public Guid ItemId { get; set; }
    }

    public class ItemLotOutputExport
    {

      
        public long LotId { get; set; }
        public Guid ItemId { get; set; }
        public Guid Id { get; set; }
    }
}
