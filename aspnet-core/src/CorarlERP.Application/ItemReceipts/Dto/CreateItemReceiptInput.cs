using CorarlERP.Addresses;
using CorarlERP.Journals.Dto;
using CorarlERP.PurchaseOrders.Dto;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceipts.ItemReceipt;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.ItemReceipts.Dto
{
   public class CreateItemReceiptInput
    {
       
        public ReceiveFromStatus ReceiveFrom { get; set; }

        public Guid? BillId { get; set; }

        public Guid VendorId { get; set; }   
       
        public Guid ClearanceAccountId { get; set; }

        public long LocationId { get; set; }

        public bool SameAsShippingAddress { get;  set; }

        public CAddress BillingAddress { get;  set; }

        public CAddress ShippingAddress { get;  set; }
   
        public decimal Total { get;  set; }

        public string Reference { get; set; }

        public string ReceiptNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public TransactionStatus Status { get; set; }

        public List<CreateOrUpdateItemReceiptItemInput> Items { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
