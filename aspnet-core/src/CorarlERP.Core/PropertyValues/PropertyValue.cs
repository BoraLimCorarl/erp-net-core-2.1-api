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
    [Table("CarlErpPropertyValues")]
    public class PropertyValue : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxValueLength = 512;
        public const int MaxCodeLength = 4;

        public int? TenantId { get; set; }
        [Required]
        [MaxLength(MaxValueLength)]
        public string Value { get; private set; }

        public long? PropertyId { get; private set; }
        public Property Property { get; private set; }
        public void SetProperty(long id) => PropertyId = id;

        public decimal NetWeight { get; private set; }
        public bool IsBaseUnit { get; private set; }
        public long? BaseUnitId { get; private set; }
        public PropertyValue BaseUnit { get; private set; }
        public void SetBaseUnit(long id) => BaseUnitId = id;
        public decimal Factor { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }
       
        [MaxLength(MaxCodeLength)]
        public string Code { get; private set; }
        public static PropertyValue Create(int? tenantId, 
               long? creatorUserId, long? propertyId, string value,
               decimal netWeight, bool isDefault,
               string code, bool isBaseUnit, long? baseUnitId, decimal factor)
        {
            return new PropertyValue()
            {
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                IsActive = true,
                Value = value,
                PropertyId = propertyId,
                NetWeight = netWeight,
                IsDefault = isDefault,
                Code = code,
                IsBaseUnit = isBaseUnit,
                BaseUnitId = baseUnitId,
                Factor = factor
            };

        }

        public static PropertyValue Create(int? tenantId, long? creatorUserId,
            Property property, string value,decimal netWeight, bool isDefault,string code, bool isBaseUnit, long? baseUnitId, decimal factor)
        {
            return new PropertyValue()
            {
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                IsActive = true,
                Value = value,
                Property = property,
                NetWeight = netWeight,
                IsDefault = isDefault,
                Code = code,
                IsBaseUnit = isBaseUnit,
                BaseUnitId = baseUnitId,
                Factor = factor
            };

        }
        public void Update(long lastModifiedUserId, string value,decimal netWeight, bool isDefault,string code, bool isBaseUnit, long? baseUnitId, decimal factor)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Value = value;
            NetWeight = netWeight;
            IsDefault = isDefault;
            Code = code;
            IsBaseUnit = isBaseUnit;
            BaseUnitId = baseUnitId;
            Factor = factor;
        }

        public void Enable(bool isEnable)
        {
            this.IsActive = isEnable;
        }
      
    }
}


