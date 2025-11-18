using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.CustomerTypes
{
    [Table("CarlErpCustomerTypeMembers")]
    public class CustomerTypeMember : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public long MemberId { get; private set; } 
        public User Member { get; private set; }


        public long CustomerTypeId { get; private set; }
        public CustomerType CustomerType { get; private set; }


        public static CustomerTypeMember Create(int? tenantId, long creatorUserId,  long memberId, long customerTypeId)
        {
            return new CustomerTypeMember()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                MemberId = memberId,
                CustomerTypeId = customerTypeId

            };
        }
      
        public void Update(long lastModifiedUserId, long memberId, long customerTypeId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            MemberId = memberId;
            CustomerTypeId = customerTypeId;
        }
    }
}
