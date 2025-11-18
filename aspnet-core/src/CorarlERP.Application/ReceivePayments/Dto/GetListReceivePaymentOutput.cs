using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReceivePayments.Dto
{
    
    public class ReceivePaymentHeader
    {
       public List<GetListReceivPaymentOutput> ReceivePaymentList { get; set; }
        public List<BalanceSummaryReceivPayment>BalanceSummaryReceicePayment { get; set; }
    }
    public class BalanceSummaryReceivPayment
    {
        public decimal TotalPament { get; set; }
        public string AccountName { get; set; }
    }
    [AutoMapFrom(typeof(ReceivePayment))]
    public class GetListReceivPaymentOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public LocationSummaryOutput Location { get; set; }
        public string AccountName { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public ReceiveFromRecievePayment ReceiveFrom { get; set; }
        public TransactionStatus Status { get; set; }
        public Guid AccountId { get; set; }
        public UserDto User { get; set; }
        public List<CustomerSummaryOutput> CustomerLists { get; set; }
        public decimal TotalCustomerCreditPayment { get; set; }
        public decimal TotalPaymentInvoice { get; set; }
        public bool IsDelete { get; set; }
    }

    [AutoMapFrom(typeof(ReceivePayment))]
    public class GetListReivePaymentHistoryOutput
    {
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public string AccountName { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public TransactionStatus Status { get; set; }
        public string Type { get; set; }
        public decimal MultiCurrencyTotalPayment { get; set; }
        public string Reference { get; set; }
    }

}
