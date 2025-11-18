using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ProductionPlans.Dto
{
    public class ProductionPlanCalculationInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class GetProductionPlanListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
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
}
