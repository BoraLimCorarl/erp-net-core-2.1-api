using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.ReportTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
   public class GetListReportSaleOrderSummaryInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        public bool UsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<long> CustomerTypes { get; set; }
        public List<DeliveryStatus> DeliveryStatuses { get; set; }
        public List<TransactionStatus> Statuses { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
    public class GetSaleOrderSummaryReportInput : GetListReportSaleOrderSummaryInput
    {
        public ReportOutput ReportOutput { get; set; }
    }
    public class GetListSaleOrderSummaryReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportSaleOrderSummaryOutput> Items { get; set; }
    }

}
