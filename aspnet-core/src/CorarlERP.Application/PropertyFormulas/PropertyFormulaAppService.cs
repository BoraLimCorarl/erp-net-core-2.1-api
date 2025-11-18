using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using CorarlERP.PropertyFormulas.Dto;
using CorarlERP.Reports;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Authorization;
using CorarlERP.MultiTenancy;

namespace CorarlERP.PropertyFormulas
{
    [AbpAuthorize]
    public class PropertyFormulaAppService : ReportBaseClass, IPropertyFormulaAppService
    {

        private readonly ICorarlRepository<ItemCodeFormula, long> _itemCodeFormulaRepository;
        private readonly ICorarlRepository<ItemCodeFormulaProperty, long> _itemCodeFormulaPropertyRepository;
        private readonly IItemCodeFormulaManager _iItemCodeFormulaManager;
        private readonly ICorarlRepository<ItemCodeFormulaItemType, long> _itemCodeFormulaItemTypeRepository;
        private readonly ICorarlRepository<Tenant, int> _tenantRepository;
        private readonly AppFolders _appFolders;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<ItemCodeFormulaCustom, long> _itemCodeFormulaCustomRepository;
        public PropertyFormulaAppService(
                                  AppFolders appFolders,
                                  ICorarlRepository<ItemCodeFormula, long> itemCodeFormulaRepository,
                                  ICorarlRepository<ItemCodeFormulaProperty, long> itemCodeFormulaPropertyRepository,
                                  IUnitOfWorkManager unitOfWorkManager,
                                  IItemCodeFormulaManager iItemCodeFormulaManager,
                                  ICorarlRepository<ItemCodeFormulaItemType, long> itemCodeFormulaItemTypeRepository,
                                  ICorarlRepository<ItemCodeFormulaCustom, long> itemCodeFormulaCustomRepository,
                                  ICorarlRepository<Tenant, int> tenantRepository)
            : base(null, appFolders, null, null)
        {
            _itemCodeFormulaRepository = itemCodeFormulaRepository;
            _itemCodeFormulaPropertyRepository = itemCodeFormulaPropertyRepository;
            _appFolders = appFolders;
            _unitOfWorkManager = unitOfWorkManager;
            _iItemCodeFormulaManager = iItemCodeFormulaManager;
            _itemCodeFormulaItemTypeRepository = itemCodeFormulaItemTypeRepository;
            _tenantRepository = tenantRepository;
            _itemCodeFormulaCustomRepository = itemCodeFormulaCustomRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_Create)]
        public async Task<long> Create(CreateOrUpdatePropertyFormulaInput input)
        {
            var duplicateProperty = input.Items.GroupBy(x => x.PropertyId).Where(g => g.Count() > 1).Any();
            if (duplicateProperty)
            {
                throw new UserFriendlyException(L("DuplicateProperty"));
            }
            var duplicateSortOrder = input.Items.GroupBy(x => x.SortOrder).Where(g => g.Count() > 1).Any();
            if (duplicateSortOrder)
            {
                throw new UserFriendlyException(L("DuplicateSortOrder"));
            }
            if(!input.UseCustomGenerate && !input.UseItemProperty && !input.UseManual)
            {
                throw new UserFriendlyException(L("CheckCustomGenerateOrUseItemPropertyIsRequired"));
            }
            if (input.ItemCodeFormulaItemTypes.Count <= 0) throw new UserFriendlyException(L("ItemTypeIsRequired"));
            if (input.Items.Count <= 0 && input.UseItemProperty) throw new UserFriendlyException(L("PropetyListIsRequired"));
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId;
            // check Duplicate Item type
            var itemTypeIds = input.ItemCodeFormulaItemTypes.Select(t => t.ItemTypeId).ToList();
            var dubplicate = await _itemCodeFormulaItemTypeRepository.GetAll().Where(t => itemTypeIds.Contains(t.ItemTypeId)).AnyAsync();
            if (dubplicate) throw new UserFriendlyException(L("DuplicateItemType"));

            var items = new List<ItemCodeFormulaProperty>();
            var itemCodeFormulaItemTypes = new List<ItemCodeFormulaItemType>();
            var itemCodeFormulaCustoms = new List<ItemCodeFormulaCustom>();
            var entities = ItemCodeFormula.Create(tenantId, userId,input.UseItemProperty,input.UseCustomGenerate,input.UseManual);
            await _iItemCodeFormulaManager.CreateAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
            foreach (var i in input.Items)
            {
                var item = ItemCodeFormulaProperty.Create(tenantId, userId, i.SortOrder, entities.Id, i.PropertyId, i.Separator);
                items.Add(item);
            }
            foreach (var i in input.ItemCodeFormulaItemTypes)
            {
                var itemType = ItemCodeFormulaItemType.Create(tenantId, userId, entities.Id, i.ItemTypeId);
                itemCodeFormulaItemTypes.Add(itemType);

            }
            if (input.ItemFormulaCustoms != null)
            {
                var c = input.ItemFormulaCustoms;
                var custom = ItemCodeFormulaCustom.Create(tenantId, userId, c.Prefix, c.ItemCode, entities.Id);
                itemCodeFormulaCustoms.Add(custom);
            }
              
            if(items.Count > 0) await _itemCodeFormulaPropertyRepository.BulkInsertAsync(items);
            if (itemCodeFormulaItemTypes.Count > 0) await _itemCodeFormulaItemTypeRepository.BulkInsertAsync(itemCodeFormulaItemTypes);
            if (itemCodeFormulaCustoms.Count > 0) await _itemCodeFormulaCustomRepository.BulkInsertAsync(itemCodeFormulaCustoms);
            return entities.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var entity = await _itemCodeFormulaRepository.GetAll().AsNoTracking().Where(u => u.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var items = await _itemCodeFormulaPropertyRepository.GetAll().AsNoTracking().Where(t => t.ItemCodeFormulaId == input.Id).ToListAsync();
            var itemCodeFormulaItemTypes = await _itemCodeFormulaItemTypeRepository.GetAll().AsNoTracking().Where(t => t.ItemCodeFormulaId == entity.Id).ToListAsync();
            var itemCodeFormulaCustom = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking().Where(s => s.ItemCodeFormulaId == input.Id).ToListAsync();
            await _itemCodeFormulaPropertyRepository.BulkDeleteAsync(items);
            await _itemCodeFormulaItemTypeRepository.BulkDeleteAsync(itemCodeFormulaItemTypes);
            await _itemCodeFormulaCustomRepository.BulkDeleteAsync(itemCodeFormulaCustom);
            await _itemCodeFormulaRepository.BulkDeleteAsync(new List<ItemCodeFormula> { entity });
           
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var entity = await _itemCodeFormulaRepository.GetAll().Where(u => u.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            await _iItemCodeFormulaManager.DisableAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var entity = await _itemCodeFormulaRepository.GetAll().Where(u => u.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            await _iItemCodeFormulaManager.EnableAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_Update)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<long> Update(CreateOrUpdatePropertyFormulaInput input)
        {
            if (!input.UseCustomGenerate && !input.UseItemProperty && !input.UseManual)
            {
                throw new UserFriendlyException(L("CheckCustomGenerateOrUseItemPropertyIsRequired"));
            }
            if (input.ItemCodeFormulaItemTypes.Count <= 0) throw new UserFriendlyException(L("ItemTypeIsRequired"));
            if (input.Items.Count <= 0 && input.UseItemProperty) throw new UserFriendlyException(L("PropetyListIsRequired"));
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId;
            var entity = new ItemCodeFormula();
            var items = new List<ItemCodeFormulaProperty>();
            var itemFormulas = new List<ItemCodeFormulaCustom>();
            var inputItemIds = input.Items.Where(t => t.Id != null).Select(t => t.Id).ToList();
            var itemForCreates = new List<ItemCodeFormulaProperty>();
            var itemForUpdates = new List<ItemCodeFormulaProperty>();
            var itemForduplicate = new List<ItemCodeFormulaProperty>();
            //item code formula types
            var itemCodeFormulaItemTypes = new List<ItemCodeFormulaItemType>();
            var inputItemCodeFormulaItemTypeIds = input.ItemCodeFormulaItemTypes.Where(t => t.Id != null).Select(t => t.Id).ToList();
            var itemCodeFormulaItemTypeForCreates = new List<ItemCodeFormulaItemType>();
            var itemCodeFormulaItemTypeForUpdates = new List<ItemCodeFormulaItemType>();
            var itemTypeIds = input.ItemCodeFormulaItemTypes.Select(t => t.ItemTypeId).ToList();

           
          
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    entity = await _itemCodeFormulaRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();
                    if (entity == null)
                    {
                        throw new UserFriendlyException(L("RecordNotFound"));
                    }
                    // check Duplicate Item type
                    var dubplicate = await _itemCodeFormulaItemTypeRepository.GetAll().Where(t => t.ItemCodeFormulaId != entity.Id && itemTypeIds.Contains(t.ItemTypeId)).AnyAsync();
                    if (dubplicate) throw new UserFriendlyException(L("DuplicateItemType"));
                    items = await _itemCodeFormulaPropertyRepository.GetAll().AsNoTracking().Where(t => t.ItemCodeFormulaId == input.Id).ToListAsync();
                    itemCodeFormulaItemTypes = await _itemCodeFormulaItemTypeRepository.GetAll().AsNoTracking().Where(u => u.ItemCodeFormulaId == input.Id).ToListAsync();
                    itemFormulas = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking().Where(t => t.ItemCodeFormulaId == input.Id).ToListAsync();
                }
            }
            entity.Update(userId,input.UseItemProperty,input.UseCustomGenerate,input.UseManual);

            // item code formula property
            var itemForDeletes = items.Where(t => !inputItemIds.Contains(t.Id)).Select(t => t).ToList();
                itemForDeletes = input.UseItemProperty ? itemForDeletes : items;
            foreach (var i in input.Items)
            {
                if (i.Id == null)
                {
                    var item = ItemCodeFormulaProperty.Create(tenantId, userId, i.SortOrder, entity.Id, i.PropertyId, i.Separator);
                    itemForCreates.Add(item);
                    itemForduplicate.Add(item);
                }
                else if (i.Id != null)
                {
                    var item = items.Where(t => t.Id == i.Id).FirstOrDefault();
                    item.Update(userId, i.SortOrder, i.PropertyId, i.Separator);
                    itemForUpdates.Add(item);
                    itemForduplicate.Add(item);
                }
            }
            var duplicateProperty = itemForduplicate.GroupBy(x => x.PropertyId).Where(g => g.Count() > 1).Any();
            if (duplicateProperty)
            {
                throw new UserFriendlyException(L("DuplicateProperty"));
            }
            var duplicateSortOrder = itemForduplicate.GroupBy(x => x.SortOrder).Where(g => g.Count() > 1).Any();
            if (duplicateSortOrder)
            {
                throw new UserFriendlyException(L("DuplicateSortOrder"));
            }

            //item code formula type 
            var itemTypeForDeletes = itemCodeFormulaItemTypes.Where(t => !inputItemCodeFormulaItemTypeIds.Contains(t.Id)).Select(t => t).ToList();
            foreach (var i in input.ItemCodeFormulaItemTypes)
            {
                if (i.Id == null)
                {
                    var item = ItemCodeFormulaItemType.Create(tenantId, userId, entity.Id, i.ItemTypeId);
                    itemCodeFormulaItemTypeForCreates.Add(item);

                }
                else if (i.Id != null)
                {
                    var item = itemCodeFormulaItemTypes.Where(t => t.Id == i.Id).FirstOrDefault();
                    item.Update(userId, entity.Id, i.ItemTypeId);
                    itemCodeFormulaItemTypeForUpdates.Add(item);
                }
            }

           

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _iItemCodeFormulaManager.UpdateAsync(entity);
                    await _itemCodeFormulaPropertyRepository.BulkInsertAsync(itemForCreates);
                    await _itemCodeFormulaPropertyRepository.BulkUpdateAsync(itemForUpdates);
                    await _itemCodeFormulaPropertyRepository.BulkDeleteAsync(itemForDeletes);
                    await _itemCodeFormulaItemTypeRepository.BulkInsertAsync(itemCodeFormulaItemTypeForCreates);
                    await _itemCodeFormulaItemTypeRepository.BulkUpdateAsync(itemCodeFormulaItemTypeForUpdates);
                    await _itemCodeFormulaItemTypeRepository.BulkDeleteAsync(itemTypeForDeletes);
                    // item code formula custom
                    if (input.UseCustomGenerate && input.ItemFormulaCustoms != null && input.ItemFormulaCustoms.Id != null)
                    {
                        var itemformula = await _itemCodeFormulaCustomRepository.GetAll().Where(s => s.ItemCodeFormulaId == input.Id).FirstOrDefaultAsync();
                        itemformula.Update(userId, input.ItemFormulaCustoms.Prefix, input.ItemFormulaCustoms.ItemCode);
                        await _itemCodeFormulaCustomRepository.UpdateAsync(itemformula);
                    }
                    else if (input.UseCustomGenerate && input.ItemFormulaCustoms != null && input.ItemFormulaCustoms.Id == null)
                    {
                      var itemformula =  ItemCodeFormulaCustom.Create(tenantId, userId, input.ItemFormulaCustoms.Prefix, input.ItemFormulaCustoms.ItemCode, entity.Id);
                        await _itemCodeFormulaCustomRepository.InsertAsync(itemformula);
                    }
                    else if(!input.UseCustomGenerate)
                    {
                        var itemformula = await _itemCodeFormulaCustomRepository.GetAll().Where(s => s.ItemCodeFormulaId == input.Id).FirstOrDefaultAsync();
                        if(itemformula != null)
                        {
                            await _itemCodeFormulaCustomRepository.DeleteAsync(itemformula);
                        }
                      
                    }

                }
                await uow.CompleteAsync();
            }

            return entity.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_GetDetail)]
        public async Task<GetDetailPropertyFormulaOutput> GetDetail(EntityDto<long> input)
        {
            var result = await _itemCodeFormulaRepository.GetAll().AsNoTracking().Where(t => t.Id == input.Id).AsNoTracking().Select(u => new GetDetailPropertyFormulaOutput
            {
                 Id = u.Id,    
                 UseCustomGenerate = u.UseCustomGenerate,
                 UseItemProperty = u.UseItemProperty,  
                 UseManual = u.UseManual,
            }).FirstOrDefaultAsync();
            result.ItemCodeFormulaProperties = await _itemCodeFormulaPropertyRepository.GetAll().AsNoTracking()
                .Include(u => u.Property)
                .Where(t => t.ItemCodeFormulaId == result.Id)
                .OrderBy(t => t.SortOrder)
                .Select(d => new GetDetailItemCodeFormulaProperty
                {
                    Id = d.Id,
                    ItemCodeFormulaId = d.ItemCodeFormulaId,
                    PropertyName = d.Property.Name,
                    PropertyId = d.PropertyId,
                    Separator = d.Separator,
                    SortOrder = d.SortOrder,
                })
                .ToListAsync();

            result.ItemCodeFormulaItemTypes = await _itemCodeFormulaItemTypeRepository.GetAll()
                .Include(u => u.ItemType).AsNoTracking().Where(t => t.ItemCodeFormulaId == result.Id).Select(d => new CreateOrUpdateItemCodeFormulaItemTypeInput
                {
                    Id = d.Id,
                    ItemCodeFormulaId = d.ItemCodeFormulaId,
                    ItemTypeId = d.ItemTypeId,
                    ItemTypeName = d.ItemType.Name
                }).ToListAsync();
            result.ItemTypeName = string.Join(", ", result.ItemCodeFormulaItemTypes.Select(t => t.ItemTypeName));
            result.ItemFormulaCustoms = await _itemCodeFormulaCustomRepository.GetAll().AsNoTracking().Where(s => s.ItemCodeFormulaId == input.Id).
                Select(s=> new CreateOrUpdateItemFormulaCustomInput { 
                 ItemCode = s.ItemCode,
                 ItemCodeFormulaId = s.ItemCodeFormulaId,
                 Prefix = s.Prefix,
                 Id = s.Id
                }).FirstOrDefaultAsync();
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_GetList)]
        public async Task<PagedResultDto<GetListFomularPropertyOutput>> GetList(GetListFomularPropertyInput input)
        {         
            var queries =  _itemCodeFormulaItemTypeRepository.GetAll().Include(u=>u.ItemCodeFormula).Include(u=>u.ItemType).AsNoTracking()
                          .WhereIf(input.IsActive != null, p => p.ItemCodeFormula.IsActive == input.IsActive)
                          .GroupBy(s=>s.ItemCodeFormula)
                          .Select(d => new GetListFomularPropertyOutput
                          {
                              UseCustomGenerate =d.Key.UseCustomGenerate,
                              UseItemProperty = d.Key.UseItemProperty,
                              Id = d.Key.Id,
                              ItemTypeName = d.Select(s=>s.ItemType.Name).ToList(),
                              IsActive = d.Key.IsActive,   
                              UseManual = d.Key.UseManual,
                          });
            var resultCount = await queries.CountAsync();
            var @entities = await queries
                                .OrderBy(input.Sorting)
                                .PageBy(input)
                                .ToListAsync();
            return new PagedResultDto<GetListFomularPropertyOutput>(resultCount, ObjectMapper.Map<List<GetListFomularPropertyOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_Find)]
        public async Task<PagedResultDto<GetListFomularPropertyOutput>> Find(GetListFomularPropertyInput input)
        {
            var queries = _itemCodeFormulaRepository
               .GetAll()
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive);           
            var resultCount = await queries.CountAsync();
            var @entities = await queries
                                .OrderBy(input.Sorting)
                                .PageBy(input)
                                .ToListAsync();
            return new PagedResultDto<GetListFomularPropertyOutput>(resultCount, ObjectMapper.Map<List<GetListFomularPropertyOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateAutoItemCode)]
        public async Task UpdateAutoItemCode(CreateOrUpdateAutoItemCodeInput input)
        {       
            var tenantId = AbpSession.TenantId;
            var tenant = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).FirstOrDefaultAsync();
            if (tenant == null) throw new UserFriendlyException(L("RecordNotFound"));
            tenant.SetAutoItemCode(ItemCodeSetting.Auto);
            await _tenantRepository.UpdateAsync(tenant);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateItemCodeSetting)]
        public async Task UpdateItemCodeSetting(CreateOrUpdateAutoItemCodeInput input)
        {
            var tenantId = AbpSession.TenantId;
            var tenant = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).FirstOrDefaultAsync();
            if (tenant == null) throw new UserFriendlyException(L("RecordNotFound"));
            tenant.SettemCodeSetting(input.ItemCodeSetting);
            await _tenantRepository.UpdateAsync(tenant);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateItemCodeSetting, AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateAutoItemCode)]
        public async Task<CreateOrUpdateAutoItemCodeInput> GetTenantDefaultItemCode()
        {
            var tenant = await _tenantRepository.GetAll().AsNoTracking().Where(t => t.Id == AbpSession.TenantId).Select(
                t => new CreateOrUpdateAutoItemCodeInput
                {                
                    ItemCodeSetting = t.ItemCodeSetting,                 
                    TenantId = t.Id,
                }).FirstOrDefaultAsync();
            return tenant;

        }
    }
}
