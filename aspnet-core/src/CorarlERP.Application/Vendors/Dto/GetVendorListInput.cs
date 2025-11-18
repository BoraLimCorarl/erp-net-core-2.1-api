using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Vendors.Dto
{
    public class GetVendorListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public bool UsePagination { get; set; }
        public List<long?> Locations { get; set; }
        public List<long> VendorTypes {get;set;}
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "VendorName";
            }
            
        }
    }
}
