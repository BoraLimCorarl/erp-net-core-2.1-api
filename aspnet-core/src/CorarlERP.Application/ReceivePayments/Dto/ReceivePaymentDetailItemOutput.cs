using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Invoices.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ReceivePayments.Dto
{
    [AutoMapFrom(typeof(ReceivePaymentDetail))]
    public class ReceivePaymentDetailItemOutput
    {
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }
        public Guid? InvoiceId { get; set; }
        public InvoiceSummaryforReceivePayment Invoice { get; set; }

        public Guid? CustomerCreditId { get; set; }
        public CustomerCreditPaymentSummary CustomerCredit { get; set; }

        public decimal OpenBalance { get; set; }

        public decimal Payment { get; set; }

        //Split Payment into Cash, Credit and Expense
        //Payment = Cash + Creadit + Expense
        public decimal Cash { get; set; }
        public decimal MultiCurrencyCash { get; set; }
        public decimal Credit { get; set; }
        public decimal MultiCurrencyCredit { get; set; }
        public decimal Expense { get; set; }
        public decimal MultiCurrencyExpense { get; set; }

        public decimal CashInPaymentCurrency { get; set; }
        public decimal CreditInPaymentCurrency { get; set; }
        public decimal ExpenseInPaymentCurrency { get; set; }

        public decimal Amount { get; set; }

        public Guid AccountId { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }

        public decimal MultiCurrencyPayment { get; set; }

        public decimal MultiCurrencyAmount { get; set; }

        public ChartAccountSummaryOutput Account { get; set; }

        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public string MultiCurrencyCode { get; set; }
        public long MultiCurrencyId { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public decimal OpenBalanceInPaymentCurrency { get; set; }
        public decimal PaymentInPaymentCurrency { get; set; }
        public decimal TotalAmountInPaymentCurrency { get; set; }
        public decimal LossGain { get; set; }
    }
}
