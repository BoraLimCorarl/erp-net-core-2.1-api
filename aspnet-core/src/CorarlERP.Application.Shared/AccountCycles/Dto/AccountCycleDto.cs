using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.AccountCycles.Dto
{
    public class AccountCycleDto
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public DateTime EndDate { get; set; }
       
    }
}
