using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using CorarlERP.CustomerCredits;

namespace CorarlERP.BatchNos
{
    [Table("CarlErpCustomerCreditItemBatchNos")]
    public class CustomerCreditItemBatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid CustomerCreditItemId { get; set; }
        public CustomerCreditDetail CustomerCreditItem { get; set; }

        public Guid BatchNoId { get; set; }
        public BatchNo BatchNo { get; set; }
        public decimal Qty { get; set; }

        public static CustomerCreditItemBatchNo Create(int tenantId, long userId, Guid customerCreditItemId, Guid batchNoId, decimal qty)
        {
            return new CustomerCreditItemBatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                CustomerCreditItemId = customerCreditItemId,
                BatchNoId = batchNoId,
                Qty = qty
            };
        }

        public void Update(long userId, Guid customerCreditItemId, Guid batchNoId, decimal qty)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            CustomerCreditItemId = customerCreditItemId;
            BatchNoId = batchNoId;
            Qty = qty;
        }
    }
}
