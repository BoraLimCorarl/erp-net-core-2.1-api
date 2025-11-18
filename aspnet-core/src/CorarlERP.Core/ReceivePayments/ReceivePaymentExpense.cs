using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ReceivePayments
{
    [Table("CarlErpReceivePaymentExpense")]
    public class ReceivePaymentExpense : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public Guid ReceivePaymentId { get; private set; }
        public ReceivePayment ReceivePayment { get; private set; }
        public void SetReceivePayment(Guid id) { ReceivePaymentId = id; }

        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }

        public decimal Amount { get; private set; }
        public void SetAmount(decimal amount) { Amount = amount; }

        [MaxLength(256)]
        public string Description { get; private set; }
        public decimal MultiCurrencyAmount { get; private set; }
        public bool IsLossGain { get; private set; }

        public static ReceivePaymentExpense Create(
           int tenantId,
           long creatorUserId,
           ReceivePayment receivePayment,
           Guid accountId,
           decimal amount, 
           decimal amountMultiCurrency,
           string description,
           bool isLossGain)
        {
            return new ReceivePaymentExpense()
            {
                Id = Guid.NewGuid(),
                Description = description,
                TenantId = tenantId,
                ReceivePayment = receivePayment,
                AccountId = accountId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Amount = amount,
                MultiCurrencyAmount = amountMultiCurrency,
                IsLossGain = isLossGain
            };
        }

        public void Update(
            long lastModifiedUserId,
            decimal amount,
            Guid accountId, 
            decimal amountMultiCurrency,
            string description,
            bool isLossGain
        )
        {
            MultiCurrencyAmount = amountMultiCurrency;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            AccountId = accountId;
            Amount = amount;
            IsLossGain = isLossGain;
        }
    }
}
