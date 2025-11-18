using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemReceiptTransfers.Dto
{
    [AutoMapFrom(typeof(ItemReceipt))]
    public class ItemReceiptTransferDetailOutput
    {
        public Guid Id { get; set; }
        public string StatusName { get; set; }
        public ItemReceipt.ReceiveFromStatus ReceiveFrom { get; set; }

        public Guid? TransferOrderId { get; set; }

        public TransactionStatus StatusCode { get; set; }
       
        public Guid ClearanceAccountId { get; set; }
        public ChartAccountSummaryOutput ClearanceAccount { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public string Memo { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public decimal Total { get; set; }

        public List<ItemReceiptItemTransferDetailOutput> ItemReceiptItemTransfers { get; set; }

        public string ReceiveNo { get; set; }

        public string  TransactionTypeName { get; set; }


    }
}
