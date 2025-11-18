using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PackageEditions
{
    [Table("CarlErpFeatures")]
    public class Feature : AuditedEntity<Guid>
    {
        [Required]
        [MaxLength(CorarlERPConsts.MaxNameLength)]
        public string Name { get; private set; }
        [MaxLength(CorarlERPConsts.MaxNameLength)]
        public string Description { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }
        public void Enable(long userId, bool enable)
        {
            IsActive = enable;
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
        }

        public static Feature Create(long userId, string name, string description, int sortOrder, bool isDefault)
        {
            return new Feature
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                Name = name,
                Description = description,
                SortOrder = sortOrder,
                IsDefault = isDefault,
                IsActive = true,
            };
        }

        public void Update(long userId, string name, string description, int sortOrder, bool isDefault)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            Name = name;
            Description = description;
            SortOrder = sortOrder;
            IsDefault = isDefault;
        }
    }


}
