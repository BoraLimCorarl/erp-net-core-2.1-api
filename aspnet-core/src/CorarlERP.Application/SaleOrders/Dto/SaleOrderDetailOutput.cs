using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.TransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders.Dto
{
    [AutoMapFrom(typeof(SaleOrder))]
    public class SaleOrderDetailOutput
    {
        public Guid Id { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Reference { get; set; }
        public TransactionStatus Status { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }
        public string StatusCode { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public long? MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }

        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }

        public bool SameAsShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }
        public DateTime ETD { get; set; }
        public List<CreateOrUpdateSaleOrderItemInput> SaleOrderItems { get; set; }
        public long? SaleTransactionTypeId { get; set; }
        public TransactionTypeSummaryOutput SaleTransactionType { get; set; }
        public string Location {get;set;}
        public long? LocationId { get; set; }
        public string UserName { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }

    [AutoMapFrom(typeof(SaleOrder))]
    public class SaleOrderSummaryOutput
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string StatusCode { get; set; }
        public DateTime OrderDate { get; set; }
    }



    [AutoMapFrom(typeof(SaleOrder))]
    public class SaleOrderHeaderOutput
    {
        public string Memo { get; set; }
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public long CurrencyId { get; set; }
        public string Reference { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public decimal Total { get; set; }
        public int CountSaleOrderItems { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public DateTime ETD { get; set; }
        public long? SaleTransactionTypeId { get; set; }
        public TransactionTypeSummaryOutput SaleTransactionType { get; set; }
    }

    [AutoMapFrom(typeof(SaleOrder))]
    public class GetListSaleOrderItemDetail
    {
        public long LocationId { get; set; }
        public Guid Id { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Remain { get; set; }
        public decimal Total { get; set; }
        public string Memo { get; set; }
        public bool SameAsShippingAddress { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryDetailOutput Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public long CurrencyId { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public List<SaleOrderItemSummaryOut> SaleOrderItems { get; set; }
        public string Reference { get; set; }
        public long? SaleTransactionTypeId { get; set; }
        public TransactionTypeSummaryOutput SaleTransactionType { get; set; }

        public long? MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }
    }


    [AutoMapFrom(typeof(SaleOrderItem))]
    public class SaleOrderItemSummaryOut
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }       
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
        public decimal UnitCost { get; set; }

        public decimal MultiCurrencyUnitCost { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal DiscountRate { get; set; }       
        public TaxDetailOutput Tax { get; set; }
        public decimal Remain
        {
            get;set;
        }

        public long TaxId { get; set; }
        public string Description { get; set; }
        public decimal TaxRate { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }


    public class SaleOrderItemFroInvoiceDto
    {
        public DateTime Date { get; set; }
        public Guid? Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }
        public Guid ItemId { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
        public decimal UnitCost { get; set; }
        public string CurrencyCode { get; set; }
        public decimal IssueQty { get; set; }
        public decimal RemainQty { get; set; }
        public string LocationName { get; set; }
    }

    public class SaleOrderInvoiceDetailDto
    {
        public DateTime IssueDate { get; set; }
        public Guid IssueId { get; set; }
        public string IssueNo { get; set; }
        public string IssueReference { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }

        public Guid? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceReference { get; set; }
        public PaidStatuse? PaidStatus { get; set; }
    }
}
