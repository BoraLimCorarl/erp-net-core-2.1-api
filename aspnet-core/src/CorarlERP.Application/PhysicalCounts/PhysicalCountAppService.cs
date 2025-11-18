using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Classes;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.PhysicalCounts;
using CorarlERP.PhysicalCounts.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.Items.Dto;
using CorarlERP.ItemIssueTransfers.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Journals;
using CorarlERP.ItemIssues;
using CorarlERP.MultiTenancy;
using CorarlERP.ItemReceiptTransfers.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemIssueTransfers;
using CorarlERP.ItemReceiptTransfers;
using CorarlERP.Inventories;
using CorarlERP.AutoSequences;
using CorarlERP.UserGroups;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.PhysicalCount;
using CorarlERP.BatchNos;
using CorarlERP.Addresses;
using Amazon.EventBridge.Model.Internal.MarshallTransformations;
using Telegram.Bot.Types.InlineQueryResults;
using Abp.Domain.Entities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.FileStorages;
using OfficeOpenXml;
using CorarlERP.Reports;
using Abp.Domain.Uow;
using CorarlERP.AccountCycles;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Features;
using CorarlERP.Invoices;
using CorarlERP.Invoices.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.Lots;
using Abp.AspNetZeroCore.Net;
using EvoPdf.HtmlToPdfClient;
using System.IO;
using System.DirectoryServices.Protocols;
using CorarlERP.Formats.Dto;
using static CorarlERP.Authorization.Roles.StaticRoleNames;
using CorarlERP.Formats;
using CorarlERP.InventoryCalculationItems;
using System.Globalization;
using Abp.Dependency;
using CorarlERP.Url;
using CorarlERP.Currencies.Dto;
using CorarlERP.ItemIssueOthers.Dto;
using CorarlERP.InventoryTransactionItems;
using Amazon.Runtime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using CorarlERP.PurchaseOrders.Dto;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using CorarlERP.FileUploads;

namespace CorarlERP.PhysicalCounts5
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount)]
    public class PhysicalCountAppService : ReportBaseClass, IPhysicalCountAppService
    {

        private readonly IPhysicalCountManager _physicalCountManager;
        private readonly ICorarlRepository<PhysicalCount.PhysicalCount, Guid> _physicalCountRepository;
        private readonly IPhysicalCountItemManager _physicalCountItemManager;
        private readonly ICorarlRepository<PhysicalCountItem, Guid> _physicalCountItemRepository;
        private readonly ICorarlRepository<PhysicalCountItemFilter, Guid> _physicalCountItemFilterRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;

        private readonly IJournalManager _journalManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IItemIssueManager _itemIssueManager;

        private readonly ICorarlRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly ICorarlRepository<ItemIssueItem, Guid> _itemIssueItemRepository;

        private readonly IRepository<Tenant, int> _tenantRepository;

        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IItemReceiptManager _itemReceiptManager;

        private readonly ICorarlRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly ICorarlRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IInventoryManager _inventoryManager;

        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;

        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Format, long> _formatRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly IRepository<PhysicalCountSetting, Guid> _physicalCountSettingRepository;
        private readonly string _baseUrl;

        private readonly IFileUploadManager _fileUploadManager;

        public PhysicalCountAppService(
            IFileUploadManager fileUploadManager,
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IRepository<PhysicalCountSetting, Guid> physicalCountSettingRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IRepository<Format, long> formatRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IFileStorageManager fileStorageManager,
            IAutoSequenceManager autoSequenceManger,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Tenant, int> tenantRepository,
            IPhysicalCountManager PhysicalCountManager,
            IPhysicalCountItemManager PhysicalCountItemManager,
            ICurrencyManager currencyManager,
            ICorarlRepository<PhysicalCount.PhysicalCount, Guid> PhysicalCountRepository,
            ICorarlRepository<PhysicalCountItem, Guid> PhysicalCountItemRepository,
            ICorarlRepository<PhysicalCountItemFilter, Guid> physicalCountItemFilterRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Currency, long> currencyRepository,
            IRepository<Location, long> locationRepository,
            IRepository<Lot, long> lotRepository,
            IRepository<Class, long> classRepository,
            ICorarlRepository<Journal, Guid> journalRepository,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            ItemIssueManager itemIssueManager,
            ItemIssueItemManager itemIssueItemManager,
            ICorarlRepository<ItemIssue, Guid> itemIssueRepository,
            ICorarlRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            ItemReceiptManager itemReceiptManager,
            ItemReceiptItemManager itemReceiptItemManager,
            ICorarlRepository<ItemReceipt, Guid> itemReceiptRepository,
            ICorarlRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IInventoryManager inventoryManager,
            IRepository<Lock, long> lockRepository,
            IRepository<AccountCycle, long> accountCyclesRepository,
            AppFolders appFolders,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository) :
            base(accountCyclesRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _lockRepository = lockRepository;
            _tenantRepository = tenantRepository;
            _physicalCountManager = PhysicalCountManager;
            _physicalCountRepository = PhysicalCountRepository;
            _physicalCountItemManager = PhysicalCountItemManager;
            _physicalCountItemRepository = PhysicalCountItemRepository;
            _itemRepository = itemRepository;
            _currencyRepository = currencyRepository;
            _locationRepository = locationRepository;
            _lotRepository = lotRepository;
            _classRepository = classRepository;

            _journalManager = journalManager;
            //_journalManager.SetJournalType(JournalType.ItemIssuePhysicalCount);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;

            _itemIssueItemManager = itemIssueItemManager;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueManager = itemIssueManager;
            _itemIssueRepository = itemIssueRepository;

            _itemReceiptItemManager = itemReceiptItemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptManager = itemReceiptManager;
            _itemReceiptRepository = itemReceiptRepository;
            _inventoryManager = inventoryManager;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _physicalCountItemFilterRepository = physicalCountItemFilterRepository;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _formatRepository = formatRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _physicalCountSettingRepository = physicalCountSettingRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _fileUploadManager = fileUploadManager;

            _baseUrl = IocManager.Instance.Resolve<IWebUrlService>().GetServerRootAddress().EnsureEndsWith('/');
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreatePhysicalCountInput input)
        {
            return await CreateHelper(input);
        }

        private void CalculateTotal(CreatePhysicalCountInput input)
        {
            foreach (var i in input.PhysicalCountItems)
            {
                i.DiffQty = i.QtyOnHand - i.CountQty;
                i.Total = Math.Abs(i.DiffQty) * i.UnitCost;
            }
        }
        private void ValidateInput(CreatePhysicalCountInput input)
        {
            if (input.PhysicalCountNo.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("PhysicalCountNo")));
        }


        private async Task<NullableIdDto<Guid>> CreateHelper(CreatePhysicalCountInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.PhysicalCount)
                                            .Where(t => t.IsLock == true && t.LockDate.Value.Date >= input.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PhysicalCount);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.PhysicalCountNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            ValidateInput(input);
            CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = PhysicalCount.PhysicalCount.Create(
                    tenantId, userId, input.LocationId,
                    input.ClassId, input.PhysicalCountNo,
                    input.PhysicalCountDate, input.Reference, input.Status, input.Memo);

            CheckErrors(await _physicalCountManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            if (!input.ItemIds.IsNullOrEmpty())
            {
                var itemFilters = input.ItemIds.Select(i => PhysicalCountItemFilter.Create(tenantId, userId, entity.Id, i)).ToList();
                await _physicalCountItemFilterRepository.BulkInsertAsync(itemFilters);
            }

            #region physicalCountItem
            var items = input.PhysicalCountItems.Select(i => PhysicalCountItem.Create(tenantId, userId, entity.Id, i.No, i.ItemId, i.LotId, i.BatchNoId, i.QtyOnHand, i.CountQty, i.DiffQty, i.UnitCost, i.Description)).ToList();
            await _physicalCountItemRepository.BulkInsertAsync(items);
            #endregion

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Create)]
        public async Task<FileDto> CreateAndPrint(CreatePhysicalCountInput input)
        {
            var saveInvoice = await CreateHelper(input);
            var result = new EntityDto<Guid>() { Id = saveInvoice.Id.Value };
            var print = await PrintHelper(result);
            return print;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var @entity = await _physicalCountManager.GetAsync(input.Id, true);

            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));
          
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.PhysicalCount)
                                            .Where(t => t.IsLock == true && t.LockDate.Value.Date >= entity.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PhysicalCount);

            if (entity.PhysicalCountNo == auto.LastAutoSequenceNumber)
            {
                var pro = await _physicalCountRepository.GetAll().Where(t => t.Id != entity.Id)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (pro != null)
                {
                    auto.UpdateLastAutoSequenceNumber(pro.PhysicalCountNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            var scheduleItems = new List<Guid>();

            var issueItemIds = await DeleteItemIssue(input, entity);
            var receiptItemIds = await DeleteItemReceipt(input, entity);

            if (!issueItemIds.IsNullOrEmpty()) scheduleItems.AddRange(issueItemIds);
            if (!receiptItemIds.IsNullOrEmpty()) scheduleItems.AddRange(receiptItemIds);


            var physicalCountItems = await _physicalCountItemRepository.GetAll().AsNoTracking().Where(u => u.PhysicalCountId == entity.Id).ToListAsync();
            await _physicalCountItemRepository.BulkDeleteAsync(physicalCountItems);

            var items = await _physicalCountItemFilterRepository.GetAll().Where(s => s.PhysicalCountId == entity.Id).ToListAsync();
            if (items.Any()) await _physicalCountItemFilterRepository.BulkDeleteAsync(items);

            CheckErrors(await _physicalCountManager.RemoveAsync(@entity));

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }


            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation) && scheduleItems.Any())
            {
                scheduleItems = scheduleItems.Distinct().ToList();
                await CurrentUnitOfWork.SaveChangesAsync();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, entity.PhysicalCountDate, scheduleItems);
            }
        }


        private async Task<List<Guid>> DeleteItemIssue(CarlEntityDto input, PhysicalCount.PhysicalCount entity)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                            .Where(t => t.IsLock == true && t.LockDate.Value.Date >= entity.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var journal = await _journalRepository.GetAll()
                                .Include(s => s.ItemIssue)
                                .AsNoTracking()
                                .Where(s => s.ItemIssueId.HasValue)
                                .FirstOrDefaultAsync(s => s.ItemIssue.PhysicalCountId == input.Id);

            if (journal == null) return null;
           
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var pro = await _journalRepository.GetAll()
                                .AsNoTracking()
                                .Where(s => s.ItemIssueId.HasValue)
                                .Where(t => t.Id != journal.Id)
                                .OrderByDescending(t => t.CreationTime)
                                .FirstOrDefaultAsync();
                if (pro != null)
                {
                    auto.UpdateLastAutoSequenceNumber(pro.JournalNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            var itemIssue = journal.ItemIssue;

            var journalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id).ToListAsync();
            if (journalItems.Any()) await _journalItemRepository.BulkDeleteAsync(journalItems);

            await _journalRepository.BulkDeleteAsync(journal);

            var batchNos = await _itemIssueItemBatchNoRepository.GetAll().AsNoTracking().Where(s => s.ItemIssueItem.ItemIssueId == itemIssue.Id).ToListAsync();
            if (batchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(batchNos);

            var itemIssueItems = await _itemIssueItemRepository.GetAll().AsNoTracking().Where(s => s.ItemIssueId == itemIssue.Id).ToListAsync();
            if (itemIssueItems.Any()) await _itemIssueItemRepository.BulkDeleteAsync(itemIssueItems);

            await _itemIssueRepository.BulkDeleteAsync(itemIssue);

            await CurrentUnitOfWork.SaveChangesAsync();
            await SyncItemIssue(itemIssue.Id);

            return itemIssueItems.Select(s => s.ItemId).Distinct().ToList();
        }

        private async Task<List<Guid>> DeleteItemReceipt(CarlEntityDto input, PhysicalCount.PhysicalCount entity)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                                            .Where(t => t.IsLock == true && t.LockDate.Value.Date >= entity.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var journal = await _journalRepository.GetAll()
                                .Include(s => s.ItemReceipt)
                                .AsNoTracking()
                                .Where(s => s.ItemReceiptId.HasValue)
                                .FirstOrDefaultAsync(s => s.ItemReceipt.PhysicalCountId == input.Id);

            if (journal == null) return null;

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var pro = await _journalRepository.GetAll()
                                .AsNoTracking()
                                .Where(s => s.ItemReceiptId.HasValue)
                                .Where(t => t.Id != journal.Id)
                                .OrderByDescending(t => t.CreationTime)
                                .FirstOrDefaultAsync();
                if (pro != null)
                {
                    auto.UpdateLastAutoSequenceNumber(pro.JournalNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            var itemReceipt = journal.ItemReceipt;

            var journalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id).ToListAsync();
            if (journalItems.Any()) await _journalItemRepository.BulkDeleteAsync(journalItems);

            await _journalRepository.BulkDeleteAsync(journal);

            var batchNos = await _itemReceiptItemBatchNoRepository.GetAll().AsNoTracking().Where(s => s.ItemReceiptItem.ItemReceiptId == itemReceipt.Id).ToListAsync();
            if (batchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(batchNos);

            var itemReceiptItems = await _itemReceiptItemRepository.GetAll().AsNoTracking().Where(s => s.ItemReceiptId == itemReceipt.Id).ToListAsync();
            if (itemReceiptItems.Any()) await _itemReceiptItemRepository.BulkDeleteAsync(itemReceiptItems);

            await _itemReceiptRepository.BulkDeleteAsync(itemReceipt);

            await CurrentUnitOfWork.SaveChangesAsync();
            await SyncItemReceipt(itemReceipt.Id);

            return itemReceiptItems.Select(s => s.ItemId).Distinct().ToList();
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetDetail)]
        public async Task<PhysicalCountDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            return await GetDetailHelper(input);
        }

        private async Task<PhysicalCountDetailOutput> GetDetailHelper(EntityDto<Guid> input)
        {
            var query = from p in _physicalCountRepository.GetAll().AsNoTracking()
                                  .Where(s => s.Id == input.Id)
                        join i in _itemIssueRepository.GetAll().AsNoTracking()
                        on p.Id equals i.PhysicalCountId
                        into iss
                        from ii in iss.DefaultIfEmpty()
                        join r in _itemReceiptRepository.GetAll().AsNoTracking()
                        on p.Id equals r.PhysicalCountId
                        into rec
                        from ri in rec.DefaultIfEmpty()
                        select new PhysicalCountDetailOutput
                        {
                            Id = p.Id,
                            PhysicalCountDate = p.PhysicalCountDate,
                            Location = new LocationSummaryOutput
                            {
                                LocationName = p.Location.LocationName,
                                Id = p.LocationId
                            },
                            LocationId = p.LocationId,
                            Class = new ClassSummaryOutput
                            {
                                Id = p.ClassId,
                                ClassName = p.Class.ClassName
                            },
                            ClassId = p.ClassId,
                            Memo = p.Memo,
                            Status = p.Status,
                            PhysicalCountNo = p.PhysicalCountNo,
                            Reference = p.Reference,
                            ItemIssueId = ii == null ? (Guid?)null : ii.Id,
                            ItemReceiptId = ri == null ? (Guid?)null : ri.Id
                        };

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemQuery = from pi in _physicalCountItemRepository.GetAll()
                                      .AsNoTracking()
                                      .Where(u => u.PhysicalCountId == input.Id)
                            join ii in _itemIssueItemRepository.GetAll()
                                       .AsNoTracking()
                            on pi.Id equals ii.PhysicalCountItemId
                            into issues
                            from issue in issues.DefaultIfEmpty()
                            join ri in _itemReceiptItemRepository.GetAll()
                                       .AsNoTracking()
                            on pi.Id equals ri.PhysicalCountItemId
                            into receipts
                            from receipt in receipts.DefaultIfEmpty()
                            orderby pi.No
                            select new PhysicalItemDetailDto
                            {
                                Id = pi.Id,
                                No = pi.No,
                                ItemId = pi.ItemId,
                                ItemName = pi.Item.ItemName,
                                ItemCode = pi.Item.ItemCode,
                                TrackSerial = pi.Item.TrackSerial,
                                LotId = pi.LotId,
                                LotName = pi.LotId.HasValue ? pi.Lot.LotName : "",
                                BatchNoId = pi.BatchNoId,
                                BatchNo = pi.BatchNoId.HasValue ? pi.BatchNo.Code : "",
                                QtyOnHand = pi.QtyOnHand,
                                CountQty = pi.CountQty,
                                DiffQty = pi.DiffQty,
                                UnitCost = pi.UnitCost,
                                Description = pi.Description,
                                ItemIssueItemId = issue == null ? (Guid?)null : issue.Id,
                                ItemReceiptItemId = receipt == null ? (Guid?)null : receipt.Id,
                            };


            var physicalCountItem = await itemQuery.ToListAsync();

            var itemFilter = await _physicalCountItemFilterRepository.GetAll()
                                .Where(s => s.PhysicalCountId == input.Id)
                                .Select(s => new ItemSummaryDto
                                {
                                    Id = s.ItemId,
                                    ItemCode = s.Item.ItemCode,
                                    ItemName = s.Item.ItemName
                                }).ToListAsync();

            result.Items = physicalCountItem;
            result.ItemFilter = itemFilter;
            return result;
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetList)]
        public async Task<PagedResultDto<PhysicalCountGetListOutput>> GetList(GetPhysicalCountListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = from p in _physicalCountRepository.GetAll()
                                    .AsNoTracking()
                                    .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId))
                                    .WhereIf(input.FromDate != null && input.ToDate != null, u => input.FromDate.Date <= u.PhysicalCountDate.Date && u.PhysicalCountDate.Date <= input.ToDate.Date)
                                    .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                                    .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        p => p.PhysicalCountNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        p.Reference.ToLower().Contains(input.Filter.ToLower()))
                                    .Select(p => new PhysicalCountGetListOutput
                                    {
                                        Id = p.Id,
                                        StatusCode = p.Status,
                                        PhysicalCountDate = p.PhysicalCountDate,
                                        PhysicalCountNo = p.PhysicalCountNo,
                                        LocationId = p.LocationId,
                                        Location = new LocationSummaryOutput()
                                        {
                                            Id = p.LocationId,
                                            LocationName = p.Location.LocationName
                                        },
                                        ClassId = p.ClassId,
                                        Class = new ClassSummaryOutput()
                                        {
                                            Id = p.ClassId,
                                            ClassName = p.Class.ClassName
                                        },
                                    })
                         join pi in _physicalCountItemRepository.GetAll().AsNoTracking()
                                    .Select(s => new { s.PhysicalCountId, s.ItemId })
                         on p.Id equals pi.PhysicalCountId
                         into items
                         where input.Items.IsNullOrEmpty() || items.Any(s => input.Items.Contains(s.ItemId))
                        
                         select p;

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<PhysicalCountGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Update, AppPermissions.Pages_Tenant_Inventory_PhysicalCount_FillIn)]
        public async Task<PhysicalCountDetailOutput> Update(UpdatePhysicalCountInput input)
        {
            await UpdateHelper(input);

            return await GetDetailHelper(new EntityDto<Guid> { Id = input.Id });
        }

        private async Task UpdateHelper(UpdatePhysicalCountInput input, bool close = false)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t =>  t.LockKey == TransactionLockType.PhysicalCount)
                                            .Where(t => t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
            }


            var @entity = await _physicalCountManager.GetAsync(input.Id, true); //this is vendor

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (!close && entity.Status == TransactionStatus.Close) throw new UserFriendlyException(L("TransactionAlreadyClosed"));

            ValidateInput(input);
            CalculateTotal(input);

            entity.Update(userId, input.LocationId, input.ClassId,
                input.Status, input.PhysicalCountNo,
                input.PhysicalCountDate,
                input.Reference, input.Memo
            );

            #region update physicalItem           
            var physicalItems = await _physicalCountItemRepository.GetAll().Where(u => u.PhysicalCountId == entity.Id).ToListAsync();

            var addItems = input.PhysicalCountItems.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
            var updateItems = input.PhysicalCountItems.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();
            var deleteItems = physicalItems.Where(s => !updateItems.Any(r => r.Id == s.Id)).ToList();

            if (addItems.Any())
            {
                var addPhysicalItems = addItems.Select(p => PhysicalCountItem.Create(tenantId, userId, entity.Id, p.No, p.ItemId, p.LotId, p.BatchNoId, p.QtyOnHand, p.CountQty, p.DiffQty, p.UnitCost, p.Description)).ToList();
                await _physicalCountItemRepository.BulkInsertAsync(addPhysicalItems);
            }

            if (updateItems.Any())
            {
                var updatePhysicalItems = new List<PhysicalCountItem>();
                foreach (var p in updateItems)
                {
                    var physicalItem = physicalItems.FirstOrDefault(u => u.Id == p.Id);
                    if (physicalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                    //here is in only same TransferOrder so no need to update TransferOrder
                    physicalItem.Update(userId, p.No, p.ItemId, p.LotId, p.BatchNoId, p.QtyOnHand, p.CountQty, p.DiffQty, p.UnitCost, p.Description);
                    updatePhysicalItems.Add(physicalItem);
                }

                await _physicalCountItemRepository.BulkUpdateAsync(updatePhysicalItems);
            }

            if (deleteItems.Any()) await _physicalCountItemRepository.BulkDeleteAsync(deleteItems);

            #endregion

            var itemFilters = await _physicalCountItemFilterRepository.GetAll().Where(s => s.PhysicalCountId == entity.Id).ToListAsync();

            var addItemFilters = new List<Guid>();
            var deleteItemFilters = new List<PhysicalCountItemFilter>();

            if (!input.ItemIds.IsNullOrEmpty())
            {
                addItemFilters = input.ItemIds.Where(s => !itemFilters.Any(r => r.ItemId == s)).ToList();
                deleteItemFilters = itemFilters.Where(s => !input.ItemIds.Contains(s.ItemId)).ToList();
            }

            if (addItemFilters.Any())
            {
                var addFilters = addItemFilters.Select(i => PhysicalCountItemFilter.Create(tenantId, userId, entity.Id, i)).ToList();
                await _physicalCountItemFilterRepository.BulkInsertAsync(addFilters);
            }

            if (deleteItemFilters.Any()) await _physicalCountItemFilterRepository.BulkDeleteAsync(deleteItemFilters);


            if (close) entity.UpdateStatusToClose();

            await _physicalCountRepository.BulkUpdateAsync(entity);

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

   
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_UpdateStatusToPublish)]
        public async Task Open(CarlEntityDto input)
        {
            var @entity = await _physicalCountManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.PhysicalCount)
                                            .Where(t => t.IsLock == true && t.LockDate.Value.Date >= entity.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
            }

            var scheduleItems = new List<Guid>();

            var issueItemIds = await DeleteItemIssue(input, entity);
            var receiptItemIds = await DeleteItemReceipt(input, entity);

            if (!issueItemIds.IsNullOrEmpty()) scheduleItems.AddRange(issueItemIds);
            if (!receiptItemIds.IsNullOrEmpty()) scheduleItems.AddRange(receiptItemIds);

            entity.UpdateStatusToPublish();
            CheckErrors(await _physicalCountManager.UpdateAsync(entity));

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation) && scheduleItems.Any())
            {
                scheduleItems = scheduleItems.Distinct().ToList();
                await CurrentUnitOfWork.SaveChangesAsync();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, entity.PhysicalCountDate, scheduleItems);
            }
        }

       
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_SyncInventory)]
        public async Task<List<PhysicalItemDetailDto>> CalculateStockBalance(CalculatePhysicalCountInput input)
        {
            return await CalculateStockBalanceHelper(input);
        }

        private async Task<List<PhysicalItemDetailDto>> CalculateStockBalanceHelper(CalculatePhysicalCountInput input)
        {

            var stockBalanceList = await _inventoryManager.GetItemsBalance(input.ItemIds, input.Date, new List<long> { input.LocationId }, input.ExceptIds);

            var itemQuery = from i in _itemRepository.GetAll().AsNoTracking()
                            join b in stockBalanceList
                            on i.Id equals b.Id
                            orderby i.ItemCode, b.LotName
                            select new PhysicalItemDetailDto
                            {
                                ItemId = i.Id,
                                ItemCode = i.ItemCode,
                                ItemName = i.ItemName,
                                TrackSerial = i.TrackSerial,
                                LotId = b.LotId,
                                LotName = b.LotName,
                                QtyOnHand = b.QtyOnHand
                            };

            var itemList = await itemQuery.ToListAsync();

            var batchBalanceList = await _inventoryManager.GetItemBatchNoBalance(input.Date, input.LocationId, input.ItemIds, input.ExceptIds);
            var batchNoDic = await _batchNoRepository.GetAll().AsNoTracking().ToDictionaryAsync(k => k.Id, v => v.Code);

            var items = new List<PhysicalItemDetailDto>();
            foreach (var i in itemList)
            {
                var batchNos = batchBalanceList.Where(s => s.ItemId == i.ItemId && s.LotId == i.LotId).ToList();

                if (batchNos.Any())
                {
                    foreach (var b in batchNos)
                    {
                        var item = new PhysicalItemDetailDto
                        {
                            ItemId = i.ItemId,
                            ItemCode = i.ItemCode,
                            ItemName = i.ItemName,
                            TrackSerial = i.TrackSerial,
                            LotId = i.LotId,
                            LotName = i.LotName,
                            BatchNoId = b.BatchNoId,
                            BatchNo = batchNoDic.ContainsKey(b.BatchNoId) ? batchNoDic[b.BatchNoId] : "",
                            QtyOnHand = b.Qty
                        };

                        items.Add(item);
                    }
                }
                else
                {
                    items.Add(i);
                }
            }


            return items;
        }

        private async Task ValidateStock(UpdatePhysicalCountInput input)
        {
            var issueItems = input.PhysicalCountItems
                            .Where(s => s.DiffQty > 0)
                            .Select(s => new { Key = $"{s.ItemId}-{s.LotId}-{s.BatchNoId}", s.ItemId, s.QtyOnHand, s.BatchNoId });

            if (!issueItems.Any()) return;

            var itemIds = issueItems.Select(s => s.ItemId).Distinct().ToList();

            var calculateInput = new CalculatePhysicalCountInput
            {
                Date = input.PhysicalCountDate,
                LocationId = input.LocationId,
                ItemIds = itemIds,
                ExceptIds = new List<Guid>()
            };

            if (input.ItemIssueId.HasValue && input.ItemIssueId != Guid.Empty) calculateInput.ExceptIds.Add(input.ItemIssueId.Value);
            if (input.ItemReceiptId.HasValue && input.ItemReceiptId != Guid.Empty) calculateInput.ExceptIds.Add(input.ItemReceiptId.Value);

            var stockBalanceDic = (await CalculateStockBalanceHelper(calculateInput))
                                 .ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => new { v.QtyOnHand, v.ItemName, v.BatchNo });

            var find = issueItems.Where(f => !stockBalanceDic.ContainsKey(f.Key)).FirstOrDefault();
            if (find != null)
            {
                var item = await _itemRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == find.ItemId);
                var batchNo = "";

                if (find.BatchNoId.HasValue && find.BatchNoId != Guid.Empty)
                {
                    batchNo = await _batchNoRepository.GetAll().AsNoTracking().Where(s => s.Id == find.BatchNoId).Select(s => s.Code).FirstOrDefaultAsync();
                }

                throw new UserFriendlyException(L("ItemOutOfStock", item?.ItemName + (batchNo.IsNullOrEmpty() ? "" : $", {batchNo}")));
            }

            var find2 = issueItems.Where(f => stockBalanceDic.ContainsKey(f.Key) && f.QtyOnHand > stockBalanceDic[f.Key].QtyOnHand).FirstOrDefault();
            if (find2 != null)
            {
                var item = stockBalanceDic[find2.Key];

                throw new UserFriendlyException(L("CannotIssueForThisBiggerStockItem",
                  item.ItemName + (item.BatchNo.IsNullOrEmpty() ? "" : $", {item.BatchNo}"),
                  String.Format("{0:n0}", item.QtyOnHand),
                  String.Format("{0:n0}", find2.QtyOnHand)));
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Update, AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Close, RequireAllPermissions = true )]
        public async Task<PhysicalCountDetailOutput> UpdateAndClose(UpdatePhysicalCountInput input)
        {
            var tenant = await GetCurrentTenantAsync();
            if (!tenant.CurrencyId.HasValue) throw new UserFriendlyException(L("IsRequired", L("Currency")));
            if (!tenant.ItemIssuePhysicalCountId.HasValue) throw new UserFriendlyException(L("IsRequired", L("AdjustmentAccount")));

            await ValidateStock(input);

            await UpdateHelper(input, true);

            input.CurrencyId = tenant.CurrencyId.Value;
            input.AdjustmentAccountId = tenant.ItemIssuePhysicalCountId.Value;

            await AdjustStockIssue(input);
            await AdjustStockReceipt(input);

            return await GetDetailHelper(new EntityDto<Guid> { Id = input.Id});
        }

        private async Task AdjustStockIssue(UpdatePhysicalCountInput input)
        {

            var issueItems = input.PhysicalCountItems.Where(s => s.DiffQty > 0).ToList();
            var itemIssue = await _itemIssueRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PhysicalCountId == input.Id);

            if (input.IsConfirm == false && (issueItems.Any() || itemIssue != null))
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                            .Where(t => t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
            }

            Journal itemIssueJournal = null;
            List<JournalItem> itemIssueJournalItems = new List<JournalItem>();
            List<ItemIssueItem> itemIssueItems = new List<ItemIssueItem>();
            List<ItemIssueItemBatchNo> itemIssueItemBatchNos = new List<ItemIssueItemBatchNo>();

            if (itemIssue != null)
            {
                itemIssueJournal = await _journalRepository.GetAll().AsNoTracking().Where(S => S.ItemIssueId == itemIssue.Id).FirstOrDefaultAsync();
                itemIssueJournalItems = await _journalItemRepository.GetAll().AsNoTracking().Where(s => s.JournalId == itemIssueJournal.Id).ToListAsync();
                itemIssueItems = await _itemIssueItemRepository.GetAll().AsNoTracking().Where(s => s.ItemIssueId == itemIssue.Id).ToListAsync();
                itemIssueItemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().AsNoTracking().Where(s => s.ItemIssueItem.ItemIssueId == itemIssue.Id).ToListAsync();
            }

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            if (!issueItems.Any())
            {
                if(itemIssue != null)
                {
                    var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
                    if (itemIssueJournal.JournalNo == auto.LastAutoSequenceNumber)
                    {
                        var pro = await _journalRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.ItemIssueId.HasValue)
                                        .Where(t => t.Id != itemIssueJournal.Id)
                                        .OrderByDescending(t => t.CreationTime)
                                        .FirstOrDefaultAsync();
                        if (pro != null)
                        {
                            auto.UpdateLastAutoSequenceNumber(pro.JournalNo);
                        }
                        else
                        {
                            auto.UpdateLastAutoSequenceNumber(null);
                        }
                        CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                    }

                    if (itemIssueJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(itemIssueJournalItems);
                    await _journalRepository.BulkDeleteAsync(itemIssueJournal);

                    if (itemIssueItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(itemIssueItemBatchNos);
                    if (itemIssueItems.Any()) await _itemIssueItemRepository.BulkDeleteAsync(itemIssueItems);
                    await _itemIssueRepository.BulkDeleteAsync(itemIssue);
                }
            }
            else
            {

                var itemDic = await _itemRepository.GetAll().AsNoTracking()
                                    .Where(s => issueItems.Any(r => r.ItemId == s.Id))
                                    .ToDictionaryAsync(k => k.Id, v => v.InventoryAccountId.Value);

                var total = issueItems.Sum(t => t.Total);
                var address = new CAddress("", "KH", "", "", "");

                if (itemIssue == null)
                {

                    var journalNo = "";
                    var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

                    if (auto.CustomFormat == true)
                    {
                        var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                                        auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                        journalNo = newAuto;
                        auto.UpdateLastAutoSequenceNumber(newAuto);
                        CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                    }


                    itemIssue = ItemIssue.Create(tenantId, userId, ReceiveFrom.None, null, true, address, address, total, null, false);
                    itemIssue.UpdatePhysicalCountId(input.Id);
                    itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssuePhysicalCount);

                    //insert to journal
                    var @entity = Journal.Create(tenantId, userId, journalNo, input.PhysicalCountDate,
                                            input.Memo, total, total, input.CurrencyId,
                                            input.ClassId, input.Reference, input.LocationId);
                    entity.UpdateStatus(input.Status);
                    #region journal transaction type 
                    var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssuePhysicalCount);
                    entity.SetJournalTransactionTypeId(transactionTypeId);
                    #endregion
                    entity.UpdatePhysicalCountItemIssue(itemIssue.Id);

                    var addJournalItems = new List<JournalItem>();

                    //insert clearance journal item into credit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity.Id, input.AdjustmentAccountId, input.Memo, total, 0, PostingKey.COGS, null);
                    addJournalItems.Add(clearanceJournalItem);


                    var addIssueBatchNos = new List<ItemIssueItemBatchNo>();
                    var addIssueItems = new List<ItemIssueItem>();

                    foreach (var i in issueItems)
                    {
                        var item = ItemIssueItem.Create(tenantId, userId, itemIssue.Id, i.ItemId, i.Description, i.DiffQty, i.UnitCost, 0, i.Total);
                        item.SetPhysicalCountItem(i.Id);
                        item.UpdateLot(i.LotId);
                        addIssueItems.Add(item);

                        var journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity.Id, itemDic[i.ItemId], input.Memo, 0, i.Total, PostingKey.Inventory, item.Id);
                        addJournalItems.Add(journalItem);

                        if (i.BatchNoId.HasValue && i.BatchNoId != Guid.Empty)
                        {
                            var batch = ItemIssueItemBatchNo.Create(tenantId, userId, item.Id, i.BatchNoId.Value, i.DiffQty);
                            addIssueBatchNos.Add(batch);
                        }
                    }

                    await _itemIssueRepository.BulkInsertAsync(itemIssue);
                    await _itemIssueItemRepository.BulkInsertAsync(addIssueItems);
                    if (addIssueItems.Any()) await _itemIssueItemBatchNoRepository.BulkInsertAsync(addIssueBatchNos);

                    await _journalRepository.BulkInsertAsync(@entity);
                    await _journalItemRepository.BulkInsertAsync(addJournalItems);
                }
                else
                {
                    itemIssue.Update(userId, itemIssue.ReceiveFrom, null, true, address, address, total, null, false);
                    await _itemIssueRepository.UpdateAsync(itemIssue);

                    itemIssueJournal.Update(userId, total, total, input.PhysicalCountDate, input.ClassId, input.Reference, input.LocationId);
                  
                    await _journalRepository.UpdateAsync(itemIssueJournal);

                    var updateJournalItems = new List<JournalItem>();

                    var clearanceJournalItem = itemIssueJournalItems.FirstOrDefault(s => s.Key == PostingKey.COGS && s.Identifier == null);
                    if (clearanceJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));
                    clearanceJournalItem.UpdateJournalItem(userId, input.AdjustmentAccountId, input.Memo, total, 0);
                    updateJournalItems.Add(clearanceJournalItem);

                    var addItems = issueItems.Where(s => !s.ItemIssueItemId.HasValue || s.ItemIssueItemId == Guid.Empty).ToList();
                    var updateItems = issueItems.Where(s => s.ItemIssueItemId.HasValue && s.ItemIssueItemId != Guid.Empty).ToList();

                    var addJournalItems = new List<JournalItem>();
                    var addIssueBatchNos = new List<ItemIssueItemBatchNo>();
                    var addIssueItems = new List<ItemIssueItem>();
                    foreach (var i in addItems)
                    {
                        var item = ItemIssueItem.Create(tenantId, userId, itemIssue.Id, i.ItemId, i.Description, i.DiffQty, i.UnitCost, 0, i.Total);
                        item.SetPhysicalCountItem(i.Id);
                        item.UpdateLot(i.LotId);
                        addIssueItems.Add(item);

                        var journalItem = JournalItem.CreateJournalItem(tenantId, userId, itemIssueJournal.Id, itemDic[i.ItemId], input.Memo, 0, i.Total, PostingKey.Inventory, item.Id);
                        addJournalItems.Add(journalItem);

                        if (i.BatchNoId.HasValue && i.BatchNoId != Guid.Empty)
                        {
                            var batch = ItemIssueItemBatchNo.Create(tenantId, userId, item.Id, i.BatchNoId.Value, i.DiffQty);
                            addIssueBatchNos.Add(batch);
                        }
                    }


                    var updateIssueItems = new List<ItemIssueItem>();
                    var updateBatchNos = new List<ItemIssueItemBatchNo>();

                    foreach (var i in updateItems)
                    {
                        var updateItem = itemIssueItems.FirstOrDefault(s => s.PhysicalCountItemId == i.Id);
                        if (updateItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                        var updateJournalItem = itemIssueJournalItems.FirstOrDefault(s => s.Identifier == updateItem.Id);
                        if (updateJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                        updateItem.Update(userId, i.ItemId, i.Description, i.DiffQty, i.UnitCost, 0, i.Total);
                        updateItem.UpdateLot(i.LotId);
                        updateIssueItems.Add(updateItem);


                        updateJournalItem.UpdateJournalItem(userId, itemDic[i.ItemId], i.Description, 0, i.Total);
                        updateJournalItems.Add(updateJournalItem);

                        var batch = itemIssueItemBatchNos.FirstOrDefault(s => s.ItemIssueItemId == updateItem.Id);
                        if (i.BatchNoId.HasValue && i.BatchNoId != Guid.Empty)
                        {
                            if (batch == null)
                            {
                                batch = ItemIssueItemBatchNo.Create(tenantId, userId, updateItem.Id, i.BatchNoId.Value, i.DiffQty);
                                addIssueBatchNos.Add(batch);
                            }
                            else
                            {
                                batch.Update(userId, updateItem.Id, i.BatchNoId.Value, i.DiffQty);
                                updateBatchNos.Add(batch);
                            }
                        }
                    }

                    var deleteJournalItems = itemIssueJournalItems.Where(s => !updateJournalItems.Any(r => r.Id == s.Id)).ToList();
                    if (deleteJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(deleteJournalItems);

                    var deleteBatchNos = itemIssueItemBatchNos.Where(s => !updateBatchNos.Any(r => r.Id == s.Id));
                    if (deleteBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteBatchNos);

                    var deleteItemIssueItems = itemIssueItems.Where(s => !updateIssueItems.Any(r => r.Id == s.Id)).ToList();
                    if (deleteItemIssueItems.Any()) await _itemIssueItemRepository.BulkDeleteAsync(deleteItemIssueItems);

                    if (addIssueItems.Any()) await _itemIssueItemRepository.BulkInsertAsync(addIssueItems);
                    if (addIssueBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkInsertAsync(addIssueBatchNos);

                    if (updateIssueItems.Any()) await _itemIssueItemRepository.BulkUpdateAsync(updateIssueItems);
                    if (updateBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkUpdateAsync(updateBatchNos);

                    if (addJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addJournalItems);
                    if (updateJournalItems.Any()) await _journalItemRepository.BulkUpdateAsync(updateJournalItems);

                }
            }

            if (itemIssue != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();

                await SyncItemIssue(itemIssue.Id);

                if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
                {

                    var scheduleItems = issueItems.Select(s => s.ItemId).Distinct().ToList();
                    await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.PhysicalCountDate, scheduleItems);
                }
            }
        }

        private async Task AdjustStockReceipt(UpdatePhysicalCountInput input)
        {
            var receiptItems = input.PhysicalCountItems
                               .Where(s => s.DiffQty < 0)
                               .Select(s =>
                               {
                                   var i = s;
                                   i.DiffQty = Math.Abs(i.DiffQty);
                                   i.Total = Math.Abs(i.DiffQty) * i.UnitCost;
                                   return i;
                               })
                               .ToList();
            var itemReceipt = await _itemReceiptRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PhysicalCountId == input.Id);

            if (input.IsConfirm == false && (receiptItems.Any() || itemReceipt != null))
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                                            .Where(t => t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
            }

            Journal itemReceiptJournal = null;
            List<JournalItem> itemReceiptJournalItems = new List<JournalItem>();
            List<ItemReceiptItem> itemReceiptItems = new List<ItemReceiptItem>();
            List<ItemReceiptItemBatchNo> itemReceiptItemBatchNos = new List<ItemReceiptItemBatchNo>();

            if (itemReceipt != null)
            {
                itemReceiptJournal = await _journalRepository.GetAll().AsNoTracking().Where(S => S.ItemReceiptId == itemReceipt.Id).FirstOrDefaultAsync();
                itemReceiptJournalItems = await _journalItemRepository.GetAll().AsNoTracking().Where(s => s.JournalId == itemReceiptJournal.Id).ToListAsync();
                itemReceiptItems = await _itemReceiptItemRepository.GetAll().AsNoTracking().Where(s => s.ItemReceiptId == itemReceipt.Id).ToListAsync();
                itemReceiptItemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().AsNoTracking().Where(s => s.ItemReceiptItem.ItemReceiptId == itemReceipt.Id).ToListAsync();
            }

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            if (!receiptItems.Any())
            {
                if(itemReceipt != null)
                {
                    var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
                    if (itemReceiptJournal.JournalNo == auto.LastAutoSequenceNumber)
                    {
                        var pro = await _journalRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.ItemReceiptId.HasValue)
                                        .Where(t => t.Id != itemReceiptJournal.Id)
                                        .OrderByDescending(t => t.CreationTime)
                                        .FirstOrDefaultAsync();
                        if (pro != null)
                        {
                            auto.UpdateLastAutoSequenceNumber(pro.JournalNo);
                        }
                        else
                        {
                            auto.UpdateLastAutoSequenceNumber(null);
                        }
                        CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                    }

                    if (itemReceiptJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(itemReceiptJournalItems);
                    await _journalRepository.BulkDeleteAsync(itemReceiptJournal);
                    if (itemReceiptItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(itemReceiptItemBatchNos);
                    if (itemReceiptItems.Any()) await _itemReceiptItemRepository.BulkDeleteAsync(itemReceiptItems);
                    await _itemReceiptRepository.BulkDeleteAsync(itemReceipt);
                }
            }
            else
            {

                var itemDic = await _itemRepository.GetAll().AsNoTracking()
                                    .Where(s => receiptItems.Any(r => r.ItemId == s.Id))
                                    .ToDictionaryAsync(k => k.Id, v => v.InventoryAccountId.Value);

                var total = receiptItems.Sum(t => t.Total);
                var address = new CAddress("", "KH", "", "", "");

                if (itemReceipt == null)
                {

                    var journalNo = "";
                    var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

                    if (auto.CustomFormat == true)
                    {
                        var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                                        auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                        journalNo = newAuto;
                        auto.UpdateLastAutoSequenceNumber(newAuto);
                        CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                    }


                    itemReceipt = ItemReceipt.Create(tenantId, userId, ItemReceipt.ReceiveFromStatus.None, null, true, address, address, total, null);
                    itemReceipt.UpdatePhysicalCountId(input.Id);
                    itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptPhysicalCount);

                    //insert to journal
                    var @entity = Journal.Create(tenantId, userId, journalNo, input.PhysicalCountDate,
                                            input.Memo, total, total, input.CurrencyId,
                                            input.ClassId, input.Reference, input.LocationId);
                    entity.UpdateStatus(input.Status);
                    #region journal transaction type 
                    var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptPhysicalCount);
                    entity.SetJournalTransactionTypeId(transactionTypeId);
                    #endregion
                    entity.UpdatePhysicalCountItemReceipt(itemReceipt.Id);

                    var addJournalItems = new List<JournalItem>();

                    //insert clearance journal item into credit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity.Id, input.AdjustmentAccountId, input.Memo, 0, total, PostingKey.COGS, null);
                    addJournalItems.Add(clearanceJournalItem);


                    var addReceiptBatchNos = new List<ItemReceiptItemBatchNo>();
                    var addReceiptItems = new List<ItemReceiptItem>();

                    foreach (var i in receiptItems)
                    {
                        var item = ItemReceiptItem.Create(tenantId, userId, itemReceipt.Id, i.ItemId, i.Description, i.DiffQty, i.UnitCost, 0, i.Total);
                        item.SetPhysicalCountItem(i.Id);
                        item.UpdateLot(i.LotId);
                        addReceiptItems.Add(item);

                        var journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity.Id, itemDic[i.ItemId], input.Memo, i.Total, 0, PostingKey.Inventory, item.Id);
                        addJournalItems.Add(journalItem);

                        if (i.BatchNoId.HasValue && i.BatchNoId != Guid.Empty)
                        {
                            var batch = ItemReceiptItemBatchNo.Create(tenantId, userId, item.Id, i.BatchNoId.Value, i.DiffQty);
                            addReceiptBatchNos.Add(batch);
                        }
                    }

                    await _itemReceiptRepository.BulkInsertAsync(itemReceipt);
                    await _itemReceiptItemRepository.BulkInsertAsync(addReceiptItems);
                    if (addReceiptItems.Any()) await _itemReceiptItemBatchNoRepository.BulkInsertAsync(addReceiptBatchNos);

                    await _journalRepository.BulkInsertAsync(@entity);
                    await _journalItemRepository.BulkInsertAsync(addJournalItems);
                }
                else
                {
                    itemReceipt.Update(userId, itemReceipt.ReceiveFrom, null, true, address, address, total, null);
                    await _itemReceiptRepository.UpdateAsync(itemReceipt);

                    itemReceiptJournal.Update(userId, total, total, input.PhysicalCountDate, input.ClassId, input.Reference, input.LocationId);
                    await _journalRepository.UpdateAsync(itemReceiptJournal);

                    var updateJournalItems = new List<JournalItem>();

                    var clearanceJournalItem = itemReceiptJournalItems.FirstOrDefault(s => s.Key == PostingKey.COGS && s.Identifier == null);
                    if (clearanceJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));
                    clearanceJournalItem.UpdateJournalItem(userId, input.AdjustmentAccountId, input.Memo, total, 0);
                    updateJournalItems.Add(clearanceJournalItem);

                    var addItems = receiptItems.Where(s => !s.ItemReceiptItemId.HasValue || s.ItemReceiptItemId == Guid.Empty).ToList();
                    var updateItems = receiptItems.Where(s => s.ItemReceiptItemId.HasValue && s.ItemReceiptItemId != Guid.Empty).ToList();

                    var addJournalItems = new List<JournalItem>();
                    var addReceiptBatchNos = new List<ItemReceiptItemBatchNo>();
                    var addReceiptItems = new List<ItemReceiptItem>();
                    foreach (var i in addItems)
                    {
                        var item = ItemReceiptItem.Create(tenantId, userId, itemReceipt.Id, i.ItemId, i.Description, i.DiffQty, i.UnitCost, 0, i.Total);
                        item.SetPhysicalCountItem(i.Id);
                        item.UpdateLot(i.LotId);
                        addReceiptItems.Add(item);

                        var journalItem = JournalItem.CreateJournalItem(tenantId, userId, itemReceiptJournal.Id, itemDic[i.ItemId], input.Memo, i.Total, 0, PostingKey.Inventory, item.Id);
                        addJournalItems.Add(journalItem);

                        if (i.BatchNoId.HasValue && i.BatchNoId != Guid.Empty)
                        {
                            var batch = ItemReceiptItemBatchNo.Create(tenantId, userId, item.Id, i.BatchNoId.Value, i.DiffQty);
                            addReceiptBatchNos.Add(batch);
                        }
                    }


                    var updateReceiptItems = new List<ItemReceiptItem>();
                    var updateBatchNos = new List<ItemReceiptItemBatchNo>();

                    foreach (var i in updateItems)
                    {
                        var updateItem = itemReceiptItems.FirstOrDefault(s => s.PhysicalCountItemId == i.Id);
                        if (updateItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                        var updateJournalItem = itemReceiptJournalItems.FirstOrDefault(s => s.Identifier == updateItem.Id);
                        if (updateJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                        updateItem.Update(userId, i.ItemId, i.Description, i.DiffQty, i.UnitCost, 0, i.Total);
                        updateItem.UpdateLot(i.LotId);
                        updateReceiptItems.Add(updateItem);

                        updateJournalItem.UpdateJournalItem(userId, itemDic[i.ItemId], i.Description, i.Total, 0);
                        updateJournalItems.Add(updateJournalItem);

                        var batch = itemReceiptItemBatchNos.FirstOrDefault(s => s.ItemReceiptItemId == updateItem.Id);
                        if (i.BatchNoId.HasValue && i.BatchNoId != Guid.Empty)
                        {
                            if (batch == null)
                            {
                                batch = ItemReceiptItemBatchNo.Create(tenantId, userId, updateItem.Id, i.BatchNoId.Value, i.DiffQty);
                                addReceiptBatchNos.Add(batch);
                            }
                            else
                            {
                                batch.Update(userId, updateItem.Id, i.BatchNoId.Value, i.DiffQty);
                                updateBatchNos.Add(batch);
                            }
                        }
                    }

                    var deleteJournalItems = itemReceiptJournalItems.Where(s => !updateJournalItems.Any(r => r.Id == s.Id)).ToList();
                    if (deleteJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(deleteJournalItems);

                    var deleteBatchNos = itemReceiptItemBatchNos.Where(s => !updateBatchNos.Any(r => r.Id == s.Id));
                    if (deleteBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteBatchNos);

                    var deleteItemReceiptItems = itemReceiptItems.Where(s => !updateReceiptItems.Any(r => r.Id == s.Id)).ToList();
                    if (deleteItemReceiptItems.Any()) await _itemReceiptItemRepository.BulkDeleteAsync(deleteItemReceiptItems);

                    if (addReceiptItems.Any()) await _itemReceiptItemRepository.BulkInsertAsync(addReceiptItems);
                    if (addReceiptBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkInsertAsync(addReceiptBatchNos);

                    if (updateReceiptItems.Any()) await _itemReceiptItemRepository.BulkUpdateAsync(updateReceiptItems);
                    if (updateBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkUpdateAsync(updateBatchNos);

                    if (addJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addJournalItems);
                    if (updateJournalItems.Any()) await _journalItemRepository.BulkUpdateAsync(updateJournalItems);

                }
            }


            if (itemReceipt != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();

                await SyncItemReceipt(itemReceipt.Id);

                if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
                {
                    var scheduleItems = receiptItems.Select(s => s.ItemId).Distinct().ToList();
                    await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.PhysicalCountDate, scheduleItems);
                }
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Print)]
        public async Task<FileDto> Print(EntityDto<Guid> input)
        {
            return await PrintHelper(input);
        }

        private async Task<FileDto> PrintHelper(EntityDto<Guid> input)
        {

            var tenant = await GetCurrentTenantAsync();
            var user = await GetCurrentUserAsync();
            var detail = await GetDetailHelper(input);
            var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(detail.PhysicalCountDate);
            var formatDate = await _formatRepository.GetAll()
                           .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                           .Select(t => t.Web).FirstOrDefaultAsync();

            var setting = await _physicalCountSettingRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();


            return await Task.Run(async () =>
            {

                var exportHtml = string.Empty;
                var templateHtml = string.Empty;

                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate("PhysicalCountPrintTemplate.html");//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var reportTitle = L("PhysicalCountForm");
                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var excepts = new List<string>
                {
                    "BatchNo",
                    "DiffQty",
                    "UnitCost",
                };

                var viewHeader = new List<CollumnOutput>() {
                    new CollumnOutput{
                        ColumnName = "No",
                        ColumnTitle = "No",
                        ColumnLength = 50,
                        Visible = setting == null || !setting.No,
                    },
                    new CollumnOutput{
                        ColumnName = "ItemCode",
                        ColumnTitle = "ItemCode",
                        ColumnLength = 100,
                        Visible = setting == null || !setting.ItemCode
                    },
                    new CollumnOutput{
                        ColumnName = "ItemName",
                        ColumnTitle = "ItemName",
                        ColumnLength = 350,
                        Visible = true
                    },
                    new CollumnOutput {
                        ColumnName = "LotName",
                        ColumnTitle = "LotName",
                        ColumnLength = 120,
                        Visible = setting == null || !setting.LotName
                    },
                    new CollumnOutput{
                        ColumnName = "QtyOnHand",
                        ColumnTitle = "SystemQty",
                        ColumnLength = 100,
                        Visible = true
                    },
                    new CollumnOutput{
                        ColumnName = "CountQty",
                        ColumnTitle = "CountedQty",
                        ColumnLength = 120,
                        Visible = true
                    },
                    new CollumnOutput{
                        ColumnName = "DiffQty",
                        ColumnTitle = "DiffQty",
                        ColumnLength = 80,
                        Visible = setting == null || !setting.DiffQty
                    },
                    new CollumnOutput {
                        ColumnName = "Description",
                        ColumnTitle = "Note",
                        ColumnLength = 100,
                        Visible = true
                    }
                };

                viewHeader = viewHeader.Where(s => s.Visible).ToList();

                foreach (var i in viewHeader)
                {
                    var rowHeader = $"<th width='{i.ColumnLength}'>{L(i.ColumnTitle)}</th>";
                    contentHeader += rowHeader;
                }

                //exportHtml = exportHtml.Replace("{{tableWidth}}", "1080"); //A4 landscape
                exportHtml = exportHtml.Replace("{{tableWidth}}", "752"); //A4 portrait

                foreach (var row in detail.Items)
                {
                    var tr = "<tr>";

                    foreach (var i in viewHeader)
                    {
                        var keyName = i.ColumnName;

                        if (keyName == "No")
                        {
                            tr += $"<td valign='top'>{row.No}</td>";
                        }
                        else if (keyName == "ItemCode")
                        {
                            tr += $"<td valign='top'>{row.ItemCode}</td>";
                        }
                        else if (keyName == "ItemName")
                        {
                            var itemName = row.ItemName;

                            if (!row.BatchNo.IsNullOrEmpty())
                            {
                                itemName = $"{row.ItemName} <br> {(row.TrackSerial ? "S/N: ":"Lot: ")} {row.BatchNo}";
                            }

                            tr += $"<td valign='top'>{itemName}</td>";
                        }
                        else if (keyName == "LotName")
                        {
                            tr += $"<td valign='top'>{row.LotName}</td>";
                        }
                        else if (keyName == "QtyOnHand")
                        {
                            tr += $"<td align='right' valign='top'>{FormatNumberCurrency(Math.Round(row.QtyOnHand, accountCyclePeriod.RoundingDigit, MidpointRounding.ToEven), accountCyclePeriod.RoundingDigit)}</td>";
                        }
                        else
                        {
                            tr += $"<td></td>";
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                var logo = "";
                if (tenant.LogoId.HasValue)
                {
                    var image = await _fileUploadManager.DownLoad(tenant.Id, tenant.LogoId.Value);

                    if(image != null)
                    {
                        var base64Str = StreamToBase64(image.Stream);
                        logo = $"<img src=\"data:{image.ContentType};base64, {base64Str}\" alt=\"logo\" style=\"max-height: 90px; max-width: 150px; display: block;\"/>";
                    }
                }

                exportHtml = exportHtml.Replace("{{Logo}}", logo);
                exportHtml = exportHtml.Replace("{{CompanyName}}", tenant.Name);
                exportHtml = exportHtml.Replace("{{ReportName}}", reportTitle);
                exportHtml = exportHtml.Replace("{{SubTitle}}", $"{L("InventoryAsOf")} {detail.PhysicalCountDate.ToString(formatDate)}");
                exportHtml = exportHtml.Replace("{{Left}}", $"{L("Location")} : {detail.Location.LocationName}");
                exportHtml = exportHtml.Replace("{{Right}}", $"{L("PhysicalCountDate")} :.......................");

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                exportHtml = exportHtml.Replace("{{Sign1}}", L("CountedBy"));
                exportHtml = exportHtml.Replace("{{Sign2}}", L("ApprovedBy"));

                HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption(PdfPageOrientation.Portrait);

                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate, PdfPageOrientation.Portrait);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");

                var result = new FileDto();
                result.FileName = $"PhysicalCount{detail.PhysicalCountDate.ToString("yyyy-MM-dd")}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_ExportPdf)]
        public async Task<FileDto> ExportPDF(EntityDto<Guid> input)
        {
            return await PrintHelper(input);
        }


        private ReportOutput GetExcelTemplateColumns()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                    // start properties with can filter
                    new CollumnOutput{
                        ColumnName = "No",
                        ColumnLength = 50,
                        ColumnTitle = L("No"),
                        ColumnType = ColumnType.Number,
                        SortOrder = 1,
                    },
                    // start properties with can filter
                    new CollumnOutput{
                        ColumnName = "ItemCode",
                        ColumnLength = 120,
                        ColumnTitle = L("ItemCode"),
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                    },
                    new CollumnOutput{
                        ColumnName = "ItemName",
                        ColumnLength = 350,
                        ColumnTitle = L("ItemName"),
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                    },
                    new CollumnOutput {
                        ColumnName = "LotName",
                        ColumnLength = 120,
                        ColumnTitle = L("LotName"),
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                    },
                    new CollumnOutput {
                        ColumnName = "BatchNo",
                        ColumnLength = 200,
                        ColumnTitle = L("BatchSerial"),
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                    },
                    new CollumnOutput{
                        ColumnName = "QtyOnHand",
                        ColumnLength = 120,
                        ColumnTitle = L("SystemQty"),
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                    },
                    new CollumnOutput{
                        ColumnName = "CountQty",
                        ColumnLength = 120,
                        ColumnTitle = L("CountedQty"),
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                    },
                    new CollumnOutput {
                        ColumnName = "UnitCost",
                        ColumnLength = 120,
                        ColumnTitle = L("UnitCost"),
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                    },
                    new CollumnOutput{
                        ColumnName = "Description",
                        ColumnLength = 250,
                        ColumnTitle = L("Note"),
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                    },
                },
                Groupby = "",
                HeaderTitle = "PhysicalCount",
                Sortby = "",
            };


            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_ImportExcel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcel(EntityDto<Guid> input)
        {

            var tenantId = AbpSession.TenantId;
            PhysicalCountDetailOutput model;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    model = await GetDetailHelper(input);
                }
            }


            var result = new FileDto();
            var sheetName = "Physical Count";

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
                var headerResule = GetExcelTemplateColumns();
                var columns = headerResule.ColumnInfo;

                foreach (var i in columns)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                var rowIndex = 2;
                foreach (var row in model.Items)
                {
                    var colIndex = 1;
                    foreach (var col in columns)
                    {
                        var property = row.GetType().GetProperty(col.ColumnName);
                        var value = property?.GetValue(row)?.ToString() ?? "";

                        if (col.ColumnType == ColumnType.Number && col.ColumnName != "No")
                        {
                            AddNumberToCell(ws, rowIndex, colIndex, Convert.ToDecimal(value));
                        }
                        else
                        {
                            AddTextToCell(ws, rowIndex, colIndex, value);
                        }
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(col.ColumnLength);

                        colIndex++;
                    }

                    rowIndex++;
                }


                result.FileName = $"PhysicalCountTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_ImportExcel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<List<CreateOrUpdatePhysicalItemInput>> ImportExcel(FileDto input)
        {
            var tenantId = AbpSession.TenantId;

            var itemDic = new Dictionary<string, Guid>();
            var lotDic = new Dictionary<string, long>();
            var batchDic = new Dictionary<string, Guid>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    itemDic = await _itemRepository.GetAll().AsNoTracking().ToDictionaryAsync(k => k.ItemCode, v => v.Id);
                    lotDic = await _lotRepository.GetAll().AsNoTracking().ToDictionaryAsync(k => k.LotName, v => v.Id);
                    batchDic = await _batchNoRepository.GetAll().AsNoTracking().ToDictionaryAsync(k => k.Code, v => v.Id);
                }
            }


            var result = new List<CreateOrUpdatePhysicalItemInput>();

            var excelPackage = await Read(input);

            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var itemCode = worksheet.Cells[i, 2].Value?.ToString();
                        var lotName = worksheet.Cells[i, 4].Value?.ToString();
                        var batchNo = worksheet.Cells[i, 5].Value?.ToString();

                        var qty = worksheet.Cells[i, 6].Value?.ToString();
                        var countQty = worksheet.Cells[i, 7].Value?.ToString();
                        var unitCost = worksheet.Cells[i, 8].Value?.ToString();
                        var description = worksheet.Cells[i, 9].Value?.ToString();

                        if (!itemDic.ContainsKey(itemCode)) throw new UserFriendlyException(L("IsNotValid", L("ItemCode") + $", Row = {i}"));
                        if (!lotDic.ContainsKey(lotName)) throw new UserFriendlyException(L("IsNotValid", L("Lot") + $", Row = {i}"));
                        if (!batchNo.IsNullOrEmpty() && !batchDic.ContainsKey(batchNo)) throw new UserFriendlyException(L("IsNotValid", L("Batch/Serial") + $", Row = {i}"));

                        var item = new CreateOrUpdatePhysicalItemInput
                        {
                            ItemId = itemDic[itemCode],
                            LotId = lotDic[lotName],
                            BatchNoId = batchNo.IsNullOrEmpty() ? (Guid?)null : batchDic[batchNo],
                            Description = description,
                            CountQty = Convert.ToDecimal(countQty),
                            DiffQty = Convert.ToDecimal(qty) - Convert.ToDecimal(countQty),
                            UnitCost = unitCost.IsNullOrEmpty() ? 0 : Convert.ToDecimal(unitCost)
                        };

                        result.Add(item);

                    }
                }
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Setting)]
        public async Task ChangeSetting(PhysicalCountSettingDto input)
        {
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            var entity = await _physicalCountSettingRepository.GetAll().FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = PhysicalCountSetting.Create(tenantId, userId, input.No, input.ItemCode, input.LotName, input.DiffQty);
                await _physicalCountSettingRepository.InsertAsync(entity);
            }
            else
            {
                entity.Udpate(userId, input.No, input.ItemCode, input.LotName, input.DiffQty);
                await _physicalCountSettingRepository.UpdateAsync(entity);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Setting)]
        public async Task<PhysicalCountSettingDto> GetSetting()
        {
            var result = await _physicalCountSettingRepository.GetAll().AsNoTracking()
                               .Select(s => new PhysicalCountSettingDto
                               {
                                   Id = s.Id,
                                   No = s.No,
                                   ItemCode = s.ItemCode,
                                   LotName = s.LotName,
                                   DiffQty = s.DiffQty,
                               })
                               .FirstOrDefaultAsync();

            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetDetail)]
        public async Task<ItemIssuePhysicalCountDetailOutput> GetItemIssue(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                              .GetAll()
                              .Include(u => u.ItemIssue)
                              .Include(u => u.Location)
                              .Include(u => u.Class)
                              .Include(u => u.Currency)
                              .Include(u => u.ItemIssue.ShippingAddress)
                              .Include(u => u.ItemIssue.BillingAddress)
                              .Include(u => u.JournalTransactionType)
                              .AsNoTracking()
                              .Where(u => u.JournalType == JournalType.ItemIssuePhysicalCount && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.COGS && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };
            //var averageCosts = await _inventoryManager.GetAvgCostQuery(@journal.Date.Date, locations).ToListAsync();

            var itemIssueItems = await (from rItem in _itemIssueItemRepository.GetAll()
                                                    .Include(u => u.Lot)
                                                    .Where(u => u.ItemIssueId == input.Id)
                                                    .AsNoTracking()

                                        join jItem in _journalItemRepository.GetAll()
                                                      .Include(u => u.Account)
                                                      .Where(u => u.Identifier.HasValue)
                                                      .Where(u => u.Key == PostingKey.Inventory)
                                                      .AsNoTracking()
                                        on rItem.Id equals jItem.Identifier
                                        join ii in _itemRepository.GetAll()
                                                   .Include(s => s.InventoryAccount)
                                                   .Include(s => s.SaleAccount)
                                                   .Include(s => s.PurchaseAccount)
                                                   .Include(s => s.SaleTax)
                                                   .Where(s => s.InventoryAccountId.HasValue)
                                                   .AsNoTracking()
                                        on rItem.ItemId equals ii.Id

                                        join i in _inventoryTransactionItemRepository.GetAll()
                                                                                      .Where(s => s.JournalType == JournalType.ItemIssueOther)
                                                                                      .Where(s => s.TransactionId == input.Id)
                                                                                      .AsNoTracking()
                                        on rItem.Id equals i.Id
                                        into cs
                                        from c in cs.DefaultIfEmpty()
                                        select
                                        new ItemIssueItemPhysicalCountDetailOutput()
                                        {
                                            LotId = rItem.LotId,
                                            LotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                                            Id = rItem.Id,
                                            Item = new ItemSummaryDetailOutput
                                            {
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.InventoryAccount),
                                                Id = ii.Id,
                                                InventoryAccountId = ii.InventoryAccountId,
                                                ItemCode = ii.ItemCode,
                                                ItemName = ii.ItemName,
                                                PurchaseAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.PurchaseAccount),
                                                PurchaseAccountId = ii.PurchaseAccountId,
                                                SaleAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.SaleAccount),
                                                SaleAccountId = ii.SaleAccountId,
                                                SalePrice = ii.SalePrice,
                                                SaleTax = ObjectMapper.Map<TaxDetailOutput>(ii.SaleTax),
                                                SaleTaxId = ii.SaleTaxId,
                                            },
                                            ItemId = rItem.ItemId,
                                            InventoryAccountId = jItem.AccountId,
                                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                            Description = rItem.Description,
                                            DiscountRate = rItem.DiscountRate,
                                            Qty = rItem.Qty,
                                            Total = c != null ? Math.Abs(c.LineCost) : rItem.Total,
                                            UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                            UseBatchNo = ii.UseBatchNo,
                                            TrackExpiration = ii.TrackExpiration,
                                            TrackSerial = ii.TrackSerial,
                                        })
                                        .ToListAsync();

            var batchDic = await _itemIssueItemBatchNoRepository.GetAll()
                               .AsNoTracking()
                               .Where(s => s.ItemIssueItem.ItemIssueId == input.Id)
                               .Select(s => new BatchNoItemOutput
                               {
                                   Id = s.Id,
                                   BatchNoId = s.BatchNoId,
                                   BatchNumber = s.BatchNo.Code,
                                   ExpirationDate = s.BatchNo.ExpirationDate,
                                   Qty = s.Qty,
                                   TransactionItemId = s.ItemIssueItemId
                               })
                               .GroupBy(s => s.TransactionItemId)
                               .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in itemIssueItems)
                {
                    if (batchDic.ContainsKey(i.Id)) i.ItemBatchNos = batchDic[i.Id];
                }
            }

            var result = ObjectMapper.Map<ItemIssuePhysicalCountDetailOutput>(journal.ItemIssue);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemIssueItems = itemIssueItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            //result.Total = journal.Credit;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.StatusName = journal.Status.ToString();
            result.InventoryTransactionTypeId = journal.JournalTransactionTypeId;
            result.InventoryTransactionTypeName = journal?.JournalTransactionType?.Name;
            //get total from inventory transaction item cache table
            result.Total = itemIssueItems.Sum(s => s.Total);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetDetail)]
        public async Task<ItemReceiptPhysicalCountDetailOutput> GetItemReceipt(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                              .GetAll()
                              .Include(u => u.ItemReceipt)
                              .Include(u => u.Location)
                              .Include(u => u.Class)
                              .Include(u => u.Currency)
                              .Include(u => u.ItemReceipt.ShippingAddress)
                              .Include(u => u.ItemReceipt.BillingAddress)
                              .Include(u => u.JournalTransactionType)
                              .AsNoTracking()
                              .Where(u => u.JournalType == JournalType.ItemReceiptPhysicalCount && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await(_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.COGS && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };
            //var averageCosts = await _inventoryManager.GetAvgCostQuery(@journal.Date.Date, locations).ToListAsync();

            var itemIssueItems = await(from rItem in _itemReceiptItemRepository.GetAll()
                                                    .Include(u => u.Lot)
                                                    .Where(u => u.ItemReceiptId == input.Id)
                                                    .AsNoTracking()

                                       join jItem in _journalItemRepository.GetAll()
                                                     .Include(u => u.Account)
                                                     .Where(u => u.Identifier.HasValue)
                                                     .Where(u => u.Key == PostingKey.Inventory)
                                                     .AsNoTracking()
                                       on rItem.Id equals jItem.Identifier
                                       join ii in _itemRepository.GetAll()
                                                  .Include(s => s.InventoryAccount)
                                                  .Include(s => s.SaleAccount)
                                                  .Include(s => s.PurchaseAccount)
                                                  .Include(s => s.SaleTax)
                                                  .Where(s => s.InventoryAccountId.HasValue)
                                                  .AsNoTracking()
                                       on rItem.ItemId equals ii.Id

                                       join i in _inventoryTransactionItemRepository.GetAll()
                                                                                     .Where(s => s.JournalType == JournalType.ItemReceiptOther)
                                                                                     .Where(s => s.TransactionId == input.Id)
                                                                                     .AsNoTracking()
                                       on rItem.Id equals i.Id
                                       into cs
                                       from c in cs.DefaultIfEmpty()
                                       select
                                       new ItemReceiptItemPhysicalCountDetailOutput()
                                       {
                                           LotId = rItem.LotId,
                                           LotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                                           Id = rItem.Id,
                                           Item = new ItemSummaryDetailOutput
                                           {
                                               InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.InventoryAccount),
                                               Id = ii.Id,
                                               InventoryAccountId = ii.InventoryAccountId,
                                               ItemCode = ii.ItemCode,
                                               ItemName = ii.ItemName,
                                               PurchaseAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.PurchaseAccount),
                                               PurchaseAccountId = ii.PurchaseAccountId,
                                               SaleAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.SaleAccount),
                                               SaleAccountId = ii.SaleAccountId,
                                               SalePrice = ii.SalePrice,
                                               SaleTax = ObjectMapper.Map<TaxDetailOutput>(ii.SaleTax),
                                               SaleTaxId = ii.SaleTaxId,
                                           },
                                           ItemId = rItem.ItemId,
                                           InventoryAccountId = jItem.AccountId,
                                           InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                           Description = rItem.Description,
                                           DiscountRate = rItem.DiscountRate,
                                           Qty = rItem.Qty,
                                           Total = c != null ? Math.Abs(c.LineCost) : rItem.Total,
                                           UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                           UseBatchNo = ii.UseBatchNo,
                                           TrackExpiration = ii.TrackExpiration,
                                           TrackSerial = ii.TrackSerial,
                                       })
                                        .ToListAsync();

            var batchDic = await _itemReceiptItemBatchNoRepository.GetAll()
                               .AsNoTracking()
                               .Where(s => s.ItemReceiptItem.ItemReceiptId == input.Id)
                               .Select(s => new BatchNoItemOutput
                               {
                                   Id = s.Id,
                                   BatchNoId = s.BatchNoId,
                                   BatchNumber = s.BatchNo.Code,
                                   ExpirationDate = s.BatchNo.ExpirationDate,
                                   Qty = s.Qty,
                                   TransactionItemId = s.ItemReceiptItemId
                               })
                               .GroupBy(s => s.TransactionItemId)
                               .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in itemIssueItems)
                {
                    if (batchDic.ContainsKey(i.Id)) i.ItemBatchNos = batchDic[i.Id];
                }
            }

            var result = ObjectMapper.Map<ItemReceiptPhysicalCountDetailOutput>(journal.ItemReceipt);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemReceiptItems = itemIssueItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            //result.Total = journal.Credit;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.StatusName = journal.Status.ToString();
            result.InventoryTransactionTypeId = journal.JournalTransactionTypeId;
            result.InventoryTransactionTypeName = journal?.JournalTransactionType?.Name;
            //get total from inventory transaction item cache table
            result.Total = itemIssueItems.Sum(s => s.Total);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Close)]
        public async Task Close(CarlEntityDto input)
        {
            var entity = await _physicalCountRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id); 
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => t.LockKey == TransactionLockType.PhysicalCount)
                                            .Where(t => t.IsLock == true && t.LockDate.Value.Date >= entity.PhysicalCountDate.Date)
                                            .AsNoTracking()
                                            .AnyAsync();

                if (locktransaction)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (entity.Status == TransactionStatus.Close) throw new UserFriendlyException(L("TransactionAlreadyClosed"));

            var tenant = await GetCurrentTenantAsync();
            if (!tenant.CurrencyId.HasValue) throw new UserFriendlyException(L("IsRequired", L("Currency")));
            if (!tenant.ItemIssuePhysicalCountId.HasValue) throw new UserFriendlyException(L("IsRequired", L("AdjustmentAccount")));

            var updateInput = new UpdatePhysicalCountInput
            {
                Id = entity.Id,
                PhysicalCountDate = entity.PhysicalCountDate,
                LocationId = entity.LocationId,
                ClassId = entity.ClassId,
                Memo = entity.Memo,
                Status = entity.Status,
                PhysicalCountNo = entity.PhysicalCountNo,
                Reference = entity.Reference,
                CurrencyId = tenant.CurrencyId.Value,
                AdjustmentAccountId = tenant.ItemIssuePhysicalCountId.Value,
                IsConfirm = input.IsConfirm,
                DateCompare = entity.PhysicalCountDate,
                PermissionLockId = input.PermissionLockId,
            };

            var physicalCountItems = await _physicalCountItemRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(u => u.PhysicalCountId == input.Id)
                                            .Select(s => new CreateOrUpdatePhysicalItemInput
                                            {
                                                Id = s.Id,
                                                No = s.No,
                                                ItemId = s.ItemId,
                                                BatchNoId = s.BatchNoId,
                                                LotId = s.LotId,
                                                Description = s.Description,
                                                QtyOnHand = s.QtyOnHand,
                                                CountQty = s.CountQty,
                                                DiffQty = s.DiffQty,
                                                UnitCost = s.UnitCost,
                                                Total = Math.Abs(s.DiffQty) * s.UnitCost,
                                            })
                                            .ToListAsync();

            updateInput.PhysicalCountItems = physicalCountItems;

            await ValidateStock(updateInput);

            entity.Close(AbpSession.UserId.Value);
            await _physicalCountRepository.BulkUpdateAsync(entity);

            await AdjustStockIssue(updateInput);
            await AdjustStockReceipt(updateInput);

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }
    }
}
