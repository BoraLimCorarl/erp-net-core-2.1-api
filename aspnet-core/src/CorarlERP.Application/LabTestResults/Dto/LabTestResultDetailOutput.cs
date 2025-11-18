using Abp.AutoMapper;
using CorarlERP.QCTests;
using System;
using System.Collections.Generic;

namespace CorarlERP.LabTestResults.Dto
{
    [AutoMapFrom(typeof(LabTestResult))]
    public class LabTestResultDetailOutput
    {
        public Guid Id { get; set; }
        public DateTime ResultDate { get; set; }
        public string ReferenceNo { get; set; }
        public Guid LabTestRequestId { get; set; }
        public string QCTestTemplateName { get; set; }
        public string SampleId { get; set; }
        public string LabName { get; set; }
        public bool DetailEntry { get; set; }
        public bool FinalPassFail { get; set; }
        public List<LabTestResultDetailInput> LabTestResultDetails { get; set; }

    }

}
