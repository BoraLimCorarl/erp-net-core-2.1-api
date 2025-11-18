using Abp.AutoMapper;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    [AutoMapFrom(typeof(Journal))]
    public class LedgerReportOutput
    {// All properties name here is use to generate report if u 1 to change please becarefull it can crash report export
        public string JournalNo { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string Memo { get; set; }
        public string JournalType { get; set; }
        public JournalType JournalCode { get; set; }
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Creator { get; set; }
        public string Partner { get; set; }
        public string Account { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
