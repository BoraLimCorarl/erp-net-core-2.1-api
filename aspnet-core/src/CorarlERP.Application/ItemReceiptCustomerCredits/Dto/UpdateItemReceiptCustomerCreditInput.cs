using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptCustomerCredits.Dto
{
    public class UpdateItemReceiptCustomerCreditInput : CreateItemReceiptCustomerCreditInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
