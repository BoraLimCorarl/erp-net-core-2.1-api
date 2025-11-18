using CorarlERP.Classes.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class PhysicalCountGetListOutput
    {
        public Guid Id { get; set; }
        public TransactionStatus StatusCode { get; set; }

        //public int CountItem { get; set; }
        public string PhysicalCountNo { get; set; }
        public DateTime PhysicalCountDate { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }
    }
}
