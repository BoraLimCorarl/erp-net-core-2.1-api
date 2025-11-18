using Abp.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.SignUps.Dto
{
    public class ValidateInput
    {
        [DisableAuditing]
        public string ClientSecret { get; set; }

        [Required]
        [DisableAuditing]
        public string ClientId { get; set; }
    }
}
