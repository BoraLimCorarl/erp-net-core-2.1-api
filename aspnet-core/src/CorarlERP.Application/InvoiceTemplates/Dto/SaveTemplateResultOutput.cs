using CorarlERP.Galleries;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
    public class SaveTemplateResultOutput
    {
        public Gallery Gallery { get; set; }
        public string OldFile { get; set; }
        public string NewFile { get; set; }
    }
}
