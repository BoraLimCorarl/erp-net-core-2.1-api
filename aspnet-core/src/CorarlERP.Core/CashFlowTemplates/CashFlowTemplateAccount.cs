using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.CashFlowTemplates
{
    
    [Table("CarlErpCashFlowTemplateAccounts")]
    public class CashFlowTemplateAccount : AuditedEntity<Guid>, IMustHaveTenant
    {
        public Guid TemplateId { get; private set; }
        public CashFlowTemplate Template { get; private set; }

        public Guid CategoryId { get; private set; }
        public CashFlowCategory Category { get; private set; }

        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }

        public Guid? AccountGroupId { get; private set; }
        public CashFlowAccountGroup AccontGroup { get; private set; }
        public Guid? OutAccountGroupId { get; private set; }
        public CashFlowAccountGroup OutAccontGroup { get; private set; }
        public int TenantId { get; set; }
        public bool? CashInFlow { get; private set; }

        public static CashFlowTemplateAccount Create(int tenantId, long? creatorUserId, Guid templateId, Guid categoryId, Guid accountId, Guid? accountGroupId, Guid? outAccountGroupId)
        {
            return new CashFlowTemplateAccount
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                TemplateId = templateId,
                CategoryId = categoryId,
                AccountId = accountId,
                AccountGroupId = accountGroupId,
                OutAccountGroupId = outAccountGroupId
            };
        }

        public void Update(long? lastModifiedUserId, Guid templateId, Guid categoryId, Guid accountId, Guid? accountGroupId, Guid? outAccountGroupId)

        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TemplateId = templateId;
            CategoryId = categoryId;
            AccountId = accountId;
            AccountGroupId = accountGroupId;
            OutAccountGroupId = outAccountGroupId;
        }
    }
}
