using Abp.AutoMapper;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Journals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Journals.Dto
{
    [AutoMapFrom(typeof(Journal))]
    public class JournalDetailOutput
    {
        public Guid Id { get; set; }

        public string JournalNo { get; set; }

        public DateTime Date { get; set; }

        public string Memo { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public long CurrencyId { get; set; }
        public Currencies.Dto.CurrencyDetailOutput Currency { get; set; }

        public long? ClassId { get; private set; }
        public Classes.Dto.ClassSummaryOutput Class { get; set; }

        public Guid? GeneralJournalId { get; set; }
        public JournalSummaryOutput GeneralJournal { get; set; }

        public string Reference { get; set; }

        public TransactionStatus Status { get; set; }
     
        public string LocationName { get; set; }
        public long? LocationId { get; set; }

        public List<JournalItemDetailOutput> JournalItems { get; set; }
        //public Guid? BillId { get; private set; }
        //public Bill Bill { get; set; }

        public Guid? ItemReceiptId { get; private set; }
        //public ItemReceipt ItemReceipt { get; private set; }

    }
   
}
[AutoMapFrom(typeof(Journal))]
public class JournalSummaryOutput
{
    public Guid Id { get; set; }

    public string JournalNo { get; private set; }

    public DateTime Date { get; private set; }
}
