using System;
using System.Collections.Generic;
using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.UserActivities.Dto
{
    public class GetDetailListUserActivityinputput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<GetListActivityOutput> Transactions { get; set; }
        public List<string> Activities { get; set; }
        public List<long> UserIds { get; set; }
        public long ErrorState { get; set; } //0 = All, 1 = Error, 2 = Success

        public bool UsePagination { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Time DESC";
            }
        }
    }
}
