using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BankTransfers.Dto
{
   public class UpdateBankTransferInput : CreateBankTransferInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
