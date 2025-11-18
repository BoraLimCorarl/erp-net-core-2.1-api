using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.TransactionTypes
{
    [Table("CarlErpTransactionTypes")]
    public class TransactionType : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get ; set; }
        public const int MaxTransactionTypeNameLength = 512;
        
        [Required]
        [MaxLength(MaxTransactionTypeNameLength)]
        public string TransactionTypeName { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public bool IsPOS { get; private set; }
        public void  SetIsPOS(bool isPOS) { IsPOS = isPOS;}


        public static TransactionType Create(int? tenantId, long creatorUserId, string transactionTypeName)
        {
            return new TransactionType()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                TransactionTypeName = transactionTypeName


            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string transactionTypeName)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TransactionTypeName = transactionTypeName;
        }
    }
}
