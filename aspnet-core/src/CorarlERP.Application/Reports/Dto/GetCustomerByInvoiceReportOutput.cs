using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    //public class GetCustomerByInvoiceReportOutput
    //{
    //    public DateTime Date { get; set; }
    //    public string JournalNo { get; set; }
    //    public JournalType JournalType { get; set; }
    //    public string CustomerName { get; set; }
    //    public Guid CustomerId { get; set; }
    //    public string CustomerCode { get; set; }
    //    public string AccountName { get; set; }
    //    public string AccountCode { get; set; }
    //    public string Description { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public decimal TotalPaid { get; set; }
    //    public decimal Balance { get; set; }
    //    public DateTime? LastPaymentDate { get; set; }
    //    public DateTime ToDate { get; set; }

    //    public int Aging { get => LastPaymentDate != null ? Convert.ToInt16(Math.Ceiling((ToDate - LastPaymentDate.Value).TotalDays)) : 0; }
    //    public decimal LastPaymentAmount { get; set; }

    //}

    //public class ReceivePaymentDetailOutput
    //{
    //    public Guid? TransactionId { get; set; }
    //    public Guid CustomerId { get; set; }
    //    public DateTime PaymentDate { get; set; }
    //    public decimal PaymentAmount { get; set; }
    //}


    //public class GetListCustomerByInvoiceReportGroupByOutput
    //{
    //    public string KeyName { get; set; }
    //    public List<GetCustomerByInvoiceReportOutput> Items { get; set; }
    //}
}
