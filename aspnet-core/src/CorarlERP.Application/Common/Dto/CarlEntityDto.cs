using System;
using Abp.Application.Services.Dto;
using CorarlERP.PermissionLocks.Dto;

namespace CorarlERP.Common.Dto
{
    public class CarlEntityDto: EntityDto<Guid>
    {
        public bool IsConfirm { get; set; }
        public long PermissionLockId { get; set; }

    }
}
