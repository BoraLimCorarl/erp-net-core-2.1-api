using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    
    public class GetListVendorAgingInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long> AccountTypes { get; set; }
        public List<Guid> Vendors { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<long?> Users { get; set; }
        public List<JournalType> JournalType { get; set; }
        public bool UsePagination { get; set; }
        public CurrencyFilter CurrencyFilter { get; set; }
        public List<long>Locations { get; set; }
        public List<long> VendorTypes { get; set; }
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

    public class GetVendorAgingReportInput : GetListVendorAgingInput
    {
        public ReportOutput ReportOutput { get; set; }
    }
}
