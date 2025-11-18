using Abp;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{

    public class GetTrialBalanceInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public ViewOption ViewOption { get; set; }
        public List<long> LocationIds { get; set; }
        public List<Guid> AccountIds { get; set; }
        public List<long> AccountTypeIds { get; set; }
    }

    public class GetTrialBalanceReportInput: GetTrialBalanceInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class TrialBalanceReportResultOutput
    { 
        public List<TrialBalanceAccountList> Accounts { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public List<LocationSummaryDto> locationSummaryHeader { get; set; }
        public Dictionary<long, TotalTrialBalanceLocationColumns> LocationColumnDic { get; set; }
    }

    public class TrialBalanceAccountList
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Dictionary<long, TotalTrialBalanceLocationColumns> TotalLocationColumns { get; set; }
    }

    public class TrialBalanceAccountOutput
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
    }


    public class TotalTrialBalanceLocationColumns
    {
        public string LocationName { get; set; }
        public long LocationId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

}
