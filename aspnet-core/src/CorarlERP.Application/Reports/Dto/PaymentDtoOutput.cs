using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Reports.Dto
{
    public class PaymentDtoOutput
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid? TransactionId { get;  set; }
        public Guid CustomerId { get;  set; }
        public string PayToInvoiceNo { get;  set; }
        public string CustomerCode { get;  set; }
        public string CustomerName { get;  set; }
        public string CustomerType { get;  set; }
        public long? CustomerTypeId { get;  set; }
        public decimal Payment { get;  set; }
        public bool IsItem { get;  set; }
        public DateTime PayToInvoiceDate { get;  set; }


        public DateTime CreationTime { get;  set; }
        public long CurrencyId { get;  set; }
        public string CurrencyCode { get;  set; }
        public long? MultiCurrencyId { get;  set; }
        public string MultiCurrencyCode { get;  set; }
        public long? SaleTypeId { get;  set; }
        public string SaleTypeName { get;  set; }


        public decimal Cash { get; set; }
        public decimal Bank { get; set; }
        public decimal Other { get; set; }        

        public decimal Credit { get;  set; }
        public decimal Expense { get;  set; }
        public string Account { get;  set; }
        public string AccountType { get;  set; }

    }
}
