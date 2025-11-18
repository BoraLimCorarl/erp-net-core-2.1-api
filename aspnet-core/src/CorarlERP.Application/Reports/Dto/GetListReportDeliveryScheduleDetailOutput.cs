using CorarlERP.Inventories.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
namespace CorarlERP.Reports.Dto
{
    public class GetListReportDeliveryScheduleDetailOutput
    {
        public Guid Id { get; set; }
        public DateTime InDate { get; set; }
        public DateTime FDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderReference { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryReference { get; set; }
        public long? LocationId { get; set; }
        public string LocationName { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusName { get; set; }

        public string User { get; set; }
        public string Memo { get; set; }
        public string Description { get; set; }

        public Guid DeliveryScheduleItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal DeliveryNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal DeliveryNetWeightInTon { get; set; }
        public decimal IssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }
        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; }
        public int RoundingDigit { get; set; }
        public string PropertyGroup { get; set; }
    }


    public class GetListReportDeliveryScheduleDetailGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportDeliveryScheduleDetailOutput> Items { get; set; }
    }

    public class DeliveryScheduleDetailColumnSummaryOutPut
    {
        public decimal TotalDeliveryQty { get; set; }       
        public decimal QtyBalance { get; set; }
        public decimal TotalDeliveryNetWeight { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal TotalDeliveryNetWeightInTon { get; set; }
        public decimal TotalIssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }
        public decimal TotalIssueQty { get; set; }
    }

    public class DeliveryScheduleItemDetailOutput
    {
        public Guid DeliveryId { get; set; }
        public Guid DeliveryScheduleItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal NetWeight { get; set; }
     
        public List<ItemPropertySummary> Properties { get; set; }
        public string Description { get; set; }
        public string PropertyGroup { get; set; }
    }
}
