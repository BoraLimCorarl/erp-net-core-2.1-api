using Abp.Auditing;
using CorarlERP.Addresses;
using CorarlERP.Invoices.Dto;
using System;
using System.Collections.Generic;

namespace CorarlERP.POS.Dto
{
    public class CreatePOSInput 
    {
        public Guid CustomerId { get; set; }
        public Guid AccountId { get; set; }
        public long LocationId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public long CurrencyId { get; set; }
        public long? ClassId { get; set; }

        public long?    MultiCurrencyId { get; set; } 
        public decimal  MultiCurrencySubTotal { get; set; } 
        public decimal  MultiCurrencyTax { get; set; }
        public decimal  MultiCurrencyTotal { get; set; }
       


        [DisableAuditing]
        public List<CreateOrUpdateInvoiceItemInput> InvoiceItems { get; set; }        
        public virtual ICollection<POSPaymentSummaryByPaymentMethodInput> ReceivePayments { get; set; }
        public virtual ICollection<POSStoreCreditPaymentInput> StoreCreditPayments { get; set; }
        public bool IsConfirm { get; set; }
    }

    public class POSPaymentSummaryByPaymentMethodInput
    {
        public Guid PaymentMethodId { get; set; }
        public decimal Total { get; set; }
        public Guid AccountId { get; set; }

        public decimal MultiCurrencyTotal { get; set; }
    }

    public class POSStoreCreditPaymentInput
    {
        public Guid StoreCreditId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Total { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
    }
}
