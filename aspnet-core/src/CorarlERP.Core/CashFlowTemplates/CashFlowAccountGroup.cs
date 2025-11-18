using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.CashFlowTemplates
{
    [Table("CarlErpCashFlowAccountGroups")]
    public class CashFlowAccountGroup : AuditedEntity<Guid>, IMustHaveTenant
    {
        public const int MaxNameLength = 512;

        [Required]
        public int SortOrder { get; private set; } 

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        public string Description { get; set; }
        public int TenantId { get; set; }

        public static CashFlowAccountGroup Create(int tenantId, long? creatorUserId, string name, int sortOrder, string description)
        {
            return new CashFlowAccountGroup
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                Name = name,
                SortOrder = sortOrder,
                Description = description,

            };
        }

        public void Update(long? lastModifiedUserId, string name, int sortOrder, string description)

        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            SortOrder = sortOrder;
            Description = description;

        }
    }
}
