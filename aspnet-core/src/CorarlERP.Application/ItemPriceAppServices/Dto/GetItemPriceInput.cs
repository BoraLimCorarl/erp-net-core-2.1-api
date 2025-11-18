using System;
using System.Collections.Generic;
using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.ItemPriceAppServices.Dto
{
    public class GetItemPriceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Items { get; set; }
        public List<long> Curencys { get; set; }
        public List<long> Locations { get; set; }
        public List<long> SaleTypes { get; set; }
        public List<long> CustomerTypes { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}
