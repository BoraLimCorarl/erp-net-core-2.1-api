using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using CorarlERP.BankTransactions.Dto;
using CorarlERP.Bills;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.Invoices;
using CorarlERP.Journals;
using CorarlERP.PayBills;
using CorarlERP.ReceivePayments;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using CorarlERP.Authorization;
using CorarlERP.Deposits;
using CorarlERP.Withdraws;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;

namespace CorarlERP.BankTransactions
{
    
    [AbpAuthorize]
    public class BankTrasactionAppService : CorarlERPAppServiceBase, IBankTransactionAppService
    {
        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IBillManager _billManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<PayBill, Guid> _payBillRepository;
        private readonly IPayBillManager _payBillManager;
        private readonly IRepository<PayBillDetail, Guid> _payBillDetailRepository;
        private readonly IPayBillDetailManager _payBillDetailManager;
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IVendorManager _vendorItemManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IRepository<Deposit, Guid> _depositRepository;
        private readonly IRepository<DepositItem, Guid> _depositItemRepository;

        private readonly IRepository<Withdraw, Guid> _withdrawRepository;
        private readonly IRepository<WithdrawItem, Guid> _withdrawItemRepository;

        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<ReceivePayment, Guid> _receivePaymentRepository;
        private readonly IReceivePaymentManager _receivePaymentManager;
        private readonly IRepository<ReceivePaymentDetail, Guid> _receivePaymentDetailRepository;
        private readonly IReceivePaymentDetailManager _receivePaymentDetailManager;
        private readonly ICustomerManager _customerItemManager;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<Location, long> _locationRepository;

        private readonly IRepository<User, long> _userRepository;
        public BankTrasactionAppService(
         IJournalManager journalManager,
         IRepository<User, long> userRepository,
         IRepository<Journal, Guid> journalRepository,
         IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
         IJournalItemManager journalItemManager,
         IRepository<JournalItem, Guid> journalItemRepository,
         IPayBillManager payBillManager,
         IBillManager billManager,
         IPayBillDetailManager payBillDetailManager,
         IVendorManager vendorManager,
         IRepository<Vendor, Guid> vendorRepository,
         IRepository<PayBill, Guid> payBillRepository,
         IRepository<PayBillDetail, Guid> payBillDetailRepository,
         IRepository<Bill, Guid> billRepository,
         IReceivePaymentManager receivePaymentManager,
         IInvoiceManager invoiceManager,
         IReceivePaymentDetailManager receivePaymentDetailManager,
         ICustomerManager customerManager,
         IRepository<Customer, Guid> customerRepository,
         IRepository<ReceivePayment, Guid> receivePaymentRepository,
         IRepository<ReceivePaymentDetail, Guid> receivePaymentDetailRepository,
         IRepository<Invoice, Guid> invoiceRepository,
         IRepository<Deposit, Guid> depositRepository,
         IRepository<DepositItem, Guid> depositItemRepository,
         IRepository<Withdraw, Guid> withdrawRepository,
         IRepository<WithdrawItem, Guid> withdrawItemRepository,
         IRepository<Location, long> locationRepository,
         
         IRepository<UserGroupMember, Guid> userGroupMemberRepository) : base(userGroupMemberRepository, locationRepository)
        {
            _userRepository = userRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _payBillDetailManager = payBillDetailManager;
            _payBillManager = payBillManager;
            _vendorItemManager = vendorManager;
            _payBillRepository = payBillRepository;
            _payBillDetailRepository = payBillDetailRepository;
            _vendorRepository = vendorRepository;
            _billRepository = billRepository;
            _billManager = billManager;
            _receivePaymentDetailManager = receivePaymentDetailManager;
            _receivePaymentManager = receivePaymentManager;
            _customerItemManager = customerManager;
            _receivePaymentRepository = receivePaymentRepository;
            _receivePaymentDetailRepository = receivePaymentDetailRepository;
            _customerRepository = customerRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _depositItemRepository = depositItemRepository;
            _depositRepository = depositRepository;
            _withdrawItemRepository = withdrawItemRepository;
            _withdrawRepository = withdrawRepository;
            _locationRepository = locationRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_BankTransactions_GetList)]
        public async Task<PagedResultDto<GetlistBankTransactionOutput>> GetListBankTransaction(GetListBankTransactionInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();


            var accountQuery = GetAccounts(input.Accounts);
            var locationQuery = GetLocations(null, input.Locations);
            var userQuery = GetUsers(input.Users);
            var vendorQuery = GetVendors(null, input.Vendors, null, vendorTypeMemberIds);
            var customerQuery = GetCustomers(null, input.Customers, null, customerTypeMemberIds);

            var filterVendor = input.Vendors != null && input.Vendors.Count > 0;
            var filterCustomer = input.Customers != null && input.Customers.Count > 0;

            var jpQuery = _journalRepository.GetAll()
                                    .Where(u => u.PayBillId != null)
                                    .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                    .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                        u => input.Locations.Contains(u.LocationId.Value))
                                    .WhereIf(input.Users != null && input.Users.Count > 0,
                                                        u => input.Users.Contains(u.CreatorUserId))
                                    .WhereIf(input.FromDate != null && input.ToDate != null,
                                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                    .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                    .AsNoTracking()
                                    .Select(u => new
                                    {
                                        Id = u.Id,
                                        PayBillId = u.PayBillId,
                                        JournalType = u.JournalType,
                                        JournalNo = u.JournalNo,
                                        Date = u.Date,
                                        Status = u.Status,
                                        CreationTimeIndex = u.CreationTimeIndex,
                                        Memo = u.Memo,
                                        CreatorUserId = u.CreatorUserId,
                                        LocationId = u.LocationId
                                    });

            var jpbQuery = from j in jpQuery
                           join u in userQuery
                           on j.CreatorUserId equals u.Id
                           join l in locationQuery
                           on j.LocationId equals l.Id
                           select new
                           {
                               Id = j.Id,
                               PayBillId = j.PayBillId,
                               JournalType = j.JournalType,
                               JournalNo = j.JournalNo,
                               Date = j.Date,
                               Status = j.Status,
                               CreationTimeIndex = j.CreationTimeIndex,
                               Memo = j.Memo,
                               CreatorUserId = j.CreatorUserId,
                               LocationId = j.LocationId,
                               UserName = u.UserName,
                               LocationName = l.LocationName
                           };

            var journalItemQuery = from ji in _journalItemRepository.GetAll()
                                                .WhereIf(input.Accounts != null && input.Accounts.Count > 0,
                                                    u => input.Accounts.Contains(u.AccountId))
                                                .AsNoTracking()
                                                .Select(s => new
                                                {
                                                    JournalId = s.JournalId,
                                                    AccountId = s.AccountId,
                                                    Identifier = s.Identifier,
                                                    Key = s.Key
                                                })
                                   join a in accountQuery
                                   on ji.AccountId equals a.Id
                                   select new
                                   {
                                       JournalId = ji.JournalId,
                                       Identifier = ji.Identifier,
                                       Key = ji.Key,
                                       AccountName = a.AccountName
                                   };

            var pbiQuery = from pbi in _payBillDetailRepository.GetAll()
                                       .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           PayBillId = s.PayBillId,
                                           VendorId = s.VendorId
                                       })
                           join v in vendorQuery
                           on pbi.VendorId equals v.Id
                           where !filterCustomer
                           select new
                           {
                               PayBillId = pbi.PayBillId,
                               VendorId = pbi.VendorId,
                               VendorName = v.VendorName
                           };

            var pbQuery = from p in _payBillRepository.GetAll()
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          TotalPayment = s.TotalPayment,
                                          TotalOpenBalance = s.TotalOpenBalance,
                                          ReceiveFrom = s.ReceiveFrom
                                      })
                          join pi in pbiQuery
                          on p.Id equals pi.PayBillId
                          into pis
                          where pis.Count() > 0 
                          select new
                          {
                              Id = p.Id,
                              TotalPayment = p.TotalPayment,
                              TotalOpenBalance = p.TotalOpenBalance,
                              ReceiveFrom = p.ReceiveFrom,
                              Vendors = pis.GroupBy(s => new VendorSummaryOutput
                              {
                                  Id = s.VendorId,
                                  VendorName = s.VendorName
                              })
                              .Select(s => s.Key)
                          };

            var payBillQuery = from p in pbQuery
                               join j in jpbQuery
                               on p.Id equals j.PayBillId
                               join ji in journalItemQuery
                                          .Where(s => s.Key == PostingKey.Payment || s.Key == PostingKey.Clearance)
                               on j.Id equals ji.JournalId
                               into jis
                               where jis.Count() > 0 
                               select new GetlistBankTransactionOutput
                               {
                                   Id = j.Id,
                                   Type = j.JournalType,
                                   TypeName = j.JournalType.ToString(),
                                   JournalNo = j.JournalNo,
                                   Date = j.Date,
                                   Status = j.Status,
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   Memo = j.Memo,
                                   User = new UserDto {
                                       Id = j.CreatorUserId.Value,
                                       UserName = j.UserName
                                   },
                                   Location = new LocationSummaryOutput {
                                       Id = j.LocationId.Value,
                                       LocationName = j.LocationName
                                   },
                                   TotalPayment = p.TotalPayment,
                                   TotalAmount = p.TotalOpenBalance,
                                   Vendor = p.Vendors.ToList(),
                                   AccountName = p.ReceiveFrom == ReceiveFromPayBill.Cash ?
                                                 jis.Where(t => t.Key == PostingKey.Payment && t.Identifier == null).Select(x => x.AccountName).FirstOrDefault() :
                                                 jis.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountName).FirstOrDefault()
                               };


            var jrQuery = _journalRepository.GetAll()
                            .Where(u => u.ReceivePaymentId != null)
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                            .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                u => input.Locations.Contains(u.LocationId.Value))
                            .WhereIf(input.Users != null && input.Users.Count > 0,
                                                u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                            .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(u => new
                            {
                                Id = u.Id,
                                ReceivePaymentId = u.ReceivePaymentId,
                                JournalType = u.JournalType,
                                JournalNo = u.JournalNo,
                                Date = u.Date,
                                Status = u.Status,
                                CreationTimeIndex = u.CreationTimeIndex,
                                Memo = u.Memo,
                                CreatorUserId = u.CreatorUserId,
                                LocationId = u.LocationId
                            });

            var jrpQuery = from j in jrQuery
                           join u in userQuery
                           on j.CreatorUserId equals u.Id
                           join l in locationQuery
                           on j.LocationId equals l.Id
                           select new
                           {
                               Id = j.Id,
                               ReceivePaymentId = j.ReceivePaymentId,
                               JournalType = j.JournalType,
                               JournalNo = j.JournalNo,
                               Date = j.Date,
                               Status = j.Status,
                               CreationTimeIndex = j.CreationTimeIndex,
                               Memo = j.Memo,
                               CreatorUserId = j.CreatorUserId,
                               LocationId = j.LocationId,
                               UserName = u.UserName,
                               LocationName = l.LocationName
                           };

            var rpiQuery = from rpi in _receivePaymentDetailRepository.GetAll()
                                       .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           ReceivePaymentId = s.ReceivePaymentId,
                                           CustomerId = s.CustomerId
                                       })
                           join c in customerQuery
                           on rpi.CustomerId equals c.Id
                           where !filterVendor
                           select new
                           {
                               ReceivePaymentId = rpi.ReceivePaymentId,
                               CustomerId = rpi.CustomerId,
                               CustomerName = c.CustomerName
                           };

            var rpQuery = from r in _receivePaymentRepository.GetAll()
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          TotalPayment = s.TotalPayment,
                                          TotalOpenBalance = s.TotalOpenBalance,
                                          ReceiveFrom = s.ReceiveFrom
                                      })
                          join ri in rpiQuery
                          on r.Id equals ri.ReceivePaymentId
                          into ris
                          where ris.Count() > 0
                          select new
                          {
                              Id = r.Id,
                              TotalPayment = r.TotalPayment,
                              TotalOpenBalance = r.TotalOpenBalance,
                              ReceiveFrom = r.ReceiveFrom,
                              Customers = ris.GroupBy(s => new CustomerSummaryOutput
                              {
                                  Id = s.CustomerId,
                                  CustomerName = s.CustomerName
                              })
                              .Select(s => s.Key)
                          };

            var receivePamentQuery = from r in rpQuery
                                     join j in jrpQuery
                                     on r.Id equals j.ReceivePaymentId
                                     join ji in journalItemQuery
                                                .Where(s => s.Key == PostingKey.Payment || s.Key == PostingKey.Clearance)
                                     on j.Id equals ji.JournalId
                                     into jis
                                     where jis.Count() > 0
                                     select new GetlistBankTransactionOutput
                                     {
                                         Id = j.Id,
                                         Type = j.JournalType,
                                         TypeName = j.JournalType.ToString(),
                                         JournalNo = j.JournalNo,
                                         Date = j.Date,
                                         Status = j.Status,
                                         CreationTimeIndex = j.CreationTimeIndex,
                                         Memo = j.Memo,
                                         User = new UserDto
                                         {
                                             Id = j.CreatorUserId.Value,
                                             UserName = j.UserName
                                         },
                                         Location = new LocationSummaryOutput
                                         {
                                             Id = j.LocationId.Value,
                                             LocationName = j.LocationName
                                         },
                                         TotalPayment = r.TotalPayment,
                                         TotalAmount = r.TotalOpenBalance,
                                         Customer = r.Customers.ToList(),
                                         AccountName = r.ReceiveFrom == ReceiveFromRecievePayment.Cash ?
                                                       jis.Where(t => t.Key == PostingKey.Payment && t.Identifier == null).Select(x => x.AccountName).FirstOrDefault() :
                                                       jis.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountName).FirstOrDefault()
                                     };

            var jdQuery = _journalRepository.GetAll()
                            .Where(u => u.DepositId != null)
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                            .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                u => input.Locations.Contains(u.LocationId.Value))
                            .WhereIf(input.Users != null && input.Users.Count > 0,
                                                u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                            .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(u => new
                            {
                                Id = u.Id,
                                Depositid = u.DepositId,
                                JournalType = u.JournalType,
                                JournalNo = u.JournalNo,
                                Date = u.Date,
                                Status = u.Status,
                                CreationTimeIndex = u.CreationTimeIndex,
                                Memo = u.Memo,
                                CreatorUserId = u.CreatorUserId,
                                LocationId = u.LocationId
                            });

            var jdpQuery = from j in jdQuery
                           join u in userQuery
                           on j.CreatorUserId equals u.Id
                           join l in locationQuery
                           on j.LocationId equals l.Id
                           select new
                           {
                               Id = j.Id,
                               DepositId = j.Depositid,
                               JournalType = j.JournalType,
                               JournalNo = j.JournalNo,
                               Date = j.Date,
                               Status = j.Status,
                               CreationTimeIndex = j.CreationTimeIndex,
                               Memo = j.Memo,
                               CreatorUserId = j.CreatorUserId,
                               LocationId = j.LocationId,
                               UserName = u.UserName,
                               LocationName = l.LocationName
                           };


            var dQuery = _depositRepository.GetAll()
                                         .AsNoTracking()
                                         .Select(s => new 
                                         {
                                             Id = s.Id,
                                             TotalPayment = s.Total,
                                             TotalAmount = s.Total,
                                             CanVoidDraft = s.BankTransferId != null,
                                             VendorId = s.ReceiveFromVendorId,
                                             CustomerId = s.ReceiveFromCustomerId,
                                         });

            var dvQuery = from d in dQuery.Where(s => s.VendorId.HasValue)
                          join v in vendorQuery
                          on d.VendorId equals v.Id
                          where !filterCustomer
                          select new DepositWithdrawPartnerOutput
                          {
                              Id = d.Id,
                              TotalPayment = d.TotalPayment,
                              TotalAmount = d.TotalAmount,
                              CanVoidDraft = d.CanVoidDraft,
                              VendorId = d.VendorId,
                              CustomerId = d.CustomerId,
                              VendorName = v.VendorName,
                          };

            var dcQuery = from d in dQuery.Where(s => s.CustomerId.HasValue)
                          join c in customerQuery
                          on d.CustomerId equals c.Id
                          where !filterVendor
                          select new DepositWithdrawPartnerOutput
                          {
                              Id = d.Id,
                              TotalPayment = d.TotalPayment,
                              TotalAmount = d.TotalAmount,
                              CanVoidDraft = d.CanVoidDraft,
                              VendorId = d.VendorId,
                              CustomerId = d.CustomerId,
                              CustomerName = c.CustomerName
                          };

            var dnovcQuery = dQuery.Where(s => !s.CustomerId.HasValue && !s.VendorId.HasValue)
                                     .Where(s => !filterVendor && !filterCustomer)
                                     .Select(s => new DepositWithdrawPartnerOutput
                                     {
                                         Id = s.Id,
                                         TotalPayment = s.TotalPayment,
                                         TotalAmount = s.TotalAmount,
                                         CanVoidDraft = s.CanVoidDraft,
                                         VendorId = s.VendorId,
                                         CustomerId = s.CustomerId
                                     });

            IQueryable<DepositWithdrawPartnerOutput> dpQuery;            
            if( filterVendor && filterCustomer)
            {
                dpQuery = dnovcQuery;
            }
            else if (filterVendor)
            {
                dpQuery = dvQuery;
            }
            else if (filterCustomer)
            {
                dpQuery = dcQuery;
            }
            else
            {
                dpQuery = dvQuery.Union(dcQuery).Union(dnovcQuery);
            }           

            var depositQuery = from d in dpQuery
                               join j in jdpQuery
                               on d.Id equals j.DepositId
                               join ji in journalItemQuery
                                          .Where(s => s.Key == PostingKey.Bank && s.Identifier == null)
                               on j.Id equals ji.JournalId

                               select new GetlistBankTransactionOutput
                               {
                                   Location = new LocationSummaryOutput { 
                                        Id = j.LocationId.Value,
                                        LocationName = j.LocationName
                                   },
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   User = new UserDto { 
                                        Id = j.CreatorUserId.Value,
                                        UserName = j.UserName
                                   },
                                   Type = j.JournalType,
                                   TypeName = j.JournalType.ToString(),
                                   Memo = j.Memo,
                                   Id = d.Id,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Status = j.Status,
                                   TotalPayment = d.TotalPayment,
                                   TotalAmount = d.TotalAmount,
                                   CanVoidDraft = d.CanVoidDraft,
                                   AccountName = ji.AccountName,
                                   Customer = d.CustomerId != null ? 
                                       new List<CustomerSummaryOutput>() 
                                       {
                                            new CustomerSummaryOutput
                                            {
                                                Id = d.CustomerId.Value,
                                                CustomerName = d.CustomerName
                                            }
                                       } : 
                                       new List<CustomerSummaryOutput>(),

                                   Vendor = d.VendorId != null ? 
                                       new List<VendorSummaryOutput>() 
                                       {
                                            new VendorSummaryOutput
                                            {
                                                Id = d.VendorId.Value,
                                                VendorName = d.VendorName
                                            }
                                       } : 
                                       new List<VendorSummaryOutput>()

                               };

            var jwQuery = _journalRepository.GetAll()
                                    .Where(u => u.WithdrawId != null)
                                    .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                    .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                        u => input.Locations.Contains(u.LocationId.Value))
                                    .WhereIf(input.Users != null && input.Users.Count > 0,
                                                        u => input.Users.Contains(u.CreatorUserId))
                                    .WhereIf(input.FromDate != null && input.ToDate != null,
                                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                    .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                    .AsNoTracking()
                                    .Select(u => new
                                    {
                                        Id = u.Id,
                                        WithdrawId = u.WithdrawId,
                                        JournalType = u.JournalType,
                                        JournalNo = u.JournalNo,
                                        Date = u.Date,
                                        Status = u.Status,
                                        CreationTimeIndex = u.CreationTimeIndex,
                                        Memo = u.Memo,
                                        CreatorUserId = u.CreatorUserId,
                                        LocationId = u.LocationId
                                    });

            var jwdQuery = from j in jwQuery
                           join u in userQuery
                           on j.CreatorUserId equals u.Id
                           join l in locationQuery
                           on j.LocationId equals l.Id
                           select new
                           {
                               Id = j.Id,
                               WithdrawId = j.WithdrawId,
                               JournalType = j.JournalType,
                               JournalNo = j.JournalNo,
                               Date = j.Date,
                               Status = j.Status,
                               CreationTimeIndex = j.CreationTimeIndex,
                               Memo = j.Memo,
                               CreatorUserId = j.CreatorUserId,
                               LocationId = j.LocationId,
                               UserName = u.UserName,
                               LocationName = l.LocationName
                           };

            var wQuery = _withdrawRepository.GetAll()
                                          .AsNoTracking()
                                          .Select(s => new 
                                          {
                                              Id = s.Id,
                                              VendorId = s.VendorId,
                                              CustomerId = s.CustomerId,
                                              TotalPayment = s.Total,
                                              TotalAmount = s.Total,
                                              CanVoidDraft = s.BankTransferId != null,
                                          });

            var wvQuery = from d in wQuery.Where(s => s.VendorId.HasValue)
                          join v in vendorQuery
                          on d.VendorId equals v.Id
                          where !filterCustomer
                          select new DepositWithdrawPartnerOutput
                          {
                              Id = d.Id,
                              TotalPayment = d.TotalPayment,
                              TotalAmount = d.TotalAmount,
                              CanVoidDraft = d.CanVoidDraft,
                              VendorId = d.VendorId,
                              CustomerId = d.CustomerId,
                              VendorName = v.VendorName,
                          };

            var wcQuery = from d in wQuery.Where(s => s.CustomerId.HasValue)
                          join c in customerQuery
                          on d.CustomerId equals c.Id
                          where !filterVendor
                          select new DepositWithdrawPartnerOutput
                          {
                              Id = d.Id,
                              TotalPayment = d.TotalPayment,
                              TotalAmount = d.TotalAmount,
                              CanVoidDraft = d.CanVoidDraft,
                              VendorId = d.VendorId,
                              CustomerId = d.CustomerId,
                              CustomerName = c.CustomerName
                          };

            var wnovcQuery = wQuery.Where(s => !s.CustomerId.HasValue && !s.VendorId.HasValue)
                                   .Where(s => !filterVendor && !filterCustomer)
                                   .Select(d => new DepositWithdrawPartnerOutput
                                   {
                                       Id = d.Id,
                                       TotalPayment = d.TotalPayment,
                                       TotalAmount = d.TotalAmount,
                                       CanVoidDraft = d.CanVoidDraft,
                                       VendorId = d.VendorId,
                                       CustomerId = d.CustomerId
                                   });

            IQueryable<DepositWithdrawPartnerOutput> wdQuery;
            if(filterVendor && filterCustomer)
            {
                wdQuery = wnovcQuery;
            }
            else if (filterVendor)
            {
                wdQuery = wvQuery;
            }
            else if (filterCustomer)
            {
                wdQuery = wcQuery;
            }
            else
            {
                wdQuery = wvQuery.Union(wcQuery).Union(wnovcQuery);
            }

            var withdrawQuery = from w in wdQuery
                               join j in jwdQuery
                               on w.Id equals j.WithdrawId
                               join ji in journalItemQuery
                                          .Where(s => s.Key == PostingKey.Bank && s.Identifier == null)
                               on j.Id equals ji.JournalId

                               select new GetlistBankTransactionOutput
                               {
                                   Location = new LocationSummaryOutput
                                   {
                                       Id = j.LocationId.Value,
                                       LocationName = j.LocationName
                                   },
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   User = new UserDto
                                   {
                                       Id = j.CreatorUserId.Value,
                                       UserName = j.UserName
                                   },
                                   Type = j.JournalType,
                                   TypeName = j.JournalType.ToString(),
                                   Memo = j.Memo,
                                   Id = w.Id,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Status = j.Status,
                                   TotalPayment = w.TotalPayment,
                                   TotalAmount = w.TotalAmount,
                                   CanVoidDraft = w.CanVoidDraft,
                                   AccountName = ji.AccountName,
                                   Customer = w.CustomerId != null ?
                                       new List<CustomerSummaryOutput>()
                                       {
                                            new CustomerSummaryOutput
                                            {
                                                Id = w.CustomerId.Value,
                                                CustomerName = w.CustomerName
                                            }
                                       } :
                                       new List<CustomerSummaryOutput>(),

                                   Vendor = w.VendorId != null ?
                                       new List<VendorSummaryOutput>()
                                       {
                                            new VendorSummaryOutput
                                            {
                                                Id = w.VendorId.Value,
                                                VendorName = w.VendorName
                                            }
                                       } :
                                       new List<VendorSummaryOutput>()
                               };

            IQueryable<GetlistBankTransactionOutput> result;
            if(filterCustomer && filterVendor)
            {
                result = withdrawQuery;
            }
            else if (filterCustomer)
            {
                result = receivePamentQuery.Union(depositQuery).Union(withdrawQuery);
            } 
            else if (filterVendor)
            {
                result = payBillQuery.Union(depositQuery).Union(withdrawQuery);
            }
            else
            {
                result = payBillQuery.Union(receivePamentQuery).Union(depositQuery).Union(withdrawQuery);
            }
            
            var resultCount = await result.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetlistBankTransactionOutput>(resultCount, new List<GetlistBankTransactionOutput>());

           
            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    result = result.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    result = result.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    result = result.OrderByDescending(s => s.TypeName).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("totalpayment"))
                {
                    result = result.OrderByDescending(s => s.TotalPayment).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    result = result.OrderByDescending(s => s.Status).ThenByDescending(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    result = result.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    result = result.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    result = result.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    result = result.OrderBy(s => s.TypeName).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("totalpayment"))
                {
                    result = result.OrderBy(s => s.TotalPayment).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    result = result.OrderBy(s => s.Status).ThenBy(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    result = result.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                }
            }

            var @entities = await result.PageBy(input).ToListAsync();

            return new PagedResultDto<GetlistBankTransactionOutput>(resultCount, ObjectMapper.Map<List<GetlistBankTransactionOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_BankTransactions_GetList)]
        public async Task<PagedResultDto<GetlistBankTransactionOutput>> GetListBankTransactionOld(GetListBankTransactionInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

#if old

            var queryPaybill = (from jItem in _journalItemRepository.GetAll()
                                .Include(u => u.Account)
                                .Include(t => t.Journal)
                                .Include(u => u.Account.AccountType)
                                .Where(t => t.Account.AccountType.AccountTypeName == "Cash" || t.Account.AccountType.AccountTypeName == "Bank")
                                .AsNoTracking()
                                join j in _journalRepository.GetAll().AsNoTracking()
                             
                                .Where(u => u.JournalType == JournalType.PayBill)
                                .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                .WhereIf(input.FromDate != null && input.ToDate != null,
                                    (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                on jItem.JournalId equals j.Id

                                join l in _locationRepository.GetAll().AsNoTracking()
                                on j.LocationId equals l.Id
                                join pb in _payBillRepository.GetAll().AsNoTracking()

                                on j.PayBillId equals pb.Id

                                join pbd in _payBillDetailRepository.GetAll().Include(u => u.Bill).Include(u => u.Vendor).AsNoTracking()
                                .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                on pb.Id equals pbd.PayBillId into g
                               // join v in _vendorRepository.GetAll().AsNoTracking() on pbd.VendorId equals v.Id 
                                join u in _userRepository.GetAll().AsNoTracking() on j.CreatorUserId equals u.Id 
                                group jItem by new { journal = j, payBill = pb, user = u ,location = l,vendor = g} into u
                                select new GetlistBankTransactionOutput
                                {
                                    Location = ObjectMapper.Map<LocationSummaryOutput>(u.Key.location),
                                    CreationTimeIndex = u.Key.journal.CreationTimeIndex,
                                    User = ObjectMapper.Map<UserDto>(u.Key.user),
                                    Type = u.Key.journal.JournalType,
                                    TypeName = Enum.GetName(u.Key.journal.JournalType.GetType(), u.Key.journal.JournalType),
                                    Memo = u.Key.journal.Memo,
                                    Id = u.Key.payBill.Id,
                                    Date = u.Key.journal.Date,
                                    JournalNo = u.Key.journal.JournalNo,
                                    TotalPayment = u.Key.payBill.TotalPayment,
                                    Status = u.Key.journal.Status,
                                    TotalAmount = u.Key.payBill.TotalOpenBalance,
                                    AccountName = u
                                            .Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Payment && t.Identifier == null)
                                            .Select(t => t.Account.AccountName).FirstOrDefault(),
                                    AccountId = u.Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Payment && t.Identifier == null)
                                            .Select(t => t.AccountId).FirstOrDefault(),
                                    Vendor =  u.Key.vendor != null ? u.Key.vendor.GroupBy(t=>t.Vendor).Select(d => new VendorSummaryOutput {

                                         Id = d.First().Id,
                                          VendorCode = d.First().Vendor.VendorCode,
                                          VendorName = d.First().Vendor.VendorName,
                                    }).ToList() : new List<VendorSummaryOutput>(),

                                })
                                .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId));

#endif

            var @queryable = from j in _journalRepository.GetAll().Include(t => t.CreatorUser).Include(t => t.Location)
                                  .Where(u => u.PayBillId != null)
                                  .Where(u => u.JournalType == JournalType.PayBill)
                                  .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                  .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                      u => input.Locations.Contains(u.LocationId.Value))
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
                                         
                                            .Include(u => u.Vendor)                                          
                                            .WhereIf(input.Vendors != null && input.Vendors.Count > 0,
                                            u => input.Vendors.Contains(u.VendorId))
                                            .AsNoTracking()

                             on j.PayBillId equals pbd.PayBillId
                             into payBillItems

                             join payBill in _payBillRepository.GetAll()

                            on j.PayBillId equals payBill.Id

                             join jItem in _journalItemRepository.GetAll().Include(t => t.Account)                                         
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

            var queryPaybill = queryable.Select(u => new GetlistBankTransactionOutput()
            {
                CreationTimeIndex = u.Journal.CreationTimeIndex,
                Vendor = u.Vendors.Select(s => new VendorSummaryOutput
                {
                    AccountId = s.AccountId,
                    Id = s.Id,
                    VendorCode = s.VendorCode,
                    VendorName = s.VendorName
                }).ToList(),
                Location = ObjectMapper.Map<LocationSummaryOutput>(u.Journal.Location),
               
                User = ObjectMapper.Map<UserDto>(u.Journal.CreatorUser),
               
                Memo = u.Journal.Memo,
                Id = u.Journal.PayBillId.Value,
                Type = u.Journal.JournalType,
                TypeName = Enum.GetName(u.Journal.JournalType.GetType(), u.Journal.JournalType),
                JournalNo = u.Journal.JournalNo,
                Date = u.Journal.Date,
                TotalPayment = u.payBills.TotalPayment, //u.accounts.Sum(t=>t.Debit),              
                Status = u.Journal.Status,
                TotalAmount = u.payBills.TotalOpenBalance,               
                AccountName = u.payBills.ReceiveFrom == ReceiveFromPayBill.Cash ?
                                u.accounts.Where(t => t.Key == PostingKey.Payment && t.Identifier == null)
                                    .Select(x => x.Account.AccountName).FirstOrDefault() :
                                u.accounts.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.Account.AccountName).FirstOrDefault(),
                AccountId = u.payBills.ReceiveFrom == ReceiveFromPayBill.VendorCredit ? u.accounts
                                .Where(t => t.Key == PostingKey.Payment && t.Identifier == null)
                                    .Select(x => x.AccountId).FirstOrDefault() :
                                u.accounts.Where(t => t.Key == PostingKey.Payment || t.Key == PostingKey.Clearance).Select(x => x.AccountId).FirstOrDefault(),
            });




            var queryRceivepaymentable = from j in _journalRepository.GetAll()
                                                 .Include(t => t.CreatorUser)
                                                 .Include(t => t.Location)
                          .WhereIf(input.Status != null && input.Status.Count > 0,
                                                      u => input.Status.Contains(u.Status))
                          .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                          .Where(u => u.JournalType == JournalType.ReceivePayment)
                          .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                          .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                          .WhereIf(input.FromDate != null && input.ToDate != null,
                              (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                              u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                      u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                      u.Memo.ToLower().Contains(input.Filter.ToLower())).AsNoTracking()

                             join recievepaymentDetail in _receivePaymentDetailRepository.GetAll()                           
                            .Include(u => u.Customer)                          
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



            var queryRceivepayment = queryRceivepaymentable.Select(u => new GetlistBankTransactionOutput()
            {
                
                CreationTimeIndex = u.Journal.CreationTimeIndex,
                Customer = u.Customers.ToList(),
                User = ObjectMapper.Map<UserDto>(u.Journal.CreatorUser),           
                //CurrencyId = u.Journal.CurrencyId,
                Memo = u.Journal.Memo,
                Id = u.Journal.ReceivePaymentId.Value,
                Date = u.Journal.Date,
                JournalNo = u.Journal.JournalNo,
                TotalPayment = u.receivePayments.TotalPayment,
                TypeName = Enum.GetName(u.Journal.JournalType.GetType(), u.Journal.JournalType),
                Type = u.Journal.JournalType,
               
                //TotalPaymentInvoice = u.receivePayments.TotalPaymentInvoice,
                //TotalCustomerCreditPayment = u.receivePayments.TotalPaymentCustomerCredit,
                Status = u.Journal.Status,
                TotalAmount = u.receivePayments.TotalOpenBalance,
                //ReceiveFrom = u.receivePayments.ReceiveFrom,
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



#if  oldCode

            var queryRceivepayment = (from jItem in _journalItemRepository.GetAll().Include(u => u.Account).Include(t => t.Journal).Include(u => u.Account.AccountType)
                                        .Where(t => t.Account.AccountType.AccountTypeName == "Cash" || t.Account.AccountType.AccountTypeName == "Bank")
                                        .AsNoTracking()
                                        join j in _journalRepository.GetAll().AsNoTracking()
                                        
                                        .Where(u => u.JournalType == JournalType.ReceivePayment)
                                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                        .WhereIf(input.FromDate != null && input.ToDate != null,
                                         (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                 u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                                 u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                      on jItem.JournalId equals j.Id
                                      join l in _locationRepository.GetAll().AsNoTracking()
                                      on j.LocationId equals l.Id
                                      join pb in _receivePaymentRepository.GetAll().AsNoTracking()                                       
                                      on jItem.Journal.ReceivePaymentId equals pb.Id
                                      join rpd in _receivePaymentDetailRepository.GetAll().Include(u => u.Invoice).AsNoTracking()
                                     .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                      on pb.Id equals rpd.ReceivePaymentId
                                      join c in _customerRepository.GetAll().AsNoTracking() on rpd.CustomerId equals c.Id
                                      join u in _userRepository.GetAll().AsNoTracking() on j.CreatorUserId equals u.Id
                                      group jItem by new { journal = j, receivPayment = pb, user = u,location = l, customer = c } into u
                                      select new GetlistBankTransactionOutput
                                      {
                                          Location = ObjectMapper.Map<LocationSummaryOutput>(u.Key.location),
                                          CreationTimeIndex = u.Key.journal.CreationTimeIndex,
                                          User = ObjectMapper.Map<UserDto>(u.Key.user),
                                          Type = u.Key.journal.JournalType,
                                          TypeName = Enum.GetName(u.Key.journal.JournalType.GetType(), u.Key.journal.JournalType),
                                          Memo = u.Key.journal.Memo,
                                          Id = u.Key.receivPayment.Id,
                                          Date = u.Key.journal.Date,
                                          JournalNo = u.Key.journal.JournalNo,
                                          TotalPayment = u.Key.receivPayment.TotalPayment,
                                          Status = u.Key.journal.Status,
                                          TotalAmount = u.Key.receivPayment.TotalOpenBalance,
                                          AccountName = u.Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Payment && t.Identifier == null)
                                                  .Select(t => t.Account.AccountName).FirstOrDefault(),
                                          AccountId = u.Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Payment && t.Identifier == null)
                                                  .Select(t => t.AccountId).FirstOrDefault(),

                                          Customer = u.Key.customer != null ? new List<CustomerSummaryOutput>() {
                                                new CustomerSummaryOutput
                                                {
                                                    Id = u.Key.customer.Id,
                                                    CustomerName = u.Key.customer.CustomerName,
                                                    CustomerCode = u.Key.customer.CustomerCode
                                                }
                                            } : new List<CustomerSummaryOutput>(),

                                         
                                      })
                                      .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId));
#endif
            //Deposit 
            var queryDeposit = (from jItem in _journalItemRepository.GetAll()
                                .Include(u => u.Account)
                                .Include(t => t.Journal)
                                .Include(u => u.Account.AccountType)
                                
                                .AsNoTracking()
                                join j in _journalRepository.GetAll()
                               
                                .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                .Where(u => u.JournalType == JournalType.Deposit)

                                .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                .WhereIf(input.FromDate != null && input.ToDate != null,
                                    (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                .WhereIf(!input.Filter.IsNullOrEmpty(),
                                   u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                .AsNoTracking()
                                on jItem.JournalId equals j.Id

                                join l in _locationRepository.GetAll().AsNoTracking()
                                on j.LocationId equals l.Id

                                join dp in _depositRepository.GetAll().AsNoTracking()                                
                                on j.DepositId equals dp.Id

                                join pdi in _depositItemRepository.GetAll()
                                .Include(u => u.Deposit).AsNoTracking()
                                on dp.Id equals pdi.DepositId

                                join u in _userRepository.GetAll().AsNoTracking() 
                                on j.CreatorUserId equals u.Id

                                join vendor in _vendorRepository.GetAll().AsNoTracking()
                                on dp.ReceiveFromVendorId equals vendor.Id
                                into v
                                from vendor in v.DefaultIfEmpty()


                                join customer in _customerRepository.GetAll().AsNoTracking()
                                on dp.ReceiveFromCustomerId equals customer.Id
                                into c
                                from customer in c.DefaultIfEmpty()


                                group jItem by new { journal = j, deposit = dp, user = u, customer = customer, vendor = vendor,location = l } into u
                                select new GetlistBankTransactionOutput
                                {
                                    Location = ObjectMapper.Map<LocationSummaryOutput>(u.Key.location),
                                    CreationTimeIndex = u.Key.journal.CreationTimeIndex,
                                    User = ObjectMapper.Map<UserDto>(u.Key.user),
                                    Type = u.Key.journal.JournalType,
                                    TypeName = Enum.GetName(u.Key.journal.JournalType.GetType(), u.Key.journal.JournalType),
                                    Memo = u.Key.journal.Memo,
                                    Id = u.Key.deposit.Id,
                                    Date = u.Key.journal.Date,
                                    JournalNo = u.Key.journal.JournalNo,
                                    TotalPayment = u.Key.deposit.Total,
                                    Status = u.Key.journal.Status,
                                    TotalAmount = u.Key.deposit.Total,
                                    CanVoidDraft = u.Key.deposit.BankTransferId != null ? true : false,
                                    AccountName = u
                                            .Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Bank && t.Identifier == null)
                                            .Select(t => t.Account.AccountName).FirstOrDefault(),
                                    AccountId = u.Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Bank && t.Identifier == null)
                                            .Select(t => t.AccountId).FirstOrDefault(),
                                    Customer = u.Key.customer != null ? new List<CustomerSummaryOutput>() {
                                                new CustomerSummaryOutput
                                                {
                                                    Id = u.Key.customer.Id,
                                                    CustomerName = u.Key.customer.CustomerName,
                                                    CustomerCode = u.Key.customer.CustomerCode
                                                }
                                            } : new List<CustomerSummaryOutput>(),

                                    Vendor = u.Key.vendor != null ? new List<VendorSummaryOutput>() {
                                                new VendorSummaryOutput
                                                {
                                                    Id = u.Key.vendor.Id,
                                                    VendorName = u.Key.vendor.VendorName,
                                                    VendorCode = u.Key.vendor.VendorCode
                                                }
                                            } : new List<VendorSummaryOutput>()
                                   
                                })
                                .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId));
            // Withdraw 
            var queryWithdraw = (from jItem in _journalItemRepository.GetAll().Include(u => u.Account).Include(t => t.Journal)
                                .Include(u => u.Account.AccountType)
                                .AsNoTracking()
                                join j in _journalRepository.GetAll()
                                .Include(u => u.Location).AsNoTracking()
                                .Where(u => u.JournalType == JournalType.Withdraw)
                                .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                .WhereIf(input.FromDate != null && input.ToDate != null,
                                    (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                on jItem.JournalId equals j.Id

                                 join l in _locationRepository.GetAll().AsNoTracking()
                                 on j.LocationId equals l.Id

                                 join wid in _withdrawRepository.GetAll().AsNoTracking()                                   
                                on j.WithdrawId equals wid.Id

                                join pdi in _withdrawItemRepository.GetAll().Include(u => u.Withdraw).AsNoTracking()
                                on wid.Id equals pdi.WithdrawId

                                 join u in _userRepository.GetAll().AsNoTracking()
                                 on j.CreatorUserId equals u.Id

                                 join vendor in _vendorRepository.GetAll().AsNoTracking()
                                 on wid.VendorId equals vendor.Id
                                 into v
                                 from vendor in v.DefaultIfEmpty()

                                 join customer in _customerRepository.GetAll().AsNoTracking()
                                 on wid.CustomerId equals customer.Id
                                 into c
                                 from customer in c.DefaultIfEmpty()

                                 group jItem by new { journal = j, withdraw = wid, user = u, customer = customer, vendor = vendor,location = l } into u
                                select new GetlistBankTransactionOutput
                                {
                                    Location = ObjectMapper.Map<LocationSummaryOutput>(u.Key.location),
                                    CreationTimeIndex = u.Key.journal.CreationTimeIndex,
                                    User = ObjectMapper.Map<UserDto>(u.Key.user),
                                    Type = u.Key.journal.JournalType,
                                    TypeName = Enum.GetName(u.Key.journal.JournalType.GetType(), u.Key.journal.JournalType),
                                    Memo = u.Key.journal.Memo,
                                    Id = u.Key.withdraw.Id,
                                    Date = u.Key.journal.Date,
                                    JournalNo = u.Key.journal.JournalNo,
                                    TotalPayment = u.Key.withdraw.Total,
                                    Status = u.Key.journal.Status,
                                    TotalAmount = u.Key.withdraw.Total,

                                    AccountName = u
                                            .Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Bank && t.Identifier == null)
                                            .Select(t => t.Account.AccountName).FirstOrDefault(),
                                    AccountId = u.Where(t => t.JournalId == u.Key.journal.Id && t.Key == PostingKey.Bank && t.Identifier == null)
                                            .Select(t => t.AccountId).FirstOrDefault(),
                                    CanVoidDraft = u.Key.withdraw.BankTransferId != null ? true : false,
                                  
                                    Customer = u.Key.customer != null ? new List<CustomerSummaryOutput>() {
                                                new CustomerSummaryOutput
                                                {
                                                    Id = u.Key.customer.Id,
                                                    CustomerName = u.Key.customer.CustomerName,
                                                    CustomerCode = u.Key.customer.CustomerCode
                                                }
                                            } : new List<CustomerSummaryOutput>(),

                                    Vendor = u.Key.vendor != null ? new List<VendorSummaryOutput>() {
                                                new VendorSummaryOutput
                                                {
                                                    Id = u.Key.vendor.Id,
                                                    VendorName = u.Key.vendor.VendorName,
                                                    VendorCode = u.Key.vendor.VendorCode
                                                }
                                            } : new List<VendorSummaryOutput>()
                                })
                                .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId));
            
            var result = queryPaybill.Union(queryRceivepayment).Union(queryDeposit).Union(queryWithdraw);
            if (input.Vendors != null && input.Customers != null)
            {
                result = result.Where(t=> t.Customer != null || t.Vendor != null);
                result.WhereIf(input.Vendors != null && input.Vendors.Count > 0, p => p.Vendor.Any(i => input.Vendors.Contains(i.Id)))
                      .WhereIf(input.Customers != null && input.Customers.Count > 0, p => p.Customer.Any(i => input.Customers.Contains(i.Id)));
            }
            if (input.Customers != null && input.Vendors == null)
            {
                result = null;
                result = queryRceivepayment;
            }
            if (input.Customers == null && input.Vendors != null)
            {
                result = null;
                result = queryPaybill.Union(queryDeposit).Union(queryWithdraw).Where(t=> t.Vendor != null);
                result = result.Where(p => p.Vendor.Any(i => input.Vendors.Contains(i.Id)));
            }
            var resultCount = await result.CountAsync();

            List<GetlistBankTransactionOutput> @entities;

            if (input.Sorting.Contains("date") && !input.Sorting.Contains(".")) input.Sorting = input.Sorting.Replace("date", "Date.Date");

            if (input.Sorting.Contains("DESC"))
            {
                @entities = await result.OrderBy(input.Sorting).ThenByDescending(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            }
            else
            {
                @entities = await result.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            }



            return new PagedResultDto<GetlistBankTransactionOutput>(resultCount, ObjectMapper.Map<List<GetlistBankTransactionOutput>>(@entities));
        }
    }
}
