using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.VendorTypes
{
    [Table("CarlErpVendorTypeMembers")]
    public class VendorTypeMember : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxVendorTypeNameLength = 512;
        public int? TenantId { get; set; }

        public long MemberId { get; private set; } 
        public User Member { get; private set; }


        public long VendorTypeId { get; private set; }
        public VendorType VendorType { get; private set; }


        public static VendorTypeMember Create(int? tenantId, long creatorUserId,  long memberId, long vendorTypeId)
        {
            return new VendorTypeMember()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                MemberId = memberId,
                VendorTypeId = vendorTypeId
               
            };
        }
      
        public void Update(long lastModifiedUserId, long memberId, long vendorTypeId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            MemberId = memberId;
            VendorTypeId = vendorTypeId;
        }
    }
}
