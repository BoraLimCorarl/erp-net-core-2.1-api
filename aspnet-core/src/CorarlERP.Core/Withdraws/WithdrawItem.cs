using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Withdraws;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.Withdraws
{
    [Table("CarlErpWithdrawItems")]
    public class WithdrawItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid WithdrawId { get; private set; }
        public Withdraw Withdraw { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal Total { get; private set; }


        private static WithdrawItem Create(int? tenantId, long? creatorUserId, string description, decimal qty, decimal unitCost, decimal total)
        {
            return new WithdrawItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Total = total,
                Qty = qty,
                UnitCost = unitCost,
            };
        }
        public static WithdrawItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid WithdrawId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal total)
        {
            var result = Create(tenantId, creatorUserId, description, qty, unitCost, total);
            result.WithdrawId = WithdrawId;
            return result;
        }

        //use for createWithdrawItem in Update Withdraw in Create new Withdraw(when this WithdrawItem does not have Id yet)       
        public static WithdrawItem Create(
            int? tenantId,
            long? creatorUserId,
            Withdraw withdraw,
            string description,
            decimal qty,
            decimal unitCost,
            decimal total)
        {
            var result = Create(tenantId, creatorUserId, description, qty, unitCost, total);
            result.Withdraw = withdraw;
            return result;

        }

        // Withdraw cannot just change Withdraw
        public void Update(long lastModifiedUserId,string description, decimal qty, decimal unitCost, decimal total)
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Total = total;
            Qty = qty;           
            UnitCost = unitCost;          
        }
    }
}
