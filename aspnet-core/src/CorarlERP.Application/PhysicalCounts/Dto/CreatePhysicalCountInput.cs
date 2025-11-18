using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class CreatePhysicalCountInput
    {
        public TransactionStatus Status { get; set; }

        public string PhysicalCountNo { get; set; }
        public DateTime PhysicalCountDate { get; set; }
        public string Reference { get; set; }
        public long LocationId { get; set; }
        public long ClassId { get; set; }
        public List<Guid> ItemIds { get; set; }
        public string Memo { get; set; }
        public List<CreateOrUpdatePhysicalItemInput> PhysicalCountItems { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
