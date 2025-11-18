using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AccountTransactions
{
    public class AccountTransaction
    {
        public DateTime Date { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public bool IsPurchase { get; set; }
        public long AccountTypeId { get; set; }
        public TypeOfAccount Type { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public long? CreationTimeIndex { get; set; }
        public long? CurrencyId { get; set; }
        public decimal MultiCurrencyDebit { get; set; }
        public decimal MultiCurrencyCredit { get; set; }
        public decimal MultiCurrencyBalance { get; set; }
        public int RoundDigits { get; set; }
    }
}
