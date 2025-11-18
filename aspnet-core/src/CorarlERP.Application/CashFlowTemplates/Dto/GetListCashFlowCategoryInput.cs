using Abp.Configuration;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.InvoiceTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CashFlowTemplates.Dto
{
    public class GetListCashFlowCategoryInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "SortOrder";
            }
        }
    }
}
