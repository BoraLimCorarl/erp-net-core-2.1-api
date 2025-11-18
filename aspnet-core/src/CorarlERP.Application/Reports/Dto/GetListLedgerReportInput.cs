using Abp.Runtime.Validation;
using CorarlERP.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.AccountTransactions;

namespace CorarlERP.Reports.Dto
{
    public class GetListLedgerReportInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> ChartOfAccounts { get; set; }
        public List<long> AccountType { get; set; }
        public List<long?> Users { get; set; }
        public List<JournalType?> JournalType { get; set; }
        public List<long> Locations { get; set; }
        public bool? NotUsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<long> RelatedAccountTypes { get; set; }
        public List<Guid> RelatedAccounts { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }


    public class LedgerExportReportInput : GetListLedgerReportInput
    {
        public ReportOutput ReportOutput { get; set; }

    }

    public class GetListLedgerReportGroupByOutput
    {

        public string KeyName { get; set; }
        public List<AccountTransactionOutput> Items { get; set; }
    }
       
}
