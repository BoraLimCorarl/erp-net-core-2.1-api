using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.PurchaseOrders.PurchaseOrder;

namespace CorarlERP.PurchaseOrders.Dto
{
    [AutoMapFrom(typeof(PurchaseOrder))]
    public class PurchaseOrderDetailOutput
    {
        public Guid Id { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Reference { get; set; }
        public TransactionStatus Status { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public string StatusCode { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public long? MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }

        public bool SameAsShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Memo { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public bool IsActive { get; set; }
        public DateTime ETA { get; set; }
        public string LocationName {get;set;}
        public long? LocationId { get; set; }
        public List<PurchaseOrderItemDetailOut> PurchaseOrderItems { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }

    [AutoMapFrom(typeof(PurchaseOrder))]
    public class PurchaserOrderSummaryOutput
    {
        public string Memo { get; set; }
        public Guid Id { get; set; }       
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }             
        public long CurrencyId { get; set; }
        public string Reference { get; set; }
        public CurrencyDetailOutput Currency { get; set; }                   
        public decimal Total { get; set; }  
        public decimal MultiCurrencyTotal { get; set; }
        public int CountPurchaseOrderItems { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        public DateTime ETA { get; set; }
    }
    [AutoMapFrom(typeof(PurchaseOrder))]
    public class GetlistPuchaseOrderItemDetail
    {

        public long LocationId { get; set; }
        public Guid Id { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Remain { get; set; }
        public decimal Total { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public string Memo { get; set; }
        public bool SameAsShippingAddress { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryDetailOutput Vendor { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public long? MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }

        public List<PurchaseOrderItemSummaryOut> PurchaseOrderItems { get; set; }
        public string Reference { get; set; }
    }

    [AutoMapFrom(typeof(PurchaseOrder))]
    public class PurchaserOrderSummaryDetailOutput
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }             
    }

    public class PurchaseOrderItemFroBillDto
    {
        public DateTime Date { get; set; } 
        public Guid? Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
        public decimal UnitCost { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal RemainQty { get; set; }
        public string LocationName { get; set; }

    }


    public class PurchaseOrderBillDetailDto
    {
        public DateTime ReceiveDate { get; set; }
        public Guid ReceiveId { get; set; }
        public string ReceiveNo { get; set; }
        public string ReceiveReference { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }

        public Guid? BillId { get; set; }
        public string BillNo { get; set; }
        public string BillReference { get; set; }
        public PaidStatuse? PaidStatus { get; set; }
    }

}
