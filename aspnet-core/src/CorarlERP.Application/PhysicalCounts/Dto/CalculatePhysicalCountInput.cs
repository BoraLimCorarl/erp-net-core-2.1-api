using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class CalculatePhysicalCountInput
    {
        public DateTime Date {  get; set; }
        public long LocationId { get; set; }
        public List<Guid> ItemIds { get; set; }
        public List<Guid> ExceptIds { get; set; }
    }

    public class ExportExcelPhysicalCountInput : CalculatePhysicalCountInput
    {
        public Guid? Id { get; set; }
    }
}
