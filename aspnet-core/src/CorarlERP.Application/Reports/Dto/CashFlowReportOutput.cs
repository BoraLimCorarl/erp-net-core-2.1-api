using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Abp.Extensions;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.AccountTransactions;
using CorarlERP.ChartOfAccounts;

namespace CorarlERP.Reports.Dto
{
    
    public class CashFlowReportResultOutput
    {
        public bool SplitCashInAndCashOutFlow { get; set; }
        public List<CashFlowCategoryReportOutput> CashInFlows { get; set; }
        public List<CashFlowCategoryReportOutput> CashOutFlows { get; set; }
        public CashFlowSummaryReportOutput CashInFlow { get; set; }
        public CashFlowSummaryReportOutput CashOutFlow { get; set; }
        public CashFlowSummaryReportOutput NetCashFlow { get; set; }
        public CashFlowSummaryReportOutput CashBeginning { get; set; }
        public CashFlowSummaryReportOutput CashEnding { get; set; }
        public Dictionary<long, string> ColumnHeaders { get; set; }
        public List<long> CashBankAccountTypes { get; set; }
        public List<CashFlowCategoryReportOutput> CashTransfers { get; set; }
    }

    public class CashFlowSummaryReportOutput
    {
        public string Name { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Total { get; set; }
        public Dictionary<long, CashFlowReportLocationColumn> LocationColumnDic { get; set; }
    }

    public class CashFlowCategoryReportOutput: CashFlowSummaryReportOutput
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SortOrder { get; set; }
        public List<CashFlowReportOutput> Accounts { get; set; }      
    }

    public class CashFlowReportOutput
    {
        public Guid JournalId { get; set; }
        public DateTime Date { get; set; }       
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public long AccountTypeId { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }        

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Total { get; set; }

        public bool CashInFlow { get; set; }      
        public Dictionary<long, CashFlowReportLocationColumn> LocationColumnDic { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategorySortOrder { get; set; }

        public List<Guid> AccountIds { get; set; }

    }

    public class CashFlowReportLocationColumn
    {
        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Total { get; set; }

    }

    public class IndirectCashFlowReportResultOutput
    {
        public IndirectCashFlowReportOutput ProfitLoss { get; set; }
        public List<IndirectCashFlowReportOutput> DepreciationAmotizations { get; set; }
        public IndirectCashFlowReportOutput ProfitLossBeforeChangesInWorkingCapital { get; set; }
        public List<IndirectCashFlowReportOutput> CurrentAssetAndCurrentLiabilies { get; set; }
        public IndirectCashFlowReportOutput NetCashFlowFromOperation { get; set; }

        public List<IndirectCashFlowReportOutput> CashFlowFromInvestments { get; set; }
        public IndirectCashFlowReportOutput NetCashFlowFromInvestment { get; set; }

        public List<IndirectCashFlowReportOutput> CashFlowFromFinancings { get; set; }
        public IndirectCashFlowReportOutput NetCashFlowFromFinancing { get; set; }

        public IndirectCashFlowReportOutput NetCashAndCashEquivalentForPeriod { get; set; }
        public IndirectCashFlowReportOutput CashAndCashEquivalentAtTheBeginningOfPeriod { get; set; }
        public IndirectCashFlowReportOutput CashAndCashEquivalentAtTheEndOfPeriod { get; set; }

        public Dictionary<long, string> ColumnHeaders { get; set; }
        public List<long> CashBankAccountTypes { get; set; }
    }

    public class IndirectCashFlowReportOutput
    {
        public string Account { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public long AccountTypeId { get; set; }
        public TypeOfAccount AccountType { get; set; }
        public string AccountTypeName { get; set; }

        public SubAccountType? SubAccountType { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public decimal Total { get; set; }

        public Dictionary<long, IndirectCashFlowReportLocationColumn> LocationColumnDic { get; set; }

    }

    public class IndirectCashFlowReportLocationColumn
    {
        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public decimal Total { get; set; }

    }

}
