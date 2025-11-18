using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using CorarlERP.ItemReceipts;

namespace CorarlERP.BatchNos
{
    [Table("CarlErpItemReceiptItemBatchNos")]
    public class ItemReceiptItemBatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid ItemReceiptItemId { get; set; }
        public ItemReceiptItem ItemReceiptItem { get; set; }

        public Guid BatchNoId { get; set; }
        public BatchNo BatchNo { get; set; }
        public decimal Qty { get; set; }

        public static ItemReceiptItemBatchNo Create(int tenantId, long userId, Guid itemReceiptItemId, Guid batchNoId, decimal qty)
        {
            return new ItemReceiptItemBatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                ItemReceiptItemId = itemReceiptItemId,
                BatchNoId = batchNoId,
                Qty = qty
            };
        }

        public void Update(long userId, Guid itemReceiptItemId, Guid batchNoId, decimal qty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            ItemReceiptItemId = itemReceiptItemId;
            BatchNoId = batchNoId;
            Qty = qty;
        }
    }
}
