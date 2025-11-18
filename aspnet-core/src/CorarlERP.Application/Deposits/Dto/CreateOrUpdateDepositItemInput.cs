using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Deposits.Dto
{
    public class CreateOrUpdateDepositItemInput
    {
        public Guid? Id { get; set; }

        public Guid AccountId { get; set; }
        public string Description { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
