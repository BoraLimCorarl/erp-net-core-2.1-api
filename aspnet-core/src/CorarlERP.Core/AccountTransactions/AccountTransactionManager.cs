using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using CorarlERP.AccountCycles;
using CorarlERP.AccountTrasactionCloses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Journals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.Authorization.Users;
using System.Collections.Generic;
using CorarlERP.PayBills;
using CorarlERP.Bills;
using CorarlERP.ReceivePayments;
using CorarlERP.Invoices;
using Abp.UI.Inputs;
using Abp.Dependency;
using CorarlERP.MultiCurrencies;
using Abp.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CorarlERP.CustomerCredits;
using CorarlERP.VendorCredit;
using Abp.Extensions;
using CorarlERP.AccountTypes;

namespace CorarlERP.AccountTransactions
{
    public class AccountTransactionManager : CorarlERPDomainServiceBase, IAccountTransactionManager
    {
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<PayBillDetail, Guid> _payBillItemRepository;
        private readonly IRepository<PayBillExpense, Guid> _payBillExpenseRepository;
        private readonly IRepository<ReceivePaymentDetail, Guid> _receivePaymentItemRepository;
        private readonly IRepository<ReceivePaymentExpense, Guid> _receivePaymentExpenseRepository;
        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IRepository<BillItem, Guid> _billItemRepository;
        private readonly IRepository<VendorCreditDetail, Guid> _vendorCreditItemRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly IRepository<CustomerCreditDetail, Guid> _customerCreditItemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<AccountTransactionClose, Guid> _accountTransactionRepository;


        public AccountTransactionManager(
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<AccountTransactionClose, Guid> accountTransactionRepository,
            IRepository<PayBillDetail, Guid> payBillItemRepository,
            IRepository<Bill, Guid> billRepository,
            IRepository<ReceivePaymentDetail, Guid> receivePaymentItemRepository,
            IRepository<Invoice, Guid> invoiceRepository
        ) : base(accountCycleRepository)
        {
            _invoiceRepository = invoiceRepository;
            _receivePaymentItemRepository = receivePaymentItemRepository;
            _billRepository = billRepository;
            _journalRepository = journalRepository;
            _payBillItemRepository = payBillItemRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _journalItemRepository = journalItemRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _billItemRepository = IocManager.Instance.Resolve<IRepository<BillItem, Guid>>();
            _vendorCreditItemRepository = IocManager.Instance.Resolve<IRepository<VendorCreditDetail, Guid>>();
            _invoiceItemRepository = IocManager.Instance.Resolve<IRepository<InvoiceItem, Guid>>();
            _customerCreditItemRepository = IocManager.Instance.Resolve<IRepository<CustomerCreditDetail, Guid>>();
            _payBillExpenseRepository = IocManager.Instance.Resolve<IRepository<PayBillExpense, Guid>>();
            _receivePaymentExpenseRepository = IocManager.Instance.Resolve<IRepository<ReceivePaymentExpense, Guid>>();
        }

        public IQueryable<AccountTransaction> GetAccountMonthlyQuery(DateTime fromDate, DateTime toDate, List<long> locations = null)
        {
            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            var isSameYear = fromDate.Year == toDate.Year;
            var result = from j in _journalItemRepository
                                    .GetAll()
                                    .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                    .WhereIf(
                                    fromDate != null && toDate != null, (
                                        u => (u.Journal.Date.Date) >= (fromDate.Date)
                                            &&
                                        (u.Journal.Date.Date) <= (toDate.Date)
                                    ))
                                    .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                    .AsNoTracking()
                         group j by new { j.AccountId, j.Account.AccountName, j.Account.AccountCode, j.Account.AccountTypeId, j.Account.AccountType.Type, Month = Convert.ToInt32(j.Journal.Date.ToString("yyyyMM")), MonthName = isSameYear ? j.Journal.Date.ToString("MMM") : j.Journal.Date.ToString("MMM-yyyy") }
                             into u
                         let totalDr = u.Sum(s => Math.Round(s.Debit, roundDigit))
                         let totalCr = u.Sum(s => Math.Round(s.Credit, roundDigit))
                         let balance = totalDr - totalCr
                         where u.Any()
                         select new AccountTransaction
                         {
                             AccountId = u.Key.AccountId,
                             AccountName = u.Key.AccountName,
                             AccountCode = u.Key.AccountCode,
                             AccountTypeId = u.Key.AccountTypeId,
                             Type = u.Key.Type,
                             Debit = totalDr,
                             Credit = totalCr,
                             Balance = balance,
                             LocationId = u.Key.Month,
                             LocationName = u.Key.MonthName,
                         };

            return result;

        }

        public IQueryable<AccountTransaction> GetAccountBalanceSheetMonthlyQuery(DateTime fromDate, DateTime toDate, List<long> locations = null)
        {

            var isSameYear = fromDate.Year == toDate.Year;

            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            var result = from j in _journalItemRepository
                                    .GetAll()
                                    .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                    .WhereIf(
                                    fromDate != null && toDate != null, (
                                        u => (u.Journal.Date.Date) >= (fromDate.Date)
                                            &&
                                        (u.Journal.Date.Date) <= (toDate.Date)
                                    ))
                                    .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                    .AsNoTracking()
                         select new AccountTransaction
                         {
                             AccountId = j.AccountId,
                             AccountName = j.Account.AccountName,
                             AccountCode = j.Account.AccountCode,
                             AccountTypeId = j.Account.AccountTypeId,
                             Type = j.Account.AccountType.Type,
                             Debit = Math.Round(j.Debit, roundDigit),
                             Credit = Math.Round(j.Credit, roundDigit),
                             Balance = Math.Round(j.Debit, roundDigit) - Math.Round(j.Credit, roundDigit),
                             LocationId = Convert.ToInt32(j.Journal.Date.ToString("yyyyMM")),
                             LocationName = isSameYear ? j.Journal.Date.ToString("MMM") : j.Journal.Date.ToString("MMM-yyyy"),
                         };

            return result;


        }

        public IQueryable<AccountTransaction> GetAccountQuery(DateTime? fromDate, DateTime toDate, bool mutlCurrency = false, List<long> locations = null, bool groupByLocation = false)
        {

            return mutlCurrency ? GetMultiCurrencyAccountQueryHelper(fromDate, toDate, locations, groupByLocation) : GetAccountQueryHelper(fromDate, toDate, locations, groupByLocation);

        }

        private IQueryable<AccountTransaction> GetAccountQueryHelper(DateTime? fromDate, DateTime toDate, List<long> locations = null, bool groupByLocation = false)
        {
            //var previousCycle = GetPreviousCloseCyleAsync(toDate);
            //var currentCycle = GetCyleAsync(toDate);
            //var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            //if (fromDate != null)
            //{

            //    var accountFromDate = _journalItemRepository
            //                         .GetAll()
            //                         .Where(u => u.Journal.Status == TransactionStatus.Publish)
            //                         //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
            //                         .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
            //                         .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
            //                         .AsNoTracking()
            //                         .Select(u => new
            //                         {
            //                             AccountId = u.AccountId,
            //                             AccountName = u.Account.AccountName,
            //                             AccountCode = u.Account.AccountCode,
            //                             AccountTypeId = u.Account.AccountTypeId,
            //                             Type = u.Account.AccountType.Type,
            //                             Debit = Math.Round(u.Debit, roundDigit),
            //                             Credit = Math.Round(u.Credit, roundDigit),
            //                             Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
            //                             LocationId = u.Journal.LocationId,
            //                         })
            //                        .GroupBy(u => new { u.AccountId, u.AccountName, u.AccountCode, u.AccountTypeId, u.Type, LocationId = groupByLocation ? u.LocationId.Value : 0 })
            //                        .Select(u => new AccountTransaction
            //                        {
            //                            LocationId = u.Key.LocationId,
            //                            AccountId = u.Key.AccountId,
            //                            AccountName = u.Key.AccountName,
            //                            AccountCode = u.Key.AccountCode,
            //                            AccountTypeId = u.Key.AccountTypeId,
            //                            Type = u.Key.Type,
            //                            Debit = u.Sum(t => t.Debit),
            //                            Credit = u.Sum(t => t.Credit),
            //                            Balance = u.Sum(t => t.Debit) - u.Sum(t => t.Credit)
            //                        });

            //    var result = accountFromDate;
            //    return result;

            //}
            //else
            //{
            //    var fromDateBeginning = previousCycle == null ? DateTime.MinValue : previousCycle.EndDate.Value.Date;
            //    var account = _journalItemRepository
            //                        .GetAll()
            //                        .Where(u => u.Journal.Status == TransactionStatus.Publish)
            //                        //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
            //                        .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly > fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
            //                        .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
            //                        .AsNoTracking()
            //                        .Select(u => new AccountTransaction
            //                        {
            //                            LocationId = u.Journal.LocationId.Value,
            //                            AccountId = u.AccountId,
            //                            AccountName = u.Account.AccountName,
            //                            AccountCode = u.Account.AccountCode,
            //                            AccountTypeId = u.Account.AccountTypeId,
            //                            Type = u.Account.AccountType.Type,
            //                            Debit = Math.Round(u.Debit, roundDigit),
            //                            Credit = Math.Round(u.Credit, roundDigit),
            //                            Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
            //                        });

            //    var concatQuery = account;

            //    if (previousCycle != null)
            //    {
            //        var accountCostClose = _accountTransactionRepository.GetAll()
            //                       .Where(t => t.AccountCycleId == previousCycle.Id)
            //                       .WhereIf(locations != null && locations.Count() > 0, s => locations.Contains(s.LocationId.Value))
            //                       .AsNoTracking()
            //                       .Select(u => new AccountTransaction
            //                       {
            //                           LocationId = u.LocationId.Value,
            //                           AccountId = u.AccountId,
            //                           AccountName = u.Account.AccountName,
            //                           AccountCode = u.Account.AccountCode,
            //                           AccountTypeId = u.Account.AccountTypeId,
            //                           Type = u.Account.AccountType.Type,
            //                           Debit = Math.Round(u.Debit, roundDigit),
            //                           Credit = Math.Round(u.Credit, roundDigit),
            //                           Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
            //                       });
            //        concatQuery = accountCostClose.Concat(concatQuery);
            //    }

            //    var result = concatQuery
            //                    .GroupBy(u => new { u.AccountId, LocationId = groupByLocation ? u.LocationId : 0L })
            //                    .Select(u => new AccountTransaction
            //                    {
            //                        LocationId = u.Key.LocationId,
            //                        AccountId = u.Key.AccountId,
            //                        AccountName = u.Min(t => t.AccountName),
            //                        AccountCode = u.Min(t => t.AccountCode),
            //                        AccountTypeId = u.Min(t => t.AccountTypeId),
            //                        Type = u.Min(t => t.Type),
            //                        Debit = u.Sum(t => t.Debit),
            //                        Credit = u.Sum(t => t.Credit),
            //                        Balance = u.Sum(t => t.Debit - t.Credit)
            //                    });

            //    return result;

            //}

            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            var journalQuery = _journalItemRepository
                .GetAll()
                .Where(u => u.Journal.Status == TransactionStatus.Publish)
                .WhereIf(locations != null && locations.Count > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value));

            bool hasStartFilter = false;
            DateTime startDate = default;
            bool isGreaterThanStart = false;

            if (fromDate != null)
            {
                startDate = fromDate.Value.Date;
                isGreaterThanStart = false;
                hasStartFilter = true;
            }
            else if (previousCycle != null)
            {
                startDate = previousCycle.EndDate.Value.Date;
                isGreaterThanStart = true;
                hasStartFilter = true;
            }

            journalQuery = journalQuery.Where(u => u.Journal.DateOnly <= toDate.Date);

            if (hasStartFilter)
            {
                if (isGreaterThanStart)
                {
                    journalQuery = journalQuery.Where(u => u.Journal.DateOnly > startDate);
                }
                else
                {
                    journalQuery = journalQuery.Where(u => u.Journal.DateOnly >= startDate);
                }
            }

            var currentProjection = journalQuery
                .AsNoTracking()
                .Select(u => new AccountTransaction
                {
                    AccountId = u.AccountId,
                    AccountName = u.Account.AccountName,
                    AccountCode = u.Account.AccountCode,
                    AccountTypeId = u.Account.AccountTypeId,
                    Type = u.Account.AccountType.Type,
                    Debit = Math.Round(u.Debit, roundDigit),
                    Credit = Math.Round(u.Credit, roundDigit),
                    LocationId = u.Journal.LocationId ?? 0L,
                });

            IQueryable<AccountTransaction> combinedProjection = currentProjection;

            if (fromDate == null && previousCycle != null)
            {
                var closingProjection = _accountTransactionRepository
                    .GetAll()
                    .Where(t => t.AccountCycleId == previousCycle.Id)
                    .WhereIf(locations != null && locations.Count > 0, s => locations.Contains(s.LocationId.Value))
                    .AsNoTracking()
                    .Select(u => new AccountTransaction
                    {
                        AccountId = u.AccountId,
                        AccountName = u.Account.AccountName,
                        AccountCode = u.Account.AccountCode,
                        AccountTypeId = u.Account.AccountTypeId,
                        Type = u.Account.AccountType.Type,
                        Debit = Math.Round(u.Debit, roundDigit),
                        Credit = Math.Round(u.Credit, roundDigit),
                        LocationId = u.LocationId ?? 0L,
                    });

                combinedProjection = closingProjection.Concat(currentProjection);
            }

            var result = combinedProjection
                .OrderBy(s => s.AccountCode)
                .GroupBy(u => new { u.AccountId, u.AccountCode, u.AccountTypeId, u.AccountName, u.Type, LocKey = groupByLocation ? u.LocationId : 0L })
                .Select(u => new AccountTransaction
                {
                    LocationId = u.Key.LocKey,
                    AccountId = u.Key.AccountId,
                    AccountName = u.Key.AccountName,
                    AccountCode = u.Key.AccountCode,
                    AccountTypeId = u.Key.AccountTypeId,
                    Type = u.Key.Type,
                    Debit = u.Sum(t => t.Debit),
                    Credit = u.Sum(t => t.Credit),
                    Balance = u.Sum(t => t.Debit) - u.Sum(t => t.Credit)
                });

            return result;
        }
        private IQueryable<AccountTransaction> GetMultiCurrencyAccountQueryHelper(DateTime? fromDate, DateTime toDate, List<long> locations = null, bool groupByLocation = false)
        {
            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            if (fromDate != null)
            {

                var multiCurrencyExceptKeys = new List<PostingKey> { PostingKey.InventoryAdjustmentInv, PostingKey.InventoryAdjustmentAdj };

                var invoiceItemQuery = from ji in _journalItemRepository.GetAll()
                                                   .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Invoice)
                                                   //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
                                                   .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
                                                   .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                   .AsNoTracking()
                                       join invi in _invoiceItemRepository.GetAll()
                                                    .AsNoTracking()
                                       on ji.Identifier equals invi.Id
                                       into invItems
                                       from invi in invItems.DefaultIfEmpty()
                                       let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                       let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                                 invi != null ? (ji.Journal.MultiCurrencyId != null ? invi.MultiCurrencyTotal : invi.Total) :
                                                                 (ji.Journal.MultiCurrencyId != null ? ji.Journal.Invoice.MultiCurrencyTotal : ji.Journal.Invoice.Total)
                                       let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                       let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                       select new AccountTransaction
                                       {
                                           AccountId = ji.AccountId,
                                           AccountName = ji.Account.AccountName,
                                           AccountCode = ji.Account.AccountCode,
                                           AccountTypeId = ji.Account.AccountTypeId,
                                           Type = ji.Account.AccountType.Type,
                                           Debit = Math.Round(ji.Debit, roundDigit),
                                           Credit = Math.Round(ji.Credit, roundDigit),
                                           Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                           LocationId = ji.Journal.LocationId.Value,
                                           CurrencyId = currencyId,
                                           MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                           MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                           MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       };

                var customerCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                      .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.CustomerCredit)
                                                      //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
                                                      .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
                                                      .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                      .AsNoTracking()
                                              join cci in _customerCreditItemRepository.GetAll()
                                                        .AsNoTracking()
                                              on ji.Identifier equals cci.Id
                                              into ccItems
                                              from cci in ccItems.DefaultIfEmpty()
                                              let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                              let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                                        cci != null ? (ji.Journal.MultiCurrencyId != null ? cci.MultiCurrencyTotal : cci.Total) :
                                                                        (ji.Journal.MultiCurrencyId != null ? ji.Journal.CustomerCredit.MultiCurrencyTotal : ji.Journal.CustomerCredit.Total)
                                              let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                              let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                              select new AccountTransaction
                                              {
                                                  AccountId = ji.AccountId,
                                                  AccountName = ji.Account.AccountName,
                                                  AccountCode = ji.Account.AccountCode,
                                                  AccountTypeId = ji.Account.AccountTypeId,
                                                  Type = ji.Account.AccountType.Type,
                                                  Debit = Math.Round(ji.Debit, roundDigit),
                                                  Credit = Math.Round(ji.Credit, roundDigit),
                                                  Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                  LocationId = ji.Journal.LocationId.Value,
                                                  CurrencyId = currencyId,
                                                  MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                  MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                  MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              };

                var billItemQuery = from ji in _journalItemRepository.GetAll()
                                           .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Bill)
                                           //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
                                           .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
                                           .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                           .AsNoTracking()
                                    join bi in _billItemRepository.GetAll()
                                            .AsNoTracking()
                                    on ji.Identifier equals bi.Id
                                    into billItems
                                    from bi in billItems.DefaultIfEmpty()
                                    let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                    let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                              bi != null ? (ji.Journal.MultiCurrencyId != null ? bi.MultiCurrencyTotal : bi.Total) :
                                                              (ji.Journal.MultiCurrencyId != null ? ji.Journal.Bill.MultiCurrencyTotal : ji.Journal.Bill.Total)
                                    let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                    let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                    select new AccountTransaction
                                    {
                                        AccountId = ji.AccountId,
                                        AccountName = ji.Account.AccountName,
                                        AccountCode = ji.Account.AccountCode,
                                        AccountTypeId = ji.Account.AccountTypeId,
                                        Type = ji.Account.AccountType.Type,
                                        Debit = Math.Round(ji.Debit, roundDigit),
                                        Credit = Math.Round(ji.Credit, roundDigit),
                                        Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                        LocationId = ji.Journal.LocationId.Value,
                                        CurrencyId = currencyId,
                                        MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                        MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                        MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                    };

                var vendorCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                      .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.VendorCredit)
                                                      //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
                                                      .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
                                                      .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                      .AsNoTracking()
                                            join vci in _billItemRepository.GetAll()
                                                        .AsNoTracking()
                                            on ji.Identifier equals vci.Id
                                            into vcItems
                                            from vci in vcItems.DefaultIfEmpty()
                                            let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                            let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                                      vci != null ? (ji.Journal.MultiCurrencyId != null ? vci.MultiCurrencyTotal : vci.Total) :
                                                                      (ji.Journal.MultiCurrencyId != null ? ji.Journal.VendorCredit.MultiCurrencyTotal : ji.Journal.VendorCredit.Total)
                                            let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                            let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                            select new AccountTransaction
                                            {
                                                AccountId = ji.AccountId,
                                                AccountName = ji.Account.AccountName,
                                                AccountCode = ji.Account.AccountCode,
                                                AccountTypeId = ji.Account.AccountTypeId,
                                                Type = ji.Account.AccountType.Type,
                                                Debit = Math.Round(ji.Debit, roundDigit),
                                                Credit = Math.Round(ji.Credit, roundDigit),
                                                Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                LocationId = ji.Journal.LocationId.Value,
                                                CurrencyId = currencyId,
                                                MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                            };

                var payBillItemQuery = from ji in _journalItemRepository.GetAll()
                                                  .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.PayBill)
                                                  //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
                                                  .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
                                                  .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                  .AsNoTracking()
                                       join pbi in _payBillItemRepository.GetAll()
                                                   .AsNoTracking()
                                       on ji.Identifier equals pbi.Id
                                       into pbillItems
                                       from pbi in pbillItems.DefaultIfEmpty()

                                       join pbex in _payBillExpenseRepository.GetAll()
                                                    .AsNoTracking()
                                       on ji.Identifier equals pbex.Id
                                       into pbillExpenses
                                       from pbex in pbillExpenses.DefaultIfEmpty()

                                       let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                       let multiCurrencyAmount = pbi != null ? (ji.Journal.MultiCurrencyId != null ? pbi.MultiCurrencyPayment : pbi.Payment) :
                                                                 pbex != null ? (ji.Journal.MultiCurrencyId != null ? pbex.MultiCurrencyAmount : pbex.Amount) :
                                                                 (ji.Journal.MultiCurrencyId != null ? ji.Journal.PayBill.MultiCurrencyTotalPayment : ji.Journal.PayBill.TotalPayment)
                                       let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                       let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                       select new AccountTransaction
                                       {
                                           AccountId = ji.AccountId,
                                           AccountName = ji.Account.AccountName,
                                           AccountCode = ji.Account.AccountCode,
                                           AccountTypeId = ji.Account.AccountTypeId,
                                           Type = ji.Account.AccountType.Type,
                                           Debit = Math.Round(ji.Debit, roundDigit),
                                           Credit = Math.Round(ji.Credit, roundDigit),
                                           Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                           LocationId = ji.Journal.LocationId.Value,
                                           CurrencyId = currencyId,
                                           MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                           MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                           MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       };

                var receivePaymentItemQuery = from ji in _journalItemRepository.GetAll()
                                                          .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.ReceivePayment)
                                                          //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Value.Date && u.Journal.Date.Date <= toDate.Date)
                                                          .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Value.Date && u.Journal.DateOnly <= toDate.Date)
                                                          .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                          .AsNoTracking()
                                              join rpi in _receivePaymentItemRepository.GetAll()
                                                          .AsNoTracking()
                                              on ji.Identifier equals rpi.Id
                                              into rpItems
                                              from rpi in rpItems.DefaultIfEmpty()

                                              join rpex in _receivePaymentExpenseRepository.GetAll()
                                                           .AsNoTracking()
                                              on ji.Identifier equals rpex.Id
                                              into rpExpenses
                                              from rpex in rpExpenses.DefaultIfEmpty()

                                              let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                              let multiCurrencyAmount = rpi != null ? (ji.Journal.MultiCurrencyId != null ? rpi.MultiCurrencyPayment : rpi.Payment) :
                                                                        rpex != null ? (ji.Journal.MultiCurrencyId != null ? rpex.MultiCurrencyAmount : rpex.Amount) :
                                                                        (ji.Journal.MultiCurrencyId != null ? ji.Journal.ReceivePayment.MultiCurrencyTotalPayment : ji.Journal.ReceivePayment.TotalPayment)
                                              let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                              let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                              select new AccountTransaction
                                              {
                                                  AccountId = ji.AccountId,
                                                  AccountName = ji.Account.AccountName,
                                                  AccountCode = ji.Account.AccountCode,
                                                  AccountTypeId = ji.Account.AccountTypeId,
                                                  Type = ji.Account.AccountType.Type,
                                                  Debit = Math.Round(ji.Debit, roundDigit),
                                                  Credit = Math.Round(ji.Credit, roundDigit),
                                                  Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                  LocationId = ji.Journal.LocationId.Value,
                                                  CurrencyId = currencyId,
                                                  MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                  MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                  MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              };

                var exceptJournalTypes = new List<JournalType>
                                         {
                                            JournalType.Bill,
                                            JournalType.VendorCredit,
                                            JournalType.PayBill,
                                            JournalType.Invoice,
                                            JournalType.CustomerCredit,
                                            JournalType.ReceivePayment
                                        };

                var otherJournalItemQuery = _journalItemRepository
                                           .GetAll()
                                           .Where(u => u.Journal.Status == TransactionStatus.Publish && !exceptJournalTypes.Contains(u.Journal.JournalType))
                                           //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date > fromDate && u.Journal.Date.Date <= toDate.Date)
                                           .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly > fromDate && u.Journal.DateOnly <= toDate.Date)
                                           .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                           .AsNoTracking()
                                           .Select(u => new AccountTransaction
                                           {
                                               LocationId = u.Journal.LocationId.Value,
                                               AccountId = u.AccountId,
                                               AccountName = u.Account.AccountName,
                                               AccountCode = u.Account.AccountCode,
                                               AccountTypeId = u.Account.AccountTypeId,
                                               Type = u.Account.AccountType.Type,
                                               Debit = Math.Round(u.Debit, roundDigit),
                                               Credit = Math.Round(u.Credit, roundDigit),
                                               Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                               CurrencyId = u.Journal.CurrencyId,
                                               MultiCurrencyDebit = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Debit, roundDigit),
                                               MultiCurrencyCredit = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Credit, roundDigit),
                                               MultiCurrencyBalance = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                           });

                var allQuery = invoiceItemQuery.Concat(customerCreditItemQuery).Concat(receivePaymentItemQuery)
                               .Concat(billItemQuery).Concat(vendorCreditItemQuery).Concat(payBillItemQuery)
                               .Concat(otherJournalItemQuery)
                               .OrderBy(s => s.AccountCode)
                                .GroupBy(u => new { u.AccountId, u.AccountCode, u.AccountName, u.AccountTypeId, u.Type, u.CurrencyId, LocationId = groupByLocation ? u.LocationId : 0L })
                                .Select(u => new AccountTransaction
                                {
                                    LocationId = u.Key.LocationId,
                                    AccountId = u.Key.AccountId,
                                    AccountName = u.Key.AccountName,
                                    AccountCode = u.Key.AccountCode,
                                    AccountTypeId = u.Key.AccountTypeId,
                                    Type = u.Key.Type,
                                    Debit = u.Sum(t => t.Debit),
                                    Credit = u.Sum(t => t.Credit),
                                    Balance = u.Sum(t => t.Debit - t.Credit),
                                    CurrencyId = u.Key.CurrencyId,
                                    MultiCurrencyDebit = u.Sum(s => s.MultiCurrencyDebit),
                                    MultiCurrencyCredit = u.Sum(s => s.MultiCurrencyCredit),
                                    MultiCurrencyBalance = u.Sum(s => s.MultiCurrencyBalance),
                                });

                return allQuery;

            }
            else
            {
                var fromDateBeginning = previousCycle == null ? DateTime.MinValue : previousCycle.EndDate.Value.Date;

                var multiCurrencyExceptKeys = new List<PostingKey> { PostingKey.InventoryAdjustmentInv, PostingKey.InventoryAdjustmentAdj };

                var invoiceItemQuery = from ji in _journalItemRepository.GetAll()
                                                   .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Invoice)
                                                   //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date >= fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                   .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly >= fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                                   .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                   .AsNoTracking()
                                       join invi in _invoiceItemRepository.GetAll()
                                                    .AsNoTracking()
                                       on ji.Identifier equals invi.Id
                                       into invItems
                                       from invi in invItems.DefaultIfEmpty()
                                       let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                       let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                                 invi != null ? (ji.Journal.MultiCurrencyId != null ? invi.MultiCurrencyTotal : invi.Total) :
                                                                 (ji.Journal.MultiCurrencyId != null ? ji.Journal.Invoice.MultiCurrencyTotal : ji.Journal.Invoice.Total)
                                       let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                       let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                       select new AccountTransaction
                                       {
                                           AccountId = ji.AccountId,
                                           AccountName = ji.Account.AccountName,
                                           AccountCode = ji.Account.AccountCode,
                                           AccountTypeId = ji.Account.AccountTypeId,
                                           Type = ji.Account.AccountType.Type,
                                           Debit = Math.Round(ji.Debit, roundDigit),
                                           Credit = Math.Round(ji.Credit, roundDigit),
                                           Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                           LocationId = ji.Journal.LocationId.Value,
                                           CurrencyId = currencyId,
                                           MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                           MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                           MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       };

                var customerCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                          .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.CustomerCredit)
                                                          //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date >= fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                          .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly >= fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                                          .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                          .AsNoTracking()
                                              join cci in _customerCreditItemRepository.GetAll()
                                                          .AsNoTracking()
                                              on ji.Identifier equals cci.Id
                                              into ccItems
                                              from cci in ccItems.DefaultIfEmpty()
                                              let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                              let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                                        cci != null ? (ji.Journal.MultiCurrencyId != null ? cci.MultiCurrencyTotal : cci.Total) :
                                                                        (ji.Journal.MultiCurrencyId != null ? ji.Journal.CustomerCredit.MultiCurrencyTotal : ji.Journal.CustomerCredit.Total)
                                              let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                              let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                              select new AccountTransaction
                                              {
                                                  AccountId = ji.AccountId,
                                                  AccountName = ji.Account.AccountName,
                                                  AccountCode = ji.Account.AccountCode,
                                                  AccountTypeId = ji.Account.AccountTypeId,
                                                  Type = ji.Account.AccountType.Type,
                                                  Debit = Math.Round(ji.Debit, roundDigit),
                                                  Credit = Math.Round(ji.Credit, roundDigit),
                                                  Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                  LocationId = ji.Journal.LocationId.Value,
                                                  CurrencyId = currencyId,
                                                  MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                  MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                  MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              };

                var billItemQuery = from ji in _journalItemRepository.GetAll()
                                               .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Bill)
                                               //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date >= fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                               .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly >= fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                               .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                                    join bi in _billItemRepository.GetAll()
                                               .AsNoTracking()
                                    on ji.Identifier equals bi.Id
                                    into billItems
                                    from bi in billItems.DefaultIfEmpty()
                                    let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                    let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                              bi != null ? (ji.Journal.MultiCurrencyId != null ? bi.MultiCurrencyTotal : bi.Total) :
                                                              (ji.Journal.MultiCurrencyId != null ? ji.Journal.Bill.MultiCurrencyTotal : ji.Journal.Bill.Total)
                                    let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                    let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                    select new AccountTransaction
                                    {
                                        AccountId = ji.AccountId,
                                        AccountName = ji.Account.AccountName,
                                        AccountCode = ji.Account.AccountCode,
                                        AccountTypeId = ji.Account.AccountTypeId,
                                        Type = ji.Account.AccountType.Type,
                                        Debit = Math.Round(ji.Debit, roundDigit),
                                        Credit = Math.Round(ji.Credit, roundDigit),
                                        Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                        LocationId = ji.Journal.LocationId.Value,
                                        CurrencyId = currencyId,
                                        MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                        MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                        MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                    };

                var vendorCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                      .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.VendorCredit)
                                                      //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date >= fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                      .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly >= fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                                      .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                      .AsNoTracking()
                                            join vci in _billItemRepository.GetAll()
                                                        .AsNoTracking()
                                            on ji.Identifier equals vci.Id
                                            into vcItems
                                            from vci in vcItems.DefaultIfEmpty()
                                            let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                            let multiCurrencyAmount = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 :
                                                                      vci != null ? (ji.Journal.MultiCurrencyId != null ? vci.MultiCurrencyTotal : vci.Total) :
                                                                      (ji.Journal.MultiCurrencyId != null ? ji.Journal.VendorCredit.MultiCurrencyTotal : ji.Journal.VendorCredit.Total)
                                            let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                            let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                            select new AccountTransaction
                                            {
                                                AccountId = ji.AccountId,
                                                AccountName = ji.Account.AccountName,
                                                AccountCode = ji.Account.AccountCode,
                                                AccountTypeId = ji.Account.AccountTypeId,
                                                Type = ji.Account.AccountType.Type,
                                                Debit = Math.Round(ji.Debit, roundDigit),
                                                Credit = Math.Round(ji.Credit, roundDigit),
                                                Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                LocationId = ji.Journal.LocationId.Value,
                                                CurrencyId = currencyId,
                                                MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                            };

                var payBillItemQuery = from ji in _journalItemRepository.GetAll()
                                                  .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.PayBill)
                                                  //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date >= fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                  .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly >= fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                                  .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                  .AsNoTracking()
                                       join pbi in _payBillItemRepository.GetAll()
                                                   .AsNoTracking()
                                       on ji.Identifier equals pbi.Id
                                       into pbillItems
                                       from pbi in pbillItems.DefaultIfEmpty()

                                       join pbex in _payBillExpenseRepository.GetAll()
                                                    .AsNoTracking()
                                       on ji.Identifier equals pbex.Id
                                       into pbillExpenses
                                       from pbex in pbillExpenses.DefaultIfEmpty()

                                       let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                       let multiCurrencyAmount = pbi != null ? (ji.Journal.MultiCurrencyId != null ? pbi.MultiCurrencyPayment : pbi.Payment) :
                                                                 pbex != null ? (ji.Journal.MultiCurrencyId != null ? pbex.MultiCurrencyAmount : pbex.Amount) :
                                                                 (ji.Journal.MultiCurrencyId != null ? ji.Journal.PayBill.MultiCurrencyTotalPayment : ji.Journal.PayBill.TotalPayment)
                                       let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                       let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                       select new AccountTransaction
                                       {
                                           AccountId = ji.AccountId,
                                           AccountName = ji.Account.AccountName,
                                           AccountCode = ji.Account.AccountCode,
                                           AccountTypeId = ji.Account.AccountTypeId,
                                           Type = ji.Account.AccountType.Type,
                                           Debit = Math.Round(ji.Debit, roundDigit),
                                           Credit = Math.Round(ji.Credit, roundDigit),
                                           Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                           LocationId = ji.Journal.LocationId.Value,
                                           CurrencyId = currencyId,
                                           MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                           MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                           MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       };

                var receivePaymentItemQuery = from ji in _journalItemRepository.GetAll()
                                                          .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.ReceivePayment)
                                                          //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date >= fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                          .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly >= fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                                          .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                          .AsNoTracking()
                                              join rpi in _receivePaymentItemRepository.GetAll()
                                                          .AsNoTracking()
                                              on ji.Identifier equals rpi.Id
                                              into rpItems
                                              from rpi in rpItems.DefaultIfEmpty()

                                              join rpex in _receivePaymentExpenseRepository.GetAll()
                                                           .AsNoTracking()
                                              on ji.Identifier equals rpex.Id
                                              into rpExpenses
                                              from rpex in rpExpenses.DefaultIfEmpty()

                                              let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                              let multiCurrencyAmount = rpi != null ? (ji.Journal.MultiCurrencyId != null ? rpi.MultiCurrencyPayment : rpi.Payment) :
                                                                        rpex != null ? (ji.Journal.MultiCurrencyId != null ? rpex.MultiCurrencyAmount : rpex.Amount) :
                                                                        (ji.Journal.MultiCurrencyId != null ? ji.Journal.ReceivePayment.MultiCurrencyTotalPayment : ji.Journal.ReceivePayment.TotalPayment)
                                              let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                              let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                              select new AccountTransaction
                                              {
                                                  AccountId = ji.AccountId,
                                                  AccountName = ji.Account.AccountName,
                                                  AccountCode = ji.Account.AccountCode,
                                                  AccountTypeId = ji.Account.AccountTypeId,
                                                  Type = ji.Account.AccountType.Type,
                                                  Debit = Math.Round(ji.Debit, roundDigit),
                                                  Credit = Math.Round(ji.Credit, roundDigit),
                                                  Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                  LocationId = ji.Journal.LocationId.Value,
                                                  CurrencyId = currencyId,
                                                  MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                  MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                  MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              };

                var exceptJournalTypes = new List<JournalType>
                                         {
                                            JournalType.Bill,
                                            JournalType.VendorCredit,
                                            JournalType.PayBill,
                                            JournalType.Invoice,
                                            JournalType.CustomerCredit,
                                            JournalType.ReceivePayment
                                        };

                var otherJournalItemQuery = _journalItemRepository
                                           .GetAll()
                                           .Where(u => u.Journal.Status == TransactionStatus.Publish && !exceptJournalTypes.Contains(u.Journal.JournalType))
                                           //.WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                           .WhereIf(fromDateBeginning != null && toDate != null, u => u.Journal.DateOnly > fromDateBeginning && u.Journal.DateOnly <= toDate.Date)
                                           .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                           .AsNoTracking()
                                           .Select(u => new AccountTransaction
                                           {
                                               LocationId = u.Journal.LocationId.Value,
                                               AccountId = u.AccountId,
                                               AccountName = u.Account.AccountName,
                                               AccountCode = u.Account.AccountCode,
                                               AccountTypeId = u.Account.AccountTypeId,
                                               Type = u.Account.AccountType.Type,
                                               Debit = Math.Round(u.Debit, roundDigit),
                                               Credit = Math.Round(u.Credit, roundDigit),
                                               Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                               CurrencyId = u.Journal.CurrencyId,
                                               MultiCurrencyDebit = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Debit, roundDigit),
                                               MultiCurrencyCredit = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Credit, roundDigit),
                                               MultiCurrencyBalance = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                           });

                var concatQuery = invoiceItemQuery.Concat(customerCreditItemQuery).Concat(receivePaymentItemQuery)
                               .Concat(billItemQuery).Concat(vendorCreditItemQuery).Concat(payBillItemQuery)
                               .Concat(otherJournalItemQuery);

                if(previousCycle != null)
                {
                    var accountCloseQuery = _accountTransactionRepository.GetAll()
                                  .Where(t => t.AccountCycleId == previousCycle.Id)
                                  .WhereIf(locations != null && locations.Count() > 0, s => locations.Contains(s.LocationId.Value))
                                  .AsNoTracking()
                                  .Select(u => new AccountTransaction
                                  {
                                      LocationId = u.LocationId.Value,
                                      AccountId = u.AccountId,
                                      AccountName = u.Account.AccountName,
                                      AccountCode = u.Account.AccountCode,
                                      AccountTypeId = u.Account.AccountTypeId,
                                      Type = u.Account.AccountType.Type,
                                      Debit = Math.Round(u.Debit, roundDigit),
                                      Credit = Math.Round(u.Credit, roundDigit),
                                      Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                      CurrencyId = u.CurrencyId,
                                      MultiCurrencyDebit = Math.Round(u.MultiCurrencyDebit, roundDigit),
                                      MultiCurrencyCredit = Math.Round(u.MultiCurrencyCredit, roundDigit),
                                      MultiCurrencyBalance = Math.Round(u.MultiCurrencyBalance, roundDigit),
                                  });

                    concatQuery =accountCloseQuery.Concat(concatQuery);
                }

                var allQuery = concatQuery
                                .OrderBy(t => t.AccountCode)
                                .GroupBy(u => new { u.AccountId, u.AccountCode, u.AccountName, u.AccountTypeId, u.Type, u.CurrencyId, LocationId = groupByLocation ? u.LocationId : 0 })
                                .Select(u => new AccountTransaction
                                {
                                    LocationId = u.Key.LocationId,
                                    AccountId = u.Key.AccountId,
                                    AccountName = u.Key.AccountName,
                                    AccountCode = u.Key.AccountCode,
                                    AccountTypeId = u.Key.AccountTypeId,
                                    Type = u.Key.Type,
                                    Debit = u.Sum(t => t.Debit),
                                    Credit = u.Sum(t => t.Credit),
                                    Balance = u.Sum(t => t.Debit - t.Credit),
                                    CurrencyId = u.Key.CurrencyId,
                                    MultiCurrencyDebit = u.Sum(s => s.MultiCurrencyDebit),
                                    MultiCurrencyCredit = u.Sum(s => s.MultiCurrencyCredit),
                                    MultiCurrencyBalance = u.Sum(s => s.MultiCurrencyBalance),

                                });

                return allQuery;

            }


        }

        public IQueryable<AccountTransactionOutput> GetAccountLedgerQuery(DateTime fromDate, DateTime toDate, List<long> locations = null)
        {
            
            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            var paybilItemQuery = from p in _payBillItemRepository.GetAll()
                                            .AsNoTracking()
                                  join b in _billRepository.GetAll()
                                            .AsNoTracking()
                                  on p.BillId equals b.Id

                                  join j in _journalRepository.GetAll()
                                              .Where(s => s.Status == TransactionStatus.Publish)
                                              .Where(t => t.JournalType == JournalType.Bill)
                                              .AsNoTracking()
                                  on b.Id equals j.BillId

                                  select new { j.Reference, p.Id, Vendor = new { b.Vendor.Id, b.Vendor.VendorCode, b.Vendor.VendorName }, p.PayBillId };

            var receivePaymentQuery = from p in _receivePaymentItemRepository.GetAll()
                                                .AsNoTracking()
                                      join i in _invoiceRepository.GetAll()
                                                .AsNoTracking()
                                      on p.InvoiceId equals i.Id
                                      join j in _journalRepository.GetAll()
                                              .Where(s => s.Status == TransactionStatus.Publish)
                                              .Where(t => t.JournalType == JournalType.Invoice)
                                              .AsNoTracking()
                                      on i.Id equals j.InvoiceId
                                      select new { j.Reference, p.Id, Customer = new { i.Customer.Id, i.Customer.CustomerName, i.Customer.CustomerCode }, p.ReceivePaymentId };

            var accountFromDate = from j in _journalItemRepository
                                            .GetAll()
                                            .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                            //.WhereIf(fromDate != null && toDate != null, u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                            .WhereIf(fromDate != null && toDate != null, u => u.Journal.DateOnly >= fromDate.Date && u.Journal.DateOnly <= toDate.Date)
                                            .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                            .AsNoTracking()
                                  join o in paybilItemQuery on j.Identifier equals o.Id into otherRef
                                  from oj in otherRef.Where(u => j.Identifier != null && j.Journal.PayBillId != null &&
                                                           u.PayBillId == j.Journal.PayBillId.Value).DefaultIfEmpty()

                                  join v in receivePaymentQuery on j.Identifier equals v.Id into otherRefInvoice
                                  from vj in otherRefInvoice.Where(u => j.Identifier != null && j.Journal.ReceivePaymentId != null &&
                                                           u.ReceivePaymentId == j.Journal.ReceivePaymentId.Value).DefaultIfEmpty()

                                  select new AccountTransactionOutput
                                  {
                                      CreationTimeIndex = j.Journal.CreationTimeIndex,
                                      OtherReferenceBill = oj == null ? null : oj.Reference,
                                      PayBillVendor = oj == null ? null : new VendorDataQuery
                                      {
                                          Id = oj.Vendor.Id,
                                          VendorCode = oj.Vendor.VendorCode,
                                          VendorName = oj.Vendor.VendorName
                                      },
                                      OtherReferenceInvoice = vj == null ? null : vj.Reference,
                                      ReceivePaymentCustomer = vj == null ? null : new CustomerDataQuery
                                      {
                                          Id = vj.Customer.Id,
                                          CustomerCode = vj.Customer.CustomerCode,
                                          CustomerName = vj.Customer.CustomerName
                                      },
                                      Reference = j.Journal.Reference,
                                      AccountId = j.AccountId,
                                      AccountName = j.Account.AccountName,
                                      AccountCode = j.Account.AccountCode,
                                      AccountTypeId = j.Account.AccountTypeId,
                                      AccountType = j.Account.AccountType.Type,
                                      Debit = Math.Round(j.Debit, roundDigit),
                                      Credit = Math.Round(j.Credit, roundDigit),
                                      Balance = Math.Round(j.Debit, roundDigit) - Math.Round(j.Credit, roundDigit),
                                      JournalDate = j.Journal.Date,
                                      DateOnly = j.Journal.DateOnly,
                                      JournalNo = j.Journal.JournalNo,
                                      JournalType = j.Journal.JournalType,
                                      JournalStatus = j.Journal.Status,
                                      CreationTime = j.Journal.CreationTime,
                                      JournalId = j.Journal.Id,
                                      JournalMemo = j.Journal.Memo,
                                      Description = j.Description,
                                      CreatorUserId = j.Journal.CreatorUserId,
                                      CreatorUserName = j.Journal.CreatorUser == null ? "" : j.Journal.CreatorUser.UserName,

                                      BillVendor = j.Journal.Bill != null ? ObjectMapper.Map<VendorDataQuery>(j.Journal.Bill.Vendor) : null,

                                      ItemReceiptVendor = j.Journal.ItemReceipt != null && j.Journal.ItemReceipt.Vendor != null ?
                                                                              ObjectMapper.Map<VendorDataQuery>(j.Journal.ItemReceipt.Vendor) : null,

                                      ItemIssueVendor = j.Journal.ItemIssueVendorCredit != null && j.Journal.ItemIssueVendorCredit.Vendor != null ?
                                                                      ObjectMapper.Map<VendorDataQuery>(j.Journal.ItemIssueVendorCredit.Vendor) : null,

                                      CustomerCreditCustomer = j.Journal.CustomerCredit != null && j.Journal.CustomerCredit.Customer != null ?
                                                                  ObjectMapper.Map<CustomerDataQuery>(j.Journal.CustomerCredit.Customer) : null,

                                      ItemIssueCustomer = j.Journal.ItemIssue != null && j.Journal.ItemIssue.Customer != null ?
                                                                      ObjectMapper.Map<CustomerDataQuery>(j.Journal.ItemIssue.Customer) : null,

                                      InviceCustomer = j.Journal.Invoice != null ? ObjectMapper.Map<CustomerDataQuery>(j.Journal.Invoice.Customer) : null,

                                      DepositVendor = j.Journal.Deposit != null && j.Journal.Deposit.Vendor != null ?
                                                              ObjectMapper.Map<VendorDataQuery>(j.Journal.Deposit.Vendor)
                                                              : j.Journal.Deposit != null && j.Journal.Deposit.Customer != null ?
                                                                  new VendorDataQuery
                                                                  {
                                                                      Id = j.Journal.Deposit.Customer.Id,
                                                                      VendorCode = j.Journal.Deposit.Customer.CustomerCode,
                                                                      VendorName = j.Journal.Deposit.Customer.CustomerName
                                                                  }
                                                                  : null,

                                      VendorCreditVendor = j.Journal.VendorCredit != null && j.Journal.VendorCredit.Vendor != null ?
                                                                  ObjectMapper.Map<VendorDataQuery>(j.Journal.VendorCredit.Vendor) : null,

                                      WithdrawlVendor = j.Journal.withdraw != null && j.Journal.withdraw.Vendor != null ?
                                                                  ObjectMapper.Map<VendorDataQuery>(j.Journal.withdraw.Vendor)
                                                                  : j.Journal.withdraw != null && j.Journal.withdraw.Customer != null ?
                                                                 new VendorDataQuery
                                                                 {
                                                                     Id = j.Journal.withdraw.Customer.Id,
                                                                     VendorCode = j.Journal.withdraw.Customer.CustomerCode,
                                                                     VendorName = j.Journal.withdraw.Customer.CustomerName
                                                                 } : null,

                                      ItemReceiptCustomer = j.Journal.ItemReceiptCustomerCredit != null && j.Journal.ItemReceiptCustomerCredit.Customer != null ?
                                                                  ObjectMapper.Map<CustomerDataQuery>(j.Journal.ItemReceiptCustomerCredit.Customer) : null,


                                      TransactionBankId = j.Journal.WithdrawId != null ? j.Journal.WithdrawId : j.Journal.DepositId,
                                      TransactionCustomerId = j.Journal.ItemIssueId != null ? j.Journal.ItemIssueId :
                                                                          j.Journal.InvoiceId != null ? j.Journal.InvoiceId :
                                                                          j.Journal.CustomerCreditId != null ? j.Journal.CustomerCreditId :
                                                                          j.Journal.ItemIssueVendorCreditId != null ? j.Journal.ItemIssueVendorCreditId :
                                                                          j.Journal.ReceivePaymentId != null ? j.Journal.ReceivePaymentId : null,
                                      TransactionGeneralJournalId = j.Journal.GeneralJournalId,
                                      TransactionVendorId = j.Journal.ItemReceiptId != null ? j.Journal.ItemReceiptId :
                                                                          j.Journal.BillId != null ? j.Journal.BillId :
                                                                          j.Journal.VendorCreditId != null ? j.Journal.VendorCreditId :
                                                                          j.Journal.ItemReceiptCustomerCreditId != null ? j.Journal.ItemReceiptCustomerCreditId :
                                                                          j.Journal.PayBillId != null ? j.Journal.PayBillId : null,

                                  };

            var result = accountFromDate;
            return result;


        }

        public IQueryable<CashAccountTransaction> GetCashAccountBeginningQueryHelper(DateTime toDate, List<long> locations = null)
        {

            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            var fromDateBeginning = previousCycle == null ? DateTime.MinValue : previousCycle.EndDate.Value.Date;

            var payBillItemQuery = from ji in _journalItemRepository.GetAll()
                                         .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                         .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.PayBill)
                                         .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                         .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                         .AsNoTracking()
                                   join pbi in _payBillItemRepository.GetAll()
                                               .AsNoTracking()
                                   on ji.Identifier equals pbi.Id
                                   into pbillItems
                                   from pbi in pbillItems.DefaultIfEmpty()

                                   join pbex in _payBillExpenseRepository.GetAll()
                                                .AsNoTracking()
                                   on ji.Identifier equals pbex.Id
                                   into pbillExpenses
                                   from pbex in pbillExpenses.DefaultIfEmpty()

                                   let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId
                                   let currencyCode = ji.Journal.MultiCurrency == null ? ji.Journal.Currency.Code : ji.Journal.MultiCurrency.Code

                                   let multiCurrencyAmount = pbi != null ? (ji.Journal.MultiCurrencyId != null ? pbi.MultiCurrencyPayment : pbi.Payment) :
                                                             pbex != null ? (ji.Journal.MultiCurrencyId != null ? pbex.MultiCurrencyAmount : pbex.Amount) :
                                                             (ji.Journal.MultiCurrencyId != null ? ji.Journal.PayBill.MultiCurrencyTotalPayment : ji.Journal.PayBill.TotalPayment)
                                   let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                   let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                   select new AccountTransaction
                                   {
                                       AccountId = ji.AccountId,
                                       AccountName = ji.Account.AccountName,
                                       AccountCode = ji.Account.AccountCode,
                                       AccountTypeId = ji.Account.AccountTypeId,
                                       Type = ji.Account.AccountType.Type,
                                       Debit = Math.Round(ji.Debit, roundDigit),
                                       Credit = Math.Round(ji.Credit, roundDigit),
                                       Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                       LocationId = ji.Journal.LocationId.Value,
                                       CurrencyId = currencyId,
                                       MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                       MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                       MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       RoundDigits = roundDigit,
                                   };

            var receivePaymentItemQuery = from ji in _journalItemRepository.GetAll()
                                                      .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                      .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.ReceivePayment)
                                                      .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                      .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                      .AsNoTracking()
                                          join rpi in _receivePaymentItemRepository.GetAll()
                                                      .AsNoTracking()
                                          on ji.Identifier equals rpi.Id
                                          into rpItems
                                          from rpi in rpItems.DefaultIfEmpty()

                                          join rpex in _receivePaymentExpenseRepository.GetAll()
                                                       .AsNoTracking()
                                          on ji.Identifier equals rpex.Id
                                          into rpExpenses
                                          from rpex in rpExpenses.DefaultIfEmpty()

                                          let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId
                                          let currencyCode = ji.Journal.MultiCurrency == null ? ji.Journal.Currency.Code : ji.Journal.MultiCurrency.Code

                                          let multiCurrencyAmount = rpi != null ? (ji.Journal.MultiCurrencyId != null ? rpi.MultiCurrencyPayment : rpi.Payment) :
                                                                    rpex != null ? (ji.Journal.MultiCurrencyId != null ? rpex.MultiCurrencyAmount : rpex.Amount) :
                                                                    (ji.Journal.MultiCurrencyId != null ? ji.Journal.ReceivePayment.MultiCurrencyTotalPayment : ji.Journal.ReceivePayment.TotalPayment)
                                          let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                          let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                          select new AccountTransaction
                                          {
                                              AccountId = ji.AccountId,
                                              AccountName = ji.Account.AccountName,
                                              AccountCode = ji.Account.AccountCode,
                                              AccountTypeId = ji.Account.AccountTypeId,
                                              Type = ji.Account.AccountType.Type,
                                              Debit = Math.Round(ji.Debit, roundDigit),
                                              Credit = Math.Round(ji.Credit, roundDigit),
                                              Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                              LocationId = ji.Journal.LocationId.Value,
                                              CurrencyId = currencyId,
                                              MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                              MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                              MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              RoundDigits = roundDigit,
                                          };


            var invoiceItemQuery = from ji in _journalItemRepository.GetAll()
                                              .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Invoice)
                                              .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                              .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                              .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                              .AsNoTracking()
                                   join invi in _invoiceItemRepository.GetAll()
                                                .AsNoTracking()
                                   on ji.Identifier equals invi.Id
                                   into invItems
                                   from invi in invItems.DefaultIfEmpty()
                                   let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                   let multiCurrencyAmount = invi != null ? (ji.Journal.MultiCurrencyId != null ? invi.MultiCurrencyTotal : invi.Total) :
                                                             (ji.Journal.MultiCurrencyId != null ? ji.Journal.Invoice.MultiCurrencyTotal : ji.Journal.Invoice.Total)
                                   let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                   let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                   select new AccountTransaction
                                   {
                                       AccountId = ji.AccountId,
                                       AccountName = ji.Account.AccountName,
                                       AccountCode = ji.Account.AccountCode,
                                       AccountTypeId = ji.Account.AccountTypeId,
                                       Type = ji.Account.AccountType.Type,
                                       Debit = Math.Round(ji.Debit, roundDigit),
                                       Credit = Math.Round(ji.Credit, roundDigit),
                                       Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                       LocationId = ji.Journal.LocationId.Value,
                                       CurrencyId = currencyId,
                                       MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                       MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                       MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       RoundDigits = roundDigit,
                                   };


            var billItemQuery = from ji in _journalItemRepository.GetAll()
                                           .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Bill)
                                           .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                           .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                           .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                           .AsNoTracking()
                                join bi in _billItemRepository.GetAll()
                                           .AsNoTracking()
                                on ji.Identifier equals bi.Id
                                into billItems
                                from bi in billItems.DefaultIfEmpty()
                                let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                let multiCurrencyAmount = bi != null ? (ji.Journal.MultiCurrencyId != null ? bi.MultiCurrencyTotal : bi.Total) :
                                                          (ji.Journal.MultiCurrencyId != null ? ji.Journal.Bill.MultiCurrencyTotal : ji.Journal.Bill.Total)
                                let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                select new AccountTransaction
                                {
                                    AccountId = ji.AccountId,
                                    AccountName = ji.Account.AccountName,
                                    AccountCode = ji.Account.AccountCode,
                                    AccountTypeId = ji.Account.AccountTypeId,
                                    Type = ji.Account.AccountType.Type,
                                    Debit = Math.Round(ji.Debit, roundDigit),
                                    Credit = Math.Round(ji.Credit, roundDigit),
                                    Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                    LocationId = ji.Journal.LocationId.Value,
                                    CurrencyId = currencyId,
                                    MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                    MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                    MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                    RoundDigits = roundDigit,
                                };


            var customerCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                    .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.CustomerCredit)
                                                    .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                    .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                    .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                    .AsNoTracking()
                                          join cci in _customerCreditItemRepository.GetAll()
                                                    .AsNoTracking()
                                          on ji.Identifier equals cci.Id
                                          into ccItems
                                          from cci in ccItems.DefaultIfEmpty()
                                          let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                          let multiCurrencyAmount = cci != null ? (ji.Journal.MultiCurrencyId != null ? cci.MultiCurrencyTotal : cci.Total) :
                                                                    (ji.Journal.MultiCurrencyId != null ? ji.Journal.CustomerCredit.MultiCurrencyTotal : ji.Journal.CustomerCredit.Total)
                                          let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                          let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                          select new AccountTransaction
                                          {
                                              AccountId = ji.AccountId,
                                              AccountName = ji.Account.AccountName,
                                              AccountCode = ji.Account.AccountCode,
                                              AccountTypeId = ji.Account.AccountTypeId,
                                              Type = ji.Account.AccountType.Type,
                                              Debit = Math.Round(ji.Debit, roundDigit),
                                              Credit = Math.Round(ji.Credit, roundDigit),
                                              Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                              LocationId = ji.Journal.LocationId.Value,
                                              CurrencyId = currencyId,
                                              MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                              MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                              MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              RoundDigits = roundDigit,
                                          };


            var vendorCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                    .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.VendorCredit)
                                                    .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                                    .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                    .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                    .AsNoTracking()
                                        join vci in _vendorCreditItemRepository.GetAll()
                                                    .AsNoTracking()
                                        on ji.Identifier equals vci.Id
                                        into vcItems
                                        from vci in vcItems.DefaultIfEmpty()
                                        let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                        let multiCurrencyAmount = vci != null ? (ji.Journal.MultiCurrencyId != null ? vci.MultiCurrencyTotal : vci.Total) :
                                                                  (ji.Journal.MultiCurrencyId != null ? ji.Journal.VendorCredit.MultiCurrencyTotal : ji.Journal.VendorCredit.Total)
                                        let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                        let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                        select new AccountTransaction
                                        {
                                            AccountId = ji.AccountId,
                                            AccountName = ji.Account.AccountName,
                                            AccountCode = ji.Account.AccountCode,
                                            AccountTypeId = ji.Account.AccountTypeId,
                                            Type = ji.Account.AccountType.Type,
                                            Debit = Math.Round(ji.Debit, roundDigit),
                                            Credit = Math.Round(ji.Credit, roundDigit),
                                            Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                            LocationId = ji.Journal.LocationId.Value,
                                            CurrencyId = currencyId,
                                            MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                            MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                            MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                            RoundDigits = roundDigit,
                                        };


            var otherCashJournalTypes = new List<JournalType> { JournalType.Deposit, JournalType.Withdraw, JournalType.GeneralJournal };
            var multiCurrencyExceptKeys = new List<PostingKey> { PostingKey.InventoryAdjustmentInv, PostingKey.InventoryAdjustmentAdj };

            var otherJournalItemQuery = _journalItemRepository
                                       .GetAll()
                                       .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                       .Where(u => u.Journal.Status == TransactionStatus.Publish && otherCashJournalTypes.Contains(u.Journal.JournalType))
                                       .Where(u => u.Journal.Date.Date > fromDateBeginning && u.Journal.Date.Date <= toDate.Date)
                                       .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                       .AsNoTracking()
                                       .Select(u => new AccountTransaction
                                       {
                                           LocationId = u.Journal.LocationId.Value,
                                           AccountId = u.AccountId,
                                           AccountName = u.Account.AccountName,
                                           AccountCode = u.Account.AccountCode,
                                           AccountTypeId = u.Account.AccountTypeId,
                                           Type = u.Account.AccountType.Type,
                                           Debit = Math.Round(u.Debit, roundDigit),
                                           Credit = Math.Round(u.Credit, roundDigit),
                                           Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                           CurrencyId = u.Journal.CurrencyId,
                                           MultiCurrencyDebit = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Debit, roundDigit),
                                           MultiCurrencyCredit = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Credit, roundDigit),
                                           MultiCurrencyBalance = multiCurrencyExceptKeys.Contains(u.Key) ? 0 : Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                           RoundDigits = roundDigit,
                                       });

            var concatedAccountQuery = payBillItemQuery
                                        .Concat(receivePaymentItemQuery)
                                        .Concat(invoiceItemQuery)
                                        .Concat(billItemQuery)
                                        .Concat(customerCreditItemQuery)
                                        .Concat(vendorCreditItemQuery)
                                        .Concat(otherJournalItemQuery);

            if(previousCycle != null)
            {
                var accountCloseQuery = _accountTransactionRepository.GetAll()
                                   .Where(t => t.AccountCycleId == previousCycle.Id)
                                   .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                   .WhereIf(locations != null && locations.Count() > 0, s => locations.Contains(s.LocationId.Value))
                                   .AsNoTracking()
                               .Select(u => new AccountTransaction
                               {
                                   LocationId = u.LocationId.Value,
                                   AccountId = u.AccountId,
                                   AccountName = u.Account.AccountName,
                                   AccountCode = u.Account.AccountCode,
                                   AccountTypeId = u.Account.AccountTypeId,
                                   Type = u.Account.AccountType.Type,
                                   Debit = Math.Round(u.Debit, roundDigit),
                                   Credit = Math.Round(u.Credit, roundDigit),
                                   Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                   CurrencyId = u.CurrencyId,
                                   MultiCurrencyDebit = Math.Round(u.MultiCurrencyDebit, roundDigit),
                                   MultiCurrencyCredit = Math.Round(u.MultiCurrencyCredit, roundDigit),
                                   MultiCurrencyBalance = Math.Round(u.MultiCurrencyDebit, roundDigit) - Math.Round(u.MultiCurrencyCredit, roundDigit),
                                   RoundDigits = roundDigit,
                               });

                concatedAccountQuery = accountCloseQuery.Concat(concatedAccountQuery);
            }

            var result = concatedAccountQuery
                            .OrderBy(s => s.AccountCode)
                            .GroupBy(u => new { u.AccountId, u.AccountCode, u.AccountName, u.AccountTypeId, u.Type, u.CurrencyId })
                            .Select(u => new CashAccountTransaction
                            {
                                AccountId = u.Key.AccountId,
                                AccountName = u.Key.AccountName,
                                AccountCode = u.Key.AccountCode,
                                AccountTypeId = u.Key.AccountTypeId,
                                Type = u.Key.Type,
                                Debit = u.Sum(t => t.Debit),
                                Credit = u.Sum(t => t.Credit),
                                Balance = u.Sum(t => t.Balance),
                                CurrencyId = u.Key.CurrencyId,
                                MultiCurrencyDebit = u.Sum(t => t.MultiCurrencyDebit),
                                MultiCurrencyCredit = u.Sum(t => t.MultiCurrencyCredit),
                                MultiCurrencyBalance = u.Sum(t => t.MultiCurrencyBalance),
                                RoundDigits = roundDigit,
                            });

            return result;

        }

        public IQueryable<CashAccountTransaction> GetCashAccountByPeriodQueryHelper(DateTime fromDate, DateTime toDate, List<long> locations = null)
        {

            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentCycle = GetCyleAsync(toDate);
            var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

            var payBillItemWithReferenceQuery = from pbi in _payBillItemRepository.GetAll()
                                                            .AsNoTracking()
                                                join bj in _journalRepository.GetAll()
                                                            .Where(s => s.JournalType == JournalType.Bill)
                                                            .Where(s => s.Status == TransactionStatus.Publish)
                                                            .AsNoTracking()
                                                on pbi.BillId equals bj.BillId into pBills
                                                from pb in pBills.DefaultIfEmpty()

                                                join vcj in _journalRepository.GetAll()
                                                    .Where(s => s.JournalType == JournalType.VendorCredit)
                                                    .Where(s => s.Status == TransactionStatus.Publish)
                                                    .AsNoTracking()
                                                on pbi.VendorCreditId equals vcj.VendorCreditId into pVendorCredits
                                                from pvc in pVendorCredits.DefaultIfEmpty()

                                                select new
                                                {
                                                    pbi.PayBillId,
                                                    pbi.VendorId,
                                                    pbi.Vendor.VendorName,
                                                    OtherJournalNo = pb != null ? pb.JournalNo : pvc.JournalNo,
                                                    OtherReference = pb != null ? pb.Reference : pvc.Reference,
                                                };

            var payBillJournalItemQuery = from ji in _journalItemRepository
                                                    .GetAll()
                                                    .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                                    .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                    .Where(u => u.Journal.JournalType == JournalType.PayBill)
                                                    .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                                    .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                    .AsNoTracking()

                                          join pbi in _payBillItemRepository.GetAll()
                                                      .AsNoTracking()
                                          on ji.Identifier equals pbi.Id
                                          into pbillItems
                                          from pbi in pbillItems.DefaultIfEmpty()

                                          join pbex in _payBillExpenseRepository.GetAll()
                                                       .AsNoTracking()
                                          on ji.Identifier equals pbex.Id
                                          into pbillExpenses
                                          from pbex in pbillExpenses.DefaultIfEmpty()

                                          let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                          let multiCurrencyAmount = pbi != null ? (ji.Journal.MultiCurrencyId != null ? pbi.MultiCurrencyPayment : pbi.Payment) :
                                                                    pbex != null ? (ji.Journal.MultiCurrencyId != null ? pbex.MultiCurrencyAmount : pbex.Amount) :
                                                                    (ji.Journal.MultiCurrencyId != null ? ji.Journal.PayBill.MultiCurrencyTotalPayment : ji.Journal.PayBill.TotalPayment)
                                          let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                          let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                          select new CashAccountTransaction
                                          {
                                              CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                              LocationId = ji.Journal.Location.Id,
                                              LocationName = ji.Journal.Location.LocationName,
                                              UserId = ji.Journal.CreatorUserId.Value,
                                              UserName = ji.Journal.CreatorUser.UserName,
                                              Reference = ji.Journal.Reference,
                                              AccountId = ji.AccountId,
                                              AccountName = ji.Account.AccountName,
                                              AccountCode = ji.Account.AccountCode,
                                              AccountTypeId = ji.Account.AccountTypeId,
                                              Type = ji.Account.AccountType.Type,
                                              Debit = Math.Round(ji.Debit, roundDigit),
                                              Credit = Math.Round(ji.Credit, roundDigit),
                                              Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                              JournalDate = ji.Journal.Date,
                                              JournalNo = ji.Journal.JournalNo,
                                              JournalType = ji.Journal.JournalType,
                                              JournalId = ji.JournalId,
                                              JournalMemo = ji.Journal.Memo,
                                              Description = ji.Description,
                                              TransactionId = ji.Journal.PayBillId,
                                              CurrencyId = currencyId,
                                              MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                              MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                              MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              RoundDigits = roundDigit,
                                          };


            var cashFromPayBillQuery = from ji in payBillJournalItemQuery

                                       join pbir in payBillItemWithReferenceQuery
                                       on ji.TransactionId equals pbir.PayBillId
                                       into payBillItems

                                       select new CashAccountTransaction
                                       {
                                           CreationTimeIndex = ji.CreationTimeIndex,
                                           LocationId = ji.LocationId,
                                           LocationName = ji.LocationName,
                                           UserId = ji.UserId,
                                           UserName = ji.UserName,
                                           Reference = ji.Reference,
                                           AccountId = ji.AccountId,
                                           AccountName = ji.AccountName,
                                           AccountCode = ji.AccountCode,
                                           AccountTypeId = ji.AccountTypeId,
                                           Type = ji.Type,
                                           Debit = ji.Debit,
                                           Credit = ji.Credit,
                                           Balance = ji.Balance,
                                           JournalDate = ji.JournalDate,
                                           JournalNo = ji.JournalNo,
                                           JournalType = ji.JournalType,
                                           JournalId = ji.JournalId,
                                           JournalMemo = ji.JournalMemo,
                                           Description = ji.Description,
                                           TransactionId = ji.TransactionId,
                                           CurrencyId = ji.CurrencyId,
                                           MultiCurrencyDebit = ji.MultiCurrencyDebit,
                                           MultiCurrencyCredit = ji.MultiCurrencyCredit,
                                           MultiCurrencyBalance = ji.MultiCurrencyBalance,
                                           RoundDigits = roundDigit,
                                           OtherReferences = payBillItems.Select(s => new OtherReferenceOutput
                                           {
                                               JournalNo = s.OtherJournalNo,
                                               Reference = s.OtherReference,
                                               PartnerId = s.VendorId,
                                               PartnerName = s.VendorName,
                                           }).ToList(),
                                       };

            var receivePaymentItemWithReferenceQuery = from pbi in _receivePaymentItemRepository.GetAll()
                                                                   .AsNoTracking()
                                                       join bj in _journalRepository.GetAll()
                                                                   .Where(s => s.Status == TransactionStatus.Publish)
                                                                   .Where(s => s.JournalType == JournalType.Invoice)
                                                                   .AsNoTracking()
                                                       on pbi.InvoiceId equals bj.InvoiceId into pBills
                                                       from pb in pBills.DefaultIfEmpty()

                                                       join vcj in _journalRepository.GetAll()
                                                                   .Where(s => s.Status == TransactionStatus.Publish)
                                                                   .Where(s => s.JournalType == JournalType.CustomerCredit)
                                                                   .AsNoTracking()
                                                       on pbi.CustomerCreditId equals vcj.CustomerCreditId into pVendorCredits
                                                       from pvc in pVendorCredits.DefaultIfEmpty()

                                                       select new
                                                       {
                                                           pbi.ReceivePaymentId,
                                                           pbi.CustomerId,
                                                           pbi.Customer.CustomerName,
                                                           OtherJournalNo = pb != null ? pb.JournalNo : pvc.JournalNo,
                                                           OtherReference = pb != null ? pb.Reference : pvc.Reference,
                                                       };

            var receivePaymentJournalItemQuery = from ji in _journalItemRepository
                                                           .GetAll()
                                                           .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                                           .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                           .Where(u => u.Journal.JournalType == JournalType.ReceivePayment)
                                                           .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                                           .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                           .AsNoTracking()

                                                 join rpi in _receivePaymentItemRepository.GetAll()
                                                             .AsNoTracking()
                                                 on ji.Identifier equals rpi.Id
                                                 into rpItems
                                                 from rpi in rpItems.DefaultIfEmpty()

                                                 join rpex in _receivePaymentExpenseRepository.GetAll()
                                                              .AsNoTracking()
                                                 on ji.Identifier equals rpex.Id
                                                 into rpExpenses
                                                 from rpex in rpExpenses.DefaultIfEmpty()

                                                 let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId
                                                 let currencyCode = ji.Journal.MultiCurrency == null ? ji.Journal.Currency.Code : ji.Journal.MultiCurrency.Code

                                                 let multiCurrencyAmount = rpi != null ? (ji.Journal.MultiCurrencyId != null ? rpi.MultiCurrencyPayment : rpi.Payment) :
                                                                           rpex != null ? (ji.Journal.MultiCurrencyId != null ? rpex.MultiCurrencyAmount : rpex.Amount) :
                                                                           (ji.Journal.MultiCurrencyId != null ? ji.Journal.ReceivePayment.MultiCurrencyTotalPayment : ji.Journal.ReceivePayment.TotalPayment)
                                                 let multiCurrencyDebit = ji.Debit > 0 ? Math.Abs(multiCurrencyAmount) : 0
                                                 let multiCurrencyCredit = ji.Credit > 0 ? Math.Abs(multiCurrencyAmount) : 0

                                                 select new CashAccountTransaction
                                                 {
                                                     CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                                     LocationId = ji.Journal.Location.Id,
                                                     LocationName = ji.Journal.Location.LocationName,
                                                     UserId = ji.Journal.CreatorUserId.Value,
                                                     UserName = ji.Journal.CreatorUser.UserName,
                                                     Reference = ji.Journal.Reference,
                                                     AccountId = ji.AccountId,
                                                     AccountName = ji.Account.AccountName,
                                                     AccountCode = ji.Account.AccountCode,
                                                     AccountTypeId = ji.Account.AccountTypeId,
                                                     Type = ji.Account.AccountType.Type,
                                                     Debit = Math.Round(ji.Debit, roundDigit),
                                                     Credit = Math.Round(ji.Credit, roundDigit),
                                                     Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                                     JournalDate = ji.Journal.Date,
                                                     JournalNo = ji.Journal.JournalNo,
                                                     JournalType = ji.Journal.JournalType,
                                                     JournalId = ji.JournalId,
                                                     JournalMemo = ji.Journal.Memo,
                                                     Description = ji.Description,
                                                     TransactionId = ji.Journal.ReceivePaymentId,
                                                     CurrencyId = currencyId,
                                                     MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                                     MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                                     MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                                     RoundDigits = roundDigit,
                                                 };


            var cashFromReceivePaymentQuery = from ji in receivePaymentJournalItemQuery

                                              join rpi in receivePaymentItemWithReferenceQuery
                                              on ji.TransactionId equals rpi.ReceivePaymentId
                                              into receiptPaymentItems

                                              select new CashAccountTransaction
                                              {
                                                  CreationTimeIndex = ji.CreationTimeIndex,
                                                  LocationId = ji.LocationId,
                                                  LocationName = ji.LocationName,
                                                  UserId = ji.UserId,
                                                  UserName = ji.UserName,
                                                  Reference = ji.Reference,
                                                  AccountId = ji.AccountId,
                                                  AccountName = ji.AccountName,
                                                  AccountCode = ji.AccountCode,
                                                  AccountTypeId = ji.AccountTypeId,
                                                  Type = ji.Type,
                                                  Debit = ji.Debit,
                                                  Credit = ji.Credit,
                                                  Balance = ji.Balance,
                                                  JournalDate = ji.JournalDate,
                                                  JournalNo = ji.JournalNo,
                                                  JournalType = ji.JournalType,
                                                  JournalId = ji.JournalId,
                                                  JournalMemo = ji.JournalMemo,
                                                  Description = ji.Description,
                                                  TransactionId = ji.TransactionId,
                                                  CurrencyId = ji.CurrencyId,
                                                  MultiCurrencyDebit = ji.MultiCurrencyDebit,
                                                  MultiCurrencyCredit = ji.MultiCurrencyCredit,
                                                  MultiCurrencyBalance = ji.MultiCurrencyBalance,
                                                  RoundDigits = roundDigit,
                                                  OtherReferences = receiptPaymentItems.Select(s => new OtherReferenceOutput
                                                  {
                                                      JournalNo = s.OtherJournalNo,
                                                      Reference = s.OtherReference,
                                                      PartnerId = s.CustomerId,
                                                      PartnerName = s.CustomerName,
                                                  }).ToList(),
                                              };

            var invoiceItemQuery = from ji in _journalItemRepository.GetAll()
                                              .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Invoice)
                                              .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                              .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                              .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                              .AsNoTracking()
                                   join invi in _invoiceItemRepository.GetAll()
                                                .AsNoTracking()
                                   on ji.Identifier equals invi.Id
                                   into invItems
                                   from invi in invItems.DefaultIfEmpty()
                                   let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                   let multiCurrencyAmount = invi != null ? (ji.Journal.MultiCurrencyId != null ? invi.MultiCurrencyTotal : invi.Total) :
                                                             (ji.Journal.MultiCurrencyId != null ? ji.Journal.Invoice.MultiCurrencyTotal : ji.Journal.Invoice.Total)
                                   let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                   let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                   select new CashAccountTransaction
                                   {
                                       CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                       LocationId = ji.Journal.Location.Id,
                                       LocationName = ji.Journal.Location.LocationName,
                                       UserId = ji.Journal.CreatorUserId.Value,
                                       UserName = ji.Journal.CreatorUser.UserName,
                                       Reference = ji.Journal.Reference,
                                       AccountId = ji.AccountId,
                                       AccountName = ji.Account.AccountName,
                                       AccountCode = ji.Account.AccountCode,
                                       AccountTypeId = ji.Account.AccountTypeId,
                                       Type = ji.Account.AccountType.Type,
                                       Debit = Math.Round(ji.Debit, roundDigit),
                                       Credit = Math.Round(ji.Credit, roundDigit),
                                       Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                       JournalDate = ji.Journal.Date,
                                       JournalNo = ji.Journal.JournalNo,
                                       JournalType = ji.Journal.JournalType,
                                       JournalId = ji.JournalId,
                                       JournalMemo = ji.Journal.Memo,
                                       Description = ji.Description,
                                       TransactionId = ji.Journal.ReceivePaymentId,
                                       CurrencyId = currencyId,
                                       MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                       MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                       MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                       RoundDigits = roundDigit,
                                       OtherReferences = new List<OtherReferenceOutput>
                                              {
                                                  new OtherReferenceOutput
                                                  {
                                                      JournalNo = "",
                                                      Reference = "",
                                                      PartnerId = ji.Journal.Invoice.CustomerId,
                                                      PartnerName = ji.Journal.Invoice.Customer.CustomerName,
                                                  }
                                              },
                                   };


            var billItemQuery = from ji in _journalItemRepository.GetAll()
                                            .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.Bill)
                                            .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                            .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                            .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                            .AsNoTracking()
                                join bi in _billItemRepository.GetAll()
                                           .AsNoTracking()
                                on ji.Identifier equals bi.Id
                                into billItems
                                from bi in billItems.DefaultIfEmpty()
                                let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                let multiCurrencyAmount = bi != null ? (ji.Journal.MultiCurrencyId != null ? bi.MultiCurrencyTotal : bi.Total) :
                                                          (ji.Journal.MultiCurrencyId != null ? ji.Journal.Bill.MultiCurrencyTotal : ji.Journal.Bill.Total)
                                let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                select new CashAccountTransaction
                                {
                                    CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                    LocationId = ji.Journal.Location.Id,
                                    LocationName = ji.Journal.Location.LocationName,
                                    UserId = ji.Journal.CreatorUserId.Value,
                                    UserName = ji.Journal.CreatorUser.UserName,
                                    Reference = ji.Journal.Reference,
                                    AccountId = ji.AccountId,
                                    AccountName = ji.Account.AccountName,
                                    AccountCode = ji.Account.AccountCode,
                                    AccountTypeId = ji.Account.AccountTypeId,
                                    Type = ji.Account.AccountType.Type,
                                    Debit = Math.Round(ji.Debit, roundDigit),
                                    Credit = Math.Round(ji.Credit, roundDigit),
                                    Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                    JournalDate = ji.Journal.Date,
                                    JournalNo = ji.Journal.JournalNo,
                                    JournalType = ji.Journal.JournalType,
                                    JournalId = ji.JournalId,
                                    JournalMemo = ji.Journal.Memo,
                                    Description = ji.Description,
                                    TransactionId = ji.Journal.ReceivePaymentId,
                                    CurrencyId = currencyId,
                                    MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                    MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                    MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                    RoundDigits = roundDigit,
                                    OtherReferences = new List<OtherReferenceOutput>
                                              {
                                                  new OtherReferenceOutput
                                                  {
                                                      JournalNo = "",
                                                      Reference = "",
                                                      PartnerId = ji.Journal.Bill.VendorId,
                                                      PartnerName = ji.Journal.Bill.Vendor.VendorName,
                                                  }
                                              },
                                };

            var customerCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                       .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.CustomerCredit)
                                                       .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                                       .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                       .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                       .AsNoTracking()
                                          join cci in _customerCreditItemRepository.GetAll()
                                                      .AsNoTracking()
                                          on ji.Identifier equals cci.Id
                                          into ccItems
                                          from cci in ccItems.DefaultIfEmpty()
                                          let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                          let multiCurrencyAmount = cci != null ? (ji.Journal.MultiCurrencyId != null ? cci.MultiCurrencyTotal : cci.Total) :
                                                                    (ji.Journal.MultiCurrencyId != null ? ji.Journal.CustomerCredit.MultiCurrencyTotal : ji.Journal.CustomerCredit.Total)
                                          let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                          let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                          select new CashAccountTransaction
                                          {
                                              CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                              LocationId = ji.Journal.Location.Id,
                                              LocationName = ji.Journal.Location.LocationName,
                                              UserId = ji.Journal.CreatorUserId.Value,
                                              UserName = ji.Journal.CreatorUser.UserName,
                                              Reference = ji.Journal.Reference,
                                              AccountId = ji.AccountId,
                                              AccountName = ji.Account.AccountName,
                                              AccountCode = ji.Account.AccountCode,
                                              AccountTypeId = ji.Account.AccountTypeId,
                                              Type = ji.Account.AccountType.Type,
                                              Debit = Math.Round(ji.Debit, roundDigit),
                                              Credit = Math.Round(ji.Credit, roundDigit),
                                              Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                              JournalDate = ji.Journal.Date,
                                              JournalNo = ji.Journal.JournalNo,
                                              JournalType = ji.Journal.JournalType,
                                              JournalId = ji.JournalId,
                                              JournalMemo = ji.Journal.Memo,
                                              Description = ji.Description,
                                              TransactionId = ji.Journal.ReceivePaymentId,
                                              CurrencyId = currencyId,
                                              MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                              MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                              MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                              RoundDigits = roundDigit,
                                              OtherReferences = new List<OtherReferenceOutput>
                                              {
                                                  new OtherReferenceOutput
                                                  {
                                                      JournalNo = "",
                                                      Reference = "",
                                                      PartnerId = ji.Journal.CustomerCredit.CustomerId,
                                                      PartnerName = ji.Journal.CustomerCredit.Customer.CustomerName,
                                                  }
                                              },
                                          };


            var vendorCreditItemQuery = from ji in _journalItemRepository.GetAll()
                                                  .Where(u => u.Journal.Status == TransactionStatus.Publish && u.Journal.JournalType == JournalType.VendorCredit)
                                                  .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                                                  .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                                  .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                                                  .AsNoTracking()
                                        join vci in _vendorCreditItemRepository.GetAll()
                                                    .AsNoTracking()
                                        on ji.Identifier equals vci.Id
                                        into vcItems
                                        from vci in vcItems.DefaultIfEmpty()
                                        let currencyId = ji.Journal.MultiCurrencyId == null ? ji.Journal.CurrencyId : ji.Journal.MultiCurrencyId

                                        let multiCurrencyAmount = vci != null ? (ji.Journal.MultiCurrencyId != null ? vci.MultiCurrencyTotal : vci.Total) :
                                                                  (ji.Journal.MultiCurrencyId != null ? ji.Journal.VendorCredit.MultiCurrencyTotal : ji.Journal.VendorCredit.Total)
                                        let multiCurrencyDebit = ji.Debit > 0 ? multiCurrencyAmount : 0
                                        let multiCurrencyCredit = ji.Credit > 0 ? multiCurrencyAmount : 0

                                        select new CashAccountTransaction
                                        {
                                            CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                            LocationId = ji.Journal.Location.Id,
                                            LocationName = ji.Journal.Location.LocationName,
                                            UserId = ji.Journal.CreatorUserId.Value,
                                            UserName = ji.Journal.CreatorUser.UserName,
                                            Reference = ji.Journal.Reference,
                                            AccountId = ji.AccountId,
                                            AccountName = ji.Account.AccountName,
                                            AccountCode = ji.Account.AccountCode,
                                            AccountTypeId = ji.Account.AccountTypeId,
                                            Type = ji.Account.AccountType.Type,
                                            Debit = Math.Round(ji.Debit, roundDigit),
                                            Credit = Math.Round(ji.Credit, roundDigit),
                                            Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                            JournalDate = ji.Journal.Date,
                                            JournalNo = ji.Journal.JournalNo,
                                            JournalType = ji.Journal.JournalType,
                                            JournalId = ji.JournalId,
                                            JournalMemo = ji.Journal.Memo,
                                            Description = ji.Description,
                                            TransactionId = ji.Journal.ReceivePaymentId,
                                            CurrencyId = currencyId,
                                            MultiCurrencyDebit = Math.Round(multiCurrencyDebit, roundDigit),
                                            MultiCurrencyCredit = Math.Round(multiCurrencyCredit, roundDigit),
                                            MultiCurrencyBalance = Math.Round(multiCurrencyDebit, roundDigit) - Math.Round(multiCurrencyCredit, roundDigit),
                                            RoundDigits = roundDigit,
                                            OtherReferences = new List<OtherReferenceOutput>
                                              {
                                                  new OtherReferenceOutput
                                                  {
                                                      JournalNo = "",
                                                      Reference = "",
                                                      PartnerId = ji.Journal.VendorCredit.VendorId,
                                                      PartnerName = ji.Journal.VendorCredit.Vendor.VendorName,
                                                  }
                                              },
                                        };



            var otherCashJournalTypes = new List<JournalType> { JournalType.Deposit, JournalType.Withdraw, JournalType.GeneralJournal };
            var multiCurrencyExceptKeys = new List<PostingKey> { PostingKey.InventoryAdjustmentInv, PostingKey.InventoryAdjustmentAdj };

            var otherCashJournalQuery = _journalItemRepository
                            .GetAll()
                            .Where(u => u.Journal.Status == TransactionStatus.Publish)
                            .Where(s => s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.Account.AccountType.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                            .Where(u => otherCashJournalTypes.Contains(u.Journal.JournalType))
                            .Where(u => u.Journal.Date.Date >= fromDate.Date && u.Journal.Date.Date <= toDate.Date)
                            .WhereIf(locations != null && locations.Count() > 0, u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))
                            .AsNoTracking()
                            .Select(ji => new CashAccountTransaction
                            {
                                CreationTimeIndex = ji.Journal.CreationTimeIndex,
                                LocationId = ji.Journal.Location.Id,
                                LocationName = ji.Journal.Location.LocationName,
                                UserId = ji.Journal.CreatorUserId.Value,
                                UserName = ji.Journal.CreatorUser.UserName,
                                Reference = ji.Journal.Reference,
                                AccountId = ji.AccountId,
                                AccountName = ji.Account.AccountName,
                                AccountCode = ji.Account.AccountCode,
                                AccountTypeId = ji.Account.AccountTypeId,
                                Type = ji.Account.AccountType.Type,
                                Debit = Math.Round(ji.Debit, roundDigit),
                                Credit = Math.Round(ji.Credit, roundDigit),
                                Balance = Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                JournalDate = ji.Journal.Date,
                                JournalNo = ji.Journal.JournalNo,
                                JournalType = ji.Journal.JournalType,
                                JournalId = ji.JournalId,
                                JournalMemo = ji.Journal.Memo,
                                Description = ji.Description,
                                CurrencyId = ji.Journal.CurrencyId,
                                MultiCurrencyDebit = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 : Math.Round(ji.Debit, roundDigit),
                                MultiCurrencyCredit = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 : Math.Round(ji.Credit, roundDigit),
                                MultiCurrencyBalance = multiCurrencyExceptKeys.Contains(ji.Key) ? 0 : Math.Round(ji.Debit, roundDigit) - Math.Round(ji.Credit, roundDigit),
                                TransactionId = ji.Journal.DepositId != null ? ji.Journal.DepositId :
                                                ji.Journal.WithdrawId != null ? ji.Journal.WithdrawId : ji.JournalId,
                                BankTransferId = ji.Journal.Deposit != null ? ji.Journal.Deposit.BankTransferId :
                                                 ji.Journal.withdraw != null ? ji.Journal.withdraw.BankTransferId : (Guid?)null,
                                RoundDigits = roundDigit,
                                OtherReferences = new List<OtherReferenceOutput> {
                                    new OtherReferenceOutput
                                    {
                                        JournalNo = "",
                                        Reference = "",
                                        PartnerId = ji.Journal.withdraw != null && ji.Journal.withdraw.Vendor != null ? ji.Journal.withdraw.VendorId :
                                                    ji.Journal.withdraw != null && ji.Journal.withdraw.Customer != null ? ji.Journal.withdraw.CustomerId :
                                                    ji.Journal.Deposit != null && ji.Journal.Deposit.Vendor != null ? ji.Journal.Deposit.ReceiveFromVendorId :
                                                    ji.Journal.Deposit != null && ji.Journal.Deposit.Customer != null ? ji.Journal.Deposit.ReceiveFromCustomerId : (Guid?) null,
                                        PartnerName = ji.Journal.withdraw != null && ji.Journal.withdraw.Vendor != null ? ji.Journal.withdraw.Vendor.VendorName :
                                                      ji.Journal.withdraw != null && ji.Journal.withdraw.Customer != null ? ji.Journal.withdraw.Customer.CustomerName :
                                                      ji.Journal.Deposit != null && ji.Journal.Deposit.Vendor != null ? ji.Journal.Deposit.Vendor.VendorName :
                                                      ji.Journal.Deposit != null && ji.Journal.Deposit.Customer != null ? ji.Journal.Deposit.Customer.CustomerName : "",
                                    }
                                },
                            });

            var resultQeury = cashFromPayBillQuery
                              .Concat(cashFromReceivePaymentQuery)
                              .Concat(otherCashJournalQuery)
                              .Concat(invoiceItemQuery)
                              .Concat(billItemQuery)
                              .Concat(customerCreditItemQuery)
                              .Concat(vendorCreditItemQuery);


            return resultQeury;


        }



    }
}
