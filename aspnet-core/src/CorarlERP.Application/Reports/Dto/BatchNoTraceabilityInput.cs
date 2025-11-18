using CorarlERP.Dto;
using System;

namespace CorarlERP.Reports.Dto
{
    public class BatchNoTraceabilityInput 
    {
        public string Filter { get; set; }

    }

    public class GetMoreBatchItemInput
    {
        public Guid TransacationId { get; set; }
    }

    public class FindBatchNoTraceabilityInput : PagedSortedAndFilteredInputDto
    {
        public string Customer { get; set; }
        public string Item { get; set; }
        public string Invoice { get; set; }
        public string ItemIssue { get; set; }
    }
    
}
