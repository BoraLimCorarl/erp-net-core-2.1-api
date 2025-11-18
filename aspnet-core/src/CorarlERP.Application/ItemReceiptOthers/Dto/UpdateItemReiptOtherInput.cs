using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptOthers.Dto
{
   public class UpdateItemReiptOtherInput : CreateItemReceiptOtherInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
