using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Locks
{
    [Table("CarlErpLocks")]
    public class Lock : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public bool IsLock { get; private set; }
        public void SetIsLock(bool value) { IsLock = value; }

        public DateTime? LockDate { get; private set; }
        public void SetLockDate(DateTime? date) { LockDate = date; }

        public TransactionLockType LockKey {get; private set ;}

        public DateTime? GenerateDate { get; private set; }

        public string Password { get; private set; }

        public int ExpiredTime { get; private set; }

        public static Lock Create(int? tenantId, long creatorUserId, bool isLock, DateTime? LockDate,TransactionLockType lockKey,DateTime? generateDate,string password,int expiredTime)
        {
            return new Lock()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,               
                LockDate = LockDate,
                IsLock = isLock,
                LockKey = lockKey,
                Password = password,
                ExpiredTime = expiredTime,
                GenerateDate = generateDate,
            };
        }     
        public void Update(long lastModifiedUserId, bool isLock, DateTime? lockDate, TransactionLockType lockKey, DateTime? generateDate, string password, int expiredTime)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            IsLock = isLock;
            LockDate = lockDate;
            LockKey = lockKey;
            Password = password;
            ExpiredTime = expiredTime;
            GenerateDate = generateDate;
        }

        public void GeneratePassword (DateTime? generateDate, string passwork, int expiredTime)
        {
            this.GenerateDate = generateDate;
            this.ExpiredTime = expiredTime;
            this.Password = passwork;
        }
        public void ClearPassword()
        {
            this.GenerateDate = null;
            this.ExpiredTime = 0;
            this.Password = null;
        }
    }
}
