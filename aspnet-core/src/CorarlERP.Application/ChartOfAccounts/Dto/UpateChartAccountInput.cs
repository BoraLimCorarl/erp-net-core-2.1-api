using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ChartOfAccounts.Dto
{
    public class UpateChartAccountInput: CreateChartAccountInput
    {
        public Guid Id { get; set; }
    }
}
