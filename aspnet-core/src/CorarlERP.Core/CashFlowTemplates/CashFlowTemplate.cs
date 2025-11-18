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
    [Table("CarlErpCashFlowTemplates")]
    public class CashFlowTemplate : AuditedEntity<Guid>, IMustHaveTenant
    {
        public const int MaxNameLength = 512;

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }
        public int TenantId { get; set; }

        public void Enable(bool isActive) => IsActive = isActive;
        public void SetDefault(bool isDefault) => IsDefault = isDefault;
        public bool SplitCashInAndCashOutFlow { get; private set; }

        public static CashFlowTemplate Create(int tenantId, long? creatorUserId, string name, string description, bool splitCashInAndCashOutFlow)
        {
            return new CashFlowTemplate
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,               
                CreationTime = Clock.Now,
                TenantId = tenantId,
                Name = name,
                Description = description,
                IsActive = true,
                SplitCashInAndCashOutFlow = splitCashInAndCashOutFlow
            };
        }

        public void Update(long? lastModifiedUserId, string name, string description, bool splitCashInAndCashOutFlow)

        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            Description = description;
            SplitCashInAndCashOutFlow = splitCashInAndCashOutFlow;

        }
    }
}
