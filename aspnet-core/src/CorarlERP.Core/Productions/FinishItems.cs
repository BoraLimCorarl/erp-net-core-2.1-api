using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.Items;
using CorarlERP.Lots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Productions
{
    [Table("CarlErpFinishItems")]
    public class FinishItems : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid ProductionId { get; private set; }

        public Production Production { get; private set; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }
        public decimal UnitCost { get; private set; }
        public decimal Total { get; private set; }
        public long ToLotId { get; private set; }
        public Lot ToLot { get; private set; }

        private static FinishItems Create(
            int? tenantId, long? creatorUserId,
            Guid itemId, string description, 
            decimal qty, decimal unitCost, decimal total)
        {
            return new FinishItems
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Qty = qty,
                UnitCost = unitCost,
                Total = total,
                ItemId = itemId
            };
        }

        public void UpdateToLot (long toLotId)
        {
            this.ToLotId = toLotId;
        }
        public static FinishItems Create(
            int? tenantId, long? creatorUserId, 
            Production production, Guid itemId,
            string description, decimal qty,
            decimal unitCost, decimal total)
        {
            var result = Create( tenantId, creatorUserId, itemId, description, qty, unitCost, total);
            result.Production = production;
            return result;

        }

        public static FinishItems Create(
            int? tenantId,long? creatorUserId, Guid productionId,
            Guid itemId, string description, decimal qty, 
            decimal unitCost, decimal total)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, total);
            result.ProductionId = productionId;
            return result;

        }

        public void Update(long lastModifiedUserId,Guid itemId, string description, decimal qty, decimal unitCost, decimal total)
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Qty = qty;
            UnitCost = unitCost;
            Total = total;
            ItemId = itemId;
        }

        public void UpdateUnitCost(decimal unitCost, decimal total)
        {
            UnitCost = unitCost;
            Total = total;
        }
    }
}
