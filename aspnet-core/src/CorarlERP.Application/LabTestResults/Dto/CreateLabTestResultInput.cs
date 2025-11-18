using CorarlERP.QCTests;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;

namespace CorarlERP.LabTestResults.Dto
{
  
    public class CreateLabTestResultInput
    {
        public DateTime ResultDate { get; set; }
        public string ReferenceNo { get; set; }
        public Guid LabTestRequestId { get; set; }
        public bool DetailEntry { get; set; }
        public bool FinalPassFail { get; set; }
        public List<LabTestResultDetailInput> LabTestResultDetails { get; set; }
    }

    public class LabTestResultDetailInput
    {
        public Guid? Id { get; set; }
        public long TestParameterId { get; set; }
        public string TestParameterName { get; set; }
        public string LimitReferenceNote { get; set; }
        public string ActualValueNote { get; set; }
        public bool PassFail { get; set; }
    }
}
