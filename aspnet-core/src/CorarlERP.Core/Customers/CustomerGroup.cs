using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.UserGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Customers
{
    [Table("CarlErpCustomerGroups")]
    public class CustomerGroup : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; }

        public Guid UserGroupId { get; private set; }
        public UserGroup UserGroup { get; private set; }

        public static CustomerGroup Create(int tenantId, long creatorUserId, Guid customerId, Guid userGroupId)
        {
            return new CustomerGroup()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                UserGroupId = userGroupId,
                CustomerId = customerId
            };
        }
        public void Update(long lastModifiedUserId, Guid customerId, Guid userGroupId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            UserGroupId = userGroupId;
            CustomerId = customerId;
        }

    }
}
