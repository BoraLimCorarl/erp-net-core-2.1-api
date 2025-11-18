using CorarlERP.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceipts.ItemReceipt;

namespace CorarlERP.ItemReceiptTransfers.Dto
{
   public class CreateItemReceiptTransferInput
    {
       
        public Guid ClearanceAccountId { get; set; }

        public long LocationId { get; set; }
        public Guid? TransferOrderId { get; set; }
        public ReceiveFromStatus ReceiveFrom { get; set; }
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

        public List<CreateOrUpdateItemReceiptItemTransferInput> Items { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
