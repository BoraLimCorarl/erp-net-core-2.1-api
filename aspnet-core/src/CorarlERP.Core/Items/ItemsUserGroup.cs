using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.UserGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Items
{
    [Table("CarlErpItemUserGroups")]
    public class ItemsUserGroup : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set ; }
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public Guid UserGroupId { get; private set; }
        public UserGroup UserGroup { get; private set; }

        public static ItemsUserGroup Create(int tenantId, long creatorUserId, Guid itemId, Guid userGroupId)
        {
            return new ItemsUserGroup()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                UserGroupId = userGroupId,
                ItemId = itemId
            };
        }

        public void Update(long lastModifiedUserId, Guid itemId, Guid userGroupId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            UserGroupId = userGroupId;
            ItemId = itemId;
        }

    }
}
