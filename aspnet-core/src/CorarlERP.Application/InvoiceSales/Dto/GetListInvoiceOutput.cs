using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Invoices.Dto
{
    public class InvoiceHeader
    {
        public List<BalanceSummary> BalanceSummary { get; set; }
        public List<GetListInvoiceOutput> InvoiceList { get; set; }
    }

    [AutoMapFrom(typeof(Invoice))]
    public class GetListInvoiceOutput
    {
        public bool IsCanVoidOrDraftOrClose { get; set; }

        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public string TypeName { get; set; }
        public JournalType TypeCode { get; set; }

        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }      
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public decimal Total { get; set; }

        public long? CreationTimeIndex { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public TransactionStatus Status { get; set; }

        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }
        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public string LocationName { get; set; }
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }

        public UserDto User { get; set; }
        public string Reference { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool IsDelete { get; set; }
    }

    [AutoMapFrom(typeof(Invoice))]
    public class getInvoiceListOutput // for receivePayment api getlist 
    {
        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }
        public decimal MultiCurrencyTotalPaid { get; set; }
        public String MultiCurrencyCode { get; set; }
        public long MultiCurrencyId { get; set; }

        public Guid AccountId { get; set; }
        public string Reference { get; set; }
    }
}
