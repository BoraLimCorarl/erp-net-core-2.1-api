using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.QCTests;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LabTestRequests.Dto
{
   public class GetLabTestRequestListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<long> QCTemplateIds { get; set; }
        public List<Guid> SampleIds { get; set; }
        public List<Guid> LabIds { get; set; }
        public List<Guid> ItemIds { get; set; }
        public List<long> LocationIds { get; set; }
        public LabTestType? LabTestType { get; set; }
        public List<LabTestStatus> LabTestStatus { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Date";
            }
        }
    }
}
