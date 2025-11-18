using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ChartOfAccounts.Dto
{
    public class ChartOfAccountDto
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
    }

    public class ChartOfAccountSummaryWithTax
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public long TaxId { get; set; }
    }
}
