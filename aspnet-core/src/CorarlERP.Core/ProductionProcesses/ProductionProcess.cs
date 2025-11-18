using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ProductionProcesses
{
    public enum ProductionProcessType
    {
        Both = 0,
        Receipt = 1,
        Issue = 2,
        None = 3,
    }

    [Table("CarlErpProductionProcess")]
    public class ProductionProcess : AuditedEntity<long>, IMustHaveTenant
    {
        public const int MaxLocationNameLength = 512;
        public int TenantId { get; set; }

        [Required]
        [MaxLength(MaxLocationNameLength)]
        public string ProcessName { get; private set; }

        public int SortOrder { get; private set; }
        [Required]
        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }

        public string Memo { get; private set; }
        public bool IsActive { get; private set; }
        public bool UseStandard { get; private set; }
        public bool IsRequiredProductionPlan { get; private set; }
        public ProductionProcessType ProductionProcessType { get; private set; }
        public static ProductionProcess Create(int tenantId, long creatorUserId, string processName, Guid accountId, string memo, int sortOrder, bool useStandard, bool isRequiredProductionPlan, ProductionProcessType type)
        {
            return new ProductionProcess()
            {
                SortOrder = sortOrder,
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ProcessName = processName,
                AccountId = accountId,
                Memo = memo,
                UseStandard = useStandard,
                IsRequiredProductionPlan = isRequiredProductionPlan,
                ProductionProcessType = type,
                IsActive = true
            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

        public void Update(long lastModifiedUserId, string processName, Guid accountId, string memo, int sortOrder, bool useStandard, bool isRequiredProductionPlan, ProductionProcessType type)
        {
            SortOrder = sortOrder;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ProcessName = processName;
            AccountId = accountId;
            Memo = memo;
            UseStandard = useStandard;
            IsRequiredProductionPlan = isRequiredProductionPlan;
            ProductionProcessType = type;
        }


    }
}
