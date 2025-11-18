using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.CustomerCredits;
using CorarlERP.Customers;
using CorarlERP.Invoices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ReceivePayments
{
    [Table("CarlErpReceivePaymentDeails")]
    public class ReceivePaymentDetail : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid ReceivePaymentId { get; private set; }
        public ReceivePayment ReceivePayment { get; private set; }

        public Guid? InvoiceId { get; private set; }
        public Invoice Invoice { get; private set; }

        public Guid? CustomerCreditId { get; private set; }
        public CustomerCredit CustomerCredit { get; private set; }

        public Guid? PayToId { get => InvoiceId.HasValue ? InvoiceId : CustomerCreditId; }


        public Customer Customer { get; private set; }
        public Guid CustomerId { get; set; }

        public DateTime DueDate { get; set; }

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

        public decimal TotalAmount { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }

        public decimal MultiCurrencyPayment { get; set; }

        public decimal MultiCurrencyTotalAmount { get; set; }
        public decimal LossGain { get; private set; }
        public decimal OpenBalanceInPaymentCurrency { get; private set; }
        public decimal PaymentInPaymentCurrency { get; private set; }
        public decimal TotalAmountInPaymentCurrency { get; private set; }
        public decimal CashInPaymentCurrency { get; private set; }
        public decimal CreditInPaymentCurrency { get; private set; }
        public decimal ExpenseInPaymentCurrency { get; private set; }

        public static ReceivePaymentDetail Create(int? tenantId,long creatorUserId,
            ReceivePayment receivePayment,
            Guid? InvoiceId,
            Guid customerId, 
            DateTime dueDate, 
            decimal openBalance, 
            decimal payment, 
            decimal totalAmount,
            decimal multiCurrencyOpenBalance,
            decimal multiCurrencyPayment,
            decimal multiCurrencyTotalAmount,
            Guid? customerCreditId,
            decimal cash,
            decimal multiCurrencyCash,
            decimal credit,
            decimal multiCurrencyCredit,
            decimal expense,
            decimal multiCurrencyExpense,
            decimal lossGain,
            decimal openBalanceInPaymentCurrency,
            decimal paymentInPaymentCurrency,
            decimal totalAmountInPaymentCurrency,
            decimal cashInPaymentCurrency,
            decimal creditInPaymentCurrency,
            decimal expenseInPaymentCurrency
        )
        {
            return new ReceivePaymentDetail()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ReceivePayment = receivePayment,
                InvoiceId = InvoiceId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                DueDate = dueDate,
                OpenBalance = openBalance,
                Payment = payment,
                TotalAmount = totalAmount,
                CustomerId = customerId,
                MultiCurrencyOpenBalance = multiCurrencyOpenBalance,
                MultiCurrencyPayment = multiCurrencyPayment,
                MultiCurrencyTotalAmount = multiCurrencyTotalAmount,
                CustomerCreditId = customerCreditId,
                Cash = cash,
                MultiCurrencyCash = multiCurrencyCash,
                Credit = credit,
                MultiCurrencyCredit = multiCurrencyCredit,
                Expense = expense,
                MultiCurrencyExpense = multiCurrencyExpense,
                LossGain = lossGain,
                OpenBalanceInPaymentCurrency = openBalanceInPaymentCurrency,
                PaymentInPaymentCurrency = paymentInPaymentCurrency,
                TotalAmountInPaymentCurrency = totalAmountInPaymentCurrency,
                CashInPaymentCurrency = cashInPaymentCurrency,
                CreditInPaymentCurrency = creditInPaymentCurrency,
                ExpenseInPaymentCurrency = expenseInPaymentCurrency
            };
        }

        public void Update( 
            long lastModifiedUserId,decimal openBalance, 
            decimal payment, decimal totalAmount,
            decimal multiCurrencyOpenBalance,
            decimal multiCurrencyPayment,
            decimal multiCurrencyTotalAmount,
            Guid? invoiceId,
            Guid? customerCreditId,
            decimal cash,
            decimal multiCurrencyCash,
            decimal credit,
            decimal multiCurrencyCredit,
            decimal expense,
            decimal multiCurrencyExpense,
            decimal lossGain,
            decimal openBalanceInPaymentCurrency,
            decimal paymentInPaymentCurrency,
            decimal totalAmountInPaymentCurrency,
            decimal cashInPaymentCurrency,
            decimal creditInPaymentCurrency,
            decimal expenseInPaymentCurrency
        )
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            OpenBalance = openBalance;
            Payment = payment;
            TotalAmount = totalAmount;
            MultiCurrencyOpenBalance = multiCurrencyOpenBalance;
            MultiCurrencyPayment = multiCurrencyPayment;
            MultiCurrencyTotalAmount = multiCurrencyTotalAmount;
            InvoiceId = invoiceId;
            CustomerCreditId = customerCreditId;
            Cash = cash;
            MultiCurrencyCash = multiCurrencyCash;
            Credit = Credit;
            MultiCurrencyCredit = multiCurrencyCredit;
            Expense = expense;
            MultiCurrencyExpense = multiCurrencyExpense;
            LossGain = lossGain;
            OpenBalanceInPaymentCurrency = openBalanceInPaymentCurrency;
            PaymentInPaymentCurrency = paymentInPaymentCurrency;
            TotalAmountInPaymentCurrency = totalAmountInPaymentCurrency;
            CashInPaymentCurrency = cashInPaymentCurrency;
            CreditInPaymentCurrency = creditInPaymentCurrency;
            ExpenseInPaymentCurrency = expenseInPaymentCurrency;
        }
    }
}
