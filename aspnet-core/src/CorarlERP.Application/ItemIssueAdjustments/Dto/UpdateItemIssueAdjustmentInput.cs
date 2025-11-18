using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueAdjustments.Dto
{
   public class UpdateItemIssueAdjustmentInput : CreateItemIssueAdjustmentInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
