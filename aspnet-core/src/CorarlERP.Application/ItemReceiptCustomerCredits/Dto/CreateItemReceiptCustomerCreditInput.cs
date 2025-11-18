using CorarlERP.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceiptCustomerCredits.ItemReceiptCustomerCredit;

namespace CorarlERP.ItemReceiptCustomerCredits.Dto
{
    public class CreateItemReceiptCustomerCreditInput
    {
        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid? CustomerCreditId { get; set; }

        public Guid CustomerId { get; set; }
        
      //  public Guid ClearanceAccountId { get; set; }

        public long LocationId { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public CAddress ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public string Reference { get; set; }

        public string ReceiptNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public TransactionStatus Status { get; set; }

        public Guid? ItemIssueSaleId { get; set; }

       

        public List<CreateOrUpdateItemReceiptCustomerCreditItemInput> Items { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
