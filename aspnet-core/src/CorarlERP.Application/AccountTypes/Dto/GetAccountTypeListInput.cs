using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.AccountTypes.Dto
{
    public class GetAccountTypeListInput: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "AccountTypeName";
            }
        }
    }
    
}
