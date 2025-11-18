using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PropertyValues
{
    [Table("CarlErpProperties")]
    public class Property : AuditedEntity<long>, IMayHaveTenant
    {

        public const int MaxNameLength = 512;


        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; private set; }
        public int? TenantId { get; set; }
        [Required]
        public bool IsActive { get; private set; }

        public bool IsUnit { get; private set; }

        public bool IsItemGroup { get; private set; }
        public bool IsStandardCostGroup { get; private set; }

        public bool IsStatic { get; private set; }
        public bool IsRequired { get; private set; }

        public static Property Create(int? tenantId, long? creatorUserId, string name,bool isUnit,bool isRequired, bool isStatic,bool isItemGroup, bool isStandardCostGroup)
        {
            return new Property()
            {
                CreatorUserId = creatorUserId,
                Name = name,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                IsActive = true,
                IsUnit = isUnit,
                IsRequired = isRequired,
                IsStatic = isStatic,
                IsItemGroup = isItemGroup,
                IsStandardCostGroup = isStandardCostGroup
            };
        }
        public void Update(long lastModifiedUserId,string name,bool isUnit, bool isRequired, bool isStatic, bool isItemGroup, bool isStandardCostGroup)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            IsUnit = isUnit;
            IsRequired = isRequired;
            IsStatic = isStatic;
            IsItemGroup = isItemGroup;
            IsStandardCostGroup = isStandardCostGroup;
        }
        public void Enable(bool isEnable)
        {
            this.IsActive = isEnable;
        }           
    }
}
