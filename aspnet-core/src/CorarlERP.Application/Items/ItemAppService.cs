using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using CorarlERP.Items.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;
using Abp.UI;
using Abp.Extensions;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.SubItems;
using CorarlERP.SubItems.Dto;
using CorarlERP.ItemTypes.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Taxes.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.Journals;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.Inventories;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Dto;
using OfficeOpenXml;
using CorarlERP.Reports;
using System.IO;
using CorarlERP.ItemTypes;
using CorarlERP.Taxes;
using CorarlERP.PropertyValues;
using CorarlERP.PropertyValues.Dto;
using CorarlERP.AccountCycles;
using Abp.AspNetZeroCore.Net;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Vendors.Dto;
using CorarlERP.UserGroups;
using CorarlERP.ItemPrices;
using Abp.Dependency;
using CorarlERP.FileStorages;
using Abp.Domain.Uow;
using CorarlERP.Lots;
using CorarlERP.Lots.Dto;
using CorarlERP.FileUploads;
using CorarlERP.Galleries;
using System.Transactions;
using CorarlERP.BatchNos;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using CorarlERP.MultiTenancy;
using System.Text.RegularExpressions;
using CorarlERP.InventoryCalculationItems;
using Abp.Application.Features;
using CorarlERP.Features;
using EvoPdf.HtmlToPdfClient;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using CorarlERP.PropertyFormulas;
using CorarlERP.Boms;
using CorarlERP.PropertyFormulas.Dto;
using CorarlERP.PurchasePrices;

namespace CorarlERP.Items
{
    [AbpAuthorize]
    public class ItemAppService : ReportBaseClass, IItemAppService
    {
        //private readonly IPropertyManager _propertyManager;
        private readonly IItemManager _itemManager;        
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly AppFolders _appFolders;
        private readonly IInventoryManager _inventoryManager;

        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<ItemType, long> _itemTypeRepository;
        private readonly IRepository<Tax, long> _taxRepository;

        private readonly ICorarlRepository<ItemProperty, Guid> _itemPropertyRepository;
        private readonly IRepository<PropertyValue, long> _propertyValueRepository;
        private readonly IRepository<Property, long> _propertyRepository;

        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<PurchasePriceItem, Guid> _purchasePriceItemRepository;

        private readonly IItemUserGroupManager _itemUserGroupManager;
        private readonly ICorarlRepository<Item, Guid> _itemRepository;
        private readonly ICorarlRepository<ItemsUserGroup, Guid> _itemUserGroupRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IRepository<ItemPriceItem, Guid> _itemPriceItemRepository;
        private readonly IRepository<ItemPrice, Guid> _itemPriceRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IFileUploadManager _fileUploadManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly ICorarlRepository<ItemLot, Guid> _itemLotRepository;
        private readonly ICorarlRepository<BatchNoFormula, long> _batchNoFormulaRepository;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<InventoryCalculationItem, Guid> _inventoryCalculationItemRepository;      
        private readonly ICorarlRepository<ItemCodeFormulaItemType, long> _itemCodeFormulaItemTypeRepository;
        private readonly ICorarlRepository<ItemCodeFormulaProperty, long> _itemCodeFormulaPropertyRepository;
        private readonly ICorarlRepository<ItemCodeFormulaCustom, long> _itemCodeFormulaCustomRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly ICorarlRepository<Bom, Guid> _bomRepository;
        private readonly ICorarlRepository<BomItem, Guid> _bomItemRepository;
        public ItemAppService(
            IItemManager itemManager,
            IRepository<PurchasePriceItem, Guid> purchasePriceItemRepository,
            ICorarlRepository<BatchNoFormula, long> batchNoFormulaRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<Item, Guid> itemRepository,                             
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemIssue, Guid> itemIssueRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IInventoryManager inventoryManager,
            IRepository<ItemType, long> itemTypeRepository,
            IRepository<Lot, long> lotRepository,
            IRepository<Tax, long> taxRepository,
            IUnitOfWorkManager unitOfWorkManager,
            AppFolders appFolders,
            ICorarlRepository<ItemLot, Guid> itemLotRepository,
            IFileStorageManager fileStorageManager,
            IFileUploadManager fileUploadManager,
            ICorarlRepository<ItemProperty, Guid> itemPropertyRepository,
            IRepository<PropertyValue, long> propertyValueRepository,
            IRepository<Property, long> propertyRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IItemUserGroupManager itemUserGroupManager,
            ICorarlRepository<ItemsUserGroup, Guid> itemUserGroupRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            ICorarlRepository<InventoryCalculationItem, Guid> inventoryCalculationItemRepository,
            IRepository<Tenant, int> tenantRepository,
            IFeatureChecker featureChecker,
         
            ICorarlRepository<ItemCodeFormulaItemType, long> itemCodeFormulaItemTypeRepository,
            ICorarlRepository<ItemCodeFormulaProperty, long> itemCodeFormulaPropertyRepository,
            ICorarlRepository<ItemCodeFormulaCustom, long> itemCodeFormulaCustomRepository,
            ICorarlRepository<Bom, Guid> bomRepository,
            ICorarlRepository<BomItem, Guid> bomItemRepository
        ) : base(null, appFolders, null, null)
        {
            _userGroupMemberRepository = userGroupMemberRepository;
            _appFolders = appFolders;
            _itemUserGroupRepository = itemUserGroupRepository;
            _itemUserGroupManager = itemUserGroupManager;
            _itemManager = itemManager;
            _itemRepository = itemRepository;         
            _fileStorageManager = fileStorageManager;
            _fileUploadManager = fileUploadManager;          
            _chartOfAccountRepository = chartOfAccountRepository;        
            _inventoryManager = inventoryManager;
            _taxRepository = taxRepository;
            _itemTypeRepository = itemTypeRepository;
            _itemPropertyRepository = itemPropertyRepository;
            _propertyValueRepository = propertyValueRepository;
            _propertyRepository = propertyRepository;
            _accountCycleRepository = accountCycleRepository;
            _itemIssueRepository = itemIssueRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemPriceItemRepository = IocManager.Instance.Resolve<IRepository<ItemPriceItem, Guid>>();
            _itemPriceRepository = IocManager.Instance.Resolve<IRepository<ItemPrice, Guid>>();
            _unitOfWorkManager = unitOfWorkManager;
            _itemLotRepository = itemLotRepository;
            _lotRepository = lotRepository;
            _batchNoFormulaRepository = batchNoFormulaRepository;
            _batchNoRepository = batchNoRepository;
            _tenantRepository = tenantRepository;
            _inventoryCalculationItemRepository = inventoryCalculationItemRepository;
            _featureChecker = featureChecker;       
            _itemCodeFormulaItemTypeRepository = itemCodeFormulaItemTypeRepository;
            _itemCodeFormulaCustomRepository = itemCodeFormulaCustomRepository;
            _itemCodeFormulaPropertyRepository = itemCodeFormulaPropertyRepository;
            _bomItemRepository = bomItemRepository;
            _bomRepository = bomRepository;
            _purchasePriceItemRepository = purchasePriceItemRepository;
        }
        #region Item       
        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Create)]
        public async Task<ItemGetListOutput> Create(CreateItemInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            await this.CheckMaxItemCountAsync(tenantId, 0);

            var GenerateInput = new GenerateItemCodeInput
            {
                ItemCode = input.ItemCode,
                TenantId = tenantId,
                ItemProperties = input.Properties,
                ItemTypeId = input.ItemTypeId,
            };
            input.ItemCode = await this.GetAutoItemCode(GenerateInput);

            if (input.ItemCode == null)
            {
                throw new UserFriendlyException(L("ItemCodeIsRequired"));
            }

            if (input.TrackSerial && input.UseBatchNo) throw new UserFriendlyException(L("CannotUseBoth", L("BatchNo"), L("Serial")));
            if (input.AutoBatchNo && !input.UseBatchNo) throw new UserFriendlyException(L("UseBatchNoMustTrueForAutoBatchNo"));
            if (input.AutoBatchNo && !input.BatchNoFormulaId.HasValue) throw new UserFriendlyException(L("AutoBatchNoMustHasBatchNoFormula"));
            if (!input.AutoBatchNo && input.BatchNoFormulaId.HasValue) throw new UserFriendlyException(L("AutoBatchNoMustTrueForBatchNoFormula"));

            var @entity = Item.Create(tenantId, userId, input.ItemName, input.ItemCode, input.SalePrice,
                input.PurchaseCost, input.ReorderPoint, input.TrackSerial, input.SaleCurrenyId,
                input.PurchaseCurrencyId, input.PurchaseTaxId, input.SaleTaxId, input.SaleAccountId, input.PurchaseAccountId,
                input.InventoryAccountId, input.ItemTypeId, input.Description, input.Barcode, input.UseBatchNo, input.AutoBatchNo, input.BatchNoFormulaId, input.TrackExpiration, input.BarcodeSameAsItemCode);
            @entity.UpdateMember(input.Member);
            entity.SetShowSubItems(input.ShowSubItems);
            entity.SetImage(input.ImageId);
            entity.SetMinMax(input.Min, input.Max);

            if (input.Properties != null)
            {
                foreach (var p in input.Properties)
                {
                    @entity.AddProperty(userId, p.PropertyId, p.PropertyValueId);
                }
            }


            #region item Group
            if (input.UserGroups != null && input.UserGroups.Count > 0)
            {
                foreach (var c in input.UserGroups)
                {
                    var itemGroup = ItemsUserGroup.Create(tenantId, userId, entity.Id, c.UserGroupId);
                    CheckErrors(await _itemUserGroupManager.CreateAsync(itemGroup));
                }
            }
            #endregion

            #region Subitem 
            

            if (input.DefaultLots != null && input.DefaultLots.Any())
            {
                //check Duplicate input
                var check = input.DefaultLots.GroupBy(s => s.Id).Where(s => s.Count() > 1).FirstOrDefault();
                if (check != null) throw new UserFriendlyException(L("Duplicated", L("Lot")));

                var lots = await _lotRepository.GetAll().AsNoTracking().ToListAsync();
                var checkUniqueByLocation = lots.Where(s => input.DefaultLots.Any(r => r.Id == s.Id))
                                                .GroupBy(s => s.LocationId)
                                                .Where(s => s.Count() > 1)
                                                .FirstOrDefault();
                if (checkUniqueByLocation != null) throw new UserFriendlyException(L("Duplicated", L("Location")));

                foreach (var lot in input.DefaultLots)
                {
                    var find = lots.FirstOrDefault(s => s.Id == lot.Id);
                    if (find == null) throw new UserFriendlyException(L("RecordNotFound"));

                    var defaultLot = ItemLot.Create(tenantId, userId, entity.Id, lot.Id);
                    await _itemLotRepository.InsertAsync(defaultLot);
                }
            }
            var itemType = await _itemTypeRepository.GetAll().AsNoTracking().Where(t => input.ItemTypeId == t.Id).Select(i => new ItemTypeDetailOutput
            {
                Id = i.Id,
                Name = i.Name,
                DisplayInventoryAccount = i.DisplayInventoryAccount,
                DisplayPurchase = i.DisplayPurchase,
                DisplayReorderPoint = i.DisplayReorderPoint,
                DisplaySale = i.DisplaySale,
                DisplaySubItem = i.DisplaySubItem,
                DisplayTrackSerialNumber = i.DisplayTrackSerialNumber
            }).FirstOrDefaultAsync();
            #endregion
            CheckErrors(await _itemManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            var result = (ObjectMapper.Map<ItemGetListOutput>(entity));
            result.ItemType = itemType;
            return result;
        }

        private async Task<string> GetAutoItemCode(GenerateItemCodeInput input)
        {
            var result = "";

            // Fetch tenant settings
            var tenant = await _tenantRepository.GetAll().AsNoTracking()
                .Where(t => t.Id == input.TenantId)
                .Select(t => new { AutoItemCode = t.ItemCodeSetting })
                .FirstOrDefaultAsync();

            // Return custom item code if formulaCustom is custom
            if (tenant.AutoItemCode == ItemCodeSetting.Custom) return input.ItemCode;

            // Fetch item code formula for the item type
            var formular = await _itemCodeFormulaItemTypeRepository.GetAll().Include(u => u.ItemCodeFormula).AsNoTracking()
                .Where(t => t.ItemTypeId == input.ItemTypeId)
                .Select(s => s)
                .FirstOrDefaultAsync();

            if (formular == null) throw new UserFriendlyException(L("CannotFindItemCodeFormular"));

            if (formular.ItemCodeFormula.UseManual) return input.ItemCode;
            
            if (formular.ItemCodeFormula.UseCustomGenerate && !formular.ItemCodeFormula.UseItemProperty)
            {
                var setting = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking()
                    .Where(s => s.ItemCodeFormulaId == formular.ItemCodeFormulaId)
                    .FirstOrDefaultAsync();
                var prefix = string.IsNullOrEmpty(setting.Prefix) ? "" : setting.Prefix;

                result = $"{prefix}{setting.ItemCode}";

                var latestCode = await _itemRepository.GetAll().AsNoTracking()
                    .Where(s => s.ItemCode.StartsWith(prefix) && s.ItemCode.Length == prefix.Length + setting.ItemCode.Length)
                    .Select(s => s.ItemCode)
                    .OrderByDescending(s => s)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrWhiteSpace(latestCode))
                {
                    var index = string.IsNullOrEmpty(prefix) ? latestCode : latestCode.Replace(prefix, "");
                    var number = Convert.ToInt32(index) + 1;
                    result = $"{prefix}{number.ToString().PadLeft(setting.ItemCode.Length, '0')}";
                }

                return result;
            }

            // If Auto item code formulaCustom and using item property
            else if (formular.ItemCodeFormula.UseItemProperty)
            {

                if (formular.ItemCodeFormulaId == 0) throw new UserFriendlyException(L("CannotFindItemCodeFormular"));

                var formularItems = await _itemCodeFormulaPropertyRepository.GetAll().AsNoTracking()
                    .Include(u => u.Property)
                    .Where(t => t.ItemCodeFormulaId == formular.ItemCodeFormulaId)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                var propertyValueIds = input.ItemProperties.Select(s => s.PropertyValueId).ToList();
                var propertyValues = await _propertyValueRepository.GetAll().AsNoTracking()
                    .Where(t => propertyValueIds.Contains(t.Id))
                    .Select(s => new { Id = s.Id, Code = s.Code, PropertyName = s.Property.Name, PropertyId = s.PropertyId })
                    .ToListAsync();

                var count = input.ItemProperties
                    .Count(s => s.PropertyValueId != null && formularItems.Select(d => d.PropertyId).Contains(s.PropertyId));

                if (formularItems.Count > count)
                {
                    var propertyValueNames = formularItems
                        .Where(s => !propertyValues.Select(d => d.PropertyId).Contains(s.PropertyId))
                        .ToList();
                    var pNames = string.Join(",", propertyValueNames.Select(s => s.Property.Name));
                    throw new UserFriendlyException(L("CannotGenerateItemCodeFormular") + $" {pNames}");
                }

                var min = formularItems.Min(s => s.SortOrder);
                var lst = from f in formularItems
                          join p in propertyValues on f.PropertyId equals p.PropertyId
                          orderby f.SortOrder
                          select new
                          {
                              OrginalCode = p.Code,
                              Code = f.SortOrder == min ? p.Code : f.Separator + p.Code,
                              Order = f.SortOrder,
                              Separator = f.SortOrder == min ? "" : f.Separator,
                          };

                var isEmptyCode = lst.Any(s => string.IsNullOrWhiteSpace(s.OrginalCode));
                if (isEmptyCode) throw new UserFriendlyException(L("PropertyValueCodeIsRequired"));

                result = string.Join("", lst.Select(s => s.Code));

                if (formular.ItemCodeFormula.UseCustomGenerate)
                {
                    var separator = string.Join("", lst.Select(s => s.Code));
                    var setting = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking()
                        .Where(s => s.ItemCodeFormulaId == formular.ItemCodeFormulaId)
                        .FirstOrDefaultAsync();

                    var prefix = string.IsNullOrEmpty(setting.Prefix) ? separator : separator + "" + setting.Prefix;
                    result = $"{prefix}{setting.ItemCode}";

                    var latestCode = await _itemRepository.GetAll().AsNoTracking()
                        .Where(s => s.ItemCode.StartsWith(prefix) && s.ItemCode.Length == prefix.Length + setting.ItemCode.Length)
                        .Select(s => s.ItemCode)
                        .OrderByDescending(s => s)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrWhiteSpace(latestCode))
                    {
                        var index = string.IsNullOrEmpty(prefix) ? latestCode : latestCode.Replace(prefix, "");
                        var number = Convert.ToInt32(index) + 1;
                        result = $"{prefix}{number.ToString().PadLeft(setting.ItemCode.Length, '0')}";
                    }
                }

                return result;
            }

            return null;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Delete)]
        [UnitOfWork(IsDisabled = true)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var tenantId = AbpSession.TenantId.Value;

            Guid? deleteImageId = null;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var @entity = await _itemManager.GetAsync(input.Id, true);

                    if (entity == null)
                    {
                        throw new UserFriendlyException(L("RecordNotFound"));
                    }
                    var itemLots = await _itemLotRepository.GetAll().AsNoTracking().Where(s => s.ItemId == input.Id).ToListAsync();
                    var calculateItems = await _inventoryCalculationItemRepository.GetAll().AsNoTracking().Where(s => s.ItemId == input.Id).ToListAsync();
                    if (itemLots.Any()) await _itemLotRepository.BulkDeleteAsync(itemLots);
                    if (calculateItems.Any()) await _inventoryCalculationItemRepository.BulkDeleteAsync(calculateItems);

                    if (entity.ImageId.HasValue) deleteImageId = entity.ImageId;

                    CheckErrors(await _itemManager.RemoveAsync(@entity));
                }

                await uow.CompleteAsync();
            }

            if (deleteImageId.HasValue) await _fileUploadManager.Delete(AbpSession.TenantId.Value, deleteImageId.Value);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _itemManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _itemManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _itemManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _itemManager.EnableAsync(@entity));
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<ListResultDto<SubitemGetListOutput>> GetSubitems(GetSubitemInput input)
        {


            // Need to Update this code
            var parentItem = await _bomRepository.GetAll().AsNoTracking().Where(s => s.ItemId == input.PernetId && s.IsDefault).Select(s=> new {Id = s.Id, ShowSubItem = s.Item.ShowSubItems }).FirstOrDefaultAsync();
            if (parentItem == null) throw new UserFriendlyException(L("RecordNotFound"));

            var subItems = _bomItemRepository.GetAll().AsNoTracking().Where(s => s.BomId == parentItem.Id);
            var @query = from s in subItems
                         join i in _itemRepository.GetAll()
                                    .Include(u => u.ItemType)
                                    .Include(u => u.PurchaseTax)
                                    .Include(u => u.InventoryAccount)
                                    .Include(u => u.PurchaseAccount)
                                    .Include(u => u.SaleAccount)
                                    .Include(u => u.SaleTax)
                                    .Where(s => s.IsActive)
                                    .AsNoTracking()
                         on s.ItemId equals i.Id
                         join l in _itemLotRepository.GetAll().Where(s => s.Lot.LocationId == input.LocationId).AsNoTracking()
                         on s.ItemId equals l.ItemId
                         into lots
                         from l in lots.DefaultIfEmpty()
                         select new SubitemGetListOutput
                         {
                             Id = i.Id,
                             InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.InventoryAccount),
                             InventoryAccountId = i.InventoryAccountId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(i.ItemType),
                             PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.PurchaseAccount),
                             PurchaseAccountId = i.PurchaseAccountId,
                             PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(i.PurchaseTax),
                             PurchaseTaxId = i.PurchaseTaxId,
                             SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.SaleAccount),
                             SaleAccountId = i.SaleAccountId,
                             SalePrice = i.SalePrice,
                             SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                             SaleTaxId = i.SaleTaxId,
                             Qty = s.Qty,
                             LotId = l == null ? (long?)null : l.LotId,
                             LotName = l == null ? "" : l.Lot.LotName
                         };

            if (!parentItem.ShowSubItem)
            {
                var itemNoLot = await query.FirstOrDefaultAsync(s => s.ItemType.DisplayInventoryAccount && !s.LotId.HasValue);
                if (itemNoLot != null) throw new UserFriendlyException(L("PleaseSetDefaultLotFor", $"{itemNoLot.ItemCode}-{itemNoLot.ItemName}"));
            }

            if (input.GetStockBalance)
            {
                var locations = new List<long?> { input.LocationId };
                var exceptDic = new Dictionary<Guid, Guid>();
                if (input.ExceptIds != null && input.ExceptIds.Any()) exceptDic = input.ExceptIds.ToDictionary(k => k.Value, v => v.Value);

                var balanceQuery = await _inventoryManager.GetItemsBalance("", input.Date, locations, exceptDic);

                var jQuery = from t in @query
                             join i in balanceQuery.Where(u => u.QtyOnHand > 0)
                             on $"{t.Id}-{t.LotId}" equals $"{i.Id}-{i.LotId}"
                             into bals
                             from b in bals.DefaultIfEmpty()

                             select new SubitemGetListOutput
                             {
                                 Id = t.Id,
                                 InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                                 InventoryAccountId = t.InventoryAccountId,
                                 ItemCode = t.ItemCode,
                                 ItemName = t.ItemName,
                                 ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                                 PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                                 PurchaseAccountId = t.PurchaseAccountId,
                                 PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                                 PurchaseTaxId = t.PurchaseTaxId,
                                 SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                                 SaleAccountId = t.SaleAccountId,
                                 SalePrice = t.SalePrice,
                                 SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                                 SaleTaxId = t.SaleTaxId,
                                 Qty = t.Qty,
                                 LotId = t.LotId,
                                 LotName = t.LotName,
                                 QtyOnHand = b == null ? 0 : b.QtyOnHand
                             };

                query = jQuery;

            }

            var @entities = await query.ToListAsync();

            if (input.GetStockBalance && !parentItem.ShowSubItem)
            {
                if (input.Qty <= 0) input.Qty = 1;
                var checkStock = entities.FirstOrDefault(s => s.ItemType.DisplayInventoryAccount && s.Qty * input.Qty > s.QtyOnHand);
                if (checkStock != null) throw new UserFriendlyException(L("ItemOutOfStock", $" {checkStock.ItemCode}-{checkStock.ItemName}"));
            }       
            return new ListResultDto<SubitemGetListOutput>(ObjectMapper.Map<List<SubitemGetListOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<PagedResultDto<ItemGetListOutput>> Find(GetItemListInputFind input)
        {
            input.FilterType = input.FilterType == 0 ? FilterType.Contain : input.FilterType;
            var userId = AbpSession.GetUserId();
            var popertyFilterList = input.PropertyDics;

            var batchItems = new List<Guid>();
            if (!input.Filter.IsNullOrWhiteSpace())
            {
                batchItems = await _batchNoRepository.GetAll()
                                   .Where(s => s.Code.ToLower().Contains(input.Filter.ToLower()))
                                   .Select(s => s.ItemId)
                                   .ToListAsync();
            }

            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                         .Include(s => s.UserGroup)
                                         .Include(s => s.Item)
                                         .Where(t => t.Item.Member == Member.UserGroup)
                                        .Where(x => x.UserGroup.LocationId != null)
                                        .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                                t => input.Locations.Contains(t.UserGroup.LocationId))
                                        .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                 .Where(s => s.MemberId == userId)
                                 .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var @itemQuery =
                       from i in _itemRepository.GetAll()
                       .Include(u => u.ItemType)
                       .Include(u => u.PurchaseTax)
                       .Include(u => u.InventoryAccount)
                       .Include(u => u.PurchaseAccount)
                       .Include(u => u.SaleAccount)
                       .Include(u => u.SaleTax)
                       .WhereIf(input.IsHasAccountId == true, u => u.InventoryAccountId != null)
                       .WhereIf(input.IsFilterService, u => u.ItemType.Name == Item_Service)
                       .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                       .WhereIf(input.IsHasPurchaseAccountId == true, p => p.PurchaseAccountId != null)
                       .WhereIf(input.IsHasSaleAccountId == true, p => p.SaleAccountId != null)
                       .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                       .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
                       .WhereIf(input.PurchaseAccount != null, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                       .WhereIf(input.SaleAccount != null, p => input.SaleAccount.Contains(p.SaleAccountId))
                       .WhereIf(input.InventoryAccount != null, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                       .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                    (input.FilterType == FilterType.Contain && (
                                       p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))) ||
                                       batchItems.Contains(p.Id)
                                    ) ||
                                    (input.FilterType == FilterType.StartWith && (
                                       p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                                       batchItems.Contains(p.Id)
                                    ) ||
                                    (input.FilterType == FilterType.Exact && (
                                       p.ItemName.ToLower().Equals(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Equals(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower()))) ||
                                       batchItems.Contains(p.Id)
                                    )
                                   )
                       //.WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                       //    p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                       //    p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                       //    (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower())) ||
                       //    batchItems.Contains(p.Id)
                       //)
                       .AsNoTracking()

                       join g in groupItemQuery on i.Id equals g.Id into ig
                       from itemGroup in ig.DefaultIfEmpty()
                       where i.Member == Member.All || itemGroup != null
                       select i;


            //TODO: Check this code for cost
            var averageCosts = new List<InventoryCostItem>();
            IQueryable<ItemGetListOutput> @query = null;

            if (input.IsFilterAverageCost && input.Date != null) //qty
            {
                var updateItemIssueIds = new Dictionary<Guid, Guid>();

                if (input.IsTransfer)
                {
                    if (input.UpdateItemIssueIds != null && input.UpdateItemIssueIds.Any())
                    {
                        var irQuery = from ii in _itemIssueRepository.GetAll().AsNoTracking()
                                                 .Where(s => input.UpdateItemIssueIds.Contains(s.TransferOrderId))
                                                 .Select(s => new { s.Id, s.TransferOrderId })
                                      join ir in _itemReceiptRepository.GetAll().AsNoTracking()
                                                 .Where(s => input.UpdateItemIssueIds.Contains(s.TransferOrderId))
                                                 .Select(s => new { s.Id, s.TransferOrderId })
                                      on ii.TransferOrderId equals ir.TransferOrderId
                                      select new { IssueId = ii.Id, ReceiptId = ir.Id };
                        var irList = await irQuery.ToListAsync();
                        if (irList.Any()) irList.ForEach(s =>
                        {
                            updateItemIssueIds.Add(s.IssueId, s.IssueId);
                            updateItemIssueIds.Add(s.ReceiptId, s.ReceiptId);
                        });
                    }
                }
                else if (input.IsProduction)
                {
                    if (input.UpdateItemIssueIds != null && input.UpdateItemIssueIds.Any())
                    {
                        var irQuery = from ii in _itemIssueRepository.GetAll().AsNoTracking()
                                                 .Where(s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId))
                                                 .Select(s => new { s.Id, s.ProductionOrderId })
                                      join ir in _itemReceiptRepository.GetAll().AsNoTracking()
                                                 .Where(s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId))
                                                 .Select(s => new { s.Id, s.ProductionOrderId })
                                      on ii.ProductionOrderId equals ir.ProductionOrderId
                                      select new { IssueId = ii.Id, ReceiptId = ir.Id };
                        var irList = await irQuery.ToListAsync();
                        if (irList.Any()) irList.ForEach(s =>
                        {
                            updateItemIssueIds.Add(s.IssueId, s.IssueId);
                            updateItemIssueIds.Add(s.ReceiptId, s.ReceiptId);
                        });
                    }

                }
                else
                {
                    updateItemIssueIds = input.UpdateItemIssueIds != null ? input.UpdateItemIssueIds.Distinct().ToDictionary(t => t.Value, t => t.Value) : new Dictionary<Guid, Guid>();
                }

                var balanceQuery = await _inventoryManager.GetItemsBalance(input.Filter, input.Date.Date, input.Locations, updateItemIssueIds);

                @query = (from i in balanceQuery
                                .Where(u => u.QtyOnHand > 0)
                          join t in @itemQuery
                          on i.Id equals t.Id
                          select new ItemGetListOutput
                          {
                              QtyOnHand = i.QtyOnHand,
                              LotId = i.LotId,
                              LotName = i.LotName,
                              Description = t.Description,
                              Id = t.Id,
                              InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                              InventoryAccountId = t.InventoryAccountId,
                              IsActive = t.IsActive,
                              ItemCode = t.ItemCode,
                              ItemName = t.ItemName,
                              ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                              PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                              PurchaseAccountId = t.PurchaseAccountId,
                              PurchaseCost = t.PurchaseCost,
                              PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                              PurchaseTaxId = t.PurchaseTaxId,
                              SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                              SaleAccountId = t.SaleAccountId,
                              SalePrice = t.SalePrice,
                              SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                              SaleTaxId = t.SaleTaxId,
                              Barcode = t.Barcode,
                              ImageId = t.ImageId,
                              UseBatchNo = t.UseBatchNo,
                              AutoBatchNo = t.AutoBatchNo,
                              TrackSerial = t.TrackSerial,
                              TrackExpiration = t.TrackExpiration,
                          });

                // take if input need to take service to output with item has stock ex: In Invoice need to show service with item
                if (input.IncludeService)
                {
                    var @itemService = @itemQuery.Where(u => !u.ItemType.DisplayInventoryAccount)
                                    .Select(t => new ItemGetListOutput
                                    {
                                        QtyOnHand = 0,
                                        LotId = null,
                                        LotName = "",
                                        Description = t.Description,
                                        Id = t.Id,
                                        InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                                        InventoryAccountId = t.InventoryAccountId,
                                        IsActive = t.IsActive,
                                        ItemCode = t.ItemCode,
                                        ItemName = t.ItemName,
                                        ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                                        PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                                        PurchaseAccountId = t.PurchaseAccountId,
                                        PurchaseCost = t.PurchaseCost,
                                        PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                                        PurchaseTaxId = t.PurchaseTaxId,
                                        SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                                        SaleAccountId = t.SaleAccountId,
                                        SalePrice = t.SalePrice,
                                        SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                                        SaleTaxId = t.SaleTaxId,
                                        ShowSubItems = t.ShowSubItems,
                                        Barcode = t.Barcode,
                                        ImageId = t.ImageId,
                                        UseBatchNo = t.UseBatchNo,
                                        AutoBatchNo = t.AutoBatchNo,
                                        TrackSerial = t.TrackSerial,
                                        TrackExpiration = t.TrackExpiration
                                    });
                    @query = @itemService.Concat(@query);
                }
            }
            else
            {
                var itemLots = from il in _itemLotRepository.GetAll()
                              .Include(u => u.Lot)
                              .WhereIf(input.Locations != null && input.Locations.Count > 0,
                               t => t.Lot != null && input.Locations.Contains(t.Lot.LocationId)).AsNoTracking()
                               select il;

                query = (from t in itemQuery
                         join il in itemLots on t.Id equals il.ItemId
                              into u
                         from d in u.DefaultIfEmpty()
                         select new ItemGetListOutput
                         {
                             QtyOnHand = 0,
                             LotId = d != null ? d.LotId : (long?)null,
                             LotName = d != null && d.Lot != null ? d.Lot.LotName : "",
                             Description = t.Description,
                             Id = t.Id,
                             InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                             InventoryAccountId = t.InventoryAccountId,
                             IsActive = t.IsActive,
                             ItemCode = t.ItemCode,
                             ItemName = t.ItemName,
                             ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                             PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                             PurchaseAccountId = t.PurchaseAccountId,
                             PurchaseCost = t.PurchaseCost,
                             PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                             PurchaseTaxId = t.PurchaseTaxId,
                             SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                             SaleAccountId = t.SaleAccountId,
                             SalePrice = t.SalePrice,
                             SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                             SaleTaxId = t.SaleTaxId,
                             ShowSubItems = t.ShowSubItems,
                             Barcode = t.Barcode,
                             ImageId = t.ImageId,
                             UseBatchNo = t.UseBatchNo,
                             AutoBatchNo = t.AutoBatchNo,
                             TrackExpiration = t.TrackExpiration,
                             TrackSerial = t.TrackSerial,
                             IsDefaultLot = d != null ? true : false
                         });
            }

            if (input.FilterType == FilterType.Exact && input.IsFilterAverageCost && !string.IsNullOrWhiteSpace(input.Filter))
            {


                query = (from i in query
                         join il in _itemLotRepository.GetAll()
                             .WhereIf(input.Locations != null && input.Locations.Count > 0,
                              t => input.Locations.Contains(t.Lot.LocationId)).AsNoTracking()
                         on i.Id equals il.ItemId
                         into u
                         //from d in u.DefaultIfEmpty()
                         select new ItemGetListOutput
                         {
                             QtyOnHand = i.QtyOnHand,
                             LotId = i.LotId,
                             LotName = i.LotName,
                             Description = i.Description,
                             Id = i.Id,
                             InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.InventoryAccount),
                             InventoryAccountId = i.InventoryAccountId,
                             IsActive = i.IsActive,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(i.ItemType),
                             PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.PurchaseAccount),
                             PurchaseAccountId = i.PurchaseAccountId,
                             PurchaseCost = i.PurchaseCost,
                             PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(i.PurchaseTax),
                             PurchaseTaxId = i.PurchaseTaxId,
                             SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.SaleAccount),
                             SaleAccountId = i.SaleAccountId,
                             SalePrice = i.SalePrice,
                             SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                             SaleTaxId = i.SaleTaxId,
                             Barcode = i.Barcode,
                             ImageId = i.ImageId,
                             UseBatchNo = i.UseBatchNo,
                             AutoBatchNo = i.AutoBatchNo,
                             TrackSerial = i.TrackSerial,
                             TrackExpiration = i.TrackExpiration,
                             IsDefaultLot = u.Where(t => t.LotId == i.LotId).Any()
                         });

            }

            var resultCount = await query.CountAsync();

            if (input.SelectedItemIds != null && input.SelectedItemIds.Any())
            {
                query = query.OrderBy(s => input.SelectedItemIds.Contains(s.Id) ? 0 : 1).ThenBy(input.Sorting);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var @entities = await query.PageBy(input).ToListAsync();

            if(input.IsPurchasePrice && input.CurrencyId.HasValue && !input.Locations.IsNullOrEmpty())
            {
                var itemIds = entities.Select(s => s.Id).ToList();

                var purchaePriceDic = await _purchasePriceItemRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(s => !s.PurchasePrice.SpecificVendor || (s.VendorId.HasValue && s.VendorId == input.VendorId))
                                            .Where(s => input.Locations.Contains(s.PurchasePrice.LocationId))
                                            .Where(s => s.CurrencyId == input.CurrencyId.Value)
                                            .Where(s => itemIds.Contains(s.ItemId))
                                            .ToDictionaryAsync(k => k.ItemId, v => v.Price);

                entities = entities.Select(s =>
                {
                    if(purchaePriceDic.ContainsKey(s.Id)) s.PurchaseCost = purchaePriceDic[s.Id];
                   
                    return s;
                }).ToList();
            }

            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<PagedResultDto<ItemGetListOutput>> FindBarcode(GetItemListInputFind input)
        {
            input.FilterType = input.FilterType == 0 ? FilterType.Contain : input.FilterType;
            var userId = AbpSession.GetUserId();
            var popertyFilterList = input.PropertyDics;

            var batchItems = new List<Guid>();
            if (!input.Filter.IsNullOrWhiteSpace())
            {
                batchItems = await _batchNoRepository.GetAll()
                                   .Where(s => s.Code.ToLower().Contains(input.Filter.ToLower()))
                                   .Select(s => s.ItemId)
                                   .ToListAsync();
            }
            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                         .Include(s => s.UserGroup)
                                         .Include(s => s.Item)
                                         .Where(t => t.Item.Member == Member.UserGroup)
                                        .Where(x => x.UserGroup.LocationId != null)
                                        .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                                t => input.Locations.Contains(t.UserGroup.LocationId))
                                        .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                 .Where(s => s.MemberId == userId)
                                 .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var @itemQuery =
                       from i in _itemRepository.GetAll()
                       .Where(t => !string.IsNullOrWhiteSpace(t.Barcode))
                       .WhereIf(input.IsFilterService, u => u.ItemType.Name == Item_Service)
                       .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                       .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value))).Count() == popertyFilterList.Count))
                       .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                    (input.FilterType == FilterType.Contain && (
                                       p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))) ||
                                       batchItems.Contains(p.Id)
                                    ) ||
                                    (input.FilterType == FilterType.StartWith && (
                                       p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                                       batchItems.Contains(p.Id)
                                    ) ||
                                    (input.FilterType == FilterType.Exact && (
                                       p.ItemName.ToLower().Equals(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Equals(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower()))) ||
                                       batchItems.Contains(p.Id)
                                    )
                                   )
                       .AsNoTracking()

                       join g in groupItemQuery on i.Id equals g.Id into ig
                       from itemGroup in ig.DefaultIfEmpty()
                       where i.Member == Member.All || itemGroup != null
                       select i;

            var query = (from t in itemQuery
                         select new ItemGetListOutput
                         {

                             Id = t.Id,
                             IsActive = t.IsActive,
                             ItemCode = t.ItemCode,
                             ItemName = t.ItemName,
                             Barcode = t.Barcode,
                             ImageId = t.ImageId,

                         });

            var resultCount = await query.CountAsync();
            query = query.OrderBy(input.Sorting);
            var @entities = await query.PageBy(input).ToListAsync();
            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<PagedResultDto<ItemGetListOutput>> FindOld(GetItemListInputFind input)
        {
            var userId = AbpSession.GetUserId();
            var popertyFilterList = input.PropertyDics;

            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                         .Include(s => s.UserGroup)
                                         .Include(s => s.Item)
                                         .Where(t => t.Item.Member == Member.UserGroup)
                                        .Where(x => x.UserGroup.LocationId != null)
                                        .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                                t => input.Locations.Contains(t.UserGroup.LocationId))
                                        .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                 .Where(s => s.MemberId == userId)
                                 .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var @itemQuery =
                       from i in _itemRepository.GetAll()
                       .Include(u => u.ItemType)
                       .Include(u => u.PurchaseTax)
                       .Include(u => u.InventoryAccount)
                       .Include(u => u.PurchaseAccount)
                       .Include(u => u.SaleAccount)
                       .Include(u => u.SaleTax)
                       .WhereIf(input.IsHasAccountId == true, u => u.InventoryAccountId != null)
                       .WhereIf(input.IsFilterService, u => u.ItemType.Name == Item_Service)
                       .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                       .WhereIf(input.IsHasPurchaseAccountId == true, p => p.PurchaseAccountId != null)
                       .WhereIf(input.IsHasSaleAccountId == true, p => p.SaleAccountId != null)
                       .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                       .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
                       .WhereIf(input.PurchaseAccount != null, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                       .WhereIf(input.SaleAccount != null, p => input.SaleAccount.Contains(p.SaleAccountId))
                       .WhereIf(input.InventoryAccount != null, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                       .WhereIf(
                         !input.Filter.IsNullOrEmpty(),
                         p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                              p.ItemCode.ToLower().Contains(input.Filter.ToLower())
                       ).AsNoTracking()

                       join g in groupItemQuery on i.Id equals g.Id into ig
                       from itemGroup in ig.DefaultIfEmpty()
                       where i.Member == Member.All || itemGroup != null
                       select i;


            //TODO: Check this code for cost
            var averageCosts = new List<InventoryCostItem>();
            IQueryable<ItemGetListOutput> @query = null;

            if (input.IsFilterAverageCost && input.Date != null) //qty
            {
                var updateItemIssueIds = new Dictionary<Guid, Guid>();

                if (input.IsTransfer)
                {
                    var itemIssueIds = await _itemIssueRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.TransferOrderId)).Select(s => s.Id).Distinct().ToListAsync();
                    var itemReceiptIds = await _itemReceiptRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.TransferOrderId)).Select(s => s.Id).Distinct().ToListAsync();

                    if (itemIssueIds.Any())
                    {
                        itemIssueIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                    if (itemReceiptIds.Any())
                    {
                        itemReceiptIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                }
                else if (input.IsProduction)
                {
                    var itemIssueIds = await _itemIssueRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId)).Select(s => s.Id).Distinct().ToListAsync();
                    var itemReceiptIds = await _itemReceiptRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId)).Select(s => s.Id).Distinct().ToListAsync();

                    if (itemIssueIds.Any())
                    {
                        itemIssueIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                    if (itemReceiptIds.Any())
                    {
                        itemReceiptIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                }
                else
                {
                    updateItemIssueIds = input.UpdateItemIssueIds != null ? input.UpdateItemIssueIds.Distinct().ToDictionary(t => t.Value, t => t.Value) : new Dictionary<Guid, Guid>();
                }

                var balanceQuery = await _inventoryManager.GetItemsBalance(input.Filter, input.Date.Date, input.Locations, updateItemIssueIds);

                @query = (from i in balanceQuery
                                .Where(u => u.QtyOnHand > 0)
                          join t in @itemQuery
                          on i.Id equals t.Id
                          select new ItemGetListOutput
                          {
                              QtyOnHand = i.QtyOnHand,
                              LotId = i.LotId,
                              LotName = i.LotName,
                              Description = t.Description,
                              Id = t.Id,
                              InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                              InventoryAccountId = t.InventoryAccountId,
                              IsActive = t.IsActive,
                              ItemCode = t.ItemCode,
                              ItemName = t.ItemName,
                              ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                              PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                              PurchaseAccountId = t.PurchaseAccountId,
                              PurchaseCost = t.PurchaseCost,
                              PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                              PurchaseTaxId = t.PurchaseTaxId,
                              SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                              SaleAccountId = t.SaleAccountId,
                              SalePrice = t.SalePrice,
                              SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                              SaleTaxId = t.SaleTaxId,
                          });

                // take if input need to take service to output with item has stock ex: In Invoice need to show service with item
                if (input.IncludeService)
                {
                    var @itemService = @itemQuery.Where(u => u.ItemType.Name == Item_Service)
                                    .Select(t => new ItemGetListOutput
                                    {

                                        QtyOnHand = 0,
                                        LotId = null,
                                        LotName = "",
                                        Description = t.Description,
                                        Id = t.Id,
                                        InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                                        InventoryAccountId = t.InventoryAccountId,
                                        IsActive = t.IsActive,
                                        ItemCode = t.ItemCode,
                                        ItemName = t.ItemName,
                                        ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                                        PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                                        PurchaseAccountId = t.PurchaseAccountId,
                                        PurchaseCost = t.PurchaseCost,
                                        PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                                        PurchaseTaxId = t.PurchaseTaxId,
                                        SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                                        SaleAccountId = t.SaleAccountId,
                                        SalePrice = t.SalePrice,
                                        SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                                        SaleTaxId = t.SaleTaxId
                                    });
                    @query = @itemService.Concat(@query);
                }
            }
            else
            {
                @query = @itemQuery.Select(t => new ItemGetListOutput
                {
                    //AverageCost = averageCosts.Where(v => v.Id == t.Id).Select(i => i.AverageCost).FirstOrDefault(),
                    QtyOnHand = 0,
                    LotId = null,
                    LotName = "",
                    Description = t.Description,
                    Id = t.Id,
                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                    InventoryAccountId = t.InventoryAccountId,
                    IsActive = t.IsActive,
                    ItemCode = t.ItemCode,
                    ItemName = t.ItemName,
                    ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                    PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                    PurchaseAccountId = t.PurchaseAccountId,
                    PurchaseCost = t.PurchaseCost,
                    PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                    PurchaseTaxId = t.PurchaseTaxId,
                    SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                    SaleAccountId = t.SaleAccountId,
                    SalePrice = t.SalePrice,
                    SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                    SaleTaxId = t.SaleTaxId,
                });
            }

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<PagedResultDto<ItemGetListOutput>> POSFind(GetItemListInputPOSFind input)
        {
            var userId = AbpSession.GetUserId();
            //var popertyFilterList = input.PropertyDics;

            var batchItems = new List<Guid>();
            if (!input.Filter.IsNullOrWhiteSpace())
            {
                batchItems = await _batchNoRepository.GetAll()
                                   .Where(s => s.Code.ToLower().Contains(input.Filter.ToLower()))
                                   .Select(s => s.ItemId)
                                   .ToListAsync();
            }

            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                         .Where(t => t.Item.Member == Member.UserGroup)
                                         .Where(x => x.UserGroup.LocationId != null)
                                         .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                                t => input.Locations.Contains(t.UserGroup.LocationId))
                                         .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                         .Where(s => s.MemberId == userId)
                                         .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var @itemQuery = from i in _itemRepository.GetAll()
                                   .WhereIf(input.IsHasAccountId == true, u => u.InventoryAccountId != null)
                                   .WhereIf(input.IsFilterService, u => u.ItemType.Name == Item_Service)
                                   .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                   .WhereIf(input.IsHasPurchaseAccountId == true, p => p.PurchaseAccountId != null)
                                   .WhereIf(input.IsHasSaleAccountId == true, p => p.SaleAccountId != null)
                                   .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                                   .WhereIf(input.PropertyValueIds != null && input.PropertyValueIds.Any(), s => s.Properties.Any(v => input.PropertyValueIds.Contains(v.PropertyValueId)))
                                   .WhereIf(input.PurchaseAccount != null, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                                   .WhereIf(input.SaleAccount != null, p => input.SaleAccount.Contains(p.SaleAccountId))
                                   .WhereIf(input.InventoryAccount != null, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                                   .WhereIf(
                                     !input.Filter.IsNullOrEmpty(), p =>
                                     (input.FilterType == FilterType.Contain && (
                                        p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                        p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                        (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))) ||
                                        batchItems.Contains(p.Id)
                                     ) ||
                                     (input.FilterType == FilterType.StartWith && (
                                        p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                        p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) ||
                                        (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                                        batchItems.Contains(p.Id)
                                     ) ||
                                     (input.FilterType == FilterType.Exact && (
                                        p.ItemName.ToLower().Equals(input.Filter.ToLower()) ||
                                        p.ItemCode.ToLower().Equals(input.Filter.ToLower()) ||
                                        (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower()))) ||
                                        batchItems.Contains(p.Id)
                                     )
                                   )
                                   .AsNoTracking()

                             join g in groupItemQuery on i.Id equals g.Id into ig
                             from itemGroup in ig.DefaultIfEmpty()
                             where i.Member == Member.All || itemGroup != null
                             select i;


            //TODO: Check this code for cost
            var averageCosts = new List<InventoryCostItem>();
            IQueryable<ItemGetListOutput> @query = null;


            var updateItemIssueIds = input.UpdateItemIssueIds != null ?
                                 input.UpdateItemIssueIds.Distinct().ToDictionary(t => t.Value, t => t.Value) :
                                 new Dictionary<Guid, Guid>();

            var balanceQuery = await _inventoryManager.GetItemsBalance(input.Filter, input.Date.Date, input.Locations, updateItemIssueIds, input.FilterType);

            var tenant = await GetCurrentTenantAsync();
            var defaultItemPrice = await _itemPriceRepository.GetAll()
                                    .Where(s => s.TransactionTypeSale != null && s.TransactionTypeSale.IsPOS)
                                    .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId))
                                    .WhereIf(input.CustomerTypes != null && input.CustomerTypes.Any(), s => input.CustomerTypes.Contains(s.CustomerTypeId))
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
            @query = (from i in balanceQuery
                            .Where(u => u.QtyOnHand > 0)
                      join t in @itemQuery
                      on i.Id equals t.Id

                      join ip in _itemPriceItemRepository.GetAll()
                                  .Where(s => defaultItemPrice != null && s.ItemPriceId == defaultItemPrice.Id)
                                  .Where(s => s.ItemPrice.TransactionTypeSale != null && s.ItemPrice.TransactionTypeSale.IsPOS)
                                  .Where(s => s.CurrencyId == tenant.POSCurrencyId)
                                  .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemPrice.LocationId))
                                  .AsNoTracking()
                      on i.Id equals ip.ItemId into itemPrices
                      from itemPrice in itemPrices.DefaultIfEmpty()

                      select new ItemGetListOutput
                      {
                          QtyOnHand = i.QtyOnHand,
                          LotId = i.LotId,
                          LotName = i.LotName,
                          Description = t.Description,
                          Id = t.Id,
                          InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                          InventoryAccountId = t.InventoryAccountId,
                          IsActive = t.IsActive,
                          ItemCode = t.ItemCode,
                          ItemName = t.ItemName,
                          ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                          PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                          PurchaseAccountId = t.PurchaseAccountId,
                          PurchaseCost = t.PurchaseCost,
                          PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                          PurchaseTaxId = t.PurchaseTaxId,
                          SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                          SaleAccountId = t.SaleAccountId,
                          SalePrice = itemPrice == null ? 0 : itemPrice.Price,
                          SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                          SaleTaxId = t.SaleTaxId,
                          Barcode = t.Barcode,
                          ImageId = t.ImageId,
                          UseBatchNo = t.UseBatchNo,
                          AutoBatchNo = t.AutoBatchNo,
                          TrackSerial = t.TrackSerial,
                          TrackExpiration = t.TrackExpiration
                      });

            // take if input need to take service to output with item has stock ex: In Invoice need to show service with item
            if (input.IncludeService)
            {
                var @itemService = from i in @itemQuery.Where(u => u.ItemType.Name == Item_Service)
                                                     .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                                         (input.FilterType == FilterType.Contain && (p.ItemName.ToLower().Contains(input.Filter.ToLower()) || p.ItemCode.ToLower().Contains(input.Filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower())))) ||
                                                         (input.FilterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) || p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower())))) ||
                                                         (input.FilterType == FilterType.Exact && (p.ItemName.ToLower().Equals(input.Filter.ToLower()) || p.ItemCode.ToLower().Equals(input.Filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower()))))
                                                     )
                                                    .AsNoTracking()
                                   join ip in _itemPriceItemRepository.GetAll()
                                               .Where(s => s.ItemPrice.TransactionTypeSale.IsPOS)
                                               .Where(s => s.CurrencyId == tenant.POSCurrencyId)
                                               .WhereIf(defaultItemPrice != null, s => s.ItemPriceId == defaultItemPrice.Id)
                                               .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemPrice.LocationId))
                                               .AsNoTracking()
                                   on i.Id equals ip.ItemId into itemPrices
                                   from itemPrice in itemPrices.DefaultIfEmpty()
                                   select new ItemGetListOutput
                                   {

                                       QtyOnHand = 0,
                                       LotId = null,
                                       LotName = "",
                                       Description = i.Description,
                                       Id = i.Id,
                                       InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.InventoryAccount),
                                       InventoryAccountId = i.InventoryAccountId,
                                       IsActive = i.IsActive,
                                       ItemCode = i.ItemCode,
                                       ItemName = i.ItemName,
                                       ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(i.ItemType),
                                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.PurchaseAccount),
                                       PurchaseAccountId = i.PurchaseAccountId,
                                       PurchaseCost = i.PurchaseCost,
                                       PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(i.PurchaseTax),
                                       PurchaseTaxId = i.PurchaseTaxId,
                                       SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.SaleAccount),
                                       SaleAccountId = i.SaleAccountId,
                                       SalePrice = itemPrice == null ? 0 : itemPrice.Price,
                                       SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                                       SaleTaxId = i.SaleTaxId,
                                       Barcode = i.Barcode,
                                       ImageId = i.ImageId,
                                       UseBatchNo = i.UseBatchNo,
                                       AutoBatchNo = i.AutoBatchNo,
                                       TrackSerial = i.TrackSerial,
                                       TrackExpiration = i.TrackExpiration
                                   };
                @query = @itemService.Concat(@query);
            }


            var resultCount = await query.CountAsync();
            var @entities = await query.PageBy(input).ToListAsync();
            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<PagedResultDto<ItemGetListOutput>> POSFindOld(GetItemListInputPOSFind input)
        {
            var userId = AbpSession.GetUserId();
            //var popertyFilterList = input.PropertyDics;

            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                         .Include(s => s.UserGroup)
                                         .Include(s => s.Item)
                                         .Where(t => t.Item.Member == Member.UserGroup)
                                         .Where(x => x.UserGroup.LocationId != null)
                                         .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                                t => input.Locations.Contains(t.UserGroup.LocationId))
                                         .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                 .Where(s => s.MemberId == userId)
                                 .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var @itemQuery =
                       from i in _itemRepository.GetAll()
                       .Include(u => u.ItemType)
                       .Include(u => u.PurchaseTax)
                       .Include(u => u.InventoryAccount)
                       .Include(u => u.PurchaseAccount)
                       .Include(u => u.SaleAccount)
                       .Include(u => u.SaleTax)
                       .WhereIf(input.IsHasAccountId == true, u => u.InventoryAccountId != null)
                       .WhereIf(input.IsFilterService, u => u.ItemType.Name == Item_Service)
                       .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                       .WhereIf(input.IsHasPurchaseAccountId == true, p => p.PurchaseAccountId != null)
                       .WhereIf(input.IsHasSaleAccountId == true, p => p.SaleAccountId != null)
                       .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                       .WhereIf(input.PropertyValueIds != null && input.PropertyValueIds.Any(), s => s.Properties.Any(v => input.PropertyValueIds.Contains(v.PropertyValueId)))
                       .WhereIf(input.PurchaseAccount != null, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                       .WhereIf(input.SaleAccount != null, p => input.SaleAccount.Contains(p.SaleAccountId))
                       .WhereIf(input.InventoryAccount != null, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                       .WhereIf(
                         !input.Filter.IsNullOrEmpty(), p =>
                         (input.FilterType == FilterType.Contain && (p.ItemName.ToLower().Contains(input.Filter.ToLower()) || p.ItemCode.ToLower().Contains(input.Filter.ToLower()))) ||
                         (input.FilterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) || p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                         (input.FilterType == FilterType.Exact && (p.ItemName.ToLower().Equals(input.Filter.ToLower()) || p.ItemCode.ToLower().Equals(input.Filter.ToLower())))
                       ).AsNoTracking()

                       join g in groupItemQuery on i.Id equals g.Id into ig
                       from itemGroup in ig.DefaultIfEmpty()
                       where i.Member == Member.All || itemGroup != null
                       select i;


            //TODO: Check this code for cost
            var averageCosts = new List<InventoryCostItem>();
            IQueryable<ItemGetListOutput> @query = null;


            var updateItemIssueIds = new Dictionary<Guid, Guid>();

            if (input.IsTransfer)
            {
                var itemIssueIds = await _itemIssueRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.TransferOrderId)).Select(s => s.Id).Distinct().ToListAsync();
                var itemReceiptIds = await _itemReceiptRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.TransferOrderId)).Select(s => s.Id).Distinct().ToListAsync();

                if (itemIssueIds.Any())
                {
                    itemIssueIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                }

                if (itemReceiptIds.Any())
                {
                    itemReceiptIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                }

            }
            else if (input.IsProduction)
            {
                var itemIssueIds = await _itemIssueRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId)).Select(s => s.Id).Distinct().ToListAsync();
                var itemReceiptIds = await _itemReceiptRepository.GetAll().AsNoTracking().WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId)).Select(s => s.Id).Distinct().ToListAsync();

                if (itemIssueIds.Any())
                {
                    itemIssueIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                }

                if (itemReceiptIds.Any())
                {
                    itemReceiptIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                }

            }
            else
            {
                updateItemIssueIds = input.UpdateItemIssueIds != null ? input.UpdateItemIssueIds.Distinct().ToDictionary(t => t.Value, t => t.Value) : new Dictionary<Guid, Guid>();
            }

            var balanceQuery = await _inventoryManager.GetItemsBalance(input.Filter, input.Date.Date, input.Locations, updateItemIssueIds, input.FilterType);


            var tenant = await GetCurrentTenantAsync();
            var defaultItemPrice = await _itemPriceRepository.GetAll().Include(s => s.TransactionTypeSale)
                                    .Where(s => s.TransactionTypeSale.IsPOS)
                                    .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId))
                                    .WhereIf(input.CustomerTypes != null && input.CustomerTypes.Any(), s => input.CustomerTypes.Contains(s.CustomerTypeId))
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
            @query = (from i in balanceQuery
                            .Where(u => u.QtyOnHand > 0)
                      join t in @itemQuery
                      on i.Id equals t.Id

                      join ip in _itemPriceItemRepository.GetAll()
                      .Include(s => s.ItemPrice.TransactionTypeSale)
                      .Where(s => s.ItemPrice.TransactionTypeSale.IsPOS)
                      .Where(s => s.CurrencyId == tenant.POSCurrencyId)
                      .Where(s => defaultItemPrice != null && s.ItemPriceId == defaultItemPrice.Id)
                      .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemPrice.LocationId))
                      .AsNoTracking()
                      on i.Id equals ip.ItemId into itemPrices
                      from itemPrice in itemPrices.DefaultIfEmpty()

                      select new ItemGetListOutput
                      {
                          QtyOnHand = i.QtyOnHand,
                          LotId = i.LotId,
                          LotName = i.LotName,
                          Description = t.Description,
                          Id = t.Id,
                          InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.InventoryAccount),
                          InventoryAccountId = t.InventoryAccountId,
                          IsActive = t.IsActive,
                          ItemCode = t.ItemCode,
                          ItemName = t.ItemName,
                          ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(t.ItemType),
                          PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.PurchaseAccount),
                          PurchaseAccountId = t.PurchaseAccountId,
                          PurchaseCost = t.PurchaseCost,
                          PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(t.PurchaseTax),
                          PurchaseTaxId = t.PurchaseTaxId,
                          SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.SaleAccount),
                          SaleAccountId = t.SaleAccountId,
                          //SalePrice = t.SalePrice,
                          SalePrice = itemPrice == null ? 0 : itemPrice.Price,
                          SaleTax = ObjectMapper.Map<TaxDetailOutput>(t.SaleTax),
                          SaleTaxId = t.SaleTaxId,
                      });

            // take if input need to take service to output with item has stock ex: In Invoice need to show service with item
            if (input.IncludeService)
            {
                var @itemService = from i in @itemQuery.Where(u => u.ItemType.Name == Item_Service)
                                .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                     (input.FilterType == FilterType.Contain && (p.ItemName.ToLower().Contains(input.Filter.ToLower()) || p.ItemCode.ToLower().Contains(input.Filter.ToLower()))) ||
                                     (input.FilterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) || p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                                     (input.FilterType == FilterType.Exact && (p.ItemName.ToLower().Equals(input.Filter.ToLower()) || p.ItemCode.ToLower().Equals(input.Filter.ToLower())))
                                 )
                                .AsNoTracking()
                                   join ip in _itemPriceItemRepository.GetAll()
                                   .Include(s => s.ItemPrice.TransactionTypeSale)
                                   .Where(s => s.ItemPrice.TransactionTypeSale.IsPOS)
                                   .Where(s => s.CurrencyId == tenant.POSCurrencyId)
                                   .WhereIf(defaultItemPrice != null, s => s.ItemPriceId == defaultItemPrice.Id)
                                   .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemPrice.LocationId))
                                   .AsNoTracking()
                                   on i.Id equals ip.ItemId into itemPrices
                                   from itemPrice in itemPrices.DefaultIfEmpty()
                                   select new ItemGetListOutput
                                   {

                                       QtyOnHand = 0,
                                       LotId = null,
                                       LotName = "",
                                       Description = i.Description,
                                       Id = i.Id,
                                       InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.InventoryAccount),
                                       InventoryAccountId = i.InventoryAccountId,
                                       IsActive = i.IsActive,
                                       ItemCode = i.ItemCode,
                                       ItemName = i.ItemName,
                                       ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(i.ItemType),
                                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.PurchaseAccount),
                                       PurchaseAccountId = i.PurchaseAccountId,
                                       PurchaseCost = i.PurchaseCost,
                                       PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(i.PurchaseTax),
                                       PurchaseTaxId = i.PurchaseTaxId,
                                       SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.SaleAccount),
                                       SaleAccountId = i.SaleAccountId,
                                       //SalePrice = i.SalePrice,
                                       SalePrice = itemPrice == null ? 0 : itemPrice.Price,
                                       SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                                       SaleTaxId = i.SaleTaxId
                                   };
                @query = @itemService.Concat(@query);
            }


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        public async Task<PagedResultDto<ItemGetListOutput>> SaleReturnFind(GetItemListInputPOSFind input)
        {
            var userId = AbpSession.GetUserId();
            //var popertyFilterList = input.PropertyDics;

            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                         .Include(s => s.UserGroup)
                                         .Include(s => s.Item)
                                         .Where(t => t.Item.Member == Member.UserGroup)
                                         .Where(x => x.UserGroup.LocationId != null)
                                         .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                                t => input.Locations.Contains(t.UserGroup.LocationId))
                                         .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                 .Where(s => s.MemberId == userId)
                                 .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var @itemQuery =
                       from i in _itemRepository.GetAll()
                       .Include(u => u.ItemType)
                       .Include(u => u.PurchaseTax)
                       .Include(u => u.InventoryAccount)
                       .Include(u => u.PurchaseAccount)
                       .Include(u => u.SaleAccount)
                       .Include(u => u.SaleTax)
                       .WhereIf(input.IsHasAccountId == true, u => u.InventoryAccountId != null)
                       .WhereIf(input.IsFilterService, u => u.ItemType.Name == Item_Service)
                       .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                       .WhereIf(input.IsHasPurchaseAccountId == true, p => p.PurchaseAccountId != null)
                       .WhereIf(input.IsHasSaleAccountId == true, p => p.SaleAccountId != null)
                       .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                       .WhereIf(input.PropertyValueIds != null && input.PropertyValueIds.Any(), s => s.Properties.Any(v => input.PropertyValueIds.Contains(v.PropertyValueId)))
                       .WhereIf(input.PurchaseAccount != null, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                       .WhereIf(input.SaleAccount != null, p => input.SaleAccount.Contains(p.SaleAccountId))
                       .WhereIf(input.InventoryAccount != null, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                       .WhereIf(
                         !input.Filter.IsNullOrEmpty(), p =>
                         (input.FilterType == FilterType.Contain && (p.ItemName.ToLower().Contains(input.Filter.ToLower()) || p.ItemCode.ToLower().Contains(input.Filter.ToLower()))) ||
                         (input.FilterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) || p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                         (input.FilterType == FilterType.Exact && (p.ItemName.ToLower().Equals(input.Filter.ToLower()) || p.ItemCode.ToLower().Equals(input.Filter.ToLower())))
                       ).AsNoTracking()

                       join g in groupItemQuery on i.Id equals g.Id into ig
                       from itemGroup in ig.DefaultIfEmpty()
                       where i.Member == Member.All || itemGroup != null
                       select i;


            //TODO: Check this code for cost

            IQueryable<ItemGetListOutput> @query = null;


            var tenant = await GetCurrentTenantAsync();
            var defaultItemPrice = await _itemPriceRepository.GetAll().Include(s => s.TransactionTypeSale)
                                    .Where(s => s.TransactionTypeSale.IsPOS)
                                    .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId))
                                    .WhereIf(input.CustomerTypes != null && input.CustomerTypes.Any(), s => input.CustomerTypes.Contains(s.CustomerTypeId))
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
            @query = (from i in @itemQuery

                      join ip in _itemPriceItemRepository.GetAll()
                      .Include(s => s.ItemPrice.TransactionTypeSale)
                      .Where(s => s.ItemPrice.TransactionTypeSale.IsPOS)
                      .Where(s => s.CurrencyId == tenant.POSCurrencyId)
                      .Where(s => defaultItemPrice != null && s.ItemPriceId == defaultItemPrice.Id)
                      .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemPrice.LocationId))
                      .AsNoTracking()
                      on i.Id equals ip.ItemId into itemPrices
                      from itemPrice in itemPrices.DefaultIfEmpty()

                      select new ItemGetListOutput
                      {

                          Description = i.Description,
                          Id = i.Id,
                          InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.InventoryAccount),
                          InventoryAccountId = i.InventoryAccountId,
                          IsActive = i.IsActive,
                          ItemCode = i.ItemCode,
                          ItemName = i.ItemName,
                          ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(i.ItemType),
                          PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.PurchaseAccount),
                          PurchaseAccountId = i.PurchaseAccountId,
                          PurchaseCost = i.PurchaseCost,
                          PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(i.PurchaseTax),
                          PurchaseTaxId = i.PurchaseTaxId,
                          SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.SaleAccount),
                          SaleAccountId = i.SaleAccountId,
                          //SalePrice = t.SalePrice,
                          SalePrice = itemPrice == null ? 0 : itemPrice.Price,
                          SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                          SaleTaxId = i.SaleTaxId,
                      });

            // take if input need to take service to output with item has stock ex: In Invoice need to show service with item
            if (input.IncludeService)
            {
                var @itemService = from i in @itemQuery.Where(u => u.ItemType.Name == Item_Service)
                                .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                     (input.FilterType == FilterType.Contain && (p.ItemName.ToLower().Contains(input.Filter.ToLower()) || p.ItemCode.ToLower().Contains(input.Filter.ToLower()))) ||
                                     (input.FilterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) || p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()))) ||
                                     (input.FilterType == FilterType.Exact && (p.ItemName.ToLower().Equals(input.Filter.ToLower()) || p.ItemCode.ToLower().Equals(input.Filter.ToLower())))
                                 )
                                .AsNoTracking()
                                   join ip in _itemPriceItemRepository.GetAll()
                                   .Include(s => s.ItemPrice.TransactionTypeSale)
                                   .Where(s => s.ItemPrice.TransactionTypeSale.IsPOS)
                                   .Where(s => s.CurrencyId == tenant.POSCurrencyId)
                                   .WhereIf(defaultItemPrice != null, s => s.ItemPriceId == defaultItemPrice.Id)
                                   .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.ItemPrice.LocationId))
                                   .AsNoTracking()
                                   on i.Id equals ip.ItemId into itemPrices
                                   from itemPrice in itemPrices.DefaultIfEmpty()
                                   select new ItemGetListOutput
                                   {

                                       QtyOnHand = 0,
                                       LotId = null,
                                       LotName = "",
                                       Description = i.Description,
                                       Id = i.Id,
                                       InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.InventoryAccount),
                                       InventoryAccountId = i.InventoryAccountId,
                                       IsActive = i.IsActive,
                                       ItemCode = i.ItemCode,
                                       ItemName = i.ItemName,
                                       ItemType = ObjectMapper.Map<ItemTypeDetailOutput>(i.ItemType),
                                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.PurchaseAccount),
                                       PurchaseAccountId = i.PurchaseAccountId,
                                       PurchaseCost = i.PurchaseCost,
                                       PurchaseTax = ObjectMapper.Map<TaxDetailOutput>(i.PurchaseTax),
                                       PurchaseTaxId = i.PurchaseTaxId,
                                       SaleAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(i.SaleAccount),
                                       SaleAccountId = i.SaleAccountId,
                                       //SalePrice = i.SalePrice,
                                       SalePrice = itemPrice == null ? 0 : itemPrice.Price,
                                       SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                                       SaleTaxId = i.SaleTaxId
                                   };
                @query = @itemService.Concat(@query);
            }


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }


        public async Task<List<ItemListIdOutput>> CheckItemExist(ItemIdsListInput input)
        {
            var @entities = new List<ItemListIdOutput>();
            if (input.ItemIds == null || input.ItemIds.Count == 0)
            {
                return @entities;
            }
            var userId = AbpSession.GetUserId();
            // get user by group member
            var userGroups = await _userGroupMemberRepository.GetAll()
                            .Include(t => t.UserGroup)
                            .Where(x => x.MemberId == userId)
                            .Where(x => x.UserGroup.LocationId != null)
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.UserGroup.LocationId))
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToListAsync();

            var @queryByGroup = _itemUserGroupRepository.GetAll()
                            .Include(u => u.Item)
                            .WhereIf(input.ItemIds != null && input.ItemIds.Count > 0, t => input.ItemIds.Contains(t.ItemId))
                            .Where(t => t.Item.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            .AsNoTracking()
                            .Select(t => t.Item);

            var @queryItemAll = _itemRepository.GetAll()
                            .WhereIf(input.ItemIds != null && input.ItemIds.Count > 0, t => input.ItemIds.Contains(t.Id))
                            .Where(t => t.Member == Member.All)
                            .AsNoTracking();

            var @itemQuery = @queryItemAll.Union(@queryByGroup);

            //TODO: Check this code for cost
            var averageCosts = new List<InventoryCostItem>();
            IQueryable<ItemListIdOutput> @query = null;

            if (input.IsFilterAverageCost && input.Date != null) //qty
            {
                var updateItemIssueIds = new Dictionary<Guid, Guid>();

                if (input.IsTransfer)
                {
                    var itemIssueIds = await _itemIssueRepository.GetAll().AsNoTracking()
                                        .WhereIf(input.UpdateItemIssueIds != null,
                                                s => input.UpdateItemIssueIds.Contains(s.TransferOrderId))
                                        .Select(s => s.Id).Distinct().ToListAsync();
                    var itemReceiptIds = await _itemReceiptRepository.GetAll().AsNoTracking()
                                        .WhereIf(input.UpdateItemIssueIds != null,
                                                s => input.UpdateItemIssueIds.Contains(s.TransferOrderId))
                                        .Select(s => s.Id).Distinct().ToListAsync();

                    if (itemIssueIds.Any())
                    {
                        itemIssueIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                    if (itemReceiptIds.Any())
                    {
                        itemReceiptIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                }
                else if (input.IsProduction)
                {
                    var itemIssueIds = await _itemIssueRepository.GetAll().AsNoTracking()
                                        .WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId))
                                        .Select(s => s.Id).Distinct().ToListAsync();
                    var itemReceiptIds = await _itemReceiptRepository.GetAll().AsNoTracking()
                                        .WhereIf(input.UpdateItemIssueIds != null, s => input.UpdateItemIssueIds.Contains(s.ProductionOrderId))
                                        .Select(s => s.Id).Distinct().ToListAsync();

                    if (itemIssueIds.Any())
                    {
                        itemIssueIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                    if (itemReceiptIds.Any())
                    {
                        itemReceiptIds.ForEach(s => { updateItemIssueIds.Add(s, s); });
                    }

                }
                else
                {
                    updateItemIssueIds = input.UpdateItemIssueIds != null ? input.UpdateItemIssueIds.Distinct().ToDictionary(t => t.Value, t => t.Value) : new Dictionary<Guid, Guid>();
                }

                //var averageCostList = await _inventoryManager.GetItemsByAverageCost(input.Date.Date, input.Locations, new List<long?>(), updateItemIssueIds);
                var averageCostList = await _inventoryManager.GetItemsBalance("", input.Date.Date, input.Locations, updateItemIssueIds);

                @query = (from t in @itemQuery
                          join i in averageCostList on t.Id equals i.Id into listItem
                          where listItem != null && listItem.Count() > 0
                          && listItem.Any(l => l.Id == t.Id)

                          select new { item = t, listItem })
                                   .SelectMany(t => t.listItem, (t, balance) =>
                                   new ItemListIdOutput
                                   {

                                       QtyOnHand = balance.QtyOnHand,
                                       Id = t.item.Id,
                                       ItemCode = t.item.ItemCode,
                                       ItemName = t.item.ItemName
                                   }).Where(u => u.QtyOnHand > 0);

            }
            else
            {
                @query = @itemQuery.Select(t => new ItemListIdOutput
                {
                    QtyOnHand = 0,
                    Id = t.Id,
                    ItemCode = t.ItemCode,
                    ItemName = t.ItemName
                });
            }

            @entities = await query.ToListAsync();
            return @entities;
        }

        //public async Task<List<ItemGetListBalanceOutput>> GetBalanceQty(GetItemBalanceQtyInput input)
        //{
        //    if (input.Items != null && input.Items.Count > 0 && input.Date != null)
        //    {
        //        //var averageCosts = await _inventoryManager.GetAvgCostQuery(input.Date.Date, input.Locations)
        //        //            .Where(t => input.Items.Contains(t.Id)).ToListAsync();

        //        var averageCosts = await _inventoryManager
        //                                  .GetItemsByAverageCost(input.Date.Date, 
        //                                                        input.Locations, 
        //                                                        new List<long?>(),
        //                                                        updatedItemIssueIds: null, 
        //                                                        itemsToSelect: input.Items.GroupBy(u=>u).ToDictionary(u=>u.Key, u=>u.Key));

        //        var @query = await _itemRepository.GetAll()
        //                   .AsNoTracking()
        //                   .Where(p => input.Items.Contains(p.Id))
        //                   .Select(t => new ItemGetListBalanceOutput
        //                   {
        //                       QtyOnHand = averageCosts.Where(v => v.Id == t.Id)
        //                                    .Select(v => v.QtyOnHand).FirstOrDefault(),
        //                       Id = t.Id,
        //                       AverageCost = averageCosts.Where(v => v.Id == t.Id)
        //                                    .Select(v => v.AverageCost).FirstOrDefault(),
        //                   }).ToListAsync();
        //        return ObjectMapper.Map<List<ItemGetListBalanceOutput>>(@query);
        //    }
        //    else
        //    {
        //        return ObjectMapper.Map<List<ItemGetListBalanceOutput>>(null);
        //    }

        //}

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_GetDetail)]
        public async Task<ItemDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _itemManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //var @subItems = await _subItemRepository
            //                        .GetAll().Include(u => u.Item).AsNoTracking()
            //                        .Where(u => u.ParentSubItemId == entity.Id).ToListAsync();


            var result = ObjectMapper.Map<ItemDetailOutput>(@entity);
            result.BatchNoFormulaName = entity.BatchNoFormula?.Name;

           // result.SubItems = ObjectMapper.Map<List<SubItemDetailOuput>>(@subItems);

            result.UserGroups = await _itemUserGroupRepository.GetAll().Include(t => t.UserGroup)
                           .Where(u => u.ItemId == entity.Id).AsNoTracking()
                           .Select(t => new GroupItems
                           {
                               Id = t.Id,
                               UserGroupId = t.UserGroupId,
                               UserGroupName = t.UserGroup.Name
                           })
                           .ToListAsync();

            var itemLots = await _itemLotRepository.GetAll().Where(s => s.ItemId == input.Id).AsNoTracking()
                .Select(s => new ItemLotDto
                {
                    Id = s.LotId,
                    LotName = s.Lot.LotName,
                    LocationId = s.Lot.LocationId,
                    LocationName = s.Lot.Location.LocationName
                }).ToListAsync();

            result.DefaultLots = itemLots;

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_GetList)]
        [HttpPost]
        public async Task<PagedResultDto<ItemGetListOutput>> GetList(GetItemListInput input)
        {

            var popertyFilterList = input.PropertyDics;

            var @query = _itemRepository
                  .GetAll()
                    .Include(u => u.ItemType)
                    .Include(u => u.InventoryAccount)
                    .Include(u => u.PurchaseTax)
                    .Include(u => u.PurchaseAccount)
                    .Include(u => u.SaleAccount)
                    .Include(u => u.SaleTax)
                    .Include(u => u.SaleCurrency)
                    .Include(u => u.PurchaseCurrency)
                    .Include(u => u.Properties)

                  .AsNoTracking()
                  .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                  .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                  .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))

                  .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                  .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                  .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                  .WhereIf(
                      !input.Filter.IsNullOrEmpty(),
                      p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                           p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                           (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                  );

            var resultCount = await query.CountAsync();
            var @entities = await query.ToListAsync();
            if (input.UsePagination)
            {
                entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {

                entities = await query.ToListAsync();
            }

            return new PagedResultDto<ItemGetListOutput>(resultCount, ObjectMapper.Map<List<ItemGetListOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Update)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<NullableIdDto<Guid>> UpdateAsync(UpdateItemInput input)
        {
            input.IsGenerateItemCode = false;
            var result = await HelperUpdate(input);
            return result;
        }
        #endregion


        #region Export and Import Excel
        private ReportOutput GetReportTemplateItem()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 120,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemType",
                        ColumnLength = 200,
                        ColumnTitle = "Item Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "InventoryAccount",
                        ColumnLength = 120,
                        ColumnTitle = "Inventory Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PurchaseCost",
                        ColumnLength = 120,
                        ColumnTitle = "PurchaseCost",
                        ColumnType = ColumnType.Money,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PurchaseAccount",
                        ColumnLength = 120,
                        ColumnTitle = "Purchase Account",
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
                        ColumnName = "PurchaseTax",
                        ColumnLength = 120,
                        ColumnTitle = "Purchase Tax",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },


                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SalePrice",
                        ColumnLength = 120,
                        ColumnTitle = "Sale Price",
                        ColumnType = ColumnType.Money,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SaleAccount",
                        ColumnLength = 120,
                        ColumnTitle = "Sale Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SaleTax",
                        ColumnLength = 120,
                        ColumnTitle = "Sale Tax",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 120,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ReorderPoint",
                        ColumnLength = 120,
                        ColumnTitle = "Reorder Point",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TrackSerial",
                        ColumnLength = 120,
                        ColumnTitle = "Track Serial",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShowSubitems",
                        ColumnLength = 120,
                        ColumnTitle = "Show Subitems",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Barcode",
                        ColumnLength = 120,
                        ColumnTitle = "Barcode",
                        ColumnType = ColumnType.String,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UseBatchNo",
                        ColumnLength = 120,
                        ColumnTitle = "UseBatchNo",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AutoBatchNo",
                        ColumnLength = 120,
                        ColumnTitle = "AutoBatchNo",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 17,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchNoFormula",
                        ColumnLength = 120,
                        ColumnTitle = "BatchNoFormula",
                        ColumnType = ColumnType.String,
                        SortOrder = 18,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TrackExpiration",
                        ColumnLength = 120,
                        ColumnTitle = "Track Expiration",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 19,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BarcodeSameAsItemCode",
                        ColumnLength = 120,
                        ColumnTitle = "Barcode Same As Item Code",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 20,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                       },

                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Min",
                        ColumnLength = 120,
                        ColumnTitle = "Min",
                        ColumnType = ColumnType.Number,
                        SortOrder = 21,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Max",
                        ColumnLength = 120,
                        ColumnTitle = "Max",
                        ColumnType = ColumnType.Number,
                        SortOrder = 22,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Items",
                Sortby = "",
            };
            return result;
        }

        private ReportOutput GetReportTemplateSubItem()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     // 
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 120,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 120,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Quantity",
                        ColumnLength = 120,
                        ColumnTitle = "Quantity",
                        ColumnType = ColumnType.Number,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ParentItem",
                        ColumnLength = 120,
                        ColumnTitle = "Parent Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Sub Items",
                Sortby = "",
            };
            return result;
        }


        private ReportOutput GetReportTemplateDefaultLots()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     // 
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 120,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 120,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LotName",
                        ColumnLength = 120,
                        ColumnTitle = "Zone Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    }
                },
                Groupby = "",
                HeaderTitle = "Default Lots",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcel(GetItemListInput input)
        {
            var popertyFilterList = input.PropertyDics;
            var tenantId = AbpSession.TenantId;
            var itemTypes = new List<string>();
            var taxs = new List<string>();
            var chartOfAccounts = new List<string>();
            var @itemData = new List<Item>();       
            var itemLots = new List<LotOutputExport>();
            var propertiesHeader = new List<Property>();
            var itemIds = new List<Guid>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    itemTypes = await _itemTypeRepository.GetAll().Select(t => t.Name).AsNoTracking().ToListAsync();
                    taxs = await _taxRepository.GetAll().Select(t => t.TaxName).AsNoTracking().ToListAsync();
                    chartOfAccounts = await _chartOfAccountRepository.GetAll().Select(t => t.AccountName).AsNoTracking().ToListAsync();
                    @itemData = await _itemRepository.GetAll()
                          .Include("Properties.Property")
                          .Include("Properties.PropertyValue")
                          .Include(u => u.ItemType)
                          .Include(u => u.SaleCurrency)
                          .Include(u => u.PurchaseCurrency)
                          .Include(u => u.SaleAccount)
                          .Include(u => u.PurchaseAccount)
                          .Include(u => u.InventoryAccount)
                          .Include(u => u.SaleTax)
                          .Include(u => u.PurchaseTax)
                          .Include(u => u.BatchNoFormula)
                          .AsNoTracking()
                         .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                         .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                         .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                              .Count() == popertyFilterList.Count))

                         .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                         .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                         .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                         .WhereIf(
                             !input.Filter.IsNullOrEmpty(),
                             p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                  p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                  (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                         ).ToListAsync();
                    itemIds = itemData.Select(s => s.Id).ToList();
                   

                    itemLots = await _itemLotRepository.GetAll()
                                        .WhereIf(itemIds.Count > 0, s => itemIds.Contains(s.ItemId))
                                        .AsNoTracking()
                                        .Select(s => new LotOutputExport
                                        {
                                            ItemName = s.Item.ItemName,
                                            LotName = s.Lot.LotName,
                                            ItemCode = s.Item.ItemCode
                                        })
                                        .ToListAsync();

                    propertiesHeader = await _propertyRepository.GetAll().Where(t => t.IsActive == true).AsNoTracking().ToListAsync();
                }
            }

            var result = new FileDto();
            var sheetName = "Items";        
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
                var headerList = GetReportTemplateItem();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                int colHeaderPropertyTable = colHeaderTable;
                foreach (var i in propertiesHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderPropertyTable, i.Name, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(100);
                    headerList.ColumnInfo.Add(new CollumnOutput
                    {
                        ColumnName = i.Name,

                    });
                    colHeaderPropertyTable += 1;
                }
                #endregion Row 1

             

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                foreach (var i in itemData)
                {
                    int columnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        switch (h.ColumnName)
                        {
                            case "ItemType":
                                AddTextToCell(ws, rowBody, columnCellBody, i.ItemType.Name, false);
                                break;
                            case "TrackSerial":
                                AddTextToCell(ws, rowBody, columnCellBody, i.TrackSerial.ToString(), false);
                                break;
                            case "TrackExpiration":
                                AddTextToCell(ws, rowBody, columnCellBody, i.TrackExpiration.ToString(), false);
                                break;
                            case "ShowSubitems":
                                AddTextToCell(ws, rowBody, columnCellBody, i.ShowSubItems.ToString(), false);
                                break;
                            case "UseBatchNo":
                                AddTextToCell(ws, rowBody, columnCellBody, i.UseBatchNo.ToString(), false);
                                break;
                            case "AutoBatchNo":
                                AddTextToCell(ws, rowBody, columnCellBody, i.AutoBatchNo.ToString(), false);
                                break;
                            case "BatchNoFormula":
                                AddTextToCell(ws, rowBody, columnCellBody, i.BatchNoFormula?.Name, true);
                                break;
                            case "BarcodeSameAsItemCode":
                                AddTextToCell(ws, rowBody, columnCellBody, i.BarcodeSameAsItemCode.ToString(), false);
                                break;
                            default:
                                WriteBodyItems(ws, rowBody, columnCellBody, h, i, count);
                                break;
                        }

                        columnCellBody++;
                    }
                    rowBody++;
                    count++;
                }
                #endregion Row Body                          
                //A workbook must have at least on cell, so lets add one... 
                var defaultLot = p.Workbook.Worksheets.Add("DefaultZones");
                defaultLot.PrinterSettings.Orientation = eOrientation.Landscape;
                defaultLot.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                defaultLot.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                defaultLot.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowDefaultLotTableHeader = 1;
                int colDefaultLotHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerDefaultLotsList = GetReportTemplateDefaultLots();

                foreach (var i in headerDefaultLotsList.ColumnInfo)
                {
                    AddTextToCell(defaultLot, rowDefaultLotTableHeader, colDefaultLotHeaderTable, i.ColumnTitle, true);

                    defaultLot.Column(colDefaultLotHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colDefaultLotHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowDefaultLotBody = rowDefaultLotTableHeader + 1;//start from row header of spreadsheet
                int countDefaultLot = 1;
                // write body
                foreach (var i in itemLots)
                {
                    int collumnDefaultLotCellBody = 1;
                    foreach (var h in headerDefaultLotsList.ColumnInfo)
                    {
                        if (h.ColumnType == ColumnType.String)
                        {
                            var value = i.GetType().GetProperty(h.ColumnName).GetValue(i, null);
                            AddTextToCell(defaultLot, rowDefaultLotBody, collumnDefaultLotCellBody, value?.ToString());
                        }

                        collumnDefaultLotCellBody += 1;
                    }
                    rowDefaultLotBody += 1;
                    countDefaultLot += 1;
                }

                #endregion Row Body    


                result.FileName = $"Item_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelTemplate(GetItemListInput input)
        {
            var popertyFilterList = input.PropertyDics;
            var tenantId = AbpSession.TenantId;
            var itemTypes = new List<string>();
            var taxs = new List<string>();
            var chartOfAccounts = new List<string>();
            var @itemData = new List<Item>();        
            var itemLots = new List<LotOutputExport>();
            var propertiesHeader = new List<Property>();
            var itemIds = new List<Guid>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    itemTypes = await _itemTypeRepository.GetAll().Select(t => t.Name).AsNoTracking().ToListAsync();
                    taxs = await _taxRepository.GetAll().Select(t => t.TaxName).AsNoTracking().ToListAsync();
                    chartOfAccounts = await _chartOfAccountRepository.GetAll().Select(t => t.AccountName).AsNoTracking().ToListAsync();
                    @itemData = await _itemRepository.GetAll()
                          .Include("Properties.Property")
                          .Include("Properties.PropertyValue")
                          .Include(u => u.ItemType)
                          .Include(u => u.SaleCurrency)
                          .Include(u => u.PurchaseCurrency)
                          .Include(u => u.SaleAccount)
                          .Include(u => u.PurchaseAccount)
                          .Include(u => u.InventoryAccount)
                          .Include(u => u.SaleTax)
                          .Include(u => u.PurchaseTax)
                          .Include(u => u.BatchNoFormula)
                          .AsNoTracking()
                         .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                         .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                         .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                              .Count() == popertyFilterList.Count))

                         .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                         .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                         .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                         .WhereIf(
                             !input.Filter.IsNullOrEmpty(),
                             p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                  p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                  (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                         ).Take(3).ToListAsync();
                    itemIds = itemData.Select(s => s.Id).ToList();                 
                    itemLots = await _itemLotRepository.GetAll()
                                        .WhereIf(itemIds.Count > 0, s => itemIds.Contains(s.ItemId))
                                        .AsNoTracking()
                                        .Select(s => new LotOutputExport
                                        {
                                            ItemName = s.Item.ItemName,
                                            LotName = s.Lot.LotName,
                                            ItemCode = s.Item.ItemCode
                                        })
                                        .ToListAsync();

                    propertiesHeader = await _propertyRepository.GetAll().Where(t => t.IsActive == true).AsNoTracking().ToListAsync();
                }
            }

            var result = new FileDto();
            var sheetName = "Items";         
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
                var headerList = GetReportTemplateItem();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                int colHeaderPropertyTable = colHeaderTable;
                foreach (var i in propertiesHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderPropertyTable, i.Name, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(100);
                    headerList.ColumnInfo.Add(new CollumnOutput
                    {
                        ColumnName = i.Name,

                    });
                    colHeaderPropertyTable += 1;
                }
                #endregion Row 1

                var dataSheet = p.Workbook.Worksheets.Add("Data");
                //Write data 
                var rowData = 1;
                foreach (var d in itemTypes.ToArray())
                {
                    dataSheet.Cells[rowData, 1].Value = d;
                    rowData++;
                }
                var listRage = $"'Data'!A1: A{rowData - 1}";
                dataSheet.Hidden = eWorkSheetHidden.Hidden;

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "ItemType")
                        {
                            SetDataValidatorNotTranslate(ws, rowBody, collumnCellBody, listRage, i.ItemType.Name);
                        }
                        else if (h.ColumnName == "TrackSerial")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackSerial.ToString(), false);
                        }
                        else if (h.ColumnName == "TrackExpiration")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackExpiration.ToString(), false);

                        }
                        else if (h.ColumnName == "ShowSubitems")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ShowSubItems.ToString(), false);

                        }
                        else if (h.ColumnName == "UseBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.UseBatchNo.ToString(), false);

                        }
                        else if (h.ColumnName == "AutoBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.AutoBatchNo.ToString(), false);

                        }
                        else if (h.ColumnName == "BatchNoFormula")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.BatchNoFormula?.Name, true);
                        }
                        else if (h.ColumnName == "BarcodeSameAsItemCode")
                        {

                            AddTextToCell(ws, rowBody, collumnCellBody, i.BarcodeSameAsItemCode.ToString(), false);

                        }
                        else
                        {
                            WriteBodyItems(ws, rowBody, collumnCellBody, h, i, count);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body                        
                #region Row 1 Header Table              
                #endregion Row 1
                #region Row Body              
                #endregion Row Body    


                //A workbook must have at least on cell, so lets add one... 
                var defaultLot = p.Workbook.Worksheets.Add("DefaultZones");
                defaultLot.PrinterSettings.Orientation = eOrientation.Landscape;
                defaultLot.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                defaultLot.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                defaultLot.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowDefaultLotTableHeader = 1;
                int colDefaultLotHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerDefaultLotsList = GetReportTemplateDefaultLots();

                foreach (var i in headerDefaultLotsList.ColumnInfo)
                {
                    AddTextToCell(defaultLot, rowDefaultLotTableHeader, colDefaultLotHeaderTable, i.ColumnTitle, true);

                    defaultLot.Column(colDefaultLotHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colDefaultLotHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowDefaultLotBody = rowDefaultLotTableHeader + 1;//start from row header of spreadsheet
                int countDefaultLot = 1;
                // write body
                foreach (var i in itemLots)
                {
                    int collumnDefaultLotCellBody = 1;
                    foreach (var h in headerDefaultLotsList.ColumnInfo)
                    {
                        if (h.ColumnType == ColumnType.String)
                        {
                            var value = i.GetType().GetProperty(h.ColumnName).GetValue(i, null);
                            AddTextToCell(defaultLot, rowDefaultLotBody, collumnDefaultLotCellBody, value?.ToString());
                        }

                        collumnDefaultLotCellBody += 1;
                    }
                    rowDefaultLotBody += 1;
                    countDefaultLot += 1;
                }

                #endregion Row Body    


                result.FileName = $"Item_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            // Open and read the XlSX file.
            List<Item> items = new List<Item>();       
            List<ItemLot> itemLots = new List<ItemLot>();

            List<ItemType> itemTypes = new List<ItemType>();
            List<ChartOfAccount> chartOfAccounts = new List<ChartOfAccount>();
            List<Tax> taxs = new List<Tax>();
            List<Lot> lots = new List<Lot>();

            List<Property> property = new List<Property>();
            List<PropertyValue> propertyValue = new List<PropertyValue>();
            List<BatchNoFormula> batchNoFormulas = new List<BatchNoFormula>();
            List<ItemCodeFormulaItemType> formularItemTypes = new List<ItemCodeFormulaItemType>();
            List<ItemCodeFormulaProperty> formulaItemProperties = new List<ItemCodeFormulaProperty>();
            List<ItemCodeFormulaCustom> formularCustoms = new List<ItemCodeFormulaCustom>();
            var itemCodes = new List<string>();
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId.Value;
            Tenant tenant;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    // Open and read the XlSX file.
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();                
                    itemLots = await _itemLotRepository.GetAll().Include(s => s.Item).Include(s => s.Lot).AsNoTracking().ToListAsync();

                    @itemTypes = await _itemTypeRepository.GetAll().AsNoTracking().ToListAsync();
                    chartOfAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
                    @taxs = await _taxRepository.GetAll().AsNoTracking().AsNoTracking().ToListAsync();
                    lots = await _lotRepository.GetAll().AsNoTracking().ToListAsync();

                    @property = await _propertyRepository.GetAll().AsNoTracking().ToListAsync();
                    @propertyValue = await _propertyValueRepository.GetAll().AsNoTracking().ToListAsync();
                    batchNoFormulas = await _batchNoFormulaRepository.GetAll().AsNoTracking().ToListAsync();
                    tenant = await _tenantRepository.GetAll().AsNoTracking().Where(t => t.Id == tenantId).FirstOrDefaultAsync();
                    //formula queries
                    formularItemTypes = await _itemCodeFormulaItemTypeRepository.GetAll().Include(s => s.ItemCodeFormula).AsNoTracking().ToListAsync();
                    formulaItemProperties = await _itemCodeFormulaPropertyRepository.GetAll().Include(u => u.Property).AsNoTracking().OrderBy(s => s.SortOrder).ToListAsync();
                    formularCustoms = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking().ToListAsync();
                }
            }


            itemCodes = items.Select(s => s.ItemCode).ToList();
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var headerList = GetReportTemplateItem();
            var colIndex = headerList.ColumnInfo.Count();

            List<Item> importItems = new List<Item>();      
            List<ItemLot> importItemLots = new List<ItemLot>();
            var addItemProperties = new List<ItemProperty>();
            List<string> Itemcodes = new List<string>();
            // List<createImport> lst = new List<createImport>();
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var ItemName = worksheet.Cells[i, 2].Value?.ToString();
                        var ItemTypeName = worksheet.Cells[i, 3].Value?.ToString();
                        var InventoryAccount = worksheet.Cells[i, 4].Value?.ToString();
                        var PurchaseCost = worksheet.Cells[i, 5].Value?.ToString();
                        var PurchaseAccount = worksheet.Cells[i, 6].Value?.ToString();
                        var PurchaseTax = worksheet.Cells[i, 7].Value?.ToString();
                        var SalePrice = worksheet.Cells[i, 8].Value?.ToString();
                        var SaleAccount = worksheet.Cells[i, 9].Value?.ToString();
                        var SaleTax = worksheet.Cells[i, 10].Value?.ToString();
                        var Description = worksheet.Cells[i, 11].Value?.ToString();
                        var ReorderPoint = worksheet.Cells[i, 12].Value?.ToString();
                        var TrackSerial = worksheet.Cells[i, 13].Value?.ToString();
                        var showSubitems = worksheet.Cells[i, 14].Value?.ToString();
                        var barcode = worksheet.Cells[i, 15].Value?.ToString();
                        var useBatchNo = worksheet.Cells[i, 16].Value?.ToString();
                        var autoBatchNo = worksheet.Cells[i, 17].Value?.ToString();
                        var batchNoFormula = worksheet.Cells[i, 18].Value?.ToString();
                        var trackExpiration = worksheet.Cells[i, 19].Value?.ToString();
                        //too do
                        var barcodeSameAsItemCode = worksheet.Cells[i, 20].Value?.ToString();
                        var min = worksheet.Cells[i, 21].Value?.ToString();
                        var max = worksheet.Cells[i, 22].Value?.ToString();

                        var itemType = itemTypes.FirstOrDefault(s => s.Name == ItemTypeName.ToString());
                        if (itemType == null) throw new UserFriendlyException(L("IsNotValid", L("ItemType") + $" {ItemTypeName} Row = {i}"));
                        var itemTypeId = itemType.Id;

                        var inputApi = new GenerateItemCodeInput
                        {
                            // ItemCode = formulaCustom.ItemCode,
                            ItemTypeId = itemTypeId,
                            TenantId = tenant.Id,
                            ItemProperties = new List<CreateOrItemPropertyInput>()
                        };

                        var formulaType = formularItemTypes.Where(s => s.ItemTypeId == itemTypeId).FirstOrDefault();
                        var formulaProperties = formulaItemProperties.Where(s => formulaType != null && s.ItemCodeFormulaId == formulaType.ItemCodeFormulaId).OrderBy(s => s.SortOrder).ToList();
                        var formulaCustom = formularCustoms.FirstOrDefault(s => formulaType != null && s.ItemCodeFormulaId == formulaType.ItemCodeFormulaId);

                        var ItemCode = GenerateItemCode(inputApi, worksheet, i, colIndex, items, propertyValue, tenant, property, formulaType, formulaCustom, importItems, formulaProperties);

                        //if item ixist in database
                        var find = items.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (find != null) continue;

                        //if item Duplicate in excel list
                        var findInList = importItems.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (findInList != null) throw new UserFriendlyException(L("Duplicated", L("ItemCode")) + $" {ItemCode} Row {i}");

                        if (string.IsNullOrWhiteSpace(ItemCode)) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $", Row {i}");

                        Guid? inventoryAccountId = null;
                        if (itemType.DisplayInventoryAccount)
                        {
                            if (string.IsNullOrEmpty(InventoryAccount)) throw new UserFriendlyException(L("PleaseEnter", L("InventoryAccount")) + " Row = " + i);

                            var inventoryAccount = chartOfAccounts.FirstOrDefault(s => s.AccountName == InventoryAccount.ToString());
                            if (inventoryAccount == null) throw new UserFriendlyException(L("IsNotValid", L("InventoryAccount") + $" {InventoryAccount} Row = {i}"));
                            inventoryAccountId = inventoryAccount.Id;
                        }

                        Guid? purchaseAccountId = null;
                        long? purchaseTaxId = null;
                        Guid? saleAccountId = null;
                        long? saleTaxId = null;

                        if (PurchaseAccount == null && SaleAccount == null)
                        {
                            throw new UserFriendlyException(L("AtLeastOnePurchaseOrSaleAccountIsRequired") + " Row = " + i);
                        }
                        if (!string.IsNullOrWhiteSpace(barcodeSameAsItemCode) && Convert.ToBoolean(barcodeSameAsItemCode) == true && !string.IsNullOrWhiteSpace(barcode))
                        {
                            throw new UserFriendlyException(L("BarcodeUserSameAsItemCodeThisFieldMaybeBlank") + " Row = " + i);
                        }
                        if (PurchaseAccount != null && PurchaseTax == null) throw new UserFriendlyException(L("PleaseEnter", L("PurchaseTax")) + $" Row = {i}");
                        if (SaleAccount != null && SaleTax == null) throw new UserFriendlyException(L("PleaseEnter", L("SaleTax")) + $" Row = {i}");

                        if (PurchaseAccount != null && PurchaseTax != null)
                        {
                            purchaseAccountId = chartOfAccounts.Where(s => s.AccountName == PurchaseAccount).Select(s => s.Id).FirstOrDefault();
                            purchaseTaxId = taxs.Where(s => s.TaxName == PurchaseTax).Select(s => s.Id).FirstOrDefault();
                        }

                        if (SaleAccount != null && SaleTax != null)
                        {
                            saleAccountId = chartOfAccounts.Where(s => s.AccountName == SaleAccount).Select(s => s.Id).FirstOrDefault();
                            saleTaxId = taxs.Where(s => s.TaxName == SaleTax).Select(s => s.Id).FirstOrDefault();
                        }

                        var useBatch = Convert.ToBoolean(useBatchNo);
                        var autoBatch = Convert.ToBoolean(autoBatchNo);
                        if (autoBatch && !useBatch) throw new UserFriendlyException(L("UseBatchNoMustTrueForAutoBatchNo") + $", Row = {i}");
                        if (autoBatch && batchNoFormula.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("AutoBatchNoMustHasBatchNoFormula") + $", Row = {i}");
                        if (!autoBatch && !batchNoFormula.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("AutoBatchNoMustTrueForBatchNoFormula") + $", Row = {i}");

                        BatchNoFormula formula = null;
                        if (useBatch && autoBatch && !string.IsNullOrWhiteSpace(batchNoFormula))
                        {
                            formula = batchNoFormulas.FirstOrDefault(s => s.Name == batchNoFormula);
                            if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $" Row = {i}");
                        }

                        var useSerial = string.IsNullOrWhiteSpace(TrackSerial) ? false : Convert.ToBoolean(TrackSerial);

                        if (useSerial && useBatch) throw new UserFriendlyException(L("CannotUseBoth", L("BatchNo"), L("Serial")) + $", Row {i}");
                        //to do
                        var @entity = Item.Create(tenantId, userId, ItemName, ItemCode,
                                        string.IsNullOrWhiteSpace(SalePrice) ? (decimal?)null : Convert.ToDecimal(SalePrice),
                                        string.IsNullOrWhiteSpace(PurchaseCost) ? (decimal?)null : Convert.ToDecimal(PurchaseCost),
                                        string.IsNullOrWhiteSpace(ReorderPoint) ? (decimal?)null : Convert.ToDecimal(ReorderPoint),
                                        useSerial,
                                        null,
                                        null, purchaseTaxId, saleTaxId, saleAccountId, purchaseAccountId,
                                        inventoryAccountId, itemTypeId, Description, barcode,
                                        string.IsNullOrWhiteSpace(useBatchNo) ? false : Convert.ToBoolean(useBatchNo),
                                        string.IsNullOrWhiteSpace(autoBatchNo) ? false : Convert.ToBoolean(autoBatchNo),
                                        formula?.Id,
                                        string.IsNullOrWhiteSpace(trackExpiration) ? false : Convert.ToBoolean(trackExpiration),
                                        string.IsNullOrWhiteSpace(barcodeSameAsItemCode) ? false : Convert.ToBoolean(barcodeSameAsItemCode)
                                         );

                        @entity.UpdateMember(Member.All);
                        entity.SetShowSubItems(string.IsNullOrWhiteSpace(showSubitems) ? false : Convert.ToBoolean(showSubitems));
                        var minVaule = min != null ? Convert.ToDecimal(min) : 0;
                        var maxVaule = min != null ? Convert.ToDecimal(max) : 0;
                        entity.SetMinMax(minVaule, maxVaule);
                        // insert with property 
                        int po = 0;
                        foreach (var pr in property)
                        {
                            po++;
                            var index = worksheet.Cells[i, colIndex + po].Value?.ToString();

                            if (string.IsNullOrWhiteSpace(index) && pr.IsRequired) throw new UserFriendlyException(L("IsRequired", pr.Name));
                            if (!string.IsNullOrWhiteSpace(index))
                            {
                                var pro = propertyValue.Where(g => g.PropertyId == pr.Id && g.Value == index).FirstOrDefault();
                                if (pro == null) throw new UserFriendlyException(L("IsNotValid", L("Property")) + $" {index} Row = {i}");

                                var p = @entity.AddProperty(userId, pro.PropertyId.Value, pro.Id);
                                addItemProperties.Add(p);
                            }

                        }

                        importItems.Add(entity);

                        itemCodes.Add(ItemCode);
                        var isDuplicated = itemCodes.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).Any();
                        if (isDuplicated) throw new UserFriendlyException(L("DuplicateItemCode", ItemCode));

                    }

                    //select item lots
                    var lotWorkSeet = excelPackage.Workbook.Worksheets["DefaultZones"];
                    for (int s = 2; s <= lotWorkSeet.Dimension.End.Row; s++)
                    {
                        var ItemCode = lotWorkSeet.Cells[s, 1].Value?.ToString();
                        var ItemName = lotWorkSeet.Cells[s, 2].Value?.ToString();
                        var LotName = lotWorkSeet.Cells[s, 3].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(ItemName) && tenant.ItemCodeSetting != ItemCodeSetting.Custom) throw new UserFriendlyException(L("PleaseEnter", L("ItemName")) + $" Row = {s}");
                        if (string.IsNullOrWhiteSpace(ItemCode) && tenant.ItemCodeSetting == ItemCodeSetting.Custom) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $" Row = {s}"); ;
                        if (string.IsNullOrWhiteSpace(LotName)) throw new UserFriendlyException(L("PleaseEnter", L("LotName")) + $" Row = {s}");

                        var checkLot = lots.FirstOrDefault(t => t.LotName == LotName);
                        if (checkLot == null) throw new UserFriendlyException(L("IsNotValid", L("Lot")) + $" {LotName} Row = {s}");

                        var allItems = importItems;
                        var checkItem = tenant.ItemCodeSetting != ItemCodeSetting.Custom ? allItems.FirstOrDefault(t => t.ItemName == ItemName) : allItems.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (checkItem == null && tenant.ItemCodeSetting != ItemCodeSetting.Custom) throw new UserFriendlyException(L("IsNotValid", L("Item")) + $" {ItemName} Row = {s}");
                        if (checkItem == null && tenant.ItemCodeSetting == ItemCodeSetting.Custom) throw new UserFriendlyException(L("IsNotValid", L("Item")) + $" {ItemCode} Row = {s}");
                        //if exist in database
                        var find = itemLots.FirstOrDefault(t => t.ItemId == checkItem.Id && t.LotId == checkLot.Id);
                        if (find != null) continue;

                        //must have only one Lot for one Location for each item
                        var allItemLosts = itemLots.Concat(importItemLots);
                        var checkDuppliate = allItemLosts.FirstOrDefault(t => t.ItemId == checkItem.Id && t.Lot.LocationId == checkLot.LocationId);
                        if (checkDuppliate != null) throw new UserFriendlyException(L("Duplicated", L("Location")) + $" Row = {s}");

                        var itemLot = ItemLot.Create(tenantId, userId, checkItem.Id, checkLot.Id);
                        itemLot.Lot = checkLot; //use to check Duplicate location
                        importItemLots.Add(itemLot);
                    }

                }
            }
            //RemoveFile(input, _appFolders);



            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await this.CheckMaxItemCountAsync(tenantId.Value, importItems.Count);
                    // Open and read the XlSX file.
                    await _itemRepository.BulkInsertAsync(importItems);
                    if (addItemProperties.Any()) await _itemPropertyRepository.BulkInsertAsync(addItemProperties);                  
                    if (importItemLots.Any()) await _itemLotRepository.BulkInsertAsync(importItemLots);
                }
                await uow.CompleteAsync();
            }

        }


        private ReportOutput GetExportEditTemplate()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsRequired = true,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ItemName",
                        ColumnLength = 100,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.Language,
                        SortOrder = 2,
                        Visible = true,
                        IsRequired = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TrackSerial",
                        ColumnLength = 120,
                        ColumnTitle = "Track Serial",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                         IsRequired = true,
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UseBatchNo",
                        ColumnLength = 120,
                        ColumnTitle = "UseBatchNo",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                         IsRequired = true,
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AutoBatchNo",
                        ColumnLength = 120,
                        ColumnTitle = "AutoBatchNo",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchNoFormula",
                        ColumnLength = 120,
                        ColumnTitle = "BatchNoFormula",
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
                        ColumnName = "TrackExpiration",
                        ColumnLength = 120,
                        ColumnTitle = "Track Expiration",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true
                    },
                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Barcode",
                        ColumnLength = 120,
                        ColumnTitle = "Barcode",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Min",
                        ColumnLength = 120,
                        ColumnTitle = "Min",
                        ColumnType = ColumnType.Number,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                         new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Max",
                        ColumnLength = 120,
                        ColumnTitle = "Max",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Items",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelEditTemplate(GetItemListInput input)
        {

            var popertyFilterList = input.PropertyDics;
            var tenantId = AbpSession.TenantId;
            var @itemData = new List<Item>();
            var formulaList = new List<string>();
            var itemLots = new List<LotOutputExport>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    @itemData = await _itemRepository.GetAll()
                                          .Include(s => s.BatchNoFormula)
                                          .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                          .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                                          .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                                          popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                                          (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                                          popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                                          popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                                               .Count() == popertyFilterList.Count))

                                          .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                                          .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                                          .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                                          .WhereIf(
                                              !input.Filter.IsNullOrEmpty(),
                                              p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                   p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                   (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                                          ).AsNoTracking()
                                           .ToListAsync();
                    var itemIds = itemData.Select(s => s.Id).ToList();
                    itemLots = await _itemLotRepository.GetAll()

                                        .WhereIf(itemIds.Count > 0, s => itemIds.Contains(s.ItemId))
                                        .AsNoTracking()
                                       .Select(s => new LotOutputExport
                                       {
                                           ItemName = s.Item.ItemName,
                                           LotName = s.Lot.LotName,
                                           ItemId = s.ItemId,
                                           ItemCode = s.Item.ItemCode,
                                       })
                                       .ToListAsync();

                    formulaList = await _batchNoFormulaRepository.GetAll().AsNoTracking().Select(s => s.Name).ToListAsync();
                }

            }


            var result = new FileDto();
            var sheetName = "Items";
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
                var headerList = GetExportEditTemplate();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }


                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var trueFalseList = new List<string> { "True", "False" };
                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "TrackSerial")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackSerial.ToString(), true);
                            // AddDropdownList(ws, rowBody, collumnCellBody, trueFalseList, i.TrackSerial.ToString());
                        }
                        else if (h.ColumnName == "UseBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.UseBatchNo.ToString(), false);
                            // AddDropdownList(ws, rowBody, collumnCellBody, trueFalseList, i.UseBatchNo.ToString());
                        }
                        else if (h.ColumnName == "AutoBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.AutoBatchNo.ToString(), false);
                            // AddDropdownList(ws, rowBody, collumnCellBody, trueFalseList, i.AutoBatchNo.ToString());
                        }
                        else if (h.ColumnName == "BatchNoFormula")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.BatchNoFormula?.Name, true);
                        }
                        //else if (h.ColumnName == "BatchNoFormula")
                        //{                          
                        //    AddDropdownList(ws, rowBody, collumnCellBody, formulaList, i.BatchNoFormula?.Name.ToString());
                        //}
                        else if (h.ColumnName == "ItemCode")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ItemCode, true);
                        }
                        else if (h.ColumnName == "ItemName")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ItemName, true);
                        }
                        else if (h.ColumnName == "Barcode")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.Barcode, true);
                        }
                        else if (h.ColumnName == "TrackExpiration")
                        {

                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackExpiration.ToString(), false);
                        }
                        else if (h.ColumnName == "Min")
                        {
                            AddNumberToCell(ws, rowBody, collumnCellBody, Convert.ToDecimal(i.Min), false, true);
                        }
                        else if (h.ColumnName == "Max")
                        {
                            AddNumberToCell(ws, rowBody, collumnCellBody, Convert.ToDecimal(i.Max), false, true);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body             


                //A workbook must have at least on cell, so lets add one... 
                var defaultLot = p.Workbook.Worksheets.Add("DefaultZones");
                defaultLot.PrinterSettings.Orientation = eOrientation.Landscape;
                defaultLot.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                defaultLot.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                defaultLot.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowDefaultLotTableHeader = 1;
                int colDefaultLotHeaderTable = 1;//start from row 1 of spreadsheet
                                                 // write header collumn table
                var headerDefaultLotsList = GetReportTemplateDefaultLots();

                foreach (var i in headerDefaultLotsList.ColumnInfo)
                {
                    AddTextToCell(defaultLot, rowDefaultLotTableHeader, colDefaultLotHeaderTable, i.ColumnTitle, true);

                    defaultLot.Column(colDefaultLotHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colDefaultLotHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowDefaultLotBody = rowDefaultLotTableHeader + 1;//start from row header of spreadsheet
                int countDefaultLot = 1;
                // write body
                var itemids = itemData.Select(s => s.Id).ToList();
                var resultItemLots = itemLots.Where(l => itemids.Contains(l.ItemId));
                foreach (var i in resultItemLots)
                {
                    int collumnDefaultLotCellBody = 1;
                    foreach (var h in headerDefaultLotsList.ColumnInfo)
                    {
                        if (h.ColumnType == ColumnType.String)
                        {
                            var value = i.GetType().GetProperty(h.ColumnName).GetValue(i, null);
                            AddTextToCell(defaultLot, rowDefaultLotBody, collumnDefaultLotCellBody, value?.ToString());
                        }

                        collumnDefaultLotCellBody += 1;
                    }
                    rowDefaultLotBody += 1;
                    countDefaultLot += 1;
                }

                #endregion Row Body    

                result.FileName = $"ItemEditTemplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelEditItem(FileDto input)
        {
            // Open and read the XlSX file.
            List<Item> items = new List<Item>();
            List<BatchNoFormula> batchNoFormulas = new List<BatchNoFormula>();

            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId.Value;
            List<Item> importItems = new List<Item>();
            List<ItemLot> importItemLots = new List<ItemLot>();
            List<Lot> lots = new List<Lot>();
            List<ItemLot> itemLots = new List<ItemLot>();
            List<ItemLotOutputExport> excludeForItemLots = new List<ItemLotOutputExport>();
            List<ItemLot> itemLotForDeletes = new List<ItemLot>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    // Open and read the XlSX file.
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();
                    batchNoFormulas = await _batchNoFormulaRepository.GetAll().AsNoTracking().ToListAsync();
                    itemLots = await _itemLotRepository.GetAll().Include(s => s.Item).Include(s => s.Lot.Location).AsNoTracking().ToListAsync();
                    lots = await _lotRepository.GetAll().AsNoTracking().ToListAsync();

                }
            }
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);



            // List<createImport> lst = new List<createImport>();
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var ItemCode = worksheet.Cells[i, 1].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(ItemCode)) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $", Row {i}");

                        //if item ixist in database
                        var find = items.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (find == null) throw new UserFriendlyException(L("IsNotValid", L("ItemCode")) + $", Row {i}");

                        var TrackSerial = worksheet.Cells[i, 3].Value?.ToString();
                        var useBatchNo = worksheet.Cells[i, 4].Value?.ToString();
                        var autoBatchNo = worksheet.Cells[i, 5].Value?.ToString();
                        var batchNoFormula = worksheet.Cells[i, 6].Value?.ToString();
                        var trackExpiration = worksheet.Cells[i, 7].Value?.ToString();
                        var barcode = worksheet.Cells[i, 8].Value?.ToString();
                        var min = worksheet.Cells[i, 9].Value?.ToString();
                        var max = worksheet.Cells[i, 10].Value?.ToString();

                        var useBatch = Convert.ToBoolean(useBatchNo);
                        var autoBatch = Convert.ToBoolean(autoBatchNo);
                        if (autoBatch && !useBatch) throw new UserFriendlyException(L("UseBatchNoMustTrueForAutoBatchNo") + $", Row = {i}");
                        if (autoBatch && batchNoFormula.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("AutoBatchNoMustHasBatchNoFormula") + $", Row = {i}");
                        if (!autoBatch && !batchNoFormula.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("AutoBatchNoMustTrueForBatchNoFormula") + $", Row = {i}");

                        BatchNoFormula formula = null;
                        if (useBatch && autoBatch && !string.IsNullOrWhiteSpace(batchNoFormula))
                        {
                            formula = batchNoFormulas.FirstOrDefault(s => s.Name == batchNoFormula);
                            if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $" Row = {i}");
                        }

                        var useSerial = string.IsNullOrWhiteSpace(TrackSerial) ? false : Convert.ToBoolean(TrackSerial);
                        var trackExpired = string.IsNullOrWhiteSpace(trackExpiration) ? false : Convert.ToBoolean(trackExpiration);

                        if (useSerial && useBatch) throw new UserFriendlyException(L("CannotUseBoth", L("BatchNo"), L("Serial")) + $", Row {i}");

                        find.UpdateUseBatch(userId, useSerial, useBatch, autoBatch, formula?.Id, trackExpired, barcode);
                        var minVaule = min != null ? Convert.ToDecimal(min) : 0;
                        var maxVaule = min != null ? Convert.ToDecimal(max) : 0;
                        find.SetMinMax(minVaule, maxVaule);
                        importItems.Add(find);

                    }


                    //select item lots
                    var lotWorkSeet = excelPackage.Workbook.Worksheets["DefaultZones"];
                    for (int s = 2; s <= lotWorkSeet.Dimension.End.Row; s++)
                    {
                        var ItemCode = lotWorkSeet.Cells[s, 1].Value?.ToString();
                        var LotName = lotWorkSeet.Cells[s, 3].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(ItemCode)) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $" Row = {s}"); ;
                        if (string.IsNullOrWhiteSpace(LotName)) throw new UserFriendlyException(L("PleaseEnter", L("LotName")) + $" Row = {s}");

                        var checkLot = lots.FirstOrDefault(t => t.LotName == LotName);
                        if (checkLot == null) throw new UserFriendlyException(L("IsNotValid", L("Lot")) + $" {LotName} Row = {s}");

                        var allItems = importItems;
                        var checkItem = allItems.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (checkItem == null) throw new UserFriendlyException(L("IsNotValid", L("Item")) + $" {ItemCode} Row = {s}");

                        //if exist in database
                        var find = itemLots.FirstOrDefault(t => t.ItemId == checkItem.Id && t.LotId == checkLot.Id);
                        if (find != null) excludeForItemLots.Add(new ItemLotOutputExport { ItemId = find.ItemId, LotId = checkLot.Id, Id = find.Id });
                        if (find != null) continue;

                        //must have only one Lot for one Location for each item
                        var allItemLosts = itemLots.Concat(importItemLots);
                        var checkDuppliate = allItemLosts.Where(t => t.Lot != null && t.ItemId == checkItem.Id && t.Lot.LocationId == checkLot.LocationId).FirstOrDefault();
                        if (checkDuppliate != null) throw new UserFriendlyException(L("Duplicated", L("Location")) + $" Row = {s}");
                        var itemLot = ItemLot.Create(tenantId, userId, checkItem.Id, checkLot.Id);
                        importItemLots.Add(itemLot);

                    }
                    var checkItems = importItems.Where(s => excludeForItemLots != null && !excludeForItemLots.Select(d => d.ItemId).ToList().Contains(s.Id)).ToList();

                    if (checkItems.Count > 0)
                    {
                        var delete = itemLots.Where(s => checkItems.Any(sb => sb.Id == s.ItemId)).ToList();
                        itemLotForDeletes.AddRange(delete);
                    }
                    var deleteOnZoneSheet = itemLots.Where(s => excludeForItemLots.Select(b => b.ItemId).ToList().Contains(s.ItemId) && !excludeForItemLots.Select(b => b.Id).ToList().Contains(s.Id)).ToList();
                    itemLotForDeletes.AddRange(deleteOnZoneSheet);


                }
            }
            //RemoveFile(input, _appFolders);


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    // Open and read the XlSX file.
                    await _itemRepository.BulkUpdateAsync(importItems);
                    await _itemLotRepository.BulkInsertAsync(importItemLots);
                    await _itemLotRepository.BulkDeleteAsync(itemLotForDeletes);
                }
                await uow.CompleteAsync();
            }

        }

        private ReportOutput GetExportEditPropertyTemplate()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsRequired = true,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ItemName",
                        ColumnLength = 300,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.Language,
                        SortOrder = 2,
                        Visible = true,
                        IsRequired = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    }
                },
                Groupby = "",
                HeaderTitle = "Items",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelEditPropertyTemplate(GetItemListInput input)
        {

            var popertyFilterList = input.PropertyDics;
            var tenantId = AbpSession.TenantId;
            var @itemData = new List<Item>();
            var itemProperties = new List<Property>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    @itemData = await _itemRepository.GetAll()
                                          .Include(s => s.Properties)
                                          .ThenInclude(s => s.PropertyValue)
                                          .Include(s => s.Properties)
                                          .ThenInclude(s => s.Property)
                                          .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                          .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                                          .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                                          popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                                          (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                                          popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                                          popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                                               .Count() == popertyFilterList.Count))

                                          .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                                          .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                                          .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                                          .WhereIf(
                                              !input.Filter.IsNullOrEmpty(),
                                              p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                   p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                   (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                                          ).AsNoTracking()
                                           .ToListAsync();
                   
                    itemProperties = await _propertyRepository.GetAll().AsNoTracking().Where(s => s.IsActive).ToListAsync();
                }

            }


            var result = new FileDto();
            var sheetName = "Items";
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
                var headerList = GetExportEditPropertyTemplate();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                foreach (var i in itemProperties)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.Name, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(100);
                    colHeaderTable += 1;
                }


                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var trueFalseList = new List<string> { "True", "False" };
                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "ItemCode")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ItemCode, true);
                        }
                        else if (h.ColumnName == "ItemName")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ItemName, true);
                        }
                        collumnCellBody += 1;
                    }

                    foreach (var h in itemProperties)
                    {
                        
                        var obj = i.Properties.FirstOrDefault(s => s.PropertyId == h.Id);
                        var value = obj?.PropertyValue?.Value;

                        AddTextToCell(ws, rowBody, collumnCellBody, value, true);

                        collumnCellBody += 1;
                    }

                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body             
  

                result.FileName = $"ItemEditPropertyTemplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelEditPropertyItem(FileDto input)
        {
            // Open and read the XlSX file.
            List<Item> items = new List<Item>();
            List<Property> properties = new List<Property>();
            List<PropertyValue> propertieValues = new List<PropertyValue>();

            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId.Value;
           
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    // Open and read the XlSX file.
                    @items = await _itemRepository.GetAll().Include(s => s.Properties).AsNoTracking().ToListAsync();
                    properties = await _propertyRepository.GetAll().AsNoTracking().Where(s => s.IsActive).ToListAsync();
                    propertieValues = await _propertyValueRepository.GetAll().AsNoTracking().ToListAsync();
                }
            }
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);


            List<ItemProperty> addItemProperties = new List<ItemProperty>();
            List<ItemProperty> updateItemProperties = new List<ItemProperty>();
            List<ItemProperty> deleteItemProperties = new List<ItemProperty>();

            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var ItemCode = worksheet.Cells[i, 1].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(ItemCode)) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $", Row {i}");

                        //if item ixist in database
                        var find = items.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (find == null) throw new UserFriendlyException(L("IsNotValid", L("ItemCode")) + $", Row {i}");

                        var col = 3;
                        foreach(var p in properties)
                        {
                            var val = worksheet.Cells[i, col].Value?.ToString();
                            if (p.IsRequired && val.IsNullOrEmpty()) throw new UserFriendlyException(L("PleaseEnter", p.Name + $", Row = {i}"));

                            if (!val.IsNullOrEmpty())
                            {
                                var value = propertieValues.FirstOrDefault(s => s.PropertyId == p.Id && s.Value == val);
                                if (value == null) throw new UserFriendlyException(L("IsNotValid", p.Name + $", Row = {i}"));

                                var itemProperty = find.Properties.FirstOrDefault(s => s.PropertyId == p.Id);
                                if(itemProperty == null)
                                {
                                    itemProperty = ItemProperty.Create(tenantId, userId, value.Id, p.Id, find.Id);
                                    addItemProperties.Add(itemProperty);
                                }
                                else
                                {
                                    itemProperty.Update(userId, value.Id, p.Id, find.Id);
                                    updateItemProperties.Add(itemProperty);
                                }
                            }
                            else
                            {
                                var itemProperty = find.Properties.FirstOrDefault(s => s.PropertyId == p.Id);
                                if (itemProperty != null)
                                {
                                    deleteItemProperties.Add(itemProperty);
                                }
                            }

                            col++;
                        }
                    }

                }
            }
         
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    // Open and read the XlSX file.
                    if (addItemProperties.Any()) await _itemPropertyRepository.BulkInsertAsync(addItemProperties);
                    if (updateItemProperties.Any()) await _itemPropertyRepository.BulkUpdateAsync(updateItemProperties);
                    if (deleteItemProperties.Any()) await _itemPropertyRepository.BulkDeleteAsync(deleteItemProperties);
                }
                await uow.CompleteAsync();
            }

        }

        #endregion


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_ExportPdf)]
        public async Task<FileDto> ExportPDF()
        {
            var tenantId = AbpSession.GetTenantId();

            var tenant = await GetCurrentTenantAsync();

            int digit = await _accountCycleRepository.GetAll()
                .Where(t => t.TenantId == tenant.Id && t.EndDate == null)
                .AsNoTracking()
                .OrderByDescending(t => t.StartDate)
                .Select(t => t.RoundingDigit)
                .FirstOrDefaultAsync();
            var input = new GetItemListInput();
            var items = await GetList(input);
            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "item.pdf",
                    FileToken = "item.html",
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

                //@todo replace our variable 
                exportHtml = exportHtml.Replace("{{companyName}}", tenant.LegalName);
                exportHtml = exportHtml.Replace("{{headerTitle}}", L("Items"));
                exportHtml = exportHtml.Replace("{{No}}", L("No"));
                exportHtml = exportHtml.Replace("{{ItemCode}}", L("ItemCode"));
                exportHtml = exportHtml.Replace("{{ItemType}}", L("ItemType"));
                exportHtml = exportHtml.Replace("{{SalePrice}}", L("SalePrice"));
                exportHtml = exportHtml.Replace("{{Status}}", L("Status"));
                exportHtml = exportHtml.Replace("{{PurchaseCost}}", L("CostPrice"));
                exportHtml = exportHtml.Replace("{{ItemName}}", L("ItemName"));
                var contentBody = string.Empty;
                var count = 1;

                foreach (var row in items.Items)
                {
                    var IsActive = "";
                    if (row.IsActive == true)
                    {
                        IsActive = L("Active");
                    }
                    else
                    {
                        IsActive = L("DisActive");
                    }

                    var rowItem = $"<tr>" +
                    $"<td> {count}</td>" +
                    $"<td>{row.ItemCode}</td>" +
                    $"<td>{row.ItemName}</td>" +
                    $"<td>{row.ItemType.Name}</td>" +
                    $"<td>{Math.Round(row.PurchaseCost != null ? row.PurchaseCost.Value : 0, digit, MidpointRounding.ToEven)}</td>" +
                    $"<td>{Math.Round(row.SalePrice != null ? row.SalePrice.Value : 0, digit, MidpointRounding.ToEven)}</td>" +
                    $"<td>{IsActive}</td>" +
                    $"</tr>";
                    contentBody += rowItem;
                    count++;
                }

                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{companyName}}", tenant.LegalName);

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                //var path = Path.Combine(AppFolders.TempFileDownloadFolder, tokenName);

                var result = new FileDto();
                result.FileName = $"Item_Report.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });


        }



        #region Export and Import Excel inventory package
        private ReportOutput GetItemNoAccountFeature()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ItemCode",
                        ColumnLength = 100,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 120,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        IsRequired = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemType",
                        ColumnLength = 200,
                        ColumnTitle = "Item Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        IsRequired = true,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PurchaseCost",
                        ColumnLength = 120,
                        ColumnTitle = "PurchaseCost",
                        ColumnType = ColumnType.Money,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SalePrice",
                        ColumnLength = 120,
                        ColumnTitle = "Sale Price",
                        ColumnType = ColumnType.Money,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },

                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 120,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ReorderPoint",
                        ColumnLength = 120,
                        ColumnTitle = "Reorder Point",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TrackSerial",
                        ColumnLength = 120,
                        ColumnTitle = "Track Serial",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShowSubitems",
                        ColumnLength = 120,
                        ColumnTitle = "Show Subitems",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Barcode",
                        ColumnLength = 120,
                        ColumnTitle = "Barcode",
                        ColumnType = ColumnType.String,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UseBatchNo",
                        ColumnLength = 120,
                        ColumnTitle = "UseBatchNo",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AutoBatchNo",
                        ColumnLength = 120,
                        ColumnTitle = "AutoBatchNo",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 17,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchNoFormula",
                        ColumnLength = 120,
                        ColumnTitle = "BatchNoFormula",
                        ColumnType = ColumnType.String,
                        SortOrder = 18,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    }
                    ,
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TrackExpiration",
                        ColumnLength = 120,
                        ColumnTitle = "Track Expiration",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 19,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BarcodeSameAsItemCode",
                        ColumnLength = 120,
                        ColumnTitle = "Barcode Same As Item Code",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 20,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                        new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Min",
                        ColumnLength = 120,
                        ColumnTitle = "Min",
                        ColumnType = ColumnType.Number,
                        SortOrder = 21,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                         new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Max",
                        ColumnLength = 120,
                        ColumnTitle = "Max",
                        ColumnType = ColumnType.Number,
                        SortOrder = 22,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Items",
                Sortby = "",
            };
            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelHasDefaultAccount(GetItemListInput input)
        {
            var tenantId = AbpSession.TenantId;
            var popertyFilterList = input.PropertyDics;
            var itemTypes = new List<string>();
            var taxs = new List<string>();
            var chartOfAccounts = new List<string>();
            var @itemData = new List<Item>();          
            var itemLots = new List<LotOutputExport>();
            var propertiesHeader = new List<Property>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    itemTypes = await _itemTypeRepository.GetAll().Select(t => t.Name).AsNoTracking().ToListAsync();
                    taxs = await _taxRepository.GetAll().Select(t => t.TaxName).AsNoTracking().ToListAsync();
                    chartOfAccounts = await _chartOfAccountRepository.GetAll().Select(t => t.AccountName).AsNoTracking().ToListAsync();
                    @itemData = await _itemRepository.GetAll()
                          .Include("Properties.Property")
                          .Include("Properties.PropertyValue")
                          .Include(u => u.ItemType)
                          .Include(u => u.SaleCurrency)
                          .Include(u => u.PurchaseCurrency)
                          .Include(u => u.BatchNoFormula)
                          .AsNoTracking()
                         .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                         .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                         .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                              .Count() == popertyFilterList.Count))

                         .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                         .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                         .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                         .WhereIf(
                             !input.Filter.IsNullOrEmpty(),
                             p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                  p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                  (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                         ).ToListAsync();                  
                    var itemIds = itemData.Select(s => s.Id).ToList();
                    itemLots = await _itemLotRepository.GetAll()
                                        .AsNoTracking()
                                        .WhereIf(itemIds.Count > 0, s => itemIds.Contains(s.ItemId))
                                        .Select(s => new LotOutputExport
                                        {
                                            ItemName = s.Item.ItemName,
                                            LotName = s.Lot.LotName,
                                            ItemId = s.ItemId,
                                            ItemCode = s.Item.ItemCode
                                        })
                                        .ToListAsync();

                    propertiesHeader = await _propertyRepository.GetAll().Where(t => t.IsActive == true).AsNoTracking().ToListAsync();
                }
            }


            var result = new FileDto();
            var sheetName = "Items";       
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
                var headerList = GetItemNoAccountFeature();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                int colHeaderPropertyTable = colHeaderTable;
                foreach (var i in propertiesHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderPropertyTable, i.Name, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(100);
                    headerList.ColumnInfo.Add(new CollumnOutput
                    {
                        ColumnName = i.Name,

                    });
                    colHeaderPropertyTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var trueFalseList = new List<string> { "True", "False" };
                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "ItemType")
                        {
                            AddDropdownList(ws, rowBody, collumnCellBody, itemTypes, i.ItemType.Name);
                        }
                        else if (h.ColumnName == "TrackSerial")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackSerial.ToString(), false);
                        }
                        else if (h.ColumnName == "TrackExpiration")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackExpiration.ToString(), false);

                        }
                        else if (h.ColumnName == "ShowSubitems")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ShowSubItems.ToString(), false);
                        }
                        else if (h.ColumnName == "UseBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.UseBatchNo.ToString(), false);
                        }
                        else if (h.ColumnName == "AutoBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.AutoBatchNo.ToString(), false);
                        }
                        else if (h.ColumnName == "BatchNoFormula")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.BatchNoFormula?.Name, true);
                        }
                        else if (h.ColumnName == "BarcodeSameAsItemCode")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.BarcodeSameAsItemCode.ToString(), false);
                        }
                        else
                        {
                            WriteBodyItems(ws, rowBody, collumnCellBody, h, i, count);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body                           
                //A workbook must have at least on cell, so lets add one... 
                var defaultLot = p.Workbook.Worksheets.Add("DefaultZones");
                defaultLot.PrinterSettings.Orientation = eOrientation.Landscape;
                defaultLot.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                defaultLot.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                defaultLot.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowDefaultLotTableHeader = 1;
                int colDefaultLotHeaderTable = 1;//start from row 1 of spreadsheet
                                                 // write header collumn table
                var headerDefaultLotsList = GetReportTemplateDefaultLots();

                foreach (var i in headerDefaultLotsList.ColumnInfo)
                {
                    AddTextToCell(defaultLot, rowDefaultLotTableHeader, colDefaultLotHeaderTable, i.ColumnTitle, true);

                    defaultLot.Column(colDefaultLotHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colDefaultLotHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowDefaultLotBody = rowDefaultLotTableHeader + 1;//start from row header of spreadsheet
                int countDefaultLot = 1;
                // write body
                foreach (var i in itemLots)
                {
                    int collumnDefaultLotCellBody = 1;
                    foreach (var h in headerDefaultLotsList.ColumnInfo)
                    {
                        if (h.ColumnType == ColumnType.String)
                        {
                            var value = i.GetType().GetProperty(h.ColumnName).GetValue(i, null);
                            AddTextToCell(defaultLot, rowDefaultLotBody, collumnDefaultLotCellBody, value?.ToString());
                        }

                        collumnDefaultLotCellBody += 1;
                    }
                    rowDefaultLotBody += 1;
                    countDefaultLot += 1;
                }

                #endregion Row Body    


                result.FileName = $"Item_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelHasDefaultAccountTemplate(GetItemListInput input)
        {
            var tenantId = AbpSession.TenantId;
            var popertyFilterList = input.PropertyDics;
            var itemTypes = new List<string>();
            var taxs = new List<string>();
            var chartOfAccounts = new List<string>();
            var @itemData = new List<Item>();           
            var itemLots = new List<LotOutputExport>();
            var propertiesHeader = new List<Property>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    itemTypes = await _itemTypeRepository.GetAll().Select(t => t.Name).AsNoTracking().ToListAsync();
                    taxs = await _taxRepository.GetAll().Select(t => t.TaxName).AsNoTracking().ToListAsync();
                    chartOfAccounts = await _chartOfAccountRepository.GetAll().Select(t => t.AccountName).AsNoTracking().ToListAsync();
                    @itemData = await _itemRepository.GetAll()
                          .Include("Properties.Property")
                          .Include("Properties.PropertyValue")
                          .Include(u => u.ItemType)
                          .Include(u => u.SaleCurrency)
                          .Include(u => u.PurchaseCurrency)
                          .Include(u => u.BatchNoFormula)
                          .AsNoTracking()
                         .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                         .WhereIf(input.ItemTypes != null && input.ItemTypes.Count > 0, p => input.ItemTypes.Contains(p.ItemTypeId))
                         .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                         (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                         popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                              .Count() == popertyFilterList.Count))

                         .WhereIf(input.PurchaseAccount != null && input.PurchaseAccount.Count > 0, p => input.PurchaseAccount.Contains(p.PurchaseAccountId))
                         .WhereIf(input.SaleAccount != null && input.SaleAccount.Count > 0, p => input.SaleAccount.Contains(p.SaleAccountId))
                         .WhereIf(input.InventoryAccount != null && input.InventoryAccount.Count > 0, p => input.InventoryAccount.Contains(p.InventoryAccountId))
                         .WhereIf(
                             !input.Filter.IsNullOrEmpty(),
                             p => p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                  p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                  (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower()))

                         ).Take(3).ToListAsync();                 
                    var itemIds = itemData.Select(s => s.Id).ToList();
                    itemLots = await _itemLotRepository.GetAll()
                                        .AsNoTracking()
                                        .WhereIf(itemIds.Count > 0, s => itemIds.Contains(s.ItemId))
                                        .Select(s => new LotOutputExport
                                        {
                                            ItemName = s.Item.ItemName,
                                            LotName = s.Lot.LotName,
                                            ItemId = s.ItemId,
                                            ItemCode = s.Item.ItemCode
                                        })
                                        .ToListAsync();

                    propertiesHeader = await _propertyRepository.GetAll().Where(t => t.IsActive == true).AsNoTracking().ToListAsync();
                }
            }


            var result = new FileDto();
            var sheetName = "Items";         
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
                var headerList = GetItemNoAccountFeature();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                int colHeaderPropertyTable = colHeaderTable;
                foreach (var i in propertiesHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderPropertyTable, i.Name, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(100);
                    headerList.ColumnInfo.Add(new CollumnOutput
                    {
                        ColumnName = i.Name,

                    });
                    colHeaderPropertyTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var trueFalseList = new List<string> { "True", "False" };
                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "ItemType")
                        {
                            AddDropdownList(ws, rowBody, collumnCellBody, itemTypes, i.ItemType.Name);
                        }
                        else if (h.ColumnName == "TrackSerial")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackSerial.ToString(), false);
                        }
                        else if (h.ColumnName == "TrackExpiration")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.TrackExpiration.ToString(), false);

                        }
                        else if (h.ColumnName == "ShowSubitems")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ShowSubItems.ToString(), false);
                        }
                        else if (h.ColumnName == "UseBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.UseBatchNo.ToString(), false);
                        }
                        else if (h.ColumnName == "AutoBatchNo")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.AutoBatchNo.ToString(), false);
                        }
                        else if (h.ColumnName == "BatchNoFormula")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.BatchNoFormula?.Name, true);
                        }
                        else if (h.ColumnName == "BarcodeSameAsItemCode")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.BarcodeSameAsItemCode.ToString(), false);
                        }
                        else
                        {
                            WriteBodyItems(ws, rowBody, collumnCellBody, h, i, count);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body                          
                //A workbook must have at least on cell, so lets add one... 
                var defaultLot = p.Workbook.Worksheets.Add("DefaultZones");
                defaultLot.PrinterSettings.Orientation = eOrientation.Landscape;
                defaultLot.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                defaultLot.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                defaultLot.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowDefaultLotTableHeader = 1;
                int colDefaultLotHeaderTable = 1;//start from row 1 of spreadsheet
                                                 // write header collumn table
                var headerDefaultLotsList = GetReportTemplateDefaultLots();

                foreach (var i in headerDefaultLotsList.ColumnInfo)
                {
                    AddTextToCell(defaultLot, rowDefaultLotTableHeader, colDefaultLotHeaderTable, i.ColumnTitle, true);

                    defaultLot.Column(colDefaultLotHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colDefaultLotHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowDefaultLotBody = rowDefaultLotTableHeader + 1;//start from row header of spreadsheet
                int countDefaultLot = 1;
                // write body
                foreach (var i in itemLots)
                {
                    int collumnDefaultLotCellBody = 1;
                    foreach (var h in headerDefaultLotsList.ColumnInfo)
                    {
                        if (h.ColumnType == ColumnType.String)
                        {
                            var value = i.GetType().GetProperty(h.ColumnName).GetValue(i, null);
                            AddTextToCell(defaultLot, rowDefaultLotBody, collumnDefaultLotCellBody, value?.ToString());
                        }

                        collumnDefaultLotCellBody += 1;
                    }
                    rowDefaultLotBody += 1;
                    countDefaultLot += 1;
                }

                #endregion Row Body    


                result.FileName = $"Item_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }




        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelHasDefaultAccount(FileDto input)
        {
            // Open and read the XlSX file.
            List<Item> items = new List<Item>();         
            List<ItemLot> itemLots = new List<ItemLot>();

            List<ItemType> itemTypes = new List<ItemType>();
            List<ChartOfAccount> chartOfAccounts = new List<ChartOfAccount>();
            List<Tax> taxs = new List<Tax>();
            List<Lot> lots = new List<Lot>();

            List<Property> property = new List<Property>();
            List<PropertyValue> propertyValue = new List<PropertyValue>();
            List<BatchNoFormula> batchNoFormulas = new List<BatchNoFormula>();
            Tenant tenant;
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId.Value;
            var formulaItemTypes = new List<ItemCodeFormulaItemType>();
            var formulaItemProperties = new List<ItemCodeFormulaProperty>();
            var Itemcodes = new List<string>();
            var formulaCustoms = new List<ItemCodeFormulaCustom>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    // Open and read the XlSX file.
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();                 
                    itemLots = await _itemLotRepository.GetAll().Include(s => s.Item).Include(s => s.Lot).AsNoTracking().ToListAsync();

                    @itemTypes = await _itemTypeRepository.GetAll().AsNoTracking().ToListAsync();
                    chartOfAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
                    @taxs = await _taxRepository.GetAll().AsNoTracking().AsNoTracking().ToListAsync();
                    lots = await _lotRepository.GetAll().AsNoTracking().ToListAsync();

                    @property = await _propertyRepository.GetAll().AsNoTracking().ToListAsync();
                    @propertyValue = await _propertyValueRepository.GetAll().AsNoTracking().ToListAsync();
                    batchNoFormulas = await _batchNoFormulaRepository.GetAll().AsNoTracking().ToListAsync();
                    tenant = await _tenantRepository.GetAll().AsNoTracking().Where(t => t.Id == tenantId).FirstOrDefaultAsync();
                    //formula queries
                    formulaItemTypes = await _itemCodeFormulaItemTypeRepository.GetAll().Include(u => u.ItemCodeFormula).AsNoTracking().ToListAsync();
                    formulaItemProperties = await _itemCodeFormulaPropertyRepository.GetAll().Include(u => u.Property).AsNoTracking().OrderBy(s => s.SortOrder).ToListAsync();
                    formulaCustoms = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking().ToListAsync();
                }
            }

            var itemCodes = items.Select(s => s.ItemCode).ToList();

            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var headerList = GetItemNoAccountFeature();
            var colIndex = headerList.ColumnInfo.Count();

            List<Item> importItems = new List<Item>();       
            List<ItemLot> importItemLots = new List<ItemLot>();
            var addItemProperties = new List<ItemProperty>();        
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {

                        var ItemName = worksheet.Cells[i, 2].Value?.ToString();
                        var ItemTypeName = worksheet.Cells[i, 3].Value?.ToString();
                        var inventoryAccountId = tenant.InventoryAccountId; // worksheet.Cells[i, 4].Value?.ToString();
                        var PurchaseCost = worksheet.Cells[i, 4].Value?.ToString();

                        var purchaseTaxId = tenant.TaxId; //worksheet.Cells[i, 7].Value?.ToString();
                        var SalePrice = worksheet.Cells[i, 5].Value?.ToString();
                        var saleAccountId = tenant.RevenueAccountId; //worksheet.Cells[i, 9].Value?.ToString();
                        var saleTaxId = tenant.TaxId;//worksheet.Cells[i, 10].Value?.ToString();
                        var Description = worksheet.Cells[i, 6].Value?.ToString();
                        var ReorderPoint = worksheet.Cells[i, 7].Value?.ToString();
                        var TrackSerial = worksheet.Cells[i, 8].Value?.ToString();
                        var showSubitems = worksheet.Cells[i, 9].Value?.ToString();
                        var barcode = worksheet.Cells[i, 10].Value?.ToString();
                        var useBatchNo = worksheet.Cells[i, 11].Value?.ToString();
                        var autoBatchNo = worksheet.Cells[i, 12].Value?.ToString();
                        var batchNoFormula = worksheet.Cells[i, 13].Value?.ToString();
                        var trackExpiration = worksheet.Cells[i, 14].Value?.ToString();
                        //to do
                        var barcodeSameAsItemCode = worksheet.Cells[i, 15].Value?.ToString();

                        var min = worksheet.Cells[i, 16].Value?.ToString();
                        var max = worksheet.Cells[i, 17].Value?.ToString();

                        var itemType = itemTypes.FirstOrDefault(s => s.Name == ItemTypeName.ToString());
                        if (itemType == null) throw new UserFriendlyException(L("IsNotValid", L("ItemType") + $" {ItemTypeName} Row = {i}"));
                        var itemTypeId = itemType.Id;

                        var inputApi = new GenerateItemCodeInput
                        {
                            ItemTypeId = itemTypeId,
                            TenantId = tenant.Id,
                            ItemProperties = new List<CreateOrItemPropertyInput>()
                        };

                        var formulaType = formulaItemTypes.Where(s => s.ItemTypeId == itemTypeId).FirstOrDefault();
                        var formulaCustom = formulaCustoms.Where(s => formulaType != null && s.ItemCodeFormulaId == formulaType.ItemCodeFormulaId).FirstOrDefault();
                        var formulaProperties = formulaItemProperties.Where(s => formulaType != null && s.ItemCodeFormulaId == formulaType.ItemCodeFormulaId).OrderBy(s => s.SortOrder).ToList();

                        var ItemCode = GenerateItemCode(inputApi, worksheet, i, colIndex, items, propertyValue, tenant, property, formulaType, formulaCustom, importItems, formulaProperties);

                        if (string.IsNullOrWhiteSpace(ItemCode)) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $", Row {i}");

                        var purchaseAccountId = itemTypes.Where(t => t.Id == itemTypeId && t.DisplayInventoryAccount).Any() ? tenant.COGSAccountId : tenant.ExpenseAccountId;  //worksheet.Cells[i, 6].Value?.ToString();                     
                        var useBatch = Convert.ToBoolean(useBatchNo);
                        var autoBatch = Convert.ToBoolean(autoBatchNo);
                        if (autoBatch && !useBatch) throw new UserFriendlyException(L("UseBatchNoMustTrueForAutoBatchNo") + $", Row = {i}");
                        if (autoBatch && batchNoFormula.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("AutoBatchNoMustHasBatchNoFormula") + $", Row = {i}");
                        if (!autoBatch && !batchNoFormula.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("AutoBatchNoMustTrueForBatchNoFormula") + $", Row = {i}");
                        if (!string.IsNullOrWhiteSpace(barcodeSameAsItemCode) && Convert.ToBoolean(barcodeSameAsItemCode) == true && !string.IsNullOrWhiteSpace(barcode))
                        {
                            throw new UserFriendlyException(L("BarcodeUserSameAsItemCodeThisFieldMaybeBlank") + " Row = " + i);
                        }
                        BatchNoFormula formula = null;
                        if (useBatch && autoBatch && !string.IsNullOrWhiteSpace(batchNoFormula))
                        {
                            formula = batchNoFormulas.FirstOrDefault(s => s.Name == batchNoFormula);
                            if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $" Row = {i}");
                        }

                        var useSerial = string.IsNullOrWhiteSpace(TrackSerial) ? false : Convert.ToBoolean(TrackSerial);

                        if (useSerial && useBatch) throw new UserFriendlyException(L("CannotUseBoth", L("BatchNo"), L("Serial")) + $", Row {i}");
                        if (purchaseAccountId == null) throw new UserFriendlyException(L("PurchaseAccountIsRequired") + $", Row {i}");
                        if (inventoryAccountId == null) throw new UserFriendlyException(L("InventoryIsRequired") + $", Row {i}");
                        if (saleAccountId == null) throw new UserFriendlyException(L("SaleAccountIsRequired") + $", Row {i}");
                        if (purchaseTaxId == null) throw new UserFriendlyException(L("TaxAccountIsRequired") + $", Row {i}");
                        if (saleTaxId == null) throw new UserFriendlyException(L("SaleTaxIsRequired") + $", Row {i}");


                        var @entity = Item.Create(tenantId, userId, ItemName, ItemCode,
                                        string.IsNullOrWhiteSpace(SalePrice) ? (decimal?)null : Convert.ToDecimal(SalePrice),
                                        string.IsNullOrWhiteSpace(PurchaseCost) ? (decimal?)null : Convert.ToDecimal(PurchaseCost),
                                        string.IsNullOrWhiteSpace(ReorderPoint) ? (decimal?)null : Convert.ToDecimal(ReorderPoint),
                                        useSerial,
                                        null,
                                        null, purchaseTaxId, saleTaxId, saleAccountId, purchaseAccountId,
                                        inventoryAccountId, itemTypeId, Description, barcode,
                                        string.IsNullOrWhiteSpace(useBatchNo) ? false : Convert.ToBoolean(useBatchNo),
                                        string.IsNullOrWhiteSpace(autoBatchNo) ? false : Convert.ToBoolean(autoBatchNo),
                                        formula?.Id,
                                        string.IsNullOrWhiteSpace(trackExpiration) ? false : Convert.ToBoolean(trackExpiration),
                                        string.IsNullOrWhiteSpace(barcodeSameAsItemCode) ? false : Convert.ToBoolean(barcodeSameAsItemCode));

                        @entity.UpdateMember(Member.All);
                        entity.SetShowSubItems(string.IsNullOrWhiteSpace(showSubitems) ? false : Convert.ToBoolean(showSubitems));
                        var minVaule = min != null ? Convert.ToDecimal(min) : 0;
                        var maxVaule = min != null ? Convert.ToDecimal(max) : 0;
                        entity.SetMinMax(minVaule, maxVaule);
                        // insert with property 
                        int po = 0;
                        foreach (var pr in property)
                        {
                            po++;
                            var index = worksheet.Cells[i, colIndex + po].Value?.ToString();

                            if (string.IsNullOrWhiteSpace(index) && pr.IsRequired) throw new UserFriendlyException(L("IsRequired", pr.Name));
                            if (!string.IsNullOrWhiteSpace(index))
                            {
                                var pro = propertyValue.FirstOrDefault(g => g.Value == index);
                                if (pro == null) throw new UserFriendlyException(L("IsNotValid", L("Property")) + $" {index} Row = {i}");

                                var p = @entity.AddProperty(userId, pro.PropertyId.Value, pro.Id);
                                addItemProperties.Add(p);
                            }

                        }

                        importItems.Add(entity);
                        itemCodes.Add(ItemCode);
                        var isDuplicated = itemCodes.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).Any();
                        if (isDuplicated) throw new UserFriendlyException(L("DuplicateItemCode", ItemCode));
                    }

                    //select item lots
                    var lotWorkSeet = excelPackage.Workbook.Worksheets["DefaultZones"];
                    for (int s = 2; s <= lotWorkSeet.Dimension.End.Row; s++)
                    {
                        var ItemCode = lotWorkSeet.Cells[s, 1].Value?.ToString();
                        var ItemName = lotWorkSeet.Cells[s, 2].Value?.ToString();
                        var LotName = lotWorkSeet.Cells[s, 3].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(ItemName) && tenant.ItemCodeSetting != ItemCodeSetting.Custom) throw new UserFriendlyException(L("PleaseEnter", L("ItemName")) + $" Row = {s}"); ;
                        if (string.IsNullOrWhiteSpace(ItemCode) && tenant.ItemCodeSetting == ItemCodeSetting.Custom) throw new UserFriendlyException(L("PleaseEnter", L("ItemCode")) + $" Row = {s}"); ;
                        if (string.IsNullOrWhiteSpace(LotName)) throw new UserFriendlyException(L("PleaseEnter", L("LotName")) + $" Row = {s}");

                        var checkLot = lots.FirstOrDefault(t => t.LotName == LotName);
                        if (checkLot == null) throw new UserFriendlyException(L("IsNotValid", L("Lot")) + $" {LotName} Row = {s}");

                        var allItems = importItems;
                        var checkItem = tenant.ItemCodeSetting != ItemCodeSetting.Custom ? allItems.FirstOrDefault(t => t.ItemName == ItemName) : allItems.FirstOrDefault(t => t.ItemCode == ItemCode);
                        if (checkItem == null && tenant.ItemCodeSetting != ItemCodeSetting.Custom) throw new UserFriendlyException(L("IsNotValid", L("Item")) + $" {ItemName} Row = {s}");
                        if (checkItem == null && tenant.ItemCodeSetting == ItemCodeSetting.Custom) throw new UserFriendlyException(L("IsNotValid", L("Item")) + $" {ItemCode} Row = {s}");
                        //if exist in database
                        var find = itemLots.FirstOrDefault(t => t.ItemId == checkItem.Id && t.LotId == checkLot.Id);
                        if (find != null) continue;

                        //must have only one Lot for one Location for each item
                        var allItemLosts = itemLots.Concat(importItemLots);
                        var checkDuppliate = allItemLosts.FirstOrDefault(t => t.ItemId == checkItem.Id && t.Lot.LocationId == checkLot.LocationId);
                        if (checkDuppliate != null) throw new UserFriendlyException(L("Duplicated", L("Location")) + $" Row = {s}");

                        var itemLot = ItemLot.Create(tenantId, userId, checkItem.Id, checkLot.Id);
                        itemLot.Lot = checkLot; //use to check Duplicate location
                        importItemLots.Add(itemLot);
                    }

                }
            }
            //RemoveFile(input, _appFolders);


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await this.CheckMaxItemCountAsync(tenantId.Value, importItems.Count);
                    // Open and read the XlSX file.
                    await _itemRepository.BulkInsertAsync(importItems);
                    if (addItemProperties.Any()) await _itemPropertyRepository.BulkInsertAsync(addItemProperties);                  
                    if (importItemLots.Any()) await _itemLotRepository.BulkInsertAsync(importItemLots);
                }
                await uow.CompleteAsync();
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Print_Barcode)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> PrintBarcode(GetListPrintBarcodeInput input)
        {
            var ids = input.Items.Select(t => t.Id).ToList();
            var tenantId = AbpSession.TenantId;
            var queries = new List<GetPrintBarcodeOutput>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    queries = await _itemRepository.GetAll().AsNoTracking().Where(i => ids.Contains(i.Id)).Select(i => new GetPrintBarcodeOutput
                    {
                        Barcode = i.Barcode,
                        Id = i.Id,
                        Qty = input.Items.Where(t => t.Id == i.Id).Select(t => t.Qty).FirstOrDefault()
                    }).ToListAsync();
                }
            }

            return await PrintTemplate(queries);
        }
        #endregion



        private async Task<FileDto> PrintTemplate(List<GetPrintBarcodeOutput> results)
        {

            var template = await _fileStorageManager.GetTemplate("printBarcodeTemplate.html");
            var exportHtml = EmbedBase64Fonts(template);
            var src = "https://barcode.tec-it.com/barcode.ashx?data";
            var alt = "Barcode Generator TEC-IT";

            return await Task.Run(async () =>
            {
                var html = @"<div style='text-align:center;'>                                       
                    <img alt = 'Barcode Generator TEC-IT' style = 'max-width:36mm; padding-top: 5px;'
                    src = 'https://barcode.tec-it.com/barcode.ashx?data=@barcode' />
                    </ div >";

                var printImage = "";

                foreach (var r in results)
                {
                    for (var i = 1; i <= r.Qty; i++)
                    {
                        printImage += html.Replace("@barcode", r.Barcode);
                    }
                }

                exportHtml = exportHtml.Replace("@imageBarcode", printImage);
                byte[] outPdfBuffer = await this.GetInitPDFBarcode(exportHtml);
                var result = new FileDto();
                result.FileName = $"PrintBarcode.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;
                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }


        private async Task CheckMaxItemCountAsync(int tenantId, int itemCount)
        {
            var maxItemCount = (await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxItemCount)).To<int>();
            if (maxItemCount <= 0)
            {
                return;
            }

            var currentUserCount = await _itemRepository.GetAll().AsNoTracking().Where(t => t.IsActive).CountAsync();
            if (currentUserCount + itemCount > maxItemCount)
            {
                throw new UserFriendlyException(L("MaximumItemCount_Error_Detail", maxItemCount));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Create)]
        public async Task<ValidationCountOutput> CheckMaxItemCount()
        {
            var maxItemCount = (await _featureChecker.GetValueAsync(AbpSession.TenantId.Value, AppFeatures.MaxItemCount)).To<int>();


            var currentUserCount = await _itemRepository.GetAll().AsNoTracking().Where(t => t.IsActive).CountAsync();

            return new ValidationCountOutput { MaxCurrentCount = currentUserCount, MaxCount = maxItemCount };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Create)]
        public async Task<GetCustomItemCodeSetting> CustomItemCode(CustomItemCodeInput input )
        {
            var result = await _tenantRepository.GetAll().AsNoTracking().Where(s => s.Id == AbpSession.TenantId)
                .Select(s => new GetCustomItemCodeSetting
                {
                    CustomCode = s.ItemCodeSetting == ItemCodeSetting.Custom,
                    Formula = s.ItemCodeSetting == ItemCodeSetting.Auto
                }).FirstOrDefaultAsync();

            if(result != null && result.Formula)
            {
                var useManual = await _itemCodeFormulaItemTypeRepository.GetAll().Include(u => u.ItemCodeFormula).AsNoTracking()
                               .Where(s => s.ItemTypeId == input.ItemTypeId.Value)
                               .Select(s => s.ItemCodeFormula.UseManual).FirstOrDefaultAsync();
                result.CustomCode = result.Formula && useManual ? true : false;
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Update)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<NullableIdDto<Guid>> UpdateAndResetItemCode(UpdateItemInput input)
        {
            input.IsGenerateItemCode = true;
            var result = await HelperUpdate(input);
            return result;
        }



        private async Task<NullableIdDto<Guid>> HelperUpdate(UpdateItemInput input)
        {
            if (input.TrackSerial && input.UseBatchNo) throw new UserFriendlyException(L("CannotUseBoth", L("BatchNo"), L("Serial")));
            if (input.AutoBatchNo && !input.UseBatchNo) throw new UserFriendlyException(L("UseBatchNoMustTrueForAutoBatchNo"));
            if (input.AutoBatchNo && !input.BatchNoFormulaId.HasValue) throw new UserFriendlyException(L("AutoBatchNoMustHasBatchNoFormula"));
            if (!input.AutoBatchNo && input.BatchNoFormulaId.HasValue) throw new UserFriendlyException(L("AutoBatchNoMustTrueForBatchNoFormula"));

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            Item entity = null;    
            var itemLots = new List<ItemLot>();
            var itemUserGroupEntity = new List<ItemsUserGroup>();
            var lots = new List<Lot>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    @entity = await _itemManager.GetAsync(input.Id);
                    if (entity == null)
                    {
                        throw new UserFriendlyException(L("RecordNotFound"));
                    }
                    if (input.IsGenerateItemCode)
                    {
                        var GenerateInput = new GenerateItemCodeInput
                        {
                            ItemCode = input.ItemCode,
                            TenantId = tenantId,
                            ItemProperties = input.Properties,
                            ItemTypeId = input.ItemTypeId,
                        };
                        input.ItemCode = await this.GetAutoItemCode(GenerateInput);
                        var @old = await _itemRepository.GetAll().AsNoTracking()
                        .Where(u => u.IsActive &&
                                    u.ItemCode.ToLower() == entity.ItemCode.ToLower() &&
                                      u.Id != entity.Id
                                    )
                        .FirstOrDefaultAsync();
                        if (old != null && old.ItemCode.ToLower() == entity.ItemCode.ToLower())
                        {
                            throw new UserFriendlyException(L("DuplicateItemCode", entity.ItemCode));

                        }
                    }               
                    itemLots = await _itemLotRepository.GetAll().Where(s => s.ItemId == input.Id).ToListAsync();
                    itemUserGroupEntity = await _itemUserGroupRepository.GetAll().Where(u => u.ItemId == entity.Id).ToListAsync();
                    lots = await _lotRepository.GetAll().AsNoTracking().ToListAsync();
                }

            }

            Guid? deleteImageId = null;
            if (entity.ImageId.HasValue && entity.ImageId != input.ImageId) deleteImageId = entity.ImageId;
            //todo
            entity.Update(userId, input.ItemName, input.ItemCode, input.SalePrice, input.PurchaseCost,
                input.ReorderPoint, input.TrackSerial, input.SaleCurrenyId,
                input.PurchaseCurrencyId, input.PurchaseTaxId, input.SaleTaxId, input.SaleAccountId,
                input.PurchaseAccountId, input.InventoryAccountId, input.ItemTypeId, input.Description,
                input.Barcode, input.UseBatchNo, input.AutoBatchNo, input.BatchNoFormulaId, input.TrackExpiration, input.BarcodeSameAsItemCode);
            @entity.UpdateMember(input.Member);
            entity.SetShowSubItems(input.ShowSubItems);
            entity.SetImage(input.ImageId);
            entity.SetMinMax(input.Min, input.Max);
            var updateItemProperties = new List<ItemProperty>();
            var addItemProperties = new List<ItemProperty>();

            foreach (var p in input.Properties)
            {
                if (p.Id != null)
                {
                    var property = entity.UpdateProperty(p.Id.Value, userId, p.PropertyId, p.PropertyValueId);
                    updateItemProperties.Add(property);
                }
                else if (p.Id == null)
                {
                    var property = @entity.AddProperty(userId, p.PropertyId, p.PropertyValueId);
                    addItemProperties.Add(property);
                }
            }

            #region update sub item
           

            #endregion

            #region update userGroup
            var updateItemUserGroups = new List<ItemsUserGroup>();
            var addItemUserGroups = new List<ItemsUserGroup>();
            var deleteItemGroups = new List<ItemsUserGroup>();

            if (input.Member == Member.UserGroup)
            {
                foreach (var c in input.UserGroups)
                {
                    if (c.Id != null)
                    {
                        var itemUser = itemUserGroupEntity.FirstOrDefault(u => u.Id == c.Id);
                        if (itemUser != null)
                        {
                            itemUser.Update(userId, entity.Id, c.UserGroupId);
                            updateItemUserGroups.Add(itemUser);
                        }
                    }
                    else if (c.Id == null)
                    {
                        var itemUser = ItemsUserGroup.Create(tenantId, userId, entity.Id, c.UserGroupId);
                        addItemUserGroups.Add(itemUser);
                    }
                }

                deleteItemGroups = itemUserGroupEntity.Where(u => !input.UserGroups.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            }
            else
            {
                deleteItemGroups = itemUserGroupEntity;
            }

            #endregion

            #region Item Lots
            var addItemLots = new List<ItemLot>();
            var deleteItemLots = new List<ItemLot>();

            if (input.DefaultLots != null && input.DefaultLots.Any())
            {
                //check Duplicate input
                var check = input.DefaultLots.GroupBy(s => s.Id).Where(s => s.Count() > 1).FirstOrDefault();
                if (check != null) throw new UserFriendlyException(L("Duplicated", L("Lot")));

                var checkUniqueByLocation = lots.Where(s => input.DefaultLots.Any(r => r.Id == s.Id))
                                                .GroupBy(s => s.LocationId)
                                                .Where(s => s.Count() > 1)
                                                .FirstOrDefault();
                if (checkUniqueByLocation != null) throw new UserFriendlyException(L("Duplicated", L("Location")));

                var addDefaultLots = input.DefaultLots.Where(s => !itemLots.Any(r => r.LotId == s.Id)).ToList();
                foreach (var lot in addDefaultLots)
                {
                    var find = lots.FirstOrDefault(s => s.Id == lot.Id);
                    if (find == null) throw new UserFriendlyException(L("RecordNotFound"));

                    var defaultLot = ItemLot.Create(tenantId, userId, entity.Id, lot.Id);
                    addItemLots.Add(defaultLot);
                }

                deleteItemLots = itemLots.Where(s => !input.DefaultLots.Any(r => r.Id == s.LotId)).ToList();

            }
            else if (itemLots.Any())
            {
                deleteItemLots = itemLots;
            }

            #endregion

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {                  

                    if (updateItemUserGroups.Any()) await _itemUserGroupRepository.BulkUpdateAsync(updateItemUserGroups);
                    if (addItemUserGroups.Any()) await _itemUserGroupRepository.BulkInsertAsync(addItemUserGroups);
                    if (deleteItemGroups.Any()) await _itemUserGroupRepository.BulkDeleteAsync(deleteItemGroups);

                    if (addItemLots.Any()) await _itemLotRepository.BulkInsertAsync(addItemLots);
                    if (deleteItemLots.Any()) await _itemLotRepository.BulkDeleteAsync(deleteItemLots);

                    if (updateItemProperties.Any()) await _itemPropertyRepository.BulkUpdateAsync(updateItemProperties);
                    if (addItemProperties.Any()) await _itemPropertyRepository.BulkInsertAsync(addItemProperties);

                    await _itemRepository.BulkUpdateAsync(new List<Item> { @entity });
                }
                await uow.CompleteAsync();
            }
            if (deleteImageId.HasValue) await _fileUploadManager.Delete(tenantId, deleteImageId.Value);

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }




        private string GenerateItemCode(GenerateItemCodeInput input, ExcelWorksheet worksheet, int rowIndex, int colIndex, List<Item> items, List<PropertyValue> propertyValue, Tenant tenant, List<Property> property, ItemCodeFormulaItemType formula, ItemCodeFormulaCustom formualCustom, List<Item> importItems, List<ItemCodeFormulaProperty> itemCodeFormulaProperties)
        {

            if(tenant.ItemCodeSetting == ItemCodeSetting.Custom) return worksheet.Cells[rowIndex, 1].Value?.ToString();

            if (formula == null) throw new UserFriendlyException(L("CannotFindItemCodeFormular"));

            if (formula.ItemCodeFormula.UseManual) return worksheet.Cells[rowIndex, 1].Value?.ToString();

            if (formula.ItemCodeFormula.UseCustomGenerate && !formula.ItemCodeFormula.UseItemProperty)
            {
                return GenerateCustomCode(formula, tenant, formualCustom, items, importItems);
            }
            else if (formula.ItemCodeFormula.UseItemProperty && !formula.ItemCodeFormula.UseCustomGenerate)
            {
                return GeneratePropertyCode(formula, tenant, worksheet, rowIndex, colIndex, propertyValue, items, property, itemCodeFormulaProperties);
            }
            else if (formula.ItemCodeFormula.UseItemProperty && formula.ItemCodeFormula.UseCustomGenerate)
            {
                return GenerateCustomPropertyCode(formula, tenant, worksheet, rowIndex, colIndex, propertyValue, items, property, itemCodeFormulaProperties, formualCustom, importItems);
            }

            return worksheet.Cells[rowIndex, 1].Value?.ToString();
        }

        private string GenerateCustomCode(ItemCodeFormulaItemType formular, Tenant tenant, ItemCodeFormulaCustom formulaCustom, List<Item> items, List<Item> importItems)
        {
            var prefix = formulaCustom.Prefix.IsNullOrEmpty() ? "" : formulaCustom.Prefix;

            var findlastCode = importItems.Any() ?
                importItems
                .Where(s => s.ItemCode.StartsWith(prefix) && s.ItemCode.Length == prefix.Length + formulaCustom.ItemCode.Length)
                .OrderByDescending(t => t.ItemCode)
                .Select(t => t.ItemCode)
                .FirstOrDefault() :
                items
                .Where(s => s.ItemCode.StartsWith(prefix) && s.ItemCode.Length == prefix.Length + formulaCustom.ItemCode.Length)
                .OrderByDescending(t => t.ItemCode)
                .Select(t => t.ItemCode)
                .FirstOrDefault();

            var result = $"{prefix}{formulaCustom.ItemCode}";

            if (!findlastCode.IsNullOrWhiteSpace())
            {
                var index = prefix.IsNullOrEmpty() ? findlastCode : findlastCode.Replace(prefix, "");
                var number = Convert.ToInt32(index) + 1;

                result = $"{prefix}{number.ToString().PadLeft(formulaCustom.ItemCode.Length, '0') }";

            }
            return result;
        }

        private string GeneratePropertyCode(ItemCodeFormulaItemType formular, Tenant tenant, ExcelWorksheet worksheet, int rowIndex, int colIndex, List<PropertyValue> propertyValue, List<Item> items, List<Property> property, List<ItemCodeFormulaProperty> formularItems)
        {
            var formularId = formular.Id;
            if (formularId == 0) throw new UserFriendlyException(L("CannotFindItemCodeFormular"));
            List<PropertyValueSummarryOutput> properties = new List<PropertyValueSummarryOutput>();
            int po1 = 0;
            foreach (var pr in property)
            {
                po1++;
                var index = worksheet.Cells[rowIndex, colIndex + po1].Value?.ToString();

                var pro = propertyValue.Where(g => g.PropertyId == pr.Id && g.Value == index).FirstOrDefault();
                if (pro != null && formularItems.Select(d => d.PropertyId).Contains(pr.Id))
                {
                    properties.Add(new PropertyValueSummarryOutput { Id = pro.Id, Code = pro.Code, PropertyId = pr.Id, ProperyName = pr.Name });
                }
            }

            if (formularItems.Count > properties.Count)
            {
                var propertyValues = formularItems.Where(s => !properties.Select(d => d.PropertyId).Contains(s.PropertyId)).ToList();
                var pNames = string.Join(",", propertyValues.Select(s => s.Property.Name));
                throw new UserFriendlyException(L("CannotGenerateItemCodeFormular") + $" {pNames} : Row = {rowIndex}");
            }

            var minValue = formularItems.Min(s => s.SortOrder);
            var lst = from f in formularItems
                      join p in properties on f.PropertyId equals p.PropertyId
                      orderby f.SortOrder
                      select new
                      {
                          Code = f.SortOrder == minValue ? p.Code : f.Separator + p.Code,
                          Order = f.SortOrder,
                          Separator = f.SortOrder == minValue ? "" : f.Separator,
                          OrginalCode = p.Code,
                      };

            var isEmptyCode = lst.Any(s => string.IsNullOrWhiteSpace(s.OrginalCode));
            if (isEmptyCode) throw new UserFriendlyException(L("PropertyValueCodeIsRequired"));
            var result = string.Join("", lst.Select(s => s.Code));
            return result;
        }

        private string GenerateCustomPropertyCode(ItemCodeFormulaItemType formular, Tenant tenant, ExcelWorksheet worksheet, int rowIndex,
            int colIndex, List<PropertyValue> propertyValue, List<Item> items, List<Property> property, List<ItemCodeFormulaProperty> formularItems, ItemCodeFormulaCustom formulaCustom, List<Item> importItems)
        {
            var formularId = formular.Id;

            if (formularId == 0) throw new UserFriendlyException(L("CannotFindItemCodeFormular"));

            List<PropertyValueSummarryOutput> properties = new List<PropertyValueSummarryOutput>();
            int po1 = 0;
            foreach (var pr in property)
            {
                po1++;
                var index = worksheet.Cells[rowIndex, colIndex + po1].Value?.ToString();

                var pro = propertyValue.Where(g => g.PropertyId == pr.Id && g.Value == index).FirstOrDefault();
                if (pro != null && formularItems.Select(d => d.PropertyId).Contains(pr.Id))
                {
                    properties.Add(new PropertyValueSummarryOutput { Id = pro.Id, Code = pro.Code, PropertyId = pr.Id, ProperyName = pr.Name });
                }
            }

            if (formularItems.Count > properties.Count)
            {
                var propertyValues = formularItems.Where(s => !properties.Select(d => d.PropertyId).Contains(s.PropertyId)).ToList();
                var pNames = string.Join(",", propertyValues.Select(s => s.Property.Name));
                throw new UserFriendlyException(L("CannotGenerateItemCodeFormular") + $" {pNames} : Row = {rowIndex}");
            }

            var minValue = formularItems.Min(s => s.SortOrder);
            var lst = from f in formularItems
                      join p in properties on f.PropertyId equals p.PropertyId
                      orderby f.SortOrder
                      select new
                      {
                          Code = f.SortOrder == minValue ? p.Code : f.Separator + p.Code,
                          Order = f.SortOrder,
                          Separator = f.SortOrder == minValue ? "" : f.Separator,
                          OrginalCode = p.Code,
                      };

            var isEmptyCode = lst.Any(s => string.IsNullOrWhiteSpace(s.OrginalCode));
            if (isEmptyCode) throw new UserFriendlyException(L("PropertyValueCodeIsRequired"));

            var separator = string.Join("", lst.Select(s => s.Code));
            var prefix = string.IsNullOrEmpty(formulaCustom.Prefix) ? separator : separator + "" + formulaCustom.Prefix;

            var result = $"{prefix}{formulaCustom.ItemCode}";

            var findlastCode = importItems.Any() ?
                importItems
                .Where(s => s.ItemCode.StartsWith(prefix) && s.ItemCode.Length == prefix.Length + formulaCustom.ItemCode.Length)
                .OrderByDescending(t => t.ItemCode)
                .Select(t => t.ItemCode)
                .FirstOrDefault() :
                items
                .Where(s => s.ItemCode.StartsWith(prefix) && s.ItemCode.Length == prefix.Length + formulaCustom.ItemCode.Length)
                .OrderByDescending(t => t.ItemCode)
                .Select(t => t.ItemCode)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(findlastCode))
            {
                var index = string.IsNullOrEmpty(prefix) ? findlastCode : findlastCode.Replace(prefix, "");
                var number = Convert.ToInt32(index) + 1;
                result = $"{prefix}{number.ToString().PadLeft(formulaCustom.ItemCode.Length, '0')}";
            }

            return result;
        }





    }
}
