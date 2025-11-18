using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Promotions.Dot
{
    public class GetPromotionCampaignListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? IsActive { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "StartDate";
            }
        }
    }

    public class FindPromotionCampaignInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime? Date { get; set; }
        public int? EditionId { get; set; }
        public bool? IsActive { get; set; }
        public CampaignType? CampaignType { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "StartDate";
            }
        }
    }
}
