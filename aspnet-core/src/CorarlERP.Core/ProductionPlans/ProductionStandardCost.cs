using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.PropertyValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ProductionPlans
{

    [Table("CarlErpProductionStandardCosts")]
    public class ProductionStandardCost : AuditedEntity<Guid>, IMustHaveTenant
    {
        public const int MaxLocationNameLength = 512;
        public int TenantId { get; set; }

        public Guid ProductionPlanId { get; private set; }
        public ProductionPlan ProductionPlan { get; private set; }

        public long? StandardCostGroupId { get; private set; }
        public PropertyValue StandardCostGroup { get; private set; }

        public decimal TotalQty { get; private set; }
        public decimal TotalNetWeight { get; private set; }

        public decimal QtyPercentage { get; private set; }
        public decimal NetWeightPercentage { get; private set; }


        public static ProductionStandardCost Create(int tenantId, long creatorUserId, Guid productionPlanId, long? standardCostGroupId, decimal totalQty, decimal totalNetWeight, decimal qtyPercentage, decimal netweightPercentage)
        {
            return new ProductionStandardCost()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ProductionPlanId = productionPlanId,
                StandardCostGroupId = standardCostGroupId,
                TotalQty = totalQty,
                TotalNetWeight = totalNetWeight,
                QtyPercentage = qtyPercentage,
                NetWeightPercentage = netweightPercentage
            };
        }

        public void Update(long lastModifiedUserId, Guid productionPlanId, long? standardCostGroupId, decimal totalQty, decimal totalNetWeight, decimal qtyPercentage, decimal netweightPercentage)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ProductionPlanId = productionPlanId;
            StandardCostGroupId = standardCostGroupId;
            TotalQty = totalQty;
            TotalNetWeight = totalNetWeight;
            QtyPercentage = qtyPercentage;
            NetWeightPercentage = netweightPercentage;
        }


    }
}
