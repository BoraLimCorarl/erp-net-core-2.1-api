using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.QCSamples.Dto
{
   public class GetQCSampleListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> ItemIds { get; set; } = new List<Guid>();
        public List<long> LocationIds { get; set; } = new List<long>();
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "SampleId";
            }
        }
    }
}
