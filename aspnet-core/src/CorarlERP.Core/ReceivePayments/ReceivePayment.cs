using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.CustomerCredits;
using CorarlERP.Locations;
using CorarlERP.PaymentMethods;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReceivePayments
{
    [Table("CarlErpReceivePayments")]
    public class ReceivePayment : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public bool FiFo { get; set; }

        public decimal TotalPayment { get; set; }
        public decimal MultiCurrencyTotalPayment { get; private set; }
        public decimal Change { get; private set; }
        public decimal MultiCurrencyChange { get; private set; }
        public void SetChange(decimal change) { Change = change; }

        public decimal TotalOpenBalance { get; set; }
        public decimal TotalOpenBalanceCustomerCredit { get; private set; }
        public decimal TotalPaymentInvoice { get; private set; }
        public decimal TotalPaymentCustomerCredit { get; private set; }
        public decimal TotalPaymentDue { get; set; }
        public decimal TotalPaymentDueCustomerCredit { get; private set; }

        public decimal MultiCurrencyTotalOpenBalance { get; private set; }
        public decimal MultiCurrencyTotalOpenBalanceCustomerCredit { get; private set; }
        public decimal MultiCurrencyTotalPaymentInvoice { get; private set; } //Not use
        public decimal MultiCurrencyTotalPaymentCustomerCredit { get; private set; } //Not use
        public decimal MultiCurrencyTotalPaymentDue { get; private set; }
        public decimal MultiCurrencyTotalPaymentDueCustomerCredit { get; private set; }

        public decimal MultiCurrencyTotalCashInvoice { get; private set; }
        public decimal MultiCurrencyTotalCreditInvoice { get; private set; }
        public decimal MultiCurrencyTotalExpenseInvoice { get; private set; }
        public decimal MultiCurrencyTotalCashCustomerCredit { get; private set; }
        public decimal MultiCurrencyTotalCreditCustomerCredit { get; private set; }
        public decimal MultiCurrencyTotalExpenseCustomerCredit { get; private set; }

        public void SetTotalPayment(decimal total) { TotalPayment = total; }
        public void SetTotalOpenBalance(decimal total) { TotalOpenBalance = total; }
        public void SetTotalOpenBalanceCustomerCredit(decimal total) { TotalOpenBalanceCustomerCredit = total; }
        public void SetTotalPaymentInvoice(decimal total) { TotalPaymentInvoice = total; }
        public void SetTotalPaymentCustomerCredit(decimal total) { TotalPaymentCustomerCredit = total; }
        public void SetTotalPaymentDue(decimal total) { TotalPaymentDue = total; }
        public void SetTotalPaymentDueCustomerCredit(decimal total) { TotalPaymentDueCustomerCredit = total; }


        public decimal TotalCashInvoice { get; set; }
        public decimal TotalCreditInvoice { get; set; }
        public decimal TotalExpenseInvoice { get; set; }
        public decimal TotalCashCustomerCredit { get; set; }
        public decimal TotalCreditCustomerCredit { get; set; }
        public decimal TotalExpenseCustomerCredit { get; set; }


        public ReceiveFromRecievePayment ReceiveFrom { get; private set; }
        public void SetReceiveFrom(ReceiveFromRecievePayment receiveFrom) { ReceiveFrom = receiveFrom; }


        public virtual PaymentMethod PaymentMethod { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public bool UseExchangeRate { get; private set; }

        public void UpdatePaymentMethodId(Guid? paymentMethodId)
        {
            this.PaymentMethodId = paymentMethodId;
        }
        public static ReceivePayment Create(
            int? tenantId,
            long creatorUserId,
            bool fiFo,
            decimal totalOpenBalance,
            decimal totalPayment,
            decimal totalPaymentDue,
            ReceiveFromRecievePayment receiveFrom,
            decimal multiCurrencyTotalPayment,
            decimal change,
            decimal totalOpenBalanceCustomerCredit,
            decimal totalPaymentCustomerCredit,
            decimal totalPaymentDueCustomerCredit,
            decimal multiCurrencyTotalPaymentCustomerCredit,
            decimal totalPaymentInvoice,
            decimal multiCurrencyTotalPaymentInvoice,
            decimal multiCurrencyChange,

            decimal totalCashInvoice,
            decimal totalCreditInvoice,
            decimal totalExpenseInvoice,
            decimal totalCashCustomerCredit,
            decimal totalCreditCustomerCredit,
            decimal totalExpenseCustomerCredit,
            bool useExchagneRate,
            decimal multiCurrencyTotalOpenBalance,
            decimal multiCurrencyTotalOpenBalanceCustomerCredit,
            decimal multiCurrencyTotalPaymentDue,
            decimal multiCurrencyTotalPaymentDueCustomerCredit,
            decimal multiCurrencyTotalCashInvoice,
            decimal multiCurrencyTotalCreditInvoice,
            decimal multiCurrencyTotalExpenseInvoice,
            decimal multiCurrencyTotalCashCustomerCredit,
            decimal multiCurrencyTotalCreditCustomerCredit,
            decimal multiCurrencyTotalExpenseCustomerCredit
         )
        {
            return new ReceivePayment()
            {

                TotalOpenBalanceCustomerCredit = totalOpenBalanceCustomerCredit,
                TotalPaymentDueCustomerCredit = totalPaymentDueCustomerCredit,
                MultiCurrencyTotalPaymentCustomerCredit = multiCurrencyTotalPaymentCustomerCredit,
                TotalPaymentCustomerCredit = totalPaymentCustomerCredit,
                MultiCurrencyChange = multiCurrencyChange,
                TotalPaymentInvoice = totalPaymentInvoice,
                MultiCurrencyTotalPaymentInvoice = multiCurrencyTotalPaymentInvoice,
                Change = change,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                FiFo = fiFo,
                TotalOpenBalance = totalOpenBalance,
                TotalPayment = totalPayment,
                TotalPaymentDue = totalPaymentDue,
                ReceiveFrom = receiveFrom,
                MultiCurrencyTotalPayment = multiCurrencyTotalPayment,
                TotalCashInvoice = totalCashInvoice,
                TotalCreditInvoice = totalCreditInvoice,
                TotalExpenseInvoice = totalExpenseInvoice,
                TotalCashCustomerCredit = totalCashCustomerCredit,
                TotalCreditCustomerCredit = totalCreditCustomerCredit,
                TotalExpenseCustomerCredit = totalExpenseCustomerCredit,
                UseExchangeRate = useExchagneRate,
                MultiCurrencyTotalOpenBalance = multiCurrencyTotalOpenBalance,
                MultiCurrencyTotalOpenBalanceCustomerCredit = multiCurrencyTotalOpenBalanceCustomerCredit,
                MultiCurrencyTotalPaymentDue = multiCurrencyTotalPaymentDue,
                MultiCurrencyTotalPaymentDueCustomerCredit = multiCurrencyTotalPaymentDueCustomerCredit,
                MultiCurrencyTotalCashInvoice = multiCurrencyTotalCashInvoice,
                MultiCurrencyTotalCreditInvoice = multiCurrencyTotalCreditInvoice,
                MultiCurrencyTotalExpenseInvoice = multiCurrencyTotalExpenseInvoice,
                MultiCurrencyTotalCashCustomerCredit = multiCurrencyTotalCashCustomerCredit,
                MultiCurrencyTotalCreditCustomerCredit = multiCurrencyTotalCreditCustomerCredit,
                MultiCurrencyTotalExpenseCustomerCredit = multiCurrencyTotalExpenseCustomerCredit
            };
        }

        public void Update(
            long lastModifiedUserId,
            bool fiFo,
            decimal totalOpenBalance,
            decimal totalPayment,
            decimal totalPaymentDue,
            ReceiveFromRecievePayment receiveFrom,
            decimal multiCurrencyTotalPayment,
            decimal change,
            decimal totalOpenBalanceCustomerCredit,
            decimal totalPaymentCustomerCredit,
            decimal totalPaymentDueCustomerCredit,
            decimal multiCurrencyTotalPaymentCustomerCredit,
            decimal totalPaymentInvoice,
            decimal multiCurrencyTotalPaymentInvoice,
            decimal multiCurrencyChange,

            decimal totalCashInvoice,
            decimal totalCreditInvoice,
            decimal totalExpenseInvoice,
            decimal totalCashCustomerCredit,
            decimal totalCreditCustomerCredit,
            decimal totalExpenseCustomerCredit,
            decimal multiCurrencyTotalOpenBalance,
            decimal multiCurrencyTotalOpenBalanceCustomerCredit,
            decimal multiCurrencyTotalPaymentDue,
            decimal multiCurrencyTotalPaymentDueCustomerCredit,
            decimal multiCurrencyTotalCashInvoice,
            decimal multiCurrencyTotalCreditInvoice,
            decimal multiCurrencyTotalExpenseInvoice,
            decimal multiCurrencyTotalCashCustomerCredit,
            decimal multiCurrencyTotalCreditCustomerCredit,
            decimal multiCurrencyTotalExpenseCustomerCredit
        )
        {
            MultiCurrencyChange = multiCurrencyChange;
            TotalPaymentInvoice = totalPaymentInvoice;
            MultiCurrencyTotalPaymentInvoice = multiCurrencyTotalPaymentInvoice;
            Change = change;

            TotalOpenBalanceCustomerCredit = totalOpenBalanceCustomerCredit;
            TotalPaymentDueCustomerCredit = totalPaymentDueCustomerCredit;
            MultiCurrencyTotalPaymentCustomerCredit = multiCurrencyTotalPaymentCustomerCredit;
            TotalPaymentCustomerCredit = totalPaymentCustomerCredit;

            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TotalOpenBalance = totalOpenBalance;
            TotalPayment = totalPayment;
            TotalPaymentDue = totalPaymentDue;
            FiFo = fiFo;
            ReceiveFrom = receiveFrom;
            //CustomerCreditId = customerCreditId;
            MultiCurrencyTotalPayment = multiCurrencyTotalPayment;

            TotalCashInvoice = totalCashInvoice;
            TotalCreditInvoice = totalCreditInvoice;
            TotalExpenseInvoice = totalExpenseInvoice;
            TotalCashCustomerCredit = totalCashCustomerCredit;
            TotalCreditCustomerCredit = totalCreditCustomerCredit;
            TotalExpenseCustomerCredit = totalExpenseCustomerCredit;
            MultiCurrencyTotalOpenBalance = multiCurrencyTotalOpenBalance;
            MultiCurrencyTotalOpenBalanceCustomerCredit = multiCurrencyTotalOpenBalanceCustomerCredit;
            MultiCurrencyTotalPaymentDue = multiCurrencyTotalPaymentDue;
            MultiCurrencyTotalPaymentDueCustomerCredit = multiCurrencyTotalPaymentDueCustomerCredit;
            MultiCurrencyTotalCashInvoice = multiCurrencyTotalCashInvoice;
            MultiCurrencyTotalCreditInvoice = multiCurrencyTotalCreditInvoice;
            MultiCurrencyTotalExpenseInvoice = multiCurrencyTotalExpenseInvoice;
            MultiCurrencyTotalCashCustomerCredit = multiCurrencyTotalCashCustomerCredit;
            MultiCurrencyTotalCreditCustomerCredit = multiCurrencyTotalCreditCustomerCredit;
            MultiCurrencyTotalExpenseCustomerCredit = multiCurrencyTotalExpenseCustomerCredit;
        }
    }
}
