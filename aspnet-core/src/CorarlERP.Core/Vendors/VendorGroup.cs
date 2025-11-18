using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.UserGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Vendors
{
    [Table("CarlErpVendorGroups")]
    public class VendorGroup : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid VendorId { get; private set; }
        public Vendor Vendor { get; private set; }

        public Guid UserGroupId { get; private set; }
        public UserGroup UserGroup { get; private set; }

        public static VendorGroup Create(int tenantId, long creatorUserId, Guid vendorId, Guid userGroupId)
        {
            return new VendorGroup()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                UserGroupId = userGroupId,
                VendorId = vendorId
            };
        }
        public void Update(long lastModifiedUserId, Guid vendorId, Guid userGroupId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            UserGroupId = userGroupId;
            VendorId = vendorId;
        }

    }
}
