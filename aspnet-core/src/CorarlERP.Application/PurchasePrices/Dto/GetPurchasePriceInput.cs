using System;
using System.Collections.Generic;
using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.PurchasePrices.Dto
{
    public class GetPurchasePriceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Items { get; set; }
        public List<long> Curencys { get; set; }
        public List<long> Locations { get; set; }
        public List<Guid?> Vendors { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}
