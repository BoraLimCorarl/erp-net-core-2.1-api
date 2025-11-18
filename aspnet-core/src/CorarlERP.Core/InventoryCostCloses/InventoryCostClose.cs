using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.AccountCycles;
using CorarlERP.Items;
using CorarlERP.Locations;
using CorarlERP.Lots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.InventoryCostCloses
{
    [Table("CarlErpInventoryCostCloses")]
    public class InventoryCostClose : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public long LocationId { get; private set; }
        public Location Location { get; private set; }

        public long AccountCycleId { get; private set; }
        public AccountCycle AccountCycle { get; private set; }

        public decimal Cost { get; private set; }

        public decimal QtyOnhand { get; private set; }

        public DateTime CloseDate { get; private set; }

        //lot Id
        //public long? LotId { get; private set; }
        //public Lot Lot { get; private set; }
        public decimal TotalCost { get; private set; }      
        public static InventoryCostClose Create(int? tenantId, long creatorUserId, Guid itemId,
                                         long locationId, long accountCycleId, decimal cost,
                                         decimal qtyOnHand,DateTime closeDate,decimal totalCost)
        {
            return new InventoryCostClose()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ItemId = itemId,
                AccountCycleId = accountCycleId,
                Cost = cost,
                LocationId = locationId,
                QtyOnhand = qtyOnHand,
                CloseDate = closeDate,
                TotalCost = totalCost,              
            };
        }

        public void Upudate(long lastModifiedUserId, Guid itemId,
                                         long locationId, long accountCycleId, 
                                         decimal cost, decimal qtyOnHand,DateTime closeDate,
                                         long lotId,decimal totalCost, decimal avgCost)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ItemId = itemId;
            LocationId = locationId;
            AccountCycleId = accountCycleId;
            Cost = cost;
            QtyOnhand = qtyOnHand;
            CloseDate = closeDate;
            TotalCost = totalCost;            
        }
    }
}
