using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Locations.Dto
{
    [AutoMapFrom(typeof(Location))]
    public class LocationDetailOutput
    {
       public long Id { get; set; }
       public string LocationName { get; set; } 
       public bool LocationParent { get; set; }
        public long? ParentLocationId { get; set; }
        public Member Member { get; set; }
        public bool IsActive { get; set; }
        public LocationSummaryOutput ParentLocation { get; set; }

        public List<LocationUserItems> LocationUserItems { get; set; }
        public string PhoneNumber { get; set; }
    }

    [AutoMapFrom(typeof(Location))]
    public class LocationSummaryOutput
    {
        public long Id { get; set; }
        public string LocationName { get; set; }
        public string PhoneNumber { get; set; }
       
    }

    
    public class LocationSummaryDto
    {
        public long Id { get; set; }
        public string LocationName { get; set; }

    }
}
