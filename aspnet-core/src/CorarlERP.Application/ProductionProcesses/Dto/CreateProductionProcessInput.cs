using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.ProductionProcesses.Dto
{
    public class CreateOrUpdateProductionProcessInput
    {
        public long? Id { get; set; }
        public Guid AccountId { get; set; }

        [Required]
        public string ProcessName { get; set; }
        public int SortOrder { get; set; }

        public string Memo { get; set; }
        public bool UseStandard { get; set; }
        public bool IsRequiredProductionPlan { get; set; }
        public ProductionProcessType ProductionProcessType { get; set; }
    }
}
