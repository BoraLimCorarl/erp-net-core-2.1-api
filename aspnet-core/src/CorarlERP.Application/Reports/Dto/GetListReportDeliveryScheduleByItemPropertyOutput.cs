using System;
using System.Collections.Generic;
using CorarlERP.Inventories.Data;

namespace CorarlERP.Reports.Dto
{
    public class GetListReportDeliveryScheduleByItemPropertyOutput
    {
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal DeliveryNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal DeliveryNetWeightInTon { get; set; }
        public decimal IssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }

        public int RoundingDigit { get; set; }
        public string SummaryBy { get; set; }
        public long Month { get; set; }
        public string MonthName { get; set; }
        public Dictionary<long, DeliveryScheduleMonthlyColoumn> MonthlyColumns { get; set; }
    }


    public class GetListReportDeliveryScheduleByItemPropertyGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportDeliveryScheduleByItemPropertyOutput> Items { get; set; }
    }

    public class DeliveryScheduleByItemPropertyColumnSummaryOutPut
    {
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal DeliveryNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal DeliveryNetWeightInTon { get; set; }
        public decimal IssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }
        public Dictionary<long, DeliveryScheduleMonthlyColoumn> MonthlyColumns { get; set; }
    }

    public class DeliveryScheduleItemByItemPropertyOutput
    {
        public Guid DeliveryScheduleId { get; set; }
        public Guid DeliverySCheduleItemId { get; set; }
        public Guid ItemId { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal NetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public string SummaryBy { get; set; }
        public long Month { get; set; }
        public string MonthName { get; set; }
    }

    public class DeliveryScheduleMonthlyColoumn
    {  
        public long Month { get; set; }
        public string MonthName { get; set; }
        public decimal DeliveryQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal DeliveryNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal DeliveryNetWeightInTon { get; set; }
        public decimal IssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }
    }

    public class DeliveryScheduleReportResultOutput
    {

        public List<GetListReportDeliveryScheduleByItemPropertyOutput> Items { get; set; }
        public decimal TotalCount { get; set; }
        public Dictionary<string, decimal> TotalResult { get; set; }
        public Dictionary<long, DeliveryScheduleMonthlyColoumn> MonthlyColumns { get; set; }
        public List<MonthDto> Months { get; set; }
    }

    public class MonthDto
    {
        public long Month { get; set; }
        public string MonthName { get; set; }

    }

}
