using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using CorarlERP.VendorCredit;

namespace CorarlERP.BatchNos
{
    [Table("CarlErpVendorCreditItemBatchNos")]
    public class VendorCreditItemBatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid VendorCreditItemId { get; set; }
        public VendorCreditDetail VendorCreditItem { get; set; }

        public Guid BatchNoId { get; set; }
        public BatchNo BatchNo { get; set; }
        public decimal Qty { get; set; }

        public static VendorCreditItemBatchNo Create(int tenantId, long userId, Guid itemIssueItemId, Guid batchNoId, decimal qty)
        {
            return new VendorCreditItemBatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                VendorCreditItemId = itemIssueItemId,
                BatchNoId = batchNoId,
                Qty = qty
            };
        }

        public void Update(long userId, Guid itemIssueItemId, Guid batchNoId, decimal qty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            VendorCreditItemId = itemIssueItemId;
            BatchNoId = batchNoId;
            Qty = qty;
        }
    }
}
