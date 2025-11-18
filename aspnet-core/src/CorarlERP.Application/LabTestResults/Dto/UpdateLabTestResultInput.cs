using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LabTestResults.Dto
{
   public class UpdateLabTestResultInput :CreateLabTestResultInput
    {
        public Guid Id { get; set; }
    }
}
