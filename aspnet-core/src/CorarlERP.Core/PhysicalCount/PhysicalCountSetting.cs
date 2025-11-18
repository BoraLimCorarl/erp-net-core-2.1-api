using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Timing;

namespace CorarlERP.PhysicalCount
{
    [Table("CarlErpPhysicalCountSettings")]
    public class PhysicalCountSetting : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public bool No { get; set; }
        public bool ItemCode { get; set; }
        public bool LotName { get; set; }
        public bool DiffQty { get; set; }

        public static PhysicalCountSetting Create(int tenantId, long userId, bool no, bool itemCode, bool lotName, bool diffQty)
        {
            return new PhysicalCountSetting
            {
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                No = no,
                ItemCode = itemCode,
                LotName = lotName,
                DiffQty = diffQty
            };
        }

        public void Udpate(long userId, bool no, bool itemCode, bool lotName, bool diffQty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            No = no;
            ItemCode = itemCode;
            LotName = lotName;
            DiffQty = diffQty;
        }
    }
}
