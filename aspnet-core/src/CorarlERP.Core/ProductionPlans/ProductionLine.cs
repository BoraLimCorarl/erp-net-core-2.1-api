using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ProductionPlans 
{ 

    [Table("CarlErpProductionLines")]
    public class ProductionLine : AuditedEntity<long>, IMustHaveTenant
    {
        public const int MaxLocationNameLength = 512;
        public int TenantId { get; set; }

        [Required]
        [MaxLength(MaxLocationNameLength)]
        public string ProductionLineName { get; private set; }

     
        public string Memo { get; private set; }
        public bool IsActive { get; private set; }
        public static ProductionLine Create(int tenantId, long creatorUserId, string name, string memo)
        {
            return new ProductionLine()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ProductionLineName = name,
                Memo = memo,
                IsActive = true
            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

        public void Update(long lastModifiedUserId, string name, string memo)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ProductionLineName = name;
            Memo = memo;
        }


    }
}
