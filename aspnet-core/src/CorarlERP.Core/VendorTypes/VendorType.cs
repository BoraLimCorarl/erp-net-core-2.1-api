using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.VendorTypes
{
    [Table("CarlErpVendorTypes")]
    public class VendorType : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxVendorTypeNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxVendorTypeNameLength)]
        public string VendorTypeName { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public static VendorType Create(int? tenantId, long creatorUserId, string vendorTypeName)
        {
            return new VendorType()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                VendorTypeName = vendorTypeName
               

            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string customerTypeName)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            VendorTypeName = customerTypeName;          
        }
    }
}
