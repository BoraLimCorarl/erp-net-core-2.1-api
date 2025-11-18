using System;
using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.MultiTenancy.Dto
{
    public class FindTenantsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public bool? IsActive { get; set; }
        public string Filter { get; set; }
      
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "TenancyName";
            }
        }
    }
}

