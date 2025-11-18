using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Invoices.Dto
{
    public class UpdateInvoiceInput: CreateInvoiceInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
