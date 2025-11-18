using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.POS.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerCredits.Dto
{
    [AutoMapFrom(typeof(CustomerCredit))]
    public class CustomerCreditDetailOutput
    {

        public string IssueNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string ReceiptCustomerCreditNo {get;set;}
        public Guid? IssueSaleId { get; set; }
        public Guid? ReceiptCustomerCreditId { get; set; }

        public DateTime ReceiptDate { get; set; }
        public string CreditNo { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus Status { get; set; }
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
        public DateTime DueDate { get; set; }
        public DateTime CreditDate { get; set; }
        public string Reference { get; set; }
        public string ItemReceiptReference { get; set; }

        public ReceiveFrom ReceiveFrom { get; set; }
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

        public List<CustomerCreditDetailInput> ItemDetail { get; set; }

        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public bool ConvertToItemReceipt { get; set; }

        public List<POSPaymentSummaryByPaymentMethodOutPut> PaymentSummaries { get; set; }
        public decimal Charge { get; set; }
        public string CreateUser { get; set; }
        public string PaidStatusName { get; set; }
        public Guid? OrderId { get; set; }
        public string OrderNo { get; set; }
        public string OrderRef { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }

    }

    [AutoMapFrom(typeof(CustomerCredit))]
    public class CustomerCreditSummaryDetailOutput
    {
        public Guid Id { get; set; }
        public string CreditNo { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal MultiCurrencyOpenBalance { get; set; }

    }

    [AutoMapFrom(typeof(CustomerCredit))]
    public class CustomerCreditSummaryOutput
    {
        public Guid Id { get; set; }
        public string CreditNo { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public int TotalItem { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public string Memo { get; set; }
        
        public CurrencyDetailOutput Currency { get; set; }
        public long? CreationTimeIndex { get; set; }
        public string LocationName { get; set; }
    }
    public class CustomerCreditPaymentSummary
    {
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }        
        public CurrencyDetailOutput MultiCurrency { get; set; }
        public long? CreationTimeIndex { get; set; }
        public string Reference { get; set; }
    }

    public class CustomerCreditFromItemReceiptCustomerCreditOutput
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid CustomerCreditId { get; set; }
        public string CustomerCreditNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }
    }
}
