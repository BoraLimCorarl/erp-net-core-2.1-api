using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders.Dto
{
    public class SaleOrderGetListOutput
    {
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }       
        public int CountItem { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }
        public string Reference { get; set; }
        public string LocationName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsCanVoidOrDraftOrClose { get; set; }
        public UserDto User { get; set; }
        public int TotalIssueCount { get; set; }
    }
}
