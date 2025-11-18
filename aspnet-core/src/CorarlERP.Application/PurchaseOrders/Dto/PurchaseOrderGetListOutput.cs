using Abp.AutoMapper;
using CorarlERP.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.PurchaseOrders.PurchaseOrder;

namespace CorarlERP.PurchaseOrders.Dto
{
    //[AutoMapFrom(typeof(PurchaseOrder))]
    public  class PurchaseOrderGetListOutput
    {
        public string StatusName { get; set; }
        public string LocationName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }
        public bool IsCanVoidOrDraftOrClose { get; set; }
        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; } 
        public int CountItem { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }
        public string Reference { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }

        public UserDto User { get; set; }
        public int TotalReceiveCount { get; set; }
    }
}
