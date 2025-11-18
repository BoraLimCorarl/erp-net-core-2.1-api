using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CorarlERP.Productions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ProductionPlans.Dto
{
    [AutoMapFrom(typeof(ProductionPlan))]
    public class ProductionPlanDetailOutput
    {
        public Guid Id { get; set; }
        public string DocumentNo { get; set; }
        public string Reference { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public ProductionPlanStatus Status { get; set; }
        public string ProductionPlanStatus { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string UserName { get; set; }
        public long? UserId { get; set; }
        public long? ProductionLineId { get; set; }
        public string ProductionLineName { get; set; }
        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal QtyYield { get; set; }
        public decimal NetWeightYield { get; set; }
        public string ProductionProcess { get; set; }
        public List<StandardCostGroupSummary> StandardCostGroups { get; set; }
        public List<StandardCostGroupSummary> IssueStandardCostGroups { get; set; }
    }

    public class ProductionPlanDetailGroupByOutput
    {
        public string KeyName { get; set; }
        public List<ProductionPlanDetailOutput> Items { get; set; }
    }


    public class ProductionPlanOutput
    {
        public long Id { get; set; }
        public string DocumentNo { get; set; }
        public int Reference { get; set; }
    }

    public class PageResultProductioinPlanSummary : PagedResultDto<ProductionPlanDetailOutput>
    {
        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalQtyBalance { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }
        public decimal TotalNetWeightBalance { get; set; }
        public decimal QtyYield { get; set; }
        public decimal NetWeightYield { get; set; }

        public IReadOnlyList<ProductionPlanSummary> Summaries { get; set; }
        public IReadOnlyList<ProductionPlanSummary> IssueSummaries { get; set; }

        public PageResultProductioinPlanSummary()
        {

        }

        public PageResultProductioinPlanSummary(
            int totalCount,
            IReadOnlyList<ProductionPlanDetailOutput> items,
            IReadOnlyList<ProductionPlanSummary> summaries,
            IReadOnlyList<ProductionPlanSummary> issueSummaries) :
            base(totalCount, items)
        {
            Summaries = summaries;
            IssueSummaries = issueSummaries;
        }
    }

    public class StandardCostGroupSummary
    {
        public Guid ProductionPlanId { get; set; }
        public string GroupName { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal QtyPercentage { get; set; }
        public decimal NetWeightPercentage { get; set; }
    }

    
}
