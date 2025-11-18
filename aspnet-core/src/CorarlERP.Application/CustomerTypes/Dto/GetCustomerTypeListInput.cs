using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CustomerTypes.Dto
{
   public class GetCustomerTypeListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "CustomerTypeName";
            }
        }
    }
}
