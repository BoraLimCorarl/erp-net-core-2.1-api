using Abp.Net.Mail;
using CorarlERP.Common.Dto;
using CorarlERP.Inventories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetListSaleInvoiceReportOutput
    {

        public long? CreationTimeIndex { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public long? CustomerTypeId { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public Guid AccountId { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal UnitPrice { get; set; }

        //public string PaymentDescription {get;set;}

        public decimal Total { get; set; }
        public decimal Qty { get; set; }

        public string TaxName { get; set; }
        public long TaxId { get; set; }
        public decimal TaxRate { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public string User { get; set; }
        public string SaleType { get; set; }
        public decimal NetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public List<CurrencyColumnTotal> CurrencyColumnTotals { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal InvoiceTotal { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public decimal ReceivePayment { get; set; }
        public decimal TotalReceive { get; set; }
        public long? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public JournalType JournalType { get; set; }
        public bool IsItem { get; set; }
        public bool IsPayment { get; set; }
        public string Description { get; set; }
        public string OrderNo { get; set; }
        public string OrderReference { get; set; }
    }

    public class GetListSaleInvoiceDetailReportOutput: GetListSaleInvoiceReportOutput
    { 
        public decimal PaidCash { get; set; }
        public decimal PaidBank { get; set; }
        public decimal PaidCredit { get; set; }        
        public decimal PaidOther { get; set; }

        public decimal ReceiveCash { get; set; }
        public decimal ReceiveBank { get; set; }
        public decimal ReceiveCredit { get; set; }
        public decimal ReceiveOther { get; set; }

        public decimal TotalReceiveCash { get; set; }
        public decimal TotalReceiveBank { get; set; }
        public decimal TotalReceiveCredit { get; set; }
        public decimal TotalReceiveOther { get; set; }
    }

        public class GetListSaleReturnReportOutput
    {

        public long? CreationTimeIndex { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public Guid AccountId { get; set; }
        public string ItemName { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemCode { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }
        public decimal Qty { get; set; }

        public string TaxName { get; set; }
        public long TaxId { get; set; }
        public decimal TaxRate { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public string User { get; set; }
        public string SaleType { get; set; }
        public decimal NetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public List<CurrencyColumnTotal> CurrencyColumnTotals { get; set; }
    }


    public class CurrencyColumnTotal

    {
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }

        public decimal PaidCash { get; set; }
        public decimal PaidBank { get; set; }
        public decimal PaidCredit { get; set; }
        public decimal PaidOther { get; set; }

        public decimal Balance { get; set; }
        public decimal ReceivePayment { get; set; }

        public decimal ReceiveCash { get; set; }
        public decimal ReceiveBank { get; set; }
        public decimal ReceiveCredit { get; set; }
        public decimal ReceiveOther { get; set; }

        public decimal TotalReceive { get; set; }

        public decimal TotalReceiveCash { get; set; }
        public decimal TotalReceiveBank { get; set; }
        public decimal TotalReceiveCredit { get; set; }
        public decimal TotalReceiveOther { get; set; }
    }


    public class SaleSummaryOut 
    {
        public Guid InvoiceId { get; set; }
        public string JournalNo { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal PaidCash { get; set; }
        public decimal PaidBank { get; set; }
        public decimal PaidCredit { get; set; }
        public decimal PaidOther { get; set; }

        public decimal Balance { get; set; }
        public List<CurrencyColumnTotal> CurrencyColumnTotals { get; set; }
        public decimal Qty { get; set; }
        public decimal ReceivePayment { get; set; }
        public decimal TotalReceive { get; set; }
        public decimal NetWeight { get; set; }

        public decimal ReceiveCash { get; set; }
        public decimal ReceiveBank { get; set; }
        public decimal ReceiveCredit { get; set; }
        public decimal ReceiveOther { get; set; }

        public decimal TotalReceiveCash { get; set; }
        public decimal TotalReceiveBank { get; set; }
        public decimal TotalReceiveCredit { get; set; }
        public decimal TotalReceiveOther { get; set; }

    }

    public class GetListSaleInvoiceByItemPropertyReportOutput
    {

        public long? CreationTimeIndex { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public long? CustomerTypeId { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public Guid? ItemId { get; set; }
        //public string ItemName { get; set; }
        //public string ItemCode { get; set; }

        //public Guid? AccountId { get; set; }
        //public string AccountName { get; set; }
        //public string AccountCode { get; set; }

        public decimal Total { get; set; }
        public decimal Qty { get; set; }

        public int RoundingDigit { get; set; }
        public string User { get; set; }
        public string SaleType { get; set; }
        public decimal NetWeight { get; set; }
        public string SummaryBy { get; set; }
        //public List<ItemPropertySummary> Properties { get; set; }
        public Guid InvoiceId { get; set; }
        //public Guid InvoiceItemId { get; set; }
        public bool IsItem { get; set; }
        public string Description { get; set; }
    }

    public class SaleInvoiceByItemPropertyColumnSummaryOutPut
    {
        public decimal TotalQty { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
