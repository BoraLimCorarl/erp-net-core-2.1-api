using CorarlERP.Addresses;
using CorarlERP.Exchanges.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.VendorCredit.Dto
{
    public class CreateVendorCreditInput
    {
        public Guid? ItemReceiptPurchaseId { get; set; }
        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid? ItemIssueVendorCreditId { get; set; }

        public TransactionStatus Status { get; set; }

        public Guid VendorId { get; set; }

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

        public string VendorCreditNo { get; set; }

        public string Memo { get; set; }

        public DateTime CreditDate { get; set; }
        public DateTime DueDate { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public List<VendorCreditDetailInput> VendorCreditDetail { get; set; }
        public DateTime? IssueDate { get; set; }
        public bool convertToItemIssueVendor { get; set; }

        public long? MultiCurrencyId { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyTax { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }

    }
}
