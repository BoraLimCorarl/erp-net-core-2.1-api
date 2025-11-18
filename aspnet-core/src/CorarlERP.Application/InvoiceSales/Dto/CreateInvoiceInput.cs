using Abp.Auditing;
using CorarlERP.Addresses;
using CorarlERP.Exchanges.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Invoices.Dto
{
    public class CreateInvoiceInput
    {
        public ReceiveFrom ReceiveFrom { get; set; }
        public TransactionStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AccountId { get; set; }
        public long LocationId { get; set; }
        public bool SameAsShippingAddress { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Reference { get; set; }
        public string ItemIssueReference { get; set; }
        public string InvoiceNo { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public long CurrencyId { get; set; }
        public long? ClassId { get; set; }
        public Guid? ItemIssueId { get; set; }
        public DateTime ETD { get; set; }
        public DateTime? IssuingDate { get; set; }
        public bool ConvertToItemIssue { get; set; }
        public long? TransactionTypeId { get; set; }
        public long? MultiCurrencyId { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        [DisableAuditing]
        public List<CreateOrUpdateInvoiceItemInput> InvoiceItems { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }

    }
}
