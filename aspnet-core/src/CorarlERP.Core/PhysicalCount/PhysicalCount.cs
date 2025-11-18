using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Classes;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PhysicalCount
{
    [Table("CarlErpPhysicalCounts")]
    public class PhysicalCount : AuditedEntity<Guid>, IMayHaveTenant
    {
        public const int MaxOrderNumberLength = 128;
        public int? TenantId { get; set; }
        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string PhysicalCountNo { get; set; }

        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get; set; }

        public DateTime PhysicalCountDate { get; private set; }
        public void SetPhysicalCountDate(DateTime physicalCountDate) => PhysicalCountDate = physicalCountDate;

        [Required]
        public long LocationId { get; private set; }
        public Location Location { get; private set; }
        
        [Required]
        public long ClassId { get; private set; }
        public Class Class { get; private set; }

        public void SetClass(long classId) => ClassId = classId;
        public void SetLocation(long  locationId) => LocationId = locationId;
        
        public string Memo { get; private set; }

        public TransactionStatus Status { get; private set; }


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

        public static PhysicalCount Create(
            int? tenantId,
            long creatorUserId,
            long locationId,
            long classId,
            string physicalCountNo,
            DateTime physicalCountDate,
            string transferReference,
            TransactionStatus status,
            string memo)
        {
            return new PhysicalCount()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Memo = memo,
                Status = status,
                LocationId = locationId,
                ClassId = classId,
                PhysicalCountNo = physicalCountNo,
                PhysicalCountDate = physicalCountDate,
                Reference = transferReference,
            };
        }

        public void Update(
            long lastModifiedUserId,
            long locationId,
            long classId,
            TransactionStatus status,
            string physicalCountNo,
            DateTime physicalCountDate,
            string reference,
            string memo)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Memo = memo;
            Status = status;
            LocationId = locationId;
            ClassId = classId;
            
            PhysicalCountNo = physicalCountNo;
            PhysicalCountDate = physicalCountDate;
            Reference = reference;
        }

        public void Close(long lastModifiedUserId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            UpdateStatusToClose();
        }
    }
}
