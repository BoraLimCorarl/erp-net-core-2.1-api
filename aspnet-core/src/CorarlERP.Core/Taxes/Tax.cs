using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Taxes
{
    [Table("CarlErpTaxes")]
    public class Tax : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxTaxNameLength = 32;

        #region Properties
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxTaxNameLength)]
        public string TaxName { get; private set; }

        [Required]
        public decimal TaxRate { get; private set; } //store in decimal 0.0000 decimal(1,4)

        [Required]
        public bool IsActive { get; private set; }
        #endregion

    
        public static Tax Create(int? tenantId, 
                                 long? creatorUserId, 
                                 string taxName, 
                                 decimal taxRate)
        {
            return new Tax()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TaxName = taxName,
                TaxRate = taxRate,
                IsActive = true
            };
        }

        public void Update(long lastModifiedUserId, string taxName, decimal taxRate)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.TaxName = taxName;
            this.TaxRate = taxRate;
           
            
        }

        public void Enable(bool isEnable)
        {
            this.IsActive = isEnable;
        }
    }
}
