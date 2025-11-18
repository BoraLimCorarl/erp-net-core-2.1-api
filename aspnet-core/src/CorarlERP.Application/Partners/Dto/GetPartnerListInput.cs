using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Partners.Dto
{
    public class GetPartnerListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        
        public List<long?> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "PartnerCode";
            }

        }
    }
}
