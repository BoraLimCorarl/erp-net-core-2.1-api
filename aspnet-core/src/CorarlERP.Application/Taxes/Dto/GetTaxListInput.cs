using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Taxes.Dto
{
    public class GetTaxListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "TaxName";
            }
        }
    }
}
