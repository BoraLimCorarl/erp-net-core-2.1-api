using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Locks
{
    [Table("CarlErpPermissionLocks")]
    public class PermissionLock : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public TransactionLockType LockTransaction { get; private set; }
        public LockAction LockAction { get; private set; }

        public DateTime? PermissionDate { get; private set; }

        public long? LocationId { get; private set; }
        public Location Location { get; private set; }

        public string TransactionNo { get; set; }

        public string PermissionCode { get; private set; }
        public DateTime PermissionCodeGenerateDate { get; private set; }

        public int ExpiredDuration { get; private set; }

        public bool IsActive { get; private set; }
        public void Enable(bool isActive) { IsActive = isActive; }

        public static PermissionLock Create(int? tenantId, long creatorUserId, TransactionLockType transactionLock, LockAction lockAction, DateTime? permissionDate, long? locationId, string transactionNo, string permissionCode, DateTime permissionCodeGenerateDate, int expiredDuration)
        {
            return new PermissionLock()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                LockTransaction = transactionLock,
                LockAction = lockAction,
                LocationId = locationId,
                PermissionDate = permissionDate,
                PermissionCode = permissionCode,
                PermissionCodeGenerateDate = permissionCodeGenerateDate,
                ExpiredDuration = expiredDuration,
                TransactionNo = transactionNo,
                IsActive = true,
            };
        }

        public void Update(long lastModifiedUserId, TransactionLockType transactionLock, LockAction lockAction, DateTime? permissionDate, long? locationId, string transactionNo, string permissionCode, int expiredDuration)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LockTransaction = transactionLock;
            LockAction = lockAction;
            LocationId = locationId;
            PermissionDate = permissionDate;
            PermissionCode = permissionCode;
            ExpiredDuration = expiredDuration;
            TransactionNo = transactionNo;           
        }

    }

}
