using Abp.Auditing;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.SignUps.Dto
{
    public class GenerateTokenInput
    {
        public string Token { get; set; }   
        
        public CreateOrUpdateSignUpInput SignUpInput { get; set; }
       
        [DisableAuditing]
        public string ClientSecret { get; set; }

        [Required]
        [DisableAuditing]
        public string ClientId { get; set; }
    }
    public class VerifyTokenInput
    {
        public string Token { get; set; }
        public string Code { get; set; }
        public CreateOrUpdateSignUpInput SignUpInput { get; set;}
        [DisableAuditing]
        public string ClientSecret { get; set; }

        [Required]
        [DisableAuditing]
        public string ClientId { get; set; }
    }
}
