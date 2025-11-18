using Abp.Auditing;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.PayBills.Dto;
using CorarlERP.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReceivePayments.Dto
{
   public class CreateReceivePaymentInput
    {
        public string PaymentNo { get; set; }
        public DateTime paymentDate { get; set; }
        public Guid? PaymentAccountId { get; set; }
        public enumStatus.EnumStatus.TransactionStatus Status { get; set; }
        public string Reference { get; set; }
        public long? ClassId { get; set; }
        public long CurrencyId { get; set; }
        public bool FiFo { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalPaymentCustomerCredit { get; set; }
        public decimal TotalPaymentInvoice { get; set; }
        public decimal TotalOpenBalance { get; set; }
        public decimal TotalOpenBalanceCustomerCredit { get; set; }
        public decimal TotalPaymentDue { get; set; }
        public decimal TotalPaymentDueCustomerCredit { get; set; }
      
        public decimal MultiCurrencyTotalPayment { get; set; }

        public decimal MultiCurrencyTotalOpenBalance { get; set; }
        public decimal MultiCurrencyTotalPaymentDue { get; set; }
        public decimal MultiCurrencyTotalOpenBalanceCustomerCredit { get; set; }
        public decimal MultiCurrencyTotalPaymentDueCustomerCredit { get; set; }

        public decimal TotalCashInvoice { get; set; }
        public decimal TotalCreditInvoice { get; set; }
        public decimal TotalExpenseInvoice { get; set; }
        public decimal TotalCashCustomerCredit { get; set; }
        public decimal TotalCreditCustomerCredit { get; set; }
        public decimal TotalExpenseCustomerCredit { get; set; }

        public decimal MultiCurrencyTotalCashInvoice { get; set; }
        public decimal MultiCurrencyTotalCreditInvoice { get; set; }
        public decimal MultiCurrencyTotalExpenseInvoice { get; set; }
        public decimal MultiCurrencyTotalCashCustomerCredit { get; set; }
        public decimal MultiCurrencyTotalCreditCustomerCredit { get; set; }
        public decimal MultiCurrencyTotalExpenseCustomerCredit { get; set; }


        public long? MultiCurrencyId { get; set; }
        public string Memo { get; set; }
        public ReceiveFromRecievePayment ReceiveFrom { get; set; }
        [DisableAuditing]
        public List<ReceivePaymentDetail> ReceivePaymentDetail { get; set; }
        [DisableAuditing]
        public List<ReceivePaymentExpenseItem> ReceivePaymentExpenseItems { get; set; }
        public long? LocationId { get; set; }
        
        public decimal MultiCurrencyTotalPaymentInvoice { get; set; }
        public decimal MultiCurrencyTotalPaymentCustomerCredit { get; set; }
        public decimal Change { get; set; }
        public decimal MultiCurrencyChange { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public decimal TotalLossGainInvoice { get; set; }
        public decimal TotalLossGainCustomerCredit { get; set; }
        public ExchangeLossGainDto ExchangeLossGain { get; set; }
    }


    public class ReceivePaymentExpenseItem
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public decimal Amount { get; set; }
        public decimal MultiCurrencyAmount { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsLossGain { get; set; }
    }
}
