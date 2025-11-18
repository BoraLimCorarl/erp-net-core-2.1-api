using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueVendorCredits.Dto
{
    public class UpdateItemIssueVendorCreditInput : CreateItemIssueVendorCreditInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
