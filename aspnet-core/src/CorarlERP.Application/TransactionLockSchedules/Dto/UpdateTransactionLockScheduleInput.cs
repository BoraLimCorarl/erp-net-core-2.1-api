using CorarlERP.PermissionLocks.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.TransactionLockSchedules.Dto
{
    public class UpdateTransactionLockScheduleInput : CreateTransactionLockScheduleInput
    {
        public long Id { get; set; }
    }
    public class CountInput { 
       public int Count { get; set; }
    } 
}
