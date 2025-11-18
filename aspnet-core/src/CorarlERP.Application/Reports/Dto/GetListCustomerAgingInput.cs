using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetListCustomerAgingInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        
        public List<long?> Users { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long> AccountTypes { get; set; }
        public List<Guid> Customers { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<JournalType> JournalType { get; set; }
        public bool UsePagination { get; set; }
        public CurrencyFilter CurrencyFilter { get; set; }
        public List<long> Locations { get; set; }
        public List<long> CustomerTypes { get; set; }
        public string GroupBy { get; set; }
        public OpeningBalanceStatus OpeningBalance { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetCustomerAgingReportInput : GetListCustomerAgingInput
    {
        public ReportOutput ReportOutput { get; set; }
    }
}
