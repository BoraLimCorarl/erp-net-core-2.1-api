using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceipts.Dto
{
  public class UpdateItemReceiptInput : CreateItemReceiptInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set;}
    }

    
}
