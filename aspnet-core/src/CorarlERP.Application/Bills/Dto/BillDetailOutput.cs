using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.MultiCurrencys.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Bills.Bill;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Bills.Dto
{
    [AutoMapFrom(typeof(Bill))]
    public class BillDetailOutput
    {
        public Guid? ItemReceiptId { get; set; }
        public ReceiveFromStatus ReceiveFrom { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public Guid ClearanceAccountId { get; set; }
        public ChartAccountSummaryOutput ClearanceAccount { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public string Reference { get; set; }
        public string ItemReceiptReference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }


        public decimal MultiCurrencySubTotal { get; set; }

        public decimal MultiCurrencyTax { get; set; }

        public decimal MultiCurrencyTotal { get; set; }

        public CurrencyDetailOutput MultiCurrency { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }

        public decimal MultiCurrencyTotalPaid { get; set; }

        public string BillNo { get; set; }

        public List<BillItemDetailOutput> BillItems { get; set; }

        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }

       

        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }

        public DateTime ETA { get; set; }

        public DateTime? ItemReceiptDate { get; set; }
        public string ItemReceiptNo { get; set; }
        public bool ConvertToItemReceipt { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }

    [AutoMapFrom(typeof(Bill))]
    public class BillSummaryOutputForGetBillItem
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public VendorDetailOutput Vendor { get; set; }

        public Guid ClearanceAccountId { get; set; }
        public ChartAccountSummaryOutput ClearanceAccount { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public string BillNo { get; set; }
        public List<BillItemSummaryOutput> BillItems { get; set; }
    }



    [AutoMapFrom(typeof(Bill))]
    public class BillSummaryOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public Guid Id { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public string BillNo { get; set; }

        public string Reference { get; set; }

        public decimal Total { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public int CountItems { get; set; }
        public DateTime ETA { get; set; }
        //public List<BillItemSummaryOutput> BillItems { get; set; }
    }

}
