using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ProductionLines.Dto
{
    public class GetProductionLineListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long?> Users { get; set; }
        public bool? IsActive { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "ProductionLineName";
            }
        }
    }
}
