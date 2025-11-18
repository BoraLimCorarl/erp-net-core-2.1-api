using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.TransactionTypes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.InvoiceTemplates
{
    public enum InvoiceTemplateType
    {
        Invoice = 0,
        SaleOrder = 1
    }

    [Table("CarlErpInvoiceTemplateMaps")]
    public class InvoiceTemplateMap : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public InvoiceTemplateType TemplateType { get; set; }

        public TransactionType SaleType { get; private set; }
        public long? SaleTypeId { get; private set; }

        public Guid TemplateId { get; set; }
        public InvoiceTemplate InvoiceTemplate { get; private set; }

        public static InvoiceTemplateMap Create(int? tenantId, long? creatorUserId, InvoiceTemplateType templateType, long? saleTypeId, Guid templateId)
        {
            return new InvoiceTemplateMap()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TemplateType = templateType,
                SaleTypeId = saleTypeId,
                TemplateId = templateId
            };
        }
        public void Update(long? lastModifiedUserId, InvoiceTemplateType templateType, long? saleTypeId, Guid templateId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TemplateType = templateType;
            SaleTypeId = saleTypeId;
            TemplateId = templateId;
        }
    }
}
