using System;
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
using CorarlERP.Journals.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.AutoSequences;
using CorarlERP.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Reports;
using OfficeOpenXml;
using System.IO;
using CorarlERP.Locations;
using CorarlERP.Locks;
using CorarlERP.AccountCycles;
using CorarlERP.Common.Dto;
using CorarlERP.FileStorages;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.Bills;
using CorarlERP.Invoices;
using CorarlERP.CustomerCredits;
using CorarlERP.TransferOrders;
using CorarlERP.Productions;
using CorarlERP.BankTransfers;
using CorarlERP.PhysicalCount;
using CorarlERP.MultiTenancy;
using CorarlERP.MultiCurrencies;
using CorarlERP.Classes;
using Abp.Domain.Uow;
using CorarlERP.KitchenOrders;

namespace CorarlERP.Journals
{
    public class GeneralJournalAppService : ReportBaseClass, IGeneralJournalAppService
    {
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Class, long> _classRepository;

        private readonly IFileStorageManager _fileStorageManager;
        private readonly AppFolders _appFolders;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IRepository<KitchenOrder, Guid> _kitchenOrderRepository;

        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<CustomerCredit, Guid> _customerCreditRepository;
        private readonly IRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;
        private readonly IRepository<Production, Guid> _productionRepository;
        private readonly IRepository<BankTransfer, Guid> _bankTransferRepository;
        private readonly IRepository<PhysicalCount.PhysicalCount, Guid> _physicalCountRepository;
        private readonly ICorarlRepository<Tenant, int> _tenantRepository;
        private readonly ICorarlRepository<MultiCurrency, long> _multiCurrencyRepository;
        public GeneralJournalAppService(
            IRepository<KitchenOrder, Guid> kitchenOrderRepository,
            IRepository<Class, long> classRepository,
            IRepository<Bill, Guid> billRepository,
            IRepository<Invoice, Guid> invoiceRepository,
            IRepository<CustomerCredit, Guid> customerCreditRepository,
            IRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            IRepository<TransferOrder, Guid> transferOrderRepository,
            IRepository<Production, Guid> productionRepository,
            IRepository<BankTransfer, Guid> bankTransferRepository,
            IRepository<PhysicalCount.PhysicalCount, Guid> physicalCountRepository,
            IJournalManager journalManager,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Location, long> locationRepository,
            IRepository<Lock, long> lockRepository,
            AppFolders appFolders,
            IFileStorageManager fileStorageManager,
            IRepository<AccountCycle, long> accountCycleRepository,
            ICorarlRepository<Tenant, int> tenantRepository,
            ICorarlRepository<MultiCurrency, long> multiCurrencyRepository
        ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _locationRepository = locationRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _appFolders = appFolders;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.GeneralJournal);
            _autoSequenceManager = autoSequenceManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _lockRepository = lockRepository;
            _fileStorageManager = fileStorageManager;
            _billRepository = billRepository;
            _invoiceRepository = invoiceRepository;
            _customerCreditRepository = customerCreditRepository;
            _vendorCreditRepository = vendorCreditRepository;
            _transferOrderRepository = transferOrderRepository;
            _productionRepository = productionRepository;
            _bankTransferRepository = bankTransferRepository;
            _physicalCountRepository = physicalCountRepository;
            _tenantRepository = tenantRepository;
            _multiCurrencyRepository = multiCurrencyRepository;
            _classRepository = classRepository;
            _kitchenOrderRepository = kitchenOrderRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateJournalInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                 .Where(t => (t.LockKey == TransactionLockType.GeneralJournal)
                 && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            if (input.Debit != input.Credit)
            {
                throw new UserFriendlyException(L("TotalDebitAndTotalCreditCanNotBeDifferent"));
            }

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }
            var @entity = Journal.Create(tenantId, userId, input.JournalNo, input.Date, input.Memo,
                                        input.Debit, input.Credit, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);


            entity.UpdateGeneralJournal(@entity);
            entity.UpdateStatus(input.Status);


            #region journalItems           
            foreach (var j in input.JournalItems)
            {
                var @journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, j.AccountId, j.Description, j.Debit, j.Credit, PostingKey.None, null);
                CheckErrors(await _journalItemManager.CreateAsync(@journalItem));
            }
            #endregion
            CheckErrors(await _journalManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.GeneralJournal };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var @entity = await _journalManager.GetAsync(input.Id, true);

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                 .Where(t => (t.LockKey == TransactionLockType.GeneralJournal)
                 && t.IsLock == true && t.LockDate.Value.Date >= entity.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var JournalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == entity.Id).ToListAsync();

            foreach (var s in JournalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(s));
            }


            CheckErrors(await _journalManager.RemoveAsync(@entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.GeneralJournal };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _journalManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _journalManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _journalManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _journalManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Find)]
        public async Task<PagedResultDto<JournalGetListOutput>> Find(GetListJournalInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var query = (from p in _journalItemRepository.GetAll().AsNoTracking()
                         join o in _journalRepository.GetAll().AsNoTracking()
                         .Include(u => u.Location)
                         .Where(u => u.JournalType == JournalType.GeneralJournal)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, p => input.Locations.Contains(p.LocationId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                         p => p.JournalNo.ToLower().Contains(input.Filter.ToLower())) on p.JournalId equals o.Id
                         join v in _chartOfAccountRepository.GetAll().AsNoTracking()
                         .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Count > 0, p => input.ChartOfAccounts.Contains(p.Id)) on p.AccountId equals v.Id
                         group p by new { journal = o } into u
                         select new JournalGetListOutput
                         {
                             Id = u.Key.journal.Id,
                             // IsActive = u.Key.journal.IsActive,
                             Date = u.Key.journal.Date,
                             Credit = u.Key.journal.Credit,
                             Debit = u.Key.journal.Debit,
                             Memo = u.Key.journal.Memo,
                             JournalNo = u.Key.journal.JournalNo,
                             LocationId = u.Key.journal.LocationId,
                             LocationName = u.Key.journal.Location.LocationName,

                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<JournalGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_GetDetail)]
        public async Task<JournalDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @entity = await _journalManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var journalItems = await _journalItemRepository
                                    .GetAll()
                                    .AsNoTracking()
                                    .Include(u => u.Account)
                                    .Where(u => u.JournalId == entity.Id)
                                    .OrderBy(t => t.CreationTime)
                                    .ToListAsync();

            var result = ObjectMapper.Map<JournalDetailOutput>(@entity);

            result.LocationName = entity?.Location?.LocationName;
            result.LocationId = entity?.LocationId;
            result.JournalItems = ObjectMapper.Map<List<JournalItemDetailOutput>>(journalItems);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_GetList)]
        public async Task<PagedResultDto<JournalGetListOutput>> GetList(GetListJournalInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, input.Locations);

            var journalQuery = _journalRepository.GetAll()
                         .Where(u => u.JournalType == JournalType.GeneralJournal)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .AsNoTracking()
                         .Select(j => new
                         {
                             Id = j.Id,
                             JournalStatus = j.Status,
                             Date = j.Date,
                             journalCredit = j.Credit,
                             journalDebit = j.Debit,
                             journalMemo = j.Memo,
                             journalNo = j.JournalNo,
                             CreatorId = j.CreatorUser.Id,
                             locationId = j.LocationId,
                             creationTimeIndex = j.CreationTimeIndex,
                         });

            var journalItemQuery = _journalItemRepository.GetAll()
                                   .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Count > 0, p => input.ChartOfAccounts.Contains(p.AccountId))
                                   .AsNoTracking()
                                   .Select(s => s.JournalId);

            var query = from j in journalQuery
                        join u in userQuery
                        on j.CreatorId equals u.Id
                        join l in locationQuery
                        on j.locationId equals l.Id
                        join ji in journalItemQuery
                        on j.Id equals ji
                        into jis
                        where jis.Count() > 0
                        select new JournalGetListOutput
                        {
                            Id = j.Id,
                            StatusCode = j.JournalStatus,
                            StatusName = j.JournalStatus.ToString(),
                            Date = j.Date,
                            Credit = j.journalCredit,
                            Debit = j.journalDebit,
                            Memo = j.journalMemo,
                            JournalNo = j.journalNo,
                            User = new UserDto
                            {
                                Id = j.CreatorId,
                                UserName = u.UserName
                            },
                            LocationId = j.locationId,
                            LocationName = l.LocationName,
                            CreationTimeIndex = j.creationTimeIndex
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<JournalGetListOutput>(resultCount, new List<JournalGetListOutput>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    query = query.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    query = query.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("memo"))
                {
                    query = query.OrderByDescending(s => s.Memo).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("debit"))
                {
                    query = query.OrderByDescending(s => s.Debit).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("credit"))
                {
                    query = query.OrderByDescending(s => s.Credit).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderByDescending(s => s.StatusCode).ThenByDescending(s => s.CreationTimeIndex);
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
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    query = query.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("memo"))
                {
                    query = query.OrderBy(s => s.Memo).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("debit"))
                {
                    query = query.OrderBy(s => s.Debit).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("credit"))
                {
                    query = query.OrderBy(s => s.Credit).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderBy(s => s.StatusCode).ThenBy(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                }
            }

            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<JournalGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_GetList)]
        public async Task<PagedResultDto<JournalGetListOutput>> GetListOld(GetListJournalInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().Include(t => t.CreatorUser).Include(u => u.Location).AsNoTracking()
                         .Where(u => u.JournalType == JournalType.GeneralJournal)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                            (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                        u.Memo.ToLower().Contains(input.Filter.ToLower()))
                         on jItem.JournalId equals j.Id

                         join v in _chartOfAccountRepository.GetAll().AsNoTracking()
                         .WhereIf(input.ChartOfAccounts != null && input.ChartOfAccounts.Count > 0, p => input.ChartOfAccounts.Contains(p.Id))
                         on jItem.AccountId equals v.Id

                         group jItem by new
                         {
                             journalId = j.Id,
                             journalStatus = j.Status,
                             Date = j.Date,
                             journalCredit = j.Credit,
                             journalDebit = j.Debit,
                             journalMemo = j.Memo,
                             journalNo = j.JournalNo,
                             CreatorId = j.CreatorUser.Id,
                             CreatorName = j.CreatorUser.UserName,
                             locationName = j.Location.LocationName,
                             locationId = j.LocationId,
                             creationTimeIndex = j.CreationTimeIndex,
                             //journal = j
                         }
                         into u
                         select new JournalGetListOutput
                         {
                             Id = u.Key.journalId,
                             StatusCode = u.Key.journalStatus,
                             StatusName = u.Key.journalStatus.ToString(),
                             Date = u.Key.Date,
                             Credit = u.Key.journalCredit,
                             Debit = u.Key.journalDebit,
                             Memo = u.Key.journalMemo,
                             JournalNo = u.Key.journalNo,
                             User = new UserDto
                             {
                                 Id = u.Key.CreatorId,
                                 UserName = u.Key.CreatorName
                             },
                             LocationId = u.Key.locationId,
                             LocationName = u.Key.locationName,
                             CreationTimeIndex = u.Key.creationTimeIndex
                         });

            var resultCount = await query.CountAsync();

            List<JournalGetListOutput> @entities;

            if (input.Sorting.Contains("date") && !input.Sorting.Contains(".")) input.Sorting = input.Sorting.Replace("date", "Date.Date");

            if (input.Sorting.Contains("DESC"))
            {
                @entities = await query.OrderBy(input.Sorting).ThenByDescending(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            }
            else
            {
                @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<JournalGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateJournalInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }
            var @entity = await _journalManager.GetAsync(input.Id, true); //this is journal

            await CheckClosePeriod(entity.Date, input.Date);

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && t.LockKey == TransactionLockType.GeneralJournal).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (input.Debit != input.Credit)
            {
                throw new UserFriendlyException(L("TotalDebitAndTotalCreditCanNotBeDifferent"));
            }
            entity.Update(
                        userId, input.JournalNo, input.Date, input.Memo, input.Debit, input.Credit,
                        input.CurrencyId, input.ClassId, input.Reference, input.Status, input.LocationId);


            #region update journalItem
            var journalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == entity.Id).ToListAsync();
            foreach (var c in input.JournalItems)
            {
                if (c.Id != null)
                {
                    var journalItem = journalItems.FirstOrDefault(u => u.Id == c.Id);
                    if (journalItem != null)
                    {

                        journalItem.UpdateJournalItem(userId, c.AccountId, c.Description, c.Debit, c.Credit);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }
                }
                else if (c.Id == null)
                {

                    var journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, c.AccountId, c.Description, c.Debit, c.Credit, PostingKey.None, null);
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));

                }
            }

            var toDeleteJournalItem = journalItems.Where(u => !input.JournalItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteJournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }


            #endregion


            CheckErrors(await _journalManager.UpdateAsync(entity, null));
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.GeneralJournal };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToDraft();

            CheckErrors(await _journalManager.UpdateAsync(entity, null));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdatePublish();

            CheckErrors(await _journalManager.UpdateAsync(entity, null));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _journalManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateVoid();

            CheckErrors(await _journalManager.UpdateAsync(entity, null));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Journal_Update)]
        public async Task<NullableIdDto<Guid>> UpdateAccount(UpdateAccount input)
        {
            var tenant = await GetCurrentTenantAsync();
            if (tenant.IsDebug)
            {
                CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);
                CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant);
            }

            var @entity = await _journalItemManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (tenant.IsDebug)
            {
                CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant);
                CurrentUnitOfWork.EnableFilter(AbpDataFilters.MustHaveTenant);
            }

            entity.UpdateJournalItemAccount(input.AccountId);

            CheckErrors(await _journalItemManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            if (entity.Identifier.HasValue && entity.Key == PostingKey.Inventory)
            {
                var item = await _inventoryTransactionItemRepository.FirstOrDefaultAsync(s => s.Id == entity.Identifier);
                if (item != null && item.InventoryAccountId != input.AccountId)
                {
                    item.SetInventoryAccount(input.AccountId);
                    await _inventoryTransactionItemRepository.UpdateAsync(item);
                }
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Journal_Update)]
        public async Task<NullableIdDto<Guid>> UpdateJournalCreationTimeIndex(UpdateJournalCreationTimeIndex input)
        {
            if (input.CreationTimeIndex == null)
            {
                throw new UserFriendlyException(L("CreationTimeIndexCannotBeEmpty"));
            }

            var @entity = await _journalRepository.GetAll().Include(s => s.ItemIssue).Include(s => s.ItemReceipt)
                                .Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.UpdateCreationTimeIndex(input.CreationTimeIndex.Value);
            CheckErrors(await _journalManager.UpdateAsync(@entity, null));

            Guid? itemIssueTransferId = null;
            Guid? itemReceiptTransferId = null;
            if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var transferId = entity.ItemIssue.TransferOrderId;
                var receiptJouranl = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemReceipt.TransferOrderId.HasValue && s.ItemReceipt.TransferOrderId == transferId);
                if (receiptJouranl != null)
                {
                    receiptJouranl.UpdateCreationTimeIndex(input.CreationTimeIndex.Value + 1);
                    CheckErrors(await _journalManager.UpdateAsync(receiptJouranl, null));
                    itemReceiptTransferId = receiptJouranl.ItemReceiptId;
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var transferId = entity.ItemReceipt.TransferOrderId;
                var issueJouranl = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemIssue.TransferOrderId.HasValue && s.ItemIssue.TransferOrderId == transferId);
                if (issueJouranl != null)
                {
                    issueJouranl.UpdateCreationTimeIndex(input.CreationTimeIndex.Value - 1);
                    CheckErrors(await _journalManager.UpdateAsync(issueJouranl, null));
                    itemIssueTransferId = issueJouranl.ItemIssueId;
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCreditId.HasValue)
            {
                await SyncItemIssueVendorCredit(entity.ItemIssueVendorCreditId.Value);
                var itemIds = await _itemIssueVendorCreditItemRepository.GetAll()
                                    .Where(s => s.ItemIssueVendorCreditId == entity.ItemIssueVendorCreditId.Value)
                                    .AsNoTracking()
                                    .GroupBy(s => s.ItemId)
                                    .Select(s => s.Key)
                                    .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCreditId.HasValue)
            {
                await SyncItemReceiptCustomerCredit(entity.ItemReceiptCustomerCreditId.Value);
                var itemIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptCustomerCreditId == entity.ItemReceiptCustomerCreditId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.ItemReceiptId.HasValue)
            {
                await SyncItemReceipt(entity.ItemReceiptId.Value);
                if (itemIssueTransferId.HasValue) await SyncItemIssue(itemIssueTransferId.Value);
                var itemIds = await _itemReceiptItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptId == entity.ItemReceiptId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.ItemIssueId.HasValue)
            {
                await SyncItemIssue(entity.ItemIssueId.Value);
                if (itemReceiptTransferId.HasValue) await SyncItemReceipt(itemReceiptTransferId.Value);
                var itemIds = await _itemIssueItemRepository.GetAll()
                                   .Where(s => s.ItemIssueId == entity.ItemIssueId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_ChangeJournalDate)]
        public async Task UpdateJournalDate(UpdateJournalDateInput input)
        {
            var @entity = await _journalRepository.GetAll()
                                .Include(s => s.ItemIssue)
                                .Include(s => s.ItemReceipt)
                                .Include(s => s.ItemReceiptCustomerCredit)
                                .Include(s => s.ItemIssueVendorCredit)
                                .Include(s => s.Bill)
                                .Include(s => s.Invoice)
                                .Include(s => s.CustomerCredit)
                                .Include(s => s.VendorCredit)
                                .Include(s => s.Deposit)
                                .Include(s => s.withdraw)
                                .Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;


            if (entity.JournalType == JournalType.Bill)
            {
                if (entity.Date.Date == entity.Bill.DueDate.Date) entity.Bill.SetDueDate(input.Date);
                if (entity.Date.Date == entity.Bill.ETA.Date) entity.Bill.SetETA(input.Date);
                if (entity.Bill.ItemReceiptDate.HasValue && entity.Date.Date == entity.Bill.ItemReceiptDate.Value.Date) entity.Bill.SetItemReceiptDate(input.Date);

                await _billRepository.UpdateAsync(entity.Bill);

                if (entity.Bill.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptId == entity.Bill.ItemReceiptId).FirstOrDefaultAsync();

                    if (receiptJournal != null)
                    {
                        receiptJournal.ChangeDate(input.Date);
                        await _journalRepository.UpdateAsync(receiptJournal);

                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                        var itemIds = await _itemReceiptItemRepository.GetAll()
                                           .Where(s => s.ItemReceiptId == receiptJournal.ItemReceiptId)
                                           .AsNoTracking()
                                           .GroupBy(s => s.ItemId)
                                           .Select(s => s.Key)
                                           .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                    }
                }

            }
            else if (entity.JournalType == JournalType.Invoice)
            {
                if (entity.Date.Date == entity.Invoice.DueDate.Date) entity.Invoice.SetDueDate(input.Date);
                if (entity.Date.Date == entity.Invoice.ETD.Date) entity.Invoice.SetETD(input.Date);
                if (entity.Invoice.ReceiveDate.HasValue && entity.Date.Date == entity.Invoice.ReceiveDate.Value.Date) entity.Invoice.SetReceiveDate(input.Date);

                await _invoiceRepository.UpdateAsync(entity.Invoice);

                if (entity.Invoice.ConvertToItemIssue)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueId == entity.Invoice.ItemIssueId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {

                        issueJournal.ChangeDate(input.Date);
                        await _journalRepository.UpdateAsync(issueJournal);

                        await CurrentUnitOfWork.SaveChangesAsync();

                        await SyncItemIssue(issueJournal.ItemIssueId.Value);
                        var itemIds = await _itemIssueItemRepository.GetAll()
                                     .Where(s => s.ItemIssueId == issueJournal.ItemIssueId.Value)
                                     .AsNoTracking()
                                     .GroupBy(s => s.ItemId)
                                     .Select(s => s.Key)
                                     .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);
                    }
                }
            }
            else if (entity.JournalType == JournalType.CustomerCredit)
            {
                if (entity.Date.Date == entity.CustomerCredit.DueDate.Date) entity.CustomerCredit.SetDueDate(input.Date);
                if (entity.CustomerCredit.ReceiveDate.HasValue && entity.Date.Date == entity.CustomerCredit.ReceiveDate.Value.Date) entity.CustomerCredit.SetReceiveDate(input.Date);

                await _customerCreditRepository.UpdateAsync(entity.CustomerCredit);

                if (entity.CustomerCredit.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptCustomerCredit.CustomerCreditId == entity.CustomerCreditId).FirstOrDefaultAsync();

                    if (receiptJournal != null)
                    {
                        receiptJournal.ChangeDate(input.Date);
                        await _journalRepository.UpdateAsync(receiptJournal);


                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceiptCustomerCredit(receiptJournal.ItemReceiptCustomerCreditId.Value);
                        var itemIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                           .Where(s => s.ItemReceiptCustomerCreditId == entity.ItemReceiptCustomerCreditId.Value)
                                           .AsNoTracking()
                                           .GroupBy(s => s.ItemId)
                                           .Select(s => s.Key)
                                           .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                    }

                }

            }
            else if (entity.JournalType == JournalType.VendorCredit)
            {
                if (entity.Date.Date == entity.VendorCredit.DueDate.Date) entity.VendorCredit.SetDueDate(input.Date);
                if (entity.VendorCredit.IssueDate.HasValue && entity.Date.Date == entity.VendorCredit.IssueDate.Value.Date) entity.VendorCredit.SetIssueDate(input.Date);

                await _vendorCreditRepository.UpdateAsync(entity.VendorCredit);

                if (entity.VendorCredit.ConvertToItemIssueVendor)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueVendorCredit.VendorCreditId == entity.VendorCreditId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {
                        issueJournal.ChangeDate(input.Date);
                        await _journalRepository.UpdateAsync(issueJournal);

                        await CurrentUnitOfWork.SaveChangesAsync();

                        await SyncItemIssueVendorCredit(issueJournal.ItemIssueVendorCreditId.Value);
                        var itemIds = await _itemIssueVendorCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueVendorCreditId == issueJournal.ItemIssueVendorCreditId.Value)
                                            .AsNoTracking()
                                            .GroupBy(s => s.ItemId)
                                            .Select(s => s.Key)
                                            .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);

                    }

                }
            }
            else if (entity.JournalType == JournalType.Withdraw && entity.withdraw.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.withdraw.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {
                    bankTransfer.SetBankTransferDate(input.Date);

                    var depositJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Deposit)
                                                 .Where(t => t.Deposit.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();
                    if (depositJournal != null)
                    {
                        depositJournal.ChangeDate(input.Date);
                        await _journalRepository.UpdateAsync(depositJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.Deposit && entity.Deposit.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.Deposit.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {
                    bankTransfer.SetBankTransferDate(input.Date);

                    var withdrawJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Withdraw)
                                                 .Where(t => t.withdraw.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();
                    if (withdrawJournal != null)
                    {
                        withdrawJournal.ChangeDate(input.Date);
                        await _journalRepository.UpdateAsync(withdrawJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.TransferOrderId);
                if (transfer != null)
                {
                    transfer.SetTranferDate(input.Date);
                    await _transferOrderRepository.UpdateAsync(transfer);
                }

                var receiptJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemReceipt.TransferOrderId.HasValue && s.ItemReceipt.TransferOrderId == transfer.Id);
                if (receiptJournal != null)
                {
                    receiptJournal.ChangeDate(input.Date);
                    CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));


                    await CurrentUnitOfWork.SaveChangesAsync();
                    await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.TransferOrderId);
                if (transfer != null)
                {
                    transfer.SetTranferDate(input.Date);
                    await _transferOrderRepository.UpdateAsync(transfer);
                }

                var issueJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemIssue.TransferOrderId.HasValue && s.ItemIssue.TransferOrderId == transfer.Id);
                if (issueJournal != null)
                {
                    issueJournal.ChangeDate(input.Date);
                    CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));

                    await CurrentUnitOfWork.SaveChangesAsync();
                    await SyncItemIssue(issueJournal.ItemIssueId.Value);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.ProductionOrderId);
                if (production != null)
                {
                    if (entity.Date.Date == production.Date.Date) production.SetDate(input.Date);
                    if (production.ReceiptDate.HasValue && entity.Date.Date == production.ReceiptDate.Value.Date) production.SetReceiptDate(input.Date);
                    if (production.IssueDate.HasValue && entity.Date.Date == production.IssueDate.Value.Date) production.SetIssueDate(input.Date);

                    await _productionRepository.UpdateAsync(production);
                }

                var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.ProductionOrderId.HasValue && s.ItemReceipt.ProductionOrderId == production.Id);
                if (receiptJournal != null)
                {
                    if (entity.Date.Date == receiptJournal.Date.Date) receiptJournal.ChangeDate(input.Date);
                    CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));

                    await CurrentUnitOfWork.SaveChangesAsync();
                    await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                    var itemIds = await _itemReceiptItemRepository.GetAll()
                                          .Where(s => s.ItemReceiptId == receiptJournal.ItemReceiptId)
                                          .AsNoTracking()
                                          .GroupBy(s => s.ItemId)
                                          .Select(s => s.Key)
                                          .ToListAsync();
                    if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.ProductionOrderId);
                if (production != null)
                {
                    if (entity.Date.Date == production.Date.Date) production.SetDate(input.Date);
                    if (production.ReceiptDate.HasValue && entity.Date.Date == production.ReceiptDate.Value.Date) production.SetReceiptDate(input.Date);
                    if (production.IssueDate.HasValue && entity.Date.Date == production.IssueDate.Value.Date) production.SetIssueDate(input.Date);

                    await _productionRepository.UpdateAsync(production);
                }

                var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.ProductionOrderId == production.Id);
                if (issueJournal != null)
                {
                    if (entity.Date.Date == issueJournal.Date.Date) issueJournal.ChangeDate(input.Date);
                    CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));

                    await CurrentUnitOfWork.SaveChangesAsync();
                    await SyncItemIssue(issueJournal.ItemIssueId.Value);
                    var itemIds = await _itemIssueItemRepository.GetAll()
                                   .Where(s => s.ItemIssueId == issueJournal.ItemIssueId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                    if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssuePhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.PhysicalCountId);
                if (physicalCount != null)
                {
                    if (entity.Date.Date == physicalCount.PhysicalCountDate.Date) physicalCount.SetPhysicalCountDate(input.Date);
                    await _physicalCountRepository.UpdateAsync(physicalCount);
                }

                var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.PhysicalCountId.HasValue && s.ItemReceipt.PhysicalCountId == physicalCount.Id);
                if (receiptJournal != null)
                {
                    if (entity.Date.Date == receiptJournal.Date.Date) receiptJournal.ChangeDate(input.Date);
                    CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));

                    await CurrentUnitOfWork.SaveChangesAsync();
                    await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                    var itemIds = await _itemReceiptItemRepository.GetAll()
                                          .Where(s => s.ItemReceiptId == receiptJournal.ItemReceiptId)
                                          .AsNoTracking()
                                          .GroupBy(s => s.ItemId)
                                          .Select(s => s.Key)
                                          .ToListAsync();
                    if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.PhysicalCountId);
                if (physicalCount != null)
                {
                    if (entity.Date.Date == physicalCount.PhysicalCountDate.Date) physicalCount.SetPhysicalCountDate(input.Date);

                    await _physicalCountRepository.UpdateAsync(physicalCount);
                }

                var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.PhysicalCountId == physicalCount.Id);
                if (issueJournal != null)
                {
                    if (entity.Date.Date == issueJournal.Date.Date) issueJournal.ChangeDate(input.Date);
                    CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));

                    await CurrentUnitOfWork.SaveChangesAsync();
                    await SyncItemIssue(issueJournal.ItemIssueId.Value);
                    var itemIds = await _itemIssueItemRepository.GetAll()
                                   .Where(s => s.ItemIssueId == issueJournal.ItemIssueId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                    if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPurchase && entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
            {
                var billJournal = await _journalRepository.GetAll()
                                  .Include(s => s.Bill)
                                  .Where(s => s.BillId.HasValue)
                                  .Where(s => s.Bill.ConvertToItemReceipt)
                                  .FirstOrDefaultAsync(s => s.Bill.ItemReceiptId == entity.ItemReceiptId);
                if (billJournal != null && billJournal.Bill != null)
                {
                    billJournal.ChangeDate(input.Date);
                    await _journalRepository.UpdateAsync(billJournal);

                    if (entity.Date.Date == billJournal.Bill.DueDate.Date) billJournal.Bill.SetDueDate(input.Date);
                    if (entity.Date.Date == billJournal.Bill.ETA.Date) billJournal.Bill.SetETA(input.Date);
                    if (billJournal.Bill.ItemReceiptDate.HasValue && entity.Date.Date == billJournal.Bill.ItemReceiptDate.Value.Date) billJournal.Bill.SetItemReceiptDate(input.Date);

                    await _billRepository.UpdateAsync(billJournal.Bill);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueSale && entity.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice)
            {
                var invoiceJournal = await _journalRepository.GetAll()
                                           .Include(s => s.Invoice)
                                           .Where(s => s.InvoiceId.HasValue)
                                           .Where(s => s.Invoice.ConvertToItemIssue)
                                           .FirstOrDefaultAsync(s => s.Invoice.ItemIssueId == entity.ItemIssueId);

                if (invoiceJournal != null && invoiceJournal.Invoice != null)
                {
                    invoiceJournal.ChangeDate(input.Date);
                    await _journalRepository.UpdateAsync(invoiceJournal);

                    if (entity.Date.Date == invoiceJournal.Invoice.DueDate.Date) invoiceJournal.Invoice.SetDueDate(input.Date);
                    if (entity.Date.Date == invoiceJournal.Invoice.ETD.Date) invoiceJournal.Invoice.SetETD(input.Date);
                    if (invoiceJournal.Invoice.ReceiveDate.HasValue && entity.Date.Date == invoiceJournal.Invoice.ReceiveDate.Value.Date) invoiceJournal.Invoice.SetReceiveDate(input.Date);

                    await _invoiceRepository.UpdateAsync(invoiceJournal.Invoice);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCredit.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                var customerCreditJournal = await _journalRepository.GetAll()
                                                  .Include(s => s.CustomerCredit)
                                                  .Where(s => s.CustomerCreditId.HasValue)
                                                  .Where(s => s.CustomerCredit.ConvertToItemReceipt)
                                                  .FirstOrDefaultAsync(s => s.CustomerCreditId == entity.ItemReceiptCustomerCredit.CustomerCreditId);

                if (customerCreditJournal != null && customerCreditJournal.CustomerCredit != null)
                {
                    customerCreditJournal.ChangeDate(input.Date);
                    await _journalRepository.UpdateAsync(customerCreditJournal);

                    if (entity.Date.Date == customerCreditJournal.CustomerCredit.DueDate.Date) customerCreditJournal.CustomerCredit.SetDueDate(input.Date);
                    if (customerCreditJournal.CustomerCredit.ReceiveDate.HasValue && entity.Date.Date == customerCreditJournal.CustomerCredit.ReceiveDate.Value.Date) customerCreditJournal.CustomerCredit.SetReceiveDate(input.Date);

                    await _customerCreditRepository.UpdateAsync(customerCreditJournal.CustomerCredit);
                }

            }
            else if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCredit.ReceiveFrom == ReceiveFrom.VendorCredit)
            {
                var vendorCreditJournal = await _journalRepository.GetAll()
                                                .Include(s => s.VendorCredit)
                                                .Where(s => s.VendorCreditId.HasValue)
                                                .Where(s => s.VendorCredit.ConvertToItemIssueVendor)
                                                .FirstOrDefaultAsync(s => s.VendorCreditId == entity.ItemIssueVendorCredit.VendorCreditId);

                if (vendorCreditJournal != null && vendorCreditJournal.VendorCredit != null)
                {
                    vendorCreditJournal.ChangeDate(input.Date);
                    await _journalRepository.UpdateAsync(vendorCreditJournal);

                    if (entity.Date.Date == vendorCreditJournal.VendorCredit.DueDate.Date) vendorCreditJournal.VendorCredit.SetDueDate(input.Date);
                    if (vendorCreditJournal.VendorCredit.IssueDate.HasValue && entity.Date.Date == vendorCreditJournal.VendorCredit.IssueDate.Value.Date) vendorCreditJournal.VendorCredit.SetIssueDate(input.Date);

                    await _vendorCreditRepository.UpdateAsync(vendorCreditJournal.VendorCredit);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueKitchenOrder)
            {
                var kitchenOrder = await _kitchenOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.KitchenOrderId);

                if (kitchenOrder != null)
                {

                    kitchenOrder.ChangeDate(input.Date);
                    await _kitchenOrderRepository.UpdateAsync(kitchenOrder);
                }
            }


            entity.ChangeDate(input.Date);
            CheckErrors(await _journalManager.UpdateAsync(@entity, null));

            await CurrentUnitOfWork.SaveChangesAsync();

            if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCreditId.HasValue)
            {
                await SyncItemIssueVendorCredit(entity.ItemIssueVendorCreditId.Value);
                var itemIds = await _itemIssueVendorCreditItemRepository.GetAll()
                                    .Where(s => s.ItemIssueVendorCreditId == entity.ItemIssueVendorCreditId.Value)
                                    .AsNoTracking()
                                    .GroupBy(s => s.ItemId)
                                    .Select(s => s.Key)
                                    .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCreditId.HasValue)
            {
                await SyncItemReceiptCustomerCredit(entity.ItemReceiptCustomerCreditId.Value);
                var itemIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptCustomerCreditId == entity.ItemReceiptCustomerCreditId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.ItemReceiptId.HasValue)
            {
                await SyncItemReceipt(entity.ItemReceiptId.Value);
                var itemIds = await _itemReceiptItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptId == entity.ItemReceiptId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.ItemIssueId.HasValue)
            {
                await SyncItemIssue(entity.ItemIssueId.Value);
                var itemIds = await _itemIssueItemRepository.GetAll()
                                   .Where(s => s.ItemIssueId == entity.ItemIssueId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }

        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Update)]
        public async Task ChangeJournalCurrency(ChangeJournalCurrencyInput input)
        {
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;
            var userMultiCurrency = await _multiCurrencyRepository.GetAll().AsNoTracking().Where(s => s.TenantId == tenantId).AnyAsync();
            var companyCurrency = await _tenantRepository.GetAll().AsNoTracking().Where(s => s.Id == tenantId).Select(s => s.CurrencyId).FirstOrDefaultAsync();

            var @entity = await _journalRepository.GetAll()
                               .Include(s => s.ItemIssue)
                               .Include(s => s.ItemReceipt)
                               .Include(s => s.ItemReceiptCustomerCredit)
                               .Include(s => s.ItemIssueVendorCredit)
                               .Include(s => s.Bill)
                               .Include(s => s.Invoice)
                               .Include(s => s.CustomerCredit)
                               .Include(s => s.VendorCredit)
                               .Include(s => s.Deposit)
                               .Include(s => s.withdraw)
                               .Where(t => t.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }


            if (!userMultiCurrency)
            {
                entity.SetCurrency(input.CurrencyId, input.CurrencyId);
            }
            else
            {
                entity.SetCurrency(companyCurrency, input.CurrencyId);
            }

            if (entity.JournalType == JournalType.Bill)
            {
                if (entity.Bill.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptId == entity.Bill.ItemReceiptId).FirstOrDefaultAsync();

                    if (receiptJournal != null)
                    {

                        if (!userMultiCurrency)
                        {
                            receiptJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            receiptJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        await _journalRepository.UpdateAsync(receiptJournal);

                    }
                }

            }
            else if (entity.JournalType == JournalType.Invoice)
            {

                if (entity.Invoice.ConvertToItemIssue)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueId == entity.Invoice.ItemIssueId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {

                        if (!userMultiCurrency)
                        {
                            issueJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            issueJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        await _journalRepository.UpdateAsync(issueJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.CustomerCredit)
            {
                if (entity.CustomerCredit.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptCustomerCredit.CustomerCreditId == entity.CustomerCreditId).FirstOrDefaultAsync();

                    if (receiptJournal != null)
                    {

                        if (!userMultiCurrency)
                        {
                            receiptJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            receiptJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        await _journalRepository.UpdateAsync(receiptJournal);
                    }

                }

            }
            else if (entity.JournalType == JournalType.VendorCredit)
            {
                if (entity.VendorCredit.ConvertToItemIssueVendor)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueVendorCredit.VendorCreditId == entity.VendorCreditId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {

                        if (!userMultiCurrency)
                        {
                            issueJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            issueJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        await _journalRepository.UpdateAsync(issueJournal);
                    }

                }
            }
            else if (entity.JournalType == JournalType.Withdraw && entity.withdraw.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.withdraw.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {
                    var depositJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Deposit)
                                                 .Where(t => t.Deposit.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();

                    if (depositJournal != null)
                    {

                        if (!userMultiCurrency)
                        {
                            depositJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            depositJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        await _journalRepository.UpdateAsync(depositJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.Deposit && entity.Deposit.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.Deposit.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {

                    var withdrawJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Withdraw)
                                                 .Where(t => t.withdraw.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();

                    if (withdrawJournal != null)
                    {
                        if (!userMultiCurrency)
                        {
                            withdrawJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            withdrawJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        await _journalRepository.UpdateAsync(withdrawJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.TransferOrderId);
                var receiptJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemReceipt.TransferOrderId.HasValue && s.ItemReceipt.TransferOrderId == transfer.Id);
                if (receiptJournal != null)
                {

                    if (!userMultiCurrency)
                    {
                        receiptJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        receiptJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    await _journalRepository.UpdateAsync(receiptJournal);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.TransferOrderId);
                var issueJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemIssue.TransferOrderId.HasValue && s.ItemIssue.TransferOrderId == transfer.Id);
                if (issueJournal != null)
                {

                    if (!userMultiCurrency)
                    {
                        issueJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        issueJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    await _journalRepository.UpdateAsync(issueJournal);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.ProductionOrderId);
                var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.ProductionOrderId.HasValue && s.ItemReceipt.ProductionOrderId == production.Id);
                if (receiptJournal != null)
                {
                    if (receiptJournal != null)
                    {

                        if (!userMultiCurrency)
                        {
                            receiptJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                        }
                        else
                        {
                            receiptJournal.SetCurrency(companyCurrency, input.CurrencyId);
                        }
                        CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                    }

                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.ProductionOrderId);
                var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.ProductionOrderId == production.Id);
                if (issueJournal != null)
                {

                    if (!userMultiCurrency)
                    {
                        issueJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        issueJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemIssuePhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.PhysicalCountId);
                var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.PhysicalCountId.HasValue && s.ItemReceipt.PhysicalCountId == physicalCount.Id);
                if (receiptJournal != null)
                {

                    if (!userMultiCurrency)
                    {
                        receiptJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        receiptJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.PhysicalCountId);
                var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.PhysicalCountId == physicalCount.Id);
                if (issueJournal != null)
                {
                    if (!userMultiCurrency)
                    {
                        issueJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        issueJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPurchase && entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
            {
                var billJournal = await _journalRepository.GetAll()
                                  .Include(s => s.Bill)
                                  .Where(s => s.BillId.HasValue)
                                  .Where(s => s.Bill.ConvertToItemReceipt)
                                  .FirstOrDefaultAsync(s => s.Bill.ItemReceiptId == entity.ItemReceiptId);
                if (billJournal != null && billJournal.Bill != null)
                {
                    if (!userMultiCurrency)
                    {
                        billJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        billJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(billJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueSale && entity.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice)
            {
                var invoiceJournal = await _journalRepository.GetAll()
                                           .Include(s => s.Invoice)
                                           .Where(s => s.InvoiceId.HasValue)
                                           .Where(s => s.Invoice.ConvertToItemIssue)
                                           .FirstOrDefaultAsync(s => s.Invoice.ItemIssueId == entity.ItemIssueId);

                if (invoiceJournal != null && invoiceJournal.Invoice != null)
                {
                    if (!userMultiCurrency)
                    {
                        invoiceJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        invoiceJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(invoiceJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCredit.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                var customerCreditJournal = await _journalRepository.GetAll()
                                                  .Include(s => s.CustomerCredit)
                                                  .Where(s => s.CustomerCreditId.HasValue)
                                                  .Where(s => s.CustomerCredit.ConvertToItemReceipt)
                                                  .FirstOrDefaultAsync(s => s.CustomerCreditId == entity.ItemReceiptCustomerCredit.CustomerCreditId);

                if (customerCreditJournal != null && customerCreditJournal.CustomerCredit != null)
                {
                    if (!userMultiCurrency)
                    {
                        customerCreditJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        customerCreditJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(customerCreditJournal, null));
                }

            }
            else if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCredit.ReceiveFrom == ReceiveFrom.VendorCredit)
            {
                var vendorCreditJournal = await _journalRepository.GetAll()
                                                .Include(s => s.VendorCredit)
                                                .Where(s => s.VendorCreditId.HasValue)
                                                .Where(s => s.VendorCredit.ConvertToItemIssueVendor)
                                                .FirstOrDefaultAsync(s => s.VendorCreditId == entity.ItemIssueVendorCredit.VendorCreditId);

                if (vendorCreditJournal != null && vendorCreditJournal.VendorCredit != null)
                {
                    if (!userMultiCurrency)
                    {
                        vendorCreditJournal.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        vendorCreditJournal.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    CheckErrors(await _journalManager.UpdateAsync(vendorCreditJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueKitchenOrder)
            {
                var kitchenOrder = await _kitchenOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.KitchenOrderId);

                if (kitchenOrder != null)
                {
                    if (!userMultiCurrency)
                    {
                        kitchenOrder.SetCurrency(input.CurrencyId, input.CurrencyId);
                    }
                    else
                    {
                        kitchenOrder.SetCurrency(companyCurrency, input.CurrencyId);
                    }
                    await _kitchenOrderRepository.UpdateAsync(kitchenOrder);
                }
            }


            CheckErrors(await _journalManager.UpdateAsync(@entity, null));
            //await CurrentUnitOfWork.SaveChangesAsync();

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Update)]
        public async Task ChangeJournalClass(ChangeJournalClassInput input)
        {
            var isClass = await _classRepository.GetAll().Where(s => s.Id == input.ClassId).AsNoTracking().AnyAsync();
            if (!isClass) throw new UserFriendlyException(L("IsNotValid", L("Class")));
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;          
            var @entity = await _journalRepository.GetAll()
                               .Include(s => s.ItemIssue)
                               .Include(s => s.ItemReceipt)
                               .Include(s => s.ItemReceiptCustomerCredit)
                               .Include(s => s.ItemIssueVendorCredit)
                               .Include(s => s.Bill)
                               .Include(s => s.Invoice)
                               .Include(s => s.CustomerCredit)
                               .Include(s => s.VendorCredit)
                               .Include(s => s.Deposit)
                               .Include(s => s.withdraw)
                               .Where(t => t.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }          
           
            
            if (entity.JournalType == JournalType.Bill)
            {
                if (entity.Bill.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptId == entity.Bill.ItemReceiptId).FirstOrDefaultAsync();
                    if (receiptJournal != null)
                    {

                        receiptJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(receiptJournal);

                    }
                }

            }
            else if (entity.JournalType == JournalType.Invoice)
            {

                if (entity.Invoice.ConvertToItemIssue)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueId == entity.Invoice.ItemIssueId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {

                        issueJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(issueJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.CustomerCredit)
            {
                if (entity.CustomerCredit.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptCustomerCredit.CustomerCreditId == entity.CustomerCreditId).FirstOrDefaultAsync();
                    if (receiptJournal != null)
                    {
                        receiptJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(receiptJournal);
                    }

                }

            }
            else if (entity.JournalType == JournalType.VendorCredit)
            {
                if (entity.VendorCredit.ConvertToItemIssueVendor)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueVendorCredit.VendorCreditId == entity.VendorCreditId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {
                        issueJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(issueJournal);
                    }

                }
            }
            else if (entity.JournalType == JournalType.Withdraw && entity.withdraw.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.withdraw.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {
                    if (bankTransfer.TransferFromClassId == entity.ClassId) bankTransfer.SetFromClass(input.ClassId);
                    if (bankTransfer.TransferToClassId == entity.ClassId) bankTransfer.SetToClass(input.ClassId);
                    await _bankTransferRepository.UpdateAsync(bankTransfer);

                    var depositJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Deposit)
                                                 .Where(t => t.Deposit.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();

                    if (depositJournal != null)
                    {
                        if(depositJournal.ClassId == entity.ClassId) depositJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(depositJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.Deposit && entity.Deposit.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.Deposit.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {
                    if (bankTransfer.TransferFromClassId == entity.ClassId) bankTransfer.SetFromClass(input.ClassId);
                    if (bankTransfer.TransferToClassId == entity.ClassId) bankTransfer.SetToClass(input.ClassId);
                    await _bankTransferRepository.UpdateAsync(bankTransfer);

                    var withdrawJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Withdraw)
                                                 .Where(t => t.withdraw.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();

                    if (withdrawJournal != null)
                    {
                        if(withdrawJournal.ClassId == entity.ClassId) withdrawJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(withdrawJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.TransferOrderId);

                if(transfer != null)
                {
                    if (transfer.TransferFromClassId == entity.ClassId) transfer.SetFromClass(input.ClassId);
                    if (transfer.TransferToClassId == entity.ClassId) transfer.SetToClass(input.ClassId);
                    await _transferOrderRepository.UpdateAsync(transfer);

                    var receiptJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemReceipt.TransferOrderId.HasValue && s.ItemReceipt.TransferOrderId == transfer.Id);
                    if (receiptJournal != null)
                    {

                        if (receiptJournal.ClassId == entity.ClassId) receiptJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(receiptJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.TransferOrderId);

                if(transfer != null)
                {
                    if (transfer.TransferFromClassId == entity.ClassId) transfer.SetFromClass(input.ClassId);
                    if (transfer.TransferToClassId == entity.ClassId) transfer.SetToClass(input.ClassId);
                    await _transferOrderRepository.UpdateAsync(transfer);

                    var issueJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemIssue.TransferOrderId.HasValue && s.ItemIssue.TransferOrderId == transfer.Id);
                    if (issueJournal != null)
                    {

                        issueJournal.SetClass(input.ClassId);
                        await _journalRepository.UpdateAsync(issueJournal);
                    }
                }              
            }
            else if (entity.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.ProductionOrderId);

                if(production != null)
                {
                    if (production.FromClassId == entity.ClassId) production.SetFromClass(input.ClassId);
                    if (production.ToClassId == entity.ClassId) production.SetToClass(input.ClassId);
                    await _productionRepository.UpdateAsync(production);


                    var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.ProductionOrderId.HasValue && s.ItemReceipt.ProductionOrderId == production.Id);
                    if (receiptJournal != null)
                    {
                        if (receiptJournal != null)
                        {
                            if(receiptJournal.ClassId == entity.ClassId) receiptJournal.SetClass(input.ClassId);
                            CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                        }

                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.ProductionOrderId);

                if(production != null)
                {
                    if (production.FromClassId == entity.ClassId) production.SetFromClass(input.ClassId);
                    if (production.ToClassId == entity.ClassId) production.SetToClass(input.ClassId);
                    await _productionRepository.UpdateAsync(production);

                    var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.ProductionOrderId == production.Id);
                    if (issueJournal != null)
                    {
                        if(issueJournal.ClassId == entity.ClassId)  issueJournal.SetClass(input.ClassId);
                        CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));
                    }
                }               
            }
            else if (entity.JournalType == JournalType.ItemIssuePhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.PhysicalCountId);

                if(physicalCount != null)
                {

                    physicalCount.SetClass(input.ClassId);
                    await _physicalCountRepository.UpdateAsync(physicalCount);

                    var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.PhysicalCountId.HasValue && s.ItemReceipt.PhysicalCountId == physicalCount.Id);
                    if (receiptJournal != null)
                    {
                        receiptJournal.SetClass(input.ClassId);
                        CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.PhysicalCountId);

                if(physicalCount != null)
                {
                    physicalCount.SetClass(input.ClassId);
                    await _physicalCountRepository.UpdateAsync(physicalCount);

                    var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.PhysicalCountId == physicalCount.Id);
                    if (issueJournal != null)
                    {
                        issueJournal.SetClass(input.ClassId);
                        CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPurchase && entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
            {
                var billJournal = await _journalRepository.GetAll()
                                  .Include(s => s.Bill)
                                  .Where(s => s.BillId.HasValue)
                                  .Where(s => s.Bill.ConvertToItemReceipt)
                                  .FirstOrDefaultAsync(s => s.Bill.ItemReceiptId == entity.ItemReceiptId);
                if (billJournal != null && billJournal.Bill != null)
                {
                    billJournal.SetClass(input.ClassId);
                    CheckErrors(await _journalManager.UpdateAsync(billJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueSale && entity.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice)
            {
                var invoiceJournal = await _journalRepository.GetAll()
                                           .Include(s => s.Invoice)
                                           .Where(s => s.InvoiceId.HasValue)
                                           .Where(s => s.Invoice.ConvertToItemIssue)
                                           .FirstOrDefaultAsync(s => s.Invoice.ItemIssueId == entity.ItemIssueId);

                if (invoiceJournal != null && invoiceJournal.Invoice != null)
                {
                    invoiceJournal.SetClass(input.ClassId);
                    CheckErrors(await _journalManager.UpdateAsync(invoiceJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCredit.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                var customerCreditJournal = await _journalRepository.GetAll()
                                                  .Include(s => s.CustomerCredit)
                                                  .Where(s => s.CustomerCreditId.HasValue)
                                                  .Where(s => s.CustomerCredit.ConvertToItemReceipt)
                                                  .FirstOrDefaultAsync(s => s.CustomerCreditId == entity.ItemReceiptCustomerCredit.CustomerCreditId);

                if (customerCreditJournal != null && customerCreditJournal.CustomerCredit != null)
                {
                    customerCreditJournal.SetClass(input.ClassId);
                    CheckErrors(await _journalManager.UpdateAsync(customerCreditJournal, null));
                }

            }
            else if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCredit.ReceiveFrom == ReceiveFrom.VendorCredit)
            {
                var vendorCreditJournal = await _journalRepository.GetAll()
                                                .Include(s => s.VendorCredit)
                                                .Where(s => s.VendorCreditId.HasValue)
                                                .Where(s => s.VendorCredit.ConvertToItemIssueVendor)
                                                .FirstOrDefaultAsync(s => s.VendorCreditId == entity.ItemIssueVendorCredit.VendorCreditId);

                if (vendorCreditJournal != null && vendorCreditJournal.VendorCredit != null)
                {
                    vendorCreditJournal.SetClass(input.ClassId);
                    CheckErrors(await _journalManager.UpdateAsync(vendorCreditJournal, null));
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueKitchenOrder)
            {
                var kitchenOrder = await _kitchenOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.KitchenOrderId);

                if (kitchenOrder != null)
                {

                    kitchenOrder.SetClass(input.ClassId);
                    await _kitchenOrderRepository.UpdateAsync(kitchenOrder);
                }
            }

            entity.SetClass(input.ClassId);

            CheckErrors(await _journalManager.UpdateAsync(@entity, null));      

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_Update)]
        public async Task ChangeJournalLocation(ChangeJournalLocationInput input)
        {

            var isLocation = await _locationRepository.GetAll().Where(s => s.Id == input.LocationId).AsNoTracking().AnyAsync();
            if (!isLocation) throw new UserFriendlyException(L("IsNotValid", L("Location")));
            var @entity = await _journalRepository.GetAll()
                                .Include(s => s.ItemIssue)
                                .Include(s => s.ItemReceipt)
                                .Include(s => s.ItemReceiptCustomerCredit)
                                .Include(s => s.ItemIssueVendorCredit)
                                .Include(s => s.Bill)
                                .Include(s => s.Invoice)
                                .Include(s => s.CustomerCredit)
                                .Include(s => s.VendorCredit)
                                .Include(s => s.Deposit)
                                .Include(s => s.withdraw)
                                .Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;


            if (entity.JournalType == JournalType.Bill)
            {
               
                if (entity.Bill.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptId == entity.Bill.ItemReceiptId).FirstOrDefaultAsync();

                    if (receiptJournal != null)
                    {
                        receiptJournal.SetLocation(input.LocationId);
                        await _journalRepository.UpdateAsync(receiptJournal);

                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                        var itemIds = await _itemReceiptItemRepository.GetAll()
                                           .Where(s => s.ItemReceiptId == receiptJournal.ItemReceiptId)
                                           .AsNoTracking()
                                           .GroupBy(s => s.ItemId)
                                           .Select(s => s.Key)
                                           .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                    }
                }

            }
            else if (entity.JournalType == JournalType.Invoice)
            {               

                if (entity.Invoice.ConvertToItemIssue)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueId == entity.Invoice.ItemIssueId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {

                        issueJournal.SetLocation(input.LocationId);
                        await _journalRepository.UpdateAsync(issueJournal);

                        await CurrentUnitOfWork.SaveChangesAsync();

                        await SyncItemIssue(issueJournal.ItemIssueId.Value);
                        var itemIds = await _itemIssueItemRepository.GetAll()
                                     .Where(s => s.ItemIssueId == issueJournal.ItemIssueId.Value)
                                     .AsNoTracking()
                                     .GroupBy(s => s.ItemId)
                                     .Select(s => s.Key)
                                     .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);
                    }
                }
            }
            else if (entity.JournalType == JournalType.CustomerCredit)
            {
              

                if (entity.CustomerCredit.ConvertToItemReceipt)
                {
                    var receiptJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemReceiptCustomerCredit.CustomerCreditId == entity.CustomerCreditId).FirstOrDefaultAsync();

                    if (receiptJournal != null)
                    {
                        receiptJournal.SetLocation(input.LocationId);
                        await _journalRepository.UpdateAsync(receiptJournal);


                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceiptCustomerCredit(receiptJournal.ItemReceiptCustomerCreditId.Value);
                        var itemIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                           .Where(s => s.ItemReceiptCustomerCreditId == entity.ItemReceiptCustomerCreditId.Value)
                                           .AsNoTracking()
                                           .GroupBy(s => s.ItemId)
                                           .Select(s => s.Key)
                                           .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                    }

                }

            }
            else if (entity.JournalType == JournalType.VendorCredit)
            {             

                if (entity.VendorCredit.ConvertToItemIssueVendor)
                {
                    var issueJournal = await _journalRepository.GetAll()
                                               .Where(t => t.ItemIssueVendorCredit.VendorCreditId == entity.VendorCreditId).FirstOrDefaultAsync();

                    if (issueJournal != null)
                    {
                        issueJournal.SetLocation(input.LocationId);
                        await _journalRepository.UpdateAsync(issueJournal);

                        await CurrentUnitOfWork.SaveChangesAsync();

                        await SyncItemIssueVendorCredit(issueJournal.ItemIssueVendorCreditId.Value);
                        var itemIds = await _itemIssueVendorCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueVendorCreditId == issueJournal.ItemIssueVendorCreditId.Value)
                                            .AsNoTracking()
                                            .GroupBy(s => s.ItemId)
                                            .Select(s => s.Key)
                                            .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);

                    }

                }
            }
            else if (entity.JournalType == JournalType.Withdraw && entity.withdraw.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.withdraw.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {

                    if (bankTransfer.FromLocationId == entity.LocationId) bankTransfer.SetFromLocation(input.LocationId);
                    if (bankTransfer.ToLocationId == entity.LocationId) bankTransfer.SetToLocation(input.LocationId);
                    await _bankTransferRepository.UpdateAsync(bankTransfer);

                    var depositJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Deposit)
                                                 .Where(t => t.Deposit.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();
                    if (depositJournal != null)
                    {
                        if(depositJournal.LocationId == entity.LocationId)  depositJournal.SetLocation(input.LocationId);
                        await _journalRepository.UpdateAsync(depositJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.Deposit && entity.Deposit.BankTransferId.HasValue)
            {
                var bankTransfer = await _bankTransferRepository.GetAll().Where(s => s.Id == entity.Deposit.BankTransferId.Value).FirstOrDefaultAsync();
                if (bankTransfer != null)
                {

                    if (bankTransfer.FromLocationId == entity.LocationId) bankTransfer.SetFromLocation(input.LocationId);
                    if (bankTransfer.ToLocationId == entity.LocationId) bankTransfer.SetToLocation(input.LocationId);
                    await _bankTransferRepository.UpdateAsync(bankTransfer);

                    var withdrawJournal = await _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.Withdraw)
                                                 .Where(t => t.withdraw.BankTransferId == bankTransfer.Id).FirstOrDefaultAsync();
                    if (withdrawJournal != null)
                    {
                        if(withdrawJournal.LocationId == entity.LocationId)  withdrawJournal.SetLocation(input.LocationId);
                        await _journalRepository.UpdateAsync(withdrawJournal);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.TransferOrderId);
                
                if(transfer != null)
                {
                    if (transfer.TransferFromLocationId == entity.LocationId) transfer.SetFromLocation(input.LocationId);
                    if (transfer.TransferToLocationId == entity.LocationId) transfer.SetToLocation(input.LocationId);
                    await _transferOrderRepository.UpdateAsync(transfer);

                    var receiptJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemReceipt.TransferOrderId.HasValue && s.ItemReceipt.TransferOrderId == transfer.Id);
                    if (receiptJournal != null)
                    {
                        if(receiptJournal.LocationId == entity.LocationId) receiptJournal.SetLocation(input.LocationId);
                        CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                    }
                }               
            }
            else if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.TransferOrderId);
                
                if(transfer != null)
                {
                    if (transfer.TransferFromLocationId == entity.LocationId) transfer.SetFromLocation(input.LocationId);
                    if (transfer.TransferToLocationId == entity.LocationId) transfer.SetToLocation(input.LocationId);
                    await _transferOrderRepository.UpdateAsync(transfer);

                    var issueJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemIssue.TransferOrderId.HasValue && s.ItemIssue.TransferOrderId == transfer.Id);
                    if (issueJournal != null)
                    {
                        issueJournal.SetLocation(input.LocationId);
                        CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));

                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemIssue(issueJournal.ItemIssueId.Value);
                    }
                }               
            }
            else if (entity.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.ProductionOrderId);

                if(production != null)
                {
                    if (production.FromLocationId == entity.LocationId) production.SetFromLocation(input.LocationId);
                    if (production.ToLocationId == entity.LocationId) production.SetToLocation(input.LocationId);
                    await _productionRepository.UpdateAsync(production);


                    var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.ProductionOrderId.HasValue && s.ItemReceipt.ProductionOrderId == production.Id);
                    if (receiptJournal != null)
                    {
                        if(receiptJournal.LocationId == entity.LocationId)  receiptJournal.SetLocation(input.LocationId);
                        CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                        var itemIds = await _itemReceiptItemRepository.GetAll()
                                              .Where(s => s.ItemReceiptId == receiptJournal.ItemReceiptId)
                                              .AsNoTracking()
                                              .GroupBy(s => s.ItemId)
                                              .Select(s => s.Key)
                                              .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                    }
                }

               
            }
            else if (entity.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.ProductionOrderId);
                
                if(production != null)
                {
                    if (production.FromLocationId == entity.LocationId) production.SetFromLocation(input.LocationId);
                    if (production.ToLocationId == entity.LocationId) production.SetToLocation(input.LocationId);
                    await _productionRepository.UpdateAsync(production);

                    var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.ProductionOrderId == production.Id);
                    if (issueJournal != null)
                    {
                        if(issueJournal.LocationId == entity.LocationId)  issueJournal.SetLocation(input.LocationId);
                        CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));

                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemIssue(issueJournal.ItemIssueId.Value);
                        var itemIds = await _itemIssueItemRepository.GetAll()
                                       .Where(s => s.ItemIssueId == issueJournal.ItemIssueId.Value)
                                       .AsNoTracking()
                                       .GroupBy(s => s.ItemId)
                                       .Select(s => s.Key)
                                       .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemIssuePhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.PhysicalCountId);

                if(physicalCount != null)
                {
                    if (physicalCount.LocationId == entity.LocationId) physicalCount.SetLocation(input.LocationId);
                    await _physicalCountRepository.UpdateAsync(physicalCount);

                    var receiptJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemReceipt.PhysicalCountId.HasValue && s.ItemReceipt.PhysicalCountId == physicalCount.Id);
                    if (receiptJournal != null)
                    {
                       if(physicalCount.LocationId == entity.LocationId)  receiptJournal.SetLocation(input.LocationId);
                        CheckErrors(await _journalManager.UpdateAsync(receiptJournal, null));
                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemReceipt(receiptJournal.ItemReceiptId.Value);
                        var itemIds = await _itemReceiptItemRepository.GetAll()
                                              .Where(s => s.ItemReceiptId == receiptJournal.ItemReceiptId)
                                              .AsNoTracking()
                                              .GroupBy(s => s.ItemId)
                                              .Select(s => s.Key)
                                              .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, receiptJournal.Date, itemIds);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPhysicalCount)
            {
                var physicalCount = await _physicalCountRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemReceipt.PhysicalCountId);

                if(physicalCount != null)
                {
                    if (physicalCount.LocationId == entity.LocationId) physicalCount.SetLocation(input.LocationId);
                    await _physicalCountRepository.UpdateAsync(physicalCount);

                    var issueJournal = await _journalRepository.GetAll().FirstAsync(s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.PhysicalCountId == physicalCount.Id);
                    if (issueJournal != null)
                    {
                       if(issueJournal.LocationId == entity.LocationId) issueJournal.SetLocation(input.LocationId);
                        CheckErrors(await _journalManager.UpdateAsync(issueJournal, null));

                        await CurrentUnitOfWork.SaveChangesAsync();
                        await SyncItemIssue(issueJournal.ItemIssueId.Value);
                        var itemIds = await _itemIssueItemRepository.GetAll()
                                       .Where(s => s.ItemIssueId == issueJournal.ItemIssueId.Value)
                                       .AsNoTracking()
                                       .GroupBy(s => s.ItemId)
                                       .Select(s => s.Key)
                                       .ToListAsync();
                        if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, issueJournal.Date, itemIds);
                    }
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPurchase && entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
            {
                var billJournal = await _journalRepository.GetAll()
                                  .Include(s => s.Bill)
                                  .Where(s => s.BillId.HasValue)
                                  .Where(s => s.Bill.ConvertToItemReceipt)
                                  .FirstOrDefaultAsync(s => s.Bill.ItemReceiptId == entity.ItemReceiptId);
                if (billJournal != null && billJournal.Bill != null)
                {
                    billJournal.SetLocation(input.LocationId);
                    await _journalRepository.UpdateAsync(billJournal);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueSale && entity.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice)
            {
                var invoiceJournal = await _journalRepository.GetAll()
                                           .Include(s => s.Invoice)
                                           .Where(s => s.InvoiceId.HasValue)
                                           .Where(s => s.Invoice.ConvertToItemIssue)
                                           .FirstOrDefaultAsync(s => s.Invoice.ItemIssueId == entity.ItemIssueId);

                if (invoiceJournal != null && invoiceJournal.Invoice != null)
                {
                    invoiceJournal.SetLocation(input.LocationId);
                    await _journalRepository.UpdateAsync(invoiceJournal);
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCredit.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                var customerCreditJournal = await _journalRepository.GetAll()
                                                  .Include(s => s.CustomerCredit)
                                                  .Where(s => s.CustomerCreditId.HasValue)
                                                  .Where(s => s.CustomerCredit.ConvertToItemReceipt)
                                                  .FirstOrDefaultAsync(s => s.CustomerCreditId == entity.ItemReceiptCustomerCredit.CustomerCreditId);

                if (customerCreditJournal != null && customerCreditJournal.CustomerCredit != null)
                {
                    customerCreditJournal.SetLocation(input.LocationId);
                    await _journalRepository.UpdateAsync(customerCreditJournal);
                }

            }
            else if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCredit.ReceiveFrom == ReceiveFrom.VendorCredit)
            {
                var vendorCreditJournal = await _journalRepository.GetAll()
                                                .Include(s => s.VendorCredit)
                                                .Where(s => s.VendorCreditId.HasValue)
                                                .Where(s => s.VendorCredit.ConvertToItemIssueVendor)
                                                .FirstOrDefaultAsync(s => s.VendorCreditId == entity.ItemIssueVendorCredit.VendorCreditId);

                if (vendorCreditJournal != null && vendorCreditJournal.VendorCredit != null)
                {
                    vendorCreditJournal.SetLocation(input.LocationId);
                    await _journalRepository.UpdateAsync(vendorCreditJournal);
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueKitchenOrder)
            {
                var kitchenOrder = await _kitchenOrderRepository.GetAll().FirstOrDefaultAsync(s => s.Id == entity.ItemIssue.KitchenOrderId);

                if (kitchenOrder != null)
                {

                    kitchenOrder.SetLocation(input.LocationId);
                    await _kitchenOrderRepository.UpdateAsync(kitchenOrder);
                }
            }

            entity.SetLocation(input.LocationId);
            CheckErrors(await _journalManager.UpdateAsync(@entity, null));

            await CurrentUnitOfWork.SaveChangesAsync();

            if (entity.JournalType == JournalType.ItemIssueVendorCredit && entity.ItemIssueVendorCreditId.HasValue)
            {
                await SyncItemIssueVendorCredit(entity.ItemIssueVendorCreditId.Value);
                var itemIds = await _itemIssueVendorCreditItemRepository.GetAll()
                                    .Where(s => s.ItemIssueVendorCreditId == entity.ItemIssueVendorCreditId.Value)
                                    .AsNoTracking()
                                    .GroupBy(s => s.ItemId)
                                    .Select(s => s.Key)
                                    .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit && entity.ItemReceiptCustomerCreditId.HasValue)
            {
                await SyncItemReceiptCustomerCredit(entity.ItemReceiptCustomerCreditId.Value);
                var itemIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptCustomerCreditId == entity.ItemReceiptCustomerCreditId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.ItemReceiptId.HasValue)
            {
                await SyncItemReceipt(entity.ItemReceiptId.Value);
                var itemIds = await _itemReceiptItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptId == entity.ItemReceiptId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
            else if (entity.ItemIssueId.HasValue)
            {
                await SyncItemIssue(entity.ItemIssueId.Value);
                var itemIds = await _itemIssueItemRepository.GetAll()
                                   .Where(s => s.ItemIssueId == entity.ItemIssueId.Value)
                                   .AsNoTracking()
                                   .GroupBy(s => s.ItemId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
                if (itemIds.Any()) await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, entity.Date, itemIds);
            }
        }

        private ReportOutput GetReportTemplateGeneralJournal()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Account",
                        ColumnLength = 200,
                        ColumnTitle = "Account Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 180,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 230,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                     new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 130,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Money,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Decription",
                        ColumnLength = 250,
                        ColumnTitle = "Decription",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Debit",
                        ColumnLength = 130,
                        ColumnTitle = "Debit",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Credit",
                        ColumnLength = 250,
                        ColumnTitle = "Credit",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                },
                Groupby = "",
                HeaderTitle = "GeneralJournal",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Journals_ExportExcel)]
        public async Task<FileDto> ExportExcelTamplate()
        {

            var result = new FileDto();
            var sheetName = "GeneralJournal";

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


                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateGeneralJournal();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }


                result.FileName = $"GeneralJournalTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Journal_Import, AppPermissions.Pages_Tenant_Accounting_Journals_ImportExcel)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
            var @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
            var userId = AbpSession.GetUserId();
            var tenant = await GetCurrentTenantAsync();
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    var locationName = worksheet.Cells[2, 2].Value?.ToString();
                    var journalNo = worksheet.Cells[2, 3].Value?.ToString();
                    var Date = worksheet.Cells[2, 4].Value?.ToString();

                    var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();

                    if (locationId == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound"));
                    }
                    if (tenant.ClassId == null)
                    {
                        throw new UserFriendlyException(L("ClassIsRequired"));
                    }
                    if (tenant.CurrencyId == null)
                    {
                        throw new UserFriendlyException(L("CurrencyIsRequired"));
                    }
                    //insert to journal
                    var transactionNo = string.Empty;
                    var memo = "Begining Stock";
                    decimal totalDebit = 0;
                    decimal totalCredit = 0;
                    var tenantId = AbpSession.GetTenantId();

                    var @entity = Journal.Create(tenantId, userId, journalNo, Convert.ToDateTime(Date), memo,
                                                0, 0, tenant.CurrencyId.Value, tenant.ClassId, memo, locationId.Id);

                    entity.UpdateGeneralJournal(@entity);
                    entity.UpdateStatus(TransactionStatus.Publish);

                    //loop all rows to insert item receipt items and to journal items
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var Description = worksheet.Cells[i, 5].Value?.ToString();
                            var Debit = worksheet.Cells[i, 6].Value;
                            var Credit = worksheet.Cells[i, 7].Value;
                            var accountCode = worksheet.Cells[i, 1].Value?.ToString();
                            var accountId = accounts.Where(s => s.AccountCode == accountCode).FirstOrDefault();
                            if (accountId == null)
                            {
                                throw new UserFriendlyException(L("NoAccountFound"));
                            }

                            var @journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, accountId.Id, Description, Convert.ToDecimal(Debit), Convert.ToDecimal(Credit), PostingKey.None, null);
                            totalDebit += Convert.ToDecimal(Debit);
                            totalCredit += Convert.ToDecimal(Credit);//update total                                           
                            CheckErrors(await _journalItemManager.CreateAsync(@journalItem));

                        }
                    }

                    if (totalCredit != totalDebit)
                    {
                        throw new UserFriendlyException(L("TotalDebitAndTotalCreditCanNotBeDifferent"));
                    }
                    entity.UpdateCreditDebit(totalCredit, totalCredit);
                    CheckErrors(await _journalManager.CreateAsync(@entity));
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            //RemoveFile(input, _appFolders);
        }
    }
}
