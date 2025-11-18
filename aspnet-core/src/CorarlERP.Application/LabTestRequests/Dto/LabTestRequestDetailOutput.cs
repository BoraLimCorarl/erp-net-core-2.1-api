using Abp.AutoMapper;
using CorarlERP.QCTests;
using System;

namespace CorarlERP.LabTestRequests.Dto
{
    [AutoMapFrom(typeof(LabTestRequest))]
    public class LabTestRequestDetailOutput
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long QCTestTemplateId { get; set; }
        public string QCTestTemplateName { get; set; }
        public Guid QCSampleId { get; set; }
        public string SampleId { get; set; }
        public LabTestType LabTestType { get; set; }
        public Guid? LabId { get; set; }
        public string LabName { get; set; }
        public LabTestStatus Status { get; set; }
        public string Remark { get; set; }

    }

}
