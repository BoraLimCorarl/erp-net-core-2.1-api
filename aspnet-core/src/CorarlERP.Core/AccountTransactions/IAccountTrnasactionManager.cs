using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CorarlERP.AccountTransactions
{
    public  interface IAccountTransactionManager : IDomainService
    {
        IQueryable<AccountTransaction> GetAccountQuery(DateTime? fromDate, DateTime toDate, bool mutlCurrency = false, List<long> locations = null, bool groupByLocation = false);

        IQueryable<AccountTransaction> GetAccountMonthlyQuery(DateTime fromDate, DateTime toDate, List<long> locations = null);
        IQueryable<AccountTransaction> GetAccountBalanceSheetMonthlyQuery(DateTime fromDate, DateTime toDate, List<long> locations = null);
        IQueryable<AccountTransactionOutput> GetAccountLedgerQuery(DateTime fromDate, DateTime toDate, List<long> locations = null);
        IQueryable<CashAccountTransaction> GetCashAccountBeginningQueryHelper(DateTime toDate, List<long> locations = null);
        IQueryable<CashAccountTransaction> GetCashAccountByPeriodQueryHelper(DateTime fromDate, DateTime toDate, List<long> locations = null);
    }
}
