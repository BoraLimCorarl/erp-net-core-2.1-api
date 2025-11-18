using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CashFlowTemplates.Dto
{
    public class CashFlowTemplateAccountDto
    {
        public Guid? Id { get; set; }
        public Guid TemplateId { get; set; }
        public Guid? InAccountGroupId { get; set; }
        public Guid? OutAccountGroupId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class CashFlowTemplateAccountDetailDto : CashFlowTemplateAccountDto
    {
        public int InAccountGroupSortOrder { get; set; }
        public int OutAccountGroupSortOrder { get; set; }
        public string InAccountGroupName { get; set; }
        public string OutAccountGroupName { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public int CategorySortOrder { get; set; }
        public string CategoryName { get; set; }
        public bool CashTransfer { get; set; }
    }

    public class GetCashFlowCategoryWithAccountOutput
    {
        public List<CashFlowCategoryDto> Categories { get; set; }
        public List<CashFlowTemplateAccountDetailDto> Accounts { get; set; }
    }


    public class CashFlowCategoryDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public CashFlowCategoryType Type { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }
}
