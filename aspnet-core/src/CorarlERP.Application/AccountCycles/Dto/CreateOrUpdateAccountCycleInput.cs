using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.AccountCycles.Dto
{
    public class CreateOrUpdateAccountCycleInput
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public long Id { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }

   
}
