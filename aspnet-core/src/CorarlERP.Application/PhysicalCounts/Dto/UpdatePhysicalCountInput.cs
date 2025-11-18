using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class UpdatePhysicalCountInput: CreatePhysicalCountInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }

        public long CurrencyId { get; set; }
        public Guid AdjustmentAccountId { get; set; }

        public Guid? ItemIssueId { get; set; }
        public Guid? ItemReceiptId { get; set; }
    }
}
