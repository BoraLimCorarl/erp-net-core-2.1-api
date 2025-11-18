using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using CorarlERP.ItemReceiptCustomerCredits;

namespace CorarlERP.BatchNos
{
    [Table("CarlErpItemReceiptCustomerCreditItemBatchNos")]
    public class ItemReceiptCustomerCreditItemBatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid ItemReceiptCustomerCreditItemId { get; set; }
        public ItemReceiptItemCustomerCredit ItemReceiptCustomerCreditItem { get; set; }

        public Guid BatchNoId { get; set; }
        public BatchNo BatchNo { get; set; }
        public decimal Qty { get; set; }

        public static ItemReceiptCustomerCreditItemBatchNo Create(int tenantId, long userId, Guid itemReceiptCustomerCreditItemId, Guid batchNoId, decimal qty)
        {
            return new ItemReceiptCustomerCreditItemBatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                ItemReceiptCustomerCreditItemId = itemReceiptCustomerCreditItemId,
                BatchNoId = batchNoId,
                Qty = qty
            };
        }

        public void Update(long userId, Guid itemReceiptCustomerCreditItemId, Guid batchNoId, decimal qty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            ItemReceiptCustomerCreditItemId = itemReceiptCustomerCreditItemId;
            BatchNoId = batchNoId;
            Qty = qty;
        }
    }
}
