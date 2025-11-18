using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Inventories;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Locations;
using CorarlERP.MultiTenancy;
using CorarlERP.Productions.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using CorarlERP.Items.Dto;
using CorarlERP.Journals.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.ItemIssueProducts.Dto;
using CorarlERP.ItemReceiptProducts.Dto;
using CorarlERP.ItemReceiptProducts;
using Abp.Collections.Extensions;
using Abp.Extensions;
using CorarlERP.Locations.Dto;
using Abp.Linq.Extensions;
using CorarlERP.ChartOfAccounts.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ProductionProcesses.Dto;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Lots;
using CorarlERP.UserGroups;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.ProductionProcesses;
using Abp.Dependency;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.TransferOrders.Dto;
using CorarlERP.BatchNos;
using System.Security.Policy;
using CorarlERP.TransferOrders;
using Org.BouncyCastle.Asn1.Utilities;
using Amazon.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CorarlERP.ProductionPlans;
using CorarlERP.ProductionPlans.Dto;
using System.Text.RegularExpressions;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.Productions
{
    public class ProductionAppService : CorarlERPAppServiceBase, IProductionAppService
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IItemReceiptManager _itemReceiptManager;
        private readonly IProductionManager _productionManager;
        private readonly ICorarlRepository<Production, Guid> _productionRepository;
        private readonly IRawMaterialItemManager _rawMaterialItemsManager;
        private readonly IFinishItemManager _finishItemManager;
        private readonly IRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;
        private readonly IRepository<FinishItems, Guid> _finishItemsRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _accountRepository;
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IItemIssueManager _itemIssueManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<Lot, long> _lotRepository; 
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<ProductionProcess, long> _productionProcessRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IProductionPlanManager _productionPlanManager;
        private readonly ICorarlRepository<ProductionStandardCostGroup, Guid> _productionStandardCostGroupRepository;
        private readonly ICorarlRepository<ProductionIssueStandardCostGroup, Guid> _productionIssueStandardCostGroupRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ProductionAppService(
            ICorarlRepository<ProductionStandardCostGroup, Guid> productionStandardCostGroupRepository,
            ICorarlRepository<ProductionIssueStandardCostGroup, Guid> productionIssueStandardCostGroupRepository,
            IProductionPlanManager productionPlanManager,
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IRepository<Lot, long> lotRepository,
            IRepository<Tenant, int> tenantRepository,
            IJournalManager journalManager,
            IRepository<Journal, Guid> journalRepository,
            IJournalItemManager journalItemManager,
            IRepository<JournalItem, Guid> journalItemRepository,
            IItemReceiptItemManager itemReceiptItemManager,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemIssue, Guid> itemIssueRepository,
            IItemReceiptManager itemReceiptManager,
            IProductionManager productionManager,
            ICorarlRepository<Production, Guid> productionRepository,
            IRawMaterialItemManager rawMaterialItemsManager,
            IFinishItemManager finishItemManager,
            IRepository<RawMaterialItems, Guid> rawMaterialItemsRepository,
            IRepository<FinishItems, Guid> finishItemsRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<ChartOfAccount, Guid> accountRepository,
            IRepository<Currency, long> currencyRepository,
            IRepository<Class, long> classRepository,
            IRepository<Location, long> locationRepository,
            IItemIssueManager itemIssueManager,
            IInventoryManager inventoryManager,
            IItemIssueItemManager itemIssueItemManager ,
            IAutoSequenceManager autoSequenceManger,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IRepository<Lock, long> lockRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            IRepository<AccountCycles.AccountCycle, long> accountCycleRepository
            ) : base(accountCycleRepository,userGroupMemberRepository, locationRepository)
        {
            _lotRepository = lotRepository;
            _tenantRepository = tenantRepository;
            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _itemReceiptItemManager = itemReceiptItemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueRepository = itemIssueRepository;
            _itemReceiptManager = itemReceiptManager;
            _productionManager = productionManager;
            _productionRepository = productionRepository;
            _rawMaterialItemsManager = rawMaterialItemsManager;
            _finishItemManager = finishItemManager;
            _rawMaterialItemsRepository = rawMaterialItemsRepository;
            _finishItemsRepository = finishItemsRepository;
            _itemRepository = itemRepository;
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
            _classRepository = classRepository;
            _locationRepository = locationRepository;
            _itemIssueManager = itemIssueManager;
            _inventoryManager = inventoryManager;
            _itemIssueItemManager = itemIssueItemManager;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _productionProcessRepository = IocManager.Instance.Resolve<IRepository<ProductionProcess, long>>();
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _productionPlanManager = productionPlanManager;
            _productionStandardCostGroupRepository = productionStandardCostGroupRepository;
            _productionIssueStandardCostGroupRepository = productionIssueStandardCostGroupRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateBatchNo(CreateProductionInput input)
        {
            var validateItems = await _itemRepository.GetAll()
                             .Include(s => s.BatchNoFormula)
                             .Where(s => input.RawMaterialItems.Any(i => i.ItemId == s.Id))
                             .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                             .AsNoTracking()
                             .ToListAsync();

            if (!validateItems.Any()) return;

            var batchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var itemUseBatchs = input.RawMaterialItems.Where(s => batchItemDic.ContainsKey(s.ItemId)).ToList();

            var find = itemUseBatchs.Where(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNoId == Guid.Empty)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {find.Item.ItemCode}-{find.Item.ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.RawMaterialItems.Where(s => serialItemHash.Contains(s.ItemId)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Item: {batchItemDic[serialQty.ItemId].ItemCode}-{batchItemDic[serialQty.ItemId].ItemName}");

            var batchNoIds = itemUseBatchs.SelectMany(s => s.ItemBatchNos).GroupBy(s => s.BatchNoId).Select(s => s.Key).ToList();
            var batchNoDic = await _batchNoRepository.GetAll().Where(s => batchNoIds.Contains(s.Id)).ToDictionaryAsync(k => k.Id, v => v);
            var notFoundBatch = batchNoIds.FirstOrDefault(s => !batchNoDic.ContainsKey(s));
            if (notFoundBatch != null && notFoundBatch != Guid.Empty) throw new UserFriendlyException(L("RecordNotFound"));

            //duplicate on transaction item use same batch more that one time
            var duplicate = itemUseBatchs.FirstOrDefault(s => s.ItemBatchNos.GroupBy(g => g.BatchNoId).Any(r => r.Count() > 1));
            if (duplicate != null) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $" , Item: {batchItemDic[duplicate.ItemId].ItemCode}-{batchItemDic[duplicate.ItemId].ItemName}"));

            var zeroQty = itemUseBatchs.FirstOrDefault(s => s.ItemBatchNos.Any(r => r.Qty <= 0));
            if (zeroQty != null) throw new UserFriendlyException(L("MustBeGreaterThen", L("Qty"), 0));

            var validateQty = itemUseBatchs.FirstOrDefault(s => s.Unit != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", {batchItemDic[validateQty.ItemId].ItemCode}-{batchItemDic[validateQty.ItemId].ItemName}"));

            var itemIds = itemUseBatchs.GroupBy(s => s.ItemId).Select(s => s.Key).ToList();
            var lots = itemUseBatchs.GroupBy(s => s.FromLotId).Select(s => s.Key.Value).ToList();
            var exceptIds = new List<Guid>();
            if (input is UpdateProductionInput)
            {
                var id = (input as UpdateProductionInput).Id;

                var itemIssue = await _itemIssueRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.ProductionOrderId.HasValue && s.ProductionOrderId == id);
                var ItemReceipt = await _itemReceiptRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.ProductionOrderId.HasValue && s.ProductionOrderId == id);
                if (itemIssue != null) exceptIds.Add(itemIssue.Id);
                if (ItemReceipt != null) exceptIds.Add(ItemReceipt.Id);
            }

            var batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIds, lots, batchNoIds, input.ItemIssueDate.Value, exceptIds);
            var balanceDic = batchBalanceItems.ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => v.Qty);

            var batchQtyUse = itemUseBatchs
                              .SelectMany(s => s.ItemBatchNos.Select(r => new { s.ItemId, s.FromLotId, r.BatchNoId, r.Qty }))
                              .GroupBy(s => new { s.ItemId, s.FromLotId, s.BatchNoId })
                              .Select(s => new
                              {
                                  ItemName = $"{batchItemDic[s.Key.ItemId].ItemCode}-{batchItemDic[s.Key.ItemId].ItemName}, BatchNo: {batchNoDic[s.Key.BatchNoId].Code}",
                                  Key = $"{s.Key.ItemId}-{s.Key.FromLotId}-{s.Key.BatchNoId}",
                                  Qty = s.Sum(t => t.Qty)
                              })
                              .ToList();

            var notInStock = batchQtyUse.FirstOrDefault(s => !balanceDic.ContainsKey(s.Key));
            if (notInStock != null) throw new UserFriendlyException(L("ItemOutOfStock", notInStock.ItemName));

            var issueOverQty = batchQtyUse.FirstOrDefault(s => balanceDic.ContainsKey(s.Key) && s.Qty > balanceDic[s.Key]);
            if (issueOverQty != null)
                throw new UserFriendlyException(L("CannotIssueForThisBiggerStockItem",
                    issueOverQty.ItemName,
                    String.Format("{0:n0}", balanceDic[issueOverQty.Key]),
                    String.Format("{0:n0}", issueOverQty.Qty)));
        }

        private async Task ValidateAddBatchNo(CreateProductionInput input)
        {
            var validateItems = await _itemRepository.GetAll()
                             .Include(s => s.BatchNoFormula)
                             .Where(s => input.FinishItems.Any(i => i.ItemId == s.Id))
                             .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                             .AsNoTracking()
                             .ToListAsync();

            if (!validateItems.Any()) return;

            var useBatchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var useBatchItems = input.FinishItems.Where(s => useBatchItemDic.ContainsKey(s.ItemId)).ToList();

            var emptyManualBatchItem = useBatchItems.FirstOrDefault(s => !useBatchItemDic[s.ItemId].AutoBatchNo && (s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 ||  s.ItemBatchNos.Any(r => r.BatchNumber.IsNullOrWhiteSpace())));
            if (emptyManualBatchItem != null) throw new UserFriendlyException(L("PleaseEnter", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {useBatchItemDic[emptyManualBatchItem.ItemId].ItemCode}-{useBatchItemDic[emptyManualBatchItem.ItemId].ItemName}");

            var expriationItemHash = validateItems.Where(s => s.TrackExpiration).Select(s => s.Id).ToHashSet();
            var expirationItems = input.FinishItems.Where(s => expriationItemHash.Contains(s.ItemId)).ToList();
            var emptyExpiration = expirationItems.FirstOrDefault(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => !r.ExpirationDate.HasValue));
            if (emptyExpiration != null) throw new UserFriendlyException(L("PleaseEnter", L("ExpiratioinDate")) + $", Item: {useBatchItemDic[emptyExpiration.ItemId].ItemCode}-{useBatchItemDic[emptyExpiration.ItemId].ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.FinishItems.Where(s => serialItemHash.Contains(s.ItemId)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Item: {useBatchItemDic[serialQty.ItemId].ItemCode}-{useBatchItemDic[serialQty.ItemId].ItemName}");


            var productionProccess = await _productionProcessRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.ProductionProcessId);

            var autoBatchItems = useBatchItems.Where(s => useBatchItemDic[s.ItemId].AutoBatchNo).ToList();
            if (autoBatchItems.Any())
            {
                foreach (var item in autoBatchItems)
                {
                    var formula = useBatchItemDic[item.ItemId].BatchNoFormula;
                    if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $", Item: {useBatchItemDic[item.ItemId].ItemCode}-{useBatchItemDic[item.ItemId].ItemName}");

                    var field = useBatchItemDic[item.ItemId].GetType().GetProperty(formula.FieldName).GetValue(useBatchItemDic[item.ItemId], null);
                    if (field == null) throw new UserFriendlyException(L("IsNotValid", formula.FieldName));

                    var code = field.ToString();

                    if (item.ItemBatchNos == null) item.ItemBatchNos = new List<BatchNoItemOutput> { new BatchNoItemOutput { Qty = item.Unit } };

                    foreach (var batch in item.ItemBatchNos)
                    {
                        batch.BatchNumber = $"{code}-{input.ItemReceiptDate.Value.ToString(formula.DateFormat)}";

                        if (productionProccess != null && productionProccess.UseStandard)
                        {
                            batch.IsStandard = true;
                            if (formula.StandardPrePos == BatchNoFormulaPrePos.Prefix)
                            {
                                batch.BatchNumber = $"{formula.PrePosCode}-{code}-{input.ItemReceiptDate.Value.ToString(formula.DateFormat)}";
                            }
                            else if (formula.StandardPrePos == BatchNoFormulaPrePos.Postfix)
                            {
                                batch.BatchNumber = $"{code}-{input.ItemReceiptDate.Value.ToString(formula.DateFormat)}-{formula.PrePosCode}";
                            }
                        }
                    }
                }
            }

            var duplicate = useBatchItems.FirstOrDefault(s => s.ItemBatchNos.GroupBy(g => g.BatchNumber).Any(r => r.Count() > 1));
            if (duplicate != null) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $" , Item: {useBatchItemDic[duplicate.ItemId].ItemCode}-{useBatchItemDic[duplicate.ItemId].ItemName}"));

            var zeroQty = useBatchItems.FirstOrDefault(s => s.ItemBatchNos.Any(r => r.Qty <= 0));
            if (zeroQty != null) throw new UserFriendlyException(L("MustBeGreaterThen", L("Qty"), 0));

            var validateQty = useBatchItems.FirstOrDefault(s => s.Unit != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", Item: {useBatchItemDic[validateQty.ItemId].ItemCode}-{useBatchItemDic[validateQty.ItemId].ItemName}"));

            var batchNos = useBatchItems.SelectMany(s => s.ItemBatchNos).GroupBy(s => s.BatchNumber).Select(s => s.Key).ToHashSet();
            var batchNoDic = await _batchNoRepository.GetAll().Where(s => batchNos.Contains(s.Code)).ToDictionaryAsync(k => k.Code, v => v);

            var addBatchNos = new List<BatchNo>();
            foreach (var item in useBatchItems)
            {
                foreach (var i in item.ItemBatchNos)
                {
                    if (batchNoDic.ContainsKey(i.BatchNumber))
                    {
                        if (batchNoDic[i.BatchNumber].ItemId != item.ItemId) throw new UserFriendlyException(L("BatchNoAlreadyUseByOtherItem") + $" {i.BatchNumber}");
                        if (expriationItemHash.Contains(item.ItemId))
                        {
                            if (batchNoDic[i.BatchNumber].ExpirationDate.Value.Date != i.ExpirationDate.Value.Date) throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {i.ExpirationDate.Value.ToString("dd-MM-yyyy")}");
                        }

                        i.BatchNoId = batchNoDic[i.BatchNumber].Id;
                    }
                    else
                    {
                        var batchNo = BatchNo.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, i.BatchNumber, item.ItemId, i.IsStandard, serialItemHash.Contains(item.ItemId), i.ExpirationDate);
                        i.BatchNoId = batchNo.Id;
                        addBatchNos.Add(batchNo);
                        batchNoDic.Add(i.BatchNumber, batchNo);
                    }
                }
            }

            var duplicateBatch = useBatchItems
                             .SelectMany(s => s.ItemBatchNos.Select(r => new { r.BatchNumber, s.ItemId }))
                             .GroupBy(s => s.BatchNumber)
                             .Select(g => g.Select(s => new { s.ItemId, s.BatchNumber }).Distinct())
                             .Where(s => s.Count() > 1)
                             .FirstOrDefault();
            if (duplicateBatch != null && duplicateBatch.Any()) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $": {duplicateBatch.First().BatchNumber}, Item : {useBatchItemDic[duplicateBatch.First().ItemId].ItemCode}-{useBatchItemDic[duplicateBatch.First().ItemId].ItemName}"));

            if (addBatchNos.Any()) await _batchNoRepository.BulkInsertAsync(addBatchNos);
        }
        private async Task CalculateTotal(CreateProductionInput input)
        {
            var round = await GetCurrentCycleAsync();

            decimal total = 0;
            foreach (var item in input.FinishItems)
            {
                item.UnitCost = Math.Round(item.UnitCost, round.RoundingDigitUnitCost);
                item.Total = Math.Round(item.Unit * item.UnitCost, round.RoundingDigit);
                total += item.Total;
            }
            input.SubTotalFinishProduction = total;

            ValidateTotal(input);

            var tenant = await GetCurrentTenantAsync();
            if(tenant.ProductionSummaryNetWeight || tenant.ProductionSummaryQty)
            {
                await CalculateSummary(input);
            }
        }

        private async Task CalculateSummary(CreateProductionInput input)
        {
            var process = await _productionProcessRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.ProductionProcessId);
            if (process == null) throw new UserFriendlyException(L("IsNotValid", L("ProductionProcess")));

            var itemIds = input.RawMaterialItems.Select(s => s.ItemId).Concat(input.FinishItems.Select(s => s.ItemId)).Distinct().ToList();
            var itemNetWeightDic = await _productionPlanManager.GetItemNetWeight(itemIds);
            var standardGroups = await _productionPlanManager.GetItemStandardGroups(itemIds);

            input.TotalIssueQty = 0;
            input.TotalReceiptQty = 0;
            input.TotalIssueNetWeight = 0;
            input.TotalReceiptNetWeight = 0;

            void calculateIssue(CreateProductionInput p)
            {
                //p.IssueQty = p.RawMaterialItems.Sum(s => s.Unit);
                //p.IssueNetWeight = p.RawMaterialItems.Sum(s => s.Unit * (itemNetWeightDic.ContainsKey(s.ItemId) ? itemNetWeightDic[s.ItemId] : 0));
                p.TotalIssueQty = 0;
                p.TotalIssueNetWeight = 0;
                p.IssueStandardGroups = new List<StandardGroupSummary>();

                foreach (var r in p.RawMaterialItems)
                {
                    var netWeight = r.Unit * (itemNetWeightDic.ContainsKey(r.ItemId) ? itemNetWeightDic[r.ItemId] : 0);

                    p.TotalIssueQty += r.Unit;
                    p.TotalIssueNetWeight += netWeight;

                    long? group = null;
                    if (standardGroups.ContainsKey(r.ItemId)) group = standardGroups[r.ItemId];

                    var find = p.IssueStandardGroups.FirstOrDefault(s => s.GroupId == group);
                    if (find == null)
                    {
                        p.IssueStandardGroups.Add(new StandardGroupSummary
                        {
                            GroupId = group,
                            TotalQty = r.Unit,
                            TotalNetWeight = netWeight
                        });
                    }
                    else
                    {
                        find.TotalQty += r.Unit;
                        find.TotalNetWeight += netWeight;
                    }
                }
            }

            void calculateReceipt(CreateProductionInput p)
            {
                p.TotalReceiptQty = 0;
                p.TotalReceiptNetWeight = 0;
                p.StandardGroups = new List<StandardGroupSummary>();

                foreach(var r in p.FinishItems)
                {
                    var netWeight = r.Unit * (itemNetWeightDic.ContainsKey(r.ItemId) ? itemNetWeightDic[r.ItemId] : 0);

                    p.TotalReceiptQty += r.Unit;
                    p.TotalReceiptNetWeight += netWeight;

                    long? group = null;
                    if (standardGroups.ContainsKey(r.ItemId)) group = standardGroups[r.ItemId];

                    var find = p.StandardGroups.FirstOrDefault(s => s.GroupId == group);
                    if (find == null)
                    {
                        p.StandardGroups.Add(new StandardGroupSummary
                        {
                            GroupId = group,
                            TotalQty = r.Unit,
                            TotalNetWeight = netWeight
                        });
                    }
                    else
                    {
                        find.TotalQty += r.Unit;
                        find.TotalNetWeight += netWeight;
                    }
                }
            }

            if (process.ProductionProcessType == ProductionProcessType.Issue)
            {
                calculateIssue(input);
            }
            else if(process.ProductionProcessType == ProductionProcessType.Receipt)
            {
                calculateReceipt(input);
            }
            else if(process.ProductionProcessType == ProductionProcessType.Both)
            {
                calculateIssue(input);
                calculateReceipt(input);
            }
        }

        private void ValidateTotal(CreateProductionInput input)
        {
            var total = input.FinishItems.Sum(s => s.Total);
            if (total != input.SubTotalFinishProduction) throw new UserFriendlyException(L("IsNotValid", L("Total")));
        }

        private async Task ValidateNetWeight(CreateProductionInput input)
        {
            var tenant = await GetCurrentTenantAsync();
            if (!tenant.ValidateProductionNetWeight) return;

            var itemIds = input.RawMaterialItems.Select(s => s.ItemId).Concat(input.FinishItems.Select(f => f.ItemId)).GroupBy(s => s).Select(s => s.Key).ToList();
            var itemWithUnits = (await GetItemProperties(itemIds).Where(s => s.IsUnit).ToListAsync()).ToDictionary(s => s.ItemId, s => s.NetWeight);

            var totalIssueNetWeight = input.RawMaterialItems.Sum(s => s.Unit * (itemWithUnits.ContainsKey(s.ItemId) ? itemWithUnits[s.ItemId] : 0));
            var totalReceiptNetWeight = input.FinishItems.Sum(s => s.Unit * (itemWithUnits.ContainsKey(s.ItemId) ? itemWithUnits[s.ItemId] : 0));

            if (totalReceiptNetWeight > totalIssueNetWeight) throw new UserFriendlyException(L("ProductionNetWeightErrorMessage"));
        }

        private async Task ValidateProductionPlan(CreateProductionInput input)
        {
            var process = await _productionProcessRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.ProductionProcessId);
            if(process == null) throw new UserFriendlyException(L("IsNotValid", L("ProductionProcess")));

            if (process.IsRequiredProductionPlan && (!input.ProductionPlanId.HasValue || input.ProductionPlanId == Guid.Empty)) throw new UserFriendlyException(L("IsRequired", L("ProductionPlan")));
        }

        private void ValidateItemLots(CreateProductionInput input)
        {
            var find = input.RawMaterialItems.Where(s => input.FinishItems.Any(r => s.ItemId == r.ItemId && s.FromLotId == r.ToLotId)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("ProductionCanNotUseSameLot") + $" {find.Item?.ItemCode}-{find.Item?.ItemName}");
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateProductionInput input)
        {

            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => t.LockKey == TransactionLockType.ProductionOrder && t.IsLock == true &&  t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateItemLots(input);
            await ValidateProductionPlan(input);
            await ValidateNetWeight(input);
            if (input.ConvertToIssueAndReceipt)
            {
                await ValidateBatchNo(input); //for issue
                await ValidateAddBatchNo(input); //for receipt
            }

            await CalculateTotal(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ProductionOrder);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.ProductionNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = Production.Create(tenantId, userId, input.FromLocationId, input.ToLocationId,
                                    input.FromClassId, input.ToClassId,
                                    input.ProductionNo, input.Date, input.Reference, 
                                    input.Status, input.Memo, input.ConvertToIssueAndReceipt, 
                                    input.ItemReceiptDate, input.ItemIssueDate, input.SubTotalRawProduction,
                                    input.SubTotalFinishProduction, input.ProductionAccountId, 
                                    input.ProductionProcessId, input.ProductionPlanId,
                                    input.TotalIssueQty, input.TotalReceiptQty,
                                    input.TotalIssueNetWeight, input.TotalReceiptNetWeight);

            #region FinishItems and   
            int indextoLot = 0; 
            foreach (var i in input.FinishItems)
            {
                indextoLot++;
                if (i.ToLotId == 0 && i.ItemId != null )
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indextoLot.ToString());
                }
                var finishItems = FinishItems.Create(tenantId, userId, entity, i.ItemId, i.Description, i.Unit, i.UnitCost, i.Total);

                finishItems.UpdateToLot(i.ToLotId);
                base.CheckErrors(await _finishItemManager.CreateAsync(finishItems));
                if (input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
                {
                    i.Id = finishItems.Id;
                }
            }

            int indexFromLot = 0;
            foreach (var i in input.RawMaterialItems)
            {
                indexFromLot++;             
                if (i.FromLotId == 0 && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexFromLot.ToString());
                }
                var @rawMaterialItems = RawMaterialItems.Create(tenantId, userId, entity, i.ItemId, i.Description, i.Unit, i.UnitCost, i.Total);
                @rawMaterialItems.UpdateFromLot(i.FromLotId.Value);
                base.CheckErrors(await _rawMaterialItemsManager.CreateAsync(@rawMaterialItems));
                if (input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
                {
                    i.Id = @rawMaterialItems.Id;
                }
            }

            #endregion
            
            CheckErrors(await _productionManager.CreateAsync(@entity, auto.RequireReference));
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
            {
                var tenant = await GetCurrentTenantAsync();
                
                var locations = new List<long?>() {
                    entity.FromLocationId
                };
                
                var createInputItemIssueProduction = new CreateItemIssueProductInput()
                {
                    Memo = input.Memo,
                    IssueNo = input.ProductionNo,
                    IsConfirm = input.IsConfirm,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.FromClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.ItemIssueDate.Value,
                    LocationId = input.FromLocationId,
                    ReceiveFrom = ReceiveFrom.ProductionOrder,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = input.ProductionAccountId,
                    ProductionId = entity.Id,
                    Total = input.SubTotalRawProduction,
                    ProductionProcessId = input.ProductionProcessId,
                    Items = input.RawMaterialItems.Select(t => new CreateOrUpdateRawMaterialItemsInput
                    {
                        FromLotId = t.FromLotId,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = t.Item.InventoryAccountId.Value,
                        Description = t.Description,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = t.Total, 
                        UnitCost = t.UnitCost,                         
                        RawMaterialId = t.Id,
                        ProductionOrderNo = t.ProductionOrderNo,                      
                        Unit = t.Unit,  
                        ItemBatchNos = t.ItemBatchNos,
                    }).ToList(),

                };
                await CreateItemIssueProduction(createInputItemIssueProduction);
                
                var createInputItemRecepitProduction = new CreateItemReceiptProductionInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.ToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.ItemReceiptDate.Value,
                    LocationId = input.ToLocationId,
                    ProductionProcessId = input.ProductionProcessId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.ProductionOrder,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = input.ProductionAccountId,
                    ProductionOrderId = entity.Id,
                    Total = input.SubTotalFinishProduction,
                    Items = input.FinishItems.Select(t => new CreateOrUpdateItemReceiptItemProductionInput
                    {
                        LotId = t.ToLotId,
                        InventoryAccountId = t.Item.InventoryAccountId.Value,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = t.Total, 
                        UnitCost = t.UnitCost, 
                        Description = t.Description,
                        FinishItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList()

                };
                await CreateItemReceiptProduct(createInputItemRecepitProduction);
            }

            if (input.StandardGroups != null && input.StandardGroups.Any())
            {
                foreach (var g in input.StandardGroups)
                {
                    var standardGroup = ProductionStandardCostGroup.Create(tenantId, userId, entity.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                    await _productionStandardCostGroupRepository.InsertAsync(standardGroup);
                }
            }

            if (input.IssueStandardGroups != null && input.IssueStandardGroups.Any())
            {
                foreach (var g in input.IssueStandardGroups)
                {
                    var standardGroup = ProductionIssueStandardCostGroup.Create(tenantId, userId, entity.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                    await _productionIssueStandardCostGroupRepository.InsertAsync(standardGroup);
                }
            }

            if (input.ProductionPlanId.HasValue)
            {
                var tenant = await GetCurrentTenantAsync();
                if(tenant.ProductionSummaryNetWeight || tenant.ProductionSummaryQty)
                {
                    await CurrentUnitOfWork.SaveChangesAsync();
                    await _productionPlanManager.CalculateByIdAsync(userId, new List<Guid> { input.ProductionPlanId.Value });
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ProductionOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        //Auto create Item Issue Production
        private async Task CreateItemIssueProduction(CreateItemIssueProductInput input)
        {

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                    && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var countItemIssueItems = (from ItemIssueitem in input.Items
                                       where (ItemIssueitem.RawMaterialId != null)
                                       select ItemIssueitem.RawMaterialId).Count();
            if (input.ReceiveFrom == ReceiveFrom.ProductionOrder && countItemIssueItems <= 0 && input.ProductionId.Value == null)
            {
                throw new UserFriendlyException(L("PleaseAddProductionOrder"));
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.IssueNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.IssueNo, input.Date,
                                    input.Memo, input.Total, input.Total, input.CurrencyId,
                                    input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueProduction);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;
            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                LotId = u.FromLotId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.FromLotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                            .Where(t => lotIds.Contains(t.Id))
                            .Select(t => (long?)t.LocationId)
                            .ToListAsync();
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, @entity, roundingId*/);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;
            #endregion Calculat Cost
            
            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                        input.ClearanceAccountId, input.Memo,
                                        input.Total, 0, PostingKey.COGS, null);

            //insert to item Issue
            var itemIssue = ItemIssue.Create(tenantId, userId, input.ReceiveFrom, null,
                                        input.SameAsShippingAddress, input.ShippingAddress,
                                        input.BillingAddress, input.Total, input.ProductionProcessId);

            itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueProduction);
            @entity.UpdateIssueProduction(itemIssue);
            
            if (input.ReceiveFrom == ReceiveFrom.ProductionOrder)
            {
                var product = await _productionRepository.GetAll().Where(u => u.Id == input.ProductionId).FirstOrDefaultAsync();
                // update received status 
                if (product == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    product.UpdateShipedStatus(TransferStatus.ShipAll);
                }
                CheckErrors(await _productionManager.UpdateAsync(product));

                itemIssue.UpdateProductionOrderId(input.ProductionId.Value);
            }

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemIssueManager.CreateAsync(itemIssue));

            
            foreach (var i in input.Items)
            {
                //insert to item Issue item
                var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, i.ItemId,
                                    i.Description, i.Qty, i.UnitCost, 0, i.Total);
                if (i.RawMaterialId != null)
                {
                    itemIssueItem.UpdateRawmatailItemId(i.RawMaterialId.Value);
                }
                itemIssueItem.UpdateLot(i.FromLotId);
                CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));
                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                                        i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemIssueItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }

        //Auto Delete Item Issue Product        
        private async Task DeleteItemIssueProduct(CarlEntityDto input)
        {
            if (input.Id == Guid.Empty) return;
            var journal = await _journalRepository.GetAll()
               .Include(u => u.ItemIssue)
               .Include(u => u.ItemIssue.ShippingAddress)
               .Include(u => u.ItemIssue.BillingAddress)
               .Where(u => u.JournalType == JournalType.ItemIssueProduction && u.ItemIssueId == input.Id)
               .FirstOrDefaultAsync();

            //query get item Issue
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                    && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var @entity = journal.ItemIssue;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != journal.Id &&
                    (t.JournalType == JournalType.ItemIssueSale ||
                       t.JournalType == JournalType.ItemIssueKitchenOrder ||
                                    t.JournalType == JournalType.ItemIssueOther ||
                                    t.JournalType == JournalType.ItemIssueTransfer ||
                                    t.JournalType == JournalType.ItemIssueAdjustment ||
                                    t.JournalType == JournalType.ItemIssueProduction))
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

            //@Delete rounded adjustment journal
            //if (journal.RoundedAdjustmentJournalId != null)
            //{
            //    var @entityRoundedAdjustment = await _journalRepository.GetAll().Where(t => t.Id == journal.RoundedAdjustmentJournalId).FirstOrDefaultAsync();

            //    if (@entityRoundedAdjustment == null)
            //    {
            //        throw new UserFriendlyException(L("RecordNotFound"));
            //    }

            //    var JournalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == @entityRoundedAdjustment.Id).ToListAsync();

            //    foreach (var s in JournalItems)
            //    {
            //        CheckErrors(await _journalItemManager.RemoveAsync(s));
            //    }
            //    CheckErrors(await _journalManager.RemoveAsync(@entityRoundedAdjustment));
            //}

            //update receive status of product order to pending 
            if (journal.JournalType == JournalType.ItemIssueProduction)
            {
                var product = _productionRepository.GetAll().Where(u => u.Id == journal.ItemIssue.ProductionOrderId).FirstOrDefault();
                // update received status 
                if (product != null)
                {
                    product.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _productionManager.UpdateAsync(product));
                }
            }
            journal.UpdateIssueProduction(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = journal.Date;

            CheckErrors(await _journalManager.RemoveAsync(journal));


            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => s.ItemIssueItem.ItemIssueId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _itemIssueItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item Issue item and delete 
            var itemIssueItems = await _itemIssueItemRepository.GetAll()
                .Where(u => u.ItemIssueId == entity.Id).ToListAsync();

            var scheduleItems = itemIssueItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in itemIssueItems)
            {

                CheckErrors(await _itemIssueItemManager.RemoveAsync(iri));
            }

            CheckErrors(await _itemIssueManager.RemoveAsync(entity));

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        //Auto Update Item Issue Product
        private async Task UpdateItemIssueProduct(UpdateItemIssueProductInput input)
        {

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository.GetAll()
                              .Where(u => u.JournalType == JournalType.ItemIssueProduction && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                       .Where(t => t.IsLock == true && t.LockDate != null &&
                                       (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                       && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                ItemIssueItemId = input.Id,
                ItemReceiptItemId = input.ItemReceiptId,
                LotId = u.FromLotId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.FromLotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                            .Where(t => lotIds.Contains(t.Id))
                            .Select(t => (long?)t.LocationId)
                            .ToListAsync();
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, @journal, roundingId*/);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;
            #endregion Calculat Cost

            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll().Where(u =>
                                                u.JournalId == journal.Id &&
                                                u.Key == PostingKey.COGS && u.Identifier == null)
                                        ).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, input.Total, 0);
            
            //update item Issue 
            var itemIssue = await _itemIssueManager.GetAsync(input.Id, true);

            if (itemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            itemIssue.Update(tenantId, input.ReceiveFrom, null,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, input.ProductionProcessId);
            
            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));

            //Update Item Issue Item and Journal Item
            var itemIssueItems = await _itemIssueItemRepository.GetAll().Where(u => u.ItemIssueId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                                .Where(u => u.JournalId == journal.Id &&
                                                            u.Key == PostingKey.Inventory && u.Identifier != null)
                                                ).ToListAsync();

            var toDeleteItemIssueItem = itemIssueItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemIssueItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemIssueItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemIssueItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            //await CurrentUnitOfWork.SaveChangesAsync();
            
            #region update receive from 
            // update if come from Production order 
            if (input.ReceiveFrom == ReceiveFrom.ProductionOrder && input.ProductionId != null)
            {
                if (itemIssue.ProductionOrderId != null && itemIssue.ProductionOrderId != input.ProductionId)
                {
                    var @ProductionOld = await _productionRepository.GetAll().Where(u => u.Id == itemIssue.ProductionOrderId).FirstOrDefaultAsync();
                    CheckErrors(await _productionManager.UpdateAsync(@ProductionOld));
                }

                var @Production = await _productionRepository.GetAll().Where(u => u.Id == input.ProductionId).FirstOrDefaultAsync();
                // update received status 
                if (@Production == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    @Production.UpdateShipedStatus(TransferStatus.ShipAll);
                }
                CheckErrors(await _productionManager.UpdateAsync(@Production));
                itemIssue.UpdateProductionOrderId(input.ProductionId);
            }
            //else // in some case if user switch from receive from Production order to other so update Production of field item issue transfer order id to null
            //{
            //    var @Production = await _productionRepository.GetAll().Where(u => u.Id == itemIssue.ProductionOrderId).FirstOrDefaultAsync();
            //    if (@Production != null)
            //    {
            //        @Production.UpdateShipedStatus(TransferStatus.Pending);
            //        CheckErrors(await _productionManager.UpdateAsync(@Production));
            //    }
            //    itemIssue.UpdateProductionOrderId(null);
            //}
            #endregion

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            if (autoSequence.CustomFormat == true) {
                input.IssueNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.IssueNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);

            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemIssueItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemIssueItem.Any(r => r.Id == s.ItemIssueItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }

            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemIssueItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemIssueItem != null)
                    {
                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.FromLotId);
                        CheckErrors(await _itemIssueItemManager.UpdateAsync(itemIssueItem));

                    }
                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, 0, c.Total);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemIssueItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemIssueItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            oldItemBatch.Update(userId, itemIssueItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _itemIssueItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }
                }
                else if (c.Id == null) //create
                {
                    //insert to item Issue item
                    var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    if (c.RawMaterialId != null)
                    {
                        itemIssueItem.UpdateRawmatailItemId(c.RawMaterialId.Value);
                    }
                    itemIssueItem.UpdateLot(c.FromLotId);
                    CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, 0, c.Total, PostingKey.Inventory, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemIssueItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }

            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        //Auto Create Item Receipt product
        private async Task CreateItemReceiptProduct(CreateItemReceiptProductionInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                   && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.ReceiptNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            //var total = input.Items.Select(t => t.Total).Sum();
            //input.Total = total;
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.ReceiptNo, input.Date,
                        input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            entity.UpdateCreationTimeIndex(entity.CreationTimeIndex.Value + 1);
            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo, 0,
                                    input.Total, PostingKey.Clearance, null);

            //insert to item Receipt
            var @itemReceipt = ItemReceipt.Create(tenantId, userId, input.ReceiveFrom, null,
                            input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress, input.Total, input.ProductionProcessId);

            itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptProduction);
            @entity.UpdateItemReceiptProduction(@itemReceipt);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptProduction);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.ProductionOrder)
            {
                var product = await _productionRepository.GetAll().Where(u => u.Id == input.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (product == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    product.UpdateShipedStatus(TransferStatus.ReceiveAll);
                }
                CheckErrors(await _productionManager.UpdateAsync(product));

                itemReceipt.UpdateProductionOrderId(input.ProductionOrderId.Value);
            }

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemReceiptManager.CreateAsync(@itemReceipt));

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            foreach (var i in input.Items)
            {
                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, i.ItemId,
                                                       i.Description, i.Qty, i.UnitCost, 0, i.Total);
                if (i.FinishItemId != null)
                {
                    @itemReceiptItem.UpdateFinishItemId(i.FinishItemId.Value);
                }

                itemReceiptItem.UpdateLot(i.LotId);
                CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));
                //insert inventory journal item into debit
                var inventoryJournalItem =
                    JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                    i.Description, i.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                    }
                }

            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceipt(itemReceipt.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }

        //Auto Delete Item Receipt product

        private async Task DeleteItemReceiptProduction(CarlEntityDto input)
        {
            if (input.Id == Guid.Empty) return;
            var @jounal = await _journalRepository.GetAll()
               .Include(u => u.ItemReceipt)
               .Include(u => u.ItemReceipt.ShippingAddress)
               .Include(u => u.ItemReceipt.BillingAddress)
               .Where(u => u.JournalType == JournalType.ItemReceiptProduction && u.ItemReceiptId == input.Id)
               .FirstOrDefaultAsync();


            if (input.IsConfirm == false)
            {
                //query get item Receipt
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                  && t.IsLock == true && t.LockDate != null  && t.LockDate.Value.Date >= jounal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = @jounal.ItemReceipt;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

            if (jounal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != jounal.Id &&
                                     (t.JournalType == JournalType.ItemReceiptPurchase ||
                                     t.JournalType == JournalType.ItemReceiptAdjustment ||
                                     t.JournalType == JournalType.ItemReceiptOther ||
                                     t.JournalType == JournalType.ItemReceiptTransfer ||
                                     t.JournalType == JournalType.ItemReceiptProduction))
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
            @jounal.UpdateItemReceiptProduction(null);


            //update receive status of product order to pending 
            if (jounal.JournalType == JournalType.ItemIssueProduction)
            {
                var product = await _productionRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (product != null)
                {
                    product.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _productionManager.UpdateAsync(product));
                }
            }


            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = jounal.Date;

            CheckErrors(await _journalManager.RemoveAsync(@jounal));

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => s.ItemReceiptItem.ItemReceiptId == input.Id).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (itemBatchNos.Any())
            {
                batchNos = itemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt item and delete 
            var @itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                .Where(u => u.ItemReceiptId == entity.Id).ToListAsync();

            var scheduleItems = itemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in @itemReceiptItems)
            {

                CheckErrors(await _itemReceiptItemManager.RemoveAsync(iri));
            }

            CheckErrors(await _itemReceiptManager.RemoveAsync(entity));

            if (batchNos.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        //Auto Update Item Receipt Product
        private async Task UpdateItemReceiptProduct(UpdateItemReceiptProductionInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository.GetAll()
                              .Where(u => u.JournalType == JournalType.ItemReceiptProduction && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true && t.LockDate != null &&
                                      (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            if (autoSequence.CustomFormat == true)
            {
                input.ReceiptNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);
            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Clearance && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);


            //update item receipt 
            var @itemReceipt = await _itemReceiptManager.GetAsync(input.Id, true);

            if (@itemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            @itemReceipt.Update(tenantId, input.ReceiveFrom, null, input.SameAsShippingAddress,
                            input.ShippingAddress, input.BillingAddress, input.Total, input.ProductionProcessId);

            #region update receive from 
            // update if come from Prouction order 
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.ProductionOrder && input.ProductionOrderId != null)
            {
                if (@itemReceipt.ProductionOrderId != null && @itemReceipt.ProductionOrderId != input.ProductionOrderId)
                {
                    var productionOld = await _productionRepository.GetAll().Where(u => u.Id == @itemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                    CheckErrors(await _productionManager.UpdateAsync(productionOld));
                }

                var production = await _productionRepository.GetAll().Where(u => u.Id == input.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    production.UpdateShipedStatus(TransferStatus.ReceiveAll);
                }
                CheckErrors(await _productionManager.UpdateAsync(production));
                @itemReceipt.UpdateProductionOrderId(input.ProductionOrderId.Value);

            }
            else // in some case if user switch from receive from Prouction order to other so update Prouction of field item issue transfer order id to null
            {
                var production = await _productionRepository.GetAll().Where(u => u.Id == @itemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _productionManager.UpdateAsync(production));
                }
                @itemReceipt.UpdateProductionOrderId(null);
            }
            #endregion

            
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemReceiptManager.UpdateAsync(@itemReceipt));


            //Update Item Receipt Item and Journal Item
            var itemReceipItems = await _itemReceiptItemRepository.GetAll().Where(u => u.ItemReceiptId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Inventory && u.Identifier != null)).ToListAsync();

            var toDeleteItemReceiptItem = itemReceipItems
                .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems
                .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptItemId)).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptItemId)).ToList();
                if (deleteItemBatchNos.Any())
                {
                    var batchs = deleteItemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                    if (batchs.Any()) batchNos.AddRange(batchNos);

                    await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                }
            }

            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemReceipItem = itemReceipItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemReceipItem.UpdateLot(c.LotId);
                        CheckErrors(await _itemReceiptItemManager.UpdateAsync(itemReceipItem));

                    }
                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, c.Total, 0);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemReceiptItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            //in case of change date and item is auto generate it will change to use new batch so delete old one if not use
                            if (oldItemBatch.BatchNoId != newBatch.BatchNoId && !batchNos.Any(b => b.Id == oldItemBatch.BatchNoId)) batchNos.Add(oldItemBatch.BatchNo);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            var batchs = deleteItemBatchNos.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                            if (batchs.Any()) batchNos.AddRange(batchs);
                            await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        var batchs = oldItemBatchs.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                        if (batchs.Any()) batchNos.AddRange(batchs);

                        await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    if (c.FinishItemId != null)
                    {
                        @itemReceiptItem.UpdateFinishItemId(c.FinishItemId.Value);
                    }
                    itemReceiptItem.UpdateLot(c.LotId);
                    CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, c.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }

            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemReceiptItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            if (batchNos.Any())
            {
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_Delete,
                      AppPermissions.Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour)]
        public async Task Delete(CarlEntityDto input)
        {
            var @entity = await _productionManager.GetAsync(input.Id, true);


            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.ProductionOrder)
                  && t.IsLock == true && t.LockDate != null  && t.LockDate.Value.Date >= entity.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var userId = AbpSession.GetUserId();
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            // validate on if this transaction is already use in other table
            var itemIssueCount = _itemIssueRepository.GetAll().Where(v => v.TransferOrderId == entity.Id).Count();
            var itemReceiptCount = _itemReceiptRepository.GetAll().Where(v => v.TransferOrderId == entity.Id).Count();
            if ((itemIssueCount > 0 || itemReceiptCount > 0) && entity.ConvertToIssueAndReceipt == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (entity.ConvertToIssueAndReceipt == true)
            {
                //delete item receipt 
                var itemReceiptProductionId = _itemReceiptRepository.GetAll()
                       .Where(t => t.ProductionOrderId == input.Id && t.TransactionType == InventoryTransactionType.ItemReceiptProduction).Select(t => t.Id).FirstOrDefault();
                var inputItemReceipt = new CarlEntityDto() {IsConfirm = input.IsConfirm, Id = itemReceiptProductionId };
                await DeleteItemReceiptProduction(inputItemReceipt);

                //delete item issue
                var itemIssueProductionId = _itemIssueRepository.GetAll()
                    .Where(t => t.ProductionOrderId == input.Id && t.TransactionType == InventoryTransactionType.ItemIssueProduction)
                    .Select(t => t.Id).FirstOrDefault();
                var inputItemIssueProduction = new CarlEntityDto() {IsConfirm = input.IsConfirm, Id = itemIssueProductionId };
                await DeleteItemIssueProduct(inputItemIssueProduction);

            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ProductionOrder);

            if (entity.ProductionNo == auto.LastAutoSequenceNumber)
            {
                var pro = await _productionRepository.GetAll().Where(t => t.Id != entity.Id )
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (pro != null)
                {
                    auto.UpdateLastAutoSequenceNumber(pro.ProductionNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            //if (entity.Status == TransactionStatus.Publish)
            //{
            //    var draftInput = new UpdateStatus();
            //    draftInput.Id = input.Id;
            //    await UpdateStatusToDraft(draftInput);
            //}

            var standardGroups = await _productionStandardCostGroupRepository.GetAll().Where(s => s.ProductionId == entity.Id).ToListAsync();
            foreach(var g in standardGroups)
            {
                await _productionStandardCostGroupRepository.DeleteAsync(g);
            }

            var issueStandardGroups = await _productionIssueStandardCostGroupRepository.GetAll().Where(s => s.ProductionId == entity.Id).ToListAsync();
            foreach (var g in issueStandardGroups)
            {
                await _productionIssueStandardCostGroupRepository.DeleteAsync(g);
            }


            var rawMaterialItems = await _rawMaterialItemsRepository.GetAll().Where(u => u.ProductionId == entity.Id).ToListAsync();
            var finishItems = await _finishItemsRepository.GetAll().Where(u => u.ProductionId == entity.Id).ToListAsync();

            var batchNoRaw = new List<BatchNo>();
            //rawMaterialItems.Where(s => s.BatchNoId.HasValue)
            //                .GroupBy(s => s.BatchNoId)
            //                .Select(s => s.FirstOrDefault().BatchNo)
            //                .ToList();
            var batchNoFinish = new List<BatchNo>();
                                //finishItems.Where(s => s.BatchNoId.HasValue)
                                //            .GroupBy(s => s.BatchNoId)
                                //            .Select(s => s.FirstOrDefault().BatchNo)
                                //            .ToList();
            var batchNos = batchNoRaw.Concat(batchNoFinish).GroupBy(s => s.Id).Select(s => s.FirstOrDefault()).ToList();
         
            foreach (var f in finishItems)
            {
                CheckErrors(await _finishItemManager.RemoveAsync(f));
            }

            foreach (var s in rawMaterialItems)
            {
                CheckErrors(await _rawMaterialItemsManager.RemoveAsync(s));
            }

            var productionPlanId = entity.ProductionPlanId;

            CheckErrors(await _productionManager.RemoveAsync(@entity));

            if (batchNos.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }

            if (productionPlanId.HasValue)
            {
                var tenant = await GetCurrentTenantAsync();
                if (tenant.ProductionSummaryNetWeight || tenant.ProductionSummaryQty)
                {
                    await CurrentUnitOfWork.SaveChangesAsync();
                    await _productionPlanManager.CalculateByIdAsync(userId, new List<Guid> { productionPlanId.Value });
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ProductionOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,
                      AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemReceipt,
                      AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemIssue
                      )]
        public async Task<PagedResultDto<ProductionGetListOutput>> Find(GetListProductionInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var productionQuery = _productionRepository.GetAll().AsNoTracking()
                                 .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.FromLocationId) || userGroups.Contains(t.ToLocationId))
                                 .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                                 .WhereIf(input.DeliveryStatus != null && input.DeliveryStatus.Count > 0, u => input.DeliveryStatus.Contains(u.ShipedStatus))
                                 .WhereIf(!input.Filter.IsNullOrEmpty(),
                                     p => p.ProductionNo.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.Reference.ToLower().Contains(input.Filter.ToLower()))
                                 .AsNoTracking()
                                 .Select(p => new
                                 {
                                     ProductionOrderId = p.Id,
                                     ProductionOrderNo = p.ProductionNo,
                                     ProductionOrderShipedStatus = p.ShipedStatus,
                                     ProductionOrderDate = p.Date,
                                     ProductionOrderStatus = p.Status,
                                 });

            var rawQuery = _rawMaterialItemsRepository.GetAll()
                           .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                           .AsNoTracking()
                           .Select(s => s.ProductionId);

            var finQuery = _finishItemsRepository.GetAll()
                           .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.Id))
                           .AsNoTracking()
                           .Select(s => s.ProductionId);

            var query = from p in productionQuery
                        join r in rawQuery
                        on p.ProductionOrderId equals r
                        into rs
                        join f in finQuery
                        on p.ProductionOrderId equals f
                        into fs
                        where rs.Count() > 0 || fs.Count() > 0
                        select new ProductionGetListOutput
                        {
                            Id = p.ProductionOrderId,
                            ProductionNo = p.ProductionOrderNo,
                            ReceiveStatus = p.ProductionOrderShipedStatus,
                            ProductionDate = p.ProductionOrderDate,
                            StatusCode = p.ProductionOrderStatus,
                            CountItem = rs.Count() + fs.Count()
                        };


            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ProductionGetListOutput>(0, new List<ProductionGetListOutput>());

            if (input.IsForItemIssue)
            {
                query = from q in query
                        join tr in _itemIssueRepository.GetAll()
                                    .Where(i => i.ProductionOrderId != null).AsNoTracking()
                                    .Select(i => i.ProductionOrderId)
                        on q.Id equals tr into p
                        where p.Count() == 0
                        select q;

            }
            else if (input.IsForItemReceipt)
            {
                query = from q in query
                        join tr in _itemReceiptRepository.GetAll()
                                .Where(i => i.ProductionOrderId != null).AsNoTracking()
                                .Select(i => i.ProductionOrderId)
                        on q.Id equals tr into p
                        where p.Count() == 0
                        select q;
            }
            
            var @entities = await query.OrderBy(s => s.ProductionDate).PageBy(input).ToListAsync();
            return new PagedResultDto<ProductionGetListOutput>(resultCount, @entities);
        }

        
        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_GetDetail)]
        public async Task<ProductionDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @entity = await _productionManager.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var locations = new List<long?>() {
                entity.FromLocationId
            };

            //var averageCosts = await _inventoryManager.GetAvgCostQuery(entity.IssueDate != null ? entity.IssueDate.Value : entity.Date, locations).ToListAsync();


            var rawMaterialItems = await (from t in _rawMaterialItemsRepository.GetAll()
                                                .Include(u => u.Item)
                                                .Include(u => u.FromLot)
                                                .Where(u => u.ProductionId == entity.Id)
                                                .AsNoTracking()
                                          join i in _inventoryTransactionItemRepository.GetAll()
                                                  .Where(s => s.JournalType == JournalType.ItemIssueProduction)
                                                  .Where(s => s.TransferOrProductionItemId.HasValue && s.TransferOrProductionId.HasValue)
                                                  .Where(s => s.TransferOrProductionId == entity.Id)
                                                  .AsNoTracking()
                                          on t.Id equals i.TransferOrProductionItemId.Value
                                          into cs
                                          from c in cs.DefaultIfEmpty()

                                          select new CreateOrUpdateRawMaterialItemsInput
                                          {
                                              FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.FromLot),
                                              FromLotId = t.FromLotId,
                                              Description = t.Description,
                                              Id = t.Id,
                                              Item = new ItemSummaryDetailOutput
                                              {
                                                  Id = t.Item.Id,
                                                  ItemCode = t.Item.ItemCode,
                                                  ItemName = t.Item.ItemName,
                                                  InventoryAccountId = t.Item.InventoryAccountId,
                                                  InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(t.Item.InventoryAccount),
                                                  //AverageCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                                                  //QtyOnHand = averageCosts.Where(v => v.Id == t.ItemId).Select(v => v.QtyOnHand).FirstOrDefault(),
                                              },
                                              Qty = t.Qty,
                                              InventoryAccountId = t.Item.InventoryAccountId.Value,
                                              UnitCost = c != null ? c.UnitCost : t.UnitCost,
                                              Total = c != null ? Math.Abs(c.LineCost) : t.Total,
                                              RawMaterialId = t.Id,
                                              ItemId = t.ItemId,
                                              Unit = t.Qty,
                                              CreateTime = t.CreationTime,
                                              UseBatchNo = t.Item.UseBatchNo,
                                              TrackSerial = t.Item.TrackSerial,
                                              TrackExpiration = t.Item.TrackExpiration
                                          })
                                        .OrderBy(t => t.CreateTime)
                                        .ToListAsync();

            var finishItems = await _finishItemsRepository.GetAll()
                                   .Include(u => u.Item)
                                   .Include(u=>u.ToLot)
                                   .Where(u => u.ProductionId == entity.Id)
                                   .AsNoTracking()
                                   .Select(t => new CreateOrUpdateFinishItemInput
                                   {
                                       ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.ToLot),
                                       ToLotId = t.ToLotId,
                                       Description = t.Description,
                                       Id = t.Id,
                                       Item = new ItemSummaryDetailOutput
                                       {
                                           Id = t.Item.Id,
                                           ItemCode = t.Item.ItemCode,
                                           InventoryAccountId = t.Item.InventoryAccountId,
                                           InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(t.Item.InventoryAccount),
                                           ItemName = t.Item.ItemName,
                                           //AverageCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                                           //QtyOnHand = averageCosts.Where(v => v.Id == t.ItemId).Select(v => v.QtyOnHand).FirstOrDefault(),
                                       },
                                       InventoryAccount = t.Item.InventoryAccountId.Value,
                                       Total = t.Total,
                                       UnitCost = t.UnitCost,
                                       ItemId = t.ItemId,
                                       Unit = t.Qty,
                                       CreateTime = t.CreationTime,
                                       UseBatchNo = t.Item.UseBatchNo,
                                       AutoBatchNo = t.Item.AutoBatchNo,
                                       TrackSerial = t.Item.TrackSerial,
                                       TrackExpiration = t.Item.TrackExpiration
                                   })
                                   .OrderBy(t => t.CreateTime)
                                   .ToListAsync();


            var issueBatchDic = await _itemIssueItemBatchNoRepository.GetAll()
                               .AsNoTracking()
                               .Where(s => s.ItemIssueItem.RawMaterialItemId.HasValue && s.ItemIssueItem.RawMaterialItem.ProductionId == input.Id)
                               .Select(s => new BatchNoItemOutput
                               {
                                   Id = s.Id,
                                   BatchNoId = s.BatchNoId,
                                   BatchNumber = s.BatchNo.Code,
                                   ExpirationDate = s.BatchNo.ExpirationDate,
                                   Qty = s.Qty,
                                   TransactionItemId = s.ItemIssueItem.RawMaterialItemId.Value
                               })
                               .GroupBy(s => s.TransactionItemId)
                               .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (issueBatchDic.Any())
            {
                foreach (var i in rawMaterialItems)
                {
                    if (issueBatchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = issueBatchDic[i.Id.Value];
                }
            }

            var receiptBatchDic = await _itemReceiptItemBatchNoRepository.GetAll()
                             .AsNoTracking()
                             .Where(s => s.ItemReceiptItem.FinishItemId.HasValue && s.ItemReceiptItem.FinishItems.ProductionId == input.Id)
                             .Select(s => new BatchNoItemOutput
                             {
                                 Id = s.Id,
                                 BatchNoId = s.BatchNoId,
                                 BatchNumber = s.BatchNo.Code,
                                 ExpirationDate = s.BatchNo.ExpirationDate,
                                 Qty = s.Qty,
                                 TransactionItemId = s.ItemReceiptItem.FinishItemId.Value
                             })
                             .GroupBy(s => s.TransactionItemId)
                             .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (receiptBatchDic.Any())
            {
                foreach (var i in finishItems)
                {
                    if (receiptBatchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = receiptBatchDic[i.Id.Value];
                }
            }

            var itemIssue = await _itemIssueRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.ProductionOrderId.HasValue && s.ProductionOrderId == input.Id);
            var itemReceipt = await _itemReceiptRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.ProductionOrderId.HasValue && s.ProductionOrderId == input.Id);


            var result = ObjectMapper.Map<ProductionDetailOutput>(@entity);
            result.ItemIssueDate = entity.IssueDate;
            result.ItemReceiptDate = entity.ReceiptDate;
            result.ProductionPlanNo = entity.ProductionPlan?.DocumentNo;
            result.RawMaterialItems = rawMaterialItems;
            result.FinishItems = finishItems;

            if (itemIssue != null) result.ItemIssueId = itemIssue.Id;
            if (itemReceipt != null) result.ItemReceiptId = itemReceipt.Id;

            //get total from inventory transaction item cache table
            result.SubTotalRawProduction = rawMaterialItems.Sum(s => s.Total);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_GetList)]
        public async Task<PageResultProductioinSummary> GetList(GetListProductionInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var productionQuery = _productionRepository.GetAll()
                                .WhereIf(input.CalculationStates != null && input.CalculationStates.Any(), s => input.CalculationStates.Contains(s.CalculationState))
                                .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.FromLocationId) || userGroups.Contains(t.ToLocationId))
                                .WhereIf(input.FromDate != null && input.ToDate != null,
                                           (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                .WhereIf(input.DeliveryStatus != null && input.DeliveryStatus.Count > 0, u => input.DeliveryStatus.Contains(u.ShipedStatus))
                                .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.ToLocationId) || input.Locations.Contains(u.FromLocationId))
                                .WhereIf(input.NoProductionPlan, s => !s.ProductionPlanId.HasValue)
                                .WhereIf(input.ProductionPlans != null && input.ProductionPlans.Any() && !input.NoProductionPlan, s => input.ProductionPlans.Contains(s.ProductionPlanId.Value))
                                .WhereIf(input.ProductionPlanStatus != null && input.ProductionPlanStatus.Any() && !input.NoProductionPlan, s => s.ProductionPlanId.HasValue && input.ProductionPlanStatus.Contains(s.ProductionPlan.Status))
                                .WhereIf(input.ProductionLines != null && input.ProductionLines.Any() && !input.NoProductionPlan, s => s.ProductionPlanId.HasValue && input.ProductionLines.Contains(s.ProductionPlan.ProductionLineId.Value))
                                .WhereIf(input.ProductionProcess != null && input.ProductionProcess.Any(), s => s.ProductionProcessId.HasValue && input.ProductionProcess.Contains(s.ProductionProcessId.Value))
                                .WhereIf(input.ProductionProcessTypes != null && input.ProductionProcessTypes.Any(), s => s.ProductionProcessId.HasValue && input.ProductionProcessTypes.Contains(s.ProductionProcess.ProductionProcessType))
                                .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    p => p.ProductionNo.ToLower().Contains(input.Filter.ToLower()) ||
                                         p.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                         p.Memo.ToLower().Contains(input.Filter.ToLower()))
                                .AsNoTracking()
                                .Select(s => new
                                {
                                    ProductionOrderId = s.Id,
                                    ProductionOrderNo = s.ProductionNo,
                                    s.Reference,
                                    ProductionOrderShipedStatus = s.ShipedStatus,
                                    ProductionOrderDate = s.Date,
                                    ProductionOrderStatus = s.Status,
                                    FromLocationId = s.FromLocationId,
                                    ToLocationId = s.ToLocationId,
                                    CreatorId = s.CreatorUser.Id,
                                    ShipedStatus = s.ShipedStatus,
                                    ConvertToIssueAndReceiptTransfer = s.ConvertToIssueAndReceipt,
                                    ProductionProcessId = s.ProductionProcessId,
                                    s.CalculationState,
                                    ProductionPlanNo = s.ProductionPlanId.HasValue ? s.ProductionPlan.DocumentNo : "",
                                    s.TotalIssueQty,
                                    s.TotalReceiptQty,
                                    s.TotalIssueNetWeight,
                                    s.TotalReceiptNetWeight
                                });

            var productionProcessQuery = _productionProcessRepository.GetAll()
                                         .AsNoTracking()
                                         .Select(s => new ProductionProcessSummaryDto
                                         {
                                             Id = s.Id,
                                             ProcessName = s.ProcessName
                                         });

            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, new List<long>());

            var pQuery = from p in productionQuery
                         join pc in productionProcessQuery
                         on p.ProductionProcessId equals pc.Id
                         join u in userQuery
                         on p.CreatorId equals u.Id
                         join fl in locationQuery
                         on p.FromLocationId equals fl.Id
                         join tl in locationQuery
                         on p.ToLocationId equals tl.Id
                         select new
                         {
                             ProductionOrderId = p.ProductionOrderId,
                             ProductionOrderNo = p.ProductionOrderNo,
                             p.Reference,
                             p.ProductionPlanNo,
                             ProductionOrderShipedStatus = p.ProductionOrderShipedStatus,
                             ProductionOrderDate = p.ProductionOrderDate,
                             ProductionOrderStatus = p.ProductionOrderStatus,
                             FromLocationId = p.FromLocationId,
                             ToLocationId = p.ToLocationId,
                             CreatorId = p.CreatorId,
                             ShipedStatus = p.ShipedStatus,
                             ConvertToIssueAndReceiptTransfer = p.ConvertToIssueAndReceiptTransfer,
                             ProductionProcessId = p.ProductionProcessId,
                             ProductionProcessName = pc.ProcessName,
                             UserName = u.UserName,
                             FromLocationName = fl.LocationName,
                             ToLocationName = tl.LocationName,
                             p.CalculationState,
                             p.TotalIssueQty,
                             p.TotalReceiptQty,
                             p.TotalIssueNetWeight,
                             p.TotalReceiptNetWeight
                         };


            var rawItemQuery = _rawMaterialItemsRepository.GetAll()
                               .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                               .AsNoTracking()
                               .Select(s => s.ProductionId);

            var finItemQuery = _finishItemsRepository.GetAll()
                               .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                               .AsNoTracking()
                               .Select(s => s.ProductionId);


            var standardGroupQuery = _productionStandardCostGroupRepository.GetAll().AsNoTracking()
                                      .Select(s => new StandardGroupSummary
                                      {
                                          ProductionId = s.ProductionId,
                                          GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                          TotalNetWeight = s.TotalNetWeight,
                                          TotalQty = s.TotalQty,
                                      });
            var issueStandardGroupQuery = _productionIssueStandardCostGroupRepository.GetAll().AsNoTracking()
                                     .Select(s => new StandardGroupSummary
                                     {
                                         ProductionId = s.ProductionId,
                                         GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                         TotalNetWeight = s.TotalNetWeight,
                                         TotalQty = s.TotalQty,
                                     });

            var query = from p in pQuery
                        join r in rawItemQuery
                        on p.ProductionOrderId equals r
                        into rs
                        join f in finItemQuery
                        on p.ProductionOrderId equals f
                        into fs
                        join g in standardGroupQuery
                        on p.ProductionOrderId equals g.ProductionId
                        into gs
                        join ig in issueStandardGroupQuery
                        on p.ProductionOrderId equals ig.ProductionId
                        into igs

                        where rs.Count() > 0 || fs.Count() > 0

                        select new ProductionGetListOutput
                        {
                            Id = p.ProductionOrderId,
                            ProductionNo = p.ProductionOrderNo,
                            Reference = p.Reference,
                            //CountItem = rs.Count() + fs.Count(),
                            ReceiveStatus = p.ProductionOrderShipedStatus,
                            ProductionDate = p.ProductionOrderDate,
                            StatusCode = p.ProductionOrderStatus,
                            FromLocation = new LocationSummaryOutput
                            {
                                Id = p.FromLocationId,
                                LocationName = p.FromLocationName
                            },
                            ToLocation = new LocationSummaryOutput
                            {
                                Id = p.ToLocationId,
                                LocationName = p.ToLocationName
                            },
                            User = new UserDto
                            {
                                Id = p.CreatorId,
                                UserName = p.UserName
                            },
                            ProductionProcess = new ProductionProcessSummaryOutput
                            {
                                ProcessName = p.ProductionProcessName,
                                Id = p.ProductionProcessId != null ? p.ProductionProcessId.Value : 0
                            },
                            CalculationState = p.CalculationState,
                            ProductionPlanNo = p.ProductionPlanNo,
                            TotalIssueQty = p.TotalIssueQty,
                            TotalReceiptQty = p.TotalReceiptQty,
                            TotalIssueNetWeight = p.TotalIssueNetWeight,
                            TotalReceiptNetWeight = p.TotalReceiptNetWeight,
                            StandardGroups = gs.OrderBy(g => g.GroupName).ToList(),
                            IssueStandardGroups = igs.OrderBy(g => g.GroupName).ToList(),
                        };
                        

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PageResultProductioinSummary(resultCount, new List<ProductionGetListOutput>(), new List<ProductionPlanSummary>(), new List<ProductionPlanSummary>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("productiondate"))
                {
                    query = query.OrderByDescending(s => s.ProductionDate);
                }
                else if (input.Sorting.ToLower().StartsWith("productionno"))
                {
                    query = query.OrderByDescending(s => s.ProductionNo);
                }
                else if (input.Sorting.ToLower().StartsWith("fromlocation"))
                {
                    query = query.OrderByDescending(s => s.FromLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("tolocation"))
                {
                    query = query.OrderByDescending(s => s.ToLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderByDescending(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderByDescending(s => s.StatusCode);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("productiondate"))
                {
                    query = query.OrderBy(s => s.ProductionDate);
                }
                else if (input.Sorting.ToLower().StartsWith("productionno"))
                {
                    query = query.OrderBy(s => s.ProductionNo);
                }
                else if (input.Sorting.ToLower().StartsWith("fromlocation"))
                {
                    query = query.OrderBy(s => s.FromLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("tolocation"))
                {
                    query = query.OrderBy(s => s.ToLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderBy(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderBy(s => s.StatusCode);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }

            var @entities = await query.PageBy(input).ToListAsync();

            decimal totalIssueQty = 0;
            decimal totalReceiptQty = 0;
            decimal totalQtyBalance = 0;
            decimal totalIssueNetWeight = 0;
            decimal totalReceiptNetWeight = 0;
            decimal totalNetWeightBalance = 0;
            decimal qtyYield = 0;
            decimal netWeightYield = 0;
            var summaries = new List<ProductionPlanSummary>();
            var issueSummaries = new List<ProductionPlanSummary>();

            var tenant = await GetCurrentTenantAsync();

            if(tenant.ProductionSummaryQty || tenant.ProductionSummaryNetWeight)
            {
                var summaryList = await query.Select(s => new {
                    s.TotalIssueQty,
                    s.TotalReceiptQty,
                    s.TotalIssueNetWeight,
                    s.TotalReceiptNetWeight,
                    s.StandardGroups,
                    s.IssueStandardGroups
                })
                .ToListAsync();

                totalIssueQty = summaryList.Sum(t => t.TotalIssueQty);
                totalReceiptQty = summaryList.Sum(t => t.TotalReceiptQty);
                totalQtyBalance = summaryList.Sum(t => t.TotalIssueQty - t.TotalReceiptQty);
                totalIssueNetWeight = summaryList.Sum(t => t.TotalIssueNetWeight);
                totalReceiptNetWeight = summaryList.Sum(t => t.TotalReceiptNetWeight);
                totalNetWeightBalance = summaryList.Sum(t => t.TotalIssueNetWeight - t.TotalReceiptNetWeight);
                qtyYield = totalIssueQty == 0 ? 0 : Math.Round(totalReceiptQty / totalIssueQty, 4) * 100;
                netWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(totalReceiptNetWeight / totalIssueNetWeight, 4) * 100;

                summaries = summaryList
                    .SelectMany(s => s.StandardGroups)
                    .OrderBy(s => s.GroupName)
                    .GroupBy(s => s.GroupName)
                    .Select(s => new ProductionPlanSummary
                    {
                        ProductionPlan = s.Key,
                        TotalReceiptQty = s.Sum(t => t.TotalQty),
                        TotalReceiptNetWeight = s.Sum(t => t.TotalNetWeight),
                        QtyYield = totalIssueQty == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssueQty, 4) * 100,
                        NetWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssueNetWeight, 4) * 100,
                    })
                    .ToList();

                issueSummaries = summaryList
                    .SelectMany(s => s.IssueStandardGroups)
                    .OrderBy(s => s.GroupName)
                    .GroupBy(s => s.GroupName)
                    .Select(s => new ProductionPlanSummary
                    {
                        ProductionPlan = s.Key,
                        TotalIssueQty = s.Sum(t => t.TotalQty),
                        TotalIssueNetWeight = s.Sum(t => t.TotalNetWeight),
                        QtyYield = totalIssueQty == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssueQty, 4) * 100,
                        NetWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssueNetWeight, 4) * 100,
                    })
                    .ToList();
            }

            return new PageResultProductioinSummary { 
                TotalCount = resultCount,
                Items = entities,
                Summaries = summaries,
                IssueSummaries = issueSummaries,
                TotalIssueQty = totalIssueQty,
                TotalReceiptQty = totalReceiptQty,
                TotalQtyBalance = totalQtyBalance,
                TotalIssueNetWeight = totalIssueNetWeight,
                TotalReceiptNetWeight = totalReceiptNetWeight,
                TotalNetWeightBalance = totalNetWeightBalance,
                QtyYield = qtyYield,
                NetWeightYield = netWeightYield
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemIssue)]
        public async Task<ProductionDetailOutput> GetListProductionOrderForItemIssue(EntityDto<Guid> input)
        {
            var @entity = await _productionManager.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var locations = new List<long?>() {
                    entity.FromLocationId
                };

           
            var raw = await _rawMaterialItemsRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include(u=>u.FromLot)
                                    .Where(u => u.ProductionId == entity.Id)
                                    .Select(t => new CreateOrUpdateRawMaterialItemsInput
                                    {
                                        FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.FromLot),
                                        FromLotId = t.FromLotId,
                                        Description = t.Description,
                                        Id = t.Id,
                                        Item = _itemRepository.GetAll().Where(r => r.Id == t.ItemId)
                                            .Select(i => new ItemSummaryDetailOutput
                                            {
                                                Id = i.Id,
                                                InventoryAccountId = i.InventoryAccountId,
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.InventoryAccount),
                                                ItemCode = i.ItemCode,
                                                ItemName = i.ItemName,
                                                //AverageCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                                                //QtyOnHand = averageCosts.Where(v => v.Id == t.ItemId).Select(v => v.QtyOnHand).FirstOrDefault(),
                                            }).FirstOrDefault(),
                                        ItemId = t.ItemId,
                                        Unit = t.Qty,
                                        CreateTime = t.CreationTime,
                                        UseBatchNo = t.Item.UseBatchNo,
                                        TrackSerial = t.Item.TrackSerial,
                                        TrackExpiration = t.Item.TrackExpiration
                                    })
                                    .OrderBy(t => t.CreateTime)
                                    .ToListAsync();
            var result = ObjectMapper.Map<ProductionDetailOutput>(@entity);
            result.RawMaterialItems = raw;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemReceipt)]
        public async Task<ProductionDetailOutput> GetListProductionOrderForItemReceipt(EntityDto<Guid> input)
        {
            var @entity = await _productionManager.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var locations = new List<long?>() {
                    entity.FromLocationId
                };
            
            var finishOrderItem = await _finishItemsRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include (u=>u.ToLot)
                                    .Where(u => u.ProductionId == entity.Id)
                                    .Select(t => new CreateOrUpdateFinishItemInput
                                    {
                                        ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.ToLot),
                                        ToLotId = t.ToLotId,
                                        Description = t.Description,
                                        Id = t.Id,
                                        Item = _itemRepository.GetAll().Where(r => r.Id == t.ItemId)
                                            .Select(i => new ItemSummaryDetailOutput
                                            {
                                                Id = i.Id,
                                                InventoryAccountId = i.InventoryAccountId,
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.InventoryAccount),
                                                ItemCode = i.ItemCode,
                                                ItemName = i.ItemName,
                                                //AverageCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                                                //QtyOnHand = averageCosts.Where(v => v.Id == t.ItemId).Select(v => v.QtyOnHand).FirstOrDefault(),
                                            }).FirstOrDefault(),
                                        ItemId = t.ItemId,
                                        Unit = t.Qty,
                                        CreateTime = t.CreationTime,
                                        UnitCost = t.UnitCost,
                                        Total = t.Total,
                                        UseBatchNo = t.Item.UseBatchNo,
                                        AutoBatchNo = t.Item.AutoBatchNo,
                                        TrackSerial = t.Item.TrackSerial,
                                        TrackExpiration = t.Item.TrackExpiration
                                    })
                                    .OrderBy(t => t.CreateTime)
                                    .ToListAsync();
            var result = ObjectMapper.Map<ProductionDetailOutput>(@entity);
            result.FinishItems = finishOrderItem;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_Update,
                      AppPermissions.Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateProductionInput input)
        {

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true && t.LockDate != null &&
                                     (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ProductionOrder)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateItemLots(input);
            await ValidateProductionPlan(input);
            await ValidateNetWeight(input);
            if (input.ConvertToIssueAndReceipt)
            {
                await ValidateBatchNo(input); //for issue
                await ValidateAddBatchNo(input); //for receipt
            }

            await CalculateTotal(input);
            
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            
            var @entity = await _productionManager.GetAsync(input.Id, true); //this is vendor
            await CheckClosePeriod(entity.Date, input.Date);
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }

            var oldConvert = entity.ConvertToIssueAndReceipt;
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            // validate on if this transaction is already use in other table
            var itemIssueCount = _itemIssueRepository.GetAll().Where(v => v.TransferOrderId == entity.Id).Count();
            var itemReceiptCount = _itemReceiptRepository.GetAll().Where(v => v.TransferOrderId == entity.Id).Count();
            if ((itemIssueCount > 0 || itemReceiptCount > 0) && entity.ConvertToIssueAndReceipt == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }

            var productionPlanIds = new List<Guid>();
            if (entity.ProductionPlanId.HasValue) productionPlanIds.Add(entity.ProductionPlanId.Value);
            if (input.ProductionPlanId.HasValue && input.ProductionPlanId != entity.ProductionPlanId) productionPlanIds.Add(input.ProductionPlanId.Value);

            entity.Update(userId, input.FromLocationId, input.ToLocationId,
                input.FromClassId, input.ToClassId, input.Status, 
                input.ProductionNo, input.Date,
                input.Reference, input.Memo, input.ConvertToIssueAndReceipt,
                input.ItemReceiptDate, input.ItemIssueDate,
                input.SubTotalRawProduction, input.SubTotalFinishProduction,
                input.ProductionAccountId, input.ProductionProcessId, input.ProductionPlanId,
                input.TotalIssueQty, input.TotalReceiptQty,
                input.TotalIssueNetWeight, input.TotalReceiptNetWeight);
            #region update raw           
            var rawItem = await _rawMaterialItemsRepository.GetAll().Where(u => u.ProductionId == entity.Id).ToListAsync();
            var finishItems = await _finishItemsRepository.GetAll().Where(u => u.ProductionId == input.Id).ToListAsync();
            int indexFromlot = 0;
            foreach (var p in input.RawMaterialItems)
            {
                indexFromlot++;
                if (p.FromLotId == 0 && p.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexFromlot.ToString());
                }

                if (p.Id != null)
                {
                    var product = rawItem.FirstOrDefault(u => u.Id == p.Id);
                    if (product != null)
                    {
                        //here is in only same TransferOrder so no need to update TransferOrder
                        product.Update(userId, p.ItemId, p.Description, p.Unit);
                        product.UpdateFromLot(p.FromLotId.Value);
                        CheckErrors(await _rawMaterialItemsManager.UpdateAsync(product));
                    }
                }
                else if (p.Id == null)
                {
                    //@entity.Id is TransferId or input.Id is also TransferOrder Id so no need to pass TransferOrderId from outside
                    var rawItemCreate = RawMaterialItems.Create(tenantId, userId, entity.Id, p.ItemId, p.Description, p.Unit, p.UnitCost, p.Total);
                    if (input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
                    {
                        p.Id = rawItemCreate.Id;
                    }
                    rawItemCreate.UpdateFromLot(p.FromLotId.Value);
                    base.CheckErrors(await _rawMaterialItemsManager.CreateAsync(rawItemCreate));

                }
            }
            var toDeleterawItem = rawItem.Where(u => !input.RawMaterialItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            #endregion

            #region update finish           
           
            int indexTolot = 0;
            foreach (var p in input.FinishItems)
            {
                indexTolot++;
                if (p.ToLotId == 0 && p.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexTolot.ToString());
                }

                if (p.Id != null)
                {
                    var finish = finishItems.FirstOrDefault(u => u.Id == p.Id);
                    if (finish != null)
                    {
                        //here is in only same TransferOrder so no need to update TransferOrder
                        finish.Update(userId, p.ItemId, p.Description, p.Unit, p.UnitCost, p.Total);
                        finish.UpdateToLot(p.ToLotId);
                        CheckErrors(await _finishItemManager.UpdateAsync(finish));
                    }
                }
                else if (p.Id == null)
                {
                    //@entity.Id is TransferId or input.Id is also TransferOrder Id so no need to pass TransferOrderId from outside
                    var finishItem = FinishItems.Create(tenantId, userId, entity.Id, p.ItemId, p.Description, p.Unit, p.UnitCost, p.Total);

                    if (input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
                    {
                        p.Id = finishItem.Id;
                    }
                    finishItem.UpdateToLot(p.ToLotId);
                    base.CheckErrors(await _finishItemManager.CreateAsync(finishItem));

                }
            }

            #endregion

            
            CheckErrors(await _productionManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            /*********************************
            * Update Item Issue Or Receipt
            * *******************************/
            if (oldConvert == true && input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
            {
                var tenant = await GetCurrentTenantAsync();
                //if (tenant.RawProductionAccountId == null)
                //{
                //    throw new UserFriendlyException(L("PleaseSetRawProductionAccountOnCompanyProfile"));
                //}
                //if (tenant.FinishProductionAccountId == null)
                //{
                //    throw new UserFriendlyException(L("PleaseSetFinishProductionAccountOnCompanyProfile"));
                //}

                var ItemIssueId = await _itemIssueRepository.GetAll()
                                   .Where(t => t.ProductionOrderId == input.Id)
                                   .Select(t => t.Id).FirstOrDefaultAsync();

                var ItemReceiptId = await _itemReceiptRepository.GetAll()
                                   .Where(t => t.ProductionOrderId == input.Id)
                                   .Select(t => t.Id).FirstOrDefaultAsync();
                if (ItemIssueId == null || ItemIssueId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("RecordItemIssueNotFound"));
                }


                var keyId = input.RawMaterialItems.Where(t => t.Id != null).Select(t => t.Id).ToList();
                var itemIssueItems = await _itemIssueItemRepository.GetAll()
                                 .Where(t => keyId.Contains(t.RawMaterialItemId) && t.ItemIssueId == ItemIssueId)
                                 .ToDictionaryAsync(t => t.RawMaterialItemId, t => t.Id);


                var createInputItemIssueProduction = new UpdateItemIssueProductInput()
                {
                    IsConfirm = input.IsConfirm,
                    Id = ItemIssueId,
                    ItemReceiptId = ItemReceiptId,
                    Memo = input.Memo,
                    IssueNo = input.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.FromClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.ItemIssueDate.Value,
                    LocationId = input.FromLocationId,
                    ReceiveFrom = ReceiveFrom.ProductionOrder,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = input.ProductionAccountId,
                    ProductionId = entity.Id,
                    ProductionProcessId = input.ProductionProcessId,
                    Total = input.SubTotalRawProduction,
                    Items = input.RawMaterialItems.Select(t => new CreateOrUpdateRawMaterialItemsInput
                    {
                        FromLotId = t.FromLotId,
                        Id = itemIssueItems.ContainsKey(t.Id) ? itemIssueItems[t.Id] : (Guid?)null,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = t.Item.InventoryAccountId.Value,
                        Description = t.Description,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        RawMaterialId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList(),

                };
                await UpdateItemIssueProduct(createInputItemIssueProduction);

               

               
                var itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                                 .Where(t=> t.ItemReceiptId == ItemReceiptId)
                                 .ToDictionaryAsync(t => t.FinishItemId, t => t.Id);
                if (ItemReceiptId == null || ItemReceiptId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("RecordItemReceiptNotFound"));
                }
                var createInputItemRecepitProduction = new UpdateItemReceiptProductionInput()
                {
                    IsConfirm  = input.IsConfirm,
                    Id = ItemReceiptId,
                    Memo = input.Memo,
                    ReceiptNo = input.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.ToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.ItemIssueDate.Value,
                    LocationId = input.ToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.ProductionOrder,
                    Reference = input.Reference,
                    ProductionProcessId = input.ProductionProcessId,
                    Status = input.Status,
                    ClearanceAccountId = input.ProductionAccountId,
                    Total = input.SubTotalFinishProduction,
                    ProductionOrderId = entity.Id,
                    Items = input.FinishItems.Select(t => new CreateOrUpdateItemReceiptItemProductionInput
                    {
                        Id = itemReceiptItems.ContainsKey(t.Id) ? itemReceiptItems[t.Id] : (Guid?)null,
                        LotId = t.ToLotId,
                        InventoryAccountId = t.Item.InventoryAccountId.Value, 
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        Description = t.Description,
                        FinishItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList()

                };
                await UpdateItemReceiptProduct(createInputItemRecepitProduction);
            }

            /*********************************
             * Delete Item Issue Or Receipt
             * *******************************/
            if (oldConvert == true && input.ConvertToIssueAndReceipt == false && input.Status == TransactionStatus.Publish)
            {
                var ItemIssueId = await _itemIssueRepository.GetAll()
                    .Where(t => t.ProductionOrderId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();
                var ItemReceiptId = await _itemReceiptRepository.GetAll()
                    .Where(t => t.ProductionOrderId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();
                var IssueId = new CarlEntityDto() {IsConfirm = input.IsConfirm, Id = ItemIssueId };
                var ReceiptId = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = ItemReceiptId };
                await DeleteItemReceiptProduction(ReceiptId);
                await DeleteItemIssueProduct(IssueId);
                entity.UpdateShipedStatus(TransferStatus.Pending);
            }
            
            /*********************************
            * Create Item Issue Or Receipt
            * *******************************/
            if (oldConvert == false && input.ConvertToIssueAndReceipt == true && input.Status == TransactionStatus.Publish)
            {
                var tenant = await GetCurrentTenantAsync();
                
                var createInputItemIssueProduction = new CreateItemIssueProductInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    IssueNo = input.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.FromClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.ItemIssueDate.Value,
                    LocationId = input.FromLocationId,
                    ReceiveFrom = ReceiveFrom.ProductionOrder,
                    Reference = input.Reference,
                    ProductionProcessId = input.ProductionProcessId,
                    Status = input.Status,
                    ClearanceAccountId = input.ProductionAccountId,
                    ProductionId = entity.Id,
                    Total = input.SubTotalRawProduction,
                    Items = input.RawMaterialItems.Select(t => new CreateOrUpdateRawMaterialItemsInput
                    {
                        FromLotId = t.FromLotId,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = t.Item.InventoryAccountId.Value,
                        Description = t.Description,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        RawMaterialId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList(),

                };
                await CreateItemIssueProduction(createInputItemIssueProduction);

                var createInputItemRecepitProduction = new CreateItemReceiptProductionInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.ToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.ItemIssueDate.Value,
                    ProductionProcessId = input.ProductionProcessId,
                    LocationId = input.ToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.ProductionOrder,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = input.ProductionAccountId,
                    Total = input.SubTotalFinishProduction,
                    ProductionOrderId = entity.Id,
                    Items = input.FinishItems.Select(t => new CreateOrUpdateItemReceiptItemProductionInput
                    {
                        LotId = t.ToLotId,
                        InventoryAccountId = t.Item.InventoryAccountId.Value, 
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        Description = t.Description,
                        FinishItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList()

                };
                await CreateItemReceiptProduct(createInputItemRecepitProduction);
                entity.UpdateShipedStatus(TransferStatus.ReceiveAll);
            }

            var toUpdaterawItem = rawItem.Where(u => !input.RawMaterialItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            foreach (var t in toDeleterawItem)
            {
                CheckErrors(await _rawMaterialItemsManager.RemoveAsync(t));
            }

            var toDeletefinishItem = finishItems.Where(u => !input.FinishItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeletefinishItem)
            {
                CheckErrors(await _finishItemManager.RemoveAsync(t));
            }

            var standardGroups = await _productionStandardCostGroupRepository.GetAll().Where(s => s.ProductionId == entity.Id).ToListAsync();

            var addGroups = input.StandardGroups?.Where(s => !standardGroups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
            var editGroups = input.StandardGroups?.Where(s => standardGroups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
            var deleteGroups = standardGroups.WhereIf(!input.StandardGroups.IsNullOrEmpty(), s => !input.StandardGroups.Any(r => r.GroupId == s.StandardCostGroupId)).ToList();

            if (!addGroups.IsNullOrEmpty())
            {
                foreach (var g in addGroups)
                {
                    var standardGroup = ProductionStandardCostGroup.Create(tenantId, userId, entity.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                    await _productionStandardCostGroupRepository.InsertAsync(standardGroup);
                }
            }

            if (!editGroups.IsNullOrEmpty())
            {
                foreach (var g in editGroups)
                {
                    var standardGroup = standardGroups.FirstOrDefault(s => s.StandardCostGroupId == g.GroupId);
                    if (standardGroup == null) throw new UserFriendlyException(L("RecordNotFound"));
                        
                    standardGroup.Update(userId, entity.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                    await _productionStandardCostGroupRepository.UpdateAsync(standardGroup);
                }
            }

            if (!deleteGroups.IsNullOrEmpty())
            {
                foreach(var g in deleteGroups)
                {
                    await _productionStandardCostGroupRepository.DeleteAsync(g);
                }
            }


            var issueStandardGroups = await _productionIssueStandardCostGroupRepository.GetAll().Where(s => s.ProductionId == entity.Id).ToListAsync();

            var addIssueGroups = input.IssueStandardGroups?.Where(s => !issueStandardGroups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
            var editIssueGroups = input.IssueStandardGroups?.Where(s => issueStandardGroups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
            var deleteIssueGroups = issueStandardGroups.WhereIf(!input.IssueStandardGroups.IsNullOrEmpty(), s => !input.IssueStandardGroups.Any(r => r.GroupId == s.StandardCostGroupId)).ToList();

            if (!addIssueGroups.IsNullOrEmpty())
            {
                foreach (var g in addIssueGroups)
                {
                    var standardGroup = ProductionIssueStandardCostGroup.Create(tenantId, userId, entity.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                    await _productionIssueStandardCostGroupRepository.InsertAsync(standardGroup);
                }
            }

            if (!editIssueGroups.IsNullOrEmpty())
            {
                foreach (var g in editIssueGroups)
                {
                    var standardGroup = issueStandardGroups.FirstOrDefault(s => s.StandardCostGroupId == g.GroupId);
                    if (standardGroup == null) throw new UserFriendlyException(L("RecordNotFound"));

                    standardGroup.Update(userId, entity.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                    await _productionIssueStandardCostGroupRepository.UpdateAsync(standardGroup);
                }
            }

            if (!deleteIssueGroups.IsNullOrEmpty())
            {
                foreach (var g in deleteIssueGroups)
                {
                    await _productionIssueStandardCostGroupRepository.DeleteAsync(g);
                }
            }


            if (productionPlanIds.Any())
            {
                var tenant = await GetCurrentTenantAsync();
                if (tenant.ProductionSummaryNetWeight || tenant.ProductionSummaryQty)
                {
                    await CurrentUnitOfWork.SaveChangesAsync();
                    await _productionPlanManager.CalculateByIdAsync(userId, productionPlanIds);
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ProductionOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var tenantId = AbpSession.GetTenantId();
            var @entity = await _productionManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //create item receipt transfer when click public

            if (entity.ConvertToIssueAndReceipt == true)
            {
                var tenant = await GetCurrentTenantAsync();
                //var currencyId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(v => v.CurrencyId).FirstOrDefaultAsync();
                var iventoryAccount = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && _rawMaterialItemsRepository.GetAll().Any(v => v.ItemId == t.Id)).ToListAsync();
                var finishAccount = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && _finishItemsRepository.GetAll().Any(v => v.ItemId == t.Id)).ToListAsync();

                //var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.RawProductionAccountId).FirstOrDefaultAsync();

                if (tenant.RawProductionAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseSetRawProductionAccountOnCompanyProfile"));
                }
                if (tenant.FinishProductionAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseSetFinishProductionAccountOnCompanyProfile"));
                }
                var locations = new List<long?>() {
                    entity.FromLocationId
                };
                
                //var averageCosts = await _inventoryManager.GetAvgCostQuery(@entity.Date.Date, locations).ToListAsync();

                var createInputItemIssueProduct = new CreateItemIssueProductInput()
                {
                    Memo = entity.Memo,
                    IssueNo = entity.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = entity.FromClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = entity.IssueDate.Value,
                    LocationId = entity.FromLocationId,
                    ReceiveFrom = ReceiveFrom.ProductionOrder,
                    Reference = entity.Reference,
                    Status = TransactionStatus.Publish,
                    ClearanceAccountId = tenant.RawProductionAccountId.Value,
                    ProductionId = entity.Id,
                    Total = entity.SubTotalRawProduction,
                    Items = _rawMaterialItemsRepository.GetAll().Include(u => u.Production)
                        .Where(d => d.Production.Id == input.Id)
                        .Select(t => new CreateOrUpdateRawMaterialItemsInput
                    {
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        Qty = t.Qty,
                        ItemId = t.ItemId,
                        //Total = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost * t.Qty).FirstOrDefault(),
                        //UnitCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                        Unit = t.Qty,
                        Id = t.Id,
                        RawMaterialId = t.Id,
                        
                    }).ToList(),

                };
                await CreateItemIssueProduction(createInputItemIssueProduct);

                //input to item Receipt Transfer and item issue
                //var ClearanceAccountIdForItemReceiptProduct = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.FinishProductionAccountId).FirstOrDefaultAsync();

                //var itemIssue = await _itemIssueItemRepository.GetAll()
                //           .Where(t => t.TransferOrderItemId != null)
                //           .Select(t => new
                //           {
                //               Id = t.ItemId,
                //               UnitCost = t.UnitCost,
                //               Qty = t.Qty,
                //               RawMaterialItemId = t.TransferOrderItemId
                //           })
                //           .ToListAsync();

                var createInputItemRecepitProduction = new CreateItemReceiptProductionInput()
                {
                    Memo = entity.Memo,
                    ReceiptNo = entity.ProductionNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = entity.ToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = entity.IssueDate.Value,
                    LocationId = entity.ToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.ProductionOrder,
                    Reference = entity.Reference,
                    Status = TransactionStatus.Publish,
                    ClearanceAccountId = tenant.FinishProductionAccountId.Value,
                    ProductionOrderId = entity.Id,
                    Total = entity.SubTotalFinishProduction,
                    Items = _finishItemsRepository.GetAll()
                    .Where(d => d.Production.Id == input.Id)
                    .Select(t => new CreateOrUpdateItemReceiptItemProductionInput
                    {
                        InventoryAccountId = (from Account in finishAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Qty = t.Qty,
                        ItemId = t.ItemId,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        Id = t.Id,
                        Description = t.Description,
                        FinishItemId = t.Id,
                    }).ToList()

                };
                await CreateItemReceiptProduct(createInputItemRecepitProduction);
            }


            entity.UpdateStatus(TransactionStatus.Publish);
            CheckErrors(await _productionManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _productionManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.ShipedStatus != TransferStatus.Pending && entity.ConvertToIssueAndReceipt == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }


            if (entity.ConvertToIssueAndReceipt == true)
            {
                //delete item receipt 

                var itemReceiptProductionId = _itemReceiptRepository.GetAll()
                       .Where(t => t.ProductionOrderId == input.Id && t.TransactionType == InventoryTransactionType.ItemReceiptProduction).Select(t => t.Id).FirstOrDefault();
                var inputItemReceipt = new CarlEntityDto() {IsConfirm = false, Id = itemReceiptProductionId };
                await DeleteItemReceiptProduction(inputItemReceipt);

                //delete item issue
                var itemIssueProductionId = _itemIssueRepository.GetAll()
                    .Where(t => t.ProductionOrderId == input.Id && t.TransactionType == InventoryTransactionType.ItemIssueProduction)
                    .Select(t => t.Id).FirstOrDefault();
                var inputItemIssueProduction = new CarlEntityDto() { Id = itemIssueProductionId };
                await DeleteItemIssueProduct(inputItemIssueProduction);

            }
            entity.UpdateStatus(TransactionStatus.Draft);
            CheckErrors(await _productionManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _productionManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (entity.ShipedStatus != TransferStatus.Pending && entity.ConvertToIssueAndReceipt == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            var itemReciptProductionId = _itemReceiptRepository.GetAll().Where(g => g.ProductionOrderId == input.Id).Select(t => t.Id).FirstOrDefault();
            var itemIssueProductionId = _itemIssueRepository.GetAll().Where(g => g.ProductionOrderId == input.Id).Select(t => t.Id).FirstOrDefault();

            if (entity.ConvertToIssueAndReceipt == true && itemReciptProductionId != null && itemIssueProductionId != null)
            {
                var jounalReceiptProduction = await _journalRepository
                .GetAll()
                .Include(u => u.ItemReceipt)
                .Include(u => u.Bill)
                .Include(u => u.ItemReceipt.ShippingAddress)
                .Include(u => u.ItemReceipt.BillingAddress)
                             .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                 u.JournalType == JournalType.ItemReceiptAdjustment ||
                                 u.JournalType == JournalType.ItemReceiptOther ||
                                 u.JournalType == JournalType.ItemReceiptTransfer 
                                 ||u.JournalType == JournalType.ItemReceiptProduction)
                                 && u.ItemReceipt.Id == itemReciptProductionId)
                             .FirstOrDefaultAsync();

                //update receive status of production order to ship all 
                if (jounalReceiptProduction.JournalType == JournalType.ItemReceiptProduction)
                {
                    // void item receipt production
                    var productionItemReceipt = await _productionRepository.GetAll().Where(u => u.Id == jounalReceiptProduction.ItemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                    // update received status 
                    if (productionItemReceipt != null)
                    {
                        productionItemReceipt.UpdateShipedStatus(TransferStatus.ShipAll);
                        CheckErrors(await _productionManager.UpdateAsync(productionItemReceipt));
                    }
                    jounalReceiptProduction.UpdateVoid();
                    //void item issue production                
                    var @jounalIssue = await _journalRepository.GetAll()
                    .Include(u => u.ItemIssue)
                    .Include(u => u.Bill)
                    .Include(u => u.ItemIssue.ShippingAddress)
                    .Include(u => u.ItemIssue.BillingAddress)
                    .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                       u.JournalType == JournalType.ItemIssueOther ||
                                       u.JournalType == JournalType.ItemIssueProduction ||
                                        u.JournalType == JournalType.ItemIssueTransfer ||
                                       u.JournalType == JournalType.ItemIssueAdjustment ||
                                       u.JournalType == JournalType.ItemIssueProduction||
                                       u.JournalType == JournalType.ItemIssueVendorCredit)
                                       && u.ItemIssueId == itemIssueProductionId)
                    .FirstOrDefaultAsync();
                    //update receive status of transfer order to pending 
                    if (@jounalIssue.JournalType == JournalType.ItemIssueProduction)
                    {
                        var @transferissue = await _productionRepository.GetAll().Where(u => u.Id == @jounalIssue.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
                        // update received status 
                        if (@jounalIssue != null)
                        {
                            @transferissue.UpdateShipedStatus(TransferStatus.Pending);
                            CheckErrors(await _productionManager.UpdateAsync(@transferissue));
                        }
                    }
                    jounalIssue.UpdateVoid();
                }

            }

            entity.UpdateStatus(TransactionStatus.Void);
            CheckErrors(await _productionManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_ProductionOrder_Calculate)]
        public async Task Calculation(ProductionCalculationInput input)
        {
            await _productionManager.CalculateAsync(AbpSession.UserId.Value, input.FromDate, input.ToDate);            
        }
    }
}
