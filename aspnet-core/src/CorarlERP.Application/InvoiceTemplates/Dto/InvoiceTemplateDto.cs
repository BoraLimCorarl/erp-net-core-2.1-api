using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.TransactionTypes.Dto;

namespace CorarlERP.InvoiceTemplates.Dto
{
    [AutoMapFrom(typeof(InvoiceTemplate))]
    public class InvoiceTemplateDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? GalleryId { get; set; }
        public string TemplateOption { get; set; }
        public bool IsActive { get; set; }
        public InvoiceTemplateType TemplateType { get; set; }
        public bool ShowDetail { get; set; }
        public bool ShowSummary { get; set; }
        public List<TransactionTypeSummaryOutput> SaleTypes { get; set; }
    }
}
