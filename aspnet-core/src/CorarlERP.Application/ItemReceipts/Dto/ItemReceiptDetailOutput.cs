using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceipts.ItemReceipt;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.ItemReceipts.Dto
{
    [AutoMapFrom(typeof(ItemReceipt))]
    public class ItemReceiptDetailOutput
    {
        public Guid Id { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public ReceiveFromStatus ReceiveFrom { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        public Guid? BillId { get; set; }
        public Guid ClearanceAccountId { get; set; }
        public ChartAccountSummaryOutput ClearanceAccount { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public string Memo { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public long? MultiCurrencyId { get; set; }
        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public CAddress ShippingAddress { get; set; } 

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }
      
        public decimal Total { get; set; }

        public List<ItemReceiptItemDetailOutput> ItemReceiptItems { get; set; }
        public string ReceiveNo { get; set; }
       // public string PurchaseNo { get; set; }
        public string BillNo { get; set; }
        public DateTime? BillDate { get; set; }
       // public DateTime? PurchaseDate { get; set; }
       public string TransactionTypeName { get; set; }
    }

    [AutoMapFrom(typeof(ItemReceipt))]
    public class ItemReceiptSummaryOutputForItemReceiptItem
    {

        public Guid VendorId { get; set; }
        public VendorSummaryDetailOutput Vendor { get; set; }

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

        public decimal SubTotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public string ReceiveNo { get; set; }
        public Guid Id { get; set; }      
        public List<ItemReceiptItemSummaryOutput> ItemReceiptItems { get; set; }          
    }

    [AutoMapFrom(typeof(ItemReceipt))]
    public class ItemReceiptSummaryOutput
    {
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public DateTime Date { get; set; }

        public Guid Id { get; set; }

        public string ItemReceiptNo { get; set; }

      //  public List<ItemReceiptItemSummaryOutput> ItemReceiptItems { get; set; }

        public int CountItems { get; set; }
        public decimal Total { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public string Reference { get; set; }
        public long? CreationTimeIndex { get; set; }
    }


    [AutoMapFrom(typeof(ItemReceiptItem))]
    public class ItemReceiptitemFromVendorCreditOutput
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid ItemReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }
        public DateTime ItemDateTime { get; set; }
        public Guid? BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool UseBatchNo { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
