using System;
using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.UserActivities.Dto
{
    public class GetUserActivityInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Value";
            }
        }
    }
}
