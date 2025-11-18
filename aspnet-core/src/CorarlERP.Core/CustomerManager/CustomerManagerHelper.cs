using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using CorarlERP.AccountCycles;
using CorarlERP.Customers;
using CorarlERP.CustomerTypes;
using CorarlERP.enumStatus;
using CorarlERP.Journals;
using CorarlERP.ReceivePayments;
using CorarlERP.UserGroups;
using CorarlERP.VendorCustomerOpenBalances;
using CorarlERP.VendorHelpers.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerManager
{
    public class CustomerManagerHelper : CorarlERPDomainServiceBase, ICustomerManagerHelper
    {

        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<ReceivePaymentDetail, Guid> _receivePaymentItemRepository;

        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<CustomerGroup, Guid> _customerGroupRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IRepository<CustomerOpenBalance, Guid> _customerOpenBalanceRepository;
        public CustomerManagerHelper(
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<CustomerOpenBalance, Guid> customerOpenBalanceRepository,
            IRepository<Customer, Guid> customerRepository,
            IRepository<CustomerGroup, Guid> customerGroupRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<ReceivePaymentDetail, Guid> receivePaymentItemRepository
        ): base (accountCycleRepository)
        {
            _userGroupMemberRepository = userGroupMemberRepository;
            _customerGroupRepository = customerGroupRepository;
            _customerRepository = customerRepository;
            _journalItemRepository = journalItemRepository;
            _journalRepository = journalRepository;
            _receivePaymentItemRepository = receivePaymentItemRepository;
            _customerOpenBalanceRepository = customerOpenBalanceRepository;
        }

        public IQueryable<InvoiceAndCreditCustomerOutput> GetInvoiceAndCreditCustomerQueryAsyn(
                                   DateTime fromDate, DateTime toDate, string filter, List<Guid> customers,
                                   List<Guid> accounts, List<JournalType> journalType, List<long> accountType, CurrencyFilter currencyFilter, List<long> locations = null, List<long> customerTypes = null)
        {


            var filterByAccountingCurrency = currencyFilter != CurrencyFilter.MultiCurrencies;

            var invoiceQuery = (from u in _journalItemRepository.GetAll()
                                                .Where(t => t.Identifier == null)
                                                .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                .Where(t => t.Journal.InvoiceId != null)
                                                .WhereIf(accounts != null && accounts.Count() > 0,
                                                      u => accounts.Contains(u.AccountId))
                                                .WhereIf(accountType != null && accountType.Count() > 0,
                                                      u => accountType.Contains(u.Account.AccountTypeId))
                                                .WhereIf(fromDate != null && toDate != null,
                                                     (u => (u.Journal.Date.Date) >= (fromDate.Date) &&
                                                     (u.Journal.Date.Date) <= (toDate.Date)))
                                                 .WhereIf(journalType != null && journalType.Count() > 0,
                                                        u => journalType.Contains(u.Journal.JournalType))
                                                .WhereIf(!filter.IsNullOrEmpty(),
                                                         u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                                .WhereIf(customers != null && customers.Count() > 0,
                                                        u => customers.Contains(u.Journal.Invoice.CustomerId))
                                                .WhereIf(customerTypes != null && customerTypes.Count() > 0,
                                                        u => u.Journal.Invoice.Customer != null && customerTypes.Contains(u.Journal.Invoice.Customer.CustomerTypeId.Value))
                                                .WhereIf(locations != null && locations.Count() > 0,
                                                        u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                                .AsNoTracking()
                                select new InvoiceAndCreditCustomerOutput
                                {
                                    CreationTimeIndex = u.Journal.CreationTimeIndex,
                                    InvoiceOrCreditId = u.Journal.InvoiceId.Value,
                                    JournalId = u.JournalId,
                                    JournalDate = u.Journal.Date,
                                    JournalMemo = u.Journal.Memo,
                                    JournalNo = u.Journal.JournalNo,
                                    JournalType = u.Journal.JournalType,
                                    CustomerId = u.Journal.Invoice.CustomerId,
                                    CustomerName = u.Journal.Invoice.Customer.CustomerName,
                                    CustomerCode = u.Journal.Invoice.Customer.CustomerCode,
                                    Date = u.Journal.Date,

                                    Aging = (int)(toDate - u.Journal.Date).TotalDays,

                                    InvoiceBalanceAmount = filterByAccountingCurrency ? u.Journal.Invoice.OpenBalance : u.Journal.Invoice.MultiCurrencyOpenBalance,
                                    InvoiceTotalAmount = filterByAccountingCurrency ? u.Journal.Invoice.Total : u.Journal.Invoice.MultiCurrencyTotal,
                                    InvoiceTotalPaidAmount = filterByAccountingCurrency ? u.Journal.Invoice.TotalPaid : u.Journal.Invoice.MultiCurrencyTotalPaid,

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
                                    } :
                                    u.Journal.MultiCurrency == null ? null
                                    : new CurrencyOutput
                                    {
                                        Id = u.Journal.MultiCurrency.Id,
                                        Code = u.Journal.MultiCurrency.Code,
                                        Name = u.Journal.MultiCurrency.Name,
                                        Symbol = u.Journal.MultiCurrency.Symbol,
                                    }

                                });


            var customerCreditQuery = _journalItemRepository.GetAll()
                                                 .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                 .Where(t => t.Identifier == null)
                                                 .Where(t => t.Journal.CustomerCreditId != null)
                                                 .WhereIf(accounts != null && accounts.Count() > 0,
                                                       u => accounts.Contains(u.AccountId))
                                                .WhereIf(accountType != null && accountType.Count() > 0,
                                                       u => accountType.Contains(u.Account.AccountTypeId))
                                                .WhereIf(fromDate != null && toDate != null,
                                                     (u => (u.Journal.Date.Date) >= (fromDate.Date) &&
                                                     (u.Journal.Date.Date) <= (toDate.Date)))
                                                 .WhereIf(journalType != null && journalType.Count() > 0,
                                                        u => journalType.Contains(u.Journal.JournalType))
                                                .WhereIf(!filter.IsNullOrEmpty(),
                                                        u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                                .WhereIf(customers != null && customers.Count() > 0,
                                                        u => customers.Contains(u.Journal.CustomerCredit.CustomerId))
                                                .WhereIf(customerTypes != null && customerTypes.Count() > 0,
                                                        u => u.Journal.CustomerCredit.Customer != null && customerTypes.Contains(u.Journal.CustomerCredit.Customer.CustomerTypeId.Value))
                                                .WhereIf(locations != null && locations.Count() > 0,
                                                        u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                                .AsNoTracking()
                                                 .Select(u => new InvoiceAndCreditCustomerOutput
                                                 {
                                                     CreationTimeIndex = u.Journal.CreationTimeIndex,
                                                     InvoiceOrCreditId = u.Journal.CustomerCreditId.Value,
                                                     JournalId = u.JournalId,
                                                     JournalDate = u.Journal.Date,
                                                     JournalMemo = u.Journal.Memo,
                                                     JournalNo = u.Journal.JournalNo,
                                                     JournalType = u.Journal.JournalType,
                                                     CustomerId = u.Journal.CustomerCredit.CustomerId,
                                                     CustomerName = u.Journal.CustomerCredit.Customer.CustomerName,
                                                     CustomerCode = u.Journal.CustomerCredit.Customer.CustomerCode,
                                                     Date = u.Journal.Date,

                                                     Aging = (int)(toDate - u.Journal.Date).TotalDays,

                                                     InvoiceBalanceAmount = 0,
                                                     InvoiceTotalAmount = 0,
                                                     InvoiceTotalPaidAmount = 0,

                                                     CreditBalanceAmount = filterByAccountingCurrency ? u.Journal.CustomerCredit.OpenBalance : u.Journal.CustomerCredit.MultiCurrencyOpenBalance,
                                                     CreditTotalAmount = filterByAccountingCurrency ? u.Journal.CustomerCredit.Total : u.Journal.CustomerCredit.MultiCurrencyTotal,
                                                     CreditTotalPaidAmount = filterByAccountingCurrency ? u.Journal.CustomerCredit.TotalPaid : u.Journal.CustomerCredit.MultiCurrencyTotalPaid,

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
                                                    u.Journal.MultiCurrency == null ? null
                                                    : new CurrencyOutput
                                                    {
                                                        Id = u.Journal.MultiCurrency.Id,
                                                        Code = u.Journal.MultiCurrency.Code,
                                                        Name = u.Journal.MultiCurrency.Name,
                                                        Symbol = u.Journal.MultiCurrency.Symbol,
                                                    }
                                                 });


            var invoiceAndCreditByCustomerUnionQuery = invoiceQuery.Concat(customerCreditQuery);

            return invoiceAndCreditByCustomerUnionQuery;

        }

        private IQueryable<Guid> GetCustomerGroup(long userId)
        {
            // get user by group member
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == userId)
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToList();

            var @query = _customerGroupRepository.GetAll()
                            .Where(t => t.Customer.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            //.Where(p => p.Customer.IsActive == true)
                            .AsNoTracking()
                            .Select(t => t.Customer);

            var @queryAll = _customerRepository.GetAll()
                            .Where(t => t.Member == Member.All)
                            //.Where(p => p.IsActive == true)
                            .AsNoTracking();

            var result = @queryAll.Union(query)
                        .Select(t => t.Id);
            return result;
        }

        public IQueryable<GetCustomerByInvoiceReportOutput> GetInvoiceAndCreditCustomerWithBalanceQuery(long userId,
                                  DateTime? fromDate, DateTime toDate, string filter, List<Guid> customers,
                                  List<Guid> accounts, List<JournalType> journalType, List<long> accountType,
                                  List<long?> users, CurrencyFilter currencyFilter,
                                  List<long> locations = null, List<long> customerTyes = null)
        {
            var filterByAccountingCurrency = currencyFilter != CurrencyFilter.MultiCurrencies;
            var userGroupByVendor = GetCustomerGroup(userId);

            IQueryable< InvoiceAndCreditCustomerOutput> closeCustomerTransaction = null;

            if (fromDate == null)
            {
                var periodCycles = GetCloseCyleQuery();

                var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                            u.EndDate.Value.Date <= toDate.Date)
                                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

                if(previousCycle != null)
                {
                    closeCustomerTransaction = from ct in _customerOpenBalanceRepository.GetAll()
                                                //.Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                                .Where(s => previousCycle != null && s.AccountCycleId == previousCycle.Id)
                                                .Where(s => s.Key == JournalType.Invoice || s.Key == JournalType.CustomerCredit)
                                                .AsNoTracking()

                                               join u in _journalItemRepository.GetAll()
                                                  .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                                  .Where(s => s.Identifier == null)
                                                  .WhereIf(accounts != null && accounts.Count() > 0, u => accounts.Contains(u.AccountId))
                                                  .WhereIf(accountType != null && accountType.Count() > 0, u => accountType.Contains(u.Account.AccountTypeId))

                                                  .Where(s => s.Journal.InvoiceId != null || s.Journal.CustomerCreditId != null)
                                                  .WhereIf(journalType != null && journalType.Count() > 0, u => journalType.Contains(u.Journal.JournalType))

                                                  .WhereIf(customerTyes != null && customerTyes.Count() > 0, u =>
                                                       ((u.Journal.Invoice == null && u.Journal.Invoice.Customer == null) || customerTyes.Contains(u.Journal.Invoice.Customer.CustomerTypeId.Value)) &&
                                                       ((u.Journal.CustomerCredit == null && u.Journal.CustomerCredit.Customer == null) || customerTyes.Contains(u.Journal.CustomerCredit.Customer.CustomerTypeId.Value)))

                                                  .WhereIf(customers != null && customers.Count() > 0, u =>
                                                       (u.Journal.Invoice == null || customers.Contains(u.Journal.Invoice.CustomerId)) &&
                                                       (u.Journal.CustomerCredit == null || customers.Contains(u.Journal.CustomerCredit.CustomerId)))
                                                  .WhereIf(locations != null && locations.Count() > 0,
                                                       u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value)).AsNoTracking()
                                                  .AsNoTracking()

                                                  .Select(s => new
                                                  {
                                                      TransactionId = s.Journal.InvoiceId != null ? s.Journal.InvoiceId : s.Journal.CustomerCreditId,
                                                      AccountName = s.Account.AccountName,
                                                      AccountCode = s.Account.AccountCode,
                                                      AccountId = s.AccountId,
                                                      InvoiceOrCreditId = s.Journal.InvoiceId != null ? s.Journal.InvoiceId.Value : s.Journal.CustomerCreditId.Value,
                                                      JournalId = s.JournalId,
                                                      JournalDate = s.Journal.Date,
                                                      JournalMemo = s.Journal.Memo,
                                                      JournalNo = s.Journal.JournalNo,
                                                      JournalType = s.Journal.JournalType,
                                                      CustomerId = s.Journal.InvoiceId != null ? s.Journal.Invoice.CustomerId : s.Journal.CustomerCredit.CustomerId,
                                                      CustomerName = s.Journal.InvoiceId != null ? s.Journal.Invoice.Customer.CustomerName : s.Journal.CustomerCredit.Customer.CustomerName,
                                                      CustomerCode = s.Journal.InvoiceId != null ? s.Journal.Invoice.Customer.CustomerCode : s.Journal.CustomerCredit.Customer.CustomerCode,
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

                                               select new InvoiceAndCreditCustomerOutput
                                               {
                                                   // CreationTimeIndex = u.CreationTimeIndex,
                                                   InvoiceOrCreditId = ct.TransactionId,
                                                   JournalId = u.JournalId,
                                                   JournalDate = u.Date,
                                                   JournalMemo = u.JournalMemo,
                                                   JournalNo = u.JournalNo,
                                                   JournalType = u.JournalType,
                                                   CustomerId = u.CustomerId,
                                                   CustomerName = u.CustomerName,
                                                   CustomerCode = u.CustomerCode,
                                                   Date = u.Date,
                                                   Aging = (int)(toDate - u.Date).TotalDays,
                                                   InvoiceBalanceAmount = ct.Key == JournalType.CustomerCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   InvoiceTotalAmount = ct.Key == JournalType.CustomerCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   InvoiceTotalPaidAmount = 0,
                                                   CreditBalanceAmount = ct.Key == JournalType.Invoice ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                                   CreditTotalAmount = ct.Key == JournalType.Invoice ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
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

            var invoiceQuery = (from u in _journalItemRepository.GetAll()
                                    .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                    .Where(t => t.Identifier == null)
                                    .Where(t => t.Journal.Invoice != null)
                                    .WhereIf(accounts != null && accounts.Count() > 0,
                                          u => accounts.Contains(u.AccountId))
                                    .WhereIf(users != null && users.Count() > 0,
                                          u => users.Contains(u.CreatorUserId))
                                    .WhereIf(accountType != null && accountType.Count() > 0,
                                          u => accountType.Contains(u.Account.AccountTypeId))
                                     .WhereIf(toDate != null,
                                        (u => ((fromDate == null) || (u.Journal.Date.Date >= fromDate.Value.Date)) &&
                                        (u.Journal.Date.Date <= toDate.Date)))
                                     .WhereIf(journalType != null && journalType.Count() > 0,
                                            u => journalType.Contains(u.Journal.JournalType))
                                    .WhereIf(!filter.IsNullOrEmpty(),
                                             u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                    .WhereIf(customers != null && customers.Count() > 0,
                                            u => customers.Contains(u.Journal.Invoice.CustomerId))
                                    .WhereIf(customerTyes != null && customerTyes.Count() > 0,
                                            u => u.Journal.Invoice.Customer != null && customerTyes.Contains(u.Journal.Invoice.Customer.CustomerTypeId.Value))
                                    .WhereIf(locations != null && locations.Count() > 0,
                                            u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                    .AsNoTracking()
                                join c in userGroupByVendor
                                    on u.Journal.Invoice.CustomerId equals c
                                select new InvoiceAndCreditCustomerOutput
                                {
                                    CreationTimeIndex = u.Journal.CreationTimeIndex,
                                    InvoiceOrCreditId = u.Journal.InvoiceId.Value,
                                    JournalId = u.JournalId,
                                    JournalDate = u.Journal.Date,
                                    JournalMemo = u.Journal.Memo,
                                    JournalNo = u.Journal.JournalNo,
                                    Reference = u.Journal.Reference,
                                    JournalType = u.Journal.JournalType,
                                    CustomerId = u.Journal.Invoice.CustomerId,
                                    CustomerName = u.Journal.Invoice.Customer.CustomerName,
                                    CustomerCode = u.Journal.Invoice.Customer.CustomerCode,
                                    Date = u.Journal.Date,
                                    LocationName = u.Journal.Location.LocationName,
                                    InvoiceBalanceAmount = filterByAccountingCurrency ? u.Journal.Invoice.OpenBalance : u.Journal.Invoice.MultiCurrencyOpenBalance,
                                    InvoiceTotalAmount = filterByAccountingCurrency ? u.Journal.Invoice.Total : u.Journal.Invoice.MultiCurrencyTotal,
                                    InvoiceTotalPaidAmount = filterByAccountingCurrency ? u.Journal.Invoice.TotalPaid : u.Journal.Invoice.MultiCurrencyTotalPaid,

                                    User = u.Journal.CreatorUser.UserName,
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
                                         Symbol = u.Journal.Currency.Symbol
                                     } :
                                     u.Journal.MultiCurrency == null ? null :
                                     new CurrencyOutput
                                     {
                                         Id = u.Journal.MultiCurrency.Id,
                                         Code = u.Journal.MultiCurrency.Code,
                                         Name = u.Journal.MultiCurrency.Name,
                                         Symbol = u.Journal.MultiCurrency.Symbol
                                     }
                                });


            var customerCreditQuery = (from u in _journalItemRepository.GetAll()
                                         .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                         .Where(t => t.Identifier == null)
                                         .Where(t => t.Journal.CustomerCredit != null)
                                         .WhereIf(users != null && users.Count() > 0,
                                           u => users.Contains(u.Journal.CreatorUserId))
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
                                         .WhereIf(customers != null && customers.Count() > 0,
                                                 u => customers.Contains(u.Journal.CustomerCredit.CustomerId))
                                          .WhereIf(customerTyes != null && customerTyes.Count() > 0,
                                             u => u.Journal.CustomerCredit.Customer != null && customerTyes.Contains(u.Journal.CustomerCredit.Customer.CustomerTypeId.Value))
                                         .WhereIf(locations != null && locations.Count() > 0,
                                             u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                         .AsNoTracking()
                                       join c in userGroupByVendor
                                       on u.Journal.CustomerCredit.CustomerId equals c
                                       select new InvoiceAndCreditCustomerOutput
                                       {
                                           CreationTimeIndex = u.Journal.CreationTimeIndex,
                                           InvoiceOrCreditId = u.Journal.CustomerCreditId.Value,
                                           JournalId = u.JournalId,
                                           JournalDate = u.Journal.Date,
                                           JournalMemo = u.Journal.Memo,
                                           JournalNo = u.Journal.JournalNo,
                                           Reference = u.Journal.Reference,
                                           JournalType = u.Journal.JournalType,
                                           CustomerId = u.Journal.CustomerCredit.CustomerId,
                                           CustomerName = u.Journal.CustomerCredit.Customer.CustomerName,
                                           CustomerCode = u.Journal.CustomerCredit.Customer.CustomerCode,
                                           Date = u.Journal.Date,
                                           InvoiceBalanceAmount = 0,
                                           InvoiceTotalAmount = 0,
                                           InvoiceTotalPaidAmount = 0,
                                           User = u.Journal.CreatorUser.UserName,
                                           LocationName = u.Journal.Location.LocationName,
                                           CreditBalanceAmount = filterByAccountingCurrency ? u.Journal.CustomerCredit.OpenBalance : u.Journal.CustomerCredit.MultiCurrencyOpenBalance,
                                           CreditTotalAmount = filterByAccountingCurrency ? u.Journal.CustomerCredit.Total : u.Journal.CustomerCredit.MultiCurrencyTotal,
                                           CreditTotalPaidAmount = filterByAccountingCurrency ? u.Journal.CustomerCredit.TotalPaid : u.Journal.CustomerCredit.MultiCurrencyTotalPaid,

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
                                                u.Journal.MultiCurrency == null ? null :
                                                new CurrencyOutput
                                                {
                                                    Id = u.Journal.MultiCurrency.Id,
                                                    Code = u.Journal.MultiCurrency.Code,
                                                    Name = u.Journal.MultiCurrency.Name,
                                                    Symbol = u.Journal.MultiCurrency.Symbol
                                                }
                                       });


            var invoiceAndCreditByCustomerConcatQuery = invoiceQuery.Concat(customerCreditQuery);

            if(closeCustomerTransaction != null) invoiceAndCreditByCustomerConcatQuery = closeCustomerTransaction.Concat(invoiceAndCreditByCustomerConcatQuery);


            var receivePaymentByCustomerQuerys = (from rpi in _receivePaymentItemRepository.GetAll()
                                                      .WhereIf(customers != null && customers.Count() > 0,
                                                                u => customers.Contains(u.CustomerId))
                                                      .AsNoTracking()
                                                  join j in _journalRepository.GetAll()
                                                          .Where(s => s.ReceivePaymentId.HasValue)
                                                          .WhereIf(locations != null && locations.Count() > 0,
                                                              u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                                          .WhereIf(toDate != null,
                                                              (u => (fromDate == null || u.Date.Date >= fromDate.Value.Date) &&
                                                                                          (u.Date.Date <= toDate.Date)))

                                                          .AsNoTracking()
                                                  on rpi.ReceivePaymentId equals j.ReceivePaymentId
                                                  orderby j.Date descending
                                                  select new ReceivePaymentDetailOutput
                                                  {
                                                      CreationTimeIndex = j.CreationTimeIndex,
                                                      CustomerId = rpi.CustomerId,
                                                      PaymentDate = j.Date,
                                                      InvoiceId = rpi.InvoiceId != null ? rpi.InvoiceId.Value : rpi.CustomerCreditId.Value,
                                                      PaymentAmount = filterByAccountingCurrency ? (rpi.InvoiceId != null ? rpi.Payment : rpi.Payment * -1) : (rpi.InvoiceId != null ? rpi.MultiCurrencyPayment : rpi.MultiCurrencyPayment * -1),
                                                      //TransactionId = rpi.InvoiceId != null ? rpi.InvoiceId.Value : rpi.CustomerCreditId.Value,
                                                  });

            //var receivePaymentByCustomerQuery = receivePaymentByCustomerQuerys.GroupBy(s => s.InvoiceId).Select(s => s.FirstOrDefault());

            var finalQuery = (from i in invoiceAndCreditByCustomerConcatQuery
                              join r in receivePaymentByCustomerQuerys
                              on i.InvoiceOrCreditId equals r.InvoiceId into ip
                              from p in ip.DefaultIfEmpty()
                              orderby i.Date ascending
                              select new GetCustomerByInvoiceReportOutput
                              {
                                  CreationTimeIndex = i.CreationTimeIndex,
                                  User = i.User,
                                  TransactionId = i.InvoiceOrCreditId,
                                  CustomerId = i.CustomerId,
                                  CustomerName = i.CustomerName,
                                  CustomerCode = i.CustomerCode,
                                  LastPaymentAmounts = p != null ? p.PaymentAmount : 0,
                                  LastPaymentDate = p != null ? p.PaymentDate : (DateTime?)null,
                                  Date = i.JournalDate,
                                  AccountCode = i.AccountCode,
                                  AccountName = i.AccountName,
                                  Balance = i.InvoiceBalanceAmount != 0 ? i.InvoiceBalanceAmount : (i.CreditBalanceAmount * -1),
                                  Description = i.JournalMemo,
                                  JournalNo = i.JournalNo,
                                  Reference = i.Reference,
                                  JournalType = i.JournalType,
                                  ToDate = toDate,
                                  TotalAmount = i.InvoiceTotalAmount != 0 ? i.InvoiceTotalAmount : (i.CreditTotalAmount * -1),
                                  TotalPaid = p != null ? p.PaymentAmount : 0, //i.InvoiceTotalPaidAmount != 0 ? i.InvoiceTotalPaidAmount : (i.CreditTotalPaidAmount * -1),
                                  Location = i.LocationName,
                                  Currency = i.Currency
                              })
                             .GroupBy(t => t.TransactionId)
                             .Select(i => new GetCustomerByInvoiceReportOutput
                             {
                                 CreationTimeIndex = i.Select(t => t.CreationTimeIndex).FirstOrDefault(),
                                 User = i.Select(t => t.User).FirstOrDefault(),
                                 TransactionId = i.Select(t => t.TransactionId).FirstOrDefault(),
                                 CustomerId = i.Select(t => t.CustomerId).FirstOrDefault(),
                                 CustomerName = i.Select(t => t.CustomerName).FirstOrDefault(),
                                 CustomerCode = i.Select(t => t.CustomerCode).FirstOrDefault(),
                                 LastPaymentAmounts = i.Select(t => t.LastPaymentAmounts).FirstOrDefault(),
                                 LastPaymentDate = i.Select(t => t.LastPaymentDate).FirstOrDefault(),
                                 Date = i.Select(t => t.Date).FirstOrDefault(),
                                 AccountCode = i.Select(t => t.AccountCode).FirstOrDefault(),
                                 AccountName = i.Select(t => t.AccountName).FirstOrDefault(),
                                 Balance = i.Select(t => t.TotalAmount).FirstOrDefault() - i.Sum(t => t.TotalPaid),// i.Select(t => t.Balance).FirstOrDefault(),
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


        public IQueryable<GetCustomerByInvoiceWithPaymentReportOutput> GetInvoiceAndCreditBalanceWithPaymentQuery(long userId,
                                 DateTime fromDate, DateTime toDate, string filter, List<Guid> customers,
                                 List<Guid> accounts, List<JournalType> journalType, List<long> accountType,
                                 List<long?> users, CurrencyFilter currencyFilter,
                                 List<long> locations = null, List<long> customerTyes = null)
        {
            var filterByAccountingCurrency = currencyFilter != CurrencyFilter.MultiCurrencies;
            var userGroupByVendor = GetCustomerGroup(userId);

           
            //IQueryable<GetCustomerByInvoiceWithPaymentReportOutput> closeCustomerTransaction = null;

            //if (fromDate == null)
            //{
            //    var periodCycles = GetCloseCyleQuery();

            //    var previousCycle = periodCycles.Where(u => u.EndDate != null &&
            //                                                u.EndDate.Value.Date <= toDate.Date)
            //                                                .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            //    if (previousCycle != null)
            //    {
            //        closeCustomerTransaction = from ct in _customerOpenBalanceRepository.GetAll()
            //                                    //.Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
            //                                    .Where(s => previousCycle != null && s.AccountCycleId == previousCycle.Id)
            //                                    .Where(s => s.Key == JournalType.Invoice || s.Key == JournalType.CustomerCredit)
            //                                    .AsNoTracking()

            //                                   join u in _journalItemRepository.GetAll()
            //                                      .Where(s => s.Journal.Status == TransactionStatus.Publish)
            //                                      .Where(s => s.Identifier == null)
            //                                      .WhereIf(accounts != null && accounts.Count() > 0, u => accounts.Contains(u.AccountId))
            //                                      .WhereIf(accountType != null && accountType.Count() > 0, u => accountType.Contains(u.Account.AccountTypeId))

            //                                      .Where(s => s.Journal.InvoiceId != null || s.Journal.CustomerCreditId != null)
            //                                      .WhereIf(journalType != null && journalType.Count() > 0, u => journalType.Contains(u.Journal.JournalType))

            //                                      .WhereIf(customerTyes != null && customerTyes.Count() > 0, u =>
            //                                           ((u.Journal.Invoice == null && u.Journal.Invoice.Customer == null) || customerTyes.Contains(u.Journal.Invoice.Customer.CustomerTypeId.Value)) &&
            //                                           ((u.Journal.CustomerCredit == null && u.Journal.CustomerCredit.Customer == null) || customerTyes.Contains(u.Journal.CustomerCredit.Customer.CustomerTypeId.Value)))

            //                                      .WhereIf(customers != null && customers.Count() > 0, u =>
            //                                           (u.Journal.Invoice == null || customers.Contains(u.Journal.Invoice.CustomerId)) &&
            //                                           (u.Journal.CustomerCredit == null || customers.Contains(u.Journal.CustomerCredit.CustomerId)))
            //                                      .WhereIf(locations != null && locations.Count() > 0,
            //                                           u => u.Journal.LocationId != null && locations.Contains(u.Journal.LocationId.Value)).AsNoTracking()
            //                                      .AsNoTracking()

            //                                      .Select(s => new
            //                                      {
            //                                          s.Journal.CreationTimeIndex,
            //                                          TransactionId = s.Journal.InvoiceId != null ? s.Journal.InvoiceId : s.Journal.CustomerCreditId,
            //                                          AccountName = s.Account.AccountName,
            //                                          AccountCode = s.Account.AccountCode,
            //                                          AccountId = s.AccountId,
            //                                          InvoiceOrCreditId = s.Journal.InvoiceId != null ? s.Journal.InvoiceId.Value : s.Journal.CustomerCreditId.Value,
            //                                          JournalId = s.JournalId,
            //                                          Date = s.Journal.Date,
            //                                          JournalMemo = s.Journal.Memo,
            //                                          JournalNo = s.Journal.JournalNo,
            //                                          Reference = s.Journal.Reference,
            //                                          User = s.Journal.CreatorUser.UserName,
            //                                          JournalType = s.Journal.JournalType,
            //                                          Factor = s.Journal.JournalType == JournalType.CustomerCredit ? -1 : 1,
            //                                          Location = s.Journal.Location.LocationName,
            //                                          CustomerId = s.Journal.InvoiceId != null ? s.Journal.Invoice.CustomerId : s.Journal.CustomerCredit.CustomerId,
            //                                          CustomerName = s.Journal.InvoiceId != null ? s.Journal.Invoice.Customer.CustomerName : s.Journal.CustomerCredit.Customer.CustomerName,
            //                                          CustomerCode = s.Journal.InvoiceId != null ? s.Journal.Invoice.Customer.CustomerCode : s.Journal.CustomerCredit.Customer.CustomerCode,
            //                                          //Aging = (int)(toDate - s.Journal.Date).TotalDays,
            //                                          Currency = filterByAccountingCurrency ?
            //                                        new CurrencyOutput
            //                                        {
            //                                            Id = s.Journal.Currency.Id,
            //                                            Code = s.Journal.Currency.Code,
            //                                            Name = s.Journal.Currency.Name,
            //                                            Symbol = s.Journal.Currency.Symbol,
            //                                        }
            //                                        : new CurrencyOutput
            //                                        {
            //                                            Id = s.Journal.MultiCurrency.Id,
            //                                            Code = s.Journal.MultiCurrency.Code,
            //                                            Name = s.Journal.MultiCurrency.Name,
            //                                            Symbol = s.Journal.MultiCurrency.Symbol,
            //                                        }

            //                                      })

            //                                   on ct.TransactionId equals u.TransactionId

            //                                   select new GetCustomerByInvoiceWithPaymentReportOutput
            //                                   {
            //                                       CreationTimeIndex = u.CreationTimeIndex,
            //                                       TransactionId = ct.TransactionId,
            //                                       Date = u.Date,
            //                                       Description = u.JournalMemo,
            //                                       JournalNo = u.JournalNo,
            //                                       JournalType = u.JournalType,
            //                                       CustomerId = u.CustomerId,
            //                                       CustomerName = u.CustomerName,
            //                                       CustomerCode = u.CustomerCode,
            //                                       Location = u.Location,
            //                                       Reference = u.Reference,
            //                                       User = u.User,
            //                                       //Aging = (int)(toDate - u.Date).TotalDays,
            //                                       TotalAmount = (ct.Key == JournalType.CustomerCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance) * u.Factor,
            //                                       AccountId = u.AccountId,
            //                                       AccountCode = u.AccountCode,
            //                                       AccountName = u.AccountName,
            //                                       Currency = new CurrencyOutput
            //                                       {
            //                                           Id = u.Currency.Id,
            //                                           Code = u.Currency.Code,
            //                                           Name = u.Currency.Name,
            //                                           Symbol = u.Currency.Symbol,
            //                                       }
            //                                   };

            //        fromDate = previousCycle.EndDate.Value.AddDays(1);
            //    }
            //}


            var invoiceQuery = (from u in _journalItemRepository.GetAll()
                                    .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                    .Where(t => t.Identifier == null)
                                    .Where(t => t.Journal.Invoice != null)
                                    .Where(u => fromDate.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= toDate.Date)
                                    .WhereIf(accounts != null && accounts.Count() > 0, u => accounts.Contains(u.AccountId))
                                    .WhereIf(users != null && users.Count() > 0, u => users.Contains(u.CreatorUserId))
                                    .WhereIf(accountType != null && accountType.Count() > 0, u => accountType.Contains(u.Account.AccountTypeId))
                                    .WhereIf(journalType != null && journalType.Count() > 0, u => journalType.Contains(u.Journal.JournalType))
                                    .WhereIf(!filter.IsNullOrEmpty(), u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                    .WhereIf(customers != null && customers.Count() > 0, u => customers.Contains(u.Journal.Invoice.CustomerId))
                                    .WhereIf(customerTyes != null && customerTyes.Count() > 0, u => u.Journal.Invoice.Customer != null && customerTyes.Contains(u.Journal.Invoice.Customer.CustomerTypeId.Value))
                                    .WhereIf(locations != null && locations.Count() > 0, u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                    .AsNoTracking()
                                join c in userGroupByVendor
                                    on u.Journal.Invoice.CustomerId equals c
                                select new GetCustomerByInvoiceWithPaymentReportOutput
                                {
                                    CreationTimeIndex = u.Journal.CreationTimeIndex,
                                    TransactionId = u.Journal.InvoiceId.Value,
                                    Date = u.Journal.Date,
                                    Description = u.Journal.Memo,
                                    JournalNo = u.Journal.JournalNo,
                                    JournalType = u.Journal.JournalType,
                                    CustomerId = u.Journal.Invoice.CustomerId,
                                    CustomerName = u.Journal.Invoice.Customer.CustomerName,
                                    CustomerCode = u.Journal.Invoice.Customer.CustomerCode,
                                    Location = u.Journal.Location.LocationName,
                                    TotalAmount = filterByAccountingCurrency ? u.Journal.Invoice.Total : u.Journal.Invoice.MultiCurrencyTotal,
                                    Reference = u.Journal.Reference,
                                    User = u.Journal.CreatorUser.UserName,
                                 
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
                                     u.Journal.MultiCurrency == null ? null :
                                     new CurrencyOutput
                                     {
                                         Id = u.Journal.MultiCurrency.Id,
                                         Code = u.Journal.MultiCurrency.Code,
                                         Name = u.Journal.MultiCurrency.Name,
                                         Symbol = u.Journal.MultiCurrency.Symbol
                                     }
                                });


            var customerCreditQuery = (from u in _journalItemRepository.GetAll()
                                         .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                         .Where(t => t.Identifier == null)
                                         .Where(t => t.Journal.CustomerCredit != null)
                                         .Where(u => fromDate.Date <= u.Journal.Date.Date && u.Journal.Date.Date <= toDate.Date)
                                         .WhereIf(users != null && users.Count() > 0, u => users.Contains(u.Journal.CreatorUserId))
                                         .WhereIf(accounts != null && accounts.Count() > 0, u => accounts.Contains(u.AccountId))
                                         .WhereIf(accountType != null && accountType.Count() > 0, u => accountType.Contains(u.Account.AccountTypeId))
                                         .WhereIf(journalType != null && journalType.Count() > 0, u => journalType.Contains(u.Journal.JournalType))
                                         .WhereIf(!filter.IsNullOrEmpty(), u => u.Journal.JournalNo.ToLower().Contains(filter.ToLower()))
                                         .WhereIf(customers != null && customers.Count() > 0, u => customers.Contains(u.Journal.CustomerCredit.CustomerId))
                                         .WhereIf(customerTyes != null && customerTyes.Count() > 0, u => u.Journal.CustomerCredit.Customer != null && customerTyes.Contains(u.Journal.CustomerCredit.Customer.CustomerTypeId.Value))
                                         .WhereIf(locations != null && locations.Count() > 0, u => u.Journal != null && locations.Contains(u.Journal.LocationId.Value))
                                         .AsNoTracking()
                                       join c in userGroupByVendor
                                       on u.Journal.CustomerCredit.CustomerId equals c
                                       select new GetCustomerByInvoiceWithPaymentReportOutput
                                       {
                                           CreationTimeIndex = u.Journal.CreationTimeIndex,
                                           TransactionId = u.Journal.CustomerCreditId.Value,
                                           Date = u.Journal.Date,
                                           Description = u.Journal.Memo,
                                           JournalNo = u.Journal.JournalNo,
                                           Reference = u.Journal.Reference,
                                           JournalType = u.Journal.JournalType,
                                           CustomerId = u.Journal.CustomerCredit.CustomerId,
                                           CustomerName = u.Journal.CustomerCredit.Customer.CustomerName,
                                           CustomerCode = u.Journal.CustomerCredit.Customer.CustomerCode,
                                           User = u.Journal.CreatorUser.UserName,
                                           Location = u.Journal.Location.LocationName,
                                           TotalAmount = filterByAccountingCurrency ? -u.Journal.CustomerCredit.Total : -u.Journal.CustomerCredit.MultiCurrencyTotal,
                                         
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
                                                u.Journal.MultiCurrency == null ? null :
                                                new CurrencyOutput
                                                {
                                                    Id = u.Journal.MultiCurrency.Id,
                                                    Code = u.Journal.MultiCurrency.Code,
                                                    Name = u.Journal.MultiCurrency.Name,
                                                    Symbol = u.Journal.MultiCurrency.Symbol
                                                }
                                       });


            var invoiceAndCreditByCustomerConcatQuery = invoiceQuery.Concat(customerCreditQuery);

            //if (closeCustomerTransaction != null) invoiceAndCreditByCustomerConcatQuery = closeCustomerTransaction.Concat(invoiceAndCreditByCustomerConcatQuery);


            var receivePaymentByCustomerQuerys = (from rpi in _receivePaymentItemRepository.GetAll()
                                                      .WhereIf(customers != null && customers.Count() > 0, u => customers.Contains(u.CustomerId))
                                                      .AsNoTracking()
                                                  join j in _journalRepository.GetAll()
                                                          .Where(s => s.ReceivePaymentId.HasValue)
                                                          .Where(u => fromDate.Date <= u.Date.Date)
                                                          .WhereIf(locations != null && locations.Count() > 0, u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                                          .AsNoTracking()
                                                  on rpi.ReceivePaymentId equals j.ReceivePaymentId
                                                  orderby j.Date 
                                                  select new GetCustomerByInvoiceWithPaymentReportOutput
                                                  {
                                                      JournalNo = j.JournalNo,
                                                      Reference = j.Reference,
                                                      CreationTimeIndex = j.CreationTimeIndex,
                                                      User = j.CreatorUser.UserName,
                                                      Date = j.Date,
                                                      TransactionId = rpi.PayToId.Value, 
                                                      PaymentId = rpi.ReceivePaymentId,
                                                      TotalAmount = filterByAccountingCurrency ? (rpi.InvoiceId != null ? rpi.Payment : -rpi.Payment) : (rpi.InvoiceId != null ? rpi.MultiCurrencyPayment : -rpi.MultiCurrencyPayment),
                                                  });

           
            var finalQuery = from i in invoiceAndCreditByCustomerConcatQuery
                             join r in receivePaymentByCustomerQuerys
                             on i.TransactionId equals r.TransactionId into payments
                             from p in payments.DefaultIfEmpty()
                             select new GetCustomerByInvoiceWithPaymentReportOutput
                             {
                                 CreationTimeIndex = i.CreationTimeIndex,
                                 User = i.User,
                                 TransactionId = i.TransactionId,
                                 CustomerId = i.CustomerId,
                                 CustomerName = i.CustomerName,
                                 CustomerCode = i.CustomerCode,
                                 Date = i.Date,
                                 AccountId = i.AccountId,
                                 AccountCode = i.AccountCode,
                                 AccountName = i.AccountName,
                                 Description = i.Description,
                                 JournalNo = i.JournalNo,
                                 Reference =  i.Reference,
                                 JournalType = i.JournalType,
                                 Location = i.Location,
                                 Currency = i.Currency,
                                 TotalAmount = i.TotalAmount,

                                 TotalPaid = p == null ? 0 : p.TotalAmount,
                                 PaymentNo = p == null ? "" : p.JournalNo,                                
                                 PaymentReference = p == null ? "" : p.Reference,                                
                                 PaymentCreationTimeIndex = p == null ? 0 : p.CreationTimeIndex,
                                 PaymentDate = p == null ? (DateTime?) null : p.Date,
                                 PaymentId = p == null ? (Guid?) null : p.PaymentId,
                                 PaymentUserName = p == null ? "" : p.User
                             }
                             into l
                             group l by new
                             {
                                 CreationTimeIndex = l.CreationTimeIndex,
                                 User = l.User,
                                 TransactionId = l.TransactionId,
                                 CustomerId = l.CustomerId,
                                 CustomerName = l.CustomerName,
                                 CustomerCode = l.CustomerCode,
                                 Date = l.Date,
                                 AccountId = l.AccountId,
                                 AccountCode = l.AccountCode,
                                 AccountName = l.AccountName,
                                 Description = l.Description,
                                 JournalNo = l.JournalNo,
                                 Reference = l.Reference,
                                 JournalType = l.JournalType,
                                 Location = l.Location,
                                 Currency = l.Currency,
                                 TotalAmount = l.TotalAmount,
                             }
                             into g

                             let paidAmount = g.Sum(t => t.TotalPaid)

                             select new GetCustomerByInvoiceWithPaymentReportOutput
                             {
                                 CreationTimeIndex = g.Key.CreationTimeIndex,
                                 User = g.Key.User,
                                 TransactionId = g.Key.TransactionId,
                                 CustomerId = g.Key.CustomerId,
                                 CustomerName = g.Key.CustomerName,
                                 CustomerCode = g.Key.CustomerCode,
                                 Date = g.Key.Date,
                                 AccountId = g.Key.AccountId,
                                 AccountCode = g.Key.AccountCode,
                                 AccountName = g.Key.AccountName,
                                 Description = g.Key.Description,
                                 JournalNo = g.Key.JournalNo,
                                 Reference = g.Key.Reference,
                                 JournalType = g.Key.JournalType,
                                 Location = g.Key.Location,
                                 Currency = g.Key.Currency,
                                 TotalAmount = g.Key.TotalAmount,

                                 TotalPaid = paidAmount,
                                 Balance = g.Key.TotalAmount - paidAmount,
                                 PaymentItems = g.ToList()
                             };

            return finalQuery;

        }
    }
}
