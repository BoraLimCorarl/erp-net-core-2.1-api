using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LabTestRequests.Dto
{
   public class UpdateLabTestRequestInput :CreateLabTestRequestInput
    {
        public Guid Id { get; set; }
    }
}
