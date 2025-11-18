using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PhysicalCount
{
    [Table("CarlErpPhysicalCountItemFilters")]
    public class PhysicalCountItemFilter : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid PhysicalCountId { get; private set; }
        public PhysicalCount PhysicalCount { get; private set; }
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }
     
        public static PhysicalCountItemFilter Create(int? tenantId, long? creatorUserId, Guid physicalCountId, Guid itemId)
        {
            return new PhysicalCountItemFilter
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                PhysicalCountId = physicalCountId,
                ItemId = itemId
            };
        }


        public void Update(
            long lastModifiedUserId,
            Guid itemId)
        {
            LastModifierUserId = lastModifiedUserId;
            ItemId = itemId;
        }
    }
}
