using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.Inventories.Data;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetListReportSaleOrderSummaryOutput
    {
        public Guid Id { get; set; }        
        public DateTime Date { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }       
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusName { get; set; }

        public decimal OrderAmount { get; set; }
        public decimal OrderAmountMultiCurrency { get; set; }
        public decimal TotalIssueCount { get; set; }

        public string User { get; set; }
        public decimal TotalOrderNetWeight { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalRemainingNetWeight { get; set; }
        public string Description { get; set; }
        public List<SaleOrderItemSummaryOutput> Items { get; set; }
        public int RoundingDigit { get; set; }
        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; }
    }


    public class SaleOrderItemSummaryOutput
    {
        public Guid OrderId { get; set; }
        public Guid OrderItemId { get; set; } 
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal OrderNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
    }

    public class SaleOrderColumnSummaryOutPut {
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalOrderAmountMultiCurrency { get; set; }
        public decimal TotalOrderNetWeight { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
    }

    public class SaleOrderItemSummaryQuery
    {
        public Guid OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal OrderNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public Guid OrderId { get; set; }
        public DateTime Date { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }

        public decimal TotalOrderAmount { get; set; }
        public decimal TotalOrderAmountMultiCurrency { get; set; }

        public string User { get; set; }
        public string Memo { get; set; }
    }
}
