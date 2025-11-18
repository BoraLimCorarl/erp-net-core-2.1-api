using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Locations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.BankTransfers
{
    [Table("CarlErpBankTransfers")]
    public class BankTransfer : BaseAuditedEntity<Guid>
    {
        public const int MaxOrderNumberLength = 128;
        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string BankTransferNo { get; private set; }
        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get; private set; }
        public DateTime BankTransferDate { get; private set; }
        public void SetBankTransferDate(DateTime bankTransferDate) => BankTransferDate = bankTransferDate;

        [Required]
        public long TransferToClassId { get; private set; }
        public Class TransferToClass { get; private set; }
        [Required]
        public long TransferFromClassId { get; private set; }
        public Class TransferFromClass { get; private set; }

        public void SetFromClass(long classId) => TransferFromClassId = classId;
        public void SetToClass(long classId) => TransferToClassId = classId;
        public string Memo { get; private set; }
        public TransactionStatus Status { get; private set; }
        [Required]
        public Guid BankTransferToAccountId { get; private set; }
        public ChartOfAccount BankTransferToAccount { get; private set; }
        [Required]
        public Guid BankTransferFromAccountId { get; private set; }
        public ChartOfAccount BankTransferFromAccount { get; private set; }
        public decimal Amount { get; private set; }
        public void UpdateStatus(TransactionStatus status)
        {
            this.Status = status;
        }

        public Location FromLocation { get; private set; }
        public long?  FromLocationId { get; private set; }
        public Location ToLocation { get; private set; }
        public long? ToLocationId { get; private set; }

        public void SetFromLocation(long locationId) => FromLocationId = locationId;
        public void SetToLocation(long locationId) => ToLocationId = locationId;


        public static BankTransfer Create(int? tenantId, long creatorUserId, Guid bankAccountFromId, Guid bankAccountToId, long classFromId,
                                          long classToId, string bankTransferNo, DateTime bankTransferDate, string reference,
                                          TransactionStatus status, string memo,decimal amount,long? fromLocationId,long? toLocationId)
        {
            return new BankTransfer()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Memo = memo,
                Status = status,
                BankTransferFromAccountId = bankAccountFromId,
                BankTransferToAccountId = bankAccountToId,
                TransferFromClassId = classFromId,
                TransferToClassId = classToId,
                BankTransferNo = bankTransferNo,
                BankTransferDate = bankTransferDate,
                Reference = reference,
                Amount = amount,
                FromLocationId = fromLocationId,
                ToLocationId = toLocationId
            };
        }

        public void UpdateStatusToDraft()
        {
            Status = TransactionStatus.Draft;
        }

        public void UpdateStatusToVoid()
        {
            Status = TransactionStatus.Void;
        }

        public void UpdateStatusToPublish()
        {
            Status = TransactionStatus.Publish;
        }

        public void Update(long lastModifiedUserId, Guid bankAccountFromId, Guid bankAccountToId, long classFromId,
                           long classToId, string bankTransferNo, DateTime bankTransferDate, string reference,
                           TransactionStatus status, string memo,decimal amount,long? fromLocationId, long? toLocationId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Memo = memo;
            Status = status;
            BankTransferFromAccountId = bankAccountFromId;
            BankTransferToAccountId = bankAccountToId;
            TransferFromClassId = classFromId;
            TransferToClassId = classToId;
            BankTransferNo = bankTransferNo;
            BankTransferDate = bankTransferDate;
            Reference = reference;
            Amount = amount;
            FromLocationId = fromLocationId;
            ToLocationId = toLocationId;

        }
    }
}
