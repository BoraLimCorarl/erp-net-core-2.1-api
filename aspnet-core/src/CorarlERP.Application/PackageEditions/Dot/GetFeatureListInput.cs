using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PackageEditions.Dot
{
    public class GetFeatureListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public bool UsePagination { get; set; } 

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "SortOrder";
            }
        }
    }

    public class FindFeatureInput : GetFeatureListInput
    {
        public List<Guid> SelectedFeatures { get; set; }
    }
}
