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
    [Table("CarlErpPermissionLockSchedules")]
    public class TransactionLockSchedule : AuditedEntity<long>, IMayHaveTenant
    {

        public int? TenantId { get; set; }

        public ScheduleType ScheduleType { get; private set; }
        public DateTime ScheduleTime { get; private set; }

       
        public ScheduleDate ScheduleDate { get; set; }
        public int DaysBeforeYesterday { get; set; }

        public bool IsActive { get; private set; }
        public void Enable(bool isActive) { IsActive = isActive; }

        public static TransactionLockSchedule Create(int? tenantId, long creatorUserId, ScheduleType scheduleType, DateTime scheduleTime, ScheduleDate scheduleDate, int daysBeforeYesterday)
        {
            return new TransactionLockSchedule()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ScheduleType = scheduleType,
                ScheduleTime = scheduleTime,
                ScheduleDate = scheduleDate,
                DaysBeforeYesterday = daysBeforeYesterday,
                IsActive = true,
            };
        }

        public void Update(long lastModifiedUserId, ScheduleType scheduleType, DateTime scheduleTime, ScheduleDate scheduleDate, int daysBeforeYesterday)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ScheduleType = scheduleType;
            ScheduleTime = scheduleTime;
            ScheduleDate = scheduleDate;
            DaysBeforeYesterday = daysBeforeYesterday;
        }
    }

    public enum ScheduleType
    {
        Daily = 1,
        Weekly = 2,
    }

    public enum ScheduleDate
    {
        Yesterday = 1,
        Today = 2,
        DaysBeforeYesterday = 3 
    }
}
