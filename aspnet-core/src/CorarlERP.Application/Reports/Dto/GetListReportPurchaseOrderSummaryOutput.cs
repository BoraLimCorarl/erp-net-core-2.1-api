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
    public class GetListReportPurchaseOrderSummaryOutput
    {
        public Guid Id { get; set; }        
        public DateTime Date { get; set; }

        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string OrderNumber { get; set; }
        public string Reference { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }       
        public DeliveryStatus ReceiveStatus { get; set; }
        public string ReceiveStatusName { get; set; }

        public decimal OrderAmount { get; set; }
        public decimal OrderAmountMultiCurrency { get; set; }
        public decimal TotalReceiveCount { get; set; }

        public string User { get; set; }
        public decimal TotalOrderNetWeight { get; set; }
        public decimal TotalReceiveNetWeight { get; set; }
        public decimal TotalRemainingNetWeight { get; set; }
        public string Description { get; set; }
        public List<PurchaseOrderItemSummaryOutput> Items { get; set; }
        public int RoundingDigit { get; set; }
    }


    public class PurchaseOrderItemSummaryOutput
    {
        public Guid OrderId { get; set; }
        public Guid OrderItemId { get; set; } 
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OrderQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal OrderNetWeight { get; set; }
        public decimal ReceiveNetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
    }

    public class ColumnSummaryOutPut {
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalOrderAmountMultiCurrency { get; set; }
        public decimal TotalOrderNetWeight { get; set; }
        public decimal TotalReceiveNetWeight { get; set; }
    }

    public class PurchaseOrderItemSummaryQuery
    {
        public Guid OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OrderQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal OrderNetWeight { get; set; }
        public decimal ReceiveNetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public Guid OrderId { get; set; }
        public DateTime Date { get; set; }

        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
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
