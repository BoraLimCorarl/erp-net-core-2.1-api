using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.DeliverySchedules.Dto
{
   public class UpdateDeliveryScheduleInput : CreateDeliveryScheduleInput
    {
        public Guid Id { get; set; }
    }
}
