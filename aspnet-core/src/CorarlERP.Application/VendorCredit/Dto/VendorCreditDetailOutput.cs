using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Bills.Bill;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.VendorCredit.Dto
{

    [AutoMapFrom(typeof(VendorCredit))]
    public class VendorCreditDetailOutput
    {


        public ReceiveFrom ReceiveFrom { get; set; }
        public string ItemReceiptPurchaseNo { get; set; }
        public Guid? ItemReceiptPurchaseId { get; set; }
        public string ItemIsseVendorCreditNo { get; set; }
        public Guid? ItemIssueVendorCreditId { get; set; }

        public string CreditNo { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus Status { get; set; }
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public string Memo { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreditDate { get; set; }
        public string Reference { get; set; }
        public string ItemIssueReference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public List<VendorCreditDetailInput> ItemDetail { get; set; }
        
        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }

        public DateTime? IssueDate { get; set; }

        public bool ConvertToItemIssueVendor { get;  set; }

        public decimal MultiCurrencySubTotal { get; set; }

        public decimal MultiCurrencyTax { get; set; }

        public decimal MultiCurrencyTotal { get; set; }

        public CurrencyDetailOutput MultiCurrency { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }

        public decimal MultiCurrencyTotalPaid { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }

    [AutoMapFrom(typeof(VendorCredit))]
    public class VendorCreditSummaryDetailOutput
    {
        public Guid Id { get; set; }
        public string CreditNo { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal MultiCurrencyOpenBalance { get; set; }
    }


    [AutoMapFrom(typeof(VendorCredit))]
    public class VendorCreditSummaryOutput
    {
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int TotalItem { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        public string Memo { get; set; }
        public string LocationName { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public long? CreationTimeIndex { get; set; }
        public decimal TotalPaid { get;  set; }
        public decimal MultiCurrencyTotalPaid { get;  set; }
        public CurrencyDetailOutput MultiCurrency { get;  set; }
        public string VendorNo { get; set; }
        public string Reference { get; set; }
    }

    public class GetListVendorCreditForItemIssueVendorCredit
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid VendorCreditId { get; set; }
        public string IssueCreditNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }
    }
}
