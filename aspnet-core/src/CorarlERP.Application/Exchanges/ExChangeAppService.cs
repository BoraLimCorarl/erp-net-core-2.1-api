using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using CorarlERP.Exchanges.Dto;
using CorarlERP.ExChanges;
using Abp.Runtime.Session;
using Abp.UI;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Currencies;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Currencies.Dto;
using System.Collections.Generic;
using CorarlERP.Authorization;
using Abp.Authorization;
using Abp.Domain.Uow;
using System.Transactions;
using CorarlERP.Journals;
using CorarlERP.Invoices;
using CorarlERP.ReceivePayments;
using CorarlERP.MultiTenancy;
using CorarlERP.AccountCycles;
using CorarlERP.CustomerCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace CorarlERP.Exchanges
{
    [AbpAuthorize]
    public class ExChangeAppService : CorarlERPAppServiceBase, IExChangeAppService
    {

        private readonly IExchangeManager _exchangeManager;
        private readonly IRepository<Exchange, Guid> _exchangeRepository;

        private readonly IExchangeItemManager _exchangeItemManager;
        private readonly IRepository<ExchangeItem, Guid> _exchangeItemRepository;

        private readonly ICorarlRepository<Journal, Guid> _journalRepository;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;
        private readonly ICorarlRepository<Invoice, Guid> _invoiceRepository;
        private readonly ICorarlRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly ICorarlRepository<ReceivePayment, Guid> _receivePaymentRepository;
        private readonly ICorarlRepository<ReceivePaymentDetail, Guid> _receivePaymentItemRepository;
        private readonly ICorarlRepository<ReceivePaymentExpense, Guid> _receivePaymentExpenseRepository;
        private readonly ICorarlRepository<CustomerCredit, Guid> _customerCreditRepository;
        private readonly ICorarlRepository<CustomerCreditDetail, Guid> _customerCreditItemRepository;
        private readonly ICorarlRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public ExChangeAppService(IExchangeManager exchangeManager,
                                  IRepository<Exchange, Guid> exchangeRepository,
                                  IExchangeItemManager exchangeItemManager,
                                  IUnitOfWorkManager unitOfWorkManager,
                                  ICorarlRepository<Journal, Guid> journalRepository,
                                  ICorarlRepository<JournalItem, Guid> journalItemRepository,
                                  ICorarlRepository<Invoice, Guid> invoiceRepository,
                                  ICorarlRepository<InvoiceItem, Guid> invoiceItemRepository,
                                  ICorarlRepository<ReceivePayment, Guid> receivePaymentRepository,
                                  ICorarlRepository<ReceivePaymentDetail, Guid> receivePaymentItemRepository,
                                  ICorarlRepository<ReceivePaymentExpense, Guid> receivePaymentExpenseRepository,
                                  ICorarlRepository<CustomerCredit, Guid> customerCreditRepository,
                                  ICorarlRepository<CustomerCreditDetail, Guid> customerCreditItemRepository,
                                  ICorarlRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
                                  ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
        IRepository<ExchangeItem, Guid> exchangeItemRepository)
        {
            _exchangeItemManager = exchangeItemManager;
            _exchangeItemRepository = exchangeItemRepository;
            _exchangeManager = exchangeManager;
            _exchangeRepository = exchangeRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _journalRepository = journalRepository;
            _journalItemRepository = journalItemRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _receivePaymentRepository = receivePaymentRepository;
            _receivePaymentItemRepository = receivePaymentItemRepository;
            _receivePaymentExpenseRepository = receivePaymentExpenseRepository;
            _customerCreditRepository = customerCreditRepository;
            _customerCreditItemRepository = customerCreditItemRepository;
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateExchangeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = Exchange.Create(tenantId, userId, input.FromDate, input.ToDate);

            #region Items


            //validate from date Create

            var lastCreate = await _exchangeRepository.GetAll().AsNoTracking().OrderByDescending(t => t.ToDate).Select(u => u.ToDate.Date).FirstOrDefaultAsync();

            if(lastCreate >= input.FromDate.Date)
            {
                throw new UserFriendlyException(L("CanNotCreateThesameDate"));
            }
            else if(input.FromDate > input.ToDate)
            {
                throw new UserFriendlyException(L("CanNotCreateByFromDateBiggerthanToDate"));
            }



            //validate the same currency 
            var sameCurrency = input.ExchangeItem.Where(t => t.FromCurrencyId == t.ToCurrencyId);

            if (sameCurrency.Count() > 0)
            {
                throw new UserFriendlyException(L("CanNoExchangeTheSameCurrency"));
            }
            //validate currency = null

            var nullCurrecny = input.ExchangeItem.Where(t => t.FromCurrencyId == 0 || t.ToCurrencyId == 0);
            if (nullCurrecny.Count() > 0)
            {
                throw new UserFriendlyException(L("PleaseSelectCurrency"));
            }

            //validate
            var exchangeByIndex = input.ExchangeItem.Select((val, index) => new { From = val.FromCurrencyId, To = val.ToCurrencyId, Index = index }).ToList();

            for (int i = 0; i < exchangeByIndex.Count(); i++)
            {
                var item = exchangeByIndex[i];
                if (exchangeByIndex.Any(a => a.Index != i && ((a.From == item.From && a.To == item.To) || (a.From == item.To && a.To == item.From))))
                    throw new UserFriendlyException(L("DuplicateCurrency"));
            }
            
            foreach (var ex in input.ExchangeItem)
            {

                var @exchangeItem = ExchangeItem.Create(tenantId, userId, ex.FromCurrencyId, ex.ToCurrencyId, ex.Bid, ex.Ask, entity);
                CheckErrors(await _exchangeItemManager.CreateAsync(@exchangeItem));

            }
            #endregion

            CheckErrors(await _exchangeManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _exchangeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var ItemPriceItems = await _exchangeItemRepository.GetAll().Where(u => u.ExchangeId == entity.Id).ToListAsync();

            foreach (var s in ItemPriceItems)
            {
                CheckErrors(await _exchangeItemManager.RemoveAsync(s));
            }

            CheckErrors(await _exchangeManager.RemoveAsync(@entity));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_Find)]
        public async Task<ListResultDto<GetExchangeRateDto>> Find(FindExchangeInput input)
        {
            var query = _exchangeItemRepository.GetAll()
                        .Where(u => u.Exchange.FromDate.Date <= input.Date.Date && input.Date.Date <= u.Exchange.ToDate.Date)
                        .Select(t => new GetExchangeRateDto
                        {
                            Id = t.Id,
                            FromCurrencyId = t.FromCurrencyId,
                            FromCurrencyCode = t.FromCurrency.Code,
                            ToCurrencyId = t.ToCurencyId,
                            ToCurrencyCode = t.ToCurrency.Code,
                            Ask = t.Ask,
                            Bid = t.Bid,
                        });
            
            var @entities = await query.ToListAsync();

            return new ListResultDto<GetExchangeRateDto>(@entities);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_Find)]
        public async Task<GetExchangeRateDto> GetExchangeRate(GetExchangeInput input)
        {

            var @exchange = await _exchangeItemRepository.GetAll()
                                .AsNoTracking()
                                .Where(u => (u.ToCurencyId == input.ToCurrency && u.FromCurrencyId == input.FromCurrency) || (u.ToCurencyId == input.FromCurrency && u.FromCurrencyId == input.ToCurrency))
                                .Where(u => input.Date.Date != null && u.Exchange.FromDate.Date <= input.Date.Date && input.Date.Date <= u.Exchange.ToDate)
                                .Select (exi => new GetExchangeRateDto
                                {

                                    Id = exi.Id,
                                    FromCurrencyId = exi.FromCurrencyId,
                                    FromCurrencyCode = exi.FromCurrency.Code,
                                    ToCurrencyId = exi.ToCurencyId,
                                    ToCurrencyCode = exi.ToCurrency.Code,
                                    Ask = exi.Ask,
                                    Bid = exi.Bid,
                                    IsInves = exi.FromCurrency.Id != input.FromCurrency,
                                })
                                .FirstOrDefaultAsync();

            return @exchange;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_GetDetail)]
        public async Task<GetDetailExchangeOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _exchangeManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var itemPriceItems = await(from exi in _exchangeItemRepository.GetAll()
                                       .Include(u => u.FromCurrency)
                                       .Include(u=>u.ToCurrency)
                                       .AsNoTracking()
                                       where(exi.ExchangeId == input.Id)
                                       select new CreateOrUpdateExchangeInput
                                       {
                                            FromCurrency = new CurrencyDetailOutput {
                                                Code = exi.FromCurrency.Code,
                                                Id = exi.FromCurrency.Id,
                                                Name = exi.FromCurrency.Name,
                                                PluralName = exi.FromCurrency.PluralName,
                                                Symbol = exi.FromCurrency.Symbol,
                                            },
                                            FromCurrencyId = exi.FromCurrencyId,
                                            Id = exi.Id,
                                           ToCurrency = new CurrencyDetailOutput
                                           {
                                               Code = exi.ToCurrency.Code,
                                               Id = exi.ToCurrency.Id,
                                               Name = exi.ToCurrency.Name,
                                               PluralName = exi.ToCurrency.PluralName,
                                               Symbol = exi.ToCurrency.Symbol,
                                           },
                                           ToCurrencyId = exi.ToCurencyId,
                                            Bid = exi.Bid,
                                            Ask = exi.Ask,

                                       }).ToListAsync();

            var result = ObjectMapper.Map<GetDetailExchangeOutput>(@entity);

            result.ExchangeItems = ObjectMapper.Map<List<CreateOrUpdateExchangeInput>>(itemPriceItems);

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_GetList)]
        public async Task<PagedResultDto<GetListExchangeOutput>> GetList(GetListExchangeInput input)
        {
            var query = _exchangeRepository.GetAll()

                             .WhereIf(input.FromDate != null && input.ToDate != null,
                                      u => input.FromDate.Date <= u.ToDate.Date && u.FromDate.Date <= input.ToDate.Date)

                             .Select(t => new GetListExchangeOutput
                             {
                                 Id = t.Id,
                                 ToDate = t.ToDate,
                                 FromDate = t.FromDate,
                             });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListExchangeOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateExchangeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();


            var @entity = await _exchangeManager.GetAsync(input.Id, true); //this is journal



            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }


            //validate from date Create

            var lastCreate = await _exchangeRepository.GetAll()
                             .AsNoTracking()
                             .Where(t=>t.Id != entity.Id)
                             .OrderByDescending(t => t.ToDate)
                             .Select(u => u.ToDate)
                             .FirstOrDefaultAsync();

            if (lastCreate >= input.FromDate)
            {
                throw new UserFriendlyException(L("CanNotCreateThesameDate"));
            }
            else if (input.FromDate > input.ToDate)
            {
                throw new UserFriendlyException(L("CanNotCreateByFromDateBiggerthanToDate"));
            }



            //validate the same currency 
            var sameCurrency = input.ExchangeItem.Where(t => t.FromCurrencyId == t.ToCurrencyId);

            if (sameCurrency.Count() > 0)
            {
                throw new UserFriendlyException(L("CanNoExchangeTheSameCurrency"));
            }
            //validate currency = null

            var nullCurrecny = input.ExchangeItem.Where(t => t.FromCurrencyId == 0 || t.ToCurrencyId == 0);
            if (nullCurrecny.Count() > 0)
            {
                throw new UserFriendlyException(L("PleaseSelectCurrency"));
            }

            //validate
            var exchangeByIndex = input.ExchangeItem.Select((val, index) => new { From = val.FromCurrencyId, To = val.ToCurrencyId, Index = index }).ToList();

            for (int i = 0; i < exchangeByIndex.Count(); i++)
            {
                var item = exchangeByIndex[i];
                if (exchangeByIndex.Any(a => a.Index != i && ((a.From == item.From && a.To == item.To) || (a.From == item.To && a.To == item.From))))
                    throw new UserFriendlyException(L("DuplicateCurrency"));
            }


            entity.Update(userId, input.FromDate, input.ToDate);


            #region update itemPriceItem
            var exchangeItems = await _exchangeItemRepository.GetAll().Where(u => u.ExchangeId == entity.Id).ToListAsync();
            foreach (var c in input.ExchangeItem)
            {
                if (c.Id != null)
                {
                    var exchangeItem = exchangeItems.FirstOrDefault(u => u.Id == c.Id);
                    if (exchangeItem != null)
                    {              

                            exchangeItem.Update(userId, c.FromCurrencyId, c.ToCurrencyId, c.Bid,c.Ask, entity.Id);
                            CheckErrors(await _exchangeItemManager.UpdateAsync(exchangeItem));

                    }
                }
                else if (c.Id == null)
                {
                   

                        var priceItem = ExchangeItem.Create(tenantId, userId, c.FromCurrencyId, c.ToCurrencyId, c.Bid,c.Ask , entity);
                        CheckErrors(await _exchangeItemManager.CreateAsync(priceItem));

                    
                }
            }

            var toDeleteitemPriceItem = exchangeItems.Where(u => !input.ExchangeItem.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteitemPriceItem)
            {
              
                    CheckErrors(await _exchangeItemManager.RemoveAsync(t));
              

            }

            #endregion

            CheckErrors(await _exchangeManager.UpdateAsync(entity));

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_GetExChangeCurrency)]
        public async Task<CreateOrUpdateExchangeInput> GetExChangeCurrency(GetExchangeInput input)
        {

            var @exchange = await (from exi in _exchangeItemRepository.GetAll()
                            .Include(u => u.Exchange)
                            .Include(u=>u.FromCurrency)
                            .Include(u=>u.ToCurrency)
                            .AsNoTracking()
                            .Where(u => (u.ToCurencyId == input.ToCurrency && u.FromCurrencyId == input.FromCurrency) || (u.ToCurencyId == input.FromCurrency && u.FromCurrencyId == input.ToCurrency))
                            .Where(u => input.Date.Date != null && u.Exchange.FromDate.Date <= input.Date.Date && input.Date.Date <= u.Exchange.ToDate)
                            select (new CreateOrUpdateExchangeInput
                            {

                                Id = exi.Id,
                                FromCurrencyId = input.FromCurrency.Value,
                                ToCurrencyId = input.ToCurrency.Value,
                                Ask = exi.Ask,
                                Bid = exi.Bid,
                                IsInves = exi.FromCurrency.Id != input.FromCurrency,

                            })).FirstOrDefaultAsync();
                           
            return @exchange;
        }

        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Exchanges_ApplyRate)]
        public async Task ApplyRate(EntityDto<Guid> input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            Tenant tenant = null;
            AccountCycle previousCycle = null;
            AccountCycle currentCycle = null;

            Exchange applyRate = null;
            List<ExchangeItem> exchangeItems = new List<ExchangeItem>();
            List<Journal> journals = new List<Journal>();
            List<JournalItem> journalItems = new List<JournalItem>();
            List<Invoice> invoices = new List<Invoice>();
            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            List<CustomerCredit> customerCredits = new List<CustomerCredit>();
            List<CustomerCreditDetail> customerCreditItems = new List<CustomerCreditDetail>();

            List<ReceivePayment> receivePayments = new List<ReceivePayment>();
            List<ReceivePaymentDetail> receivePaymentItems = new List<ReceivePaymentDetail>();
            List<ReceivePaymentExpense> receivePaymentExpenses = new List<ReceivePaymentExpense>();
            List<Journal> receivePaymentJournals = new List<Journal>();
            List<JournalItem> receivePaymentJournalItems = new List<JournalItem>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    tenant = await GetCurrentTenantAsync(); 

                    applyRate = await _exchangeManager.GetAsync(input.Id);
                    if (applyRate == null) throw new UserFriendlyException(L("RecordNotFound"));

                    //check close period
                    await CheckClosePeriod(applyRate.FromDate, applyRate.ToDate);

                    previousCycle = await GetPreviousCloseCyleAsync(applyRate.ToDate);
                    currentCycle = await GetCurrentCycleAsync();

                    exchangeItems = await _exchangeItemRepository.GetAll().Where(s => s.ExchangeId == input.Id).ToListAsync();

                    journalItems = await _journalItemRepository.GetAll()
                                         .Include(s => s.Journal)
                                         .Where(s => s.Journal.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                         .Where(s => (s.Journal.JournalType == enumStatus.EnumStatus.JournalType.Invoice && 
                                                      s.Journal.Invoice.TransactionTypeSale != null && 
                                                      s.Journal.Invoice.TransactionTypeSale.IsPOS) ||
                                                     (s.Journal.JournalType == enumStatus.EnumStatus.JournalType.CustomerCredit && 
                                                     s.Journal.CustomerCredit.IsPOS) 
                                         )
                                         .Where(s => applyRate.FromDate.Date <= s.Journal.Date.Date && 
                                                s.Journal.Date.Date <= applyRate.ToDate.Date)
                                         .ToListAsync();

                    //if (journalItems == null || !journalItems.Any()) throw new UserFriendlyException(L("NoRecordToApply"));
                    if (journalItems == null || !journalItems.Any()) return;

                    journals = journalItems.Select(s => s.Journal).Distinct().ToList();
                    var invoiceIds = journals.Where(s => s.InvoiceId.HasValue).Select(s => s.InvoiceId).ToList();
                    var customerCreditIds = journals.Where(s => s.CustomerCreditId.HasValue).Select(s => s.CustomerCreditId).ToList();
                   
                    invoiceItems = await _invoiceItemRepository.GetAll()
                                         .Include(s => s.Invoice)
                                         .Where(s => invoiceIds.Any(i => i == s.InvoiceId))
                                         .ToListAsync();

                    invoices = invoiceItems.Select(s => s.Invoice).Distinct().ToList();

                    customerCreditItems = await _customerCreditItemRepository.GetAll()
                                            .Include(s => s.CustomerCredit)
                                            .Where(s => customerCreditIds.Any(r => r == s.CustomerCreditId))
                                            .ToListAsync();

                    customerCredits = customerCreditItems.Select(s => s.CustomerCredit).Distinct().ToList();

                  
                    receivePaymentItems = await _receivePaymentItemRepository.GetAll()
                                                .Include(s => s.ReceivePayment)
                                                .Where(s => (s.InvoiceId.HasValue && invoiceIds.Any(i =>  i == s.InvoiceId)) ||
                                                    (s.CustomerCreditId.HasValue && customerCreditIds.Any(i => i == s.CustomerCreditId))
                                                )
                                                .OrderBy(s => s.ReceivePaymentId)
                                                .ToListAsync();

                    receivePayments = receivePaymentItems
                                      .OrderByDescending(s => s.MultiCurrencyOpenBalance)
                                      .ThenBy(s => s.ReceivePaymentId)
                                      .Select(s => s.ReceivePayment)
                                      .Distinct()
                                      .ToList();

                    var receivePaymentIds = receivePayments.Select(s => s.Id).ToList();

                    receivePaymentJournalItems = await _journalItemRepository.GetAll()
                                                       .Include(s => s.Journal)
                                                       .Where( s => s.Journal.JournalType == enumStatus.EnumStatus.JournalType.ReceivePayment)
                                                       .Where(s => s.Journal.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                       .Where(s => receivePaymentIds.Any(r => r == s.Journal.ReceivePaymentId))                                                       
                                                       .ToListAsync();

                    receivePaymentJournals = receivePaymentJournalItems
                                             //.OrderBy(s => s.Journal.Date.Date)
                                             //.ThenBy(s => s.Journal.CreationTimeIndex)
                                             .Select(s => s.Journal)
                                             .Distinct()
                                             .ToList();

                    receivePaymentExpenses = await _receivePaymentExpenseRepository.GetAll()
                                                   .Where(s => receivePaymentIds.Any(i => i == s.ReceivePaymentId)).ToListAsync();

                }

                await uow.CompleteAsync();
            }


            var exchangeRate = exchangeItems
                               .Where(s => (s.FromCurrencyId == tenant.POSCurrencyId && s.ToCurencyId == tenant.CurrencyId) ||
                                           (s.FromCurrencyId == tenant.CurrencyId && s.ToCurencyId == tenant.POSCurrencyId)
                                     ).FirstOrDefault();

            if (exchangeRate == null) throw new UserFriendlyException(L("PleaseSetExcahngeRate"));

            var rate = exchangeRate.FromCurrencyId == tenant.POSCurrencyId ? exchangeRate.Ask : 1 / exchangeRate.Ask;

            var roundTotalDigits = previousCycle == null ? currentCycle.RoundingDigit : previousCycle.RoundingDigit;
            var roundUnitCostDigits = previousCycle == null ? currentCycle.RoundingDigitUnitCost : previousCycle.RoundingDigitUnitCost;


            foreach(var inv in invoices)
            {
                decimal invoiceTotal = 0;
                decimal invoiceTax = 0;

                var items = invoiceItems.Where(s => s.InvoiceId == inv.Id).OrderBy(r => r.CreationTime).ToList();
                foreach(var i in items)
                {
                    var unitCost = Math.Round(i.MultiCurrencyUnitCost * rate, roundUnitCostDigits);
                    var lineTotal = Math.Round(i.MultiCurrencyTotal * rate, roundTotalDigits);
                    var multiTax = i.MultiCurrencyTotal * (i.Tax == null ? 0 : i.Tax.TaxRate);                    
                    var tax = Math.Round(multiTax * rate, roundTotalDigits);

                    i.SetUnitCost(unitCost);
                    i.SetTotal(lineTotal);

                    var revenueJournalItem = journalItems.FirstOrDefault(s => s.Identifier == i.Id && s.Key == enumStatus.EnumStatus.PostingKey.Revenue);
                    if (revenueJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                    revenueJournalItem.SetDebitCredit(0, lineTotal);

                    invoiceTotal += lineTotal;
                    invoiceTax += tax;                   
                }

                inv.SetSubTotal(invoiceTotal);
                inv.SetTax(invoiceTax);
                inv.SetTotal(invoiceTotal + invoiceTax);

                //Reset Open Bance, Paid Balance, but keep paid status
                inv.SetOpenBalance(inv.Total);
                inv.SetTotalPaid(0);

                var invoiceJournal = journals.FirstOrDefault(s => s.InvoiceId == inv.Id);
                if (invoiceJournal == null) throw new UserFriendlyException(L("RecordNotFound"));
                invoiceJournal.SetDebitCredit(inv.Total);

                var journalItemHeader = journalItems.FirstOrDefault(s => s.JournalId == invoiceJournal.Id && s.Identifier == null);
                if(journalItemHeader == null) throw new UserFriendlyException(L("RecordNotFound"));
                journalItemHeader.SetDebitCredit(inv.Total, 0);
            }

            foreach (var inv in customerCredits)
            {
                decimal invoiceTotal = 0;
                decimal invoiceTax = 0;

                var items = customerCreditItems.Where(s => s.CustomerCreditId == inv.Id).OrderBy(r => r.CreationTime).ToList();
                foreach (var i in items)
                {
                    var unitCost = Math.Round(i.MultiCurrencyUnitCost * rate, roundUnitCostDigits);
                    var lineTotal = Math.Round(i.MultiCurrencyTotal * rate, roundTotalDigits);
                    var multiTax = i.MultiCurrencyTotal * (i.Tax == null ? 0 : i.Tax.TaxRate);
                    var tax = Math.Round(multiTax * rate, roundTotalDigits);

                    i.SetUnitCost(unitCost);
                    i.SetTotal(lineTotal);

                    var returnJournalItem = journalItems.FirstOrDefault(s => s.Identifier == i.Id && s.Key == enumStatus.EnumStatus.PostingKey.SaleAllowance);
                    if (returnJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                    returnJournalItem.SetDebitCredit(lineTotal, 0);

                    invoiceTotal += lineTotal;
                    invoiceTax += tax;
                }

                inv.SetSubTotal(invoiceTotal);
                inv.SetTax(invoiceTax);
                inv.SetTotal(invoiceTotal + invoiceTax);

                //Reset Open Bance, Paid Balance, but keep paid status
                inv.SetOpenBalance(inv.Total);
                inv.SetTotalPaid(0);

                var invoiceJournal = journals.FirstOrDefault(s => s.CustomerCreditId == inv.Id);
                if (invoiceJournal == null) throw new UserFriendlyException(L("RecordNotFound"));
                invoiceJournal.SetDebitCredit(inv.Total);

                var journalItemHeader = journalItems.FirstOrDefault(s => s.JournalId == invoiceJournal.Id && s.Identifier == null);
                if (journalItemHeader == null) throw new UserFriendlyException(L("RecordNotFound"));
                journalItemHeader.SetDebitCredit(0, inv.Total);
            }


            List<ReceivePaymentExpense> addReceivePaymentExpenses = new List<ReceivePaymentExpense>();
            List<ReceivePaymentExpense> editReceivePaymentExpenses = new List<ReceivePaymentExpense>();
            List<ReceivePaymentExpense> deleteReceivePaymentExpenses = new List<ReceivePaymentExpense>();
            List<JournalItem> addJournalItems = new List<JournalItem>();
            List<JournalItem> editJournalItems = new List<JournalItem>();
            List<JournalItem> deleteJournalItems = new List<JournalItem>();

            Dictionary<Guid, decimal> invoicePaymentDic = new Dictionary<Guid, decimal>();

            #region Old
            //foreach (var j in receivePaymentJournals)
            //{

            //    if (!(j.CurrencyId == j.MultiCurrencyId || j.MultiCurrencyId == tenant.POSCurrencyId)) continue;

            //    var paymentItems = receivePaymentItems.Where(s => s.ReceivePaymentId == j.ReceivePaymentId).ToList();
            //    var isSettlement = paymentItems.Any(s => s.InvoiceId.HasValue) && paymentItems.Any(s => s.CustomerCreditId.HasValue);
            //    var isRefund = !paymentItems.Any(s => s.InvoiceId.HasValue) && paymentItems.Any(s => s.CustomerCreditId.HasValue);

            //    decimal paymentLossGain = 0;
            //    decimal totalOpenBalance = 0;
            //    decimal totalOpebBalanceCustomerCredit = 0;
            //    decimal totalPaymentInvoice = 0;
            //    decimal totalPaymentCustomerCredit = 0;
            //    decimal totalPaymentDue = 0;
            //    decimal totalPaymentDueCustomerCredit = 0;

            //    decimal totalCashInvoice = 0;
            //    decimal totalCreditInvocie = 0;
            //    decimal totalExpenseInvoice = 0;
            //    decimal totalCashCustomerCredit = 0;
            //    decimal totalCreditCustomerCredit = 0;
            //    decimal totalExpenseCustomerCredit = 0;

            //    if(j.JournalNo == "RP21-0030")
            //    {

            //    }

            //    foreach (var pi in paymentItems)
            //    {
            //        var paymentJournalItem = receivePaymentJournalItems.FirstOrDefault(s => s.Identifier == pi.Id && s.JournalId == j.Id);
            //        if (paymentJournalItem == null) throw new UserFriendlyException("RecordNotFound");

            //        var openBalance = Math.Round(pi.MultiCurrencyOpenBalance * rate, roundTotalDigits);
            //        var paymentAmount = Math.Round(pi.MultiCurrencyPayment * rate, roundTotalDigits);
            //        var balanceDue = openBalance - paymentAmount;

            //        var cashPayment = Math.Round(pi.MultiCurrencyCash * rate, roundTotalDigits);
            //        var creditPayment = Math.Round(pi.MultiCurrencyCredit * rate, roundTotalDigits);
            //        var expensePayment = Math.Round(pi.MultiCurrencyExpense * rate, roundTotalDigits);

            //        pi.OpenBalance = openBalance;
            //        pi.Payment = paymentAmount;
            //        pi.TotalAmount = openBalance - paymentAmount;

            //        pi.Cash = cashPayment;
            //        pi.Credit = creditPayment;                    
            //        pi.Expense = expensePayment;

            //        if (pi.InvoiceId.HasValue)
            //        {
            //            var invoice = invoices.FirstOrDefault(s => s.Id == pi.InvoiceId);
            //            var lossGainAmount = invoice.OpenBalance - pi.Payment;

            //            if (pi.MultiCurrencyOpenBalance == pi.MultiCurrencyPayment && lossGainAmount != 0)
            //            {                           
            //                pi.Payment += lossGainAmount;
            //                pi.TotalAmount -= lossGainAmount;

            //                paymentLossGain += lossGainAmount;
            //            }

            //            totalOpenBalance += pi.OpenBalance;
            //            totalPaymentInvoice += pi.Payment;
            //            totalPaymentDue += pi.TotalAmount;

            //            totalCashInvoice += cashPayment;
            //            totalCreditInvocie += creditPayment;
            //            totalExpenseInvoice += expensePayment;

            //            //Update Payment Journal Item
            //            paymentJournalItem.SetDebitCredit(0, pi.Payment);

            //            //Update Invoice PaidAmount, and OpenBalnce
            //            invoice.UpdateTotalPaid(pi.Payment);
            //            invoice.UpdateOpenBalance(-pi.Payment);
            //        }
            //        else
            //        {
            //            var customerCredit = customerCredits.FirstOrDefault(s => s.Id == pi.CustomerCreditId);

            //            var lossGainAmount = customerCredit.OpenBalance - pi.Payment;

            //            if (pi.MultiCurrencyOpenBalance == pi.MultiCurrencyPayment && lossGainAmount != 0)
            //            {
            //                pi.Payment += lossGainAmount;
            //                pi.TotalAmount -= lossGainAmount;

            //                paymentLossGain += -lossGainAmount;
            //            }

            //            totalOpebBalanceCustomerCredit += pi.OpenBalance;
            //            totalPaymentCustomerCredit += pi.Payment;
            //            totalPaymentDueCustomerCredit += pi.TotalAmount;

            //            totalCashCustomerCredit += cashPayment;
            //            totalCreditCustomerCredit += creditPayment;
            //            totalExpenseCustomerCredit += expensePayment;

            //            //Update Payment Journal Item
            //            paymentJournalItem.SetDebitCredit(pi.Payment, 0);

            //            //Update CustomerCredit PaidAmount, and OpenBalnce
            //            customerCredit.IncreaseTotalPaid(pi.Payment);
            //            customerCredit.IncreaseOpenbalance(-pi.Payment);
            //        }

            //    }

            //    if (paymentLossGain != 0 && !tenant.ExchangeLossAndGainId.HasValue) throw new UserFriendlyException(L("PleaseSelect", L("ExchangeLossGainAccount")));

            //    var paymentExpense = receivePaymentExpenses.FirstOrDefault(s => s.ReceivePaymentId == payment.Id);
            //    var paymentExpenseJournalItem = paymentExpense == null ? null :
            //                                    receivePaymentJournalItems.FirstOrDefault(s => s.Identifier == paymentExpense.Id && s.Key == enumStatus.EnumStatus.PostingKey.PaymentChange);


            //    if (paymentLossGain != 0)
            //    {
            //        if (isSettlement)
            //        {
            //            payment.SetReceiveFrom(enumStatus.EnumStatus.ReceiveFromRecievePayment.Cash);
            //            j.SetMemo("Exchange Loss/Gain" + paymentLossGain);

            //            var paymentJournalItemHeader = receivePaymentJournalItems
            //                .FirstOrDefault(s => s.JournalId == j.Id &&
            //                    s.Key == enumStatus.EnumStatus.PostingKey.Payment &&
            //                    s.Identifier == null);
            //            if (paymentJournalItemHeader == null)
            //            {
            //                paymentJournalItemHeader = JournalItem.CreateJournalItem(tenantId, userId, j.Id, tenant.ExchangeLossAndGainId.Value, j.Memo, 0, 0, enumStatus.EnumStatus.PostingKey.Payment, null);

            //                addJournalItems.Add(paymentJournalItemHeader);
            //            }
            //            else
            //            {
            //                paymentJournalItemHeader.UpdateJournalItemAccount(tenant.ExchangeLossAndGainId.Value);

            //                editJournalItems.Add(paymentJournalItemHeader);
            //            }

            //            if (paymentLossGain > 0)
            //            {
            //                paymentJournalItemHeader.SetDebitCredit(paymentLossGain, 0);
            //            }
            //            else
            //            {
            //                paymentJournalItemHeader.SetDebitCredit(0, -paymentLossGain);
            //            }
            //        }
            //        else
            //        {
            //            if (paymentExpense == null)
            //            {
            //                paymentExpense = ReceivePaymentExpense.Create(tenantId, userId, payment, tenant.ExchangeLossAndGainId.Value, paymentLossGain, 0, j.Memo);
            //                paymentExpense.SetReceivePayment(payment.Id);

            //                addReceivePaymentExpenses.Add(paymentExpense);
            //            }
            //            else
            //            {
            //                paymentExpense.SetAmount(paymentLossGain);
            //                editReceivePaymentExpenses.Add(paymentExpense);
            //            }

            //            if (paymentExpenseJournalItem == null)
            //            {
            //                paymentExpenseJournalItem = JournalItem.CreateJournalItem(tenantId, userId, j.Id, tenant.ExchangeLossAndGainId.Value, j.Memo, paymentLossGain, 0, enumStatus.EnumStatus.PostingKey.PaymentChange, paymentExpense.Id);

            //                addJournalItems.Add(paymentExpenseJournalItem);
            //            }
            //            else
            //            {
            //                editJournalItems.Add(paymentExpenseJournalItem);
            //            }

            //            if (paymentLossGain < 0)
            //            {
            //                paymentExpenseJournalItem.SetDebitCredit(0, -paymentLossGain);
            //            }
            //            else
            //            {
            //                paymentExpenseJournalItem.SetDebitCredit(paymentLossGain, 0);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (paymentExpense != null)
            //        {
            //            deleteReceivePaymentExpenses.Add(paymentExpense);
            //        }
            //        if(paymentExpenseJournalItem != null)
            //        {
            //            deleteJournalItems.Add(paymentExpenseJournalItem);
            //        }
            //    }


            //    //update receive payment
            //    payment.SetTotalOpenBalance(totalOpenBalance);
            //    payment.SetTotalPaymentInvoice(totalPaymentInvoice);
            //    payment.SetTotalPaymentDue(totalPaymentDue);
            //    payment.SetTotalOpenBalanceCustomerCredit(totalOpebBalanceCustomerCredit);
            //    payment.SetTotalPaymentCustomerCredit(totalPaymentCustomerCredit);
            //    payment.SetTotalPaymentDueCustomerCredit(totalPaymentDueCustomerCredit);

            //    if (isSettlement)
            //    {
            //        payment.SetTotalPayment(paymentLossGain);
            //        j.SetDebitCredit(paymentLossGain);

            //        if(paymentLossGain == 0)
            //        {
            //            var paymentJournalItemHeader = receivePaymentJournalItems.
            //            FirstOrDefault(s => s.Id == payment.Id && s.Identifier == null && s.Key == enumStatus.EnumStatus.PostingKey.Payment);
            //            if(paymentJournalItemHeader != null)
            //            {
            //                paymentJournalItemHeader.SetDebitCredit(0, 0);

            //                editJournalItems.Add(paymentJournalItemHeader);
            //            }
            //        }                    
            //    }
            //    else
            //    {
            //        var totalPayment = j.CurrencyId == j.MultiCurrencyId ? payment.MultiCurrencyTotalPayment : Math.Round(payment.MultiCurrencyTotalPayment * rate, roundTotalDigits);
            //        payment.SetTotalPayment(totalPayment);
            //        j.SetDebitCredit(totalPayment);

            //        payment.SetChange(paymentLossGain);

            //        var paymentJournalItemHeader = receivePaymentJournalItems.
            //        FirstOrDefault(s => s.JournalId == j.Id && s.Identifier == null && s.Key == enumStatus.EnumStatus.PostingKey.Payment);
            //        if (paymentJournalItemHeader == null) throw new UserFriendlyException(L("RecordNotFound"));

            //        if(totalPayment > 0)
            //        {
            //            //receive payemnt
            //            paymentJournalItemHeader.SetDebitCredit(totalPayment, 0);
            //        }
            //        else
            //        {
            //            //refund
            //            paymentJournalItemHeader.SetDebitCredit(0, -totalPayment);
            //        }

            //        editJournalItems.Add(paymentJournalItemHeader);
            //    }

            //}
            #endregion

            foreach (var payment in receivePayments)
            {
                var j = receivePaymentJournals.FirstOrDefault(s => s.ReceivePaymentId == payment.Id);

                if (!(j.CurrencyId == j.MultiCurrencyId || j.MultiCurrencyId == tenant.POSCurrencyId)) continue;

                var paymentItems = receivePaymentItems.Where(s => s.ReceivePaymentId == j.ReceivePaymentId).ToList();
                var isSettlement = paymentItems.Any(s => s.InvoiceId.HasValue) && paymentItems.Any(s => s.CustomerCreditId.HasValue);
                //var isRefund = !paymentItems.Any(s => s.InvoiceId.HasValue) && paymentItems.Any(s => s.CustomerCreditId.HasValue);

                decimal paymentLossGain = 0;
                decimal totalOpenBalance = 0;
                decimal totalOpebBalanceCustomerCredit = 0;
                decimal totalPaymentInvoice = 0;
                decimal totalPaymentCustomerCredit = 0;
                decimal totalPaymentDue = 0;
                decimal totalPaymentDueCustomerCredit = 0;

                decimal totalCashInvoice = 0;
                decimal totalCreditInvocie = 0;
                decimal totalExpenseInvoice = 0;
                decimal totalCashCustomerCredit = 0;
                decimal totalCreditCustomerCredit = 0;
                decimal totalExpenseCustomerCredit = 0;

                if (j.JournalNo == "RP21-0030")
                {

                }

                foreach (var pi in paymentItems)
                {
                    var paymentJournalItem = receivePaymentJournalItems.FirstOrDefault(s => s.Identifier == pi.Id && s.JournalId == j.Id);
                    if (paymentJournalItem == null) throw new UserFriendlyException("RecordNotFound");

                    var openBalance = Math.Round(pi.MultiCurrencyOpenBalance * rate, roundTotalDigits);
                    var paymentAmount = Math.Round(pi.MultiCurrencyPayment * rate, roundTotalDigits);
                    var balanceDue = openBalance - paymentAmount;

                    var cashPayment = Math.Round(pi.MultiCurrencyCash * rate, roundTotalDigits);
                    var creditPayment = Math.Round(pi.MultiCurrencyCredit * rate, roundTotalDigits);
                    var expensePayment = Math.Round(pi.MultiCurrencyExpense * rate, roundTotalDigits);

                    pi.OpenBalance = openBalance;
                    pi.Payment = paymentAmount;
                    pi.TotalAmount = openBalance - paymentAmount;

                    pi.Cash = cashPayment;
                    pi.Credit = creditPayment;
                    pi.Expense = expensePayment;

                    if (pi.InvoiceId.HasValue)
                    {
                        var invoice = invoices.FirstOrDefault(s => s.Id == pi.InvoiceId);
                        var lossGainAmount = invoice.OpenBalance - pi.Payment;

                        if (pi.MultiCurrencyOpenBalance == pi.MultiCurrencyPayment && lossGainAmount != 0)
                        {
                            pi.Payment += lossGainAmount;
                            pi.TotalAmount -= lossGainAmount;

                            paymentLossGain += lossGainAmount;
                        }

                        totalOpenBalance += pi.OpenBalance;
                        totalPaymentInvoice += pi.Payment;
                        totalPaymentDue += pi.TotalAmount;

                        totalCashInvoice += cashPayment;
                        totalCreditInvocie += creditPayment;
                        totalExpenseInvoice += expensePayment;

                        //Update Payment Journal Item
                        paymentJournalItem.SetDebitCredit(0, pi.Payment);

                        //Update Invoice PaidAmount, and OpenBalnce
                        invoice.UpdateTotalPaid(pi.Payment);
                        invoice.UpdateOpenBalance(-pi.Payment);
                    }
                    else
                    {
                        var customerCredit = customerCredits.FirstOrDefault(s => s.Id == pi.CustomerCreditId);

                        var lossGainAmount = customerCredit.OpenBalance - pi.Payment;

                        if (pi.MultiCurrencyOpenBalance == pi.MultiCurrencyPayment && lossGainAmount != 0)
                        {
                            pi.Payment += lossGainAmount;
                            pi.TotalAmount -= lossGainAmount;

                            paymentLossGain += -lossGainAmount;
                        }

                        totalOpebBalanceCustomerCredit += pi.OpenBalance;
                        totalPaymentCustomerCredit += pi.Payment;
                        totalPaymentDueCustomerCredit += pi.TotalAmount;

                        totalCashCustomerCredit += cashPayment;
                        totalCreditCustomerCredit += creditPayment;
                        totalExpenseCustomerCredit += expensePayment;

                        //Update Payment Journal Item
                        paymentJournalItem.SetDebitCredit(pi.Payment, 0);

                        //Update CustomerCredit PaidAmount, and OpenBalnce
                        customerCredit.IncreaseTotalPaid(pi.Payment);
                        customerCredit.IncreaseOpenbalance(-pi.Payment);
                    }

                }

                if (paymentLossGain != 0 && !tenant.ExchangeLossAndGainId.HasValue) throw new UserFriendlyException(L("PleaseSelect", L("ExchangeLossGainAccount")));

                var paymentExpense = receivePaymentExpenses.FirstOrDefault(s => s.ReceivePaymentId == payment.Id);
                var paymentExpenseJournalItem = paymentExpense == null ? null :
                                                receivePaymentJournalItems.FirstOrDefault(s => s.Identifier == paymentExpense.Id && s.Key == enumStatus.EnumStatus.PostingKey.PaymentChange);


                if (paymentLossGain != 0)
                {
                    if (isSettlement)
                    {
                        payment.SetReceiveFrom(enumStatus.EnumStatus.ReceiveFromRecievePayment.Cash);
                        j.SetMemo("Exchange Loss/Gain" + paymentLossGain);

                        var paymentJournalItemHeader = receivePaymentJournalItems
                            .FirstOrDefault(s => s.JournalId == j.Id &&
                                s.Key == enumStatus.EnumStatus.PostingKey.Payment &&
                                s.Identifier == null);
                        if (paymentJournalItemHeader == null)
                        {
                            paymentJournalItemHeader = JournalItem.CreateJournalItem(tenantId, userId, j.Id, tenant.ExchangeLossAndGainId.Value, j.Memo, 0, 0, enumStatus.EnumStatus.PostingKey.Payment, null);

                            addJournalItems.Add(paymentJournalItemHeader);
                        }
                        else
                        {
                            paymentJournalItemHeader.UpdateJournalItemAccount(tenant.ExchangeLossAndGainId.Value);

                            editJournalItems.Add(paymentJournalItemHeader);
                        }

                        if (paymentLossGain > 0)
                        {
                            paymentJournalItemHeader.SetDebitCredit(paymentLossGain, 0);
                        }
                        else
                        {
                            paymentJournalItemHeader.SetDebitCredit(0, -paymentLossGain);
                        }
                    }
                    else
                    {
                        if (paymentExpense == null)
                        {
                            paymentExpense = ReceivePaymentExpense.Create(tenantId, userId, payment, tenant.ExchangeLossAndGainId.Value, paymentLossGain, 0, j.Memo, false);
                            paymentExpense.SetReceivePayment(payment.Id);

                            addReceivePaymentExpenses.Add(paymentExpense);
                        }
                        else
                        {
                            paymentExpense.SetAmount(paymentLossGain);
                            editReceivePaymentExpenses.Add(paymentExpense);
                        }

                        if (paymentExpenseJournalItem == null)
                        {
                            paymentExpenseJournalItem = JournalItem.CreateJournalItem(tenantId, userId, j.Id, tenant.ExchangeLossAndGainId.Value, j.Memo, paymentLossGain, 0, enumStatus.EnumStatus.PostingKey.PaymentChange, paymentExpense.Id);

                            addJournalItems.Add(paymentExpenseJournalItem);
                        }
                        else
                        {
                            editJournalItems.Add(paymentExpenseJournalItem);
                        }

                        if (paymentLossGain < 0)
                        {
                            paymentExpenseJournalItem.SetDebitCredit(0, -paymentLossGain);
                        }
                        else
                        {
                            paymentExpenseJournalItem.SetDebitCredit(paymentLossGain, 0);
                        }
                    }
                }
                else
                {
                    if (paymentExpense != null)
                    {
                        deleteReceivePaymentExpenses.Add(paymentExpense);
                    }
                    if (paymentExpenseJournalItem != null)
                    {
                        deleteJournalItems.Add(paymentExpenseJournalItem);
                    }
                }


                //update receive payment
                payment.SetTotalOpenBalance(totalOpenBalance);
                payment.SetTotalPaymentInvoice(totalPaymentInvoice);
                payment.SetTotalPaymentDue(totalPaymentDue);
                payment.SetTotalOpenBalanceCustomerCredit(totalOpebBalanceCustomerCredit);
                payment.SetTotalPaymentCustomerCredit(totalPaymentCustomerCredit);
                payment.SetTotalPaymentDueCustomerCredit(totalPaymentDueCustomerCredit);

                if (isSettlement)
                {
                    payment.SetTotalPayment(paymentLossGain);
                    j.SetDebitCredit(paymentLossGain);

                    if (paymentLossGain == 0)
                    {
                        var paymentJournalItemHeader = receivePaymentJournalItems.
                        FirstOrDefault(s => s.Id == payment.Id && s.Identifier == null && s.Key == enumStatus.EnumStatus.PostingKey.Payment);
                        if (paymentJournalItemHeader != null)
                        {
                            paymentJournalItemHeader.SetDebitCredit(0, 0);

                            editJournalItems.Add(paymentJournalItemHeader);
                        }
                    }
                }
                else
                {
                    var totalPayment = j.CurrencyId == j.MultiCurrencyId ? payment.MultiCurrencyTotalPayment : Math.Round(payment.MultiCurrencyTotalPayment * rate, roundTotalDigits);
                    payment.SetTotalPayment(totalPayment);
                    j.SetDebitCredit(totalPayment);

                    payment.SetChange(paymentLossGain);

                    var paymentJournalItemHeader = receivePaymentJournalItems.
                    FirstOrDefault(s => s.JournalId == j.Id && s.Identifier == null && s.Key == enumStatus.EnumStatus.PostingKey.Payment);
                    if (paymentJournalItemHeader == null) throw new UserFriendlyException(L("RecordNotFound"));

                    if (totalPayment > 0)
                    {
                        //receive payemnt
                        paymentJournalItemHeader.SetDebitCredit(totalPayment, 0);
                    }
                    else
                    {
                        //refund
                        paymentJournalItemHeader.SetDebitCredit(0, -totalPayment);
                    }

                    editJournalItems.Add(paymentJournalItemHeader);
                }

            }


            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _invoiceItemRepository.BulkUpdateAsync(invoiceItems);
                    await _invoiceRepository.BulkUpdateAsync(invoices);
                    await _customerCreditItemRepository.BulkUpdateAsync(customerCreditItems);
                    await _customerCreditRepository.BulkUpdateAsync(customerCredits);
                    await _journalItemRepository.BulkUpdateAsync(journalItems);
                    await _journalRepository.BulkUpdateAsync(journals);

                    await _receivePaymentItemRepository.BulkUpdateAsync(receivePaymentItems);
                    await _receivePaymentRepository.BulkUpdateAsync(receivePayments);
                    await _journalItemRepository.BulkUpdateAsync(receivePaymentJournalItems);
                    await _journalRepository.BulkUpdateAsync(receivePaymentJournals);

                    await _journalItemRepository.BulkDeleteAsync(deleteJournalItems);
                    await _receivePaymentExpenseRepository.BulkDeleteAsync(deleteReceivePaymentExpenses);
                    await _journalItemRepository.BulkUpdateAsync(editJournalItems);
                    await _receivePaymentExpenseRepository.BulkUpdateAsync(editReceivePaymentExpenses);
                    await _receivePaymentExpenseRepository.BulkInsertAsync(addReceivePaymentExpenses);
                    await _journalItemRepository.BulkInsertAsync(addJournalItems);                                        

                }

                await uow.CompleteAsync();
            }

        }
    }
}
