using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
    public class DefaultTemplateInput
    {
        public string Filter { get; set; }
        public InvoiceTemplateType? TemplateType { get; set; }
    }
}
