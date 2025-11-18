using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Lots.Dto
{
    public class CreateLotInput
    {
        public long LocationId { get; set; }

        [Required]
        public string LotName { get; set; }

    }
}
