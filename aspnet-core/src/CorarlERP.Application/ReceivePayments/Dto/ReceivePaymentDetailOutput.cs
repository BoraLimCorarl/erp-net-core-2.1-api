using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.PayBills.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReceivePayments.Dto
{
    [AutoMapFrom(typeof(ReceivePayment))]
    public class ReceivePaymentDetailOutput
    {
        public string ReceivePaymentNo { get; set; }
        public bool FiFo { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }

        public long MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }

        public decimal MultiCurrencyTotalOpenBalance { get; set; }
        public decimal MultiCurrencyTotalPaymentDue { get; set; }
        public decimal MultiCurrencyTotalOpenBalanceCustomerCredit { get; set; }
        public decimal MultiCurrencyTotalPaymentDueCustomerCredit { get; set; }

        public decimal MultiCurrencyTotalPayment { get; set; }

        public Guid? ReceivePaymentAccountId { get; set; }
        public ChartAccountSummaryOutput ReceivePaymentAccount { get; set; }


        public string LocationName { get; set; }
        public long? LocationId { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public string Memo { get; set; }

        public DateTime ReceivePaymentDate { get; set; }

        public string Reference { get; set; }

        public decimal TotalOpenBalance { get; set; }

        public decimal TotalPayment { get; set; }

        public decimal TotalPaymentDue { get; set; }

        public ReceiveFromRecievePayment ReceiveFrom { get; set; }

        public decimal MultiCurrencyTotalPaymentInvoice { get; set; }
        public decimal MultiCurrencyTotalPaymentCustomerCredit { get; set; }
        public decimal Change { get; set; }
        public decimal MultiCurrencyChange { get; set; }
        //public CustomerCreditSummaryDetailOutput CustomerCredit { get; set; }
        //public Guid? CustomerCreditId { get; private set; }
        public decimal TotalPaymentCustomerCredit { get; set; }
        public decimal TotalPaymentInvoice { get; set; }
        public decimal TotalOpenBalanceCustomerCredit { get; set; }
        public decimal TotalPaymentDueCustomerCredit { get; set; }
        public List<ReceivePaymentExpenseItem> ReceivePaymentExpenseItems { get; set; }
        public List<ReceivePaymentDetailItemOutput> ReceivePaymentDetailItems { get; set; }

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

        public Guid? PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public decimal TotalLossGainInvoice { get; set; }
        public decimal TotalLossGainCustomerCredit { get; set; }
        public ExchangeLossGainDto ExchangeLossGain { get; set; }
    }
}
