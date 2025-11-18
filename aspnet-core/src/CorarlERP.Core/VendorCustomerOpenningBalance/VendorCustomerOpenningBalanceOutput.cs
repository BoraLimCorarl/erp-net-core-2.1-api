using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorCustomerOpenningBalance
{
    public class TransactionOpenningBalanceOutput
    {
        public Guid? TransactionId { get; set; }
        public JournalType Key { get; set; }
        public decimal Balance { get; set; }
        public decimal MultiCurrencyBalance { get; set; }
        public long LocationId { get; set; }
    }
}
