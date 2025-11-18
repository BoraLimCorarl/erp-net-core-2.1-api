using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.PropertyValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Productions
{

    [Table("CarlErpProductionStandardCostGroups")]
    public class ProductionStandardCostGroup : AuditedEntity<Guid>, IMustHaveTenant
    {
        public const int MaxLocationNameLength = 512;
        public int TenantId { get; set; }

        public Guid ProductionId { get; private set; }
        public Production Production { get; private set; }

        public long? StandardCostGroupId { get; private set; }
        public PropertyValue StandardCostGroup { get; private set; }

        public decimal TotalQty { get; private set; }
        public decimal TotalNetWeight { get; private set; }


        public static ProductionStandardCostGroup Create(int tenantId, long creatorUserId, Guid productionId, long? standardCostGroupId, decimal totalQty, decimal totalNetWeight)
        {
            return new ProductionStandardCostGroup()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ProductionId = productionId,
                StandardCostGroupId = standardCostGroupId,
                TotalQty = totalQty,
                TotalNetWeight = totalNetWeight
            };
        }

        public void Update(long lastModifiedUserId, Guid productionId, long? standardCostGroupId, decimal totalQty, decimal totalNetWeight)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ProductionId = productionId;
            StandardCostGroupId = standardCostGroupId;
            TotalQty = totalQty;
            TotalNetWeight = totalNetWeight;
        }


    }
}
