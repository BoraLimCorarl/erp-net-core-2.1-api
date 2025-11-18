using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.Inventories.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
namespace CorarlERP.Reports.Dto
{
    public class GetListReportDeliveryScheduleSummaryInput : PagedSortedAndFilteredInputDto, IShouldNormalize
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
                Sorting = "InDate.Date";
            }
        }
    }
    public class GetDeliveryScheduleSummaryReportInput : GetListReportDeliveryScheduleSummaryInput
    {
        public ReportOutput ReportOutput { get; set; }
    }
    public class GetListDeliveryScheduleSummaryReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportDeliveryScheduleSummaryOutput> Items { get; set; }
    }


    public class GetListReportDeliveryScheduleSummaryOutput
    {
        public Guid Id { get; set; }
        public DateTime InDate { get; set; }
        public DateTime FDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public string OrderRef { get; set; }
        public Guid? OrderId { get; set; }

        public string DeliveryNo { get; set; }
        public string DeliveryRef { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusName { get; set; }
        public string User { get; set; }
        public decimal TotalDeliveryNetWeight { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalRemainingNetWeight { get; set; }     
        public string Description { get; set; }
        public List<DeliveryScheduleItemSummaryOutput> Items { get; set; }
        public int RoundingDigit { get; set; }
        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; }
    }


    public class DeliveryScheduleItemSummaryOutput
    {
        public Guid? OrderId { get; set; }
        public Guid? OrderItemId { get; set; }

        public Guid DeliveryId { get; set; }
        public Guid DeliveryScheduleItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal DeliveryNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
    }

    public class DeliveryScheduleColumnSummaryOutPut
    {     
        public decimal TotalDeliveryNetWeight { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
    }

    public class DeliveryScheduleItemSummaryQuery
    {
        public Guid OrderItemId { get; set; }
        public Guid DeliveryScheduleItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal DeliveryNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public Guid OrderId { get; set; }
        public Guid DeliveryId { get; set; }
        public DateTime InDate { get; set; }
        public DateTime FDate { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public string OrderRef { get; set; }

        public string DeliveryNo { get; set; }
        public string DeliveryRef { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }

        public string User { get; set; }
        public string Memo { get; set; }
    }
}
