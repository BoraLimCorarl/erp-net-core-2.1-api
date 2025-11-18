using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using CorarlERP.AccountCycles;
using CorarlERP.AccountTrasactionCloses;
using CorarlERP.Bills;
using CorarlERP.Invoices;
using CorarlERP.Journals;
using CorarlERP.PayBills;
using CorarlERP.ReceivePayments;
using CorarlERP.VendorCustomerOpenBalances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorCustomerOpenningBalance
{
    public class VendorOpenningBalanceManager : CorarlERPDomainServiceBase, IVendorOpenningBalanceManager
    {
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<PayBillDetail, Guid> _payBillItemRepository;
        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<AccountTransactionClose, Guid> _accountTransactionRepository;
        private readonly IRepository<VendorOpenBalance, Guid> _vendorOpenBalanceRepository;


        public VendorOpenningBalanceManager(
            IRepository<Journal, Guid> journalRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<AccountTransactionClose, Guid> accountTransactionRepository,
            IRepository<PayBillDetail, Guid> payBillItemRepository,
            IRepository<Bill, Guid> billRepository,
            IRepository<VendorOpenBalance, Guid> vendorOpenBalanceRepository
        ) : base(accountCycleRepository)
        {
            _billRepository = billRepository;
            _journalRepository = journalRepository;
            _payBillItemRepository = payBillItemRepository;
            _journalItemRepository = journalItemRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _vendorOpenBalanceRepository = vendorOpenBalanceRepository;
        }

        public IQueryable<TransactionOpenningBalanceOutput> GetOpenTransactionQuery(DateTime? fromDate, DateTime toDate, List<long>locations = null)
        {            
            if (fromDate != null)
            {
                var transactionFromDate = _journalRepository
                                     .GetAll()
                                     .Where(u => u.JournalType == JournalType.Bill || u.JournalType == JournalType.VendorCredit)
                                     .Where(s => s.BillId.HasValue || s.VendorCreditId.HasValue)
                                     .Where(u => u.Status == TransactionStatus.Publish)
                                     .WhereIf(fromDate != null && toDate != null,
                                        (u => u.Date.Date >= fromDate.Value.Date && u.Date.Date <= toDate.Date))
                                     .WhereIf(locations != null && locations.Count() > 0, u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                     .AsNoTracking()
                                     .Select(u => new 
                                     {
                                         TransactionId = u.BillId != null ? u.BillId : u.VendorCreditId,
                                         Key = u.JournalType,
                                         LocationId = u.LocationId,
                                         Total = u.BillId != null ? u.Bill.Total : u.VendorCredit.Total,
                                         MultiCurrencyTotal = u.BillId != null ? u.Bill.MultiCurrencyTotal : u.VendorCredit.MultiCurrencyTotal 
                                     });
                

                var billPaymentItems = from pbi in _payBillItemRepository.GetAll().AsNoTracking()
                                       join j in _journalRepository.GetAll()
                                                .WhereIf(fromDate != null && toDate != null,
                                                    (u => u.Date.Date >= fromDate.Value.Date && u.Date.Date <= toDate.Date))
                                                .AsNoTracking()
                                       on pbi.PayBillId equals j.PayBillId
                                       select new
                                       {
                                           TransactionId = pbi.BillId,
                                           VendorCrecitId = pbi.VendorCreditId,
                                           PaymentAmount = pbi.Payment,
                                           MultiCurrencyPaymentAmount = pbi.MultiCurrencyPayment,
                                       };

                var transactionPyamnets = from t in transactionFromDate

                                          join pb in billPaymentItems
                                          on t.TransactionId equals pb.TransactionId
                                          into pbs
                                          where pbs != null

                                          join vcp in billPaymentItems.Where(s => s.VendorCrecitId != null)
                                          on t.TransactionId equals vcp.TransactionId
                                          into vcps
                                          where vcps != null

                                          select new TransactionOpenningBalanceOutput
                                          {
                                              TransactionId = t.TransactionId,
                                              Key = t.Key,
                                              LocationId = t.LocationId.Value,
                                              Balance = t.Total - (pbs != null ? pbs.Sum(s => s.PaymentAmount) : vcps.Sum(s => s.PaymentAmount)),
                                              MultiCurrencyBalance = t.MultiCurrencyTotal - (pbs != null ? pbs.Sum(s => s.MultiCurrencyPaymentAmount) : vcps.Sum(s => s.MultiCurrencyPaymentAmount))
                                          };

                var result = transactionPyamnets.Where(s => s.Balance > 0);

                return result;

            }
            else
            {

                var previousCycle = GetPreviousCloseCyleAsync(toDate);

                var transactionClose = _vendorOpenBalanceRepository.GetAll().Include(s => s.AccountCycle)
                                        .Where(t => previousCycle != null && t.AccountCycleId == previousCycle.Id)
                                        .WhereIf(locations != null && locations.Count() > 0, s => locations.Contains(s.LocationId))
                                        .AsNoTracking()
                                        .Select(s => new 
                                        {
                                            TransactionId = (Guid?) s.TransactionId,
                                            Key = s.Key,
                                            LocationId = (long?) s.LocationId,
                                            Total = s.Balance,
                                            MultiCurrencyTotal = s.MuliCurrencyBalance,
                                        });

                var transactionFromDate = _journalRepository
                                     .GetAll()
                                     .Where(u => u.JournalType == JournalType.Bill || u.JournalType == JournalType.VendorCredit)
                                     .Where(u => u.BillId.HasValue || u.VendorCreditId.HasValue)
                                     .Where(u => u.Status == TransactionStatus.Publish)
                                     .Where(u => previousCycle == null || u.Date.Date > previousCycle.EndDate.Value)
                                     .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)                                       
                                     .WhereIf(locations != null && locations.Count() > 0, u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                     .AsNoTracking()
                                     .Select(u => new
                                     {
                                         TransactionId = u.BillId != null ? u.BillId : u.VendorCreditId,
                                         Key = u.JournalType,
                                         LocationId = u.LocationId,
                                         Total = u.BillId != null ? u.Bill.Total : u.VendorCredit.Total,
                                         MultiCurrencyTotal = u.BillId != null ? u.Bill.MultiCurrencyTotal : u.VendorCredit.MultiCurrencyTotal
                                     });

                var billPaymentItems = from pbi in _payBillItemRepository.GetAll().AsNoTracking()
                                       join j in _journalRepository.GetAll()
                                                .Where(u => previousCycle == null || u.Date.Date > previousCycle.EndDate.Value)
                                                .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                                .AsNoTracking()
                                        on pbi.PayBillId equals j.PayBillId
                                       select new
                                       {
                                           TransactionId = pbi.BillId,
                                           VendorCrecitId = pbi.VendorCreditId,
                                           PaymentAmount = pbi.Payment,
                                           MultiCurrencyPaymentAmount = pbi.MultiCurrencyPayment,
                                       };
                
                var transactionPyamnets = from t in transactionFromDate.Union(transactionClose)

                                          join pb in billPaymentItems.Where(s => s.TransactionId != null)
                                          on t.TransactionId equals pb.TransactionId
                                          into pbs
                                          where pbs != null

                                          join vcp in billPaymentItems.Where(s => s.VendorCrecitId != null)
                                          on t.TransactionId equals vcp.VendorCrecitId
                                          into vcps
                                          where vcps != null

                                          let billPaymentAmount = pbs != null ? pbs.Sum(s => s.PaymentAmount) : 0
                                          let vendorCreditPaymentAmount = vcps != null ? vcps.Sum(s => s.PaymentAmount) : 0
                                          let totalPaymentAmount = billPaymentAmount + vendorCreditPaymentAmount

                                          let billMultiPaymentAmount = pbs != null ? pbs.Sum(s => s.MultiCurrencyPaymentAmount) : 0
                                          let vendorCreditMultiPaymentAmount = vcps != null ? vcps.Sum(s => s.MultiCurrencyPaymentAmount) : 0
                                          let totalMultiPaymentAmount = billMultiPaymentAmount + vendorCreditMultiPaymentAmount 

                                          select new TransactionOpenningBalanceOutput
                                          {
                                              TransactionId = t.TransactionId,
                                              Key = t.Key,
                                              LocationId = t.LocationId.Value,
                                              Balance = t.Total - totalPaymentAmount,
                                              MultiCurrencyBalance = t.MultiCurrencyTotal - totalMultiPaymentAmount,
                                          };

                var result = transactionPyamnets.Where(s => s.Balance > 0);

                return result;
                

            }


        }
        

    }

    public class CustomerOpenningBalanceManager : CorarlERPDomainServiceBase, ICustomerOpenningBalanceManager
    {
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<ReceivePaymentDetail, Guid> _receivePaymentItemRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<AccountTransactionClose, Guid> _accountTransactionRepository;
        private readonly IRepository<CustomerOpenBalance, Guid> _customerOpenBalanceRepository;


        public CustomerOpenningBalanceManager(
            IRepository<Journal, Guid> journalRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<AccountTransactionClose, Guid> accountTransactionRepository,
            IRepository<ReceivePaymentDetail, Guid> receivePaymentItemRepository,
            IRepository<Invoice, Guid> invoiceRepository,
            IRepository<CustomerOpenBalance, Guid> customerOpenBalanceRepository
        ) : base(accountCycleRepository)
        {
            _invoiceRepository = invoiceRepository;
            _receivePaymentItemRepository = receivePaymentItemRepository;
            _journalRepository = journalRepository;
            _journalItemRepository = journalItemRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _customerOpenBalanceRepository = customerOpenBalanceRepository;
        }

        public IQueryable<TransactionOpenningBalanceOutput> GetOpenTransactionQuery(DateTime? fromDate, DateTime toDate, List<long> locations = null)
        {
            if (fromDate != null)
            {
                var transactionFromDate = _journalRepository
                                     .GetAll()
                                     .Where(u => 
                                        u.JournalType == JournalType.Invoice ||
                                        u.JournalType == JournalType.CustomerCredit)
                                     .Where(s => s.InvoiceId.HasValue || s.CustomerCreditId.HasValue)
                                     .Where(u => u.Status == TransactionStatus.Publish)
                                     .WhereIf(fromDate != null && toDate != null,
                                        (u => u.Date.Date >= fromDate.Value.Date && u.Date.Date <= toDate.Date))
                                     .WhereIf(locations != null && locations.Count() > 0, u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                     .AsNoTracking()
                                     .Select(u => new
                                     {
                                         TransactionId = u.InvoiceId != null ? u.InvoiceId : u.CustomerCreditId,
                                         Key = u.JournalType,
                                         LocationId = u.LocationId,
                                         Total = u.InvoiceId != null ? u.Invoice.Total : u.CustomerCredit.Total,
                                         MultiCurrencyTotal = u.InvoiceId != null ? u.Invoice.MultiCurrencyTotal : u.CustomerCredit.MultiCurrencyTotal,
                                     });

                var inovicePaymentItems = from pbi in _receivePaymentItemRepository.GetAll().Include(s => s.ReceivePayment).AsNoTracking()

                                          join j in _journalRepository.GetAll()
                                           .WhereIf(fromDate != null && toDate != null,
                                               (u => u.Date.Date >= fromDate.Value.Date && u.Date.Date <= toDate.Date))
                                           .AsNoTracking()
                                           on pbi.ReceivePaymentId equals j.ReceivePaymentId
                                          select new
                                          {
                                              TransactionId = pbi.InvoiceId,
                                              CustomerCrecitId = pbi.CustomerCreditId,
                                              PaymentAmount = pbi.Payment,
                                              MultiCurrencyPaymentAmount = pbi.MultiCurrencyPayment,
                                          };


                var transactionPyamnets = from t in transactionFromDate

                                          join ip in inovicePaymentItems
                                          on t.TransactionId equals ip.TransactionId
                                          into ips
                                          where ips != null

                                          join ccp in inovicePaymentItems.Where(s => s.CustomerCrecitId != null)
                                          on t.TransactionId equals ccp.TransactionId
                                          into ccps
                                          where ccps != null

                                          select new TransactionOpenningBalanceOutput
                                          {
                                              TransactionId = t.TransactionId,
                                              Key = t.Key,
                                              LocationId = t.LocationId.Value,
                                              Balance = t.Total - (
                                                                    ips != null ? ips.Sum(s => s.PaymentAmount) : 
                                                                    ccps != null ? ccps.Sum(s => s.PaymentAmount) : 0),
                                              MultiCurrencyBalance = t.MultiCurrencyTotal - (
                                                                    ips != null ? ips.Sum(s => s.MultiCurrencyPaymentAmount) :
                                                                    ccps != null ? ccps.Sum(s => s.MultiCurrencyPaymentAmount) : 0),
                                          };

                var result = transactionPyamnets.Where(s => s.Balance > 0);

                return result;

            }
            else
            {

                var previousCycle = GetPreviousCloseCyleAsync(toDate);

                var transactionClose = _customerOpenBalanceRepository.GetAll().Include(s => s.AccountCycle)
                                        .Where(t => previousCycle != null && t.AccountCycleId == previousCycle.Id)
                                        .WhereIf(locations != null && locations.Count() > 0, s => locations.Contains(s.LocationId))
                                        .AsNoTracking()
                                        .Select(s => new
                                        {
                                            TransactionId = (Guid?)s.TransactionId,
                                            Key = s.Key,
                                            LocationId = (long?)s.LocationId,
                                            Total = s.Balance,
                                            MultiCurrencyTotal = s.MuliCurrencyBalance,
                                        });


                var transactionFromDate = _journalRepository
                                     .GetAll()
                                     .Where(u => 
                                        u.JournalType == JournalType.Invoice ||
                                        u.JournalType == JournalType.CustomerCredit)
                                     .Where(s => s.InvoiceId.HasValue || s.CustomerCreditId.HasValue)
                                     .Where(u => u.Status == TransactionStatus.Publish)
                                     .Where(u => previousCycle == null || u.Date.Date > previousCycle.EndDate.Value)
                                     .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                     .WhereIf(locations != null && locations.Count() > 0, u => u.LocationId != null && locations.Contains(u.LocationId.Value))
                                     .AsNoTracking()
                                     .Select(u => new
                                     {
                                         TransactionId = u.InvoiceId != null ? u.InvoiceId : u.CustomerCreditId,
                                         Key = u.JournalType,
                                         LocationId = u.LocationId,
                                         Total = u.InvoiceId != null ? u.Invoice.Total : u.CustomerCredit.Total,
                                         MultiCurrencyTotal = u.InvoiceId != null ? u.Invoice.MultiCurrencyTotal : u.CustomerCredit.MultiCurrencyTotal,
                                     });

                var inovicePaymentItems = from pbi in _receivePaymentItemRepository.GetAll().Include(s => s.ReceivePayment).AsNoTracking()

                                          join j in _journalRepository.GetAll()
                                            .Where(u => previousCycle == null || u.Date.Date > previousCycle.EndDate.Value)
                                            .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                            .AsNoTracking()
                                           on pbi.ReceivePaymentId equals j.ReceivePaymentId
                                          select new
                                          {
                                              TransactionId = pbi.InvoiceId,
                                              CustomerCrecitId = pbi.CustomerCreditId,
                                              PaymentAmount = pbi.Payment,
                                              MultiCurrencyPaymentAmount = pbi.MultiCurrencyPayment,
                                          };


                var transactionPyamnets = from t in transactionFromDate.Union(transactionClose)

                                          join ip in inovicePaymentItems.Where(s => s.TransactionId != null)
                                          on t.TransactionId equals ip.TransactionId
                                          into ips
                                          where ips != null

                                          join ccp in inovicePaymentItems.Where(s => s.CustomerCrecitId != null)
                                          on t.TransactionId equals ccp.CustomerCrecitId
                                          into ccps
                                          where ccps != null

                                          let invoicePaymentAmount = ips != null ? ips.Sum(s => s.PaymentAmount) : 0
                                          let customerCreditPaymentAmount = ccps != null ? ccps.Sum(s => s.PaymentAmount) : 0
                                          let totalPaymentAmount = invoicePaymentAmount + customerCreditPaymentAmount

                                          let invoiceMultiPaymentAmount = ips != null ? ips.Sum(s => s.MultiCurrencyPaymentAmount) : 0
                                          let customerCreditMultiPaymentAmount = ccps != null ? ccps.Sum(s => s.MultiCurrencyPaymentAmount) : 0
                                          let totalMultiPaymentAmount = invoiceMultiPaymentAmount + customerCreditMultiPaymentAmount

                                          select new TransactionOpenningBalanceOutput
                                          {
                                              TransactionId = t.TransactionId,
                                              Key = t.Key,
                                              LocationId = t.LocationId.Value,
                                              Balance = t.Total - totalPaymentAmount,
                                              MultiCurrencyBalance = t.MultiCurrencyTotal - totalMultiPaymentAmount,
                                          };

                var result = transactionPyamnets.Where(s => s.Balance > 0);

                return result;


            }


        }


    }
}
