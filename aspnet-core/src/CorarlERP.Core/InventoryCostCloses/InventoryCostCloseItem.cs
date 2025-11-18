using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Lots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.InventoryCostCloses
{
    [Table("CarlErpInventoryCostCloseItems")]
    public class InventoryCostCloseItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set ; }

        public long? LotId { get; private set; }
        public Lot Lot { get; private set; } 

        public decimal Qty { get; private set; }

        public InventoryCostClose InventoryCostClose { get; private set; }
        public Guid InventoryCostCloseId { get; private set; }
        
        public static InventoryCostCloseItem Create(int? tenantId, long creatorUserId, 
                                      long? lotId,decimal qty,Guid inventoryCostCloseId )
        {
            return new InventoryCostCloseItem()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                InventoryCostCloseId = inventoryCostCloseId,
                LotId = lotId,
                Qty = qty,
            };
        }
        public void Upudate(long lastModifiedUserId,
                                         long? lotId, decimal qty, Guid inventoryCostCloseId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LotId = lotId;
            Qty = qty;
            InventoryCostCloseId = inventoryCostCloseId;
        }
    }

}

