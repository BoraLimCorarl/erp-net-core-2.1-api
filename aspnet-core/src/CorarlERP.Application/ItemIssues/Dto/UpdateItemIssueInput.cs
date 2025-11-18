using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssues.Dto
{
   public class UpdateItemIssueInput : CreateItemIssueInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
