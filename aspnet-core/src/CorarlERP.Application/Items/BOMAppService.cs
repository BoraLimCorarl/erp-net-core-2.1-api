using Abp.Application.Services.Dto;
using Abp.Authorization;
using CorarlERP.Boms;
using CorarlERP.Items.Dto;
using CorarlERP.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;
using Abp.UI;
using Abp.Extensions;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.ChartOfAccounts;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Currencies.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.UserGroups;
using CorarlERP.Dto;
using OfficeOpenXml;
using CorarlERP.FileStorages;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using Abp.Domain.Uow;

namespace CorarlERP.Items
{
    [AbpAuthorize]
    public class BOMAppService : ReportBaseClass, IBOMAppService
    {

        private readonly AppFolders _appFolders;
        private readonly ICorarlRepository<Item, Guid> _itemRepository;
        private readonly ICorarlRepository<Bom, Guid> _bomRepository;
        private readonly ICorarlRepository<BomItem, Guid> _bomItemRepository;
        private readonly ICorarlRepository<ItemLot, Guid> _itemLotRepository;
        private readonly ICorarlRepository<ItemsUserGroup, Guid> _itemUserGroupRepository;
        private readonly ICorarlRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public BOMAppService(
          AppFolders appFolders,
          ICorarlRepository<Bom, Guid> bomRepository,
          ICorarlRepository<BomItem, Guid> bomItemRepository,
          ICorarlRepository<Item, Guid> itemRepository,
          ICorarlRepository<ItemLot, Guid> itemLotRepository,
          ICorarlRepository<ItemsUserGroup, Guid> itemUserGroupRepository,
          ICorarlRepository<UserGroupMember, Guid> userGroupMemberRepository,
          IFileStorageManager fileStorageManager,
          IUnitOfWorkManager unitOfWorkManager
      ) : base(null, appFolders, null, null)
        {

            _appFolders = appFolders;
            _bomItemRepository = bomItemRepository;
            _bomRepository = bomRepository;
            _itemRepository = itemRepository;
            _itemLotRepository = itemLotRepository;
            _itemUserGroupRepository = itemUserGroupRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Create)]
        public async Task<Guid> Create(CreateBomInput input)
        {

            #region  check duplicate main item and sub item
            var duplicate = input.BOMItems.Where(s => s.ItemId == input.ItemId).Any();
            if (duplicate) throw new UserFriendlyException(L("DuplicateMainItemAndSubItem"));
            #endregion
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId;
            var entities = Bom.Create(tenantId, userId, input.Name, input.ItemId, input.IsDefault, input.Description, input.Qty);
            var bomItems = new List<BomItem>();
            foreach (var bi in input.BOMItems)
            {
                var bomItem = BomItem.Create(tenantId, userId, bi.ItemId, entities.Id, bi.Qty);
                bomItems.Add(bomItem);
            }
            await this.CheckDuplicateBOM(entities);
            await this.CheckDuplicateBOMDefault(entities);
            await _bomRepository.BulkInsertAsync(entities);
            await _bomItemRepository.BulkInsertAsync(bomItems);
            return entities.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var entities = await _bomRepository.GetAll().AsNoTracking().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            if (entities == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var bomItems = await _bomItemRepository.GetAll().AsNoTracking().Where(s => s.BomId == entities.Id).ToListAsync();
            await _bomItemRepository.BulkDeleteAsync(bomItems);
            await _bomRepository.BulkDeleteAsync(entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var entities = await _bomRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            if (entities == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entities.Disable();
            await _bomRepository.UpdateAsync(entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var entities = await _bomRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            if (entities == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entities.Enable();
            await _bomRepository.UpdateAsync(entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Find)]
        public async Task<PagedResultDto<GetBomOutput>> Find(GetListBomInput input)
        {

            var @query = _bomRepository
                 .GetAll()
                 .Include(u => u.Item)
                 .AsNoTracking()
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(input.ItemIds != null && input.ItemIds.Count > 0, p => input.ItemIds.Contains(p.ItemId))
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .Select(s => new GetBomOutput
                {
                    Description = s.Description,
                    Id = s.Id,
                    IsActive = s.IsActive,
                    IsDefault = s.IsDefault,
                    ItemCode = s.Item.ItemCode,
                    ItemId = s.Item.Id,
                    ItemName = s.Item.ItemName,
                    Name = s.Name
                }).ToListAsync();


            return new PagedResultDto<GetBomOutput>(resultCount, @entities);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_GetDetail)]
        public async Task<GetBomOutput> GetDetail(EntityDto<Guid> input)
        {
            var entities = await _bomRepository.GetAll().Include(u => u.Item).AsNoTracking().Where(s => s.Id == input.Id).
                Select(s => new GetBomOutput
                {
                    Description = s.Description,
                    Id = s.Id,
                    IsActive = s.IsActive,
                    IsDefault = s.IsDefault,
                    ItemCode = s.Item.ItemCode,
                    ItemId = s.Item.Id,
                    ItemName = s.Item.ItemName,
                    Name = s.Name,
                    Qty = s.Qty,
                }).FirstOrDefaultAsync();
            if (entities == null) throw new UserFriendlyException(L("RecordNotFound"));

            entities.BOMItems = await _bomItemRepository.GetAll().Include(u => u.Item).AsNoTracking()
                                  .Where(s => s.BomId == entities.Id).Select(s => new GetBomItemOutput
                                  {
                                      ItemCode = s.Item.ItemCode,
                                      ItemId = s.Item.Id,
                                      ItemName = s.Item.ItemName,
                                      Qty = s.Qty,
                                      BOMId = s.BomId,
                                      Id = s.Id,
                                  }).ToListAsync();

            return entities;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Update)]
        [HttpPost]
        public async Task<PagedResultDto<GetBomOutput>> GetList(GetListBomInput input)
        {
            var @query = _bomRepository
                 .GetAll()
                 .Include(u => u.Item)
                 .AsNoTracking()
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(input.ItemIds != null && input.ItemIds.Count > 0, p => input.ItemIds.Contains(p.ItemId))
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .Select(s => new GetBomOutput
                {
                    Description = s.Description,
                    Id = s.Id,
                    IsActive = s.IsActive,
                    IsDefault = s.IsDefault,
                    ItemCode = s.Item.ItemCode,
                    ItemId = s.Item.Id,
                    ItemName = s.Item.ItemName,
                    Name = s.Name,
                    Qty = s.Qty,
                }).ToListAsync();


            return new PagedResultDto<GetBomOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Update)]
        public async Task<Guid> Update(UpdateBomInput input)
        {
            #region  check duplicate main item and sub item
            var duplicate = input.BOMItems.Where(s => s.ItemId == input.ItemId).Any();
            if (duplicate) throw new UserFriendlyException(L("DuplicateMainItemAndSubItem"));
            #endregion
            var entities = await _bomRepository.GetAll().Include(u => u.Item).AsNoTracking().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            if (entities == null) throw new UserFriendlyException(L("RecordNotFound"));
            var userId = AbpSession.UserId.Value;
            var tenantId = AbpSession.TenantId.Value;
            entities.Update(userId, input.Name, input.ItemId, input.IsDefault, input.Description, input.Qty);
            await this.CheckDuplicateBOM(entities);
            await this.CheckDuplicateBOMDefault(entities);

            #region update sub item
            var updateBOMitems = new List<BomItem>();
            var addBOMitems = new List<BomItem>();
            var @subItems = await _bomItemRepository.GetAll().AsNoTracking().Where(s => s.BomId == entities.Id).ToListAsync();
            foreach (var s in input.BOMItems)
            {
                if (s.Id != null)
                {
                    var @subItem = @subItems.FirstOrDefault(u => u.Id == s.Id);
                    if (@subItem != null)
                    {
                        @subItem.Update(userId, s.ItemId, entities.Id, s.Qty);
                        updateBOMitems.Add(subItem);
                    }
                }
                else if (s.Id == null)
                {
                    var @subItem = BomItem.Create(tenantId, userId, s.ItemId, entities.Id, s.Qty);
                    addBOMitems.Add(subItem);
                }
            }
            var deleteBOMItems = @subItems.Where(u => !input.BOMItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            #endregion
            await _bomRepository.BulkUpdateAsync(entities);
            await _bomItemRepository.BulkInsertAsync(addBOMitems);
            await _bomItemRepository.BulkUpdateAsync(updateBOMitems);
            await _bomItemRepository.BulkDeleteAsync(deleteBOMItems);
            return entities.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        [HttpPost]
        public async Task<PagedResultDto<GetListItemForBOMOutput>> FindItemTypeItem(GetListItemForBOMInput input)
        {

            var popertyFilterList = input.PropertyDics;
            input.FilterType = input.FilterType == 0 ? FilterType.Contain : input.FilterType;
            var query = _itemRepository.GetAll().Include(u => u.ItemType)
                             .AsNoTracking()
                               .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                    (input.FilterType == FilterType.Contain && (
                                       p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower())))
                                    ) ||
                                    (input.FilterType == FilterType.StartWith && (
                                       p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower())))
                                    ) ||
                                    (input.FilterType == FilterType.Exact && (
                                       p.ItemName.ToLower().Equals(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Equals(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower())))
                                    )
                                   )
                             .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
                             .Where(s => s.ItemType.DisplayPurchase && s.ItemType.DisplayInventoryAccount &&
                             s.ItemType.DisplaySale && s.ItemType.DisplayReorderPoint && s.ItemType.DisplayTrackSerialNumber &&
                             s.ItemType.Name == "Item");

            var resultCount = await query.CountAsync();
            if (input.SelectedItemIds != null && input.SelectedItemIds.Any())
            {
                query = query.OrderBy(s => input.SelectedItemIds.Contains(s.Id) ? 0 : 1).ThenBy(input.Sorting);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }
            var @entities = await query
               .PageBy(input)
               .Select(s => new GetListItemForBOMOutput
               {
                   Id = s.Id,
                   ItemCode = s.ItemCode,
                   ItemName = s.ItemName,
                   ImageId = s.ImageId,
                   Barcode = s.Barcode,
               }).ToListAsync();
            return new PagedResultDto<GetListItemForBOMOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        [HttpPost]
        public async Task<PagedResultDto<GetListItemForBOMOutput>> FindItemForBOM(GetListItemForBOMInput input)
        {
            var popertyFilterList = input.PropertyDics;
            input.FilterType = input.FilterType == 0 ? FilterType.Contain : input.FilterType;
            var query = _itemRepository.GetAll().Include(u => u.ItemType)
                             .AsNoTracking()
                             .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                    (input.FilterType == FilterType.Contain && (
                                       p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower())))
                                    ) ||
                                    (input.FilterType == FilterType.StartWith && (
                                       p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower())))
                                    ) ||
                                    (input.FilterType == FilterType.Exact && (
                                       p.ItemName.ToLower().Equals(input.Filter.ToLower()) ||
                                       p.ItemCode.ToLower().Equals(input.Filter.ToLower()) ||
                                       (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower())))
                                    )
                                   )
                              .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
                             .Where(s => s.ItemType.Name == "Menu" || s.ItemType.Name == "Assembly" || s.ItemType.Name == "Item");

            var resultCount = await query.CountAsync();

            if (input.SelectedItemIds != null && input.SelectedItemIds.Any())
            {
                query = query.OrderBy(s => input.SelectedItemIds.Contains(s.Id) ? 0 : 1).ThenBy(input.Sorting);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }
            var @entities = await query
               .PageBy(input)
               .Select(s => new GetListItemForBOMOutput
               {
                   Id = s.Id,
                   ItemCode = s.ItemCode,
                   ItemName = s.ItemName,
                   ImageId = s.ImageId,
                   Barcode = s.Barcode,
               }).ToListAsync();
            return new PagedResultDto<GetListItemForBOMOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        [HttpPost]
        public async Task<PagedResultDto<GetListItemFromBOMOutput>> FindItemItmeTypeMenu(GetListItemForBOMInput input)
        {
            var userId = AbpSession.UserId.Value;
            input.FilterType = input.FilterType == 0 ? FilterType.Contain : input.FilterType;
            var popertyFilterList = input.PropertyDics;
            // Fetch items by filter location
            var groupItemQuery = from g in _itemUserGroupRepository.GetAll()
                                    .Include(s => s.UserGroup)
                                    .Include(s => s.Item)
                                    .Where(t => t.Item.Member == Member.UserGroup)
                                   .Where(x => x.UserGroup.LocationId != null)
                                   .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                                           t => t.UserGroup != null && input.Locations.Contains(t.UserGroup.LocationId.Value))
                                   .AsNoTracking()
                                 join u in _userGroupMemberRepository.GetAll()
                                 .Where(s => s.MemberId == userId)
                                 .AsNoTracking()
                                 on g.UserGroupId equals u.UserGroupId
                                 group g by g.Item into i
                                 select i.Key;

            var items = from item in _itemRepository.GetAll()
                                 .AsNoTracking()
                                 .Include(u => u.ItemType)
                                 .Where(s => s.ItemType.DisplayItemMenu)
                                 .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null || popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 || popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value))).Count() == popertyFilterList.Count))
                                 .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                  (input.FilterType == FilterType.Contain && (
                                     p.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                     (p.Barcode != null && p.Barcode.ToLower().Contains(input.Filter.ToLower())))
                                  ) ||
                                  (input.FilterType == FilterType.StartWith && (
                                     p.ItemName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                     p.ItemCode.ToLower().StartsWith(input.Filter.ToLower()) ||
                                     (p.Barcode != null && p.Barcode.ToLower().StartsWith(input.Filter.ToLower())))
                                  ) ||
                                  (input.FilterType == FilterType.Exact && (
                                     p.ItemName.ToLower().Equals(input.Filter.ToLower()) ||
                                     p.ItemCode.ToLower().Equals(input.Filter.ToLower()) ||
                                     (p.Barcode != null && p.Barcode.ToLower().Equals(input.Filter.ToLower())))
                                  )
                                 )
                        join g in groupItemQuery on item.Id equals g.Id into ig
                        from itemGroup in ig.DefaultIfEmpty()
                        where item.Member == Member.All || itemGroup != null
                        select item;

            // Fetch items that have DisplayItemMenu set to true and join with BOMs
            var itemsWithBOM = from item in items
                               join bom in _bomRepository.GetAll().Where(s => s.IsActive && s.IsDefault)
                                   .AsNoTracking() on item.Id equals bom.ItemId
                               into bomGroup
                               from d in bomGroup.DefaultIfEmpty()
                               select new GetListItemFromBOMOutput
                               {
                                   BomId = d != null ? d.Id : (Guid?)null,
                                   BomName = d != null ? d.Name : null,
                                   Id = item.Id,
                                   ItemCode = item.ItemCode,
                                   ItemName = item.ItemName,
                                   ImageId = item.ImageId,
                                   Barcode = item.Barcode,
                                   SaleTaxId = item.SaleTaxId,

                               };

            // Group by Item ID and select distinct items with their associated BOM details
            var groupedQuery = itemsWithBOM
                .GroupBy(s => s.Id)
                .Select(group => new GetListItemFromBOMOutput
                {
                    BomId = group.Where(b => b.BomId != null).Select(sb => sb.BomId).FirstOrDefault(),
                    BomName = group.Where(b => b.BomId != null).Select(sb => sb.BomName).FirstOrDefault(),
                    Id = group.Key,
                    ItemCode = group.FirstOrDefault().ItemCode,
                    ItemName = group.FirstOrDefault().ItemName,
                    ImageId = group.FirstOrDefault().ImageId,
                    Barcode = group.FirstOrDefault().Barcode,
                    SaleTaxId = group.FirstOrDefault().SaleTaxId,

                });



            // Count total results for pagination
            var totalCount = await groupedQuery.Select(s => s.Id).CountAsync();

            // Apply sorting, pagination, and fetch the result list
            var paginatedItems = await groupedQuery
                .OrderBy(input.Sorting ?? "ItemCode") // Default sorting if input.Sorting is null
                .PageBy(input)
                .ToListAsync();

            // Return paginated results with total count
            return new PagedResultDto<GetListItemFromBOMOutput>(totalCount, paginatedItems);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        [HttpPost]
        public async Task<List<GetBomItemOutput>> GetBomItemByBomId(BomItemInput input)
        {
            var itemQuery = _bomItemRepository.GetAll()
                .AsNoTracking()
                .Include(u => u.Item)
                .Include(u => u.BOM)
                .Where(s => s.BomId == input.BomId)
                .Select(s => new
                {
                    Id = s.Id,
                    ItemId = s.ItemId,
                    Qty = s.Qty,
                    InventoryAccountId = s.Item.InventoryAccountId,
                    PurchaseAccountId = s.Item.PurchaseAccountId,
                    ItemCode = s.Item.ItemCode,
                    ItemName = s.Item.ItemName,
                    PurchaseCurrencyId = s.Item.PurchaseCurrencyId,
                    PurchaseTaxId = s.Item.PurchaseTaxId,
                    SaleAccountId = s.Item.SaleAccountId,
                    SaleCurrenyId = s.Item.SaleCurrenyId,
                    SaleTaxId = s.Item.SaleTaxId,
                    BomId = s.BomId,
                    BomName = s.BomId != null ? s.BOM.Name : null,
                });
            var itemLots = from il in _itemLotRepository.GetAll()
                        .Include(u => u.Lot)
                        .WhereIf(input.Locations != null && input.Locations.Count > 0,
                         t => t.Lot != null && input.Locations.Contains(t.Lot.LocationId)).AsNoTracking()
                           select il;

            var query = await (from s in itemQuery
                               join il in itemLots on s.ItemId equals il.ItemId
                                    into u
                               from d in u.DefaultIfEmpty()
                               select new GetBomItemOutput
                               {
                                   Id = s.Id,
                                   ItemId = s.ItemId,
                                   Qty = s.Qty,
                                   InventoryAccountId = s.InventoryAccountId,
                                   PurchaseAccountId = s.PurchaseAccountId,
                                   ItemCode = s.ItemCode,
                                   ItemName = s.ItemName,
                                   PurchaseCurrencyId = s.PurchaseCurrencyId,
                                   PurchaseTaxId = s.PurchaseTaxId,
                                   SaleAccountId = s.SaleAccountId,
                                   SaleCurrenyId = s.SaleCurrenyId,
                                   SaleTaxId = s.SaleTaxId,
                                   BOMId = s.BomId,
                                   LotId = d != null ? d.LotId : (long?)null,
                                   LotName = d != null && d.Lot != null ? d.Lot.LotName : null,
                               }).ToListAsync();
            return query;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Export)]
        [UnitOfWork(IsDisabled = true)]
        [HttpPost]
        public async Task<FileDto> ExportExcel(GetListBomExcelInput input)
        {

            var tenantId = AbpSession.TenantId.Value;
            var boms = new List<GetBomOutput>();
            var bomItems = new List<GetBomItemExcel>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    boms = await _bomRepository
                                   .GetAll()
                                   .AsNoTracking()
                                   .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                   .WhereIf(input.ItemIds != null && input.ItemIds.Count > 0, p => input.ItemIds.Contains(p.ItemId))
                                   .WhereIf(
                                       !input.Filter.IsNullOrEmpty(),
                                       p => p.Name.ToLower().Contains(input.Filter.ToLower())).Select(s => new GetBomOutput
                                       {
                                           Description = s.Description,
                                           Id = s.Id,
                                           IsActive = s.IsActive,
                                           IsDefault = s.IsDefault,
                                           ItemCode = s.Item.ItemCode,
                                           ItemId = s.Item.Id,
                                           ItemName = s.Item.ItemName,
                                           Name = s.Name,
                                           Qty = s.Qty,
                                       }).ToListAsync();
                    var bomIds = boms.Select(s => s.Id).ToList();
                    bomItems = await _bomItemRepository.GetAll()
                        .Include(u => u.BOM.Item)
                        .AsNoTracking()
                        .Where(s => bomIds != null && bomIds.Count > 0 && bomIds.Contains(s.BomId)).Select(bi => new GetBomItemExcel
                        {
                            BOMId = bi.BomId,
                            ItemCode = bi.Item.ItemCode,
                            Qty = bi.Qty,
                            BomName = bi.BOM.Name,
                            Id = bi.Id,
                            ItemId = bi.ItemId,
                            ItemName = bi.Item.ItemName,
                            MainOutputItemCode = bi.BOM.Item.ItemCode
                        }).ToListAsync();

                }
            }

            var result = new FileDto();
            var sheetName = "BOM";
            var sheetSubItem = "BOMItem";
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet


                var wsSub = p.Workbook.Worksheets.Add(sheetSubItem);
                wsSub.PrinterSettings.Orientation = eOrientation.Landscape;
                wsSub.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                wsSub.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                wsSub.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet


                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateItem();
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1
                #region header sub item 
                int rowSubTableHeader = 1;
                int colSubHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerSubList = GetReportTemplateSubItem();
                var reportCollumnSubHeader = headerSubList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnSubHeader)
                {
                    AddTextToCell(wsSub, rowSubTableHeader, colSubHeaderTable, i.ColumnTitle, true);
                    wsSub.Column(colSubHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colSubHeaderTable += 1;
                }
                #endregion

                #region write body item 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                foreach (var i in boms)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "Name")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.Name.ToString(), false);
                        }
                        else if (h.ColumnName == "Default")
                        {
                            var isDefault = i.IsDefault ? "True" : "False";
                            AddTextToCell(ws, rowBody, collumnCellBody, isDefault, false);
                        }
                        else if (h.ColumnName == "Description")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.Description, false);
                        }

                        else if (h.ColumnName == "MainOutputItem")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.ItemCode, false);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion

                #region write body item 
                int rowSubBody = rowSubTableHeader + 1;//start from row header of spreadsheet
                int countSub = 1;
                foreach (var i in bomItems)
                {
                    int collumnCellBodySubItem = 1;
                    foreach (var h in headerSubList.ColumnInfo)
                    {
                        if (h.ColumnName == "BomName")
                        {
                            AddTextToCell(wsSub, rowSubBody, collumnCellBodySubItem, i.BomName.ToString(), false);
                        }
                        else if (h.ColumnName == "ItemCode")
                        {
                            AddTextToCell(wsSub, rowSubBody, collumnCellBodySubItem, i.ItemCode.ToString(), false);
                        }
                        else if (h.ColumnName == "Qty")
                        {
                            AddTextToCell(wsSub, rowSubBody, collumnCellBodySubItem, i.Qty.ToString(), false);
                        }
                        else if (h.ColumnName == "MainOutputItem")
                        {
                            AddTextToCell(wsSub, rowSubBody, collumnCellBodySubItem, i.MainOutputItemCode, false);
                        }
                        collumnCellBodySubItem += 1;
                    }
                    rowSubBody += 1;
                    countSub += 1;
                }
                #endregion
                result.FileName = $"BOMTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_BOMs_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;
            var items = new List<ItemSummaryWithAccount>();
            var itemCodes = new List<string>();
            var listBom = new List<Bom>();
            var listBomItem = new List<BomItem>();
            var Boms = new List<Bom>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    items = await _itemRepository.GetAll().AsNoTracking()
                                        .Select(s => new ItemSummaryWithAccount
                                        {
                                            Id = s.Id,
                                            ItemCode = s.ItemCode,
                                            ItemName = s.ItemName,
                                            PurchaseAccountId = s.PurchaseAccountId,
                                            PurchaseTaxId = s.PurchaseTaxId,
                                            SaleAccountId = s.SaleAccountId,
                                            SaleTaxId = s.SaleTaxId,
                                            InventoryAccountId = s.InventoryAccountId,
                                            ItemTypeId = s.ItemTypeId,
                                            ItemTypeName = s.ItemType.Name
                                        }).ToListAsync();
                    Boms = await _bomRepository.GetAll().AsNoTracking().ToListAsync();

                }
            }


            itemCodes = items.Select(s => s.ItemCode).ToList();
            var excelPackage = await Read(input);
            var headerList = GetReportTemplateItem();
            var colIndex = headerList.ColumnInfo.Count();


            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets["BOM"];

                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var name = worksheet.Cells[i, 1].Value?.ToString();
                        var ItemCode = worksheet.Cells[i, 2].Value?.ToString();
                        var Default = worksheet.Cells[i, 3].Value?.ToString();
                        var Description = worksheet.Cells[i, 4].Value?.ToString();
                        var isDefault = Default == "True" ? true : false;
                        if (string.IsNullOrWhiteSpace(name)) throw new UserFriendlyException(L("BOMNameIsRequired") + $" {name} Row {i}");
                        var itemId = items
                            .Where(s => s.ItemTypeName == "Menu" || s.ItemTypeName == "Assembly" || s.ItemTypeName == "Item")
                            .Where(s => s.ItemCode == ItemCode).Select(s => s.Id).FirstOrDefault();
                        if (itemId == null) throw new UserFriendlyException(L("MainItemIsRequired") + $" {ItemCode} Row {i}");
                        var @entity = Bom.Create(tenantId, userId, name, itemId, isDefault, Description, 1);
                        var @old = Boms.Where(u => u.IsActive && u.ItemId == entity.ItemId && u.Name.ToLower() == entity.Name.ToLower() && u.Id != entity.Id).FirstOrDefault();
                        var duplicateName = listBom.Where(u => u.IsActive && u.ItemId == entity.ItemId && u.Name.ToLower() == entity.Name.ToLower() && u.Id != entity.Id).Any();
                        if (old != null || duplicateName)
                        {
                            throw new UserFriendlyException(L("DuplicateBOMName", entity.Name));
                        }
                        var @oldDefault = Boms.Where(u => u.IsActive && u.ItemId == entity.ItemId && entity.IsDefault && u.IsDefault && u.Id != entity.Id).FirstOrDefault();
                        var @oldDefaultItem = Boms.Where(u => u.IsActive && u.ItemId == entity.ItemId && entity.IsDefault && u.IsDefault && u.Id != entity.Id).Any();

                        if (@oldDefault != null || @oldDefaultItem)
                        {
                            throw new UserFriendlyException(L("DuplicateBOMDefaultValue") + $" {entity.Name}");
                        }

                        listBom.Add(entity);
                    }

                    //select item lots
                    var lotWorkSeet = excelPackage.Workbook.Worksheets["BOMItem"];
                    for (int s = 2; s <= lotWorkSeet.Dimension.End.Row; s++)
                    {
                        var BomName = lotWorkSeet.Cells[s, 1].Value?.ToString();
                        var mainOutputItemCode = lotWorkSeet.Cells[s, 2].Value?.ToString();
                        var ItemCode = lotWorkSeet.Cells[s, 3].Value?.ToString();
                        var Qty = lotWorkSeet.Cells[s, 4].Value?.ToString();
                        var mainItemCodeId = items.Where(i => i.ItemCode == mainOutputItemCode).Select(b => b.Id).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(mainOutputItemCode)) throw new UserFriendlyException(L("MainOutputItemCodeIsRequired") + $" Row {s}");
                        var bom = listBom.Where(b => b.Name == BomName && b.ItemId == mainItemCodeId).Select(b => b).FirstOrDefault();
                        if (bom == null) throw new UserFriendlyException(L("CanNotBOMNameInSheetBom") + $" Row {s}");
                        if (string.IsNullOrWhiteSpace(BomName)) throw new UserFriendlyException(L("BOMNameIsRequired") + $" Row {s}");
                        if (string.IsNullOrWhiteSpace(Qty)) throw new UserFriendlyException(L("QtyIsRequired") + $" Row {s}");
                        var itemId = items
                           .Where(i => i.ItemTypeName == "Item")
                           .Where(i => i.ItemCode == ItemCode).Select(i => i.Id).FirstOrDefault();
                        if (itemId == null) throw new UserFriendlyException(L("InputComponentCodeIsRequired") + $" Row {s}");

                        if (bom.ItemId == itemId) throw new UserFriendlyException(L("DuplicateMainItemAndSubItem") + $" Row {s}");
                        var UpdateQty = listBomItem.Where(bs => bs.BomId == bom.Id && bs.ItemId == itemId).FirstOrDefault();
                        if (UpdateQty != null)
                        {
                            var total = UpdateQty.Qty + Convert.ToDecimal(Qty);
                            UpdateQty.SetQty(total);
                        }
                        else
                        {
                            var bomItem = BomItem.Create(tenantId, userId, itemId, bom.Id, Convert.ToDecimal(Qty));
                            listBomItem.Add(bomItem);
                        }




                    }

                    var bomIdFromBomItems = listBomItem.Select(s => s.BomId).ToList();
                    var bomNoSubItems = listBom.Where(b => !bomIdFromBomItems.Contains(b.Id)).Select(b => new
                    {
                        BomName = b.Name,
                        MainOutputItemCode = items.Where(s => s.Id == b.ItemId).Select(s => s.ItemCode).FirstOrDefault()
                    }).FirstOrDefault();
                    if (bomNoSubItems != null)
                    {
                        throw new UserFriendlyException(L("MainOutputItemCodeIsRequired") + $" {bomNoSubItems.MainOutputItemCode}  Sheet BOMItem");
                    }

                }
            }

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _bomRepository.BulkInsertAsync(listBom);
                    await _bomItemRepository.BulkInsertAsync(listBomItem);
                }
                await uow.CompleteAsync();
            }

        }


        #region helper 

        private ReportOutput GetReportTemplateItem()
        {
            var columns = new List<CollumnOutput>()
            {
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Name",
                    ColumnLength = 180,
                    ColumnTitle = "BOM Name",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },

                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "MainOutputItem",
                    ColumnLength = 250,
                    ColumnTitle = "Main Out put Item Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Default",
                    ColumnLength = 250,
                    ColumnTitle = "Default",
                    ColumnType = ColumnType.Bool,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Description",
                    ColumnLength = 250,
                    ColumnTitle = "Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

            };
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.ToList(),
                Groupby = "",
                HeaderTitle = "BOM",
                Sortby = "",
            };

            return result;
        }


        private ReportOutput GetReportTemplateSubItem()
        {
            var columns = new List<CollumnOutput>()
            {
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "BomName",
                    ColumnLength = 250,
                    ColumnTitle = "BOM Name",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "MainOutputItem",
                    ColumnLength = 250,
                    ColumnTitle = "Main Out put Item Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemCode",
                    ColumnLength = 250,
                    ColumnTitle = "Input Component Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Qty",
                    ColumnLength = 250,
                    ColumnTitle = "Qty",
                    ColumnType = ColumnType.Number,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

            };
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.ToList(),
                Groupby = "",
                HeaderTitle = "BOM",
                Sortby = "",
            };

            return result;
        }


        private async Task CheckDuplicateBOM(Bom @entity)
        {
            var @old = await _bomRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.ItemId == entity.ItemId &&
                                       u.Name.ToLower() == entity.Name.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateBOMName", entity.Name));
            }
        }

        private async Task CheckDuplicateBOMDefault(Bom @entity)
        {
            var @old = await _bomRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.ItemId == entity.ItemId &&
                                       entity.IsDefault && u.IsDefault &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateBOMDefaultValue"));
            }
        }

        #endregion
    }
}
