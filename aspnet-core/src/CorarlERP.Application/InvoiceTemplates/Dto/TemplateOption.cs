using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
    public class TemplateOption
    {
        public PageSize PageSize { get; set; }  
        public List<FieldProperty> DetailColumns { get; set; }
    }

    public class PageSize
    {
        public string Name { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Margin { get; set; }
        public string Unit { get; set; }
    }

    public class FieldProperty
    {
        public string Field { get; set; }    
        public bool  Visible { get; set; }
    }
}
