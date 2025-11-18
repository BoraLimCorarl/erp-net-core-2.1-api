using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Deposits
{
    [Table("CarlErpDepositItems")]
    public class DepositItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid DepositId { get; private set; }
        public Deposit Deposit { get; private set; }
        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }
        public decimal Qty { get; private set; }
        public decimal UnitCost { get; private set; }       
        public decimal Total { get; private set; }

        private static DepositItem Create(
            int? tenantId, 
            long? creatorUserId,
            Guid accountId, 
            decimal qty, 
            decimal unitCost, 
            decimal total)
        {
            return new DepositItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Total = total,
                Qty = qty,
                AccountId = accountId,
                UnitCost = unitCost
            };
        }

        //use for create bill Item in Update bill in Create new bill(when this billItem does not have Id yet)       
        public static DepositItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid depositId,
            Guid accountId,
            decimal qty,
            decimal unitCost,
            decimal total)
        {
            var result = Create(tenantId, creatorUserId, accountId, qty, unitCost, total);
            result.DepositId = depositId;
            return result;

        }

        public void Update(
            long lastModifiedUserId,
            Guid accountId,
            decimal qty, 
            decimal unitCost, 
            decimal total)
        {
            LastModifierUserId = lastModifiedUserId;
            Total = total;
            Qty = qty;
            AccountId = accountId;
            UnitCost = unitCost;
        }
    }
}
