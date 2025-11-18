using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PayBills.Dto
{
    public class UpdatePayBillInput : CreatePayBillInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
