using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Locks
{
    [Table("CarlErpPermissionLockScheduleItems")]
    public class TransactionLockSheduleItem : AuditedEntity<long>, IMayHaveTenant
    {

        public int? TenantId { get; set; }

        public long TransactionLockSheduleId { get; private set; }
        public TransactionLockSchedule TransactionLockShedule { get; private set; }
        public TransactionLockType LockTransaction { get; private set; }

        public static TransactionLockSheduleItem Create(int tenantId, long userId, long transactionLockScheduleId, TransactionLockType transactionLockType)
        {
            return new TransactionLockSheduleItem
            {
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                TransactionLockSheduleId = transactionLockScheduleId,
                LockTransaction = transactionLockType
            };
        }

        public void Update(long userId, long transactionLockScheduleId, TransactionLockType transactionLockType)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            TransactionLockSheduleId = transactionLockScheduleId;
            LockTransaction = transactionLockType;            
        }
    }
}
