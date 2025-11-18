using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.InventoryTransactionTypes
{
    [Table("CarlErpJournalTransactionTypes")]
    public class JournalTransactionType : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; private set; }
        public bool IsIssue { get; private set; }
        public bool IsDefault { get; private set; }
        public bool Active { get; private set; }
        public InventoryTransactionType InventoryTransactionType { get; private set; }

        public static JournalTransactionType Create(int? tenantId, long creatorUserId,string name,bool isIssue,bool isDefault, InventoryTransactionType inventoryTransactionType)
        {
            return new JournalTransactionType()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Name = name,
                IsDefault = isDefault,
                IsIssue = isIssue,
                InventoryTransactionType = inventoryTransactionType,
                Active = true
            };
                
        }
        public void Update (long lastModifiedUserId, string name, bool isIssue, bool isDefault, InventoryTransactionType inventoryTransactionType)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            IsDefault = isDefault;
            IsIssue = isIssue;
            InventoryTransactionType = inventoryTransactionType;
        }
        public void UpdateStatus(bool status)
        {
            this.Active = status;
        }
    }

}
