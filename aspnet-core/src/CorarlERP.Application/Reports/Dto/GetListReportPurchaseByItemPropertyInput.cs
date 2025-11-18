using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;

namespace CorarlERP.Reports.Dto
{  
    public class GetListReportPurchaseByItemPropertyInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Vendors { get; set; }
        public List<long> VendorTypes { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        //public BillType BillTypes { get; set; }
        public bool UsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
    public class GetPurchaseByItemPropertyReportInput : GetListReportPurchaseByItemPropertyInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

}
