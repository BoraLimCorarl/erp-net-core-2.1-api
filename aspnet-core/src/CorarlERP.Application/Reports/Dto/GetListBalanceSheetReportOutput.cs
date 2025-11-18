using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{

    public class GetBalanceSheetInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public ViewOption ViewOption { get; set; }
        public List<long> LocationIds { get; set; }
    }

    public class GetBalanceSheetReportInput: GetBalanceSheetInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class AccountGroupHeader
    {
        public string Name { get; set; }
        public decimal TotalAmount
        {
            get
            {
                var result = AccountLists == null || AccountLists.Count == 0 ? 0 :
                     AccountLists.Sum(t => t.TotalAmount);
                return result;
            }
        }

        public Dictionary<long, TotalBalanceSheetLocationColumns> TotalLocationColumns { get; set; }
        public List<GetListBalanceSheetReportOutput> AccountLists { get; set; }
    }

    public class MainAccountGroupHeader
    {
        public string Title { get; set; }
        public decimal TotalAmount { get; set; }

        public Dictionary<long, TotalBalanceSheetLocationColumns> TotalLocationColumns { get; set; }
        public int RoundingDigit { get; set; }
        public List<LocationSummaryOutput> LocationSummaryHeader { get; set; }
        public List<AccountGroupHeader> Items { get; set; }
    }

    public class GetListBalanceSheetReportOutput
    {
        public long Id { get; set; }
        public string AccountTypeName { get; set; }
        public List<AccountList> Items { get; set; }
       
        public TypeOfAccount AccountType { get; set; }


        public Dictionary<long, TotalBalanceSheetLocationColumns> TotalLocationColumns { get; set; }
        public Dictionary<long, TotalBalanceSheetLocationColumns> NetIncomeTotalLocationColumns { get; set; }
        public Dictionary<long, TotalBalanceSheetLocationColumns> RetainedEarningTotalLocationColumns { get; set; }
        public decimal? NetIncome { get; set; }

        public decimal TotalAmount
        {
            get
            {
                var result = Items == null || Items.Count == 0 ? 0 :
                     Items.Sum(t => t.TotalAmount);

                result += NetIncome == null ? 0 : NetIncome.Value;

                result += RetainedEarning == null ? 0 : RetainedEarning.Value;

                return result;
            }
        }

        public decimal? RetainedEarning { get;  set; }
    }

    public class AccountList
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<long, TotalBalanceSheetLocationColumns> TotalLocationColumns { get; set; }
    }

    public class TotalBalanceSheetLocationColumns
    {
        public string LocationName { get; set; }
        public long LocationId { get; set; }
        public decimal Total { get; set; }
    }


    public class AccountTransactionOutPut
    {
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public long AccountTypeId { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public TypeOfAccount Type { get; set; }
        public decimal Balance { get; set; }

    }

}
