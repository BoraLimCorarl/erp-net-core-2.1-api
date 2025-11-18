using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.POSSettings
{
    [Table("CarlErpPOSSettings")]
    public class POSSetting : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public bool AllowSelectCustomer { private set; get; }
        public bool UseMemberCard { get; private set; }
        public bool IsActive { get; private set; }

        public static POSSetting Create(int? tenantId, long creatorUserId, bool allowSelectCustomer, bool useMemberCard)
        {
            return new POSSetting()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                UseMemberCard = useMemberCard,
                AllowSelectCustomer =allowSelectCustomer,
            };
        }

        public void Update(long lastModifiedUserId, bool allowSelectCustomer, bool useMemberCard)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            AllowSelectCustomer = allowSelectCustomer;
            UseMemberCard = useMemberCard;

        }

    }
}
