using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.Inventories.Data;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetListReportProfitByInvoiceOutput
    {
        public Guid InvoiceId { get; set; }        
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string Reference { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }     

        public long? LocationId { get; set; }
        public string LocationName { get; set; }       
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusName { get; set; }

        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; }
        public int RoundingDigit { get; set; }

        public string User { get; set; }
        public string Memo { get; set; }
        public string Description { get; set; }

        public Guid InvoiceItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public decimal Qty { get; set; }
        public decimal NetWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LinePrice { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineCost { get; set; }
        public decimal Profit { get; set;}
        public decimal Percentage { get; set;}
      
    }


    public class GetListReportProfitByInvoiceGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListReportProfitByInvoiceOutput> Items { get; set; }
    }

    public class ProfitByInvoiceColumnSummaryOutput
    {
        public decimal TotalQty { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProfitByInvoiceItemDetailOutput
    {
        public Guid InvoiceId { get; set; }
        public Guid InvoiceItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal NetWeight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LinePrice { get; set; }
        public decimal LineCost { get; set; }
        public decimal Profit { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public string Description { get; set; }
    }

}
