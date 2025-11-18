using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Web.Models.TokenAuth
{
    public class GetTokenOutput
    {
        public string Token { get; set; }
        public DateTime Expired { get; set; }
    }
}
