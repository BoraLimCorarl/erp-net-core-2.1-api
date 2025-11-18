using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.TransferOrders.Dto
{
    public class UpdateTransferOrderInput:  CreateTransferOrderInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
