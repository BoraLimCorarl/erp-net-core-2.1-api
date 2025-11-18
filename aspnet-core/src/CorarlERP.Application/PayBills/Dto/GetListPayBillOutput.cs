using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.PayBills.Dto
{
    [AutoMapFrom(typeof(PayBill))]
    public class GetListPayBillOutput
    {
        public Guid Id { get; set; }        
        public string Memo { get; set; }
        public string JournalNo { get; set; }
        public long? CreationTimeIndex { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public ReceiveFromPayBill ReceiveFrom { get; set; }
        public TransactionStatus Status { get; set; }
        public string AccountName { get; set; }
        public LocationSummaryOutput Location { get; set; }
        public Guid AccountId { get; set; }
        public List<VendorSummaryOutput> VendorList { get; set; }
        public UserDto User { get; set; }
        public decimal TotalVendorCreditPayment { get; set; }
        public decimal TotalPaymentBill { get; set; }
        public string Reference { get; set; }
        public bool IsDelete { get; set; }
    }

    [AutoMapFrom(typeof(PayBill))]
    public class GetListPayBillHistoryOutput
    {
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal MultiCurrencyTotalPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public TransactionStatus Status { get; set; }
        public string Type { get; set; }
        public string AccountName { get; set; }
        public string Reference { get; set; }
    }

    public class PayBillHeader
    {
        public List<BalanceSummaryPayBill> BalanceSummary { get; set; }
        public List<GetListPayBillOutput> PayBillList { get; set; }
    }

    public class BalanceSummaryPayBill {
        public decimal TotalPament {get;set;}
        public string AccountName { get; set; }
    }

}
