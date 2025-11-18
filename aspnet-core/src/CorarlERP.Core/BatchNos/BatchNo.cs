using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.BatchNos
{

    [Table("CarlErpBatchNos")]
    public class BatchNo : AuditedEntity<Guid>, IMustHaveTenant
    {
        [Required]
        public string Code { get; private set; }

        public Guid ItemId { get; private set; }
        public bool IsStandard { get; private set; }

        public decimal ReceiptQty { get; private set; }
        public decimal IssueQty { get; private set; }
        public decimal BalanceQty { get; private set; }

        public int TenantId { get; set; }
        public bool IsSerial { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? ExpirationDate { get; set; }

        public static BatchNo Create(int tenantId, long userId, string code, Guid itemId, bool isStandard, bool isSerial, DateTime? expirationDate)
        {
            return new BatchNo
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                Code = code,
                ItemId = itemId,
                IsStandard = isStandard,
                IsSerial = isSerial,
                ExpirationDate = expirationDate.HasValue ? expirationDate.Value.Date : (DateTime?) null
            };
        }

        public void Update(long userId, string code, Guid itemId, bool isStandard, bool isSerial, DateTime? expirationDate)
        {
            LastModifierUserId = userId;
            LastModificationTime= Clock.Now;
            Code = code;
            ItemId = itemId;
            IsStandard = isStandard;
            IsSerial = isSerial;
            ExpirationDate = expirationDate.HasValue ? expirationDate.Value.Date : (DateTime?) null;
        }

        public void SetBalance(decimal receiptQty, decimal issueQty)
        {
            ReceiptQty = receiptQty;
            IssueQty = issueQty;
            BalanceQty = receiptQty - issueQty;
        }
    }
}
