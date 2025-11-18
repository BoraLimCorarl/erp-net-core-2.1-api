using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemIssueVendorCredits.ItemIssueVendorCredit;

namespace CorarlERP.ItemIssueVendorCredits.Dto
{
    [AutoMapFrom(typeof(ItemIssueVendorCredit))]
    public class ItemIssueVendorCreditDetailOutput
    {
        public Guid Id { get; set; }

        public Guid? ItemReceiptPurchaseId { get; set; }
        public string ItemReceiptPurchaseNo { get; set; }
        public DateTime ItemReceiptPurchaseDate { get; set; }

        public Guid? VendorCreditId { get; set; }
        public string VendorCreditNo { get; set; }
        public DateTime VendorCreditDate { get; set; }

        public string StatusName { get; set; }
        public string IssueNo { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid VendorId { get; set; }
        public VendorSummaryDetailOutput Vendor { get; set; }
        
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

        public List<CreateOrUpdateItemIssueVendorCreditItemInput> Items { get; set; }
        public string TransactionTypeName { get; set; }
    }

    [AutoMapFrom(typeof(ItemIssueVendorCredit))]
    public class ItemIssueVendorCreditSummaryOutput
    {
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public string ItemIssueNo { get; set; }
        public int CountItems { get; set; }
        public decimal Total { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public string Reference { get; set; }
        public string LocationName { get; set; }
    }

    [AutoMapFrom(typeof(ItemIssueVendorCreditItem))]
    public class ItemIssueVendorCreditItemFromVendorCreditOutput
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid ItemIssueVendorCreditId { get; set; }
        public string IssueCreditNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }
        public DateTime ItemDateTime { get; set; }
    }
}
