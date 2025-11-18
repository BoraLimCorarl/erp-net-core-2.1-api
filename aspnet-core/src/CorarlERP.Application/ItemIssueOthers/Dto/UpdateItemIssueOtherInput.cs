using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueOthers.Dto
{
   public class UpdateItemIssueOtherInput : CreateItemIssueOtherInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
