using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.InvoiceTemplates.Dto
{
    [AutoMapFrom(typeof(InvoiceTemplateMap))]
    public class InvoiceTemplateMapDto
    {
        public Guid? Id { get; set; }

        public InvoiceTemplateType TemplateType { get; set; }
        public string TemplateTypeName { get; set; }

        public long? SaleTypeId { get; set; }
        public string SaleTypeName { get; set; }

        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; }
    }
}
