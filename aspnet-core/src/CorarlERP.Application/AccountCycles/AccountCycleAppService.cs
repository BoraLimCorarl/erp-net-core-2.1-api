using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorarlERP.AccountCycles.Dto;
using CorarlERP.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Locations;
using CorarlERP.InventoryCostCloses;
using CorarlERP.Inventories;
using CorarlERP.AccountTrasactionCloses;
using CorarlERP.AccountTransactions;
using CorarlERP.VendorCustomerOpenningBalance;
using CorarlERP.VendorCustomerOpenBalances;
using Abp.Domain.Uow;
using System.Transactions;
using Abp.Dependency;
using CorarlERP.Items;

namespace CorarlERP.AccountCycles
{
    [AbpAuthorize]
    public class AccountCycleAppService : CorarlERPAppServiceBase, IAccountCycleAppService
    {
        private readonly IAccountCycleManager _accountCycleManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<Invoices.Invoice, Guid> _invoiceRepository;
        private readonly IRepository<CustomerCredits.CustomerCredit, Guid> _customerCreditRepository;
       

        private readonly ITenantManager _tenantManager;
       
        private readonly IInventoryManager _inventoryManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly ICorarlRepository<InventoryCostClose, Guid> _inventoryCostCloseRepository;
        private readonly ICorarlRepository<InventoryCostCloseItem, Guid> _inventoryCostCloseItemRepository;
        private readonly ICorarlRepository<InventoryCostCloseItemBatchNo, Guid> _inventoryCostCloseItemBatchNoRepository;
        private readonly IInventoryCostCloseManager _inventoryCostCloseManager;
        private readonly IInventoryCostCloseItemManager _inventoryCostCloseItemManager;

        private readonly ICorarlRepository<AccountTransactionClose, Guid> _accountTrasactionCloseRepository;
        private readonly IAccountTransactionCloseManager _accountTransactionCloseManager;
        private readonly IAccountTransactionManager _accountTransactionManager;

        private readonly IVendorOpenningBalanceManager _vendorOpenningBalanceManager;
        private readonly ICustomerOpenningBalanceManager _customerOpenningBalanceManager;
        private readonly IVendorOpenBalanceManager _vendorOpenBalanceManager;
        private readonly ICustomerOpenBalanceManager _customerOpenBalanceManager;
        private readonly ICorarlRepository<VendorOpenBalance, Guid> _vendorOpenBalanceRepository;
        private readonly ICorarlRepository<CustomerOpenBalance, Guid> _customerOpenBalanceRepository;

        public AccountCycleAppService(
            IAccountCycleManager accountCycleManager,
            TenantManager tenantManager,
            IInventoryCostCloseManager inventoryCostCloseManager,
            IInventoryCostCloseItemManager inventoryCostCloseItemManager,
            ICorarlRepository<InventoryCostClose,Guid>inventoryCostCloseRepository ,
            ICorarlRepository<InventoryCostCloseItemBatchNo, Guid> inventoryCostCloseItemBatchNoRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Location, long> locationRepository,
            IRepository<AccountCycle,long> accountCycleRepository,
            IInventoryManager inventoryManager,
            ICorarlRepository<AccountTransactionClose, Guid> accountTrasactionCloseRepository,
            IAccountTransactionCloseManager accountTransactionCloseManager,
            IAccountTransactionManager accountTransactionManager,
            ICorarlRepository<InventoryCostCloseItem, Guid> inventoryCostCloseItemRepository,
            IVendorOpenningBalanceManager vendorOpenningBalanceManager,
            ICustomerOpenningBalanceManager customerOpenningBalanceManager,
            IVendorOpenBalanceManager vendorOpenBalanceManager,
            ICustomerOpenBalanceManager customerOpenBalanceManager,
            ICorarlRepository<VendorOpenBalance, Guid> vendorOpenBalanceRepository,
            ICorarlRepository<CustomerOpenBalance, Guid> customerOpenBalanceRepository
            ) :base(accountCycleRepository, null, null)
        {

            _customerCreditRepository = IocManager.Instance.Resolve<IRepository<CustomerCredits.CustomerCredit, Guid>>();
            _invoiceRepository = IocManager.Instance.Resolve<IRepository<Invoices.Invoice, Guid>>();
            _accountCycleManager = accountCycleManager;
            _accountCycleRepository = accountCycleRepository;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _inventoryManager = inventoryManager;
            _inventoryCostCloseManager = inventoryCostCloseManager;
            _inventoryCostCloseRepository = inventoryCostCloseRepository;
            _accountTransactionManager = accountTransactionManager;
            _accountTransactionCloseManager = accountTransactionCloseManager;
            _accountTrasactionCloseRepository = accountTrasactionCloseRepository;
            _inventoryCostCloseItemManager = inventoryCostCloseItemManager;
            _inventoryCostCloseItemRepository = inventoryCostCloseItemRepository;
            _vendorOpenningBalanceManager = vendorOpenningBalanceManager;
            _customerOpenningBalanceManager = customerOpenningBalanceManager;
            _vendorOpenBalanceManager = vendorOpenBalanceManager;
            _customerOpenBalanceManager = customerOpenBalanceManager;
            _vendorOpenBalanceRepository = vendorOpenBalanceRepository;
            _customerOpenBalanceRepository = customerOpenBalanceRepository;
            _inventoryCostCloseItemBatchNoRepository = inventoryCostCloseItemBatchNoRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Close_Period_Create)]
        public async Task<AccountCycleDetailOutput> Create(CreateOrUpdateAccountCycleInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update account cycle date
            var accountCycle = await _accountCycleRepository.GetAll().Where(t=>t.Id == input.Id).FirstOrDefaultAsync();

            if (accountCycle == null) throw new UserFriendlyException(L("RecordNotFound"));

            accountCycle.UpdateEndate(input.EndDate);
            accountCycle.UpdateRemark(input.Remark);
            CheckErrors(await _accountCycleManager.UpdateAsync(accountCycle));
           

            //update Account Cycle Id in table Tenant 
            var nextDate = input.EndDate.AddDays(1);
            var @entity = AccountCycle.Create(tenantId, userId, nextDate, null, input.RoundingDigit, input.RoundingDigitUnitCost);               
            CheckErrors(await _accountCycleManager.CreateAsync(@entity));


            var tenant = await GetCurrentTenantAsync();
            tenant.UpdateAccountCycleId(entity.Id);
            CheckErrors(await _tenantManager.UpdateAsync(tenant));
            

            //create Inventory transaction 
            var itemBalanceDic = await _inventoryManager.GetItemsBalanceForReport(null, null, input.EndDate, null, null, null, null, null)
                                .Where(s=>s.Qty != 0).GroupBy(g => $"{g.ItemId}-{g.LocationId}").ToDictionaryAsync(k => k.Key, v => v);
            var itemCostDic = await _inventoryManager.GetItemCostSummaryQuery(null, input.EndDate, null, userId)
                              .Where(s=>s.Qty !=0 ).ToListAsync();

            var itemBatchQuery = await _inventoryManager.GetItemBatchNoBalance(null, null, null, input.EndDate, null);

            var checkNegativeQty = itemBatchQuery.FirstOrDefault(s => s.Qty < 0);
            if (checkNegativeQty != null) throw new UserFriendlyException(L("InvalidStockBalance") + " " + L("BatchNo"));          
            var itemBatchBalance = itemBatchQuery.Where(s => s.Qty != 0).ToList();

            //create Account transaction close 
            var accountTrasaction = await _accountTransactionManager.GetAccountQuery(null, input.EndDate, true, null, true).ToListAsync();
           
            //Create Vendor Customer Open Balance 
            var vendorOpenBalances = await _vendorOpenningBalanceManager.GetOpenTransactionQuery(null, input.EndDate, null).ToListAsync();
            var customerOpenBalances = await _customerOpenningBalanceManager.GetOpenTransactionQuery(null, input.EndDate, null).ToListAsync();

            //var invoiceTesting = from v in vendorCustomerOpenBalances
            //                     join i in _invoiceRepository.GetAll().AsNoTracking()
            //                     .Where(t => t.CustomerId == new Guid("8435df81-e757-442a-b5fa-34d66ecf996e"))
            //                     on v.TransactionId equals i.Id
            //                     select v;
            //var customerCreditTesting = from v in vendorCustomerOpenBalances
            //                     join i in _customerCreditRepository.GetAll().AsNoTracking()
            //                     .Where(t => t.CustomerId == new Guid("8435df81-e757-442a-b5fa-34d66ecf996e"))
            //                     on v.TransactionId equals i.Id
            //                    select v;

            await CurrentUnitOfWork.SaveChangesAsync();


            var InventoryCostCloseEntity = new List<InventoryCostClose>();
            var InventoryCostCloseItemEntity = new List<InventoryCostCloseItem>();
            foreach (var inv in itemCostDic)
            {

                var transaction = InventoryCostClose.Create(tenantId, userId, inv.ItemId, inv.LocationId, input.Id, inv.Cost, inv.Qty, input.EndDate, inv.TotalCost);
                //CheckErrors(await _inventoryCostCloseManager.CreateAsync(transaction));
                InventoryCostCloseEntity.Add(transaction);

                var key = $"{inv.ItemId}-{inv.LocationId}";
                var itemLots = itemBalanceDic.ContainsKey(key) ? itemBalanceDic[key] : null;
                if (itemLots == null) throw new UserFriendlyException(L("InvalidStockBalance"));

                foreach (var l in itemLots)
                {
                    var transactionItem = InventoryCostCloseItem.Create(tenantId, userId, l.LotId, l.Qty, transaction.Id);
                    //CheckErrors(await _inventoryCostCloseItemManager.CreateAsync(transactionItem));
                    InventoryCostCloseItemEntity.Add(transactionItem);
                }
            }

            var AccountTransactionCloseEntity = new List<AccountTransactionClose>();
            foreach (var a in accountTrasaction)
            {
                var accountTransactionClose = AccountTransactionClose.Create(tenantId, userId, a.AccountId, a.Debit, a.Credit, a.Balance, input.EndDate, input.Id, a.LocationId, a.CurrencyId, a.MultiCurrencyDebit, a.MultiCurrencyCredit, a.MultiCurrencyBalance);
                //CheckErrors(await _accountTransactionCloseManager.CreateAsync(accountTransactionClose));
                AccountTransactionCloseEntity.Add(accountTransactionClose);
            }

            var vendorOpenBalanceEntity = new List<VendorOpenBalance>();
            foreach (var a in vendorOpenBalances)
            {
                var transactionClose = VendorOpenBalance.Create(tenantId, userId, a.TransactionId.Value, a.Key, input.Id, a.Balance, a.MultiCurrencyBalance, a.LocationId, input.EndDate);
                vendorOpenBalanceEntity.Add(transactionClose);
            }

            var customerOpenBalanceEntity = new List<CustomerOpenBalance>();
            foreach (var a in customerOpenBalances)
            {
                var transactionClose = CustomerOpenBalance.Create(tenantId, userId, a.TransactionId.Value, a.Key, input.Id, a.Balance, a.MultiCurrencyBalance, a.LocationId, input.EndDate);
                customerOpenBalanceEntity.Add(transactionClose);
            }


            var itemBatchs = new List<InventoryCostCloseItemBatchNo>();
            foreach (var i in itemBatchBalance)
            {
                var itemCost = InventoryCostCloseEntity.FirstOrDefault(s => s.ItemId == i.ItemId && s.LocationId == i.LocationId);
                var batchItem = InventoryCostCloseItemBatchNo.Create(tenantId, userId, i.LotId, i.BatchNoId, i.Qty, itemCost.Id);
                itemBatchs.Add(batchItem);
            }


            await _inventoryCostCloseRepository.BulkInsertAsync(InventoryCostCloseEntity);
            await _inventoryCostCloseItemRepository.BulkInsertAsync(InventoryCostCloseItemEntity);
            await _accountTrasactionCloseRepository.BulkInsertAsync(AccountTransactionCloseEntity);
            await _vendorOpenBalanceRepository.BulkInsertAsync(vendorOpenBalanceEntity);
            await _customerOpenBalanceRepository.BulkInsertAsync(customerOpenBalanceEntity);
            await _inventoryCostCloseItemBatchNoRepository.BulkInsertAsync(itemBatchs);
            
            return ObjectMapper.Map<AccountCycleDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Close_Period_Delete)]
        public async Task Delete()
        {
            //step1
            var accountCycle = await _accountCycleRepository.GetAll().ToListAsync();
            if (accountCycle.Count() <= 1){
                throw new UserFriendlyException(L("RecordNotFound"));
            }
           
            var tenantId = AbpSession.GetTenantId();
            var @entity = accountCycle.Where(t => t.EndDate == null).OrderByDescending(s => s.Id).FirstOrDefault();
            //var lastDate = entity.StartDate.AddDays(-1);

            //step2 Update end date in table accountCycle to null
            var reverseToEntity = await _accountCycleRepository.GetAll().Where(t => t.Id < entity.Id).OrderByDescending(s => s.Id).FirstOrDefaultAsync();
            if (reverseToEntity != null)
            {
                reverseToEntity.UpdateEndate(null);
                await _accountCycleRepository.UpdateAsync(reverseToEntity);
            }

            //delete table inventory cost close 
            var inventory = await _inventoryCostCloseRepository.GetAll().Where(t => t.AccountCycleId >= reverseToEntity.Id).AsNoTracking().ToListAsync();

            var inventoryItems = await _inventoryCostCloseItemRepository.GetAll()
                                    .Where(s => s.InventoryCostClose.AccountCycleId >= reverseToEntity.Id)
                                    .AsNoTracking().ToListAsync();

            if(inventoryItems.Any()) await _inventoryCostCloseItemRepository.BulkDeleteAsync(inventoryItems);

            var itemBatchs = await _inventoryCostCloseItemBatchNoRepository.GetAll().Where(s => s.InventoryCostClose.AccountCycleId >= reverseToEntity.Id).AsNoTracking().ToListAsync();
            if(itemBatchs.Any()) await _inventoryCostCloseItemBatchNoRepository.BulkDeleteAsync(itemBatchs);

            //foreach (var i in inventoryItems)
            //{
            //    CheckErrors(await _inventoryCostCloseItemManager.RemoveAsync(i));
            //}

            await _inventoryCostCloseRepository.BulkDeleteAsync(inventory);
            //foreach (var i in inventory)
            //{
            //    CheckErrors(await _inventoryCostCloseManager.RemoveAsync(i));
            //}

            //delete table account transaction close 

            var accounttransaction = await _accountTrasactionCloseRepository.GetAll().Where(t => t.AccountCycleId >= reverseToEntity.Id).AsNoTracking().ToListAsync();
            if(accounttransaction.Any()) await _accountTrasactionCloseRepository.BulkDeleteAsync(accounttransaction);
            //foreach(var a in accounttransaction)
            //{
            //    CheckErrors(await _accountTransactionCloseManager.RemoveAsync(a));
            //}

            //delete vendor open abalance
            var vendorTransactions = await _vendorOpenBalanceRepository.GetAll().Where(t => t.AccountCycleId >= reverseToEntity.Id).AsNoTracking().ToListAsync();
            if(vendorTransactions.Any())  await _vendorOpenBalanceRepository.BulkDeleteAsync(vendorTransactions);

            //delete customer open balance
            var customerTransactions = await _customerOpenBalanceRepository.GetAll().Where(t => t.AccountCycleId >= reverseToEntity.Id).AsNoTracking().ToListAsync();
            if (customerTransactions.Any()) await _customerOpenBalanceRepository.BulkDeleteAsync(customerTransactions);



            //step3 Update AccountCycle Id in table Tenant 
            var tenant = await GetCurrentTenantAsync();// _tenantRepository.GetAll().Where(t => t.AccountCycleId == @entity.Id).FirstOrDefault();
            tenant.UpdateAccountCycleId(reverseToEntity.Id);
            CheckErrors(await _tenantManager.UpdateAsync(tenant));
           
            CheckErrors(await _accountCycleManager.RemoveAsync(@entity));

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Close_Period_GetList)]
        public async Task<PagedResultDto<AccountCycleDetailOutput>> GetList(GetListAccountCycleInput input)
        {
            var @query = _accountCycleRepository.GetAll().AsNoTracking();
            var resultCount = await query.CountAsync();
            if (resultCount == 0)//if zero auto create one default for tenant account cycle id
            {
                var tenantId = AbpSession.GetTenantId();
                var userId = AbpSession.GetUserId();

                //update Account Cycle Id in table Tenant 
                int year = DateTime.Now.Year;
                DateTime startDate = new DateTime(year, 1, 1);

                var @entity = AccountCycle.Create(tenantId, userId, startDate, null, 2, 2);
                CheckErrors(await _accountCycleManager.CreateAsync(@entity));

                var tenant = await GetCurrentTenantAsync();
                tenant.UpdateAccountCycleId(entity.Id);
                CheckErrors(await _tenantManager.UpdateAsync(tenant));

            }
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<AccountCycleDetailOutput>(resultCount, ObjectMapper.Map<List<AccountCycleDetailOutput>>(@entities));
        }
    }
}
