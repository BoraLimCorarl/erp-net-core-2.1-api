using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Locations;
using CorarlERP.PaymentMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PayBills
{
    [Table("CarlErpPayBills")]
    public class PayBill : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public bool FiFo { get; private set; }

        public decimal TotalPayment { get; private set; }
        public decimal MultiCurrencyTotalPayment { get; private set; }
        public decimal Change { get; private set; }
        public decimal MultiCurrencyChange { get; private set; }

     
        public decimal TotalOpenBalance { get; private set; }
        public decimal TotalOpenBalanceVendorCredit { get; private set; }
        public decimal TotalPaymentVendorCredit { get; private set; }
        public decimal TotalPaymentBill { get; private set; }
        public decimal TotalPaymentDue { get; private set; }
        public decimal TotalPaymentDueVendorCredit { get; private set; }

        public decimal MultiCurrencyTotalOpenBalance { get; private set; }
        public decimal MultiCurrencyTotalOpenBalanceVendorCredit { get; private set; }
        public decimal MultiCurrencyTotalPaymentBill { get; private set; } //Not use
        public decimal MultiCurrencyTotalPaymentVendorCredit { get; private set; } //Not use
        public decimal MultiCurrencyTotalPaymentDue { get; private set; }
        public decimal MultiCurrencyTotalPaymentDueVendorCredit { get; private set; }

        public ReceiveFromPayBill ReceiveFrom { get; private set; }

        public virtual PaymentMethod PaymentMethod { get; set; }
        public Guid? PaymentMethodId { get; set; }

        public void SetPaymentMethod(Guid? paymentMethodId) => PaymentMethodId = paymentMethodId;
        public bool UseExchangeRate { get; private set; }

        public static PayBill Create(
            int? tenantId,
            long creatorUserId,
            bool fiFo,
            decimal totalOpenBalance,
            decimal totalPayment,
            decimal totalPaymentDue,
            ReceiveFromPayBill receiveFrom,
            decimal multiCurrencyTotalPayment,
            decimal change,
            decimal totalOpenBalanceVendorCredit, 
            decimal totalPaymentVendorCredit,
            decimal totalPaymentDueVendorCredit,
            decimal multiCurrencyTotalPaymentVendorCredit,
            decimal totalPaymentBill,
            decimal multiCurrencyTotalPaymentBill,
            decimal multiCurrencyChange,
            bool useExchangeRate,
            decimal multiCurrencyTotalOpenBalance,
            decimal multiCurrencyTotalOpenBalanceVendorCredit,
            decimal multiCurrencyTotalPaymentDue,
            decimal multiCurrencyTotalPaymentDueVendorCredit
            )
        {
            return new PayBill()
            {
                MultiCurrencyChange = multiCurrencyChange,
                TotalPaymentBill = totalPaymentBill,
                MultiCurrencyTotalPaymentBill = multiCurrencyTotalPaymentBill,
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
                TotalOpenBalanceVendorCredit = totalOpenBalanceVendorCredit,
                TotalPaymentDueVendorCredit = totalPaymentDueVendorCredit,
                MultiCurrencyTotalPaymentVendorCredit = multiCurrencyTotalPaymentVendorCredit,
                TotalPaymentVendorCredit = totalPaymentVendorCredit,
                UseExchangeRate = useExchangeRate,
                MultiCurrencyTotalOpenBalance = multiCurrencyTotalOpenBalance,
                MultiCurrencyTotalOpenBalanceVendorCredit = multiCurrencyTotalOpenBalanceVendorCredit,
                MultiCurrencyTotalPaymentDue = multiCurrencyTotalPaymentDue,
                MultiCurrencyTotalPaymentDueVendorCredit = multiCurrencyTotalPaymentDueVendorCredit,
            };
        }

        public void Update(
            long lastModifiedUserId,
            bool fiFo,
            decimal totalOpenBalance,
            decimal totalPayment,
            decimal totalPaymentDue,
            ReceiveFromPayBill receiveFrom,
            decimal multiCurrencyTotalPayment,
            decimal change,
            decimal totalOpenBalanceVendorCredit,
            decimal totalPaymentVendorCredit,
            decimal totalPaymentDueVendorCredit,
            decimal multiCurrencyTotalPaymentVendorCredit,
            decimal totalPaymentBill,
            decimal multiCurrencyTotalPaymentBill,
            decimal multiCurrencyChange,
            decimal multiCurrencyTotalOpenBalance,
            decimal multiCurrencyTotalOpenBalanceVendorCredit,
            decimal multiCurrencyTotalPaymentDue,
            decimal multiCurrencyTotalPaymentDueVendorCredit
        )
        {
            MultiCurrencyChange = multiCurrencyChange;
            TotalPaymentBill = totalPaymentBill;
            MultiCurrencyTotalPaymentBill = multiCurrencyTotalPaymentBill;
            Change = change;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TotalOpenBalance = totalOpenBalance;
            TotalPayment = totalPayment;
            TotalPaymentDue = totalPaymentDue;
            FiFo = fiFo;
            ReceiveFrom = receiveFrom;
            MultiCurrencyTotalOpenBalance = multiCurrencyTotalOpenBalance;
            MultiCurrencyTotalPayment = multiCurrencyTotalPayment;

            TotalOpenBalanceVendorCredit = totalOpenBalanceVendorCredit;
            TotalPaymentDueVendorCredit = totalPaymentDueVendorCredit;
            MultiCurrencyTotalPaymentVendorCredit = multiCurrencyTotalPaymentVendorCredit;
            TotalPaymentVendorCredit = totalPaymentVendorCredit;
            MultiCurrencyTotalOpenBalance = multiCurrencyTotalOpenBalance;
            MultiCurrencyTotalOpenBalanceVendorCredit = multiCurrencyTotalOpenBalanceVendorCredit;
            MultiCurrencyTotalPaymentDue = multiCurrencyTotalPaymentDue;
            MultiCurrencyTotalPaymentDueVendorCredit = multiCurrencyTotalPaymentDueVendorCredit;
        }

    }
}
