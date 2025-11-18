using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InventoryCalculationSchedules.Dto
{
    public class GetListInventoryCalculationScheduleOutput
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? NextExecution { get; set; }
        public DateTime? LastExecution { get; set; }
        public string LastJobState { get; set; }
        public string Cron { get; set; }
        public string Queue { get; set; }
        public string Method { get; set; }

        public DateTime ScheduleTime { get; set; }
        public bool IsActive { get; set; }
    }
}
