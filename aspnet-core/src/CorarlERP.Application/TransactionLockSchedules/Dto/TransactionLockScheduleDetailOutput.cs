using Abp.AutoMapper;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransactionLockSchedules.Dto
{
    [AutoMapFrom(typeof(TransactionLockSchedule))]
    public class TransactionLockScheduleDetailOutput
    {
        public long Id { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public string ScheduleTypeName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int DaysBeforeYesterday { get; set; } 
        public List<TransactionLockScheduleItemDto> TransactionLockScheduleItems { get; set; }
        public ScheduleDate ScheduleDate { get; set; }
        public string ScheduleDateName { get; set; }

        public bool IsActive { get; set; }
    }

}
