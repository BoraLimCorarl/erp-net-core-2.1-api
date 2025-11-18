using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Abp.Extensions;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.AccountTransactions;

namespace CorarlERP.Reports.Dto
{
    public class CashReportOutput
    {
        public Guid JournalId { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public DateTime JournalDate { get; set; }
        public long? UserId { get; set; }
        public string User { get; set; }
      
        public string JournalMemo { get; set; }
        public string Description { get; set; }
       
        public Guid JournalItemId { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public long AccountTypeId { get; set; }
        public TypeOfAccount AccountType { get; set; }
        public long? CreationTimeIndex { get; set; }
        public DateTime? CreationTime { get; set; }

        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }

        public decimal Beginning { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }            
        public decimal AccumBalance { get; set; }

        public decimal Beginnings { get; set; }
        public decimal Debits { get; set; }
        public decimal Credits { get; set; }
        public decimal Balances { get; set; }
        public decimal AccumBalances { get; set; }
        public List<CashReportMultiCurrencyColumn> MultiCurrencyColumns { get; set; }       

        public long? LocationId { get; set; }
        public string LocationName { get; set; }

        public JournalType? JournalType { get; set; }

        public Guid? TransactionId { get; set; }
        public Guid? BankTransferId { get; set; }

        public List<OtherReferenceOutput> OtherReferences { get; set; }
        public string PartnerNames { get => OtherReferences == null || !OtherReferences.Any() ? "" : OtherReferences.Select(s => s.PartnerName).Distinct().Join(", "); }
        public string OtherJournalNos { get => OtherReferences == null || !OtherReferences.Any() ? "" : OtherReferences.Select(s => s.JournalNo).Join(", "); }
        public string OtherRefs { get => OtherReferences == null || !OtherReferences.Any() ? "" : OtherReferences.Select(s => s.Reference).Join(", "); }       

        public int RoundDigits { get; set; }
    }

    public class CashReportMultiCurrencyColumn
    {
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }

        public decimal Beginnings { get; set; }
        public decimal Debits { get; set; }
        public decimal Credits { get; set; }
        public decimal Balances { get; set; }
        public decimal AccumBalances { get; set; }
    }

}
