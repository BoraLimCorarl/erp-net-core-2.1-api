using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.UserGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PaymentMethods
{
    [Table("CarlErpPaymentMethodUserGroups")]
    public class PaymentMethodUserGroup : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set ; }
        public Guid PaymentMethodId { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }

        public Guid UserGroupId { get; private set; }
        public UserGroup UserGroup { get; private set; }

        public static PaymentMethodUserGroup Create(int tenantId, long creatorUserId, Guid paymentMethodId, Guid userGroupId)
        {
            return new PaymentMethodUserGroup()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                UserGroupId = userGroupId,
                PaymentMethodId = paymentMethodId
            };
        }

        public void Update(long lastModifiedUserId, Guid paymentMethodId, Guid userGroupId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            UserGroupId = userGroupId;
            PaymentMethodId = paymentMethodId;
        }

    }
}
