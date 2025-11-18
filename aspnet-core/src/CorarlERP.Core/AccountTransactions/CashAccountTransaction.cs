using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AccountTransactions
{
    public class CashAccountTransaction
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public long AccountTypeId { get; set; }
        public TypeOfAccount Type { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public long? CreationTimeIndex { get; set; }

        public Guid JournalId { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public DateTime JournalDate { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }

        public string JournalMemo { get; set; }
        public string Description { get; set; }
        public JournalType? JournalType { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid? BankTransferId { get; set; }

        public List<OtherReferenceOutput> OtherReferences { get; set; }

        public long? CurrencyId { get; set; }
        public decimal MultiCurrencyDebit { get; set; }
        public decimal MultiCurrencyCredit { get; set; }
        public decimal MultiCurrencyBalance { get; set; }
        public int RoundDigits { get; set; }
    }

    public class OtherReferenceOutput
    {
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid? PartnerId { get; set; }
        public string PartnerName { get; set; }

    }
}
