using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts;
using CorarlERP.MultiTenancy.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using CorarlERP.AutoSequences;
using System.Collections.Generic;
using CorarlERP.MultiCurrencies;
using CorarlERP.Dto;
using Abp.AspNetZeroCore.Net;
using System.IO;
using CorarlERP.Features;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Journals;
using CorarlERP.PurchaseOrders;
using CorarlERP.SaleOrders;
using CorarlERP.BankTransfers;
using CorarlERP.Productions;
using CorarlERP.TransferOrders;
using CorarlERP.ProductionPlans;

namespace CorarlERP.MultiTenancy
{
    [AbpAuthorize]
    public class CompanyProfileAppService : CorarlERPAppServiceBase, ICompanyProfileAppService
    {

        private readonly ITenantManager _tenantManager;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartofAccountRepository;
        private readonly IAccountCycleManager _accountCycleManger;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IDefaultChartOfAccountManager _defaultChartOfAccountManager;

        private readonly IRepository<MultiCurrencies.MultiCurrency, long> _multiCurrencyRepository;
        private readonly IMultiCurrencyManager _multiCurrencyManager;
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly IRepository<BankTransfer, Guid> _bankTransferRepository;
        private readonly IRepository<Production, Guid> _productionRepository;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;
        private readonly IRepository<PhysicalCount.PhysicalCount, Guid> _physicalCountRepository;
        private readonly IRepository<ProductionPlan, Guid> _productionPlanRepository;
        public CompanyProfileAppService(ITenantManager tenantManager,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IAccountCycleManager accountCycleManager,
            IRepository<ChartOfAccount, Guid> chartofAccountRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<MultiCurrencies.MultiCurrency, long> multiCurrencyRepository,
            IDefaultChartOfAccountManager defaultChartOfAccountManager,
            IAppFolders appFolders,
            IMultiCurrencyManager multiCurrencyManager,
            IAutoSequenceManager autoSequenceManager,
            IRepository<Journal, Guid> journalRepository,
            IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
            IRepository<SaleOrder, Guid> saleOrderRepository,
            IRepository<BankTransfer, Guid> bankTransferRepository,
            IRepository<Production, Guid> productionRepository,
            IRepository<TransferOrder, Guid> transferOrderRepository,
            IRepository<PhysicalCount.PhysicalCount, Guid> physicalCountRepository,
            IRepository<ProductionPlan, Guid> productionPlanRepository)
        {
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _accountCycleManger = accountCycleManager;
            _accountCycleRepository = accountCycleRepository;
            _chartofAccountRepository = chartofAccountRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _multiCurrencyManager = multiCurrencyManager;
            _multiCurrencyRepository = multiCurrencyRepository;
            _appFolders = appFolders;
            _defaultChartOfAccountManager = defaultChartOfAccountManager;
            _journalRepository = journalRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _saleOrderRepository = saleOrderRepository;
            _bankTransferRepository = bankTransferRepository;
            _productionRepository = productionRepository;
            _transferOrderRepository = transferOrderRepository;
            _physicalCountRepository = physicalCountRepository;
            _productionPlanRepository = productionPlanRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_CompanyProfile_Update)]
        public async Task<NullableIdDto<int>> Update(UpdateTenantInput input)
        {
            var @entity = await _tenantManager.GetAsync(input.Id, true);
            var tenantId = await GetCurrentTenantAsync();
            var userId = AbpSession.GetUserId();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (input.IsMultiCurrency && (input.MultiCurrencies == null || !input.MultiCurrencies.Any()))
            {
                throw new UserFriendlyException(L("CanNotBeEmpty"));
            }
          
            //Create Account cycle Default.
            if (entity.AccountCycleId == null || entity.AccountCycle?.Id == null)
            {
                entity.AccountCycle = AccountCycle.Create(input.Id, userId, input.AccountCycle.StartDate, null, input.RoundingDigit, input.RoundingDigitUnitCost);
                CheckErrors(await _accountCycleManger.CreateAsync(entity.AccountCycle));
            }
            else
            {
                var accountCycle = _accountCycleRepository.GetAll().AsNoTracking().Count();
                if (accountCycle == 1)
                {
                    entity?.AccountCycle?.Update(userId, input.AccountCycle.StartDate, null, input.RoundingDigit, input.RoundingDigitUnitCost);
                    CheckErrors(await _accountCycleManger.UpdateAsync(entity.AccountCycle));
                }
            }


            var autoSequence = await _autoSequenceRepository.GetAll().ToListAsync();
            var toUpdateDict = input.Items.ToDictionary(u => u.Id);


            foreach (var auto in autoSequence)
            {

                if (toUpdateDict.ContainsKey(auto.Id))
                {
                    var toUpdate = toUpdateDict[auto.Id];

                    if (toUpdate.DefaultPrefix != auto.DefaultPrefix ||
                       toUpdate.YearFormat != auto.YearFormat ||
                       toUpdate.SymbolFormat != auto.SymbolFormat ||
                       toUpdate.NumberFormat != auto.NumberFormat ||
                       toUpdate.CustomFormat != auto.CustomFormat ||
                       toUpdate.RequireReference != auto.RequireReference ||
                       input.IsAutoSequence != entity.IsAutoSequence)
                    {
                        auto.Update(tenantId.Id, userId, toUpdate.DocumentType,
                              toUpdate.DefaultPrefix, toUpdate.SymbolFormat,
                              toUpdate.NumberFormat, entity.IsAutoSequence && toUpdate.CustomFormat,
                              toUpdate.YearFormat, toUpdate.LastAutoSequeneNumber);
                        auto.UpdateRequireReference(toUpdate.RequireReference);
                        CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                    }
                }

            }


            //Update Tenant
            entity.UpdateTenant(
                input.LegalName,
                input.BusinessId,
                input.PhoneNumber,
                input.Website,
                input.CompanyAddress,
                input.LegalAddress,
                input.SameAsCompanyAddress,

                input.CurrencyId,
                input.FormatDateId,
                input.FormatNumberId,
                input.Name, input.Email,
                input.TransitAccountId,
                input.SaleAllowanceAccountId,
                input.BillPaymentAccountId,
                input.LocationId, input.ClassId,

                input.ItemIssueVendorCreditId,
                input.ItemIssueTransferId,
                input.ItemIssueAdjustmentId,
                input.ItemIssueOtherId,
                input.ItemIssuePhysicalCountId,

                input.ItemRecieptCustomerCreditId,
                input.ItemRecieptTransferId,
                input.ItemRecieptAdjustmentId,
                input.ItemRecieptOtherId,
                input.ItemRecieptPhysicalCountId,
                input.BankTransferAccountId,
                input.RawProductionAccountId,
                input.FinishProductionAccountId,
                input.RoundDigitAccountId,
                input.VendorAccountId,
                input.CustomerAccountId,
                input.IsAutoSequence,
                input.ExchangeLossAndGainId,
                input.SplitCashCreditPayment,
                input.InventoryAccountId,
                input.COGSAccountId,
                input.RevenueAccountId,
                input.ExpenseAccountId,
                input.ValidateProductionNetWeight,
                input.TaxId,
                input.ProductionSummaryQty,
                input.ProductionSummaryNetWeight,
                input.UseExchangeRate
                );
            //if(input.AutoItemCode == false)
            //{
            //    input.ItemCode = null;
            //    input.Prifix = null;
            //}
            entity.SetLog(input.LogoId);
            entity.SetUseBatchNo(input.UseBatchNo);

            // entity.SetAutoItemCode(input.AutoItemCode, input.Prifix, input.ItemCode);
            entity.SetProperty(input.PropertyId == 0 ? null : input.PropertyId);
            entity.SetPOSCurrency(input.POSCurrencyId == 0 ? null : input.POSCurrencyId);
            entity.SetUseDefaultAccount(input.UseDefaultAccount);
           // entity.SetUseBatchNo(input.UseFormula);
            var multiCurrencs = await _multiCurrencyRepository.GetAll().Where(t => t.TenantId == entity.Id).ToListAsync();
            var inputIds = input.MultiCurrencies.Select(s => s.Id);
            var remove = multiCurrencs.Where(s => !inputIds.Contains(s.Id)).ToList();

            if (input.IsMultiCurrency)
            {

                var add = input.MultiCurrencies.Where(s => s.Id == null);
                if (add.Any())
                {
                    foreach (var a in add)
                    {
                        var create = MultiCurrencies.MultiCurrency.Create(tenantId.Id, userId, a.CurrencyId);
                        await _multiCurrencyManager.CreateAsync(create);
                    }
                }

            }

            if (remove.Any())
            {
                foreach (var i in remove)
                {
                    await _multiCurrencyManager.RemoveAsync(i);
                }
            }
            CheckErrors(await _tenantManager.UpdateAsync(@entity));

            await _defaultChartOfAccountManager.CreateDefaultChartAccounts(entity.Id);

            return new NullableIdDto<int>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_CompanyProfile_GetDetail)]
        public async Task<TenantDetailOutput> GetDetail(EntityDto<int> input)
        {
            var @entity = await _tenantManager.GetAsync(input.Id);
            var multiCurrencys = await _multiCurrencyRepository.GetAll().Include(u => u.Currency).Where(t => t.TenantId == input.Id)

                .Select(t => new MultiCurrencyOutput
                {
                    Code = t.Currency.Code,
                    Name = t.Currency.Name,
                    CurrencyId = t.CurrencyId,
                    Id = t.Id,
                    Symbol = t.Currency.Symbol,

                }).ToListAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }




            if (entity.AccountCycleId == null || entity.AccountCycle?.Id == null)
            {
                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                //DateTime lastDay = new DateTime(year, 12, 31);
                entity.AccountCycle = new AccountCycle()
                {
                    StartDate = firstDay,
                    RoundingDigit = 2,
                    RoundingDigitUnitCost = 2,
                    EndDate = null
                };
            }
            var result = ObjectMapper.Map<TenantDetailOutput>(@entity);
            if (entity.ItemCodeSetting == ItemCodeSetting.Auto)
            {
                result.AutoItemCode = true;
            }
            result.CanChangeCurrentPeriod = true;
            var accountCycle = await _accountCycleRepository.GetAll().AsNoTracking().CountAsync();
            if (accountCycle > 1)
            {
                result.CanChangeCurrentPeriod = false;
            }

            var autoSequence = await _autoSequenceRepository.GetAll().ToListAsync();
            var autoSequenceManager = _autoSequenceManager.GetDocumentTypes();

            var autoSequenceOutput = (from au in autoSequence
                                      join aum in autoSequenceManager
                                      on au.DocumentType equals aum.Type
                                      orderby aum.SortOrer
                                      select new GetLsitAutoSequenceDetailOutput
                                      {
                                          DefaultPrefix = au.DefaultPrefix,
                                          SymbolFormat = au.SymbolFormat,
                                          NumberFormat = au.NumberFormat,
                                          CustomFormat = au.CustomFormat,
                                          YearFormat = au.YearFormat.Value,
                                          DocumentType = au.DocumentType,
                                          KeyName = aum.Group,
                                          Id = au.Id,
                                          LastAutoSequeneNumber = au.LastAutoSequenceNumber,
                                          RequireReference = au.RequireReference
                                      }).GroupBy(u => u.KeyName)
                                      .Select(u => new AutoSequenceGroupOutput
                                      {
                                          GroupByName = u.Key.ToString(),
                                          KeyName = u.Key,
                                          Items = u.Select(t => new GetLsitAutoSequenceDetailOutput
                                          {
                                              CustomFormat = t.CustomFormat,
                                              RequireReference = t.RequireReference,
                                              DefaultPrefix = t.DefaultPrefix,
                                              DocumentType = t.DocumentType,
                                              DocumentTypeName = t.DocumentType.ToString(),
                                              KeyName = t.KeyName,
                                              NumberFormat = t.NumberFormat,
                                              SymbolFormat = t.SymbolFormat,
                                              YearFormat = t.YearFormat,
                                              YearFormatString = t.YearFormat.ToString(),
                                              LastAutoSequeneNumber = t.LastAutoSequeneNumber,
                                              Id = t.Id,
                                          }).ToList()
                                      }).ToList();


            result.AutoSequenceItems = autoSequenceOutput;
            result.MultiCurreceis = multiCurrencys;

            return result;
        }

        public async Task<FileDto> DownloadDebugFile()
        {
            var tenant = await GetCurrentTenantAsync();
            //if (!tenant.DebugMode)
            //{
            //    throw new UserFriendlyException(L("DebuggerIsOff"));
            //}
            var tokenPath = Path.Combine(_appFolders.WebLogsFolder, $"Debug_{tenant.Id}.txt");
            if (!File.Exists(tokenPath))
            {
                throw new UserFriendlyException(L("NoDataOnDebugFile"));
            }
            string str = "";
            using (StreamReader r = new StreamReader(tokenPath))
            {
                str = r.ReadToEnd();
            }
            var token = $"{Guid.NewGuid()}.txt";

            var newPath = Path.Combine(_appFolders.TempFileDownloadFolder, token);
            //=================Token====================
            using (StreamWriter r = new StreamWriter(newPath))
            {
                r.Write(str);
            }
            //FileDto filePdo = new FileDto();
            ////filePdo.SubFolder = "\\Taxes\\";
            //filePdo.FileName = $"Debugger_Log_{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt";
            //filePdo.FileToken = token;
            //filePdo.FileType = MimeTypeNames.TextPlain;
            //return filePdo;


            var result = new FileDto();
            result.FileName = $"Debugger_Log_{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt";
            result.FileToken = token;
            result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
            result.FileType = MimeTypeNames.TextPlain;

            return result;


        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_CompanyProfile_Update)]
        public async Task<Guid> UpdateAutoSequence(UpdateAutoSequenceInput input)
        {
            var autoSequence = await _autoSequenceRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            autoSequence.UpdateLastAutoSequenceNumber(input.LastAutoSequenceNumber);
            await _autoSequenceRepository.UpdateAsync(autoSequence);
            return autoSequence.Id;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_CompanyProfile_Update)]
        public async Task<UpdateAutoSequenceOutput> GetDetailAutoSequence(EntityDto<Guid> input)
        {
            var autoSequence = await _autoSequenceRepository.GetAll().AsNoTracking()
                .Where(t => t.Id == input.Id).Select(t => new UpdateAutoSequenceOutput
                {
                    Id = t.Id,
                    LastAutoSequenceNumber = t.LastAutoSequenceNumber
                }).FirstOrDefaultAsync();
            return autoSequence;

        }
        [AbpAuthorize(AppPermissions.Pages_Tenants_CompanyProfile_Update)]
        public async Task<string> GetLastNo(UpdateTransactionNoInput input)
        {
            var latestCode = "";
            var currentDate = DateTime.Now;
            var prefixTem = string.IsNullOrEmpty(input.DefaultPrefix) ? "" : input.DefaultPrefix;
            var yearFormatTemp = input.YearFormat == YearFormat.YY ? currentDate.ToString("yy") :
                              input.YearFormat == YearFormat.YYYY ? currentDate.ToString("yyyy") : "";
            var symbolFormatTemp = string.IsNullOrEmpty(input.SymbolFormat) ? "" : input.SymbolFormat;
            var combinePrefixeTemp = $"{prefixTem}{yearFormatTemp}{symbolFormatTemp}";

            if (input.DocumentType == DocumentType.ItemReceipt)
            {
                latestCode = await _journalRepository
                  .GetAll().AsNoTracking()
                  .Where(t => t.JournalType == JournalType.ItemReceiptPurchase)
                  .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                  .Select(s => s.JournalNo)
                  .AsNoTracking()
                  .OrderByDescending(s => s)
                  .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.PurchaseOrder)
            {

                latestCode = await _purchaseOrderRepository
                     .GetAll().AsNoTracking()
                     .Where(s => s.OrderNumber.StartsWith(combinePrefixeTemp))
                     .Select(s => s.OrderNumber)
                     .AsNoTracking()
                     .OrderByDescending(s => s)
                     .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.ItemReceipt_CustomerCredit)
            {
                latestCode = await _journalRepository
                     .GetAll().AsNoTracking()
                     .Where(t => t.JournalType == JournalType.ItemReceiptCustomerCredit)
                     .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                     .Select(s => s.JournalNo)
                     .AsNoTracking()
                     .OrderByDescending(s => s)
                     .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.Bill)
            {
                latestCode = await _journalRepository
                        .GetAll().AsNoTracking()
                        .Where(t => t.JournalType == JournalType.Bill)
                        .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                        .Select(s => s.JournalNo)
                        .AsNoTracking()
                        .OrderByDescending(s => s)
                        .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.VendorCredit)
            {
                latestCode = await _journalRepository
                           .GetAll().AsNoTracking()
                           .Where(t => t.JournalType == JournalType.VendorCredit)
                           .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                           .Select(s => s.JournalNo)
                           .AsNoTracking()
                           .OrderByDescending(s => s)
                           .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.PayBill)
            {
                latestCode = await _journalRepository
                              .GetAll().AsNoTracking()
                              .Where(t => t.JournalType == JournalType.PayBill)
                              .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                              .Select(s => s.JournalNo)
                              .AsNoTracking()
                              .OrderByDescending(s => s)
                              .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.SaleOrder)
            {
                latestCode = await _saleOrderRepository
                         .GetAll().AsNoTracking()
                         .Where(s => s.OrderNumber.StartsWith(combinePrefixeTemp))
                         .Select(s => s.OrderNumber)
                         .AsNoTracking()
                         .OrderByDescending(s => s)
                         .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.Invoice)
            {
                latestCode = await _journalRepository
                                 .GetAll().AsNoTracking()
                                 .Where(t => t.JournalType == JournalType.Invoice)
                                 .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                 .Select(s => s.JournalNo)
                                 .AsNoTracking()
                                 .OrderByDescending(s => s)
                                 .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.CustomerCredit)
            {
                latestCode = await _journalRepository
                                    .GetAll().AsNoTracking()
                                    .Where(t => t.JournalType == JournalType.CustomerCredit)
                                    .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                    .Select(s => s.JournalNo)
                                    .AsNoTracking()
                                    .OrderByDescending(s => s)
                                    .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.ItemIssue)
            {
                latestCode = await _journalRepository
                                      .GetAll().AsNoTracking()
                                      .Where(t => t.JournalType == JournalType.ItemIssueSale)
                                      .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                      .Select(s => s.JournalNo)
                                      .AsNoTracking()
                                      .OrderByDescending(s => s)
                                      .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.ItemIssue_VendorCredit)
            {
                latestCode = await _journalRepository
                                        .GetAll().AsNoTracking()
                                        .Where(t => t.JournalType == JournalType.ItemIssueVendorCredit)
                                        .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                        .Select(s => s.JournalNo)
                                        .AsNoTracking()
                                        .OrderByDescending(s => s)
                                        .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.RecievePayment)
            {
                latestCode = await _journalRepository.GetAll().AsNoTracking()
                                                         .Where(t => t.JournalType == JournalType.ReceivePayment)
                                                         .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                                         .Select(s => s.JournalNo)
                                                         .AsNoTracking()
                                                         .OrderByDescending(s => s)
                                                         .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.BankTransferOrder)
            {
                latestCode = await _bankTransferRepository
                                          .GetAll().AsNoTracking()
                                          .Where(s => s.BankTransferNo.StartsWith(combinePrefixeTemp))
                                          .Select(s => s.BankTransferNo)
                                          .AsNoTracking()
                                          .OrderByDescending(s => s)
                                          .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.Deposit)
            {
                latestCode = await _journalRepository
                                           .GetAll().AsNoTracking()
                                           .Where(t => t.JournalType == JournalType.Deposit)
                                           .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                           .Select(s => s.JournalNo)
                                           .AsNoTracking()
                                           .OrderByDescending(s => s)
                                           .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.Withdraw)
            {
                latestCode = await _journalRepository
                                              .GetAll().AsNoTracking()
                                              .Where(t => t.JournalType == JournalType.Withdraw)
                                              .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                              .Select(s => s.JournalNo)
                                              .AsNoTracking()
                                              .OrderByDescending(s => s)
                                              .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.ProductionOrder)
            {
                latestCode = await _productionRepository
                                            .GetAll().AsNoTracking()
                                            .Where(s => s.ProductionNo.StartsWith(combinePrefixeTemp))
                                            .Select(s => s.ProductionNo)
                                            .AsNoTracking()
                                            .OrderByDescending(s => s)
                                            .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.TransferOrder)
            {

                latestCode = await _transferOrderRepository
                                            .GetAll().AsNoTracking()
                                            .Where(s => s.TransferNo.StartsWith(combinePrefixeTemp))
                                            .Select(s => s.TransferNo)
                                            .AsNoTracking()
                                            .OrderByDescending(s => s)
                                            .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.PhysicalCount)
            {
                latestCode = await _physicalCountRepository
                                            .GetAll().AsNoTracking()
                                            .Where(s => s.PhysicalCountNo.StartsWith(combinePrefixeTemp))
                                            .Select(s => s.PhysicalCountNo)
                                            .AsNoTracking()
                                            .OrderByDescending(s => s)
                                            .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.POS)
            {
                latestCode = await _journalRepository
                                            .GetAll()
                                            .Include(u => u.Invoice.TransactionTypeSale)
                                            .AsNoTracking()
                                            .Where(t => t.JournalType == JournalType.Invoice && t.Invoice.TransactionTypeSale.IsPOS)
                                            .Where(s => s.JournalNo.StartsWith(combinePrefixeTemp))
                                            .Select(s => s.JournalNo)
                                            .AsNoTracking()
                                            .OrderByDescending(s => s)
                                            .FirstOrDefaultAsync();
            }
            else if (input.DocumentType == DocumentType.ProductionPlan)
            {
                latestCode = await _productionPlanRepository
                                          .GetAll().AsNoTracking()
                                          .Where(s => s.DocumentNo.StartsWith(combinePrefixeTemp))
                                          .Select(s => s.DocumentNo)
                                          .AsNoTracking()
                                          .OrderByDescending(s => s)
                                          .FirstOrDefaultAsync();

            }
            return latestCode != null ? latestCode : "";
        }


        [AbpAuthorize(AppPermissions.Pages_Tenants_CompanyProfile_ClearDefaultValue)]
        public async Task ReSetDefaultValue() {

            var tenant = await _tenantRepository.GetAll().Where(t => t.Id == AbpSession.TenantId).FirstOrDefaultAsync();
            tenant.ResetDefaultValue();
            await _tenantRepository.UpdateAsync(tenant);

        }
    }
}
