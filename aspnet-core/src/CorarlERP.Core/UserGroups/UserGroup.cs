using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.UserGroups
{
    [Table("CarlErpUserGroups")]
    public class UserGroup : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get;set; }
        public string Name { get; set; }
        public bool IsActive { get; private set; }
        public long? LocationId { get; private set; }
        public Location Location { get; private set; }

        public static UserGroup Create(int? tenantId,long? creatorUserId,string name )
        {
            return new UserGroup()
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                Name = name,
                IsActive = true
            };
        }

        public void Update(long lastModifiedUserId, string name)
        {          
            LastModifierUserId = lastModifiedUserId;                         
            Name = name;

        }

        public void UpdateLocationId(long locationId)
        {
            LocationId = locationId;
        }

        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
    }
}
