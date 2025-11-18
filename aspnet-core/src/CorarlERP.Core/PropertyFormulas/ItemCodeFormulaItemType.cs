using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ItemTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PropertyFormulas
{

    [Table("CarlErpItemCodeFormulaItemTypes")]
    public class ItemCodeFormulaItemType : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId {get ; set; }
        public ItemCodeFormula ItemCodeFormula { get; private set; }
        public long ItemCodeFormulaId { get; private  set; }
        public ItemType ItemType { get; private set; }
        public long ItemTypeId { get; private set; }

        public static ItemCodeFormulaItemType Create(int? tenantId, long? creatorUserId, long itemCodeFormulaId,long itemTypeId)
        {
            return new ItemCodeFormulaItemType()
            {
                CreatorUserId = creatorUserId,
                TenantId = tenantId,                         
                CreationTime = Clock.Now,
                ItemCodeFormulaId = itemCodeFormulaId,
                ItemTypeId = itemTypeId,

            };
        }
        public void Update(long? lastModifiedUserId, long itemCodeFormulaId, long itemTypeId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ItemCodeFormulaId = itemCodeFormulaId;
            ItemTypeId = itemTypeId;
        }
       
    }
}
