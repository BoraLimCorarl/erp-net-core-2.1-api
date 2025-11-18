using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Withdraws.Dto
{
  public  class CreateOrUpdateWithdrawItemInput
    {
        public Guid? Id { get; set; }        
        public string Description { get; set; }     
        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public Guid AccountId { get; set; }

    }
}
