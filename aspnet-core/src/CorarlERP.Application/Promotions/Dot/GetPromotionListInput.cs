using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Promotions.Dot
{
    public class GetPromotionListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public PromotionType? PromotionType { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "PromotionName";
            }
        }
    }
}
