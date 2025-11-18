using CorarlERP.Addresses;
using CorarlERP.Exchanges.Dto;
using CorarlERP.POS.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerCredits.Dto
{
    public class CreateCustomerCreditInput
    {
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
        public string ItemReceiptReference { get; set; }

        public string CustomerCreditNo { get; set; }

        public string Memo { get; set; }

        public DateTime CreditDate { get; set; }
        public DateTime DueDate { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public long? MultiCurrencyId { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyTax { get; set; }

        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid? ItemReceiptCustomerCreditId { get; set; }

        public Guid? ItemIssueSaleId { get; set; }

        public List<CustomerCreditDetailInput> CustomerCreditDetail { get; set; }

        public DateTime? ReceiveDate { get; set; }
        public bool ConvertToItemReceipt { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

        public bool IsPOS { get; set; }
        public virtual ICollection<POSPaymentSummaryByPaymentMethodInput> ReceivePayments { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }
}
