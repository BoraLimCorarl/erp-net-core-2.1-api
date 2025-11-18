using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.CashFlowTemplates
{

    [Table("CarlErpCashFlowTemplateCategories")]
    public class CashFlowTemplateCategory : AuditedEntity<Guid>, IMustHaveTenant
    {
        public Guid TemplateId { get; private set; }
        public CashFlowTemplate Template { get; private set; }

        public Guid CategoryId { get; private set; }
        public CashFlowCategory Category { get; private set; }

        public int TenantId { get; set; }

        public static CashFlowTemplateCategory Create(int tenantId, long? creatorUserId, Guid templateId, Guid categoryId)
        {
            return new CashFlowTemplateCategory
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                TemplateId = templateId,
                CategoryId = categoryId,
            };
        }

        public void Update(long? lastModifiedUserId, Guid templateId, Guid categoryId)

        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TemplateId = templateId;
            CategoryId = categoryId;
        }
    }
}
