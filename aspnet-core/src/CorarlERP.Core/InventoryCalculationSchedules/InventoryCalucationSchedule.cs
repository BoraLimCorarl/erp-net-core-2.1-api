using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.InventoryCalculationSchedules
{
    [Table("CarlErpInventoryCalculationSchedules")]
    public class InventoryCalculationSchedule: AuditedEntity<Guid>
    {
        public DateTime ScheduleTime { get; private set; }
        public bool IsActive { get; private set; }
        public void Enable(bool isEnable) => IsActive = isEnable;

        public static InventoryCalculationSchedule Create(long userId, DateTime scheduleTime)
        {
            return new InventoryCalculationSchedule
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                ScheduleTime = scheduleTime,
                IsActive = true
            };
        }

        public void Update(long userId, DateTime scheduleTime)
        {
            LastModificationTime = Clock.Now;
            LastModifierUserId = userId;
            ScheduleTime = scheduleTime;
        }
    }
}
