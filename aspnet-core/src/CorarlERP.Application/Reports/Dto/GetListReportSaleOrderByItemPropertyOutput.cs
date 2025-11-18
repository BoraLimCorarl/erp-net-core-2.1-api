using System;
using System.Collections.Generic;
using CorarlERP.Inventories.Data;

namespace CorarlERP.Reports.Dto
{
    public class GetListReportSaleOrderByItemPropertyOutput
    {
        public decimal OrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal OrderNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal OrderNetWeightInTon { get; set; }
        public decimal IssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }

        public decimal Total { get; set; }

        public int RoundingDigit { get; set; }
        public string SummaryBy { get; set; }
    }


    public class GetListReportSaleOrderByItemPropertyGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportSaleOrderByItemPropertyOutput> Items { get; set; }
    }

    public class SaleOrderByItemPropertyColumnSummaryOutPut
    {
        public decimal TotalOrderQty { get; set; }
        public decimal TotalIssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal TotalOrderNetWeight { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal TotalOrderNetWeightInTon { get; set; }
        public decimal TotalIssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SaleOrderItemByItemPropertyOutput
    {
        public Guid OrderId { get; set; }
        public Guid OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public decimal OrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal NetWeight { get; set; }
        public decimal Total { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public string SummaryBy { get; set; }
    }

}
