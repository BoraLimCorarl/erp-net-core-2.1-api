using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PermissionLocks.Dto
{
    public class CreatePermissionLockInput
    {
        public TransactionLockType LockTransaction { get; set; }
        public LockAction LockAction { get; set; }

        public DateTime? PermissionDate { get; set; }

        public long? LocationId { get; set; }

        public string TransactionNo { get; set; }

        public string PermissionCode { get; set; }
        public DateTime PermissionCodeGenerateDate { get; set; }
        
        public int ExpiredDuration { get; set; }

        public bool IsActive { get; set; }

    }
}
