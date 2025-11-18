using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.VendorCredit.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.PayBills.Dto
{
    [AutoMapFrom(typeof(PayBill))]
    public class PayBillDetailOutput
    {
        public string PayBillNo { get; set; }
        public bool FiFo { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }

        public Guid? PaymentAccountId { get; set; }
        public ChartAccountSummaryOutput PaymentAccount { get; set; }


        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        
        public long? LocationId { get; set; }
        public string LocationName { get; set; }


        public string Memo { get; set; }

        public DateTime PaymentDate { get; set; }
        
        public string Reference { get; set; }

        public decimal TotalOpenBalance { get; set; }
        public decimal TotalOpenBalanceVendorCredit { get; set; }

        public decimal TotalPayment { get; set; }
        public decimal TotalPaymentVendorCredit { get; set; }
        public decimal TotalPaymentBill { get; set; }

        public decimal TotalPaymentDue { get; set; }
        public decimal TotalPaymentDueVendorCredit { get; set; }


        public long MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }

        public decimal MultiCurrencyTotalOpenBalance { get; set; }
        public decimal MultiCurrencyTotalPaymentDue { get; set; }
        public decimal MultiCurrencyTotalOpenBalanceVendorCredit { get; set; }
        public decimal MultiCurrencyTotalPaymentDueVendorCredit { get; set; }

        public decimal MultiCurrencyTotalPayment { get; set; }
        public decimal MultiCurrencyTotalPaymentBill { get; set; }        
        public decimal MultiCurrencyTotalPaymentVendorCredit { get; set; }

        public decimal MultiCurrencyChange { get; set; }
        public decimal Change { get; set; }

        public decimal TotalCashBill { get; set; }
        public decimal TotalCreditBill { get; set; }
        public decimal TotalExpenseBill { get; set; }
        public decimal TotalCashVendorCredit { get; set; }
        public decimal TotalCreditVendorCredit { get; set; }
        public decimal TotalExpenseVendorCredit { get; set; }

        public ReceiveFromPayBill ReceiveFrom { get; set; }

        //public VendorCreditSummaryDetailOutput VendorCredit { get; set; }
        //public Guid? VendorCreditId { get; private set; }

        public List<PayBillDetailItemOutput> PayBillDetailItems { get; set; }
        public List<PayBillExpenseItem> PayBillExpenseItems { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
        public decimal TotalLossGainBill { get; set; }
        public decimal TotalLossGainVendorCredit { get; set; }
        public ExchangeLossGainDto ExchangeLossGain { get; set; }

    }
}
