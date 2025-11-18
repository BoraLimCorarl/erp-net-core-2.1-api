using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyValues.Dto
{
    public class GetPropertyValueListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public long PropertyId { get; set; }
        public bool? IsActive { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Value";
            };
        }
    }
}
