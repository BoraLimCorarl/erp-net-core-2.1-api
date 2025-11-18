using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.TransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssues.Dto
{
    [AutoMapFrom(typeof(ItemIssue))]
   public class ItemIssueDetailOutput
    {
        public Guid Id { get; set; }
        public string IssueNo { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public ReceiveFrom ReceiveFrom { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public Guid? InvoiceId { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public string Memo { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public long? MulitCurrencyId { get; set; }
        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }
        public bool ConvertToInvoice { get; set; }

        public decimal Total { get; set; }

        public List<ItemIssueItemDetailOutput> ItemIssueItems { get; set; }
        
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public long? TransactionTypeSaleId { get; set; }
        public TransactionTypeSummaryOutput TransactionTypeSale { get; set; }
        public string TransactionTypeName { get; set; }
    }

    [AutoMapFrom(typeof(ItemIssue))]
    public class ItemIssueSummaryOutput
    {
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public string ItemIssueNo { get; set; }
        public string InvoiceNo { get; set; }
        public int CountItems { get; set; }
        public decimal Total { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public string Reference { get; set; }
        public long? CreationIndex { get; set; }
        public Guid? ItemIssueId { get; set; }
        public Guid? InvoiceId { get; set; }

        public long MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }
    }

    [AutoMapFrom(typeof(ItemIssue))]
    public class ItemIssueSummaryOutputForItemIssueItem
    {
        public long? TransactionSaleTypeId { get; set; }
        public TransactionTypeSummaryOutput TransactionSaleType { get; set; }
        public Guid Id { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public ReceiveFrom ReceiveFrom { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryDetailOutput Customer { get; set; }
        public Guid? InvoiceId { get; set; }
       
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

        public string IssueNo { get; set; }

        public List<ItemIssueItemDetailOutput> ItemIssueItems { get; set; }
       
    }

    public class ItemIssueForCustomerCreditOutput
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid ItemIssueId { get; set; }
        public string IssueNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }

        public Guid? OrderId { get; set; }
        public string OrderNo { get; set; }
        public string OrderReference { get; set; }
        public Guid? BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public bool UseBatchNo { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public Guid? DeliveryId { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryReference { get; set; }
    }


    public class ItemInvoiceForCustomerCreditOutput
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid ItemIssueId { get; set; }
        public string InvoiceNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }
        public decimal Price { get; set; }
        public Guid InvoiceId { get; set; }
    }

}
