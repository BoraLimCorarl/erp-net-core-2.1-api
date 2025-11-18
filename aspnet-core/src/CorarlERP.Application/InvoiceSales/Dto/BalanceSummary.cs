using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Invoices.Dto
{
    public class BalanceSummary
    {
        public decimal Total { get; set; }       
        public string chartOfAccount {get;set;}
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }

    }
}
