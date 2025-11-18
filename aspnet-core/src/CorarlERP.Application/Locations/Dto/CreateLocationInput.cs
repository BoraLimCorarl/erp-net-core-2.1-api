using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Locations.Dto
{
    public class CreateLocationInput
    {
        public long? ParentLocationId { get; set; }
        public Member Member { get; set; }
        [Required]
        public string LocationName { get; set; }

        public bool LocationParent { get; set; }       

        public string PhoneNumber { get; set; }
        public List<LocationUserItems> LocationUserItems { get; set; }
    }


    public class LocationUserItems
    {
        public Guid? Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
    }
}
