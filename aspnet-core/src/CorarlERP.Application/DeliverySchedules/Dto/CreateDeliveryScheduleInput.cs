using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.DeliverySchedules.Dto
{
   public class CreateDeliveryScheduleInput
    {
        public string DeliveryNo { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid CustomerId { get; set; }        
        public DateTime InitialDeliveryDate { get; set; }
        public DateTime FinalDeliveryDate { get; set; }
        public DateTime? DateCompare { get; set; }
        public Guid? SaleOrderId { get; set; }   
        public TransactionStatus Status { get; set; }      
        public DeliveryStatus ReceiveStatus { get; set; }
        public string Reference { get; set; }
        public string Memo { get; set; }
        public Guid? ItemIssueId { get; set; }

        public List<DeliveryScheduleItemInput> Items { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

    }
}
