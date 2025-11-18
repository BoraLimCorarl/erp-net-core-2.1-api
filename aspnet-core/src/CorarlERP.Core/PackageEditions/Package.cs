using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PackageEditions
{
   
    [Table("CarlErpPackages")]
    public class Package : AuditedEntity<Guid>
    {
        [Required]
        [MaxLength(CorarlERPConsts.MaxNameLength)]
        public string Code { get; private set; }
        [Required]
        [MaxLength(CorarlERPConsts.MaxNameLength)]
        public string Name { get; private set; }
     
        [MaxLength(CorarlERPConsts.MaxNameLength)]
        public string Description { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }
        public void Enable(long userId, bool enable)
        {
            IsActive = enable;
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
        }

        public static Package Create(long userId, string code, string name, string description, int sortOrder)
        {
            return new Package
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                Code = code,
                Name = name,
                Description = description,
                SortOrder = sortOrder,
                IsActive = true,
            };
        }

        public void Update(long userId, string code, string name, string description, int sortOrder)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            Code = code;
            Name = name;
            Description = description;
            SortOrder = sortOrder;
        }
    }
}
