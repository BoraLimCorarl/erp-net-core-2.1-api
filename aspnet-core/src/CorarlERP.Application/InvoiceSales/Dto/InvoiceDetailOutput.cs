using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.TransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Invoices.Dto
{
    [AutoMapFrom(typeof(Invoice))]
    public class InvoiceDetailOutput
    {
        public decimal OriginalQtyFromSaleOrder { get; set; }
        public decimal OriginalQtyFromDeliverySchedule { get; set; }
        public Guid? ItemIssueId { get; set; }
        public ReceiveFrom ReceiveFrom { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }
        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ETD { get; set; }
        public string Reference { get; set; }
        public string ItemIssueReference { get; set; }
        public CAddress ShippingAddress { get; set; }
        public CAddress BillingAddress { get; set; }
        public bool SameAsShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNo { get; set; }
        public List<CreateOrUpdateInvoiceItemInput> InvoiceItems { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }
        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public bool ConvertToItemIssue { get; set; }
        public string ItemIssueNo { get; set; }
        public DateTime? ItemIssueDate { get; set; }
        public TransactionTypeSummaryOutput TransactionTypeSale { get; set; }
        public long? TrasactionTypeSaleId { get; set; }

        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }
        public decimal MultiCurrencyOpenBalance { get; set; }
        public decimal MultiCurrencyTotalPaid { get; set; }
        public long? CreationTimeIndex { get; set; }
        public string UserName { get;set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }

   
    [AutoMapFrom(typeof(Invoice))]
    public class InvoiceSummaryOutput
    {
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }

        public Guid Id { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }
       
        public DateTime ETD { get; set; }

        public string InvoiceNo { get; set; }

        public string Reference { get; set; }

        public decimal Total { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public int CountItems { get; set; }
        public long? CreationIndex { get; internal set; }
        //public List<BillItemSummaryOutput> BillItems { get; set; }
    }


    [AutoMapFrom(typeof(Invoice))]
    public class InvoiceSummaryOutputForGetInvoiceItem
    {
        public long? TransactionSaleTypeId { get; set; }
        public TransactionTypeSummaryOutput TransactionSaleType { get; set; }
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDetailOutput Customer { get; set; }

        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

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
        public List<InvoiceItemSummaryOutput> InvoiceItems { get; set; }
    }

    [AutoMapFrom(typeof(InvoiceItem))]
    public class InvoiceItemSummaryOutput
    {

        public Guid? SaleOrderId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }
        public Guid? OrderItemId { get; set; }
        public Guid? ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public string Description { get; set; }

        public long TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal Total { get; set; }

        public Guid? ItemReceiptItemId { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public long? LotId { get; set; }
        public LotSummaryOutput LotDetail { get; set; }

        public decimal DiscountRate { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }

    [AutoMapFrom(typeof(Invoice))]
    public class InvoiceSummaryforReceivePayment
    {
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal MultiCurrencyTotalPaid { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }
        public string Reference { get; set; }
    }
}
