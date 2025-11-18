using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Classes
{
    [Table("CarlErpClasses")]
    public class Class : AuditedEntity<long>, IMayHaveTenant
    {

        public const int MaxClassNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxClassNameLength)]
        public string ClassName { get; private set; }

        public bool ClassParent { get; private set; }

        public long? ParentClassId { get;private set; }
        public Class ParentClass { get;private set;}

        [Required]
        public bool IsActive { get; private set; }

        public static Class Create(int? tenantId, long creatorUserId,string className,bool classParent,long? parentClassId)
        {
            return new Class()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                ClassParent = classParent,
                ClassName = className,
                ParentClassId = parentClassId,

            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update (long lastModifiedUserId,string className,bool classParent, long? parentClassId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ClassName = className;
            ClassParent = classParent;
            ParentClassId = parentClassId;
        }
    }
}
