using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InventoryCalculationSchedules.Dto
{
    public class InventoryCalculationScheduleDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduleTime { get; set; }
        public bool IsActive { get; set; }
    }
}
