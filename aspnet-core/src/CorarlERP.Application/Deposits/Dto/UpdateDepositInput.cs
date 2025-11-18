using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Deposits.Dto
{
    public class UpdateDepositInput : CreateDepositInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
