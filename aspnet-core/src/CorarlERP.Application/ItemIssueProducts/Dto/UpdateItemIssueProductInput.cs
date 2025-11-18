using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueProducts.Dto
{
   public class UpdateItemIssueProductInput : CreateItemIssueProductInput
    {
        public Guid Id { get; set; }
        public Guid ItemReceiptId { get; set; }
    }
}
