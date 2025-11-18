using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Productions
{
    public class StandardGroupSummary
    {
        public Guid ProductionId { get; set; }
        public long? GroupId { get; set; }
        public string GroupName { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal QtyPercentage { get; set; }
        public decimal NetWeightPercentage { get; set; }
    }
}
