using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerCredits;
using CorarlERP.Customers;
using CorarlERP.Invoices;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.ReceivePayments.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Invoices.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts;
using CorarlERP.AutoSequences;
using CorarlERP.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.AccountCycles;
using CorarlERP.Common.Dto;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.VendorCustomerOpenBalances;
using CorarlERP.Bills.Dto;
using Abp.Domain.Uow;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using OfficeOpenXml;
using CorarlERP.Reports;
using CorarlERP.FileStorages;
using CorarlERP.PayBills.Dto;
using CorarlERP.PayBills;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Items;

namespace CorarlERP.ReceivePayments
{
    [AbpAuthorize]
    public class ReceivePaymentAppService : ReportBaseClass, IReceivePaymentAppService
    {
        private readonly ICorarlRepository<Invoice, Guid> _invoiceRepository;
        private readonly IInvoiceManager _invoiceManager;

        private readonly ICorarlRepository<ReceivePayment, Guid> _receivePaymentRepository;
        private readonly IReceivePaymentManager _receivePaymentManager;
        private readonly ICorarlRepository<ReceivePaymentDetail, Guid> _receivePaymentDetailRepository;
        private readonly IReceivePaymentDetailManager _receivePaymentDetailManager;

        private readonly IJournalManager _journalManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;


        private readonly ICurrencyManager _currencyManager;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly ICustomerManager _customerManager;
        private readonly IRepository<Customer, Guid> _customerRepository;

        private readonly ICustomerCreditManager _customerCreditManager;
        private readonly ICorarlRepository<CustomerCredit, Guid> _customerCreditRepository;
        private readonly ICorarlRepository<ReceivePaymentExpense, Guid> _receivePaymentExpenseRepository;
        private readonly IReceivePaymentExpenseManager _receivePaymentExpenseManager;

        private readonly ICorarlRepository<ReceivePaymentExchangeRate, Guid> _exchangeRateRepository;
        private readonly ICorarlRepository<ReceivePaymentItemExchangeRate, Guid> _receivePaymentItemExchangeRateRepository;

        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly ICorarlRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<MultiCurrencies.MultiCurrency, long> _multiCurrencyRepository;
        private readonly ICorarlRepository<CustomerCreditDetail, Guid> _customerCreditDetailRepository;
        private readonly IRepository<CustomerOpenBalance, Guid> _customerOpenBalanceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly AppFolders _appFolders;
        public ReceivePaymentAppService(
            ICorarlRepository<ReceivePaymentExchangeRate, Guid> exchangeRateRepository,
            ICorarlRepository<ReceivePaymentItemExchangeRate, Guid> receivePaymentItemExchangeRateRepository,
            IJournalManager journalManager,
            ICorarlRepository<Journal, Guid> journalRepository,
            IJournalItemManager journalItemManager,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            IReceivePaymentManager receivePaymentManager,
            IInvoiceManager invoiceManager,
            IReceivePaymentDetailManager receivePaymentDetailManager,
            IRepository<CustomerOpenBalance, Guid> customerOpenBalanceRepository,
            ICustomerManager customerManager,
            ICurrencyManager currencyManager,
            IRepository<Currency, long> currencyRepository,
            IRepository<Customer, Guid> customerRepository,
            ICustomerCreditManager customerCreditManager,
            ICorarlRepository<CustomerCredit, Guid> customerCreditRepository,
            ICorarlRepository<ReceivePayment, Guid> receivePaymentRepository,
            ICorarlRepository<ReceivePaymentDetail, Guid> receivePaymentDetailRepository,
            ICorarlRepository<Invoice, Guid> invoiceRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IAutoSequenceManager autoSequenceManger,
            ICorarlRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<MultiCurrencies.MultiCurrency, long> multiCurrencyRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            ICorarlRepository<ReceivePaymentExpense, Guid> receivePaymentExpenseRepository,
            IRepository<Lock, long> lockRepository,
            IReceivePaymentExpenseManager receivePaymentExpenseManager,
            ICorarlRepository<AccountCycle, long> accountCycleRepository,
            ICorarlRepository<CustomerCreditDetail, Guid> customerCreditDetailRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IFileStorageManager fileStorageManager,
            AppFolders appFolders
        ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _receivePaymentExpenseManager = receivePaymentExpenseManager;
            _receivePaymentExpenseRepository = receivePaymentExpenseRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalManager.SetJournalType(JournalType.ReceivePayment);
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _receivePaymentDetailManager = receivePaymentDetailManager;
            _receivePaymentManager = receivePaymentManager;
            _currencyManager = currencyManager;
            _customerManager = customerManager;
            _receivePaymentRepository = receivePaymentRepository;
            _receivePaymentDetailRepository = receivePaymentDetailRepository;
            _customerRepository = customerRepository;
            _currencyRepository = currencyRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _customerCreditManager = customerCreditManager;
            _customerCreditRepository = customerCreditRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _multiCurrencyRepository = multiCurrencyRepository;
            _lockRepository = lockRepository;
            _customerOpenBalanceRepository = customerOpenBalanceRepository;
            _customerCreditDetailRepository = customerCreditDetailRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _accountCycleRepository = accountCycleRepository;
            _fileStorageManager = fileStorageManager;
            _appFolders = appFolders;
            _exchangeRateRepository = exchangeRateRepository;
            _receivePaymentItemExchangeRateRepository = receivePaymentItemExchangeRateRepository;

        }

        private async Task ValidateExchangeRate(CreateReceivePaymentInput input)
        {
            if (!input.UseExchangeRate) return;

            if (input.CurrencyId != input.MultiCurrencyId && input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));

            var find = input.ReceivePaymentDetail.Any(s => s.MultiCurrencyId != input.MultiCurrencyId && s.ExchangeRate == null);

            if (find) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));

            if (input.ExchangeLossGain != null &&
                input.ExchangeLossGain.Amount != 0 &&
                (input.ExchangeLossGain.AccountId == null || input.ExchangeLossGain.AccountId == Guid.Empty))
            {
                var tenant = await GetCurrentTenantAsync();

                if (tenant.ExchangeLossAndGainId == null || tenant.ExchangeLossAndGainId == Guid.Empty) throw new UserFriendlyException(L("IsRequired", L("ExchangeLossGainAccount")));

                input.ExchangeLossGain.AccountId = tenant.ExchangeLossAndGainId.Value;
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_Create,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_Receivepayment_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateReceivePaymentInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.ReceivePayment || t.LockKey == TransactionLockType.BankTransaction)
                   && t.IsLock == true && t.LockDate.Value.Date >= input.paymentDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            if (input.ReceivePaymentDetail.Count == 0)
            {
                throw new UserFriendlyException(L("InvoiceInfoHaveAtLeastOneRecord"));
            }
            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            await ValidateExchangeRate(input);

            if (input.Change == 0)
            {
                input.ReceivePaymentExpenseItems = new List<ReceivePaymentExpenseItem>();
            }
            else
            {
                var totalAmountCurrency = input.ReceivePaymentExpenseItems == null ? 0 : input.ReceivePaymentExpenseItems.Sum(t => t.Amount);
                var totalAmountMultiCurrency = input.ReceivePaymentExpenseItems == null ? 0 : input.ReceivePaymentExpenseItems.Sum(t => t.MultiCurrencyAmount);
                if (input.Change != totalAmountCurrency || input.MultiCurrencyChange != totalAmountMultiCurrency)
                {
                    throw new UserFriendlyException(L("YouMustAddAccountOfChange"));
                }
            }

            if (input.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit)
            {
                input.TotalPayment = 0;
                input.MultiCurrencyTotalPayment = 0;
                input.Change = 0;
                input.MultiCurrencyChange = 0;
                if (input.TotalPaymentInvoice != input.TotalPaymentCustomerCredit
                    && input.MultiCurrencyTotalPaymentInvoice != input.MultiCurrencyTotalPaymentCustomerCredit)
                {
                    throw new UserFriendlyException(L("TotalInvoicePaymentMustEqualCustomerCreditPayment"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.PaymentNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.PaymentNo, input.paymentDate, input.Memo, input.TotalPayment, input.TotalPayment, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            var isMultiCurrency = await _multiCurrencyRepository.GetAll().ToListAsync();
            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
            {
                input.MultiCurrencyId = input.CurrencyId;
            }
            else if (input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyChange = input.Change;
                input.MultiCurrencyTotalPayment = input.TotalPayment;
            }

            entity.UpdateMultiCurrency(input.MultiCurrencyId);

            if (input.ReceiveFrom == ReceiveFromRecievePayment.Cash)
            {
                if (input.TotalPayment == 0)
                {
                    throw new UserFriendlyException(L("YouMustHavePayment"));
                }
                if (input.PaymentAccountId == null || input.PaymentAccountId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("PaymentAccountIsRequired"));
                }
                //insert clearance journal item into credit
                var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                                    input.PaymentAccountId.Value,
                                                    input.Memo, input.TotalPayment, 0,
                                                    PostingKey.Payment, null);
                if (input.TotalPayment < 0)
                {
                    clearanceJournalItem.SetDebitCredit(0, input.TotalPayment * -1);
                }
                CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            }

            //insert to Receivepayment
            var receivePayment = ReceivePayment.Create(tenantId, userId, input.FiFo, input.TotalOpenBalance,
                                input.TotalPayment, input.TotalPaymentDue, input.ReceiveFrom,
                                input.MultiCurrencyTotalPayment, input.Change, input.TotalOpenBalanceCustomerCredit,
                                input.TotalPaymentCustomerCredit, input.TotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalPaymentCustomerCredit, input.TotalPaymentInvoice, input.MultiCurrencyTotalPaymentInvoice,
                                input.MultiCurrencyChange,
                                input.TotalCashInvoice,
                                input.TotalCreditInvoice,
                                input.TotalExpenseInvoice,
                                input.TotalCashCustomerCredit,
                                input.TotalCreditCustomerCredit,
                                input.TotalExpenseCustomerCredit,
                                input.UseExchangeRate,
                                input.MultiCurrencyTotalOpenBalance,
                                input.MultiCurrencyTotalOpenBalanceCustomerCredit,
                                input.MultiCurrencyTotalPaymentDue,
                                input.MultiCurrencyTotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalCashInvoice,
                                input.MultiCurrencyTotalCreditInvoice,
                                input.MultiCurrencyTotalExpenseInvoice,
                                input.MultiCurrencyTotalCashCustomerCredit,
                                input.MultiCurrencyTotalCreditCustomerCredit,
                                input.MultiCurrencyTotalExpenseCustomerCredit);

            if (input.PaymentMethodId.HasValue) receivePayment.UpdatePaymentMethodId(input.PaymentMethodId);

            // update journal type status to Receivepayment
            entity.UpdateReceivePayment(receivePayment);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _receivePaymentManager.CreateAsync(receivePayment));

            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = ReceivePaymentExchangeRate.Create(tenantId, userId, receivePayment.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }

            var @invoices = new List<Invoice>();
            var @customerCredits = new List<CustomerCredit>();
            if (input.ReceivePaymentDetail.Where(x => x.InvoiceId != null).Count() > 0)
            {
                @invoices = await _invoiceRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.InvoiceId == x.Id)).ToListAsync();
            }
            if (input.ReceivePaymentDetail.Where(x => x.CustomerCreditId != null).Count() > 0)
            {
                @customerCredits = await _customerCreditRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.CustomerCreditId == x.Id)).ToListAsync();
            }
            // loop receive Payment detail
            foreach (var i in input.ReceivePaymentDetail)
            {
                if (input.UseExchangeRate)
                {
                    i.Payment += i.LossGain;
                    i.TotalAmount -= i.LossGain;
                }

                if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                {
                    i.MultiCurrencyAmountToBeSubstract = i.AmountToBeSubstract;
                    i.MultiCurrencyOpenBalance = i.OpenBalance;
                    i.MultiCurrencyPayment = i.Payment;
                    i.MultiCurrencyTotalAmount = i.TotalAmount;
                }

                //insert to receivePayment detail
                var receivePaymentDetail = ReceivePaymentDetail.Create(tenantId, userId, receivePayment,
                                            i.InvoiceId, i.CustomerId, i.DueDate, i.OpenBalance, i.Payment,
                                            i.TotalAmount, i.MultiCurrencyOpenBalance, i.MultiCurrencyPayment,
                                            i.MultiCurrencyTotalAmount, i.CustomerCreditId, 
                                            i.Cash, i.MultiCurrencyCash, i.Credit, i.MultiCurrencyCredit, i.Expense, i.MultiCurrencyExpense, 
                                            i.LossGain, i.OpenBalanceInPaymentCurrency, i.PaymentInPaymentCurrency, i.TotalAmountInPaymentCurrency,
                                            i.CashInPaymentCurrency, i.CreditInPaymentCurrency, i.ExpenseInPaymentCurrency);
                CheckErrors(await _receivePaymentDetailManager.CreateAsync(receivePaymentDetail));

                if (input.UseExchangeRate && i.MultiCurrencyId != input.MultiCurrencyId)
                {
                    var exchange = ReceivePaymentItemExchangeRate.Create(tenantId, userId, receivePaymentDetail.Id, i.ExchangeRate.FromCurrencyId, i.ExchangeRate.ToCurrencyId, i.ExchangeRate.Bid, i.ExchangeRate.Ask);
                    await _receivePaymentItemExchangeRateRepository.InsertAsync(exchange);
                }

                JournalItem inventoryJournalItem;

                if (i.InvoiceId != null)
                {
                    //insert journal item into with credit  and default debit zero
                    inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                            i.accountId, "", 0, i.Payment, PostingKey.AR,
                                            receivePaymentDetail.Id);

                    // update total balance in invoice 
                    var updateTotalbalance = @invoices.Where(u => u.Id == receivePaymentDetail.InvoiceId).FirstOrDefault();
                    updateTotalbalance.UpdateOpenBalance(i.Payment * -1);
                    updateTotalbalance.UpdateMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);
                    if (input.Status == TransactionStatus.Publish)
                    {
                        if (i.Payment != 0 || i.MultiCurrencyPayment != 0)
                        {
                            updateTotalbalance.UpdateTotalPaid(i.Payment);
                            updateTotalbalance.UpdateMultiCurrencyTotalPaid(i.MultiCurrencyPayment);

                            if ((!input.UseExchangeRate && updateTotalbalance.OpenBalance == 0) ||
                                (input.UseExchangeRate && updateTotalbalance.MultiCurrencyOpenBalance == 0))
                            {
                                updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                            }
                            else
                            {
                                updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                            }                           
                        }
                        CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));
                    }
                }
                else
                {
                    //insert journal item into with credit  and default debit zero
                    inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                            i.accountId, "", i.Payment, 0, PostingKey.Clearance,
                                            receivePaymentDetail.Id);
                    var customerCreditOpenBalance = @customerCredits.Where(u => u.Id == i.CustomerCreditId).FirstOrDefault();

                    customerCreditOpenBalance.IncreaseOpenbalance(i.Payment * -1);
                    customerCreditOpenBalance.IncreaseMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);
                    if (input.Status == TransactionStatus.Publish)
                    {
                        customerCreditOpenBalance.IncreaseTotalPaid(i.Payment);
                        customerCreditOpenBalance.IncreaseMultiCurrencyTotalPaid(i.MultiCurrencyPayment);
                        if ((!input.UseExchangeRate && customerCreditOpenBalance.OpenBalance == 0) ||
                              (input.UseExchangeRate && customerCreditOpenBalance.MultiCurrencyOpenBalance == 0))
                        {
                            customerCreditOpenBalance.UpdateToPaid();
                        }
                        else
                        {
                            customerCreditOpenBalance.UpdateToPartial();
                        }
                    }
                    CheckErrors(await _customerCreditManager.UpdateAsync(customerCreditOpenBalance));

                }

                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
            }
            // loop of paybill expense account detail
            if ((input.ReceiveFrom == ReceiveFromRecievePayment.Cash || input.UseExchangeRate) && input.ReceivePaymentExpenseItems != null)
            {
                foreach (var i in input.ReceivePaymentExpenseItems)
                {
                    if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                    {
                        i.MultiCurrencyAmount = i.Amount;
                    }
                    //insert to pay bill detail
                    var receiveExpense = ReceivePaymentExpense.Create(tenantId, userId, receivePayment,
                                            i.AccountId, i.Amount, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                    CheckErrors(await _receivePaymentExpenseManager.CreateAsync(receiveExpense));

                    JournalItem journalItem;
                    if (i.Amount > 0)
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    i.Description, i.Amount, 0, PostingKey.PaymentChange,
                                                    receiveExpense.Id);
                    }
                    else
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    i.Description, 0, i.Amount * -1, PostingKey.PaymentChange,
                                                    receiveExpense.Id);
                    }
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));
                }
            }

            if (input.UseExchangeRate && input.ExchangeLossGain != null && input.ExchangeLossGain.Amount != 0)
            {
                var receivePaymentExpense = ReceivePaymentExpense.Create(tenantId, userId, receivePayment, input.ExchangeLossGain.AccountId,
                           input.ExchangeLossGain.Amount, 0, "Exchange Loss/Gain by System", true);
                CheckErrors(await _receivePaymentExpenseManager.CreateAsync(receivePaymentExpense));

                JournalItem journalItem;
                if (input.ExchangeLossGain.Amount > 0)
                {
                    journalItem = JournalItem.CreateJournalItem(
                                                tenantId, userId, entity, input.ExchangeLossGain.AccountId,
                                                receivePaymentExpense.Description, input.ExchangeLossGain.Amount, 0, PostingKey.PaymentChange,
                                                receivePaymentExpense.Id);
                }
                else
                {
                    journalItem = JournalItem.CreateJournalItem(
                                                tenantId, userId, entity, input.ExchangeLossGain.AccountId,
                                                receivePaymentExpense.Description, 0, -input.ExchangeLossGain.Amount, PostingKey.PaymentChange,
                                                receivePaymentExpense.Id);
                }
                CheckErrors(await _journalItemManager.CreateAsync(journalItem));
            }


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ReceivePayment, TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
           
            return new NullableIdDto<Guid>() { Id = receivePayment.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetList)]
        public async Task<PagedResultDto<ReceivePaymentHeader>> GetList(GetLIstReceivePaymentInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var jQuery = _journalRepository.GetAll()
                        .Where(u => u.JournalType == JournalType.ReceivePayment)
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                        .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                        .WhereIf(input.FromDate != null && input.ToDate != null,
                            (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                u.Reference.ToLower().Contains(input.Filter.ToLower()))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            Id = s.Id,
                            JournalNo = s.JournalNo,
                            Reference = s.Reference,
                            CreationTimeIndex = s.CreationTimeIndex,
                            Status = s.Status,
                            Memo = s.Memo,
                            PaymentDate = s.Date,
                            ReceivePaymentId = s.ReceivePaymentId,
                            CurrencyId = s.CurrencyId,
                            LocationId = s.LocationId,
                            CreatorUserId = s.CreatorUserId
                        });

            var currencyQuery = GetCurrencies();
            var locationQuery = GetLocations(null, input.Locations);
            var userQuery = GetUsers(input.Users);

            var journalQuery = from j in jQuery
                               join c in currencyQuery
                               on j.CurrencyId equals c.Id
                               join l in locationQuery
                               on j.LocationId equals l.Id
                               join u in userQuery
                               on j.CreatorUserId equals u.Id
                               select new
                               {
                                   Id = j.Id,
                                   JournalNo = j.JournalNo,
                                   Reference = j.Reference,
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   Status = j.Status,
                                   Memo = j.Memo,
                                   PaymentDate = j.PaymentDate,
                                   ReceivePaymentId = j.ReceivePaymentId,
                                   CurrencyId = j.CurrencyId,
                                   LocationId = j.LocationId,
                                   CreatorUserId = j.CreatorUserId,
                                   CurrencyCode = c.Code,
                                   LocationName = l.LocationName,
                                   UserName = u.UserName
                               };

            var accountQuery = GetAccounts(input.Accounts);
            var journalItemQuery = from ji in _journalItemRepository.GetAll()
                                       .Where(s => s.Key == PostingKey.Payment || s.Key == PostingKey.Clearance)
                                       .WhereIf(input.Accounts != null && input.Accounts.Count > 0,
                                            u => input.Accounts.Contains(u.AccountId))
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           Id = s.Id,
                                           Identifier = s.Identifier,
                                           Key = s.Key,
                                           AccountId = s.AccountId,
                                           JournalId = s.JournalId
                                       })
                                   join a in accountQuery
                                   on ji.AccountId equals a.Id
                                   select new
                                   {
                                       Id = ji.Id,
                                       Identifier = ji.Identifier,
                                       Key = ji.Key,
                                       AccountId = ji.AccountId,
                                       AccountName = a.AccountName,
                                       JournalId = ji.JournalId
                                   };

            var receivePaymentQuery = from p in _receivePaymentRepository.GetAll()
                                          .AsNoTracking()
                                          .Select(s => new
                                          {
                                              Id = s.Id,
                                              ReceiveFrom = s.ReceiveFrom,
                                              TotalPayment = s.TotalPayment,
                                              TotalPaymentInvoice = s.TotalPaymentInvoice,
                                              TotalPaymentCustomerCredit = s.TotalPaymentCustomerCredit,
                                              TotalOpenBalance = s.TotalOpenBalance
                                          })
                                      join j in journalQuery
                                      on p.Id equals j.ReceivePaymentId
                                      select new
                                      {
                                          Id = p.Id,
                                          ReceiveFrom = p.ReceiveFrom,
                                          TotalPayment = p.TotalPayment,
                                          TotalPaymentInvoice = p.TotalPaymentInvoice,
                                          TotalPaymentCustomerCredit = p.TotalPaymentCustomerCredit,
                                          TotalOpenBalance = p.TotalOpenBalance,
                                          JournalId = j.Id,
                                          JournalNo = j.JournalNo,
                                          Reference = j.Reference,
                                          CreationTimeIndex = j.CreationTimeIndex,
                                          CurrencyCode = j.CurrencyCode,
                                          CurrencyId = j.CurrencyId,
                                          Status = j.Status,
                                          Memo = j.Memo,
                                          PaymentDate = j.PaymentDate,
                                          ReceivePaymentId = j.ReceivePaymentId,
                                          CreatorUserId = j.CreatorUserId,
                                          LocationId = j.LocationId,
                                          UserName = j.UserName,
                                          LocationName = j.LocationName
                                      };

            var paymentQuery = from p in receivePaymentQuery
                               join ji in journalItemQuery
                               on p.JournalId equals ji.JournalId
                               into jItems
                               where jItems.Count() > 0
                               select new
                               {
                                   CreationTimeIndex = p.CreationTimeIndex,
                                   User = new UserDto
                                   {
                                       Id = p.CreatorUserId.Value,
                                       UserName = p.UserName,
                                   },
                                   Currency = new CurrencyDetailOutput
                                   {
                                       Id = p.CurrencyId,
                                       Code = p.CurrencyCode,
                                   },
                                   CurrencyId = p.CurrencyId,
                                   Memo = p.Memo,
                                   PaymentDate = p.PaymentDate,
                                   JournalNo = p.JournalNo,
                                   Reference = p.Reference,
                                   Status = p.Status,
                                   Id = p.Id,
                                   TotalPayment = p.TotalPayment,
                                   TotalPaymentInvoice = p.TotalPaymentInvoice,
                                   TotalCustomerCreditPayment = p.TotalPaymentCustomerCredit,
                                   TotalAmount = p.TotalOpenBalance,
                                   ReceiveFrom = p.ReceiveFrom,
                                   Location = new LocationSummaryOutput
                                   {
                                       Id = p.LocationId.Value,
                                       LocationName = p.LocationName,
                                   },
                                   AccountName = p.ReceiveFrom == ReceiveFromRecievePayment.Cash ?
                                   jItems.Where(t => t.Key == PostingKey.Payment && t.Identifier == null).Select(x => x.AccountName).FirstOrDefault() :
                                   jItems.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountName).FirstOrDefault(),
                               };

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, null, customerTypeMemberIds);
            var receivePaymentDetailQuery = from ri in _receivePaymentDetailRepository.GetAll()
                                                .WhereIf(input.Customers != null && input.Customers.Count > 0,
                                                        u => input.Customers.Contains(u.CustomerId))
                                                .WhereIf(input.CustomerTypes != null && input.CustomerTypes.Count > 0,
                                                        u => u.Customer != null && u.Customer.CustomerTypeId != null && input.CustomerTypes.Contains(u.Customer.CustomerTypeId.Value))
                                                .AsNoTracking()
                                                .Select(s => new
                                                {
                                                    Id = s.Id,
                                                    ReceivePaymentId = s.ReceivePaymentId,
                                                    CustomerId = s.CustomerId
                                                })
                                            join c in customerQuery
                                            on ri.CustomerId equals c.Id
                                            select new
                                            {
                                                Id = ri.Id,
                                                ReceivePaymentId = ri.ReceivePaymentId,
                                                CustomerId = ri.CustomerId,
                                                CustomerName = c.CustomerName,
                                            };


            var query = from p in paymentQuery
                        join pi in receivePaymentDetailQuery
                        on p.Id equals pi.ReceivePaymentId
                        into pItems
                        where pItems.Count() > 0
                        select new GetListReceivPaymentOutput
                        {
                            CustomerLists = pItems.GroupBy(g => new { g.CustomerId, g.CustomerName })
                               .Select(s => new CustomerSummaryOutput
                               {
                                   CustomerName = s.Key.CustomerName,
                                   Id = s.Key.CustomerId
                               }).ToList(),
                            CreationTimeIndex = p.CreationTimeIndex,
                            User = p.User,
                            Currency = p.Currency,
                            CurrencyId = p.CurrencyId,
                            Memo = p.Memo,
                            PaymentDate = p.PaymentDate,
                            JournalNo = p.JournalNo,
                            Reference = p.Reference,
                            Status = p.Status,
                            Id = p.Id,
                            TotalPayment = p.TotalPayment,
                            TotalPaymentInvoice = p.TotalPaymentInvoice,
                            TotalCustomerCreditPayment = p.TotalCustomerCreditPayment,
                            TotalAmount = p.TotalAmount,
                            ReceiveFrom = p.ReceiveFrom,
                            Location = p.Location,
                            AccountName = p.AccountName
                        };


            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ReceivePaymentHeader>(resultCount, new List<ReceivePaymentHeader>());

            var summary = new List<BalanceSummaryReceivPayment>();

            // get total record of summary for first index
            summary.Add(
               new BalanceSummaryReceivPayment
               {
                   AccountName = null,
                   TotalPament = await query.SumAsync(t =>
                               t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice == 0 ? t.TotalPayment
                               : t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice > 0 ? t.TotalPayment + t.TotalCustomerCreditPayment
                               : t.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit ? t.TotalCustomerCreditPayment : 0)
               }
            );

            var allAcc = await query.Select(t =>
                        new List<BalanceSummaryReceivPayment> {
                            new BalanceSummaryReceivPayment{
                                AccountName = t.ReceiveFrom != ReceiveFromRecievePayment.CustomerCredit ? t.AccountName : "Credit",
                                TotalPament = t.ReceiveFrom != ReceiveFromRecievePayment.CustomerCredit ? t.TotalPayment : t.TotalCustomerCreditPayment,
                            },
                            new BalanceSummaryReceivPayment{
                                AccountName = t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice > 0 ? "Credit" : "",
                                TotalPament = t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice > 0 ? t.TotalCustomerCreditPayment : 0
                            },
                        })
                        .SelectMany(s => s.Where(r => r.AccountName != ""))
                        .GroupBy(g => g.AccountName)
                        .Select(a => new BalanceSummaryReceivPayment
                        {
                            AccountName = a.Key,
                            TotalPament = a.Sum(t => t.TotalPament),
                        }).ToListAsync();

            summary = summary.Concat(allAcc).ToList();


            var headersQuery = new List<ReceivePaymentHeader> { };

            if (input.Sorting.EndsWith("DESC"))
            {
                IQueryable<GetListReceivPaymentOutput> descQuery;

                if (input.Sorting.ToLower().StartsWith("paymentdate"))
                {
                    descQuery = query.OrderByDescending(s => s.PaymentDate.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    descQuery = query.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("totalpayment"))
                {
                    descQuery = query.OrderByDescending(s => s.TotalPayment).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    descQuery = query.OrderByDescending(s => s.Status).ThenByDescending(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    descQuery = query.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                }

                query = descQuery;
            }
            else
            {

                IQueryable<GetListReceivPaymentOutput> ascQuery;

                if (input.Sorting.ToLower().StartsWith("paymentdate"))
                {
                    ascQuery = query.OrderBy(s => s.PaymentDate.Date).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    ascQuery = query.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("totalpayment"))
                {
                    ascQuery = query.OrderBy(s => s.TotalPayment).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    ascQuery = query.OrderBy(s => s.Status).ThenBy(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    ascQuery = query.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                }

                query = ascQuery;
            }

            headersQuery = new List<ReceivePaymentHeader> {
                new ReceivePaymentHeader
                {
                    BalanceSummaryReceicePayment = summary,
                    ReceivePaymentList = input.UsePagination ? await query.PageBy(input).ToListAsync() : await  query.ToListAsync()
                }
            };

            var @entities = headersQuery;
            return new PagedResultDto<ReceivePaymentHeader>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetList)]
        public async Task<PagedResultDto<ReceivePaymentHeader>> GetListOld(GetLIstReceivePaymentInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var @queryable = from j in _journalRepository.GetAll().Include(t => t.CreatorUser).Include(t => t.Location)
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                            .Where(u => u.JournalType == JournalType.ReceivePayment)
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))

                            .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))

                            .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                            .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Memo.ToLower().Contains(input.Filter.ToLower())).AsNoTracking()

                             join recievepaymentDetail in _receivePaymentDetailRepository.GetAll()
                            //.Include(u => u.ReceivePayment)
                            //.Include(u => u.Invoice)
                            .Include(u => u.Customer)
                            .WhereIf(input.InvoiceId != null && input.InvoiceId.Count > 0,
                                    u => input.InvoiceId.Contains(u.InvoiceId))
                            .WhereIf(input.Customers != null && input.Customers.Count > 0,
                                    u => input.Customers.Contains(u.CustomerId))
                            .AsNoTracking()
                             on j.ReceivePaymentId equals recievepaymentDetail.ReceivePaymentId
                             into receivePaymentItems

                             join recievePayment in _receivePaymentRepository.GetAll().AsNoTracking()
                             on j.ReceivePaymentId equals recievePayment.Id

                             join jItem in _journalItemRepository.GetAll().Include(t => t.Account)
                            .WhereIf(input.Accounts != null && input.Accounts.Count > 0,
                                    u => input.Accounts.Contains(u.AccountId))
                            .AsNoTracking()

                             on j.Id equals jItem.JournalId into journalItems
                             where receivePaymentItems != null && receivePaymentItems.Count() > 0 &&
                                   journalItems != null && journalItems.Count() > 0 &&
                                   recievePayment.Id == j.ReceivePaymentId

                             select new
                             {
                                 accounts = journalItems,
                                 receivePayments = recievePayment,
                                 Journal = j,
                                 Customers =
                                         receivePaymentItems
                                         .GroupBy(u => u.Customer)
                                            .Select(t => new CustomerSummaryOutput
                                            {
                                                Id = t.Key.Id,
                                                AccountId = t.Key.AccountId.Value,
                                                CustomerCode = t.Key.CustomerCode,
                                                CustomerName = t.Key.CustomerName
                                            })
                             };



            var query = queryable.Select(u => new GetListReceivPaymentOutput()
            {
                CreationTimeIndex = u.Journal.CreationTimeIndex,
                CustomerLists = u.Customers.ToList(),
                User = ObjectMapper.Map<UserDto>(u.Journal.CreatorUser),
                Currency = ObjectMapper.Map<CurrencyDetailOutput>(u.Journal.Currency),
                CurrencyId = u.Journal.CurrencyId,
                Memo = u.Journal.Memo,
                Id = u.Journal.ReceivePaymentId.Value,
                PaymentDate = u.Journal.Date,
                JournalNo = u.Journal.JournalNo,
                TotalPayment = u.receivePayments.TotalPayment,
                TotalPaymentInvoice = u.receivePayments.TotalPaymentInvoice,
                TotalCustomerCreditPayment = u.receivePayments.TotalPaymentCustomerCredit,
                Status = u.Journal.Status,
                TotalAmount = u.receivePayments.TotalOpenBalance,
                ReceiveFrom = u.receivePayments.ReceiveFrom,
                Location = ObjectMapper.Map<LocationSummaryOutput>(u.Journal.Location),
                //AccountId = u.accounts.Select(t => t.Account.Id).FirstOrDefault()
                AccountName = u.receivePayments.ReceiveFrom == ReceiveFromRecievePayment.Cash ?
                                u.accounts.Where(t => t.Key == PostingKey.Payment && t.Identifier == null)
                                    .Select(x => x.Account.AccountName).FirstOrDefault() :
                                u.accounts.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.Account.AccountName).FirstOrDefault(),
                AccountId = u.receivePayments.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit ? u.accounts
                                .Where(t => t.Key == PostingKey.Payment && t.Identifier == null)
                                    .Select(x => x.AccountId).FirstOrDefault() :
                                u.accounts.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountId).FirstOrDefault(),
            });

            var summary = new List<BalanceSummaryReceivPayment>();

            if (query.Count() > 0)
            {
                // get total record of summary for first index
                summary.Add(
                   new BalanceSummaryReceivPayment
                   {
                       AccountName = null,
                       TotalPament = await query.SumAsync(t =>
                                   t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice == 0 ? t.TotalPayment
                                   : t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice > 0 ? t.TotalPayment + t.TotalCustomerCreditPayment
                                   : t.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit ? t.TotalCustomerCreditPayment : 0)
                   }
                );

                var allAcc = await query.Select(t =>

                        new List<BalanceSummaryReceivPayment> {
                            new BalanceSummaryReceivPayment{
                                AccountName = t.ReceiveFrom != ReceiveFromRecievePayment.CustomerCredit ? t.AccountName : "Credit",
                                TotalPament = t.ReceiveFrom != ReceiveFromRecievePayment.CustomerCredit ? t.TotalPayment : t.TotalCustomerCreditPayment,
                            },
                            new BalanceSummaryReceivPayment{
                                AccountName = t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice > 0 ? "Credit" : "",
                                TotalPament = t.ReceiveFrom == ReceiveFromRecievePayment.Cash && t.TotalPaymentInvoice > 0 ? t.TotalCustomerCreditPayment : 0
                            },
                        })
                        .SelectMany(s => s.Where(r => r.AccountName != ""))
                        .GroupBy(g => g.AccountName)
                        .Select(a => new BalanceSummaryReceivPayment
                        {
                            AccountName = a.Key,
                            TotalPament = a.Sum(t => t.TotalPament),
                        }).ToListAsync();

                summary = summary.Concat(allAcc).ToList();
                //summary.Add(
                //    new BalanceSummaryReceivPayment
                //    {
                //        AccountName = null,
                //        TotalPament = await query.SumAsync(t => t.ReceiveFrom == ReceiveFromRecievePayment.Cash ? t.TotalPayment : 0)
                //    }
                //);
                //var allAcc = query.GroupBy(ct => new { ct.AccountName, ct.ReceiveFrom }).Select(a => new BalanceSummaryReceivPayment
                //{
                //    AccountName = a.Key.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit ? "Credit" : a.Key.AccountName,
                //    TotalPament = a.Sum(t => t.TotalPayment),

                //});

                //summary = summary.Concat(allAcc).ToList();
            }

            var headersQuery = new List<ReceivePaymentHeader> { };

            if (input.Sorting.Contains("date") && !input.Sorting.Contains(".")) input.Sorting = input.Sorting.Replace("date", "Date.Date");
            if (input.Sorting.Contains("DESC"))
            {
                headersQuery = new List<ReceivePaymentHeader> {
                    new ReceivePaymentHeader
                    {
                        BalanceSummaryReceicePayment = summary,
                        ReceivePaymentList = await query.OrderBy(input.Sorting).ThenByDescending(t=>t.CreationTimeIndex).PageBy(input).ToListAsync()
                   }
                };
            }
            else
            {
                headersQuery = new List<ReceivePaymentHeader> {
                    new ReceivePaymentHeader
                    {
                        BalanceSummaryReceicePayment = summary,
                        ReceivePaymentList = await query.OrderBy(input.Sorting).ThenBy(t=>t.CreationTimeIndex).PageBy(input).ToListAsync()
                    }
                };
            }

            var resultCount = await query.CountAsync();
            var @entities = headersQuery;
            return new PagedResultDto<ReceivePaymentHeader>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_Find)]
        public async Task<PagedResultDto<GetListReceivPaymentOutput>> Find(GetLIstReceivePaymentInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll()
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .Where(u => u.JournalType == JournalType.ReceivePayment)
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(!input.Filter.IsNullOrEmpty(), u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))
                         .AsNoTracking()
                         on jItem.JournalId equals j.Id
                         join pb in _receivePaymentRepository.GetAll().AsNoTracking()
                          on j.ReceivePaymentId equals pb.Id
                         join pbd in _receivePaymentDetailRepository.GetAll().Include(u => u.Invoice).Include(u => u.Customer).AsNoTracking()
                         .WhereIf(input.InvoiceId != null && input.InvoiceId.Count > 0, u => input.InvoiceId.Contains(u.InvoiceId))
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                         on pb.Id equals pbd.ReceivePaymentId
                         join c in _currencyRepository.GetAll().AsNoTracking() on j.CurrencyId equals c.Id
                         group j by new { journal = j, receivPayment = pb, currency = c } into u
                         select new GetListReceivPaymentOutput
                         {
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(u.Key.currency),
                             CurrencyId = u.Key.currency.Id,
                             Memo = u.Key.journal.Memo,
                             Id = u.Key.receivPayment.Id,
                             PaymentDate = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             TotalPayment = u.Key.receivPayment.TotalPayment,
                             Status = u.Key.journal.Status,
                             TotalAmount = u.Key.receivPayment.TotalOpenBalance,
                             ReceiveFrom = u.Key.receivPayment.ReceiveFrom
                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListReceivPaymentOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_Delete,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_ReceivePayment_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var journal = await _journalRepository.GetAll()
                          .Include(u => u.ReceivePayment)
                          .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                          .FirstOrDefaultAsync();
            if (journal == null || journal.ReceivePayment == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.ReceivePayment || t.LockKey == TransactionLockType.BankTransaction)
                  && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var exchanges = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.ReceivePaymentId == input.Id).ToListAsync();
            if (exchanges.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchanges);

            var payBillItemExchanges = await _receivePaymentItemExchangeRateRepository.GetAll().AsNoTracking().Where(s => s.ReceivePaymentItem.ReceivePaymentId == input.Id).ToListAsync();
            if (payBillItemExchanges.Any()) await _receivePaymentItemExchangeRateRepository.BulkDeleteAsync(payBillItemExchanges);


            var @entity = journal.ReceivePayment;

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.ReceivePayment)
                                .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (jo != null)
                {
                    auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            //query get journal and delete
            journal.UpdateReceivePayment(null);

            //query get expense item and delete             
            var receivePaymentExpenseItems = await _receivePaymentExpenseRepository.GetAll()
                                            .Where(u => u.ReceivePaymentId == entity.Id).ToListAsync();
            foreach (var e in receivePaymentExpenseItems)
            {
                CheckErrors(await _receivePaymentExpenseManager.RemoveAsync(e));
            }

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            //query get invoice item and delete 
            var receivePaymentItems = await _receivePaymentDetailRepository.GetAll()
                                    .Include(t => t.Invoice).Include(t => t.CustomerCredit)
                                    .Where(u => u.ReceivePaymentId == entity.Id).ToListAsync();

            foreach (var r in receivePaymentItems)
            {
                if (journal.Status == TransactionStatus.Publish)
                {
                    if (r.Invoice != null)
                    {
                        // update total balance in invoice               
                        var invoiceItem = r.Invoice;
                        invoiceItem.UpdateOpenBalance(r.Payment);
                        invoiceItem.UpdateMultiCurrencyOpenBalance(r.MultiCurrencyPayment);
                        invoiceItem.UpdateTotalPaid(r.Payment * -1);
                        invoiceItem.UpdateMultiCurrencyTotalPaid(r.MultiCurrencyPayment * -1);
                        if (invoiceItem.TotalPaid == 0)
                        {
                            invoiceItem.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            invoiceItem.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _invoiceManager.UpdateAsync(invoiceItem));
                    }
                    else if (r.CustomerCredit != null)
                    {
                        var customerCreditItem = r.CustomerCredit;
                        customerCreditItem.IncreaseOpenbalance(r.Payment);
                        customerCreditItem.IncreaseMultiCurrencyOpenBalance(r.MultiCurrencyPayment);
                        customerCreditItem.IncreaseTotalPaid(r.Payment * -1);
                        customerCreditItem.IncreaseMultiCurrencyTotalPaid(r.MultiCurrencyPayment * -1);
                        if (customerCreditItem.TotalPaid == 0)
                        {
                            customerCreditItem.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            customerCreditItem.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _customerCreditManager.UpdateAsync(customerCreditItem));
                    }
                }

                CheckErrors(await _receivePaymentDetailManager.RemoveAsync(r));
            }


            CheckErrors(await _journalManager.RemoveAsync(journal));

            CheckErrors(await _receivePaymentManager.RemoveAsync(entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ReceivePayment, TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }


        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetDetail,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_ReceivePayment_View)]
        public async Task<ReceivePaymentDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository.GetAll()
                              .Include(u => u.Currency)
                              .Include(u => u.MultiCurrency)
                              .Include(u => u.ReceivePayment.PaymentMethod.PaymentMethodBase)
                              .Include(u => u.Location)
                              .Include(u => u.Class)
                              .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                              .AsNoTracking()
                              .FirstOrDefaultAsync();

            if (@journal == null || @journal.ReceivePayment == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //var clearanceAccount = await (_journalItemRepository.GetAll()
            //                           .Include(u => u.Account)
            //                           .AsNoTracking()
            //                           .Where(
            //                                u => u.JournalId == journal.Id &&
            //                                u.Key == PostingKey.Payment && u.Identifier == null
            //                            )).FirstOrDefaultAsync();

            var receivePaymentDetailItem = await (from p in _receivePaymentDetailRepository.GetAll()
                                        .Include(u => u.Customer)
                                        .Include(u => u.Invoice)
                                        .Include(u => u.CustomerCredit)
                                        .Where(u => u.ReceivePaymentId == input.Id)
                                        .AsNoTracking()

                                                  join ji in _journalItemRepository.GetAll()
                                                  .Include(u => u.Journal.MultiCurrency)
                                                  .AsNoTracking()
                                                  on p.Id equals ji.Identifier

                                                  join bj in _journalRepository.GetAll().Include(u => u.MultiCurrency)
                                                  .AsNoTracking()
                                                  on p.InvoiceId equals bj.InvoiceId into bjs
                                                  from nBj in bjs.DefaultIfEmpty()

                                                  join vj in _journalRepository.GetAll().Include(u => u.MultiCurrency)
                                                  .AsNoTracking()
                                                  on p.CustomerCreditId equals vj.CustomerCreditId into vjs
                                                  from nVj in vjs.DefaultIfEmpty()

                                                  where nBj != null || nVj != null

                                                  select new ReceivePaymentDetailItemOutput()
                                                  {
                                                      CreationTime = p.CreationTime,
                                                      InvoiceId = p.InvoiceId,

                                                      Invoice = nBj != null && p.Invoice != null ? new InvoiceSummaryforReceivePayment()
                                                      {
                                                          JournalNo = nBj.JournalNo,
                                                          Reference = nBj.Reference,
                                                          DueDate = p.Invoice.DueDate,
                                                          Date = nBj.Date,
                                                          Id = p.Invoice.Id,
                                                          MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(nBj.MultiCurrency)
                                                      } : null,
                                                      Id = p.Id,
                                                      CustomerId = p.CustomerId,
                                                      Customer = ObjectMapper.Map<CustomerSummaryOutput>(p.Customer),
                                                      AccountId = ji.AccountId,
                                                      Account = ObjectMapper.Map<ChartAccountSummaryOutput>(ji.Account),
                                                      CustomerCreditId = p.CustomerCreditId,
                                                      CustomerCredit = nVj != null && p.CustomerCredit != null ? new CustomerCreditPaymentSummary()
                                                      {
                                                          Id = nVj.CustomerCredit.Id,
                                                          Reference = nVj.Reference,
                                                          JournalNo = nVj.JournalNo,
                                                          DueDate = nVj.CustomerCredit.DueDate,
                                                          Date = nVj.Date,
                                                          MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(nVj.MultiCurrency)
                                                      } : null,
                                                      //VendorCredit = ObjectMapper.Map<VendorCreditSummaryOutput>(p.VendorCredit),
                                                      OpenBalance = p.OpenBalance,
                                                      Payment = p.Payment - p.LossGain,
                                                      Amount = p.TotalAmount + p.LossGain,
                                                      MultiCurrencyPayment = p.MultiCurrencyPayment,
                                                      MultiCurrencyAmount = p.MultiCurrencyTotalAmount,
                                                      MultiCurrencyOpenBalance = p.MultiCurrencyOpenBalance,
                                                      Cash = p.Cash,
                                                      MultiCurrencyCash = p.MultiCurrencyCash,
                                                      Credit = p.Credit,
                                                      MultiCurrencyCredit = p.MultiCurrencyCredit,
                                                      Expense = p.Expense,
                                                      MultiCurrencyExpense = p.MultiCurrencyExpense,
                                                      MultiCurrencyId = nBj != null && p.Invoice != null ? nBj.MultiCurrency.Id : nVj.MultiCurrency.Id,
                                                      MultiCurrencyCode = nBj != null && p.Invoice != null ? nBj.MultiCurrency.Code : nVj.MultiCurrency.Code,
                                                      LossGain = p.LossGain,
                                                      OpenBalanceInPaymentCurrency = p.OpenBalanceInPaymentCurrency,
                                                      PaymentInPaymentCurrency = p.PaymentInPaymentCurrency,
                                                      TotalAmountInPaymentCurrency = p.TotalAmountInPaymentCurrency,
                                                      CashInPaymentCurrency = p.CashInPaymentCurrency,
                                                      CreditInPaymentCurrency = p.CreditInPaymentCurrency,
                                                      ExpenseInPaymentCurrency = p.ExpenseInPaymentCurrency
                                                  }).OrderBy(u => u.CreationTime).ToListAsync();


            var result = ObjectMapper.Map<ReceivePaymentDetailOutput>(journal.ReceivePayment);

            var receivePaymentExpenseItem = await _receivePaymentExpenseRepository.GetAll()
                    .Include(u => u.Account)
                    .Where(u => u.ReceivePaymentId == input.Id)
                    .AsNoTracking()
                    .Join(
                        _journalItemRepository.GetAll()
                        .AsNoTracking(),
                        u => u.Id,
                        s => s.Identifier,
                        (pbItem, jItem) =>
                        new ReceivePaymentExpenseItem()
                        {
                            CreationTime = pbItem.CreationTime,
                            AccountId = jItem.AccountId,
                            Account = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                            Amount = pbItem.Amount,
                            Description = pbItem.Description,
                            Id = pbItem.Id,
                            MultiCurrencyAmount = pbItem.MultiCurrencyAmount,
                            IsLossGain = pbItem.IsLossGain
                        }
                    )
                    .OrderBy(u => u.CreationTime).ToListAsync();

            result.ReceivePaymentExpenseItems = receivePaymentExpenseItem.Where(s => !s.IsLossGain).ToList();

            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.ReceivePaymentId == input.Id)
                                            .Select(s => new GetExchangeRateDto
                                            {
                                                Id = s.Id,
                                                FromCurrencyCode = s.FromCurrency.Code,
                                                ToCurrencyCode = s.ToCurrency.Code,
                                                FromCurrencyId = s.FromCurrencyId,
                                                ToCurrencyId = s.ToCurrencyId,
                                                Bid = s.Bid,
                                                Ask = s.Ask,
                                                IsInves = s.FromCurrencyId == journal.CurrencyId
                                            })
                                            .FirstOrDefaultAsync();


                var exchangeDic = await _receivePaymentItemExchangeRateRepository.GetAll().AsNoTracking()
                    .Where(s => s.ReceivePaymentItem.ReceivePaymentId == input.Id)
                    .Select(s => new GetExchangeRateDto
                    {
                        Id = s.ReceivePaymentItemId,
                        FromCurrencyCode = s.FromCurrency.Code,
                        ToCurrencyCode = s.ToCurrency.Code,
                        FromCurrencyId = s.FromCurrencyId,
                        ToCurrencyId = s.ToCurrencyId,
                        Bid = s.Bid,
                        Ask = s.Ask
                    })
                    .ToDictionaryAsync(k => k.Id);


                result.TotalLossGainInvoice = 0;
                result.TotalLossGainCustomerCredit = 0;

                foreach (var item in receivePaymentDetailItem)
                {
                    if (exchangeDic.ContainsKey(item.Id))
                    {
                        item.ExchangeRate = exchangeDic[item.Id];
                        item.ExchangeRate.IsInves = exchangeDic[item.Id].FromCurrencyId == journal.MultiCurrencyId;
                    }

                    if (item.Invoice != null)
                    {
                        result.TotalLossGainInvoice += item.LossGain;
                    }
                    else
                    {
                        result.TotalLossGainCustomerCredit += item.LossGain;
                    }

                }

                result.ExchangeLossGain = receivePaymentExpenseItem.Where(s => s.IsLossGain).Select(s => new ExchangeLossGainDto { AccountId = s.AccountId, Amount = s.Amount }).FirstOrDefault();
            }


            result.ReceivePaymentDetailItems = receivePaymentDetailItem;
            result.ReceivePaymentNo = journal.JournalNo;
            result.ReceivePaymentDate = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.MultiCurrencyId = journal.MultiCurrencyId.Value;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.LocationId = journal.LocationId;
            result.LocationName = journal.Location?.LocationName;
            result.StatusName = journal.Status.ToString();
            result.TotalCashInvoice = journal.ReceivePayment.TotalCashInvoice;
            result.TotalCreditInvoice = journal.ReceivePayment.TotalCreditInvoice;
            result.TotalExpenseInvoice = journal.ReceivePayment.TotalExpenseInvoice;
            result.TotalCashCustomerCredit = journal.ReceivePayment.TotalCashCustomerCredit;
            result.TotalCreditCustomerCredit = journal.ReceivePayment.TotalCreditCustomerCredit;
            result.TotalExpenseCustomerCredit = journal.ReceivePayment.TotalExpenseCustomerCredit;
            result.PaymentMethodId = journal.ReceivePayment.PaymentMethodId;
            result.PaymentMethodName = journal.ReceivePayment.PaymentMethod == null ? "" : journal.ReceivePayment.PaymentMethod.PaymentMethodBase.Name;

            if (journal.ReceivePayment.ReceiveFrom == ReceiveFromRecievePayment.Cash)
            {
                var clearanceAccount = await (_journalItemRepository.GetAll()
                                           .Include(u => u.Account)
                                           .AsNoTracking()
                                           .Where(
                                                u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.Payment && u.Identifier == null
                                            )).FirstOrDefaultAsync();
                result.ReceivePaymentAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount.Account);
                result.ReceivePaymentAccountId = clearanceAccount.AccountId;
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_Update,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_Receivepayment_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateReceivePaymentInput input)
        {

            #region new 
            if (input.ReceivePaymentDetail.Count == 0)
            {
                throw new UserFriendlyException(L("BillInfoHaveAtLeastOneRecord"));
            }

            await ValidateExchangeRate(input);

            if (input.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit)
            {
                input.TotalPayment = 0;
                input.MultiCurrencyTotalPayment = 0;
                input.Change = 0;
                input.MultiCurrencyChange = 0;
            }

            if (input.Change == 0)
            {
                input.ReceivePaymentExpenseItems = new List<ReceivePaymentExpenseItem>();
            }
            else
            {
                var totalAmountCurrency = input.ReceivePaymentExpenseItems == null ? 0 : input.ReceivePaymentExpenseItems.Sum(t => t.Amount);
                var totalAmountMultiCurrency = input.ReceivePaymentExpenseItems == null ? 0 : input.ReceivePaymentExpenseItems.Sum(t => t.MultiCurrencyAmount);
                if (input.Change != totalAmountCurrency || input.MultiCurrencyChange != totalAmountMultiCurrency)
                {
                    throw new UserFriendlyException(L("YouMustAddAccountOfChange"));
                }
            }

            if (input.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit
                && input.TotalPaymentInvoice != input.TotalPaymentCustomerCredit
                && input.MultiCurrencyTotalPaymentInvoice != input.MultiCurrencyTotalPaymentCustomerCredit)
            {
                throw new UserFriendlyException(L("TotalBillPaymentMustEqualCustomerCreditPayment"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Include(u => u.ReceivePayment)
                              .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                              .FirstOrDefaultAsync();
            if (@journal == null || @journal.ReceivePayment == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                    .Where(t => t.IsLock == true &&
                                    (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.paymentDate.Date)
                                    && (t.LockKey == TransactionLockType.ReceivePayment || t.LockKey == TransactionLockType.BankTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            await CheckClosePeriod(journal.Date, input.paymentDate);
            journal.Update(tenantId, input.PaymentNo, input.paymentDate, input.Memo, input.TotalPayment, input.TotalPayment, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);


            var isMultiCurrency = await _multiCurrencyRepository.GetAll().ToListAsync();
            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
            {
                input.MultiCurrencyId = input.CurrencyId;
            }
            else if (input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyChange = input.Change;
                input.MultiCurrencyTotalPayment = input.TotalPayment;
            }

            journal.UpdateMultiCurrency(input.MultiCurrencyId);

            if (input.ReceiveFrom == ReceiveFromRecievePayment.Cash)
            {
                if (input.PaymentAccountId == null || input.PaymentAccountId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("PaymentAccountIsRequired"));
                }

                var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                          .Where(u => u.JournalId == journal.Id &&
                                                   u.Key == PostingKey.Payment && u.Identifier == null))
                                            .FirstOrDefaultAsync();
                if (@clearanceAccountItem == null)
                {
                    var journalItem = JournalItem.CreateJournalItem(tenantId, userId, journal.Id,
                                            input.PaymentAccountId.Value, input.Memo, input.TotalPayment, 0,
                                            PostingKey.Payment, null);
                    if (input.TotalPayment < 0)
                    {
                        journalItem.SetDebitCredit(0, input.TotalPayment * -1);
                    }
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));
                }
                else
                {
                    clearanceAccountItem.UpdateJournalItem(tenantId, input.PaymentAccountId.Value,
                                            input.Memo, input.TotalPayment, 0);
                    if (input.TotalPayment < 0)
                    {
                        clearanceAccountItem.SetDebitCredit(0, input.TotalPayment * -1);
                    }
                    CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
                }
            }
            else
            {
                //update Clearance account 
                var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                          .Where(u => u.JournalId == journal.Id &&
                                                   u.Key == PostingKey.Payment && u.Identifier == null))
                                            .FirstOrDefaultAsync();
                if (@clearanceAccountItem != null)
                {
                    CheckErrors(await _journalItemManager.RemoveAsync(@clearanceAccountItem));
                }
            }

            //update receivePayment 
            var receivePayment = @journal.ReceivePayment;
            receivePayment.Update(tenantId, input.FiFo, input.TotalOpenBalance, input.TotalPayment,
                                input.TotalPaymentDue, input.ReceiveFrom,
                                input.MultiCurrencyTotalPayment, input.Change,
                                input.TotalOpenBalanceCustomerCredit, input.TotalPaymentCustomerCredit,
                                input.TotalPaymentDueCustomerCredit, input.MultiCurrencyTotalPaymentCustomerCredit,
                                input.TotalPaymentInvoice, input.MultiCurrencyTotalPaymentInvoice, input.MultiCurrencyChange,
                                input.TotalCashInvoice,
                                input.TotalCreditInvoice,
                                input.TotalExpenseInvoice,
                                input.TotalCashCustomerCredit,
                                input.TotalCreditCustomerCredit,
                                input.TotalExpenseCustomerCredit,
                                input.MultiCurrencyTotalOpenBalance,
                                input.MultiCurrencyTotalOpenBalanceCustomerCredit,
                                input.MultiCurrencyTotalPaymentDue,
                                input.MultiCurrencyTotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalCashInvoice,
                                input.MultiCurrencyTotalCreditInvoice,
                                input.MultiCurrencyTotalExpenseInvoice,
                                input.MultiCurrencyTotalCashCustomerCredit,
                                input.MultiCurrencyTotalCreditCustomerCredit,
                                input.MultiCurrencyTotalExpenseCustomerCredit);

            if (input.PaymentMethodId.HasValue) receivePayment.UpdatePaymentMethodId(input.PaymentMethodId);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));


            CheckErrors(await _receivePaymentManager.UpdateAsync(receivePayment));

            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.ReceivePaymentId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = ReceivePaymentExchangeRate.Create(tenantId, userId, receivePayment.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, receivePayment.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if (exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }

            //Update receivePayment detail and Journal Item detail
            var receivePaymentDetail = await _receivePaymentDetailRepository.GetAll().Include(t => t.Invoice)
                                .Include(t => t.CustomerCredit).Where(u => u.ReceivePaymentId == input.Id).ToListAsync();

            var @journalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           (u.Key == PostingKey.AR || u.Key == PostingKey.Clearance)
                                           && u.Identifier != null))
                                  .ToListAsync();

            var toDeleteReceivePaymentItem = receivePaymentDetail.Where(u => !input.ReceivePaymentDetail.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = @journalItems.Where(u => !input.ReceivePaymentDetail.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            var receivePaymentExpense = await _receivePaymentExpenseRepository.GetAll().Where(u => u.ReceivePaymentId == input.Id).ToListAsync();
            var @journalItemsExpense = await (_journalItemRepository.GetAll()
                                        .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.PaymentChange && u.Identifier != null))
                                        .ToListAsync();

            var exchangeLossGainItem = receivePaymentExpense.FirstOrDefault(s => s.IsLossGain);
            JournalItem exchangeLossGainJournalItem = null;
            if (exchangeLossGainItem != null) exchangeLossGainJournalItem = journalItemsExpense.FirstOrDefault(s => s.Identifier == exchangeLossGainItem.Id);


            var toDeleteReceivePaymentExpenseItem = receivePaymentExpense
                                                    .WhereIf(exchangeLossGainItem != null, s => s.Id != exchangeLossGainItem.Id)
                                                    .Where(u => !input.ReceivePaymentExpenseItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalExpenseItem = @journalItemsExpense
                                             .WhereIf(exchangeLossGainJournalItem != null, s => s.Id != exchangeLossGainJournalItem.Id)
                                             .Where(u => !input.ReceivePaymentExpenseItems.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            // get receive payment and customer credit for update of add new item in receive items list
            var @invoices = new List<Invoice>();
            var customerCredits = new List<CustomerCredit>();
            if (input.ReceivePaymentDetail.Where(x => x.InvoiceId != null && x.Id == null).Count() > 0)
            {
                invoices = await _invoiceRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.Id == null && t.InvoiceId == x.Id))
                                .ToListAsync();
            }
            if (input.ReceivePaymentDetail.Where(x => x.CustomerCreditId != null && x.Id == null).Count() > 0)
            {
                customerCredits = await _customerCreditRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.Id == null && t.CustomerCreditId == x.Id)).ToListAsync();
            }

            var receivePaymentItemExchanges = await _receivePaymentItemExchangeRateRepository.GetAll().AsNoTracking().Where(s => s.ReceivePaymentItem.ReceivePaymentId == input.Id).ToListAsync();
            var updateExchangeHash = new HashSet<Guid>();

            //Update and Journal Item
            foreach (var c in input.ReceivePaymentDetail)
            {

                if (input.UseExchangeRate)
                {
                    c.Payment += c.LossGain;
                    c.TotalAmount -= c.LossGain;
                }

                if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                {
                    c.MultiCurrencyAmountToBeSubstract = c.AmountToBeSubstract;
                    c.MultiCurrencyOpenBalance = c.OpenBalance;
                    c.MultiCurrencyPayment = c.Payment;
                    c.MultiCurrencyTotalAmount = c.TotalAmount;
                }

                if (c.Id != null)
                {
                    var receivePaymentItem = receivePaymentDetail.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @journalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    // var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (receivePaymentItem != null)
                    {
                        var amountToBeSubstract = c.Payment - receivePaymentItem.Payment;
                        var amountToBeSubstractMulti = c.MultiCurrencyPayment - receivePaymentItem.MultiCurrencyPayment;
                        //set update before check update status paid for bill below for condition if publish
                        receivePaymentItem.Update(tenantId, c.OpenBalance, c.Payment, c.TotalAmount,
                                            c.MultiCurrencyOpenBalance, c.MultiCurrencyPayment,
                                            c.MultiCurrencyTotalAmount, c.InvoiceId, c.CustomerCreditId, 
                                            c.Cash, c.MultiCurrencyCash, c.Credit, c.MultiCurrencyCredit, c.Expense, c.MultiCurrencyExpense, 
                                            c.LossGain, c.OpenBalanceInPaymentCurrency, c.PaymentInPaymentCurrency, c.TotalAmountInPaymentCurrency,
                                            c.CashInPaymentCurrency, c.CreditInPaymentCurrency, c.ExpenseInPaymentCurrency);
                        CheckErrors(await _receivePaymentDetailManager.UpdateAsync(receivePaymentItem));

                        if (input.UseExchangeRate && input.MultiCurrencyId != c.MultiCurrencyId)
                        {
                            var itemExchange = receivePaymentItemExchanges.FirstOrDefault(r => r.ReceivePaymentItemId == c.Id);
                            if (itemExchange == null)
                            {
                                itemExchange = ReceivePaymentItemExchangeRate.Create(tenantId, userId, receivePaymentItem.Id, c.ExchangeRate.FromCurrencyId, c.ExchangeRate.ToCurrencyId, c.ExchangeRate.Bid, c.ExchangeRate.Ask);
                                await _receivePaymentItemExchangeRateRepository.InsertAsync(itemExchange);
                            }
                            else
                            {
                                itemExchange.Update(userId, receivePaymentItem.Id, c.ExchangeRate.FromCurrencyId, c.ExchangeRate.ToCurrencyId, c.ExchangeRate.Bid, c.ExchangeRate.Ask);
                                await _receivePaymentItemExchangeRateRepository.UpdateAsync(itemExchange);
                                updateExchangeHash.Add(itemExchange.Id);
                            }
                        }

                        if (c.InvoiceId != null)
                        {
                            if (receivePaymentItem.Invoice != null)
                            {
                                // update total balance in Invoice
                                var updateTotalbalance = receivePaymentItem.Invoice;

                                updateTotalbalance.UpdateOpenBalance(amountToBeSubstract * -1);
                                updateTotalbalance.UpdateMultiCurrencyOpenBalance(amountToBeSubstractMulti * -1);

                                //update status of Invoice
                                if (input.Status == TransactionStatus.Publish)
                                {
                                    if (c.Payment != 0 || c.MultiCurrencyPayment != 0)
                                    {
                                        updateTotalbalance.UpdateTotalPaid(amountToBeSubstract);
                                        updateTotalbalance.UpdateMultiCurrencyTotalPaid(amountToBeSubstractMulti);
                                        if ((!input.UseExchangeRate && updateTotalbalance.OpenBalance == 0) ||
                                            (input.UseExchangeRate && updateTotalbalance.MultiCurrencyOpenBalance == 0))
                                        {
                                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                                        }
                                        else
                                        {
                                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                                        }
                                    }
                                    CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));
                                }
                            }
                            if (journalItem != null)
                            {
                                //update journal item with debit and default credit zero
                                journalItem.UpdateJournalItem(userId, c.accountId, "", 0, c.Payment);

                                CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                            }
                        }
                        else if (c.CustomerCreditId != null)
                        {
                            // update total balance in customer credit
                            var updateTotalbalance = receivePaymentItem.CustomerCredit;

                            updateTotalbalance.IncreaseOpenbalance(amountToBeSubstract * -1);
                            updateTotalbalance.IncreaseMultiCurrencyOpenBalance(amountToBeSubstractMulti * -1);

                            if (input.Status == TransactionStatus.Publish)
                            {
                                if (c.Payment != 0 || c.MultiCurrencyPayment != 0)
                                {
                                    updateTotalbalance.IncreaseTotalPaid(amountToBeSubstract);
                                    updateTotalbalance.IncreaseMultiCurrencyTotalPaid(amountToBeSubstractMulti);
                                    if ((!input.UseExchangeRate && updateTotalbalance.OpenBalance == 0) ||
                                        (input.UseExchangeRate && updateTotalbalance.MultiCurrencyOpenBalance == 0))
                                    {
                                        updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                                    }
                                    else
                                    {
                                        updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                                    }
                                }
                                CheckErrors(await _customerCreditManager.UpdateAsync(updateTotalbalance));
                            }
                            if (journalItem != null)
                            {
                                journalItem.UpdateJournalItem(userId, c.accountId, "", c.Payment, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                            }
                        }
                    }
                }
                else if (c.Id == null) //create
                {
                    //insert to paybill detail item
                    var receivePaymentDetailItem = ReceivePaymentDetail.Create(tenantId, userId, receivePayment, c.InvoiceId, c.CustomerId,
                            c.DueDate, c.OpenBalance, c.Payment, c.TotalAmount, c.MultiCurrencyOpenBalance,
                            c.MultiCurrencyPayment, c.MultiCurrencyTotalAmount, c.CustomerCreditId, 
                            c.Cash, c.MultiCurrencyCash, c.Credit, c.MultiCurrencyCredit, c.Expense, c.MultiCurrencyExpense, 
                            c.LossGain, c.OpenBalanceInPaymentCurrency, c.PaymentInPaymentCurrency, c.TotalAmountInPaymentCurrency,
                            c.CashInPaymentCurrency, c.CreditInPaymentCurrency, c.ExpenseInPaymentCurrency);
                    CheckErrors(await _receivePaymentDetailManager.CreateAsync(receivePaymentDetailItem));

                    if (input.UseExchangeRate && input.MultiCurrencyId != c.MultiCurrencyId)
                    {
                        var itemExchange = ReceivePaymentItemExchangeRate.Create(tenantId, userId, receivePaymentDetailItem.Id, c.ExchangeRate.FromCurrencyId, c.ExchangeRate.ToCurrencyId, c.ExchangeRate.Bid, c.ExchangeRate.Ask);
                        await _receivePaymentItemExchangeRateRepository.InsertAsync(itemExchange);
                    }

                    if (c.InvoiceId != null)
                    {
                        // update total balance in invoice
                        var updateTotalbalance = invoices.Where(u => u.Id == c.InvoiceId).FirstOrDefault();
                        updateTotalbalance.UpdateOpenBalance(c.Payment * -1);
                        updateTotalbalance.UpdateMultiCurrencyOpenBalance(c.MultiCurrencyPayment * -1);
                        if (input.Status == TransactionStatus.Publish)
                        {
                            if (c.Payment != 0 || c.MultiCurrencyPayment != 0)
                            {
                                updateTotalbalance.UpdateTotalPaid(c.Payment);
                                updateTotalbalance.UpdateMultiCurrencyTotalPaid(c.MultiCurrencyPayment);
                                if ((!input.UseExchangeRate && updateTotalbalance.OpenBalance == 0) ||
                                      (input.UseExchangeRate && updateTotalbalance.MultiCurrencyOpenBalance == 0))
                                {
                                    updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                                }
                                else
                                {
                                    updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                                }
                            }
                            CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));
                        }

                        var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                                    c.accountId, "", 0, c.Payment, PostingKey.AR, receivePaymentDetailItem.Id);
                        CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                    }
                    else if (c.CustomerCreditId != null)
                    {
                        // update total balance in vendor credit
                        var updateTotalbalance = customerCredits.Where(u => u.Id == c.CustomerCreditId).FirstOrDefault();
                        updateTotalbalance.IncreaseOpenbalance(c.Payment * -1);
                        updateTotalbalance.IncreaseMultiCurrencyOpenBalance(c.MultiCurrencyPayment * -1);
                        if (input.Status == TransactionStatus.Publish)
                        {
                            if (c.Payment != 0 || c.MultiCurrencyPayment != 0)
                            {
                                updateTotalbalance.IncreaseTotalPaid(c.Payment);
                                updateTotalbalance.IncreaseMultiCurrencyTotalPaid(c.MultiCurrencyPayment);
                                if ((!input.UseExchangeRate && updateTotalbalance.OpenBalance == 0) ||
                                    (input.UseExchangeRate && updateTotalbalance.MultiCurrencyOpenBalance == 0))
                                {
                                    updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                                }
                                else
                                {
                                    updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                                }
                            }
                            CheckErrors(await _customerCreditManager.UpdateAsync(updateTotalbalance));
                        }

                        //insert journal item into debit with default credit zero
                        var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                                c.accountId, "", c.Payment, 0, PostingKey.Clearance,
                                receivePaymentDetailItem.Id);
                        CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                    }
                }
            }


            if ((input.ReceiveFrom == ReceiveFromRecievePayment.Cash || input.UseExchangeRate) && input.ReceivePaymentExpenseItems != null && input.ReceivePaymentExpenseItems.Count > 0)
            {
                foreach (var i in input.ReceivePaymentExpenseItems)
                {
                    if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                    {
                        i.MultiCurrencyAmount = i.Amount;
                    }
                    if (i.Id != null)
                    {
                        var receivePaymentItemExpense = receivePaymentExpense.FirstOrDefault(u => u.Id == i.Id);
                        var journalItemExpense = journalItemsExpense.FirstOrDefault(u => u.Identifier == i.Id);

                        if (receivePaymentItemExpense != null)
                        {
                            receivePaymentItemExpense.Update(userId, i.Amount, i.AccountId, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                            CheckErrors(await _receivePaymentExpenseManager.UpdateAsync(receivePaymentItemExpense));

                        }
                        if (journalItemExpense != null)
                        {
                            if (i.Amount > 0)
                            {
                                journalItemExpense.SetDebitCredit(i.Amount, 0);
                            }
                            else
                            {
                                journalItemExpense.SetDebitCredit(0, i.Amount * -1);
                            }
                            CheckErrors(await _journalItemManager.UpdateAsync(journalItemExpense));
                        }
                    }
                    else
                    {
                        var receivePaymentExpenseItem = ReceivePaymentExpense.Create(tenantId, userId, journal.ReceivePayment, i.AccountId, i.Amount, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                        CheckErrors(await _receivePaymentExpenseManager.CreateAsync(receivePaymentExpenseItem));


                        JournalItem journalItemExpense;
                        if (i.Amount > 0)
                        {
                            journalItemExpense = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, i.AccountId,
                                                        i.Description, i.Amount, 0, PostingKey.PaymentChange,
                                                        receivePaymentExpenseItem.Id);
                        }
                        else
                        {
                            journalItemExpense = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, i.AccountId,
                                                        i.Description, 0, i.Amount * -1, PostingKey.PaymentChange,
                                                        receivePaymentExpenseItem.Id);
                        }
                        CheckErrors(await _journalItemManager.CreateAsync(journalItemExpense));
                    }

                }
            }


            var deleteExchanges = receivePaymentItemExchanges.Where(s => !updateExchangeHash.Contains(s.Id)).ToList();
            if (deleteExchanges.Any()) await _receivePaymentItemExchangeRateRepository.BulkDeleteAsync(deleteExchanges);

            if (input.UseExchangeRate && input.ExchangeLossGain != null && input.ExchangeLossGain.Amount != 0)
            {
                if (exchangeLossGainItem != null)
                {
                    exchangeLossGainItem.Update(userId, input.ExchangeLossGain.Amount, input.ExchangeLossGain.AccountId, 0, exchangeLossGainItem.Description, true);
                    await _receivePaymentExpenseManager.UpdateAsync(exchangeLossGainItem);

                    if (exchangeLossGainJournalItem == null)
                    {
                        exchangeLossGainJournalItem = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, input.ExchangeLossGain.AccountId,
                                                        exchangeLossGainItem.Description, input.ExchangeLossGain.Amount, 0, PostingKey.PaymentChange,
                                                        exchangeLossGainItem.Id);

                        if (input.ExchangeLossGain.Amount < 0) exchangeLossGainJournalItem.SetDebitCredit(0, -input.ExchangeLossGain.Amount);

                        CheckErrors(await _journalItemManager.CreateAsync(exchangeLossGainJournalItem));
                    }
                    else
                    {
                        exchangeLossGainJournalItem.UpdateJournalItem(userId, input.ExchangeLossGain.AccountId, exchangeLossGainItem.Description, input.ExchangeLossGain.Amount, 0);

                        if (input.ExchangeLossGain.Amount < 0) exchangeLossGainJournalItem.SetDebitCredit(0, -input.ExchangeLossGain.Amount);

                        CheckErrors(await _journalItemManager.UpdateAsync(exchangeLossGainJournalItem));
                    }
                }
                else
                {
                    var addExchangeLossGain = ReceivePaymentExpense.Create(tenantId, userId, receivePayment, input.ExchangeLossGain.AccountId, input.ExchangeLossGain.Amount, 0, "Exchange Loss/Gain by System", true);
                    await _receivePaymentExpenseManager.CreateAsync(addExchangeLossGain);

                    var addExchangeLossGainJournalItem = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, input.ExchangeLossGain.AccountId,
                                                        addExchangeLossGain.Description, input.ExchangeLossGain.Amount, 0, PostingKey.PaymentChange,
                                                        addExchangeLossGain.Id);

                    if (input.ExchangeLossGain.Amount < 0) addExchangeLossGainJournalItem.SetDebitCredit(0, -input.ExchangeLossGain.Amount);

                    CheckErrors(await _journalItemManager.CreateAsync(addExchangeLossGainJournalItem));
                }
            }
            else if (exchangeLossGainItem != null || exchangeLossGainJournalItem != null)
            {
                await _journalItemManager.RemoveAsync(exchangeLossGainJournalItem);
                await _receivePaymentExpenseRepository.DeleteAsync(exchangeLossGainItem);
            }


            foreach (var t in toDeleteReceivePaymentItem)
            {
                if (t.Invoice != null)
                {
                    var invoice = t.Invoice;
                    invoice.UpdateOpenBalance(t.Payment);
                    invoice.UpdateTotalPaid(-1 * t.Payment);

                    invoice.UpdateMultiCurrencyOpenBalance(t.MultiCurrencyPayment);
                    invoice.UpdateMultiCurrencyTotalPaid(-1 * t.MultiCurrencyPayment);
                    if (invoice.TotalPaid == 0)
                    {
                        invoice.UpdatePaidStatus(PaidStatuse.Pending);
                    }
                    else
                    {
                        invoice.UpdatePaidStatus(PaidStatuse.Partial);
                    }
                    CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                }
                else if (t.CustomerCredit != null)
                {
                    var customerCredit = t.CustomerCredit;
                    customerCredit.IncreaseOpenbalance(t.Payment);
                    customerCredit.IncreaseTotalPaid(-1 * t.Payment);

                    customerCredit.IncreaseMultiCurrencyOpenBalance(t.MultiCurrencyPayment);
                    customerCredit.IncreaseMultiCurrencyTotalPaid(-1 * t.MultiCurrencyPayment);
                    if (customerCredit.TotalPaid == 0)
                    {
                        customerCredit.UpdatePaidStatus(PaidStatuse.Pending);
                    }
                    else
                    {
                        customerCredit.UpdatePaidStatus(PaidStatuse.Partial);
                    }
                    CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
                }
                CheckErrors(await _receivePaymentDetailManager.RemoveAsync(t));
            }


            foreach (var t in toDeleteReceivePaymentExpenseItem)
            {
                CheckErrors(await _receivePaymentExpenseManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalExpenseItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ReceivePayment, TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = receivePayment.Id };
            #endregion
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                 .GetAll()
                                 .Include(u => u.ReceivePayment)
                                 .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                                 .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get invoice item and update status and reveres total paid 
            var receivePaymentItems = await _receivePaymentDetailRepository.GetAll()
                .Where(u => u.ReceivePaymentId == entity.ReceivePaymentId).ToListAsync();
            //query get invoice credit and update status
            //var customerCredit = (from vd in _customerCreditRepository.GetAll()
            //                      join pb in _receivePaymentRepository.GetAll() on vd.Id equals pb.CustomerCreditId
            //                      where (pb.Id == input.Id)
            //                      select vd).FirstOrDefault();
            foreach (var bi in receivePaymentItems)
            {
                // update total balance in invoice               
                var updateTotalbalance = _invoiceRepository.GetAll().Where(u => u.Id == bi.InvoiceId).FirstOrDefault();

                updateTotalbalance.UpdateTotalPaid(bi.Payment * -1);
                updateTotalbalance.UpdateMultiCurrencyTotalPaid(bi.MultiCurrencyPayment * -1);
                if (updateTotalbalance.TotalPaid == 0)
                {
                    updateTotalbalance.UpdatePaidStatus(PaidStatuse.Pending);
                }
                else
                {
                    updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                }
                CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));

            }

            //update status on table customer credit
            //if (customerCredit != null && customerCredit.Id != null)
            //{
            //    var payment = receivePaymentItems.Sum(t => t.Payment);
            //    customerCredit.IncreaseTotalPaid(payment * -1);

            //    var multiPayment = receivePaymentItems.Sum(t => t.MultiCurrencyPayment);
            //    customerCredit.IncreaseMultiCurrencyTotalPaid(multiPayment * -1);
            //    if (customerCredit.TotalPaid == 0)
            //    {
            //        customerCredit.UpdatePaidStatus(PaidStatuse.Pending);
            //    }
            //    else
            //    {
            //        customerCredit.UpdatePaidStatus(PaidStatuse.Partial);
            //    }
            //    CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            //}

            entity.UpdateStatusToDraft();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                  .GetAll()
                                  .Include(u => u.ReceivePayment)
                                  .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                                  .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get invoice item and update status and reveres total paid 
            var receivePaymentItems = await _receivePaymentDetailRepository.GetAll()
                .Where(u => u.ReceivePaymentId == entity.ReceivePaymentId).ToListAsync();

            foreach (var bi in receivePaymentItems)
            {
                // update total balance in invoice               
                var updateTotalbalance = _invoiceRepository.GetAll().Where(u => u.Id == bi.InvoiceId).FirstOrDefault();

                updateTotalbalance.UpdateTotalPaid(bi.Payment);
                if (bi.Payment > 0)
                {
                    if (updateTotalbalance.OpenBalance == 0 && updateTotalbalance.Total == updateTotalbalance.TotalPaid)
                    {
                        updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                    }
                    else
                    {
                        updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                    }
                }
                CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));

            }

            //query get customer credit and update status
            //var customerCredit = (from vd in _customerCreditRepository.GetAll()
            //                      join pb in _receivePaymentRepository.GetAll() on vd.Id equals pb.CustomerCreditId
            //                      where (pb.Id == input.Id)
            //                      select vd).FirstOrDefault();
            //Update status customer Credit
            //if (customerCredit != null && customerCredit.Id != null)
            //{
            //    var payment = receivePaymentItems.Sum(t => t.Payment);
            //    customerCredit.IncreaseTotalPaid(payment);
            //    if (customerCredit.OpenBalance == 0 && customerCredit.Total == customerCredit.TotalPaid)
            //    {
            //        customerCredit.UpdatePaidStatus(PaidStatuse.Paid);
            //    }
            //    else
            //    {
            //        customerCredit.UpdatePaidStatus(PaidStatuse.Partial);
            //    }
            //    CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            //}

            entity.UpdatePublish();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @jounal = await _journalRepository.GetAll()
                .Include(u => u.ReceivePayment)
                .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get invoice item and update status and reveres total paid 
            var receivePaymentItems = await _receivePaymentDetailRepository.GetAll()
                .Where(u => u.ReceivePaymentId == @jounal.ReceivePaymentId).ToListAsync();

            //query get customer credit and update status
            //var customerCredit = (from ccr in _customerCreditRepository.GetAll()
            //                      join pb in _receivePaymentRepository.GetAll() on ccr.Id equals pb.CustomerCreditId
            //                      where (pb.Id == input.Id)
            //                      select ccr).FirstOrDefault();
            //if (customerCredit != null && customerCredit.Id != null)
            //{
            //    var payment = receivePaymentItems.Sum(t => t.Payment);
            //    customerCredit.IncreaseOpenbalance(payment);
            //    customerCredit.IncreaseTotalPaid(payment * -1);
            //    if (jounal.Status == TransactionStatus.Publish)
            //    {
            //        if (customerCredit.TotalPaid == 0)
            //        {
            //            customerCredit.UpdatePaidStatus(PaidStatuse.Pending);
            //        }
            //        else
            //        {
            //            customerCredit.UpdatePaidStatus(PaidStatuse.Partial);
            //        }
            //    }
            //    CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            //}
            foreach (var bi in receivePaymentItems)
            {
                // update total balance in invoice               
                var updateTotalbalance = _invoiceRepository.GetAll().Where(u => u.Id == bi.InvoiceId).FirstOrDefault();
                updateTotalbalance.UpdateOpenBalance(bi.Payment);
                // verify status if publish than update paid status
                if (jounal.Status == TransactionStatus.Publish)
                {
                    updateTotalbalance.UpdateTotalPaid(-1 * bi.Payment);
                    if (updateTotalbalance.TotalPaid == 0)
                    {
                        updateTotalbalance.UpdatePaidStatus(PaidStatuse.Pending);
                    }
                    else
                    {
                        updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                    }
                }
                CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));

            }


            jounal.UpdateVoid();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);
            CheckErrors(await _journalManager.UpdateAsync(jounal, autoSequence.DocumentType));
            return new NullableIdDto<Guid>() { Id = jounal.ReceivePaymentId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewInvoiceHistory)]
        public async Task<PagedResultDto<GetListReivePaymentHistoryOutput>> ViewInvoiceHistory(GetListHistoryInput input)
        {
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking().Include(t => t.Account).Where(t => t.Identifier == null)
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.ReceivePayment)

                         on jItem.JournalId equals j.Id
                         join pb in _receivePaymentRepository.GetAll().AsNoTracking()
                         on j.ReceivePaymentId equals pb.Id

                         join rpd in _receivePaymentDetailRepository.GetAll().Include(u => u.Invoice).Include(u => u.Customer).AsNoTracking()
                         on pb.Id equals rpd.ReceivePaymentId
                         where (rpd.InvoiceId == input.Id)
                         join c in _currencyRepository.GetAll().AsNoTracking() on j.CurrencyId equals c.Id

                         group jItem by new { journal = j, receivePayment = pb, currency = c, receivePaymentDetail = rpd } into u

                         select new GetListReivePaymentHistoryOutput
                         {
                             Id = u.Key.receivePayment.Id,
                             PaymentDate = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             TotalPayment = u.Key.receivePaymentDetail.Payment,
                             MultiCurrencyTotalPayment = u.Key.receivePaymentDetail.MultiCurrencyPayment,
                             Status = u.Key.journal.Status,
                             AccountName = (from acc in _chartOfAccountRepository.GetAll().AsNoTracking()
                                            .Where(t => u.Any(a => a.AccountId == t.Id))
                                            select acc.AccountName).FirstOrDefault(),
                             Type = u.Key.receivePayment.ReceiveFrom.ToString(),
                             Reference = u.Key.journal.Reference,
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListReivePaymentHistoryOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewCustomerCreditHistory)]
        public async Task<PagedResultDto<GetListReivePaymentHistoryOutput>> ViewCustomerCreditHistory(GetListHistoryInput input)
        {
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.ReceivePayment)

                         on jItem.JournalId equals j.Id
                         join pb in _receivePaymentRepository.GetAll().AsNoTracking()
                         on j.ReceivePaymentId equals pb.Id

                         join rpd in _receivePaymentDetailRepository.GetAll().Include(u => u.Invoice).Include(u => u.Customer).AsNoTracking()
                         on pb.Id equals rpd.ReceivePaymentId
                         where (rpd.CustomerCreditId == input.Id)
                         join c in _currencyRepository.GetAll().AsNoTracking() on j.CurrencyId equals c.Id

                         group jItem by new { journal = j, receivePayment = pb, currency = c, receivePaymentDetail = rpd } into u

                         select new GetListReivePaymentHistoryOutput
                         {
                             Id = u.Key.receivePayment.Id,
                             PaymentDate = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             TotalPayment = u.Key.receivePaymentDetail.Payment,
                             Status = u.Key.journal.Status,
                             AccountName = (from acc in _chartOfAccountRepository.GetAll().AsNoTracking()
                                           .Where(t => u.Any(a => a.AccountId == t.Id))
                                            select acc.AccountName).FirstOrDefault(),
                             Type = u.Key.receivePayment.ReceiveFrom.ToString(),
                             Reference = u.Key.journal.Reference,
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListReivePaymentHistoryOutput>(resultCount, @entities);
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetList)]
        public async Task<MultiCurrencyPagedResultDto<GetListCustomerBalanceOutput>> GetCustomerBalance(GetListCustomerBalanceInput input)
        {

            return input.CurrencyFilter == CurrencyFilter.MultiCurrencies ? await GetCustomerBalanceMultiCurrencyHelper(input) : await GetCustomerBalanceHelper(input);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_MultiDelete)]
        [UnitOfWork(IsDisabled = true)]
        public async Task MultiDelete(GetListDeleteInput input)
        {

            try
            {
                var tenantId = AbpSession.TenantId.Value;
                var journals = new List<Journal>();
                var customerCredit = new List<CustomerCredit>();
                var invoices = new List<Invoice>();
                var receivePaymentItems = new List<ReceivePaymentDetail>();
                var receivePaymentExpenseItems = new List<ReceivePaymentExpense>();
                var @jounalItems = new List<JournalItem>();
                var auto = new AutoSequence();
                var @entity = new List<ReceivePayment>();
                var exchanges = new List<ReceivePaymentExchangeRate>();
                var receivePaymentItemExchanges = new List<ReceivePaymentItemExchangeRate>();
                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        journals = await _journalRepository.GetAll()
                           .AsNoTracking()
                           .Include(u => u.ReceivePayment)
                           .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId != null && input.Ids.Contains(u.ReceivePaymentId.Value))
                           .ToListAsync();

                        var noReceipt = journals.OrderByDescending(t => t.JournalNo).Select(t => t.JournalNo).FirstOrDefault();
                        var journalIds = journals.Select(t => t.Id).ToList();

                        var isReceivePayment = journals.Select(t => t.ReceivePayment).Any();
                        if (!isReceivePayment)
                        {
                            throw new UserFriendlyException(L("RecordNotFound"));
                        }

                        var lockDate = journals.OrderByDescending(t => t.Date).Select(t => t.Date.Date).FirstOrDefault();
                        var locktransaction = await _lockRepository.GetAll()
                           .AsNoTracking()
                          .Where(t => (t.LockKey == TransactionLockType.ReceivePayment || t.LockKey == TransactionLockType.BankTransaction)
                          && t.IsLock == true && t.LockDate.Value.Date >= lockDate).AsNoTracking().CountAsync();

                        if (locktransaction > 0)
                        {
                            throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                        }

                        var @closePeroid = await _accountCycleRepository.GetAll().AsNoTracking().AnyAsync(t => t.EndDate != null && lockDate > t.EndDate.Value.Date);
                        if (closePeroid)
                        {
                            throw new UserFriendlyException(L("PeriodIsClose"));
                        }

                        auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);
                        if (noReceipt == auto.LastAutoSequenceNumber)
                        {
                            var jo = await _journalRepository.GetAll().AsNoTracking().Where(t => !journalIds.Contains(t.Id) && t.JournalType == JournalType.ReceivePayment)
                                            .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                            if (jo != null)
                            {
                                auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                            }
                            else
                            {
                                auto.UpdateLastAutoSequenceNumber(null);
                            }

                        }

                        receivePaymentExpenseItems = await _receivePaymentExpenseRepository.GetAll()
                                                        .AsNoTracking()
                                                        .Where(u => input.Ids.Contains(u.ReceivePaymentId)).ToListAsync();
                        @jounalItems = await _journalItemRepository.GetAll()
                                       .AsNoTracking().Where(u => journalIds.Contains(u.JournalId)).ToListAsync();
                        //query get invoice item and delete 
                        receivePaymentItems = await _receivePaymentDetailRepository.GetAll()
                                                .AsNoTracking()
                                                .Include(t => t.Invoice).Include(t => t.CustomerCredit)
                                                .Where(u => u.ReceivePaymentId != null && input.Ids.Contains(u.ReceivePaymentId)).ToListAsync();

                        exchanges = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => input.Ids.Contains(s.ReceivePaymentId)).ToListAsync();
                        receivePaymentItemExchanges = await _receivePaymentItemExchangeRateRepository.GetAll().AsNoTracking().Where(s => input.Ids.Contains(s.ReceivePaymentItem.ReceivePaymentId)).ToListAsync();
                    }
                }
                @entity = journals.Select(t => t.ReceivePayment).ToList();
                foreach (var j in journals)
                {
                    j.UpdateReceivePayment(null);
                }
                foreach (var r in receivePaymentItems)
                {
                    if (r.Invoice != null)
                    {
                        // update total balance in invoice
                        var find = invoices.FirstOrDefault(s => s.Id == r.InvoiceId);

                        var invoiceItem = find == null ? r.Invoice : find;
                        invoiceItem.UpdateOpenBalance(r.Payment);
                        invoiceItem.UpdateMultiCurrencyOpenBalance(r.MultiCurrencyPayment);
                        invoiceItem.UpdateTotalPaid(r.Payment * -1);
                        invoiceItem.UpdateMultiCurrencyTotalPaid(r.MultiCurrencyPayment * -1);
                        if (invoiceItem.TotalPaid == 0)
                        {
                            invoiceItem.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            invoiceItem.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        if (find == null) invoices.Add(invoiceItem);
                    }
                    else if (r.CustomerCredit != null)
                    {
                        var find = customerCredit.FirstOrDefault(s => s.Id == r.CustomerCreditId);
                        var customerCreditItem = find == null ? r.CustomerCredit : find;
                        customerCreditItem.IncreaseOpenbalance(r.Payment);
                        customerCreditItem.IncreaseMultiCurrencyOpenBalance(r.MultiCurrencyPayment);
                        customerCreditItem.IncreaseTotalPaid(r.Payment * -1);
                        customerCreditItem.IncreaseMultiCurrencyTotalPaid(r.MultiCurrencyPayment * -1);
                        if (customerCreditItem.TotalPaid == 0)
                        {
                            customerCreditItem.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            customerCreditItem.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        if (find == null) customerCredit.Add(customerCreditItem);
                    }
                }

                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    if (exchanges.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchanges);
                    if (receivePaymentItemExchanges.Any()) await _receivePaymentItemExchangeRateRepository.BulkDeleteAsync(receivePaymentItemExchanges);

                    await _autoSequenceRepository.BulkUpdateAsync(new List<AutoSequence> { auto });
                    await _journalRepository.BulkUpdateAsync(journals);
                    await _receivePaymentExpenseRepository.BulkDeleteAsync(receivePaymentExpenseItems);
                    await _journalItemRepository.BulkDeleteAsync(jounalItems);
                    await _invoiceRepository.BulkUpdateAsync(invoices);
                    await _customerCreditRepository.BulkUpdateAsync(customerCredit);
                    await _receivePaymentDetailRepository.BulkDeleteAsync(receivePaymentItems);
                    await _journalRepository.BulkDeleteAsync(journals);
                    await _receivePaymentRepository.BulkDeleteAsync(entity);
                    await uow.CompleteAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ReceivePayments_Export_Excel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelReceivePayment(GetLIstReceivePaymentInput input)
        {
            input.UsePagination = false;
            var tenantId = AbpSession.TenantId;
            List<GetListReceivPaymentOutput> receiveData = new List<GetListReceivPaymentOutput>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var results = (await GetList(input)).Items;
                    receiveData = results.Select(t => t.ReceivePaymentList).FirstOrDefault() != null ? results.Select(t => t.ReceivePaymentList).FirstOrDefault() : new List<GetListReceivPaymentOutput>();

                }
            }
            var result = new FileDto();
            var sheetName = "ReceivePayment";
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

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateReceivePayment();
                var reportHasShowFooterTotal = headerList.ColumnInfo.Where(t => t.AllowFunction != null).ToList();
                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                // write body
                foreach (var i in receiveData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        WriteBodyReceivePayment(ws, rowBody, collumnCellBody, h, i, count);
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in headerList.ColumnInfo)
                    {
                        if (i.AllowFunction != null)
                        {
                            int rowFooter = rowTableHeader + 1;// get start row after header 
                            var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                            var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                            if (i.ColumnType == ColumnType.Number)
                            {
                                AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                            }
                            else
                            {
                                AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"ReceivePayment.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        private ReportOutput GetReportTemplateReceivePayment()
        {

            var columns = new List<CollumnOutput>() {

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "PaymentDate",
                    ColumnLength = 180,
                    ColumnTitle = "Payment Date",
                    ColumnType = ColumnType.Date,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "PaymentNo",
                    ColumnLength = 230,
                    ColumnTitle = "Payment No",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Reference",
                    ColumnLength = 130,
                    ColumnTitle = "Reference",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Account",
                    ColumnLength = 230,
                    ColumnTitle = "Account",
                    ColumnType = ColumnType.String,
                    SortOrder = 4,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },

                  new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "PaymentType",
                    ColumnLength = 230,
                    ColumnTitle = "Payment Type",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Customer",
                    ColumnLength = 250,
                    ColumnTitle = "Customer",
                    ColumnType = ColumnType.String,
                    SortOrder = 6,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Location",
                    ColumnLength = 200,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 7,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "TotalPayment",
                    ColumnLength = 250,
                    ColumnTitle = "Total Payment",
                    ColumnType = ColumnType.Money,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = "Sum",
                    IsDisplay = true,
                    DisableDefault = false,
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
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "User",
                    ColumnLength = 250,
                    ColumnTitle = "User",
                    ColumnType = ColumnType.String,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Status",
                    ColumnLength = 250,
                    ColumnTitle = "Status",
                    ColumnType = ColumnType.String,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                }
            };
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.ToList(),
                Groupby = "",
                HeaderTitle = "ReceivePayment",
                Sortby = "",
            };

            return result;
        }



        private async Task<MultiCurrencyPagedResultDto<GetListCustomerBalanceOutput>> GetCustomerBalanceMultiCurrencyHelper(GetListCustomerBalanceInput input)
        {
            var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);
            var fromDateBeginning = previousCycle != null ? previousCycle.EndDate.Value.AddDays(1) : DateTime.MinValue;

            var customerGroupIds = await GetUserGroupCustomers();

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomerWithCodeType(customerGroupIds, input.Customers, input.CustomerTypes, customerTypeMemberIds);

            // get provious 
            var previousInvoiceQuery = from i in _customerOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.Invoice)
                                            .AsNoTracking()

                                       join j in _journalRepository.GetAll()
                                           .Where(s => s.Status == TransactionStatus.Publish)
                                           .Where(s => s.InvoiceId.HasValue)
                                           .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Invoice.CustomerId))
                                           .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                           .AsNoTracking()
                                       on i.TransactionId equals j.InvoiceId

                                       join ji in _journalItemRepository.GetAll()
                                            .Where(s => s.Identifier == null)
                                            .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                            .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                            .AsNoTracking()

                                       on j.Id equals ji.JournalId

                                       select new CustomerBalanceQuery
                                       {
                                           TransactionId = i.TransactionId,
                                           CustomerId = j.Invoice.CustomerId,
                                           CurrencyCode = j.MultiCurrency.Code,
                                           CurrencyId = j.MultiCurrencyId.Value,
                                           Total = i.MuliCurrencyBalance,
                                           Paid = 0,
                                           Balance = i.MuliCurrencyBalance,
                                           AccountId = ji.AccountId
                                       };

            var invoiceQuery = from u in _journalItemRepository.GetAll()
                                               .Where(t => t.Identifier == null)
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(t => t.Journal.InvoiceId.HasValue)
                                               .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                               .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                               .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Journal.Invoice.CustomerId))
                                               .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                               select new CustomerBalanceQuery
                               {
                                   TransactionId = u.Journal.InvoiceId.Value,
                                   CustomerId = u.Journal.Invoice.CustomerId,
                                   CurrencyCode = u.Journal.MultiCurrency.Code,
                                   CurrencyId = u.Journal.MultiCurrencyId.Value,
                                   Total = u.Journal.Invoice.MultiCurrencyTotal,
                                   Paid = u.Journal.Invoice.MultiCurrencyTotalPaid,
                                   Balance = u.Journal.Invoice.MultiCurrencyOpenBalance,
                                   AccountId = u.AccountId
                               };


            var invoicePayments = from rpi in _receivePaymentDetailRepository.GetAll()
                                    .Where(s => s.InvoiceId.HasValue)
                                    .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.CustomerId))
                                    .AsNoTracking()

                                  join j in _journalRepository.GetAll()
                                    .Where(s => s.Status == TransactionStatus.Publish)
                                    .Where(u => u.ReceivePaymentId.HasValue)
                                    .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                    .AsNoTracking()
                                  on rpi.ReceivePaymentId equals j.ReceivePaymentId

                                  join ij in _journalRepository.GetAll()
                                    .Where(s => s.Status == TransactionStatus.Publish)
                                    .Where(u => u.InvoiceId.HasValue)
                                    .AsNoTracking()
                                  on rpi.InvoiceId equals ij.InvoiceId

                                  select new
                                  {
                                      CustomerId = rpi.CustomerId,
                                      PaymentAmount = rpi.MultiCurrencyPayment,
                                      TransactionId = rpi.InvoiceId.Value,
                                      CurrencyId = ij.MultiCurrencyId.Value,
                                      Date = j.Date,
                                      PaymentId = rpi.ReceivePaymentId
                                  };

            var invoiceBalanceQuery = from i in invoiceQuery.Concat(previousInvoiceQuery)
                                      join p in invoicePayments
                                      on i.TransactionId equals p.TransactionId
                                      into ps
                                      let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                      select new CustomerBalanceQuery
                                      {
                                          TransactionId = i.TransactionId,
                                          CustomerId = i.CustomerId,
                                          CurrencyCode = i.CurrencyCode,
                                          CurrencyId = i.CurrencyId,
                                          Total = i.Total,
                                          Paid = paid,
                                          Balance = i.Total - paid,
                                          AccountId = i.AccountId
                                      };


            var previousCreditQuery = from i in _customerOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.CustomerCredit)
                                            .AsNoTracking()

                                      join j in _journalRepository.GetAll()
                                          .Where(s => s.Status == TransactionStatus.Publish)
                                          .Where(s => s.CustomerCreditId.HasValue)
                                          .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Invoice.CustomerId))
                                          .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                          .AsNoTracking()
                                      on i.TransactionId equals j.CustomerCreditId

                                      join ji in _journalItemRepository.GetAll()
                                           .Where(s => s.Identifier == null)
                                           .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                           .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                           .AsNoTracking()

                                      on j.Id equals ji.JournalId

                                      select new CustomerBalanceQuery
                                      {
                                          TransactionId = i.TransactionId,
                                          CustomerId = j.CustomerCredit.CustomerId,
                                          CurrencyId = j.MultiCurrencyId.Value,
                                          CurrencyCode = j.MultiCurrency.Code,
                                          Total = -i.MuliCurrencyBalance,
                                          Paid = 0,
                                          Balance = -i.MuliCurrencyBalance,
                                          AccountId = ji.AccountId
                                      };


            var customerCreditQuery = from u in _journalItemRepository.GetAll()
                                               .Where(t => t.Identifier == null)
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(t => t.Journal.CustomerCreditId.HasValue)
                                               .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                               .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                               .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Journal.CustomerCredit.CustomerId))
                                               .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                                      select new CustomerBalanceQuery
                                      {
                                          TransactionId = u.Journal.CustomerCreditId.Value,
                                          CustomerId = u.Journal.CustomerCredit.CustomerId,
                                          CurrencyCode = u.Journal.MultiCurrency.Code,
                                          CurrencyId = u.Journal.MultiCurrencyId.Value,
                                          Total = -u.Journal.CustomerCredit.MultiCurrencyTotal,
                                          Paid = -u.Journal.CustomerCredit.MultiCurrencyTotalPaid,
                                          Balance = -u.Journal.CustomerCredit.MultiCurrencyOpenBalance,
                                          AccountId = u.AccountId
                                      };



            var creditPayments = from rpi in _receivePaymentDetailRepository.GetAll()
                                    .Where(s => s.CustomerCreditId.HasValue)
                                    .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.CustomerId))
                                    .AsNoTracking()

                                 join j in _journalRepository.GetAll()
                                    .Where(s => s.Status == TransactionStatus.Publish)
                                    .Where(u => u.ReceivePaymentId.HasValue)
                                    .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                    .AsNoTracking()
                                 on rpi.ReceivePaymentId equals j.ReceivePaymentId

                                 select new
                                 {
                                     CustomerId = rpi.CustomerId,
                                     PaymentAmount = -rpi.MultiCurrencyPayment,
                                     TransactionId = rpi.CustomerCreditId.Value,
                                 };

            var creditBalanceQuery = from i in customerCreditQuery.Concat(previousCreditQuery)
                                     join p in creditPayments
                                     on i.TransactionId equals p.TransactionId
                                     into ps
                                     let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                     select new CustomerBalanceQuery
                                     {
                                         TransactionId = i.TransactionId,
                                         CustomerId = i.CustomerId,
                                         CurrencyId = i.CurrencyId,
                                         CurrencyCode = i.CurrencyCode,
                                         Total = i.Total,
                                         Paid = paid,
                                         Balance = i.Total - paid,
                                         AccountId = i.AccountId
                                     };


            var lastPapymentQuery = from p in invoicePayments
                                    orderby p.Date descending
                                    group p by p.CustomerId into g
                                    select new
                                    {
                                        CustomerId = g.Key,
                                        PaymentItems = g.GroupBy(i => i.PaymentId).FirstOrDefault().ToList()
                                    };

            var currencies = await (from b in invoiceBalanceQuery.Concat(creditBalanceQuery)
                                    join c in customerQuery
                                    on b.CustomerId equals c.Id
                                    group b by new
                                    {
                                        b.CurrencyId,
                                        b.CurrencyCode
                                    }
                                    into g
                                    select g.Key)
                                    .ToListAsync();

            var balanceQuery = from c in customerQuery

                               join b in invoiceBalanceQuery.Concat(creditBalanceQuery)
                               on c.Id equals b.CustomerId
                               into bs

                               join p in lastPapymentQuery
                               on c.Id equals p.CustomerId
                               into ps
                               from p in ps.DefaultIfEmpty()

                               where bs != null && bs.Any()

                               select new GetListCustomerBalanceOutput
                               {
                                   CustomerId = c.Id,
                                   CustomerCode = c.CustomerCode,
                                   CustomerName = c.CustomerName,
                                   CustomerTypeName = c.CustomerTypeName,
                                   Balance = bs.Sum(s => s.Balance),
                                   ToDate = input.ToDate,
                                   LastPaymentDate = p == null ? (DateTime?)null : p.PaymentItems.FirstOrDefault().Date,
                                   LastPayment = p == null ? 0 : p.PaymentItems.Sum(t => t.PaymentAmount),
                                   MultiCurrencCols = currencies.Select(c => new MultiCurrencyColumn
                                   {
                                       CurrencyId = c.CurrencyId,
                                       CurrencyCode = c.CurrencyCode,
                                       Balance = bs.Where(b => b.CurrencyId == c.CurrencyId).Sum(t => t.Balance),
                                       LastPayment = p == null ? 0 : p.PaymentItems.Where(t => t.CurrencyId == c.CurrencyId).Sum(t => t.PaymentAmount)
                                   }).ToList(),
                               };

            var allQuery = balanceQuery.Where(s => s.MultiCurrencCols.Any(b => b.Balance != 0));

            var totalCount = await allQuery.CountAsync();
            var items = await allQuery.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new MultiCurrencyPagedResultDto<GetListCustomerBalanceOutput> { Currencies = currencies.Select(s => s.CurrencyCode).ToList(), Items = items, TotalCount = totalCount };
        }

        private async Task<MultiCurrencyPagedResultDto<GetListCustomerBalanceOutput>> GetCustomerBalanceHelper(GetListCustomerBalanceInput input)
        {
            var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);
            var fromDateBeginning = previousCycle != null ? previousCycle.EndDate.Value.AddDays(1) : DateTime.MinValue;

            var customerGroupIds = await GetUserGroupCustomers();

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomerWithCodeType(customerGroupIds, input.Customers, input.CustomerTypes, customerTypeMemberIds);

            // get provious 
            var previousInvoiceQuery = from i in _customerOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.Invoice)
                                            .AsNoTracking()

                                       join j in _journalRepository.GetAll()
                                           .Where(s => s.Status == TransactionStatus.Publish)
                                           .Where(s => s.InvoiceId.HasValue)
                                           .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Invoice.CustomerId))
                                           .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                           .AsNoTracking()
                                       on i.TransactionId equals j.InvoiceId

                                       join ji in _journalItemRepository.GetAll()
                                            .Where(s => s.Identifier == null)
                                            .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                            .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                            .AsNoTracking()

                                       on j.Id equals ji.JournalId

                                       select new CustomerBalanceQuery
                                       {
                                           TransactionId = i.TransactionId,
                                           CustomerId = j.Invoice.CustomerId,
                                           Total = i.Balance,
                                           Paid = 0,
                                           Balance = i.Balance,
                                           AccountId = ji.AccountId
                                       };

            var invoiceQuery = from u in _journalItemRepository.GetAll()
                                               .Where(t => t.Identifier == null)
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(t => t.Journal.InvoiceId.HasValue)
                                               .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                               .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                               .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Journal.Invoice.CustomerId))
                                               .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                               select new CustomerBalanceQuery
                               {
                                   TransactionId = u.Journal.InvoiceId.Value,
                                   CustomerId = u.Journal.Invoice.CustomerId,
                                   Total = u.Journal.Invoice.Total,
                                   Paid = u.Journal.Invoice.TotalPaid,
                                   Balance = u.Journal.Invoice.OpenBalance,
                                   AccountId = u.AccountId
                               };


            var invoicePayments = from rpi in _receivePaymentDetailRepository.GetAll()
                                      .Where(s => s.InvoiceId.HasValue)
                                      .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.CustomerId))
                                      .AsNoTracking()

                                  join j in _journalRepository.GetAll()
                                      .Where(s => s.Status == TransactionStatus.Publish)
                                      .Where(u => u.ReceivePaymentId.HasValue)
                                      .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                      .AsNoTracking()
                                  on rpi.ReceivePaymentId equals j.ReceivePaymentId
                                  select new
                                  {
                                      CustomerId = rpi.CustomerId,
                                      PaymentAmount = rpi.Payment,
                                      TransactionId = rpi.InvoiceId.Value,
                                      Date = j.Date,
                                      PaymentId = rpi.ReceivePaymentId
                                  };

            var invoiceBalanceQuery = from i in invoiceQuery.Concat(previousInvoiceQuery)
                                      join p in invoicePayments
                                      on i.TransactionId equals p.TransactionId
                                      into ps
                                      let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                      select new CustomerBalanceQuery
                                      {
                                          TransactionId = i.TransactionId,
                                          CustomerId = i.CustomerId,
                                          Total = i.Total,
                                          Paid = paid,
                                          Balance = i.Total - paid,
                                          AccountId = i.AccountId
                                      };


            var previousCreditQuery = from i in _customerOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.CustomerCredit)
                                            .AsNoTracking()

                                      join j in _journalRepository.GetAll()
                                          .Where(s => s.Status == TransactionStatus.Publish)
                                          .Where(s => s.CustomerCreditId.HasValue)
                                          .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Invoice.CustomerId))
                                          .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                          .AsNoTracking()
                                      on i.TransactionId equals j.CustomerCreditId

                                      join ji in _journalItemRepository.GetAll()
                                           .Where(s => s.Identifier == null)
                                           .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                           .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                           .AsNoTracking()

                                      on j.Id equals ji.JournalId

                                      select new CustomerBalanceQuery
                                      {
                                          TransactionId = i.TransactionId,
                                          CustomerId = j.CustomerCredit.CustomerId,
                                          Total = -i.Balance,
                                          Paid = 0,
                                          Balance = -i.Balance,
                                          AccountId = ji.AccountId
                                      };


            var customerCreditQuery = from u in _journalItemRepository.GetAll()
                                               .Where(t => t.Identifier == null)
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(t => t.Journal.CustomerCreditId.HasValue)
                                               .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                               .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                               .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.Journal.CustomerCredit.CustomerId))
                                               .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                                      select new CustomerBalanceQuery
                                      {
                                          TransactionId = u.Journal.CustomerCreditId.Value,
                                          CustomerId = u.Journal.CustomerCredit.CustomerId,
                                          Total = -u.Journal.CustomerCredit.Total,
                                          Paid = -u.Journal.CustomerCredit.TotalPaid,
                                          Balance = -u.Journal.CustomerCredit.OpenBalance,
                                          AccountId = u.AccountId
                                      };



            var creditPayments = from rpi in _receivePaymentDetailRepository.GetAll()
                                      .Where(s => s.CustomerCreditId.HasValue)
                                      .WhereIf(input.Customers != null && input.Customers.Count() > 0, u => input.Customers.Contains(u.CustomerId))
                                      .AsNoTracking()

                                 join j in _journalRepository.GetAll()
                                     .Where(s => s.Status == TransactionStatus.Publish)
                                     .Where(u => u.ReceivePaymentId.HasValue)
                                     .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                     .AsNoTracking()
                                 on rpi.ReceivePaymentId equals j.ReceivePaymentId
                                 select new
                                 {
                                     CustomerId = rpi.CustomerId,
                                     PaymentAmount = -rpi.Payment,
                                     TransactionId = rpi.CustomerCreditId.Value
                                 };

            var creditBalanceQuery = from i in customerCreditQuery.Concat(previousCreditQuery)
                                     join p in creditPayments
                                     on i.TransactionId equals p.TransactionId
                                     into ps
                                     let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                     select new CustomerBalanceQuery
                                     {
                                         TransactionId = i.TransactionId,
                                         CustomerId = i.CustomerId,
                                         Total = i.Total,
                                         Paid = paid,
                                         Balance = i.Total - paid,
                                         AccountId = i.AccountId
                                     };

            var lastPapymentQuery = from p in invoicePayments
                                    orderby p.Date descending
                                    group p by p.CustomerId into g
                                    select new
                                    {
                                        CustomerId = g.Key,
                                        PaymentItems = g.GroupBy(i => i.PaymentId).FirstOrDefault().ToList()
                                    };

            var balanceQuery = from c in customerQuery

                               join b in invoiceBalanceQuery.Concat(creditBalanceQuery)
                               on c.Id equals b.CustomerId
                               into bs

                               join p in lastPapymentQuery
                               on c.Id equals p.CustomerId
                               into ps
                               from p in ps.DefaultIfEmpty()

                               where bs != null && bs.Any()

                               select new GetListCustomerBalanceOutput
                               {
                                   CustomerId = c.Id,
                                   CustomerCode = c.CustomerCode,
                                   CustomerName = c.CustomerName,
                                   CustomerTypeName = c.CustomerTypeName,
                                   Balance = bs == null ? 0 : bs.Sum(s => s.Balance),
                                   ToDate = input.ToDate,
                                   LastPaymentDate = p == null ? (DateTime?)null : p.PaymentItems.FirstOrDefault().Date,
                                   LastPayment = p == null ? 0 : p.PaymentItems.Sum(t => t.PaymentAmount),
                               };

            var allQuery = balanceQuery.Where(b => b.Balance != 0);

            var totalCount = await allQuery.CountAsync();
            var items = await allQuery.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new MultiCurrencyPagedResultDto<GetListCustomerBalanceOutput> { Items = items, TotalCount = totalCount };
        }




    }
}
