using CorarlERP.Invoices.Dto;
using CorarlERP.ReceivePayments.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.POS.Dto
{
    public class UpdatePOSInput: CreatePOSInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
