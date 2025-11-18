using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.DeliverySchedules.Dto
{
   public  class GetListDeliveryScheduleOutput
    {
        public Guid Id { get; set; }
        public string DeliveryNo { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }      
        public DateTime InitialDeliveryDate { get; set; }
        public DateTime FinalDeliveryDate { get; set; }
        public Guid? SaleOrderId { get;  set; }
        public string SaleOrderNo { get;  set; }

        public TransactionStatus Status { get;  set; }
        public string StatusName { get;  set; }
        public DeliveryStatus ReceiveStatus { get;  set; }
        public string ReceiveStatusName { get;  set; }
        public string Reference { get; set; }

        public UserDto User { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public decimal CountItem { get; set; }
    }
}
