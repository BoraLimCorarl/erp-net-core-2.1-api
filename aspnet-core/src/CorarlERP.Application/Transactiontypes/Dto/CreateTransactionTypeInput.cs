using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.TransactionTypes.Dto
{
    public class CreateTransactionTypeInput
    {      
        public string TransactionTypeName { get; set; }

        public bool IsPOS { get; set; }

    }
}
