using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Authorization.Users;
using CorarlERP.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP
{
    public abstract class BaseAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IMayHaveTenant
    {
        
        public virtual User LastModifierUser { get; protected set; }
        public virtual User CreatorUser { get; protected set; }
        public int? TenantId { get; set; }
    }
}
