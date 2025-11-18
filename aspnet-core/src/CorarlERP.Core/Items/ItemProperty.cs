using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Items;
using CorarlERP.PropertyValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Items
{
    [Table("CarlErpItemProperties")]
    public class ItemProperty : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public long PropertyId { get; private set; }
        public Property Property { get; set; }
        
        public long? PropertyValueId { get; private set; }
        public PropertyValue PropertyValue { get; set; }

        [Required]
        public bool IsActive { get; private set; }

        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }

        public static ItemProperty Create(int? tenantId, long creatorUserId,  long? propertyValueId,
            long propertyId, Guid itemId)
        {
            return new ItemProperty()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreationTime =Clock.Now,
                CreatorUserId = creatorUserId,
                PropertyId = propertyId,
                PropertyValueId = propertyValueId,
                ItemId = itemId,
            };
        }

        public void Update(long userId, long? propertyValueId,
          long propertyId, Guid itemId)
        {
            LastModificationTime = Clock.Now;
            LastModifierUserId = userId;
            PropertyId = propertyId;
            PropertyValueId = propertyValueId;
            ItemId = itemId;
        }
    }
}
