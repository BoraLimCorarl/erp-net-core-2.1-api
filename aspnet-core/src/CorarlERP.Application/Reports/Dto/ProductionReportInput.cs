using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.ProductionPlans;
using CorarlERP.ProductionPlans.Dto;
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;


namespace CorarlERP.Reports.Dto
{
    public class ProductionReportInput: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public string GroupBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<long?> Users { get; set; }
        public List<ProductionPlanStatus> PlanStatuses { get; set; }
        public List<long> Locations { get; set; }
        public List<long> ProductionLines { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "DocumentNo";
            }
        }
    }

    public class GetProductionReportInput : ProductionReportInput
    {
        public ReportOutput ReportOutput { get; set; }
    }


    public class ProductionOrderReportInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public string GroupBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<long?> Users { get; set; }
        public List<long> Locations { get; set; }
        public List<long> ProductionLines { get; set; }

        public List<TransactionStatus?> Status { get; set; }
        public List<TransferStatus?> DeliveryStatus { get; set; }

        public List<CalculationState> CalculationStates { get; set; }
        public List<Guid> ProductionPlans { get; set; }
        public List<ProductionPlanStatus> ProductionPlanStatus { get; set; }
        public List<long?> ProductionProcess { get; set; }
        public bool NoProductionPlan { get; set; }
        public List<ProductionProcessType> ProductionProcessTypes { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "DocumentNo";
            }
        }
    }

    public class GetProductionOrderReportInput : ProductionOrderReportInput
    {
        public ReportOutput ReportOutput { get; set; }
    }
}
