using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.ProductionPlans;
using CorarlERP.ProductionProcesses;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Productions.Dto
{
    public class ProductionCalculationInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class GetListProductionInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid?> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsForItemIssue { get; set; }
        public bool IsForItemReceipt { get; set; }

        public List<TransactionStatus?> Status { get; set; }
        public List<TransferStatus?> DeliveryStatus { get; set; }
        public List<long?> FromLocations { get; set; }
        public List<long?> Users { get; set; }
        public List<long?> ToLocations { get; set; }
        public List<long?> Locations { get; set; }
        public List<CalculationState> CalculationStates { get; set; }
        public List<Guid> ProductionPlans { get; set; }
        public List<long?> ProductionLines { get; set; }
        public List<ProductionPlanStatus> ProductionPlanStatus { get; set; }
        public List<long?> ProductionProcess { get; set; }
        public bool NoProductionPlan { get; set; }
        public List<ProductionProcessType> ProductionProcessTypes { get; set; }

        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "ProductionNo";
            }

        }
    }
}
