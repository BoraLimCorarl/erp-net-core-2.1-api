using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.ProductionPlans.Dto
{
    public class CreateOrUpdateProductionPlanInput
    {
        public Guid Id { get; set; }
        public string DocumentNo { get; set; }
        public string Reference { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string ProductionProcess { get; set; }
        public ProductionPlanStatus Status { get; set; }
        public long LocationId { get; set; }
        public long? ProductionLineId { get; set; }
    }
}
