using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Reports.Dto
{
    public class GetListInventoryValuationInput : PagedSortedAndFilteredInputDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> InventoryAccounts { get; set; }
        public List<long?> AccountTypes { get; set;} 
        public List<Guid> Items { get; set; }
        public List<long?>Locations { get; set; }
        //public List<long?> ItemProperties { get; set; }
        public List<GetListPropertyFilter> ItemProperties { get; set; }
        public bool UsePagination { get; set; }
    }

    public class GetInventoryValuationDetailReportInput : GetListInventoryValuationInput
    {
        //public string GroupBy { get; set; }
        public ReportOutput ReportOutput { get; set; }
    }


    public class RecalculateCostInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Items { get; set; }
    }

    public class InventoryTransactionItemSyncInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

}
