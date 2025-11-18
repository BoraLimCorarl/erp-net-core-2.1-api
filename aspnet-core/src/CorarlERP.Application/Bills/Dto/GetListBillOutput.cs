using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.Journals;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Bills.Bill;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;
using static CorarlERP.VendorCredit.VendorCredit;

namespace CorarlERP.Bills.Dto
{
    public class BillHeader
    {
        public List<BalanceSummary> BalanceSummary { get; set; }
        public List<GetListBillOutput> BillList { get; set; }
    }

    [AutoMapFrom(typeof(Bill))]
    public class GetListBillOutput
    {

        public long? CreationTimeIndex { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool IsCanVoidOrDraftOrClose { get; set; }

        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public string TypeName { get; set; }
        public JournalType TypeCode { get; set; }

        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public TransactionStatus Status { get; set; }

        public decimal TotalPaid { get; set; }
        public decimal OpenBalance { get; set; }
        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        
        public string LocationName { get; set; }
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }
        public UserDto User { get; set; }
        public string Reference { get; set; }
        public bool IsDelete { get; set; }
    }

    [AutoMapFrom(typeof(Bill))]
    public class getBillListOutput // for paybill api getlist 
    {
        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        
        public decimal Total { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        
        public decimal OpenBalance { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal MultiCurrencyTotal { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }

        public decimal MultiCurrencyTotalPaid { get; set; }

        public long? MultiCurrencyId { get; set; }
        
        public string MultiCurrencyCode { get; set; }

        public Guid AccountId { get; set; }
        public string Reference { get; set; }
    }


    [AutoMapFrom(typeof(Bill))]
    public class BillSummaryPayBillOutput
    {
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal MultiCurrencyTotalPaid { get; set; }
        public CurrencyDetailOutput MultiCurrency {get;set;}
        public string Reference { get; set; }

    }


}
