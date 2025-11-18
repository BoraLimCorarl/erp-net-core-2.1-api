using CorarlERP.Dto;
using System;
using System.Collections.Generic;

namespace CorarlERP.Reports.Dto
{
    public enum TrackType
    {
        BatchNo = 1,
        Serial = 2,
        Expiration = 3,
    }

    public class BatchNoBalanceInput : PagedSortedAndFilteredInputDto
    {
        public DateTime ToDate { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public List<Guid> Items { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public string GroupBy { get; set; }
        public TrackType? TrackType { get; set; }
        public string BatchNumber { get; set; }

    }

    public class GetBatchNoBalanceReportInput : BatchNoBalanceInput
    {
        public ReportOutput ReportOutput { get; set; }
    }
}
