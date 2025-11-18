using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorHelpers.Data
{
    public class BillAndCreditVendorOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid BillOrCreditId { get; set; }
        public Guid JournalId { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public DateTime JournalDate { get; set; }
        public JournalType JournalType { get; set; }
        public string JournalMemo { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public long? VendorTypeId { get; set; }
        public DateTime Date { get; set; }
        public decimal CreditBalanceAmount { get; set; }
        public decimal CreditTotalAmount { get; set; }
        public decimal CreditTotalPaidAmount { get; set; }
        public decimal BillBalanceAmount { get; set; }
        public decimal BillTotalAmount { get; set; }
        public decimal BillTotalPaidAmount { get; set; }
        public DateTime ToDate { get; set; }
        public UserDto User { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public CurrencyOutput Currency { get; set; }

        public int Aging { get; set; }
        public string LocationName { get; set; }
    }

    public class CurrencyOutput
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

    }

    public class GetVendorByBillReportOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public JournalType? JournalType { get; set; }
        public string VendorName { get; set; }
        public Guid VendorId { get; set; }
        public string VendorCode { get; set; }
        public long? VendorTypeId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? ToDate { get; set; }
        public UserDto User { get; set; }
        public int Aging { get => LastPaymentDate != null && ToDate != null ? (int) (ToDate.Value.Date.AddDays(1).AddTicks(-1) - LastPaymentDate.Value).TotalDays : 0; }
        public decimal LastPaymentAmounts { get; set; }
        public CurrencyOutput Currency { get; set; }
        public string Location { get; set; }
        //public List<string> CurrencyCodes { get; set; }

        //public Dictionary<string, decimal> TotalAmounts { get; set; }
        //public Dictionary<string, decimal> TotalPaids { get; set; }
        //public Dictionary<string, decimal> Balances { get; set; }
        //public Dictionary<string, decimal> LastPaymentAmounts { get; set; }

        public List<GetVendorByBillReportOutputItemByCurrency> TotalColsByCurency { get; set; }
       
    }

   
    public class GetVendorByBillReportOutputItemByCurrency
    {
        public string CurrencyCode { get; set; }
        public decimal TotalAmounts { get; set; }
        public decimal Balances { get; set; }
        public decimal TotalPaids { get; set; }
        public decimal LastPaymentAmounts { get; set; }

    }


    public class BillPaymentDetailOutput
    {
        public Guid BillId { get; set; }
        public long? CreationTimeIndex { get; set; }
        //public Guid TransactionId { get; set; }
        public Guid VendorId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public CurrencyOutput Currency { get; set; }       
    }


    public class GetListVendorByBillReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetVendorByBillReportOutput> Items { get; set; }
    }
}
