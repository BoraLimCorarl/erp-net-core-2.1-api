using Abp.Configuration;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.InvoiceTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CashFlowTemplates.Dto
{
    public class GetListCashFlowTemplateInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public bool? IsActive { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}
