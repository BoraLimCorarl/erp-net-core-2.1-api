using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
    public class InvoiceTemplateResultOutput
    {
        public string Html { get; set; }
    }

    public class InvoiceTemplateWithOptionResultOutput : InvoiceTemplateResultOutput
    {
        public string Html { get; set; }
        public bool ShowDetail { get; set; }
        public bool ShowSummary { get; set; }
        public TemplateOption TemplateOption { get; set; }
    }
}
