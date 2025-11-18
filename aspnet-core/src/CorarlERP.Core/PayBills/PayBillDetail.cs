using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Bills;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PayBills
{
    [Table("CarlErpPayBillDeail")]
    public class PayBillDetail : AuditedEntity<Guid>, IMayHaveTenant
    {

        public int? TenantId { get; set; }

        public Guid PayBillId { get; private set; }
        public PayBill PayBill { get; private set; }

        public Guid? BillId { get; private set; }
        public Bill Bill { get; private set; }
        public Guid? VendorCreditId { get; private set; }
        public VendorCredit.VendorCredit VendorCredit { get; private set; }

        public Guid? PayToId { get => BillId.HasValue ? BillId : VendorCreditId; }

        public Vendor Vendor { get; private set; }
        public Guid VendorId { get; set; }

        public DateTime DueDate { get; set; }

        public decimal OpenBalance { get; set; }

        public decimal Payment { get; set; }

        //Split Payment into Cash, Credit and Expense
        //Payment = Cash + Creadit + Expense
        //public decimal Cash { get; set; }
        //public decimal MultiCurrencyCash { get; set; }
        //public decimal Credit { get; set; }
        //public decimal MultiCurrencyCredit { get; set; }
        //public decimal Expense { get; set; }
        //public decimal MultiCurrencyExpense { get; set; }


        public decimal TotalAmount { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }

        public decimal MultiCurrencyPayment { get; set; }

        public decimal MultiCurrencyTotalAmount { get; set; }
        public decimal LossGain { get; private set; }
        public decimal OpenBalanceInPaymentCurrency { get; private set; }
        public decimal PaymentInPaymentCurrency { get; private set; }
        public decimal TotalAmountInPaymentCurrency { get; private set; }

        public static PayBillDetail Create(
           int? tenantId,
           long creatorUserId,
           PayBill payBill,
           Guid? billId,
           Guid vendorId,
           DateTime dueDate,
           decimal openBalance,
           decimal payment,
           decimal totalAmount,
           decimal multiCurrencyOpenBalance,
           decimal multiCurrencyPayment,
           decimal multiCurrencyTotalAmount,
           Guid? vendorCreditId,
           decimal lossGain,
            decimal openBalanceInPaymentCurrency,
            decimal paymentInPaymentCurrency,
            decimal totalAmountInPaymentCurrency
            )
        {
            return new PayBillDetail()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PayBill = payBill,
                BillId = billId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                DueDate = dueDate,
                OpenBalance = openBalance,
                Payment = payment,
                TotalAmount = totalAmount,
                VendorId = vendorId,
                MultiCurrencyOpenBalance = multiCurrencyOpenBalance,
                MultiCurrencyPayment = multiCurrencyPayment,
                MultiCurrencyTotalAmount = multiCurrencyTotalAmount,
                VendorCreditId = vendorCreditId,
                LossGain = lossGain,
                OpenBalanceInPaymentCurrency = openBalanceInPaymentCurrency,
                PaymentInPaymentCurrency = paymentInPaymentCurrency,
                TotalAmountInPaymentCurrency = totalAmountInPaymentCurrency,
            };
        }

        public void Update(
            long lastModifiedUserId,
            decimal openBalance,
            decimal payment,
            decimal totalAmount,
            decimal multiCurrencyOpenBalance,
            decimal multiCurrencyPayment,
            decimal multiCurrencyTotalAmount,
            Guid? billId,
            Guid? vendorCreditId,
            decimal lossGain,
            decimal openBalanceInPaymentCurrency,
            decimal paymentInPaymentCurrency,
            decimal totalAmountInPaymentCurrency
        )
        {
            BillId = billId;
            VendorCreditId = vendorCreditId;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            OpenBalance = openBalance;
            Payment = payment;
            TotalAmount = totalAmount;
            MultiCurrencyOpenBalance = multiCurrencyOpenBalance;
            MultiCurrencyPayment = multiCurrencyPayment;
            MultiCurrencyTotalAmount = multiCurrencyTotalAmount;
            LossGain = lossGain;
            OpenBalanceInPaymentCurrency = openBalanceInPaymentCurrency;
            PaymentInPaymentCurrency = paymentInPaymentCurrency;
            TotalAmountInPaymentCurrency = totalAmountInPaymentCurrency;
        }

    }
}
