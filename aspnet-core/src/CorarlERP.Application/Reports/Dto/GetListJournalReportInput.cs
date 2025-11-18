using Abp.Runtime.Validation;
using CorarlERP.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Reports.Dto
{
    public class GetListJournalReportInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> ChartOfAccounts { get; set; }
        public List<long?> Users { get; set; }
        public List<JournalType> JournalType { get; set; }
        public List<long> Locations { get; set; }
        public List<long> AccountTypes { get; set; }
        public bool? NotUsePagination { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }


    public class JournalExportReportInput : GetListJournalReportInput
    {
        public ReportOutput ReportOutput { get; set; }

    }
}
