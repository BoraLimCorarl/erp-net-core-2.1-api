using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ReceivePayments.Dto
{
    public class UpdateReceivePaymentInput : CreateReceivePaymentInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
