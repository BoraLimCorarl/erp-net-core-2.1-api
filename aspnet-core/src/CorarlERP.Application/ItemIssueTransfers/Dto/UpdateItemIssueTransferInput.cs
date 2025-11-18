using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueTransfers.Dto
{
   public class UpdateItemIssueTransferInput : CreateItemIssueTransferInput
    {
        public Guid Id { get; set; }
        public Guid ItemReceiptId { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
