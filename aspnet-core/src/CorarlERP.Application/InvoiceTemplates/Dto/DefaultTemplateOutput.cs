using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
    public class DefaultTemplateOutput
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public InvoiceTemplateType TemplateType { get; set; }
    }
}
