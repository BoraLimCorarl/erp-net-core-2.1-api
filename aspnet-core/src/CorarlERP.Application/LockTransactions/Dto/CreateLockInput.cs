using System;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.LockTransactions.Dto
{
    public class CreateLockInput
    {
        public long Id { get; set; }

        public bool IsLock { get; set; }

        public DateTime? LockDate { get; set; }

        public TransactionLockType TransactionLockType { get; set; }

        public string Password{ get; set; }

        public  int ExpiredTime { get; set; }

        public DateTime? GenerateDate { get; set; }

    }

}
