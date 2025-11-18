using System;
using System.Collections.Generic;

namespace CorarlERP.Reports.Dto
{   
    public class GetListQCTestReportOutput
    {
        public Guid Id { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ResultDate { get; set; }       
        public string SampleId { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public string Location { get; set; }
        public string Amount { get; set; }
        public string ReferenceNo { get; set; }
        public bool DetailEntry { get; set; }
        public string LabName { get; set; }
        public string Remark { get; set; }
        public bool FinalPassFail { get; set; }
        public List<TestResultDetailOutput> TestResultDetails { get; set; }
    }

    public class TestResultDetailOutput
    {
        public Guid LabTestResultId { get; set; }
        public long TestParameterId { get; set; }
        public string TestParameterName { get; set; }
        public string LimitReferenceNote { get; set; }
        public string ActualValue { get; set; }
        public bool PassFail { get; set; }
    }

    public class GetListQCTestReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListQCTestReportOutput> Items { get; set; }
    }

}
