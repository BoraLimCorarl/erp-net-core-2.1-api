using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    public class GetBatchNoFormulaListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long?> Users { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}
