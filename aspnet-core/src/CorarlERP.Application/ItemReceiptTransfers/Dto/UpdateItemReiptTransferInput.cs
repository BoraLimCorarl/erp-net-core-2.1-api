using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptTransfers.Dto
{
   public class UpdateItemReceiptTransferInput : CreateItemReceiptTransferInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
