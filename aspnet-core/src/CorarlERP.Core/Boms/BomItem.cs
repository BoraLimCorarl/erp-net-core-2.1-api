using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Boms
{

    [Table("CarlErpBOMItems")]
    public class BomItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Item Item { get; private set; }
        public Guid ItemId { get; private set; }
        public Bom BOM { get; private set; }
        public Guid BomId { get; private set; }
        public  decimal Qty { get; private set; }
        public static BomItem Create(int? tenantId,
                              long? creatorUserId,                              
                              Guid itemId,
                              Guid bomId,
                              decimal qty)
        {
            return new BomItem()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ItemId = itemId,
                BomId = bomId,
                Qty = qty
            };
        }
        public void Update(long lastModifiedUserId, Guid itemId,Guid bomId,decimal qty)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.ItemId = itemId;
            this.Qty = qty;
            this.BomId = bomId;
        }
        public void SetQty (decimal qty)
        {
            this.Qty = qty;
        }
    }
}
