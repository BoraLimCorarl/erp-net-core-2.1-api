using Abp.Auditing;
using CorarlERP.Addresses;
using CorarlERP.Exchanges.Dto;
using CorarlERP.PermissionLocks.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Bills.Bill;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Bills.Dto
{
   public class CreateBillInput
    {
        public ReceiveFromStatus ReceiveFrom { get; set; }

        public TransactionStatus Status { get; set; }

        public Guid VendorId { get; set; }

        public Guid ClearanceAccountId { get; set; }

        public long LocationId { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public CAddress ShippingAddress { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public string Reference { get; set; }
        public string ItemReceiptReference { get; set; }

        public string BillNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime ETA { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }
        public long? MultiCurrencyId { get; set; }
        public Guid? ItemReceiptId { get; set; }

        [DisableAuditing]
        public List<CreateOrUpdateBillItemInput> BillItems { get; set; }

        public bool ConvertToItemReceipt { get; set; }
        public DateTime RecieptDate { get; set; }

        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyTax { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }
}
