using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.CustomerTypes
{
    [Table("CarlErpCustomerTypes")]
    public class CustomerType : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxCustomerTypeNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxCustomerTypeNameLength)]
        public string CustomerTypeName { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public static CustomerType Create(int? tenantId, long creatorUserId, string customerTypeName)
        {
            return new CustomerType()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                CustomerTypeName = customerTypeName
               

            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string customerTypeName)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CustomerTypeName = customerTypeName;          
        }
    }
}
