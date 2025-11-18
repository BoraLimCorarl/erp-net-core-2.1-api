using Abp.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Web.Models.TokenAuth
{
    public abstract class AuthenticationBaseModel
    {
        [DisableAuditing]
        public string ClientSecret { get; set; }


        [Required]
        [DisableAuditing]
        public string ClientId { get; set; }

        //[DisableAuditing]
        //public string ClientDevice { get; set; }

        //[DisableAuditing]
        //public string ClientIpAddress { get; set; }
    }
}
