using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
   public class GetInvoiceTemplateListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public InvoiceTemplateType? TemplateType { get; set; }
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
