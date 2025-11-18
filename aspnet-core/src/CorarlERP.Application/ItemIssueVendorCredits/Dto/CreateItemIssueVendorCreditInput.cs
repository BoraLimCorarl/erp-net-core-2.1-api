using CorarlERP.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemIssueVendorCredits.ItemIssueVendorCredit;

namespace CorarlERP.ItemIssueVendorCredits.Dto
{
    public class CreateItemIssueVendorCreditInput
    {
        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid? VendorCreditId { get; set; }

        public Guid VendorId { get; set; }

        public Guid? ItemReceiptPurchaseId { get; set; }
       // public Guid AccountId { get; set; }

        public long LocationId { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public CAddress ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public string Reference { get; set; }

        public string IssueNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public TransactionStatus Status { get; set; }

        public List<CreateOrUpdateItemIssueVendorCreditItemInput> Items { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
