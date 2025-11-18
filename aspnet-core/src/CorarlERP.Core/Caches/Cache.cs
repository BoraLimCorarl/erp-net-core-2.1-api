using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Caches
{
    [Table("CarlErpCache")]
    public class Cache : AuditedEntity<long>, IMayHaveTenant
    {

        public const int MaxClassNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxClassNameLength)]
        public string KeyName { get; private set; }

        public string KeyValue { get; private set; }

        public static Cache Create(int? tenantId, long? creatorUserId, string key, string value)
        {
            return new Cache()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                KeyName = key,
                KeyValue = value

            };
        }

        public void Update(long? lastModifiedUserId, string key, string value)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            KeyName = key;
            KeyValue = value;
        }

    }
}
