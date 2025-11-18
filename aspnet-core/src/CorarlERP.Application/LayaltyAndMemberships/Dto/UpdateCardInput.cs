using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LayaltyAndMemberships.Dto
{
    public class UpdateCardInput : CreateCardInput
    { 
        public Guid Id { get; set; }
    }
}
