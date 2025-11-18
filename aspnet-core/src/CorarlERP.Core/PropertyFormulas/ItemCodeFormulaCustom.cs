using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.PropertyFormulas
{


    [Table("CarlErpItemCodeFormulaCustoms")]
    public class ItemCodeFormulaCustom : AuditedEntity<long>, IMayHaveTenant
    {  
        public int? TenantId { get; set; }
        public string Prefix { get; private set; }
        public string ItemCode { get; private set; }
        public ItemCodeFormula ItemCodeFormula { get; private set; }
        public long ItemCodeFormulaId { get; private set; }
        public static ItemCodeFormulaCustom Create(int? tenantId, long? creatorUserId,string prefix, string itemCode, long itemCodeFormulaId)
        {
            return new ItemCodeFormulaCustom()
            {
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                CreationTime = Clock.Now,
                Prefix = prefix,
                ItemCode = itemCode,
                ItemCodeFormulaId = itemCodeFormulaId,
            };
        }
        public void Update(long? lastModifiedUserId, string prefix, string itemCode)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Prefix = prefix;
            ItemCode = itemCode;
           
        }
    }
}
