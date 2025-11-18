using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransferOrders
{
    [Table("CarlErpTransferOrders")]
    public class TransferOrder : BaseAuditedEntity<Guid>
    {
        public const int MaxOrderNumberLength = 128;
        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string TransferNo { get; set; }
        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get; set; }
        public DateTime TransferDate { get; private set; }
        public void SetTranferDate(DateTime transferDate) => TransferDate = transferDate;
        
        [Required]
        public long TransferToLocationId { get; private set; }
        public Location TransferToLocation { get; private set; }
        [Required]
        public long TransferFromLocationId { get; private set; }
        public Location TransferFromLocation { get; private set; }
        [Required]
        public long TransferToClassId { get; private set; }
        public Class TransferToClass { get; private set; }
        [Required]
        public long TransferFromClassId { get; private set; }
        public Class TransferFromClass { get; private set; }

        public void SetFromClass(long classId) => TransferFromClassId = classId;
        public void SetToClass(long classId) => TransferToClassId = classId;
        public void SetFromLocation (long  locationId) => TransferFromLocationId = locationId;
        public void SetToLocation (long locationId) => TransferToLocationId = locationId;


        public string Memo { get; private set; }
        public TransferStatus ShipedStatus { get; private set; }
        public TransactionStatus Status { get; private set; }

        public bool ConvertToIssueAndReceiptTransfer { get; private set; }
        public DateTime? ItemReceiptTransferDate { get; private set; }
        public DateTime? ItemIssueTransferDate { get; private set; }

        public void UpdateStatusToDraft()
        {
            Status = TransactionStatus.Draft;
        }
        public void UpdateStatusToClose()
        {
            Status = TransactionStatus.Close;
        }
        public void UpdateStatusToVoid()
        {
            Status = TransactionStatus.Void;
        }
        public void UpdateStatusToPublish()
        {
            Status = TransactionStatus.Publish;
        }

        public static TransferOrder Create(
            int? tenantId,
            long creatorUserId,
            long locationFromId,
            long locationToId,
            long classFromId,
            long classToId,
            string transferNo,
            DateTime transferDate,
            string transferReference,
            TransactionStatus status,
            string memo,
            bool convertToIssueAndReceiptTransfer,
            DateTime? itemReceiptTransferDate,
            DateTime? itemIssueTransferDate
            )
        {
            return new TransferOrder()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Memo = memo,
                Status = status,
                TransferFromLocationId = locationFromId,
                TransferToLocationId = locationToId,
                TransferFromClassId = classFromId,
                TransferToClassId = classToId,
                TransferNo = transferNo,
                TransferDate = transferDate,
                Reference = transferReference,
                ShipedStatus = TransferStatus.Pending,
                ConvertToIssueAndReceiptTransfer = convertToIssueAndReceiptTransfer,
                ItemIssueTransferDate = itemIssueTransferDate,
                ItemReceiptTransferDate = itemReceiptTransferDate
            };
        }
        
        public void UpdateShipedStatus(TransferStatus status)
        {
            ShipedStatus = status;
        }

        public void Update(
            long lastModifiedUserId,
            long locationFromId,
            long locationToId,
            long classFromId,
            long classToId,
            TransactionStatus status,
            string transferNo,
            DateTime transferDate,
            string transferReference,
            string memo,
            bool convertToIssueAndReceiptTransfer,
            DateTime? itemReceiptTransferDate,
            DateTime? itemIssueTransferDate)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Memo = memo;
            Status = status;
            TransferFromLocationId = locationFromId;
            TransferToLocationId = locationToId;
            TransferFromClassId = classFromId;
            TransferToClassId = classToId;
            TransferNo = transferNo;
            TransferDate = transferDate;
            Reference = transferReference;
            ConvertToIssueAndReceiptTransfer = convertToIssueAndReceiptTransfer;
            ItemReceiptTransferDate = itemReceiptTransferDate;
            ItemIssueTransferDate = itemIssueTransferDate;
        }
    }
}
