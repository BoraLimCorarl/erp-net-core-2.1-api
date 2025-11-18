using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos
{
    public class BatchNoItemBalanceOutput
    {
        public Guid ItemId { get; set; }
        public long LocationId { get; set; }
        public long LotId { get; set; }
        public Guid BatchNoId { get; set; }
        public decimal Qty { get; set; }
    }

    public class BatchNoItemOutput
    {
        public Guid? Id { get; set; }
        public Guid BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public decimal Qty { get; set; }
        public Guid TransactionItemId { get; set; }
        public bool IsStandard { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class BatchNoWithExpiration
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
