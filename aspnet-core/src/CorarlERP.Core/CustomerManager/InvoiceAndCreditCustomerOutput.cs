using CorarlERP.VendorHelpers.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerManager
{
    public class InvoiceAndCreditCustomerOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid InvoiceOrCreditId { get; set; }
        public Guid JournalId { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public DateTime JournalDate { get; set; }
        public JournalType JournalType { get; set; }
        public string JournalMemo { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public DateTime Date { get; set; }
        public decimal CreditBalanceAmount { get; set; }
        public decimal CreditTotalAmount { get; set; }
        public decimal CreditTotalPaidAmount { get; set; }
        public decimal InvoiceBalanceAmount { get; set; }
        public decimal InvoiceTotalAmount { get; set; }
        public decimal InvoiceTotalPaidAmount { get; set; }
        public DateTime ToDate { get; set; }
        public string User { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public CurrencyOutput Currency { get; set; }

        public int Aging { get; set; }
        public string LocationName { get; set; }
    }

    public class GetCustomerByInvoiceReportOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public JournalType? JournalType { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string AccountName { get; set; }
        public string User { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Aging { get => LastPaymentDate != null && ToDate != null ? (int)(ToDate.Value.Date.AddDays(1).AddTicks(-1) - LastPaymentDate.Value).TotalDays : 0; }
        public decimal LastPaymentAmounts { get; set; }
        public CurrencyOutput Currency { get; set; }
        
        public List<GetCustomerByInvoiceReportOutputItemByCurrency> TotalColsByCurency { get; set; }
        public string Location { get; set; }
    }

  

    public class GetCustomerByInvoiceReportOutputItemByCurrency
    {
        public string CurrencyCode { get; set; }
        public decimal TotalAmounts { get; set; }
        public decimal Balances { get; set; }
        public decimal TotalPaids { get; set; }
        public decimal LastPaymentAmounts { get; set; }

    }


    public class ReceivePaymentDetailOutput
    {
        public long? CreationTimeIndex { get; set; }
        public string PaymentNo { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public Guid InvoiceId { get; set; }
    }


    public class GetListCustomerByInvoiceReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetCustomerByInvoiceReportOutput> Items { get; set; }
    }

    public class GetListCustomerByInvoiceWithPaymentReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetCustomerByInvoiceWithPaymentReportOutput> Items { get; set; }
    }

    public class GetCustomerByInvoiceWithPaymentReportOutput 
    {
        public List<GetCustomerByInvoiceWithPaymentReportOutput> PaymentItems { get; set; }

        public long? CreationTimeIndex { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public JournalType? JournalType { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public CurrencyOutput Currency { get; set; }
        public string Reference { get; set; }

        public List<GetCustomerByInvoiceReportOutputItemByCurrency> TotalColsByCurency { get; set; }
        public string Location { get; set; }

        public string PaymentNo { get; set; }
        public string PaymentReference { get; set; }
        public long? PaymentCreationTimeIndex { get; set; }
        public DateTime? PaymentDate { get; set; }
        public Guid? PaymentId { get; set; }
        public string PaymentUserName { get; set; }
      
    }
}
