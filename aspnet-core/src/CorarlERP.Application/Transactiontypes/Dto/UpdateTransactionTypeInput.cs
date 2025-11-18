using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.TransactionTypes.Dto
{
   public class UpdateTransactionTypeInput :CreateTransactionTypeInput
    {
        public long Id { get; set; }
    }
}
