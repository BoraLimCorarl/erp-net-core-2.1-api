using Abp.Auditing;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Exchanges.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.PayBills.Dto
{
    public class CreatePayBillInput
    {
        public string PaymentNo { get; set; }
        public DateTime paymentDate { get; set; }
        public Guid? PaymentAccountId { get; set; }
        public TransactionStatus Status { get; set; }
        public string Reference { get; set; }
        public long? ClassId { get; set; }
        public long CurrencyId { get; set; }
        public bool FiFo { get; set; }
        public decimal TotalPayment {get;set;}
        public decimal TotalPaymentVendorCredit {get;set;}
        public decimal TotalPaymentBill {get;set;}
        public decimal TotalOpenBalance { get; set; }
        public decimal TotalOpenBalanceVendorCredit { get; set; }
        public decimal TotalPaymentDue { get; set; }
        public decimal TotalPaymentDueVendorCredit { get; set; }
        //public Guid? VendorCreditId { get; set; }
        public string Memo { get; set; }
        public ReceiveFromPayBill ReceiveFrom { get; set; }
        public long? LocationId { get; set; }
        [DisableAuditing]
        public List<PayBillDetail> PayBillDetail { get; set; }
        [DisableAuditing]
        public List<PayBillExpenseItem> PayBillExpenseItem { get; set; }

        public decimal MultiCurrencyTotalOpenBalance { get; set; }
        public decimal MultiCurrencyTotalPaymentDue { get; set; } 
        public decimal MultiCurrencyTotalOpenBalanceVendorCredit { get; set; }
        public decimal MultiCurrencyTotalPaymentDueVendorCredit { get; set; }

        public decimal MultiCurrencyTotalPayment { get; set; }
        public decimal MultiCurrencyTotalPaymentBill { get; set; }
        public decimal MultiCurrencyTotalPaymentVendorCredit { get; set; }
        public decimal Change { get; set; }
        public decimal MultiCurrencyChange { get; set; }
        public long? MultiCurrencyId { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

        public decimal TotalCashBill { get; set; }
        public decimal TotalCreditBill { get; set; }
        public decimal TotalExpenseBill { get; set; }
        public decimal TotalCashVendorCredit { get; set; }
        public decimal TotalCreditVendorCredit { get; set; }
        public decimal TotalExpenseVendorCredit { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public decimal TotalLossGainBill { get; set; }
        public decimal TotalLossGainVendorCredit { get; set; }
        public ExchangeLossGainDto ExchangeLossGain { get; set; }
    }

    public class PayBillExpenseItem
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public decimal MultiCurrencyAmount { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsLossGain { get; set; }
    }

    public class ExchangeLossGainDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
