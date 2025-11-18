using Abp.AutoMapper;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SubItems.Dto
{
   // [AutoMapFrom(typeof(SubItem))]
    public class SubItemDetailOuput
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public ItemSummaryOutput Item { get; set; }
        public Guid ItemId { get; set; }
    }

  
}
