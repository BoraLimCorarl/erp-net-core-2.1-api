using CorarlERP.Customers.Dto;
using CorarlERP.Exchanges.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ReceivePayments.Dto
{
    public class ReceivePaymentDetail
    {
        public Guid? Id { get; set; }
        public DateTime DueDate { get; set; }
        public Guid ReceivePaymentId { get; set; }
        public Guid accountId { get; set; }
        public Guid? InvoiceId { get; set; }
        public Guid? CustomerCreditId { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Vendor { get; set; }

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


        public decimal TotalAmount { get; set; }

        public decimal AmountToBeSubstract { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }
        public decimal MultiCurrencyPayment { get; set; }
        public decimal MultiCurrencyTotalAmount { get; set; }
        public decimal MultiCurrencyAmountToBeSubstract { get; set; }
        public long MultiCurrencyId { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public decimal OpenBalanceInPaymentCurrency { get; set; }
        public decimal PaymentInPaymentCurrency { get; set; }
        public decimal TotalAmountInPaymentCurrency { get; set; }
        public decimal LossGain { get; set; }
    }
}
