using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceSales.Dto
{
    public class PaymentSummaryDto
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string CurrencyCode { get; set; }
        public string JournalType { get; set; }
    }

    public class PaymentSummaryResultOutput
    {
        public string CurrencyCode { get; set; } 
        public decimal Total { get; set; }
        public List<PaymentSummaryDto> Items { get; set; }
    }

    public class PaymentSummaryGetListInput
    {
        public Guid InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Guid CustomerId { get; set; }
        public long CreationTimeIndex { get; set; }
    }
}
