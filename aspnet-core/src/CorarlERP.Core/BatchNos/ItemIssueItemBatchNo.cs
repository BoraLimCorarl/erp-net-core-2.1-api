using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using CorarlERP.ItemIssues;

namespace CorarlERP.BatchNos
{
    [Table("CarlErpItemIssueItemBatchNos")]
    public class ItemIssueItemBatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid ItemIssueItemId { get; set; }
        public ItemIssueItem ItemIssueItem { get; set; }

        public Guid BatchNoId { get; set; }
        public BatchNo BatchNo { get; set; }
        public decimal Qty { get; set; }

        public static ItemIssueItemBatchNo Create(int tenantId, long userId, Guid itemIssueItemId, Guid batchNoId, decimal qty)
        {
            return new ItemIssueItemBatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                ItemIssueItemId = itemIssueItemId,
                BatchNoId = batchNoId,
                Qty = qty
            };
        }

        public void Update(long userId, Guid itemIssueItemId, Guid batchNoId, decimal qty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            ItemIssueItemId = itemIssueItemId;
            BatchNoId = batchNoId;
            Qty = qty;
        }
    }
}
