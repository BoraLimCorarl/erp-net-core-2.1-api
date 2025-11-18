using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    public class GetBatchNoListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public Guid ItemId { get; set; }
        public Guid? SelectedValue { get; set; }
        public bool? isStandard { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Code DESC";
            }
        }
    }
}
