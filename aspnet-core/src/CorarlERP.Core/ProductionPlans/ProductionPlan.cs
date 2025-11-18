using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CorarlERP.Locations;

namespace CorarlERP.ProductionPlans
{
    public enum ProductionPlanStatus
    {
        Open = 1,
        Closed = 2
    }

    [Table("CarlErpProductionPlans")]
    public class ProductionPlan : BaseAuditedEntity<Guid>
    {
        [Required]
        public string DocumentNo { get; private set; }
        public string Reference { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string Description { get; private set; }
        public string ProductionProcess { get; private set; }
        public ProductionPlanStatus Status { get; private set; }
        public long LocationId { get; private set; }
        public Location Location { get; private set; }

        public long? ProductionLineId { get; private set; }
        public ProductionLine ProductionLine { get; private set; }

        public decimal TotalIssueQty { get; private set; }
        public decimal TotalReceiptQty { get; private set; }
        public decimal TotalIssueNetWeight { get; private set; }
        public decimal TotalReceiptNetWeight { get; private set; }

        public void UpdateSummary(decimal totalIssueQty, decimal totalReceiptQty, decimal totalIssueNetWeight, decimal totalReceiptNetWeight)
        {
            TotalIssueQty = totalIssueQty;
            TotalReceiptQty = totalReceiptQty;
            TotalIssueNetWeight = totalIssueNetWeight;
            TotalReceiptNetWeight = totalReceiptNetWeight;
        }

        public static ProductionPlan Create(int tenantId, long userId, long locationId, string documentNo, string reference, DateTime? startDate, DateTime? endDate, string description, long? productionLineId, string productionProcess)
        {
            return new ProductionPlan
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                LocationId = locationId,
                DocumentNo = documentNo,
                Reference = reference,
                StartDate = startDate,
                EndDate = endDate,
                Description = description,
                ProductionLineId = productionLineId,
                Status = ProductionPlanStatus.Open,
                ProductionProcess = productionProcess
            };
        }

        public void Update(long userId, long locationId, string documentNo, string reference, DateTime? startDate, DateTime? endDate, string description, long? productionLineId, string productionProcess)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            LocationId = locationId;
            DocumentNo = documentNo;
            Reference = reference;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            ProductionLineId = productionLineId;
            ProductionProcess = productionProcess;
        }

        public void Close(long userId)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            Status = ProductionPlanStatus.Closed;
        }

        public void Open(long userId)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            Status = ProductionPlanStatus.Open;
        }
    }
}
