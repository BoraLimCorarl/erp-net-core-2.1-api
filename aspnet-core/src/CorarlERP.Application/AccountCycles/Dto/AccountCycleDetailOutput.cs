using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.AccountCycles.Dto
{
    [AutoMapFrom(typeof(AccountCycle))]
    public class AccountCycleDetailOutput
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
    }
}
