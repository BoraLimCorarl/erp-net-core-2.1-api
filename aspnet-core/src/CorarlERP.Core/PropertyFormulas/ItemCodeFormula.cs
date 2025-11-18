using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PropertyFormulas
{
    [Table("CarlErpItemCodeFormulas")]
    public class ItemCodeFormula : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 512;
        public int? TenantId { get; set; }
        public bool IsActive { get; private set; }
        public bool UseItemProperty { get; private set; }
        public bool UseCustomGenerate { get; private set; }     
        public bool UseManual { get; private set; }
        public static ItemCodeFormula Create(int? tenantId, long? creatorUserId, bool useItemProperty , bool useCustomGenerate,bool useManual)
        {
            return new ItemCodeFormula()
            {
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                UseCustomGenerate = useCustomGenerate,
                UseItemProperty = useItemProperty,
                IsActive = true,
                CreationTime = Clock.Now,
                UseManual = useManual,

            };
        }
        public void Update(long? lastModifiedUserId, bool useItemProperty, bool useCustomGenerate,bool useManual)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            UseItemProperty = useItemProperty;
            UseCustomGenerate = useCustomGenerate;
            UseManual = useManual;
        }
        public void Enable (bool active)
        {
            this.IsActive = active;
        }
    }
}
