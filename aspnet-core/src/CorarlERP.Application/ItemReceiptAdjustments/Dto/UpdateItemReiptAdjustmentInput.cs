using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptAdjustments.Dto
{
    public  class UpdateItemReiptAdjustmentInput : CreateItemReceiptAdjustmentInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
