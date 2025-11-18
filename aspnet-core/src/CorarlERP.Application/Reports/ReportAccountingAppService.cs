using Abp.Application.Services.Dto;
using Abp.AspNetZeroCore.Net;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.AccountTransactions;
using CorarlERP.AccountTrasactionCloses;
using CorarlERP.AccountTypes;
using CorarlERP.Authorization;
using CorarlERP.Authorization.Users;
using CorarlERP.Bills;
using CorarlERP.CashFlowTemplates;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.Dto;
using CorarlERP.FileStorages;
using CorarlERP.Formats;
using CorarlERP.InventoryCostCloses;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.Migrations;
using CorarlERP.MultiTenancy;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Vendors;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports
{
    [AbpAuthorize]
    public class ReportAccountingAppService : ReportBaseClass, IReportAccountingAppService
    {
        private readonly IJournalManager _journalManager;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<AccountType, long> _accountTypeRepository;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<AccountTransactionClose, Guid> _accountCloseRepository;

        private readonly IAccountTransactionManager _accountTransactionManager;
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IRepository<Format, long> _formatRepository;
        private readonly IRepository<Location, long> _locationsRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IFileStorageManager _fileStorageManager;

        private readonly IRepository<CashFlowTemplate, Guid> _cashFlowTemplateRepository;
        private readonly IRepository<CashFlowTemplateCategory, Guid> _cashFlowTemplateCategoryRepository;
        private readonly IRepository<CashFlowTemplateAccount, Guid> _cashFlowTemplateAccountRepository;
       
        private readonly IRepository<Currency, long> _currencyRepository;
        public ReportAccountingAppService(
            IRepository<CashFlowTemplate, Guid> cashFlowTemplateRepository,
            IRepository<CashFlowTemplateCategory, Guid> cashFlowTemplateCategoryRepository,
            IRepository<CashFlowTemplateAccount, Guid> cashFlowTemplateAccountRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<AccountType, long> accountTypeRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IAccountTransactionManager accountTransactionManager,
            IRepository<Bills.Bill, Guid> billRepository,
             AppFolders appFolders,
             IFileStorageManager fileStorageManager,
             IRepository<Location, long> locationsRepository,
             IRepository<Currency, long> currencyRepository,
            IRepository<Format, long> formatRepository,
            IUnitOfWorkManager unitOfWorkManager
           ) : base(accountCycleRepository, appFolders, null, null)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _journalManager = journalManager;
            _locationsRepository = locationsRepository;
            _journalItemManager = journalItemManager;
            _journalRepository = journalRepository;
            _journalItemRepository = journalItemRepository;
            _accountTypeRepository = accountTypeRepository;
            _tenantRepository = tenantRepository;
            _accountTransactionManager = accountTransactionManager;
            _appFolders = appFolders;
            _accountCycleRepository = accountCycleRepository;
            _formatRepository = formatRepository;
            _billRepository = billRepository;
            _currencyRepository = currencyRepository;
            _fileStorageManager = fileStorageManager;

            _accountCloseRepository = IocManager.Instance.Resolve<IRepository<AccountTransactionClose, Guid>>();
            _classRepository = IocManager.Instance.Resolve<IRepository<Class, long>>();
            _cashFlowTemplateAccountRepository = cashFlowTemplateAccountRepository;
            _cashFlowTemplateCategoryRepository = cashFlowTemplateCategoryRepository;
            _cashFlowTemplateRepository = cashFlowTemplateRepository;
        }


        #region Profile & Loss Report 

        private async Task<List<GetListIncomeReportOutput>> GetProfitLossStandard(GetIncomeInput input)
        {
            var accountTypeList = new[] { TypeOfAccount.Income, TypeOfAccount.COGS, TypeOfAccount.Expense };
            var account = await _accountTransactionManager.GetAccountQuery(input.FromDate, input.ToDate, false, input.Locations)
                            .Where(u => accountTypeList.Contains(u.Type))
                            .Select(u => new
                            {
                                AccountId = u.AccountId,
                                AccountName = u.AccountName,
                                AccountCode = u.AccountCode,
                                AccountTypeId = u.AccountTypeId,
                                Type = u.Type,
                                Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                    u.Type == TypeOfAccount.FixedAsset ||
                                    u.Type == TypeOfAccount.COGS ||
                                    u.Type == TypeOfAccount.Expense ?
                                    1 : -1) * u.Balance,
                            }).ToListAsync();

            var accountDict = account
                                .GroupBy(u => u.AccountTypeId)
                                .Select(u => new
                                {
                                    AccountTypeId = u.Key,
                                    TotalAmount = u.Sum(t => t.Balance),
                                    Values = u.ToList()
                                }).ToDictionary(u => u.AccountTypeId);

            var accountTypes = await _accountTypeRepository.GetAll().AsNoTracking().Where(t => accountTypeList.Contains(t.Type)).ToListAsync();

            var previousCycle = await GetPreviousRoundingCloseCyleAsync(input.ToDate);

            // Group account type of account chart from journal item above
            var query = (from c in accountTypes
                         select new GetListIncomeReportOutput
                         {
                             Id = c.Id,
                             RoundingDigit = previousCycle != null ? previousCycle.RoundingDigit : 2,
                             AccountTypeName = c.AccountTypeName,
                             AccountType = c.Type,
                             TotalAmount = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                             AccountLists = accountDict.ContainsKey(c.Id) ?
                                          accountDict[c.Id].Values
                                         .Select(t => new IncomeAccountDetailOutput
                                         {
                                             AccountCode = t.AccountCode,
                                             AccountName = t.AccountName,
                                             Id = t.AccountId,
                                             Total = t.Balance,
                                             TotalTemp = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                                         }).OrderBy(x => x.AccountCode).ToList() : new List<IncomeAccountDetailOutput>()
                         }).OrderBy(t => t.AccountType).ToList();

            var result = ObjectMapper.Map<List<GetListIncomeReportOutput>>(query);

            return result;
        }
        private async Task<List<GetListIncomeReportOutput>> GetProfitLossByLocation(GetIncomeInput input)
        {

            var locationListDics = await _locationsRepository.GetAll().AsNoTracking()
                                .ToDictionaryAsync(t => t.Id, t => t.LocationName);
            var accountTypeList = new[] { TypeOfAccount.Income, TypeOfAccount.COGS, TypeOfAccount.Expense };

            var queryResult = await _accountTransactionManager.GetAccountQuery(input.FromDate, input.ToDate, false, input.Locations, true)
                            .Where(u => accountTypeList.Contains(u.Type)).ToListAsync();

            var locations = queryResult.GroupBy(s => s.LocationId)
                            .Select(s => new LocationSummaryOutput { Id = s.Key, LocationName = locationListDics[s.Key] })
                            .OrderBy(x => x.LocationName).ToList();

            var account = queryResult
                            .GroupBy(x => new { x.AccountId, x.AccountCode, x.AccountName, x.AccountTypeId, x.Type })
                            .Select(u => new
                            {
                                AccountId = u.Key.AccountId,
                                AccountName = u.Key.AccountName,
                                AccountCode = u.Key.AccountCode,
                                AccountTypeId = u.Key.AccountTypeId,
                                Type = u.Key.Type,
                                Balance = (
                                    u.Key.Type == TypeOfAccount.COGS ||
                                    u.Key.Type == TypeOfAccount.Expense ?
                                    1 : -1) * u.Sum(s => s.Balance),
                                TotalLocationColumns = locations.Select(z => new TotalLocationColumns
                                {
                                    LocationName = z.LocationName,
                                    LocationId = z.Id,
                                    Total = (
                                        u.Key.Type == TypeOfAccount.COGS ||
                                        u.Key.Type == TypeOfAccount.Expense ?
                                        1 : -1) * u.Where(r => r.LocationId == z.Id).Sum(x => x.Balance),
                                    Percentage = 0,
                                }).ToDictionary(c => c.LocationId, c => c),


                            });



            var accountDict = account
                                .GroupBy(u => u.AccountTypeId)
                                .Select(u => new
                                {
                                    AccountTypeId = u.Key,
                                    TotalAmount = u.Sum(t => t.Balance),
                                    Values = u.ToList(),
                                    TotalLocationColumns = locations.Select(z => new TotalLocationColumns
                                    {
                                        LocationName = z.LocationName,
                                        LocationId = z.Id,
                                        Total = u.Sum(s => s.TotalLocationColumns.ContainsKey(z.Id) ? s.TotalLocationColumns[z.Id].Total : 0),
                                        Percentage = 0,
                                    }).ToDictionary(c => c.LocationId, c => c)
                                }).ToDictionary(u => u.AccountTypeId);

            var accountTypes = await _accountTypeRepository.GetAll().AsNoTracking().Where(t => accountTypeList.Contains(t.Type)).ToListAsync();

            var previousCycle = await GetPreviousRoundingCloseCyleAsync(input.ToDate);



            // Group account type of account chart from journal item above
            var query = (from c in accountTypes
                         select new GetListIncomeReportOutput
                         {
                             Id = c.Id,
                             RoundingDigit = previousCycle != null ? previousCycle.RoundingDigit : 2,
                             AccountTypeName = c.AccountTypeName,
                             AccountType = c.Type,
                             TotalAmount = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                             AccountLists = accountDict.ContainsKey(c.Id) ?
                                          accountDict[c.Id].Values
                                         .Select(t => new IncomeAccountDetailOutput
                                         {
                                             AccountCode = t.AccountCode,
                                             AccountName = t.AccountName,
                                             Id = t.AccountId,
                                             Total = t.Balance,
                                             TotalTemp = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                                             TotalLocationColumns = t.TotalLocationColumns
                                         }).OrderBy(x => x.AccountCode).ToList() : new List<IncomeAccountDetailOutput>(),
                             LocationSummaryHeader = locations,
                             TotalLocationSummaryByAccountType = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalLocationColumns : null,

                         }).OrderBy(t => t.AccountType).ToList();

            var result = ObjectMapper.Map<List<GetListIncomeReportOutput>>(query);

            return result;
        }


        private async Task<List<GetListIncomeReportOutput>> GetProfitLossByClass(GetIncomeInput input)
        {
            List< GetListIncomeReportOutput> query = null;
            string debugMessage = "";
            bool isDebug = false;
            List<AccountTransaction> debugDataList = null;

            try
            {
                var locationListDics = await _classRepository.GetAll().AsNoTracking()
                                .ToDictionaryAsync(t => t.Id, t => t.ClassName);
                var accountTypeList = new[] { TypeOfAccount.Income, TypeOfAccount.COGS, TypeOfAccount.Expense };

                var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);
                var currentCycle = await GetCurrentCycleAsync();
                var roundDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;

                debugMessage += "Query Begin" + Environment.NewLine;

                var queryResult = await _journalItemRepository
                                        .GetAll()
                                        .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                        .Where(u => input.FromDate.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                        .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.Journal.LocationId != null && input.Locations.Contains(u.Journal.LocationId.Value))
                                        .Where(u => accountTypeList.Contains(u.Account.AccountType.Type))
                                        .AsNoTracking()
                                        .Select(u => new
                                        {
                                            AccountId = u.AccountId,
                                            AccountName = u.Account.AccountName,
                                            AccountCode = u.Account.AccountCode,
                                            AccountTypeId = u.Account.AccountTypeId,
                                            Type = u.Account.AccountType.Type,
                                            Debit = Math.Round(u.Debit, roundDigit),
                                            Credit = Math.Round(u.Credit, roundDigit),
                                            Balance = Math.Round(u.Debit, roundDigit) - Math.Round(u.Credit, roundDigit),
                                            LocationId = u.Journal.ClassId,
                                        })
                                    .GroupBy(u => new { u.AccountId, u.AccountName, u.AccountCode, u.AccountTypeId, u.Type, LocationId = u.LocationId.HasValue ? u.LocationId.Value : 0 })
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
                                        Balance = u.Sum(t => t.Debit) - u.Sum(t => t.Credit)
                                    }).ToListAsync();

                debugMessage += "Query End" + Environment.NewLine;
                debugDataList = queryResult;

                var locations = queryResult.GroupBy(s => s.LocationId)
                                .Select(s => new LocationSummaryOutput { Id = s.Key, LocationName = locationListDics.ContainsKey(s.Key) ? locationListDics[s.Key] : "" })
                                .OrderBy(x => x.LocationName).ToList();

                debugMessage += "Get Locations from Query Result" + Environment.NewLine;

                var account = queryResult
                                .GroupBy(x => new { x.AccountId, x.AccountCode, x.AccountName, x.AccountTypeId, x.Type })
                                .Select(u => new
                                {
                                    AccountId = u.Key.AccountId,
                                    AccountName = u.Key.AccountName,
                                    AccountCode = u.Key.AccountCode,
                                    AccountTypeId = u.Key.AccountTypeId,
                                    Type = u.Key.Type,
                                    Balance = (
                                        u.Key.Type == TypeOfAccount.COGS ||
                                        u.Key.Type == TypeOfAccount.Expense ?
                                        1 : -1) * u.Sum(s => s.Balance),
                                    TotalLocationColumns = locations.Select(z => new TotalLocationColumns
                                    {
                                        LocationName = z.LocationName,
                                        LocationId = z.Id,
                                        Total = (u.Key.Type == TypeOfAccount.COGS || u.Key.Type == TypeOfAccount.Expense ? 1 : -1) *
                                                (u.Any(r => r.LocationId == z.Id) ? u.Where(r => r.LocationId == z.Id).Sum(x => x.Balance) : 0),
                                        Percentage = 0,
                                    }).ToDictionary(c => c.LocationId, c => c),

                                });

                debugMessage += "Group Query Result by Account { x.AccountId, x.AccountCode, x.AccountName, x.AccountTypeId, x.Type }" + Environment.NewLine;

                var accountDict = account
                                    .GroupBy(u => u.AccountTypeId)
                                    .Select(u => new
                                    {
                                        AccountTypeId = u.Key,
                                        TotalAmount = u.Sum(t => t.Balance),
                                        Values = u.ToList(),
                                        TotalLocationColumns = locations.Select(z => new TotalLocationColumns
                                        {
                                            LocationName = z.LocationName,
                                            LocationId = z.Id,
                                            Total = u.Sum(s => s.TotalLocationColumns.ContainsKey(z.Id) ? s.TotalLocationColumns[z.Id].Total : 0),
                                            Percentage = 0,
                                        }).ToDictionary(c => c.LocationId, c => c)
                                    }).ToDictionary(u => u.AccountTypeId);

                debugMessage += "Prepair List by Account Type" + Environment.NewLine;

                var accountTypes = await _accountTypeRepository.GetAll().AsNoTracking().Where(t => accountTypeList.Contains(t.Type)).ToListAsync();


                // Group account type of account chart from journal item above
                query = (from c in accountTypes
                             select new GetListIncomeReportOutput
                             {
                                 Id = c.Id,
                                 RoundingDigit = roundDigit,
                                 AccountTypeName = c.AccountTypeName,
                                 AccountType = c.Type,
                                 TotalAmount = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                                 AccountLists = accountDict.ContainsKey(c.Id) ?
                                              accountDict[c.Id].Values
                                             .Select(t => new IncomeAccountDetailOutput
                                             {
                                                 AccountCode = t.AccountCode,
                                                 AccountName = t.AccountName,
                                                 Id = t.AccountId,
                                                 Total = t.Balance,
                                                 TotalTemp = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                                                 TotalLocationColumns = t.TotalLocationColumns
                                             }).OrderBy(x => x.AccountCode).ToList() : new List<IncomeAccountDetailOutput>(),
                                 LocationSummaryHeader = locations,
                                 TotalLocationSummaryByAccountType = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalLocationColumns : null,

                             }).OrderBy(t => t.AccountType).ToList();

                debugMessage += "Final Query" + Environment.NewLine;

            }
            catch (Exception ex)
            {
                isDebug = true;
                throw ex;
            }
            finally
            {
                if (isDebug)
                {

                    if(debugDataList != null)
                    {
                        debugMessage += "Query Data No Class:" + Environment.NewLine;
                        var noClassDataList = debugDataList.Where(s => s.LocationId == 0).ToList();
                        foreach (var nq in noClassDataList)
                        {
                            debugMessage += JsonConvert.SerializeObject(nq) + Environment.NewLine;
                        }

                        debugMessage += "All Query Data:" + Environment.NewLine;
                        foreach (var q in debugDataList)
                        {
                            debugMessage += JsonConvert.SerializeObject(q) + Environment.NewLine;
                        }
                    }

                    var tenant = await GetCurrentTenantAsync();
                    await DebugLogWriter(tenant.Id, isDebug, debugMessage, _appFolders);
                }                
            }

            var result = ObjectMapper.Map<List<GetListIncomeReportOutput>>(query);

            return result;
        }

        private async Task<List<GetListIncomeReportOutput>> GetProfitLossByMonthly(GetIncomeInput input)
        {
            var accountTypeList = new[] { TypeOfAccount.Income, TypeOfAccount.COGS, TypeOfAccount.Expense };

            var queryResult = await _accountTransactionManager.GetAccountMonthlyQuery(input.FromDate, input.ToDate, input.Locations)
                                .Where(u => accountTypeList.Contains(u.Type)).ToListAsync();
            var monthly = queryResult
                            .OrderBy(s => s.LocationId)
                            .GroupBy(s => new { s.LocationId, s.LocationName })
                            .Select(s => new LocationSummaryOutput { Id = s.Key.LocationId, LocationName = s.Key.LocationName })
                            .ToList();

            var account = queryResult
                            .GroupBy(x => new { x.AccountId, x.AccountCode, x.AccountName, x.AccountTypeId, x.Type })
                            .Select(u => new
                            {
                                AccountId = u.Key.AccountId,
                                AccountName = u.Key.AccountName,
                                AccountCode = u.Key.AccountCode,
                                AccountTypeId = u.Key.AccountTypeId,
                                Type = u.Key.Type,
                                Balance = (
                                    u.Key.Type == TypeOfAccount.COGS ||
                                    u.Key.Type == TypeOfAccount.Expense ?
                                    1 : -1) * u.Sum(s => s.Balance),
                                TotalLocationColumns = monthly.Select(z => new TotalLocationColumns
                                {
                                    LocationName = z.LocationName,
                                    LocationId = z.Id,
                                    Total = (
                                        u.Key.Type == TypeOfAccount.COGS ||
                                        u.Key.Type == TypeOfAccount.Expense ?
                                        1 : -1) * u.Where(r => r.LocationId == z.Id).Sum(x => x.Balance),
                                    Percentage = 0,
                                }).ToDictionary(c => c.LocationId, c => c),
                            });

            var accountDict = account
                                .GroupBy(u => u.AccountTypeId)
                                .Select(u => new
                                {
                                    AccountTypeId = u.Key,
                                    TotalAmount = u.Sum(t => t.Balance),
                                    Values = u.ToList(),
                                    TotalLocationColumns = monthly.Select(z => new TotalLocationColumns
                                    {
                                        LocationName = z.LocationName,
                                        LocationId = z.Id,
                                        Total = u.Sum(s => s.TotalLocationColumns.ContainsKey(z.Id) ? s.TotalLocationColumns[z.Id].Total : 0),
                                        Percentage = 0,
                                    }).ToDictionary(c => c.LocationId, c => c)
                                }).ToDictionary(u => u.AccountTypeId);

            var accountTypes = await _accountTypeRepository.GetAll().AsNoTracking().Where(t => accountTypeList.Contains(t.Type)).ToListAsync();

            var previousCycle = await GetPreviousRoundingCloseCyleAsync(input.ToDate);


            // Group account type of account chart from journal item above
            var query = (from c in accountTypes
                         select new GetListIncomeReportOutput
                         {
                             Id = c.Id,
                             RoundingDigit = previousCycle != null ? previousCycle.RoundingDigit : 2,
                             AccountTypeName = c.AccountTypeName,
                             AccountType = c.Type,
                             TotalAmount = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                             AccountLists = accountDict.ContainsKey(c.Id) ?
                                          accountDict[c.Id].Values
                                         .Select(t => new IncomeAccountDetailOutput
                                         {
                                             AccountCode = t.AccountCode,
                                             AccountName = t.AccountName,
                                             Id = t.AccountId,
                                             Total = t.Balance,
                                             TotalTemp = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalAmount : 0,
                                             TotalLocationColumns = t.TotalLocationColumns
                                         }).OrderBy(x => x.AccountCode).ToList() : new List<IncomeAccountDetailOutput>(),
                             LocationSummaryHeader = monthly,
                             TotalLocationSummaryByAccountType = accountDict.ContainsKey(c.Id) ? accountDict[c.Id].TotalLocationColumns : null,

                         }).OrderBy(t => t.AccountType).ToList();

            var result = ObjectMapper.Map<List<GetListIncomeReportOutput>>(query);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Income)]
        public async Task<List<GetListIncomeReportOutput>> GetIncomeReport(GetIncomeInput input)
        {
            return input.ViewOption == ViewOption.Month ? await this.GetProfitLossByMonthly(input) :
                                       input.ViewOption == ViewOption.Location ? await this.GetProfitLossByLocation(input) :
                                       input.ViewOption == ViewOption.Class ? await this.GetProfitLossByClass(input) :
                                       await this.GetProfitLossStandard(input);

        }

        public ReportOutput GetReportTemplateIncome()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ViewOption",
                        ColumnLength = 150,
                        ColumnTitle = "View Option",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = false,
                        IsDisplay = false,
                        DefaultValue = ((int)ViewOption.Standard).ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Account",
                        ColumnLength = 400,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = false,
                        DisableDefault= true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LocationPercentage",
                        ColumnLength = 150,
                        ColumnTitle = "Location %",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = false,
                        DisableDefault= true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 150,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.Money,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PercentTag",
                        ColumnLength = 110,
                        ColumnTitle = "%",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                },
                Groupby = "",
                HeaderTitle = "Profit & Loss Report",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "Ledger",
                    ColumnTitle = "AccountLedgerTemplate",
                    DefaultValue = ReportType.ReportType_Ledger
                }
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Income_Export_Excel)]
        public async Task<FileDto> ExportExcelIncomeReport(GetIncomeReportInput input)
        {
            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var rounding = await GetCurrentCycleAsync();
            var incomeData = await GetIncomeReport(input);

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);

                ws.PrinterSettings.FitToPage = true;
                ws.PrinterSettings.PaperSize = ePaperSize.A4; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);

                #endregion Row 1

                #region Row 2 Header
                var header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                int ih = 0;
                var amountToDivide = input.IsHasSubHeader ? 2 : 1;
                foreach (var i in reportCollumnHeader)
                {
                    if ((input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class) && input.IsHasSubHeader && i.ColumnName != "Account"
                                    && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                    {
                        var colWidth = i.ColumnLength;
                        var subColWidth = i.ColumnLength / amountToDivide;

                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(colWidth);
                        MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader, colHeaderTable + amountToDivide - 1, ExcelHorizontalAlignment.Center);

                        if (input.IsHasSubHeader)
                        {
                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, "Total", true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(subColWidth);
                            colHeaderTable += 1;

                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, "%", true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(subColWidth);
                            colHeaderTable += 1;
                            reportCountColHead++;
                        }
                    }
                    else
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                        if (input.IsHasSubHeader) MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader + 1, colHeaderTable, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center);

                        colHeaderTable += 1;
                    }
                    ih += 1;

                }

                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1 + (input.IsHasSubHeader ? 1 : 0);//start from row header of spreadsheet
                var totalIncome = incomeData.Where(t => t.AccountType == TypeOfAccount.Income).Sum(t => t.TotalAmount);
                var totalCOGS = incomeData.Where(t => t.AccountType == TypeOfAccount.COGS).Sum(t => t.TotalAmount);
                var totalExpense = incomeData.Where(t => t.AccountType == TypeOfAccount.Expense).Sum(t => t.TotalAmount);
                var grossProfit = totalIncome - totalCOGS;
                var netProfitLoss = grossProfit - totalExpense;

                // write body
                var incomeAndCogs = incomeData.Where(a => a.AccountType == TypeOfAccount.Income || a.AccountType == TypeOfAccount.COGS).ToList();
                //Title of Asset, Liaibility
                int collumnCellBody = 1;
                AddTextToCell(ws, rowBody, collumnCellBody, L("ProfitAndLoss"), true);
                rowBody += 1;

                var totalIncomeLocationColumns = new Dictionary<string, decimal>();
                var totalCOGSLocationColumns = new Dictionary<string, decimal>();
                var grossProfitTotalColumns = new Dictionary<string, decimal>();
                var totalExpenseLocationColumns = new Dictionary<string, decimal>();
                var totalNetProfitLossLocationColumns = new Dictionary<string, decimal>();
                if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                {
                    var incomeList = incomeData.SelectMany(s => s.AccountLists.Select(t => new { AccountType = s.AccountType, Account = t })).ToList();

                    foreach (var item in incomeList)
                    {
                        foreach (var l in reportCollumnHeader)
                        {
                            if (l.ColumnName != "Account" && l.ColumnName != "Total" && l.ColumnName != "PercentTag")
                            {
                                var ColumnName = Int32.Parse(l.ColumnName);
                                if (item.AccountType == TypeOfAccount.Income)
                                {
                                    if (totalIncomeLocationColumns.ContainsKey(l.ColumnName))
                                    {
                                        totalIncomeLocationColumns[l.ColumnName] += item.Account.TotalLocationColumns[ColumnName].Total;
                                    }
                                    else
                                    {
                                        totalIncomeLocationColumns.Add(l.ColumnName, item.Account.TotalLocationColumns[ColumnName].Total);
                                    }
                                }
                                else if (item.AccountType == TypeOfAccount.COGS)
                                {
                                    if (totalCOGSLocationColumns.ContainsKey(l.ColumnName))
                                    {
                                        totalCOGSLocationColumns[l.ColumnName] += item.Account.TotalLocationColumns[ColumnName].Total;
                                    }
                                    else
                                    {
                                        totalCOGSLocationColumns.Add(l.ColumnName, item.Account.TotalLocationColumns[ColumnName].Total);
                                    }
                                }
                                else if (item.AccountType == TypeOfAccount.Expense)
                                {
                                    if (totalExpenseLocationColumns.ContainsKey(l.ColumnName))
                                    {
                                        totalExpenseLocationColumns[l.ColumnName] += item.Account.TotalLocationColumns[ColumnName].Total;
                                    }
                                    else
                                    {
                                        totalExpenseLocationColumns.Add(l.ColumnName, item.Account.TotalLocationColumns[ColumnName].Total);
                                    }
                                }

                                // Gross Profit
                                var total = item.AccountType == TypeOfAccount.Income ? item.Account.TotalLocationColumns[ColumnName].Total : item.Account.TotalLocationColumns[ColumnName].Total * -1;
                                if (item.AccountType == TypeOfAccount.COGS || item.AccountType == TypeOfAccount.Income)
                                {
                                    if (grossProfitTotalColumns.ContainsKey(l.ColumnName))
                                    {
                                        grossProfitTotalColumns[l.ColumnName] += total;
                                    }
                                    else
                                    {
                                        grossProfitTotalColumns.Add(l.ColumnName, total);
                                    }
                                }
                                //Net income
                                if (totalNetProfitLossLocationColumns.ContainsKey(l.ColumnName))
                                {
                                    totalNetProfitLossLocationColumns[l.ColumnName] += total;
                                }
                                else
                                {
                                    totalNetProfitLossLocationColumns.Add(l.ColumnName, total);
                                }
                            }
                        }
                    }
                }

                //****************** write income and cogs row ******************//

                foreach (var i in incomeAndCogs)
                {
                    if (i.AccountLists.Count > 0)
                    {
                        AddTextToCell(ws, rowBody, collumnCellBody​, i.AccountTypeName, true, 5);
                        decimal sumpercent = 0;
                        foreach (var item in i.AccountLists)
                        {
                            int colItem = 1;
                            rowBody += 1;

                            decimal percent = 0;
                            if (totalIncome == 0)
                            {
                                percent = 0;
                            }
                            else
                            {
                                percent = (item.Total / totalIncome) * 100;
                            }
                            foreach (var h in reportCollumnHeader)
                            {
                                if (h.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, colItem, item.AccountCode + " - " + item.AccountName, false, 10);
                                }
                                else if (h.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colItem, item.Total, false, false, false, rounding.RoundingDigit);
                                }
                                else if (h.ColumnName == "PercentTag")
                                {
                                    AddNumberToCell(ws, rowBody, colItem, percent, false, false, true, rounding.RoundingDigit);
                                }
                                else
                                {
                                    if (item.TotalLocationColumns != null)
                                    {
                                        var ColumnName = Int32.Parse(h.ColumnName);
                                        decimal locationPercentage = 0;
                                        var locationTotal = totalIncomeLocationColumns[h.ColumnName];
                                        if (locationTotal != 0)
                                        {
                                            locationPercentage = (item.TotalLocationColumns[ColumnName].Total / locationTotal) * 100;
                                        }
                                        AddNumberToCell(ws, rowBody, colItem, item.TotalLocationColumns[ColumnName].Total, false, false, false, rounding.RoundingDigit);
                                        if (input.IsHasSubHeader)
                                        {
                                            colItem += 1;
                                            AddNumberToCell(ws, rowBody, colItem, locationPercentage, false, false, true, rounding.RoundingDigit);
                                        }
                                    }
                                }
                                colItem += 1;
                            }

                            sumpercent += percent;
                        }
                        rowBody += 1;
                        AddTextToCell(ws, rowBody, collumnCellBody​, L("Total") + " " + i.AccountTypeName, true, 5);


                        var revenueTotalIndex = 1;
                        foreach (var h in reportCollumnHeader)
                        {
                            if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                            {
                                if (h.ColumnName != "Account" && h.ColumnName != "Total" && h.ColumnName != "PercentTag")
                                {
                                    decimal locationPercentage = 0;
                                    var columnName = Int32.Parse(h.ColumnName);
                                    var locationTotal = totalIncomeLocationColumns.ContainsKey(h.ColumnName) ? totalIncomeLocationColumns[h.ColumnName] : 0;

                                    var totalIncomeOrCogsByAccountType = i.TotalLocationSummaryByAccountType.ContainsKey(columnName) ? i.TotalLocationSummaryByAccountType[columnName].Total : 0;
                                    if (locationTotal != 0)
                                    {
                                        locationPercentage = totalIncomeOrCogsByAccountType / locationTotal * 100;
                                    }
                                    AddNumberToCell(ws, rowBody, revenueTotalIndex, totalIncomeOrCogsByAccountType, false, false, false, rounding.RoundingDigit);

                                    if (input.IsHasSubHeader)
                                    {
                                        revenueTotalIndex++;
                                        AddNumberToCell(ws, rowBody, revenueTotalIndex, locationPercentage, false, false, true, rounding.RoundingDigit);

                                    }
                                }
                            }


                            if (h.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, revenueTotalIndex, i.TotalAmount, false, false, false, rounding.RoundingDigit);
                            }
                            else if (h.ColumnName == "PercentTag")
                            {
                                AddNumberToCell(ws, rowBody, revenueTotalIndex, sumpercent, false, false, true, rounding.RoundingDigit);
                            }
                            revenueTotalIndex++;
                        }

                        // Formula Sub Total of Account Type
                        //var fromCellAT = GetAddressName(ws, rowBody - i.AccountLists.Count(), collumnCellBody​ + 1);
                        //var toCellAT = GetAddressName(ws, rowBody - 1, collumnCellBody​ + 1);

                        //AddFormula(ws, rowBody, collumnCellBody​ + 1, "SUM(" + fromCellAT + ":" + toCellAT + ")", true);

                        //// get current address to auto formula of gross profit 
                        //grossProfitCellAddress += grossProfitCellAddress.Count() > 0 ? i.AccountType == TypeOfAccount.COGS ? "-" : "+" : "";
                        //grossProfitCellAddress += GetAddressName(ws, rowBody, collumnCellBody​ + 1);

                        ////get push income cell address to auto formula
                        //if (i.AccountType == TypeOfAccount.Income)
                        //{
                        //    incomeCellAddress += incomeCellAddress.Count() > 0 ? "+" + GetAddressName(ws, rowBody, collumnCellBody​ + 1) : GetAddressName(ws, rowBody, collumnCellBody​ + 1);
                        //}
                        rowBody += 1;
                    }

                }
                decimal p_GrossProfit = 0;
                decimal p_NetProfit = 0;
                decimal p_Expence = 0;
                if (grossProfit != 0 && totalIncome != 0)
                {
                    p_GrossProfit = (grossProfit / totalIncome) * 100;
                }
                if (netProfitLoss != 0 && totalIncome != 0)
                {
                    p_NetProfit = (netProfitLoss / totalIncome) * 100;
                }
                if (totalExpense != 0 && totalIncome != 0)
                {
                    p_Expence = (totalExpense / totalIncome) * 100;
                }


                AddTextToCell(ws, rowBody, collumnCellBody, L("GrossProfit"), true);
                var grossProfitIndex = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                    {
                        if (i.ColumnName != "Account" && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                        {
                            decimal locationPercentage = 0;
                            var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;
                            if (locationTotal != 0)
                            {
                                locationPercentage = (grossProfitTotalColumns[i.ColumnName] / locationTotal) * 100;
                            }
                            AddNumberToCell(ws, rowBody, grossProfitIndex, grossProfitTotalColumns.ContainsKey(i.ColumnName) ? grossProfitTotalColumns[i.ColumnName] : 0, false, false, false, rounding.RoundingDigit);
                            if (input.IsHasSubHeader)
                            {
                                grossProfitIndex++;
                                AddNumberToCell(ws, rowBody, grossProfitIndex, locationPercentage, false, false, true, rounding.RoundingDigit);
                            }
                        }
                    }
                    if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, grossProfitIndex, grossProfit, false, false, false, rounding.RoundingDigit);
                    }
                    else if (i.ColumnName == "PercentTag")
                    {
                        AddNumberToCell(ws, rowBody, grossProfitIndex, p_GrossProfit, false, false, true, rounding.RoundingDigit);
                    }
                    grossProfitIndex++;
                }
                //AddFormula(ws, rowBody, collumnCellBody​ + 1, "SUM(" + grossProfitCellAddress + ")", true);
                //// map get cell address for formula of net profit 
                //netProfitAndLossCellAddress += netProfitAndLossCellAddress.Count() > 0 ? GetAddressName(ws, rowBody, collumnCellBody​ + 1) + "-" : GetAddressName(ws, rowBody, collumnCellBody​ + 1);

                //if (reportCountColHead > 2)
                //{


                //    var cellGrossProfit = GetAddressName(ws, rowBody, 2);
                //    if (string.IsNullOrEmpty(incomeCellAddress))
                //    {
                //        AddFormula(ws, rowBodyOfPercentTag, 3, "0", false, false, true);
                //    }
                //    else
                //    {
                //        var formulaGrossProfit = cellGrossProfit + "/(" + incomeCellAddress + ")*100";
                //        AddFormula(ws, rowBody, 3, formulaGrossProfit, true, true);
                //    }

                //}
                rowBody += 1;

                //******************* write expense row ******************//
                var expense = incomeData.Where(a => a.AccountType == TypeOfAccount.Expense).ToList();
                foreach (var i in expense)
                {
                    if (i.AccountLists.Count > 0)
                    {
                        AddTextToCell(ws, rowBody, collumnCellBody​, i.AccountTypeName, true, 5);

                        rowBody += 1;
                        decimal sumPercent = 0;
                        foreach (var item in i.AccountLists)
                        {
                            var percent = totalIncome == 0 ? 0 : (item.Total / totalIncome) * 100;
                            int colItem = 1;
                            foreach (var h in reportCollumnHeader)
                            {
                                if (h.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, colItem, item.AccountCode + " - " + item.AccountName, false, 10);
                                }
                                else if (h.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colItem, item.Total, false, false, false, rounding.RoundingDigit);
                                }
                                else if (h.ColumnName == "PercentTag")
                                {
                                    AddNumberToCell(ws, rowBody, colItem, percent, false, false, true, rounding.RoundingDigit);
                                }
                                else
                                {
                                    if (item.TotalLocationColumns != null)
                                    {
                                        var ColumnName = Int32.Parse(h.ColumnName);
                                        decimal locationPercentage = 0;
                                        var locationTotal = totalIncomeLocationColumns.ContainsKey(h.ColumnName) ? totalIncomeLocationColumns[h.ColumnName] : 0;

                                        var totalLocationExpense = item.TotalLocationColumns.ContainsKey(ColumnName) ? item.TotalLocationColumns[ColumnName].Total : 0;
                                        if (locationTotal != 0)
                                        {
                                            locationPercentage = (totalLocationExpense / locationTotal) * 100;
                                        }
                                        AddNumberToCell(ws, rowBody, colItem, totalLocationExpense, false, false, false, rounding.RoundingDigit);
                                        if (input.IsHasSubHeader)
                                        {
                                            colItem += 1;
                                            AddNumberToCell(ws, rowBody, colItem, locationPercentage, false, false, true, rounding.RoundingDigit);
                                        }
                                    }
                                }
                                colItem += 1;
                            }
                            sumPercent += percent;

                            rowBody += 1;
                        }
                        AddTextToCell(ws, rowBody, collumnCellBody​, L("Total") + " " + i.AccountTypeName, true, 5);

                        var expenseTotalIndex = 1;
                        foreach (var h in reportCollumnHeader)
                        {
                            if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                            {
                                if (h.ColumnName != "Account" && h.ColumnName != "Total" && h.ColumnName != "PercentTag")
                                {
                                    decimal locationPercentage = 0;
                                    var locationTotal = totalIncomeLocationColumns.ContainsKey(h.ColumnName) ? totalIncomeLocationColumns[h.ColumnName] : 0;

                                    var columnname = Int32.Parse(h.ColumnName);
                                    var totalExpenseByAccountType = i.TotalLocationSummaryByAccountType.ContainsKey(columnname) ? i.TotalLocationSummaryByAccountType[columnname].Total : 0;
                                    if (locationTotal != 0)
                                    {
                                        locationPercentage = (totalExpenseByAccountType / locationTotal) * 100;
                                    }
                                    AddNumberToCell(ws, rowBody, expenseTotalIndex, totalExpenseByAccountType, false, false, false, rounding.RoundingDigit);
                                    if (input.IsHasSubHeader)
                                    {
                                        expenseTotalIndex++;
                                        AddNumberToCell(ws, rowBody, expenseTotalIndex, locationPercentage, false, false, true, rounding.RoundingDigit);
                                    }
                                }
                            }
                            if (h.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, expenseTotalIndex, i.TotalAmount, false, false, false, rounding.RoundingDigit);
                            }
                            else if (h.ColumnName == "PercentTag")
                            {
                                AddNumberToCell(ws, rowBody, expenseTotalIndex, sumPercent, false, false, true, rounding.RoundingDigit);
                            }
                            expenseTotalIndex++;
                        }
                        rowBody += 1;
                    }
                }

                AddTextToCell(ws, rowBody, collumnCellBody, L("NetProfitAndLoss"), true);
                var grossColIndex = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                    {
                        if (i.ColumnName != "Account" && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                        {
                            decimal locationPercentage = 0;
                            var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;
                            if (locationTotal != 0)
                            {
                                locationPercentage = (totalNetProfitLossLocationColumns[i.ColumnName] / locationTotal) * 100;
                            }
                            AddNumberToCell(ws, rowBody, grossColIndex, totalNetProfitLossLocationColumns.ContainsKey(i.ColumnName) ? totalNetProfitLossLocationColumns[i.ColumnName] : 0, false, false, false, rounding.RoundingDigit);
                            if (input.IsHasSubHeader)
                            {
                                grossColIndex++;
                                AddNumberToCell(ws, rowBody, grossColIndex, locationPercentage, false, false, true, rounding.RoundingDigit);
                            }
                        }
                    }

                    if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, grossColIndex, netProfitLoss, false, false, false, rounding.RoundingDigit);
                    }
                    else if (i.ColumnName == "PercentTag")
                    {
                        AddNumberToCell(ws, rowBody, grossColIndex, p_NetProfit, false, false, true, rounding.RoundingDigit);

                    }
                    grossColIndex++;


                }

                //AddFormula(ws, rowBody, collumnCellBody​ + 1, "SUM(" + netProfitAndLossCellAddress + ")", true);

                //if (reportCountColHead > 2)
                //{//net profit & loss percent tag

                //    var cellNetGrossProfit = GetAddressName(ws, rowBody, 2);
                //    if (string.IsNullOrEmpty(incomeCellAddress))
                //    {
                //        AddFormula(ws, rowBodyOfPercentTag, 3, "0", false, true, true);
                //    }
                //    else
                //    {
                //        var formulaNetGrossProfit = cellNetGrossProfit + "/(" + incomeCellAddress + ")*100";
                //        AddFormula(ws, rowBody, 3, formulaNetGrossProfit, true, true);
                //    }

                //}
                #endregion Row Body


                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.xlsx";
                result.FileToken = Guid.NewGuid().ToString();
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Income_Export_Pdf)]
        public async Task<FileDto> ExportPdfIncomeReport(GetIncomeReportInput input)
        {

            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                             .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                             .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;
            var user = await GetCurrentUserAsync();

            var incomes = await GetIncomeReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "BalanceSheet.pdf",
                    FileToken = "BalanceSheet.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;
                //@todo replace our variable 

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);

                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                int colSpan = 0;
                var subHeader = (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class) && input.IsHasSubHeader ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";
                            if ((input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class) && input.IsHasSubHeader && i.ColumnName != "Account"
                                     && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                            {

                                var subColWidth = Convert.ToInt32(i.ColumnLength / 2);
                                rowHeader = $"<th  colspan='2' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                                colSpan++;
                                // add sub header tr
                                subHeader += $"<th style='width: {subColWidth}px;'>Total</th>";
                                subHeader += $"<th style='width: {subColWidth}px;'>%</th>";
                            }
                            else
                            {
                                rowHeader = $"<th rowspan='2' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                            }
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                            colSpan++;

                            //var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            //contentHeader += rowHeader;
                            //totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());
                subHeader += (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class) && input.IsHasSubHeader ? "</tr>" : "";
                #region Row Body 
                var totalIncome = incomes.Where(t => t.AccountType == TypeOfAccount.Income).Sum(t => t.TotalAmount);
                var totalCOGS = incomes.Where(t => t.AccountType == TypeOfAccount.COGS).Sum(t => t.TotalAmount);
                var totalExpense = incomes.Where(t => t.AccountType == TypeOfAccount.Expense).Sum(t => t.TotalAmount);

                var grossProfit = totalIncome - totalCOGS;
                var netProfitLoss = grossProfit - totalExpense;

                var totalIncomeLocationColumns = new Dictionary<string, decimal>();
                var totalCOGSLocationColumns = new Dictionary<string, decimal>();
                var grossProfitTotalColumns = new Dictionary<string, decimal>();
                var totalExpenseLocationColumns = new Dictionary<string, decimal>();
                var totalNetProfitLossLocationColumns = new Dictionary<string, decimal>();
                if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                {
                    var incomeList = incomes.SelectMany(s => s.AccountLists.Select(t => new { AccountType = s.AccountType, Account = t })).ToList();

                    foreach (var item in incomeList)
                    {
                        foreach (var l in viewHeader)
                        {
                            if (l.ColumnName != "Account" && l.ColumnName != "Total" && l.ColumnName != "PercentTag")
                            {
                                var columnName = Int32.Parse(l.ColumnName);
                                if (item.AccountType == TypeOfAccount.Income)
                                {
                                    if (totalIncomeLocationColumns.ContainsKey(l.ColumnName))
                                    {
                                        totalIncomeLocationColumns[l.ColumnName] += item.Account.TotalLocationColumns[columnName].Total;
                                    }
                                    else
                                    {
                                        totalIncomeLocationColumns.Add(l.ColumnName, item.Account.TotalLocationColumns[columnName].Total);
                                    }
                                }
                                else if (item.AccountType == TypeOfAccount.COGS)
                                {
                                    if (totalCOGSLocationColumns.ContainsKey(l.ColumnName))
                                    {
                                        totalCOGSLocationColumns[l.ColumnName] += item.Account.TotalLocationColumns[columnName].Total;
                                    }
                                    else
                                    {
                                        totalCOGSLocationColumns.Add(l.ColumnName, item.Account.TotalLocationColumns[columnName].Total);
                                    }
                                }
                                else if (item.AccountType == TypeOfAccount.Expense)
                                {
                                    if (totalExpenseLocationColumns.ContainsKey(l.ColumnName))
                                    {
                                        totalExpenseLocationColumns[l.ColumnName] += item.Account.TotalLocationColumns[columnName].Total;
                                    }
                                    else
                                    {
                                        totalExpenseLocationColumns.Add(l.ColumnName, item.Account.TotalLocationColumns[columnName].Total);
                                    }
                                }

                                // Gross Profit
                                var total = item.AccountType == TypeOfAccount.Income ? item.Account.TotalLocationColumns[columnName].Total : item.Account.TotalLocationColumns[columnName].Total * -1;
                                if (item.AccountType == TypeOfAccount.COGS || item.AccountType == TypeOfAccount.Income)
                                {
                                    if (grossProfitTotalColumns.ContainsKey(l.ColumnName))
                                    {
                                        grossProfitTotalColumns[l.ColumnName] += total;
                                    }
                                    else
                                    {
                                        grossProfitTotalColumns.Add(l.ColumnName, total);
                                    }
                                }
                                //Net income
                                if (totalNetProfitLossLocationColumns.ContainsKey(l.ColumnName))
                                {
                                    totalNetProfitLossLocationColumns[l.ColumnName] += total;
                                }
                                else
                                {
                                    totalNetProfitLossLocationColumns.Add(l.ColumnName, total);
                                }
                            }
                        }
                    }
                }


                // write body
                var incomeAndCogs = incomes.Where(a => a.AccountType == TypeOfAccount.Income || a.AccountType == TypeOfAccount.COGS).ToList();

                var header = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold'>";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            header += $"<td>{L("ProfitAndLoss")}</td>";
                        }
                        else
                        {
                            header += $"<td></td>";
                        }
                    }
                }
                header += "</tr>";
                contentBody += header;
                foreach (var bs in incomeAndCogs)
                {
                    if (bs.AccountLists.Count > 0)
                    {
                        //var headerAccount = $"<tr valign='top' style='page-break-before: auto; page-break-after: auto;font-weight: bold'>" +
                        //                $"<td colspan={colSpan}> <div style='margin-left:30px' >" + bs.AccountTypeName + 
                        //                $"</div></td></tr>";
                        var headerAccount = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    headerAccount += $"<td><div style='margin-left:30px'>{bs.AccountTypeName}</div></td>";
                                }
                                else
                                {
                                    headerAccount += $"<td></td>";
                                }
                            }
                        }
                        headerAccount += "</tr>";
                        contentBody += headerAccount;
                        decimal sumpercent = 0;
                        foreach (var item in bs.AccountLists)
                        {
                            decimal percent = 0;
                            if (totalIncome == 0)
                            {
                                percent = 0;
                            }
                            else
                            {
                                percent = (item.Total / totalIncome) * 100;
                            }
                            var subitem = $"<tr valign='top' style='page-break-before: auto; page-break-after: auto;'>";

                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "Account")
                                    {
                                        subitem += $"<td><div style='margin-left:60px'> {item.AccountCode + "-" + item.AccountName}</div></td>";
                                    }
                                    else if (i.ColumnName == "Total")
                                    {
                                        subitem += $"<td style='text-align: right;padding-right:8px;'>{FormatNumberCurrency(Math.Round(item.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "PercentTag")
                                    {
                                        subitem += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(percent, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                    }
                                    else
                                    {
                                        if (item.TotalLocationColumns != null)
                                        {
                                            var ColumnName = Int32.Parse(i.ColumnName);
                                            decimal locationPercentage = 0;
                                            var locationTotal = totalIncomeLocationColumns[i.ColumnName];
                                            if (locationTotal != 0)
                                            {
                                                locationPercentage = (item.TotalLocationColumns[ColumnName].Total / locationTotal) * 100;
                                            }

                                            subitem += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(item.TotalLocationColumns[ColumnName].Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                            if (input.IsHasSubHeader)
                                            {
                                                subitem += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(locationPercentage, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                            }
                                        }
                                    }
                                }
                            }
                            subitem += "</tr>";
                            contentBody += subitem;
                            sumpercent += percent;


                        }

                        var total = $"<tr valign='top' style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>" +
                                    $"<td><div style='margin-left:30px'> {L("Total") + " " + bs.AccountTypeName}</div></td>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                                {
                                    if (i.ColumnName != "Account" && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                                    {
                                        decimal locationPercentage = 0;
                                        var columnName = Int32.Parse(i.ColumnName);
                                        var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;

                                        var totalIncomeOrCogsByAccountType = bs.TotalLocationSummaryByAccountType.ContainsKey(columnName) ? bs.TotalLocationSummaryByAccountType[columnName].Total : 0;
                                        if (locationTotal != 0)
                                        {
                                            locationPercentage = totalIncomeOrCogsByAccountType / locationTotal * 100;
                                        }

                                        total += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(totalIncomeOrCogsByAccountType, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        if (input.IsHasSubHeader)
                                        {
                                            total += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(locationPercentage, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                        }
                                    }
                                }


                                if (i.ColumnName == "Total")
                                {
                                    total += $"<td style='text-align: right;padding-right:8px;'>{FormatNumberCurrency(Math.Round(bs.TotalAmount, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "PercentTag")
                                {
                                    total += $"<td style='text-align: right;padding-right:8px;'>{FormatNumberCurrency(Math.Round(sumpercent, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                }

                            }
                        }

                        total += $"</tr>";
                        contentBody += total;
                    }
                }
                decimal p_GrossProfit = 0;
                decimal p_NetProfit = 0;
                decimal p_Expence = 0;
                if (grossProfit != 0 && totalIncome != 0)
                {
                    p_GrossProfit = (grossProfit / totalIncome) * 100;
                }
                if (netProfitLoss != 0 && totalIncome != 0)
                {
                    p_NetProfit = (netProfitLoss / totalIncome) * 100;
                }
                if (totalExpense != 0 && totalIncome != 0)
                {
                    p_Expence = (totalExpense / totalIncome) * 100;
                }

                var GrossProfit = $"<tr valign='top' style='page-break-before: auto; page-break-after: auto;font-weight: bold'><td>" + L("GrossProfit") + "</td> ";

                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                        {
                            if (i.ColumnName != "Account" && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                            {
                                decimal locationPercentage = 0;
                                var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;
                                if (locationTotal != 0)
                                {
                                    locationPercentage = (grossProfitTotalColumns[i.ColumnName] / locationTotal) * 100;
                                }
                                GrossProfit += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(grossProfitTotalColumns.ContainsKey(i.ColumnName) ? grossProfitTotalColumns[i.ColumnName] : 0, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                if (input.IsHasSubHeader)
                                {
                                    GrossProfit += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(locationPercentage, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                }
                            }
                        }
                        if (i.ColumnName == "Total")
                        {
                            GrossProfit += $"<td style='text-align: right;padding-right:8px;'>" + FormatNumberCurrency(Math.Round(grossProfit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "</td>";
                        }
                        else if (i.ColumnName == "PercentTag")
                        {
                            GrossProfit += $"<td style='text-align: right;padding-right:8px;'>" + FormatNumberCurrency(Math.Round(p_GrossProfit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + " % " + " </td>";
                        }
                    }
                }

                GrossProfit += "</tr>";
                contentBody += GrossProfit;

                //******************* write expense row ******************//
                var expense = incomes.Where(a => a.AccountType == TypeOfAccount.Expense).ToList();
                foreach (var bs in expense)
                {
                    if (bs.AccountLists.Count > 0)
                    {
                        var headerAccount = $"<tr valign='top' style='page-break-before: auto; page-break-after: auto;font-weight: bold'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    headerAccount += $"<td><div style='margin-left:30px'>{bs.AccountTypeName}</div></td>";
                                }
                                else
                                {
                                    headerAccount += $"<td></td>";
                                }
                            }
                        }
                        headerAccount += "</tr>";
                        contentBody += headerAccount;
                        decimal sumPercent = 0;
                        foreach (var item in bs.AccountLists)
                        {

                            var percent = totalIncome == 0 ? 0 : (item.Total / totalIncome) * 100;
                            //var subitem = $"<tr>" +
                            //                $"<td><span style='margin-left:60px'> {item.AccountCode + "-" + item.AccountName}</span></td>" +
                            //                $"<td style='text-align: right;padding-right:8px;'>{FormatNumberCurrency(Math.Round(item.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>" +
                            //                $"<td> {FormatNumberCurrency(Math.Round(percent, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>" +
                            //                $"</tr>";

                            var subitem = $"<tr valign='top' style='page-break-before: auto; page-break-after: auto;'>";

                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "Account")
                                    {
                                        subitem += $"<td><div style='margin-left:60px'> {item.AccountCode + "-" + item.AccountName}</div></td>";
                                    }
                                    else if (i.ColumnName == "Total")
                                    {
                                        subitem += $"<td style='text-align: right;padding-right:8px;'>{FormatNumberCurrency(Math.Round(item.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "PercentTag")
                                    {
                                        subitem += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(percent, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                    }
                                    else
                                    {
                                        if (item.TotalLocationColumns != null)
                                        {
                                            var ColumnName = Int32.Parse(i.ColumnName);
                                            decimal locationPercentage = 0;
                                            var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;
                                            if (locationTotal != 0)
                                            {
                                                locationPercentage = (item.TotalLocationColumns[ColumnName].Total / locationTotal) * 100;
                                            }
                                            subitem += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(item.TotalLocationColumns.ContainsKey(ColumnName) ? item.TotalLocationColumns[ColumnName].Total : 0, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                            if (input.IsHasSubHeader)
                                            {
                                                subitem += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(locationPercentage, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                            }
                                        }
                                    }
                                }
                            }
                            subitem += "</tr>";
                            contentBody += subitem;
                            sumPercent += percent;

                        }

                        var total = $"<tr valign='top' style='font-weight: bold'>" +
                                    $"<td><div style='margin-left:30px'> {L("Total") + " " + bs.AccountTypeName}</div></td>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                                {
                                    if (i.ColumnName != "Account" && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                                    {
                                        decimal locationPercentage = 0;
                                        var columnName = Int32.Parse(i.ColumnName);
                                        var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;

                                        var totalExpenseByAccountType = bs.TotalLocationSummaryByAccountType.ContainsKey(columnName) ? bs.TotalLocationSummaryByAccountType[columnName].Total : 0;
                                        if (locationTotal != 0)
                                        {
                                            locationPercentage = totalExpenseByAccountType / locationTotal * 100;
                                        }

                                        total += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(totalExpenseByAccountType, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        if (input.IsHasSubHeader)
                                        {
                                            total += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(locationPercentage, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                        }
                                    }

                                }
                                if (i.ColumnName == "Total")
                                {
                                    total += $"<td style='text-align: right;padding-right:8px;'>{FormatNumberCurrency(Math.Round(bs.TotalAmount, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "PercentTag")
                                {
                                    total += $"<td style='text-align: right;padding-right:8px;'>{ FormatNumberCurrency(Math.Round(sumPercent, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td> ";
                                }
                            }
                        }

                        total += $"</tr>";
                        contentBody += total;
                    }

                }

                var NetProfit = "<tr valign='top' style='font-weight: bold'><td>" + L("NetProfitAndLoss") + "</td> ";

                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {

                        if (input.ViewOption == ViewOption.Location || input.ViewOption == ViewOption.Month || input.ViewOption == ViewOption.Class)
                        {
                            if (i.ColumnName != "Account" && i.ColumnName != "Total" && i.ColumnName != "PercentTag")
                            {
                                decimal locationPercentage = 0;
                                var locationTotal = totalIncomeLocationColumns.ContainsKey(i.ColumnName) ? totalIncomeLocationColumns[i.ColumnName] : 0;
                                if (locationTotal != 0)
                                {
                                    locationPercentage = (totalNetProfitLossLocationColumns[i.ColumnName] / locationTotal) * 100;
                                }
                                NetProfit += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(totalNetProfitLossLocationColumns.ContainsKey(i.ColumnName) ? totalNetProfitLossLocationColumns[i.ColumnName] : 0, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                if (input.IsHasSubHeader)
                                {
                                    NetProfit += $"<td style='text-align: right;padding-right:8px;'> {FormatNumberCurrency(Math.Round(locationPercentage, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%"}</td>";
                                }
                            }
                        }

                        if (i.ColumnName == "Total")
                        {
                            NetProfit += "<td style='text-align: right;padding-right:8px;'>" + FormatNumberCurrency(Math.Round(netProfitLoss, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "</td>";
                        }
                        else if (i.ColumnName == "PercentTag")
                        {
                            NetProfit += "<td  style='text-align: right;padding-right:8px;'>" + FormatNumberCurrency(Math.Round(p_NetProfit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit) + "%" + "</td>";

                        }
                    }
                }


                NetProfit += " </tr>";
                contentBody += NetProfit;
                #endregion Row Body

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", subHeader);

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }
        #endregion


        #region Balance Sheet Report 

        [UnitOfWork(IsDisabled = true)]
        private async Task<List<MainAccountGroupHeader>> GetBalanceSheetStandardReport(GetBalanceSheetInput input)
        {
            var tenantId = AbpSession.TenantId;
            var balanceSheetHeaderList = new[] { TypeOfAccount.CurrentAsset, TypeOfAccount.FixedAsset, TypeOfAccount.CurrentLiability, TypeOfAccount.LongTermLiability, TypeOfAccount.Equity };
            var incomeStatementHeaderList = new[] { TypeOfAccount.COGS, TypeOfAccount.Income, TypeOfAccount.Expense };

            var accountOldPeriod = new List<AccountTransactionOutPut>();
            var accountNewPeriod = new List<AccountTransactionOutPut>();
            var accountTypes = new List<IGrouping<TypeOfAccount, AccountType>>();
            AccountCycle previousCycle;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    previousCycle = await GetPreviousRoundingCloseCyleAsync(input.ToDate);

                    accountOldPeriod = await _accountTransactionManager.GetAccountQuery(null, input.FromDate.AddDays(-1), false, input.LocationIds, true)
                                        .Select(u => new AccountTransactionOutPut
                                        {
                                            AccountId = u.AccountId,
                                            AccountName = u.AccountName,
                                            AccountCode = u.AccountCode,
                                            AccountTypeId = u.AccountTypeId,
                                            Type = u.Type,
                                            LocationId = u.LocationId,
                                            Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                                 u.Type == TypeOfAccount.FixedAsset ||
                                                 u.Type == TypeOfAccount.COGS ||
                                                 u.Type == TypeOfAccount.Expense ?
                                                 1 : -1) * u.Balance
                                        }).ToListAsync();

                    accountNewPeriod = await _accountTransactionManager.GetAccountQuery(input.FromDate, input.ToDate, false, input.LocationIds, true)
                            .Select(u => new AccountTransactionOutPut
                            {
                                AccountId = u.AccountId,
                                AccountName = u.AccountName,
                                AccountCode = u.AccountCode,
                                AccountTypeId = u.AccountTypeId,
                                Type = u.Type,
                                LocationId = u.LocationId,
                                Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                     u.Type == TypeOfAccount.FixedAsset ||
                                     u.Type == TypeOfAccount.COGS ||
                                     u.Type == TypeOfAccount.Expense ?
                                     1 : -1) * u.Balance
                            }).ToListAsync();

                    accountTypes = await _accountTypeRepository.GetAll().AsNoTracking()
                                            .Where(t => balanceSheetHeaderList.Contains(t.Type))
                                            .GroupBy(t => t.Type)
                                            .ToListAsync();

                }

            }

            var retainedEarning = accountOldPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .Select(u => (u.Type == TypeOfAccount.Income ? 1 : -1) * u.Balance)
                                    .Sum();

            var balanceSheetAccountOldPeriod = accountOldPeriod.Where(u => balanceSheetHeaderList.Contains(u.Type));
            var balanceSheetAccountNewPeriod = accountNewPeriod.Where(u => balanceSheetHeaderList.Contains(u.Type));


            var balanceSheetAccount = balanceSheetAccountOldPeriod
                                      .Union(balanceSheetAccountNewPeriod)
                                      .GroupBy(a => new { a.AccountCode, a.AccountId, a.AccountName, a.AccountTypeId })
                                        .Select(a => new
                                        {
                                            a.Key.AccountTypeId,
                                            a.Key.AccountName,
                                            a.Key.AccountCode,
                                            a.Key.AccountId,
                                            Balance = a.Sum(b => b.Balance)
                                        }).ToList();

            var balanceSheetAcountDict = balanceSheetAccount
                                        //.Where(u => u.Balance > 0)
                                        .GroupBy(u => u.AccountTypeId)
                                        .ToDictionary(u => u.Key);

            var profitLoss = accountNewPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .Select(u => (u.Type == TypeOfAccount.Income ? 1 : -1) * u.Balance)
                                    .Sum();

            var query = (from c in accountTypes
                         select new AccountGroupHeader
                         {
                             Name = Enum.GetName(typeof(TypeOfAccount), c.Key),
                             AccountLists = c.Select(a => new GetListBalanceSheetReportOutput()
                             {
                                 Id = a.Id,
                                 AccountTypeName = a.AccountTypeName,
                                 AccountType = a.Type,
                                 Items = balanceSheetAcountDict.ContainsKey(a.Id) ?
                                         balanceSheetAcountDict[a.Id]
                                         .Select(t => new AccountList
                                         {
                                             AccountCode = t.AccountCode,
                                             AccountName = t.AccountName,
                                             Id = t.AccountId,
                                             TotalAmount = t.Balance
                                         }).OrderBy(x => x.AccountCode).ToList() : new List<AccountList>(),
                                 RetainedEarning = c.Key == TypeOfAccount.Equity ? retainedEarning : (decimal?)null,
                                 NetIncome = c.Key == TypeOfAccount.Equity ? profitLoss : (decimal?)null,
                             }).ToList(),
                         }).ToList();


            var asset = new List<MainAccountGroupHeader>()
                {
                    new MainAccountGroupHeader()
                    {
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Asset",
                        TotalAmount = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset").Sum(t=>t.TotalAmount),
                        Items = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset")
                            .Select(t=> new AccountGroupHeader(){
                                Name = t.Name,
                                AccountLists = t.AccountLists,
                            }).ToList()
                    },
                };
            var LiabilityEquity = new List<MainAccountGroupHeader>()
            {

                    new MainAccountGroupHeader()
                    {
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Lialbility",
                        TotalAmount = query.Where(t=>t.Name == "CurrentLiability" || t.Name == "LongTermLiability").Sum(t=>t.TotalAmount),
                        Items = query.Where(t=>t.Name == "CurrentLiability" || t.Name == "LongTermLiability")
                            .Select(t=> new AccountGroupHeader(){
                                Name = t.Name,
                                AccountLists = t.AccountLists,
                            }).ToList()
                    },
                    new MainAccountGroupHeader()
                    {
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Equity",
                        TotalAmount = query.Where(t=>t.Name == "Equity").Sum(t => t.TotalAmount),
                        Items = query.Where(t=>t.Name == "Equity")
                            .Select(t=> new AccountGroupHeader(){
                                Name = t.Name,
                                AccountLists = t.AccountLists,
                            }).ToList()
                    }
            };


            var totalLiabilityEquity = new List<MainAccountGroupHeader>()
                {
                    new MainAccountGroupHeader()
                    {
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Liability + Equity",
                        Items = new List<AccountGroupHeader>(),
                        TotalAmount = LiabilityEquity.Sum(t=>t.TotalAmount)
                    },
                };

            var result = asset.Concat(LiabilityEquity).Concat(totalLiabilityEquity).ToList();
            return result;

        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<List<MainAccountGroupHeader>> GetBalanceSheetLocationReport(GetBalanceSheetInput input)
        {
            var tenantId = AbpSession.TenantId;
            var balanceSheetHeaderList = new[] { TypeOfAccount.CurrentAsset, TypeOfAccount.FixedAsset, TypeOfAccount.CurrentLiability, TypeOfAccount.LongTermLiability, TypeOfAccount.Equity };

            var incomeStatementHeaderList = new[] { TypeOfAccount.COGS, TypeOfAccount.Income, TypeOfAccount.Expense };

            var accountOldPeriod = new List<AccountTransactionOutPut>();
            var accountNewPeriod = new List<AccountTransactionOutPut>();
            var locationListDics = new Dictionary<long, string>();
            var accountTypes = new List<IGrouping<TypeOfAccount, AccountType>>();
            AccountCycle previousCycle;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    previousCycle = await GetPreviousRoundingCloseCyleAsync(input.ToDate);

                    accountOldPeriod = await _accountTransactionManager.GetAccountQuery(null, input.FromDate.AddDays(-1), false, input.LocationIds, true)
                                        .Select(u => new AccountTransactionOutPut
                                        {
                                            AccountId = u.AccountId,
                                            AccountName = u.AccountName,
                                            AccountCode = u.AccountCode,
                                            AccountTypeId = u.AccountTypeId,
                                            Type = u.Type,
                                            LocationId = u.LocationId,
                                            Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                                 u.Type == TypeOfAccount.FixedAsset ||
                                                 u.Type == TypeOfAccount.COGS ||
                                                 u.Type == TypeOfAccount.Expense ?
                                                 1 : -1) * u.Balance
                                        }).ToListAsync();

                    accountNewPeriod = await _accountTransactionManager.GetAccountQuery(input.FromDate, input.ToDate, false, input.LocationIds, true)
                            .Select(u => new AccountTransactionOutPut
                            {
                                AccountId = u.AccountId,
                                AccountName = u.AccountName,
                                AccountCode = u.AccountCode,
                                AccountTypeId = u.AccountTypeId,
                                Type = u.Type,
                                LocationId = u.LocationId,
                                Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                     u.Type == TypeOfAccount.FixedAsset ||
                                     u.Type == TypeOfAccount.COGS ||
                                     u.Type == TypeOfAccount.Expense ?
                                     1 : -1) * u.Balance,

                            }).ToListAsync();

                    locationListDics = await _locationsRepository.GetAll().AsNoTracking()
                               .ToDictionaryAsync(t => t.Id, t => t.LocationName);

                    accountTypes = await _accountTypeRepository.GetAll().AsNoTracking()
                                            .Where(t => balanceSheetHeaderList.Contains(t.Type))
                                            .GroupBy(t => t.Type)
                                            .ToListAsync();

                }

            }

            var balanceSheetAccountOldPeriod = accountOldPeriod.Where(u => balanceSheetHeaderList.Contains(u.Type));
            var balanceSheetAccountNewPeriod = accountNewPeriod.Where(u => balanceSheetHeaderList.Contains(u.Type));

            var queryResult = balanceSheetAccountOldPeriod.Concat(balanceSheetAccountNewPeriod);


            var locations = queryResult.GroupBy(s => s.LocationId)
                            .Select(s => new LocationSummaryOutput
                            {
                                Id = s.Key,
                                LocationName = locationListDics[s.Key]
                            }).OrderBy(t => t.LocationName).ToList();

            var balanceSheetAccount = queryResult.GroupBy(a => new { a.AccountCode, a.AccountId, a.AccountName, a.AccountTypeId })
                                                        .Select(a => new
                                                        {
                                                            a.Key.AccountTypeId,
                                                            a.Key.AccountName,
                                                            a.Key.AccountCode,
                                                            a.Key.AccountId,
                                                            //a.Key.LocationId,
                                                            Balance = a.Sum(b => b.Balance),
                                                            TotalLocationColumns = locations.Select(z => new TotalBalanceSheetLocationColumns
                                                            {
                                                                LocationName = z.LocationName,
                                                                LocationId = z.Id,
                                                                Total = a.Where(r => r.LocationId == z.Id).Sum(x => x.Balance)
                                                            }).ToDictionary(c => c.LocationId, c => c),

                                                        }).ToList();

            var balanceSheetAcountDict = balanceSheetAccount
                                        //.Where(u => u.Balance > 0)
                                        .GroupBy(u => u.AccountTypeId)
                                        .ToDictionary(u => u.Key);

            var retainedEarning = accountOldPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .Select(u => (u.Type == TypeOfAccount.Income ? 1 : -1) * u.Balance)
                                    .Sum();

            var retainedEarningTotalLocationCols = accountOldPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .GroupBy(x => x.LocationId)
                                    .Select(u => new TotalBalanceSheetLocationColumns
                                    {
                                        LocationId = u.Key,
                                        Total = u.Sum(x => (x.Type == TypeOfAccount.Income ? 1 : -1) * x.Balance)
                                    }).ToDictionary(c => c.LocationId, c => c);

            var retainedEarningTotalLocationColsResult = locationListDics.Select(x => new TotalBalanceSheetLocationColumns
            {
                LocationId = x.Key,
                Total = retainedEarningTotalLocationCols.ContainsKey(x.Key) ? retainedEarningTotalLocationCols[x.Key].Total : 0
            }).ToDictionary(c => c.LocationId, c => c);

            var profitLoss = accountNewPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .Select(u => (u.Type == TypeOfAccount.Income ? 1 : -1) * u.Balance)
                                    .Sum();

            var profitLossTotalLocationCols = accountNewPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .GroupBy(x => x.LocationId)
                                    .Select(u => new TotalBalanceSheetLocationColumns
                                    {
                                        LocationId = u.Key,
                                        Total = u.Sum(x => (x.Type == TypeOfAccount.Income ? 1 : -1) * x.Balance)
                                    }).ToDictionary(c => c.LocationId, c => c);

            var profitLossTotalLocationColsResult = locationListDics.Select(x => new TotalBalanceSheetLocationColumns
            {
                LocationId = x.Key,
                Total = profitLossTotalLocationCols.ContainsKey(x.Key) ? profitLossTotalLocationCols[x.Key].Total : 0
            }).ToDictionary(c => c.LocationId, c => c);


            var query = (from c in accountTypes
                         select new AccountGroupHeader
                         {
                             Name = Enum.GetName(typeof(TypeOfAccount), c.Key),
                             AccountLists = c.Select(a => new GetListBalanceSheetReportOutput()
                             {
                                 Id = a.Id,
                                 AccountTypeName = a.AccountTypeName,
                                 AccountType = a.Type,
                                 Items = balanceSheetAcountDict.ContainsKey(a.Id) ?
                                         balanceSheetAcountDict[a.Id]
                                         .Select(t => new AccountList
                                         {
                                             AccountCode = t.AccountCode,
                                             AccountName = t.AccountName,
                                             Id = t.AccountId,
                                             TotalAmount = t.Balance,
                                             TotalLocationColumns = t.TotalLocationColumns
                                         }).OrderBy(x => x.AccountCode).ToList() : new List<AccountList>(),
                                 RetainedEarning = c.Key == TypeOfAccount.Equity ? retainedEarning : (decimal?)null,
                                 RetainedEarningTotalLocationColumns = c.Key == TypeOfAccount.Equity ? retainedEarningTotalLocationColsResult : null,
                                 TotalLocationColumns = balanceSheetAcountDict.ContainsKey(a.Id) ?
                                         balanceSheetAcountDict[a.Id]
                                         .SelectMany(t => t.TotalLocationColumns)
                                         .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns
                                         {
                                             LocationId = key,
                                             LocationName = "",
                                             Total = value.Sum(x => x.Value.Total) +
                                                (retainedEarningTotalLocationColsResult.ContainsKey(key) && c.Key == TypeOfAccount.Equity ? retainedEarningTotalLocationColsResult[key].Total : 0) +
                                                (profitLossTotalLocationColsResult.ContainsKey(key) && c.Key == TypeOfAccount.Equity ? profitLossTotalLocationColsResult[key].Total : 0)
                                         }).ToDictionary(x => x.LocationId, x => x)
                                         : c.Key == TypeOfAccount.Equity ? retainedEarningTotalLocationColsResult.Concat(profitLossTotalLocationColsResult)
                                         .GroupBy(g => g.Key).Select(s => new TotalBalanceSheetLocationColumns
                                         {
                                             LocationId = s.Key,
                                             LocationName = "",
                                             Total = s.Sum(t => t.Value.Total)
                                         }).ToDictionary(d => d.LocationId, d => d)
                                         : null,
                                 NetIncome = c.Key == TypeOfAccount.Equity ? profitLoss : (decimal?)null,
                                 NetIncomeTotalLocationColumns = c.Key == TypeOfAccount.Equity ? profitLossTotalLocationColsResult : null
                             }).ToList(),

                         }).ToList();


            var groupResult = new List<MainAccountGroupHeader>()
                {
                    new MainAccountGroupHeader()
                    {
                        LocationSummaryHeader = locations,
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Asset",
                        TotalAmount = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset").Sum(t=>t.TotalAmount),
                        Items = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset")
                            .Select(t=> new AccountGroupHeader(){
                                Name = t.Name,
                                AccountLists = t.AccountLists,
                                //TotalLocationColumns = t.TotalLocationColumns,

                                 TotalLocationColumns = t.AccountLists.Where(c=>c.TotalLocationColumns != null).SelectMany(c => c.TotalLocationColumns)
                                         .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                         }).ToDictionary(x=>x.LocationId, x => x)

                            }).ToList(),
                        TotalLocationColumns = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset")
                                          .SelectMany(t => t.AccountLists)
                                          .Where(c=>c.TotalLocationColumns != null)
                                          .SelectMany(x => x.TotalLocationColumns)
                                         .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                         }).ToDictionary(x=>x.LocationId, x => x)
                    },
                };
            var LiabilityEquity = new List<MainAccountGroupHeader>()
            {
                new MainAccountGroupHeader()
                {
                    LocationSummaryHeader = locations,
                    RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                    Title = "Liability",
                    TotalAmount = query.Where(t=>t.Name == "CurrentLiability" || t.Name == "LongTermLiability").Sum(t=>t.TotalAmount),
                    Items = query.Where(t=>t.Name == "CurrentLiability" || t.Name == "LongTermLiability")
                        .Select(t=> new AccountGroupHeader(){
                            Name = t.Name,
                            AccountLists = t.AccountLists,
                            TotalLocationColumns = t.AccountLists.Where(c=>c.TotalLocationColumns != null).SelectMany(c => c.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total)
                                        }).ToDictionary(x=>x.LocationId, x => x)
                        }).ToList(),
                        TotalLocationColumns = query.Where(t => t.Name == "CurrentLiability" || t.Name == "LongTermLiability")
                                        .SelectMany(t => t.AccountLists)
                                        .Where(c=>c.TotalLocationColumns != null)
                                        .SelectMany(x => x.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total)
                                        }).ToDictionary(x=>x.LocationId, x => x)
                },
                new MainAccountGroupHeader()
                {
                    LocationSummaryHeader = locations,
                    RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                    Title = "Equity",
                    TotalAmount = query.Where(t => t.Name == "Equity").Sum(t => t.TotalAmount),
                    Items = query.Where(t => t.Name == "Equity")
                        .Select(t => new AccountGroupHeader(){
                            Name = t.Name,
                            AccountLists = t.AccountLists,
                            TotalLocationColumns = t.AccountLists.Where(c=>c.TotalLocationColumns != null).SelectMany(c => c.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total) 
                                                //+
                                                //(retainedEarningTotalLocationColsResult.ContainsKey(key) ? retainedEarningTotalLocationColsResult[key].Total : 0) +
                                                //(profitLossTotalLocationColsResult.ContainsKey(key) ? profitLossTotalLocationColsResult[key].Total : 0)
                                        }).ToDictionary(x=>x.LocationId, x => x)
                        }).ToList(),
                    TotalLocationColumns = query.Where(t => t.Name == "Equity")
                                    .SelectMany(t => t.AccountLists)
                                    .Where(c => c.TotalLocationColumns != null)
                                    .SelectMany(x => x.TotalLocationColumns)
                                    .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total) 
                                                //    +
                                                //(retainedEarningTotalLocationColsResult.ContainsKey(key) ? retainedEarningTotalLocationColsResult[key].Total : 0) +
                                                //(profitLossTotalLocationColsResult.ContainsKey(key) ? profitLossTotalLocationColsResult[key].Total : 0)
                                    }).ToDictionary(x=>x.LocationId, x => x)
                }
            };


            var totalLiabilityEquity = new List<MainAccountGroupHeader>()
                {
                    new MainAccountGroupHeader()
                    {
                        LocationSummaryHeader = locations,
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Liability + Equity",
                        Items = new List<AccountGroupHeader>(),
                        TotalAmount = LiabilityEquity.Sum(t=>t.TotalAmount),
                        TotalLocationColumns = LiabilityEquity.Where(c => c.TotalLocationColumns != null)
                                        .SelectMany(c => c.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns
                                        {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                        }).ToDictionary(x => x.LocationId, x => x)
                    },
                };

            var result = groupResult.Concat(LiabilityEquity).Concat(totalLiabilityEquity).ToList();

            return result;

        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<List<MainAccountGroupHeader>> GetBalanceSheetMonthlyReport(GetBalanceSheetInput input)
        {
            var tenantId = AbpSession.TenantId;
            var balanceSheetHeaderList = new[] { TypeOfAccount.CurrentAsset, TypeOfAccount.FixedAsset, TypeOfAccount.CurrentLiability, TypeOfAccount.LongTermLiability, TypeOfAccount.Equity };

            var incomeStatementHeaderList = new[] { TypeOfAccount.COGS, TypeOfAccount.Income, TypeOfAccount.Expense };

            var accountOldPeriod = new List<AccountTransactionOutPut>();
            var accountNewPeriod = new List<AccountTransactionOutPut>();


            var accountTypes = new List<IGrouping<TypeOfAccount, AccountType>>();
            AccountCycle previousCycle;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    previousCycle = await GetPreviousRoundingCloseCyleAsync(input.ToDate);

                    var fromDate = input.FromDate.Year == 1970 ? new DateTime(input.ToDate.Year, 01, 01) : input.FromDate;// mean as of from frontend
                    accountOldPeriod = await _accountTransactionManager.GetAccountQuery(null, fromDate.AddDays(-1), false, input.LocationIds)
                                        .Select(u => new AccountTransactionOutPut
                                        {
                                            AccountId = u.AccountId,
                                            AccountName = u.AccountName,
                                            AccountCode = u.AccountCode,
                                            AccountTypeId = u.AccountTypeId,
                                            Type = u.Type,
                                            Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                                 u.Type == TypeOfAccount.FixedAsset ||
                                                 u.Type == TypeOfAccount.COGS ||
                                                 u.Type == TypeOfAccount.Expense ?
                                                 1 : -1) * u.Balance
                                        }).ToListAsync();

                    accountNewPeriod = await _accountTransactionManager.GetAccountBalanceSheetMonthlyQuery(fromDate, input.ToDate, input.LocationIds)
                            .Select(u => new AccountTransactionOutPut
                            {
                                AccountId = u.AccountId,
                                AccountName = u.AccountName,
                                AccountCode = u.AccountCode,
                                AccountTypeId = u.AccountTypeId,
                                Type = u.Type,
                                LocationId = u.LocationId,
                                LocationName = u.LocationName,
                                Balance = (u.Type == TypeOfAccount.CurrentAsset ||
                                     u.Type == TypeOfAccount.FixedAsset ||
                                     u.Type == TypeOfAccount.COGS ||
                                     u.Type == TypeOfAccount.Expense ?
                                     1 : -1) * u.Balance,

                            }).ToListAsync();

                    accountTypes = await _accountTypeRepository.GetAll().AsNoTracking()
                                            .Where(t => balanceSheetHeaderList.Contains(t.Type))
                                            .GroupBy(t => t.Type)
                                            .ToListAsync();

                }

            }

            var balanceSheetAccountOldPeriod = accountOldPeriod.Where(u => balanceSheetHeaderList.Contains(u.Type));
            var balanceSheetAccountNewPeriod = accountNewPeriod.Where(u => balanceSheetHeaderList.Contains(u.Type));

            var queryResult = balanceSheetAccountOldPeriod.Concat(balanceSheetAccountNewPeriod);


            var locations = balanceSheetAccountNewPeriod.OrderBy(s => s.LocationId).GroupBy(s => new { s.LocationId, s.LocationName })
                            .Select(s => new LocationSummaryOutput
                            {
                                Id = s.Key.LocationId,
                                LocationName = s.Key.LocationName
                            }).OrderBy(t => t.Id).ToList();

            var locationListDics = locations.ToDictionary(s => s.Id, s => s.LocationName);

            var balanceSheetAccount = queryResult.GroupBy(a => new { a.AccountCode, a.AccountId, a.AccountName, a.AccountTypeId })
                                                        .Select(a => new
                                                        {
                                                            a.Key.AccountTypeId,
                                                            a.Key.AccountName,
                                                            a.Key.AccountCode,
                                                            a.Key.AccountId,
                                                            Balance = a.Sum(b => b.Balance),
                                                            TotalLocationColumns = locations.Select(z => new TotalBalanceSheetLocationColumns
                                                            {
                                                                LocationName = z.LocationName,
                                                                LocationId = z.Id,
                                                                Total = a.Where(r => r.LocationId <= z.Id).Sum(x => x.Balance)
                                                            }).ToDictionary(c => c.LocationId, c => c),

                                                        }).ToList();

            var balanceSheetAcountDict = balanceSheetAccount
                                        .GroupBy(u => u.AccountTypeId)
                                        .ToDictionary(u => u.Key);

            var incomeQuery = accountOldPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                  .Union(accountNewPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type)));

            var retainedEarning = accountOldPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type)).Select(u => (u.Type == TypeOfAccount.Income ? 1 : -1) * u.Balance).Sum();


            var retainedEarningTotalLocationCols = locations.Select(x =>
                                         new TotalBalanceSheetLocationColumns
                                         {
                                             LocationId = x.Id,
                                             Total = incomeQuery.Where(u => u.LocationId < x.Id)
                                                    .Sum(t => (t.Type == TypeOfAccount.Income ? 1 : -1) * t.Balance)
                                         }
                                    ).ToDictionary(c => c.LocationId, c => c);


            var retainedEarningTotalLocationColsResult = locationListDics.Select(x => new TotalBalanceSheetLocationColumns
            {
                LocationId = x.Key,
                Total = retainedEarningTotalLocationCols.ContainsKey(x.Key) ? retainedEarningTotalLocationCols[x.Key].Total : 0
            }).ToDictionary(c => c.LocationId, c => c);

            var profitLoss = accountNewPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .Select(u => (u.Type == TypeOfAccount.Income ? 1 : -1) * u.Balance)
                                    .Sum();

            var profitLossTotalLocationCols = accountNewPeriod.Where(u => incomeStatementHeaderList.Contains(u.Type))
                                    .GroupBy(x => x.LocationId)
                                    .Select(u => new TotalBalanceSheetLocationColumns
                                    {
                                        LocationId = u.Key,
                                        Total = u.Sum(x => (x.Type == TypeOfAccount.Income ? 1 : -1) * x.Balance)
                                    }).ToDictionary(c => c.LocationId, c => c);

            var profitLossTotalLocationColsResult = locationListDics.Select(x => new TotalBalanceSheetLocationColumns
            {
                LocationId = x.Key,
                Total = profitLossTotalLocationCols.ContainsKey(x.Key) ? profitLossTotalLocationCols[x.Key].Total : 0
            }).ToDictionary(c => c.LocationId, c => c);


            var query = (from c in accountTypes
                         select new AccountGroupHeader
                         {
                             Name = Enum.GetName(typeof(TypeOfAccount), c.Key),
                             AccountLists = c.Select(a => new GetListBalanceSheetReportOutput()
                             {
                                 Id = a.Id,
                                 AccountTypeName = a.AccountTypeName,
                                 AccountType = a.Type,
                                 Items = balanceSheetAcountDict.ContainsKey(a.Id) ?
                                         balanceSheetAcountDict[a.Id]
                                         .Select(t => new AccountList
                                         {
                                             AccountCode = t.AccountCode,
                                             AccountName = t.AccountName,
                                             Id = t.AccountId,
                                             TotalAmount = t.Balance,
                                             TotalLocationColumns = t.TotalLocationColumns
                                         }).OrderBy(x => x.AccountCode).ToList() : new List<AccountList>(),
                                 RetainedEarning = c.Key == TypeOfAccount.Equity ? retainedEarning : (decimal?)null,
                                 RetainedEarningTotalLocationColumns = c.Key == TypeOfAccount.Equity ? retainedEarningTotalLocationColsResult : null,
                                 TotalLocationColumns = balanceSheetAcountDict.ContainsKey(a.Id) ?
                                         balanceSheetAcountDict[a.Id]
                                         .SelectMany(t => t.TotalLocationColumns)
                                         .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns
                                         {
                                             LocationId = key,
                                             LocationName = "",
                                             Total = value.Sum(x => x.Value.Total) +
                                                (retainedEarningTotalLocationColsResult.ContainsKey(key) && c.Key == TypeOfAccount.Equity ? retainedEarningTotalLocationColsResult[key].Total : 0) +
                                                (profitLossTotalLocationColsResult.ContainsKey(key) && c.Key == TypeOfAccount.Equity ? profitLossTotalLocationColsResult[key].Total : 0)
                                         }).ToDictionary(x => x.LocationId, x => x) 
                                         : c.Key == TypeOfAccount.Equity ? retainedEarningTotalLocationColsResult.Concat(profitLossTotalLocationColsResult)
                                         .GroupBy(g => g.Key).Select(s => new TotalBalanceSheetLocationColumns
                                         { 
                                                LocationId = s.Key,
                                                LocationName = "",
                                                Total = s.Sum(t => t.Value.Total)
                                         }).ToDictionary(d => d.LocationId, d => d) 
                                         : null,
                                 NetIncome = c.Key == TypeOfAccount.Equity ? profitLoss : (decimal?)null,
                                 NetIncomeTotalLocationColumns = c.Key == TypeOfAccount.Equity ? profitLossTotalLocationColsResult : null
                             }).ToList(),

                         }).ToList();


            var groupResult = new List<MainAccountGroupHeader>()
                {
                    new MainAccountGroupHeader()
                    {
                        LocationSummaryHeader = locations,
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Asset",
                        TotalAmount = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset").Sum(t=>t.TotalAmount),
                        Items = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset")
                            .Select(t=> new AccountGroupHeader(){
                                Name = t.Name,
                                AccountLists = t.AccountLists,
                                //TotalLocationColumns = t.TotalLocationColumns,

                                 TotalLocationColumns = t.AccountLists.Where(c=>c.TotalLocationColumns != null).SelectMany(c => c.TotalLocationColumns)
                                         .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                         }).ToDictionary(x=>x.LocationId, x => x)

                            }).ToList(),
                        TotalLocationColumns = query.Where(t => t.Name == "CurrentAsset" || t.Name == "FixedAsset")
                                          .SelectMany(t => t.AccountLists)
                                          .Where(c=>c.TotalLocationColumns != null)
                                          .SelectMany(x => x.TotalLocationColumns)
                                         .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                         }).ToDictionary(x=>x.LocationId, x => x)
                    },
                };
                       

            var LiabilityEquity = new List<MainAccountGroupHeader>()
            {
                new MainAccountGroupHeader()
                {
                    LocationSummaryHeader = locations,
                    RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                    Title = "Liability",
                    TotalAmount = query.Where(t=>t.Name == "CurrentLiability" || t.Name == "LongTermLiability").Sum(t=>t.TotalAmount),
                    Items = query.Where(t=>t.Name == "CurrentLiability" || t.Name == "LongTermLiability")
                        .Select(t=> new AccountGroupHeader(){
                            Name = t.Name,
                            AccountLists = t.AccountLists,
                            TotalLocationColumns = t.AccountLists.Where(c=>c.TotalLocationColumns != null).SelectMany(c => c.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                        }).ToDictionary(x=>x.LocationId, x => x)
                        }).ToList(),
                    TotalLocationColumns = query.Where(t => t.Name == "CurrentLiability" || t.Name == "LongTermLiability")
                                    .SelectMany(t => t.AccountLists)
                                    .Where(c=>c.TotalLocationColumns != null)
                                    .SelectMany(x => x.TotalLocationColumns)
                                    .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total)
                                    }).ToDictionary(x=>x.LocationId, x => x)
                },
                new MainAccountGroupHeader()
                {
                    LocationSummaryHeader = locations,
                    RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                    Title = "Equity",
                    TotalAmount = query.Where(t => t.Name == "Equity").Sum(t => t.TotalAmount),
                    Items = query.Where(t => t.Name == "Equity")
                        .Select(t => new AccountGroupHeader(){
                            Name = t.Name,
                            AccountLists = t.AccountLists,
                            TotalLocationColumns = t.AccountLists.Where(c=>c.TotalLocationColumns != null).SelectMany(c => c.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total) 
                                        }).ToDictionary(x=>x.LocationId, x => x)
                        }).ToList(),
                    TotalLocationColumns = query.Where(t => t.Name == "Equity")
                                    .SelectMany(t => t.AccountLists)
                                    .Where(c => c.TotalLocationColumns != null)
                                    .SelectMany(x => x.TotalLocationColumns)
                                    .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns {
                                        LocationId = key,
                                        LocationName = "",
                                        Total = value.Sum(x => x.Value.Total) 
                                    }).ToDictionary(x=>x.LocationId, x => x)
                }
            };


            var totalLiabilityEquity = new List<MainAccountGroupHeader>()
                {
                    new MainAccountGroupHeader()
                    {
                        LocationSummaryHeader = locations,
                        RoundingDigit = previousCycle !=null ? previousCycle.RoundingDigit : 2,
                        Title = "Liability + Equity",
                        Items = new List<AccountGroupHeader>(),
                        TotalAmount = LiabilityEquity.Sum(t=>t.TotalAmount),
                        TotalLocationColumns = LiabilityEquity.Where(c => c.TotalLocationColumns != null)
                                        .SelectMany(c => c.TotalLocationColumns)
                                        .GroupBy(v => v.Key, (key, value) => new TotalBalanceSheetLocationColumns
                                        {
                                            LocationId = key,
                                            LocationName = "",
                                            Total = value.Sum(x => x.Value.Total)
                                        }).ToDictionary(x => x.LocationId, x => x)
                    },
                };

            var result = groupResult.Concat(LiabilityEquity).Concat(totalLiabilityEquity).ToList();

            return result;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BalanceSheet)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<List<MainAccountGroupHeader>> GetBalanceSheetReport(GetBalanceSheetInput input)
        {
            var result = input.ViewOption == ViewOption.Standard ? await GetBalanceSheetStandardReport(input)
                        : input.ViewOption == ViewOption.Location ? await GetBalanceSheetLocationReport(input)
                        : await GetBalanceSheetMonthlyReport(input);
            return result;
        }

        public ReportOutput GetReportTemplateBalanceSheet()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ViewOption",
                        ColumnLength = 150,
                        ColumnTitle = "View Option",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = false,
                        IsDisplay = false,
                        DefaultValue = ((int)ViewOption.Standard).ToString()
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Account",
                        ColumnLength = 400,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.Array,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 200,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.Money,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                },
                Groupby = "",
                HeaderTitle = "Balance Sheet Report",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "Ledger",
                    ColumnTitle = "AccountLedgerTemplate",
                    DefaultValue = ReportType.ReportType_Ledger
                },
                DefaultTemplate2 = new DefaultSaveTemplateOutput
                {
                    ColumnName = "ProfitAndLoss",
                    ColumnTitle = "ProfitAndLossReportTemplate",
                    DefaultValue = ReportType.ReportType_ProfitAndLoss
                }
            };
            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BalanceSheet_Export_Excel)]
        public async Task<FileDto> ExportExcelBalanceSheetReport(GetBalanceSheetReportInput input)
        {

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var balanceSheetData = await GetBalanceSheetReport(input);

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.FitToPage = true;
                ws.PrinterSettings.PaperSize = ePaperSize.A4; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                #region Row 2 Header
                var header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                int ih = 0;
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    if (ih > 0)
                    {
                        ws.Cells[rowTableHeader, colHeaderTable].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    ih += 1;
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                foreach (var bs in balanceSheetData)
                {
                    //Title of Asset, liaibility
                    int collumnCellBody = 1;
                    if (bs.Title != "Liability + Equity")
                    {
                        AddTextToCell(ws, rowBody, collumnCellBody, bs.Title, true);
                        rowBody += 1;
                    }
                    var totalAddressAccount = "";
                    foreach (var item in bs.Items)
                    {
                        var countItem = item.AccountLists.Where(t => t.Items.Count > 0).Count();
                        if (item.TotalAmount != 0 || (item.AccountLists.Count > 0 && countItem > 0))
                        {
                            // title of account type
                            AddTextToCell(ws, rowBody, collumnCellBody, L(item.Name), true, 5);
                            rowBody += 1;
                            var subTotalAccountTypeAddress = "";
                            foreach (var acc in item.AccountLists)
                            {
                                if (acc.TotalAmount != 0 || acc.Items.Count > 0 || acc.AccountType == TypeOfAccount.Equity)
                                {
                                    // accounts name
                                    AddTextToCell(ws, rowBody, collumnCellBody, acc.AccountTypeName, true, 10);
                                    rowBody += 1;
                                    foreach (var i in acc.Items)
                                    {
                                        int colItem = 2;
                                        foreach (var t in reportCollumnHeader)
                                        {
                                            if (t.ColumnName == "Account")
                                            {
                                                AddTextToCell(ws, rowBody, collumnCellBody, i.AccountCode + " - " + i.AccountName, false, 15);
                                            }
                                            else if (t.ColumnName == "Total")
                                            {
                                                AddNumberToCell(ws, rowBody, colItem++, i.TotalAmount);
                                            }
                                            else
                                            {
                                                if (i.TotalLocationColumns != null)
                                                {
                                                    var ColumnName = Int32.Parse(t.ColumnName);
                                                    AddNumberToCell(ws, rowBody, colItem++, i.TotalLocationColumns.ContainsKey(ColumnName)? i.TotalLocationColumns[ColumnName].Total : 0);

                                                }
                                            }

                                        }
                                        // account sub items
                                        //AddTextToCell(ws, rowBody, collumnCellBody, i.AccountCode + " - " + i.AccountName, false, 15);
                                        //AddNumberToCell(ws, rowBody, collumnCellBody + 1, i.TotalAmount);
                                        rowBody += 1;
                                    }

                                    var decrease = 0;
                                    if (acc.RetainedEarning != null)
                                    {
                                        int colItem = 1;
                                        foreach (var t in reportCollumnHeader)
                                        {
                                            if (t.ColumnName == "Account")
                                            {
                                                AddTextToCell(ws, rowBody, colItem++, L("RetainedEarning"), false, 15);
                                            }
                                            else if (t.ColumnName == "Total")
                                            {
                                                AddNumberToCell(ws, rowBody, colItem++, acc.RetainedEarning.Value);
                                            }
                                            else
                                            {
                                                if (acc.RetainedEarningTotalLocationColumns != null)
                                                {
                                                    var ColumnName = Int32.Parse(t.ColumnName);
                                                    AddNumberToCell(ws, rowBody, colItem++, acc.RetainedEarningTotalLocationColumns.ContainsKey(ColumnName) ? acc.RetainedEarningTotalLocationColumns[ColumnName].Total : 0);

                                                }
                                            }

                                        }
                                        //AddNumberToCell(ws, rowBody, collumnCellBody + 1, acc.RetainedEarning.Value);
                                        rowBody += 1;
                                        decrease += 1;
                                    }

                                    if (acc.NetIncome != null)
                                    {
                                        int colItem = 1;
                                        foreach (var t in reportCollumnHeader)
                                        {
                                            if (t.ColumnName == "Account")
                                            {
                                                AddTextToCell(ws, rowBody, colItem++, L("NetIncome"), false, 15);
                                            }
                                            else if (t.ColumnName == "Total")
                                            {
                                                AddNumberToCell(ws, rowBody, colItem++, acc.NetIncome.Value);
                                            }
                                            else
                                            {
                                                if (acc.NetIncomeTotalLocationColumns != null)
                                                {
                                                    var ColumnName = Int32.Parse(t.ColumnName);
                                                    AddNumberToCell(ws, rowBody, colItem++, acc.NetIncomeTotalLocationColumns.ContainsKey(ColumnName) ? acc.NetIncomeTotalLocationColumns[ColumnName].Total : 0);

                                                }
                                            }

                                        }
                                        //AddTextToCell(ws, rowBody, collumnCellBody, L("NetIncome"), false, 15);
                                        //AddNumberToCell(ws, rowBody, collumnCellBody + 1, acc.NetIncome.Value);
                                        decrease += 1;
                                        rowBody += 1;
                                    }


                                    int colItemTotal = 2;
                                    foreach (var t in reportCollumnHeader)
                                    {
                                        if (t.ColumnName == "Account")
                                        {
                                            AddTextToCell(ws, rowBody, collumnCellBody, L("Total") + " " + acc.AccountTypeName, true, 10);
                                            //AddTextToCell(ws, rowBody, colItem++, L("RetainedEarning"), false, 15);
                                        }
                                        else if (t.ColumnName == "Total")
                                        {
                                            AddNumberToCell(ws, rowBody, colItemTotal++, acc.TotalAmount);
                                        }
                                        else
                                        {
                                            if (acc.TotalLocationColumns != null)
                                            {
                                                var ColumnName = Int32.Parse(t.ColumnName);
                                                AddNumberToCell(ws, rowBody, colItemTotal++, acc.TotalLocationColumns.ContainsKey(ColumnName) ? acc.TotalLocationColumns[ColumnName].Total : 0);

                                            }
                                        }

                                    }

                                    //AddTextToCell(ws, rowBody, collumnCellBody, L("Total") + " " + acc.AccountTypeName, true, 10);

                                    //// Formula Sub Total of Account Type
                                    //var fromCellAT = GetAddressName(ws, rowBody - acc.Items.Count() - decrease, collumnCellBody​ + 1);
                                    //var toCellAT = GetAddressName(ws, rowBody - 1, collumnCellBody​ + 1);
                                    //AddFormula(ws, rowBody, collumnCellBody​ + 1, "SUM(" + fromCellAT + ":" + toCellAT + ")", true);
                                    //// map cell address
                                    //subTotalAccountTypeAddress += subTotalAccountTypeAddress.Count() > 0 ? "," + GetAddressName(ws, rowBody, collumnCellBody​ + 1) : GetAddressName(ws, rowBody, collumnCellBody​ + 1);
                                    rowBody += 1;
                                }
                            }

                            int colItemTotalAccount = 2;
                            foreach (var t in reportCollumnHeader)
                            {
                                if (t.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, collumnCellBody, L("Total") + " " + L(item.Name), true, 5);
                                }
                                else if (t.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colItemTotalAccount++, item.TotalAmount);
                                }
                                else
                                {
                                    if (item.TotalLocationColumns != null)
                                    {
                                        var ColumnName = Int32.Parse(t.ColumnName);
                                        AddNumberToCell(ws, rowBody, colItemTotalAccount++, item.TotalLocationColumns.ContainsKey(ColumnName) ? item.TotalLocationColumns[ColumnName].Total : 0);

                                    }
                                }

                            }


                            //AddTextToCell(ws, rowBody, collumnCellBody, L("Total") + " " + L(item.Name), true, 5);
                            //// Formula Sub Total of Account
                            //AddFormula(ws, rowBody, collumnCellBody​ + 1, "SUM(" + subTotalAccountTypeAddress + ")", true);
                            ////map cell address to auto formula of main acc 
                            //totalAddressAccount += totalAddressAccount.Count() > 0 ? "," + GetAddressName(ws, rowBody, collumnCellBody​ + 1) : GetAddressName(ws, rowBody, collumnCellBody​ + 1);

                            rowBody += 1;
                        }
                    }
                    int colItemTotalMain = 2;
                    foreach (var t in reportCollumnHeader)
                    {
                        if (t.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody​, L("Total") + " " + bs.Title, true);
                        }
                        else if (t.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colItemTotalMain++, bs.TotalAmount);
                        }
                        else
                        {
                            if (bs.TotalLocationColumns != null)
                            {
                                var ColumnName = Int32.Parse(t.ColumnName);
                                AddNumberToCell(ws, rowBody, colItemTotalMain++, bs.TotalLocationColumns.ContainsKey(ColumnName) ? bs.TotalLocationColumns[ColumnName].Total : 0);

                            }
                        }

                    }

                    //AddTextToCell(ws, rowBody, collumnCellBody​, L("Total") + " " + bs.Title, true);
                    ////AddNumberToCell(ws, rowBody, collumnCellBody​ + 1, bs.TotalAmount, true);
                    //AddFormula(ws, rowBody, collumnCellBody​ + 1, "SUM(" + totalAddressAccount + ")", true);
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body

                #region Row Footer 
                //if (reportHasShowFooterTotal.Count > 0)
                //{
                //    int footerRow = rowBody;
                //    int footerColNumber = 1;
                //    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                //    foreach (var i in reportCollumnHeader)
                //    {
                //        if (i.AllowFunction != null)
                //        {
                //            int rowFooter = rowTableHeader + 1;// get start row after 
                //            var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                //            AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                //        }
                //        footerColNumber += 1;
                //    }
                //}
                #endregion Row Footer


                result.FileName = $"BalanceSheet_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BalanceSheet_Export_Pdf)]
        public async Task<FileDto> ExportPdfBalanceSheetReport(GetBalanceSheetReportInput input)
        {
            var tenantId = AbpSession.TenantId;
            Tenant tenant;
            string formatDate;
            int rounding;
            User user;
            var balanceSheets = new List<MainAccountGroupHeader>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    tenant = await GetCurrentTenantAsync();
                    rounding = (await GetCurrentCycleAsync()).RoundingDigit;
                    formatDate = await _formatRepository.GetAll()
                             .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                             .Select(t => t.Web).FirstOrDefaultAsync();
                    user = await GetCurrentUserAsync();
                    balanceSheets = await GetBalanceSheetReport(input);
                }

                await uow.CompleteAsync();
            }

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "BalanceSheet.pdf",
                    FileToken = "BalanceSheet.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int colspan = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            colspan++;
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body            
                foreach (var bs in balanceSheets)
                {
                    //leavel 1
                    var headerAccount = "";
                    if (bs.Title != "Liability + Equity")
                    {
                        headerAccount += $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold'>";
                        foreach (var t in viewHeader)
                        {
                            if (t.Visible)
                            {
                                if (t.ColumnName == "Account")
                                {
                                    headerAccount += $"<td>{bs.Title}</td>";
                                }
                                else
                                {
                                    headerAccount += $"<td></td>";

                                }
                            }
                        }
                        headerAccount += $"</tr>";
                    }

                    contentBody += headerAccount;
                    foreach (var item in bs.Items)
                    {
                        var countItem = item.AccountLists.Where(t => t.Items.Count > 0).Count();
                        if (item.TotalAmount != 0 || (item.AccountLists.Count > 0 && countItem > 0))
                        {

                            //level 2
                            var subitem = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold'>";
                            foreach (var t in viewHeader)
                            {
                                if (t.Visible)
                                {
                                    if (t.ColumnName == "Account")
                                    {
                                        subitem += $"<td><div style='margin-left:30px'>{item.Name}</div></td>";
                                    }
                                    else
                                    {
                                        subitem += $"<td></td>";

                                    }
                                }
                            }
                            subitem += $"</tr>";
                            contentBody += subitem;
                            foreach (var acc in item.AccountLists)
                            {
                                if (acc.TotalAmount != 0 || acc.Items.Count > 0 || acc.AccountType == TypeOfAccount.Equity)
                                {
                                    // level 3
                                    var chartAccount = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold'>";
                                    foreach (var t in viewHeader)
                                    {
                                        if (t.Visible)
                                        {
                                            if (t.ColumnName == "Account")
                                            {
                                                chartAccount += $"<td><div style='margin-left:60px'>{acc.AccountTypeName}</div></td>";
                                            }
                                            else
                                            {
                                                chartAccount += $"<td></td>";

                                            }
                                        }
                                    }
                                    chartAccount += $"</tr>";
                                    contentBody += chartAccount;

                                    //level 4
                                    foreach (var i in acc.Items)
                                    {
                                        var itemLast = "<tr valign='top' style='page-break-before: auto; page-break-after: auto;'>";
                                        foreach (var t in viewHeader)
                                        {
                                            if (t.Visible)
                                            {
                                                if (t.ColumnName == "Account")
                                                {
                                                    itemLast += $"<td><div style='margin-left:90px'>{i.AccountCode + " - " + i.AccountName}</div></td>";
                                                }
                                                else if (t.ColumnName == "Total")
                                                {
                                                    itemLast += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(i.TotalAmount, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                }
                                                else
                                                {
                                                    if (i.TotalLocationColumns != null)
                                                    {
                                                        var ColumnName = Int32.Parse(t.ColumnName);
                                                        itemLast += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(i.TotalLocationColumns.ContainsKey(ColumnName) ? i.TotalLocationColumns[ColumnName].Total : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                    }
                                                }
                                            }
                                        }
                                        itemLast += "</tr>";
                                        contentBody += itemLast;
                                    }

                                    if (acc.RetainedEarning != null)
                                    {
                                        var retainedEarning = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                                        foreach (var t in viewHeader)
                                        {
                                            if (t.Visible)
                                            {
                                                if (t.ColumnName == "Account")
                                                {
                                                    retainedEarning += $"<td><div style='margin-left:90px'>{L("RetainedEarning")}</div></td>";
                                                }
                                                else if (t.ColumnName == "Total")
                                                {
                                                    retainedEarning += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(acc.RetainedEarning.Value, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                }
                                                else
                                                {
                                                    if (acc.RetainedEarningTotalLocationColumns != null)
                                                    {
                                                        var ColumnName = Int32.Parse(t.ColumnName);
                                                        retainedEarning += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(acc.RetainedEarningTotalLocationColumns.ContainsKey(ColumnName) ? acc.RetainedEarningTotalLocationColumns[ColumnName].Total : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                    }
                                                }
                                            }
                                        }
                                        retainedEarning += "</tr>";
                                        contentBody += retainedEarning;
                                    }
                                    if (acc.NetIncome != null)
                                    {
                                        var netIncome = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                                        foreach (var t in viewHeader)
                                        {
                                            if (t.Visible)
                                            {
                                                if (t.ColumnName == "Account")
                                                {
                                                    netIncome += $"<td><div style='margin-left:90px'>{L("NetIncome")}</div></td>";
                                                }
                                                else if (t.ColumnName == "Total")
                                                {
                                                    netIncome += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(acc.NetIncome.Value, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                }
                                                else
                                                {
                                                    if (acc.NetIncomeTotalLocationColumns != null)
                                                    {
                                                        var ColumnName = Int32.Parse(t.ColumnName);
                                                        netIncome += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(acc.NetIncomeTotalLocationColumns.ContainsKey(ColumnName) ? acc.NetIncomeTotalLocationColumns[ColumnName].Total : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                    }
                                                }
                                            }
                                        }
                                        netIncome += "</tr>";
                                        contentBody += netIncome;
                                    }


                                    var chartAccountTotal = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold;'>";
                                    foreach (var t in viewHeader)
                                    {
                                        if (t.Visible)
                                        {
                                            if (t.ColumnName == "Account")
                                            {
                                                chartAccountTotal += $"<td><div style='margin-left:60px'>{L("Total")} {acc.AccountTypeName}</div></td>";
                                            }
                                            else if (t.ColumnName == "Total")
                                            {
                                                chartAccountTotal += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(acc.TotalAmount, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                            }
                                            else
                                            {
                                                if (acc.TotalLocationColumns != null)
                                                {
                                                    var ColumnName = Int32.Parse(t.ColumnName);
                                                    chartAccountTotal += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(acc.TotalLocationColumns.ContainsKey(ColumnName) ? acc.TotalLocationColumns[ColumnName].Total : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                                }
                                            }
                                        }
                                    }
                                    chartAccountTotal += "</tr>";
                                    //var chartAccountTotal = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold;'>" +
                                    //                        $"<td><span style='margin-left:60px'>" + L("Total") + " " + acc.AccountTypeName +
                                    //                        "</span></td><td  style='text-align:right'>" + 
                                    //                        FormatNumberCurrency(Math.Round(acc.TotalAmount, rounding, MidpointRounding.ToEven), rounding) + "</td></tr>";
                                    contentBody += chartAccountTotal;
                                }
                            }

                            var subitemTotal = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold;'>";
                            foreach (var t in viewHeader)
                            {
                                if (t.Visible)
                                {
                                    if (t.ColumnName == "Account")
                                    {
                                        subitemTotal += $"<td><div style='margin-left:30px'>{L("Total")} {item.Name}</div></td>";
                                    }
                                    else if (t.ColumnName == "Total")
                                    {
                                        subitemTotal += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(item.TotalAmount, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                    }
                                    else
                                    {
                                        if (item.TotalLocationColumns != null)
                                        {
                                            var ColumnName = Int32.Parse(t.ColumnName);
                                            subitemTotal += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(item.TotalLocationColumns.ContainsKey(ColumnName) ? item.TotalLocationColumns[ColumnName].Total : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                        }
                                    }
                                }
                            }
                            subitemTotal += "</tr>";

                            //var subitemTotal = $"<tr style='font-weight: bold'><td><span style='margin-left:30px'>" + L("Total") + 
                            //                    " " + item.Name + "</span></td><td  style='text-align:right'>" + 
                            //                    FormatNumberCurrency(Math.Round(item.TotalAmount, rounding, MidpointRounding.ToEven), rounding) + "</td></tr>";
                            contentBody += subitemTotal;
                        }
                    }
                    var totalMeain = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold;'>";
                    foreach (var t in viewHeader)
                    {
                        if (t.Visible)
                        {
                            if (t.ColumnName == "Account")
                            {
                                totalMeain += $"<td>{L("Total")} {bs.Title}</td>";
                            }
                            else if (t.ColumnName == "Total")
                            {
                                totalMeain += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(bs.TotalAmount, rounding, MidpointRounding.ToEven), rounding)}</td>";
                            }
                            else
                            {
                                if (bs.TotalLocationColumns != null)
                                {
                                    var ColumnName = Int32.Parse(t.ColumnName);
                                    totalMeain += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(bs.TotalLocationColumns.ContainsKey(ColumnName) ? bs.TotalLocationColumns[ColumnName].Total : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                }
                            }
                        }
                    }
                    totalMeain += "</tr>";

                    //var totalMeain = $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold;'><td>" + L("Total") + " " + bs.Title + "</td><td style='text-align:right'>" + 
                    //FormatNumberCurrency(Math.Round(bs.TotalAmount, rounding, MidpointRounding.ToEven), rounding) + "</td></tr>";
                    contentBody += totalMeain;
                }
                #endregion Row Body

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }
        #endregion


        #region Trial Balance Report 

        [UnitOfWork(IsDisabled = true)]
        private async Task<TrialBalanceReportResultOutput> GetTrialBalanceStandardReport(GetTrialBalanceInput input)
        {
            var tenantId = AbpSession.TenantId;
           
            AccountCycle previousClose;
            var accountList = new List<TrialBalanceAccountList>();
            
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    previousClose = await GetPreviousCloseCyleAsync(input.ToDate);

                    if (previousClose == null)
                    {
                        accountList = await _journalItemRepository
                                            .GetAll()
                                            .AsNoTracking()
                                            .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                            .Where(u => u.Journal.DateOnly <= input.ToDate.Date)
                                            .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                            .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                            .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => s.Journal.LocationId.HasValue && input.LocationIds.Contains(s.Journal.LocationId.Value))
                                            .GroupBy(s => new { s.Account.AccountCode, s.Account.AccountName, s.AccountId })
                                            .Select(s => new TrialBalanceAccountList
                                            {
                                                Id = s.Key.AccountId,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                Debit = s.Sum(t => t.Debit - t.Credit)
                                            })
                                            .OrderBy(s => s.AccountCode)
                                            .ToListAsync();

                    }
                    else
                    {   
                        var accountClosePeriodQuery = _accountCloseRepository.GetAll()
                                                  .Where(s => s.AccountCycleId == previousClose.Id)
                                                  .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                                  .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => input.LocationIds.Contains(s.LocationId.Value))
                                                  .AsNoTracking()
                                                  .Select(s => new TrialBalanceAccountList
                                                  {
                                                      Id = s.AccountId,
                                                      AccountCode = s.Account.AccountCode,
                                                      AccountName = s.Account.AccountName,
                                                      Debit = s.Balance > 0 ? s.Balance : 0,
                                                      Credit = s.Balance < 0 ? -s.Balance : 0,
                                                  });

                        var accountForPeriod = _journalItemRepository
                                                .GetAll()
                                                .AsNoTracking()
                                                .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                                .Where(u => u.Journal.DateOnly > previousClose.EndDate.Value.Date && u.Journal.DateOnly <= input.ToDate.Date)
                                                .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                                .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                                .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => s.Journal.LocationId.HasValue && input.LocationIds.Contains(s.Journal.LocationId.Value))
                                                .Select(u => new TrialBalanceAccountList
                                                {
                                                    Id = u.AccountId,
                                                    AccountName = u.Account.AccountName,
                                                    AccountCode = u.Account.AccountCode,
                                                    Debit = u.Debit,
                                                    Credit = u.Credit,
                                                });


                        accountList = await accountClosePeriodQuery.Concat(accountForPeriod)
                                            .OrderBy(s => s.AccountCode)
                                            .GroupBy(s => new {  s.AccountCode, s.AccountName, s.Id})
                                            .Select(s => new TrialBalanceAccountList
                                            {
                                                Id = s.Key.Id,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                Debit = s.Sum(t => t.Debit - t.Credit)
                                            })
                                            .ToListAsync();
                    }
                }
            }

            //Enumerable is more faster then .ToList()
            var items = accountList.Where(s => s.Debit < 0);
           
            foreach (var s in items)
            {   
                s.Credit = -s.Debit;
                s.Debit = 0;
            };

            return new TrialBalanceReportResultOutput
            {
                Accounts = accountList,
                Debit = accountList.Sum(t => t.Debit),
                Credit = accountList.Sum(t => t.Credit)
            };

        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<TrialBalanceReportResultOutput> GetTrialBalanceLocationReport(GetTrialBalanceInput input)
        {
            var tenantId = AbpSession.TenantId;

            AccountCycle previousClose;
            var accountList = new List<TrialBalanceAccountOutput>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    previousClose = await GetPreviousCloseCyleAsync(input.ToDate);

                    if (previousClose == null)
                    {
                        accountList = await _journalItemRepository
                                            .GetAll()
                                            .AsNoTracking()
                                            .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                            .Where(u => u.Journal.DateOnly <= input.ToDate.Date)
                                            .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                            .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                            .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => s.Journal.LocationId.HasValue && input.LocationIds.Contains(s.Journal.LocationId.Value))
                                            .GroupBy(s => new { s.Account.AccountCode, s.Account.AccountName, s.AccountId, s.Journal.LocationId, s.Journal.Location.LocationName })
                                            .Select(s => new TrialBalanceAccountOutput
                                            {
                                                Id = s.Key.AccountId,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                LocationId = s.Key.LocationId.Value,
                                                LocationName = s.Key.LocationName,
                                                Debit = s.Sum(t => t.Debit - t.Credit)
                                            })
                                            .OrderBy(s => s.AccountCode)
                                            .ToListAsync();

                    }
                    else
                    {
                        var accountClosePeriodQuery = _accountCloseRepository.GetAll()
                                                  .Where(s => s.AccountCycleId == previousClose.Id)
                                                  .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                                  .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => input.LocationIds.Contains(s.LocationId.Value))
                                                  .AsNoTracking()
                                                  .Select(s => new TrialBalanceAccountOutput
                                                  {
                                                      Id = s.AccountId,
                                                      AccountCode = s.Account.AccountCode,
                                                      AccountName = s.Account.AccountName,
                                                      LocationId = s.LocationId.Value,
                                                      LocationName = s.Location.LocationName,
                                                      Debit = s.Balance > 0 ? s.Balance : 0,
                                                      Credit = s.Balance < 0 ? -s.Balance : 0,
                                                  });

                        var accountForPeriod = _journalItemRepository
                                                .GetAll()
                                                .AsNoTracking()
                                                .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                                .Where(u => u.Journal.DateOnly > previousClose.EndDate.Value.Date && u.Journal.DateOnly <= input.ToDate.Date)
                                                .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                                .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                                .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => s.Journal.LocationId.HasValue && input.LocationIds.Contains(s.Journal.LocationId.Value))
                                                .Select(u => new TrialBalanceAccountOutput
                                                {
                                                    Id = u.AccountId,
                                                    AccountName = u.Account.AccountName,
                                                    AccountCode = u.Account.AccountCode,
                                                    LocationId = u.Journal.LocationId.Value,
                                                    LocationName = u.Journal.Location.LocationName,
                                                    Debit = u.Debit,
                                                    Credit = u.Credit,
                                                });


                        accountList = await accountClosePeriodQuery.Concat(accountForPeriod)
                                            .OrderBy(s => s.AccountCode)
                                            .GroupBy(s => new { s.AccountCode, s.AccountName, s.Id, s.LocationId, s.LocationName })
                                            .Select(s => new TrialBalanceAccountOutput
                                            {
                                                Id = s.Key.Id,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                LocationId = s.Key.LocationId,
                                                LocationName = s.Key.LocationName,
                                                Debit = s.Sum(t => t.Debit - t.Credit)
                                            })
                                            .ToListAsync();
                    }

                }
            }


            var locations = accountList.Select(s => new { s.LocationId, s.LocationName}).Distinct().OrderBy(s => s.LocationId).ToList();

            var accounts = accountList
                           .GroupBy(s => new { s.AccountName, s.AccountCode,  s.Id })
                           .Select(s => new TrialBalanceAccountList
                           {
                               Id = s.Key.Id,
                               AccountCode = s.Key.AccountCode,
                               AccountName = s.Key.AccountName,
                               Debit = s.Where(t => t.Debit > 0).Sum(t => t.Debit),
                               Credit = s.Where(T => T.Debit < 0).Sum(t => -t.Debit),
                               TotalLocationColumns = locations.Select(l => new TotalTrialBalanceLocationColumns
                               {
                                   LocationId = l.LocationId,
                                   LocationName = l.LocationName,
                                   Debit = s.Where(t => t.LocationId == l.LocationId && t.Debit > 0).Sum(t => t.Debit),
                                   Credit = s.Where(t => t.LocationId == l.LocationId && t.Debit < 0).Sum(t => -t.Debit)
                               })
                               .ToDictionary(k => k.LocationId, v => v)
                           })
                           .OrderBy(s => s.AccountCode)
                           .ToList();


            return new TrialBalanceReportResultOutput
            {
                Accounts = accounts,
                Debit = accounts.Sum(t => t.Debit),
                Credit = accounts.Sum(t => t.Credit),
                locationSummaryHeader = locations.Select(s => new LocationSummaryDto { Id = s.LocationId, LocationName = s.LocationName }).ToList(),
                LocationColumnDic = locations.Select(l => new TotalTrialBalanceLocationColumns
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Debit = accounts.Sum(t => t.TotalLocationColumns[l.LocationId].Debit),
                    Credit = accounts.Sum(t => t.TotalLocationColumns[l.LocationId].Credit)
                })
                .ToDictionary(k => k.LocationId, v => v)
            };

        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<TrialBalanceReportResultOutput> GetTrialBalanceMonthlyReport(GetTrialBalanceInput input)
        {
            var tenantId = AbpSession.TenantId;
            var isSameYear = input.FromDate.Year == input.ToDate.Year;

            AccountCycle previousClose;
            var accountList = new List<TrialBalanceAccountOutput>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    previousClose = await GetPreviousCloseCyleAsync(input.ToDate);

                    if (previousClose == null)
                    {
                        accountList = await _journalItemRepository
                                            .GetAll()
                                            .AsNoTracking()
                                            .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                            .Where(u => u.Journal.DateOnly <= input.ToDate.Date)
                                            .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                            .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                            .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => s.Journal.LocationId.HasValue && input.LocationIds.Contains(s.Journal.LocationId.Value))
                                            .GroupBy(s => new { 
                                                s.Account.AccountCode, 
                                                s.Account.AccountName,
                                                s.AccountId,
                                                LocationId =  s.Journal.Date <= input.FromDate.Date ? 
                                                               Convert.ToInt32(input.FromDate.ToString("yyyyMM")) : 
                                                               Convert.ToInt32(s.Journal.Date.ToString("yyyyMM")), 
                                                LocationName = s.Journal.Date <= input.FromDate.Date ? 
                                                               (isSameYear ? input.FromDate.ToString("MMM") : input.FromDate.ToString("MMM-yyyy")) :
                                                               (isSameYear ? s.Journal.Date.ToString("MMM") : s.Journal.Date.ToString("MMM-yyyy"))
                                            })
                                            .Select(s => new TrialBalanceAccountOutput
                                            {
                                                Id = s.Key.AccountId,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                LocationId = s.Key.LocationId,
                                                LocationName = s.Key.LocationName,
                                                Debit = s.Sum(t => t.Debit - t.Credit)
                                            })
                                            .ToListAsync();

                    }
                    else
                    {
                        var accountClosePeriodQuery = _accountCloseRepository.GetAll()
                                                  .Where(s => s.AccountCycleId == previousClose.Id)
                                                  .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                                  .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => input.LocationIds.Contains(s.LocationId.Value))
                                                  .AsNoTracking()
                                                  .Select(s => new TrialBalanceAccountOutput
                                                  {  
                                                      AccountCode = s.Account.AccountCode,
                                                      AccountName = s.Account.AccountName,
                                                      Id = s.AccountId,
                                                      LocationId = s.Date <= input.FromDate.Date ?
                                                               Convert.ToInt32(input.FromDate.ToString("yyyyMM")) :
                                                               Convert.ToInt32(s.Date.ToString("yyyyMM")),
                                                      LocationName = s.Date <= input.FromDate.Date ?
                                                               (isSameYear ? input.FromDate.ToString("MMM") : input.FromDate.ToString("MMM-yyyy")) :
                                                               (isSameYear ? s.Date.ToString("MMM") : s.Date.ToString("MMM-yyyy")),
                                                      Debit = s.Balance > 0 ? s.Balance : 0,
                                                      Credit = s.Balance < 0 ? -s.Balance : 0,
                                                  });

                        var accountForPeriod = _journalItemRepository
                                                .GetAll()
                                                .AsNoTracking()
                                                .Where(u => u.Journal.Status == TransactionStatus.Publish)
                                                .Where(u => u.Journal.DateOnly > previousClose.EndDate.Value.Date && u.Journal.DateOnly <= input.ToDate.Date)
                                                .WhereIf(!input.AccountTypeIds.IsNullOrEmpty(), s => input.AccountTypeIds.Contains(s.Account.AccountTypeId))
                                                .WhereIf(!input.AccountIds.IsNullOrEmpty(), s => input.AccountIds.Contains(s.AccountId))
                                                .WhereIf(!input.LocationIds.IsNullOrEmpty(), s => s.Journal.LocationId.HasValue && input.LocationIds.Contains(s.Journal.LocationId.Value))
                                                .Select(u => new TrialBalanceAccountOutput
                                                {
                                                    Id = u.AccountId,
                                                    AccountName = u.Account.AccountName,
                                                    AccountCode = u.Account.AccountCode,
                                                    LocationId = u.Journal.Date <= input.FromDate.Date ?
                                                               Convert.ToInt32(input.FromDate.ToString("yyyyMM")) :
                                                               Convert.ToInt32(u.Journal.Date.ToString("yyyyMM")),
                                                    LocationName = u.Journal.Date <= input.FromDate.Date ?
                                                               (isSameYear ? input.FromDate.ToString("MMM") : input.FromDate.ToString("MMM-yyyy")) :
                                                               (isSameYear ? u.Journal.Date.ToString("MMM") : u.Journal.Date.ToString("MMM-yyyy")),
                                                    Debit = u.Debit,
                                                    Credit = u.Credit,
                                                });


                        accountList = await accountClosePeriodQuery.Concat(accountForPeriod)
                                            .OrderBy(s => s.AccountCode)
                                            .GroupBy(s => new { s.Id, s.AccountName, s.AccountCode, s.LocationId, s.LocationName })
                                            .Select(s => new TrialBalanceAccountOutput
                                            {
                                                Id = s.Key.Id,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                LocationId = s.Key.LocationId,
                                                LocationName = s.Key.LocationName,
                                                Debit = s.Sum(t => t.Debit - t.Credit)
                                            })
                                            .ToListAsync();
                    }

                }
            }


            var locations = accountList.Select(s => new { s.LocationId, s.LocationName }).Distinct().OrderBy(s => s.LocationId).ToList();

            var accounts = accountList
                           .GroupBy(s => new { s.Id, s.AccountName, s.AccountCode })
                           .Select(s => 
                           {
                               var total = s.Sum(t => t.Debit);

                               return new TrialBalanceAccountList
                               {
                                   Id = s.Key.Id,
                                   AccountCode = s.Key.AccountCode,
                                   AccountName = s.Key.AccountName,
                                   Debit = total > 0 ? total : 0,
                                   Credit = total < 0 ? -total : 0,
                                   TotalLocationColumns = locations.Select(l => 
                                   {
                                       var subTotal = s.Where(t => t.LocationId <= l.LocationId).Sum(t => t.Debit);

                                       return new TotalTrialBalanceLocationColumns
                                       {
                                           LocationId = l.LocationId,
                                           LocationName = l.LocationName,
                                           Debit = subTotal > 0 ? subTotal : 0,
                                           Credit = subTotal < 0 ? -subTotal : 0
                                       };
                                   })
                                   .ToDictionary(k => k.LocationId, v => v)
                               };
                           })
                           .OrderBy(s => s.AccountCode)
                           .ToList();

            return new TrialBalanceReportResultOutput
            {
                Accounts = accounts,
                Debit = accounts.Sum(t => t.Debit),
                Credit = accounts.Sum(t => t.Credit),
                locationSummaryHeader = locations.Select(s => new LocationSummaryDto { Id = s.LocationId, LocationName = s.LocationName }).ToList(),
                LocationColumnDic = locations.Select(l => new TotalTrialBalanceLocationColumns
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Debit = accounts.Sum(t => t.TotalLocationColumns[l.LocationId].Debit),
                    Credit = accounts.Sum(t => t.TotalLocationColumns[l.LocationId].Credit)
                })
                .ToDictionary(k => k.LocationId, v => v)
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_TrialBalance)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<TrialBalanceReportResultOutput> GetTrialBalanceReport(GetTrialBalanceInput input)
        {
            var result = input.ViewOption == ViewOption.Standard ? await GetTrialBalanceStandardReport(input)
                        : input.ViewOption == ViewOption.Location ? await GetTrialBalanceLocationReport(input)
                        : await GetTrialBalanceMonthlyReport(input);
            return result;
        }

        public ReportOutput GetReportTemplateTrialBalance()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ViewOption",
                        ColumnLength = 150,
                        ColumnTitle = "View Option",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = false,
                        IsDisplay = false,
                        DefaultValue = ((int)ViewOption.Standard).ToString()
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 150,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.Array,
                        SortOrder = 0,
                        Visible = false,
                        IsDisplay = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 400,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.Array,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 260,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.Money,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                },
                Groupby = "",
                HeaderTitle = L("TrialBalanceReport"),
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "Ledger",
                    ColumnTitle = "AccountLedgerTemplate",
                    DefaultValue = ReportType.ReportType_Ledger
                }
            };
            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_TrialBalance_Export_Excel)]
        public async Task<FileDto> ExportExcelTrialBalanceReport(GetTrialBalanceReportInput input)
        {

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Sum(s => s.ColumnName == "Account" ? 1 : 2);
            var sheetName = report.HeaderTitle;

            var trialBalanceOutput = await GetTrialBalanceReport(input);

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.FitToPage = true;
                ws.PrinterSettings.PaperSize = ePaperSize.A4; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                #region Row 2 Header
                var header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {   
                    if (i.ColumnName != "Account")
                    {
                        if (input.ViewOption == ViewOption.Standard)
                        {
                            AddTextToCell(ws, rowTableHeader, colHeaderTable, L("Debit"), true);
                            AddTextToCell(ws, rowTableHeader, colHeaderTable + 1, L("Credit"), true);
                            ws.Cells[rowTableHeader, colHeaderTable].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells[rowTableHeader, colHeaderTable + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                            MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader, colHeaderTable + 1, ExcelHorizontalAlignment.Center);

                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, L("Debit"), true);
                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable + 1, L("Credit"), true);
                            ws.Cells[rowTableHeader + 1, colHeaderTable].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells[rowTableHeader + 1, colHeaderTable + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength / 2);
                        ws.Column(colHeaderTable + 1).Width = ConvertPixelToInches(i.ColumnLength / 2);

                        colHeaderTable++;
                    }
                    else 
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                        if (input.ViewOption != ViewOption.Standard)
                        {
                            MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader + 1, colHeaderTable, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center);
                        }
                    }

                    colHeaderTable ++;
                }

                if (input.ViewOption != ViewOption.Standard)
                {
                    rowTableHeader++;
                }

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                
                var subTotalAccountTypeAddress = "";
                foreach (var acc in trialBalanceOutput.Accounts)
                {
                    int collumnCellBody = 1;

                    foreach (var t in reportCollumnHeader)
                    {
                        if (t.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, $"{acc.AccountCode} - {acc.AccountName}");
                        }
                        else if (t.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, collumnCellBody, acc.Debit);
                            collumnCellBody++;
                            AddNumberToCell(ws, rowBody, collumnCellBody, acc.Credit);
                        }
                        else if(acc.TotalLocationColumns != null)
                        {  
                            var ColumnName = Int32.Parse(t.ColumnName);
                            AddNumberToCell(ws, rowBody, collumnCellBody, acc.TotalLocationColumns.ContainsKey(ColumnName) ? acc.TotalLocationColumns[ColumnName].Debit : 0);
                            collumnCellBody++;
                            AddNumberToCell(ws, rowBody, collumnCellBody, acc.TotalLocationColumns.ContainsKey(ColumnName) ? acc.TotalLocationColumns[ColumnName].Credit : 0);
                        }
                        collumnCellBody++;
                    }

                    rowBody++;
                }


                #endregion Row Body

                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, L("Total"), true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            int rowFooter = rowTableHeader + 1;// get start row after 
                            var sumDebitFrom = ws.Cells[rowFooter, footerColNumber].Address;
                            var sumDebitTo = ws.Cells[footerRow - 1, footerColNumber].Address;

                            var sumDebitValue = $"SUM({sumDebitFrom}:{sumDebitTo})";
                            AddFormula(ws, footerRow, footerColNumber, sumDebitValue, true);

                            var sumCreditFrom = ws.Cells[rowFooter, footerColNumber + 1].Address;
                            var sumCreditTo = ws.Cells[footerRow - 1, footerColNumber + 1].Address;

                            var sumCreditValue = $"SUM({sumCreditFrom}:{sumCreditTo})";
                            AddFormula(ws, footerRow, footerColNumber + 1, sumCreditValue, true);

                        }

                        footerColNumber += i.ColumnName == "Account" ? 1 : 2;
                    }
                }
                #endregion Row Footer


                result.FileName = $"TrialBalance_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_TrialBalance_Export_Pdf)]
        public async Task<FileDto> ExportPdfTrialBalanceReport(GetTrialBalanceReportInput input)
        {
            var tenantId = AbpSession.TenantId;
            Tenant tenant;
            string formatDate;
            int rounding;
            User user;
            TrialBalanceReportResultOutput trialBalanceResult = null;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    tenant = await GetCurrentTenantAsync();
                    rounding = (await GetCurrentCycleAsync()).RoundingDigit;
                    formatDate = await _formatRepository.GetAll()
                             .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                             .Select(t => t.Web).FirstOrDefaultAsync();
                    user = await GetCurrentUserAsync();
                    trialBalanceResult = await GetTrialBalanceReport(input);
                }

                await uow.CompleteAsync();
            }

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "TrialBalance.pdf",
                    FileToken = "BalanceSheet.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentSubHeader = string.Empty;

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {   
                            if (input.ViewOption == ViewOption.Standard)
                            {
                                if(i.ColumnName == "Account")
                                {
                                    contentHeader += $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                                }
                                else
                                {
                                    contentHeader += $"<th width='{i.ColumnLength/2}'>{L("Debit")}</th>";
                                    contentHeader += $"<th width='{i.ColumnLength/2}'>{L("Credit")}</th>";
                                }
                            }
                            else
                            {
                                if(i.ColumnName == "Account")
                                {
                                    contentHeader += $"<th width='{i.ColumnLength}' rowspan='2'>{i.ColumnTitle}</th>";
                                }
                                else
                                {
                                    contentHeader += $"<th width='{i.ColumnLength}' colspan='2'>{i.ColumnTitle}</th>";

                                    contentSubHeader += $"<th width='{i.ColumnLength / 2}'>{L("Debit")}</th>";
                                    contentSubHeader += $"<th width='{i.ColumnLength / 2}'>{L("Credit")}</th>";
                                }
                            }

                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                if (!contentSubHeader.IsNullOrEmpty()) contentSubHeader = $"<tr>{contentSubHeader}</tr>";

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body            
                foreach (var account in trialBalanceResult.Accounts)
                {                   
                    var rowContent = $"<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var t in viewHeader)
                    {
                        if (t.Visible)
                        {
                            if (t.ColumnName == "Account")
                            {
                                rowContent += $"<td>{account.AccountCode} - {account.AccountName}</td>";
                            }
                            else if (t.ColumnName == "Total")
                            {
                                rowContent += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(account.Debit, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                rowContent += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(account.Credit, rounding, MidpointRounding.ToEven), rounding)}</td>";
                            }
                            else if(input.ViewOption != ViewOption.Standard && account.TotalLocationColumns != null)
                            {  
                                var ColumnName = Int32.Parse(t.ColumnName);
                                rowContent += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(account.TotalLocationColumns.ContainsKey(ColumnName) ? account.TotalLocationColumns[ColumnName].Debit  : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                rowContent += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(account.TotalLocationColumns.ContainsKey(ColumnName) ? account.TotalLocationColumns[ColumnName].Credit  : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                            }
                        }
                    }
                    rowContent += "</tr>";

                    contentBody += rowContent;
                }
                #endregion Row Body

                #region footer total
                var footerContent = "";

                foreach (var t in viewHeader)
                {
                    if (t.Visible)
                    {
                        if (t.ColumnName == "Account")
                        {
                            footerContent += $"<td>{L("Total")}</td>";
                        }
                        else if (t.AllowFunction != null)
                        {
                            if (t.ColumnName == "Total")
                            {
                                footerContent += $"<td style='text-align:right;'> {FormatNumberCurrency(Math.Round(trialBalanceResult.Debit, rounding, MidpointRounding.ToEven), rounding)} </td>";
                                footerContent += $"<td style='text-align:right'> {FormatNumberCurrency(Math.Round(trialBalanceResult.Credit, rounding, MidpointRounding.ToEven), rounding)} </td>";
                            }
                            else if(trialBalanceResult.LocationColumnDic != null)
                            {
                                var ColumnName = Int32.Parse(t.ColumnName);
                                footerContent += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(trialBalanceResult.LocationColumnDic.ContainsKey(ColumnName) ? trialBalanceResult.LocationColumnDic[ColumnName].Debit : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                                footerContent += $"<td style='text-align:right'>{FormatNumberCurrency(Math.Round(trialBalanceResult.LocationColumnDic.ContainsKey(ColumnName) ? trialBalanceResult.LocationColumnDic[ColumnName].Credit : 0, rounding, MidpointRounding.ToEven), rounding)}</td>";
                            }
                            else
                            {
                                footerContent += $"<td></td><td></td>";
                            }
                        }
                        else
                        {
                            footerContent += $"<td></td><td></td>";
                        }
                    }
                }

                if (!footerContent.IsNullOrEmpty())
                {
                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto;font-weight: bold; border-top: 1px solid #000;'> {footerContent}</tr>";
                }
                #endregion


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", contentSubHeader);
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }
        #endregion



        #region Journal Report
        public async Task<ReportOutput> GetReportTemplateJournal()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "No",
                        ColumnLength = 90,
                        ColumnTitle = "Trans No",
                        ColumnType = ColumnType.AutoNumber,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 120,
                        ColumnTitle = "Journal Type",
                        ColumnType = ColumnType.StatusCode,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 100,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 140,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Memo",
                        ColumnLength = 100,
                        ColumnTitle = "Memo",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 100,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 180,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.Array,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Debit",
                        ColumnLength = 110,
                        ColumnTitle = "Debit",
                        ColumnType = ColumnType.Array,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Credit",
                        ColumnLength = 110,
                        ColumnTitle = "Credit",
                        ColumnType = ColumnType.Array,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 120,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    //new CollumnOutput{
                    //    AllowGroupby = false,
                    //    AllowFilter = true,
                    //    ColumnName = "Location",
                    //    ColumnLength = 150,
                    //    ColumnTitle = "Location",
                    //    ColumnType = ColumnType.String,
                    //    SortOrder = 10,
                    //    Visible = true,
                    //    IsDisplay = true,
                    //    AllowShowHideFilter = true,
                    //    ShowHideFilter = true,
                    //},
                    //new CollumnOutput{
                    //    AllowGroupby = false,
                    //    AllowFilter = true,
                    //    ColumnName = "Partner",
                    //    ColumnLength = 150,
                    //    ColumnTitle = "Partner Name",
                    //    ColumnType = ColumnType.String,
                    //    SortOrder = 11,
                    //    Visible = true,
                    //    AllowFunction = null,
                    //    IsDisplay = true,
                    //    AllowShowHideFilter = true,
                    //    ShowHideFilter = true,
                    //},
                    //new CollumnOutput{
                    //    AllowGroupby = false,
                    //    AllowFilter = true,
                    //    ColumnName = "ItemName",
                    //    ColumnLength = 150,
                    //    ColumnTitle = "ItemName",
                    //    ColumnType = ColumnType.String,
                    //    SortOrder = 12,
                    //    Visible = true,
                    //    AllowFunction = null,
                    //    IsDisplay = true,
                    //    AllowShowHideFilter = true,
                    //    ShowHideFilter = true,
                    //},

                },
                Groupby = "",
                HeaderTitle = "Journal Report",
                Sortby = "",
            };


            var tenant = await GetCurrentTenantAsync();

            if(tenant != null && tenant.IsDebug)
            {
                result.ColumnInfo.AddRange(
                    new List<CollumnOutput>
                    {
                        new CollumnOutput {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Location",
                            ColumnLength = 140,
                            ColumnTitle = "Location",
                            ColumnType = ColumnType.String,
                            SortOrder = 11,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            AllowShowHideFilter = false,
                            ShowHideFilter = false,
                        },
                        new CollumnOutput {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Class",
                            ColumnLength = 140,
                            ColumnTitle = "Class",
                            ColumnType = ColumnType.String,
                            SortOrder = 12,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            AllowShowHideFilter = false,
                            ShowHideFilter = false,
                        },
                        new CollumnOutput {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Currency",
                            ColumnLength = 140,
                            ColumnTitle = "Currency",
                            ColumnType = ColumnType.String,
                            SortOrder = 13,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            AllowShowHideFilter = false,
                            ShowHideFilter = false,
                        },
                    }
                );
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Journal)]
        public async Task<PagedResultWithTotalColuumnsDto<JournalReportOutput>> GetListJournalReport(GetListJournalReportInput input)
        {
            var tenant = await GetCurrentTenantAsync();

            return tenant != null && tenant.IsDebug ? await GetListJournalReportDebugHelper(input, tenant) : await GetListJournalReportHelper(input);
            
        }


        private async Task<PagedResultWithTotalColuumnsDto<JournalReportOutput>> GetListJournalReportHelper(GetListJournalReportInput input)
        {

            var periodCycle = await GetCloseCyleAsync();

            var roundDigit = periodCycle.Where(t => t.StartDate.Date <= input.ToDate.Date)
                             .OrderByDescending(t => t.StartDate)
                             .Select(t => t.RoundingDigit).FirstOrDefault();

            input.MaxResultCount = input.IsLoadMore ? 5 : 10;

            var userQuery = GetUsers(input.Users);
            var accountQuery = GetAccountWithCode(input.ChartOfAccounts, input.AccountTypes);

            var query = from j in _journalRepository.GetAll()
                                          .Where(j => j.Status == TransactionStatus.Publish)
                                          .WhereIf(input.FromDate != null, j => input.FromDate.Value.Date <= j.Date.Date)
                                          .Where(j => j.Date.Date <= input.ToDate.Date)
                                          .WhereIf(!input.JournalType.IsNullOrEmpty(), j => input.JournalType.Contains(j.JournalType))
                                          .WhereIf(!input.Users.IsNullOrEmpty(), j => input.Users.Contains(j.CreatorUserId))
                                          .WhereIf(!input.Filter.IsNullOrEmpty(), j => 
                                                j.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                j.Reference.ToLower().Contains(input.Filter.ToLower()))
                                          .WhereIf(!input.Locations.IsNullOrEmpty(), j => j.LocationId != null && input.Locations.Contains(j.LocationId.Value))
                                          .AsNoTracking()

                        join ji in _journalItemRepository.GetAll()
                                   .AsNoTracking()
                                   .Select(s => new JournalItemDetailOutput
                                   {
                                       CreationTime = s.CreationTime,
                                       Id = s.Id,
                                       Credit = Math.Round(s.Credit, roundDigit),
                                       Account = new ChartAccountSummaryOutput
                                       {
                                           Id = s.AccountId,
                                           AccountCode = s.Account.AccountCode,
                                           AccountName = s.Account.AccountName
                                       },
                                       AccountId = s.AccountId,
                                       Debit = Math.Round(s.Debit, roundDigit),
                                       Description = s.Description,
                                       JournalId = s.JournalId
                                   })

                        on j.Id equals ji.JournalId
                        into jis
                        where input.ChartOfAccounts.IsNullOrEmpty() || jis.Any(p => input.ChartOfAccounts.Contains(p.AccountId))

                        select new JournalReportOutput
                        {
                            LocationId = j.LocationId,
                            RoundingDigit = roundDigit,
                            CreationTime = j.CreationTime,
                            Id = j.Id,
                            JournalType = j.JournalType.ToString(),
                            JournalCode = j.JournalType,
                            Date = j.Date,
                            Memo = j.Memo,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            JournalItems = jis.OrderByDescending(t => t.Debit).ThenBy(t => t.CreationTime).ToList(),
                            TotalCredit = jis.Sum(t => Math.Round(t.Credit, roundDigit)),
                            TotalDebit = jis.Sum(t => Math.Round(t.Debit, roundDigit)),
                            User = j.CreatorUserId.HasValue ? j.CreatorUser.UserName : "",
                            CreationTimeIndex = j.CreationTimeIndex,
                            BankTransferId = j.DepositId.HasValue ? j.Deposit.BankTransferId : j.WithdrawId.HasValue ? j.withdraw.BankTransferId : (Guid?) null,
                            TransferId = j.ItemIssueId.HasValue ? j.ItemIssue.TransferOrderId : j.ItemReceiptId.HasValue ? j.ItemReceipt.TransferOrderId : (Guid?) null,
                            ProductionId = j.ItemIssueId.HasValue ? j.ItemIssue.ProductionOrderId : j.ItemReceiptId.HasValue ? j.ItemReceipt.ProductionOrderId : (Guid?) null,
                            TransactionId = j.BillId.HasValue ? j.BillId : 
                                            j.InvoiceId.HasValue ? j.InvoiceId : 
                                            j.VendorCreditId.HasValue ? j.VendorCreditId :
                                            j.CustomerCreditId.HasValue ? j.CustomerCreditId : 
                                            j.ItemIssueId.HasValue ? j.ItemIssueId :
                                            j.ItemReceiptId.HasValue ? j.ItemReceiptId :
                                            j.DepositId.HasValue ? j.DepositId : 
                                            j.WithdrawId.HasValue ? j.WithdrawId : (Guid?) null
                        };

            var resultCount = 0;

            if (input.IsLoadMore == false)
            {
                resultCount = await query.CountAsync();
                if (resultCount == 0) return new PagedResultWithTotalColuumnsDto<JournalReportOutput>(0, new List<JournalReportOutput>(), new Dictionary<string, decimal>());
            }

            var sumOfColumns = new Dictionary<string, decimal>();

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await query.Select(t => new { t.TotalCredit, t.TotalDebit }).ToListAsync();
                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "Credit") sumOfColumns.Add(c, sumList.Sum(t => t.TotalCredit));
                    else if (c == "Debit") sumOfColumns.Add(c, sumList.Sum(u => u.TotalDebit));
                }
            }

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    query = query.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    query = query.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                }
            }

            var @entities = new List<JournalReportOutput>();
            if (input.NotUsePagination == true)
            {
                entities = await query.ToListAsync();
            }
            else
            {
                entities = await query.PageBy(input).ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<JournalReportOutput>(resultCount, @entities, sumOfColumns);
        }

        private async Task<PagedResultWithTotalColuumnsDto<JournalReportOutput>> GetListJournalReportDebugHelper(GetListJournalReportInput input, Tenant tenant)
        {

            var periodCycle = await GetCloseCyleAsync();

            var roundDigit = periodCycle.Where(t => t.StartDate.Date <= input.ToDate.Date)
                                        .OrderByDescending(t => t.StartDate)
                                        .Select(t => t.RoundingDigit).FirstOrDefault();

            var tenantId = AbpSession.TenantId.Value;

            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant);


            input.MaxResultCount = input.IsLoadMore ? 5 : 10;


            var query = from j in _journalRepository.GetAll()
                                         .Where(s => s.TenantId == tenantId)
                                         .Where(j => j.Status == TransactionStatus.Publish)                                        
                                         .WhereIf(input.FromDate != null, j => input.FromDate.Value.Date <= j.Date.Date)
                                         .Where(j => j.Date.Date <= input.ToDate.Date)
                                         .WhereIf(input.JournalType != null && input.JournalType.Count > 0, j => input.JournalType.Contains(j.JournalType))
                                         .WhereIf(input.Users != null && input.Users.Count > 0, j => input.Users.Contains(j.CreatorUserId))
                                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                                 j => j.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                 j.Reference.ToLower().Contains(input.Filter.ToLower()))
                                         .WhereIf(input.Locations != null && input.Locations.Count() > 0,
                                                  j => j.LocationId != null && input.Locations.Contains(j.LocationId.Value))
                                         .AsNoTracking()
                        join ji in _journalItemRepository.GetAll()
                                   .Where(s => s.TenantId == tenantId)
                                   .AsNoTracking()
                                   .Select(s => new JournalItemDetailOutput
                                   {
                                       CreationTime = s.CreationTime,
                                       Id = s.Id,
                                       Credit = Math.Round(s.Credit, roundDigit),
                                       Account = new ChartAccountSummaryOutput
                                       {
                                           Id = s.AccountId,
                                           AccountCode = s.Account.AccountCode,
                                           AccountName = s.Account.AccountName
                                       },
                                       AccountId = s.AccountId,
                                       Debit = Math.Round(s.Debit, roundDigit),
                                       Description = s.Description,
                                       JournalId = s.JournalId,
                                       TenantId = s.Account.TenantId.Value
                                   })
                        on j.Id equals ji.JournalId

                        into jis
                        where 
                            (tenant.CurrencyId.HasValue && j.CurrencyId != tenant.CurrencyId) ||
                            (j.LocationId.HasValue && j.Location.TenantId != tenantId) ||
                            (j.ClassId.HasValue && j.Class.TenantId != tenantId) ||
                            (j.BillId.HasValue && j.Bill.Vendor.TenantId != tenantId) ||
                            (j.VendorCreditId.HasValue && j.VendorCredit.Vendor.TenantId != tenantId) ||
                            (j.InvoiceId.HasValue && j.Invoice.Customer.TenantId != tenantId) ||
                            (j.CustomerCreditId.HasValue && j.CustomerCredit.Customer.TenantId != tenantId) ||
                            (j.DepositId.HasValue && j.Deposit.ReceiveFromVendorId.HasValue && j.Deposit.Vendor.TenantId != tenantId) ||
                            (j.WithdrawId.HasValue && j.withdraw.CustomerId.HasValue && j.withdraw.Customer.TenantId != tenantId) ||
                            jis.Any(s => s.TenantId != tenantId) 

                        select new JournalReportOutput
                        {
                            LocationId = j.LocationId,
                            Location = j.LocationId.HasValue ? j.Location.LocationName : "",
                            CurrencyId = j.CurrencyId,
                            Currency = j.Currency.Code,
                            ClassId = j.ClassId,
                            Class = j.ClassId.HasValue ? j.Class.ClassName : "",
                            RoundingDigit = roundDigit,
                            CreationTime = j.CreationTime,
                            Id = j.Id,
                            JournalType = j.JournalType.ToString(),
                            JournalCode = j.JournalType,
                            Date = j.Date,
                            Memo = j.Memo,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            JournalItems = jis.OrderByDescending(t => t.Debit).ThenBy(t => t.CreationTime).ToList(),
                            TotalCredit = jis.Sum(t => Math.Round(t.Credit, roundDigit)),
                            TotalDebit = jis.Sum(t => Math.Round(t.Debit, roundDigit)),
                            User = j.CreatorUserId.HasValue ? j.CreatorUser.UserName : "",
                            CreationTimeIndex = j.CreationTimeIndex,
                            BankTransferId = j.DepositId.HasValue ? j.Deposit.BankTransferId : j.WithdrawId.HasValue ? j.withdraw.BankTransferId : (Guid?)null,
                            TransferId = j.ItemIssueId.HasValue ? j.ItemIssue.TransferOrderId : j.ItemReceiptId.HasValue ? j.ItemReceipt.TransferOrderId : (Guid?)null,
                            ProductionId = j.ItemIssueId.HasValue ? j.ItemIssue.ProductionOrderId : j.ItemReceiptId.HasValue ? j.ItemReceipt.ProductionOrderId : (Guid?)null,
                            TransactionId = j.BillId.HasValue ? j.BillId :
                                            j.InvoiceId.HasValue ? j.InvoiceId :
                                            j.VendorCreditId.HasValue ? j.VendorCreditId :
                                            j.CustomerCreditId.HasValue ? j.CustomerCreditId :
                                            j.ItemIssueId.HasValue ? j.ItemIssueId :
                                            j.ItemReceiptId.HasValue ? j.ItemReceiptId :
                                            j.DepositId.HasValue ? j.DepositId :
                                            j.WithdrawId.HasValue ? j.WithdrawId : j.Id
                        };

            var resultCount = 0;

            if (input.IsLoadMore == false)
            {
                resultCount = await query.CountAsync();
                if (resultCount == 0) return new PagedResultWithTotalColuumnsDto<JournalReportOutput>(0, new List<JournalReportOutput>(), new Dictionary<string, decimal>());
            }

            var sumOfColumns = new Dictionary<string, decimal>();

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await query.Select(t => new { t.TotalCredit, t.TotalDebit }).ToListAsync();
                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "Credit") sumOfColumns.Add(c, sumList.Sum(t => t.TotalCredit));
                    else if (c == "Debit") sumOfColumns.Add(c, sumList.Sum(u => u.TotalDebit));
                }
            }

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    query = query.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    query = query.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                }
            }

            var @entities = new List<JournalReportOutput>();
            if (input.NotUsePagination == true)
            {
                entities = await query.ToListAsync();
            }
            else
            {
                entities = await query.PageBy(input).ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<JournalReportOutput>(resultCount, @entities, sumOfColumns);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Journal_Export_Excel)]
        public async Task<FileDto> ExportExcelJournalReport(JournalExportReportInput input)
        {
            input.NotUsePagination = true;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var journalData = (await GetListJournalReport(input)).Items;

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                string header = "";

                #region Row 2 Header
                if (!input.FromDate.HasValue || input.FromDate.Value.Date <= new DateTime(1970, 01, 01).Date)
                {
                    header = "As Of - " + input.ToDate.ToString("dd-MM-yyyy");
                }
                else
                {
                    header = input.FromDate.Value.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                }
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                foreach (var i in journalData)
                {
                    int collumnCellBody = 1;
                    foreach (var item in reportCollumnHeader)
                    {
                        if (item.ColumnName == "Account" || item.ColumnName == "Credit" || item.ColumnName == "Debit" || item.ColumnName == "Description")
                        {
                            int cloneRowBody = rowBody;//row inner sub items
                            //write sub item of account, credit or debit 
                            foreach (var sub in i.JournalItems)
                            {
                                WriteBodyJournalSubItems(ws, cloneRowBody, collumnCellBody, item, sub);
                                cloneRowBody += 1;
                            }
                        }
                        else
                        {
                            WriteBodyJournal(ws, rowBody, collumnCellBody, item, i, count);
                        }
                        collumnCellBody += 1;

                    }

                    //sub total of group by item
                    int collumnCellGroupBody = 1;
                    foreach (var g in reportCollumnHeader)// map with correct key of properties 
                    {
                        if (g.AllowFunction != null)
                        {
                            var fromCell = GetAddressName(ws, rowBody, collumnCellGroupBody);
                            var toCell = GetAddressName(ws, rowBody + i.JournalItems.Count - 1, collumnCellGroupBody);
                            if (footerGroupDict.ContainsKey(g.ColumnName))
                            {
                                footerGroupDict[g.ColumnName] += fromCell + ":" + toCell + ",";
                            }
                            else
                            {
                                footerGroupDict.Add(g.ColumnName, fromCell + ":" + toCell + ",");
                            }
                            AddFormula(ws, rowBody + i.JournalItems.Count, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                        }
                        collumnCellGroupBody += 1;
                    }

                    rowBody += 1 + i.JournalItems.Count;
                    count += 1;
                }

                #endregion Row Body

                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            //if (input.GroupBy != null)
                            //{
                            //sum custom range cell depend on group item
                            int rowFooter = rowTableHeader + 1;// get start row after 
                            var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                            AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                            //}
                            //else
                            //{
                            //    int rowFooter = rowTableHeader + 1;// get start row after header 
                            //    var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                            //    var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                            //    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                            //}
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Journal_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Journal_Export_Pdf)]
        public async Task<FileDto> ExportPDFJournalReport(JournalExportReportInput input)
        {
            if (!input.FromDate.HasValue) input.FromDate = DateTime.MinValue;

            input.NotUsePagination = true;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;
            var journals = await GetListJournalReport(input);
            var journalItems = journals.Items.ToList();
            var user = await GetCurrentUserAsync();
            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;
                //@todo replace our variable 
                exportHtml = exportHtml.Replace("{{companyName}}", tenant.LegalName);
                exportHtml = exportHtml.Replace("{{headerTitle}}", L("Journal"));
                var date = string.Empty;
                if (formatDate != null)
                {
                    date = (input.FromDate.Value.Date <= new DateTime(1970,1,1) ? "As Of" : input.FromDate.Value.ToString(formatDate)) + " - " + input.ToDate.Date.ToString(formatDate);
                }
                else
                {
                    date = (input.FromDate.Value.Date <= new DateTime(1970, 1, 1) ? "As Of" : input.FromDate.Value.ToString("dd-MM-yyyy")) + " - " +  input.ToDate.ToString("dd-MM-yyyy");
                }
                exportHtml = exportHtml.Replace("{{date}}", date);
                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());
                #region Row Body              

                var item = viewHeader;
                //var No = 1;

                var list = journalItems.SelectMany((j, index) =>
                {
                    var jNo = "";
                    decimal totalDr = 0;
                    decimal totalCr = 0;
                    var lists = j.JournalItems.Select(ji =>
                    {
                        var jItem = new
                        {
                            No = index + 1,
                            JournalNo = jNo == j.JournalNo ? "" : j.JournalNo,
                            Memo = jNo == j.JournalNo ? "" : j.Memo,
                            Date = jNo == j.JournalNo ? "" : j.Date.ToString(formatDate),
                            JournalType = jNo == j.JournalNo ? null : j.JournalType,
                            User = jNo == j.JournalNo ? "" : j.User,
                            JournalItem = ji,
                            rowSpan = j.JournalItems.Count() + 1,
                            Description = ji.Description,
                        };
                        totalDr += ji.Debit;
                        totalCr += ji.Credit;
                        jNo = j.JournalNo;
                        return jItem;
                    }).ToList();

                    var jTotal = new
                    {
                        No = index + 1,
                        JournalNo = jNo == j.JournalNo ? "" : j.JournalNo,
                        Memo = jNo == j.JournalNo ? "" : j.Memo,
                        Date = jNo == j.JournalNo ? "" : j.Date.ToString(formatDate),
                        JournalType = jNo == j.JournalNo ? null : j.JournalType,
                        User = jNo == j.JournalNo ? "" : j.User,
                        JournalItem = new JournalItemDetailOutput { Debit = totalDr, Credit = totalCr },
                        rowSpan = 1,
                        Description = "",
                    };

                    lists.Add(jTotal);

                    return lists;
                });


                foreach (var i in list)
                {
                    var rowSpan = i.JournalNo == "" ? "" : "rowspan='" + i.rowSpan + "'";

                    var subGroupItem = i.JournalItem.Account == null ? $"<tr style='font-weight: bold;' valign='top'>" : $"<tr valign='top'>";

                    foreach (var h in viewHeader)
                    {
                        if (h.Visible)
                        {
                            if (h.ColumnName == "No" && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>" + i.No + "</td>";
                            }
                            else if (h.ColumnName == "JournalNo" && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>" + i.JournalNo + "</td>";
                            }
                            else if (h.ColumnName == "Memo" && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>{i.Memo}</td>";
                            }
                            else if (h.ColumnName == "Description" && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>{i.Description}</td>";
                            }
                            else if (h.ColumnName == "Date" && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>{i.Date}</td>";
                            }
                            else if (h.ColumnName == "JournalType" && i.JournalType != null && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>{L(i.JournalType) } </td>";
                            }
                            else if (h.ColumnName == "User" && i.JournalNo != "")
                            {
                                subGroupItem += $"<td { rowSpan }>{i.User} </td>";
                            }
                            else if (h.ColumnName == "Account" && i.JournalItem.Account != null)
                            {
                                //var listListTd = "<td>";
                                //foreach (var ix in i.JournalItems)
                                //{
                                //    listListTd += $"<div title='{ix.Account.AccountCode + "-" + ix.Account.AccountName}'>{ix.Account.AccountCode + "-" + ix.Account.AccountName}</div>";
                                //}
                                //listListTd += "</td>";

                                subGroupItem += $"<td title='{i.JournalItem.Account.AccountCode + "-" + i.JournalItem.Account.AccountName}'>{i.JournalItem.Account.AccountCode + "-" + i.JournalItem.Account.AccountName}</td>";
                            }
                            else if (h.ColumnName == "Credit")
                            {
                                //var listListTd = "<td>";
                                //foreach (var ix in i.JournalItems)
                                //{
                                //    listListTd += $"<div style='text-align:right;'>{FormatNumberCurrency(Math.Round(ix.Credit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</div>";
                                //}
                                //listListTd += $"<div style='text-align:right;font-weight: bold;'>{FormatNumberCurrency(Math.Round(i.JournalItems.Sum(t => t.Credit), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</div>";
                                //listListTd += "</td>";
                                //subGroupItem += listListTd;
                                subGroupItem += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(i.JournalItem.Credit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                            }
                            else if (h.ColumnName == "Debit")
                            {
                                //var listListTd = "<td>";
                                //foreach (var ix in i.JournalItems)
                                //{
                                //    listListTd += $"<div style='text-align:right;'>{FormatNumberCurrency(Math.Round(ix.Debit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</div>";
                                //}
                                //listListTd += $"<div style='font-weight: bold;text-align:right;'>" +
                                //                $"{FormatNumberCurrency(Math.Round(i.JournalItems.Sum(t => t.Debit), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}" +
                                //                $"</div>";
                                //listListTd += "</td>";
                                subGroupItem += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(i.JournalItem.Debit, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                            }
                            else if (i.JournalItem.Account == null && (h.ColumnName == "Debit" || h.ColumnName == "Credit" || h.ColumnName == "Account"))
                            {
                                subGroupItem += $"<td></td>";
                            }
                        }
                    }
                    subGroupItem += "</tr>";
                    contentBody += subGroupItem;
                    //No++;
                }

                #endregion Row Body
                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (!string.IsNullOrEmpty(i.AllowFunction))/*listSum.Contains(i.ColumnName))*/
                            {
                                if (i.ColumnName == "Debit")

                                {
                                    tr += $"<td style='font-weight: bold;'>{FormatNumberCurrency(Math.Round(journals.Items.Sum(t => t.TotalCredit), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                if (i.ColumnName == "Credit")

                                {
                                    tr += $"<td style='font-weight: bold;'>{FormatNumberCurrency(Math.Round(journals.Items.Sum(t => t.TotalDebit), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                            }
                            else //none sum
                            {
                                if (index == 1)
                                {
                                    tr += $"<td style='font-weight: bold;'>{L("Total")}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";

                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer
                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate.Value, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"JournalReport.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }
        #endregion

        #region Ledger Report 
        public ReportOutput GetReportTemplateLedger()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 150,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = false,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "RelatedAccountType",
                        ColumnLength = 150,
                        ColumnTitle = "Related Account Type",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = false,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                     },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 250,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "RelatedAccount",
                        ColumnLength = 250,
                        ColumnTitle = "Related Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 19,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalDate",
                        ColumnLength = 120,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountCode",
                        ColumnLength = 150,
                        ColumnTitle = "Account Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountName",
                        ColumnLength = 150,
                        ColumnTitle = "Account Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 130,
                        ColumnTitle = "Journal Type",
                        ColumnType = ColumnType.StatusCode,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 150,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalMemo",
                        ColumnLength = 150,
                        ColumnTitle = "Memo",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 150,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 150,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OtherReference",
                        ColumnLength = 160,
                        ColumnTitle = "Other Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PartnerName",
                        ColumnLength = 150,
                        ColumnTitle = "Partner Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 150,
                        ColumnTitle = "Beginning",
                        ColumnType = ColumnType.Money,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Debit",
                        ColumnLength = 150,
                        ColumnTitle = "Debit",
                        ColumnType = ColumnType.Money,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Credit",
                        ColumnLength = 150,
                        ColumnTitle = "Credit",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Balance",
                        ColumnLength = 150,
                        ColumnTitle = "Balance",
                        ColumnType = ColumnType.Money,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccumBalance",
                        ColumnLength = 150,
                        ColumnTitle = "Accum Balance",
                        ColumnType = ColumnType.Money,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 150,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 16,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                },
                Groupby = "",
                HeaderTitle = "Ledger Report",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Ledger)]
        public async Task<PagedResultWithTotalColuumnsDto<AccountTransactionOutput>> GetListLedgerReport(GetListLedgerReportInput input)
        {
            var allCashAccountJournals = new List<Guid>();
            var journalIds = new List<Guid>();

            var hasRelativeAccountType = !input.RelatedAccountTypes.IsNullOrEmpty() || !input.RelatedAccounts.IsNullOrEmpty();

            if (hasRelativeAccountType)
            {
                //journalsIds = await _journalItemRepository.GetAll()
                //                    .Where(s => s.Journal.Status == TransactionStatus.Publish)
                //                    //.Where(s => input.FromDate.Date <= s.Journal.Date.Date && input.ToDate.Date >= s.Journal.Date.Date)
                //                    .Where(s => input.FromDate.Date <= s.Journal.DateOnly && input.ToDate.Date >= s.Journal.DateOnly)
                //                    .WhereIf(!input.RelatedAccountTypes.IsNullOrEmpty(), s => input.RelatedAccountTypes.Contains(s.Account.AccountTypeId))
                //                    .WhereIf(!input.RelatedAccounts.IsNullOrEmpty(), s => input.RelatedAccounts.Contains(s.AccountId))
                //                    .WhereIf(!input.Locations.IsNullOrEmpty(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                //                    .AsNoTracking()
                //                    .GroupBy(s => s.JournalId)
                //                    .Select(s => s.Key)
                //                    .ToListAsync();

                var journalIdQuery = from j in _journalRepository.GetAll().AsNoTracking()
                                         .Where(s => s.Status == TransactionStatus.Publish)
                                         .Where(s => s.Date.Date >= input.FromDate.Date && s.Date.Date <= input.ToDate.Date)
                                         .WhereIf(!input.Locations.IsNullOrEmpty(), s => input.Locations.Contains(s.LocationId.Value))
                                     join ji in _journalItemRepository.GetAll().AsNoTracking()
                                                 .Select(s => new
                                                 {
                                                     s.AccountId,
                                                     s.Account.AccountTypeId,
                                                     s.JournalId
                                                 })
                                     on j.Id equals ji.JournalId
                                     into js
                                     from i in js
                                     where input.RelatedAccountTypes.IsNullOrEmpty() || js.Any(r => input.RelatedAccountTypes.Contains(r.AccountTypeId))
                                     where input.RelatedAccounts.IsNullOrEmpty() || js.Any(r => input.RelatedAccounts.Contains(r.AccountId))
                                     where input.RelatedAccountTypes.IsNullOrEmpty() || js.All(s => input.RelatedAccountTypes.Contains(s.AccountTypeId)) || !input.RelatedAccountTypes.Contains(i.AccountTypeId)
                                     where input.RelatedAccounts.IsNullOrEmpty() || js.All(s => input.RelatedAccounts.Contains(s.AccountId)) || !input.RelatedAccounts.Contains(i.AccountId)
                                     let isAll = (!input.RelatedAccountTypes.IsNullOrEmpty() && js.All(s => input.RelatedAccountTypes.Contains(s.AccountTypeId))) ||
                                                 (!input.RelatedAccounts.IsNullOrEmpty() && js.All(s => input.RelatedAccounts.Contains(s.AccountId)))
                                     select new { i.JournalId, IsAll = isAll }
                                     into list
                                     group list by new { list.JournalId, list.IsAll }
                                     into g
                                     select g.Key;
                                    

                var cashJournals = await journalIdQuery.ToListAsync();
                journalIds = cashJournals.Select(s => s.JournalId).ToList();
                allCashAccountJournals = cashJournals.Where(s => s.IsAll).Select(s => s.JournalId).ToList();
            }


            var previousDate = input.FromDate.AddDays(-1);
            var accountOldPeriod = hasRelativeAccountType ? null :
                                 _accountTransactionManager.GetAccountQuery(null, previousDate, false, input.Locations)
                                 .Select(u => new AccountTransactionOutput
                                 {
                                     CreationTimeIndex = u.CreationTimeIndex,
                                     AccountId = u.AccountId,
                                     AccountName = u.AccountName,
                                     AccountCode = u.AccountCode,
                                     AccountTypeId = u.AccountTypeId,
                                     AccountType = u.Type,
                                     Debit = 0,//u.Balance > 0 ? u.Balance : 0,
                                     Credit = 0,//u.Balance < 0 ? u.Balance * -1 : 0,
                                     Balance = u.Balance,
                                     Beginning = u.Balance,
                                     JournalDate = previousDate,
                                     DateOnly = previousDate.Date,
                                     JournalNo = "",
                                     JournalType = (JournalType?)null,
                                     JournalStatus = (TransactionStatus?)null,
                                     CreationTime = (DateTime?)null,
                                     JournalId = (Guid?)null,
                                     JournalMemo = "",
                                     CreatorUserId = (long?)null,
                                 })
                                 .WhereIf(!input.AccountType.IsNullOrEmpty(), u => input.AccountType.Contains(u.AccountTypeId))
                                 .WhereIf(!input.ChartOfAccounts.IsNullOrEmpty(), u => input.ChartOfAccounts.Contains(u.AccountId))
                                 .WhereIf(!input.Users.IsNullOrEmpty(), u => input.Users.Contains(u.CreatorUserId))
                                 .WhereIf(!input.JournalType.IsNullOrEmpty(), u => input.JournalType.Contains(u.JournalType));

            var accountLedgerNewPeriod = _accountTransactionManager
                                        .GetAccountLedgerQuery(input.FromDate, input.ToDate, input.Locations)
                                        .WhereIf(!input.AccountType.IsNullOrEmpty(), u => input.AccountType.Contains(u.AccountTypeId))
                                        .WhereIf(!input.ChartOfAccounts.IsNullOrEmpty(), u => input.ChartOfAccounts.Contains(u.AccountId))
                                        .WhereIf(!input.Users.IsNullOrEmpty(), u => input.Users.Contains(u.CreatorUserId))
                                        .WhereIf(!input.JournalType.IsNullOrEmpty(), u => input.JournalType.Contains(u.JournalType));

            var accountAccTotalDict = new Dictionary<Guid, decimal>();

            var allQuery = !hasRelativeAccountType ?
                           accountOldPeriod.Concat(accountLedgerNewPeriod) :
                           accountLedgerNewPeriod
                           .Where(s => journalIds.Any(r => r == s.JournalId))
                           .WhereIf(!input.RelatedAccountTypes.IsNullOrEmpty(), s => allCashAccountJournals.Contains(s.JournalId.Value) || !input.RelatedAccountTypes.Contains(s.AccountTypeId))
                           .WhereIf(!input.RelatedAccounts.IsNullOrEmpty(), s => allCashAccountJournals.Contains(s.JournalId.Value) || !input.RelatedAccounts.Contains(s.AccountId));


            var result = allQuery
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                u.Reference.ToLower().Contains(input.Filter.ToLower()) );


            var @entities = new List<AccountTransactionOutput>();
            if (input.NotUsePagination != false)
            {
                entities = await result.OrderBy(i => i.AccountCode).
                                        //ThenBy(i => i.JournalDate.Date).
                                        ThenBy(i => i.DateOnly).
                                        ThenBy(t => t.CreationTimeIndex).
                                        Skip(input.SkipCount).
                                        Take(input.MaxResultCount).
                                        ToListAsync();


            }
            else
            {
                var query = await result.OrderBy(i => i.AccountCode)
                                       //.ThenBy(i => i.JournalDate.Date)
                                       .ThenBy(i => i.DateOnly)
                                       .ThenBy(t => t.CreationTimeIndex).ToListAsync();
                //long running must use async
                entities = await Task.Run(() =>
                {
                    var r = query.
                              Select(i =>
                              {
                                  if (!accountAccTotalDict.ContainsKey(i.AccountId))
                                  {
                                      accountAccTotalDict.Add(i.AccountId, 0);
                                  }
                                  i.AccumBalance = accountAccTotalDict[i.AccountId] += i.Balance;
                                  return i;
                              }).ToList();
                    return r;
                });
            }


            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }
            var sumOfColumns = new Dictionary<string, decimal>();

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var totalCredit = 0m;
                var totalDebit = 0m;
                var totalBeginning = 0m;
                var totalBalance = 0m;
                //var calculatedBalance = false;
                //var calculatedCredit = false;
                //var calculatedDebit = false;
                //var calculatedBeginning = false;

                var sumObj = await result
                    .Select(u => new { u.Credit, u.Debit, u.Beginning, u.Balance })
                    .GroupBy(s => 1)
                    .Select(s => new
                    { 
                        Credit = s.Sum(t => t.Credit), 
                        Debit = s.Sum(t => t.Debit), 
                        Beginning = s.Sum(t => t.Beginning),
                        Balance = s.Sum(t => t.Balance) 
                    })
                    .FirstOrDefaultAsync();

                //var sumList = await result.Select(u => new { u.Credit, u.Debit, u.Beginning, u.Balance }).ToListAsync();

                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "Credit")
                    {
                        //calculatedCredit = true;
                        //totalCredit = sumList.Select(s => s.Credit).Sum();
                        totalCredit = sumObj == null ? 0 : sumObj.Credit;
                        sumOfColumns.Add(c, totalCredit);
                    }
                    if (c == "Balance")
                    {
                        //calculatedBalance = true;
                        //totalBalance = sumList.Select(u => u.Balance).Sum();
                        totalBalance = sumObj == null ? 0 : sumObj.Balance;
                        sumOfColumns.Add(c, totalBalance);
                    }
                    else if (c == "Debit")
                    {
                        //calculatedDebit = true;
                        //totalDebit = sumList.Select(u => u.Debit).Sum();
                        totalDebit = sumObj == null ? 0 : sumObj.Debit;
                        sumOfColumns.Add(c, totalDebit);
                    }
                    else if (c == "Beginning")
                    {
                        //calculatedBeginning = true;
                        //totalBeginning = sumList.Select(u => u.Beginning).Sum();
                        totalBeginning = sumObj == null ? 0 : sumObj.Beginning;
                        sumOfColumns.Add(c, totalBeginning);
                    }
                    else if (c == "AccumBalance")
                    {
                        //totalBalance = calculatedBalance ? totalBalance : sumList.Select(u => u.Balance).Sum();
                        //totalDebit = calculatedDebit ? totalDebit : sumList.Select(u => u.Debit).Sum();
                        //totalCredit = calculatedCredit ? totalCredit : sumList.Select(u => u.Credit).Sum();
                        //totalBeginning = calculatedBeginning ? totalBeginning : sumList.Select(u => u.Beginning).Sum();
                        //var accumBalance = (totalBeginning + totalDebit) - totalCredit;
                        var accumBalance = sumObj == null ? 0 : sumObj.Beginning + sumObj.Debit - sumObj.Credit;
                        sumOfColumns.Add(c, accumBalance);
                    }

                }
            }


            return new PagedResultWithTotalColuumnsDto<AccountTransactionOutput>(resultCount, @entities, sumOfColumns);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Ledger_Export_Excel)]
        public async Task<FileDto> ExportExcelLedgerReport(LedgerExportReportInput input)
        {
            input.NotUsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var journalData = (await GetListLedgerReport(input)).Items;

            var result = new FileDto();

            ////Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                string header = "";
                #region Row 2 Header
                if (input.FromDate.Date <= new DateTime(1970, 01, 01).Date)
                {
                    header = "As Of - " + input.ToDate.ToString("dd-MM-yyyy");
                }
                else
                {
                    header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                }
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                var groupBy = new List<GetListLedgerReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Account":
                            groupBy = journalData
                                .GroupBy(t => t.AccountId)
                                .Select(t => new GetListLedgerReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.AccountCode + " - " + x.AccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyLedger(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {

                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);

                                if (item.ColumnName == "AccumBalance")
                                {
                                    AddFormula(ws, rowBody, collumnCellGroupBody, "=" + toCell, true);
                                    collumnCellGroupBody += 1;
                                    continue;
                                }


                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in journalData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)
                        {
                            WriteBodyLedger(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;

                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var g in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (g.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(g.ColumnName))
                                {
                                    footerGroupDict[g.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(g.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                //AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }

                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body

                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            int rowFooter = rowTableHeader + 1;// get start row after 
                            if (i.ColumnName == "AccumBalance")
                            {
                                var debit = GetAddressName(ws, footerRow, footerColNumber - 2);
                                var credit = GetAddressName(ws, footerRow, footerColNumber - 1);
                                var sumValue = "SUM(" + debit + "-" + credit + ")";
                                AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                            }
                            else
                            {
                                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                                {
                                    var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);

                                }
                                else
                                {
                                    var fromCell = GetAddressName(ws, rowTableHeader + 1, footerColNumber);
                                    var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);

                                    var sumValue = "SUM(" + fromCell + ":" + toCell + ")";
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }
                            }
                        }

                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Ledger_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Ledger_Export_Pdf)]
        public async Task<FileDto> ExportPDFLedgerReport(LedgerExportReportInput input)
        {
            input.NotUsePagination = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var journals = await GetListLedgerReport(input);
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }
                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body              
                var groupBy = new List<GetListAccountTransactionGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Account":
                            groupBy = journals.Items
                                .GroupBy(t => t.AccountCode)
                                .Select(t => new GetListAccountTransactionGroupByOutput
                                {
                                    KeyName = t.Select(x => x.AccountCode + " - " + x.AccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var i in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";

                            foreach (var h in viewHeader)
                            {
                                if (h.Visible)
                                {
                                    if (h.ColumnName == "JournalNo")
                                    {
                                        trGroup += $"<td>" + i.JournalNo + "</td>";
                                    }
                                    else if (h.ColumnName == "JournalMemo")
                                    {
                                        trGroup += $"<td>{i.JournalMemo}</td>";
                                    }
                                    else if (h.ColumnName == "Description")
                                    {
                                        trGroup += $"<td>{i.Description}</td>";
                                    }
                                    else if (h.ColumnName == "JournalDate")
                                    {
                                        trGroup += $"<td>{i.JournalDate.ToString(formatDate)}</td>";
                                    }
                                    else if (h.ColumnName == "JournalType" && i.JournalType != null)
                                    {
                                        trGroup += $"<td>{L(i.JournalType.ToString()) } </td>";
                                    }
                                    else if (h.ColumnName == "AccountCode")
                                    {
                                        trGroup += $"<td>{i.AccountCode} </td>";
                                    }
                                    else if (h.ColumnName == "AccountName")
                                    {
                                        trGroup += $"<td>{i.AccountName} </td>";
                                    }
                                    else if (h.ColumnName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(i.Beginning, rounding.RoundingDigit)} </td>";
                                    }
                                    else if (h.ColumnName == "Credit")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(i.Credit, rounding.RoundingDigit)} </td>";
                                    }
                                    else if (h.ColumnName == "Debit")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(i.Debit, rounding.RoundingDigit)} </td>";
                                    }
                                    else if (h.ColumnName == "Balance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(i.Balance, rounding.RoundingDigit)} </td>";
                                    }
                                    else if (h.ColumnName == "AccumBalance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(i.AccumBalance, rounding.RoundingDigit)} </td>";
                                    }
                                    else if (h.ColumnName == "Reference")
                                    {
                                        trGroup += $"<td>{i.Reference} </td>";
                                    }

                                    else if (h.ColumnName == "OtherReference")
                                    {
                                        trGroup += $"<td>{i.OtherReference} </td>";
                                    }
                                    else if (h.ColumnName == "PartnerName")
                                    {
                                        trGroup += $"<td>{i.PartnerName} </td>";
                                    }
                                    else if (h.ColumnName == "User")
                                    {
                                        trGroup += $"<td>{i.User} </td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "Credit")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Credit), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Debit")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Debit), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "AccumBalance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Select(x => x.AccumBalance).LastOrDefault(), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Balance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Balance), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;
                }
                else
                {
                    foreach (var i in journals.Items)
                    {
                        var subGroupItem = "<tr style='page-break-inside:avoid;'>";
                        foreach (var h in viewHeader)
                        {
                            if (h.Visible)
                            {
                                if (h.ColumnName == "JournalNo")
                                {
                                    subGroupItem += $"<td>" + i.JournalNo + "</td>";
                                }
                                else if (h.ColumnName == "JournalMemo")
                                {
                                    subGroupItem += $"<td>{i.JournalMemo}</td>";
                                }
                                else if (h.ColumnName == "Description")
                                {
                                    subGroupItem += $"<td>{i.Description}</td>";
                                }
                                else if (h.ColumnName == "JournalDate")
                                {
                                    subGroupItem += $"<td>{i.JournalDate.ToString(formatDate)}</td>";
                                }
                                else if (h.ColumnName == "JournalType" && i.JournalType != null)
                                {
                                    subGroupItem += $"<td>{L(i.JournalType.ToString()) } </td>";
                                }
                                else if (h.ColumnName == "AccountCode")
                                {
                                    subGroupItem += $"<td>{i.AccountCode} </td>";
                                }
                                else if (h.ColumnName == "AccountName")
                                {
                                    subGroupItem += $"<td>{i.AccountName} </td>";
                                }
                                else if (h.ColumnName == "Beginning")
                                {
                                    subGroupItem += $"<td align='right'>{FormatNumberCurrency(i.Beginning, rounding.RoundingDigit)} </td>";
                                }
                                else if (h.ColumnName == "Credit")
                                {
                                    subGroupItem += $"<td align='right'>{FormatNumberCurrency(i.Credit, rounding.RoundingDigit)} </td>";
                                }
                                else if (h.ColumnName == "Debit")
                                {
                                    subGroupItem += $"<td align='right'>{FormatNumberCurrency(i.Debit, rounding.RoundingDigit)} </td>";
                                }
                                else if (h.ColumnName == "Balance")
                                {
                                    subGroupItem += $"<td align='right'>{FormatNumberCurrency(i.Balance, rounding.RoundingDigit)} </td>";
                                }
                                else if (h.ColumnName == "AccumBalance")
                                {
                                    subGroupItem += $"<td align='right'>{FormatNumberCurrency(i.AccumBalance, rounding.RoundingDigit)} </td>";
                                }
                                else if (h.ColumnName == "Reference")
                                {
                                    subGroupItem += $"<td>{i.Reference} </td>";
                                }

                                else if (h.ColumnName == "OtherReference")
                                {
                                    subGroupItem += $"<td>{i.OtherReference} </td>";
                                }
                                else if (h.ColumnName == "PartnerName")
                                {
                                    subGroupItem += $"<td>{i.PartnerName} </td>";
                                }
                                else if (h.ColumnName == "User")
                                {
                                    subGroupItem += $"<td>{i.User} </td>";
                                }
                                else
                                {
                                    subGroupItem += $"<td></td>";
                                }
                            }
                        }
                        subGroupItem += "</tr>";
                        contentBody += subGroupItem;
                    }
                }
                #endregion Row Body
                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr  style='page-break-inside:avoid;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {

                            if (!string.IsNullOrEmpty(i.AllowFunction) && journals.TotalResult.ContainsKey(i.ColumnName))
                            {
                                tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(journals.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                            }
                            else //none sum
                            {
                                if (index == 1)
                                {
                                    tr += $"<td style='font-weight: bold;'>{L("Total")}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        #endregion


        #region Cash Report


        public ReportOutput GetCashReportTemplate()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 150,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                     },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 250,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = "Account",
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountCode",
                        ColumnLength = 150,
                        ColumnTitle = "Account Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountName",
                        ColumnLength = 150,
                        ColumnTitle = "Account Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalDate",
                        ColumnLength = 120,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 130,
                        ColumnTitle = "Journal Type",
                        ColumnType = ColumnType.StatusCode,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 150,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 150,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalMemo",
                        ColumnLength = 150,
                        ColumnTitle = "JournalMemo",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 150,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OtherJournalNos",
                        ColumnLength = 160,
                        ColumnTitle = "Other Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PartnerNames",
                        ColumnLength = 150,
                        ColumnTitle = "Partner Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 150,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 150,
                        ColumnTitle = "Beginning",
                        ColumnType = ColumnType.Money,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Debit",
                        ColumnLength = 150,
                        ColumnTitle = "Debit",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Credit",
                        ColumnLength = 150,
                        ColumnTitle = "Credit",
                        ColumnType = ColumnType.Money,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Balance",
                        ColumnLength = 150,
                        ColumnTitle = "Balance",
                        ColumnType = ColumnType.Money,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccumBalance",
                        ColumnLength = 150,
                        ColumnTitle = "Accum Balance",
                        ColumnType = ColumnType.Money,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginnings",
                        ColumnLength = 150,
                        ColumnTitle = "Beginnings",
                        ColumnType = ColumnType.Money,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Debits",
                        ColumnLength = 150,
                        ColumnTitle = "Debits",
                        ColumnType = ColumnType.Money,
                        SortOrder = 17,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Credits",
                        ColumnLength = 150,
                        ColumnTitle = "Credits",
                        ColumnType = ColumnType.Money,
                        SortOrder = 18,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Balances",
                        ColumnLength = 150,
                        ColumnTitle = "Balances",
                        ColumnType = ColumnType.Money,
                        SortOrder = 19,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccumBalances",
                        ColumnLength = 150,
                        ColumnTitle = "Accum Balances",
                        ColumnType = ColumnType.Money,
                        SortOrder = 20,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                },
                Groupby = "",
                HeaderTitle = "Cash Report",
                Sortby = "",
            };

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Cash)]
        public async Task<PagedResultWithTotalColuumnsDto<CashReportOutput>> GetListCashReport(GetListCashReportInput input)
        {
            var previousDate = input.FromDate.AddDays(-1);

            var accountOldPeriodQuery = _accountTransactionManager.GetCashAccountBeginningQueryHelper(previousDate, input.Locations)

                                 .WhereIf(input.AccountType != null && input.AccountType.Count > 0, u => input.AccountType.Contains(u.AccountTypeId))
                                 .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Count > 0, u => input.ChartOfAccounts.Contains(u.AccountId))
                                 .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.UserId))
                                 .WhereIf(input.JournalType != null && input.JournalType.Count > 0, u => input.JournalType.Contains(u.JournalType));

            var accountLedgerNewPeriodQuery = _accountTransactionManager.GetCashAccountByPeriodQueryHelper(input.FromDate, input.ToDate, input.Locations)
                                         .WhereIf(input.AccountType != null && input.AccountType.Count > 0, u => input.AccountType.Contains(u.AccountTypeId))
                                         .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Count > 0, u => input.ChartOfAccounts.Contains(u.AccountId))
                                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.UserId))
                                         .WhereIf(input.JournalType != null && input.JournalType.Count > 0, u => input.JournalType.Contains(u.JournalType));


            var currencyList = await (from c in _currencyRepository.GetAll().AsNoTracking()
                                      join q in accountOldPeriodQuery.Concat(accountLedgerNewPeriodQuery)
                                      on c.Id equals q.CurrencyId into currencies
                                      where currencies.Count() > 0
                                      select new { c.Id, c.Code }).ToListAsync();

            var accountOldPeriod = accountOldPeriodQuery.Select(s => new CashReportOutput
            {
                CreationTimeIndex = s.CreationTimeIndex,
                LocationId = s.LocationId,
                LocationName = s.LocationName,
                UserId = s.UserId,
                User = s.UserName,
                Reference = s.Reference,
                AccountId = s.AccountId,
                AccountName = s.AccountName,
                AccountCode = s.AccountCode,
                AccountTypeId = s.AccountTypeId,
                AccountType = s.Type,
                Beginning = s.Balance,
                Debit = 0,
                Credit = 0,
                Balance = s.Balance,
                Beginnings = s.MultiCurrencyBalance,
                Debits = 0,
                Credits = 0,
                Balances = s.MultiCurrencyBalance,
                CurrencyId = s.CurrencyId.Value,
                MultiCurrencyColumns = currencyList.Select(c => new CashReportMultiCurrencyColumn
                {
                    CurrencyId = c.Id,
                    CurrencyCode = c.Code,
                    Beginnings = s.CurrencyId == c.Id ? s.MultiCurrencyBalance : 0,
                    Balances = s.CurrencyId == c.Id ? s.MultiCurrencyBalance : 0,
                }).ToList(),
                JournalDate = previousDate,
                JournalNo = s.JournalNo,
                JournalType = s.JournalType,
                JournalId = s.JournalId,
                JournalMemo = s.JournalMemo,
                Description = s.Description,
                TransactionId = s.TransactionId,
                BankTransferId = s.BankTransferId,
                OtherReferences = s.OtherReferences,
                RoundDigits = s.RoundDigits,
            });



            var accountLedgerNewPeriod = accountLedgerNewPeriodQuery.Select(s => new CashReportOutput
            {
                CreationTimeIndex = s.CreationTimeIndex,
                LocationId = s.LocationId,
                LocationName = s.LocationName,
                UserId = s.UserId,
                User = s.UserName,
                Reference = s.Reference,
                AccountId = s.AccountId,
                AccountName = s.AccountName,
                AccountCode = s.AccountCode,
                AccountTypeId = s.AccountTypeId,
                AccountType = s.Type,
                Debit = s.Debit,
                Credit = s.Credit,
                Balance = s.Balance,
                Debits = s.MultiCurrencyDebit,
                Credits = s.MultiCurrencyCredit,
                Balances = s.MultiCurrencyBalance,
                CurrencyId = s.CurrencyId.Value,
                MultiCurrencyColumns = currencyList.Select(c => new CashReportMultiCurrencyColumn
                {
                    CurrencyId = c.Id,
                    CurrencyCode = c.Code,
                    Debits = s.CurrencyId == c.Id ? s.MultiCurrencyDebit : 0,
                    Credits = s.CurrencyId == c.Id ? s.MultiCurrencyCredit : 0,
                    Balances = s.CurrencyId == c.Id ? s.MultiCurrencyBalance : 0,
                }).ToList(),
                JournalDate = s.JournalDate,
                JournalNo = s.JournalNo,
                JournalType = s.JournalType,
                JournalId = s.JournalId,
                JournalMemo = s.JournalMemo,
                Description = s.Description,
                TransactionId = s.TransactionId,
                BankTransferId = s.BankTransferId,
                OtherReferences = s.OtherReferences,
                RoundDigits = s.RoundDigits,
            });


            var result = accountOldPeriod.Concat(accountLedgerNewPeriod)
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                u.JournalMemo.ToLower().Contains(input.Filter.ToLower()));


            var @entities = new List<CashReportOutput>();
            if (input.NotUsePagination != false)
            {
                entities = await result.OrderBy(i => i.AccountCode).
                                        ThenBy(i => i.JournalDate.Date).
                                        ThenBy(t => t.CreationTimeIndex).
                                        Skip(input.SkipCount).
                                        Take(input.MaxResultCount).
                                        ToListAsync();

            }
            else
            {
                var query = await result.OrderBy(i => i.AccountCode)
                                       .ThenBy(i => i.JournalDate.Date)
                                       .ThenBy(t => t.CreationTimeIndex).ToListAsync();

                var runningSumAccuBalanceDict = new Dictionary<string, decimal>();
                var runningSumAccuBalancesDict = new Dictionary<string, decimal>();

                //long running must use async
                entities = await Task.Run(() =>
                {
                    return query.Select(i =>
                    {
                        var key = "";
                        if (!input.GroupBy.IsNullOrWhiteSpace()) key = i.AccountId.ToString();
                        if (!runningSumAccuBalanceDict.ContainsKey(key)) runningSumAccuBalanceDict.Add(key, 0);                       
                        i.AccumBalance = runningSumAccuBalanceDict[key] += i.Beginning + i.Debit - i.Credit;

                        if (!runningSumAccuBalancesDict.ContainsKey(key)) runningSumAccuBalancesDict.Add(key, 0);
                        i.AccumBalances = runningSumAccuBalancesDict[key] += i.Beginnings + i.Debits - i.Credits;

                        i.MultiCurrencyColumns = i.MultiCurrencyColumns.Select(c =>
                        {
                            var item = c;

                            var keyC = $"{key}-{c.CurrencyId}";

                            if (!runningSumAccuBalancesDict.ContainsKey(keyC)) runningSumAccuBalancesDict.Add(keyC, 0);

                            item.AccumBalances = runningSumAccuBalancesDict[keyC] += c.Beginnings + c.Debits - c.Credits;

                            return item;
                        }).ToList();

                        return i;
                    }).ToList();
                });
               
            }


            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }
            var sumOfColumns = new Dictionary<string, decimal>();
            var sumOfMultiCurrencyColumns = new Dictionary<string, Dictionary<string, decimal>>();

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumResultQuery = await result.Select(s => new 
                { 
                    s.Beginning, 
                    s.Debit, 
                    s.Credit, 
                    s.Balance, 
                    s.MultiCurrencyColumns 
                }).ToListAsync();

                var sumResult = sumResultQuery.GroupBy(g => 1).Select(s => new
                {
                    TotalBeginning = s.Sum(r => r.Beginning),
                    TotalDebit = s.Sum(r => r.Debit),
                    TotalCredit = s.Sum(r => r.Credit),
                    TotalBalance = s.Sum(r => r.Balance),
                    MultiCurrencyColumns = s.SelectMany(r => r.MultiCurrencyColumns).ToList(),
                }).FirstOrDefault();

                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "Beginning")
                    {
                        sumOfColumns.Add(c, sumResult == null ? 0 : sumResult.TotalBeginning);
                    }
                    else if (c == "Debit")
                    {
                        sumOfColumns.Add(c, sumResult == null ? 0 : sumResult.TotalDebit);
                    }
                    else if (c == "Credit")
                    {
                        sumOfColumns.Add(c, sumResult == null ? 0 : sumResult.TotalCredit);
                    }
                    else if (c == "Balance")
                    {
                        sumOfColumns.Add(c, sumResult == null ? 0 : sumResult.TotalBalance);
                    }
                    else if (c == "AccumBalance")
                    {
                        //use this fromula to verify if the calucation is correct or not
                        //It should be has same result as sumResult.TotalBalance
                        sumOfColumns.Add(c, sumResult == null ? 0 : sumResult.TotalBeginning + sumResult.TotalDebit - sumResult.TotalCredit);
                    }
                    else if (c == "Beginnings" || c == "Debits" || c == "Credits" || c == "Balances" || c == "AccumBalances")
                    {
                        var multiCurrencyColumns = new Dictionary<string, decimal>();

                        foreach (var currency in currencyList)
                        {
                            multiCurrencyColumns.Add(
                                currency.Code, 
                                sumResult == null ? 0 : sumResult.MultiCurrencyColumns
                                .Where(s => s.CurrencyId == currency.Id)
                                .Sum(s => 
                                    c == "Beginnings" ? s.Beginnings : 
                                    c == "Debits" ? s.Debits : 
                                    c == "Credits" ? s.Credits :
                                    c == "Balances" ? s.Balances : 
                                    s.Beginnings + s.Debits - s.Credits)
                                );
                        }

                        sumOfMultiCurrencyColumns.Add(c, multiCurrencyColumns);
                    }
                }
            }

            var returnResult = new PagedResultWithTotalColuumnsDto<CashReportOutput>(resultCount, @entities, sumOfColumns);
            returnResult.TotalResults = sumOfMultiCurrencyColumns;

            return returnResult;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Cash_Export_Excel)]
        public async Task<FileDto> ExportExcelCashReport(ExportCashReportInput input)
        {
            input.NotUsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var journalData = (await GetListCashReport(input)).Items;

            var result = new FileDto();

            ////Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                

                var subCols = journalData == null ? null : journalData.First().MultiCurrencyColumns.Select(s => s.CurrencyCode).ToList();

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                string header = "";
                #region Row 2 Header
                if (input.FromDate.Date <= new DateTime(1970, 01, 01).Date)
                {
                    header = "As Of - " + input.ToDate.ToString("dd-MM-yyyy");
                }
                else
                {
                    header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                }
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    if (subCols.Count() > 1 && (
                        i.ColumnName == "Beginnings" ||
                        i.ColumnName == "Debits" ||
                        i.ColumnName == "Credits" ||
                        i.ColumnName == "Balances" ||
                        i.ColumnName == "AccumBalances"))
                    {
                        var colWidth = i.ColumnLength;
                        var subColWidth = i.ColumnLength / subCols.Count();

                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(colWidth);
                        MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader, colHeaderTable + subCols.Count() - 1, ExcelHorizontalAlignment.Center);

                        foreach (var subCol in subCols)
                        {
                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, subCol, true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(subColWidth);
                            colHeaderTable += 1;
                        }
                    }
                    else
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                        colHeaderTable += 1;
                    }

                }

                if (subCols.Count() > 1) rowTableHeader += 1;
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                var groupBy = new List<GetListCashReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Account":
                            groupBy = journalData
                                .GroupBy(t => t.AccountId)
                                .Select(t => new GetListCashReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.AccountCode + " - " + x.AccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                if (subCols.Count() > 1 && (
                                   item.ColumnName == "Beginnings" ||
                                   item.ColumnName == "Debits" ||
                                   item.ColumnName == "Credits" ||
                                   item.ColumnName == "Balances" ||
                                   item.ColumnName == "AccumBalances"))
                                {
                                    foreach (var subCol in subCols)
                                    {
                                        var model = i.MultiCurrencyColumns.FirstOrDefault(s => s.CurrencyCode == subCol);
                                        var cellValue = model.GetType().GetProperty(item.ColumnName)?.GetValue(model, null);
                                        var value = cellValue ?? 0;
                                        AddCellValue(ws, rowBody, collumnCellBody, item, value, i.RoundDigits);
                                        collumnCellBody += 1;
                                    }
                                }
                                else
                                {
                                    WriteBodyCashReport(ws, rowBody, collumnCellBody, item, i, count);
                                    collumnCellBody += 1;
                                }
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                if (subCols.Count() > 1 && (
                                   item.ColumnName == "Beginnings" ||
                                   item.ColumnName == "Debits" ||
                                   item.ColumnName == "Credits" ||
                                   item.ColumnName == "Balances" ))
                                {
                                    foreach (var subCol in subCols)
                                    {
                                        var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                        var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);

                                        var keyCol = item.ColumnName + "-" + subCol;

                                        if (footerGroupDict.ContainsKey(keyCol))
                                        {
                                            footerGroupDict[keyCol] += fromCell + ":" + toCell + ",";
                                        }
                                        else
                                        {
                                            footerGroupDict.Add(keyCol, fromCell + ":" + toCell + ",");
                                        }

                                        AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);


                                        collumnCellGroupBody ++;
                                    }

                                    collumnCellGroupBody--;
                                }
                                if(item.ColumnName == "AccumBalance" || item.ColumnName == "AccumBalances")
                                {

                                }
                                else
                                {
                                    var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                    var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);

                                    if (footerGroupDict.ContainsKey(item.ColumnName))
                                    {
                                        footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                    }
                                    else
                                    {
                                        footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                    }
                                    AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in journalData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)
                        {
                            if (subCols.Count() > 1 && (
                                   item.ColumnName == "Beginnings" ||
                                   item.ColumnName == "Debits" ||
                                   item.ColumnName == "Credits" ||
                                   item.ColumnName == "Balances" ||
                                   item.ColumnName == "AccumBalances"))
                            {
                                foreach (var subCol in subCols)
                                {
                                    var model = i.MultiCurrencyColumns.FirstOrDefault(s => s.CurrencyCode == subCol);
                                    var cellValue = model.GetType().GetProperty(item.ColumnName)?.GetValue(model, null);
                                    var value = cellValue ?? 0;
                                    AddCellValue(ws, rowBody, collumnCellBody, item, value, i.RoundDigits);
                                    collumnCellBody += 1;
                                }
                            }
                            else
                            {
                                WriteBodyCashReport(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                        }


                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body

                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            int rowFooter = rowTableHeader + 1;// get start row after 
                            if (i.ColumnName == "AccumBalance" || i.ColumnName == "AccumBalances")
                            {
                                
                            }
                            else
                            {
                                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                                {
                                    if (subCols.Count() > 1 && (
                                       i.ColumnName == "Beginnings" ||
                                       i.ColumnName == "Debits" ||
                                       i.ColumnName == "Credits" ||
                                       i.ColumnName == "Balances"))
                                    {
                                        foreach (var subCol in subCols)
                                        {
                                            var key = i.ColumnName + "-" + subCol;
                                            var sumValue = "SUM(" + footerGroupDict[key] + ")";
                                            AddFormula(ws, footerRow, footerColNumber, sumValue, true);

                                            footerColNumber++;
                                        }
                                        footerColNumber--;
                                    }
                                    else
                                    {
                                        var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                        AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                    }
                                }
                                else
                                {
                                    if (subCols.Count() > 1 && (
                                       i.ColumnName == "Beginnings" ||
                                       i.ColumnName == "Debits" ||
                                       i.ColumnName == "Credits" ||
                                       i.ColumnName == "Balances"))
                                    {
                                        foreach (var subCol in subCols)
                                        {
                                            var fromCell = GetAddressName(ws, rowTableHeader + 1, footerColNumber);
                                            var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                            var sumValue = "SUM(" + fromCell + ":" + toCell + ")";
                                            AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                            footerColNumber++;
                                        }
                                        footerColNumber--;
                                    }
                                    else
                                    {
                                        var fromCell = GetAddressName(ws, rowTableHeader + 1, footerColNumber);
                                        var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);

                                        var sumValue = "SUM(" + fromCell + ":" + toCell + ")";
                                        AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                    }
                                }
                            }
                        }

                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"{sheetName}_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Cash_Export_Pdf)]
        public async Task<FileDto> ExportPDFCashReport(ExportCashReportInput input)
        {
            input.NotUsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }
            var digit = (await GetCurrentCycleAsync()).RoundingDigit;
            var user = await GetCurrentUserAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;

            var saleInvoices = await GetListCashReport(input);

            var subCols = saleInvoices.Items == null ? null : saleInvoices.Items.First().MultiCurrencyColumns.Select(s => s.CurrencyCode).ToList();
          
            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                var multiCurrencyHeader = subCols.Count() > 1 ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";
                            if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                            {
                                var index = 0;
                                var subColWidth = Convert.ToInt32(i.ColumnLength / saleInvoices.TotalResults[i.ColumnName].Count());
                                rowHeader = $"<th  colspan='{saleInvoices.TotalResults[i.ColumnName].Count()}' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                                foreach (var subHeader in saleInvoices.TotalResults[i.ColumnName])
                                {
                                    if (index > 0) reportCountColHead++;
                                    multiCurrencyHeader += $"<th style='width: {subColWidth}px;'>{subHeader.Key}</th>";
                                    index++;
                                }
                            }
                            else
                            {
                                rowHeader = $"<th rowspan='2' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                            }
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                            reportCountColHead--;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                multiCurrencyHeader += subCols.Count() > 1 ? $"</tr>" : "";
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body              

                var groupBy = new List<GetListCashReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {                       
                        case "Account":
                            groupBy = saleInvoices.Items
                                .GroupBy(t => t.AccountId)
                                .Select(t => new GetListCashReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.AccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {
                    var trGroup = "";
                    foreach (var k in groupBy)
                    {
                        trGroup += $"<tr style='page-break-before: auto; page-break-after: auto;'>" +
                                   $"<td style='font-weight: bold;' colspan='{reportCountColHead}'> " + k.KeyName +
                                   $" </td></tr>";
                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "AccountName")
                                    {
                                        trGroup += $"<td>{row.AccountName}</td>";
                                    }
                                    else if (i.ColumnName == "AccountCode")
                                    {
                                        trGroup += $"<td>{row.AccountCode}</td>";
                                    }
                                    else if (i.ColumnName == "JournalNo")
                                    {
                                        trGroup += $"<td>{row.JournalNo}</td>";
                                    }
                                    else if (i.ColumnName == "Beginning")
                                    {
                                       trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Beginning, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "Debit")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Debit, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "Credit")
                                    {
                                       trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Credit, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "Balance")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Balance, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "AccumBalance")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.AccumBalances, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "Beginnings")
                                    {
                                        if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var subHeader in subCols)
                                            {
                                                var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Beginnings);
                                                trGroup += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                            }
                                        }
                                        else
                                        {
                                            trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Beginnings, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else if (i.ColumnName == "Debits")
                                    {
                                        if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var subHeader in subCols)
                                            {
                                                var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Debits);
                                                trGroup += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                            }
                                        }
                                        else
                                        {
                                            trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Debits, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else if (i.ColumnName == "Credits")
                                    {
                                        if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var subHeader in subCols)
                                            {
                                                var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Credits);
                                                trGroup += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                            }
                                        }
                                        else
                                        {
                                            trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Credits, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else if (i.ColumnName == "Balances")
                                    {
                                        if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var subHeader in subCols)
                                            {
                                                var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Balances);
                                                trGroup += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                            }
                                        }
                                        else
                                        {
                                            trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Balances, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else if (i.ColumnName == "AccumBalances")
                                    {
                                        if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var subHeader in subCols)
                                            {
                                                var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.AccumBalances);
                                                trGroup += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                            }
                                        }
                                        else
                                        {
                                            trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.AccumBalances, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else if (i.ColumnName == "JournalDate")
                                    {
                                        trGroup += $"<td>{row.JournalDate.ToString(formatDate)}</td>";
                                    }
                                    else if (i.ColumnName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (i.ColumnName == "PartnerNames")
                                    {
                                        trGroup += $"<td>{row.PartnerNames}</td>";
                                    }
                                    else if (i.ColumnName == "OtherJournalNos")
                                    {
                                        trGroup += $"<td>{row.OtherJournalNos}</td>";
                                    }
                                    else if (i.ColumnName == "JournalType" && row.JournalType != null)
                                    {
                                        trGroup += $"<td>{L(row.JournalType.ToString()) } </td>";
                                    }                                    
                                    else if (i.ColumnName == "User")
                                    {
                                        trGroup += $"<td>{row.User}</td>";
                                    }
                                    else if (i.ColumnName == "JournalMemo")
                                    {
                                        trGroup += $"<td>{row.JournalMemo}</td>";
                                    }
                                    else if (i.ColumnName == "Description")
                                    {
                                        trGroup += $"<td>{row.Description}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))/*listSum.Contains(i.ColumnName))*/
                                {
                                    if ((i.ColumnName == "Beginnings" || i.ColumnName == "Debits" || i.ColumnName == "Credits" || i.ColumnName == "Balances" || i.ColumnName == "AccumBalances")
                                        && subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = k.Items.SelectMany(s => s.MultiCurrencyColumns.Where(r => r.CurrencyCode == subHeader))
                                                        .Sum(s => i.ColumnName == "Beginnings" ? s.Beginnings : i.ColumnName == "Debits" ? s.Debits : i.ColumnName == "Credits" ? s.Credits : i.ColumnName == "Balances" ? s.Balances : 0);

                                            if (i.ColumnName == "AccumBalances") trGroup += $"<td></td>";
                                            else trGroup += $"<td style='font-weight: bold; text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        if (i.ColumnName == "Beginnings" || i.ColumnName == "Debits" || i.ColumnName == "Credits" || i.ColumnName == "Balances")
                                        {
                                            trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(s => i.ColumnName == "Beginnings" ? s.Beginnings : i.ColumnName == "Debits" ? s.Debits : i.ColumnName == "Credits" ? s.Credits : i.ColumnName == "Balances" ? s.Balances : 0), digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                        else if (i.ColumnName == "Beginning" || i.ColumnName == "Debit" || i.ColumnName == "Credit" || i.ColumnName == "Balance")
                                        {
                                            trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(s => i.ColumnName == "Beginning" ? s.Beginning : i.ColumnName == "Debit" ? s.Debit : i.ColumnName == "Credit" ? s.Credit : i.ColumnName == "Balance" ? s.Balance : 0), digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                        else 
                                        {
                                            trGroup += $"<td style='font-weight: bold;text-align:right;'></td>";
                                        }
                                    }
                                }
                                else //none sum
                                {
                                    trGroup += $"<td></td>";
                                }

                            }
                        }
                        trGroup += "</tr>";
                    }
                    contentBody += trGroup;
                }
                else // write body no group by
                {
                    foreach (var row in saleInvoices.Items)
                    {
                        var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "AccountName")
                                {
                                    tr += $"<td>{row.AccountName}</td>";
                                }
                                else if (i.ColumnName == "AccountCode")
                                {
                                    tr += $"<td>{row.AccountCode}</td>";
                                }
                                else if (i.ColumnName == "JournalNo")
                                {
                                    tr += $"<td>{row.JournalNo}</td>";
                                }
                                else if (i.ColumnName == "Beginning")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Beginning, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "Debit")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Debit, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "Credit")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Credit, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "Balance")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Balance, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "AccumBalance")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.AccumBalances, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "Beginnings")
                                {
                                    if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Beginnings);
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Beginnings, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                }
                                else if (i.ColumnName == "Debits")
                                {
                                    if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Debits);
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Debits, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                }
                                else if (i.ColumnName == "Credits")
                                {
                                    if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Credits);
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Credits, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                }
                                else if (i.ColumnName == "Balances")
                                {
                                    if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Balances);
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Balances, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                }
                                else if (i.ColumnName == "AccumBalances")
                                {
                                    if (subCols.Count() > 1 && saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = row.MultiCurrencyColumns.Where(s => s.CurrencyCode == subHeader).Sum(s => s.AccumBalances);
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.AccumBalances, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                }
                                else if (i.ColumnName == "JournalDate")
                                {
                                    tr += $"<td>{row.JournalDate.ToString(formatDate)}</td>";
                                }
                                else if (i.ColumnName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (i.ColumnName == "PartnerNames")
                                {
                                    tr += $"<td>{row.PartnerNames}</td>";
                                }
                                else if (i.ColumnName == "OtherJournalNos")
                                {
                                    tr += $"<td>{row.OtherJournalNos}</td>";
                                }
                                else if (i.ColumnName == "JournalType" && row.JournalType != null)
                                {
                                    tr += $"<td>{L(row.JournalType.ToString()) } </td>";
                                }
                                else if (i.ColumnName == "User")
                                {
                                    tr += $"<td>{row.User}</td>";
                                }
                                else if (i.ColumnName == "JournalMemo")
                                {
                                    tr += $"<td>{row.JournalMemo}</td>";
                                }
                                else if (i.ColumnName == "Description")
                                {
                                    tr += $"<td>{row.Description}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (index == 0)
                            {
                                tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    if (saleInvoices.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subCol in subCols)
                                        {
                                            var total = saleInvoices.TotalResults[i.ColumnName].ContainsKey(subCol) ? saleInvoices.TotalResults[i.ColumnName][subCol] : 0;
                                            tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(total, digit, MidpointRounding.ToEven), digit)}</td>";
                                        }
                                    }
                                    else if (saleInvoices.TotalResult.ContainsKey(i.ColumnName))
                                    {
                                        tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(saleInvoices.TotalResult[i.ColumnName], digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", multiCurrencyHeader);

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        #endregion

        #region CashFlow Report


        public ReportOutput GetCashFlowReportTemplate()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                    
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ViewOption",
                        ColumnLength = 150,
                        ColumnTitle = "View Option",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = false,
                        IsDisplay = false,
                        DefaultValue = ((int)ViewOption.Standard).ToString()
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 150,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                     },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 450,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DefaultValue = "Account",
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },                   
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Debit",
                        ColumnLength = 150,
                        ColumnTitle = "Debit",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Credit",
                        ColumnLength = 150,
                        ColumnTitle = "Credit",
                        ColumnType = ColumnType.Money,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 150,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.Money,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        DisableDefault = false,
                    },                    
                    
                },
                Groupby = "",
                HeaderTitle = "Cash Flow Report",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "Ledger",
                    ColumnTitle = "AccountLedgerTemplate",
                    DefaultValue = ReportType.ReportType_Ledger
                },

                DefaultTemplate2 = new DefaultSaveTemplateOutput
                {
                    ColumnName = "ProfitAndLoss",
                    ColumnTitle = "ProfitAndLossReportTemplate",
                    DefaultValue = ReportType.ReportType_ProfitAndLoss
                }
            };

            return result;
        }

        #region old code
        //[AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow)]
        //public async Task<CashFlowReportResultOutput> GetListCashFlowReport(GetListCashFlowReportInput input)
        //{
        //    return input.ViewOption == ViewOption.Month ? await GetListCashFlowMonthlyReport(input) :
        //           input.ViewOption == ViewOption.Location ? await GetListCashFlowLocationReport(input) :
        //           await GetListCashFlowStandardReport(input);
        //}

        //private async Task<CashFlowReportResultOutput> GetListCashFlowStandardReport(GetListCashFlowReportInput input)
        //{
        //    var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
        //                               .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.AccountTypeName.Trim()))
        //                               .AsNoTracking()
        //                               .Select(s => s.Id)
        //                               .ToListAsync();

        //    var previousDate = input.FromDate.AddDays(-1);

        //    var previousClose = await GetPreviousCloseCyleAsync(previousDate);

        //    var cashBankFromClosePeriodQuery = _accountCloseRepository.GetAll()
        //                                       .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
        //                                       .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.Account.AccountType.AccountTypeName.Trim()))
        //                                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
        //                                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                                       .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
        //                                       .AsNoTracking()
        //                                       .Select(s => new CashFlowReportOutput
        //                                       {
        //                                           AccountId = s.AccountId,
        //                                           AccountCode = s.Account.AccountCode,
        //                                           AccountName = s.Account.AccountName,
        //                                           AccountTypeId = s.Account.AccountTypeId,
        //                                           AccountType = s.Account.AccountType.Type,
        //                                           AccountTypeName = s.Account.AccountName.Trim(),
        //                                           Debit = s.Debit,
        //                                           Credit = s.Credit,
        //                                           Total = s.Balance,
        //                                       });

        //    var cashBankFromOldPeriodQuery = _journalItemRepository.GetAll()
        //                                      .Where(s => s.Journal.Status == TransactionStatus.Publish)
        //                                      .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
        //                                      .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.Account.AccountType.AccountTypeName.Trim()))
        //                                      .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
        //                                      .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                                      .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
        //                                      .AsNoTracking()
        //                                      .Select(s => new CashFlowReportOutput
        //                                      {
        //                                          AccountId = s.AccountId,
        //                                          AccountCode = s.Account.AccountCode,
        //                                          AccountName = s.Account.AccountName,
        //                                          AccountTypeId = s.Account.AccountTypeId,
        //                                          AccountType = s.Account.AccountType.Type,
        //                                          AccountTypeName = s.Account.AccountName.Trim(),
        //                                          Debit = s.Debit,
        //                                          Credit = s.Credit,
        //                                          Total = s.Debit - s.Credit,
        //                                      });


        //    var allAccountQuery = await _journalItemRepository.GetAll()
        //                            .Where(s => s.Journal.Status == TransactionStatus.Publish)
        //                            .Where(s => s.Journal.Date.Date >= input.FromDate.Date && s.Journal.Date.Date <= input.ToDate.Date)
        //                            .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
        //                            .AsNoTracking()
        //                            .Select(ji => new CashFlowReportOutput
        //                            {
        //                                AccountId = ji.AccountId,
        //                                AccountCode = ji.Account.AccountCode,
        //                                AccountName = ji.Account.AccountName,
        //                                AccountTypeId = ji.Account.AccountTypeId,
        //                                AccountType = ji.Account.AccountType.Type,
        //                                AccountTypeName = ji.Account.AccountType.AccountTypeName.Trim(),
        //                                Debit = ji.Debit,
        //                                Credit = ji.Credit,
        //                                Total = ji.Debit - ji.Credit,
        //                                JournalId = ji.JournalId,
        //                            })
        //                            .ToListAsync();

        //    var cashBankList = allAccountQuery
        //                        .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.AccountTypeName))
        //                        .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.AccountTypeId))
        //                        .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                        .ToList();

        //    var cashBankJournalIds = cashBankList.Select(s => s.JournalId).Distinct().ToList();

        //    var cashBankCurrrentPeriodQuery = allAccountQuery
        //                                       .Where(s => cashBankJournalIds.Any(r => r == s.JournalId))
        //                                       .Where(ji => !CorarlERPConsts.CashBankAccountTypes.Contains(ji.AccountTypeName))
        //                                       .Select(ji => new CashFlowReportOutput
        //                                       {
        //                                           AccountId = ji.AccountId,
        //                                           AccountCode = ji.AccountCode,
        //                                           AccountName = ji.AccountName,
        //                                           AccountTypeId = ji.AccountTypeId,
        //                                           AccountType = ji.AccountType,
        //                                           AccountTypeName = ji.AccountTypeName,
        //                                           Debit = ji.Credit,
        //                                           Credit = ji.Debit,
        //                                           Total = ji.Credit - ji.Debit,
        //                                           JournalId = ji.JournalId,
        //                                       });

        //    var beginningList = await cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery).ToListAsync();

        //    var beginning = beginningList.GroupBy(s => 1)
        //                    .Select(s => new CashFlowReportOutput
        //                    {
        //                        Debit = s.Sum(r => r.Debit),
        //                        Credit = s.Sum(r => r.Credit),
        //                        Total = s.Sum(r => r.Total),
        //                    })
        //                    .FirstOrDefault() ??
        //                    new CashFlowReportOutput();

        //    var cashInOutFlow = from s in cashBankCurrrentPeriodQuery
        //                        orderby s.AccountCode, s.Debit
        //                        group s by new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, CashIn = s.Debit != 0 }
        //                        into g
        //                        select new CashFlowReportOutput
        //                        {
        //                            AccountTypeId = g.Key.AccountTypeId,
        //                            AccountType = g.Key.AccountType,
        //                            AccountTypeName = g.Key.AccountTypeName,
        //                            AccountCode = g.Key.AccountCode,
        //                            AccountName = g.Key.AccountName,
        //                            AccountId = g.Key.AccountId,
        //                            CashInFlow = g.Key.CashIn,

        //                            Debit = g.Sum(r => r.Debit),
        //                            Credit = g.Sum(r => r.Credit),
        //                            Total = g.Sum(r => r.Total),
        //                        };

        //    var cashInOutList = cashInOutFlow.ToList();

        //    var cashInFlows = cashInOutList.Where(s => s.CashInFlow).ToList();
        //    var cashOutFlows = cashInOutList.Where(s => !s.CashInFlow).ToList();

        //    var cashInFlow = cashInFlows.GroupBy(s => 1)
        //                   .Select(s => new CashFlowReportOutput
        //                   {
        //                       Debit = s.Sum(r => r.Debit),
        //                       Credit = s.Sum(r => r.Credit),
        //                       Total = s.Sum(r => r.Total),
        //                   })
        //                   .FirstOrDefault() ??
        //                   new CashFlowReportOutput();

        //    var cashOutFlow = cashOutFlows.GroupBy(s => 1)
        //                 .Select(s => new CashFlowReportOutput
        //                 {
        //                     Debit = s.Sum(r => r.Debit),
        //                     Credit = s.Sum(r => r.Credit),
        //                     Total = s.Sum(r => r.Total),
        //                 })
        //                 .FirstOrDefault() ??
        //                 new CashFlowReportOutput();

        //    var netIncreaseInCash = cashInOutList.GroupBy(s => 1)
        //                            .Select(s => new CashFlowReportOutput
        //                            {
        //                                Debit = s.Sum(r => r.Debit),
        //                                Credit = s.Sum(r => r.Credit),
        //                                Total = s.Sum(r => r.Total),
        //                            }).FirstOrDefault() ?? new CashFlowReportOutput();

        //    var cashEnding = beginningList.Concat(cashInOutList).GroupBy(s => 1)
        //                .Select(s => new CashFlowReportOutput
        //                {
        //                    Debit = s.Sum(r => r.Debit),
        //                    Credit = s.Sum(r => r.Credit),
        //                    Total = s.Sum(r => r.Total),
        //                })
        //                .FirstOrDefault() ??
        //                new CashFlowReportOutput();


        //    var result = new CashFlowReportResultOutput
        //    {
        //        Beginning = beginning,
        //        CashInFlow = cashInFlow,
        //        CashOutFlow = cashOutFlow,
        //        CashInFlows = cashInFlows,
        //        CashOutFlows = cashOutFlows,
        //        NetIncreaseInCash = netIncreaseInCash,
        //        Ending = cashEnding,
        //        CashBankAccountTypes = cashBankAccountTypeIds,
        //    };

        //    return result;
        //}

        //private async Task<CashFlowReportResultOutput> GetListCashFlowLocationReport(GetListCashFlowReportInput input)
        //{

        //    var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
        //                               .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.AccountTypeName.Trim()))
        //                               .AsNoTracking()
        //                               .Select(s => s.Id)
        //                               .ToListAsync();

        //    var previousDate = input.FromDate.AddDays(-1);

        //    var previousClose = await GetPreviousCloseCyleAsync(previousDate);

        //    var cashBankFromClosePeriodQuery = _accountCloseRepository.GetAll()
        //                                       .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
        //                                       .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.Account.AccountType.AccountTypeName.Trim()))
        //                                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
        //                                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                                       .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
        //                                       .AsNoTracking()
        //                                       .Select(s => new CashFlowReportOutput
        //                                       {
        //                                           AccountId = s.AccountId,
        //                                           AccountCode = s.Account.AccountCode,
        //                                           AccountName = s.Account.AccountName,
        //                                           AccountTypeId = s.Account.AccountTypeId,
        //                                           AccountType = s.Account.AccountType.Type,
        //                                           AccountTypeName = s.Account.AccountName.Trim(),
        //                                           Debit = s.Debit,
        //                                           Credit = s.Credit,
        //                                           Total = s.Balance,
        //                                           LocationId = s.Location.Id,
        //                                           LocationName = s.Location.LocationName,
        //                                       });

        //    var cashBankFromOldPeriodQuery = _journalItemRepository.GetAll()
        //                                      .Where(s => s.Journal.Status == TransactionStatus.Publish)
        //                                      .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
        //                                      .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.Account.AccountType.AccountTypeName.Trim()))
        //                                      .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
        //                                      .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                                      .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
        //                                      .AsNoTracking()
        //                                      .Select(s => new CashFlowReportOutput
        //                                      {
        //                                          AccountId = s.AccountId,
        //                                          AccountCode = s.Account.AccountCode,
        //                                          AccountName = s.Account.AccountName,
        //                                          AccountTypeId = s.Account.AccountTypeId,
        //                                          AccountType = s.Account.AccountType.Type,
        //                                          AccountTypeName = s.Account.AccountName.Trim(),
        //                                          Debit = s.Debit,
        //                                          Credit = s.Credit,
        //                                          Total = s.Debit - s.Credit,
        //                                          LocationId = s.Journal.Location.Id,
        //                                          LocationName = s.Journal.Location.LocationName,
        //                                      });

        //    var allAccountQuery = await _journalItemRepository.GetAll()
        //                            .Where(s => s.Journal.Status == TransactionStatus.Publish)
        //                            .Where(s => s.Journal.Date.Date >= input.FromDate.Date && s.Journal.Date.Date <= input.ToDate.Date)
        //                            .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
        //                            .AsNoTracking()
        //                            .Select(ji => new CashFlowReportOutput
        //                            {
        //                                AccountId = ji.AccountId,
        //                                AccountCode = ji.Account.AccountCode,
        //                                AccountName = ji.Account.AccountName,
        //                                AccountTypeId = ji.Account.AccountTypeId,
        //                                AccountType = ji.Account.AccountType.Type,
        //                                AccountTypeName = ji.Account.AccountType.AccountTypeName.Trim(),
        //                                LocationId = ji.Journal.Location.Id,
        //                                LocationName = ji.Journal.Location.LocationName,
        //                                Debit = ji.Debit,
        //                                Credit = ji.Credit,
        //                                Total = ji.Debit - ji.Credit,
        //                                JournalId = ji.JournalId,
        //                            })
        //                            .ToListAsync();


        //    var cashBankList = allAccountQuery
        //                       .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.AccountTypeName))
        //                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.AccountTypeId))
        //                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                       .ToList();

        //    var cashBankJournalIds = cashBankList.Select(s => s.JournalId).Distinct().ToList();

        //    var cashBankCurrrentPeriodQuery = allAccountQuery
        //                                       .Where(s => cashBankJournalIds.Any(r => r == s.JournalId))
        //                                       .Where(ji => !CorarlERPConsts.CashBankAccountTypes.Contains(ji.AccountTypeName))
        //                                       .Select(ji => new CashFlowReportOutput
        //                                       {
        //                                           AccountId = ji.AccountId,
        //                                           AccountCode = ji.AccountCode,
        //                                           AccountName = ji.AccountName,
        //                                           AccountTypeId = ji.AccountTypeId,
        //                                           AccountType = ji.AccountType,
        //                                           AccountTypeName = ji.AccountTypeName,
        //                                           LocationId = ji.LocationId,
        //                                           LocationName = ji.LocationName,
        //                                           Debit = ji.Credit,
        //                                           Credit = ji.Debit,
        //                                           Total = ji.Credit - ji.Debit,
        //                                           JournalId = ji.JournalId,
        //                                       });

        //    var allBeginningList = await cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery)
        //                        .Select(s => new CashFlowReportOutput
        //                        {
        //                            AccountTypeId = s.AccountTypeId,
        //                            AccountType = s.AccountType,
        //                            AccountTypeName = s.AccountTypeName,
        //                            AccountCode = s.AccountCode,
        //                            AccountName = s.AccountName,
        //                            AccountId = s.AccountId,
        //                            Debit = s.Debit,
        //                            Credit = s.Credit,
        //                            Total = s.Debit - s.Credit,
        //                            LocationName = s.LocationName,
        //                            LocationId = s.LocationId,
        //                        })
        //                        .ToListAsync();

        //    var locations = allBeginningList
        //                    .Concat(cashBankCurrrentPeriodQuery)
        //                    .Select(s => new { s.LocationId, s.LocationName })
        //                    .Distinct().OrderBy(s => s.LocationName).ToList();

        //    var beginningList = allBeginningList
        //                        .Select(s => new CashFlowReportOutput
        //                        {
        //                            AccountTypeId = s.AccountTypeId,
        //                            AccountType = s.AccountType,
        //                            AccountTypeName = s.AccountTypeName,
        //                            AccountCode = s.AccountCode,
        //                            AccountName = s.AccountName,
        //                            AccountId = s.AccountId,

        //                            Debit = s.Debit,
        //                            Credit = s.Credit,
        //                            Total = s.Debit - s.Credit,

        //                            LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                            {
        //                                LocationId = l.LocationId,
        //                                LocationName = l.LocationName,
        //                                Debit = s.LocationId == l.LocationId ? s.Debit : 0,
        //                                Credit = s.LocationId == l.LocationId ? s.Credit : 0,
        //                                Total = s.LocationId == l.LocationId ? s.Debit - s.Credit : 0,
        //                            }).ToDictionary(t => t.LocationId, t => t),
        //                        })
        //                        .ToList();

        //    var beginning = beginningList.GroupBy(s => 1)
        //                    .Select(s => new CashFlowReportOutput
        //                    {
        //                        Debit = s.Sum(r => r.Debit),
        //                        Credit = s.Sum(r => r.Credit),
        //                        Total = s.Sum(r => r.Total),
        //                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                        {
        //                            LocationId = l.LocationId,
        //                            LocationName = l.LocationName,
        //                            Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                            Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                            Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                        }).ToDictionary(t => t.LocationId, t => t),
        //                    })
        //                    .FirstOrDefault() ??
        //                    new CashFlowReportOutput
        //                    {
        //                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                        {
        //                            LocationId = l.LocationId,
        //                            LocationName = l.LocationName,
        //                        }).ToDictionary(t => t.LocationId, t => t),
        //                    };

        //    var cashInOutFlow = from s in cashBankCurrrentPeriodQuery
        //                        orderby s.AccountCode, s.Debit
        //                        group s by new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, CashIn = s.Debit != 0 }
        //                        into g
        //                        select new CashFlowReportOutput
        //                        {
        //                            AccountTypeId = g.Key.AccountTypeId,
        //                            AccountType = g.Key.AccountType,
        //                            AccountTypeName = g.Key.AccountTypeName,
        //                            AccountCode = g.Key.AccountCode,
        //                            AccountName = g.Key.AccountName,
        //                            AccountId = g.Key.AccountId,
        //                            CashInFlow = g.Key.CashIn,

        //                            Debit = g.Sum(r => r.Debit),
        //                            Credit = g.Sum(r => r.Credit),
        //                            Total = g.Sum(r => r.Total),

        //                            LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                            {
        //                                LocationId = l.LocationId,
        //                                LocationName = l.LocationName,
        //                                Debit = g.Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                                Credit = g.Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                                Total = g.Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                            }).ToDictionary(t => t.LocationId, t => t)
        //                        };

        //    var cashInOutList = cashInOutFlow.ToList();
        //    var cashInFlows = cashInOutList.Where(s => s.CashInFlow).ToList();
        //    var cashOutFlows = cashInOutList.Where(s => !s.CashInFlow).ToList();

        //    var cashInFlow = cashInFlows.GroupBy(s => 1)
        //                   .Select(s => new CashFlowReportOutput
        //                   {
        //                       Debit = s.Sum(r => r.Debit),
        //                       Credit = s.Sum(r => r.Credit),
        //                       Total = s.Sum(r => r.Total),
        //                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                       {
        //                           LocationId = l.LocationId,
        //                           LocationName = l.LocationName,
        //                           Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                           Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                           Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                       }).ToDictionary(t => t.LocationId, t => t),
        //                   })
        //                   .FirstOrDefault() ??
        //                   new CashFlowReportOutput
        //                   {
        //                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                       {
        //                           LocationId = l.LocationId,
        //                           LocationName = l.LocationName,
        //                       }).ToDictionary(t => t.LocationId, t => t),
        //                   };

        //    var cashOutFlow = cashOutFlows.GroupBy(s => 1)
        //                 .Select(s => new CashFlowReportOutput
        //                 {
        //                     Debit = s.Sum(r => r.Debit),
        //                     Credit = s.Sum(r => r.Credit),
        //                     Total = s.Sum(r => r.Total),
        //                     LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                     {
        //                         LocationId = l.LocationId,
        //                         LocationName = l.LocationName,
        //                         Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                         Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                         Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                     }).ToDictionary(t => t.LocationId, t => t),
        //                 })
        //                 .FirstOrDefault() ??
        //                 new CashFlowReportOutput
        //                 {
        //                     LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                     {
        //                         LocationId = l.LocationId,
        //                         LocationName = l.LocationName,
        //                     }).ToDictionary(t => t.LocationId, t => t),
        //                 };

        //    var netIncreaseInCash = cashInOutList.GroupBy(s => 1)
        //                           .Select(s => new CashFlowReportOutput
        //                           {
        //                               Debit = s.Sum(r => r.Debit),
        //                               Credit = s.Sum(r => r.Credit),
        //                               Total = s.Sum(r => r.Total),
        //                               LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                               {
        //                                   LocationId = l.LocationId,
        //                                   LocationName = l.LocationName,
        //                                   Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                                   Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                                   Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                               }).ToDictionary(t => t.LocationId, t => t),
        //                           }).FirstOrDefault() ??
        //                           new CashFlowReportOutput
        //                           {
        //                               LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                               {
        //                                   LocationId = l.LocationId,
        //                                   LocationName = l.LocationName,
        //                               }).ToDictionary(t => t.LocationId, t => t),
        //                           };


        //    var cashEnding = beginningList.Concat(cashInOutList).GroupBy(s => 1)
        //                .Select(s => new CashFlowReportOutput
        //                {
        //                    Debit = s.Sum(r => r.Debit),
        //                    Credit = s.Sum(r => r.Credit),
        //                    Total = s.Sum(r => r.Total),
        //                    LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                    {
        //                        LocationId = l.LocationId,
        //                        LocationName = l.LocationName,
        //                        Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                        Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                        Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),


        //                    }).ToDictionary(t => t.LocationId, t => t),
        //                })
        //                .FirstOrDefault();


        //    var result = new CashFlowReportResultOutput
        //    {
        //        Beginning = beginning​​,
        //        CashInFlow = cashInFlow,
        //        CashOutFlow = cashOutFlow,
        //        CashInFlows = cashInFlows,
        //        CashOutFlows = cashOutFlows,
        //        NetIncreaseInCash = netIncreaseInCash,
        //        Ending = cashEnding,
        //        ColumnHeaders = locations.ToDictionary(s => s.LocationId, s => s.LocationName),
        //        CashBankAccountTypes = cashBankAccountTypeIds,
        //    };

        //    return result;
        //}

        //private async Task<CashFlowReportResultOutput> GetListCashFlowMonthlyReport(GetListCashFlowReportInput input)
        //{
        //    var isSameYear = input.FromDate.Year == input.ToDate.Year;
        //    var firstMonth = Convert.ToInt32(input.FromDate.ToString("yyyyMM"));
        //    var latestMonth = Convert.ToInt32(input.ToDate.ToString("yyyyMM"));

        //    var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
        //                                .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.AccountTypeName.Trim()))
        //                                .AsNoTracking()
        //                                .Select(s => s.Id)
        //                                .ToListAsync();

        //    var previousDate = input.FromDate.AddDays(-1);

        //    var previousClose = await GetPreviousCloseCyleAsync(previousDate);

        //    var cashBankFromClosePeriodQuery = _accountCloseRepository.GetAll()
        //                                       .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
        //                                       .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.Account.AccountType.AccountTypeName.Trim()))
        //                                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
        //                                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                                       .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
        //                                       .AsNoTracking()
        //                                       .Select(s => new CashFlowReportOutput
        //                                       {
        //                                           AccountId = s.AccountId,
        //                                           AccountCode = s.Account.AccountCode,
        //                                           AccountName = s.Account.AccountName,
        //                                           AccountTypeId = s.Account.AccountTypeId,
        //                                           AccountType = s.Account.AccountType.Type,
        //                                           AccountTypeName = s.Account.AccountName.Trim(),
        //                                           Debit = s.Debit,
        //                                           Credit = s.Credit,
        //                                           Total = s.Balance,
        //                                       });

        //    var cashBankFromOldPeriodQuery = _journalItemRepository.GetAll()
        //                                      .Where(s => s.Journal.Status == TransactionStatus.Publish)
        //                                      .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
        //                                      .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.Account.AccountType.AccountTypeName.Trim()))
        //                                      .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
        //                                      .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                                      .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
        //                                      .AsNoTracking()
        //                                      .Select(s => new CashFlowReportOutput
        //                                      {
        //                                          AccountId = s.AccountId,
        //                                          AccountCode = s.Account.AccountCode,
        //                                          AccountName = s.Account.AccountName,
        //                                          AccountTypeId = s.Account.AccountTypeId,
        //                                          AccountType = s.Account.AccountType.Type,
        //                                          AccountTypeName = s.Account.AccountName.Trim(),
        //                                          Debit = s.Debit,
        //                                          Credit = s.Credit,
        //                                          Total = s.Debit - s.Credit,
        //                                      });

        //    var allAccountQuery = await _journalItemRepository.GetAll()
        //                            .Where(s => s.Journal.Status == TransactionStatus.Publish)
        //                            .Where(s => s.Journal.Date.Date >= input.FromDate.Date && s.Journal.Date.Date <= input.ToDate.Date)
        //                            .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
        //                            .AsNoTracking()
        //                            .Select(ji => new CashFlowReportOutput
        //                            {
        //                                AccountId = ji.AccountId,
        //                                AccountCode = ji.Account.AccountCode,
        //                                AccountName = ji.Account.AccountName,
        //                                AccountTypeId = ji.Account.AccountTypeId,
        //                                AccountType = ji.Account.AccountType.Type,
        //                                AccountTypeName = ji.Account.AccountType.AccountTypeName.Trim(),
        //                                LocationId = Convert.ToInt32(ji.Journal.Date.ToString("yyyyMM")),
        //                                LocationName = isSameYear ? ji.Journal.Date.ToString("MMM") : ji.Journal.Date.ToString("MMM-yyyy"),
        //                                Debit = ji.Debit,
        //                                Credit = ji.Credit,
        //                                Total = ji.Debit - ji.Credit,
        //                                JournalId = ji.JournalId,
        //                            })
        //                            .ToListAsync();

        //    var cashBankList = allAccountQuery
        //                       .Where(s => CorarlERPConsts.CashBankAccountTypes.Contains(s.AccountTypeName))
        //                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.AccountTypeId))
        //                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
        //                       .ToList();

        //    var cashBankJournalIds = cashBankList.Select(s => s.JournalId).Distinct().ToList();

        //    var cashBankCurrrentPeriodQuery = allAccountQuery
        //                                       .Where(s => cashBankJournalIds.Any(r => r == s.JournalId))
        //                                       .Where(ji => !CorarlERPConsts.CashBankAccountTypes.Contains(ji.AccountTypeName))
        //                                       .Select(ji => new CashFlowReportOutput
        //                                       {
        //                                           AccountId = ji.AccountId,
        //                                           AccountCode = ji.AccountCode,
        //                                           AccountName = ji.AccountName,
        //                                           AccountTypeId = ji.AccountTypeId,
        //                                           AccountType = ji.AccountType,
        //                                           AccountTypeName = ji.AccountTypeName,
        //                                           LocationId = ji.LocationId,
        //                                           LocationName = ji.LocationName,
        //                                           Debit = ji.Credit,
        //                                           Credit = ji.Debit,
        //                                           Total = ji.Credit - ji.Debit,
        //                                           JournalId = ji.JournalId,
        //                                       });

        //    var locations = cashBankCurrrentPeriodQuery
        //                    .Select(s => new { s.LocationId, s.LocationName })
        //                    .Distinct().OrderBy(s => s.LocationId).ToList();

        //    var beginningList = await cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery)
        //                        .Select(s => new CashFlowReportOutput
        //                        {
        //                            AccountTypeId = s.AccountTypeId,
        //                            AccountType = s.AccountType,
        //                            AccountTypeName = s.AccountTypeName,
        //                            AccountCode = s.AccountCode,
        //                            AccountName = s.AccountName,
        //                            AccountId = s.AccountId,

        //                            Debit = s.Debit,
        //                            Credit = s.Credit,
        //                            Total = s.Debit - s.Credit,
        //                        })
        //                        .ToListAsync();

        //    var cashInOutFlow = from s in cashBankCurrrentPeriodQuery
        //                        orderby s.AccountCode, s.Debit
        //                        select new CashFlowReportOutput
        //                        {
        //                            AccountTypeId = s.AccountTypeId,
        //                            AccountType = s.AccountType,
        //                            AccountTypeName = s.AccountTypeName,
        //                            AccountCode = s.AccountCode,
        //                            AccountName = s.AccountName,
        //                            AccountId = s.AccountId,
        //                            CashInFlow = s.Debit != 0,

        //                            Debit = s.Debit,
        //                            Credit = s.Credit,
        //                            Total = s.Total,

        //                            LocationId = s.LocationId,
        //                            LocationName = s.LocationName,
        //                        };

        //    var cashInOutList = cashInOutFlow.ToList();

        //    var beginning = beginningList.Concat(cashInOutList).GroupBy(s => 1)
        //                    .Select(s => new CashFlowReportOutput
        //                    {
        //                        Debit = s.Where(r => r.LocationId < latestMonth).Sum(r => r.Debit),
        //                        Credit = s.Where(r => r.LocationId < latestMonth).Sum(r => r.Credit),
        //                        Total = s.Where(r => r.LocationId < latestMonth).Sum(r => r.Total),
        //                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                        {
        //                            LocationId = l.LocationId,
        //                            LocationName = l.LocationName,
        //                            Debit = s.Where(t => t.LocationId < l.LocationId).Sum(t => t.Debit),
        //                            Credit = s.Where(t => t.LocationId < l.LocationId).Sum(t => t.Credit),
        //                            Total = s.Where(t => t.LocationId < l.LocationId).Sum(t => t.Total),
        //                        }).ToDictionary(t => t.LocationId, t => t),
        //                    })
        //                    .FirstOrDefault() ??
        //                    new CashFlowReportOutput
        //                    {
        //                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                        {
        //                            LocationId = l.LocationId,
        //                            LocationName = l.LocationName,
        //                        }).ToDictionary(t => t.LocationId, t => t),
        //                    };

        //    var cashInOutSummary = cashInOutList
        //        .OrderBy(s => s.AccountCode)
        //        .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeId, s.AccountTypeName, s.CashInFlow })
        //        .Select(s => new CashFlowReportOutput
        //        {
        //            AccountTypeId = s.Key.AccountTypeId,
        //            AccountType = s.Key.AccountType,
        //            AccountTypeName = s.Key.AccountTypeName,
        //            AccountCode = s.Key.AccountCode,
        //            AccountName = s.Key.AccountName,
        //            AccountId = s.Key.AccountId,
        //            CashInFlow = s.Key.CashInFlow,

        //            Debit = s.Sum(t => t.Debit),
        //            Credit = s.Sum(t => t.Credit),
        //            Total = s.Sum(t => t.Total),

        //            LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //            {
        //                LocationId = l.LocationId,
        //                LocationName = l.LocationName,
        //                Debit = s.Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                Credit = s.Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                Total = s.Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //            }).ToDictionary(t => t.LocationId, t => t),
        //        }).ToList();

        //    var cashInFlows = cashInOutSummary.Where(s => s.CashInFlow).ToList();
        //    var cashOutFlows = cashInOutSummary.Where(s => !s.CashInFlow).ToList();

        //    var cashInFlow = cashInFlows.GroupBy(s => 1)
        //                   .Select(s => new CashFlowReportOutput
        //                   {
        //                       Debit = s.Sum(r => r.Debit),
        //                       Credit = s.Sum(r => r.Credit),
        //                       Total = s.Sum(r => r.Total),
        //                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                       {
        //                           LocationId = l.LocationId,
        //                           LocationName = l.LocationName,
        //                           Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                           Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                           Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                       }).ToDictionary(t => t.LocationId, t => t),
        //                   })
        //                   .FirstOrDefault() ??
        //                    new CashFlowReportOutput
        //                    {
        //                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                        {
        //                            LocationId = l.LocationId,
        //                            LocationName = l.LocationName,
        //                        }).ToDictionary(t => t.LocationId, t => t),
        //                    };

        //    var cashOutFlow = cashOutFlows.GroupBy(s => 1)
        //                     .Select(s => new CashFlowReportOutput
        //                     {
        //                         Debit = s.Sum(r => r.Debit),
        //                         Credit = s.Sum(r => r.Credit),
        //                         Total = s.Sum(r => r.Total),
        //                         LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                         {
        //                             LocationId = l.LocationId,
        //                             LocationName = l.LocationName,
        //                             Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                             Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                             Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                         }).ToDictionary(t => t.LocationId, t => t),
        //                     })
        //                    .FirstOrDefault() ??
        //                    new CashFlowReportOutput
        //                    {
        //                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                        {
        //                            LocationId = l.LocationId,
        //                            LocationName = l.LocationName,
        //                        }).ToDictionary(t => t.LocationId, t => t),
        //                    };

        //    var netIncreaseInCash = cashInOutSummary.GroupBy(s => 1)
        //                           .Select(s => new CashFlowReportOutput
        //                           {
        //                               Debit = s.Sum(r => r.Debit),
        //                               Credit = s.Sum(r => r.Credit),
        //                               Total = s.Sum(r => r.Total),
        //                               LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                               {
        //                                   LocationId = l.LocationId,
        //                                   LocationName = l.LocationName,
        //                                   Debit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Debit),
        //                                   Credit = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Credit),
        //                                   Total = s.SelectMany(t => t.LocationColumnDic.Values).Where(t => t.LocationId == l.LocationId).Sum(t => t.Total),
        //                               }).ToDictionary(t => t.LocationId, t => t),
        //                           }).FirstOrDefault() ??
        //                           new CashFlowReportOutput
        //                           {
        //                               LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                               {
        //                                   LocationId = l.LocationId,
        //                                   LocationName = l.LocationName,
        //                               }).ToDictionary(t => t.LocationId, t => t),
        //                           };


        //    var cashEnding = beginningList.Concat(cashInOutList).GroupBy(s => 1)
        //                .Select(s => new CashFlowReportOutput
        //                {
        //                    Debit = s.Sum(r => r.Debit),
        //                    Credit = s.Sum(r => r.Credit),
        //                    Total = s.Sum(r => r.Total),
        //                    LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
        //                    {
        //                        LocationId = l.LocationId,
        //                        LocationName = l.LocationName,
        //                        Debit = s.Where(t => t.LocationId <= l.LocationId).Sum(t => t.Debit),
        //                        Credit = s.Where(t => t.LocationId <= l.LocationId).Sum(t => t.Credit),
        //                        Total = s.Where(t => t.LocationId <= l.LocationId).Sum(t => t.Total),

        //                    }).ToDictionary(t => t.LocationId, t => t),
        //                })
        //                .FirstOrDefault();


        //    var result = new CashFlowReportResultOutput
        //    {
        //        Beginning = beginning,
        //        CashInFlow = cashInFlow,
        //        CashOutFlow = cashOutFlow,
        //        CashInFlows = cashInFlows,
        //        CashOutFlows = cashOutFlows,
        //        NetIncreaseInCash = netIncreaseInCash,
        //        Ending = cashEnding,
        //        ColumnHeaders = locations.ToDictionary(s => s.LocationId, s => s.LocationName),
        //        CashBankAccountTypes = cashBankAccountTypeIds,
        //    };

        //    return result;
        //}



        //[AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow_Export_Excel)]
        //public async Task<FileDto> ExportExcelCashFlowReport(ExportCashFlowReportInput input)
        //{
        //    // Query get collumn header 
        //    //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
        //    var report = input.ReportOutput;
        //    var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
        //    var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
        //    int reportCountColHead = reportCollumnHeader.Count();
        //    var sheetName = report.HeaderTitle;

        //    var reportData = await GetListCashFlowReport(input);
        //    var locationsColumns = reportData.ColumnHeaders ?? new Dictionary<long, string>();

        //    var result = new FileDto();

        //    var roundDigits = (await GetCurrentCycleAsync()).RoundingDigit;

        //    ////Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
        //    using (var p = new ExcelPackage())
        //    {
        //        //A workbook must have at least on cell, so lets add one... 
        //        var ws = p.Workbook.Worksheets.Add(sheetName);
        //        ws.PrinterSettings.Orientation = eOrientation.Landscape;
        //        ws.PrinterSettings.FitToPage = true;
        //        //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
        //        ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
        //        ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

        //        #region Row 1 Header
        //        AddTextToCell(ws, 1, 1, sheetName, true);
        //        MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
        //        #endregion Row 1

        //        #region Row 2 Header
        //        string header = "";
        //        if (input.FromDate.Date <= new DateTime(1970, 01, 01).Date)
        //        {
        //            header = "As Of - " + input.ToDate.ToString("dd-MM-yyyy");
        //        }
        //        else
        //        {
        //            header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
        //        }
        //        AddTextToCell(ws, 2, 1, header, true);
        //        MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
        //        #endregion Row 2

        //        #region Row 3 Header Table
        //        int rowTableHeader = 3;
        //        int colHeaderTable = 1;//start from row 1 of spreadsheet
        //        // write header collumn table
        //        foreach (var i in reportCollumnHeader)
        //        {
        //            AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
        //            ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
        //            if (i.ColumnName != "Account") ws.Column(colHeaderTable).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            colHeaderTable += 1;
        //        }

        //        #endregion Row 3

        //        #region Row Body 
        //        // use for check auto dynamic col footer of sum value
        //        Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
        //        int rowBody = rowTableHeader + 1;//start from row header of spreadsheet

        //        //Beginning
        //        colHeaderTable = 1;
        //        foreach (var i in reportCollumnHeader)
        //        {
        //            if (i.ColumnName == "Account")
        //            {
        //                AddTextToCell(ws, rowBody, colHeaderTable, "Beginning", true);
        //            }
        //            else if (i.ColumnName == "Total")
        //            {
        //                AddNumberToCell(ws, rowBody, colHeaderTable, reportData.Beginning.Total, true, false, false, roundDigits);
        //            }
        //            else
        //            {
        //                var val = reportData.Beginning.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                          reportData.Beginning.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
        //            }
        //            colHeaderTable += 1;
        //        }
        //        rowBody++;


        //        //Cash In Flow
        //        AddTextToCell(ws, rowBody, 1, "Cash In Flow", true);
        //        rowBody++;

        //        foreach (var r in reportData.CashInFlows)
        //        {
        //            colHeaderTable = 1;
        //            foreach (var i in reportCollumnHeader)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountCode} - {r.AccountName}", false, 4);
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
        //                }
        //                else
        //                {
        //                    var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
        //                }
        //                colHeaderTable += 1;
        //            }
        //            rowBody++;
        //        }

        //        //Total Cash In Flow
        //        colHeaderTable = 1;
        //        foreach (var i in reportCollumnHeader)
        //        {
        //            if (i.ColumnName == "Account")
        //            {
        //                AddTextToCell(ws, rowBody, colHeaderTable, "Total Cash In Flow", true);
        //            }
        //            else if (i.ColumnName == "Total")
        //            {
        //                AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashInFlow.Total, true, false, false, roundDigits);
        //            }
        //            else
        //            {
        //                var val = reportData.CashInFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                            reportData.CashInFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
        //            }
        //            colHeaderTable += 1;
        //        }
        //        rowBody++;


        //        //Cash Out Flow
        //        AddTextToCell(ws, rowBody, 1, "Cash Out Flow", true);
        //        rowBody++;

        //        foreach (var r in reportData.CashOutFlows)
        //        {
        //            colHeaderTable = 1;
        //            foreach (var i in reportCollumnHeader)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountCode} - {r.AccountName}", false, 4);
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
        //                }
        //                else
        //                {
        //                    var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
        //                }
        //                colHeaderTable += 1;
        //            }
        //            rowBody++;
        //        }

        //        //Total Cash Out Flow
        //        colHeaderTable = 1;
        //        foreach (var i in reportCollumnHeader)
        //        {
        //            if (i.ColumnName == "Account")
        //            {
        //                AddTextToCell(ws, rowBody, colHeaderTable, "Total Cash Out Flow", true);
        //            }
        //            else if (i.ColumnName == "Total")
        //            {
        //                AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashOutFlow.Total, true, false, false, roundDigits);
        //            }
        //            else
        //            {
        //                var val = reportData.CashOutFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                            reportData.CashOutFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
        //            }
        //            colHeaderTable += 1;
        //        }
        //        rowBody++;

        //        //Net Increase In Cash
        //        colHeaderTable = 1;
        //        foreach (var i in reportCollumnHeader)
        //        {
        //            if (i.ColumnName == "Account")
        //            {
        //                AddTextToCell(ws, rowBody, colHeaderTable, "Net Increase In Cash", true);
        //            }
        //            else if (i.ColumnName == "Total")
        //            {
        //                AddNumberToCell(ws, rowBody, colHeaderTable, reportData.NetIncreaseInCash.Total, true, false, false, roundDigits);
        //            }
        //            else
        //            {
        //                var val = reportData.NetIncreaseInCash.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                            reportData.NetIncreaseInCash.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
        //            }
        //            colHeaderTable += 1;
        //        }
        //        rowBody++;

        //        //Cash Ending
        //        colHeaderTable = 1;
        //        foreach (var i in reportCollumnHeader)
        //        {
        //            if (i.ColumnName == "Account")
        //            {
        //                AddTextToCell(ws, rowBody, colHeaderTable, "Cash Ending", true);
        //            }
        //            else if (i.ColumnName == "Total")
        //            {
        //                if (i.AllowFunction != null)
        //                {
        //                    AddNumberToCell(ws, rowBody, colHeaderTable, reportData.Ending.Total, true, false, false, roundDigits);
        //                }
        //            }
        //            else
        //            {
        //                var val = reportData.Ending.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                            reportData.Ending.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
        //            }
        //            colHeaderTable += 1;
        //        }
        //        rowBody++;


        //        #endregion Row Body

        //        #region Row Footer 
        //        //if (reportHasShowFooterTotal.Count > 0)
        //        //{
        //        //    int footerRow = rowBody;
        //        //    int footerColNumber = 1;
        //        //    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
        //        //    foreach (var i in reportCollumnHeader)
        //        //    {
        //        //        if (i.AllowFunction != null)
        //        //        {

        //        //        }

        //        //        footerColNumber += 1;
        //        //    }
        //        //}
        //        #endregion Row Footer


        //        result.FileName = $"{sheetName}_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
        //        result.FileToken = $"{Guid.NewGuid()}.xlsx";
        //        result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

        //        await _fileStorageManager.UploadTempFile(result.FileToken, p);
        //    }

        //    return result;
        //}

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow_Export_Pdf)]
        //public async Task<FileDto> ExportPDFCashFlowReport(ExportCashFlowReportInput input)
        //{
        //    var tenant = await GetCurrentTenantAsync();
        //    var formatDate = await _formatRepository.GetAll()
        //                    .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
        //                    .Select(t => t.Web).FirstOrDefaultAsync();

        //    if (formatDate.IsNullOrEmpty())
        //    {
        //        formatDate = "dd/MM/yyyy";
        //    }
        //    var digit = (await GetCurrentCycleAsync()).RoundingDigit;
        //    var user = await GetCurrentUserAsync();
        //    var report = input.ReportOutput;
        //    var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
        //    var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

        //    var sheetName = report.HeaderTitle;

        //    var reportData = await GetListCashFlowReport(input);

        //    return await Task.Run(async () =>
        //    {
        //        var exportHtml = string.Empty;
        //        var templateHtml = string.Empty;
        //        FileDto fileDto = new FileDto()
        //        {
        //            SubFolder = null,
        //            FileName = "BalanceSheet.pdf",
        //            FileToken = "BalanceSheet.html",
        //            FileType = MimeTypeNames.TextHtml
        //        };
        //        try
        //        {
        //            templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
        //            templateHtml = templateHtml.Trim();
        //        }
        //        catch (FileNotFoundException)
        //        {
        //            throw new UserFriendlyException("FileNotFound");
        //        }
        //        //ToDo: Replace and concat string to be the same what frontend did
        //        exportHtml = templateHtml;

        //        var contentBody = string.Empty;
        //        var contentHeader = string.Empty;

        //        var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
        //        var documentWidth = 1080;
        //        decimal totalVisibleColsWidth = 0;
        //        decimal totalTableWidth = 0;
        //        int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
        //                {
        //                    var align = i.ColumnName == "Account" ? "" : "style='text-align: center'";

        //                    var rowHeader = $"<th {align} width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
        //                    contentHeader += rowHeader;
        //                    totalTableWidth += i.ColumnLength;
        //                }
        //                else
        //                {
        //                    i.Visible = false;
        //                    reportCountColHead--;
        //                }
        //                totalVisibleColsWidth += i.ColumnLength;
        //            }
        //        }


        //        exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

        //        #region Row Body      

        //        //Beginning
        //        var beginningContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    beginningContent += $"<td>Beginning</td>";
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    beginningContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.Beginning.Total, digit), digit)}</td>";
        //                }
        //                else
        //                {
        //                    var val = reportData.Beginning.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              reportData.Beginning.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    beginningContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{beginningContent}</tr>";


        //        //Cash In Flow
        //        var cashInFlowContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    cashInFlowContent += $"<td>Cash In Flow</td>";
        //                }
        //                else
        //                {
        //                    cashInFlowContent += $"<td></td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowContent}</tr>";

        //        foreach (var row in reportData.CashInFlows)
        //        {
        //            var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
        //            foreach (var i in viewHeader)
        //            {
        //                if (i.Visible)
        //                {
        //                    if (i.ColumnName == "Account")
        //                    {
        //                        tr += $"<td style='text-indent: 15px'>{row.AccountCode} - {row.AccountName}</td>";
        //                    }
        //                    else if (i.ColumnName == "Total")
        //                    {
        //                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, digit), digit)}</td>";
        //                    }
        //                    else
        //                    {
        //                        var val = row.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                                  row.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                    }
        //                }
        //            }
        //            tr += "</tr>";
        //            contentBody += tr;
        //        }

        //        //Total Cash In Flow
        //        var totalCashInFlowContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    totalCashInFlowContent += $"<td>Total Cash In Flow</td>";
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    totalCashInFlowContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashInFlow.Total, digit), digit)}</td>";
        //                }
        //                else
        //                {
        //                    var val = reportData.CashInFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              reportData.CashInFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    totalCashInFlowContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{totalCashInFlowContent}</tr>";

        //        //Cash Out Flow
        //        var cashOutFlowContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    cashOutFlowContent += $"<td>Cash Out Flow</td>";
        //                }
        //                else
        //                {
        //                    cashOutFlowContent += $"<td></td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashOutFlowContent}</tr>";

        //        foreach (var row in reportData.CashOutFlows)
        //        {
        //            var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
        //            foreach (var i in viewHeader)
        //            {
        //                if (i.Visible)
        //                {
        //                    if (i.ColumnName == "Account")
        //                    {
        //                        tr += $"<td style='text-indent: 15px'>{row.AccountCode} - {row.AccountName}</td>";
        //                    }
        //                    else if (i.ColumnName == "Total")
        //                    {
        //                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, digit), digit)}</td>";
        //                    }
        //                    else
        //                    {
        //                        var val = row.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                                  row.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                    }
        //                }
        //            }
        //            tr += "</tr>";
        //            contentBody += tr;
        //        }

        //        //Total Cash Out Flow
        //        var totalCashOutFlowContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    totalCashOutFlowContent += $"<td>Total Cash Out Flow</td>";
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    totalCashOutFlowContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashOutFlow.Total, digit), digit)}</td>";
        //                }
        //                else
        //                {
        //                    var val = reportData.CashOutFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              reportData.CashOutFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    totalCashOutFlowContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{totalCashOutFlowContent}</tr>";

        //        //Net Increase In Cash
        //        var netIncreaseInCashContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    netIncreaseInCashContent += $"<td>Net Increase In Cash</td>";
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    netIncreaseInCashContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.NetIncreaseInCash.Total, digit), digit)}</td>";
        //                }
        //                else
        //                {
        //                    var val = reportData.NetIncreaseInCash.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              reportData.NetIncreaseInCash.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    netIncreaseInCashContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{netIncreaseInCashContent}</tr>";


        //        //Cash Ending
        //        var cashEndingContent = "";
        //        foreach (var i in viewHeader)
        //        {
        //            if (i.Visible)
        //            {
        //                if (i.ColumnName == "Account")
        //                {
        //                    cashEndingContent += $"<td>Cash Ending</td>";
        //                }
        //                else if (i.ColumnName == "Total")
        //                {
        //                    if (i.AllowFunction != null)
        //                    {
        //                        cashEndingContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.Ending.Total, digit), digit)}</td>";
        //                    }
        //                    else
        //                    {
        //                        cashEndingContent += $"<td></td>";
        //                    }

        //                }
        //                else
        //                {
        //                    var val = reportData.Ending.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
        //                              reportData.Ending.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

        //                    cashEndingContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
        //                }
        //            }
        //        }
        //        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashEndingContent}</tr>";


        //        #endregion Row Body

        //        #region Row Footer 


        //        #endregion Row Footer

        //        exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
        //        exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
        //        exportHtml = exportHtml.Replace("{{subHeader}}", "");

        //        EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
        //        AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
        //        AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

        //        byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
        //        var tokenName = $"{Guid.NewGuid()}.pdf";
        //        var result = new FileDto();
        //        result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
        //        result.FileToken = $"{Guid.NewGuid()}.pdf";
        //        result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
        //        result.FileType = MimeTypeNames.ApplicationPdf;

        //        await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
        //        return result;
        //    });
        //}
        #endregion

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow_Export_Excel)]
        public async Task<FileDto> ExportExcelCashFlowReport(ExportCashFlowReportInput input)
        {
            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var reportData = await GetListCashFlowReport(input);
            var locationsColumns = reportData.ColumnHeaders ?? new Dictionary<long, string>();

            var result = new FileDto();

            var roundDigits = (await GetCurrentCycleAsync()).RoundingDigit;

            ////Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                #region Row 2 Header
                string header = "";
                if (input.FromDate.Date <= new DateTime(1970, 01, 01).Date)
                {
                    header = "As Of - " + input.ToDate.ToString("dd-MM-yyyy");
                }
                else
                {
                    header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                }
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    if (i.ColumnName != "Account") ws.Column(colHeaderTable).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colHeaderTable += 1;
                }

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet

                AddTextToCell(ws, rowBody, 1, L("CashFlowsFromOperatingActivities"), true);
                rowBody++;

                //Profit Loss
                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.ProfitLoss.Account , false, 4);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.ProfitLoss.Total, false, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.ProfitLoss.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.ProfitLoss.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;

                if (reportData.DepreciationAmotizations.Any())
                {
                    AddTextToCell(ws, rowBody, 1, L("AdjustmentForDepreciationAndAmortization"), false, 4);
                    rowBody++;

                    foreach (var r in reportData.DepreciationAmotizations)
                    {
                        colHeaderTable = 1;
                        foreach (var i in reportCollumnHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountCode} - {r.AccountName}", false, 8);
                            }
                            else if (i.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                            }
                            else
                            {
                                var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                            }
                            colHeaderTable += 1;
                        }
                        rowBody++;
                    }


                    colHeaderTable = 1;
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, colHeaderTable, reportData.ProfitLossBeforeChangesInWorkingCapital.Account, true, 4);
                        }
                        else if (i.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colHeaderTable, reportData.ProfitLossBeforeChangesInWorkingCapital.Total, true, false, false, roundDigits);
                        }
                        else
                        {
                            var val = reportData.ProfitLossBeforeChangesInWorkingCapital.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.ProfitLossBeforeChangesInWorkingCapital.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                        }
                        colHeaderTable += 1;
                    }
                    rowBody++;

                }


                AddTextToCell(ws, rowBody, 1, L("IncreaseDecreaseInCurrentAssetsAndLiabilities"), false, 4);
                rowBody++;

                foreach (var r in reportData.CurrentAssetAndCurrentLiabilies)
                {
                    colHeaderTable = 1;
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountCode} - {r.AccountName}", false, 8);
                        }
                        else if (i.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                        }
                        else
                        {
                            var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                        }
                        colHeaderTable += 1;
                    }
                    rowBody++;
                }


                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlowFromOperation.Account, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlowFromOperation.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.NetCashFlowFromOperation.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.NetCashFlowFromOperation.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;


                AddTextToCell(ws, rowBody, 1, L("CashFlowsFromInvestingActivities"), true);
                rowBody++;

                foreach (var r in reportData.CashFlowFromInvestments)
                {
                    colHeaderTable = 1;
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountCode} - {r.AccountName}", false, 4);
                        }
                        else if (i.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                        }
                        else
                        {
                            var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                        }
                        colHeaderTable += 1;
                    }
                    rowBody++;
                }


                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlowFromInvestment.Account, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlowFromInvestment.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.NetCashFlowFromInvestment.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.NetCashFlowFromInvestment.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;



                AddTextToCell(ws, rowBody, 1, L("CashFlowsFromFinancingActivities"), true);
                rowBody++;

                foreach (var r in reportData.CashFlowFromFinancings)
                {
                    colHeaderTable = 1;
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountCode} - {r.AccountName}", false, 4);
                        }
                        else if (i.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                        }
                        else
                        {
                            var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                        }
                        colHeaderTable += 1;
                    }
                    rowBody++;
                }


                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlowFromFinancing.Account, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlowFromFinancing.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.NetCashFlowFromFinancing.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.NetCashFlowFromFinancing.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;



                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.NetCashAndCashEquivalentForPeriod.Account, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.NetCashAndCashEquivalentForPeriod.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.NetCashAndCashEquivalentForPeriod.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.NetCashAndCashEquivalentForPeriod.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;


                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.Account, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;



                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.CashAndCashEquivalentAtTheEndOfPeriod.Account, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashAndCashEquivalentAtTheEndOfPeriod.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.CashAndCashEquivalentAtTheEndOfPeriod.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.CashAndCashEquivalentAtTheEndOfPeriod.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;



                #endregion Row Body

                #region Row Footer 
                //if (reportHasShowFooterTotal.Count > 0)
                //{
                //    int footerRow = rowBody;
                //    int footerColNumber = 1;
                //    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                //    foreach (var i in reportCollumnHeader)
                //    {
                //        if (i.AllowFunction != null)
                //        {

                //        }

                //        footerColNumber += 1;
                //    }
                //}
                #endregion Row Footer


                result.FileName = $"{sheetName}_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow_Export_Pdf)]
        public async Task<FileDto> ExportPDFCashFlowReport(ExportCashFlowReportInput input)
        {
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }
            var digit = (await GetCurrentCycleAsync()).RoundingDigit;
            var user = await GetCurrentUserAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;

            var reportData = await GetListCashFlowReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "BalanceSheet.pdf",
                    FileToken = "BalanceSheet.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var align = i.ColumnName == "Account" ? "" : "style='text-align: center'";

                            var rowHeader = $"<th {align} width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                            reportCountColHead--;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }


                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body     

                var cashFlowFromOperatingHeaderContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            cashFlowFromOperatingHeaderContent += $"<td>{L("CashFlowsFromOperatingActivities")}</td>";
                        }
                        else
                        {
                            cashFlowFromOperatingHeaderContent += $"<td style='text-align: right'></td>";
                        }
                    }
                }

                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashFlowFromOperatingHeaderContent}</tr>";


                var profitLossContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            profitLossContent += $"<td style='text-indent: 15px'>{ reportData.ProfitLoss.Account }</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            profitLossContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.ProfitLoss.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.ProfitLoss.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.ProfitLoss.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            profitLossContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto;'>{profitLossContent}</tr>";


                if (reportData.DepreciationAmotizations.Any())
                {
                    var adjustmentForDepreciationContent = "";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "Account")
                            {
                                adjustmentForDepreciationContent += $"<td style='text-indent: 15px'>{L("AdjustmentForDepreciationAndAmortization")}</td>";
                            }
                            else
                            {
                                adjustmentForDepreciationContent += $"<td style='text-align: right'></td>";
                            }
                        }
                    }

                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto;'>{adjustmentForDepreciationContent}</tr>";


                    foreach (var row in reportData.DepreciationAmotizations)
                    {
                        var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    tr += $"<td style='text-indent: 30px'>{row.AccountCode} - {row.AccountName}</td>";
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, digit), digit)}</td>";
                                }
                                else
                                {
                                    var val = row.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              row.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }


                    var operatingProfitLossBeforeChangeInWorkingCapitalContent = "";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "Account")
                            {
                                operatingProfitLossBeforeChangeInWorkingCapitalContent += $"<td style='text-indent: 15px'>{reportData.ProfitLossBeforeChangesInWorkingCapital.Account}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                operatingProfitLossBeforeChangeInWorkingCapitalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.ProfitLossBeforeChangesInWorkingCapital.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = reportData.ProfitLossBeforeChangesInWorkingCapital.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          reportData.ProfitLossBeforeChangesInWorkingCapital.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                operatingProfitLossBeforeChangeInWorkingCapitalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                    }
                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{operatingProfitLossBeforeChangeInWorkingCapitalContent}</tr>";
                }


                var increaseDecreaseInCurrentAssetAndLiabilitiesHeaderContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            increaseDecreaseInCurrentAssetAndLiabilitiesHeaderContent += $"<td style='text-indent: 15px'>{L("IncreaseDecreaseInCurrentAssetsAndLiabilities")}</td>";
                        }
                        else
                        {
                            increaseDecreaseInCurrentAssetAndLiabilitiesHeaderContent += $"<td style='text-align: right'></td>";
                        }
                    }
                }

                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto;'>{increaseDecreaseInCurrentAssetAndLiabilitiesHeaderContent}</tr>";


                foreach (var row in reportData.CurrentAssetAndCurrentLiabilies)
                {
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "Account")
                            {
                                tr += $"<td style='text-indent: 30px'>{row.AccountCode} - {row.AccountName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = row.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          row.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                var netCashUsedInOperatingActivitiesContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            netCashUsedInOperatingActivitiesContent += $"<td>{reportData.NetCashFlowFromOperation.Account}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            netCashUsedInOperatingActivitiesContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.NetCashFlowFromOperation.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.NetCashFlowFromOperation.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.NetCashFlowFromOperation.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            netCashUsedInOperatingActivitiesContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{netCashUsedInOperatingActivitiesContent}</tr>";



                var cashFlowFromInvestingActivitiesHeaderContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            cashFlowFromInvestingActivitiesHeaderContent += $"<td>{L("CashFlowsFromInvestingActivities")}</td>";
                        }
                        else
                        {
                            cashFlowFromInvestingActivitiesHeaderContent += $"<td style='text-align: right'></td>";
                        }
                    }
                }

                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashFlowFromInvestingActivitiesHeaderContent}</tr>";


                foreach (var row in reportData.CashFlowFromInvestments)
                {
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "Account")
                            {
                                tr += $"<td style='text-indent: 15px'>{row.AccountCode} - {row.AccountName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = row.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          row.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                var netCashUsedInInvestingActivitiesContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            netCashUsedInInvestingActivitiesContent += $"<td>{reportData.NetCashFlowFromInvestment.Account}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            netCashUsedInInvestingActivitiesContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.NetCashFlowFromInvestment.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.NetCashFlowFromInvestment.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.NetCashFlowFromInvestment.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            netCashUsedInInvestingActivitiesContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{netCashUsedInInvestingActivitiesContent}</tr>";



                var cashFlowFromFinancingActivitiesHeaderContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            cashFlowFromFinancingActivitiesHeaderContent += $"<td>{L("CashFlowsFromFinancingActivities")}</td>";
                        }
                        else
                        {
                            cashFlowFromFinancingActivitiesHeaderContent += $"<td style='text-align: right'></td>";
                        }
                    }
                }

                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashFlowFromFinancingActivitiesHeaderContent}</tr>";


                foreach (var row in reportData.CashFlowFromFinancings)
                {
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "Account")
                            {
                                tr += $"<td style='text-indent: 15px'>{row.AccountCode} - {row.AccountName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = row.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          row.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                var netCashUsedInFinancingActivitiesContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            netCashUsedInFinancingActivitiesContent += $"<td>{reportData.NetCashFlowFromFinancing.Account}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            netCashUsedInFinancingActivitiesContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.NetCashFlowFromFinancing.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.NetCashFlowFromFinancing.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.NetCashFlowFromFinancing.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            netCashUsedInFinancingActivitiesContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{netCashUsedInFinancingActivitiesContent}</tr>";


                var netCashChangeForPeriodContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            netCashChangeForPeriodContent += $"<td>{reportData.NetCashAndCashEquivalentForPeriod.Account}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            netCashChangeForPeriodContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.NetCashAndCashEquivalentForPeriod.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.NetCashAndCashEquivalentForPeriod.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.NetCashAndCashEquivalentForPeriod.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            netCashChangeForPeriodContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{netCashChangeForPeriodContent}</tr>";


                var cashAtTheBeginningOfPeriodContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            cashAtTheBeginningOfPeriodContent += $"<td>{reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.Account}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            cashAtTheBeginningOfPeriodContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.CashAndCashEquivalentAtTheBeginningOfPeriod.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            cashAtTheBeginningOfPeriodContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashAtTheBeginningOfPeriodContent}</tr>";


                var cashAtTheEndingOfPeriodContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (i.ColumnName == "Account")
                        {
                            cashAtTheEndingOfPeriodContent += $"<td>{reportData.CashAndCashEquivalentAtTheEndOfPeriod.Account}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            cashAtTheEndingOfPeriodContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashAndCashEquivalentAtTheEndOfPeriod.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.CashAndCashEquivalentAtTheEndOfPeriod.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.CashAndCashEquivalentAtTheEndOfPeriod.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            cashAtTheEndingOfPeriodContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashAtTheEndingOfPeriodContent}</tr>";

                #endregion Row Body

                #region Row Footer 


                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;
            });
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow)]
        public async Task<IndirectCashFlowReportResultOutput> GetListCashFlowReport(GetListCashFlowReportInput input)
        {
            return input.ViewOption == ViewOption.Month ? await GetListCashFlowMonthlyReport(input) :
                   input.ViewOption == ViewOption.Location ? await GetListCashFlowLocationReport(input) :
                   await GetListCashFlowStandardReport(input);
        }

        private async Task<IndirectCashFlowReportResultOutput> GetListCashFlowStandardReport(GetListCashFlowReportInput input)
        {
            var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
                                       .Where(s => s.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                       .AsNoTracking()
                                       .Select(s => s.Id)
                                       .ToListAsync();

            var previousDate = input.FromDate.AddDays(-1);

            var previousClose = await GetPreviousCloseCyleAsync(previousDate);

            var cashBankFromClosePeriodQuery = await _accountCloseRepository.GetAll()
                                                   .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
                                                   .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                   .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                   .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                   .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                                   .AsNoTracking()
                                                   .SumAsync(s => s.Balance);

            var cashBankFromOldPeriodQuery = await _journalItemRepository.GetAll()
                                                  .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                  .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
                                                  .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                  .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                                  .AsNoTracking()
                                                  .SumAsync(t => t.Debit - t.Credit);

            var currentPeriodAccountQuery = await _journalItemRepository.GetAll()
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(s => !cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                               .Where(s => s.Journal.Date.Date >= input.FromDate.Date && s.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                               .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                               .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                               .AsNoTracking()
                                               .OrderBy(s => s.Account.AccountCode)
                                               .GroupBy(s => new { 
                                                   s.AccountId, 
                                                   s.Account.AccountCode, 
                                                   s.Account.AccountName, 
                                                   s.Account.AccountType.Type, 
                                                   s.Account.AccountType.AccountTypeName, 
                                                   s.Account.AccountTypeId, 
                                                   s.Account.SubAccountType 
                                               })
                                               .Select(g => new IndirectCashFlowReportOutput
                                               {
                                                   AccountId = g.Key.AccountId,
                                                   AccountCode = g.Key.AccountCode,
                                                   AccountName = g.Key.AccountName,
                                                   AccountTypeId = g.Key.AccountTypeId,
                                                   AccountType = g.Key.Type,
                                                   AccountTypeName = g.Key.AccountTypeName,
                                                   SubAccountType = g.Key.SubAccountType,
                                                   Total = g.Sum(t => t.Credit - t.Debit)
                                               })
                                               .ToListAsync();

            var profitLossAccountTypes = new HashSet<TypeOfAccount> {
                TypeOfAccount.Income,
                TypeOfAccount.COGS,
                TypeOfAccount.Expense
            };

            var netProfitLoss = currentPeriodAccountQuery
                                .Where(s => profitLossAccountTypes.Contains(s.AccountType))
                                .Sum(s => s.Total);

            var depreciations = currentPeriodAccountQuery
                                .Where(s => s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization)
                                .ToList();

           var currentAssetAndCurrentLiabilities = currentPeriodAccountQuery
                                                    .Where(s => s.AccountType == TypeOfAccount.CurrentAsset || s.AccountType == TypeOfAccount.OtherCurrentAsset || s.AccountType == TypeOfAccount.CurrentLiability)
                                                    .ToList();

            var fixedAndNoneCurrentAssets = currentPeriodAccountQuery
                                            .Where(s => s.AccountType == TypeOfAccount.FixedAsset && !(s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization))
                                            .ToList();

            var equitiesAndLongTermLiabilities = currentPeriodAccountQuery
                                                 .Where(s => s.AccountType == TypeOfAccount.Equity || s.AccountType == TypeOfAccount.LongTermLiability)
                                                 .ToList();


            var netCashChangeForCurrentPeriod = currentPeriodAccountQuery.Sum(t => t.Total);

            var result = new IndirectCashFlowReportResultOutput
            {                
                ProfitLoss = new IndirectCashFlowReportOutput { Account = L("NetProfitAndLoss"), Total = netProfitLoss },
                DepreciationAmotizations = depreciations,
                ProfitLossBeforeChangesInWorkingCapital = new IndirectCashFlowReportOutput { Account = L("OperatingProfitBeforeChangesInWorkingCapital"), Total = netProfitLoss + depreciations.Sum(t => t.Total) },
                
                CurrentAssetAndCurrentLiabilies = currentAssetAndCurrentLiabilities,
                NetCashFlowFromOperation = new IndirectCashFlowReportOutput { Account = L("NetCashUsedInOperatingActivities"), Total = netProfitLoss + depreciations.Sum(t => t.Total) + currentAssetAndCurrentLiabilities.Sum(t => t.Total)},
                
                CashFlowFromInvestments = fixedAndNoneCurrentAssets,
                NetCashFlowFromInvestment = new IndirectCashFlowReportOutput { Account = L("NetCashUsedInInvestingActivities"), Total = fixedAndNoneCurrentAssets.Sum(t => t.Total) },

                CashFlowFromFinancings = equitiesAndLongTermLiabilities,
                NetCashFlowFromFinancing = new IndirectCashFlowReportOutput { Account = L("NetCashGeneratedFromFinancingActivities"), Total = equitiesAndLongTermLiabilities.Sum(t => t.Total) },

                NetCashAndCashEquivalentForPeriod = new IndirectCashFlowReportOutput { Account = L("NetChangesInCashAndCashEquivalentsOfPeriod"), Total = netCashChangeForCurrentPeriod },
                CashAndCashEquivalentAtTheBeginningOfPeriod = new IndirectCashFlowReportOutput { Account = L("CashAndCashEquivalentsAtTheBeginningOfPeriod"), Total = cashBankFromClosePeriodQuery + cashBankFromOldPeriodQuery },
                CashAndCashEquivalentAtTheEndOfPeriod = new IndirectCashFlowReportOutput { Account = L("CashAndCashEquivalentsAtTheEndOfPeriod"), Total = cashBankFromClosePeriodQuery + cashBankFromOldPeriodQuery + netCashChangeForCurrentPeriod },

                CashBankAccountTypes = cashBankAccountTypeIds,
            };

            return result;
        }

        private async Task<IndirectCashFlowReportResultOutput> GetListCashFlowLocationReport(GetListCashFlowReportInput input)
        {

            var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
                                       .Where(s => s.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                       .AsNoTracking()
                                       .Select(s => s.Id)
                                       .ToListAsync();

            var previousDate = input.FromDate.AddDays(-1);

            var previousClose = await GetPreviousCloseCyleAsync(previousDate);

            var cashBankFromClosePeriodQuery = await _accountCloseRepository.GetAll()
                                                       .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
                                                       .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                       .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                                       .AsNoTracking()
                                                       .Select(s => new IndirectCashFlowReportOutput
                                                       {
                                                           Total = s.Balance,
                                                           LocationId = s.Location.Id,
                                                           LocationName = s.Location.LocationName,
                                                       })
                                                       .ToListAsync();

            var cashBankFromOldPeriodQuery = await _journalItemRepository.GetAll()
                                                  .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                  .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
                                                  .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                  .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                                  .AsNoTracking()
                                                  .Select(s => new IndirectCashFlowReportOutput
                                                  {
                                                      Total = s.Debit - s.Credit,
                                                      LocationId = s.Journal.Location.Id,
                                                      LocationName = s.Journal.Location.LocationName,
                                                  })
                                                  .ToListAsync();

            var beginningList = cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery).ToList();


            var currentPeriodAccountQuery = await _journalItemRepository.GetAll()
                                                .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                .Where(s => !cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                .Where(s => s.Journal.Date.Date >= input.FromDate.Date && s.Journal.Date.Date <= input.ToDate.Date)
                                                .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                                .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                .AsNoTracking()
                                                .OrderBy(s => s.Account.AccountCode)
                                                .GroupBy(s => new {
                                                    s.AccountId,
                                                    s.Account.AccountCode,
                                                    s.Account.AccountName,
                                                    s.Account.AccountType.Type,
                                                    s.Account.AccountType.AccountTypeName,
                                                    s.Account.AccountTypeId,
                                                    s.Account.SubAccountType,
                                                    s.Journal.LocationId,
                                                    s.Journal.Location.LocationName
                                                })
                                                .Select(g => new IndirectCashFlowReportOutput
                                                {
                                                    AccountId = g.Key.AccountId,
                                                    AccountCode = g.Key.AccountCode,
                                                    AccountName = g.Key.AccountName,
                                                    AccountTypeId = g.Key.AccountTypeId,
                                                    AccountType = g.Key.Type,
                                                    AccountTypeName = g.Key.AccountTypeName,
                                                    SubAccountType = g.Key.SubAccountType,
                                                    LocationId = g.Key.LocationId.Value,
                                                    LocationName = g.Key.LocationName,
                                                    Total = g.Sum(t => t.Credit - t.Debit)
                                                })
                                                .ToListAsync();


            var locations = currentPeriodAccountQuery
                            .Concat(beginningList)
                            .OrderBy(s => s.LocationName)
                            .GroupBy(s => new { s.LocationId, s.LocationName })
                            .Select(s => s.Key )
                            .ToList();

            var emptyLocationDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
            {
                LocationId = l.LocationId,
                LocationName = l.LocationName,
            })
            .ToDictionary(t => t.LocationId, t => t);

            var beginning = beginningList
                            .GroupBy(s => 1)
                            .Select(s => new IndirectCashFlowReportOutput
                            {
                                Account = L("CashAndCashEquivalentsAtTheBeginningOfPeriod"),
                                Total = s.Sum(t => t.Total),

                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                {
                                    LocationId = l.LocationId,
                                    LocationName = l.LocationName,
                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                }).ToDictionary(t => t.LocationId, t => t),
                            })
                            .FirstOrDefault() ??
                            new IndirectCashFlowReportOutput {
                                Account = L("CashAndCashEquivalentsAtTheBeginningOfPeriod"),
                                LocationColumnDic = emptyLocationDic,
                            };

            var profitLossAccountTypes = new HashSet<TypeOfAccount> {
                TypeOfAccount.Income,
                TypeOfAccount.COGS,
                TypeOfAccount.Expense
            };

            var netProfitLoss = currentPeriodAccountQuery
                                .Where(s => profitLossAccountTypes.Contains(s.AccountType))
                                .GroupBy(s => 1)
                                .Select(s => new IndirectCashFlowReportOutput
                                {
                                    Account = L("NetProfitAndLoss"),
                                    Total = s.Sum(t => t.Total),

                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                    {
                                        LocationId = l.LocationId,
                                        LocationName = l.LocationName,
                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                    }).ToDictionary(t => t.LocationId, t => t),
                                })
                                .FirstOrDefault() ??
                                new IndirectCashFlowReportOutput
                                {
                                    Account = L("NetProfitAndLoss"),
                                    LocationColumnDic = emptyLocationDic,
                                };

            var depreciations = currentPeriodAccountQuery
                                .Where(s => s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization)
                                .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType})
                                .Select(s => new IndirectCashFlowReportOutput
                                {
                                    AccountId = s.Key.AccountId,
                                    AccountCode = s.Key.AccountCode,
                                    AccountName = s.Key.AccountName,
                                    AccountType = s.Key.AccountType,
                                    SubAccountType = s.Key.SubAccountType,
                                    AccountTypeId = s.Key.AccountTypeId,
                                    AccountTypeName = s.Key.AccountTypeName,
                                    Total = s.Sum(t => t.Total),
                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                    {
                                        LocationId = l.LocationId,
                                        LocationName = l.LocationName,
                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                    }).ToDictionary(t => t.LocationId, t => t)
                                })
                                .ToList();

            var  profitLossBeforeChangesInWorkingCapital = currentPeriodAccountQuery
                                                            .Where(s => profitLossAccountTypes.Contains(s.AccountType) ||
                                                                        s.SubAccountType == SubAccountType.AccumulatedDepreciation || 
                                                                        s.SubAccountType == SubAccountType.AccumulatedAmortization)
                                                            .GroupBy(s => 1)
                                                            .Select(s => new IndirectCashFlowReportOutput
                                                            {
                                                                Account = L("OperatingProfitBeforeChangesInWorkingCapital"),
                                                                Total = s.Sum(t => t.Total),

                                                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                                {
                                                                    LocationId = l.LocationId,
                                                                    LocationName = l.LocationName,
                                                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                                }).ToDictionary(t => t.LocationId, t => t),
                                                            })
                                                            .FirstOrDefault() ??
                                                            new IndirectCashFlowReportOutput
                                                            {
                                                                Account = L("OperatingProfitBeforeChangesInWorkingCapital"),
                                                                LocationColumnDic = emptyLocationDic,
                                                            };

            var currentAssetAndCurrentLiabilities = currentPeriodAccountQuery
                                                    .Where(s => s.AccountType == TypeOfAccount.CurrentAsset || s.AccountType == TypeOfAccount.OtherCurrentAsset || s.AccountType == TypeOfAccount.CurrentLiability)
                                                    .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                                    .Select(s => new IndirectCashFlowReportOutput
                                                    {
                                                        AccountId = s.Key.AccountId,
                                                        AccountCode = s.Key.AccountCode,
                                                        AccountName = s.Key.AccountName,
                                                        AccountType = s.Key.AccountType,
                                                        SubAccountType = s.Key.SubAccountType,
                                                        AccountTypeId = s.Key.AccountTypeId,
                                                        AccountTypeName = s.Key.AccountTypeName,
                                                        Total = s.Sum(t => t.Total),
                                                        LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                        {
                                                            LocationId = l.LocationId,
                                                            LocationName = l.LocationName,
                                                            Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                        }).ToDictionary(t => t.LocationId, t => t)
                                                    })
                                                    .ToList();

            var netCashFlowFromOperation = currentPeriodAccountQuery
                                            .Where(s => profitLossAccountTypes.Contains(s.AccountType) || 
                                                        s.SubAccountType == SubAccountType.AccumulatedDepreciation || 
                                                        s.SubAccountType == SubAccountType.AccumulatedAmortization ||
                                                        s.AccountType == TypeOfAccount.CurrentAsset || 
                                                        s.AccountType == TypeOfAccount.OtherCurrentAsset || 
                                                        s.AccountType == TypeOfAccount.CurrentLiability)
                                            .GroupBy(s => 1)
                                            .Select(s => new IndirectCashFlowReportOutput
                                            {
                                                Account = L("NetCashUsedInOperatingActivities"),
                                                Total = s.Sum(t => t.Total),
                                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                {
                                                    LocationId = l.LocationId,
                                                    LocationName = l.LocationName,
                                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                }).ToDictionary(t => t.LocationId, t => t)
                                            })
                                            .FirstOrDefault() ??
                                            new IndirectCashFlowReportOutput
                                            {
                                                Account = L("NetCashUsedInOperatingActivities"),
                                                LocationColumnDic = emptyLocationDic
                                            };
                                                    

            var fixedAndNoneCurrentAssets = currentPeriodAccountQuery
                                            .Where(s => s.AccountType == TypeOfAccount.FixedAsset && !(s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization))
                                            .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                            .Select(s => new IndirectCashFlowReportOutput
                                            {
                                                AccountId = s.Key.AccountId,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                AccountType = s.Key.AccountType,
                                                SubAccountType = s.Key.SubAccountType,
                                                AccountTypeId = s.Key.AccountTypeId,
                                                AccountTypeName = s.Key.AccountTypeName,
                                                Total = s.Sum(t => t.Total),
                                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                {
                                                    LocationId = l.LocationId,
                                                    LocationName = l.LocationName,
                                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                }).ToDictionary(t => t.LocationId, t => t)
                                            })
                                            .ToList();

            var netCashFlowFromInvestment = currentPeriodAccountQuery
                                           .Where(s => s.AccountType == TypeOfAccount.FixedAsset && !(s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization))
                                           .GroupBy(s => 1)
                                           .Select(s => new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashUsedInInvestingActivities"),
                                               Total = s.Sum(t => t.Total),
                                               LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                               {
                                                   LocationId = l.LocationId,
                                                   LocationName = l.LocationName,
                                                   Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                               }).ToDictionary(t => t.LocationId, t => t)
                                           })
                                           .FirstOrDefault() ??
                                           new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashUsedInInvestingActivities"),
                                               LocationColumnDic = emptyLocationDic
                                           };

            var equitiesAndLongTermLiabilities = currentPeriodAccountQuery
                                                .Where(s => s.AccountType == TypeOfAccount.Equity || s.AccountType == TypeOfAccount.LongTermLiability)
                                                .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                                .Select(s => new IndirectCashFlowReportOutput
                                                {
                                                    AccountId = s.Key.AccountId,
                                                    AccountCode = s.Key.AccountCode,
                                                    AccountName = s.Key.AccountName,
                                                    AccountType = s.Key.AccountType,
                                                    SubAccountType = s.Key.SubAccountType,
                                                    AccountTypeId = s.Key.AccountTypeId,
                                                    AccountTypeName = s.Key.AccountTypeName,
                                                    Total = s.Sum(t => t.Total),
                                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                    {
                                                        LocationId = l.LocationId,
                                                        LocationName = l.LocationName,
                                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                    }).ToDictionary(t => t.LocationId, t => t)
                                                })
                                                .ToList();

            var netCashFlowFromFinancing = currentPeriodAccountQuery
                                           .Where(s => s.AccountType == TypeOfAccount.Equity || s.AccountType == TypeOfAccount.LongTermLiability)
                                           .GroupBy(s => 1)
                                           .Select(s => new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashGeneratedFromFinancingActivities"),
                                               Total = s.Sum(t => t.Total),
                                               LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                               {
                                                   LocationId = l.LocationId,
                                                   LocationName = l.LocationName,
                                                   Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                               }).ToDictionary(t => t.LocationId, t => t)
                                           })
                                           .FirstOrDefault() ??
                                           new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashGeneratedFromFinancingActivities"),
                                               LocationColumnDic = emptyLocationDic
                                           };


            var netCashChangeForCurrentPeriod = currentPeriodAccountQuery
                                                .GroupBy(s => 1)
                                                .Select(s => new IndirectCashFlowReportOutput
                                                {
                                                    Account = L("NetChangesInCashAndCashEquivalentsOfPeriod"),
                                                    Total = s.Sum(t => t.Total),
                                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                    {
                                                        LocationId = l.LocationId,
                                                        LocationName = l.LocationName,
                                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                    }).ToDictionary(t => t.LocationId, t => t)
                                                })
                                                .FirstOrDefault() ??
                                                new IndirectCashFlowReportOutput
                                                {
                                                    Account = L("NetChangesInCashAndCashEquivalentsOfPeriod"),
                                                    LocationColumnDic = emptyLocationDic
                                                };

            var result = new IndirectCashFlowReportResultOutput
            {
                ProfitLoss = netProfitLoss,
                DepreciationAmotizations = depreciations,
                ProfitLossBeforeChangesInWorkingCapital = profitLossBeforeChangesInWorkingCapital,

                CurrentAssetAndCurrentLiabilies = currentAssetAndCurrentLiabilities,
                NetCashFlowFromOperation = netCashFlowFromOperation,

                CashFlowFromInvestments = fixedAndNoneCurrentAssets,
                NetCashFlowFromInvestment = netCashFlowFromInvestment,

                CashFlowFromFinancings = equitiesAndLongTermLiabilities,
                NetCashFlowFromFinancing = netCashFlowFromFinancing,

                NetCashAndCashEquivalentForPeriod = netCashChangeForCurrentPeriod,
                CashAndCashEquivalentAtTheBeginningOfPeriod = beginning,
                CashAndCashEquivalentAtTheEndOfPeriod = new IndirectCashFlowReportOutput { 
                    Account = L("CashAndCashEquivalentsAtTheEndOfPeriod"), 
                    Total = beginning.Total + netCashChangeForCurrentPeriod.Total,
                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                    {
                        LocationId = l.LocationId,
                        LocationName = l.LocationName,
                        Total = beginning.LocationColumnDic[l.LocationId].Total + netCashChangeForCurrentPeriod.LocationColumnDic[l.LocationId].Total,
                    }).ToDictionary(t => t.LocationId, t => t)
                },

                ColumnHeaders = locations.ToDictionary(s => s.LocationId, s => s.LocationName),
                CashBankAccountTypes = cashBankAccountTypeIds,
            };

            return result;
        }

        private async Task<IndirectCashFlowReportResultOutput> GetListCashFlowMonthlyReport(GetListCashFlowReportInput input)
        {
            var isSameYear = input.FromDate.Year == input.ToDate.Year;
            var latestMonth = Convert.ToInt32(input.ToDate.ToString("yyyyMM"));

            var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
                                        .Where(s => s.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                        .AsNoTracking()
                                        .Select(s => s.Id)
                                        .ToListAsync();

            var previousDate = input.FromDate.AddDays(-1);

            var previousClose = await GetPreviousCloseCyleAsync(previousDate);


            var cashBankFromClosePeriodQuery = await _accountCloseRepository.GetAll()
                                                       .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
                                                       .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                       .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                       .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                       .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                                       .AsNoTracking()
                                                       .Select(s => new IndirectCashFlowReportOutput
                                                       {
                                                           Total = s.Balance,
                                                       })
                                                       .ToListAsync();

            var cashBankFromOldPeriodQuery = await _journalItemRepository.GetAll()
                                                  .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                  .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
                                                  .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                  .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                  .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                                  .AsNoTracking()
                                                  .Select(s => new IndirectCashFlowReportOutput
                                                  {
                                                      Total = s.Debit - s.Credit,
                                                  })
                                                  .ToListAsync();

            var beginningList = cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery).ToList();


            var currentPeriodAccountQuery = await _journalItemRepository.GetAll()
                                                .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                .Where(s => !cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                                .Where(s => s.Journal.Date.Date >= input.FromDate.Date && s.Journal.Date.Date <= input.ToDate.Date)
                                                .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                                .WhereIf(input.AccountType != null && input.AccountType.Any(), s => input.AccountType.Contains(s.Account.AccountTypeId))
                                                .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Any(), s => input.ChartOfAccounts.Contains(s.AccountId))
                                                .AsNoTracking()                                        
                                                .Select(ji => new IndirectCashFlowReportOutput
                                                {
                                                    AccountId = ji.AccountId,
                                                    AccountCode = ji.Account.AccountCode,
                                                    AccountName = ji.Account.AccountName,
                                                    AccountTypeId = ji.Account.AccountTypeId,
                                                    AccountType = ji.Account.AccountType.Type,
                                                    AccountTypeName = ji.Account.AccountType.AccountTypeName.Trim(),
                                                    SubAccountType = ji.Account.SubAccountType,
                                                    LocationId = Convert.ToInt32(ji.Journal.Date.ToString("yyyyMM")),
                                                    LocationName = isSameYear ? ji.Journal.Date.ToString("MMM") : ji.Journal.Date.ToString("MMM-yyyy"),                                            
                                                    Total = ji.Credit - ji.Debit,
                                                })
                                                .ToListAsync();


            var locations = currentPeriodAccountQuery
                            .OrderBy(s => s.LocationId)
                            .GroupBy(s => new { s.LocationId, s.LocationName })
                            .Select(s => s.Key)
                            .ToList();

            var emptyLocationDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
            {
                LocationId = l.LocationId,
                LocationName = l.LocationName,
            })
            .ToDictionary(t => t.LocationId, t => t);

            var beginning = beginningList
                            .Concat(currentPeriodAccountQuery)
                            .GroupBy(s => 1)
                            .Select(s => new IndirectCashFlowReportOutput
                            {
                                Account = L("CashAndCashEquivalentsAtTheBeginningOfPeriod"),
                                Total = s.Where(r => r.LocationId < latestMonth).Sum(r => r.Total),

                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                {
                                    LocationId = l.LocationId,
                                    LocationName = l.LocationName,
                                    Total = s.Sum(t => t.LocationId < l.LocationId ? t.Total : 0),
                                }).ToDictionary(t => t.LocationId, t => t),
                            })
                            .FirstOrDefault() ??
                            new IndirectCashFlowReportOutput
                            {
                                Account = L("CashAndCashEquivalentsAtTheBeginningOfPeriod"),
                                LocationColumnDic = emptyLocationDic,
                            };

            var profitLossAccountTypes = new HashSet<TypeOfAccount> {
                TypeOfAccount.Income,
                TypeOfAccount.COGS,
                TypeOfAccount.Expense
            };

            var netProfitLoss = currentPeriodAccountQuery
                                .Where(s => profitLossAccountTypes.Contains(s.AccountType))
                                .GroupBy(s => 1)
                                .Select(s => new IndirectCashFlowReportOutput
                                {
                                    Account = L("NetProfitAndLoss"),
                                    Total = s.Sum(t => t.Total),

                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                    {
                                        LocationId = l.LocationId,
                                        LocationName = l.LocationName,
                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                    }).ToDictionary(t => t.LocationId, t => t),
                                })
                                .FirstOrDefault() ??
                                new IndirectCashFlowReportOutput
                                {
                                    Account = L("NetProfitAndLoss"),
                                    LocationColumnDic = emptyLocationDic,
                                };

            var depreciations = currentPeriodAccountQuery
                                .Where(s => s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization)
                                .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                .Select(s => new IndirectCashFlowReportOutput
                                {
                                    AccountId = s.Key.AccountId,
                                    AccountCode = s.Key.AccountCode,
                                    AccountName = s.Key.AccountName,
                                    AccountType = s.Key.AccountType,
                                    SubAccountType = s.Key.SubAccountType,
                                    AccountTypeId = s.Key.AccountTypeId,
                                    AccountTypeName = s.Key.AccountTypeName,
                                    Total = s.Sum(t => t.Total),
                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                    {
                                        LocationId = l.LocationId,
                                        LocationName = l.LocationName,
                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                    }).ToDictionary(t => t.LocationId, t => t)
                                })
                                .ToList();

            var profitLossBeforeChangesInWorkingCapital = currentPeriodAccountQuery
                                                            .Where(s => profitLossAccountTypes.Contains(s.AccountType) ||
                                                                        s.SubAccountType == SubAccountType.AccumulatedDepreciation ||
                                                                        s.SubAccountType == SubAccountType.AccumulatedAmortization)
                                                            .GroupBy(s => 1)
                                                            .Select(s => new IndirectCashFlowReportOutput
                                                            {
                                                                Account = L("OperatingProfitBeforeChangesInWorkingCapital"),
                                                                Total = s.Sum(t => t.Total),

                                                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                                {
                                                                    LocationId = l.LocationId,
                                                                    LocationName = l.LocationName,
                                                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                                }).ToDictionary(t => t.LocationId, t => t),
                                                            })
                                                            .FirstOrDefault() ??
                                                            new IndirectCashFlowReportOutput
                                                            {
                                                                Account = L("OperatingProfitBeforeChangesInWorkingCapital"),
                                                                LocationColumnDic = emptyLocationDic,
                                                            };

            var currentAssetAndCurrentLiabilities = currentPeriodAccountQuery
                                                    .Where(s => s.AccountType == TypeOfAccount.CurrentAsset || s.AccountType == TypeOfAccount.OtherCurrentAsset || s.AccountType == TypeOfAccount.CurrentLiability)
                                                    .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                                    .Select(s => new IndirectCashFlowReportOutput
                                                    {
                                                        AccountId = s.Key.AccountId,
                                                        AccountCode = s.Key.AccountCode,
                                                        AccountName = s.Key.AccountName,
                                                        AccountType = s.Key.AccountType,
                                                        SubAccountType = s.Key.SubAccountType,
                                                        AccountTypeId = s.Key.AccountTypeId,
                                                        AccountTypeName = s.Key.AccountTypeName,
                                                        Total = s.Sum(t => t.Total),
                                                        LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                        {
                                                            LocationId = l.LocationId,
                                                            LocationName = l.LocationName,
                                                            Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                        }).ToDictionary(t => t.LocationId, t => t)
                                                    })
                                                    .ToList();

            var netCashFlowFromOperation = currentPeriodAccountQuery
                                            .Where(s => profitLossAccountTypes.Contains(s.AccountType) ||
                                                        s.SubAccountType == SubAccountType.AccumulatedDepreciation ||
                                                        s.SubAccountType == SubAccountType.AccumulatedAmortization ||
                                                        s.AccountType == TypeOfAccount.CurrentAsset ||
                                                        s.AccountType == TypeOfAccount.OtherCurrentAsset ||
                                                        s.AccountType == TypeOfAccount.CurrentLiability)
                                            .GroupBy(s => 1)
                                            .Select(s => new IndirectCashFlowReportOutput
                                            {
                                                Account = L("NetCashUsedInOperatingActivities"),
                                                Total = s.Sum(t => t.Total),
                                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                {
                                                    LocationId = l.LocationId,
                                                    LocationName = l.LocationName,
                                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                }).ToDictionary(t => t.LocationId, t => t)
                                            })
                                            .FirstOrDefault() ??
                                            new IndirectCashFlowReportOutput
                                            {
                                                Account = L("NetCashUsedInOperatingActivities"),
                                                LocationColumnDic = emptyLocationDic
                                            };


            var fixedAndNoneCurrentAssets = currentPeriodAccountQuery
                                            .Where(s => s.AccountType == TypeOfAccount.FixedAsset && !(s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization))
                                            .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                            .Select(s => new IndirectCashFlowReportOutput
                                            {
                                                AccountId = s.Key.AccountId,
                                                AccountCode = s.Key.AccountCode,
                                                AccountName = s.Key.AccountName,
                                                AccountType = s.Key.AccountType,
                                                SubAccountType = s.Key.SubAccountType,
                                                AccountTypeId = s.Key.AccountTypeId,
                                                AccountTypeName = s.Key.AccountTypeName,
                                                Total = s.Sum(t => t.Total),
                                                LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                {
                                                    LocationId = l.LocationId,
                                                    LocationName = l.LocationName,
                                                    Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                }).ToDictionary(t => t.LocationId, t => t)
                                            })
                                            .ToList();

            var netCashFlowFromInvestment = currentPeriodAccountQuery
                                           .Where(s => s.AccountType == TypeOfAccount.FixedAsset && !(s.SubAccountType == SubAccountType.AccumulatedDepreciation || s.SubAccountType == SubAccountType.AccumulatedAmortization))
                                           .GroupBy(s => 1)
                                           .Select(s => new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashUsedInInvestingActivities"),
                                               Total = s.Sum(t => t.Total),
                                               LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                               {
                                                   LocationId = l.LocationId,
                                                   LocationName = l.LocationName,
                                                   Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                               }).ToDictionary(t => t.LocationId, t => t)
                                           })
                                           .FirstOrDefault() ??
                                           new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashUsedInInvestingActivities"),
                                               LocationColumnDic = emptyLocationDic
                                           };

            var equitiesAndLongTermLiabilities = currentPeriodAccountQuery
                                                .Where(s => s.AccountType == TypeOfAccount.Equity || s.AccountType == TypeOfAccount.LongTermLiability)
                                                .GroupBy(s => new { s.AccountId, s.AccountCode, s.AccountName, s.AccountType, s.AccountTypeName, s.AccountTypeId, s.SubAccountType })
                                                .Select(s => new IndirectCashFlowReportOutput
                                                {
                                                    AccountId = s.Key.AccountId,
                                                    AccountCode = s.Key.AccountCode,
                                                    AccountName = s.Key.AccountName,
                                                    AccountType = s.Key.AccountType,
                                                    SubAccountType = s.Key.SubAccountType,
                                                    AccountTypeId = s.Key.AccountTypeId,
                                                    AccountTypeName = s.Key.AccountTypeName,
                                                    Total = s.Sum(t => t.Total),
                                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                    {
                                                        LocationId = l.LocationId,
                                                        LocationName = l.LocationName,
                                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                    }).ToDictionary(t => t.LocationId, t => t)
                                                })
                                                .ToList();

            var netCashFlowFromFinancing = currentPeriodAccountQuery
                                           .Where(s => s.AccountType == TypeOfAccount.Equity || s.AccountType == TypeOfAccount.LongTermLiability)
                                           .GroupBy(s => 1)
                                           .Select(s => new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashGeneratedFromFinancingActivities"),
                                               Total = s.Sum(t => t.Total),
                                               LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                               {
                                                   LocationId = l.LocationId,
                                                   LocationName = l.LocationName,
                                                   Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                               }).ToDictionary(t => t.LocationId, t => t)
                                           })
                                           .FirstOrDefault() ??
                                           new IndirectCashFlowReportOutput
                                           {
                                               Account = L("NetCashGeneratedFromFinancingActivities"),
                                               LocationColumnDic = emptyLocationDic
                                           };


            var netCashChangeForCurrentPeriod = currentPeriodAccountQuery
                                                .GroupBy(s => 1)
                                                .Select(s => new IndirectCashFlowReportOutput
                                                {
                                                    Account = L("NetChangesInCashAndCashEquivalentsOfPeriod"),
                                                    Total = s.Sum(t => t.Total),
                                                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                                                    {
                                                        LocationId = l.LocationId,
                                                        LocationName = l.LocationName,
                                                        Total = s.Sum(t => t.LocationId == l.LocationId ? t.Total : 0),
                                                    }).ToDictionary(t => t.LocationId, t => t)
                                                })
                                                .FirstOrDefault() ??
                                                new IndirectCashFlowReportOutput
                                                {
                                                    Account = L("NetChangesInCashAndCashEquivalentsOfPeriod"),
                                                    LocationColumnDic = emptyLocationDic
                                                };

            var result = new IndirectCashFlowReportResultOutput
            {
                ProfitLoss = netProfitLoss,
                DepreciationAmotizations = depreciations,
                ProfitLossBeforeChangesInWorkingCapital = profitLossBeforeChangesInWorkingCapital,

                CurrentAssetAndCurrentLiabilies = currentAssetAndCurrentLiabilities,
                NetCashFlowFromOperation = netCashFlowFromOperation,

                CashFlowFromInvestments = fixedAndNoneCurrentAssets,
                NetCashFlowFromInvestment = netCashFlowFromInvestment,

                CashFlowFromFinancings = equitiesAndLongTermLiabilities,
                NetCashFlowFromFinancing = netCashFlowFromFinancing,

                NetCashAndCashEquivalentForPeriod = netCashChangeForCurrentPeriod,
                CashAndCashEquivalentAtTheBeginningOfPeriod = beginning,
                CashAndCashEquivalentAtTheEndOfPeriod = new IndirectCashFlowReportOutput
                {
                    Account = L("CashAndCashEquivalentsAtTheEndOfPeriod"),
                    Total = beginning.Total + netCashChangeForCurrentPeriod.Total,
                    LocationColumnDic = locations.Select(l => new IndirectCashFlowReportLocationColumn
                    {
                        LocationId = l.LocationId,
                        LocationName = l.LocationName,
                        Total = beginning.LocationColumnDic[l.LocationId].Total + netCashChangeForCurrentPeriod.LocationColumnDic[l.LocationId].Total,
                    }).ToDictionary(t => t.LocationId, t => t)
                },

                ColumnHeaders = locations.ToDictionary(s => s.LocationId, s => s.LocationName),
                CashBankAccountTypes = cashBankAccountTypeIds,
            };


            return result;
        }


        #endregion


        [UnitOfWork(IsDisabled = true)]
        public async Task CleanupSaveTemplate()
        {

            var _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            var _reportTemplateRepository = IocManager.Instance.Resolve<ICorarlRepository<ReportTemplate, long>>();
            var _reportColumnTemplateRepository = IocManager.Instance.Resolve<ICorarlRepository<ReportColumnTemplate, Guid>>();
            var _tenantRepository = IocManager.Instance.Resolve<ICorarlRepository<Tenant, int>>();

            var reportColumns = new List<ReportColumnTemplate>();
            var tenantList = new List<Tenant>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    reportColumns = await _reportColumnTemplateRepository.GetAll().Include(s => s.ReportTemplate)
                                    .Where(s =>
                                     s.ReportTemplate.ReportType == ReportType.ReportType_ProfitAndLoss ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_BalanceSheet ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_Ledger ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_Journal 
                                     )
                                    .ToListAsync();

                    tenantList = await _tenantRepository.GetAll().ToListAsync();
                }
            }

            var columnToUpdates = new List<ReportColumnTemplate>();

            foreach (var tenant in tenantList)
            {
                var profitLossReportFromTemplateColumns = new List<CollumnOutput>();
                var balanceSheetReportFromTemplateColumns = new List<CollumnOutput>();
                var ledgerReportFromTemplateColumns = new List<CollumnOutput>();
                var journalReportFromTemplateColumns = new List<CollumnOutput>();

                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                    {
                        profitLossReportFromTemplateColumns = GetReportTemplateIncome().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        balanceSheetReportFromTemplateColumns = GetReportTemplateBalanceSheet().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        ledgerReportFromTemplateColumns = GetReportTemplateLedger().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        journalReportFromTemplateColumns = (await GetReportTemplateJournal()).ColumnInfo.Where(s => s.IsDisplay).ToList();
                    }
                }

                var profitLossReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_ProfitAndLoss).ToList();
                var balanceSheetReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_BalanceSheet).ToList();
                var ledgerReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_Ledger).ToList();
                var journalReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_Journal).ToList();

                foreach (var col in profitLossReportFromTemplateColumns)
                {
                    var updateCols = profitLossReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };

                foreach (var col in balanceSheetReportFromTemplateColumns)
                {
                    var updateCols = balanceSheetReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };


                foreach (var col in ledgerReportFromTemplateColumns)
                {
                    var updateCols = ledgerReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };


                foreach (var col in journalReportFromTemplateColumns)
                {
                    var updateCols = journalReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };
            }


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _reportColumnTemplateRepository.BulkUpdateAsync(columnToUpdates);
                }

                await uow.CompleteAsync();
            }

        }


        #region Direct Cash Flow

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow)]
        public async Task<CashFlowReportResultOutput> GetListDirectCashFlowReport(GetListDirectCashFlowReportInput input)
        {
            return input.ViewOption == ViewOption.Month ? await GetListDirectCashFlowMonthlyReport(input) :
                   input.ViewOption == ViewOption.Location ? await GetListDirectCashFlowLocationReport(input) :
                   await GetListDirectCashFlowStandardReport(input);
        }

        private async Task<CashFlowTemplate> GetCashFlowTemplateOrDefault(Guid? templateId)
        {
            CashFlowTemplate template;

            if (templateId.HasValue)
            {
                template = await _cashFlowTemplateRepository.GetAll().AsNoTracking().FirstAsync(s => s.Id == templateId.Value);
                if (template == null) throw new UserFriendlyException(L("IsNotValid", L("Template")));
            }
            else
            {
                template = await _cashFlowTemplateRepository.GetAll().AsNoTracking().FirstAsync(s => s.IsDefault);
                if (template == null) throw new UserFriendlyException(L("IsRequired", L("Template")));
            }

            return template;
        }

        private async Task<CashFlowReportResultOutput> GetListDirectCashFlowStandardReport(GetListDirectCashFlowReportInput input)
        {

            var template = await GetCashFlowTemplateOrDefault(input.CashFlowTemplateId);
           
            var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
                                              .Where(s => s.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                              .AsNoTracking()
                                              .Select(s => s.Id)
                                              .ToListAsync();
           
            var previousDate = input.FromDate.AddDays(-1);

            var previousClose = await GetPreviousCloseCyleAsync(previousDate);

            var cashBankFromClosePeriodQuery = _accountCloseRepository.GetAll()
                                               .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
                                               .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                               .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                               .AsNoTracking()
                                               .Select(s => new CashFlowReportOutput
                                               {
                                                   AccountId = s.AccountId,
                                                   Total = s.Balance,
                                               });

            var cashBankFromOldPeriodQuery = _journalItemRepository.GetAll()
                                              .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                              .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
                                              .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                              .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                              .AsNoTracking()
                                              .Select(s => new CashFlowReportOutput
                                              {
                                                  AccountId = s.AccountId,                                 
                                                  Total = s.Debit - s.Credit,
                                              });

            var allAccountQuery = from j in _journalRepository.GetAll().AsNoTracking()
                                        .Where(s => s.Status == TransactionStatus.Publish)
                                        .Where(s => s.Date.Date >= input.FromDate.Date && s.Date.Date <= input.ToDate.Date)
                                        .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                  join ji in _journalItemRepository.GetAll().AsNoTracking()
                                              .Select(s => new CashFlowReportOutput
                                              {
                                                  AccountId = s.AccountId,
                                                  AccountTypeId = s.Account.AccountTypeId,
                                                  Debit = s.Debit,
                                                  Credit = s.Credit,
                                                  JournalId = s.JournalId,
                                              })
                                  on j.Id equals ji.JournalId
                                  into js
                                  from i in js
                                  where js.Any(r => cashBankAccountTypeIds.Contains(r.AccountTypeId))
                                  where js.All(s => cashBankAccountTypeIds.Contains(s.AccountTypeId)) || !cashBankAccountTypeIds.Contains(i.AccountTypeId)
                                  group i by new
                                  {
                                      i.AccountId,
                                  }
                                  into g
                                  select new CashFlowReportOutput
                                  {
                                      AccountId = g.Key.AccountId,
                                      Debit = g.Sum(t => t.Debit),
                                      Credit = g.Sum(t => t.Credit),
                                      Total = g.Sum(t => t.Credit - t.Debit)
                                  };


            var beginningList = await cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery).ToListAsync();
            var cashBankCurrrentPeriodDic = await allAccountQuery.ToDictionaryAsync(k => k.AccountId, v => v);

            var beginning = new CashFlowSummaryReportOutput()
            {
                Name = L("CashBeginningForPeriod"),
                Total = beginningList.Sum(t => t.Total),
            };

            var categories = await _cashFlowTemplateCategoryRepository.GetAll()
                                  .AsNoTracking()
                                  .Where(s => s.TemplateId == template.Id)
                                  .Select(s => new 
                                  {
                                      CategoryId = s.CategoryId,
                                      CategoryName = s.Category.Name,
                                      SortOrder = s.Category.SortOrder,
                                      Type = s.Category.Type                                      
                                  })
                                  .OrderBy(s => s.SortOrder)
                                  .ToListAsync();

            var templateAccounts = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking()
                                        .Where(s => s.TemplateId == template.Id)
                                        .Select(s => new
                                        {
                                            s.AccountId,
                                            s.Account.AccountCode,
                                            s.Account.AccountName,
                                            s.CategoryId,
                                            InAccountGroupId = s.AccountGroupId,
                                            InAccountGroupName = s.AccountGroupId.HasValue ? s.AccontGroup.Name : "",
                                            InAccountGroupSortOrder = s.AccountGroupId.HasValue ? s.AccontGroup.SortOrder : 0,
                                            OutAccountGroupId = s.OutAccountGroupId,
                                            OutAccountGroupName = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.Name : "",
                                            OutAccountGroupSortOrder = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.SortOrder : 0
                                        })
                                        .OrderBy(s => s.InAccountGroupSortOrder)
                                        .ThenBy(s => s.AccountCode)
                                        .ToListAsync();

            var inAccountTemplates = templateAccounts
                                    .GroupBy(s => new
                                    {
                                        s.CategoryId,
                                        AccountId = s.InAccountGroupId.HasValue ? s.InAccountGroupId.Value : s.AccountId,
                                        AccountCode = s.InAccountGroupId.HasValue ? "" : s.AccountCode,
                                        AccountName = s.InAccountGroupId.HasValue ? s.InAccountGroupName : s.AccountName,
                                        SortOrder = s.InAccountGroupSortOrder
                                    })
                                    .Select(s => new
                                    {
                                        s.Key.CategoryId,
                                        s.Key.AccountId,
                                        s.Key.AccountCode,
                                        s.Key.AccountName,
                                        s.Key.SortOrder,
                                        SubAccounts = s.Select(a => a.AccountId).ToList()
                                    })
                                    .ToList();

            var outAccountTemplates = templateAccounts
                                    .OrderBy(s => s.OutAccountGroupSortOrder)
                                    .ThenBy(s => s.AccountCode)
                                    .GroupBy(s => new
                                    {
                                        s.CategoryId,
                                        AccountId = s.OutAccountGroupId.HasValue ? s.OutAccountGroupId.Value : s.AccountId,
                                        AccountCode = s.OutAccountGroupId.HasValue ? "" : s.AccountCode,
                                        AccountName = s.OutAccountGroupId.HasValue ? s.OutAccountGroupName : s.AccountName,
                                        SortOrder = s.OutAccountGroupSortOrder
                                    })
                                    .Select(s => new
                                    {
                                        s.Key.CategoryId,
                                        s.Key.AccountId,
                                        s.Key.AccountCode,
                                        s.Key.AccountName,
                                        s.Key.SortOrder,
                                        SubAccounts = s.Select(a => a.AccountId).ToList()
                                    })
                                    .ToList();

            var cashInFlows = new List<CashFlowCategoryReportOutput>();
            var cashOutFlows = new List<CashFlowCategoryReportOutput>();
            var cashInFlow = new CashFlowSummaryReportOutput();
            var cashOutFlow = new CashFlowSummaryReportOutput();
          
            if (template.SplitCashInAndCashOutFlow)
            {
                cashInFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = inAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(t => cashBankCurrrentPeriodDic.ContainsKey(t) && cashBankCurrrentPeriodDic[t].Credit != 0))
                               .Select(s => new CashFlowReportOutput
                               {
                                    CategoryId = c.CategoryId,
                                    CategoryName = c.CategoryName,
                                    AccountId = s.AccountId,
                                    AccountCode = s.AccountCode,
                                    AccountName = s.AccountName,
                                    AccountIds = s.SubAccounts,
                                    Total = s.SubAccounts.Sum(t => cashBankCurrrentPeriodDic.ContainsKey(t) ? cashBankCurrrentPeriodDic[t].Credit : 0)
                               })
                               .ToList();


                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,                        
                        Total = items.Sum(t => t.Total)
                    };
                })
                .ToList();


                cashInFlow = new CashFlowSummaryReportOutput()
                {
                    Name = L("TotalCashInflow"),
                    Total = cashInFlows.Sum(t => t.Total)
                };


                cashOutFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = outAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(t => cashBankCurrrentPeriodDic.ContainsKey(t) && cashBankCurrrentPeriodDic[t].Debit != 0))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   Total = s.SubAccounts.Sum(t => cashBankCurrrentPeriodDic.ContainsKey(t) ? cashBankCurrrentPeriodDic[t].Debit : 0)
                               })
                               .ToList();

                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total)
                    };
                })
                .ToList();

                cashOutFlow = new CashFlowSummaryReportOutput()
                {
                    Name = L("TotalCashOutflow"),
                    Total = cashOutFlows.Sum(t => t.Total)
                };
            }
            else
            {
                cashInFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = inAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(t => cashBankCurrrentPeriodDic.ContainsKey(t)))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   Total = s.SubAccounts.Sum(t => cashBankCurrrentPeriodDic.ContainsKey(t) ? cashBankCurrrentPeriodDic[t].Total : 0)
                               })
                               .ToList();

                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total)
                    };
                })
                .ToList();
            }


            var netCashFlow = new CashFlowSummaryReportOutput
            {
                Name = L("NetCashFlowForPeriod"),
                Total = cashInFlows.Sum(t => t.Total) - cashOutFlows.Sum(t => t.Total),
            };

            var cashEnding = new CashFlowSummaryReportOutput
            {
                Name = L("CashEndingForPeriod"),
                Total = beginning.Total + netCashFlow.Total
            };


            var cashTransfers = new List<CashFlowCategoryReportOutput>();
            var cashTransferCategories = categories.Where(s => s.Type == CashFlowCategoryType.CashTransfer).ToList();
            if (cashTransferCategories.Any()) 
            {
                cashTransfers = cashTransferCategories
                .Select(c =>
                {
                    var inItems = inAccountTemplates
                                .Where(s => s.CategoryId == c.CategoryId)
                                .Where(s => s.SubAccounts.Any(t => cashBankCurrrentPeriodDic.ContainsKey(t) && cashBankCurrrentPeriodDic[t].Credit != 0))
                                .Select(s => new CashFlowReportOutput
                                {
                                    CategoryId = c.CategoryId,
                                    CategoryName = c.CategoryName,
                                    AccountId = s.AccountId,
                                    AccountCode = s.AccountCode,
                                    AccountName = s.AccountName,
                                    AccountIds = s.SubAccounts,
                                    CategorySortOrder = s.SortOrder,
                                    Total = s.SubAccounts.Sum(t => cashBankCurrrentPeriodDic.ContainsKey(t) ? cashBankCurrrentPeriodDic[t].Credit : 0)
                                })
                                .ToList();

                    var outItems = outAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(t => cashBankCurrrentPeriodDic.ContainsKey(t) && cashBankCurrrentPeriodDic[t].Debit != 0))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   CategorySortOrder = s.SortOrder,
                                   Total = s.SubAccounts.Sum(t => cashBankCurrrentPeriodDic.ContainsKey(t) ? -cashBankCurrrentPeriodDic[t].Debit : 0)
                               })
                               .ToList();

                    var items = inItems.Concat(outItems).OrderBy(s => s.CategorySortOrder).ThenBy(s => s.AccountCode).ToList();
                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total)
                    };
                })
                .ToList();
            }


            var result = new CashFlowReportResultOutput
            {
                SplitCashInAndCashOutFlow = template.SplitCashInAndCashOutFlow,
                CashBeginning = beginning,
                CashInFlow = cashInFlow,
                CashOutFlow = cashOutFlow,
                CashInFlows = cashInFlows,
                CashOutFlows = cashOutFlows,
                NetCashFlow = netCashFlow,
                CashEnding = cashEnding,
                CashTransfers = cashTransfers,
                CashBankAccountTypes = cashBankAccountTypeIds,
            };

            return result;
        }

        private async Task<CashFlowReportResultOutput> GetListDirectCashFlowLocationReport(GetListDirectCashFlowReportInput input)
        {
            var template = await GetCashFlowTemplateOrDefault(input.CashFlowTemplateId);

            var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
                                              .Where(s => s.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                              .AsNoTracking()
                                              .Select(s => s.Id)
                                              .ToListAsync();

            var previousDate = input.FromDate.AddDays(-1);

            var previousClose = await GetPreviousCloseCyleAsync(previousDate);

            var cashBankFromClosePeriodQuery = _accountCloseRepository.GetAll()
                                               .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
                                               .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                               .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                               .AsNoTracking()
                                               .Select(s => new CashFlowReportOutput
                                               {
                                                   Total = s.Balance,
                                                   LocationId = s.LocationId.Value,
                                                   LocationName = s.Location.LocationName
                                               });

            var cashBankFromOldPeriodQuery = _journalItemRepository.GetAll()
                                              .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                              .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
                                              .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                              .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                              .AsNoTracking()
                                              .Select(s => new CashFlowReportOutput
                                              {
                                                  Total = s.Debit - s.Credit,
                                                  LocationId = s.Journal.LocationId.Value,
                                                  LocationName = s.Journal.Location.LocationName
                                              });


            var allAccountQuery = from j in _journalRepository.GetAll().AsNoTracking()
                                         .Where(s => s.Status == TransactionStatus.Publish)
                                         .Where(s => s.Date.Date >= input.FromDate.Date && s.Date.Date <= input.ToDate.Date)
                                         .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                  join ji in _journalItemRepository.GetAll().AsNoTracking()
                                              .Select(s => new CashFlowReportOutput
                                              {
                                                  AccountId = s.AccountId,
                                                  AccountTypeId = s.Account.AccountTypeId,
                                                  Debit = s.Debit,
                                                  Credit = s.Credit,
                                                  JournalId = s.JournalId,  
                                                  LocationId = s.Journal.LocationId.Value,
                                                  LocationName = s.Journal.Location.LocationName
                                              })
                                  on j.Id equals ji.JournalId
                                  into js
                                  from i in js
                                  where js.Any(r => cashBankAccountTypeIds.Contains(r.AccountTypeId))
                                  where js.All(s => cashBankAccountTypeIds.Contains(s.AccountTypeId)) || !cashBankAccountTypeIds.Contains(i.AccountTypeId)
                                  group i by new
                                  {
                                      i.AccountId,
                                      i.LocationId,
                                      i.LocationName
                                  }
                                  into g
                                  select new CashFlowReportOutput
                                  {
                                      AccountId = g.Key.AccountId,
                                      LocationId = g.Key.LocationId,
                                      LocationName = g.Key.LocationName,
                                      Debit = g.Sum(t => t.Debit),
                                      Credit = g.Sum(t => t.Credit),                              
                                      Total = g.Sum(t => t.Credit - t.Debit),                              
                                  };


            var beginningList = await cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery).ToListAsync();
            var cashBankCurrrentPeriodList = await allAccountQuery.ToListAsync();

            var locations = beginningList.Concat(cashBankCurrrentPeriodList)
                            .GroupBy(s => new { s.LocationId, s.LocationName })
                            .Select(s => s.Key)
                            .ToList();

            var beginning = new CashFlowSummaryReportOutput()
            {
                Name = L("CashBeginningForPeriod"),
                Total = beginningList.Sum(t => t.Total),
                LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Total = beginningList.Where(s => s.LocationId == l.LocationId).Sum(t => t.Total)
                })
                .ToDictionary(k => k.LocationId, v => v)
            };


            var categories = await _cashFlowTemplateCategoryRepository.GetAll()
                                  .AsNoTracking()
                                  .Where(s => s.TemplateId == template.Id)
                                  .Select(s => new 
                                  {
                                      CategoryId = s.CategoryId,
                                      CategoryName = s.Category.Name,
                                      SortOrder = s.Category.SortOrder,
                                      Type = s.Category.Type
                                  })
                                  .OrderBy(s => s.SortOrder)
                                  .ToListAsync();

            var templateAccounts = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking()
                                         .Where(s => s.TemplateId == template.Id)
                                         .Select(s => new
                                         {
                                             s.AccountId,
                                             s.Account.AccountCode,
                                             s.Account.AccountName,
                                             s.CategoryId,
                                             InAccountGroupId = s.AccountGroupId,
                                             InAccountGroupName = s.AccountGroupId.HasValue ? s.AccontGroup.Name : "",
                                             InAccountGroupSortOrder = s.AccountGroupId.HasValue ? s.AccontGroup.SortOrder : 0,
                                             OutAccountGroupId = s.OutAccountGroupId,
                                             OutAccountGroupName = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.Name : "",
                                             OutAccountGroupSortOrder = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.SortOrder : 0
                                         })
                                         .OrderBy(s => s.InAccountGroupSortOrder)
                                         .ThenBy(s => s.AccountCode)
                                         .ToListAsync();

            var inAccountTemplates = templateAccounts
                                    .GroupBy(s => new
                                    {
                                        s.CategoryId,
                                        AccountId = s.InAccountGroupId.HasValue ? s.InAccountGroupId.Value : s.AccountId,
                                        AccountCode = s.InAccountGroupId.HasValue ? "" : s.AccountCode,
                                        AccountName = s.InAccountGroupId.HasValue ? s.InAccountGroupName : s.AccountName,
                                        SortOrder = s.InAccountGroupSortOrder
                                    })
                                    .Select(s => new
                                    {
                                        s.Key.CategoryId,
                                        s.Key.AccountId,
                                        s.Key.AccountCode,
                                        s.Key.AccountName,
                                        s.Key.SortOrder,
                                        SubAccounts = s.Select(a => a.AccountId).ToList()
                                    })
                                    .ToList();

            var outAccountTemplates = templateAccounts
                                    .OrderBy(s => s.OutAccountGroupSortOrder)
                                    .ThenBy(s => s.AccountCode)
                                    .GroupBy(s => new
                                    {
                                        s.CategoryId,
                                        AccountId = s.OutAccountGroupId.HasValue ? s.OutAccountGroupId.Value : s.AccountId,
                                        AccountCode = s.OutAccountGroupId.HasValue ? "" : s.AccountCode,
                                        AccountName = s.OutAccountGroupId.HasValue ? s.OutAccountGroupName : s.AccountName,
                                        SortOrder = s.OutAccountGroupSortOrder
                                    })
                                    .Select(s => new
                                    {
                                        s.Key.CategoryId,
                                        s.Key.AccountId,
                                        s.Key.AccountCode,
                                        s.Key.AccountName,
                                        s.Key.SortOrder,
                                        SubAccounts = s.Select(a => a.AccountId).ToList()
                                    })
                                    .ToList();


            var cashInFlows = new List<CashFlowCategoryReportOutput>();
            var cashOutFlows = new List<CashFlowCategoryReportOutput>();
            var cashInFlow = new CashFlowSummaryReportOutput();
            var cashOutFlow = new CashFlowSummaryReportOutput();

            if (template.SplitCashInAndCashOutFlow)
            {
                cashInFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = inAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Credit != 0)))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Credit),
                                   LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                   {
                                       LocationId = l.LocationId,
                                       LocationName = l.LocationName,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Credit),
                                   })
                                   .ToDictionary(k => k.LocationId, v => v)
                               })
                               .ToList();


                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();


                cashInFlow = new CashFlowSummaryReportOutput()
                {
                    Name = L("TotalCashInflow"),
                    Total = cashInFlows.Sum(t => t.Total),
                    LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                    {
                        LocationId = l.LocationId,
                        LocationName = l.LocationName,
                        Total = cashInFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                    })
                    .ToDictionary(k => k.LocationId, v => v)
                };


                cashOutFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = outAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Debit != 0)))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Debit),
                                   LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                   {
                                       LocationId = l.LocationId,
                                       LocationName = l.LocationName,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Debit),
                                   })
                                   .ToDictionary(k => k.LocationId, v => v)
                               })
                               .ToList();

                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();

                cashOutFlow = new CashFlowSummaryReportOutput()
                {
                    Name = L("TotalCashOutflow"),
                    Total = cashOutFlows.Sum(t => t.Total),
                    LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                    {
                        LocationId = l.LocationId,
                        LocationName = l.LocationName,
                        Total = cashOutFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                    })
                    .ToDictionary(k => k.LocationId, v => v)
                };
            }
            else
            {
                cashInFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = inAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a)))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Total),
                                   LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                   {
                                       LocationId = l.LocationId,
                                       LocationName = l.LocationName,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Total),
                                   })
                                   .ToDictionary(k => k.LocationId, v => v)
                               })
                               .ToList();

                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();
            }


            var netCashFlow = new CashFlowSummaryReportOutput
            {
                Name = L("NetCashFlowForPeriod"),
                Total = cashInFlows.Sum(t => t.Total) - cashOutFlows.Sum(t => t.Total),
                LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Total = cashInFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total) - cashOutFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                })
                .ToDictionary(k => k.LocationId, v => v)
            };

            var cashEnding = new CashFlowSummaryReportOutput
            {
                Name = L("CashEndingForPeriod"),
                Total = beginning.Total + netCashFlow.Total,
                LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Total = beginning.LocationColumnDic[l.LocationId].Total + netCashFlow.LocationColumnDic[l.LocationId].Total,
                })
                .ToDictionary(k => k.LocationId, v => v)
            };


            var cashTransfers = new List<CashFlowCategoryReportOutput>();
            var cashTransferCategories = categories.Where(s => s.Type == CashFlowCategoryType.CashTransfer).ToList();
            if (cashTransferCategories.Any())
            {
                cashTransfers = cashTransferCategories
                .Select(c =>
                {
                    var inItems = inAccountTemplates
                                   .Where(s => s.CategoryId == c.CategoryId)
                                   .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Credit != 0)))
                                   .Select(s => new CashFlowReportOutput
                                   {
                                       CategoryId = c.CategoryId,
                                       CategoryName = c.CategoryName,
                                       AccountId = s.AccountId,
                                       AccountCode = s.AccountCode,
                                       AccountName = s.AccountName,
                                       AccountIds = s.SubAccounts,
                                       CategorySortOrder = s.SortOrder,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Credit),
                                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                       {
                                           LocationId = l.LocationId,
                                           LocationName = l.LocationName,
                                           Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Credit),
                                       })
                                       .ToDictionary(k => k.LocationId, v => v)
                                   })
                                   .ToList();

                    var outItems = outAccountTemplates
                                   .Where(s => s.CategoryId == c.CategoryId)
                                   .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Debit != 0)))
                                   .Select(s => new CashFlowReportOutput
                                   {
                                       CategoryId = c.CategoryId,
                                       CategoryName = c.CategoryName,
                                       AccountId = s.AccountId,
                                       AccountCode = s.AccountCode,
                                       AccountName = s.AccountName,
                                       AccountIds = s.SubAccounts,
                                       CategorySortOrder = s.SortOrder,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => -t.Debit),
                                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                       {
                                           LocationId = l.LocationId,
                                           LocationName = l.LocationName,
                                           Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => -t.Debit),
                                       })
                                       .ToDictionary(k => k.LocationId, v => v)
                                   })
                                   .ToList();

                    var items = inItems.Concat(outItems).OrderBy(s => s.CategorySortOrder).ThenBy(s => s.AccountCode).ToList();
                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();
            }     


            var result = new CashFlowReportResultOutput
            {
                SplitCashInAndCashOutFlow = template.SplitCashInAndCashOutFlow,
                CashBeginning = beginning,
                CashInFlow = cashInFlow,
                CashOutFlow = cashOutFlow,
                CashInFlows = cashInFlows,
                CashOutFlows = cashOutFlows,
                NetCashFlow = netCashFlow,
                CashEnding = cashEnding,
                CashTransfers = cashTransfers,
                CashBankAccountTypes = cashBankAccountTypeIds,
                ColumnHeaders = locations.ToDictionary(k => k.LocationId, v => v.LocationName)
            };

            return result;
        }

        private async Task<CashFlowReportResultOutput> GetListDirectCashFlowMonthlyReport(GetListDirectCashFlowReportInput input)
        {

            var isSameYear = input.FromDate.Year == input.ToDate.Year;


            var template = await GetCashFlowTemplateOrDefault(input.CashFlowTemplateId);

            var cashBankAccountTypeIds = await _accountTypeRepository.GetAll()
                                              .Where(s => s.AccountTypeName.Trim() == AccountTypeEnums.Cash.GetName() || s.AccountTypeName.Trim() == AccountTypeEnums.Bank.GetName())
                                              .AsNoTracking()
                                              .Select(s => s.Id)
                                              .ToListAsync();

            var previousDate = input.FromDate.AddDays(-1);

            var previousClose = await GetPreviousCloseCyleAsync(previousDate);

            var cashBankFromClosePeriodQuery = _accountCloseRepository.GetAll()
                                               .Where(s => previousClose != null && previousClose.Id == s.AccountCycleId)
                                               .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                               .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                               .AsNoTracking()
                                               .Select(s => new CashFlowReportOutput
                                               {
                                                   Total = s.Balance,
                                               });

            var cashBankFromOldPeriodQuery = _journalItemRepository.GetAll()
                                              .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                              .Where(s => s.Journal.Date.Date <= previousDate && (previousClose == null || s.Journal.Date.Date > previousClose.EndDate.Value.Date))
                                              .Where(s => cashBankAccountTypeIds.Contains(s.Account.AccountTypeId))
                                              .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Journal.LocationId.Value))
                                              .AsNoTracking()
                                              .Select(s => new CashFlowReportOutput
                                              {
                                                  Total = s.Debit - s.Credit,
                                              });


            var allAccountQuery = from j in _journalRepository.GetAll().AsNoTracking()
                                         .Where(s => s.Status == TransactionStatus.Publish)
                                         .Where(s => s.Date.Date >= input.FromDate.Date && s.Date.Date <= input.ToDate.Date)
                                         .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId.Value))
                                  join ji in _journalItemRepository.GetAll().AsNoTracking()
                                              .Select(s => new CashFlowReportOutput
                                              {
                                                  AccountId = s.AccountId,
                                                  AccountTypeId = s.Account.AccountTypeId,
                                                  Debit = s.Debit,
                                                  Credit = s.Credit,
                                                  JournalId = s.JournalId,
                                                  LocationId = Convert.ToInt32(s.Journal.Date.ToString("yyyyMM")),
                                                  LocationName = isSameYear ? s.Journal.Date.ToString("MMM") : s.Journal.Date.ToString("MMM-yyyy"),
                                              })
                                  on j.Id equals ji.JournalId
                                  into js
                                  from i in js
                                  where js.Any(r => cashBankAccountTypeIds.Contains(r.AccountTypeId))
                                  where js.All(s => cashBankAccountTypeIds.Contains(s.AccountTypeId)) || !cashBankAccountTypeIds.Contains(i.AccountTypeId)
                                  group i by new
                                  {
                                      i.AccountId,
                                      i.LocationId,
                                      i.LocationName
                                  }
                                  into g
                                  select new CashFlowReportOutput
                                  {
                                      AccountId = g.Key.AccountId,
                                      LocationId = g.Key.LocationId,
                                      LocationName = g.Key.LocationName,
                                      Debit = g.Sum(t => t.Debit),
                                      Credit = g.Sum(t => t.Credit),
                                      Total = g.Sum(t => t.Credit - t.Debit)
                                  };


            var beginningList = await cashBankFromClosePeriodQuery.Concat(cashBankFromOldPeriodQuery).ToListAsync();
            var cashBankCurrrentPeriodList = await allAccountQuery.ToListAsync();

            var locations = cashBankCurrrentPeriodList
                           .OrderBy(s => s.LocationId)
                           .GroupBy(s => new { s.LocationId, s.LocationName })
                           .Select(s => s.Key)
                           .ToList();

            var beginning = new CashFlowSummaryReportOutput()
            {
                Name = L("CashBeginningForPeriod"),
                Total = beginningList.Sum(t => t.Total),
                LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Total = beginningList.Concat(cashBankCurrrentPeriodList).Where(s => s.LocationId < l.LocationId).Sum(t => t.Total)
                })
                .ToDictionary(k => k.LocationId, v => v)
            };


            var categories = await _cashFlowTemplateCategoryRepository.GetAll()
                                  .AsNoTracking()
                                  .Where(s => s.TemplateId == template.Id)
                                  .Select(s => new
                                  {
                                      CategoryId = s.CategoryId,
                                      CategoryName = s.Category.Name,
                                      SortOrder = s.Category.SortOrder,
                                      Type = s.Category.Type
                                  })
                                  .OrderBy(s => s.SortOrder)
                                  .ToListAsync();

            var templateAccounts = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking()
                                         .Where(s => s.TemplateId == template.Id)
                                         .Select(s => new
                                         {
                                             s.AccountId,
                                             s.Account.AccountCode,
                                             s.Account.AccountName,
                                             s.CategoryId,
                                             InAccountGroupId = s.AccountGroupId,
                                             InAccountGroupName = s.AccountGroupId.HasValue ? s.AccontGroup.Name : "",
                                             InAccountGroupSortOrder = s.AccountGroupId.HasValue ? s.AccontGroup.SortOrder : 0,
                                             OutAccountGroupId = s.OutAccountGroupId,
                                             OutAccountGroupName = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.Name : "",
                                             OutAccountGroupSortOrder = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.SortOrder : 0
                                         })
                                         .OrderBy(s => s.InAccountGroupSortOrder)
                                         .ThenBy(s => s.AccountCode)
                                         .ToListAsync();

            var inAccountTemplates = templateAccounts
                                    .GroupBy(s => new
                                    {
                                        s.CategoryId,
                                        AccountId = s.InAccountGroupId.HasValue ? s.InAccountGroupId.Value : s.AccountId,
                                        AccountCode = s.InAccountGroupId.HasValue ? "" : s.AccountCode,
                                        AccountName = s.InAccountGroupId.HasValue ? s.InAccountGroupName : s.AccountName,
                                        SortOrder = s.InAccountGroupSortOrder
                                    })
                                    .Select(s => new
                                    {
                                        s.Key.CategoryId,
                                        s.Key.AccountId,
                                        s.Key.AccountCode,
                                        s.Key.AccountName,
                                        s.Key.SortOrder,
                                        SubAccounts = s.Select(a => a.AccountId).ToList()
                                    })
                                    .ToList();

            var outAccountTemplates = templateAccounts
                                    .OrderBy(s => s.OutAccountGroupSortOrder)
                                    .ThenBy(s => s.AccountCode)
                                    .GroupBy(s => new
                                    {
                                        s.CategoryId,
                                        AccountId = s.OutAccountGroupId.HasValue ? s.OutAccountGroupId.Value : s.AccountId,
                                        AccountCode = s.OutAccountGroupId.HasValue ? "" : s.AccountCode,
                                        AccountName = s.OutAccountGroupId.HasValue ? s.OutAccountGroupName : s.AccountName,
                                        SortOrder = s.OutAccountGroupSortOrder
                                    })
                                    .Select(s => new
                                    {
                                        s.Key.CategoryId,
                                        s.Key.AccountId,
                                        s.Key.AccountCode,
                                        s.Key.AccountName,
                                        s.Key.SortOrder,
                                        SubAccounts = s.Select(a => a.AccountId).ToList()
                                    })
                                    .ToList();


            var cashInFlows = new List<CashFlowCategoryReportOutput>();
            var cashOutFlows = new List<CashFlowCategoryReportOutput>();
            var cashInFlow = new CashFlowSummaryReportOutput();
            var cashOutFlow = new CashFlowSummaryReportOutput();

            if (template.SplitCashInAndCashOutFlow)
            {
                cashInFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)    
                .Select(c =>
                {
                    var items = inAccountTemplates
                              .Where(s => s.CategoryId == c.CategoryId)
                              .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Credit != 0)))
                              .Select(s => new CashFlowReportOutput
                              {
                                  CategoryId = c.CategoryId,
                                  CategoryName = c.CategoryName,
                                  AccountId = s.AccountId,
                                  AccountCode = s.AccountCode,
                                  AccountName = s.AccountName,
                                  AccountIds = s.SubAccounts,
                                  Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Credit),
                                  LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                  {
                                      LocationId = l.LocationId,
                                      LocationName = l.LocationName,
                                      Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Credit),
                                  })
                                  .ToDictionary(k => k.LocationId, v => v)
                              })
                              .ToList();


                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();


                cashInFlow = new CashFlowSummaryReportOutput()
                {
                    Name = L("TotalCashInflow"),
                    Total = cashInFlows.Sum(t => t.Total),
                    LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                    {
                        LocationId = l.LocationId,
                        LocationName = l.LocationName,
                        Total = cashInFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                    })
                    .ToDictionary(k => k.LocationId, v => v)
                };


                cashOutFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = outAccountTemplates
                              .Where(s => s.CategoryId == c.CategoryId)
                              .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Debit != 0)))
                              .Select(s => new CashFlowReportOutput
                              {
                                  CategoryId = c.CategoryId,
                                  CategoryName = c.CategoryName,
                                  AccountId = s.AccountId,
                                  AccountCode = s.AccountCode,
                                  AccountName = s.AccountName,
                                  AccountIds = s.SubAccounts,
                                  Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Debit),
                                  LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                  {
                                      LocationId = l.LocationId,
                                      LocationName = l.LocationName,
                                      Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Debit),
                                  })
                                  .ToDictionary(k => k.LocationId, v => v)
                              })
                              .ToList();

                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();

                cashOutFlow = new CashFlowSummaryReportOutput()
                {
                    Name = L("TotalCashOutflow"),
                    Total = cashOutFlows.Sum(t => t.Total),
                    LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                    {
                        LocationId = l.LocationId,
                        LocationName = l.LocationName,
                        Total = cashOutFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                    })
                    .ToDictionary(k => k.LocationId, v => v)
                };
            }
            else
            {
                cashInFlows = categories
                .Where(s => s.Type != CashFlowCategoryType.CashTransfer)
                .Select(c =>
                {
                    var items = inAccountTemplates
                               .Where(s => s.CategoryId == c.CategoryId)
                               .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a)))
                               .Select(s => new CashFlowReportOutput
                               {
                                   CategoryId = c.CategoryId,
                                   CategoryName = c.CategoryName,
                                   AccountId = s.AccountId,
                                   AccountCode = s.AccountCode,
                                   AccountName = s.AccountName,
                                   AccountIds = s.SubAccounts,
                                   Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Total),
                                   LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                   {
                                       LocationId = l.LocationId,
                                       LocationName = l.LocationName,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Total),
                                   })
                                   .ToDictionary(k => k.LocationId, v => v)
                               })
                               .ToList();

                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();
            }


            var netCashFlow = new CashFlowSummaryReportOutput
            {
                Name = L("NetCashFlowForPeriod"),
                Total = cashInFlows.Sum(t => t.Total) - cashOutFlows.Sum(t => t.Total),
                LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Total = cashInFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total) - cashOutFlows.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                })
                .ToDictionary(k => k.LocationId, v => v)
            };

            var cashEnding = new CashFlowSummaryReportOutput
            {
                Name = L("CashEndingForPeriod"),
                Total = beginning.Total + netCashFlow.Total,
                LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                {
                    LocationId = l.LocationId,
                    LocationName = l.LocationName,
                    Total = beginning.LocationColumnDic[l.LocationId].Total + netCashFlow.LocationColumnDic[l.LocationId].Total,
                })
                .ToDictionary(k => k.LocationId, v => v)
            };


            var cashTransfers = new List<CashFlowCategoryReportOutput>();
            var cashTransferCategories = categories.Where(s => s.Type == CashFlowCategoryType.CashTransfer).ToList();
            if (cashTransferCategories.Any())
            {
                cashTransfers = cashTransferCategories
                .Select(c =>
                {
                    var inItems = inAccountTemplates
                                   .Where(s => s.CategoryId == c.CategoryId)
                                   .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Credit != 0)))
                                   .Select(s => new CashFlowReportOutput
                                   {
                                       CategoryId = c.CategoryId,
                                       CategoryName = c.CategoryName,
                                       AccountId = s.AccountId,
                                       AccountCode = s.AccountCode,
                                       AccountName = s.AccountName,
                                       AccountIds = s.SubAccounts,
                                       CategorySortOrder = s.SortOrder,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => t.Credit),
                                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                       {
                                           LocationId = l.LocationId,
                                           LocationName = l.LocationName,
                                           Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => t.Credit),
                                       })
                                       .ToDictionary(k => k.LocationId, v => v)
                                   })
                                   .ToList();

                    var outItems = outAccountTemplates
                                   .Where(s => s.CategoryId == c.CategoryId)
                                   .Where(s => s.SubAccounts.Any(a => cashBankCurrrentPeriodList.Any(t => t.AccountId == a && t.Debit != 0)))
                                   .Select(s => new CashFlowReportOutput
                                   {
                                       CategoryId = c.CategoryId,
                                       CategoryName = c.CategoryName,
                                       AccountId = s.AccountId,
                                       AccountCode = s.AccountCode,
                                       AccountName = s.AccountName,
                                       AccountIds = s.SubAccounts,
                                       CategorySortOrder = s.SortOrder,
                                       Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId)).Sum(t => -t.Debit),
                                       LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                                       {
                                           LocationId = l.LocationId,
                                           LocationName = l.LocationName,
                                           Total = cashBankCurrrentPeriodList.Where(a => s.SubAccounts.Contains(a.AccountId) && a.LocationId == l.LocationId).Sum(t => -t.Debit),
                                       })
                                       .ToDictionary(k => k.LocationId, v => v)
                                   })
                                   .ToList();

                    var items = inItems.Concat(outItems).OrderBy(s => s.CategorySortOrder).ThenBy(s => s.AccountCode).ToList();
                    return new CashFlowCategoryReportOutput
                    {
                        CategoryName = c.CategoryName,
                        SortOrder = c.SortOrder,
                        Accounts = items,
                        Total = items.Sum(t => t.Total),
                        LocationColumnDic = locations.Select(l => new CashFlowReportLocationColumn
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Total = items.Sum(t => t.LocationColumnDic[l.LocationId].Total),
                        })
                        .ToDictionary(k => k.LocationId, v => v)
                    };
                })
                .ToList();
            }

            var result = new CashFlowReportResultOutput
            {
                SplitCashInAndCashOutFlow = template.SplitCashInAndCashOutFlow,
                CashBeginning = beginning,
                CashInFlow = cashInFlow,
                CashOutFlow = cashOutFlow,
                CashInFlows = cashInFlows,
                CashOutFlows = cashOutFlows,
                NetCashFlow = netCashFlow,
                CashEnding = cashEnding,
                CashTransfers = cashTransfers,
                CashBankAccountTypes = cashBankAccountTypeIds,
                ColumnHeaders = locations.ToDictionary(k => k.LocationId, v => v.LocationName)
            };

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow)]
        public ReportOutput GetDirectCashFlowReportTemplate()
        {
            
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                    
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ViewOption",
                        ColumnLength = 150,
                        ColumnTitle = "View Option",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = false,
                        IsDisplay = false,
                        DefaultValue = ((int)ViewOption.Standard).ToString()
                     },                   
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Account",
                        ColumnLength = 450,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DefaultValue = "Account",
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },                  
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 150,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.Money,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        DisableDefault = false,
                    },

                },
                Groupby = "",
                HeaderTitle = "Direct Cash Flow",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "Cash Flow Template",
                    ColumnTitle = "Cash Flow Template",
                    DefaultValue = ReportType.ReportType_DriectCashFlowTemplate
                },
                DefaultTemplate2 = new DefaultSaveTemplateOutput
                {
                    ColumnName = "Ledger",
                    ColumnTitle = "AccountLedgerTemplate",
                    DefaultValue = ReportType.ReportType_Ledger
                }
            };

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow_Export_Excel)]
        public async Task<FileDto> ExportExcelDirectCashFlowReport(ExportDirectCashFlowReportInput input)
        {
            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var reportData = await GetListDirectCashFlowReport(input);
            var locationsColumns = reportData.ColumnHeaders ?? new Dictionary<long, string>();

            var result = new FileDto();

            var roundDigits = (await GetCurrentCycleAsync()).RoundingDigit;

            ////Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1

                #region Row 2 Header
                string header = "";
                if (input.FromDate.Date <= new DateTime(1970, 01, 01).Date)
                {
                    header = "As Of - " + input.ToDate.ToString("dd-MM-yyyy");
                }
                else
                {
                    header = input.FromDate.ToString("dd-MM-yyyy") + " " + input.ToDate.ToString("dd-MM-yyyy");
                }
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    if (i.ColumnName != "Account") ws.Column(colHeaderTable).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    colHeaderTable += 1;
                }

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet


                if (reportData.SplitCashInAndCashOutFlow)
                {
                    AddTextToCell(ws, rowBody, 1, L("CashInflow"), false);
                    rowBody++;

                    foreach (var c in reportData.CashInFlows)
                    {
                        AddTextToCell(ws, rowBody, 1, $"{L("CashInflowFrom")} {c.CategoryName}", false, 4);
                        rowBody++;

                        foreach (var r in c.Accounts)
                        {
                            colHeaderTable = 1;
                            foreach (var i in reportCollumnHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountName}", false, 8);
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                                }
                                else
                                {
                                    var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                                }
                                colHeaderTable += 1;
                            }
                            rowBody++;
                        }


                        colHeaderTable = 1;
                        foreach (var i in reportCollumnHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                AddTextToCell(ws, rowBody, colHeaderTable, $"{L("TotalCashInflowFrom")} {c.CategoryName}", true, 4);
                            }
                            else if (i.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, colHeaderTable, c.Total, true, false, false, roundDigits);
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                            }
                            colHeaderTable += 1;
                        }
                        rowBody++;
                    }

                    colHeaderTable = 1;
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, colHeaderTable, $"{reportData.CashInFlow.Name}", true);
                        }
                        else if (i.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashInFlow.Total, true, false, false, roundDigits);
                        }
                        else
                        {
                            var val = reportData.CashInFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.CashInFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                        }
                        colHeaderTable += 1;
                    }
                    rowBody++;



                    AddTextToCell(ws, rowBody, 1, L("CashOutflow"), false);
                    rowBody++;

                    foreach (var c in reportData.CashOutFlows)
                    {
                        AddTextToCell(ws, rowBody, 1, $"{L("CashOutflowFrom")} {c.CategoryName}", false, 4);
                        rowBody++;

                        foreach (var r in c.Accounts)
                        {
                            colHeaderTable = 1;
                            foreach (var i in reportCollumnHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountName}", false, 8);
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                                }
                                else
                                {
                                    var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                                }
                                colHeaderTable += 1;
                            }
                            rowBody++;
                        }


                        colHeaderTable = 1;
                        foreach (var i in reportCollumnHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                AddTextToCell(ws, rowBody, colHeaderTable, $"{L("TotalCashOutflowFrom")} {c.CategoryName}", true, 4);
                            }
                            else if (i.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, colHeaderTable, c.Total, true, false, false, roundDigits);
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                            }
                            colHeaderTable += 1;
                        }
                        rowBody++;
                    }

                    colHeaderTable = 1;
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            AddTextToCell(ws, rowBody, colHeaderTable, $"{reportData.CashOutFlow.Name}", true);
                        }
                        else if (i.ColumnName == "Total")
                        {
                            AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashOutFlow.Total, true, false, false, roundDigits);
                        }
                        else
                        {
                            var val = reportData.CashOutFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.CashOutFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                        }
                        colHeaderTable += 1;
                    }
                    rowBody++;

                }
                else
                {
                    foreach(var c in reportData.CashInFlows)
                    {
                        AddTextToCell(ws, rowBody, 1, $"{L("CashFlowFrom")} {c.CategoryName}", false);
                        rowBody++;

                        foreach (var r in c.Accounts)
                        {
                            colHeaderTable = 1;
                            foreach (var i in reportCollumnHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountName}", false, 4);
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                                }
                                else
                                {
                                    var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                                }
                                colHeaderTable += 1;
                            }
                            rowBody++;
                        }


                        colHeaderTable = 1;
                        foreach (var i in reportCollumnHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                AddTextToCell(ws, rowBody, colHeaderTable, $"{L("TotalCashFlowFrom")} {c.CategoryName}" , true);
                            }
                            else if (i.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, colHeaderTable, c.Total, true, false, false, roundDigits);
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                            }
                            colHeaderTable += 1;
                        }
                        rowBody++;
                    }
                }


                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlow.Name, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.NetCashFlow.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.NetCashFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.NetCashFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;


                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.CashBeginning.Name, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashBeginning.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.CashBeginning.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.CashBeginning.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;



                if(!reportData.CashTransfers.IsNullOrEmpty())
                {
                    foreach (var c in reportData.CashTransfers)
                    {
                        AddTextToCell(ws, rowBody, 1, $"{c.CategoryName}", false);
                        rowBody++;

                        foreach (var r in c.Accounts)
                        {
                            colHeaderTable = 1;
                            foreach (var i in reportCollumnHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    AddTextToCell(ws, rowBody, colHeaderTable, $"{r.AccountName}", false, 4);
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    AddNumberToCell(ws, rowBody, colHeaderTable, r.Total, false, false, false, roundDigits);
                                }
                                else
                                {
                                    var val = r.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              r.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    AddNumberToCell(ws, rowBody, colHeaderTable, val, false, false, false, roundDigits);
                                }
                                colHeaderTable += 1;
                            }
                            rowBody++;
                        }


                        colHeaderTable = 1;
                        foreach (var i in reportCollumnHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                AddTextToCell(ws, rowBody, colHeaderTable, $"{L("Total")} {c.CategoryName}", true);
                            }
                            else if (i.ColumnName == "Total")
                            {
                                AddNumberToCell(ws, rowBody, colHeaderTable, c.Total, true, false, false, roundDigits);
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                            }
                            colHeaderTable += 1;
                        }
                        rowBody++;
                    }
                }



                colHeaderTable = 1;
                foreach (var i in reportCollumnHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        AddTextToCell(ws, rowBody, colHeaderTable, reportData.CashEnding.Name, true);
                    }
                    else if (i.ColumnName == "Total")
                    {
                        AddNumberToCell(ws, rowBody, colHeaderTable, reportData.CashEnding.Total, true, false, false, roundDigits);
                    }
                    else
                    {
                        var val = reportData.CashEnding.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.CashEnding.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        AddNumberToCell(ws, rowBody, colHeaderTable, val, true, false, false, roundDigits);
                    }
                    colHeaderTable += 1;
                }
                rowBody++;



                #endregion Row Body

                #region Row Footer 
                //if (reportHasShowFooterTotal.Count > 0)
                //{
                //    int footerRow = rowBody;
                //    int footerColNumber = 1;
                //    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                //    foreach (var i in reportCollumnHeader)
                //    {
                //        if (i.AllowFunction != null)
                //        {

                //        }

                //        footerColNumber += 1;
                //    }
                //}
                #endregion Row Footer


                result.FileName = $"{sheetName}_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlow_Export_Pdf)]
        public async Task<FileDto> ExportPDFDirectCashFlowReport(ExportDirectCashFlowReportInput input)
        {
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }
            var digit = (await GetCurrentCycleAsync()).RoundingDigit;
            var user = await GetCurrentUserAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;

            var reportData = await GetListDirectCashFlowReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "BalanceSheet.pdf",
                    FileToken = "BalanceSheet.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.Where(c => c.Visible).OrderBy(x => x.SortOrder).ToList();
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                foreach (var i in viewHeader)
                {                   
                    if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                    {
                        var align = i.ColumnName == "Account" ? "" : "style='text-align: center'";

                        var rowHeader = $"<th {align} width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                        contentHeader += rowHeader;
                        totalTableWidth += i.ColumnLength;
                    }
                    else
                    {
                        i.Visible = false;
                        reportCountColHead--;
                    }
                    totalVisibleColsWidth += i.ColumnLength;
                }


                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                viewHeader = viewHeader.Where(t => t.Visible).ToList();

                #region Row Body     


                if (reportData.SplitCashInAndCashOutFlow)
                {
                    var cashInFlowHeaderContent = "";
                    foreach (var i in viewHeader)
                    {  
                        if (i.ColumnName == "Account")
                        {
                            cashInFlowHeaderContent += $"<td>{L("CashInflow")}</td>";
                        }
                        else
                        {
                            cashInFlowHeaderContent += $"<td style='text-align: right'></td>";
                        }
                    }
                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowHeaderContent}</tr>";

                  
                    foreach(var c in reportData.CashInFlows)
                    {
                        var cashInFlowCategoryHeaderContent = "";
                        foreach (var i in viewHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                cashInFlowCategoryHeaderContent += $"<td style='text-indent: 15px'>{L("CashInflowFrom")} { c.CategoryName }</td>";
                            }
                            else
                            {
                                cashInFlowCategoryHeaderContent += $"<td style='text-align: right'></td>";
                            }

                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowCategoryHeaderContent}</tr>";


                        foreach (var a in c.Accounts)
                        {
                            var cashInFlowCategoryContent = "";
                            foreach (var i in viewHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    cashInFlowCategoryContent += $"<td style='text-indent: 30px'>{a.AccountName}</td>";
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    cashInFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(a.Total, digit), digit)}</td>";
                                }
                                else
                                {
                                    var val = a.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              a.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    cashInFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                                }
                            }

                            contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; '>{cashInFlowCategoryContent}</tr>";
                        }


                        var cashInFlowCategoryTotalContent = "";
                        foreach (var i in viewHeader)
                        {

                            if (i.ColumnName == "Account")
                            {
                                cashInFlowCategoryTotalContent += $"<td style='text-indent: 15px'>{L("TotalCashInflowFrom")} {c.CategoryName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                cashInFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(c.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                cashInFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowCategoryTotalContent}</tr>";

                    }


                    var totalCashInFlowTotalContent = "";
                    foreach (var i in viewHeader)
                    {  
                        if (i.ColumnName == "Account")
                        {
                            totalCashInFlowTotalContent += $"<td>{reportData.CashInFlow.Name }</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            totalCashInFlowTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashInFlow.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.CashInFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.CashInFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            totalCashInFlowTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{totalCashInFlowTotalContent}</tr>";



                    var cashOutFlowHeaderContent = "";
                    foreach (var i in viewHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            cashOutFlowHeaderContent += $"<td>{L("CashOutflow")}</td>";
                        }
                        else
                        {
                            cashOutFlowHeaderContent += $"<td style='text-align: right'></td>";
                        }
                    }
                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashOutFlowHeaderContent}</tr>";


                    foreach (var c in reportData.CashOutFlows)
                    {
                        var cashOutFlowCategoryHeaderContent = "";
                        foreach (var i in viewHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                cashOutFlowCategoryHeaderContent += $"<td style='text-indent: 15px'>{L("CashOutflowFrom")} {c.CategoryName}</td>";
                            }
                            else
                            {
                                cashOutFlowCategoryHeaderContent += $"<td style='text-align: right'></td>";
                            }

                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashOutFlowCategoryHeaderContent}</tr>";


                        foreach (var a in c.Accounts)
                        {
                            var cashOutFlowCategoryContent = "";
                            foreach (var i in viewHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    cashOutFlowCategoryContent += $"<td style='text-indent: 30px'>{a.AccountName}</td>";
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    cashOutFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(a.Total, digit), digit)}</td>";
                                }
                                else
                                {
                                    var val = a.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              a.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    cashOutFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                                }
                            }

                            contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; '>{cashOutFlowCategoryContent}</tr>";
                        }


                        var cashOutFlowCategoryTotalContent = "";
                        foreach (var i in viewHeader)
                        {

                            if (i.ColumnName == "Account")
                            {
                                cashOutFlowCategoryTotalContent += $"<td style='text-indent: 15px'>{L("TotalCashOutflowFrom")} {c.CategoryName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                cashOutFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(c.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                cashOutFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashOutFlowCategoryTotalContent}</tr>";

                    }


                    var totalCashOutFlowTotalContent = "";
                    foreach (var i in viewHeader)
                    {
                        if (i.ColumnName == "Account")
                        {
                            totalCashOutFlowTotalContent += $"<td>{reportData.CashOutFlow.Name}</td>";
                        }
                        else if (i.ColumnName == "Total")
                        {
                            totalCashOutFlowTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashOutFlow.Total, digit), digit)}</td>";
                        }
                        else
                        {
                            var val = reportData.CashOutFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                      reportData.CashOutFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                            totalCashOutFlowTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                        }
                    }
                    contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{totalCashOutFlowTotalContent}</tr>";

                }
                else
                {
                    foreach (var c in reportData.CashInFlows)
                    {
                        var cashInFlowCategoryHeaderContent = "";
                        foreach (var i in viewHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                cashInFlowCategoryHeaderContent += $"<td style='text-indent: 15px'>{L("CashFlowFrom")} {c.CategoryName}</td>";
                            }
                            else
                            {
                                cashInFlowCategoryHeaderContent += $"<td style='text-align: right'></td>";
                            }

                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowCategoryHeaderContent}</tr>";


                        foreach (var a in c.Accounts)
                        {
                            var cashInFlowCategoryContent = "";
                            foreach (var i in viewHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    cashInFlowCategoryContent += $"<td style='text-indent: 30px'>{a.AccountName}</td>";
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    cashInFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(a.Total, digit), digit)}</td>";
                                }
                                else
                                {
                                    var val = a.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              a.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    cashInFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                                }
                            }

                            contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; '>{cashInFlowCategoryContent}</tr>";
                        }


                        var cashInFlowCategoryTotalContent = "";
                        foreach (var i in viewHeader)
                        {

                            if (i.ColumnName == "Account")
                            {
                                cashInFlowCategoryTotalContent += $"<td style='text-indent: 15px'>{L("TotalCashFlowFrom")} {c.CategoryName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                cashInFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(c.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                cashInFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowCategoryTotalContent}</tr>";

                    }

                }


                var netCashFlowTotalContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        netCashFlowTotalContent += $"<td>{reportData.NetCashFlow.Name}</td>";
                    }
                    else if (i.ColumnName == "Total")
                    {
                        netCashFlowTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.NetCashFlow.Total, digit), digit)}</td>";
                    }
                    else
                    {
                        var val = reportData.NetCashFlow.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.NetCashFlow.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        netCashFlowTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{netCashFlowTotalContent}</tr>";


                var cashBeginningContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        cashBeginningContent += $"<td>{reportData.CashBeginning.Name}</td>";
                    }
                    else if (i.ColumnName == "Total")
                    {
                        cashBeginningContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashBeginning.Total, digit), digit)}</td>";
                    }
                    else
                    {
                        var val = reportData.CashBeginning.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.CashBeginning.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        cashBeginningContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashBeginningContent}</tr>";


                if (!reportData.CashTransfers.IsNullOrEmpty())
                {
                    foreach (var c in reportData.CashTransfers)
                    {
                        var cashInFlowCategoryHeaderContent = "";
                        foreach (var i in viewHeader)
                        {
                            if (i.ColumnName == "Account")
                            {
                                cashInFlowCategoryHeaderContent += $"<td >{c.CategoryName}</td>";
                            }
                            else
                            {
                                cashInFlowCategoryHeaderContent += $"<td style='text-align: right'></td>";
                            }

                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowCategoryHeaderContent}</tr>";


                        foreach (var a in c.Accounts)
                        {
                            var cashInFlowCategoryContent = "";
                            foreach (var i in viewHeader)
                            {
                                if (i.ColumnName == "Account")
                                {
                                    cashInFlowCategoryContent += $"<td style='text-indent: 15px'>{a.AccountName}</td>";
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    cashInFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(a.Total, digit), digit)}</td>";
                                }
                                else
                                {
                                    var val = a.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                              a.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                    cashInFlowCategoryContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                                }
                            }

                            contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; '>{cashInFlowCategoryContent}</tr>";
                        }


                        var cashInFlowCategoryTotalContent = "";
                        foreach (var i in viewHeader)
                        {

                            if (i.ColumnName == "Account")
                            {
                                cashInFlowCategoryTotalContent += $"<td >{L("Total")} {c.CategoryName}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                cashInFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(c.Total, digit), digit)}</td>";
                            }
                            else
                            {
                                var val = c.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                          c.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                                cashInFlowCategoryTotalContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                            }
                        }
                        contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashInFlowCategoryTotalContent}</tr>";

                    }
                }


                var cashEndingContent = "";
                foreach (var i in viewHeader)
                {
                    if (i.ColumnName == "Account")
                    {
                        cashEndingContent += $"<td>{reportData.CashEnding.Name}</td>";
                    }
                    else if (i.ColumnName == "Total")
                    {
                        cashEndingContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(reportData.CashEnding.Total, digit), digit)}</td>";
                    }
                    else
                    {
                        var val = reportData.CashEnding.LocationColumnDic.ContainsKey(Convert.ToInt32(i.ColumnName)) ?
                                  reportData.CashEnding.LocationColumnDic[Convert.ToInt32(i.ColumnName)].Total : 0;

                        cashEndingContent += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(val, digit), digit)}</td>";
                    }
                }
                contentBody += $"<tr style='page-break-before: auto; page-break-after: auto; font-weight: bold;'>{cashEndingContent}</tr>";


                #endregion Row Body

                #region Row Footer 


                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;
            });
        }

        #endregion

    }
}
