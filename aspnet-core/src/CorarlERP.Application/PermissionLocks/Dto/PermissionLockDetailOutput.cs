using Abp.AutoMapper;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PermissionLocks.Dto
{
    [AutoMapFrom(typeof(PermissionLock))]
    public class PermissionLockDetailOutput
    {
        public long Id { get; set; }
        public TransactionLockType LockTransaction { get; set; }
        public string LockTransactionName { get; set; }
        public LockAction LockAction { get; set; }
        public string LockActionName { get; set; }

        public DateTime? PermissionDate { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }

        public string TransactionNo { get; set; }

        public string PermissionCode { get; set; }
        public DateTime PermissionCodeGenerateDate { get; set; }
        public int ExpiredDuration { get; set; }

        public bool IsActive { get; set; }
    }

}
