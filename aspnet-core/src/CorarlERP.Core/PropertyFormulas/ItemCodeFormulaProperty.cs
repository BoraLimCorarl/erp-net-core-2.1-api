using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.PropertyValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PropertyFormulas
{


    [Table("CarlErpItemCodeFormulaProperties")]
    public class ItemCodeFormulaProperty : AuditedEntity<long>, IMayHaveTenant
    {

        public const int MaxSeparatorLength = 2;
        public int? TenantId { get; set; }
        public Property Property { get; private set; }
        public long PropertyId { get; private set; }

        public ItemCodeFormula ItemCodeFormula { get; private set; }
        public long ItemCodeFormulaId { get; private set; }

        public int SortOrder { get; private set; }      
        [MaxLength(MaxSeparatorLength)]
        public string Separator { get; private set; }

        public static ItemCodeFormulaProperty Create(int? tenantId, long? creatorUserId, int sortOrder, long itemCodeFormulaId, long propertyId,string separator)
        {
            return new ItemCodeFormulaProperty()
            {
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                CreationTime = Clock.Now,
                SortOrder = sortOrder,
                ItemCodeFormulaId = itemCodeFormulaId,
                PropertyId = propertyId,
                Separator = separator,

            };
        }
        public void Update(long? lastModifiedUserId, int sortOrder, long propertyId,string separator)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SortOrder = sortOrder;           
            PropertyId = propertyId;
            Separator = separator;
        }
    }
}
