using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.ProductionPlans;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ProductionLines.Dto
{
    [AutoMapFrom(typeof(ProductionLine))]
    public class ProductionLineDetailOutput
    {
        public long Id { get; set; }
        public string ProductionLineName { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }
    }

}
