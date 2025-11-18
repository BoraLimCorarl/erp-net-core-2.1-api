using System;
using System.Collections.Generic;
using System.Text;
using Abp.Timing;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using CorarlERP.Lots;
using CorarlERP.Locations;

namespace CorarlERP.Items
{
    [Table("CarlErpItemLots")]
    public class ItemLot : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public long LotId { get; private set; }
        public Lot Lot { get; set; }

        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }

        public static ItemLot Create(int? tenantId, long creatorUserId, Guid itemId, long lotId)
        {
            return new ItemLot()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreationTime = Clock.Now,
                CreatorUserId = creatorUserId,
                ItemId = itemId,
                LotId = lotId
            };
        }

        public void Update(long userId, Guid itemId, long lotId)
        {
            LastModificationTime = Clock.Now;
            LastModifierUserId = userId;
            ItemId = itemId;
            LotId = lotId;
        }
    }
}
