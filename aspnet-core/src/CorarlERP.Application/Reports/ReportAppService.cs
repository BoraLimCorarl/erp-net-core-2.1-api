using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Journals;
using CorarlERP.Reports.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.Inventories;
using CorarlERP.InventoryCostCloses;
using CorarlERP.MultiTenancy;
using CorarlERP.AccountCycles;
using CorarlERP.AccountTransactions;
using CorarlERP.ReportTemplates;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using CorarlERP.Dto;
using CorarlERP.Bills;
using CorarlERP.Invoices;
using CorarlERP.TransferOrders;
using CorarlERP.Productions;
using Abp.AspNetZeroCore.Net;
using Abp.UI;
using CorarlERP.Formats;
using CorarlERP.Inventories.Data;
using Microsoft.AspNetCore.Identity;
using CorarlERP.PropertyValues;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Common.Dto;
using Abp.Runtime.Session;
using Abp.Domain.Uow;
using System.Transactions;
using Abp.Timing;
using Abp.Dependency;
using CorarlERP.ProductionProcesses;
using CorarlERP.Lots;
using CorarlERP.FileStorages;
using CorarlERP.InventoryTransactionItems;
using Abp.Application.Features;
using CorarlERP.Features;
using CorarlERP.BatchNos;
using CorarlERP.Authorization.Users.Dto;
using System.Runtime.InteropServices;
using CorarlERP.UserGroups;
using System.Text.RegularExpressions;
using CorarlERP.ProductionPlans.Dto;
using CorarlERP.Productions.Dto;
using CorarlERP.ProductionPlans;
using CorarlERP.Locations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace CorarlERP.Reports
{
    [AbpAuthorize]
    public class ReportAppService : ReportBaseClass, IReportAppService
    {
        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IItemIssueManager _itemIssueManager;
        private readonly IJournalManager _journalManager;
        private readonly IJournalItemManager _journalItemManager;
        private readonly ITransferOrderManager _transferOrderManager;
        private readonly IProductionManager _productionOrderManager;
        private readonly IRawMaterialItemManager _rawMaterialItemManager;
        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IItemReceiptManager _itemReceiptManager;

        private readonly ICorarlRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;
        private readonly ICorarlRepository<Production, Guid> _productionsRepository;
        private readonly IRepository<FinishItems, Guid> _finishItemsRepository;
        private readonly IFinishItemManager _finishItemManager;

        private readonly IInventoryManager _inventoryManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<AccountType, long> _accountTypeRepository;
        //private readonly IRepository<InventoryTransaction, Guid> _inventoryTransactionRepository;
        private readonly ICorarlRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly ICorarlRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        private readonly ICorarlRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly ICorarlRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly ICorarlRepository<Invoice, Guid> _invoiceRepository;

        private readonly IRepository<InventoryCostClose, Guid> _inventoryCostCloseRepository;
        private readonly IRepository<InventoryCostCloseItem, Guid> _inventoryCostCloseItemRepository;

        private readonly ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;

        private readonly IItemIssueVendorCreditItemManager _itemIssueVendorCreditItemManager;
        private readonly IItemIssueVendorCreditManager _itemIssueVendorCreditManager;
        private readonly ICorarlRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;

        private readonly IItemReceiptCustomerCreditManager _itemReceiptCustomerCreditManager;
        private readonly ICorarlRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;

        private readonly IItemReceiptItemCustomerCreditManager _itemReceiptItemCustomerCreditManager;

        private readonly IRepository<AccountCycle, long> _accountCycleRepository;

        private readonly IAccountTransactionManager _accountTransactionManager;
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IFileStorageManager _fileStorageManager;

        private readonly IRepository<Lot, long> _lostRepository;

        private readonly IRepository<Format, long> _formatRepository;
        private readonly IRepository<ProductionProcess, long> _productionProcessRepository;

        private readonly IRepository<Property, long> _propertyRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemProperty, Guid> _itemPropertyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ProductionStandardCost, Guid> _productionStandardCostRepository;
        private readonly ICorarlRepository<ProductionIssueStandardCost, Guid> _productionIssueStandardCostRepository;
        private readonly ICorarlRepository<ProductionStandardCostGroup, Guid> _productionStandardCostGroupRepository;
        private readonly ICorarlRepository<ProductionIssueStandardCostGroup, Guid> _productionIssueStandardCostGroupRepository;
        private readonly ICorarlRepository<ProductionPlan, Guid> _productionPlanRepository;
        private readonly IProductionPlanManager _productionPlanManager;
        private readonly IProductionManager _productionManager;
        private readonly IRepository<InventoryCostCloseItemBatchNo, Guid> _inventoryCostCloseItemBatchNoRepository;
        public ReportAppService(
            IProductionManager productionManager,
            IProductionPlanManager productionPlanManager,
            ICorarlRepository<ProductionStandardCost, Guid> productionStandardCostRepository,
            ICorarlRepository<ProductionIssueStandardCost, Guid> productionIssueStandardCostRepository,
            ICorarlRepository<ProductionStandardCostGroup, Guid> productionStandardCostGroupRepository,
            ICorarlRepository<ProductionIssueStandardCostGroup, Guid> productionIssueStandardCostGroupRepository,
            IRepository<InventoryCostCloseItemBatchNo, Guid> inventoryCostCloseItemBatchNoRepository,
            ICorarlRepository<ProductionPlan, Guid> productionPlanRepository,
            IItemIssueManager itemIssueManager,
            IItemIssueItemManager itemIssueItemManager,
            IJournalManager journalManager,
            IFileStorageManager fileStorageManager,
            IJournalItemManager journalItemManager,
            IProductionManager productionOrderManager,
            IRawMaterialItemManager rawMaterialItemManager,
            ITransferOrderManager transferOrderManager,
            IItemReceiptManager itemReceiptManager,
            IItemReceiptItemManager itemReceiptItemManager,
            ICorarlRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCreditRepository,
            ItemIssueVendorCreditManager itemIssueVendorCreditManager,
            ItemIssueVendorCreditItemManager itemIssueVendorCreditItemManager,
            ICorarlRepository<RawMaterialItems, Guid> rawMaterialItemsRepository,
            IRepository<FinishItems, Guid> finishItemsRepository,
            IFinishItemManager finishItemManager,
            IRepository<Item, Guid> itemRepository,
            ICorarlRepository<Journal, Guid> journalRepository,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            IRepository<AccountType, long> accountTypeRepository,
            //IRepository<InventoryTransaction, Guid> inventoryTransactionRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            ICorarlRepository<ItemReceipt, Guid> itemReceiptRepository,
            ICorarlRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            ICorarlRepository<ItemIssue, Guid> itemIssueRepository,
            ICorarlRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            ICorarlRepository<Invoice, Guid> invoiceRepository,
            ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<InventoryCostClose, Guid> inventoryCostCloseRepository,
            IRepository<InventoryCostCloseItem, Guid> inventoryCostCloseItemRepository,
            ICorarlRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IAccountTransactionManager accountTransactionManager,
            IInventoryManager inventoryManager,
            IRepository<Property, long> propertyRepository,
            IRepository<ItemProperty, Guid> itemPropertyRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            AppFolders appFolders,
            IItemReceiptCustomerCreditManager itemReceiptCustomerCreditManager,
            ICorarlRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
            IItemReceiptItemCustomerCreditManager itemReceiptItemCustomerCreditManager,
            IRepository<Format, long> formatRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ICorarlRepository<Production, Guid> productionsRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            ICorarlRepository<Location, long> locationRepository
           ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _itemReceiptItemCustomerCreditManager = itemReceiptItemCustomerCreditManager;
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;
            _itemReceiptCustomerCreditManager = itemReceiptCustomerCreditManager;
            _productionsRepository = productionsRepository;
            _itemIssueManager = itemIssueManager;
            _unitOfWorkManager = unitOfWorkManager;
            _itemIssueItemManager = itemIssueItemManager;
            _journalManager = journalManager;
            _journalItemManager = journalItemManager;
            _productionOrderManager = productionOrderManager;
            _rawMaterialItemManager = rawMaterialItemManager;
            _transferOrderManager = transferOrderManager;
            _itemReceiptManager = itemReceiptManager;
            _itemReceiptItemManager = itemReceiptItemManager;
            _itemRepository = itemRepository;
            _rawMaterialItemsRepository = rawMaterialItemsRepository;
            _finishItemsRepository = finishItemsRepository;
            _finishItemManager = finishItemManager;
            _propertyRepository = propertyRepository;
            //_inventoryTransactionRepository = inventoryTransactionRepository;
            _journalRepository = journalRepository;
            _journalItemRepository = journalItemRepository;
            _accountTypeRepository = accountTypeRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemIssueRepository = itemIssueRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemIssueVendorCreditItemManager = itemIssueVendorCreditItemManager;
            _itemIssueVendorCreditManager = itemIssueVendorCreditManager;
            _itemIssueVendorCreditRepository = itemIssueVendorCreditRepository;
            _inventoryCostCloseRepository = inventoryCostCloseRepository;
            _inventoryCostCloseItemRepository = inventoryCostCloseItemRepository;
            _tenantRepository = tenantRepository;
            _inventoryManager = inventoryManager;
            _accountTransactionManager = accountTransactionManager;
            _appFolders = appFolders;
            _itemPropertyRepository = itemPropertyRepository;
            _accountCycleRepository = accountCycleRepository;
            _formatRepository = formatRepository;
            _fileStorageManager = fileStorageManager;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;

            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
            _batchNoRepository = batchNoRepository;
            _invoiceRepository = invoiceRepository;

            _productionProcessRepository = IocManager.Instance.Resolve<IRepository<ProductionProcess, long>>();
            _lostRepository = IocManager.Instance.Resolve<IRepository<Lot, long>>();
            _productionPlanRepository = productionPlanRepository;
            _productionStandardCostRepository = productionStandardCostRepository;
            _productionIssueStandardCostRepository = productionIssueStandardCostRepository;
            _productionStandardCostGroupRepository = productionStandardCostGroupRepository;
            _productionIssueStandardCostGroupRepository = productionIssueStandardCostGroupRepository;
            _productionPlanManager = productionPlanManager;
            _productionManager = productionManager;
            _inventoryCostCloseItemBatchNoRepository = inventoryCostCloseItemBatchNoRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory, AppPermissions.Pages_Tenant_Common_Inventory)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListInventoryReportOutputNew>> GetInventoryReport(GetInventoryInput input)
        {
            var userId = AbpSession.GetUserId();


            var itemWithPropertyQuery = GetItemWithPropertiesQuery(input.Filter, input.Items, input.InventoryAccount, input.AccountTypes, input.PropertyDics);


            var query = _inventoryManager.GetItemCostSummaryByLocationQuery(input.FromDate, input.ToDate, input.Location, userId, input.Items);

            var result = from q in query
                                    .WhereIf(input.StockMovement != null && input.StockMovement.Count() > 0, t =>
                                        (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && t.BeginningQty != 0) ||
                                        (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && t.InQty != 0) ||
                                        (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && t.OutQty != 0)
                                    )
                         join i in itemWithPropertyQuery
                         on q.ItemId equals i.ItemId

                         let unit = i.Properties.Where(s => s.IsUnit).FirstOrDefault()
                         let netWeight = unit == null ? 0 : unit.NetWeight
                         let sbStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero :
                                        q.Qty > 0 ? InventoryStockBalanceStatus.Positive :
                                        InventoryStockBalanceStatus.Negative
                         where input.StockBalance == null ||
                               input.StockBalance.Count() == 0 ||
                               input.StockBalance.Contains(sbStatus)

                         select new GetListInventoryReportOutputNew
                         {
                             Id = q.ItemId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             SalePrice = i.SalePrice ?? 0,
                             TotalQtyOnHand = q.Qty,
                             TotalInQty = q.InQty,
                             Beginning = q.BeginningQty,
                             TotalOutQty = q.OutQty,
                             StockBalanceStatus = sbStatus,

                             TotalCost = q.TotalCost,
                             InventoryAccountId = i.InventoryAccountId ?? Guid.Empty,
                             InventoryAccountName = i.InventoryAccountName,
                             InventoryAccountCode = i.InventoryAccountCode,
                             LocationId = q.LocationId,
                             LocationName = q.LocationName,
                             RoundingDigit = q.RoundingDigit,
                             RoundingDigitUnitCost = q.RoundingDigitUnitCost,
                             NetWeight = netWeight * q.Qty,
                             AverageCost = q.Cost,
                             PropertySummary = i.Properties,
                         };

            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }

            var sumOfColumns = new Dictionary<string, decimal>();

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<GetListInventoryReportOutputNew>(resultCount, new List<GetListInventoryReportOutputNew>(), sumOfColumns);
            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await result.Select(t => new
                {
                    t.NetWeight,
                    t.TotalQtyOnHand,
                    t.DisplayTotalCost,
                    t.TotalInQty,
                    t.TotalOutQty,
                    t.Beginning
                }).ToListAsync();

                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "NetWeight") sumOfColumns.Add(c, sumList.Select(u => u.NetWeight).Sum());
                    else if (c == "DisplayTotalCost") sumOfColumns.Add(c, sumList.Select(u => u.DisplayTotalCost).Sum());
                    else if (c == "TotalQtyOnHand") sumOfColumns.Add(c, sumList.Select(u => u.TotalQtyOnHand).Sum());
                    else if (c == "Beginning") sumOfColumns.Add(c, sumList.Select(u => u.Beginning).Sum());
                    else if (c == "TotalInQty") sumOfColumns.Add(c, sumList.Select(u => u.TotalInQty).Sum());
                    else if (c == "TotalOutQty") sumOfColumns.Add(c, sumList.Select(u => u.TotalOutQty).Sum());
                }
            }

            if (!input.GroupBy.IsNullOrWhiteSpace())
            {
                switch (input.GroupBy)
                {
                    case "Account":
                        result = result.OrderBy(s => s.InventoryAccountCode);
                        break;
                    case "Location":
                        result = result.OrderBy(s => s.LocationId);
                        break;
                    default:
                        result = result.OrderBy(s => s.ItemCode);
                        break;
                }
            }


            var @entities = new List<GetListInventoryReportOutputNew>();

            if (input.UsePagination == true)
            {
                @entities = await result.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<GetListInventoryReportOutputNew>(resultCount, @entities, sumOfColumns);
        }

        public ReportOutput GetReportTemplateInventory()
        {
            var itemProperty = _propertyRepository.GetAll().AsNoTracking()
                               .Where(t => t.IsActive == true)
                               .Select(t => new CollumnOutput
                               {
                                   AllowGroupby = false,
                                   AllowFilter = true,
                                   ColumnName = t.Name,
                                   ColumnLength = 150,
                                   ColumnTitle = t.Name,
                                   ColumnType = ColumnType.ItemProperty,
                                   SortOrder = 13,
                                   Visible = true,
                                   AllowFunction = null,
                                   MoreFunction = null,
                                   IsDisplay = true,
                                   DefaultValue = t.Id.ToString(),//to init the value of property
                                   AllowShowHideFilter = true,
                                   ShowHideFilter = true,
                               }).OrderBy(t => t.ColumnName);
            var columnInfo = new List<CollumnOutput>() {
                      //start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = true,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "StockBalance",
                        ColumnLength = 300,
                        ColumnTitle = "Stock Balance",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = InventoryStockBalanceStatus.Positive.ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryMovement",
                        ColumnLength = 300,
                        ColumnTitle = "Inventory Movement",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 150,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 180,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 330,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 200,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total In Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total Out Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 170,
                        ColumnTitle = "Ending Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AverageCost",
                        ColumnLength = 200,
                        ColumnTitle = "Average Cost",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        //AllowFunction = "Sum",
                        //MoreFunction = new List<MoreFunction>() {
                        //    new MoreFunction
                        //    {
                        //        KeyName = null
                        //    },
                        //    new MoreFunction
                        //    {
                        //        KeyName = "Sum"
                        //    }
                        //},
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "DisplayTotalCost",
                        ColumnLength = 170,
                        ColumnTitle = "Total Cost",
                        ColumnType = ColumnType.Money,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SalePrice",
                        ColumnLength = 170,
                        ColumnTitle = "Sale Price",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 12,
                        Visible = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        //AllowFunction = "Sum",
                        //MoreFunction = new List<MoreFunction>(){
                        //    new MoreFunction
                        //    {
                        //        KeyName = null
                        //    },
                        //    new MoreFunction
                        //    {
                        //        KeyName = "Sum"
                        //    }
                        //},
                        IsDisplay = true
                    },
                    //new CollumnOutput{
                    //    AllowGroupby = false,
                    //    AllowFilter = false,
                    //    ColumnName = "Unit",
                    //    ColumnLength = 130,
                    //    ColumnTitle = "Unit",
                    //    ColumnType = ColumnType.Object,
                    //    SortOrder = 9,
                    //    Visible = true,
                    //    IsDisplay = true
                    //},
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },

                };
            var totalCols = columnInfo.Concat(itemProperty).ToList();

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                totalCols = totalCols.Where(s => s.ColumnName != "Account" && s.ColumnName != "AccountType").ToList();
            }

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = totalCols,
                Groupby = "",
                HeaderTitle = "Inventory Valuation Summary",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "StockBalance",
                    ColumnTitle = L("StockBalanceTemplate"),
                    DefaultValue = ReportType.ReportType_StockBalance
                },
                DefaultTemplate2 = new DefaultSaveTemplateOutput
                {
                    ColumnName = "InventoryValuationDetail",
                    ColumnTitle = L("InventoryValuationDetailTemplate"),
                    DefaultValue = ReportType.ReportType_InventoryDetail
                },
                DefaultTemplate3 = new DefaultSaveTemplateOutput
                {
                    ColumnName = "InventoryTransaction",
                    ColumnTitle = L("InventoryTransactionTemplate"),
                    DefaultValue = ReportType.ReportType_InventoryTransaction
                }
            };
            return result;
        }

        public ReportOutput GetReportTemplateInventoryValuationDetail()
        {
            var itemProperty = _propertyRepository.GetAll().AsNoTracking()
                               .Where(t => t.IsActive == true)
                               .Select(t => new CollumnOutput
                               {
                                   AllowGroupby = false,
                                   AllowFilter = true,
                                   ColumnName = t.Name,
                                   ColumnLength = 150,
                                   ColumnTitle = t.Name,
                                   ColumnType = ColumnType.ItemProperty,
                                   SortOrder = 14,
                                   Visible = true,
                                   AllowFunction = null,
                                   MoreFunction = null,
                                   IsDisplay = true,
                                   DefaultValue = t.Id.ToString(),//to init the value of property
                                   AllowShowHideFilter = true,
                                   ShowHideFilter = true,
                               });
            var columnInfo = new List<CollumnOutput>() {
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = false,
                        MoreFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 300,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DisableDefault = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "No",
                        ColumnLength = 65,
                        ColumnTitle = "No",
                        ColumnType = ColumnType.AutoNumber,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 140,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 165,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalType",
                        ColumnLength = 140,
                        ColumnTitle = "Journal Type",
                        ColumnType = ColumnType.StatusCode,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },

                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 200,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 140,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 140,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "InQty",
                        ColumnLength = 100,
                        ColumnTitle = "In Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OutQty",
                        ColumnLength = 100,
                        ColumnTitle = "Out Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UnitCost",
                        ColumnLength = 110,
                        ColumnTitle = "Unit Cost",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LineTotal",
                        ColumnLength = 110,
                        ColumnTitle = "Line Total",
                        ColumnType = ColumnType.Money,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQty",
                        ColumnLength = 110,
                        ColumnTitle = "Total Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalCost",
                        ColumnLength = 110,
                        ColumnTitle = "Total Cost",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AVGCost",
                        ColumnLength = 110,
                        ColumnTitle = "AVG Cost",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 140,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TransactionNo",
                        ColumnLength = 165,
                        ColumnTitle = "Transaction No",
                        ColumnType = ColumnType.String,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                };
            var totalCols = columnInfo.Concat(itemProperty).ToList();

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                totalCols = totalCols.Where(s => s.ColumnName != "Account" && s.ColumnName != "AccountType").ToList();
            }


            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = totalCols,
                Groupby = "",
                HeaderTitle = "Inventory Valuation Detail",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Excel)]
        public async Task<FileDto> ExportExcelInventoryReport(GetInventoryReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var inventoryData = (await GetInventoryReport(input)).Items;

            var result = new FileDto();

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

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListInventoryReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = inventoryData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListInventoryReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListInventoryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountName + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListInventoryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyInventoryValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in inventoryData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyInventoryValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = footerGroupDict.ContainsKey(i.ColumnName) ? "SUM(" + footerGroupDict[i.ColumnName] + ")" : "";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Inventory_Valuation_Summary_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Detail_Export_Excel)]
        public async Task<FileDto> ExportExcelInventoryValuationDetailReport(GetInventoryValuationDetailReportInput input)
        {
            //input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            input.UsePagination = false;
            var inventoryData = (await GetInventoryValuationDetailReport(input)).Items;

            var result = new FileDto();

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

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = inventoryData
                                .GroupBy(t => t.GroupKey)
                                .Select(x =>
                                {
                                    decimal runningQty = 0;
                                    decimal runningCost = 0;
                                    decimal avgCost = 0;
                                    decimal oldTotalQty = 0;

                                    var r = new InventoryValuationDetail
                                    {
                                        ItemName = x.Select(a => a.ItemCode + " - " + a.ItemName).FirstOrDefault(),
                                        Items = x.Select(t =>
                                        {
                                            t.UnitCost = Math.Round(t.UnitCost, t.RoundingDigitUnitCost);
                                            var i = t;
                                            oldTotalQty = runningQty;
                                            runningQty += t.InQty + t.OutQty;
                                            runningCost += t.LineTotal;

                                            if (oldTotalQty < 0 && t.JournalType != null)
                                            {
                                                runningCost = Math.Round(runningQty * t.UnitCost, t.RoundingDigit);
                                            }


                                            avgCost = Math.Round(Math.Abs(+(runningQty == 0 ? 0 : runningCost / runningQty)), t.RoundingDigitUnitCost);

                                            runningCost = runningQty == 0 ? 0 : runningCost;

                                            t.TotalQty = runningQty;
                                            t.TotalCost = runningCost;
                                            t.AVGCost = avgCost;
                                            return i;
                                        }).ToList()
                                    };
                                    return r;
                                })
                                .ToList();
                foreach (var k in groupBy)
                {
                    //key group by name
                    AddTextToCell(ws, rowBody, 1, k.ItemName, true);
                    MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                    rowBody += 1;
                    count = 1;
                    //list of item
                    foreach (var i in k.Items)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyInventoryValuationDetail(ws, rowBody, collumnCellBody, item, i, i.Properties, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }

                }
                #endregion Row Body

                result.FileName = $"Inventory_Valuation_Detail_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        private List<ItemDto> CalculateAvgCostDetailManualAsync(DateTime toDate, List<AccountCycle> accountCyles, List<ItemDto> inventoryItemDetail, bool groupByLocation = false, Dictionary<string, ItemCostSummaryDto> itemLatestCostDic = null)
        {
            var result = new List<ItemDto>();


            var calculatedResult = _inventoryManager.CalculateManualInventoryValuationDetail(accountCyles, inventoryItemDetail, null, groupByLocation, itemLatestCostDic);

            foreach (var r in calculatedResult)
            {
                var data = new ItemDto()
                {
                    InventoryAccountId = r.InventoryAccountId,
                    IsPurchase = true,
                    LocationId = r.LocationId,
                    RoundingDigit = 0,
                    Qty = r.TotalQty,
                    TotalCost = r.TotalCost,
                    UnitCost = r.CurrentAvgCost,
                    JournalDate = toDate,
                    JournalType = null,
                    LocationName = r.LocationName,
                    InventoryAccountCode = r.InventoryAccountCode,
                    InventoryAccountName = r.InventoryAccountName,
                    ItemCode = r.ItemCode,
                    ItemId = r.ItemId,
                    ItemName = r.ItemName,
                    SalePrice = null,
                    TransactionId = null,
                    TransactionItemId = null,

                    TotalLocationCost = r.TotalLocationCost,
                    TotalLocationQty = r.TotalLocationQty,
                };

                result.Add(data);
            }

            return result;

        }

        #region RecalculateCost Old
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Summary_RecalculateAvg)]
        [UnitOfWork(IsDisabled = true)]
        public async Task RecalculateCostOld(RecalculateCostInput input)
        {
            var tenantId = AbpSession.TenantId;
            Guid? ItemRecieptAdjustmentId;
            var periodCycles = new List<AccountCycle>();
            var currentPeriod = new AccountCycle();
            List<ItemDto> calculateItemList = new List<ItemDto>();
            Guid? roundingAccountId;

            var roundingItemsOuptput = new Dictionary<Guid, List<RoundingAdjustmentItemOutput>>();
            var journalToUpdate = new Dictionary<Guid, Guid>();
            var itemLatestCostDic = new Dictionary<string, ItemCostSummaryDto>();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    periodCycles = await GetCloseCyleAsync();
                    currentPeriod = periodCycles.Where(u => u.EndDate == null).FirstOrDefault();
                    var fromDate = currentPeriod == null ? (DateTime?)null : currentPeriod.StartDate.AddDays(-1);


                    //begining
                    var inventoryDetailFromDate = fromDate == null ? new List<ItemDto>() :
                                        await _inventoryManager.GetInventoryValuationDetailQuery(fromDate.Value, null)
                                            //.WhereIf(input.Items != null && input.Items.Count > 0, t => input.Items.Contains(t.ItemId))
                                            .ToListAsync();

                    //from
                    var inventoryDetailToDate = await _inventoryManager.GetInventoryValuationDetailQuery(fromDate, null, null)
                                                //.WhereIf(input.Items != null && input.Items.Count > 0, t => input.Items.Contains(t.ItemId))
                                                .ToListAsync();


                    var saleReturnItemIds = inventoryDetailFromDate.Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                                .GroupBy(g => g.ItemId).Select(s => s.Key).ToList();
                    itemLatestCostDic = fromDate == null ? null : await _inventoryManager.GetItemLatestCostSummaryQuery(fromDate.Value, null, saleReturnItemIds, null)
                                                                .ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);

                    inventoryDetailToDate.AddRange(inventoryDetailFromDate);

                    calculateItemList = inventoryDetailToDate;


                    roundingAccountId = (await GetCurrentTenantAsync()).RoundDigitAccountId;
                }

                await uow.CompleteAsync();
            }


            var calculatedResult = _inventoryManager.CalculateManualInventoryValuationDetail(periodCycles, calculateItemList, journalToUpdate, false, itemLatestCostDic);

            var result = calculatedResult.SelectMany(u => u.Items).Select(u => u).ToList();

            var transactionsToUpdate = result.Where(u => u.JournalId != null && u.TransactionId != null
                                                        /*&& journalToUpdate != null &&
                                                        journalToUpdate.ContainsKey(u.JournalId.Value)*/
                                                        )
                                             .Where(u => u.JournalType != null &&
                                                    (u.JournalType == JournalType.ItemIssueOther ||
                                                    u.JournalType == JournalType.ItemIssueSale ||
                                                    u.JournalType == JournalType.ItemIssueTransfer ||
                                                    u.JournalType == JournalType.ItemIssueProduction ||
                                                    u.JournalType == JournalType.ItemIssueVendorCredit ||
                                                    u.JournalType == JournalType.ItemReceiptCustomerCredit ||
                                                    u.JournalType == JournalType.ItemIssuePhysicalCount ||
                                                    u.JournalType == JournalType.ItemIssueAdjustment ||
                                                    u.JournalType == JournalType.ItemReceiptTransfer)
                                                )
                                                .GroupBy(u => new
                                                {
                                                    JournalId = u.JournalId.Value,
                                                    u.JournalNo,
                                                    JournalType = u.JournalType.Value,
                                                    u.Date,
                                                    TransactionId = u.TransactionId.Value
                                                }).ToList();

            var itemReceiptJournalIds = result.Where(u => u.JournalId != null && u.TransactionId != null /*&& journalToUpdate != null &&
                                                      journalToUpdate.ContainsKey(u.JournalId.Value)*/)
                                            .Where(u => u.JournalType != null &&
                                                   (u.JournalType == JournalType.ItemReceiptOther ||
                                                   u.JournalType == JournalType.ItemReceiptPurchase ||
                                                   u.JournalType == JournalType.ItemReceiptTransfer ||
                                                   u.JournalType == JournalType.ItemReceiptProduction ||
                                                   u.JournalType == JournalType.ItemReceiptCustomerCredit ||
                                                   u.JournalType == JournalType.ItemReceiptPhysicalCount ||
                                                   u.JournalType == JournalType.ItemReceiptAdjustment)
                                               )
                                               .GroupBy(u => u.JournalId.Value)
                                               .Select(s => s.Key)
                                               .ToList();


            var journalListForItemReceipt = new List<Journal>();
            var journalItemForItemReceiptList = new List<JournalItem>();
            var journalsList = new List<Journal>();
            var journalItemsList = new List<JournalItem>();

            var itemReceiptsList = new List<ItemReceipt>();
            var itemReceiptItemsList = new List<ItemReceiptItem>();
            var itemReceiptCustomerCreditList = new List<ItemReceiptCustomerCredit>();
            var itemReceiptCustomerCreditItemsList = new List<ItemReceiptItemCustomerCredit>();
            var itemIssuesList = new List<ItemIssue>();
            var itemIssueItemsList = new List<ItemIssueItem>();
            var itemIssueVendorCreditsList = new List<ItemIssueVendorCredit>();
            var itemIssueVendorCreditItemsList = new List<ItemIssueVendorCreditItem>();
            var rawMaterialItemsList = new List<RawMaterialItems>();
            var productOrderList = new List<Production>();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    ItemRecieptAdjustmentId = (await GetCurrentTenantAsync()).ItemRecieptAdjustmentId;

                    journalsList = await _journalRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.JournalId).Contains(x.Id))
                                        .ToListAsync();

                    journalListForItemReceipt = await _journalRepository.GetAll()
                                        .Where(x => itemReceiptJournalIds.Contains(x.Id))
                                        .ToListAsync();

                    journalItemsList = await _journalItemRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.JournalId).Contains(x.JournalId))
                                        .Select(x => x)
                                        .ToListAsync();
                    journalItemForItemReceiptList = await _journalItemRepository.GetAll()
                                       .Where(x => itemReceiptJournalIds.Contains(x.JournalId))
                                       .Select(x => x)
                                       .ToListAsync();
                    itemIssuesList = await _itemIssueRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.Id))
                                        .ToListAsync();

                    itemIssueItemsList = await _itemIssueItemRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.ItemIssueId))
                                        .ToListAsync();
                    itemReceiptsList = await _itemReceiptRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.Id))
                                        .ToListAsync();

                    itemReceiptItemsList = await _itemReceiptItemRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.ItemReceiptId))
                                        .ToListAsync();

                    itemReceiptCustomerCreditList = await _itemReceiptCustomerCreditRepository.GetAll()
                                       .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.Id))
                                       .ToListAsync();

                    itemReceiptCustomerCreditItemsList = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.ItemReceiptCustomerCreditId))
                                        .ToListAsync();

                    itemIssueVendorCreditsList = await _itemIssueVendorCreditRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.Id))
                                        .ToListAsync();

                    itemIssueVendorCreditItemsList = await _itemIssueVendorCreditItemRepository.GetAll()
                                        .Where(x => transactionsToUpdate.Select(t => t.Key.TransactionId).Contains(x.ItemIssueVendorCreditId))
                                        .ToListAsync();

                    rawMaterialItemsList = await _rawMaterialItemsRepository.GetAll()
                                            .Where(u => itemIssuesList.Where(x => x.ProductionOrderId != null)
                                                                            .Select(x => x.ProductionOrderId).Contains(u.ProductionId))
                                            .ToListAsync();

                    productOrderList = await _productionsRepository.GetAll().Where(u => itemIssuesList.Where(x => x.ProductionOrderId != null)
                                                                            .Select(x => x.ProductionOrderId).Contains(u.Id)).ToListAsync();


                }

                await uow.CompleteAsync();
            }


            var journalsEntity = new List<Journal>();
            var journalItemsCreateEntity = new List<JournalItem>();
            var journalItemsUpdateEntity = new List<JournalItem>();
            var journalItemsRemoveEntity = new List<JournalItem>();
            var itemIssuesEntity = new List<ItemIssue>();
            var itemIssueItemsEntity = new List<ItemIssueItem>();

            var itemReceiptsEntity = new List<ItemReceipt>();
            var itemReceiptItemsEntity = new List<ItemReceiptItem>();
            var itemReceiptCustomerCreditEntity = new List<ItemReceiptCustomerCredit>();
            var itemReceiptCustomerCreditItemsEntity = new List<ItemReceiptItemCustomerCredit>();
            var itemIssueVendorCreditsEntity = new List<ItemIssueVendorCredit>();
            var itemIssueVendorCreditItemsEntity = new List<ItemIssueVendorCreditItem>();
            var rawMaterialItemsEntity = new List<RawMaterialItems>();
            var productOrderEntity = new List<Production>();


            /***********************************************************************
             * Start Update Transaction Journal, Goods Issue And Goods Receipt *****
             ***********************************************************************/
            foreach (var t in transactionsToUpdate)
            {

                if (t.Key.JournalType == JournalType.ItemIssueSale)
                {
                    // get item issue
                    var itemIssue = itemIssuesList.Where(x => x.Id == t.Key.TransactionId).FirstOrDefault(); //await _itemIssueManager.GetAsync(t.Key.TransactionId, true);
                    // get item issue items
                    var itemIssueItems = itemIssueItemsList.Where(x => x.ItemIssueId == t.Key.TransactionId).ToList();
                    //await _itemIssueItemRepository.GetAll()
                    //.Where(u => u.ItemIssueId == t.Key.TransactionId).ToListAsync();

                    //var journalItem = journalItemsList.Where(u => u.Identifier != null && u.JournalId == t.Key.JournalId).ToList();
                    //await (_journalItemRepository.GetAll()
                    //      .Where(u => u.Identifier != null && u.JournalId == t.Key.JournalId)
                    //  ).ToListAsync();
                    decimal totalCost = 0;

                    foreach (var c in itemIssueItems)
                    {
                        var updatedItem = t.Where(u => u.TransactionItemId == c.Id).FirstOrDefault();
                        if (updatedItem != null)
                        {
                            var lineTotal = Math.Abs(updatedItem.LineTotal);
                            c.UpdateUnitCost(updatedItem.UnitCost, lineTotal);

                            //CheckErrors(await _itemIssueItemManager.UpdateAsync(c));
                            itemIssueItemsEntity.Add(c);

                            //update journal items 
                            var journalItem = journalItemsList.Where(u => u.Identifier == c.Id).ToList();
                            if (journalItem != null && journalItem.Count > 0)
                            {

                                foreach (var i in journalItem)
                                {
                                    if (i.Key == PostingKey.Inventory)
                                    {
                                        // update inventory posting key credit
                                        i.SetDebitCredit(0, lineTotal);
                                        //CheckErrors(await _journalItemManager.UpdateAsync(i));
                                        journalItemsUpdateEntity.Add(i);
                                    }
                                    if (i.Key == PostingKey.COGS)
                                    {
                                        // update cogs posting key debit
                                        i.SetDebitCredit(lineTotal, 0);
                                        journalItemsUpdateEntity.Add(i);
                                        //CheckErrors(await _journalItemManager.UpdateAsync(i));
                                    }
                                }
                            }
                            totalCost += lineTotal;
                        }
                    }
                    itemIssue.UpdateTotal(totalCost);
                    itemIssuesEntity.Add(itemIssue);
                    //CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));

                    // update journal
                    var journal = journalsList.Where(x => x.Id == t.Key.JournalId).FirstOrDefault();// await _journalManager.GetAsync(t.Key.JournalId, true);
                    if (journal != null)
                    {

                        journal.UpdateCreditDebit(totalCost, totalCost);
                        // CheckErrors(await AutoRoundingCheckHelperAsync(roundingItemsOuptput, journal, roundingAccountId));
                        journalsEntity.Add(journal);
                        //CheckErrors(await _journalManager.UpdateAsync(journal, null, false));
                    }
                }

                else if (t.Key.JournalType == JournalType.ItemIssueVendorCredit)
                {
                    // get item issue
                    var itemIssueVendorCredit = itemIssueVendorCreditsList.Where(x => x.Id == t.Key.TransactionId).FirstOrDefault();//await _itemIssueVendorCreditManager.GetAsync(t.Key.TransactionId, true);
                    // get item issue items
                    var itemIssueVendorCreditItems = itemIssueVendorCreditItemsList.Where(x => x.ItemIssueVendorCreditId == t.Key.TransactionId).ToList();
                    //await _itemIssueVendorCreditItemRepository.GetAll()
                    //.Where(u => u.ItemIssueVendorCreditId == t.Key.TransactionId).ToListAsync();
                    //var journalItems = 
                    //await (_journalItemRepository.GetAll()
                    //      .Where(u => u.Identifier != null &&
                    //           u.Key == PostingKey.Inventory &&
                    //           u.JournalId == t.Key.JournalId)
                    //    ).ToListAsync();

                    decimal totalCost = 0;
                    foreach (var c in itemIssueVendorCreditItems)
                    {
                        var updatedItem = t.Where(u => u.TransactionItemId == c.Id).FirstOrDefault();
                        if (updatedItem != null)
                        {
                            var lineTotal = Math.Abs(updatedItem.LineTotal);
                            c.UpdateUnitCost(updatedItem.UnitCost, lineTotal);
                            itemIssueVendorCreditItemsEntity.Add(c);
                            //CheckErrors(await _itemIssueVendorCreditItemManager.UpdateAsync(c));
                            //update journal items 
                            var updateJournalItems = journalItemsList.Where(x => x.Identifier == c.Id).ToList();

                            if (updateJournalItems != null)
                            {
                                foreach (var journalItem in updateJournalItems)
                                {
                                    if (journalItem.Key == PostingKey.COGS)
                                    {
                                        journalItem.SetDebitCredit(lineTotal, 0);
                                    }
                                    else
                                    {
                                        journalItem.SetDebitCredit(0, lineTotal);
                                    }
                                    journalItemsUpdateEntity.Add(journalItem);
                                    //CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                                }
                            }

                            totalCost += lineTotal;
                        }
                    }

                    itemIssueVendorCredit.UpdateTotal(totalCost);
                    itemIssueVendorCreditsEntity.Add(itemIssueVendorCredit);
                    //CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(itemIssueVendorCredit));

                    // update journal
                    var journal = journalsList.Where(x => x.Id == t.Key.JournalId).FirstOrDefault(); //await _journalManager.GetAsync(t.Key.JournalId, true);
                    journal.UpdateCreditDebit(totalCost, totalCost);
                    journalsEntity.Add(journal);
                    //CheckErrors(await _journalManager.UpdateAsync(journal, null));

                }

                else if (t.Key.JournalType == JournalType.ItemReceiptCustomerCredit)
                {
                    // get item receipt CustomerCredit
                    var itemReceiptCustomerCredit = itemReceiptCustomerCreditList.Where(x => x.Id == t.Key.TransactionId).FirstOrDefault();// await _itemReceiptCustomerCreditManager.GetAsync(t.Key.TransactionId, true);
                    // get item CustomerCredit items
                    var itemReceiptCustomerCreditItems = itemReceiptCustomerCreditItemsList
                                                        .Where(u => u.ItemReceiptCustomerCreditId == t.Key.TransactionId).ToList();
                    //await _itemReceiptCustomerCreditItemRepository.GetAll()
                    //            .Where(u => u.ItemReceiptCustomerCreditId == t.Key.TransactionId).ToListAsync();

                    //var journalItems = await (_journalItemRepository.GetAll()
                    //                      .Where(u => u.Identifier != null &&
                    //                           (u.Key == PostingKey.Inventory || u.Key == PostingKey.COGS) &&
                    //                           u.JournalId == t.Key.JournalId)
                    //                    ).ToListAsync();

                    decimal totalCost = 0;
                    foreach (var c in itemReceiptCustomerCreditItems)
                    {
                        var updatedItem = t.Where(u => u.TransactionItemId == c.Id).FirstOrDefault();
                        if (updatedItem != null)
                        {

                            var lineTotal = Math.Abs(updatedItem.LineTotal);
                            c.UpdateUnitCost(updatedItem.UnitCost, lineTotal);
                            itemReceiptCustomerCreditItemsEntity.Add(c);
                            //CheckErrors(await _itemReceiptItemCustomerCreditManager.UpdateAsync(c));
                            //update journal items 
                            var updateJournalItems = journalItemForItemReceiptList.Where(x => x.Identifier == c.Id).ToList();

                            if (updateJournalItems != null)
                            {
                                foreach (var journalItem in updateJournalItems)
                                {

                                    if (journalItem.Key == PostingKey.COGS)
                                    {
                                        journalItem.SetDebitCredit(0, lineTotal);
                                    }
                                    else
                                    {
                                        journalItem.SetDebitCredit(lineTotal, 0);
                                    }
                                    journalItemsUpdateEntity.Add(journalItem);
                                    //CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                                }
                            }

                            totalCost += lineTotal;
                        }
                    }

                    itemReceiptCustomerCredit.UpdateTotal(totalCost);
                    itemReceiptCustomerCreditEntity.Add(itemReceiptCustomerCredit);
                    //CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(itemReceiptCustomerCredit));

                    // update journal
                    var journal = journalListForItemReceipt.Where(x => x.Id == t.Key.JournalId).FirstOrDefault(); //await _journalManager.GetAsync(t.Key.JournalId, true);

                    journal.UpdateCreditDebit(totalCost, totalCost);
                    journalsEntity.Add(journal);
                    //CheckErrors(await _journalManager.UpdateAsync(journal, null,false));


                }

                else if (t.Key.JournalType == JournalType.ItemIssueAdjustment
                    || t.Key.JournalType == JournalType.ItemIssueOther
                    || t.Key.JournalType == JournalType.ItemIssueTransfer
                    || t.Key.JournalType == JournalType.ItemIssuePhysicalCount)
                {
                    // get item issue //
                    var itemIssue = itemIssuesList.Where(x => x.Id == t.Key.TransactionId).FirstOrDefault();// await _itemIssueManager.GetAsync(t.Key.TransactionId, true);
                    // get item issue items //
                    var itemIssueItems = itemIssueItemsList.Where(x => x.ItemIssueId == t.Key.TransactionId).ToList();

                    decimal totalCost = 0;
                    foreach (var c in itemIssueItems)
                    {
                        var updatedItem = t.Where(u => u.TransactionItemId == c.Id).FirstOrDefault();
                        if (updatedItem != null)
                        {
                            var lineTotal = Math.Abs(updatedItem.LineTotal);

                            c.UpdateUnitCost(updatedItem.UnitCost, lineTotal);
                            itemIssueItemsEntity.Add(c);
                            //CheckErrors(await _itemIssueItemManager.UpdateAsync(c));
                            // update journal items  //
                            var journalItem = journalItemsList.Where(x => x.Identifier == c.Id).FirstOrDefault();
                            if (journalItem != null)
                            {
                                journalItem.SetDebitCredit(0, lineTotal);
                                journalItemsUpdateEntity.Add(journalItem);
                                //CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                            }
                            totalCost += lineTotal;
                        }
                    }

                    itemIssue.UpdateTotal(totalCost);
                    itemIssuesEntity.Add(itemIssue);
                    //CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));

                    // update journal
                    var journal = journalsList.Where(x => x.Id == t.Key.JournalId).FirstOrDefault(); //await _journalManager.GetAsync(t.Key.JournalId, true);

                    // CheckErrors(await AutoRoundingCheckHelperAsync(roundingItemsOuptput, journal, roundingAccountId));

                    journal.UpdateCreditDebit(totalCost, totalCost);
                    journalsEntity.Add(journal);
                    //CheckErrors(await _journalManager.UpdateAsync(journal, null,false));

                    //update sub journal
                    var cogsAccount = journalItemsList.Where(u => u.JournalId == journal.Id &&
                                    u.Key == PostingKey.COGS && u.Identifier == null).FirstOrDefault();
                    //await (_journalItemRepository.GetAll()
                    //  .Where(u => u.JournalId == journal.Id &&
                    //           u.Key == PostingKey.COGS && u.Identifier == null)
                    //  ).FirstOrDefaultAsync();
                    cogsAccount.SetDebitCredit(totalCost, 0);
                    journalItemsUpdateEntity.Add(cogsAccount);
                    //CheckErrors(await _journalItemManager.UpdateAsync(cogsAccount));

                }

                else if (t.Key.JournalType == JournalType.ItemIssueProduction)
                {
                    // get item issue //
                    var itemIssue = itemIssuesList.Where(x => x.Id == t.Key.TransactionId).FirstOrDefault();// await _itemIssueManager.GetAsync(t.Key.TransactionId, true);

                    // get item issue items //
                    var itemIssueItems = itemIssueItemsList.Where(x => x.ItemIssueId == t.Key.TransactionId).ToList();
                    //await _itemIssueItemRepository.GetAll()
                    //    .Where(u => u.ItemIssueId == t.Key.TransactionId).ToListAsync();
                    //var journalItems = await (_journalItemRepository.GetAll()
                    //                      .Where(u => u.Identifier != null &&
                    //                           u.Key == PostingKey.Inventory &&
                    //                           u.JournalId == t.Key.JournalId)
                    //                    ).ToListAsync();

                    decimal totalCost = 0;
                    foreach (var c in itemIssueItems)
                    {
                        var updatedItem = t.Where(u => u.TransactionItemId == c.Id).FirstOrDefault();
                        if (updatedItem != null)
                        {
                            var lineTotal = Math.Abs(updatedItem.LineTotal);
                            c.UpdateUnitCost(updatedItem.UnitCost, lineTotal);
                            itemIssueItemsEntity.Add(c);
                            //CheckErrors(await _itemIssueItemManager.UpdateAsync(c));
                            // update journal items  //
                            var journalItem = journalItemsList.Where(x => x.Identifier == c.Id).FirstOrDefault();
                            if (journalItem != null)
                            {
                                journalItem.SetDebitCredit(0, lineTotal);
                                journalItemsUpdateEntity.Add(journalItem);
                                //CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                            }
                            if (itemIssue.ReceiveFrom == ReceiveFrom.ProductionOrder)
                            {
                                var productionOrderItem = rawMaterialItemsList
                                                        .Where(u => u.ProductionId == itemIssue.ProductionOrderId.Value && u.Id == c.RawMaterialItemId)
                                                        .FirstOrDefault();
                                //await _rawMaterialItemsRepository.GetAll()
                                //    .Where(u => u.ProductionId == itemIssue.ProductionOrderId.Value
                                //     && u.ItemId == c.ItemId)
                                //    .FirstOrDefaultAsync();
                                if (productionOrderItem != null)
                                {
                                    productionOrderItem.UpdateUnitCost(updatedItem.UnitCost, lineTotal);
                                    rawMaterialItemsEntity.Add(productionOrderItem);
                                    //CheckErrors(await _rawMaterialItemManager.UpdateAsync(productionOrderItem));
                                }
                            }
                            totalCost += lineTotal;
                        }
                    }

                    itemIssue.UpdateTotal(totalCost);
                    itemIssuesEntity.Add(itemIssue);
                    //CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));

                    //update produciton order
                    if (itemIssue.ReceiveFrom == ReceiveFrom.ProductionOrder && itemIssue.ProductionOrderId != null)
                    {
                        var productionOrder = productOrderList.Where(x => x.Id == itemIssue.ProductionOrderId.Value).FirstOrDefault();
                        //await _productionOrderManager.GetAsync(itemIssue.ProductionOrderId.Value, true);
                        productionOrder.UpdateTotalRaw(totalCost);
                        productOrderEntity.Add(productionOrder);
                    }
                    // update journal
                    var journal = journalsList.Where(x => x.Id == t.Key.JournalId).FirstOrDefault();// await _journalManager.GetAsync(t.Key.JournalId, true);

                    //  CheckErrors(await AutoRoundingCheckHelperAsync(roundingItemsOuptput, journal, roundingAccountId));

                    journal.UpdateCreditDebit(totalCost, totalCost);
                    journalsEntity.Add(journal);
                    //CheckErrors(await _journalManager.UpdateAsync(journal, null,false));

                    //update sub journal
                    var cogsAccount = journalItemsList.Where(u => u.JournalId == journal.Id &&
                                                    u.Key == PostingKey.COGS && u.Identifier == null)
                                                    .FirstOrDefault();
                    //await (_journalItemRepository.GetAll()
                    //  .Where(u => u.JournalId == journal.Id &&
                    //           u.Key == PostingKey.COGS && u.Identifier == null)
                    //  ).FirstOrDefaultAsync();
                    cogsAccount.SetDebitCredit(totalCost, 0);
                    journalItemsUpdateEntity.Add(cogsAccount);
                    //CheckErrors(await _journalItemManager.UpdateAsync(cogsAccount));

                }


                else if (t.Key.JournalType == JournalType.ItemReceiptTransfer)
                {
                    // get item issue //
                    var itemReceipt = itemReceiptsList.Where(x => x.Id == t.Key.TransactionId).FirstOrDefault();// await _itemReceiptManager.GetAsync(t.Key.TransactionId, true);
                    if (itemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.TransferOrder)
                    {
                        // get item issue items //
                        var itemReceiptItems = itemReceiptItemsList.Where(x => x.ItemReceiptId == t.Key.TransactionId).ToList();
                        //await _itemReceiptItemRepository.GetAll()
                        //.Where(u => u.ItemReceiptId == t.Key.TransactionId).ToListAsync();
                        //var journalItems = await (_journalItemRepository.GetAll()
                        //                      .Where(u => u.Identifier != null &&
                        //                           u.Key == PostingKey.Inventory &&
                        //                           u.JournalId == t.Key.JournalId)
                        //                    ).ToListAsync();

                        decimal totalCost = 0;
                        foreach (var c in itemReceiptItems)
                        {
                            var updatedItem = t.Where(u => u.TransactionItemId == c.Id).FirstOrDefault();

                            if (updatedItem != null)
                            {
                                var lineTotal = Math.Abs(updatedItem.LineTotal);
                                c.UpdateUnitCost(updatedItem.UnitCost, lineTotal);
                                itemReceiptItemsEntity.Add(c);
                                //CheckErrors(await _itemReceiptItemManager.UpdateAsync(c));
                                // update journal items  //
                                var journalItem = journalItemsList.Where(x => x.Identifier == c.Id).FirstOrDefault();
                                if (journalItem != null)
                                {
                                    journalItem.SetDebitCredit(lineTotal, 0);
                                    journalItemsUpdateEntity.Add(journalItem);
                                    //CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                                }
                                totalCost += lineTotal;
                            }
                        }

                        itemReceipt.UpdateTotal(totalCost);
                        itemReceiptsEntity.Add(itemReceipt);
                        //CheckErrors(await _itemReceiptManager.UpdateAsync(itemReceipt));

                        // update journal
                        var journal = journalsList.Where(x => x.Id == t.Key.JournalId).FirstOrDefault();// await _journalManager.GetAsync(t.Key.JournalId, true);
                        journal.UpdateCreditDebit(totalCost, totalCost);
                        journalsEntity.Add(journal);
                        //CheckErrors(await _journalManager.UpdateAsync(journal, null,false));

                        //update sub journal
                        var clearanceAccount = journalItemsList.Where(u => u.JournalId == journal.Id &&
                                                   u.Key == PostingKey.Clearance && u.Identifier == null)
                                                   .FirstOrDefault();
                        //await (_journalItemRepository.GetAll()
                        //      .Where(u => u.JournalId == journal.Id &&
                        //               u.Key == PostingKey.Clearance && u.Identifier == null)
                        //      ).FirstOrDefaultAsync();
                        clearanceAccount.SetDebitCredit(0, totalCost);
                        journalItemsUpdateEntity.Add(clearanceAccount);
                        //CheckErrors(await _journalItemManager.UpdateAsync(clearanceAccount));
                    }
                }

            }

            var itemReceiptIds = result.Where(s => s.JournalType != null &&
                                (s.JournalType == JournalType.ItemReceiptPurchase ||
                                s.JournalType == JournalType.ItemReceiptTransfer ||
                                s.JournalType == JournalType.ItemReceiptProduction ||
                                s.JournalType == JournalType.ItemReceiptAdjustment ||
                                s.JournalType == JournalType.ItemReceiptCustomerCredit ||
                                s.JournalType == JournalType.ItemReceiptOther ||
                                s.JournalType == JournalType.ItemReceiptPhysicalCount)
                                ).Select(s => s.TransactionItemId);



            var oldAdjustmentJournalItems = journalItemForItemReceiptList.Where(s =>
                                            (s.Key == PostingKey.InventoryAdjustmentInv || s.Key == PostingKey.InventoryAdjustmentAdj)).ToList();
            //await _journalItemRepository.GetAll().Where(s => 
            //                            (s.Key == PostingKey.InventoryAdjustmentInv || s.Key == PostingKey.InventoryAdjustmentAdj) && 
            //                            itemReceiptIds.Contains(s.Identifier)).ToListAsync();
            if (oldAdjustmentJournalItems.Any())
            {
                foreach (var delItem in oldAdjustmentJournalItems)
                    journalItemsRemoveEntity.Add(delItem);
                //CheckErrors(await _journalItemManager.RemoveAsync(delItem));
            }

            var adjusmentItems = result.Where(s => s.AdjustmentJournalItem != null).Select(s => s.AdjustmentJournalItem).ToList();
            if (adjusmentItems.Any())
            {
                foreach (var item in adjusmentItems)
                {
                    if (ItemRecieptAdjustmentId == null) throw new UserFriendlyException(L("ItemReceiptAdjusmentAccountIsNotSet"));

                    var journal = journalListForItemReceipt.Where(x => x.Id == item.JournalId).FirstOrDefault();// await _journalManager.GetAsync(item.JournalId, true);

                    var inventoryJournalItem = JournalItem.CreateJournalItem(
                                    journal.TenantId, journal.CreatorUserId, item.JournalId, item.InventoryAccountId,
                                    item.Description, 0, item.Total, PostingKey.InventoryAdjustmentInv, item.ItemReceiptItemId);
                    journalItemsCreateEntity.Add(inventoryJournalItem);
                    //CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));


                    var adjustmentJournalItem = JournalItem.CreateJournalItem(
                                           journal.TenantId, journal.CreatorUserId, item.JournalId, ItemRecieptAdjustmentId.Value,
                                           item.Description, item.Total, 0, PostingKey.InventoryAdjustmentAdj, item.ItemReceiptItemId);
                    journalItemsCreateEntity.Add(adjustmentJournalItem);
                    //CheckErrors(await _journalItemManager.CreateAsync(adjustmentJournalItem));

                    if (item.Total < 0)
                    {
                        var total = Math.Abs(item.Total);

                        inventoryJournalItem.SetDebitCredit(total, 0);
                        adjustmentJournalItem.SetDebitCredit(0, total);
                    }
                }
            }


            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _journalItemRepository.BulkDeleteAsync(journalItemsRemoveEntity);
                    await _journalItemRepository.BulkInsertAsync(journalItemsCreateEntity);
                    await _journalRepository.BulkUpdateAsync(journalsEntity);
                    await _journalItemRepository.BulkUpdateAsync(journalItemsUpdateEntity);
                    await _itemIssueRepository.BulkUpdateAsync(itemIssuesEntity);
                    await _itemIssueItemRepository.BulkUpdateAsync(itemIssueItemsEntity);

                    await _itemReceiptCustomerCreditRepository.BulkUpdateAsync(itemReceiptCustomerCreditEntity);
                    await _itemReceiptCustomerCreditItemRepository.BulkUpdateAsync(itemReceiptCustomerCreditItemsEntity);

                    await _itemIssueVendorCreditRepository.BulkUpdateAsync(itemIssueVendorCreditsEntity);
                    await _itemIssueVendorCreditItemRepository.BulkUpdateAsync(itemIssueVendorCreditItemsEntity);
                    await _itemReceiptRepository.BulkUpdateAsync(itemReceiptsEntity);
                    await _itemReceiptItemRepository.BulkUpdateAsync(itemReceiptItemsEntity);
                    await _productionsRepository.BulkUpdateAsync(productOrderEntity);
                    await _rawMaterialItemsRepository.BulkUpdateAsync(rawMaterialItemsEntity);

                }

                await uow.CompleteAsync();
            }



        }
        #endregion


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Summary_RecalculateAvg,
                      AppPermissions.Pages_Tenant_Report_Inventory_Detail_RecalculateAvg)]
        [UnitOfWork(IsDisabled = true)]
        public async Task RecalculateCost(RecalculateCostInput input)
        {
            await _inventoryManager.RecalculationCostHelper(AbpSession.TenantId.Value, input.FromDate, input.ToDate, input.Items);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Summary_RecalculateAvg,
                      AppPermissions.Pages_Tenant_Report_Inventory_Detail_RecalculateAvg)]
        [UnitOfWork(IsDisabled = true)]
        public async Task Sync(InventoryTransactionItemSyncInput input)
        {
            await SyncHelper(input);
        }


        private async Task SyncHelper(InventoryTransactionItemSyncInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            //var userId = AbpSession.GetUserId();

            var inventoryTansactionOriginalList = new List<InventoryTransactionItem>();
            var inventoryTansactionCacheList = new List<InventoryTransactionItem>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    inventoryTansactionCacheList = await _inventoryTransactionItemRepository.GetAll()
                                                      .Where(s => !input.FromDate.HasValue || s.Date.Date >= input.FromDate.Value.Date)
                                                      .Where(s => !input.ToDate.HasValue || s.Date.Date <= input.ToDate.Value.Date)
                                                      .AsNoTracking()
                                                      .ToListAsync();

                    var itemReceiptItems = await (from ir in _itemReceiptItemRepository.GetAll()
                                                         .AsNoTracking()
                                                  join j in _journalRepository.GetAll()
                                                            .Where(s => s.ItemReceiptId.HasValue)
                                                            .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                            .Where(s => !input.FromDate.HasValue || s.Date.Date >= input.FromDate.Value.Date)
                                                            .Where(s => !input.ToDate.HasValue || s.Date.Date <= input.ToDate.Value.Date)
                                                            .AsNoTracking()
                                                  on ir.ItemReceiptId equals j.ItemReceiptId
                                                  join ji in _journalItemRepository.GetAll()
                                                             .Where(s => s.Key == PostingKey.Inventory)
                                                             .AsNoTracking()
                                                  on ir.Id equals ji.Identifier

                                                  select InventoryTransactionItem.Create(
                                                              tenantId,
                                                              ir.CreatorUserId.Value,
                                                              ir.CreationTime,
                                                              ir.LastModifierUserId,
                                                              ir.LastModificationTime,
                                                              ir.Id,
                                                              ir.ItemReceiptId,
                                                              ir.ItemReceipt.TransferOrderId.HasValue ? ir.ItemReceipt.TransferOrderId : ir.ItemReceipt.ProductionOrderId,
                                                              ir.TransferOrderItemId.HasValue ? ir.TransferOrderItemId : ir.FinishItemId,
                                                              j.Id,
                                                              j.Date,
                                                              j.CreationTimeIndex.Value,
                                                              j.JournalType,
                                                              j.JournalNo,
                                                              j.Reference,
                                                              ir.ItemId,
                                                              ji.AccountId,
                                                              ir.Lot.LocationId,
                                                              ir.Lot.Id,
                                                              ir.Qty,
                                                              ir.UnitCost,
                                                              ir.Total,
                                                              true,
                                                              ir.Description)
                                              )
                                              .ToListAsync();

                    var itemReceiptCustomerCreditItems = await (from ir in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                       .AsNoTracking()
                                                                join j in _journalRepository.GetAll()
                                                                          .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                                          .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                                          .Where(s => !input.FromDate.HasValue || s.Date.Date >= input.FromDate.Value.Date)
                                                                          .Where(s => !input.ToDate.HasValue || s.Date.Date <= input.ToDate.Value.Date)
                                                                          .AsNoTracking()
                                                                on ir.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                                join ji in _journalItemRepository.GetAll()
                                                                           .Where(s => s.Key == PostingKey.Inventory)
                                                                           .AsNoTracking()
                                                                on ir.Id equals ji.Identifier

                                                                select InventoryTransactionItem.Create(
                                                                        tenantId,
                                                                        ir.CreatorUserId.Value,
                                                                        ir.CreationTime,
                                                                        ir.LastModifierUserId,
                                                                        ir.LastModificationTime,
                                                                        ir.Id,
                                                                        ir.ItemReceiptCustomerCreditId,
                                                                        null,
                                                                        null,
                                                                        j.Id,
                                                                        j.Date,
                                                                        j.CreationTimeIndex.Value,
                                                                        j.JournalType,
                                                                        j.JournalNo,
                                                                        j.Reference,
                                                                        ir.ItemId,
                                                                        ji.AccountId,
                                                                        ir.Lot.LocationId,
                                                                        ir.Lot.Id,
                                                                        ir.Qty,
                                                                        ir.UnitCost,
                                                                        ir.Total,
                                                                        true,
                                                                        ir.Description)
                                                            )
                                                            .ToListAsync();

                    var itemIssueItems = await (from ir in _itemIssueItemRepository.GetAll()
                                                       .AsNoTracking()
                                                join j in _journalRepository.GetAll()
                                                          .Where(s => s.ItemIssueId.HasValue)
                                                          .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                          .Where(s => !input.FromDate.HasValue || s.Date.Date >= input.FromDate.Value.Date)
                                                          .Where(s => !input.ToDate.HasValue || s.Date.Date <= input.ToDate.Value.Date)
                                                          .AsNoTracking()
                                                on ir.ItemIssueId equals j.ItemIssueId
                                                join ji in _journalItemRepository.GetAll()
                                                           .Where(s => s.Key == PostingKey.Inventory)
                                                           .AsNoTracking()
                                                on ir.Id equals ji.Identifier

                                                select InventoryTransactionItem.Create(
                                                    tenantId,
                                                    ir.CreatorUserId.Value,
                                                    ir.CreationTime,
                                                    ir.LastModifierUserId,
                                                    ir.LastModificationTime,
                                                    ir.Id,
                                                    ir.ItemIssueId,
                                                    ir.ItemIssue.TransferOrderId.HasValue ? ir.ItemIssue.TransferOrderId : ir.ItemIssue.ProductionOrderId,
                                                    ir.TransferOrderItemId.HasValue ? ir.TransferOrderItemId : ir.RawMaterialItemId,
                                                    j.Id,
                                                    j.Date,
                                                    j.CreationTimeIndex.Value,
                                                    j.JournalType,
                                                    j.JournalNo,
                                                    j.Reference,
                                                    ir.ItemId,
                                                    ji.AccountId,
                                                    ir.Lot.LocationId,
                                                    ir.Lot.Id,
                                                    -ir.Qty,
                                                    ir.UnitCost,
                                                    -ir.Total,
                                                    false,
                                                    ir.Description)
                                            ).ToListAsync();

                    var itemIssueVendorCreditItems = await (from ir in _itemIssueVendorCreditItemRepository.GetAll()
                                                                   .AsNoTracking()
                                                            join j in _journalRepository.GetAll()
                                                                      .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                                      .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                                      .Where(s => !input.FromDate.HasValue || s.Date.Date >= input.FromDate.Value.Date)
                                                                      .Where(s => !input.ToDate.HasValue || s.Date.Date <= input.ToDate.Value.Date)
                                                                      .AsNoTracking()
                                                            on ir.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                            join ji in _journalItemRepository.GetAll()
                                                                       .Where(s => s.Key == PostingKey.Inventory)
                                                                       .AsNoTracking()
                                                            on ir.Id equals ji.Identifier
                                                            select InventoryTransactionItem.Create(
                                                                tenantId,
                                                                ir.CreatorUserId.Value,
                                                                ir.CreationTime,
                                                                ir.LastModifierUserId,
                                                                ir.LastModificationTime,
                                                                ir.Id,
                                                                ir.ItemIssueVendorCreditId,
                                                                null,
                                                                null,
                                                                j.Id,
                                                                j.Date,
                                                                j.CreationTimeIndex.Value,
                                                                j.JournalType,
                                                                j.JournalNo,
                                                                j.Reference,
                                                                ir.ItemId,
                                                                ji.AccountId,
                                                                ir.Lot.LocationId,
                                                                ir.Lot.Id,
                                                                -ir.Qty,
                                                                ir.UnitCost,
                                                                -ir.Total,
                                                                false,
                                                                ir.Description)
                                                        )
                                                        .ToListAsync();

                    inventoryTansactionOriginalList.AddRange(itemReceiptItems);
                    inventoryTansactionOriginalList.AddRange(itemReceiptCustomerCreditItems);
                    inventoryTansactionOriginalList.AddRange(itemIssueItems);
                    inventoryTansactionOriginalList.AddRange(itemIssueVendorCreditItems);

                    //Old Transaction Items Outside Date Range
                    var syncIds = inventoryTansactionOriginalList.Select(s => s.Id).ToHashSet();
                    var existingOutsideDateRangeList = await _inventoryTransactionItemRepository.GetAll()
                                                            .Where(s => (input.FromDate.HasValue && s.Date.Date < input.FromDate.Value.Date) ||
                                                                        (input.ToDate.HasValue && s.Date.Date > input.ToDate.Value.Date))
                                                            .Where(s => syncIds.Contains(s.Id))
                                                            .AsNoTracking()
                                                            .ToListAsync();

                    inventoryTansactionCacheList.AddRange(existingOutsideDateRangeList);

                }
            }



            var inventoryTansactionOriginalDic = inventoryTansactionOriginalList.ToDictionary(s => s.Id, s => s);
            var inventoryTansactionCacheDic = inventoryTansactionCacheList.ToDictionary(s => s.Id, s => s);

            var deleteInventoryTansactionList = inventoryTansactionCacheList.Where(s => !inventoryTansactionOriginalDic.ContainsKey(s.Id)).ToList();
            var createInventoryTansactionList = inventoryTansactionOriginalList.Where(s => !inventoryTansactionCacheDic.ContainsKey(s.Id)).ToList();
            var updateInventoryTansactionList = inventoryTansactionOriginalList.Where(s => inventoryTansactionCacheDic.ContainsKey(s.Id) &&
                                                                                        !s.IsSameAs(inventoryTansactionCacheDic[s.Id])).ToList();

            var syncProductions = deleteInventoryTansactionList
                                .Concat(createInventoryTansactionList)
                                .Concat(updateInventoryTansactionList)
                                .Where(s => s.JournalType == JournalType.ItemIssueProduction || s.JournalType == JournalType.ItemReceiptProduction)
                                .Where(s => s.TransferOrProductionId.HasValue)
                                .GroupBy(s => s.TransferOrProductionId.Value)
                                .Select(s => s.Key)
                                .ToHashSet();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (deleteInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(deleteInventoryTansactionList);
                    if (createInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(createInventoryTansactionList);
                    if (updateInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkUpdateAsync(updateInventoryTansactionList);

                    if (syncProductions.Any())
                    {
                        var productions = await _productionsRepository.GetAll()
                                         .Where(s => syncProductions.Contains(s.Id))
                                         .AsNoTracking()
                                         .ToListAsync();
                        foreach (var p in productions)
                        {
                            p.SetCalculateionState(CalculationState.Synced);
                        }

                        if (productions.Any()) await _productionsRepository.BulkUpdateAsync(productions);
                    }

                }
                await uow.CompleteAsync();
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Detail)]
        [HttpPost]
        public async Task<PagedResultDto<InventoryValuationDetailItem>> GetInventoryValuationDetailReport(GetListInventoryValuationInput input)
        {

            //var periodCycles = await GetCloseCyleAsync(input.FromDate);
            var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);

            var userId = AbpSession.GetUserId();
            //var fromDate = input.FromDate == null ? (DateTime?)null : input.FromDate.Value.AddDays(-1);


            //begining
            var fromDateBeginning = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;
            var toDateBeginning = input.FromDate.AddDays(-1);

            var inventoryDetailFromDate = _inventoryManager.GetItemCostSummaryQuery(fromDateBeginning, toDateBeginning, input.Locations, userId, input.Items)
                                            .Select(u => new ItemDto
                                            {
                                                TransferOrderItemId = null,
                                                IsPurchase = u.Qty > 0,
                                                ItemId = u.ItemId,
                                                TransactionId = null,
                                                TotalCost = u.TotalCost,
                                                UnitCost = u.Cost,
                                                Qty = u.Qty,
                                                LocationId = u.LocationId,
                                                LocationName = u.LocationName,
                                                JournalDate = toDateBeginning,
                                                RoundingDigit = u.RoundingDigit,
                                                RoundingDigitUnitCost = u.RoundingDigitUnitCost,

                                                TotalLocationCost = u.TotalCost,
                                                TotalLocationQty = u.Qty,
                                                Description = u.Description
                                            });

            //var inventoryDetailFromDate = fromDate == null ? null : _inventoryManager.GetInventoryValuationDetailQuery(fromDate.Value, input.Locations, input.Items, userId);


            //from
            var inventoryDetailToDate = _inventoryManager.GetInventoryValuationDetailQuery(toDateBeginning, input.ToDate, input.Locations, input.Items, userId);


            //var query = inventoryDetailFromDate.Concat(inventoryDetailToDate)
            //            .WhereIf(input.Locations != null && input.Locations.Count > 0,
            //                   t => input.Locations.Contains(t.LocationId))
            //            .OrderBy(o => o.ItemCode)
            //            .ThenBy(o => o.LocationId)
            //            .ThenBy(o => o.JournalDate.Date)
            //            .ThenBy(o => o.CreationTimeIndex)
            //            .ThenBy(o => o.CreationTime)
            //            .Select(s => s);


            var query = inventoryDetailFromDate
                        .Concat(inventoryDetailToDate)
                        .OrderBy(o => $"{o.ItemId}-{o.LocationId}-{o.OrderIndex}");


            //var result = (from q in query
            //              join i in GetItemPropertiesQuery()
            //              .WhereIf(input.Items != null && input.Items.Count() > 0, t => input.Items.Contains(t.ItemId))
            //              .WhereIf(input.InventoryAccounts != null && input.InventoryAccounts.Count() > 0, t => input.InventoryAccounts.Contains(t.InventoryAccountId))
            //              .WhereIf(input.AccountTypes != null && input.AccountTypes.Count() > 0, t => t.AccountTypeId != null && input.AccountTypes.Contains(t.AccountTypeId))
            //              .WhereIf(
            //                    !input.Filter.IsNullOrWhiteSpace(),
            //                    p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
            //                        p.ItemCode.ToLower().Contains(input.Filter.ToLower())
            //                )
            //                .AsNoTracking()
            //                on q.ItemId equals i.ItemId

            //              select new InventoryValuationDetailItem
            //              {
            //                  TransferOrderItemId = q.TransferOrderItemId,
            //                  JournalType = q.JournalType,
            //                  Date = q.JournalDate,
            //                  ItemCode = i.ItemCode,
            //                  GroupKey = i.ItemCode + " " + q.LocationName,
            //                  ItemName = i.ItemName,
            //                  ItemId = i.ItemId,
            //                  InventoryAccountId = i.InventoryAccountId.Value,
            //                  JournalNo = q.JournalNo,
            //                  JournalId = q.JournalId,
            //                  CreationTimeIndex = q.CreationTimeIndex,
            //                  TransactionId = q.TransactionId,
            //                  TransactionItemId = q.TransactionItemId,
            //                  LocationId = q.LocationId,
            //                  LocationName = q.LocationName,
            //                  OrderId = q.OrderId,
            //                  RoundingDigit = q.RoundingDigit,
            //                  RoundingDigitUnitCost = q.RoundingDigitUnitCost,
            //                  Reference = q.JournalReference,
            //                  Properties = i.Properties,
            //                  InQty = q.IsPurchase ? q.Qty : 0,
            //                  OutQty = q.IsPurchase ? 0 : q.Qty,
            //                  UnitCost = q.UnitCost,
            //                  LineTotal = q.TotalCost,
            //                  TotalQty = 0,
            //                  TotalCost = 0,
            //                  AVGCost = 0,
            //                  Description = q.Description,
            //              })
            //                .WhereIf(input.ItemProperties != null && input.ItemProperties.Count > 0, p => (p.Properties.Where(i =>
            //                                       input.ItemProperties.Any(v => v.PropertyId == i.PropertyId) &&
            //                                       input.ItemProperties.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
            //                                       (input.ItemProperties.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
            //                                       input.ItemProperties.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
            //                                       input.ItemProperties.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.ValueId)))
            //                           .Count() == input.ItemProperties.Count))
            //                ;



            var accountTypes = input.AccountTypes == null ? null : input.AccountTypes.Where(s => s.HasValue).Select(s => s.Value).ToList();
            var itemQuery = GetItemWithPropertiesQuery(input.Filter, input.Items, input.InventoryAccounts, accountTypes, input.ItemProperties);

            var result = from q in query
                         join i in itemQuery
                         on q.ItemId equals i.ItemId

                         select new InventoryValuationDetailItem
                         {
                             TransferOrderItemId = q.TransferOrderItemId,
                             JournalType = q.JournalType,
                             Date = q.JournalDate,
                             ItemCode = i.ItemCode,
                             GroupKey = i.ItemCode + " " + q.LocationName,
                             ItemName = i.ItemName,
                             ItemId = i.ItemId,
                             InventoryAccountId = i.InventoryAccountId.Value,
                             JournalNo = q.JournalNo,
                             JournalId = q.JournalId,
                             CreationTimeIndex = q.CreationTimeIndex,
                             TransactionId = q.TransactionId,
                             TransactionItemId = q.TransactionItemId,
                             LocationId = q.LocationId,
                             LocationName = q.LocationName,
                             OrderId = q.OrderId,
                             RoundingDigit = q.RoundingDigit,
                             RoundingDigitUnitCost = q.RoundingDigitUnitCost,
                             Reference = q.JournalReference,
                             Properties = i.Properties,
                             InQty = q.IsPurchase ? q.Qty : 0,
                             OutQty = q.IsPurchase ? 0 : q.Qty,
                             UnitCost = q.UnitCost,
                             LineTotal = q.TotalCost,
                             TotalQty = 0,
                             TotalCost = 0,
                             AVGCost = 0,
                             Description = q.Description,
                             TransactionNo = q.TransactionNo
                         };


            var totalCount = await result.CountAsync();
            var @entities = new List<InventoryValuationDetailItem>();

            if (totalCount == 0) return new PagedResultDto<InventoryValuationDetailItem>(totalCount, @entities);

            if (input.UsePagination == true)
            {
                @entities = await result.OrderBy(t => t.ItemCode).Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.OrderBy(t => t.ItemCode).ToListAsync();

            }
            return new PagedResultDto<InventoryValuationDetailItem>(totalCount, @entities);
        }


        #region Export to PDF

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Pdf)]
        public async Task<FileDto> ExportPdfInventoryReport(GetInventoryReportInput input)
        {
            input.UsePagination = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var inventoryData = (await GetInventoryReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }
                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                //decimal totalTableWidth = 0;
                //foreach (var i in viewHeader)
                //{
                //    if (i.Visible)
                //    {
                //        var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                //        contentHeader += rowHeader;
                //        totalTableWidth += i.ColumnLength;
                //    }
                //}
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());
                #region Row Body              

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<GetListInventoryReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListInventoryReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListInventoryReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.ItemCode + " - " + a.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListInventoryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountCode + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    var p = row.PropertySummary.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                    if (keyName == "Account")
                                    {
                                        trGroup += $"<td>{row.InventoryAccountCode} - {row.InventoryAccountName}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (keyName == "Lot")
                                    {
                                        trGroup += $"<td>{row.LotName}</td>";
                                    }
                                    else if (keyName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (keyName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "AverageCost")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.AverageCost, rounding.RoundingDigitUnitCost, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "DisplayTotalCost")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.DisplayTotalCost, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "SalePrice")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (p != null)
                                    {
                                        trGroup += $"<td>{p.Value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalInQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalOutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalQtyOnHand), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "DisplayTotalCost")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.DisplayTotalCost), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;
                }
                else // write body no group by
                {
                    foreach (var row in inventoryData.Items)
                    {
                        var tr = "<tr style=\"page-break-inside:avoid; border: 1px solid #000;\">";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                var p = row.PropertySummary.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                if (keyName == "Account")
                                {
                                    tr += $"<td>{row.InventoryAccountCode} - {row.InventoryAccountName}</td>";
                                }
                                else if (keyName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (keyName == "Lot")
                                {
                                    tr += $"<td>{row.LotName}</td>";
                                }
                                else if (keyName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (keyName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (keyName == "Beginning")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalInQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalOutQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalQtyOnHand")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "AverageCost")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.AverageCost, rounding.RoundingDigitUnitCost, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "DisplayTotalCost")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.DisplayTotalCost, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "SalePrice")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "NetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (p != null)
                                {
                                    tr += $"<td>{p.Value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style=\"page-break-inside:auto;\">";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && inventoryData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(inventoryData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                var htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Pdf)]
        public async Task<FileDto> ExportPdfInventoryTransactionReport(GetInventoryTransactionExportReportInput input)
        {
            input.UsePagination = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;
            var inventoryData = (await GetInventoryTransactionReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }
                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());
                #region Row Body              

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<GetListTransactionReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "LocationName":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.ItemCode + " - " + a.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "JournalTypeName":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.JournalTypeName)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.JournalTypeName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "JournalTransactionType":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.JournalTransactionTypeId)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(s => s.Issue).FirstOrDefault() == true ? t.Select(x => L("ItemIssueSale") + " - " + x.JournalTransactionTypeName).FirstOrDefault() : t.Select(x => L("ItemReceipt") + " - " + x.JournalTransactionTypeName).FirstOrDefault(),
                                    Items = t.Select(x =>
                                    {
                                        x.JournalTransactionTypeName = x.Issue == true ?
                                        L("ItemIssueSale") + " - " + x.JournalTransactionTypeName :
                                        L("ItemReceiptPurchase") + " - " + x.JournalTransactionTypeName;
                                        return x;
                                    }).ToList()
                                })
                                .ToList();
                            break;
                        case "JournalNo":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.JournalNo)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.JournalNo).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Type":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.Type)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.Type).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "PartnerName":
                            groupBy = inventoryData.Items
                                .GroupBy(t => t.PartnerName)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.PartnerName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    if (input.GroupBy == "JournalTypeName" && input.ShowGroupHeaderIfNoRecords)
                    {
                        foreach (var g in inventoryData.Groups)
                        {

                            var k = new GetListTransactionReportGroupByOutput
                            {
                                KeyName = g,
                                Items = new List<GetListInventoryTransationReportOutput>(),
                            };

                            var find = groupBy.Find(s => s.KeyName == g);
                            if (find != null) k = find;

                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;page-break-inside:avoid;'>" +
                                                "<td style='font-weight: bold;' colspan=" + reportCountColHead + ">" +
                                                k.KeyName + " </td></tr>";

                            if (k.Items.Count() == 0)
                            {
                                trGroup += $"<tr style='page-break-before: auto; page-break-after: auto;page-break-inside:avoid;'><td colspan=\"{reportCountColHead}\"><div style=\"text-align:center;\">{L("NoRecords")}</div></td></tr>";
                            }

                            foreach (var row in k.Items)
                            {
                                trGroup += "<tr style='page-break-before: auto; page-break-after: auto;page-break-inside:avoid;'>";
                                foreach (var i in viewHeader)
                                {
                                    if (i.Visible)
                                    {
                                        var keyName = i.ColumnName;

                                        var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                        if (keyName == "JournalTypeName")
                                        {
                                            trGroup += $"<td>{row.JournalTypeName}</td>";
                                        }
                                        else if (keyName == "JournalNo")
                                        {
                                            trGroup += $"<td>{row.JournalNo}</td>";
                                        }
                                        else if (keyName == "Date")
                                        {
                                            trGroup += $"<td>{row.Date.ToString(formatDate)}</td>";
                                        }
                                        else if (keyName == "Type")
                                        {
                                            trGroup += $"<td>{row.Type}</td>";
                                        }
                                        else if (keyName == "ItemCode")
                                        {
                                            trGroup += $"<td>{row.ItemCode}</td>";
                                        }
                                        else if (keyName == "ItemName")
                                        {
                                            trGroup += $"<td>{row.ItemName}</td>";
                                        }
                                        else if (keyName == "Reference")
                                        {
                                            trGroup += $"<td>{row.Reference}</td>";
                                        }
                                        else if (keyName == "Beginning")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalInQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalOutQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "LocationName")
                                        {
                                            trGroup += $"<td>{row.LocationName}</td>";
                                        }
                                        else if (keyName == "Lot")
                                        {
                                            trGroup += $"<td>{row.LotName}</td>";
                                        }
                                        else if (keyName == "NetWeight")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "PartnerName")
                                        {
                                            trGroup += $"<td>{row.PartnerName}</td>";
                                        }
                                        else if (keyName == "Memo")
                                        {
                                            trGroup += $"<td>{row.Memo}</td>";
                                        }
                                        else if (keyName == "User")
                                        {
                                            trGroup += $"<td>{row.User}</td>";
                                        }
                                        else if (p != null)
                                        {
                                            trGroup += $"<td>{p.Value}</td>";
                                        }
                                        else if (keyName == "Description")
                                        {
                                            trGroup += $"<td>{row.Description}</td>";
                                        }
                                        else
                                        {
                                            trGroup += $"<td></td>";
                                        }
                                    }
                                }
                                trGroup += "</tr>";
                            }

                            // sum footer of group 
                            trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (!string.IsNullOrEmpty(i.AllowFunction))
                                    {
                                        var keyName = i.ColumnName;
                                        if (keyName == "Beginning")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalInQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalInQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalOutQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalOutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "NetWeight")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }
                    }
                    else
                    {
                        foreach (var k in groupBy)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;page-break-inside:avoid;'>" +
                                                "<td style='font-weight: bold;' colspan=" + reportCountColHead + ">" +
                                                k.KeyName + " </td></tr>";

                            foreach (var row in k.Items)
                            {
                                trGroup += "<tr style='page-break-before: auto; page-break-after: auto;page-break-inside:avoid;'>";
                                foreach (var i in viewHeader)
                                {
                                    if (i.Visible)
                                    {
                                        var keyName = i.ColumnName;

                                        var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                        if (keyName == "JournalTypeName")
                                        {
                                            trGroup += $"<td>{row.JournalTypeName}</td>";
                                        }
                                        else if (keyName == "JournalNo")
                                        {
                                            trGroup += $"<td>{row.JournalNo}</td>";
                                        }
                                        else if (keyName == "Date")
                                        {
                                            trGroup += $"<td>{row.Date.ToString(formatDate)}</td>";
                                        }
                                        else if (keyName == "Type")
                                        {
                                            trGroup += $"<td>{row.Type}</td>";
                                        }
                                        else if (keyName == "ItemCode")
                                        {
                                            trGroup += $"<td>{row.ItemCode}</td>";
                                        }
                                        else if (keyName == "JournalTransactionType")
                                        {
                                            trGroup += $"<td>{row.JournalTransactionTypeName}</td>";
                                        }

                                        else if (keyName == "ItemName")
                                        {
                                            trGroup += $"<td>{row.ItemName}</td>";
                                        }
                                        else if (keyName == "Reference")
                                        {
                                            trGroup += $"<td>{row.Reference}</td>";
                                        }
                                        else if (keyName == "Beginning")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalInQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalOutQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "LocationName")
                                        {
                                            trGroup += $"<td>{row.LocationName}</td>";
                                        }
                                        else if (keyName == "Lot")
                                        {
                                            trGroup += $"<td>{row.LotName}</td>";
                                        }
                                        else if (keyName == "NetWeight")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "PartnerName")
                                        {
                                            trGroup += $"<td>{row.PartnerName}</td>";
                                        }
                                        else if (keyName == "Memo")
                                        {
                                            trGroup += $"<td>{row.Memo}</td>";
                                        }
                                        else if (keyName == "User")
                                        {
                                            trGroup += $"<td>{row.User}</td>";
                                        }
                                        else if (keyName == "Description")
                                        {
                                            trGroup += $"<td>{row.Description}</td>";
                                        }
                                        else if (keyName == "BatchSerial")
                                        {
                                            var batchs = row.ItemBatchNos == null || !row.ItemBatchNos.Any() ? "" : string.Join(", ", row.ItemBatchNos.Select(s => $"{s.BatchNumber}" + (s.Qty == 1 ? "" : $"({s.Qty})")));
                                            trGroup += $"<td>{batchs}</td>";
                                        }
                                        else if (keyName == "ItemBatchSerial")
                                        {
                                            var batchs = row.ItemBatchNos == null || !row.ItemBatchNos.Any() ? "" : string.Join(", ", row.ItemBatchNos.Select(s => $"{s.BatchNumber}" + (s.Qty == 1 ? "" : $"({s.Qty})")));
                                            trGroup += $"<td>{row.ItemName} <br> {batchs}</td>";
                                        }
                                        else if (p != null)
                                        {
                                            trGroup += $"<td>{p.Value}</td>";
                                        }
                                        else
                                        {
                                            trGroup += $"<td></td>";
                                        }
                                    }
                                }
                                trGroup += "</tr>";
                            }

                            // sum footer of group 
                            trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (!string.IsNullOrEmpty(i.AllowFunction))
                                    {
                                        var keyName = i.ColumnName;
                                        if (keyName == "Beginning")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalInQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalInQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalOutQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalOutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "TotalQty")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (keyName == "NetWeight")
                                        {
                                            trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }
                    }

                    contentBody += trGroup;
                }
                else // write body no group by
                {
                    foreach (var row in inventoryData.Items)
                    {
                        var tr = "<tr style='page-break-inside: avoid'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                                if (keyName == "JournalTypeName")
                                {
                                    tr += $"<td>{row.JournalTypeName}</td>";
                                }
                                else if (keyName == "JournalNo")
                                {
                                    tr += $"<td>{row.JournalNo}</td>";
                                }
                                else if (keyName == "Date")
                                {
                                    tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                                }
                                else if (keyName == "Type")
                                {
                                    tr += $"<td>{row.Type}</td>";
                                }
                                else if (keyName == "JournalTransactionType")
                                {
                                    row.JournalTransactionTypeName = row.Issue == true ? L("ItemIssueSale") + " - " + row.JournalTransactionTypeName : L("ItemReceiptPurchase") + " - " + row.JournalTransactionTypeName;
                                    tr += $"<td>{row.JournalTransactionTypeName}</td>";
                                }
                                else if (keyName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (keyName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (keyName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (keyName == "Beginning")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalInQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalOutQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "LocationName")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (keyName == "Lot")
                                {
                                    tr += $"<td>{row.LotName}</td>";
                                }
                                else if (keyName == "NetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "PartnerName")
                                {
                                    tr += $"<td>{row.PartnerName}</td>";
                                }
                                else if (keyName == "Memo")
                                {
                                    tr += $"<td>{row.Memo}</td>";
                                }
                                else if (keyName == "User")
                                {
                                    tr += $"<td>{row.User}</td>";
                                }
                                else if (keyName == "Description")
                                {
                                    tr += $"<td>{row.Description}</td>";
                                }
                                else if (keyName == "BatchSerial")
                                {
                                    var batchs = row.ItemBatchNos == null || !row.ItemBatchNos.Any() ? "" : string.Join(", ", row.ItemBatchNos.Select(s => $"{s.BatchNumber}" + (s.Qty == 1 ? "" : $"({s.Qty})")));
                                    tr += $"<td>{batchs}</td>";
                                }
                                else if (keyName == "ItemBatchSerial")
                                {
                                    var batchs = row.ItemBatchNos == null || !row.ItemBatchNos.Any() ? "" : string.Join(", ", row.ItemBatchNos.Select(s => $"{s.BatchNumber}" + (s.Qty == 1 ? "" : $"({s.Qty})")));
                                    tr += $"<td>{row.ItemName} <br> {batchs}</td>";
                                }
                                else if (p != null)
                                {
                                    tr += $"<td>{p.Value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-inside: avoid'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && inventoryData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(inventoryData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                var htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_Detail_Export_Pdf)]
        public async Task<FileDto> ExportPdfInventoryValuationDetailReport(GetInventoryValuationDetailReportInput input)
        {
            input.UsePagination = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var inventoryData = (await GetInventoryValuationDetailReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                if (formatDate.IsNullOrEmpty())
                {
                    formatDate = "dd/MM/yyyy";
                }
                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());
                #region Row Body              

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = inventoryData.Items
                                .GroupBy(t => t.ItemId)
                                .Select(x =>
                                {
                                    decimal runningQty = 0;
                                    decimal runningCost = 0;
                                    decimal avgCost = 0;
                                    decimal oldTotalQty = 0;

                                    var r = new InventoryValuationDetailItemGroupBy
                                    {
                                        KeyName = x.Select(a => a.ItemCode + " - " + a.ItemName).FirstOrDefault(),
                                        Items = x.Select(t =>
                                        {
                                            t.UnitCost = Math.Round(t.UnitCost, t.RoundingDigitUnitCost);
                                            var i = t;
                                            oldTotalQty = runningQty;
                                            runningQty += t.InQty + t.OutQty;
                                            runningCost += t.LineTotal;

                                            if (oldTotalQty < 0 && t.JournalType != null)
                                            {
                                                runningCost = Math.Round(runningQty * t.UnitCost, t.RoundingDigit);
                                            }


                                            avgCost = Math.Round(Math.Abs(+(runningQty == 0 ? 0 : runningCost / runningQty)), t.RoundingDigitUnitCost);

                                            runningCost = runningQty == 0 ? 0 : runningCost;

                                            t.TotalQty = runningQty;
                                            t.TotalCost = runningCost;
                                            t.AVGCost = avgCost;
                                            return i;
                                        }).ToList()
                                    };
                                    return r;
                                })

                                .ToList();
                // write body

                if (groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";
                        int index = 1;
                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                    if (keyName == "No")
                                    {
                                        trGroup += $"<td>{index}</td>";
                                        index++;
                                    }
                                    else if (keyName == "JournalNo")
                                    {
                                        trGroup += $"<td>{row.JournalNo}</td>";
                                    }
                                    else if (keyName == "Date")
                                    {
                                        trGroup += $"<td>{row.Date.ToString(formatDate)}</td>";
                                    }
                                    else if (keyName == "JournalType")
                                    {
                                        trGroup += $"<td>{row.JournalType}</td>";
                                    }
                                    else if (keyName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (keyName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (keyName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td align='right'>{row.LocationName}</td>";
                                    }
                                    else if (keyName == "InQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.InQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (keyName == "TransactionNo")
                                    {
                                        trGroup += $"<td>{row.TransactionNo}</td>";
                                    }
                                    else if (keyName == "Lot")
                                    {
                                        trGroup += $"<td align='right'>{row.LotName}</td>";
                                    }
                                    else if (keyName == "UnitCost")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.UnitCost, rounding.RoundingDigitUnitCost, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "LineTotal")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.LineTotal, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalCost")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalCost, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "AVGCost")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.AVGCost, rounding.RoundingDigitUnitCost, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Description")
                                    {
                                        trGroup += $"<td>{row.Description}</td>";
                                    }
                                    else if (p != null)
                                    {
                                        trGroup += $"<td>{p.Value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                    }

                    contentBody += trGroup;
                }

                #endregion Row Body

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                var htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });
        }

        #endregion

        #region Inventory Transaction Report 

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListInventoryTransationReportOutput>> GetInventoryTransactionReport(InventoryTransactionInput input)
        {
            input.MaxResultCount = 25;//set default max result 
            var popertyFilterList = input.ItemProperties;
            var userId = AbpSession.GetUserId();
            var query = _inventoryManager.GetInventoryTransactionReportQuery(
                                             input.FromDate,
                                             input.ToDate,
                                             input.Locations,
                                             input.Lots,
                                             input.Items,
                                             input.JournalTypes,
                                             input.Users,
                                             userId,
                                            !input.GroupBy.IsNullOrWhiteSpace() && input.GroupBy == "Item",
                                             input.JournalTransactionTypeIds,
                                             input.InventoryTypes);



            var accountTypes = input.AccountTypes == null ? null : input.AccountTypes.Where(s => s.HasValue).Select(s => s.Value).ToList();
            var accounts = input.InventoryAccounts == null ? null : input.InventoryAccounts.Select(s => (Guid?)s).ToList();
            var itemQuery = GetItemWithPropertiesQuery("", input.Items, accounts, accountTypes, input.ItemProperties);

            var result = from u in query
                                    .WhereIf(!input.Filter.IsNullOrEmpty(),
                                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                            u.JournalReference.ToLower().Contains(input.Filter.ToLower()))
                         join p in itemQuery
                         on u.ItemId equals p.ItemId
                         let properties = p.Properties
                         select new GetListInventoryTransationReportOutput
                         {
                             JournalTransactionTypeId = u.JournalTransactionTypeId,
                             Issue = u.Issue,
                             JournalTransactionTypeName = u.JournalTransactionTypeName,
                             Id = u.TransactionId,
                             Properties = properties,
                             Date = u.JournalDate,
                             Type = u.IsPurchase == true ? L("InventoryIn") : L("InventoryOut"),
                             JournalType = u.JournalType,
                             JournalNo = u.JournalNo,
                             ItemCode = p.ItemCode,
                             ItemName = p.ItemName,

                             LocationId = u.LocationId,
                             LocationName = u.LocationName,
                             LotId = u.LotId,
                             LotName = u.LotName,
                             Memo = u.JournalMemo,
                             PartnerName = u.PartnerName,
                             Reference = u.JournalReference,
                             TotalQty = u.Qty,
                             Beginning = u.JournalType == null ? u.Qty : 0,
                             TotalInQty = u.IsPurchase && u.JournalType != null ? u.Qty : 0,
                             TotalOutQty = !u.IsPurchase && u.JournalType != null ? -u.Qty : 0,
                             User = u.UserName,
                             CreationTimeIndex = u.CreationTimeIndex,
                             CreationTime = u.CreationTime,
                             Description = u.Description,
                             ItemBatchNos = u.ItemBatchNos,
                             Unit = properties.Any(r => r.IsUnit) ?
                                       ObjectMapper.Map<UnitDto>(p.Properties.Select(t => t).FirstOrDefault(t => t.IsUnit))
                                       : null,
                             Rank = u.JournalType == JournalType.ItemReceiptPurchase || u.JournalType == JournalType.ItemReceiptOther || u.JournalType == JournalType.ItemReceiptAdjustment ? 0
                                   : u.JournalType == JournalType.ItemIssueTransfer || u.JournalType == JournalType.ItemIssueProduction ? 1
                                   : u.JournalType == JournalType.ItemReceiptTransfer || u.JournalType == JournalType.ItemReceiptProduction ? 2
                                   : u.JournalType == JournalType.ItemIssueVendorCredit ? 3 //purchase return
                                   : u.JournalType == JournalType.ItemIssueSale || u.JournalType == JournalType.ItemIssueOther || u.JournalType == JournalType.ItemIssueAdjustment ? 4
                                   : u.JournalType == JournalType.ItemReceiptCustomerCredit ? 5 //sale return
                                   : u.JournalType == JournalType.ItemReceiptPhysicalCount ? 6
                                   : u.JournalType == JournalType.ItemIssuePhysicalCount ? 7
                                   : 8,
                             ProductionProcessName = u.ProductionProcessName,
                             //JournalTypeName = u.JournalType != null ? u.JournalType.ToString() : null
                             JournalTypeName = u.JournalType != null ?
                                               (u.JournalType.Value == JournalType.ItemIssueProduction ? L("ItemIssueWithProduction", u.ProductionProcessName != null && u.ProductionProcessName != "" ? u.ProductionProcessName : L("Production")) :
                                               u.JournalType == JournalType.ItemIssueVendorCredit ? L("ItemIssueWithVendorCredit") :
                                               u.JournalType == JournalType.ItemReceiptProduction ? L("ItemReceiptWithProduction", u.ProductionProcessName != null && u.ProductionProcessName != "" ? u.ProductionProcessName : L("Production")) :
                                               u.JournalType == JournalType.ItemReceiptCustomerCredit ? L("ItemReceiptWithCustomerCredit") :
                                               u.JournalType == JournalType.ItemReceiptOther ? L("ItemReceiptWithOther") :
                                               u.JournalType == JournalType.ItemIssueOther ? L("ItemIssueWithOther") :
                                               u.JournalType == JournalType.ItemReceiptTransfer ? L("ItemReceiptWithTransfer") :
                                               u.JournalType == JournalType.ItemIssueTransfer ? L("ItemIssueWithTransfer") :
                                               u.JournalType == JournalType.ItemReceiptPurchase ? L("ItemReceiptWithPurchase") :
                                               u.JournalType == JournalType.ItemIssueSale ? L("ItemIssueWithSale") :
                                               u.JournalType == JournalType.ItemReceiptPhysicalCount ? L("ItemReceiptWithPhysicalCount") :
                                               u.JournalType == JournalType.ItemIssuePhysicalCount ? L("ItemIssueWithPhysicalCount") :
                                               u.JournalType == JournalType.ItemReceiptAdjustment ? L("ItemReceiptWithAdjustment") :
                                               u.JournalType == JournalType.ItemIssueAdjustment ? L("ItemIssueWithAdjustment") :
                                               "") : L("Beginning")
                         };


            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }

            var @entities = new List<GetListInventoryTransationReportOutput>();
            var sumOfColumns = new Dictionary<string, decimal>();

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<GetListInventoryTransationReportOutput>(resultCount, @entities, sumOfColumns);
            }

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await result.Select(t => new { t.NetWeight, t.TotalQty, t.TotalOutQty, t.TotalInQty, t.Beginning }).ToListAsync();

                //var totalQty = sumList.Select(u => u.TotalInQty - u.TotalOutQty).Sum();


                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "NetWeight") sumOfColumns.Add(c, sumList.Select(u => u.NetWeight).Sum());
                    else if (c == "TotalQty") sumOfColumns.Add(c, sumList.Select(u => u.TotalQty).Sum());
                    else if (c == "Beginning") sumOfColumns.Add(c, sumList.Select(u => u.Beginning).Sum());
                    else if (c == "TotalInQty") sumOfColumns.Add(c, sumList.Select(u => u.TotalInQty).Sum());
                    else if (c == "TotalOutQty") sumOfColumns.Add(c, sumList.Select(u => u.TotalOutQty).Sum());
                }
            }



            //var orderBy = "Date.Date, CreationTimeIndex, CreationTime";
            if (!input.GroupBy.IsNullOrWhiteSpace())
            {
                switch (input.GroupBy)
                {
                    case "Item":
                        //orderBy = "ItemCode," + orderBy;
                        result = result.OrderBy(s => s.ItemCode).ThenBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex).ThenBy(s => s.CreationTime);
                        break;
                    case "LocationName":
                        //orderBy = "LocationId," + orderBy;
                        result = result.OrderBy(s => s.LocationId).ThenBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex).ThenBy(s => s.CreationTime);
                        break;
                    case "Lot":
                        //orderBy = "LotId," + orderBy;
                        result = result.OrderBy(s => s.LotId).ThenBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex).ThenBy(s => s.CreationTime);
                        break;
                    case "JournalTypeName":
                        //orderBy = "Rank, JournalType," + orderBy;
                        result = result.OrderBy(s => s.Rank).ThenBy(s => s.JournalType).ThenBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex).ThenBy(s => s.CreationTime);
                        break;
                    case "JournalTransactionType":
                        //orderBy = "Rank, JournalType," + orderBy;
                        result = result.OrderBy(s => s.JournalTransactionTypeName).ThenBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex).ThenBy(s => s.CreationTime);
                        break;
                    default:
                        //orderBy = input.GroupBy + "," + orderBy;
                        result = result.OrderBy(input.GroupBy).ThenBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex).ThenBy(s => s.CreationTime);
                        break;
                }
            }

            //result = result.OrderBy(orderBy);


            if (input.UsePagination == true)
            {
                @entities = await result.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.ToListAsync();
            }

            var outputResult = new PagedResultWithTotalColuumnsDto<GetListInventoryTransationReportOutput>(resultCount, @entities, sumOfColumns);

            if (input.GroupBy == "JournalTypeName" && input.ShowGroupHeaderIfNoRecords)
            {
                var groups = new List<string> {
                    L("ItemReceiptWithPurchase"),
                    L("ItemIssueWithSale"),
                    L("ItemIssueWithVendorCredit"),
                    L("ItemReceiptWithCustomerCredit"),
                    L("ItemReceiptWithOther"),
                    L("ItemIssueWithOther"),
                    L("ItemReceiptWithTransfer"),
                    L("ItemIssueWithTransfer"),
                    L("ItemReceiptWithAdjustment"),
                    L("ItemIssueWithAdjustment"),
                };

                var productionProcess = await _productionProcessRepository.GetAll().ToListAsync();
                if (productionProcess.Count() > 1)
                {
                    foreach (var i in productionProcess)
                    {
                        groups.Add(L("ItemReceiptWithProduction", i.ProcessName));
                        groups.Add(L("ItemIssueWithProduction", i.ProcessName));
                    }
                }
                else
                {
                    groups.Add(L("ItemReceiptWithProduction", L("Production")));
                    groups.Add(L("ItemIssueWithProduction", L("Production")));
                }

                groups.Add(L("ItemReceiptWithPhysicalCount"));
                groups.Add(L("ItemIssueWithPhysicalCount"));

                outputResult.Groups = groups;
            }

            return outputResult;
        }

        public ReportOutput GetReportTemplateInventoryTransaction()
        {
            var itemProperty = _propertyRepository.GetAll().AsNoTracking()
                                .Where(t => t.IsActive == true)
                                .Select(t => new CollumnOutput
                                {
                                    AllowGroupby = false,
                                    AllowFilter = true,
                                    ColumnName = t.Name,
                                    ColumnLength = 150,
                                    ColumnTitle = t.Name,
                                    ColumnType = ColumnType.ItemProperty,
                                    SortOrder = 17,
                                    Visible = true,
                                    AllowFunction = null,
                                    MoreFunction = null,
                                    IsDisplay = true,
                                    AllowShowHideFilter = true,
                                    ShowHideFilter = true,
                                    DefaultValue = t.Id.ToString(),//to init the value of property
                                });
            var columnInfo = new List<CollumnOutput>() {
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = false,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryAccount",
                        ColumnLength = 330,
                        ColumnTitle = "InventoryAccount",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 200,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },
                    //new CollumnOutput{
                    //    AllowGroupby = true,
                    //    AllowFilter = false,
                    //    ColumnName = "JournalTypeName",
                    //    ColumnLength = 150,
                    //    ColumnTitle = "Journal Type",
                    //    ColumnType = ColumnType.String,
                    //    SortOrder = 2,
                    //    Visible = true,
                    //    AllowFunction = null,
                    //    IsDisplay = true,
                    //    AllowShowHideFilter = true,
                    //    ShowHideFilter = true,
                    //},
                
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 150,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 100,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "Type",
                        ColumnLength = 150,
                        ColumnTitle = "Stock Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 200,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 170,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false

                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "LocationName",
                        ColumnLength = 200,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },
                     new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Lot",
                        ColumnLength = 200,
                        ColumnTitle = "Zone",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 150,
                        ColumnTitle = "Total Qty In",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 150,
                        ColumnTitle = "Total Qty Out",
                        ColumnType = ColumnType.Number,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQty",
                        ColumnLength = 150,
                        ColumnTitle = "Total Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = "Sum",
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "PartnerName",
                        ColumnLength = 200,
                        ColumnTitle = "Partner",
                        ColumnType = ColumnType.String,
                        SortOrder = 15,
                        Visible = true,
                        IsDisplay = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Memo",
                        ColumnLength = 170,
                        ColumnTitle = "Memo",
                        ColumnType = ColumnType.String,
                        SortOrder = 16,
                        Visible = true,
                        IsDisplay = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 140,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 17,
                        Visible = true,
                        IsDisplay = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ShowGroupHeaderIfNoRecords",
                        ColumnLength = 150,
                        ColumnTitle = "ShowGroupHeaderIfNoRecords",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        DisableDefault = true,
                        ShowHideFilter = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 140,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 18,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchSerial",
                        ColumnLength = 170,
                        ColumnTitle = "Batch/Serial",
                        ColumnType = ColumnType.Array,
                        SortOrder = 19,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    }
                    ,
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemBatchSerial",
                        ColumnLength = 170,
                        ColumnTitle = "Item Batch/Serial",
                        ColumnType = ColumnType.Array,
                        SortOrder = 20,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false
                    },
                       new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryTypes",
                        ColumnLength = 200,
                        ColumnTitle = L("InventoryType"),
                        ColumnType = ColumnType.String,
                        SortOrder = 21,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                       new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "JournalTransactionType",
                        ColumnLength = 200,
                        ColumnTitle = "Transaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },


                };

            var result = columnInfo.Concat(itemProperty).ToList();

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                result = result.Where(s => s.ColumnName != "InventoryAccount" && s.ColumnName != "AccountType").ToList();
            }

            var list = new ReportOutput()
            {
                ColumnInfo = result,
                Groupby = "",
                HeaderTitle = "Inventory Transaction",
                Sortby = "",

            };

            return list;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Excel)]
        public async Task<FileDto> ExportExcelInventoryTransactionReport(GetInventoryTransactionExportReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var resultData = await GetInventoryTransactionReport(input);
            var inventoryData = resultData.Items.ToList();
            inventoryData = inventoryData.Select(s =>
            {
                s.JournalTransactionTypeName = s.Issue == true ? L("ItemIssueSale") + " - " + s.JournalTransactionTypeName : L("ItemReceiptPurchase") + " - " + s.JournalTransactionTypeName;
                return s;
            }).ToList();
            var result = new FileDto();

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
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListTransactionReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "LocationName":
                            groupBy = inventoryData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = inventoryData
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "JournalTypeName":
                            groupBy = inventoryData
                                .GroupBy(t => t.JournalTypeName)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.JournalTypeName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "JournalTransactionType":
                            groupBy = inventoryData
                                .GroupBy(t => t.JournalTransactionTypeName)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.JournalTransactionTypeName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "JournalNo":
                            groupBy = inventoryData
                                .GroupBy(t => t.JournalNo)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.JournalNo).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "PartnerName":
                            groupBy = inventoryData
                                .GroupBy(t => t.PartnerName)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.PartnerName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Type":
                            groupBy = inventoryData
                                .GroupBy(t => t.Type)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.Type).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListTransactionReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                decimal totalQtyInFooter = 0;
                decimal totalQtyOutFooter = 0;
                decimal totalBeginingFooter = 0;
                decimal totalNetweightFooter = 0;
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    if (input.GroupBy == "JournalTypeName" && input.ShowGroupHeaderIfNoRecords)
                    {
                        foreach (var g in resultData.Groups)
                        {
                            var k = new GetListTransactionReportGroupByOutput
                            {
                                KeyName = g,
                                Items = new List<GetListInventoryTransationReportOutput>(),
                            };

                            var find = groupBy.Find(s => s.KeyName == g);
                            if (find != null) k = find;


                            //key group by name
                            AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                            MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                            rowBody += 1;
                            count = 1;

                            if (k.Items.Count() == 0)
                            {
                                AddTextToCell(ws, rowBody, 1, L("NoRecords"));
                                rowBody += 1;
                                count += 1;
                            }

                            //list of item
                            foreach (var i in k.Items)
                            {
                                int collumnCellBody = 1;
                                foreach (var item in reportCollumnHeader)// map with correct key of properties 
                                {
                                    WriteBodyTransactionValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                                    collumnCellBody += 1;
                                }
                                rowBody += 1;
                                count += 1;
                            }

                            //sub total of group by item
                            int collumnCellGroupBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                if (item.AllowFunction != null)
                                {
                                    var totalQtyIn = k.Items.Sum(x => x.TotalInQty);
                                    var totalQtyOut = k.Items.Sum(x => x.TotalOutQty);
                                    var totalBeginning = k.Items.Sum(x => x.Beginning);
                                    if (item.ColumnName == "TotalInQty")
                                    {
                                        totalQtyInFooter += totalQtyIn;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, totalQtyIn, true);
                                    }
                                    else if (item.ColumnName == "TotalOutQty")
                                    {
                                        totalQtyOutFooter += totalQtyOut;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, totalQtyOut, true);
                                    }
                                    else if (item.ColumnName == "Beginning")
                                    {
                                        totalBeginingFooter += totalBeginning;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, totalBeginning, true);
                                    }
                                    else if (item.ColumnName == "NetWeight")
                                    {
                                        var NetWeight = k.Items.Sum(t => t.NetWeight);
                                        totalNetweightFooter += NetWeight;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, NetWeight, true);
                                    }
                                    else if (item.ColumnName == "TotalQty")
                                    {
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, (totalBeginning + totalQtyIn) - totalQtyOut, true);
                                    }
                                    var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                    var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                    if (footerGroupDict.ContainsKey(item.ColumnName))
                                    {
                                        footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                    }
                                    else
                                    {
                                        footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                    }
                                    //AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);

                                }
                                collumnCellGroupBody += 1;
                            }
                            rowBody += 1;
                        }
                    }
                    else
                    {
                        foreach (var k in groupBy)
                        {
                            //key group by name
                            AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                            MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                            rowBody += 1;
                            count = 1;
                            //list of item
                            foreach (var i in k.Items)
                            {
                                int collumnCellBody = 1;
                                foreach (var item in reportCollumnHeader)// map with correct key of properties 
                                {
                                    WriteBodyTransactionValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                                    collumnCellBody += 1;
                                }
                                rowBody += 1;
                                count += 1;
                            }

                            //sub total of group by item
                            int collumnCellGroupBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                if (item.AllowFunction != null)
                                {
                                    var totalQtyIn = k.Items.Sum(x => x.TotalInQty);
                                    var totalQtyOut = k.Items.Sum(x => x.TotalOutQty);
                                    var totalBeginning = k.Items.Sum(x => x.Beginning);
                                    if (item.ColumnName == "TotalInQty")
                                    {
                                        totalQtyInFooter += totalQtyIn;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, totalQtyIn, true);
                                    }
                                    else if (item.ColumnName == "TotalOutQty")
                                    {
                                        totalQtyOutFooter += totalQtyOut;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, totalQtyOut, true);
                                    }
                                    else if (item.ColumnName == "Beginning")
                                    {
                                        totalBeginingFooter += totalBeginning;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, totalBeginning, true);
                                    }
                                    else if (item.ColumnName == "NetWeight")
                                    {
                                        var NetWeight = k.Items.Sum(t => t.NetWeight);
                                        totalNetweightFooter += NetWeight;
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, NetWeight, true);
                                    }
                                    else if (item.ColumnName == "TotalQty")
                                    {
                                        AddNumberToCell(ws, rowBody, collumnCellGroupBody, (totalBeginning + totalQtyIn) - totalQtyOut, true);
                                    }
                                    var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                    var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                    if (footerGroupDict.ContainsKey(item.ColumnName))
                                    {
                                        footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                    }
                                    else
                                    {
                                        footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                    }
                                    //AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);

                                }
                                collumnCellGroupBody += 1;
                            }
                            rowBody += 1;
                        }
                    }

                }
                else
                {
                    foreach (var i in inventoryData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyTransactionValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                //var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                //if (i.ColumnType == ColumnType.Number)
                                //{
                                //    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                //}
                                //else
                                //{
                                //    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                //}
                                var totalQtyIn = totalQtyInFooter; //groupBy.Sum(t => t.Items.Sum(x => x.TotalInQty));
                                var totalQtyOut = totalQtyOutFooter;// groupBy.Sum(t => t.Items.Sum(x => x.TotalOutQty));
                                var netWeight = totalNetweightFooter;// groupBy.Sum(t => t.Items.Sum(x => x.NetWeightExcelExport));
                                if (i.ColumnName == "TotalInQty")
                                {
                                    AddNumberToCell(ws, footerRow, footerColNumber, totalQtyIn, true);
                                }
                                else if (i.ColumnName == "TotalOutQty")
                                {
                                    AddNumberToCell(ws, footerRow, footerColNumber, totalQtyOut, true);
                                }
                                else if (i.ColumnName == "NetWeight")
                                {
                                    AddNumberToCell(ws, footerRow, footerColNumber, netWeight, true);
                                }
                                else if (i.ColumnName == "TotalQty")
                                {
                                    AddNumberToCell(ws, footerRow, footerColNumber, totalQtyIn - totalQtyOut, true);
                                }
                                else if (i.ColumnName == "Beginning")
                                {
                                    AddNumberToCell(ws, footerRow, footerColNumber, totalBeginingFooter, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"InventoryTransaction_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Stock_Balance, AppPermissions.Pages_Tenant_Common_Stock_Balance)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>> GetStockBalanceReport(GetStockBalanceInput input)
        {
            var userId = AbpSession.GetUserId();

            //var itemDics = input.Items == null ? null : input.Items.ToDictionary(u => u, u => u);

            var query = _inventoryManager.GetItemsBalanceForReport(input.Filter, input.FromDate, input.ToDate, input.Location, input.Lots, input.InventoryAccount, input.Items, input.PropertyDics, userId, input.JournalTransactionTypeIds);

            //var popertyFilterList = input.PropertyDics;
            //var result = (from q in query.AsNoTracking()
            //              join i in GetItemPropertiesQuery()
            //              .WhereIf(input.Items != null && input.Items.Count() > 0, t => input.Items.Contains(t.ItemId))
            //              .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count() > 0,
            //                        t => input.InventoryAccount.Contains(t.InventoryAccountId))
            //              .WhereIf(input.AccountTypes != null && input.AccountTypes.Count() > 0,
            //                        t =>  t.AccountTypeId != null && input.AccountTypes.Contains(t.AccountTypeId))
            //              .WhereIf(
            //                    !input.Filter.IsNullOrWhiteSpace(),
            //                    p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
            //                        p.ItemCode.ToLower().Contains(input.Filter.ToLower()))
            //              .AsNoTracking()
            //              on q.ItemId equals i.ItemId

            //              select new GetListStockBalanceReportOutputNew
            //              {
            //                  LotId = q.LotId,
            //                  LotName = q.LotName,
            //                  Id = i.ItemId,
            //                  ItemCode = i.ItemCode,
            //                  ItemName = i.ItemName,
            //                  SalePrice = i.SalePrice ?? 0,
            //                  TotalQtyOnHand = q.Qty,
            //                  TotalInQty = q.InQty,
            //                  Beginning = q.BeginningQty,
            //                  TotalOutQty = q.OutQty,
            //                  StockBalanceStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero : q.Qty > 0 ? InventoryStockBalanceStatus.Positive : InventoryStockBalanceStatus.Negative,
            //                  TotalCost = 0,
            //                  InventoryAccountId = i.InventoryAccountId,
            //                  InventoryAccountName = i.InventoryAccountName,
            //                  InventoryAccountCode = i.InventoryAccountCode,
            //                  LocationId = q.LocationId,
            //                  LocationName = q.LocationName,
            //                  RoundingDigit = 2,
            //                  RoundingDigitUnitCost = 2,
            //                  Properties = i.Properties,

            //                  OutNetWeight = i.Properties == null ? 0 : i.Properties
            //                             .Where(t => t.IsUnit)
            //                             .Select(t => t.NetWeight * q.OutQty).FirstOrDefault(),

            //                  NetWeight = i.Properties == null ? 0 : i.Properties
            //                             .Where(t => t.IsUnit)
            //                             .Select(t => t.NetWeight * q.Qty).FirstOrDefault(),
            //                  AverageCost = 0
            //              })
            //              .WhereIf(input.StockBalance != null && input.StockBalance.Count() > 0,
            //                            t => input.StockBalance.Contains(t.StockBalanceStatus))
            //              .WhereIf(input.StockMovement != null && input.StockMovement.Count() > 0,
            //                            t => (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && t.Beginning != 0) ||
            //                                 (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && t.TotalInQty != 0) ||
            //                                 (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && t.TotalOutQty != 0)
            //                            )
            //              .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
            //                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.ValueId)))
            //                             .Count() == popertyFilterList.Count));

            var accountTypes = input.AccountTypes == null ? null : input.AccountTypes.Where(s => s.HasValue).Select(s => s.Value).ToList();
            var itemQuery = GetItemWithPropertiesQuery(input.Filter, input.Items, input.InventoryAccount, accountTypes, input.PropertyDics);

            var result = from q in query.AsNoTracking()
                         join i in itemQuery
                         on q.ItemId equals i.ItemId

                         let stockBalanceStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero : q.Qty > 0 ? InventoryStockBalanceStatus.Positive : InventoryStockBalanceStatus.Negative
                         let unit = i.Properties.Where(s => s.IsUnit).FirstOrDefault()
                         let netWeight = unit == null ? 0 : unit.NetWeight

                         where (
                                   input.StockBalance == null ||
                                   input.StockBalance.Count() == 0 ||
                                   input.StockBalance.Contains(stockBalanceStatus)
                               ) &&
                               (
                                   input.StockMovement == null ||
                                   input.StockMovement.Count() == 0 ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && q.BeginningQty != 0) ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && q.InQty != 0) ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && q.OutQty != 0)
                               )

                         select new GetListStockBalanceReportOutputNew
                         {
                             LotId = q.LotId,
                             LotName = q.LotName,
                             Id = i.ItemId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             SalePrice = i.SalePrice ?? 0,
                             TotalQtyOnHand = q.Qty,
                             TotalInQty = q.InQty,
                             Beginning = q.BeginningQty,
                             TotalOutQty = q.OutQty,
                             StockBalanceStatus = stockBalanceStatus,
                             TotalCost = 0,
                             InventoryAccountId = i.InventoryAccountId,
                             InventoryAccountName = i.InventoryAccountName,
                             InventoryAccountCode = i.InventoryAccountCode,
                             LocationId = q.LocationId,
                             LocationName = q.LocationName,
                             RoundingDigit = 2,
                             RoundingDigitUnitCost = 2,
                             Properties = i.Properties,
                             OutNetWeight = netWeight * q.OutQty,
                             NetWeight = netWeight * q.Qty,
                             AverageCost = 0,
                             Max = i.Max,
                             Min = i.Min
                         };


            var resultCount = 0;
            var sumOfColumns = new Dictionary<string, decimal>();
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>(resultCount, new List<GetListStockBalanceReportOutputNew>(), sumOfColumns);
            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await result.Select(t => new { t.NetWeight, t.TotalQtyOnHand, t.Beginning, t.TotalInQty, t.TotalOutQty, t.OutNetWeight }).ToListAsync();
                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "NetWeight") sumOfColumns.Add(c, sumList.Select(u => u.NetWeight).Sum());
                    else if (c == "Beginning") sumOfColumns.Add(c, result.Select(u => u.Beginning).Sum());
                    else if (c == "TotalInQty") sumOfColumns.Add(c, result.Select(u => u.TotalInQty).Sum());
                    else if (c == "TotalOutQty") sumOfColumns.Add(c, result.Select(u => u.TotalOutQty).Sum());
                    else if (c == "TotalQtyOnHand") sumOfColumns.Add(c, sumList.Select(u => u.TotalQtyOnHand).Sum());
                    else if (c == "OutNetWeight") sumOfColumns.Add(c, sumList.Select(u => u.OutNetWeight).Sum());
                }
            }


            if (!input.GroupBy.IsNullOrWhiteSpace())
            {
                switch (input.GroupBy)
                {
                    case "Item":
                        result = result.OrderBy(s => s.ItemCode);
                        break;
                    case "Account":
                        result = result.OrderBy(s => s.InventoryAccountCode);
                        break;
                    case "Location":
                        result = result.OrderBy(s => s.LocationId);
                        break;
                    default:
                        //group only 4 so default is lot
                        result = result.OrderBy(s => s.LotId);
                        break;
                }

            }

            var @entities = new List<GetListStockBalanceReportOutputNew>();

            if (input.UsePagination == true)
            {
                @entities = await result.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>(resultCount, @entities, sumOfColumns);
        }

        public ReportOutput GetReportTemplateStock()
        {
            var itemProperty = _propertyRepository.GetAll().AsNoTracking()
                               .Where(t => t.IsActive == true)
                               .Select(t => new CollumnOutput
                               {
                                   AllowGroupby = false,
                                   AllowFilter = true,
                                   ColumnName = t.Name,
                                   ColumnLength = 150,
                                   ColumnTitle = t.Name,
                                   ColumnType = ColumnType.ItemProperty,
                                   SortOrder = 11,
                                   Visible = true,
                                   AllowFunction = null,
                                   MoreFunction = null,
                                   IsDisplay = true,
                                   AllowShowHideFilter = true,
                                   ShowHideFilter = true,
                                   DefaultValue = t.Id.ToString(),//to init the value of property
                               });
            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = true,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "StockBalance",
                        ColumnLength = 300,
                        ColumnTitle = "Stock Balance",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = InventoryStockBalanceStatus.Positive.ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryMovement",
                        ColumnLength = 300,
                        ColumnTitle = "Inventory Movement",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 180,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 250,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 180,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        DefaultValue = "Location",
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Lot",
                        ColumnLength = 180,
                        ColumnTitle = "Zone",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total In Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total Out Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OutNetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Out Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 170,
                        ColumnTitle = "Ending Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SalePrice",
                        ColumnLength = 170,
                        ColumnTitle = "Sale Price",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Min",
                        ColumnLength = 180,
                        ColumnTitle = "Min",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Max",
                        ColumnLength = 180,
                        ColumnTitle = "Max",
                        ColumnType = ColumnType.Number,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryTypes",
                        ColumnLength = 200,
                        ColumnTitle = L("InventoryType"),
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalTransactionType",
                        ColumnLength = 200,
                        ColumnTitle = "Transaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },


                };
            var totalCols = columnInfo.Concat(itemProperty).ToList();

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                totalCols = totalCols.Where(s => s.ColumnName != "Account" && s.ColumnName != "AccountType").ToList();
            }

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = totalCols,
                Groupby = "",
                HeaderTitle = "Stock Balance",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "InventoryTransaction",
                    ColumnTitle = L("InventoryTransactionTemplate"),
                    DefaultValue = ReportType.ReportType_InventoryTransaction
                }
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Stock_Balance_Export_Excel)]
        public async Task<FileDto> ExportExcelStockBalanceReport(GetStockBalanceReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var inventoryData = (await GetStockBalanceReport(input)).Items;

            var result = new FileDto();

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

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListStockBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = inventoryData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountName + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = inventoryData
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyStockValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in inventoryData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyStockValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Stock_Balance_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Stock_Balance_Export_Pdf)]
        public async Task<FileDto> ExportPdfStockBalanceReport(GetStockBalanceReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var stockBalanceData = (await GetStockBalanceReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "StockBalanceReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentGroupby = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<GetListStockBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountCode + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Lot":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                    if (keyName == "Lot")
                                    {
                                        trGroup += $"<td>{row.LotName}</td>";
                                    }
                                    else if (keyName == "Account")
                                    {
                                        trGroup += $"<td>{row.InventoryAccountCode + "-" + row.InventoryAccountName}</td>";
                                    }
                                    else if (keyName == "Min")
                                    {
                                        trGroup += $"<td>{FormatNumberCurrency(Math.Round(row.Min, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Max")
                                    {
                                        trGroup += $"<td>{FormatNumberCurrency(Math.Round(row.Max, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (keyName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "SalePrice")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (p != null)
                                    {
                                        trGroup += $"<td>{p.Value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalInQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalOutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalQtyOnHand), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.OutNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;

                }
                else // write body no group by
                {
                    foreach (var row in stockBalanceData.Items)
                    {
                        var tr = "<tr>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                if (keyName == "Lot")
                                {
                                    tr += $"<td>{row.LotName}</td>";
                                }
                                else if (keyName == "Account")
                                {
                                    tr += $"<td>{row.InventoryAccountCode + "-" + row.InventoryAccountName}</td>";
                                }
                                else if (keyName == "Min")
                                {
                                    tr += $"<td>{FormatNumberCurrency(Math.Round(row.Min, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "Max")
                                {
                                    tr += $"<td>{FormatNumberCurrency(Math.Round(row.Max, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (keyName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (keyName == "Beginning")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalInQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalOutQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalQtyOnHand")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (keyName == "NetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "OutNetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "SalePrice")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (p != null)
                                {
                                    tr += $"<td>{p.Value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && stockBalanceData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(stockBalanceData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }

                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });


        }


        #region Asset Report
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Asset_Balance)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListAssetBalanceReportOutput>> GetAssetBalanceReport(GetAssetBalanceInput input)
        {
            var userId = AbpSession.GetUserId();

            var lots = await _lostRepository.GetAllListAsync();

            var query = _inventoryManager.GetAssetBalanceForReport(input.Filter, input.FromDate, input.ToDate, input.Location, input.Lots, input.Items, input.PropertyDics, userId);

            var itemDetailQuery = _inventoryManager.GetItemsBalanceForReport(string.Empty, input.FromDate, input.ToDate, null, null, null, null, null, null, input.JournalTransactionTypeIds);


            //var popertyFilterList = input.PropertyDics;
            //var result = (from q in query.AsNoTracking()
            //              join i in GetItemPropertiesQuery()
            //              .WhereIf(input.Items != null && input.Items.Count() > 0, t => input.Items.Contains(t.ItemId))
            //              .WhereIf(
            //                    !input.Filter.IsNullOrWhiteSpace(),
            //                    p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
            //                        p.ItemCode.ToLower().Contains(input.Filter.ToLower()))
            //              .AsNoTracking()
            //              on q.ItemId equals i.ItemId

            //              join d in itemDetailQuery
            //              on i.ItemCode equals d.LotName
            //              into detail

            //              let dqty = detail.Sum(s => s.Qty)

            //              select new GetListAssetBalanceReportOutput
            //              {
            //                  LotId = q.LotId,
            //                  LotName = q.LotName,
            //                  Id = i.ItemId,
            //                  ItemCode = i.ItemCode,
            //                  ItemName = i.ItemName,
            //                  TotalQtyOnHand = q.Qty,
            //                  TotalInQty = q.InQty,
            //                  Beginning = q.BeginningQty,
            //                  TotalOutQty = q.OutQty,
            //                  StockBalanceStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero : q.Qty > 0 ? InventoryStockBalanceStatus.Positive : InventoryStockBalanceStatus.Negative,
            //                  InventoryAccountId = i.InventoryAccountId,
            //                  InventoryAccountName = i.InventoryAccountName,
            //                  InventoryAccountCode = i.InventoryAccountCode,
            //                  LocationId = q.LocationId,
            //                  LocationName = q.LocationName,
            //                  RoundingDigit = 2,
            //                  Properties = i.Properties,
            //                  Status = $"{dqty:#,###} items",
            //                  LinkLot = lots.Where(l => l.LotName == i.ItemCode).Select(l => l.Id).FirstOrDefault()
            //              })
            //              .WhereIf(input.StockBalance != null && input.StockBalance.Count() > 0,
            //                            t => input.StockBalance.Contains(t.StockBalanceStatus))
            //              .WhereIf(input.StockMovement != null && input.StockMovement.Count() > 0,
            //                            t => (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && t.Beginning != 0) ||
            //                                 (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && t.TotalInQty != 0) ||
            //                                 (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && t.TotalOutQty != 0)
            //                            )
            //              .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
            //                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.ValueId)))
            //                             .Count() == popertyFilterList.Count));


            var itemQuery = GetItemWithPropertiesQuery(input.Filter, input.Items, null, null, input.PropertyDics);

            var result = from q in query.AsNoTracking()
                         join i in itemQuery
                         on q.ItemId equals i.ItemId

                         join d in itemDetailQuery
                         on i.ItemCode equals d.LotName
                         into detail

                         let dQty = detail.Sum(s => s.Qty)
                         let stockBalanceStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero : q.Qty > 0 ? InventoryStockBalanceStatus.Positive : InventoryStockBalanceStatus.Negative

                         where (
                                   input.StockBalance == null ||
                                   input.StockBalance.Count() == 0 ||
                                   input.StockBalance.Contains(stockBalanceStatus)
                               ) &&
                               (
                                   input.StockMovement == null ||
                                   input.StockMovement.Count() == 0 ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && q.BeginningQty != 0) ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && q.InQty != 0) ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && q.OutQty != 0)
                               )

                         select new GetListAssetBalanceReportOutput
                         {
                             LotId = q.LotId,
                             LotName = q.LotName,
                             Id = i.ItemId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             TotalQtyOnHand = q.Qty,
                             TotalInQty = q.InQty,
                             Beginning = q.BeginningQty,
                             TotalOutQty = q.OutQty,
                             StockBalanceStatus = stockBalanceStatus,
                             InventoryAccountId = i.InventoryAccountId,
                             InventoryAccountName = i.InventoryAccountName,
                             InventoryAccountCode = i.InventoryAccountCode,
                             LocationId = q.LocationId,
                             LocationName = q.LocationName,
                             RoundingDigit = 2,
                             Properties = i.Properties,
                             Status = $"{dQty:#,###} items",
                             LinkLot = lots.Where(l => l.LotName == i.ItemCode).Select(l => l.Id).FirstOrDefault()
                         };

            var resultCount = 0;
            var @entities = new List<GetListAssetBalanceReportOutput>();
            var sumOfColumns = new Dictionary<string, decimal>();
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<GetListAssetBalanceReportOutput>(resultCount, @entities, sumOfColumns);

            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await result.Select(t => new { t.TotalQtyOnHand, t.Beginning, t.TotalInQty, t.TotalOutQty }).ToListAsync();
                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "Beginning") sumOfColumns.Add(c, result.Select(u => u.Beginning).Sum());
                    else if (c == "TotalInQty") sumOfColumns.Add(c, result.Select(u => u.TotalInQty).Sum());
                    else if (c == "TotalOutQty") sumOfColumns.Add(c, result.Select(u => u.TotalOutQty).Sum());
                    else if (c == "TotalQtyOnHand") sumOfColumns.Add(c, sumList.Select(u => u.TotalQtyOnHand).Sum());
                }
            }


            if (!input.GroupBy.IsNullOrWhiteSpace())
            {
                switch (input.GroupBy)
                {
                    case "Item":
                        result = result.OrderBy(s => s.ItemCode);
                        break;
                    case "Location":
                        result = result.OrderBy(s => s.LocationId);
                        break;
                    default:
                        //group only 4 so default is lot
                        result = result.OrderBy(s => s.LotId);
                        break;
                }
            }



            if (input.UsePagination == true)
            {
                @entities = await result.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<GetListAssetBalanceReportOutput>(resultCount, @entities, sumOfColumns);
        }

        public ReportOutput GetReportTemplateAsset()
        {
            var itemProperty = _propertyRepository.GetAll().AsNoTracking()
                               .Where(t => t.IsActive == true)
                               .Select(t => new CollumnOutput
                               {
                                   AllowGroupby = false,
                                   AllowFilter = true,
                                   ColumnName = t.Name,
                                   ColumnLength = 150,
                                   ColumnTitle = t.Name,
                                   ColumnType = ColumnType.ItemProperty,
                                   SortOrder = 11,
                                   Visible = true,
                                   AllowFunction = null,
                                   MoreFunction = null,
                                   IsDisplay = true,
                                   AllowShowHideFilter = true,
                                   ShowHideFilter = true,
                                   DefaultValue = t.Id.ToString(),//to init the value of property
                               });
            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = true,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "StockBalance",
                        ColumnLength = 300,
                        ColumnTitle = "Stock Balance",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = InventoryStockBalanceStatus.Positive.ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 180,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    // new CollumnOutput{
                    //    AllowGroupby = false,
                    //    AllowFilter = true,
                    //    ColumnName = "AccountType",
                    //    ColumnLength = 300,
                    //    ColumnTitle = "AccountType",
                    //    ColumnType = ColumnType.String,
                    //    SortOrder = 3,
                    //    Visible = true,
                    //    AllowFunction = null,
                    //    DisableDefault = false,
                    //    IsDisplay = false,
                    //    AllowShowHideFilter = true,
                    //    ShowHideFilter = true,
                    //},
                    //new CollumnOutput{
                    //    AllowGroupby = true,
                    //    AllowFilter = true,
                    //    ColumnName = "Account",
                    //    ColumnLength = 250,
                    //    ColumnTitle = "Account",
                    //    ColumnType = ColumnType.String,
                    //    SortOrder = 2,
                    //    Visible = true,
                    //    AllowFunction = null,
                    //    IsDisplay = true,
                    //    AllowShowHideFilter = true,
                    //    ShowHideFilter = true,
                    //},
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 180,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        DefaultValue = "Location",
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Lot",
                        ColumnLength = 180,
                        ColumnTitle = "Zone",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total In Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total Out Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 170,
                        ColumnTitle = "Ending Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Status",
                        ColumnLength = 100,
                        ColumnTitle = "Status",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryTypes",
                        ColumnLength = 200,
                        ColumnTitle = L("InventoryType"),
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalTransactionType",
                        ColumnLength = 200,
                        ColumnTitle = "Transaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                };
            var totalCols = columnInfo.Concat(itemProperty).ToList();
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = totalCols,
                Groupby = "",
                HeaderTitle = "Asset Balance",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "AssetItemDetail",
                    ColumnTitle = L("AssetItemDetailTemplate"),
                    DefaultValue = ReportType.ReportType_AssetItemDetail
                }
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Excel)]
        public async Task<FileDto> ExportExcelAssetBalanceReport(GetAssetBalanceReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var inventoryData = (await GetAssetBalanceReport(input)).Items;

            var result = new FileDto();

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

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListAssetBalanceReportGroupByOutput>();

                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = inventoryData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountName + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = inventoryData
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyAssetBalance(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in inventoryData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyAssetBalance(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = footerGroupDict.ContainsKey(i.ColumnName) ? "SUM(" + footerGroupDict[i.ColumnName] + ")" : "";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Stock_Balance_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Pdf)]
        public async Task<FileDto> ExportPdfAssetBalanceReport(GetAssetBalanceReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var stockBalances = await GetAssetBalanceReport(input);
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                //FileDto fileDto = new FileDto()
                //{
                //    SubFolder = null,
                //    FileName = "StockBalanceReportPdf.pdf",
                //    FileToken = "InventoryReportPdf.html",
                //    FileType = MimeTypeNames.TextHtml
                //};
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate("InventoryReportPdf.html");//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentGroupby = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<GetListAssetBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = stockBalances.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = stockBalances.Items
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountCode + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = stockBalances.Items
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Lot":
                            groupBy = stockBalances.Items
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListAssetBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                    if (keyName == "Lot")
                                    {
                                        trGroup += $"<td>{row.LotName}</td>";
                                    }
                                    else if (keyName == "Account")
                                    {
                                        trGroup += $"<td>{row.InventoryAccountCode + "-" + row.InventoryAccountName}</td>";
                                    }
                                    else if (keyName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (keyName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    //else if (keyName == "NetWeight")
                                    //{
                                    //    trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    //}
                                    //else if (keyName == "OutNetWeight")
                                    //{
                                    //    trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    //}
                                    //else if (keyName == "SalePrice")
                                    //{
                                    //    trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    //}
                                    else if (p != null)
                                    {
                                        trGroup += $"<td>{p.Value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalInQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalOutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalQtyOnHand), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    //else if (keyName == "NetWeight")
                                    //{
                                    //    trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    //}
                                    //else if (keyName == "OutNetWeight")
                                    //{
                                    //    trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.OutNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    //}
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;

                }
                else // write body no group by
                {
                    foreach (var row in stockBalances.Items)
                    {
                        var tr = "<tr>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                if (keyName == "Lot")
                                {
                                    tr += $"<td>{row.LotName}</td>";
                                }
                                else if (keyName == "Account")
                                {
                                    tr += $"<td>{row.InventoryAccountCode + "-" + row.InventoryAccountName}</td>";
                                }
                                else if (keyName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (keyName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (keyName == "Beginning")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalInQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalOutQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalQtyOnHand")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                //else if (keyName == "NetWeight")
                                //{
                                //    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                //}
                                //else if (keyName == "OutNetWeight")
                                //{
                                //    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                //}
                                //else if (keyName == "SalePrice")
                                //{
                                //    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                //}
                                else if (p != null)
                                {
                                    tr += $"<td>{p.Value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && stockBalances.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(stockBalances.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }

                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });

        }
        #endregion

        #region Asset Itew Detail Report
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Asset_Balance)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>> GetAssetItemDetailReport(GetStockBalanceInput input)
        {
            var userId = AbpSession.GetUserId();

            //var itemDics = input.Items == null ? null : input.Items.ToDictionary(u => u, u => u);

            var query = _inventoryManager.GetItemsBalanceForReport(input.Filter, input.FromDate, input.ToDate, input.Location, input.Lots, input.InventoryAccount, input.Items, input.PropertyDics, userId, input.JournalTransactionTypeIds);
            //var popertyFilterList = input.PropertyDics;
            //var result = (from q in query.AsNoTracking()
            //              join i in GetItemPropertiesQuery()
            //              .WhereIf(input.Items != null && input.Items.Count() > 0, t => input.Items.Contains(t.ItemId))
            //              .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count() > 0,
            //                        t => input.InventoryAccount.Contains(t.InventoryAccountId))
            //              .WhereIf(input.AccountTypes != null && input.AccountTypes.Count() > 0,
            //                        t => t.AccountTypeId != null && input.AccountTypes.Contains(t.AccountTypeId))
            //              .WhereIf(
            //                    !input.Filter.IsNullOrWhiteSpace(),
            //                    p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
            //                        p.ItemCode.ToLower().Contains(input.Filter.ToLower()))
            //              .AsNoTracking()
            //              on q.ItemId equals i.ItemId

            //              select new GetListStockBalanceReportOutputNew
            //              {
            //                  LotId = q.LotId,
            //                  LotName = q.LotName,
            //                  Id = i.ItemId,
            //                  ItemCode = i.ItemCode,
            //                  ItemName = i.ItemName,
            //                  SalePrice = i.SalePrice ?? 0,
            //                  TotalQtyOnHand = q.Qty,
            //                  TotalInQty = q.InQty,
            //                  Beginning = q.BeginningQty,
            //                  TotalOutQty = q.OutQty,
            //                  StockBalanceStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero : q.Qty > 0 ? InventoryStockBalanceStatus.Positive : InventoryStockBalanceStatus.Negative,
            //                  TotalCost = 0,
            //                  InventoryAccountId = i.InventoryAccountId,
            //                  InventoryAccountName = i.InventoryAccountName,
            //                  InventoryAccountCode = i.InventoryAccountCode,
            //                  LocationId = q.LocationId,
            //                  LocationName = q.LocationName,
            //                  RoundingDigit = 2,
            //                  RoundingDigitUnitCost = 2,
            //                  Properties = i.Properties,

            //                  OutNetWeight = i.Properties == null ? 0 : i.Properties
            //                             .Where(t => t.IsUnit)
            //                             .Select(t => t.NetWeight * q.OutQty).FirstOrDefault(),

            //                  NetWeight = i.Properties == null ? 0 : i.Properties
            //                             .Where(t => t.IsUnit)
            //                             .Select(t => t.NetWeight * q.Qty).FirstOrDefault(),
            //                  AverageCost = 0
            //              })
            //              .WhereIf(input.StockBalance != null && input.StockBalance.Count() > 0,
            //                            t => input.StockBalance.Contains(t.StockBalanceStatus))
            //              .WhereIf(input.StockMovement != null && input.StockMovement.Count() > 0,
            //                            t => (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && t.Beginning != 0) ||
            //                                 (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && t.TotalInQty != 0) ||
            //                                 (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && t.TotalOutQty != 0)
            //                            )
            //              .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
            //                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
            //                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.ValueId)))
            //                             .Count() == popertyFilterList.Count));

            var accountTypes = input.AccountTypes == null ? null : input.AccountTypes.Where(s => s.HasValue).Select(s => s.Value).ToList();
            var itemQuery = GetItemWithPropertiesQuery(input.Filter, input.Items, input.InventoryAccount, accountTypes, input.PropertyDics);

            var result = from q in query.AsNoTracking()
                         join i in itemQuery
                         on q.ItemId equals i.ItemId

                         let stockBalanceStatus = q.Qty == 0 ? InventoryStockBalanceStatus.Zero : q.Qty > 0 ? InventoryStockBalanceStatus.Positive : InventoryStockBalanceStatus.Negative
                         let unit = i.Properties.Where(s => s.IsUnit).FirstOrDefault()
                         let netWeight = unit == null ? 0 : unit.NetWeight

                         where (
                                   input.StockBalance == null ||
                                   input.StockBalance.Count() == 0 ||
                                   input.StockBalance.Contains(stockBalanceStatus)
                               ) &&
                               (
                                   input.StockMovement == null ||
                                   input.StockMovement.Count() == 0 ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.Beginning) && q.BeginningQty != 0) ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.StockIn) && q.InQty != 0) ||
                                   (input.StockMovement.Contains(InventoryMovementStatus.StockOut) && q.OutQty != 0)
                               )

                         select new GetListStockBalanceReportOutputNew
                         {
                             LotId = q.LotId,
                             LotName = q.LotName,
                             Id = i.ItemId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             SalePrice = i.SalePrice ?? 0,
                             TotalQtyOnHand = q.Qty,
                             TotalInQty = q.InQty,
                             Beginning = q.BeginningQty,
                             TotalOutQty = q.OutQty,
                             StockBalanceStatus = stockBalanceStatus,
                             TotalCost = 0,
                             InventoryAccountId = i.InventoryAccountId,
                             InventoryAccountName = i.InventoryAccountName,
                             InventoryAccountCode = i.InventoryAccountCode,
                             LocationId = q.LocationId,
                             LocationName = q.LocationName,
                             RoundingDigit = 2,
                             RoundingDigitUnitCost = 2,
                             Properties = i.Properties,
                             OutNetWeight = netWeight * q.OutQty,
                             NetWeight = netWeight * q.Qty,
                             AverageCost = 0
                         };

            var resultCount = 0;
            var sumOfColumns = new Dictionary<string, decimal>();
            var @entities = new List<GetListStockBalanceReportOutputNew>();
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();

                if (resultCount == 0) return new PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>(resultCount, @entities, sumOfColumns);

            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await result.Select(t => new { t.NetWeight, t.TotalQtyOnHand, t.Beginning, t.TotalInQty, t.TotalOutQty, t.OutNetWeight }).ToListAsync();
                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "NetWeight") sumOfColumns.Add(c, sumList.Select(u => u.NetWeight).Sum());
                    else if (c == "Beginning") sumOfColumns.Add(c, result.Select(u => u.Beginning).Sum());
                    else if (c == "TotalInQty") sumOfColumns.Add(c, result.Select(u => u.TotalInQty).Sum());
                    else if (c == "TotalOutQty") sumOfColumns.Add(c, result.Select(u => u.TotalOutQty).Sum());
                    else if (c == "TotalQtyOnHand") sumOfColumns.Add(c, sumList.Select(u => u.TotalQtyOnHand).Sum());
                    else if (c == "OutNetWeight") sumOfColumns.Add(c, sumList.Select(u => u.OutNetWeight).Sum());
                }
            }

            if (!input.GroupBy.IsNullOrWhiteSpace())
            {
                switch (input.GroupBy)
                {
                    case "Item":
                        result = result.OrderBy(s => s.ItemCode);
                        break;
                    case "Account":
                        result = result.OrderBy(s => s.InventoryAccountCode);
                        break;
                    case "Location":
                        result = result.OrderBy(s => s.LocationId);
                        break;
                    default:
                        //group only 4 so default is lot
                        result = result.OrderBy(s => s.LotId);
                        break;
                }
            }


            if (input.UsePagination == true)
            {
                @entities = await result.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>(resultCount, @entities, sumOfColumns);
        }

        public ReportOutput GetReportTemplateAssetItemDetail()
        {
            var itemProperty = _propertyRepository.GetAll().AsNoTracking()
                               .Where(t => t.IsActive == true)
                               .Select(t => new CollumnOutput
                               {
                                   AllowGroupby = false,
                                   AllowFilter = true,
                                   ColumnName = t.Name,
                                   ColumnLength = 150,
                                   ColumnTitle = t.Name,
                                   ColumnType = ColumnType.ItemProperty,
                                   SortOrder = 11,
                                   Visible = true,
                                   AllowFunction = null,
                                   MoreFunction = null,
                                   IsDisplay = true,
                                   AllowShowHideFilter = true,
                                   ShowHideFilter = true,
                                   DefaultValue = t.Id.ToString(),//to init the value of property
                               });
            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = true,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "StockBalance",
                        ColumnLength = 300,
                        ColumnTitle = "Stock Balance",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = InventoryStockBalanceStatus.Positive.ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryMovement",
                        ColumnLength = 300,
                        ColumnTitle = "Inventory Movement",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 180,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 250,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 180,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        DefaultValue = "Location",
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Lot",
                        ColumnLength = 180,
                        ColumnTitle = "Zone",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total In Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 170,
                        ColumnTitle = "Total Out Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OutNetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Out Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 170,
                        ColumnTitle = "Ending Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SalePrice",
                        ColumnLength = 170,
                        ColumnTitle = "Sale Price",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "InventoryTypes",
                        ColumnLength = 200,
                        ColumnTitle = L("InventoryType"),
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalTransactionType",
                        ColumnLength = 200,
                        ColumnTitle = "Transaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                };
            var totalCols = columnInfo.Concat(itemProperty).ToList();

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                totalCols = totalCols.Where(s => s.ColumnName != "Account" && s.ColumnName != "AccountType").ToList();
            }


            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = totalCols,
                Groupby = "",
                HeaderTitle = "Asset Item Detail",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "InventoryTransaction",
                    ColumnTitle = L("InventoryTransactionTemplate"),
                    DefaultValue = ReportType.ReportType_InventoryTransaction
                }
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Excel)]
        public async Task<FileDto> ExportExcelAssetItemDetailReport(GetStockBalanceReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var inventoryData = (await GetStockBalanceReport(input)).Items;

            var result = new FileDto();

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

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListStockBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = inventoryData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountName + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = inventoryData
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = inventoryData
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyStockValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in inventoryData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyStockValuationSummary(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Stock_Balance_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Pdf)]
        public async Task<FileDto> ExportPdfAssetItemDetailReport(GetStockBalanceReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var stockBalanceData = (await GetStockBalanceReport(input));
            var stocBalances = await GetStockBalanceReport(input);
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "StockBalanceReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentGroupby = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<GetListStockBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.InventoryAccountId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.InventoryAccountCode + " - " + x.InventoryAccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Item":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.ItemCode)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Lot":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LotId)
                                .Select(t => new GetListStockBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                    if (keyName == "Lot")
                                    {
                                        trGroup += $"<td>{row.LotName}</td>";
                                    }
                                    else if (keyName == "Account")
                                    {
                                        trGroup += $"<td>{row.InventoryAccountCode + "-" + row.InventoryAccountName}</td>";
                                    }
                                    else if (keyName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (keyName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "SalePrice")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (p != null)
                                    {
                                        trGroup += $"<td>{p.Value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "Beginning")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.Beginning), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalInQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalInQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalOutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalOutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalQtyOnHand")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalQtyOnHand), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.OutNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;

                }
                else // write body no group by
                {
                    foreach (var row in stocBalances.Items)
                    {
                        var tr = "<tr>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                if (keyName == "Lot")
                                {
                                    tr += $"<td>{row.LotName}</td>";
                                }
                                else if (keyName == "Account")
                                {
                                    tr += $"<td>{row.InventoryAccountCode + "-" + row.InventoryAccountName}</td>";
                                }
                                else if (keyName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (keyName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (keyName == "Beginning")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.Beginning, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalInQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalInQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalOutQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalOutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalQtyOnHand")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalQtyOnHand, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (keyName == "NetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "OutNetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "SalePrice")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.SalePrice, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (p != null)
                                {
                                    tr += $"<td>{p.Value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && stockBalanceData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(stockBalanceData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }

                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });


        }
        #endregion


        [UnitOfWork(IsDisabled = true)]
        public async Task CleanupSaveTemplate()
        {

            var _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            var _reportTemplateRepository = IocManager.Instance.Resolve<ICorarlRepository<ReportTemplate, long>>();
            var _reportColumnTemplateRepository = IocManager.Instance.Resolve<ICorarlRepository<ReportColumnTemplate, Guid>>();
            var _tenantRepository = IocManager.Instance.Resolve<ICorarlRepository<Tenant, int>>();

            var reportColumns = new List<ReportColumnTemplate>();
            var tenantList = new List<Tenant>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    reportColumns = await _reportColumnTemplateRepository.GetAll().Include(s => s.ReportTemplate)
                                    .Where(s =>
                                     s.ReportTemplate.ReportType == ReportType.ReportType_Inventory ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_InventoryDetail ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_InventoryTransaction ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_StockBalance
                                     )
                                    .ToListAsync();

                    tenantList = await _tenantRepository.GetAll().ToListAsync();
                }
            }

            var columnToUpdates = new List<ReportColumnTemplate>();

            foreach (var tenant in tenantList)
            {
                var inventorySummaryReportFromTemplateColumns = new List<CollumnOutput>();
                var inventoryDetailReportFromTemplateColumns = new List<CollumnOutput>();
                var inventoryTransactionReportFromTemplateColumns = new List<CollumnOutput>();
                var stockBalanceReportFromTemplateColumns = new List<CollumnOutput>();

                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                    {
                        inventorySummaryReportFromTemplateColumns = GetReportTemplateInventory().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        inventoryDetailReportFromTemplateColumns = GetReportTemplateInventoryValuationDetail().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        inventoryTransactionReportFromTemplateColumns = GetReportTemplateInventoryTransaction().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        stockBalanceReportFromTemplateColumns = GetReportTemplateStock().ColumnInfo.Where(s => s.IsDisplay).ToList();
                    }
                }

                var inventorySummaryReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_Inventory).ToList();
                var inventoryDetailReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_InventoryDetail).ToList();
                var inventoryTransactionReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_InventoryTransaction).ToList();
                var stockBalanceReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_StockBalance).ToList();

                foreach (var col in inventorySummaryReportFromTemplateColumns)
                {
                    var updateCols = inventorySummaryReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                }
                ;

                foreach (var col in inventoryDetailReportFromTemplateColumns)
                {
                    var updateCols = inventoryDetailReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                }
                ;


                foreach (var col in inventoryTransactionReportFromTemplateColumns)
                {
                    var updateCols = inventoryTransactionReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                }
                ;


                foreach (var col in stockBalanceReportFromTemplateColumns)
                {
                    var updateCols = stockBalanceReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                }
                ;
            }


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _reportColumnTemplateRepository.BulkUpdateAsync(columnToUpdates);
                }

                await uow.CompleteAsync();
            }

        }


        #region Batch No Traceability

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Traceability)]
        public ReportOutput GetReportTemplateBatchNoTraceability()
        {
            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 100,
                        ColumnTitle = "Transaction No",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalTypeName",
                        ColumnLength = 100,
                        ColumnTitle = "Tranaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 100,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PartnerName",
                        ColumnLength = 100,
                        ColumnTitle = "Partner Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchNumber",
                        ColumnLength = 100,
                        ColumnTitle = "Batch No",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 180,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Qty",
                        ColumnLength = 170,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    }
                    ,
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 100,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "From",
                        ColumnLength = 100,
                        ColumnTitle = "Show From",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "To",
                        ColumnLength = 100,
                        ColumnTitle = "Show To",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                };
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columnInfo,
                Groupby = "",
                HeaderTitle = "Batch No Traceability",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Traceability)]
        public async Task<BatchNoTraceabilityBalanceOutput> GetBatchNoTraceabilityReport(BatchNoTraceabilityInput input)
        {
            if (input.Filter.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("BatchNo")));

            var batchQuery = from b in _batchNoRepository.GetAll().AsNoTracking()
                                       .Where(s => s.Code.ToLower() == input.Filter.ToLower())
                             join i in _itemRepository.GetAll().Include(s => s.Properties).ThenInclude(s => s.PropertyValue).AsNoTracking()
                             on b.ItemId equals i.Id
                             let unit = i.Properties.FirstOrDefault(s => s.PropertyValue.Property.IsUnit)
                             select new
                             {
                                 Id = b.Id,
                                 Code = b.Code,
                                 ItemCode = i.ItemCode,
                                 ItemName = i.ItemName,
                                 Unit = unit == null ? "" : unit.PropertyValue.Value,
                                 NetWeight = unit == null ? 0 : unit.PropertyValue.NetWeight
                             };

            var batch = await batchQuery.FirstOrDefaultAsync();

            if (batch == null) throw new UserFriendlyException(L("IsNotValid", L("BatchNo")));

            var itemReceiptBatch = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                             .AsNoTracking()
                                             .Where(s => s.BatchNoId == batch.Id)
                                   join r in _itemReceiptRepository.GetAll()
                                             .AsNoTracking()
                                   on b.ItemReceiptItem.ItemReceiptId equals r.Id
                                   join j in _journalRepository.GetAll()
                                             .AsNoTracking()
                                             .Where(s => s.ItemReceiptId.HasValue)
                                             .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                   on b.ItemReceiptItem.ItemReceiptId equals j.ItemReceiptId
                                   select new BatchNoTraceabilityOutput
                                   {
                                       Date = j.Date,
                                       JournalNo = j.JournalNo,
                                       Reference = j.Reference,
                                       JournalType = j.JournalType,
                                       JournalTypeName = j.JournalType == JournalType.ItemReceiptPurchase ? "Purchase" :
                                                         j.JournalType == JournalType.ItemReceiptTransfer ? "Transfer" :
                                                         j.JournalType == JournalType.ItemReceiptAdjustment ? "Adjustment" :
                                                         j.JournalType == JournalType.ItemReceiptProduction ? "Production" :
                                                         j.JournalType == JournalType.ItemReceiptOther ? "Other" : j.JournalType.ToString(),
                                       TransactionId = r.Id,
                                       ItemId = b.ItemReceiptItem.ItemId,
                                       ItemCode = b.ItemReceiptItem.Item.ItemCode,
                                       ItemName = b.ItemReceiptItem.Item.ItemName,
                                       PartnerName = r.Vendor == null ? "" : r.Vendor.VendorName,
                                       BatchNoId = b.BatchNoId,
                                       BatchNumber = b.BatchNo.Code,
                                       IsReceipt = true,
                                       Qty = b.Qty,
                                       TransferId = r.TransferOrderId,
                                       ProductioinId = r.ProductionOrderId,
                                       ProductionPlanId = r.ProductionOrder.ProductionPlanId,
                                       ProductionPlanNo = r.ProductionOrder.ProductionPlan != null ? r.ProductionOrder.ProductionPlan.DocumentNo : ""
                                   };


            var itemIssueBatch = from b in _itemIssueItemBatchNoRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(s => s.BatchNoId == batch.Id)
                                 join r in _itemIssueRepository.GetAll()
                                           .AsNoTracking()
                                 on b.ItemIssueItem.ItemIssueId equals r.Id
                                 join j in _journalRepository.GetAll()
                                           .AsNoTracking()
                                           .Where(s => s.ItemIssueId.HasValue)
                                           .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                 on b.ItemIssueItem.ItemIssueId equals j.ItemIssueId
                                 select new BatchNoTraceabilityOutput
                                 {
                                     Date = j.Date,
                                     JournalNo = j.JournalNo,
                                     Reference = j.Reference,
                                     JournalType = j.JournalType,
                                     JournalTypeName = j.JournalType == JournalType.ItemIssueSale ? "Sale" :
                                                         j.JournalType == JournalType.ItemIssueTransfer ? "Transfer" :
                                                         j.JournalType == JournalType.ItemIssueAdjustment ? "Adjustment" :
                                                         j.JournalType == JournalType.ItemIssueProduction ? "Production" :
                                                         j.JournalType == JournalType.ItemIssueOther ? "Other" : j.JournalType.ToString(),
                                     TransactionId = r.Id,
                                     ItemId = b.ItemIssueItem.ItemId,
                                     ItemCode = b.ItemIssueItem.Item.ItemCode,
                                     ItemName = b.ItemIssueItem.Item.ItemName,
                                     PartnerName = r.Customer == null ? "" : r.Customer.CustomerName,
                                     BatchNoId = b.BatchNoId,
                                     BatchNumber = b.BatchNo.Code,
                                     IsReceipt = false,
                                     Qty = b.Qty,
                                     TransferId = r.TransferOrderId,
                                     ProductioinId = r.ProductionOrderId,
                                     ProductionPlanId = r.ProductionOrder.ProductionPlanId,
                                     ProductionPlanNo = r.ProductionOrder.ProductionPlan != null ? r.ProductionOrder.ProductionPlan.DocumentNo : ""
                                 };

            var itemIssueVendorCreditBatch = from b in _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                                        .AsNoTracking()
                                                        .Where(s => s.BatchNoId == batch.Id)
                                             join r in _itemIssueVendorCreditRepository.GetAll()
                                                       .AsNoTracking()
                                             on b.ItemIssueVendorCreditItem.ItemIssueVendorCreditId equals r.Id
                                             join j in _journalRepository.GetAll()
                                                       .AsNoTracking()
                                                       .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                       .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                             on b.ItemIssueVendorCreditItem.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                             select new BatchNoTraceabilityOutput
                                             {
                                                 Date = j.Date,
                                                 JournalNo = j.JournalNo,
                                                 Reference = j.Reference,
                                                 JournalType = j.JournalType,
                                                 JournalTypeName = "PurchaseReturn",
                                                 TransactionId = r.Id,
                                                 ItemId = b.ItemIssueVendorCreditItem.ItemId,
                                                 ItemCode = b.ItemIssueVendorCreditItem.Item.ItemCode,
                                                 ItemName = b.ItemIssueVendorCreditItem.Item.ItemName,
                                                 PartnerName = r.Vendor == null ? "" : r.Vendor.VendorName,
                                                 BatchNoId = b.BatchNoId,
                                                 BatchNumber = b.BatchNo.Code,
                                                 IsReceipt = false,
                                                 Qty = b.Qty,
                                             };

            var itemReceiptCustomerCreditBatch = from b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                           .AsNoTracking()
                                                           .Where(s => s.BatchNoId == batch.Id)
                                                 join r in _itemReceiptCustomerCreditRepository.GetAll()
                                                           .AsNoTracking()
                                                 on b.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId equals r.Id
                                                 join j in _journalRepository.GetAll()
                                                           .AsNoTracking()
                                                           .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                           .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                 on b.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                 select new BatchNoTraceabilityOutput
                                                 {
                                                     Date = j.Date,
                                                     JournalNo = j.JournalNo,
                                                     Reference = j.Reference,
                                                     JournalType = j.JournalType,
                                                     JournalTypeName = "SaleReturn",
                                                     TransactionId = r.Id,
                                                     ItemId = b.ItemReceiptCustomerCreditItem.ItemId,
                                                     ItemCode = b.ItemReceiptCustomerCreditItem.Item.ItemCode,
                                                     ItemName = b.ItemReceiptCustomerCreditItem.Item.ItemName,
                                                     PartnerName = r.Customer == null ? "" : r.Customer.CustomerName,
                                                     BatchNoId = b.BatchNoId,
                                                     BatchNumber = b.BatchNo.Code,
                                                     IsReceipt = true,
                                                     Qty = b.Qty,
                                                 };

            var query = itemReceiptBatch
                        .Concat(itemIssueBatch)
                        .Concat(itemReceiptCustomerCreditBatch)
                        .Concat(itemIssueVendorCreditBatch)
                        .OrderBy(s => s.Date)
                        .GroupBy(s => new
                        {
                            s.TransactionId,
                            s.Date,
                            s.JournalType,
                            s.JournalTypeName,
                            s.JournalNo,
                            s.Reference,
                            s.BatchNoId,
                            s.BatchNumber,
                            s.PartnerName,
                            s.ItemId,
                            s.ItemCode,
                            s.ItemName,
                            s.IsReceipt,
                            s.TransferId,
                            s.ProductioinId,
                            s.ProductionPlanId,
                            s.ProductionPlanNo
                        })
                        .Select(s => new BatchNoTraceabilityOutput
                        {
                            Date = s.Key.Date,
                            JournalNo = s.Key.JournalNo,
                            Reference = s.Key.Reference,
                            JournalType = s.Key.JournalType,
                            JournalTypeName = s.Key.JournalTypeName,
                            TransactionId = s.Key.TransactionId,
                            ItemId = s.Key.ItemId,
                            ItemCode = s.Key.ItemCode,
                            ItemName = s.Key.ItemName,
                            PartnerName = s.Key.PartnerName,
                            BatchNoId = s.Key.BatchNoId,
                            BatchNumber = s.Key.BatchNumber,
                            IsReceipt = s.Key.IsReceipt,
                            TransferId = s.Key.TransferId,
                            ProductioinId = s.Key.ProductioinId,
                            ProductionPlanId = s.Key.ProductionPlanId,
                            ProductionPlanNo = s.Key.ProductionPlanNo,
                            Qty = s.Sum(t => t.Qty),
                        });

            var items = await query.ToListAsync();
            var receiptQty = items.Where(s => s.IsReceipt).Sum(t => t.Qty);
            var issueQty = items.Where(s => !s.IsReceipt).Sum(t => t.Qty);
            var balanceQty = items.Sum(t => t.IsReceipt ? t.Qty : -t.Qty);
            var receiptItems = items.Where(s => s.IsReceipt).ToList();
            var issueItems = items.Where(s => !s.IsReceipt).ToList();

            var productionPlanReceipts = new List<ProductionPlanBatchOutput>();
            var productionPlanIssues = new List<ProductionPlanBatchOutput>();

            var receiptHash = receiptItems
                               .Where(s => s.ProductioinId.HasValue)
                               .Where(s => s.JournalType == JournalType.ItemReceiptProduction)
                               .GroupBy(s => s.ProductioinId)
                               .Select(s => s.Key.Value)
                               .ToHashSet();

            if (receiptHash.Any())
            {
                var productionFrom = from b in _itemIssueItemBatchNoRepository.GetAll()
                                                     .AsNoTracking()
                                     join r in _itemIssueRepository.GetAll()
                                               .AsNoTracking()
                                               .Where(s => s.ProductionOrderId.HasValue)
                                               .Where(s => receiptHash.Contains(s.ProductionOrderId.Value))
                                     on b.ItemIssueItem.ItemIssueId equals r.Id
                                     join j in _journalRepository.GetAll()
                                               .AsNoTracking()
                                               .Where(s => s.ItemIssueId.HasValue)
                                               .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                     on b.ItemIssueItem.ItemIssueId equals j.ItemIssueId
                                     join i in _itemRepository.GetAll().Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                               .AsNoTracking()
                                     on b.BatchNo.ItemId equals i.Id

                                     let unit = i.Properties.Where(s => s.PropertyValue.Property.IsUnit).FirstOrDefault()

                                     select new BatchNoTraceabilityOutput
                                     {
                                         Date = j.Date,
                                         JournalNo = j.JournalNo,
                                         Reference = j.Reference,
                                         JournalType = j.JournalType,
                                         JournalTypeName = j.JournalType.ToString(),
                                         TransactionId = r.Id,
                                         ItemId = b.ItemIssueItem.ItemId,
                                         ItemCode = b.ItemIssueItem.Item.ItemCode,
                                         ItemName = b.ItemIssueItem.Item.ItemName,
                                         PartnerName = r.Customer == null ? "" : r.Customer.CustomerName,
                                         BatchNoId = b.BatchNoId,
                                         BatchNumber = b.BatchNo.Code,
                                         IsReceipt = false,
                                         Qty = b.Qty,
                                         ProductioinId = r.ProductionOrderId,
                                         ProductionPlanId = r.ProductionOrder.ProductionPlanId,
                                         ProductionPlanNo = r.ProductionOrder.ProductionPlan != null ? r.ProductionOrder.ProductionPlan.DocumentNo : "",
                                         Unit = unit == null ? "" : unit.PropertyValue.Value,
                                         NetWeight = unit == null ? 0 : unit.PropertyValue.NetWeight
                                     };

                var productionBatchQuery = productionFrom
                                          .OrderBy(s => s.Date)
                                          .GroupBy(s => new
                                          {
                                              s.TransactionId,
                                              s.Date,
                                              s.JournalType,
                                              s.JournalTypeName,
                                              s.JournalNo,
                                              s.Reference,
                                              s.BatchNoId,
                                              s.BatchNumber,
                                              s.PartnerName,
                                              s.ItemId,
                                              s.ItemCode,
                                              s.ItemName,
                                              s.ProductioinId,
                                              s.ProductionPlanId,
                                              s.ProductionPlanNo,
                                              s.IsReceipt,
                                              s.Unit,
                                              s.NetWeight
                                          })
                                          .Select(s => new BatchNoTraceabilityOutput
                                          {
                                              Date = s.Key.Date,
                                              JournalNo = s.Key.JournalNo,
                                              Reference = s.Key.Reference,
                                              JournalType = s.Key.JournalType,
                                              JournalTypeName = s.Key.JournalTypeName,
                                              TransactionId = s.Key.TransactionId,
                                              ItemId = s.Key.ItemId,
                                              ItemCode = s.Key.ItemCode,
                                              ItemName = s.Key.ItemName,
                                              PartnerName = s.Key.PartnerName,
                                              BatchNoId = s.Key.BatchNoId,
                                              BatchNumber = s.Key.BatchNumber,
                                              IsReceipt = s.Key.IsReceipt,
                                              ProductioinId = s.Key.ProductioinId,
                                              ProductionPlanId = s.Key.ProductionPlanId,
                                              ProductionPlanNo = s.Key.ProductionPlanNo,
                                              Unit = s.Key.Unit,
                                              NetWeight = s.Key.NetWeight,
                                              Qty = s.Sum(t => t.Qty),

                                          });

                var productionFromItems = await productionBatchQuery.ToListAsync();
                var productionToItems = receiptItems.Where(s => s.ProductioinId.HasValue && receiptHash.Contains(s.ProductioinId.Value)).ToList();

                productionPlanReceipts = productionFromItems.Concat(productionToItems)
                .GroupBy(g => new
                {
                    g.ProductionPlanId,
                    g.ProductionPlanNo
                })
                .Select(p => new ProductionPlanBatchOutput
                {
                    ProductioinId = p.Key.ProductionPlanId,
                    ProductionPlanNo = p.Key.ProductionPlanNo,
                    IssueQty = p.Where(s => !s.IsReceipt).Sum(t => t.Qty),
                    ReceiptQty = p.Where(s => s.IsReceipt && s.BatchNoId == batch.Id).Sum(t => t.Qty),
                    BalanceQty = p.Where(s => s.BatchNoId == batch.Id).Sum(t => t.IsReceipt ? t.Qty : -t.Qty),
                    ProductionIssueItems = p.Where(s => !s.IsReceipt).ToList(),
                    ProductionReceiptItems = p.Where(s => s.IsReceipt).ToList()
                })
                .ToList();

                receiptItems = receiptItems.Where(s => !s.ProductioinId.HasValue || !receiptHash.Contains(s.ProductioinId.Value)).ToList();
            }

            var issueHash = issueItems
                              .Where(s => s.ProductioinId.HasValue)
                              .Where(s => s.JournalType == JournalType.ItemIssueProduction)
                              .GroupBy(s => s.ProductioinId)
                              .Select(s => s.Key.Value)
                              .ToHashSet();

            if (issueHash.Any())
            {
                var productionTo = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                              .AsNoTracking()
                                   join r in _itemReceiptRepository.GetAll()
                                       .AsNoTracking()
                                       .Where(s => s.ProductionOrderId.HasValue)
                                       .Where(s => issueHash.Contains(s.ProductionOrderId.Value))
                                   on b.ItemReceiptItem.ItemReceiptId equals r.Id
                                   join j in _journalRepository.GetAll()
                                       .AsNoTracking()
                                       .Where(s => s.ItemReceiptId.HasValue)
                                       .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                   on b.ItemReceiptItem.ItemReceiptId equals j.ItemReceiptId
                                   join i in _itemRepository.GetAll().Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                            .AsNoTracking()
                                   on b.BatchNo.ItemId equals i.Id

                                   let unit = i.Properties.Where(s => s.PropertyValue.Property.IsUnit).FirstOrDefault()
                                   select new BatchNoTraceabilityOutput
                                   {
                                       Date = j.Date,
                                       JournalNo = j.JournalNo,
                                       Reference = j.Reference,
                                       JournalType = j.JournalType,
                                       JournalTypeName = j.JournalType.ToString(),
                                       TransactionId = r.Id,
                                       ItemId = b.ItemReceiptItem.ItemId,
                                       ItemCode = b.ItemReceiptItem.Item.ItemCode,
                                       ItemName = b.ItemReceiptItem.Item.ItemName,
                                       PartnerName = r.Vendor == null ? "" : r.Vendor.VendorName,
                                       BatchNoId = b.BatchNoId,
                                       BatchNumber = b.BatchNo.Code,
                                       IsReceipt = true,
                                       Qty = b.Qty,
                                       ProductioinId = r.ProductionOrderId,
                                       ProductionPlanId = r.ProductionOrder.ProductionPlanId,
                                       ProductionPlanNo = r.ProductionOrder.ProductionPlan != null ? r.ProductionOrder.ProductionPlan.DocumentNo : "",
                                       Unit = unit == null ? "" : unit.PropertyValue.Value,
                                       NetWeight = unit == null ? 0 : unit.PropertyValue.NetWeight
                                   };

                var productionBatchQuery = productionTo
                                      .OrderBy(s => s.Date)
                                      .GroupBy(s => new
                                      {
                                          s.TransactionId,
                                          s.Date,
                                          s.JournalType,
                                          s.JournalTypeName,
                                          s.JournalNo,
                                          s.Reference,
                                          s.BatchNoId,
                                          s.BatchNumber,
                                          s.PartnerName,
                                          s.ItemId,
                                          s.ItemCode,
                                          s.ItemName,
                                          s.IsReceipt,
                                          s.ProductioinId,
                                          s.ProductionPlanId,
                                          s.ProductionPlanNo,
                                          s.Unit,
                                          s.NetWeight
                                      })
                                      .Select(s => new BatchNoTraceabilityOutput
                                      {
                                          Date = s.Key.Date,
                                          JournalNo = s.Key.JournalNo,
                                          Reference = s.Key.Reference,
                                          JournalType = s.Key.JournalType,
                                          JournalTypeName = s.Key.JournalTypeName,
                                          TransactionId = s.Key.TransactionId,
                                          ItemId = s.Key.ItemId,
                                          ItemCode = s.Key.ItemCode,
                                          ItemName = s.Key.ItemName,
                                          PartnerName = s.Key.PartnerName,
                                          BatchNoId = s.Key.BatchNoId,
                                          BatchNumber = s.Key.BatchNumber,
                                          IsReceipt = s.Key.IsReceipt,
                                          ProductioinId = s.Key.ProductioinId,
                                          ProductionPlanId = s.Key.ProductionPlanId,
                                          ProductionPlanNo = s.Key.ProductionPlanNo,
                                          Unit = s.Key.Unit,
                                          NetWeight = s.Key.NetWeight,
                                          Qty = s.Sum(t => t.Qty),
                                      });

                var productionToItems = await productionBatchQuery.ToListAsync();
                var productionFromItems = issueItems.Where(s => s.ProductioinId.HasValue && issueHash.Contains(s.ProductioinId.Value)).ToList();

                productionPlanIssues = productionFromItems.Concat(productionToItems)
                .GroupBy(g => new
                {
                    g.ProductionPlanId,
                    g.ProductionPlanNo
                })
                .Select(p => new ProductionPlanBatchOutput
                {
                    ProductioinId = p.Key.ProductionPlanId,
                    ProductionPlanNo = p.Key.ProductionPlanNo,
                    IssueQty = p.Where(s => !s.IsReceipt && s.BatchNoId == batch.Id).Sum(t => t.Qty),
                    ReceiptQty = p.Where(s => s.IsReceipt).Sum(t => t.Qty),
                    BalanceQty = p.Where(s => s.BatchNoId == batch.Id).Sum(t => t.IsReceipt ? t.Qty : -t.Qty),
                    ProductionIssueItems = p.Where(s => !s.IsReceipt).ToList(),
                    ProductionReceiptItems = p.Where(s => s.IsReceipt).ToList()
                })
                .ToList();

                issueItems = issueItems.Where(s => !s.ProductioinId.HasValue || !issueHash.Contains(s.ProductioinId.Value)).ToList();
            }


            return new BatchNoTraceabilityBalanceOutput
            {
                BatchNumber = input.Filter,
                ItemName = batch.ItemName,
                ItemCode = batch.ItemCode,
                Unit = batch.Unit,
                NetWeight = batch.NetWeight,
                ProductionPlanReceipts = productionPlanReceipts,
                ProductionPlanIssues = productionPlanIssues,
                ReceiptQty = receiptQty,
                IssueQty = issueQty,
                BalanceQty = balanceQty,
                ReceiptItems = receiptItems,
                IssueItems = issueItems
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Traceability)]
        public async Task<ListResultDto<MoreBatchNoOutput>> GetMoreBatchItems(GetMoreBatchItemInput input)
        {

            var productionFrom = from b in _itemIssueItemBatchNoRepository.GetAll()
                                           .AsNoTracking()
                                           .Where(s => s.ItemIssueItem.ItemIssueId == input.TransacationId)
                                 join r in _itemIssueRepository.GetAll()
                                           .AsNoTracking()
                                           .Where(s => s.ProductionOrderId.HasValue)
                                           .Where(s => s.Id == input.TransacationId)
                                 on b.ItemIssueItem.ItemIssueId equals r.Id
                                 join j in _journalRepository.GetAll()
                                           .AsNoTracking()
                                           .Where(s => s.ItemIssueId.HasValue)
                                           .Where(s => s.ItemIssueId == input.TransacationId)
                                           .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                 on b.ItemIssueItem.ItemIssueId equals j.ItemIssueId
                                 join i in _itemRepository.GetAll().Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                           .AsNoTracking()
                                 on b.BatchNo.ItemId equals i.Id

                                 let unit = i.Properties.Where(s => s.PropertyValue.Property.IsUnit).FirstOrDefault()

                                 select new MoreBatchNoOutput
                                 {
                                     ItemId = b.ItemIssueItem.ItemId,
                                     ItemCode = b.ItemIssueItem.Item.ItemCode,
                                     ItemName = b.ItemIssueItem.Item.ItemName,
                                     BatchNoId = b.BatchNoId,
                                     BatchNumber = b.BatchNo.Code,
                                     IsReceipt = false,
                                     Qty = b.Qty,
                                     ProductioinId = r.ProductionOrderId,
                                     Unit = unit == null ? "" : unit.PropertyValue.Value,
                                     NetWeight = unit == null ? 0 : unit.PropertyValue.NetWeight
                                 };

            var productionTo = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                         .AsNoTracking()
                                         .Where(s => s.ItemReceiptItem.ItemReceiptId == input.TransacationId)
                               join r in _itemReceiptRepository.GetAll()
                                   .AsNoTracking()
                                   .Where(s => s.ProductionOrderId.HasValue)
                                   .Where(s => s.Id == input.TransacationId)
                               on b.ItemReceiptItem.ItemReceiptId equals r.Id
                               join j in _journalRepository.GetAll()
                                   .AsNoTracking()
                                   .Where(s => s.ItemReceiptId.HasValue)
                                   .Where(s => s.ItemReceiptId == input.TransacationId)
                                   .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                               on b.ItemReceiptItem.ItemReceiptId equals j.ItemReceiptId
                               join i in _itemRepository.GetAll().Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                        .AsNoTracking()
                               on b.BatchNo.ItemId equals i.Id

                               let unit = i.Properties.Where(s => s.PropertyValue.Property.IsUnit).FirstOrDefault()
                               select new MoreBatchNoOutput
                               {
                                   ItemId = b.ItemReceiptItem.ItemId,
                                   ItemCode = b.ItemReceiptItem.Item.ItemCode,
                                   ItemName = b.ItemReceiptItem.Item.ItemName,
                                   BatchNoId = b.BatchNoId,
                                   BatchNumber = b.BatchNo.Code,
                                   IsReceipt = true,
                                   Qty = b.Qty,
                                   ProductioinId = r.ProductionOrderId,
                                   Unit = unit == null ? "" : unit.PropertyValue.Value,
                                   NetWeight = unit == null ? 0 : unit.PropertyValue.NetWeight
                               };

            var productionBatchQuery = productionFrom.Concat(productionTo)
                                        .OrderBy(s => s.BatchNumber)
                                        .GroupBy(s => new
                                        {
                                            s.BatchNoId,
                                            s.BatchNumber,
                                            s.ItemId,
                                            s.ItemCode,
                                            s.ItemName,
                                            s.ProductioinId,
                                            s.IsReceipt,
                                            s.Unit,
                                            s.NetWeight
                                        })
                                        .Select(s => new MoreBatchNoOutput
                                        {
                                            ItemId = s.Key.ItemId,
                                            ItemCode = s.Key.ItemCode,
                                            ItemName = s.Key.ItemName,
                                            BatchNoId = s.Key.BatchNoId,
                                            BatchNumber = s.Key.BatchNumber,
                                            IsReceipt = s.Key.IsReceipt,
                                            ProductioinId = s.Key.ProductioinId,
                                            Unit = s.Key.Unit,
                                            NetWeight = s.Key.NetWeight,
                                            Qty = s.Sum(t => t.Qty),

                                        });

            var productionItems = await productionBatchQuery.ToListAsync();

            return new ListResultDto<MoreBatchNoOutput> { Items = productionItems };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Traceability)]
        public async Task<PagedResultDto<FindBatchNoTraceabilityOutput>> FindBatchNos(FindBatchNoTraceabilityInput input)
        {
            var query = from b in _itemIssueItemBatchNoRepository.GetAll()
                                  .AsNoTracking()
                                  .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.BatchNo.Code.ToLower().Contains(input.Filter.ToLower()))
                        join r in _itemIssueRepository.GetAll()
                                  .AsNoTracking()
                                  .Where(s => s.CustomerId.HasValue)
                                  .WhereIf(!input.Customer.IsNullOrWhiteSpace(), s =>
                                        s.Customer.CustomerCode.ToLower().Contains(input.Customer.ToLower()) ||
                                        s.Customer.CustomerName.ToLower().Contains(input.Customer.ToLower()))
                        on b.ItemIssueItem.ItemIssueId equals r.Id
                        join j in _journalRepository.GetAll()
                                  .AsNoTracking()
                                  .Where(s => s.ItemIssueId.HasValue)
                                  .Where(s => s.JournalType == JournalType.ItemIssueSale)
                                  .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                  .WhereIf(!input.ItemIssue.IsNullOrWhiteSpace(), s =>
                                    s.JournalNo.ToLower().Contains(input.ItemIssue.ToLower()) ||
                                    (!string.IsNullOrEmpty(s.Reference) && s.Reference.ToLower().Contains(input.ItemIssue.ToLower())))
                        on r.Id equals j.ItemIssueId
                        join i in _itemRepository.GetAll()
                                  .AsNoTracking()
                                  .WhereIf(!input.Item.IsNullOrWhiteSpace(), s =>
                                        s.ItemCode.ToLower().Contains(input.Item.ToLower()) ||
                                        s.ItemName.ToLower().Contains(input.Item.ToLower()))
                        on b.BatchNo.ItemId equals i.Id

                        join inv in _invoiceRepository.GetAll()
                                    .AsNoTracking()
                                    .Where(s => s.ItemIssueId.HasValue)
                        on r.Id equals inv.ItemIssueId
                        into invs
                        from inv in invs.DefaultIfEmpty()

                        join ij in _journalRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.JournalType == JournalType.Invoice)
                                        .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                        .WhereIf(!input.Invoice.IsNullOrWhiteSpace(), s =>
                                            s.JournalNo.ToLower().Contains(input.Invoice.ToLower()) ||
                                            (!string.IsNullOrEmpty(s.Reference) && s.Reference.ToLower().Contains(input.Invoice.ToLower())))
                        on inv.Id equals ij.InvoiceId
                        into ijs
                        from ij in ijs.DefaultIfEmpty()

                        where input.Invoice.IsNullOrWhiteSpace() || ij != null

                        select new FindBatchNoTraceabilityOutput
                        {
                            ItemId = b.ItemIssueItem.ItemId,
                            ItemCode = b.ItemIssueItem.Item.ItemCode,
                            ItemName = b.ItemIssueItem.Item.ItemName,
                            BatchNoId = b.BatchNoId,
                            BatchNumber = b.BatchNo.Code,
                            Qty = b.Qty,
                            CustomerCode = r.Customer.CustomerCode,
                            CustomerName = r.Customer.CustomerName,
                            Date = j.Date,
                            Reference = j.Reference,
                            ItemIssueNo = j.JournalNo,
                            InvoiceNo = ij == null ? "" : ij.JournalNo,
                            InvoiceRef = ij == null ? "" : ij.Reference,
                            ItemIssueId = r.Id
                        };

            var count = await query.CountAsync();
            var items = new List<FindBatchNoTraceabilityOutput>();
            if (count > 0)
            {
                items = await query.OrderBy(s => s.BatchNumber).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<FindBatchNoTraceabilityOutput> { Items = items, TotalCount = count };

        }

        #endregion

        #region BatchNo Balance

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BatchNoBalance)]
        public async Task<ReportOutput> GetReportTemplateBatchNoBalance()
        {

            var itemProperty = await _propertyRepository.GetAll()
                              .AsNoTracking()
                              .Where(t => t.IsActive == true)
                              .Select(t => new CollumnOutput
                              {
                                  AllowGroupby = false,
                                  AllowFilter = true,
                                  ColumnName = t.Name,
                                  ColumnLength = 150,
                                  ColumnTitle = t.Name,
                                  ColumnType = ColumnType.ItemProperty,
                                  SortOrder = 14,
                                  Visible = false,
                                  AllowFunction = null,
                                  MoreFunction = null,
                                  IsDisplay = true,
                                  DefaultValue = t.Id.ToString(),//to init the value of property
                                  AllowShowHideFilter = true,
                                  ShowHideFilter = true,
                              })
                              .OrderBy(t => t.ColumnName)
                              .ToListAsync();

            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "asOf",
                        AllowFunction = null,
                        DisableDefault = true,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                     new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "TrackType",
                        ColumnLength = 300,
                        ColumnTitle = "Track Type",
                        ColumnType = ColumnType.Number,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "BatchNumber",
                        ColumnLength = 100,
                        ColumnTitle = "Batch No",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = false,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 180,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 100,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                       new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Lot",
                        ColumnLength = 100,
                        ColumnTitle = "Zone",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 100,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Aging",
                        ColumnLength = 140,
                        ColumnTitle = "Aging (Days)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ExpirationDate",
                        ColumnLength = 100,
                        ColumnTitle = "Expiration Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                     new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Expiring",
                        ColumnLength = 140,
                        ColumnTitle = "Expiring (Days)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                         AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "InQty",
                        ColumnLength = 170,
                        ColumnTitle = "In Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                       new CollumnOutput{
                         AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OutQty",
                        ColumnLength = 170,
                        ColumnTitle = "Out Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                         AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BalanceQty",
                        ColumnLength = 170,
                        ColumnTitle = "Balance Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                     new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "ItemGroup",
                        ColumnLength = 100,
                        ColumnTitle = "Item Group",
                        ColumnType = ColumnType.String,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                };

            var allCols = columnInfo.Concat(itemProperty).ToList();

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = allCols,
                Groupby = "",
                HeaderTitle = "Batch No Balance",
                Sortby = "",
            };
            return result;
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BatchNoBalance)]
        public async Task<PagedResultWithTotalColuumnsDto<BatchNoBalanceOutput>> GetBatchNoBalanceReport(BatchNoBalanceInput input)
        {

            var userGroupLocations = await GetUserGroupLocations();

            var previousClose = await GetPreviousCloseCyleAsync(input.ToDate);

            var saleReturnQuery = from b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                            .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.ItemReceiptCustomerCreditItem.LotId))
                                            .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemReceiptCustomerCreditItem.Lot.LocationId))
                                            .WhereIf(userGroupLocations != null && userGroupLocations.Any(), s =>
                                                s.ItemReceiptCustomerCreditItem.Lot.Location.Member == Member.All ||
                                                userGroupLocations.Contains(s.ItemReceiptCustomerCreditItem.Lot.LocationId))
                                            .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemReceiptCustomerCreditItem.ItemId))
                                            .WhereIf(input.TrackType.HasValue, s =>
                                                (input.TrackType == TrackType.Serial && s.BatchNo.IsSerial) ||
                                                (input.TrackType == TrackType.Expiration && s.BatchNo.ExpirationDate.HasValue) ||
                                                (input.TrackType == TrackType.BatchNo && !s.BatchNo.IsSerial && !s.BatchNo.ExpirationDate.HasValue))
                                            .WhereIf(!input.BatchNumber.IsNullOrWhiteSpace(), s => s.BatchNo.Code.ToLower().Contains(input.BatchNumber.ToLower()))
                                            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                                      s.ItemReceiptCustomerCreditItem.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                      s.ItemReceiptCustomerCreditItem.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                      (!s.ItemReceiptCustomerCreditItem.Item.Barcode.IsNullOrEmpty() && s.ItemReceiptCustomerCreditItem.Item.Barcode.ToLower().Contains(input.Filter.ToLower()))
                                                  )
                                            .AsNoTracking()
                                  join j in _journalRepository.GetAll()
                                               .AsNoTracking()
                                               .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                               .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                               .Where(s => s.Date.Date <= input.ToDate.Date)
                                               .WhereIf(previousClose != null, s => s.Date.Date > previousClose.EndDate.Value.Date)
                                  on b.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId

                                  select new BatchNoBalanceOutput
                                  {
                                      BatchNoId = b.BatchNoId,
                                      BatchNumber = b.BatchNo.Code,
                                      ExpirationDate = b.BatchNo.ExpirationDate,
                                      Date = j.Date.Date,
                                      LocationId = b.ItemReceiptCustomerCreditItem.Lot.LocationId,
                                      LocationName = b.ItemReceiptCustomerCreditItem.Lot.Location.LocationName,
                                      LotId = b.ItemReceiptCustomerCreditItem.LotId.Value,
                                      LotName = b.ItemReceiptCustomerCreditItem.Lot.LotName,
                                      ItemId = b.ItemReceiptCustomerCreditItem.ItemId,
                                      BalanceQty = b.Qty,
                                  };


            var itemReceiptQuery = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                              .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.ItemReceiptItem.LotId))
                                              .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemReceiptItem.Lot.LocationId))
                                              .WhereIf(userGroupLocations != null && userGroupLocations.Any(), s =>
                                                    s.ItemReceiptItem.Lot.Location.Member == Member.All ||
                                                    userGroupLocations.Contains(s.ItemReceiptItem.Lot.LocationId))
                                              .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemReceiptItem.ItemId))
                                              .WhereIf(input.TrackType.HasValue, s =>
                                                    (input.TrackType == TrackType.Serial && s.BatchNo.IsSerial) ||
                                                    (input.TrackType == TrackType.Expiration && s.BatchNo.ExpirationDate.HasValue) ||
                                                    (input.TrackType == TrackType.BatchNo && !s.BatchNo.IsSerial && !s.BatchNo.ExpirationDate.HasValue))
                                              .WhereIf(!input.BatchNumber.IsNullOrWhiteSpace(), s => s.BatchNo.Code.ToLower().Contains(input.BatchNumber.ToLower()))
                                              .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                                        s.ItemReceiptItem.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                        s.ItemReceiptItem.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                        (!s.ItemReceiptItem.Item.Barcode.IsNullOrEmpty() && s.ItemReceiptItem.Item.Barcode.ToLower().Contains(input.Filter.ToLower()))
                                                    )
                                              .AsNoTracking()
                                   join j in _journalRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(s => s.ItemReceiptId.HasValue)
                                                .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                .Where(s => s.Date.Date <= input.ToDate.Date)
                                                .WhereIf(previousClose != null, s => s.Date.Date > previousClose.EndDate.Value.Date)
                                   on b.ItemReceiptItem.ItemReceiptId equals j.ItemReceiptId

                                   select new BatchNoBalanceOutput
                                   {
                                       BatchNoId = b.BatchNoId,
                                       BatchNumber = b.BatchNo.Code,
                                       ExpirationDate = b.BatchNo.ExpirationDate,
                                       Date = j.Date.Date,
                                       LocationId = b.ItemReceiptItem.Lot.LocationId,
                                       LocationName = b.ItemReceiptItem.Lot.Location.LocationName,
                                       LotId = b.ItemReceiptItem.LotId.Value,
                                       LotName = b.ItemReceiptItem.Lot.LotName,
                                       ItemId = b.ItemReceiptItem.ItemId,
                                       BalanceQty = b.Qty,
                                   };

            var concatQuery = itemReceiptQuery.Concat(saleReturnQuery);
            if (previousClose != null)
            {
                var closeBatchQuery = _inventoryCostCloseItemBatchNoRepository.GetAll()
                                      .AsNoTracking()
                                      .Where(s => s.InventoryCostClose.AccountCycleId == previousClose.Id)
                                      .Select(s => new BatchNoBalanceOutput
                                      {
                                          BatchNoId = s.BatchNoId,
                                          BatchNumber = s.BatchNo.Code,
                                          ExpirationDate = s.BatchNo.ExpirationDate,
                                          Date = previousClose.EndDate.Value,
                                          LocationId = s.Lot.LocationId,
                                          LocationName = s.Lot.Location.LocationName,
                                          LotId = s.LotId,
                                          LotName = s.Lot.LotName,
                                          ItemId = s.InventoryCostClose.ItemId,
                                          BalanceQty = s.Qty,
                                      });

                concatQuery = closeBatchQuery.Concat(concatQuery);
            }


            var receiptQuery = from r in concatQuery
                               select new BatchNoBalanceOutput
                               {
                                   BatchNoId = r.BatchNoId,
                                   BatchNumber = r.BatchNumber,
                                   ExpirationDate = r.ExpirationDate,
                                   Date = r.Date,
                                   LocationId = r.LocationId,
                                   LocationName = r.LocationName,
                                   LotId = r.LotId,
                                   LotName = r.LotName,
                                   ItemId = r.ItemId,
                                   BalanceQty = r.BalanceQty
                               }
                               into list
                               group list by new
                               {
                                   BatchNoId = list.BatchNoId,
                                   BatchNumber = list.BatchNumber,
                                   ExpirationDate = list.ExpirationDate,
                                   LocationId = list.LocationId,
                                   LocationName = list.LocationName,
                                   LotId = list.LotId,
                                   LotName = list.LotName,
                                   ItemId = list.ItemId,
                               }
                               into g
                               select new BatchNoBalanceOutput
                               {
                                   BatchNoId = g.Key.BatchNoId,
                                   BatchNumber = g.Key.BatchNumber,
                                   ExpirationDate = g.Key.ExpirationDate,
                                   Date = g.Max(t => t.Date),
                                   LocationId = g.Key.LocationId,
                                   LocationName = g.Key.LocationName,
                                   LotId = g.Key.LotId,
                                   LotName = g.Key.LotName,
                                   ItemId = g.Key.ItemId,
                                   BalanceQty = g.Sum(s => s.BalanceQty)
                               };


            var itemIssueQuery = from b in _itemIssueItemBatchNoRepository.GetAll()
                                            .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.ItemIssueItem.LotId))
                                            .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemIssueItem.Lot.LocationId))
                                            .WhereIf(userGroupLocations != null && userGroupLocations.Any(), s =>
                                                    s.ItemIssueItem.Lot.Location.Member == Member.All ||
                                                    userGroupLocations.Contains(s.ItemIssueItem.Lot.LocationId))
                                            .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemIssueItem.ItemId))
                                            .WhereIf(input.TrackType.HasValue, s =>
                                                (input.TrackType == TrackType.Serial && s.BatchNo.IsSerial) ||
                                                (input.TrackType == TrackType.Expiration && s.BatchNo.ExpirationDate.HasValue) ||
                                                (input.TrackType == TrackType.BatchNo && !s.BatchNo.IsSerial && !s.BatchNo.ExpirationDate.HasValue))
                                            .WhereIf(!input.BatchNumber.IsNullOrWhiteSpace(), s => s.BatchNo.Code.ToLower().Contains(input.BatchNumber.ToLower()))
                                            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                                    s.ItemIssueItem.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                    s.ItemIssueItem.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                    (!s.ItemIssueItem.Item.Barcode.IsNullOrEmpty() && s.ItemIssueItem.Item.Barcode.ToLower().Contains(input.Filter.ToLower()))
                                                )
                                            .AsNoTracking()
                                 join j in _journalRepository.GetAll()
                                              .AsNoTracking()
                                              .Where(s => s.ItemIssueId.HasValue)
                                              .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                              .Where(s => s.Date.Date <= input.ToDate.Date)
                                              .WhereIf(previousClose != null, s => s.Date.Date > previousClose.EndDate.Value.Date)
                                 on b.ItemIssueItem.ItemIssueId equals j.ItemIssueId

                                 select new BatchNoBalanceOutput
                                 {
                                     BatchNoId = b.BatchNoId,
                                     LotId = b.ItemIssueItem.LotId.Value,
                                     BalanceQty = b.Qty,
                                 };

            var purchaseReturnQuery = from b in _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                               .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.ItemIssueVendorCreditItem.LotId))
                                               .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemIssueVendorCreditItem.Lot.LocationId))
                                               .WhereIf(userGroupLocations != null && userGroupLocations.Any(), s =>
                                                    s.ItemIssueVendorCreditItem.Lot.Location.Member == Member.All ||
                                                    userGroupLocations.Contains(s.ItemIssueVendorCreditItem.Lot.LocationId))
                                               .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemIssueVendorCreditItem.ItemId))
                                               .WhereIf(input.TrackType.HasValue, s =>
                                                    (input.TrackType == TrackType.Serial && s.BatchNo.IsSerial) ||
                                                    (input.TrackType == TrackType.Expiration && s.BatchNo.ExpirationDate.HasValue) ||
                                                    (input.TrackType == TrackType.BatchNo && !s.BatchNo.IsSerial && !s.BatchNo.ExpirationDate.HasValue))
                                               .WhereIf(!input.BatchNumber.IsNullOrWhiteSpace(), s => s.BatchNo.Code.ToLower().Contains(input.BatchNumber.ToLower()))
                                               .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                                         s.ItemIssueVendorCreditItem.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                         s.ItemIssueVendorCreditItem.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                         (!s.ItemIssueVendorCreditItem.Item.Barcode.IsNullOrEmpty() && s.ItemIssueVendorCreditItem.Item.Barcode.ToLower().Contains(input.Filter.ToLower()))
                                                     )
                                               .AsNoTracking()
                                      join j in _journalRepository.GetAll()
                                                   .AsNoTracking()
                                                   .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                   .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                   .Where(s => s.Date.Date <= input.ToDate.Date)
                                                   .WhereIf(previousClose != null, s => s.Date.Date > previousClose.EndDate.Value.Date)
                                      on b.ItemIssueVendorCreditItem.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId

                                      select new BatchNoBalanceOutput
                                      {
                                          BatchNoId = b.BatchNoId,
                                          LotId = b.ItemIssueVendorCreditItem.LotId.Value,
                                          BalanceQty = b.Qty,
                                      };

            var query = from r in receiptQuery
                        join i in itemIssueQuery
                        on $"{r.BatchNoId}-{r.LotId}" equals $"{i.BatchNoId}-{i.LotId}"
                        into issues
                        join pr in purchaseReturnQuery
                        on $"{r.BatchNoId}-{r.LotId}" equals $"{pr.BatchNoId}-{pr.LotId}"
                        into pReturns

                        let issueQty = issues == null ? 0 : issues.Sum(t => t.BalanceQty)
                        let purchaseReturnQty = pReturns == null ? 0 : pReturns.Sum(t => t.BalanceQty)
                        let balanceQty = r.BalanceQty - issueQty - purchaseReturnQty
                        let inQty = r.BalanceQty
                        let outQty = issueQty + purchaseReturnQty

                        where balanceQty != 0

                        select new BatchNoBalanceOutput
                        {
                            BatchNoId = r.BatchNoId,
                            BatchNumber = r.BatchNumber,
                            ExpirationDate = r.ExpirationDate,
                            Date = r.Date,
                            LocationId = r.LocationId,
                            LocationName = r.LocationName,
                            LotId = r.LotId,
                            LotName = r.LotName,
                            ItemId = r.ItemId,
                            InQty = inQty,
                            OutQty = outQty,
                            BalanceQty = balanceQty,
                            ToDate = input.ToDate,
                        };

            var userGroupItems = await GetUserGroupItems();
            var itemQuery = GetItemWithProperties(userGroupItems, input.Items, input.PropertyDics);

            var result = from r in query.AsNoTracking()
                         join i in itemQuery
                         on r.ItemId equals i.Id

                         let unit = i.Properties.Where(s => s.IsUnit).FirstOrDefault()
                         let netWeight = unit == null ? 0 : unit.NetWeight
                         let itemGroup = i.Properties.Where(s => s.IsItemGroup).FirstOrDefault()

                         select new BatchNoBalanceOutput
                         {
                             BatchNoId = r.BatchNoId,
                             BatchNumber = r.BatchNumber,
                             ExpirationDate = r.ExpirationDate,
                             Date = r.Date,
                             LocationId = r.LocationId,
                             LocationName = r.LocationName,
                             LotId = r.LotId,
                             LotName = r.LotName,
                             ItemId = r.ItemId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             Unit = unit == null ? "" : unit.Value,
                             ItemGroup = itemGroup == null ? "" : itemGroup.Value,
                             Properties = i.Properties,
                             InQty = r.InQty,
                             OutQty = r.OutQty,
                             BalanceQty = r.BalanceQty,
                             NetWeight = r.BalanceQty * netWeight,
                             ToDate = input.ToDate,
                         };

            var resultCount = 0;

            var sumOfColumns = new Dictionary<string, decimal>();
            if (input.IsLoadMore == false)
            {
                resultCount = await result.CountAsync();
            }

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<BatchNoBalanceOutput>(resultCount, new List<BatchNoBalanceOutput>(), sumOfColumns);
            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await result.Select(t => new { t.NetWeight, t.BalanceQty, t.InQty, t.OutQty }).ToListAsync();
                foreach (var c in input.ColumnNamesToSum)
                {
                    if (c == "NetWeight") sumOfColumns.Add(c, sumList.Select(u => u.NetWeight).Sum());
                    else if (c == "InQty") sumOfColumns.Add(c, sumList.Select(u => u.InQty).Sum());
                    else if (c == "OutQty") sumOfColumns.Add(c, sumList.Select(u => u.OutQty).Sum());
                    else if (c == "BalanceQty") sumOfColumns.Add(c, sumList.Select(u => u.BalanceQty).Sum());
                }
            }



            switch (input.GroupBy)
            {
                case "Item":
                    result = result.OrderBy(s => s.ItemCode).ThenBy(s => s.LocationName).ThenBy(s => s.LotName).ThenBy(s => s.BatchNumber);
                    break;
                case "ItemGroup":
                    result = result.OrderBy(s => s.ItemGroup).ThenBy(s => s.LocationName).ThenBy(s => s.LotName).ThenBy(s => s.BatchNumber);
                    break;
                case "Lot":
                    result = result.OrderBy(s => s.LotName).ThenBy(s => s.LocationName).ThenBy(s => s.LotName).ThenBy(s => s.BatchNumber);
                    break;
                case "Location":
                    result = result.OrderBy(s => s.LocationName).ThenBy(s => s.LotName).ThenBy(s => s.BatchNumber);
                    break;
                default:
                    //group only 4 so default is lot
                    result = result.OrderBy(s => s.LocationName).ThenBy(s => s.LotName).ThenBy(s => s.BatchNumber);
                    break;
            }



            var @entities = new List<BatchNoBalanceOutput>();

            if (input.UsePagination == true)
            {
                @entities = await result.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await result.ToListAsync();
            }

            return new PagedResultWithTotalColuumnsDto<BatchNoBalanceOutput>(resultCount, @entities, sumOfColumns);

        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BatchNoBalance_ExportExcel)]
        public async Task<FileDto> ExportExcelBatchNoBalanceReport(GetBatchNoBalanceReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var batchData = (await GetBatchNoBalanceReport(input)).Items;

            var result = new FileDto();

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

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<BatchNoBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = batchData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = batchData
                                .GroupBy(t => t.LotId)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "ItemGroup":
                            groupBy = batchData
                                .GroupBy(t => t.ItemGroup)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.ItemGroup).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Item":
                            groupBy = batchData
                                .GroupBy(t => t.ItemId)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyBatchNoBalance(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in batchData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyBatchNoBalance(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"BatchNo_Balance_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_BatchNoBalance_ExportPdf)]
        public async Task<FileDto> ExportPdfBatchNoBalanceReport(GetBatchNoBalanceReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var stockBalanceData = (await GetBatchNoBalanceReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "BatchNoBalanceReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentGroupby = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<BatchNoBalanceReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Lot":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LotId)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LotName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "ItemGroup":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.ItemGroup)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.ItemGroup).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Item":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.ItemId)
                                .Select(t => new BatchNoBalanceReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode + " - " + x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                    if (keyName == "Lot")
                                    {
                                        trGroup += $"<td>{row.LotName}</td>";
                                    }
                                    else if (keyName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (keyName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (keyName == "BatchNumber")
                                    {
                                        trGroup += $"<td>{row.BatchNumber}</td>";
                                    }
                                    else if (keyName == "Date")
                                    {
                                        trGroup += $"<td>{row.Date.ToString(formatDate)}</td>";
                                    }
                                    else if (keyName == "ExpirationDate")
                                    {
                                        trGroup += $"<td>{row.ExpirationDate?.ToString(formatDate)}</td>";
                                    }
                                    else if (keyName == "Aging")
                                    {
                                        trGroup += $"<td>{row.Aging}</td>";
                                    }
                                    else if (keyName == "Expiring")
                                    {
                                        trGroup += $"<td>{row.Expiring}</td>";
                                    }
                                    else if (keyName == "InQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.InQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "BalanceQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.BalanceQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (p != null)
                                    {
                                        trGroup += $"<td>{p.Value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "InQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.InQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "OutQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.OutQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "BalanceQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.BalanceQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;

                }
                else // write body no group by
                {
                    foreach (var row in stockBalanceData.Items)
                    {
                        var tr = "<tr>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                var p = row.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

                                if (keyName == "Lot")
                                {
                                    tr += $"<td>{row.LotName}</td>";
                                }
                                else if (keyName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (keyName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (keyName == "BatchNumber")
                                {
                                    tr += $"<td>{row.BatchNumber}</td>";
                                }
                                else if (keyName == "Date")
                                {
                                    tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                                }
                                else if (keyName == "ExpirationDate")
                                {
                                    tr += $"<td>{row.ExpirationDate?.ToString(formatDate)}</td>";
                                }
                                else if (keyName == "Aging")
                                {
                                    tr += $"<td>{row.Aging}</td>";
                                }
                                else if (keyName == "Expiring")
                                {
                                    tr += $"<td>{row.Expiring}</td>";
                                }
                                else if (keyName == "InQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.InQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "OutQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.OutQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "BalanceQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.BalanceQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (keyName == "NetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (p != null)
                                {
                                    tr += $"<td>{p.Value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && stockBalanceData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(stockBalanceData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }

                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, DateTime.MinValue, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });

        }

        #endregion


        #region Production Report

        public async Task<ReportOutput> GetReportTemplateProduction()
        {

            var columnInfo = new List<CollumnOutput>() {
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Search",
                    ColumnLength = 150,
                    ColumnTitle = "Search",
                    ColumnType = ColumnType.Language,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                    IsDisplay = false//no need to show in col check it belong to filter option
                },
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "DateRange",
                    ColumnLength = 150,
                    ColumnTitle = "Date Range",
                    ColumnType = ColumnType.Language,
                    SortOrder = 0,
                    Visible = true,
                    DefaultValue = "asOf",
                    AllowFunction = null,
                    DisableDefault = true,
                    MoreFunction = null,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                    IsDisplay = false//no need to show in col check it belong to filter option
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "DocumentNo",
                    ColumnLength = 150,
                    ColumnTitle = "Document No",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Reference",
                    ColumnLength = 150,
                    ColumnTitle = "Reference",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = true,
                    AllowFilter = true,
                    ColumnName = "Location",
                    ColumnLength = 150,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ProductionLine",
                    ColumnLength = 150,
                    ColumnTitle = "Production Line",
                    ColumnType = ColumnType.String,
                    SortOrder = 4,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },


                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "StartDate",
                    ColumnLength = 150,
                    ColumnTitle = "Start Date",
                    ColumnType = ColumnType.Date,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "EndDate",
                    ColumnLength = 150,
                    ColumnTitle = "End Date",
                    ColumnType = ColumnType.Date,
                    SortOrder = 6,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "User",
                    ColumnLength = 100,
                    ColumnTitle = "User",
                    ColumnType = ColumnType.String,
                    SortOrder = 17,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ProductionPlanStatus",
                    ColumnLength = 100,
                    ColumnTitle = "Status",
                    ColumnType = ColumnType.String,
                    SortOrder = 18,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ProductionProcess",
                    ColumnLength = 100,
                    ColumnTitle = "Production Process",
                    ColumnType = ColumnType.String,
                    SortOrder = 19,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Description",
                    ColumnLength = 100,
                    ColumnTitle = "Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 20,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
            };

            var tenant = await GetCurrentTenantAsync();

            if (tenant.ProductionSummaryQty)
            {
                columnInfo.AddRange(new List<CollumnOutput>
                {
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalIssueQty",
                        ColumnLength = 170,
                        ColumnTitle = "Issue Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalReceiptQty",
                        ColumnLength = 170,
                        ColumnTitle = "Receipt Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "QtyBalance",
                        ColumnLength = 170,
                        ColumnTitle = "Differences",
                        ColumnType = ColumnType.Number,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "QtyYield",
                        ColumnLength = 170,
                        ColumnTitle = "Yield",
                        ColumnType = ColumnType.Percentage,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IssueStandardCostGroups",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Issue",
                        ColumnType = ColumnType.Array,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "StandardCostGroups",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Receive",
                        ColumnType = ColumnType.Array,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                });
            }
            if (tenant.ProductionSummaryNetWeight)
            {
                columnInfo.AddRange(new List<CollumnOutput>
                {
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalIssueNetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Issue (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalReceiptNetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Receipt (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeightBalance",
                        ColumnLength = 170,
                        ColumnTitle = "Differences (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeightYield",
                        ColumnLength = 170,
                        ColumnTitle = "Yield",
                        ColumnType = ColumnType.Percentage,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IssueStandardCostGroupsKg",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Issue (Kg)",
                        ColumnType = ColumnType.Array,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "StandardCostGroupsKg",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Receive (Kg)",
                        ColumnType = ColumnType.Array,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                });
            }

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columnInfo,
                Groupby = "",
                HeaderTitle = "Production",
                Sortby = "",
            };
            return result;
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionPlan)]
        public async Task<PagedResultProductionSummaryDto<ProductionPlanDetailOutput>> GetProductionReport(ProductionReportInput input)
        {

            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var query = from s in _productionPlanRepository.GetAll()
                                    .WhereIf(input.FromDate.HasValue, s => input.FromDate <= s.StartDate)
                                    .WhereIf(input.ToDate.HasValue, s => s.StartDate.Value.Date <= input.ToDate)
                                    .WhereIf(input.PlanStatuses != null && input.PlanStatuses.Any(), s => input.PlanStatuses.Contains(s.Status))
                                    .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId))
                                    .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId))
                                    .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                                    .WhereIf(input.ProductionLines != null && input.ProductionLines.Any(), s => input.ProductionLines.Contains(s.ProductionLineId.Value))
                                    .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                        s.DocumentNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        s.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                        (s.ProductionLineId.HasValue && s.ProductionLine.ProductionLineName.ToLower().Contains(input.Filter.ToLower()))
                                    )
                                    .AsNoTracking()

                        join g in _productionStandardCostRepository.GetAll().AsNoTracking()
                                  .Select(s => new StandardCostGroupSummary
                                  {
                                      ProductionPlanId = s.ProductionPlanId,
                                      GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                      QtyPercentage = s.QtyPercentage * 100,
                                      NetWeightPercentage = s.NetWeightPercentage * 100,
                                      TotalNetWeight = s.TotalNetWeight,
                                      TotalQty = s.TotalQty,
                                  })
                        on s.Id equals g.ProductionPlanId
                        into groups

                        join ig in _productionIssueStandardCostRepository.GetAll().AsNoTracking()
                                 .Select(s => new StandardCostGroupSummary
                                 {
                                     ProductionPlanId = s.ProductionPlanId,
                                     GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                     QtyPercentage = s.QtyPercentage * 100,
                                     NetWeightPercentage = s.NetWeightPercentage * 100,
                                     TotalNetWeight = s.TotalNetWeight,
                                     TotalQty = s.TotalQty,
                                 })
                       on s.Id equals ig.ProductionPlanId
                       into igroups

                        select new ProductionPlanDetailOutput
                        {
                            Id = s.Id,
                            Reference = s.Reference,
                            DocumentNo = s.DocumentNo,
                            StartDate = s.StartDate,
                            EndDate = s.EndDate,
                            Description = s.Description,
                            LocationId = s.LocationId,
                            LocationName = s.Location.LocationName,
                            Status = s.Status,
                            ProductionPlanStatus = s.Status.ToString(),
                            UserId = s.CreatorUserId,
                            UserName = s.CreatorUserId.HasValue ? s.CreatorUser.UserName : "",
                            ProductionLineId = s.ProductionLineId,
                            ProductionLineName = s.ProductionLineId.HasValue ? s.ProductionLine.ProductionLineName : "",
                            TotalIssueQty = s.TotalIssueQty,
                            TotalReceiptQty = s.TotalReceiptQty,
                            TotalIssueNetWeight = s.TotalIssueNetWeight,
                            TotalReceiptNetWeight = s.TotalReceiptNetWeight,
                            QtyBalance = s.TotalIssueQty - s.TotalReceiptQty,
                            NetWeightBalance = s.TotalIssueNetWeight - s.TotalReceiptNetWeight,
                            QtyYield = s.TotalIssueQty == 0 ? 0 : Math.Round(s.TotalReceiptQty / s.TotalIssueQty, 4) * 100,
                            NetWeightYield = s.TotalIssueNetWeight == 0 ? 0 : Math.Round(s.TotalReceiptNetWeight / s.TotalIssueNetWeight, 4) * 100,
                            StandardCostGroups = groups.OrderBy(g => g.GroupName).ToList(),
                            IssueStandardCostGroups = igroups.OrderBy(g => g.GroupName).ToList(),
                            ProductionProcess = s.ProductionProcess
                        };

            var resultCount = 0;

            var sumOfColumns = new Dictionary<string, decimal>();
            var summaries = new List<ProductionPlanSummary>();
            var issueSummaries = new List<ProductionPlanSummary>();
            if (input.IsLoadMore == false)
            {
                resultCount = await query.CountAsync();
            }

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultProductionSummaryDto<ProductionPlanDetailOutput>(resultCount, new List<ProductionPlanDetailOutput>(), sumOfColumns, summaries, issueSummaries);
            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var summaryList = await query.Select(s => new
                {
                    s.TotalIssueQty,
                    s.TotalReceiptQty,
                    s.TotalIssueNetWeight,
                    s.TotalReceiptNetWeight,
                    s.StandardCostGroups,
                    s.IssueStandardCostGroups
                })
               .ToListAsync();

                decimal totalIssueQty = summaryList.Sum(t => t.TotalIssueQty);
                decimal totalReceiptQty = summaryList.Sum(t => t.TotalReceiptQty);
                decimal totalQtyBalance = summaryList.Sum(t => t.TotalIssueQty - t.TotalReceiptQty);
                decimal totalIssueNetWeight = summaryList.Sum(t => t.TotalIssueNetWeight);
                decimal totalReceiptNetWeight = summaryList.Sum(t => t.TotalReceiptNetWeight);
                decimal totalNetWeightBalance = summaryList.Sum(t => t.TotalIssueNetWeight - t.TotalReceiptNetWeight);
                decimal qtyYield = totalIssueQty == 0 ? 0 : Math.Round(totalReceiptQty / totalIssueQty, 4) * 100;
                decimal netWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(totalReceiptNetWeight / totalIssueNetWeight, 4) * 100;

                summaries = summaryList
                                .SelectMany(s => s.StandardCostGroups)
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
                                .SelectMany(s => s.IssueStandardCostGroups)
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

                foreach (var c in input.ColumnNamesToSum)
                {
                    switch (c)
                    {
                        case "TotalIssueQty":
                            sumOfColumns.Add(c, totalIssueQty);
                            break;
                        case "TotalReceiptQty":
                            sumOfColumns.Add(c, totalReceiptQty);
                            break;
                        case "QtyBalance":
                            sumOfColumns.Add(c, totalQtyBalance);
                            break;
                        case "QtyYield":
                            sumOfColumns.Add(c, qtyYield);
                            break;
                        case "TotalIssueNetWeight":
                            sumOfColumns.Add(c, totalIssueNetWeight);
                            break;
                        case "TotalReceiptNetWeight":
                            sumOfColumns.Add(c, totalReceiptNetWeight);
                            break;
                        case "NetWeightBalance":
                            sumOfColumns.Add(c, totalNetWeightBalance);
                            break;
                        case "NetWeightYield":
                            sumOfColumns.Add(c, netWeightYield);
                            break;
                    }
                }
            }

            switch (input.GroupBy)
            {
                case "Location":
                    query = query.OrderBy(s => s.LocationName).ThenBy(s => s.DocumentNo);
                    break;
                default:
                    //group only 4 so default is lot
                    query = query.OrderBy(s => s.DocumentNo);
                    break;
            }

            var @entities = new List<ProductionPlanDetailOutput>();

            if (input.UsePagination == true)
            {
                @entities = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await query.ToListAsync();
            }

            return new PagedResultProductionSummaryDto<ProductionPlanDetailOutput>(resultCount, @entities, sumOfColumns, summaries, issueSummaries);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportExcel)]
        public async Task<FileDto> ExportExcelProductionReport(GetProductionReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var getResult = await GetProductionReport(input);
            var batchData = getResult.Items;

            var result = new FileDto();

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
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.Value.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<ProductionPlanDetailGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = batchData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new ProductionPlanDetailGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyProduction(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }

                                if (item.ColumnName == "StandardCostGroups")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                    var standardGroups = k.Items.Where(s => !s.StandardCostGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.StandardCostGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalQty = s.Sum(t => t.TotalQty),
                                                              QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "StandardCostGroupsKg")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                    var standardGroups = k.Items.Where(s => !s.StandardCostGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.StandardCostGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                              NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "IssueStandardCostGroups")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                    var standardGroups = k.Items.Where(s => !s.IssueStandardCostGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.IssueStandardCostGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalQty = s.Sum(t => t.TotalQty),
                                                              QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "IssueStandardCostGroupsKg")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                    var standardGroups = k.Items.Where(s => !s.IssueStandardCostGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.IssueStandardCostGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                              NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "QtyYield")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);
                                    decimal totalReceipt = k.Items.Sum(x => x.TotalReceiptQty);
                                    decimal yield = totalIssue == 0 ? 0 : Math.Round(totalReceipt / totalIssue, 4) * 100;
                                    AddNumberToCell(ws, rowBody, collumnCellGroupBody, yield, false, false, true);
                                }
                                else if (item.ColumnName == "NetWeightYield")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);
                                    decimal totalReceipt = k.Items.Sum(x => x.TotalReceiptNetWeight);
                                    decimal yield = totalIssue == 0 ? 0 : Math.Round(totalReceipt / totalIssue, 4) * 100;
                                    AddNumberToCell(ws, rowBody, collumnCellGroupBody, yield, false, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in batchData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyProduction(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (i.ColumnName == "StandardCostGroups")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptQty:0.00} = {s.QtyYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "StandardCostGroupsKg")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "IssueStandardCostGroups")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueQty:0.00} = {s.QtyYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "IssueStandardCostGroupsKg")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "QtyYield")
                            {
                                decimal totalIssue = batchData.Sum(x => x.TotalIssueQty);
                                decimal totalReceipt = batchData.Sum(x => x.TotalReceiptQty);
                                decimal yield = totalIssue == 0 ? 0 : Math.Round(totalReceipt / totalIssue, 4) * 100;
                                AddNumberToCell(ws, footerRow, footerColNumber, yield, false, false, true);
                            }
                            else if (i.ColumnName == "NetWeightYield")
                            {
                                decimal totalIssue = batchData.Sum(x => x.TotalIssueNetWeight);
                                decimal totalReceipt = batchData.Sum(x => x.TotalReceiptNetWeight);
                                decimal yield = totalIssue == 0 ? 0 : Math.Round(totalReceipt / totalIssue, 4) * 100;
                                AddNumberToCell(ws, footerRow, footerColNumber, yield, false, false, true);
                            }
                            else if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Production_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportPdf)]
        public async Task<FileDto> ExportPdfProductionReport(GetProductionReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var stockBalanceData = (await GetProductionReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "ProductionReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentGroupby = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<ProductionPlanDetailGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new ProductionPlanDetailGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='vertical-align:top; page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='vertical-align:top; page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    if (keyName == "DocumentNo")
                                    {
                                        trGroup += $"<td>{row.DocumentNo}</td>";
                                    }
                                    else if (keyName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (keyName == "ProductionProcess")
                                    {
                                        trGroup += $"<td>{row.ProductionProcess}</td>";
                                    }
                                    else if (keyName == "Description")
                                    {
                                        trGroup += $"<td>{row.Description}</td>";
                                    }
                                    else if (keyName == "ProductionLine")
                                    {
                                        trGroup += $"<td>{row.ProductionLineName}</td>";
                                    }
                                    else if (keyName == "StartDate")
                                    {
                                        trGroup += $"<td>{row.StartDate.Value.ToString(formatDate)}</td>";
                                    }
                                    else if (keyName == "EndDate")
                                    {
                                        trGroup += $"<td>{row.EndDate?.ToString(formatDate)}</td>";
                                    }
                                    else if (keyName == "ProductionPlanStatus")
                                    {
                                        trGroup += $"<td>{row.ProductionPlanStatus}</td>";
                                    }
                                    else if (keyName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (keyName == "User")
                                    {
                                        trGroup += $"<td>{row.UserName}</td>";
                                    }
                                    else if (keyName == "TotalIssueQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "QtyBalance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.QtyBalance, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "QtyYield")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.QtyYield, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}%</td>";
                                    }

                                    else if (keyName == "TotalIssueNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeightBalance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeightBalance, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeightYield")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeightYield, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}%</td>";
                                    }
                                    else if (keyName == "StandardCostGroups")
                                    {
                                        var value = "";
                                        foreach (var g in row.StandardCostGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalQty:0.00} = {g.QtyPercentage:0.00}%</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else if (keyName == "StandardCostGroupsKg")
                                    {
                                        var value = "";
                                        foreach (var g in row.StandardCostGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg = {g.NetWeightPercentage:0.00}%</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else if (keyName == "IssueStandardCostGroups")
                                    {
                                        var value = "";
                                        foreach (var g in row.IssueStandardCostGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalQty:0.00} = {g.QtyPercentage:0.00}%</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else if (keyName == "IssueStandardCostGroupsKg")
                                    {
                                        var value = "";
                                        foreach (var g in row.IssueStandardCostGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg = {g.NetWeightPercentage:0.00}%</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='vertical-align:top; font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "TotalIssueQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalIssueQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalReceiptQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "QtyBalance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.QtyBalance), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "QtyYield")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);
                                        decimal totalReceipt = k.Items.Sum(x => x.TotalReceiptQty);
                                        decimal yield = totalIssue == 0 ? 0 : Math.Round(totalReceipt / totalIssue, 4) * 100;

                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(yield, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}%</td>";
                                    }
                                    else if (keyName == "TotalIssueNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalIssueNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalReceiptNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeightBalance")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.NetWeightBalance), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "NetWeightYield")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);
                                        decimal totalReceipt = k.Items.Sum(x => x.TotalReceiptNetWeight);
                                        decimal yield = totalIssue == 0 ? 0 : Math.Round(totalReceipt / totalIssue, 4) * 100;

                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(yield, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}%</td>";
                                    }
                                    else if (keyName == "StandardCostGroups")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                        var standardGroups = k.Items.Where(s => !s.StandardCostGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.StandardCostGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalQty = s.Sum(t => t.TotalQty),
                                                                  QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                    else if (keyName == "StandardCostGroupsKg")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                        var standardGroups = k.Items.Where(s => !s.StandardCostGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.StandardCostGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                                  NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                    else if (keyName == "IssueStandardCostGroups")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                        var standardGroups = k.Items.Where(s => !s.IssueStandardCostGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.IssueStandardCostGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalQty = s.Sum(t => t.TotalQty),
                                                                  QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                    else if (keyName == "IssueStandardCostGroupsKg")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                        var standardGroups = k.Items.Where(s => !s.IssueStandardCostGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.IssueStandardCostGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                                  NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;

                }
                else // write body no group by
                {
                    foreach (var row in stockBalanceData.Items)
                    {
                        var tr = "<tr style='vertical-align:top;'>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                if (keyName == "DocumentNo")
                                {
                                    tr += $"<td>{row.DocumentNo}</td>";
                                }
                                else if (keyName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (keyName == "ProductionProcess")
                                {
                                    tr += $"<td>{row.ProductionProcess}</td>";
                                }
                                else if (keyName == "Description")
                                {
                                    tr += $"<td>{row.Description}</td>";
                                }
                                else if (keyName == "ProductionLine")
                                {
                                    tr += $"<td>{row.ProductionLineName}</td>";
                                }
                                else if (keyName == "StartDate")
                                {
                                    tr += $"<td>{row.StartDate.Value.ToString(formatDate)}</td>";
                                }
                                else if (keyName == "EndDate")
                                {
                                    tr += $"<td>{row.EndDate?.ToString(formatDate)}</td>";
                                }
                                else if (keyName == "ProductionPlanStatus")
                                {
                                    tr += $"<td>{row.ProductionPlanStatus}</td>";
                                }
                                else if (keyName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (keyName == "User")
                                {
                                    tr += $"<td>{row.UserName}</td>";
                                }
                                else if (keyName == "TotalIssueQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalReceiptQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "QtyBalance")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.QtyBalance, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "QtyYield")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.QtyYield, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}%</td>";
                                }

                                else if (keyName == "TotalIssueNetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalReceiptNetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "NetWeightBalance")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeightBalance, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "NetWeightYield")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.NetWeightYield, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}%</td>";
                                }
                                else if (keyName == "StandardCostGroups")
                                {
                                    var value = "";
                                    foreach (var g in row.StandardCostGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalQty:0.00} = {g.QtyPercentage:0.00}%</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else if (keyName == "StandardCostGroupsKg")
                                {
                                    var value = "";
                                    foreach (var g in row.StandardCostGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg = {g.NetWeightPercentage:0.00}%</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else if (keyName == "IssueStandardCostGroups")
                                {
                                    var value = "";
                                    foreach (var g in row.IssueStandardCostGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalQty:0.00} = {g.QtyPercentage:0.00}%</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else if (keyName == "IssueStandardCostGroupsKg")
                                {
                                    var value = "";
                                    foreach (var g in row.IssueStandardCostGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg = {g.NetWeightPercentage:0.00}%</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='vertical-align:top; page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "StandardCostGroups")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptQty:0.00} = {s.QtyYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "StandardCostGroupsKg")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "IssueStandardCostGroups")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueQty:0.00} = {s.QtyYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "IssueStandardCostGroupsKg")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && stockBalanceData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(stockBalanceData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}{(i.ColumnName == "QtyYield" || i.ColumnName == "NetWeightYield" ? "%" : "")}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }

                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, DateTime.MinValue, input.ToDate.Value, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionPlan_Calculate)]
        public async Task Calculation(ProductionPlanCalculationInput input)
        {
            await _productionPlanManager.CalculateAsync(AbpSession.UserId.Value, input.FromDate, input.ToDate);
        }

        #endregion

        #region Production Order Report

        public async Task<ReportOutput> GetReportTemplateProductionOrder()
        {

            var columnInfo = new List<CollumnOutput>() {
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Search",
                    ColumnLength = 150,
                    ColumnTitle = "Search",
                    ColumnType = ColumnType.Language,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                    IsDisplay = false//no need to show in col check it belong to filter option
                },
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "DateRange",
                    ColumnLength = 150,
                    ColumnTitle = "Date Range",
                    ColumnType = ColumnType.Language,
                    SortOrder = 0,
                    Visible = true,
                    DefaultValue = "asOf",
                    AllowFunction = null,
                    DisableDefault = true,
                    MoreFunction = null,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                    IsDisplay = false//no need to show in col check it belong to filter option
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ProductionDate",
                    ColumnLength = 100,
                    ColumnTitle = "Date",
                    ColumnType = ColumnType.Date,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ProductionNo",
                    ColumnLength = 150,
                    ColumnTitle = "Production No",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Reference",
                    ColumnLength = 150,
                    ColumnTitle = "Reference",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = true,
                    AllowFilter = true,
                    ColumnName = "Location",
                    ColumnLength = 150,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "FromLocation",
                    ColumnLength = 150,
                    ColumnTitle = "From Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 4,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ToLocation",
                    ColumnLength = 150,
                    ColumnTitle = "To Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ProductionProcess",
                    ColumnLength = 150,
                    ColumnTitle = "Production Process",
                    ColumnType = ColumnType.String,
                    SortOrder = 6,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ProductionProcessType",
                    ColumnLength = 150,
                    ColumnTitle = "Production Process Type",
                    ColumnType = ColumnType.String,
                    SortOrder = 6,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ProductionPlanNo",
                    ColumnLength = 150,
                    ColumnTitle = "Production Plan No",
                    ColumnType = ColumnType.String,
                    SortOrder = 7,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "User",
                    ColumnLength = 100,
                    ColumnTitle = "User",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ReceiveStatus",
                    ColumnLength = 120,
                    ColumnTitle = "Receive Status",
                    ColumnType = ColumnType.StatusCode,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                 new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Status",
                    ColumnLength = 100,
                    ColumnTitle = "Status",
                    ColumnType = ColumnType.StatusCode,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },

            };

            var tenant = await GetCurrentTenantAsync();

            if (tenant.ProductionSummaryQty)
            {
                columnInfo.AddRange(new List<CollumnOutput>
                {
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalIssueQty",
                        ColumnLength = 170,
                        ColumnTitle = "Issue Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalReceiptQty",
                        ColumnLength = 170,
                        ColumnTitle = "Receipt Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IssueStandardGroups",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Issue",
                        ColumnType = ColumnType.Array,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "StandardGroups",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Receive",
                        ColumnType = ColumnType.Array,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                });
            }
            if (tenant.ProductionSummaryNetWeight)
            {
                columnInfo.AddRange(new List<CollumnOutput>
                {
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalIssueNetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Issue (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalReceiptNetWeight",
                        ColumnLength = 170,
                        ColumnTitle = "Receipt (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 15,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IssueStandardGroupsKg",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Issue (Kg)",
                        ColumnType = ColumnType.Array,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "StandardGroupsKg",
                        ColumnLength = 250,
                        ColumnTitle = "Standard Group Receive (Kg)",
                        ColumnType = ColumnType.Array,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                });
            }

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columnInfo,
                Groupby = "",
                HeaderTitle = "Production Order",
                Sortby = "",
            };
            return result;
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionOrder)]
        public async Task<PagedResultProductionSummaryDto<ProductionOrderReportOutput>> GetProductionOrderReport(ProductionOrderReportInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var query = from s in _productionsRepository.GetAll()
                                    .WhereIf(input.FromDate.HasValue, s => input.FromDate <= s.Date.Date)
                                    .WhereIf(input.ToDate.HasValue, s => s.Date.Date <= input.ToDate)
                                    .WhereIf(input.CalculationStates != null && input.CalculationStates.Any(), s => input.CalculationStates.Contains(s.CalculationState))
                                    .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.FromLocationId) || userGroups.Contains(t.ToLocationId))
                                    .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                    .WhereIf(input.DeliveryStatus != null && input.DeliveryStatus.Count > 0, u => input.DeliveryStatus.Contains(u.ShipedStatus))
                                    .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                                    .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.ToLocationId) || input.Locations.Contains(u.FromLocationId))
                                    .WhereIf(input.NoProductionPlan, s => !s.ProductionPlanId.HasValue)
                                    .WhereIf(input.ProductionPlans != null && input.ProductionPlans.Any() && !input.NoProductionPlan, s => input.ProductionPlans.Contains(s.ProductionPlanId.Value))
                                    .WhereIf(input.ProductionPlanStatus != null && input.ProductionPlanStatus.Any() && !input.NoProductionPlan, s => s.ProductionPlanId.HasValue && input.ProductionPlanStatus.Contains(s.ProductionPlan.Status))
                                    .WhereIf(input.ProductionLines != null && input.ProductionLines.Any() && !input.NoProductionPlan, s => s.ProductionPlanId.HasValue && input.ProductionLines.Contains(s.ProductionPlan.ProductionLineId.Value))
                                    .WhereIf(input.ProductionProcess != null && input.ProductionProcess.Any(), s => s.ProductionProcessId.HasValue && input.ProductionProcess.Contains(s.ProductionProcessId.Value))
                                    .WhereIf(input.ProductionProcessTypes != null && input.ProductionProcessTypes.Any(), s => s.ProductionProcessId.HasValue && input.ProductionProcessTypes.Contains(s.ProductionProcess.ProductionProcessType))
                                    .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                        s.ProductionNo.ToLower().Contains(input.Filter.ToLower()) ||
                                        s.Reference.ToLower().Contains(input.Filter.ToLower())
                                    )
                                    .AsNoTracking()

                        join g in _productionStandardCostGroupRepository.GetAll().AsNoTracking()
                                  .Select(s => new StandardGroupSummary
                                  {
                                      ProductionId = s.ProductionId,
                                      GroupId = s.StandardCostGroupId,
                                      GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                      TotalNetWeight = s.TotalNetWeight,
                                      TotalQty = s.TotalQty,
                                  })
                        on s.Id equals g.ProductionId
                        into groups

                        join ig in _productionIssueStandardCostGroupRepository.GetAll().AsNoTracking()
                                  .Select(s => new StandardGroupSummary
                                  {
                                      ProductionId = s.ProductionId,
                                      GroupId = s.StandardCostGroupId,
                                      GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                      TotalNetWeight = s.TotalNetWeight,
                                      TotalQty = s.TotalQty,
                                  })
                        on s.Id equals ig.ProductionId
                        into igroups

                        select new ProductionOrderReportOutput
                        {
                            Id = s.Id,
                            Reference = s.Reference,
                            ProductionNo = s.ProductionNo,
                            ProductionDate = s.Date,
                            Description = s.Memo,
                            FromLocation = s.FromLocation.LocationName,
                            ToLocation = s.ToLocation.LocationName,
                            User = s.CreatorUserId.HasValue ? s.CreatorUser.UserName : "",
                            TotalIssueQty = s.TotalIssueQty,
                            TotalReceiptQty = s.TotalReceiptQty,
                            TotalIssueNetWeight = s.TotalIssueNetWeight,
                            TotalReceiptNetWeight = s.TotalReceiptNetWeight,
                            Status = s.Status,
                            ProductionPlanNo = s.ProductionPlanId.HasValue ? s.ProductionPlan.DocumentNo : "",
                            ProductionProcess = s.ProductionProcessId.HasValue ? s.ProductionProcess.ProcessName : "",
                            ReceiveStatus = s.ShipedStatus,
                            StandardGroups = groups.OrderBy(g => g.GroupName).ToList(),
                            IssueStandardGroups = igroups.OrderBy(g => g.GroupName).ToList()
                        };

            var resultCount = 0;

            var summaries = new List<ProductionPlanSummary>();
            var issueSummaries = new List<ProductionPlanSummary>();
            var sumOfColumns = new Dictionary<string, decimal>();

            if (input.IsLoadMore == false)
            {
                resultCount = await query.CountAsync();
            }

            if (resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultProductionSummaryDto<ProductionOrderReportOutput>(resultCount, new List<ProductionOrderReportOutput>(), sumOfColumns, summaries, issueSummaries);
            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var summaryList = await query.Select(s => new
                {
                    s.TotalIssueQty,
                    s.TotalReceiptQty,
                    s.TotalIssueNetWeight,
                    s.TotalReceiptNetWeight,
                    s.StandardGroups,
                    s.IssueStandardGroups
                })
               .ToListAsync();

                decimal totalIssueQty = summaryList.Sum(t => t.TotalIssueQty);
                decimal totalReceiptQty = summaryList.Sum(t => t.TotalReceiptQty);
                decimal totalQtyBalance = summaryList.Sum(t => t.TotalIssueQty - t.TotalReceiptQty);
                decimal totalIssueNetWeight = summaryList.Sum(t => t.TotalIssueNetWeight);
                decimal totalReceiptNetWeight = summaryList.Sum(t => t.TotalReceiptNetWeight);
                decimal totalNetWeightBalance = summaryList.Sum(t => t.TotalIssueNetWeight - t.TotalReceiptNetWeight);
                decimal qtyYield = totalIssueQty == 0 ? 0 : Math.Round(totalReceiptQty / totalIssueQty, 4) * 100;
                decimal netWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(totalReceiptNetWeight / totalIssueNetWeight, 4) * 100;

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

                foreach (var c in input.ColumnNamesToSum)
                {
                    switch (c)
                    {
                        case "TotalIssueQty":
                            sumOfColumns.Add(c, totalIssueQty);
                            break;
                        case "TotalReceiptQty":
                            sumOfColumns.Add(c, totalReceiptQty);
                            break;
                        case "TotalIssueNetWeight":
                            sumOfColumns.Add(c, totalIssueNetWeight);
                            break;
                        case "TotalReceiptNetWeight":
                            sumOfColumns.Add(c, totalReceiptNetWeight);
                            break;
                    }
                }
            }

            switch (input.GroupBy)
            {
                case "Location":
                    query = query.OrderBy(s => s.FromLocation).ThenBy(s => s.ProductionNo);
                    break;
                default:
                    //group only 4 so default is lot
                    query = query.OrderBy(s => s.ProductionNo);
                    break;
            }

            var @entities = new List<ProductionOrderReportOutput>();

            if (input.UsePagination == true)
            {
                @entities = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await query.ToListAsync();
            }

            var result = new PagedResultProductionSummaryDto<ProductionOrderReportOutput>(resultCount, @entities, sumOfColumns, summaries, issueSummaries);


            return result;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportExcel)]
        public async Task<FileDto> ExportExcelProductionOrderReport(GetProductionOrderReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var getResult = await GetProductionOrderReport(input);
            var batchData = getResult.Items;

            var result = new FileDto();

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
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.Value.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<ProductionGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = batchData
                                .GroupBy(t => t.FromLocation)
                                .Select(t => new ProductionGroupByOutput
                                {
                                    KeyName = t.Select(a => a.FromLocation).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyProductionOrder(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }

                                if (item.ColumnName == "StandardGroups")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                    var standardGroups = k.Items.Where(s => !s.StandardGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.StandardGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalQty = s.Sum(t => t.TotalQty),
                                                              QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "StandardGroupsKg")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                    var standardGroups = k.Items.Where(s => !s.StandardGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.StandardGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                              NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "IssueStandardGroups")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                    var standardGroups = k.Items.Where(s => !s.IssueStandardGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.IssueStandardGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalQty = s.Sum(t => t.TotalQty),
                                                              QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else if (item.ColumnName == "IssueStandardGroupsKg")
                                {
                                    decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                    var standardGroups = k.Items.Where(s => !s.IssueStandardGroups.IsNullOrEmpty())
                                                          .SelectMany(s => s.IssueStandardGroups)
                                                          .OrderBy(s => s.GroupName)
                                                          .GroupBy(s => s.GroupName)
                                                          .Select(s => new StandardGroupSummary
                                                          {
                                                              GroupName = s.Key,
                                                              TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                              NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                          })
                                                          .ToList();

                                    var yield = string.Join(Environment.NewLine, standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                    AddTextToCell(ws, rowBody, collumnCellGroupBody, yield, true, 0, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in batchData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyProductionOrder(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (i.ColumnName == "StandardGroups")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptQty:0.00} = {s.QtyYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "StandardGroupsKg")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "IssueStandardGroups")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueQty:0.00} = {s.QtyYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (i.ColumnName == "IssueStandardGroupsKg")
                            {
                                var yield = string.Join(Environment.NewLine, getResult.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                AddTextToCell(ws, footerRow, footerColNumber, yield, true, 0, false, true);
                            }
                            else if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }

                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Production_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportPdf)]
        public async Task<FileDto> ExportPdfProductionOrderReport(GetProductionOrderReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            var rounding = await GetCurrentCycleAsync();

            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var stockBalanceData = (await GetProductionOrderReport(input));
            var user = await GetCurrentUserAsync();

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "ProductionReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var contentGroupby = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th width='{i.ColumnLength}'>{i.ColumnTitle}</th>";
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                var groupBy = new List<ProductionGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = stockBalanceData.Items
                                .GroupBy(t => t.FromLocation)
                                .Select(t => new ProductionGroupByOutput
                                {
                                    KeyName = t.Select(a => a.FromLocation).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {

                    var trGroup = "";

                    foreach (var k in groupBy)
                    {
                        trGroup += "<tr style='vertical-align:top; page-break-before: auto; page-break-after: auto;'><td style='font-weight: bold;' colspan=" + reportCountColHead + ">" + k.KeyName + " </td></tr>";

                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='vertical-align:top; page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    var keyName = i.ColumnName;

                                    if (keyName == "ProductionNo")
                                    {
                                        trGroup += $"<td>{row.ProductionNo}</td>";
                                    }
                                    else if (keyName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (keyName == "ProductionProcess")
                                    {
                                        trGroup += $"<td>{row.ProductionProcess}</td>";
                                    }
                                    else if (keyName == "ProductionDate")
                                    {
                                        trGroup += $"<td>{row.ProductionDate.ToString(formatDate)}</td>";
                                    }
                                    else if (keyName == "ProductionPlanNo")
                                    {
                                        trGroup += $"<td>{row.ProductionPlanNo}</td>";
                                    }
                                    else if (keyName == "User")
                                    {
                                        trGroup += $"<td>{row.User}</td>";
                                    }
                                    else if (keyName == "FromLocation")
                                    {
                                        trGroup += $"<td>{row.FromLocation}</td>";
                                    }
                                    else if (keyName == "ToLocation")
                                    {
                                        trGroup += $"<td>{row.ToLocation}</td>";
                                    }
                                    else if (keyName == "Status")
                                    {
                                        trGroup += $"<td>{row.Status.ToString()}</td>";
                                    }
                                    else if (keyName == "ReceiveStatus")
                                    {
                                        trGroup += $"<td>{row.ReceiveStatus.ToString()}</td>";
                                    }
                                    else if (keyName == "TotalIssueQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalIssueNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "StandardGroups")
                                    {
                                        var value = "";
                                        foreach (var g in row.StandardGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalQty:0.00}</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else if (keyName == "StandardGroupsKg")
                                    {
                                        var value = "";
                                        foreach (var g in row.StandardGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else if (keyName == "IssueStandardGroups")
                                    {
                                        var value = "";
                                        foreach (var g in row.IssueStandardGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalQty:0.00}</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else if (keyName == "IssueStandardGroupsKg")
                                    {
                                        var value = "";
                                        foreach (var g in row.IssueStandardGroups)
                                        {
                                            value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg</div>";
                                        }
                                        trGroup += $"<td>{value}</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }

                        // sum footer of group 
                        trGroup += "<tr style='vertical-align:top; font-weight: bold;page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    var keyName = i.ColumnName;
                                    if (keyName == "TotalIssueQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalIssueQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptQty")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalReceiptQty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalIssueNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalIssueNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "TotalReceiptNetWeight")
                                    {
                                        trGroup += $"<td align='right'>{FormatNumberCurrency(Math.Round(k.Items.Sum(x => x.TotalReceiptNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (keyName == "StandardGroups")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                        var standardGroups = k.Items.Where(s => !s.StandardGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.StandardGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalQty = s.Sum(t => t.TotalQty),
                                                                  QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                    else if (keyName == "StandardGroupsKg")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                        var standardGroups = k.Items.Where(s => !s.StandardGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.StandardGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                                  NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                    else if (keyName == "IssueStandardGroups")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueQty);

                                        var standardGroups = k.Items.Where(s => !s.IssueStandardGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.IssueStandardGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalQty = s.Sum(t => t.TotalQty),
                                                                  QtyPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                    else if (keyName == "IssueStandardGroupsKg")
                                    {
                                        decimal totalIssue = k.Items.Sum(x => x.TotalIssueNetWeight);

                                        var standardGroups = k.Items.Where(s => !s.IssueStandardGroups.IsNullOrEmpty())
                                                              .SelectMany(s => s.IssueStandardGroups)
                                                              .OrderBy(s => s.GroupName)
                                                              .GroupBy(s => s.GroupName)
                                                              .Select(s => new StandardGroupSummary
                                                              {
                                                                  GroupName = s.Key,
                                                                  TotalNetWeight = s.Sum(t => t.TotalNetWeight),
                                                                  NetWeightPercentage = totalIssue == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssue, 4) * 100
                                                              })
                                                              .ToList();

                                        var yield = string.Join("<br>", standardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00} = {s.NetWeightPercentage:0.00}%"));

                                        trGroup += $"<td>{yield}</td>";
                                    }
                                }
                                else
                                {
                                    trGroup += $"<td></td>";
                                }
                            }
                        }
                        trGroup += "</tr>";
                    }

                    contentBody += trGroup;

                }
                else // write body no group by
                {
                    foreach (var row in stockBalanceData.Items)
                    {
                        var tr = "<tr style='vertical-align:top;'>";

                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                var keyName = i.ColumnName;

                                if (keyName == "ProductionNo")
                                {
                                    tr += $"<td>{row.ProductionNo}</td>";
                                }
                                else if (keyName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (keyName == "ProductionProcess")
                                {
                                    tr += $"<td>{row.ProductionProcess}</td>";
                                }
                                else if (keyName == "ProductionDate")
                                {
                                    tr += $"<td>{row.ProductionDate.ToString(formatDate)}</td>";
                                }
                                else if (keyName == "FromLocation")
                                {
                                    tr += $"<td>{row.FromLocation}</td>";
                                }
                                else if (keyName == "ToLocation")
                                {
                                    tr += $"<td>{row.ToLocation}</td>";
                                }
                                else if (keyName == "User")
                                {
                                    tr += $"<td>{row.User}</td>";
                                }
                                else if (keyName == "Status")
                                {
                                    tr += $"<td>{row.Status.ToString()}</td>";
                                }
                                else if (keyName == "ReceiveStatus")
                                {
                                    tr += $"<td>{row.ReceiveStatus.ToString()}</td>";
                                }
                                else if (keyName == "TotalIssueQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalReceiptQty")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptQty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }

                                else if (keyName == "TotalIssueNetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalIssueNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "TotalReceiptNetWeight")
                                {
                                    tr += $"<td align='right'>{FormatNumberCurrency(Math.Round(row.TotalReceiptNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (keyName == "StandardGroups")
                                {
                                    var value = "";
                                    foreach (var g in row.StandardGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalQty:0.00}</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else if (keyName == "StandardGroupsKg")
                                {
                                    var value = "";
                                    foreach (var g in row.StandardGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else if (keyName == "IssueStandardGroups")
                                {
                                    var value = "";
                                    foreach (var g in row.IssueStandardGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalQty:0.00}</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else if (keyName == "IssueStandardGroupsKg")
                                {
                                    var value = "";
                                    foreach (var g in row.IssueStandardGroups)
                                    {
                                        value += $"<div>{g.GroupName}: {g.TotalNetWeight:0.00}Kg</div>";
                                    }
                                    tr += $"<td>{value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='vertical-align:top; page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (index == 0)
                        {
                            tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                        }
                        else
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "StandardGroups")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptQty:0.00} = {s.QtyYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "StandardGroupsKg")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.StandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalReceiptNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "IssueStandardGroups")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueQty:0.00} = {s.QtyYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && i.ColumnName == "IssueStandardGroupsKg")
                                {
                                    var yield = string.Join("<br>", stockBalanceData.IssueStandardGroups.Select(s => $"{s.ProductionPlan}: {s.TotalIssueNetWeight:0.00} = {s.NetWeightYield:0.00}%"));

                                    tr += $"<td>{yield}</td>";
                                }
                                else if (!string.IsNullOrEmpty(i.AllowFunction) && stockBalanceData.TotalResult.ContainsKey(i.ColumnName))
                                {
                                    tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(stockBalanceData.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}{(i.ColumnName == "QtyYield" || i.ColumnName == "NetWeightYield" ? "%" : "")}</td>";
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }

                            }
                        }
                        index++;
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }


                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();

                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, DateTime.MinValue, input.ToDate.Value, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ProductionOrder_Calculate)]
        public async Task ProductionOrderCalculation(ProductionPlanCalculationInput input)
        {
            await _productionManager.CalculateAsync(AbpSession.UserId.Value, input.FromDate, input.ToDate);
        }

        #endregion

    }
}