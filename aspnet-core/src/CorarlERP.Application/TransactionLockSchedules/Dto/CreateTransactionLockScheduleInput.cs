using CorarlERP.Locks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransactionLockSchedules.Dto
{
    public class CreateTransactionLockScheduleInput
    {
        public ScheduleType ScheduleType { get; set; }
        public DateTime ScheduleTime { get; set; }
        public ScheduleDate ScheduleDate { get; set; }
        public int DaysBeforeYesterday { get; set; }

        public List<TransactionLockScheduleItemDto> TransactionLockScheduleItems { get; set; }
        public bool IsActive { get; set; }

    }

    public class TransactionLockScheduleItemDto
    {
        public long Id { get; set; }
        public TransactionLockType TransactionLockType { get; set; }
        public string TransactionLockTypeName { get; set; }
    }
}
