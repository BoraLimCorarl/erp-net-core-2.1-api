using System;
using System.Collections.Generic;

namespace CorarlERP.LockTransactions.Dto
{
    public class UpdateLockInput : CreateLockInput
    {       
       public List<CreateLockInput> LockItems { get; set; }
    }
}
