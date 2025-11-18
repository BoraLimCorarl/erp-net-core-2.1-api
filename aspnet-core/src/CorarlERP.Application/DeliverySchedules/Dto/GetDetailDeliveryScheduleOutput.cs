using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.DeliverySchedules.Dto
{
   public  class GetDetailDeliveryScheduleOutput
    {
        public Guid Id { get; set; }
        public string DeliveryNo { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime InitialDeliveryDate { get; set; }
        public DateTime FinalDeliveryDate { get; set; }
        public Guid? SaleOrderId { get; set; }
        public string SaleOrderNo { get; set; }

        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; }
        public DeliveryStatus ReceiveStatus { get; set; }
        public string ReceiveStatusName { get; set; }
        public List<DeliveryScheduleItemInput> Items { get; set; }
        public string Reference { get; set; }
        public string Memo { get; set; }
    }
}
