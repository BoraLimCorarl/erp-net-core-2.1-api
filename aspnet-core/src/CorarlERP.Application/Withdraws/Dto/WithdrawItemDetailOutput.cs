using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Withdraws.Dto
{
    [AutoMapFrom(typeof(WithdrawItem))]
    public class WithdrawItemDetailOutput
    {
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }
       
        public string Description { get; set; }
       
        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }   

        public decimal Total { get; set; }

        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

    }
}
