using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SubItems.Dto
{
    public class GetSubItemInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string  Name { get; set; }
        public string Code { get; set; }
        public void Normalize()
        {
            Sorting = "Name";
        }
    }
}
