using Abp.Application.Services.Dto;
using CorarlERP.Productions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Common.Dto
{
    public class PagedResultWithTotalColuumnsDto<T> : PagedResultDto<T>
    {
        public PagedResultWithTotalColuumnsDto(int totalCount, IReadOnlyList<T> items, Dictionary<string, decimal> totalResult) : base(totalCount, items)
        {
            this.TotalResult = totalResult;
        }

        //column name to get to total
        public Dictionary<string, decimal> TotalResult { get; set; }

        public PagedResultWithTotalColuumnsDto(int totalCount, IReadOnlyList<T> items, Dictionary<string, Dictionary<string, decimal>> totalResults) : base(totalCount, items)
        {
            this.TotalResults = totalResults;
        }

        public Dictionary<string, Dictionary<string, decimal>> TotalResults { get; set; }

        public List<string> Groups { get; set; }
    }

    public class PagedResultProductionSummaryDto<T> : PagedResultDto<T>
    {
        public PagedResultProductionSummaryDto(int totalCount, IReadOnlyList<T> items, Dictionary<string, decimal> totalResult, List<ProductionPlanSummary> standardGroups, List<ProductionPlanSummary> issueStandardGroups) : base(totalCount, items)
        {
            TotalResult = totalResult;
            StandardGroups = standardGroups;
            IssueStandardGroups = issueStandardGroups;
        }

        //column name to get to total
        public Dictionary<string, decimal> TotalResult { get; set; }

        public List<ProductionPlanSummary> StandardGroups { get; set; }
        public List<ProductionPlanSummary> IssueStandardGroups { get; set; }
    }
}
