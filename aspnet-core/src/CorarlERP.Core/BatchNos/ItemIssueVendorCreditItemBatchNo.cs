using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using CorarlERP.ItemIssueVendorCredits;

namespace CorarlERP.BatchNos
{
    [Table("CarlErpItemIssueVendorCreditItemBatchNos")]
    public class ItemIssueVendorCreditItemBatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid ItemIssueVendorCreditItemId { get; set; }
        public ItemIssueVendorCreditItem ItemIssueVendorCreditItem { get; set; }

        public Guid BatchNoId { get; set; }
        public BatchNo BatchNo { get; set; }
        public decimal Qty { get; set; }

        public static ItemIssueVendorCreditItemBatchNo Create(int tenantId, long userId, Guid itemIssueVendorCreditItemId, Guid batchNoId, decimal qty)
        {
            return new ItemIssueVendorCreditItemBatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                ItemIssueVendorCreditItemId = itemIssueVendorCreditItemId,
                BatchNoId = batchNoId,
                Qty = qty
            };
        }

        public void Update(long userId, Guid itemIssueVendorCreditItemId, Guid batchNoId, decimal qty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            ItemIssueVendorCreditItemId = itemIssueVendorCreditItemId;
            BatchNoId = batchNoId;
            Qty = qty;
        }
    }
}
