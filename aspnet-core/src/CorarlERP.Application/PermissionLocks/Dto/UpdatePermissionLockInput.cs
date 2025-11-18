using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PermissionLocks.Dto
{
    public class UpdatePermissionLockInput : CreatePermissionLockInput
    {
        public long Id { get; set; }
    }
}
