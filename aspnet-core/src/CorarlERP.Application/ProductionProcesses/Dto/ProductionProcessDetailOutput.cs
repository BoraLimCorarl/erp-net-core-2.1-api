using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ProductionProcesses.Dto
{
    [AutoMapFrom(typeof(ProductionProcess))]
    public class ProductionProcessDetailOutput
    {
        public long Id { get; set; }
        public string ProcessName { get; set; }
        public string Memo { get; set; }
        public Guid AccountId { get; set; }
        public int SortOrder { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public bool IsActive { get; set; }
        public bool UseStandard { get; set; }
        public bool IsRequiredProductionPlan { get; set; }
        public ProductionProcessType ProductionProcessType { get; set; }

    }

    public class ProductionProcessSummaryOutput
    {
        public long Id { get; set; }
        public string ProcessName { get; set; }
        public int SortOrder { get; set; }
        public string Memo { get; set; }
        public bool UseStandard { get; set; }
    }

    public class ProductionProcessSummaryDto
    {
        public long Id { get; set; }
        public string ProcessName { get; set; }
    }

}
