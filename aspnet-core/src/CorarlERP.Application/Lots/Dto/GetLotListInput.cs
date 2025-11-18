using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Lots.Dto
{
    public class GetLotListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public bool? IsExcept { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "LotName";
            }
        }
    }

    public class FindLotInput : GetLotListInput
    {
        public List<long> SelectedValues { get; set; }
    }
}
