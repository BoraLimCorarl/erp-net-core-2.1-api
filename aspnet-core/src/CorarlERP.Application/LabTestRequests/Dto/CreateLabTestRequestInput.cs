using CorarlERP.QCTests;
using CorarlERP.Vendors;
using System;

namespace CorarlERP.LabTestRequests.Dto
{
  
    public class CreateLabTestRequestInput
    {
        public DateTime Date { get; set; }
        public long QCTestTemplateId { get; set; }
        public Guid QCSampleId { get; set; }
        public LabTestType LabTestType { get; set; }
        public Guid? LabId { get; set; }
        public LabTestStatus Status { get; set; }
        public string Remark { get; set; }
    }

}
