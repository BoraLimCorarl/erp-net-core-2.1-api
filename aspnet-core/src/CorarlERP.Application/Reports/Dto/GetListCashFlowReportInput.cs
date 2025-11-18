using Abp.Runtime.Validation;
using CorarlERP.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.AccountTransactions;

namespace CorarlERP.Reports.Dto
{
    public class GetListCashFlowReportInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> ChartOfAccounts { get; set; }
        public List<long> AccountType { get; set; }
        //public List<long?> Users { get; set; }
        //public List<JournalType?> JournalType { get; set; }
        public List<long> Locations { get; set; }
        public ViewOption ViewOption { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "AccountCode";
            }
        }
    }


    public class ExportCashFlowReportInput : GetListCashFlowReportInput
    {
        public ReportOutput ReportOutput { get; set; }

    }


    public class GetListDirectCashFlowReportInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long> Locations { get; set; }
        public ViewOption ViewOption { get; set; }
        public Guid? CashFlowTemplateId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "AccountCode";
            }
        }
    }


    public class ExportDirectCashFlowReportInput : GetListDirectCashFlowReportInput
    {
        public ReportOutput ReportOutput { get; set; }

    }

}
