using System;

using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.Journals;
using CorarlERP.PayBills.Dto;
using CorarlERP.Vendors;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Journals.Dto;
using CorarlERP.Bills.Dto;
using CorarlERP.Vendors.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Bills;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.VendorCredit;
using CorarlERP.VendorCredit.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Invoices.Dto;
using CorarlERP.AutoSequences;
using CorarlERP.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.AccountCycles;
using CorarlERP.Common.Dto;
using CorarlERP.VendorCustomerOpenBalances;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using Abp.Domain.Uow;
using CorarlERP.Exchanges.Dto;
using IdentityServer4.Validation;
using CorarlERP.Items;

namespace CorarlERP.PayBills
{
    [AbpAuthorize]
    public class PayBillAppService : CorarlERPAppServiceBase, IPayBillAppService
    {


        private readonly ICorarlRepository<Bill, Guid> _billRepository;
        private readonly IBillManager _billManager;

        private readonly ICorarlRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly ICorarlRepository<PayBill, Guid> _payBillRepository;
        private readonly IPayBillManager _payBillManager;
        private readonly ICorarlRepository<PayBillDetail, Guid> _payBillDetailRepository;
        private readonly IPayBillDetailManager _payBillDetailManager;
        private readonly ICorarlRepository<PayBillExchangeRate, Guid> _exchangeRateRepository;
        private readonly ICorarlRepository<PayBillItemExchangeRate, Guid> _payBillItemExchangeRateRepository;

        private readonly ICorarlRepository<PayBillExpense, Guid> _payBillExpenseRepository;
        private readonly IPayBillExpenseManager _payBillExpenseManager;

        private readonly IJournalManager _journalManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;


        private readonly ICurrencyManager _currencyManager;
        private readonly ICorarlRepository<Currency, long> _currencyRepository;

        private readonly IVendorManager _vendorItemManager;
        private readonly ICorarlRepository<Vendor, Guid> _vendorRepository;

        private readonly IVendorCreditManager _vendorCreditManager;
        private readonly ICorarlRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly ICorarlRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly ICorarlRepository<Lock, long> _lockRepository;
        private readonly ICorarlRepository<MultiCurrencies.MultiCurrency, long> _multiCurrencyRepository;
        private readonly ICorarlRepository<VendorOpenBalance, Guid> _vendorOpenBalanceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<AccountCycle, long> _accountCycleRepository;

        public PayBillAppService(
            IJournalManager journalManager,
            ICorarlRepository<PayBillExchangeRate, Guid> exchangeRateRepository,
            ICorarlRepository<PayBillItemExchangeRate, Guid> payBillItemExchangeRateRepository,
            ICorarlRepository<Journal, Guid> journalRepository,
            ICorarlRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            ICorarlRepository<VendorOpenBalance, Guid> vendorOpenBalanceRepository,
            IJournalItemManager journalItemManager,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            IPayBillManager payBillManager,
            IBillManager billManager,
            IPayBillDetailManager payBillDetailManager,
            IVendorManager vendorManager,
            ICurrencyManager currencyManager,
            ICorarlRepository<Currency, long> currencyRepository,
            ICorarlRepository<Vendor, Guid> vendorRepository,
            IVendorCreditManager vendorCreditManager,
            ICorarlRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            ICorarlRepository<PayBill, Guid> payBillRepository,
            ICorarlRepository<PayBillDetail, Guid> payBillDetailRepository,
            IAutoSequenceManager autoSequenceManger,
            ICorarlRepository<AutoSequence, Guid> autoSequenceRepository,
            ICorarlRepository<Bill, Guid> billRepository,
            ICorarlRepository<MultiCurrencies.MultiCurrency, long> multiCurrencyRepository,
            ICorarlRepository<UserGroupMember, Guid> userGroupMemberRepository,
            ICorarlRepository<Locations.Location, long> locationRepository,
            ICorarlRepository<PayBillExpense, Guid> payBillEpxenseRepository,
            ICorarlRepository<AccountCycle, long> accountCycleRepository,
            ICorarlRepository<Lock, long> lockRepository,
            IPayBillExpenseManager payBillEpxenseManager,
            IUnitOfWorkManager unitOfWorkManager         
        ) : base(accountCycleRepository, userGroupMemberRepository, locationRepository)
        {
            _payBillExpenseRepository = payBillEpxenseRepository;
            _payBillExpenseManager = payBillEpxenseManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalManager.SetJournalType(JournalType.PayBill);
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _payBillDetailManager = payBillDetailManager;
            _payBillManager = payBillManager;
            _currencyManager = currencyManager;
            _vendorItemManager = vendorManager;

            _payBillRepository = payBillRepository;
            _payBillDetailRepository = payBillDetailRepository;
            _vendorRepository = vendorRepository;
            _currencyRepository = currencyRepository;
            _billRepository = billRepository;
            _billManager = billManager;
            _vendorCreditManager = vendorCreditManager;
            _vendorCreditRepository = vendorCreditRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _multiCurrencyRepository = multiCurrencyRepository;
            _lockRepository = lockRepository;
            _vendorOpenBalanceRepository = vendorOpenBalanceRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _accountCycleRepository = accountCycleRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _payBillItemExchangeRateRepository = payBillItemExchangeRateRepository;
        }

        private async Task ValidateExchangeRate(CreatePayBillInput input)
        {
            if (!input.UseExchangeRate) return;

            if (input.CurrencyId != input.MultiCurrencyId && input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));

            var find = input.PayBillDetail.Any(s => s.MultiCurrencyId != input.MultiCurrencyId && s.ExchangeRate == null);

            if(find) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));

            if(input.ExchangeLossGain != null && 
                input.ExchangeLossGain.Amount != 0 && 
                (input.ExchangeLossGain.AccountId == null || input.ExchangeLossGain.AccountId == Guid.Empty))
            {   
                var tenant = await GetCurrentTenantAsync();

                if (tenant.ExchangeLossAndGainId == null || tenant.ExchangeLossAndGainId == Guid.Empty) throw new UserFriendlyException(L("IsRequired",L("ExchangeLossGainAccount")));

                input.ExchangeLossGain.AccountId = tenant.ExchangeLossAndGainId.Value;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_Create,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreatePayBillInput input)
        {

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                      .Where(t => (t.LockKey == TransactionLockType.PayBill || t.LockKey == TransactionLockType.BankTransaction)
                                      && t.IsLock == true && t.LockDate.Value.Date >= input.paymentDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }


            //validate pay bill detail list
            if (input.PayBillDetail.Count == 0)
            {
                throw new UserFriendlyException(L("BillInfoHaveAtLeastOneRecord"));
            }

            await ValidateExchangeRate(input);


            if (input.Change == 0)
            {
                input.PayBillExpenseItem = new List<PayBillExpenseItem>();
            }
            else
            {
                var totalAmountCurrency = input.PayBillExpenseItem.Sum(t => t.Amount);
                var totalAmountMultiCurrency = input.PayBillExpenseItem.Sum(t => t.MultiCurrencyAmount);
                if (input.Change != totalAmountCurrency || input.MultiCurrencyChange != totalAmountMultiCurrency)
                {
                    throw new UserFriendlyException(L("YouMustAddAccountOfChange"));
                }
            }

            if (input.ReceiveFrom == ReceiveFromPayBill.VendorCredit)
            {
                input.TotalPayment = 0;
                input.MultiCurrencyTotalPayment = 0;
                input.Change = 0;
                input.MultiCurrencyChange = 0;
                if (input.TotalPaymentBill != input.TotalPaymentVendorCredit
                    && input.MultiCurrencyTotalPaymentBill != input.MultiCurrencyTotalPaymentVendorCredit)
                {
                    throw new UserFriendlyException(L("TotalBillPaymentMustEqualVendorCreditPayment"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.PaymentNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(
                                tenantId,
                                userId,
                                input.PaymentNo,
                                input.paymentDate,
                                input.Memo,
                                input.TotalPayment,
                                input.TotalPayment,
                                input.CurrencyId,
                                input.ClassId,
                                input.Reference,
                                input.LocationId);
            entity.UpdateStatus(input.Status);

            var IsMultiCurrency = await _multiCurrencyRepository.GetAll().ToListAsync();


            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !IsMultiCurrency.Any())
            {
                input.MultiCurrencyId = input.CurrencyId;
            }
            else if (input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyChange = input.Change;
                input.MultiCurrencyTotalPayment = input.TotalPayment;
            }


            entity.UpdateMultiCurrency(input.MultiCurrencyId);

            if (input.ReceiveFrom == ReceiveFromPayBill.Cash)
            {
                if (input.PaymentAccountId == null || input.PaymentAccountId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("RecordNotFound"));
                }
                //insert clearance journal item into credit
                var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                        input.PaymentAccountId.Value, input.Memo, 0,
                                        input.TotalPayment, PostingKey.Payment, null);
                if (input.TotalPayment < 0)
                {
                    clearanceJournalItem.SetDebitCredit(input.TotalPayment * -1, 0);
                }
                CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            }

            //insert to payBill
            var payBill = PayBill.Create(tenantId, userId, input.FiFo,
                                            input.TotalOpenBalance, input.TotalPayment, input.TotalPaymentDue,
                                            input.ReceiveFrom, input.MultiCurrencyTotalPayment, input.Change,
                                            input.TotalOpenBalanceVendorCredit, input.TotalPaymentVendorCredit,
                                            input.TotalPaymentDueVendorCredit, input.MultiCurrencyTotalPaymentVendorCredit,
                                            input.TotalPaymentBill, input.MultiCurrencyTotalPaymentBill, input.MultiCurrencyChange, 
                                            input.UseExchangeRate, 
                                            input.MultiCurrencyTotalOpenBalance, input.MultiCurrencyTotalOpenBalanceVendorCredit,
                                            input.MultiCurrencyTotalPaymentDue, input.MultiCurrencyTotalPaymentDueVendorCredit                                                                                     
                                         );

            if (input.PaymentMethodId.HasValue) payBill.SetPaymentMethod(input.PaymentMethodId);

            // update journal type status to paybill
            entity.UpdatePayBill(payBill);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _payBillManager.CreateAsync(payBill));

            if(input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = PayBillExchangeRate.Create(tenantId, userId, payBill.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }


            var @bills = new List<Bill>();
            var @vendorCredits = new List<VendorCredit.VendorCredit>();
            if (input.PayBillDetail.Where(x => x.BillId != null).Count() > 0)
            {
                @bills = await _billRepository.GetAll().Where(x => input.PayBillDetail.Any(t => t.BillId == x.Id)).ToListAsync();
            }
            if (input.PayBillDetail.Where(x => x.VendorCreditId != null).Count() > 0)
            {
                @vendorCredits = await _vendorCreditRepository.GetAll().Where(x => input.PayBillDetail.Any(t => t.VendorCreditId == x.Id)).ToListAsync();
            }
            // loop pay bill detail
            foreach (var i in input.PayBillDetail)
            {
                if (input.UseExchangeRate)
                {
                    i.Payment += i.LossGain;
                    i.TotalAmount -= i.LossGain;
                }

                if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !IsMultiCurrency.Any())
                {
                    i.MultiCurrencyAmountToBeSubstract = i.AmountToBeSubstract;
                    i.MultiCurrencyOpenBalance = i.OpenBalance;
                    i.MultiCurrencyPayment = i.Payment;
                    i.MultiCurrencyTotalAmount = i.TotalAmount;
                }


                //insert to pay bill detail
                var @payBillDetail = PayBillDetail.Create(tenantId, userId, payBill, i.BillId, i.VendorId,
                            i.DueDate, i.OpenBalance, i.Payment, i.TotalAmount, i.MultiCurrencyOpenBalance,
                            i.MultiCurrencyPayment, i.MultiCurrencyTotalAmount, i.VendorCreditId, i.LossGain,
                            i.OpenBalanceInPaymentCurrency, i.PaymentInPaymentCurrency, i.TotalAmountInPaymentCurrency
                            );
                CheckErrors(await _payBillDetailManager.CreateAsync(@payBillDetail));

                if (input.UseExchangeRate && i.MultiCurrencyId != input.MultiCurrencyId)
                {
                    var exchange = PayBillItemExchangeRate.Create(tenantId, userId, payBillDetail.Id, i.ExchangeRate.FromCurrencyId, i.ExchangeRate.ToCurrencyId, i.ExchangeRate.Bid, i.ExchangeRate.Ask);
                    await _payBillItemExchangeRateRepository.InsertAsync(exchange);
                }

                JournalItem journalItem;
                //insert journal item into with debit and default credit zero
                if (i.BillId != null)
                {

                    journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                                i.AccountId, "", i.Payment, 0, PostingKey.AP, @payBillDetail.Id);

                    if (i.Payment < 0)
                    {
                        journalItem.SetDebitCredit(0, -i.Payment);
                    }

                    // update total balance in bill 
                    var updateTotalbalance = @bills.Where(u => u.Id == payBillDetail.BillId).FirstOrDefault();
                    updateTotalbalance.UpdateOpenBalance(i.Payment * -1);
                    updateTotalbalance.UpdateMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);

                    if (input.Status == TransactionStatus.Publish)
                    {  
                        if(i.Payment != 0 || i.MultiCurrencyPayment != 0)
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
                    }

                    CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                }
                else
                {
                    journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                                i.AccountId, "", 0, i.Payment, PostingKey.Clearance, @payBillDetail.Id);

                    if (i.Payment < 0)
                    {
                        journalItem.SetDebitCredit(-i.Payment, 0);
                    }

                    // update total balance in bill 
                    var updateTotalbalance = @vendorCredits.Where(u => u.Id == payBillDetail.VendorCreditId).FirstOrDefault();
                    updateTotalbalance.IncreaseOpenbalance(i.Payment * -1);
                    updateTotalbalance.IncreaseMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);

                    if (input.Status == TransactionStatus.Publish)
                    {
                        if (i.Payment != 0 || i.MultiCurrencyPayment != 0)
                        {
                            updateTotalbalance.IncreaseTotalPaid(i.Payment);
                            updateTotalbalance.IncreaseMultiCurrencyTotalPaid(i.MultiCurrencyPayment);
                           
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
                        CheckErrors(await _vendorCreditManager.UpdateAsync(updateTotalbalance));
                    }
                }

                CheckErrors(await _journalItemManager.CreateAsync(journalItem));
            }

            // loop of paybill expense account detail
            if ((input.ReceiveFrom == ReceiveFromPayBill.Cash || input.UseExchangeRate) && input.PayBillExpenseItem != null)
            {
                foreach (var i in input.PayBillExpenseItem)
                {
                    if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !IsMultiCurrency.Any())
                    {
                        i.MultiCurrencyAmount = i.Amount;
                    }
                    //insert to pay bill detail
                    var @payBillExpense = PayBillExpense.Create(tenantId, userId, payBill, i.AccountId,
                            i.Amount, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                    CheckErrors(await _payBillExpenseManager.CreateAsync(@payBillExpense));

                    JournalItem journalItem;
                    if (i.Amount > 0)
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    i.Description, 0, i.Amount, PostingKey.PaymentChange,
                                                    @payBillExpense.Id);
                    }
                    else
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    i.Description, i.Amount * -1, 0, PostingKey.PaymentChange,
                                                    @payBillExpense.Id);
                    }
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));
                }
            }

            if(input.UseExchangeRate && input.ExchangeLossGain != null && input.ExchangeLossGain.Amount != 0)
            {
                var @payBillExpense = PayBillExpense.Create(tenantId, userId, payBill, input.ExchangeLossGain.AccountId,
                           input.ExchangeLossGain.Amount, 0, "Exchange Loss/Gain by System", true);
                CheckErrors(await _payBillExpenseManager.CreateAsync(@payBillExpense));

                JournalItem journalItem;
                if (input.ExchangeLossGain.Amount > 0)
                {
                    journalItem = JournalItem.CreateJournalItem(
                                                tenantId, userId, entity, input.ExchangeLossGain.AccountId,
                                                payBillExpense.Description, 0, input.ExchangeLossGain.Amount, PostingKey.PaymentChange,
                                                @payBillExpense.Id);
                }
                else
                {
                    journalItem = JournalItem.CreateJournalItem(
                                                tenantId, userId, entity, input.ExchangeLossGain.AccountId,
                                                payBillExpense.Description, -input.ExchangeLossGain.Amount, 0, PostingKey.PaymentChange,
                                                @payBillExpense.Id);
                }
                CheckErrors(await _journalItemManager.CreateAsync(journalItem));
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.PayBill,TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = payBill.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_GetList)]
        public async Task<PagedResultDto<PayBillHeader>> GetList(GetListPayBillInput input)
        {

            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var jQuery = _journalRepository.GetAll()
                        .Where(u => u.JournalType == JournalType.PayBill)
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
                            PayBillId = s.PayBillId,
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
                                   PayBillId = j.PayBillId,
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

            var payBillQuery = from p in _payBillRepository.GetAll()
                                          .AsNoTracking()
                                          .Select(s => new
                                          {
                                              Id = s.Id,
                                              ReceiveFrom = s.ReceiveFrom,
                                              TotalPayment = s.TotalPayment,
                                              TotalPaymentBill = s.TotalPaymentBill,
                                              TotalPaymentVendorCredit = s.TotalPaymentVendorCredit,
                                              TotalOpenBalance = s.TotalOpenBalance
                                          })
                               join j in journalQuery
                               on p.Id equals j.PayBillId
                               select new
                               {
                                   Id = p.Id,
                                   ReceiveFrom = p.ReceiveFrom,
                                   TotalPayment = p.TotalPayment,
                                   TotalPaymentBill = p.TotalPaymentBill,
                                   TotalPaymentVendorCredit = p.TotalPaymentVendorCredit,
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
                                   PayBillId = j.PayBillId,
                                   CreatorUserId = j.CreatorUserId,
                                   LocationId = j.LocationId,
                                   UserName = j.UserName,
                                   LocationName = j.LocationName
                               };

            var paymentQuery = from p in payBillQuery
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
                                   TotalPaymentBill = p.TotalPaymentBill,
                                   TotalVendorCreditPayment = p.TotalPaymentVendorCredit,
                                   TotalAmount = p.TotalOpenBalance,
                                   ReceiveFrom = p.ReceiveFrom,
                                   Location = new LocationSummaryOutput
                                   {
                                       Id = p.LocationId.Value,
                                       LocationName = p.LocationName,
                                   },
                                   AccountName = p.ReceiveFrom == ReceiveFromPayBill.Cash ?
                                   jItems.Where(t => t.Key == PostingKey.Payment && t.Identifier == null).Select(x => x.AccountName).FirstOrDefault() :
                                   jItems.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountName).FirstOrDefault(),
                               };


            var vendorQuery = GetVendors(null, input.Vendors, null, vendoTypeMemberIds);
            var payBillDetailQuery = from ri in _payBillDetailRepository.GetAll()
                                                .WhereIf(input.Vendors != null && input.Vendors.Count > 0,
                                                        u => input.Vendors.Contains(u.VendorId))
                                                .WhereIf(input.VendorTypes != null && input.VendorTypes.Count > 0,
                                                        u => u.Vendor != null && u.Vendor.VendorTypeId != null && input.VendorTypes.Contains(u.Vendor.VendorTypeId.Value))
                                                .AsNoTracking()
                                                .Select(s => new
                                                {
                                                    Id = s.Id,
                                                    PayBillId = s.PayBillId,
                                                    VendorId = s.VendorId
                                                })
                                     join c in vendorQuery
                                     on ri.VendorId equals c.Id
                                     select new
                                     {
                                         Id = ri.Id,
                                         PayBillId = ri.PayBillId,
                                         VendorId = ri.VendorId,
                                         VendorName = c.VendorName,
                                     };


            var query = from p in paymentQuery
                        join pi in payBillDetailQuery
                        on p.Id equals pi.PayBillId
                        into pItems
                        where pItems.Count() > 0
                        select new GetListPayBillOutput
                        {
                            VendorList = pItems.GroupBy(g => new { g.VendorId, g.VendorName })
                               .Select(s => new VendorSummaryOutput
                               {
                                   VendorName = s.Key.VendorName,
                                   Id = s.Key.VendorId
                               }).ToList(),
                            CreationTimeIndex = p.CreationTimeIndex,
                            User = p.User,
                            Currency = p.Currency,
                            CurrencyId = p.CurrencyId,
                            Memo = p.Memo,
                            PaymentDate = p.PaymentDate,
                            JournalNo = p.JournalNo,
                            Status = p.Status,
                            Id = p.Id,
                            TotalPayment = p.TotalPayment,
                            TotalPaymentBill = p.TotalPaymentBill,
                            TotalVendorCreditPayment = p.TotalVendorCreditPayment,
                            TotalAmount = p.TotalAmount,
                            ReceiveFrom = p.ReceiveFrom,
                            Location = p.Location,
                            AccountName = p.AccountName,
                            Reference = p.Reference,
                        };


            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<PayBillHeader>(resultCount, new List<PayBillHeader>());

            var summary = new List<BalanceSummaryPayBill>();

            // get total record of summary for first index
            summary.Add(
                new BalanceSummaryPayBill
                {
                    AccountName = null,
                    TotalPament = await query.SumAsync(t =>
                                t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill == 0 ? t.TotalPayment
                                : t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill > 0 ? t.TotalPayment + t.TotalVendorCreditPayment
                                : t.ReceiveFrom == ReceiveFromPayBill.VendorCredit ? t.TotalVendorCreditPayment : 0)
                }
            );

            var allAcc = await query.Select(t =>

                    new List<BalanceSummaryPayBill> {
                        new BalanceSummaryPayBill{
                            AccountName = t.ReceiveFrom != ReceiveFromPayBill.VendorCredit ? t.AccountName : "Credit",
                            TotalPament = t.ReceiveFrom != ReceiveFromPayBill.VendorCredit ? t.TotalPayment : t.TotalVendorCreditPayment,
                        },
                        new BalanceSummaryPayBill{
                            AccountName = t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill > 0 ? "Credit" : "",
                            TotalPament = t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill > 0 ? t.TotalVendorCreditPayment : 0
                        },
                    })
                    .SelectMany(s => s.Where(r => r.AccountName != ""))
                    .GroupBy(g => g.AccountName)
                    .Select(a => new BalanceSummaryPayBill
                    {
                        AccountName = a.Key,
                        TotalPament = a.Sum(t => t.TotalPament),
                    }).ToListAsync();

            summary = summary.Concat(allAcc).ToList();

            var headersQuery = new List<PayBillHeader> { };

            if (input.Sorting.EndsWith("DESC"))
            {
                IQueryable<GetListPayBillOutput> descQuery;

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

                IQueryable<GetListPayBillOutput> ascQuery;

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

            headersQuery = new List<PayBillHeader> {
                new PayBillHeader
                {
                    BalanceSummary = summary,
                    PayBillList = await query.PageBy(input).ToListAsync()
                }
            };

            var @entities = headersQuery;
            return new PagedResultDto<PayBillHeader>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_GetList)]
        public async Task<PagedResultDto<PayBillHeader>> GetListOld(GetListPayBillInput input)
        {

            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            #region new query 
            var @queryable = from j in _journalRepository.GetAll().Include(t => t.CreatorUser).Include(t => t.Location)
                                    .Where(u => u.PayBillId != null)
                                    .Where(u => u.JournalType == JournalType.PayBill)
                                    .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                    .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                        u => input.Locations.Contains(u.LocationId))
                                    .WhereIf(input.Users != null && input.Users.Count > 0,
                                                        u => input.Users.Contains(u.CreatorUserId))
                                    .WhereIf(input.FromDate != null && input.ToDate != null,
                                       (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                    .WhereIf(!input.Filter.IsNullOrEmpty(),
                                       u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                               u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                               u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                    .AsNoTracking()


                             join pbd in _payBillDetailRepository.GetAll()
                                            .Include(u => u.PayBill)
                                            //.Include(u => u.Bill)
                                            .Include(u => u.Vendor)
                                            .WhereIf(input.BillIds != null && input.BillIds.Count > 0, u => input.BillIds.Contains(u.BillId))
                                            .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                            .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                                            .AsNoTracking()

                             on j.PayBillId equals pbd.PayBillId
                             into payBillItems

                             join payBill in _payBillRepository.GetAll()

                            on j.PayBillId equals payBill.Id

                             join jItem in _journalItemRepository.GetAll().Include(t => t.Account)
                                            //.Where(t => (t.Key == PostingKey.Payment && t.Identifier == null) || t.Key == PostingKey.Clearance)
                                            .WhereIf(input.Accounts != null && input.Accounts.Count > 0,
                                                    u => input.Accounts.Contains(u.AccountId))
                                            .AsNoTracking()

                             on j.Id equals jItem.JournalId into journalItems
                             where payBillItems != null && payBillItems.Count() > 0 &&
                                   journalItems != null && journalItems.Count() > 0 && payBill.Id == j.PayBillId

                             select new
                             {

                                 accounts = journalItems,
                                 payBills = payBill,
                                 Journal = j,
                                 Vendors =
                                         payBillItems
                                         .GroupBy(u => u.Vendor)
                                            .Select(t => new VendorSummaryOutput
                                            {
                                                Id = t.Key.Id,
                                                AccountId = t.Key.AccountId.Value,
                                                VendorCode = t.Key.VendorCode,
                                                VendorName = t.Key.VendorName
                                            })
                             };



            var query = queryable.Select(u => new GetListPayBillOutput()
            {
                CreationTimeIndex = u.Journal.CreationTimeIndex,
                VendorList = u.Vendors.Select(s => new VendorSummaryOutput
                {
                    AccountId = s.AccountId,
                    Id = s.Id,
                    VendorCode = s.VendorCode,
                    VendorName = s.VendorName
                }).ToList(),
                Location = ObjectMapper.Map<LocationSummaryOutput>(u.Journal.Location),
                Currency = ObjectMapper.Map<CurrencyDetailOutput>(u.Journal.Currency),
                User = ObjectMapper.Map<UserDto>(u.Journal.CreatorUser),
                CurrencyId = u.Journal.CurrencyId,
                Memo = u.Journal.Memo,
                Id = u.Journal.PayBillId.Value,
                PaymentDate = u.Journal.Date,
                JournalNo = u.Journal.JournalNo,
                TotalPayment = u.payBills.TotalPayment, //u.accounts.Sum(t=>t.Debit), 
                TotalVendorCreditPayment = u.payBills.TotalPaymentVendorCredit,
                TotalPaymentBill = u.payBills.TotalPaymentBill,
                Status = u.Journal.Status,
                TotalAmount = u.payBills.TotalOpenBalance,
                ReceiveFrom = u.payBills.ReceiveFrom,
                AccountName = u.payBills.ReceiveFrom == ReceiveFromPayBill.Cash ?
                                u.accounts.Where(t => t.Key == PostingKey.Payment && t.Identifier == null)
                                    .Select(x => x.Account.AccountName).FirstOrDefault() :
                                u.accounts.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.Account.AccountName).FirstOrDefault(),
                AccountId = u.payBills.ReceiveFrom == ReceiveFromPayBill.VendorCredit ? u.accounts
                                .Where(t => t.Key == PostingKey.Payment && t.Identifier == null)
                                    .Select(x => x.AccountId).FirstOrDefault() :
                                u.accounts.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountId).FirstOrDefault(),
            });
            #endregion

            var resultCount = await query.CountAsync();
            var summary = new List<BalanceSummaryPayBill>();

            if (resultCount > 0)
            {
                // get total record of summary for first index
                summary.Add(
                    new BalanceSummaryPayBill
                    {
                        AccountName = null,
                        TotalPament = await query.SumAsync(t =>
                                    t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill == 0 ? t.TotalPayment
                                    : t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill > 0 ? t.TotalPayment + t.TotalVendorCreditPayment
                                    : t.ReceiveFrom == ReceiveFromPayBill.VendorCredit ? t.TotalVendorCreditPayment : 0)
                    }
                );

                var allAcc = await query.Select(t =>

                        new List<BalanceSummaryPayBill> {
                            new BalanceSummaryPayBill{
                                AccountName = t.ReceiveFrom != ReceiveFromPayBill.VendorCredit ? t.AccountName : "Credit",
                                TotalPament = t.ReceiveFrom != ReceiveFromPayBill.VendorCredit ? t.TotalPayment : t.TotalVendorCreditPayment,
                            },
                            new BalanceSummaryPayBill{
                                AccountName = t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill > 0 ? "Credit" : "",
                                TotalPament = t.ReceiveFrom == ReceiveFromPayBill.Cash && t.TotalPaymentBill > 0 ? t.TotalVendorCreditPayment : 0
                            },
                        })
                        .SelectMany(s => s.Where(r => r.AccountName != ""))
                        .GroupBy(g => g.AccountName)
                        .Select(a => new BalanceSummaryPayBill
                        {
                            AccountName = a.Key,
                            TotalPament = a.Sum(t => t.TotalPament),
                        }).ToListAsync();

                summary = summary.Concat(allAcc).ToList();
            }

            var headersQuery = new List<PayBillHeader> { };

            if (input.Sorting.Contains("date") && !input.Sorting.Contains(".")) input.Sorting = input.Sorting.Replace("date", "Date.Date");

            if (input.Sorting.Contains("DESC"))
            {
                headersQuery = new List<PayBillHeader> {
                    new PayBillHeader
                    {
                        BalanceSummary = summary,
                        PayBillList = await query.OrderBy(input.Sorting).ThenByDescending(t=>t.CreationTimeIndex).PageBy(input).ToListAsync()
                   }
                };
            }
            else
            {
                headersQuery = new List<PayBillHeader>
                {
                    new PayBillHeader
                    {
                        BalanceSummary = summary,
                        PayBillList = await query.OrderBy(input.Sorting).ThenBy(t=>t.CreationTimeIndex).PageBy(input).ToListAsync()

                   }
                };
            }
            var @entities = headersQuery;
            return new PagedResultDto<PayBillHeader>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_Find)]
        public async Task<PagedResultDto<GetListPayBillOutput>> Find(GetListPayBillInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.PayBill)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                          .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                        u => input.Locations.Contains(u.LocationId))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(
                             input.FromDate != null && input.ToDate != null,
                            (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date))
                         )
                         .WhereIf(
                            !input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower())
                         )
                         on jItem.JournalId equals j.Id

                         join pb in _payBillRepository.GetAll().AsNoTracking()

                         on j.PayBillId equals pb.Id

                         join pbd in _payBillDetailRepository.GetAll().Include(u => u.Bill).Include(u => u.Vendor).AsNoTracking()
                         .WhereIf(input.BillIds != null && input.BillIds.Count > 0, u => input.BillIds.Contains(u.BillId))
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                         on pb.Id equals pbd.PayBillId

                         join c in _currencyRepository.GetAll().AsNoTracking() on j.CurrencyId equals c.Id

                         group j by new { journal = j, payBill = pb, currency = c } into u

                         select new GetListPayBillOutput
                         {
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(u.Key.currency),
                             CurrencyId = u.Key.currency.Id,
                             Memo = u.Key.journal.Memo,
                             Id = u.Key.payBill.Id,
                             PaymentDate = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             TotalPayment = u.Key.payBill.TotalPayment,
                             Status = u.Key.journal.Status,
                             TotalAmount = u.Key.payBill.TotalOpenBalance,
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();


            return new PagedResultDto<GetListPayBillOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.PayBill)
                                .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get bill item and update status and reveres total paid 
            var payBillItems = await _payBillDetailRepository.GetAll()
                .Where(u => u.PayBillId == entity.PayBillId).ToListAsync();
            //query get vendor credit and update status
            //var vendorCredit = (from vd in _vendorCreditRepository.GetAll()
            //                    join pb in _payBillRepository.GetAll() on vd.Id equals pb.VendorCreditId
            //                    where (pb.Id == input.Id)
            //                    select vd).FirstOrDefault();
            foreach (var bi in payBillItems)
            {
                if (bi.BillId != null)
                {
                    // update total balance in bill               
                    var updateTotalbalance = await _billRepository.GetAll().Where(u => u.Id == bi.BillId).FirstOrDefaultAsync();
                    if (updateTotalbalance != null)
                    {
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
                        CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                    }
                }
                else if (bi.VendorCreditId != null)
                {
                    var vendorCredit = await _vendorCreditRepository.GetAll().Where(u => u.Id == bi.VendorCreditId).FirstOrDefaultAsync();

                    //update status on table vendor credit
                    if (vendorCredit != null)
                    {
                        vendorCredit.IncreaseTotalPaid(bi.Payment * -1);
                        vendorCredit.IncreaseMultiCurrencyTotalPaid(bi.MultiCurrencyPayment * -1);
                        if (vendorCredit.TotalPaid == 0)
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
                    }
                }

            }


            entity.UpdateStatusToDraft();

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.PayBill)
                                .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get bill item and update status and reveres total paid 
            var payBillItems = await _payBillDetailRepository.GetAll()
                .Where(u => u.PayBillId == entity.PayBillId).ToListAsync();

            foreach (var bi in payBillItems)
            {
                if (bi.BillId != null)
                {
                    // update total balance in bill               
                    var updateTotalbalance = _billRepository.GetAll().Where(u => u.Id == bi.BillId).FirstOrDefault();

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
                    CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                }
                else if (bi.VendorCreditId != null)
                {
                    var vendorCredit = await _vendorCreditRepository.GetAll().Where(u => u.Id == bi.VendorCreditId).FirstOrDefaultAsync();

                    //update status on table vendor credit
                    if (vendorCredit != null)
                    {
                        vendorCredit.IncreaseTotalPaid(bi.Payment * -1);
                        vendorCredit.IncreaseMultiCurrencyTotalPaid(bi.MultiCurrencyPayment * -1);
                        if (vendorCredit.TotalPaid == 0)
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
                    }
                }
            }

            entity.UpdatePublish();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @jounal = await _journalRepository.GetAll()
                            .Include(u => u.PayBill)
                            .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId == input.Id)
                            .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get bill item and update status and reveres total paid 
            var payBillItems = await _payBillDetailRepository.GetAll()
                                .Where(u => u.PayBillId == @jounal.PayBillId).ToListAsync();

            foreach (var bi in payBillItems)
            {
                if (bi.BillId != null)
                {
                    // update total balance in bill               
                    var updateTotalbalance = await _billRepository.GetAll().Where(u => u.Id == bi.BillId).FirstOrDefaultAsync();
                    updateTotalbalance.UpdateOpenBalance(bi.Payment * -1);
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
                    CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                }
                else if (bi.VendorCreditId != null)
                {
                    var vendorCredit = await _vendorCreditRepository.GetAll().Where(u => u.Id == bi.VendorCreditId).FirstOrDefaultAsync();

                    //update status on table vendor credit
                    if (vendorCredit != null)
                    {
                        vendorCredit.IncreaseTotalPaid(bi.Payment * -1);
                        vendorCredit.IncreaseMultiCurrencyTotalPaid(bi.MultiCurrencyPayment * -1);
                        if (vendorCredit.TotalPaid == 0)
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
                    }
                }

            }


            jounal.UpdateVoid();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);
            CheckErrors(await _journalManager.UpdateAsync(jounal, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = jounal.ItemReceiptId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_Delete,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var journal = await _journalRepository.GetAll()
                            .Include(u => u.PayBill)
                            .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId == input.Id)
                            .FirstOrDefaultAsync();

            //query get paybill
            var @entity = journal.PayBill;

            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.PayBill || t.LockKey == TransactionLockType.BankTransaction)
                   && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.PayBill)
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

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var exchanges = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.PayBillId == input.Id).ToListAsync();
            if (exchanges.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchanges);

            var payBillItemExchanges = await _payBillItemExchangeRateRepository.GetAll().AsNoTracking().Where(s => s.PayBillItem.PayBillId == input.Id).ToListAsync();
            if (payBillItemExchanges.Any()) await _payBillItemExchangeRateRepository.BulkDeleteAsync(payBillItemExchanges);

            //query get journal and delete
            journal.UpdatePayBill(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            CheckErrors(await _journalManager.RemoveAsync(journal));


            var payBillExpenseItems = await _payBillExpenseRepository.GetAll()
                            .AsNoTracking()
                            .Where(u => u.PayBillId == entity.Id).ToListAsync();
            if(payBillExpenseItems.Any()) await _payBillExpenseRepository.BulkDeleteAsync(payBillExpenseItems);


            //query get bill item and delete 
            var payBillItems = await _payBillDetailRepository.GetAll()
                                .Include(t => t.Bill).Include(t => t.VendorCredit)
                                .Where(u => u.PayBillId == entity.Id).ToListAsync();


            foreach (var bi in payBillItems)
            {
                if (bi.Bill != null)
                {
                    // update total balance in bill               
                    var updateTotalbalance = bi.Bill;// await _billRepository.GetAll().Where(u => u.Id == bi.BillId).FirstOrDefaultAsync();
                    if (updateTotalbalance != null)
                    {
                        updateTotalbalance.UpdateTotalPaid(-1 * bi.Payment);
                        updateTotalbalance.UpdateMultiCurrencyTotalPaid(-1 * bi.MultiCurrencyPayment);

                        updateTotalbalance.UpdateOpenBalance(bi.Payment);
                        updateTotalbalance.UpdateMultiCurrencyOpenBalance(bi.MultiCurrencyPayment);
                        if (updateTotalbalance.TotalPaid == 0)
                        {
                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                    }
                }
                else if (bi.VendorCredit != null)
                {
                    var vendorCredit = bi.VendorCredit;// await _vendorCreditRepository.GetAll().Where(u => u.Id == bi.VendorCreditId).FirstOrDefaultAsync();

                    //update status on table vendor credit
                    if (vendorCredit != null)
                    {

                        vendorCredit.IncreaseOpenbalance(bi.Payment);
                        vendorCredit.IncreaseMultiCurrencyOpenBalance(bi.MultiCurrencyPayment);

                        vendorCredit.IncreaseTotalPaid(bi.Payment * -1);
                        vendorCredit.IncreaseMultiCurrencyTotalPaid(bi.MultiCurrencyPayment * -1);
                        if (vendorCredit.TotalPaid == 0)
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
                    }
                }
                CheckErrors(await _payBillDetailManager.RemoveAsync(bi));
            }

            CheckErrors(await _payBillManager.RemoveAsync(entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.PayBill, TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_GetDetail,
                      AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Delete)]
        public async Task<PayBillDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Currency)
                                .Include(u => u.PayBill.PaymentMethod.PaymentMethodBase)
                                .Include(u => u.Location)
                                .Include(u => u.MultiCurrency)
                                .Include(u => u.Class)
                                .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId == input.Id)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.PayBill == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var payBillDetailItem = await (from p in _payBillDetailRepository.GetAll()
                                    .Include(u => u.Vendor)
                                    .Include(u => u.Bill)
                                    .Include(u => u.VendorCredit)
                                    .Where(u => u.PayBillId == input.Id)
                                    .AsNoTracking()

                                           join ji in _journalItemRepository.GetAll()
                                           .AsNoTracking()
                                           on p.Id equals ji.Identifier

                                           join bj in _journalRepository.GetAll().Include(u => u.MultiCurrency)
                                           .AsNoTracking()
                                           on p.BillId equals bj.BillId into bjs
                                           from nBj in bjs.DefaultIfEmpty()

                                           join vj in _journalRepository.GetAll().Include(u => u.MultiCurrency)
                                           .AsNoTracking()
                                           on p.VendorCreditId equals vj.VendorCreditId into vjs
                                           from nVj in vjs.DefaultIfEmpty()

                                           where nBj != null || nVj != null

                                           select new PayBillDetailItemOutput()
                                           {
                                               CreationTime = p.CreationTime,
                                               BillId = p.BillId,
                                               //Bill = ObjectMapper.Map<BillSummaryPayBillOutput>(p.Bill),
                                               Bill = nBj != null && p.Bill != null ? new BillSummaryPayBillOutput()
                                               {
                                                   JournalNo = nBj.JournalNo,
                                                   DueDate = p.Bill.DueDate,
                                                   Date = nBj.Date,
                                                   Reference = nBj.Reference,
                                                   MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(nBj.MultiCurrency)
                                               } : null,
                                               Id = p.Id,
                                               VendorId = p.VendorId,
                                               Vendor = ObjectMapper.Map<VendorSummaryOutput>(p.Vendor),
                                               AccountId = ji.AccountId,
                                               Account = ObjectMapper.Map<ChartAccountSummaryOutput>(ji.Account),
                                               VendorCreditId = p.VendorCreditId,
                                               VendorCredit = nVj != null && p.VendorCredit != null ? new VendorCreditSummaryOutput()
                                               {
                                                   JournalNo = nVj.JournalNo,
                                                   DueDate = nVj.VendorCredit.DueDate,
                                                   Date = nVj.Date,
                                                   MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(nVj.MultiCurrency),
                                                   Reference = nVj.Reference,
                                               } : null,
                                               //VendorCredit = ObjectMapper.Map<VendorCreditSummaryOutput>(p.VendorCredit),
                                               OpenBalance = p.OpenBalance,
                                               Payment = p.Payment - p.LossGain,
                                               Amount = p.TotalAmount + p.LossGain,
                                               MultiCurrencyPayment = p.MultiCurrencyPayment,
                                               MultiCurrencyAmount = p.MultiCurrencyTotalAmount,
                                               MultiCurrencyOpenBalance = p.MultiCurrencyOpenBalance,
                                               MultiCurrencyId = nBj != null && p.Bill != null ? nBj.MultiCurrency.Id : nVj.MultiCurrency.Id,
                                               MultiCurrencyCode = nBj != null && p.Bill != null ? nBj.MultiCurrency.Code : nVj.MultiCurrency.Code,
                                               LossGain = p.LossGain,
                                               OpenBalanceInPaymentCurrency = p.OpenBalanceInPaymentCurrency,
                                               PaymentInPaymentCurrency = p.PaymentInPaymentCurrency,
                                               TotalAmountInPaymentCurrency = p.TotalAmountInPaymentCurrency
                                           }).OrderBy(u => u.CreationTime).ToListAsync();
            var result = ObjectMapper.Map<PayBillDetailOutput>(journal.PayBill);
            var payBillExpenseItem = await _payBillExpenseRepository.GetAll()
            .Include(u => u.Account)
            .Where(u => u.PayBillId == input.Id)
            .AsNoTracking()
               .Join(
                   _journalItemRepository.GetAll()
                   .AsNoTracking(),
                   u => u.Id,
                   s => s.Identifier,
                   (pbItem, jItem) =>
                   new PayBillExpenseItem()
                   {
                       CreationTime = pbItem.CreationTime,
                       AccountId = jItem.AccountId,
                       Description = pbItem.Description,
                       Account = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                       Amount = pbItem.Amount,
                       Id = pbItem.Id,
                       MultiCurrencyAmount = pbItem.MultiCurrencyAmount,
                       IsLossGain = pbItem.IsLossGain
                   }
               )
               .OrderBy(u => u.CreationTime).ToListAsync();

            result.PayBillExpenseItems = payBillExpenseItem.Where(s => !s.IsLossGain).ToList();
           
            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.PayBillId == input.Id)
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


                var exchangeDic = await _payBillItemExchangeRateRepository.GetAll().AsNoTracking()
                    .Where(s => s.PayBillItem.PayBillId == input.Id)
                    .Select(s => new GetExchangeRateDto
                    {
                        Id = s.PayBillItemId,
                        FromCurrencyCode = s.FromCurrency.Code,
                        ToCurrencyCode = s.ToCurrency.Code,
                        FromCurrencyId = s.FromCurrencyId,
                        ToCurrencyId = s.ToCurrencyId,
                        Bid = s.Bid,
                        Ask = s.Ask
                    })
                    .ToDictionaryAsync(k => k.Id);


                result.TotalLossGainBill = 0;
                result.TotalLossGainVendorCredit = 0;

                foreach (var item in payBillDetailItem)
                {
                    if (exchangeDic.ContainsKey(item.Id))
                    {   
                        item.ExchangeRate = exchangeDic[item.Id];
                        item.ExchangeRate.IsInves = exchangeDic[item.Id].FromCurrencyId == journal.MultiCurrencyId; 
                    }

                    if (item.Bill != null)
                    {
                        result.TotalLossGainBill += item.LossGain;
                    }
                    else
                    {  
                        result.TotalLossGainVendorCredit += item.LossGain;
                    }
                   
                }

                result.ExchangeLossGain = payBillExpenseItem.Where(s => s.IsLossGain).Select(s => new ExchangeLossGainDto { AccountId = s.AccountId, Amount = s.Amount }).FirstOrDefault();
            }


            result.PayBillNo = journal.JournalNo;
            result.PaymentDate = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.MultiCurrencyId = journal.MultiCurrencyId.Value;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.PayBillDetailItems = payBillDetailItem;
            result.PaymentMethodId = journal.PayBill.PaymentMethodId;
            result.PaymentMethodName = journal.PayBill.PaymentMethod == null ? "" : journal.PayBill.PaymentMethod.PaymentMethodBase.Name;


            if (journal.PayBill.ReceiveFrom == ReceiveFromPayBill.Cash)
            {
                var clearanceAccount = await (_journalItemRepository.GetAll()
                                           .Include(u => u.Account)
                                           .AsNoTracking()
                                           .Where(
                                                u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.Payment && u.Identifier == null
                                            )).FirstOrDefaultAsync();
                result.PaymentAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount.Account);
                result.PaymentAccountId = clearanceAccount.AccountId;
            }

            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.LocationId = journal.LocationId;
            result.LocationName = journal.Location?.LocationName;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_Update,
            AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdatePayBillInput input)
        {
            //validate paybill detail item
            if (input.PayBillDetail.Count == 0)
            {
                throw new UserFriendlyException(L("BillInfoHaveAtLeastOneRecord"));
            }

            await ValidateExchangeRate(input);

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.paymentDate.Date)
                                      && (t.LockKey == TransactionLockType.PayBill || t.LockKey == TransactionLockType.BankTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (input.ReceiveFrom == ReceiveFromPayBill.VendorCredit)
            {
                input.TotalPayment = 0;
                input.MultiCurrencyTotalPayment = 0;
                input.Change = 0;
                input.MultiCurrencyChange = 0;
            }

            if (input.Change == 0)
            {
                input.PayBillExpenseItem = new List<PayBillExpenseItem>();
            }
            else
            {
                var totalAmountCurrency = input.PayBillExpenseItem.Sum(t => t.Amount);
                var totalAmountMultiCurrency = input.PayBillExpenseItem.Sum(t => t.MultiCurrencyAmount);
                if (input.Change != totalAmountCurrency || input.MultiCurrencyChange != totalAmountMultiCurrency)
                {
                    throw new UserFriendlyException(L("YouMustAddAccountOfChange"));
                }
            }

            if (input.ReceiveFrom == ReceiveFromPayBill.VendorCredit
                && input.TotalPaymentBill != input.TotalPaymentVendorCredit
                && input.MultiCurrencyTotalPaymentBill != input.MultiCurrencyTotalPaymentVendorCredit)
            {
                throw new UserFriendlyException(L("TotalBillPaymentMustEqualVendorCreditPayment"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Include(u => u.PayBill)
                              .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId == input.Id)
                              .FirstOrDefaultAsync();
            if (@journal == null || @journal.PayBill == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
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

            if (input.ReceiveFrom == ReceiveFromPayBill.Cash)
            {
                if (input.PaymentAccountId == null || input.PaymentAccountId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("PaymentAccountIsRequired"));
                }
                //update Clearance account 
                var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                          .Where(u => u.JournalId == journal.Id &&
                                                   u.Key == PostingKey.Payment && u.Identifier == null))
                                            .FirstOrDefaultAsync();
                if (@clearanceAccountItem == null)
                {
                    var journalItem = JournalItem.CreateJournalItem(tenantId, userId, journal.Id, input.PaymentAccountId.Value, input.Memo, 0, input.TotalPayment, PostingKey.Payment, null);
                    if (input.TotalPayment < 0)
                    {
                        journalItem.SetDebitCredit(input.TotalPayment * -1, 0);
                    }
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));
                }
                else
                {
                    clearanceAccountItem.UpdateJournalItem(tenantId, input.PaymentAccountId.Value, input.Memo, 0, input.TotalPayment);
                    if (input.TotalPayment < 0)
                    {
                        clearanceAccountItem.SetDebitCredit(input.TotalPayment * -1, 0);
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

            //update paybill 
            var @paybill = @journal.PayBill;
            @paybill.Update(tenantId, input.FiFo, input.TotalOpenBalance, input.TotalPayment,
                            input.TotalPaymentDue, input.ReceiveFrom,
                            input.MultiCurrencyTotalPayment, input.Change,
                            input.TotalOpenBalanceVendorCredit, input.TotalPaymentVendorCredit,
                            input.TotalPaymentDueVendorCredit, input.MultiCurrencyTotalPaymentVendorCredit,
                            input.TotalPaymentBill, input.MultiCurrencyTotalPaymentBill, input.MultiCurrencyChange,
                            input.MultiCurrencyTotalOpenBalance, input.MultiCurrencyTotalOpenBalanceVendorCredit,
                            input.MultiCurrencyTotalPaymentDue, input.MultiCurrencyTotalPaymentDueVendorCredit
                            );

            if (input.PaymentMethodId.HasValue) paybill.SetPaymentMethod(input.PaymentMethodId);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));


            CheckErrors(await _payBillManager.UpdateAsync(@paybill));


            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PayBillId == input.Id);
            if(input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = PayBillExchangeRate.Create(tenantId, userId, paybill.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, paybill.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if(exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }


            //Update paybill detail and Journal Item detail
            var payBillDetail = await _payBillDetailRepository.GetAll().Include(t => t.Bill).Include(t => t.VendorCredit).Where(u => u.PayBillId == input.Id).ToListAsync();

            var @journalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           (u.Key == PostingKey.AP || u.Key == PostingKey.Clearance) && u.Identifier != null)).ToListAsync();

            var toDeletePayBillItem = payBillDetail.Where(u => !input.PayBillDetail.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = @journalItems.Where(u => !input.PayBillDetail.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            var payBillExpense = await _payBillExpenseRepository.GetAll().Where(u => u.PayBillId == input.Id).ToListAsync();
            var @journalItemsExpense = await (_journalItemRepository.GetAll()
                                        .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.PaymentChange && u.Identifier != null)).ToListAsync();

            var exchangeLossGainItem = payBillExpense.FirstOrDefault(s => s.IsLossGain);
            JournalItem exchangeLossGainJournalItem = null;
            if(exchangeLossGainItem != null) exchangeLossGainJournalItem = journalItemsExpense.FirstOrDefault(s => s.Identifier == exchangeLossGainItem.Id);

            var toDeletePayBillExpenseItem = payBillExpense
                                             .WhereIf(exchangeLossGainItem != null, s => s.Id != exchangeLossGainItem.Id)
                                             .Where(u => !input.PayBillExpenseItem.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalExpenseItem = @journalItemsExpense
                                             .WhereIf(exchangeLossGainJournalItem != null, s => s.Id != exchangeLossGainJournalItem.Id)
                                             .Where(u => !input.PayBillExpenseItem.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            // get bill and vendor credit for update of add new item in paybill items list
            var @bills = new List<Bill>();
            var @vendorCredits = new List<VendorCredit.VendorCredit>();
            if (input.PayBillDetail.Where(x => x.BillId != null && x.Id == null).Count() > 0)
            {
                @bills = await _billRepository.GetAll().Where(x => input.PayBillDetail.Any(t => t.Id == null && t.BillId == x.Id)).ToListAsync();
            }
            if (input.PayBillDetail.Where(x => x.VendorCreditId != null && x.Id == null).Count() > 0)
            {
                @vendorCredits = await _vendorCreditRepository.GetAll().Where(x => input.PayBillDetail.Any(t => t.Id == null && t.VendorCreditId == x.Id)).ToListAsync();
            }


            var payBillItemExchanges = await _payBillItemExchangeRateRepository.GetAll().AsNoTracking().Where(s => s.PayBillItem.PayBillId == input.Id).ToListAsync();
            var updateExchangeHash = new HashSet<Guid>();

            //Update and Journal Item
            foreach (var c in input.PayBillDetail)
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
                    var paybillItem = payBillDetail.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @journalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (paybillItem == null) throw new UserFriendlyException(L("RecordNotFound"));
                    
                    var amountToBeSubstract = c.Payment - paybillItem.Payment;
                    var amountToBeSubstractMulti = c.MultiCurrencyPayment - paybillItem.MultiCurrencyPayment;
                    paybillItem.Update(tenantId, c.OpenBalance, c.Payment, c.TotalAmount,
                                        c.MultiCurrencyOpenBalance, c.MultiCurrencyPayment,
                                        c.MultiCurrencyTotalAmount, c.BillId, c.VendorCreditId, c.LossGain,
                                        c.OpenBalanceInPaymentCurrency, c.PaymentInPaymentCurrency, c.TotalAmountInPaymentCurrency
                                        );
                    CheckErrors(await _payBillDetailManager.UpdateAsync(paybillItem));

                    if(input.UseExchangeRate && input.MultiCurrencyId != c.MultiCurrencyId)
                    {
                        var itemExchange = payBillItemExchanges.FirstOrDefault(r => r.PayBillItemId == c.Id);
                        if(itemExchange == null)
                        {
                            itemExchange = PayBillItemExchangeRate.Create(tenantId, userId, paybillItem.Id, c.ExchangeRate.FromCurrencyId, c.ExchangeRate.ToCurrencyId, c.ExchangeRate.Bid, c.ExchangeRate.Ask);
                            await _payBillItemExchangeRateRepository.InsertAsync(itemExchange);
                        }
                        else
                        {
                            itemExchange.Update(userId, paybillItem.Id, c.ExchangeRate.FromCurrencyId, c.ExchangeRate.ToCurrencyId, c.ExchangeRate.Bid, c.ExchangeRate.Ask);
                            await _payBillItemExchangeRateRepository.UpdateAsync(itemExchange);
                            updateExchangeHash.Add(itemExchange.Id);
                        }
                    }


                    if (c.BillId != null)
                    {
                        if (paybillItem.Bill != null)
                        {
                            // update total balance in bill
                            var updateTotalbalance = paybillItem.Bill;

                            updateTotalbalance.UpdateOpenBalance(amountToBeSubstract * -1);
                            updateTotalbalance.UpdateMultiCurrencyOpenBalance(amountToBeSubstractMulti * -1);

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
                                CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                            }
                        }
                        if (journalItem != null)
                        {
                            journalItem.UpdateJournalItem(userId, c.AccountId, "", c.Payment, 0);

                            if (c.Payment < 0)
                            {
                                journalItem.SetDebitCredit(0, -c.Payment);
                            }

                            CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                        }
                    }
                    else if (c.VendorCreditId != null)
                    {
                        // update total balance in credit
                        var updateTotalbalance = paybillItem.VendorCredit;

                        updateTotalbalance.IncreaseOpenbalance(amountToBeSubstract * -1);
                        updateTotalbalance.IncreaseMultiCurrencyOpenBalance(amountToBeSubstractMulti * -1);

                        //update status
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
                            CheckErrors(await _vendorCreditManager.UpdateAsync(updateTotalbalance));
                        }
                        if (journalItem != null)
                        {
                            journalItem.UpdateJournalItem(userId, c.AccountId, "", 0, c.Payment);

                            if (c.Payment < 0)
                            {
                                journalItem.SetDebitCredit(-c.Payment, 0);
                            }
                            CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                        }
                    }
                }
                else if (c.Id == null) //create
                {
                    //insert to paybill detail item
                    var @paybillDetailItem = PayBillDetail.Create(tenantId, userId, paybill, c.BillId, c.VendorId,
                            c.DueDate, c.OpenBalance, c.Payment, c.TotalAmount, c.MultiCurrencyOpenBalance,
                            c.MultiCurrencyPayment, c.MultiCurrencyTotalAmount, c.VendorCreditId, c.LossGain,
                            c.OpenBalanceInPaymentCurrency, c.PaymentInPaymentCurrency, c.TotalAmountInPaymentCurrency
                            );
                    CheckErrors(await _payBillDetailManager.CreateAsync(@paybillDetailItem));

                    if(input.UseExchangeRate && input.MultiCurrencyId != c.MultiCurrencyId)
                    {
                        var itemExchange = PayBillItemExchangeRate.Create(tenantId, userId, paybillDetailItem.Id, c.ExchangeRate.FromCurrencyId, c.ExchangeRate.ToCurrencyId, c.ExchangeRate.Bid, c.ExchangeRate.Ask);
                        await _payBillItemExchangeRateRepository.InsertAsync(itemExchange);
                    }

                    if (c.BillId != null)
                    {
                        // update total balance in bill 
                        var updateTotalbalance = @bills.Where(u => u.Id == c.BillId).FirstOrDefault();
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
                            CheckErrors(await _billManager.UpdateAsync(updateTotalbalance));
                        }

                        var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.AccountId, "", c.Payment, 0, PostingKey.AP, @paybillDetailItem.Id);

                        if (c.Payment < 0)
                        {
                            inventoryJournalItem.SetDebitCredit(0, -c.Payment);
                        }
                        CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                    }
                    else if (c.VendorCreditId != null)
                    {
                        // update total balance in vendor credit
                        var updateTotalbalance = @vendorCredits.Where(u => u.Id == c.VendorCreditId).FirstOrDefault();
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
                            CheckErrors(await _vendorCreditManager.UpdateAsync(updateTotalbalance));
                        }

                        //insert journal item into debit with default credit zero
                        var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.AccountId, "", 0, c.Payment, PostingKey.Clearance, @paybillDetailItem.Id);

                        if (c.Payment < 0)
                        {
                            inventoryJournalItem.SetDebitCredit(-c.Payment, 0);
                        }
                        CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                    }
                }
            }


            if ((input.ReceiveFrom == ReceiveFromPayBill.Cash || input.UseExchangeRate) && input.PayBillExpenseItem != null && input.PayBillExpenseItem.Count > 0)
            {
                foreach (var i in input.PayBillExpenseItem)
                {
                    if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                    {
                        i.MultiCurrencyAmount = i.Amount;
                    }
                    if (i.Id != null)
                    {
                        var paybillItemExpense = payBillExpense.FirstOrDefault(u => u.Id == i.Id);
                        var journalItemExpense = journalItemsExpense.FirstOrDefault(u => u.Identifier == i.Id);
                        // var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                        if (paybillItemExpense != null)
                        {
                            //set update before check update status paid for bill below for condition if publish
                            paybillItemExpense.Update(userId, i.Amount, i.AccountId, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                            CheckErrors(await _payBillExpenseManager.UpdateAsync(paybillItemExpense));

                        }
                        if (journalItemExpense != null)
                        {
                            if (i.Amount > 0)
                            {
                                journalItemExpense.SetDebitCredit(0, i.Amount);
                            }
                            else
                            {
                                journalItemExpense.SetDebitCredit(i.Amount * -1, 0);
                            }
                            CheckErrors(await _journalItemManager.UpdateAsync(journalItemExpense));
                        }
                    }
                    else
                    {
                        //insert to pay bill expense detail
                        var @payBillExpenseItem = PayBillExpense.Create(tenantId, userId, journal.PayBill,
                                                            i.AccountId, i.Amount, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                        CheckErrors(await _payBillExpenseManager.CreateAsync(@payBillExpenseItem));


                        JournalItem journalItemExpense;
                        if (i.Amount > 0)
                        {
                            journalItemExpense = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, i.AccountId,
                                                        i.Description, 0, i.Amount, PostingKey.PaymentChange,
                                                        @payBillExpenseItem.Id);
                        }
                        else
                        {
                            journalItemExpense = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, i.AccountId,
                                                        i.Description, i.Amount * -1, 0, PostingKey.PaymentChange,
                                                        @payBillExpenseItem.Id);
                        }
                        CheckErrors(await _journalItemManager.CreateAsync(journalItemExpense));
                    }

                }
            }


            var deleteExchanges = payBillItemExchanges.Where(s => !updateExchangeHash.Contains(s.Id)).ToList();
            if (deleteExchanges.Any()) await _payBillItemExchangeRateRepository.BulkDeleteAsync(deleteExchanges);

            if(input.UseExchangeRate && input.ExchangeLossGain != null && input.ExchangeLossGain.Amount != 0)
            {
                if(exchangeLossGainItem != null)
                {
                    exchangeLossGainItem.Update(userId, input.ExchangeLossGain.Amount, input.ExchangeLossGain.AccountId, 0, exchangeLossGainItem.Description, true);
                    await _payBillExpenseManager.UpdateAsync(exchangeLossGainItem);

                    if(exchangeLossGainJournalItem == null)
                    {
                        exchangeLossGainJournalItem = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, input.ExchangeLossGain.AccountId,
                                                        exchangeLossGainItem.Description, 0, input.ExchangeLossGain.Amount, PostingKey.PaymentChange,
                                                        exchangeLossGainItem.Id);

                        if (input.ExchangeLossGain.Amount < 0) exchangeLossGainJournalItem.SetDebitCredit(-input.ExchangeLossGain.Amount, 0);

                        CheckErrors(await _journalItemManager.CreateAsync(exchangeLossGainJournalItem));
                    }
                    else
                    {
                        exchangeLossGainJournalItem.UpdateJournalItem(userId, input.ExchangeLossGain.AccountId, exchangeLossGainItem.Description, 0, input.ExchangeLossGain.Amount);

                        if (input.ExchangeLossGain.Amount < 0) exchangeLossGainJournalItem.SetDebitCredit(-input.ExchangeLossGain.Amount, 0);

                        CheckErrors(await _journalItemManager.UpdateAsync(exchangeLossGainJournalItem));
                    }
                }
                else
                {
                    var addExchangeLossGain = PayBillExpense.Create(tenantId, userId, paybill, input.ExchangeLossGain.AccountId, input.ExchangeLossGain.Amount, 0, "Exchange Loss/Gain by System", true);
                    await _payBillExpenseManager.CreateAsync(addExchangeLossGain);

                    var addExchangeLossGainJournalItem = JournalItem.CreateJournalItem(
                                                        tenantId, userId, journal.Id, input.ExchangeLossGain.AccountId,
                                                        addExchangeLossGain.Description, 0, input.ExchangeLossGain.Amount, PostingKey.PaymentChange,
                                                        addExchangeLossGain.Id);

                    if (input.ExchangeLossGain.Amount < 0) addExchangeLossGainJournalItem.SetDebitCredit(-input.ExchangeLossGain.Amount, 0);

                    CheckErrors(await _journalItemManager.CreateAsync(addExchangeLossGainJournalItem));                    
                }
            }
            else if(exchangeLossGainItem != null || exchangeLossGainJournalItem != null)
            {
                await _journalItemManager.RemoveAsync(exchangeLossGainJournalItem);
                await _payBillExpenseRepository.DeleteAsync(exchangeLossGainItem);
            }

            foreach (var t in toDeletePayBillItem)
            {
                if (t.Bill != null)
                {
                    var bill = t.Bill;// await _billRepository.GetAll().Where(u => u.Id == t.BillId).FirstOrDefaultAsync();
                    bill.UpdateOpenBalance(t.Payment);
                    bill.UpdateTotalPaid(-1 * t.Payment);

                    bill.UpdateMultiCurrencyOpenBalance(t.MultiCurrencyPayment);
                    bill.UpdateMultiCurrencyTotalPaid(-1 * t.MultiCurrencyPayment);
                    if (bill.TotalPaid == 0)
                    {
                        bill.UpdatePaidStatus(PaidStatuse.Pending);
                    }
                    else
                    {
                        bill.UpdatePaidStatus(PaidStatuse.Partial);
                    }
                    CheckErrors(await _billManager.UpdateAsync(bill));
                }
                else if (t.VendorCredit != null)
                {
                    var vendorCredit = t.VendorCredit;// await _vendorCreditRepository.GetAll().Where(u => u.Id == t.VendorCreditId).FirstOrDefaultAsync();
                    vendorCredit.IncreaseOpenbalance(t.Payment);
                    vendorCredit.IncreaseTotalPaid(-1 * t.Payment);

                    vendorCredit.IncreaseMultiCurrencyOpenBalance(t.MultiCurrencyPayment);
                    vendorCredit.IncreaseMultiCurrencyTotalPaid(-1 * t.MultiCurrencyPayment);
                    if (vendorCredit.TotalPaid == 0)
                    {
                        vendorCredit.UpdatePaidStatus(PaidStatuse.Pending);
                    }
                    else
                    {
                        vendorCredit.UpdatePaidStatus(PaidStatuse.Partial);
                    }
                    CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
                }
                CheckErrors(await _payBillDetailManager.RemoveAsync(t));
            }


            foreach (var t in toDeletePayBillExpenseItem)
            {
                CheckErrors(await _payBillExpenseManager.RemoveAsync(t));
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
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.PayBill, TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = @paybill.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_ViewBillHistory)]
        public async Task<PagedResultDto<GetListPayBillHistoryOutput>> ViewBillHistory(GetListViewHistoryInput input)
        {
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.PayBill)

                         on jItem.JournalId equals j.Id
                         join pb in _payBillRepository.GetAll().AsNoTracking()
                         on j.PayBillId equals pb.Id

                         join pbd in _payBillDetailRepository.GetAll().Include(u => u.Bill).Include(u => u.Vendor).AsNoTracking()
                         on pb.Id equals pbd.PayBillId
                         where (pbd.BillId == input.Id)
                         join c in _currencyRepository.GetAll().AsNoTracking() on j.CurrencyId equals c.Id

                         group jItem by new { journal = j, payBill = pb, currency = c, payBillDetail = pbd } into u

                         select new GetListPayBillHistoryOutput
                         {
                             Id = u.Key.payBill.Id,
                             PaymentDate = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             TotalPayment = u.Key.payBillDetail.Payment,
                             MultiCurrencyTotalPayment = u.Key.payBillDetail.MultiCurrencyPayment,
                             Status = u.Key.journal.Status,
                             AccountName = (from acc in _chartOfAccountRepository.GetAll().AsNoTracking()
                                            .Where(t => u.Any(a => a.AccountId == t.Id))
                                            select acc.AccountName).FirstOrDefault(),
                             Type = u.Key.payBill.ReceiveFrom.ToString(),
                             Reference = u.Key.journal.Reference
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListPayBillHistoryOutput>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_ViewVendorCreditHistory)]
        public async Task<PagedResultDto<GetListPayBillHistoryOutput>> ViewVendorCreditHistory(GetListViewHistoryInput input)
        {
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.PayBill)

                         on jItem.JournalId equals j.Id
                         join pb in _payBillRepository.GetAll().AsNoTracking()
                         on j.PayBillId equals pb.Id

                         join pbd in _payBillDetailRepository.GetAll().Include(u => u.Bill).Include(u => u.Vendor).AsNoTracking()
                         on pb.Id equals pbd.PayBillId
                         //where (pb.VendorCreditId == input.Id)
                         join c in _currencyRepository.GetAll().AsNoTracking() on j.CurrencyId equals c.Id

                         group jItem by new { journal = j, payBill = pb, currency = c, payBillDetail = pbd } into u

                         select new GetListPayBillHistoryOutput
                         {
                             Id = u.Key.payBill.Id,
                             PaymentDate = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             TotalPayment = u.Key.payBillDetail.Payment,
                             MultiCurrencyTotalPayment = u.Key.payBillDetail.MultiCurrencyPayment,
                             Status = u.Key.journal.Status,
                             AccountName = (from acc in _chartOfAccountRepository.GetAll().AsNoTracking()
                                           .Where(t => u.Any(a => a.AccountId == t.Id))
                                            select acc.AccountName).FirstOrDefault(),
                             Type = u.Key.payBill.ReceiveFrom.ToString(),
                             Reference = u.Key.journal.Reference
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListPayBillHistoryOutput>(resultCount, @entities);
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_GetList)]
        public async Task<MultiCurrencyPagedResultDto<GetListVendorBalanceOutput>> GetVendorBalance(GetListVendorBalanceInput input)
        {

            return input.CurrencyFilter == CurrencyFilter.MultiCurrencies ? await GetVendorBalanceMultiCurrencyHelper(input) : await GetVendorBalanceHelper(input);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_PayBills_MultiDelete)]
        [UnitOfWork(IsDisabled = true)]
        public async Task MultiDelete(GetListDeleteInput input)
        {
            var tenantId = AbpSession.TenantId.Value;
            var journal = new List<Journal>();
            var vendorCredits = new List<VendorCredit.VendorCredit>();
            var bills = new List<Bill>();
            var payBillItems = new List<PayBillDetail>();
            var @jounalItems = new List<JournalItem>();
            var auto = new AutoSequence();
            var @entity = new List<PayBill>();
            var payBillExpenseItems = new List<PayBillExpense>();
            var exchanges = new List<PayBillExchangeRate>();
            var payBillItemExchanges = new List<PayBillItemExchangeRate>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    journal = await _journalRepository.GetAll()
                    .Include(u => u.PayBill)
                    .AsNoTracking()
                    .Where(u => u.JournalType == JournalType.PayBill && u.PayBillId != null && input.Ids.Contains(u.PayBillId.Value))
                    .ToListAsync();
                    //query get paybill
                    @entity = journal.Select(t => t.PayBill).ToList();
                    
                    if (entity == null)
                    {
                        throw new UserFriendlyException(L("RecordNotFound"));
                    }
                    var lockDate = journal.OrderByDescending(t => t.Date).Select(t => t.Date.Date).FirstOrDefault();
                    var locktransaction = await _lockRepository.GetAll()
                       .Where(t => (t.LockKey == TransactionLockType.PayBill || t.LockKey == TransactionLockType.BankTransaction)
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
                    auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PayBill);
                    var noReceipt = journal.OrderByDescending(t => t.JournalNo).Select(t => t.JournalNo).FirstOrDefault();
                    var journalIds = journal.Select(t => t.Id).ToList();

                    if (noReceipt == auto.LastAutoSequenceNumber)
                    {
                        var jo = await _journalRepository.GetAll().AsNoTracking().Where(t => !journalIds.Contains(t.Id) && t.JournalType == JournalType.PayBill)
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

                    

                    //query get journal and delete
                    foreach (var j in journal)
                    {
                        j.UpdatePayBill(null);
                    }

                    //query get journal item and delete
                    @jounalItems = await _journalItemRepository.GetAll().AsNoTracking().Where(u => journalIds.Contains(u.JournalId)).ToListAsync();
                    //query get bill expense item and delete 

                     payBillExpenseItems = await _payBillExpenseRepository.GetAll().AsNoTracking()
                                       .Where(u => input.Ids.Contains(u.PayBillId)).ToListAsync();
                
                    //query get bill item and delete 
                    payBillItems = await _payBillDetailRepository.GetAll().AsNoTracking()
                                        .Include(t => t.Bill).Include(t => t.VendorCredit)
                                        .Where(u => input.Ids.Contains(u.PayBillId)).ToListAsync();

                    exchanges = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => input.Ids.Contains(s.PayBillId)).ToListAsync();
                    payBillItemExchanges = await _payBillItemExchangeRateRepository.GetAll().AsNoTracking().Where(s => input.Ids.Contains(s.PayBillItem.PayBillId)).ToListAsync();
                }
            }
          
            foreach (var bi in payBillItems)
            {
                if (bi.Bill != null)
                {
                    // update total balance in bill
                    var find = bills.FirstOrDefault(s => s.Id == bi.BillId);
                    var updateTotalbalance = find == null ? bi.Bill : find;
                    if (updateTotalbalance != null)
                    {
                        updateTotalbalance.UpdateTotalPaid(-1 * bi.Payment);
                        updateTotalbalance.UpdateMultiCurrencyTotalPaid(-1 * bi.MultiCurrencyPayment);

                        updateTotalbalance.UpdateOpenBalance(bi.Payment);
                        updateTotalbalance.UpdateMultiCurrencyOpenBalance(bi.MultiCurrencyPayment);
                        if (updateTotalbalance.TotalPaid == 0)
                        {
                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                        if (find == null) bills.Add(updateTotalbalance);                    
                    }
                }
                else if (bi.VendorCredit != null)
                {
                    var find = vendorCredits.FirstOrDefault(s => s.Id == bi.VendorCreditId);
                    var vendorCredit = find == null? bi.VendorCredit : find;

                    //update status on table vendor credit
                    if (vendorCredit != null)
                    {

                        vendorCredit.IncreaseOpenbalance(bi.Payment);
                        vendorCredit.IncreaseMultiCurrencyOpenBalance(bi.MultiCurrencyPayment);

                        vendorCredit.IncreaseTotalPaid(bi.Payment * -1);
                        vendorCredit.IncreaseMultiCurrencyTotalPaid(bi.MultiCurrencyPayment * -1);
                        if (vendorCredit.TotalPaid == 0)
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Pending);
                        }
                        else
                        {
                            vendorCredit.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                       if(find == null) vendorCredits.Add(vendorCredit);                      
                    }
                }
              //  payBillItems.Add(bi);              
            }

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                if (exchanges.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchanges);
                if (payBillItemExchanges.Any()) await _payBillItemExchangeRateRepository.BulkDeleteAsync(payBillItemExchanges);

                await _autoSequenceRepository.BulkUpdateAsync(new List<AutoSequence> { auto });
                await _journalRepository.BulkUpdateAsync(journal);
                await _payBillExpenseRepository.BulkDeleteAsync(payBillExpenseItems);
                await _journalItemRepository.BulkDeleteAsync(jounalItems);
                await _payBillDetailRepository.BulkDeleteAsync(payBillItems);
                await _billRepository.BulkUpdateAsync(bills);
                await _vendorCreditRepository.BulkUpdateAsync(vendorCredits);             
                await _journalRepository.BulkDeleteAsync(journal);
                await _payBillRepository.BulkDeleteAsync(entity);
                await uow.CompleteAsync();
            }

        }


        private async Task<MultiCurrencyPagedResultDto<GetListVendorBalanceOutput>> GetVendorBalanceMultiCurrencyHelper(GetListVendorBalanceInput input)
        {
            var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);
            var fromDateBeginning = previousCycle != null ? previousCycle.EndDate.Value.AddDays(1) : DateTime.MinValue;

            var vendorGroupIds = await GetUserGroupVendors();

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var vendorQuery = GetVendorWithCodeType(vendorGroupIds, input.Vendors, input.VendorTypes, vendorTypeMemberIds);

            // get provious 
            var previousBillQuery = from i in _vendorOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.Bill)
                                            .AsNoTracking()

                                    join j in _journalRepository.GetAll()
                                        .Where(s => s.Status == TransactionStatus.Publish)
                                        .Where(s => s.BillId.HasValue)
                                        .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Bill.VendorId))
                                        .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                        .AsNoTracking()
                                    on i.TransactionId equals j.BillId

                                    join ji in _journalItemRepository.GetAll()
                                         .Where(s => s.Identifier == null)
                                         .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                         .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                         .AsNoTracking()

                                    on j.Id equals ji.JournalId

                                    select new VendorBalanceQuery
                                    {
                                        TransactionId = i.TransactionId,
                                        VendorId = j.Bill.VendorId,
                                        CurrencyCode = j.MultiCurrency.Code,
                                        CurrencyId = j.MultiCurrencyId.Value,
                                        Total = i.MuliCurrencyBalance,
                                        Paid = 0,
                                        Balance = i.MuliCurrencyBalance,
                                        AccountId = ji.AccountId
                                    };

            var billQuery = from u in _journalItemRepository.GetAll()
                                        .Where(t => t.Identifier == null)
                                        .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                        .Where(t => t.Journal.BillId.HasValue)
                                        .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                        .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                        .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                        .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Journal.Bill.VendorId))
                                        .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                        .AsNoTracking()
                            select new VendorBalanceQuery
                            {
                                TransactionId = u.Journal.BillId.Value,
                                VendorId = u.Journal.Bill.VendorId,
                                CurrencyCode = u.Journal.MultiCurrency.Code,
                                CurrencyId = u.Journal.MultiCurrencyId.Value,
                                Total = u.Journal.Bill.MultiCurrencyTotal,
                                Paid = u.Journal.Bill.MultiCurrencyTotalPaid,
                                Balance = u.Journal.Bill.MultiCurrencyOpenBalance,
                                AccountId = u.AccountId
                            };


            var billPayments = from rpi in _payBillDetailRepository.GetAll()
                                    .Where(s => s.BillId.HasValue)
                                    .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.VendorId))
                                    .AsNoTracking()

                               join j in _journalRepository.GetAll()
                                 .Where(s => s.Status == TransactionStatus.Publish)
                                 .Where(u => u.PayBillId.HasValue)
                                 .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                 .AsNoTracking()
                               on rpi.PayBillId equals j.PayBillId

                               join ij in _journalRepository.GetAll()
                                 .Where(s => s.Status == TransactionStatus.Publish)
                                 .Where(u => u.BillId.HasValue)
                                 .AsNoTracking()
                               on rpi.BillId equals ij.BillId

                               select new
                               {
                                   VendorId = rpi.VendorId,
                                   PaymentAmount = rpi.MultiCurrencyPayment,
                                   TransactionId = rpi.BillId.Value,
                                   CurrencyId = ij.MultiCurrencyId.Value,
                                   Date = j.Date,
                                   PaymentId = rpi.PayBillId
                               };

            var billBalanceQuery = from i in billQuery.Concat(previousBillQuery)
                                   join p in billPayments
                                   on i.TransactionId equals p.TransactionId
                                   into ps
                                   let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                   select new VendorBalanceQuery
                                   {
                                       TransactionId = i.TransactionId,
                                       VendorId = i.VendorId,
                                       CurrencyCode = i.CurrencyCode,
                                       CurrencyId = i.CurrencyId,
                                       Total = i.Total,
                                       Paid = paid,
                                       Balance = i.Total - paid,
                                       AccountId = i.AccountId
                                   };


            var previousCreditQuery = from i in _vendorOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.VendorCredit)
                                            .AsNoTracking()

                                      join j in _journalRepository.GetAll()
                                          .Where(s => s.Status == TransactionStatus.Publish)
                                          .Where(s => s.VendorCreditId.HasValue)
                                          .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Bill.VendorId))
                                          .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                          .AsNoTracking()
                                      on i.TransactionId equals j.VendorCreditId

                                      join ji in _journalItemRepository.GetAll()
                                           .Where(s => s.Identifier == null)
                                           .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                           .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                           .AsNoTracking()

                                      on j.Id equals ji.JournalId

                                      select new VendorBalanceQuery
                                      {
                                          TransactionId = i.TransactionId,
                                          VendorId = j.VendorCredit.VendorId,
                                          CurrencyId = j.MultiCurrencyId.Value,
                                          CurrencyCode = j.MultiCurrency.Code,
                                          Total = -i.MuliCurrencyBalance,
                                          Paid = 0,
                                          Balance = -i.MuliCurrencyBalance,
                                          AccountId = ji.AccountId
                                      };


            var vendorCreditQuery = from u in _journalItemRepository.GetAll()
                                               .Where(t => t.Identifier == null)
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(t => t.Journal.VendorCreditId.HasValue)
                                               .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                               .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                               .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Journal.VendorCredit.VendorId))
                                               .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                                    select new VendorBalanceQuery
                                    {
                                        TransactionId = u.Journal.VendorCreditId.Value,
                                        VendorId = u.Journal.VendorCredit.VendorId,
                                        CurrencyCode = u.Journal.MultiCurrency.Code,
                                        CurrencyId = u.Journal.MultiCurrencyId.Value,
                                        Total = -u.Journal.VendorCredit.MultiCurrencyTotal,
                                        Paid = -u.Journal.VendorCredit.MultiCurrencyTotalPaid,
                                        Balance = -u.Journal.VendorCredit.MultiCurrencyOpenBalance,
                                        AccountId = u.AccountId
                                    };



            var creditPayments = from rpi in _payBillDetailRepository.GetAll()
                                    .Where(s => s.VendorCreditId.HasValue)
                                    .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.VendorId))
                                    .AsNoTracking()

                                 join j in _journalRepository.GetAll()
                                    .Where(s => s.Status == TransactionStatus.Publish)
                                    .Where(u => u.PayBillId.HasValue)
                                    .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                    .AsNoTracking()
                                 on rpi.PayBillId equals j.PayBillId

                                 select new
                                 {
                                     VendorId = rpi.VendorId,
                                     PaymentAmount = -rpi.MultiCurrencyPayment,
                                     TransactionId = rpi.VendorCreditId.Value,
                                 };

            var creditBalanceQuery = from i in vendorCreditQuery.Concat(previousCreditQuery)
                                     join p in creditPayments
                                     on i.TransactionId equals p.TransactionId
                                     into ps
                                     let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                     select new VendorBalanceQuery
                                     {
                                         TransactionId = i.TransactionId,
                                         VendorId = i.VendorId,
                                         CurrencyId = i.CurrencyId,
                                         CurrencyCode = i.CurrencyCode,
                                         Total = i.Total,
                                         Paid = paid,
                                         Balance = i.Total - paid,
                                         AccountId = i.AccountId
                                     };


            var lastPapymentQuery = from p in billPayments
                                    orderby p.Date descending
                                    group p by p.VendorId into g
                                    select new
                                    {
                                        VendorId = g.Key,
                                        PaymentItems = g.GroupBy(i => i.PaymentId).FirstOrDefault().ToList()
                                    };

            var currencies = await (from b in billBalanceQuery.Concat(creditBalanceQuery)
                                    join c in vendorQuery
                                    on b.VendorId equals c.Id
                                    group b by new
                                    {
                                        b.CurrencyId,
                                        b.CurrencyCode
                                    }
                                    into g
                                    select g.Key)
                                    .ToListAsync();

            var balanceQuery = from c in vendorQuery

                               join b in billBalanceQuery.Concat(creditBalanceQuery)
                               on c.Id equals b.VendorId
                               into bs

                               join p in lastPapymentQuery
                               on c.Id equals p.VendorId
                               into ps
                               from p in ps.DefaultIfEmpty()

                               where bs != null && bs.Any()

                               select new GetListVendorBalanceOutput
                               {
                                   VendorId = c.Id,
                                   VendorCode = c.VendorCode,
                                   VendorName = c.VendorName,
                                   VendorTypeName = c.VendorTypeName,
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

            return new MultiCurrencyPagedResultDto<GetListVendorBalanceOutput> { Currencies = currencies.Select(s => s.CurrencyCode).ToList(), Items = items, TotalCount = totalCount };
        }

        private async Task<MultiCurrencyPagedResultDto<GetListVendorBalanceOutput>> GetVendorBalanceHelper(GetListVendorBalanceInput input)
        {
            var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);
            var fromDateBeginning = previousCycle != null ? previousCycle.EndDate.Value.AddDays(1) : DateTime.MinValue;

            var vendorGroupIds = await GetUserGroupVendors();

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var vendorQuery = GetVendorWithCodeType(vendorGroupIds, input.Vendors, input.VendorTypes, vendorTypeMemberIds);

            // get provious 
            var previousBillQuery = from i in _vendorOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.Bill)
                                            .AsNoTracking()

                                    join j in _journalRepository.GetAll()
                                        .Where(s => s.Status == TransactionStatus.Publish)
                                        .Where(s => s.BillId.HasValue)
                                        .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Bill.VendorId))
                                        .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                        .AsNoTracking()
                                    on i.TransactionId equals j.BillId

                                    join ji in _journalItemRepository.GetAll()
                                         .Where(s => s.Identifier == null)
                                         .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                         .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                         .AsNoTracking()

                                    on j.Id equals ji.JournalId

                                    select new VendorBalanceQuery
                                    {
                                        TransactionId = i.TransactionId,
                                        VendorId = j.Bill.VendorId,
                                        Total = i.Balance,
                                        Paid = 0,
                                        Balance = i.Balance,
                                        AccountId = ji.AccountId
                                    };

            var billQuery = from u in _journalItemRepository.GetAll()
                                        .Where(t => t.Identifier == null)
                                        .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                        .Where(t => t.Journal.BillId.HasValue)
                                        .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                        .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                        .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                        .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Journal.Bill.VendorId))
                                        .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                        .AsNoTracking()
                            select new VendorBalanceQuery
                            {
                                TransactionId = u.Journal.BillId.Value,
                                VendorId = u.Journal.Bill.VendorId,
                                Total = u.Journal.Bill.Total,
                                Paid = u.Journal.Bill.TotalPaid,
                                Balance = u.Journal.Bill.OpenBalance,
                                AccountId = u.AccountId
                            };


            var billPayments = from rpi in _payBillDetailRepository.GetAll()
                                      .Where(s => s.BillId.HasValue)
                                      .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.VendorId))
                                      .AsNoTracking()

                               join j in _journalRepository.GetAll()
                                   .Where(s => s.Status == TransactionStatus.Publish)
                                   .Where(u => u.PayBillId.HasValue)
                                   .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                   .AsNoTracking()
                               on rpi.PayBillId equals j.PayBillId
                               select new
                               {
                                   VendorId = rpi.VendorId,
                                   PaymentAmount = rpi.Payment,
                                   TransactionId = rpi.BillId.Value,
                                   Date = j.Date,
                                   PaymentId = rpi.PayBillId
                               };

            var billBalanceQuery = from i in billQuery.Concat(previousBillQuery)
                                   join p in billPayments
                                   on i.TransactionId equals p.TransactionId
                                   into ps
                                   let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                   select new VendorBalanceQuery
                                   {
                                       TransactionId = i.TransactionId,
                                       VendorId = i.VendorId,
                                       Total = i.Total,
                                       Paid = paid,
                                       Balance = i.Total - paid,
                                       AccountId = i.AccountId
                                   };


            var previousCreditQuery = from i in _vendorOpenBalanceRepository.GetAll()
                                            .Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => s.Key == JournalType.VendorCredit)
                                            .AsNoTracking()

                                      join j in _journalRepository.GetAll()
                                          .Where(s => s.Status == TransactionStatus.Publish)
                                          .Where(s => s.VendorCreditId.HasValue)
                                          .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Bill.VendorId))
                                          .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => u.LocationId.HasValue && input.Locations.Contains(u.LocationId.Value))
                                          .AsNoTracking()
                                      on i.TransactionId equals j.VendorCreditId

                                      join ji in _journalItemRepository.GetAll()
                                           .Where(s => s.Identifier == null)
                                           .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                           .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                           .AsNoTracking()

                                      on j.Id equals ji.JournalId

                                      select new VendorBalanceQuery
                                      {
                                          TransactionId = i.TransactionId,
                                          VendorId = j.VendorCredit.VendorId,
                                          Total = -i.Balance,
                                          Paid = 0,
                                          Balance = -i.Balance,
                                          AccountId = ji.AccountId
                                      };


            var vendorCreditQuery = from u in _journalItemRepository.GetAll()
                                               .Where(t => t.Identifier == null)
                                               .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                               .Where(t => t.Journal.VendorCreditId.HasValue)
                                               .Where(u => fromDateBeginning.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= input.ToDate.Date)
                                               .WhereIf(input.Accounts != null && input.Accounts.Any(), u => input.Accounts.Contains(u.AccountId))
                                               .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), u => input.AccountTypes.Contains(u.Account.AccountTypeId))
                                               .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.Journal.VendorCredit.VendorId))
                                               .WhereIf(input.Locations != null && input.Locations.Count() > 0, u => input.Locations.Contains(u.Journal.LocationId.Value))
                                               .AsNoTracking()
                                    select new VendorBalanceQuery
                                    {
                                        TransactionId = u.Journal.VendorCreditId.Value,
                                        VendorId = u.Journal.VendorCredit.VendorId,
                                        Total = -u.Journal.VendorCredit.Total,
                                        Paid = -u.Journal.VendorCredit.TotalPaid,
                                        Balance = -u.Journal.VendorCredit.OpenBalance,
                                        AccountId = u.AccountId
                                    };



            var creditPayments = from rpi in _payBillDetailRepository.GetAll()
                                      .Where(s => s.VendorCreditId.HasValue)
                                      .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u => input.Vendors.Contains(u.VendorId))
                                      .AsNoTracking()

                                 join j in _journalRepository.GetAll()
                                     .Where(s => s.Status == TransactionStatus.Publish)
                                     .Where(u => u.PayBillId.HasValue)
                                     .Where(u => u.Date.Date >= fromDateBeginning.Date && u.Date.Date <= input.ToDate.Date)
                                     .AsNoTracking()
                                 on rpi.PayBillId equals j.PayBillId
                                 select new
                                 {
                                     VendorId = rpi.VendorId,
                                     PaymentAmount = -rpi.Payment,
                                     TransactionId = rpi.VendorCreditId.Value
                                 };

            var creditBalanceQuery = from i in vendorCreditQuery.Concat(previousCreditQuery)
                                     join p in creditPayments
                                     on i.TransactionId equals p.TransactionId
                                     into ps
                                     let paid = ps == null ? 0 : ps.Sum(s => s.PaymentAmount)
                                     select new VendorBalanceQuery
                                     {
                                         TransactionId = i.TransactionId,
                                         VendorId = i.VendorId,
                                         Total = i.Total,
                                         Paid = paid,
                                         Balance = i.Total - paid,
                                         AccountId = i.AccountId
                                     };

            var lastPapymentQuery = from p in billPayments
                                    orderby p.Date descending
                                    group p by p.VendorId into g
                                    select new
                                    {
                                        VendorId = g.Key,
                                        PaymentItems = g.GroupBy(i => i.PaymentId).FirstOrDefault().ToList()
                                    };

            var balanceQuery = from c in vendorQuery

                               join b in billBalanceQuery.Concat(creditBalanceQuery)
                               on c.Id equals b.VendorId
                               into bs

                               join p in lastPapymentQuery
                               on c.Id equals p.VendorId
                               into ps
                               from p in ps.DefaultIfEmpty()

                               where bs != null && bs.Any()

                               select new GetListVendorBalanceOutput
                               {
                                   VendorId = c.Id,
                                   VendorCode = c.VendorCode,
                                   VendorName = c.VendorName,
                                   VendorTypeName = c.VendorTypeName,
                                   Balance = bs.Sum(s => s.Balance),
                                   ToDate = input.ToDate,
                                   LastPaymentDate = p == null ? (DateTime?)null : p.PaymentItems.FirstOrDefault().Date,
                                   LastPayment = p == null ? 0 : p.PaymentItems.Sum(t => t.PaymentAmount)
                               };

            var allQuery = balanceQuery.Where(b => b.Balance != 0);

            var totalCount = await allQuery.CountAsync();
            var items = await allQuery.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new MultiCurrencyPagedResultDto<GetListVendorBalanceOutput> { Items = items, TotalCount = totalCount };
        }


    }
}
