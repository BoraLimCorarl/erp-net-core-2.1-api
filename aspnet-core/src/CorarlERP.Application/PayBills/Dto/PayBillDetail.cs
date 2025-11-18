using CorarlERP.Exchanges.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PayBills.Dto
{
    public class PayBillDetail
    {
        public Guid? Id { get; set; }
        public DateTime DueDate { get; set; }
        public Guid? BillId { get; set; }
        public Guid? VendorCreditId { get; set; }

        public Guid AccountId { get; set; }

        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

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

        public decimal AmountToBeSubstract { get; set; }
        
        public decimal MultiCurrencyOpenBalance { get; set; }
        public decimal MultiCurrencyPayment { get; set; }
        public decimal MultiCurrencyTotalAmount { get; set; }
        public decimal MultiCurrencyAmountToBeSubstract { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public long MultiCurrencyId { get; set; }

        public decimal OpenBalanceInPaymentCurrency { get; set; }
        public decimal PaymentInPaymentCurrency { get; set; }
        public decimal TotalAmountInPaymentCurrency { get; set; }
        public decimal LossGain { get; set; }

    }
}
