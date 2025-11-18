using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CustomerCredits.Dto
{
    public class UpdateCustomerCreditInput : CreateCustomerCreditInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
