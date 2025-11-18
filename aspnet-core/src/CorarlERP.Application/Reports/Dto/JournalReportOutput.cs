using Abp.AutoMapper;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    [AutoMapFrom(typeof(Journal))]
    public class JournalReportOutput
    {// All properties name here is use to generate report if u 1 to change please becarefull it can crash report export
        public string JournalNo { get; set; }
        public DateTime CreationTime { get; set; }

        public long? CreationTimeIndex { get; set; }
        public decimal Debit { get; set; }
        public decimal TotalDebit { get; set; }

        public decimal Credit { get; set; }
        public decimal TotalCredit { get; set; }
        public string Memo { get; set; }

        public string JournalType { get; set; }
        public JournalType JournalCode { get; set; }
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public TransactionStatus Status { get; set; }
        public string User { get; set; }
        public long? UserId { get; set; }
        public string Location { get; set; }
        public long? LocationId { get; set; }
        public long CurrencyId { get; set; }
        public string Currency { get; set; }
        public long? ClassId { get; set; }
        public string Class { get; set; }

        public Guid? BankTransferId { get; set; }
        public Guid? ProductionId { get; set; }
        public Guid? TransferId { get; set; }
        public Guid? TransactionId { get; set; }

        //public string Partner { get => string.IsNullOrWhiteSpace(VendorName) ? CustomerName : VendorName; }
        //public Guid? PartnerId { get => VendorId == null || VendorId == Guid.Empty ? CustomerId : VendorId; }

        //public string VendorName { get; set; }
        //public Guid? VendorId { get; set; }
        //public string CustomerName { get; set; }
        //public Guid? CustomerId { get; set; }

        public int RoundingDigit { get; set; }
        public List<JournalItemDetailOutput> JournalItems { get; set; }
        
    }
    
}
