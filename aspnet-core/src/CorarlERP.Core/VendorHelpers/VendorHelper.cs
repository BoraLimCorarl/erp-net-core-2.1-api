using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using CorarlERP.AccountCycles;
using CorarlERP.CustomerManager;
using CorarlERP.CustomerTypes;
using CorarlERP.Dto;
using CorarlERP.Journals;
using CorarlERP.PayBills;
using CorarlERP.UserGroups;
using CorarlERP.VendorCustomerOpenBalances;
using CorarlERP.VendorHelpers.Data;
using CorarlERP.Vendors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorHelpers
{
    public class VendorHelper : CorarlERPDomainServiceBase, IVendorHelper
    {

        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IRepository<PayBillDetail, Guid> _paybillItemRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IRepository<VendorGroup, Guid> _vendorGroupRepository;
        private readonly IRepository<VendorOpenBalance, Guid> _vendorOpenBalanceRepository;

        public VendorHelper(
             IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<VendorOpenBalance, Guid> vendorOpenBalanceRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<PayBillDetail, Guid> paybillItemRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Vendor, Guid> vendorRepository,
            IRepository<VendorGroup, Guid> vendorGroupRepository
        ):
            base(accountCycleRepository)
        {
            _vendorGroupRepository = vendorGroupRepository;
            _vendorRepository = vendorRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _journalRepository = journalRepository;
            _journalItemRepository = journalItemRepository;
            _paybillItemRepository = paybillItemRepository;
            _vendorOpenBalanceRepository = vendorOpenBalanceRepository;
        }
        private IQueryable<Guid> GetVendorGroup(long userId)
        {
            // get user by group member
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == userId)
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToList();

            var @query = _vendorGroupRepository.GetAll()
                            .Where(t => t.Vendor.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            //.Where(p => p.Vendor.IsActive == true)
                            .AsNoTracking()
                            .Select(t => t.Vendor);

            var @queryAll = _vendorRepository.GetAll()
                            .Where(t => t.Member == Member.All)
                            //.Where( p => p.IsActive == true)
                            .AsNoTracking();

            var result = @queryAll.Union(query)
                        .Select(t => t.Id);
            return result;
        }

        public IQueryable<BillAndCreditVendorOutput> GetBillAndCreditVendorQueryAsyn(
                                   DateTime? fromDate, DateTime toDate, string filter, List<Guid> vendors,
                                   List<Guid> accounts, List<JournalType> journalType, List<long> accountType, CurrencyFilter currencyFilter, List<long> locations = null, List<long> vendorTypes = null)
        {

            var filterByAccountingCurrency = currencyFilter != CurrencyFilter.MultiCurrencies;

            var billQuery = (from u in _journalItemRepository.GetAll()
                                    .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                    .Where(t => t.Identifier == null)
                                    .Where(t => t.Journal.Bill != null)
                                    .WhereIf(accounts != null && accounts.Count() > 0,
                                          u => accounts.Contains(u.AccountId))
                                    .WhereIf(accountType != null && accountType.Count() > 0,
                                          u => accountType.Contains(u.Account.AccountTypeId))
                                    .WhereIf(toDate != null,
                                             (u => ((fromDate == null) || (u.Journal.Date.Date >= fromDate.Value.Date)) &&
                                             (u.Journal.Date.Date <= toDate.Date)))
                                     .WhereIf(journalType != null && journalType.Count() > 0,
                                            u => journalType.Contains(u.Journal.JournalType))
                                    .WhereIf(!filter.IsNullOrEmpty(),
                                             u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                    .WhereIf(vendors != null && vendors.Count() > 0,
                                            u => vendors.Contains(u.Journal.Bill.VendorId))
                                    .WhereIf(vendorTypes != null && vendorTypes.Count() > 0,
                                            u => u.Journal.Bill.Vendor != null && vendorTypes.Contains(u.Journal.Bill.Vendor.VendorTypeId.Value))
                                    .WhereIf(locations != null && locations.Count() > 0,
                                          u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))

                                    .AsNoTracking()
                             select new BillAndCreditVendorOutput
                             {
                                 CreationTimeIndex = u.Journal.CreationTimeIndex,
                                 BillOrCreditId = u.Journal.BillId.Value,
                                 JournalId = u.JournalId,
                                 JournalDate = u.Journal.Date,
                                 JournalMemo = u.Journal.Memo,
                                 JournalNo = u.Journal.JournalNo,
                                 JournalType = u.Journal.JournalType,
                                 VendorId = u.Journal.Bill.VendorId,
                                 VendorName = u.Journal.Bill.Vendor.VendorName,
                                 VendorCode = u.Journal.Bill.Vendor.VendorCode,
                                 VendorTypeId = u.Journal.Bill.Vendor.VendorTypeId,
                                 Date = u.Journal.Date,

                                 Aging = (int)(toDate - u.Journal.Date).TotalDays,

                                 BillBalanceAmount = filterByAccountingCurrency ? u.Journal.Bill.OpenBalance : u.Journal.Bill.MultiCurrencyOpenBalance,
                                 BillTotalAmount = filterByAccountingCurrency ? u.Journal.Bill.Total : u.Journal.Bill.MultiCurrencyTotal,
                                 BillTotalPaidAmount = filterByAccountingCurrency ? u.Journal.Bill.TotalPaid : u.Journal.Bill.MultiCurrencyTotalPaid,

                                 CreditBalanceAmount = 0,
                                 CreditTotalAmount = 0,
                                 CreditTotalPaidAmount = 0,

                                 AccountId = u.AccountId,
                                 AccountCode = u.Account.AccountCode,
                                 AccountName = u.Account.AccountName,
                                 Currency = filterByAccountingCurrency ?
                                 new CurrencyOutput
                                 {
                                     Id = u.Journal.Currency.Id,
                                     Code = u.Journal.Currency.Code,
                                     Name = u.Journal.Currency.Name,
                                     Symbol = u.Journal.Currency.Symbol,
                                 }
                                 : new CurrencyOutput
                                 {
                                     Id = u.Journal.MultiCurrency.Id,
                                     Code = u.Journal.MultiCurrency.Code,
                                     Name = u.Journal.MultiCurrency.Name,
                                     Symbol = u.Journal.MultiCurrency.Symbol,
                                 }
                             });


            var vendorCreditQuery = (from u in _journalItemRepository.GetAll()
                                                 .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                 .Where(t => t.Identifier == null)
                                                 .Where(t => t.Journal.VendorCredit != null)
                                                 .WhereIf(accounts != null && accounts.Count() > 0,
                                                       u => accounts.Contains(u.AccountId))
                                                .WhereIf(accountType != null && accountType.Count() > 0,
                                                        u => accountType.Contains(u.Account.AccountTypeId))
                                                .WhereIf(toDate != null,
                                                 (u => ((fromDate == null) || (u.Journal.Date.Date >= fromDate.Value.Date)) &&
                                                 (u.Journal.Date.Date <= toDate.Date)))
                                                 .WhereIf(journalType != null && journalType.Count() > 0,
                                                        u => journalType.Contains(u.Journal.JournalType))
                                                .WhereIf(!filter.IsNullOrEmpty(),
                                                         u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                                .WhereIf(vendors != null && vendors.Count() > 0,
                                                        u => vendors.Contains(u.Journal.VendorCredit.VendorId))
                                                .WhereIf(vendorTypes != null && vendorTypes.Count() > 0,
                                                        u => u.Journal.VendorCredit.Vendor != null && vendorTypes.Contains(u.Journal.VendorCredit.Vendor.VendorTypeId.Value))
                                                .WhereIf(locations != null && locations.Count() > 0,
                                                 u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value))

                                                 .AsNoTracking()
                                     select new BillAndCreditVendorOutput
                                     {
                                         CreationTimeIndex = u.Journal.CreationTimeIndex,
                                         BillOrCreditId = u.Journal.VendorCreditId.Value,
                                         JournalId = u.JournalId,
                                         JournalDate = u.Journal.Date,
                                         JournalMemo = u.Journal.Memo,
                                         JournalNo = u.Journal.JournalNo,
                                         JournalType = u.Journal.JournalType,
                                         VendorId = u.Journal.VendorCredit.VendorId,
                                         VendorName = u.Journal.VendorCredit.Vendor.VendorName,
                                         VendorCode = u.Journal.VendorCredit.Vendor.VendorCode,
                                         VendorTypeId = u.Journal.VendorCredit.Vendor.VendorTypeId,
                                         Date = u.Journal.Date,

                                         Aging = (int)(toDate - u.Journal.Date).TotalDays,

                                         BillBalanceAmount = 0,
                                         BillTotalAmount = 0,
                                         BillTotalPaidAmount = 0,

                                         CreditBalanceAmount = filterByAccountingCurrency ? u.Journal.VendorCredit.OpenBalance : u.Journal.VendorCredit.MultiCurrencyOpenBalance,
                                         CreditTotalAmount = filterByAccountingCurrency ? u.Journal.VendorCredit.Total : u.Journal.VendorCredit.MultiCurrencyTotal,
                                         CreditTotalPaidAmount = filterByAccountingCurrency ? u.Journal.VendorCredit.TotalPaid : u.Journal.VendorCredit.MultiCurrencyTotalPaid,

                                         AccountId = u.AccountId,
                                         AccountCode = u.Account.AccountCode,
                                         AccountName = u.Account.AccountName,
                                         Currency = filterByAccountingCurrency ?
                                                     new CurrencyOutput
                                                     {
                                                         Id = u.Journal.Currency.Id,
                                                         Code = u.Journal.Currency.Code,
                                                         Name = u.Journal.Currency.Name,
                                                         Symbol = u.Journal.Currency.Symbol,
                                                     } :
                                                     new CurrencyOutput
                                                     {
                                                         Id = u.Journal.MultiCurrency.Id,
                                                         Code = u.Journal.MultiCurrency.Code,
                                                         Name = u.Journal.MultiCurrency.Name,
                                                         Symbol = u.Journal.MultiCurrency.Symbol,
                                                     }
                                     });


            var invoiceAndCreditByCustomerUnionQuery = billQuery.Union(vendorCreditQuery);

            return invoiceAndCreditByCustomerUnionQuery;

        }



        public IQueryable<GetVendorByBillReportOutput> GetBillAndCreditVendorBillsWithBalanceQuery(long userId,
                                  DateTime? fromDate, DateTime toDate, string filter, List<Guid> vendors,
                                  List<Guid> accounts, List<JournalType> journalType, List<long> accountType,
                                  List<long?> users, CurrencyFilter currencyFilter, List<long> locations = null,
                                  List<long> vendorTypes = null)
        {

            var filterByAccountingCurrency = currencyFilter != CurrencyFilter.MultiCurrencies;
            var userGroupByVendor = GetVendorGroup(userId);

            IQueryable<BillAndCreditVendorOutput> closeVendorTransaction = null;

            if (fromDate == null)
            {
                var periodCycles = GetCloseCyleQuery();

                var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                            u.EndDate.Value.Date <= toDate.Date)
                                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

                if (previousCycle != null)
                {
                    closeVendorTransaction = from ct in _vendorOpenBalanceRepository.GetAll()
                                                //.Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                                .Where(s => previousCycle != null && s.AccountCycleId == previousCycle.Id)
                                                .Where(s => s.Key == JournalType.Bill || s.Key == JournalType.VendorCredit)
                                                .AsNoTracking()

                                               join u in _journalItemRepository.GetAll()
                                                  .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                  .Where(s => s.Identifier == null)
                                                  .WhereIf(accounts != null && accounts.Count() > 0, u => accounts.Contains(u.AccountId))
                                                  .WhereIf(accountType != null && accountType.Count() > 0, u => accountType.Contains(u.Account.AccountTypeId))

                                                  .Where(s => s.Journal.BillId != null || s.Journal.VendorCreditId != null)
                                                  .WhereIf(journalType != null && journalType.Count() > 0, u => journalType.Contains(u.Journal.JournalType))

                                                  .WhereIf(vendorTypes != null && vendorTypes.Count() > 0, u =>
                                                       ((u.Journal.Bill == null && u.Journal.Bill.Vendor == null) || vendorTypes.Contains(u.Journal.Bill.Vendor.VendorTypeId.Value)) &&
                                                       ((u.Journal.VendorCredit == null && u.Journal.VendorCredit.Vendor == null) || vendorTypes.Contains(u.Journal.VendorCredit.Vendor.VendorTypeId.Value)))

                                                  .WhereIf(vendors != null && vendors.Count() > 0, u =>
                                                       (u.Journal.Bill == null || vendors.Contains(u.Journal.Bill.VendorId)) &&
                                                       (u.Journal.VendorCredit == null || vendors.Contains(u.Journal.VendorCredit.VendorId)))
                                                  .WhereIf(locations != null && locations.Count() > 0,
                                                       u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value)).AsNoTracking()
                                                  .AsNoTracking()

                                                  .Select(s => new
                                                  {
                                                      TransactionId = s.Journal.BillId != null ? s.Journal.BillId : s.Journal.VendorCreditId,
                                                      AccountName = s.Account.AccountName,
                                                      AccountCode = s.Account.AccountCode,
                                                      AccountId = s.AccountId,
                                                      JournalId = s.JournalId,
                                                      JournalDate = s.Journal.Date,
                                                      JournalMemo = s.Journal.Memo,
                                                      JournalNo = s.Journal.JournalNo,
                                                      JournalType = s.Journal.JournalType,
                                                      VendorId = s.Journal.BillId != null ? s.Journal.Bill.VendorId : s.Journal.VendorCredit.VendorId,
                                                      VendorName = s.Journal.BillId != null ? s.Journal.Bill.Vendor.VendorName : s.Journal.VendorCredit.Vendor.VendorName,
                                                      VendorCode = s.Journal.BillId != null ? s.Journal.Bill.Vendor.VendorCode : s.Journal.VendorCredit.Vendor.VendorCode,
                                                      Date = s.Journal.Date,
                                                      Aging = (int)(toDate - s.Journal.Date).TotalDays,
                                                      Currency = filterByAccountingCurrency ?
                                                    new CurrencyOutput
                                                    {
                                                        Id = s.Journal.Currency.Id,
                                                        Code = s.Journal.Currency.Code,
                                                        Name = s.Journal.Currency.Name,
                                                        Symbol = s.Journal.Currency.Symbol,
                                                    }
                                                    : new CurrencyOutput
                                                    {
                                                        Id = s.Journal.MultiCurrency.Id,
                                                        Code = s.Journal.MultiCurrency.Code,
                                                        Name = s.Journal.MultiCurrency.Name,
                                                        Symbol = s.Journal.MultiCurrency.Symbol,
                                                    }

                                                  })

                                               on ct.TransactionId equals u.TransactionId

                                               select new BillAndCreditVendorOutput
                                               {
                                                   // CreationTimeIndex = u.CreationTimeIndex,
                                                   BillOrCreditId = ct.TransactionId,
                                                   JournalId = u.JournalId,
                                                   JournalDate = u.Date,
                                                   JournalMemo = u.JournalMemo,
                                                   JournalNo = u.JournalNo,
                                                   JournalType = u.JournalType,
                                                   VendorId = u.VendorId,
                                                   VendorName = u.VendorName,
                                                   VendorCode = u.VendorCode,
                                                   Date = u.Date,
                                                   Aging = (int)(toDate - u.Date).TotalDays,
                                                   BillBalanceAmount = ct.Key == JournalType.VendorCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   BillTotalAmount = ct.Key == JournalType.VendorCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   BillTotalPaidAmount = 0,
                                                   CreditBalanceAmount = ct.Key == JournalType.Bill ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   CreditTotalAmount = ct.Key == JournalType.Bill ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   CreditTotalPaidAmount = 0,
                                                   AccountId = u.AccountId,
                                                   AccountCode = u.AccountCode,
                                                   AccountName = u.AccountName,
                                                   Currency = new CurrencyOutput
                                                   {
                                                       Id = u.Currency.Id,
                                                       Code = u.Currency.Code,
                                                       Name = u.Currency.Name,
                                                       Symbol = u.Currency.Symbol,
                                                   }
                                               };

                    fromDate = previousCycle.EndDate.Value.AddDays(1);
                }
            }


            var billQuery = (from u in _journalItemRepository.GetAll()
                                    .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                    .Where(t => t.Identifier == null)
                                   .Where(t => t.Journal.Bill != null)
                                   .WhereIf(accounts != null && accounts.Count() > 0,
                                         u => accounts.Contains(u.AccountId))
                                   .WhereIf(users != null && users.Count() > 0,
                                         u => users.Contains(u.Journal.CreatorUserId))
                                   .WhereIf(accountType != null && accountType.Count() > 0,
                                         u => accountType.Contains(u.Account.AccountTypeId))
                                   .WhereIf(toDate != null,
                                            (u => ((fromDate == null) || (u.Journal.Date.Date >= fromDate.Value.Date)) &&
                                            (u.Journal.Date.Date <= toDate.Date)))
                                    .WhereIf(journalType != null && journalType.Count() > 0,
                                           u => journalType.Contains(u.Journal.JournalType))
                                   .WhereIf(!filter.IsNullOrEmpty(),
                                            u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                   .WhereIf(vendors != null && vendors.Count() > 0,
                                           u => vendors.Contains(u.Journal.Bill.VendorId))
                                   .WhereIf(vendorTypes != null && vendorTypes.Count() > 0,
                                           u => u.Journal.Bill.Vendor != null && vendorTypes.Contains(u.Journal.Bill.Vendor.VendorTypeId.Value))

                                   .WhereIf(locations != null && locations.Count() > 0,
                                           u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                    .AsNoTracking()
                             join v in userGroupByVendor
                                on u.Journal.Bill.VendorId equals v
                             select new BillAndCreditVendorOutput
                             {
                                 CreationTimeIndex = u.Journal.CreationTimeIndex,
                                 BillOrCreditId = u.Journal.BillId.Value,
                                 JournalId = u.JournalId,
                                 JournalDate = u.Journal.Date,
                                 JournalMemo = u.Journal.Memo,
                                 JournalNo = u.Journal.JournalNo,
                                 Reference = u.Journal.Reference,
                                 JournalType = u.Journal.JournalType,
                                 VendorId = u.Journal.Bill.VendorId,
                                 VendorName = u.Journal.Bill.Vendor.VendorName,
                                 VendorCode = u.Journal.Bill.Vendor.VendorCode,
                                 VendorTypeId = u.Journal.Bill.Vendor.VendorTypeId,
                                 Date = u.Journal.Date,

                                 BillBalanceAmount = filterByAccountingCurrency ? u.Journal.Bill.OpenBalance : u.Journal.Bill.MultiCurrencyOpenBalance,
                                 BillTotalAmount = filterByAccountingCurrency ? u.Journal.Bill.Total : u.Journal.Bill.MultiCurrencyTotal,
                                 BillTotalPaidAmount = filterByAccountingCurrency ? u.Journal.Bill.TotalPaid : u.Journal.Bill.MultiCurrencyTotalPaid,

                                 LocationName = u.Journal.Location.LocationName,
                                 CreditBalanceAmount = 0,
                                 CreditTotalAmount = 0,
                                 CreditTotalPaidAmount = 0,
                                 User = ObjectMapper.Map<UserDto>(u.Journal.CreatorUser),
                                 AccountId = u.AccountId,
                                 AccountCode = u.Account.AccountCode,
                                 AccountName = u.Account.AccountName,

                                 Currency = filterByAccountingCurrency ?
                                     new CurrencyOutput
                                     {
                                         Id = u.Journal.Currency.Id,
                                         Code = u.Journal.Currency.Code,
                                         Name = u.Journal.Currency.Name,
                                         Symbol = u.Journal.Currency.Symbol
                                     } :
                                     new CurrencyOutput
                                     {
                                         Id = u.Journal.MultiCurrency.Id,
                                         Code = u.Journal.MultiCurrency.Code,
                                         Name = u.Journal.MultiCurrency.Name,
                                         Symbol = u.Journal.MultiCurrency.Symbol
                                     }
                             });


            var vendorCreditQuery = (from u in _journalItemRepository.GetAll()
                                        .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                        .Where(t => t.Identifier == null)
                                        .Where(t => t.Journal.VendorCredit != null)
                                        .WhereIf(accounts != null && accounts.Count() > 0,
                                            u => accounts.Contains(u.AccountId))
                                        .WhereIf(users != null && users.Count() > 0,
                                            u => users.Contains(u.Journal.CreatorUserId))
                                        .WhereIf(accountType != null && accountType.Count() > 0,
                                            u => accountType.Contains(u.Account.AccountTypeId))
                                        .WhereIf(toDate != null,
                                            (u => ((fromDate == null) || (u.Journal.Date.Date >= fromDate.Value.Date)) &&
                                            (u.Journal.Date.Date <= toDate.Date)))
                                            .WhereIf(journalType != null && journalType.Count() > 0,
                                                u => journalType.Contains(u.Journal.JournalType))
                                        .WhereIf(!filter.IsNullOrEmpty(),
                                                    u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                        .WhereIf(vendors != null && vendors.Count() > 0,
                                            u => vendors.Contains(u.Journal.VendorCredit.VendorId))
                                        .WhereIf(vendorTypes != null && vendorTypes.Count() > 0,
                                           u => u.Journal.VendorCredit.Vendor != null && vendorTypes.Contains(u.Journal.VendorCredit.Vendor.VendorTypeId.Value))

                                        .WhereIf(locations != null && locations.Count() > 0,
                                           u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                        .AsNoTracking()
                                     join v in userGroupByVendor
                                     on u.Journal.VendorCredit.VendorId equals v
                                     select new BillAndCreditVendorOutput
                                     {
                                         CreationTimeIndex = u.Journal.CreationTimeIndex,
                                         BillOrCreditId = u.Journal.VendorCreditId.Value,
                                         JournalId = u.JournalId,
                                         JournalDate = u.Journal.Date,
                                         JournalMemo = u.Journal.Memo,
                                         JournalNo = u.Journal.JournalNo,
                                         Reference = u.Journal.Reference,
                                         JournalType = u.Journal.JournalType,
                                         VendorId = u.Journal.VendorCredit.VendorId,
                                         VendorName = u.Journal.VendorCredit.Vendor.VendorName,
                                         VendorCode = u.Journal.VendorCredit.Vendor.VendorCode,
                                         VendorTypeId = u.Journal.VendorCredit.Vendor.VendorTypeId,
                                         Date = u.Journal.Date,
                                         BillBalanceAmount = 0,
                                         BillTotalAmount = 0,
                                         BillTotalPaidAmount = 0,
                                         User = ObjectMapper.Map<UserDto>(u.Journal.CreatorUser),

                                         CreditBalanceAmount = filterByAccountingCurrency ? u.Journal.VendorCredit.OpenBalance : u.Journal.VendorCredit.MultiCurrencyOpenBalance,
                                         CreditTotalAmount = filterByAccountingCurrency ? u.Journal.VendorCredit.Total : u.Journal.VendorCredit.MultiCurrencyTotal,
                                         CreditTotalPaidAmount = filterByAccountingCurrency ? u.Journal.VendorCredit.TotalPaid : u.Journal.VendorCredit.MultiCurrencyTotalPaid,

                                         AccountId = u.AccountId,
                                         AccountCode = u.Account.AccountCode,
                                         AccountName = u.Account.AccountName,
                                         LocationName = u.Journal.Location.LocationName,
                                         Currency = filterByAccountingCurrency ?
                                              new CurrencyOutput
                                              {
                                                  Id = u.Journal.Currency.Id,
                                                  Code = u.Journal.Currency.Code,
                                                  Name = u.Journal.Currency.Name,
                                                  Symbol = u.Journal.Currency.Symbol
                                              } :
                                              new CurrencyOutput
                                              {
                                                  Id = u.Journal.MultiCurrency.Id,
                                                  Code = u.Journal.MultiCurrency.Code,
                                                  Name = u.Journal.MultiCurrency.Name,
                                                  Symbol = u.Journal.MultiCurrency.Symbol
                                              }
                                     });


            var invoiceAndCreditByCustomerConcatQuery = billQuery.Concat(vendorCreditQuery);

            if(closeVendorTransaction != null) invoiceAndCreditByCustomerConcatQuery = closeVendorTransaction.Concat(invoiceAndCreditByCustomerConcatQuery);

            var payBillByVendorQuerys = (from pbi in _paybillItemRepository.GetAll().AsNoTracking()
                                         join j in _journalRepository.GetAll()
                                         .Where(u => u.PayBillId != null)
                                         .WhereIf(locations != null && locations.Count() > 0,
                                             u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                         .WhereIf(toDate != null,
                                             (u => (fromDate == null || u.Date.Date >= fromDate.Value.Date) &&
                                                                         (u.Date.Date <= toDate.Date)))
                                         .AsNoTracking()
                                         on pbi.PayBillId equals j.PayBillId
                                         orderby j.Date descending
                                         select new BillPaymentDetailOutput
                                         {
                                             CreationTimeIndex = j.CreationTimeIndex,
                                             VendorId = pbi.VendorId,
                                             PaymentDate = j.Date,
                                             BillId = pbi.BillId != null ? pbi.BillId.Value : pbi.VendorCreditId.Value,
                                             //PaymentAmount = filterByAccountingCurrency ? pbi.Payment : pbi.MultiCurrencyPayment,
                                             PaymentAmount = filterByAccountingCurrency ? (pbi.BillId != null ? pbi.Payment : pbi.Payment * -1) : (pbi.BillId != null ? pbi.MultiCurrencyPayment : pbi.MultiCurrencyPayment * -1),

                                             //TransactionId = pbi.BillId != null ? pbi.BillId.Value : pbi.VendorCreditId.Value,
                                         });

            //var payBillByVendorQuery = payBillByVendorQuerys.GroupBy(s => s.BillId).Select(s => s.FirstOrDefault());


            var finalQuery = (from i in invoiceAndCreditByCustomerConcatQuery
                              join p in payBillByVendorQuerys
                              on i.BillOrCreditId equals p.BillId
                              into ip
                              from p in ip.DefaultIfEmpty()
                              orderby i.Date ascending
                              select new GetVendorByBillReportOutput
                              {
                                  CreationTimeIndex = i.CreationTimeIndex,
                                  User = i.User,
                                  TransactionId = i.BillOrCreditId,
                                  VendorId = i.VendorId,
                                  VendorName = i.VendorName,
                                  VendorCode = i.VendorCode,
                                  VendorTypeId = i.VendorTypeId,
                                  LastPaymentAmounts = p != null ? p.PaymentAmount : 0,
                                  LastPaymentDate = p != null ? p.PaymentDate : (DateTime?)null,
                                  Date = i.JournalDate,
                                  AccountCode = i.AccountCode,
                                  AccountName = i.AccountName,
                                  Balance = i.BillBalanceAmount != 0 ? i.BillBalanceAmount : (i.CreditBalanceAmount * -1),
                                  Description = i.JournalMemo,
                                  JournalNo = i.JournalNo,
                                  Reference = i.Reference,
                                  JournalType = i.JournalType,
                                  ToDate = toDate,
                                  TotalAmount = i.BillTotalAmount != 0 ? i.BillTotalAmount : (i.CreditTotalAmount * -1),
                                  TotalPaid = p != null ? p.PaymentAmount : 0, //i.BillTotalPaidAmount != 0 ? i.BillTotalPaidAmount : (i.CreditTotalPaidAmount * -1),
                                  Location = i.LocationName,
                                  Currency = i.Currency,
                              })
                             .GroupBy(t => t.TransactionId)
                             .Select(i => new GetVendorByBillReportOutput
                             {
                                 CreationTimeIndex = i.Select(t => t.CreationTimeIndex).FirstOrDefault(),
                                 User = i.Select(t => t.User).FirstOrDefault(),
                                 TransactionId = i.Select(t => t.TransactionId).FirstOrDefault(),
                                 VendorId = i.Select(t => t.VendorId).FirstOrDefault(),
                                 VendorName = i.Select(t => t.VendorName).FirstOrDefault(),
                                 VendorCode = i.Select(t => t.VendorCode).FirstOrDefault(),
                                 VendorTypeId = i.Select(t => t.VendorTypeId).FirstOrDefault(),
                                 LastPaymentAmounts = i.Select(t => t.LastPaymentAmounts).FirstOrDefault(),
                                 LastPaymentDate = i.Select(t => t.LastPaymentDate).FirstOrDefault(),
                                 Date = i.Select(t => t.Date).FirstOrDefault(),
                                 AccountCode = i.Select(t => t.AccountCode).FirstOrDefault(),
                                 AccountName = i.Select(t => t.AccountName).FirstOrDefault(),
                                 Balance = i.Select(t => t.TotalAmount).FirstOrDefault() - i.Sum(t => t.TotalPaid),//  i.Select(t => t.Balance).FirstOrDefault(),
                                 Description = i.Select(t => t.Description).FirstOrDefault(),
                                 JournalNo = i.Select(t => t.JournalNo).FirstOrDefault(),
                                 Reference = i.Select(t => t.Reference).FirstOrDefault(),
                                 JournalType = i.Select(t => t.JournalType).FirstOrDefault(),
                                 ToDate = toDate,
                                 TotalAmount = i.Select(t => t.TotalAmount).FirstOrDefault(),
                                 TotalPaid = i.Sum(t => t.TotalPaid),
                                 Location = i.Select(t => t.Location).FirstOrDefault(),
                                 Currency = i.Select(t => t.Currency).FirstOrDefault()
                             });

            return finalQuery;

        }

       

    }
}
