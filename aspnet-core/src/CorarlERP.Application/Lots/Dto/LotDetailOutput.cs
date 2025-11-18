using Abp.AutoMapper;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Lots.Dto
{
    [AutoMapFrom(typeof(Lot))]
    public class LotDetailOutput
    {
        public long Id { get; set; }
        public string LotName { get; set; }
        public long LocationId { get; set; }
        public bool IsActive { get; set; }
        public LocationSummaryOutput Location { get; set; }
    }
    [AutoMapFrom(typeof(Lot))]
    public class LotSummaryOutput
    {
        public long Id { get; set; }
        public string LotName { get; set; }

    }
    public class ItemLotDto
    {
        public long Id { get; set; }
        public string LotName { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
    }
}
