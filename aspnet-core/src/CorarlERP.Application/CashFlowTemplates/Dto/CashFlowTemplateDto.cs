using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CashFlowTemplates.Dto
{
    public class CreateOrUpdateCashFlowTemplateDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CashFlowCategoryDto> Categories { get; set; }
        public List<CashFlowTemplateAccountDto> Accounts { get; set; }
        public bool SplitCashInAndCashOutFlow { get; set; }
    }

    public class CashFlowTemplateDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool SplitCashInAndCashOutFlow { get; set; }

        public List<CashFlowCategoryDto> Categories { get; set; }
        public List<CashFlowTemplateAccountDetailDto> Accounts { get; set; }
    }

    public class CashFlowTemplateListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool SplitCashInAndCashOutFlow { get; set; }
    }

    public class CashFlowTemplateSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
