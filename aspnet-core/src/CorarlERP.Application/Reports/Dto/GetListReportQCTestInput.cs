using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.QCTests;
using System;
using System.Collections.Generic;

namespace CorarlERP.Reports.Dto
{  
    public class GetListReportQCTestInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Labs { get; set; }
        public List<long> LabTypes { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        public List<long> TestParameterIds { get; set; }
        public List<long> QCTestTemplateIds { get; set; }
        public List<LabTestStatus> LabTestStatus { get; set; }
        public bool? PassFail { get; set; }
        public bool UsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "SendDate.Date";
            }
        }
    }
    public class GetQCTestReportInput : GetListReportQCTestInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

}
