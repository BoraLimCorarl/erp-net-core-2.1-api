using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.ProductionPlans.Dto;
using CorarlERP.ProductionProcesses.Dto;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Productions.Dto
{

    public class ProductionPlanSummary
    {
        public string ProductionPlan { get; set; }
        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalQtyBalance {  get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }
        public decimal TotalNetWeightBalance { get; set; }
        public decimal QtyYield { get; set; }
        public decimal NetWeightYield { get; set; }
    }

    public class PageResultProductioinSummary : PagedResultDto<ProductionGetListOutput>
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

        public PageResultProductioinSummary()
        {

        }

        public PageResultProductioinSummary(
            int totalCount, 
            IReadOnlyList<ProductionGetListOutput> items, 
            IReadOnlyList<ProductionPlanSummary> summaries,
            IReadOnlyList<ProductionPlanSummary> issueSummaries) : 
            base(totalCount, items)
        {
            Summaries = summaries;
            IssueSummaries = issueSummaries;
        }
    }

    public class ProductionGetListOutput
    {
        public Guid Id { get; set; }
        public TransactionStatus StatusCode { get; set; }

        public int CountItem { get; set; }
        public TransferStatus ReceiveStatus { get; set; }
        public string ProductionNo { get; set; }
        public string Reference { get; set; }
        public DateTime ProductionDate { get; set; }
        public bool CanDrafOrVoid { get; set; }
        public LocationSummaryOutput FromLocation { get; set; }

        public ProductionProcessSummaryOutput ProductionProcess { get; set; }

        public LocationSummaryOutput ToLocation { get; set; }
        public UserDto User { get; set; }
        public CalculationState CalculationState { get; set; }
        public string ProductionPlanNo { get; set; }

        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }

        public List<StandardGroupSummary> StandardGroups { get; set; }
        public List<StandardGroupSummary> IssueStandardGroups { get; set; }
    }


    public class ProductionOrderReportOutput
    {
        public Guid Id { get; set; }
        public TransactionStatus Status { get; set; }

        public TransferStatus ReceiveStatus { get; set; }
        public string ProductionNo { get; set; }
        public string Reference { get; set; }
        public DateTime ProductionDate { get; set; }
        public string FromLocation { get; set; }

        public string ProductionProcess { get; set; }
        public string Description { get; set; }

        public string ToLocation { get; set; }
        public string User { get; set; }
        //public CalculationState CalculationState { get; set; }
        public string ProductionPlanNo { get; set; }

        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }

        public List<StandardGroupSummary> StandardGroups { get; set; }
        public List<StandardGroupSummary> IssueStandardGroups { get; set; }
    }


    public class ProductionGroupByOutput
    {
        public string KeyName { get; set; }
        public List<ProductionOrderReportOutput> Items { get; set; }
    }

}
