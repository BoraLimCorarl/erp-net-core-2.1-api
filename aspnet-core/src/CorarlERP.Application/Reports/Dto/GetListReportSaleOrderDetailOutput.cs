using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.Inventories.Data;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetListReportSaleOrderDetailOutput
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

        public string User { get; set; }
        public string Memo { get; set; }
        public string Description { get; set; }

        public Guid OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public decimal OrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal QtyBalance { get; set; }
        public decimal OrderNetWeight { get; set; }
        public decimal IssueNetWeight { get; set; }
        public decimal NetWeightBalance { get; set; }
        public decimal OrderNetWeightInTon { get; set; }
        public decimal IssueNetWeightInTon { get; set; }
        public decimal NetWeightBalanceInTon { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal UnitPriceInTon { get; set; }


        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; }
        public int RoundingDigit { get; set; }
        public string PropertyGroup { get; set; }
    }


    public class GetListReportSaleOrderDetailGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportSaleOrderDetailOutput> Items { get; set; }
    }

    public class SaleOrderDetailColumnSummaryOutPut
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

    public class SaleOrderItemDetailOutput
    {
        public Guid OrderId { get; set; }
        public Guid OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal NetWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public string Description { get; set; }
        public string PropertyGroup { get; set; }
    }

}
