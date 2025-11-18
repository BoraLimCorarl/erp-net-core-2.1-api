using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AccountTypes.Dto
{

    public class CreateAccountTypeInput
    {
        [Required]
        public string AccountTypeName { get; set; }
        public TypeOfAccount Type { get; set; }
        public string Description { get; set; }
        
    }
}
