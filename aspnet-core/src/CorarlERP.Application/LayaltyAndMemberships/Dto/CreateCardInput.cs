using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LayaltyAndMemberships.Dto
{
    public class CreateCardInput
    {                  
        public string CardId { get;  set; }      
        public Guid? CustomerId { get;  set; }
        public string Remark { get;  set; }
        public string CardNumber { get;  set; }     
        public string SerialNumber { get; set; }

    }
}
