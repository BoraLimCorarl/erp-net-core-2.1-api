using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PayBills
{
    [Table("CarlErpPayBillExpense")]
    public class PayBillExpense : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public Guid PayBillId { get; private set; }
        public PayBill PayBill { get; private set; }

        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }

        [MaxLength(256)]
        public string Description { get; private set; }

        public decimal Amount { get; private set; }
        public decimal MultiCurrencyAmount { get; private set; }
        public bool IsLossGain { get; private set; } 

        public static PayBillExpense Create(
           int tenantId,
           long creatorUserId,
           PayBill payBill,
           Guid accountId,
           decimal amount, 
           decimal amountMultiCurrency,
           string description,
           bool isLossGain)
        {
            return new PayBillExpense()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PayBill = payBill,
                AccountId = accountId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Amount = amount,
                MultiCurrencyAmount = amountMultiCurrency,
                Description = description,
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
        ) {
            MultiCurrencyAmount = amountMultiCurrency;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            AccountId = accountId;
            Amount = amount;
            Description = description;
            IsLossGain = isLossGain;
        }
    }
}
