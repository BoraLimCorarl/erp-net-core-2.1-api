using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using CorarlERP.ItemTypes.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Collections.Extensions;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.UI;
using CorarlERP.Authorization;
using Abp.Authorization;

namespace CorarlERP.ItemTypes
{
    public class ItemTypeAppService : CorarlERPAppServiceBase, IItemTypeAppService
    {
        private readonly ItemTypeManager _itemTypeManager;
        private readonly IRepository<ItemType, long> _ItemTypeRepository;
        private readonly IDefaultValues _defaultValues;
        public ItemTypeAppService(ItemTypeManager itemTypeManager,
                           IRepository<ItemType, long> itemTypeRepository,
                           IDefaultValues defaultValues)
        {
            _itemTypeManager = itemTypeManager;
            _ItemTypeRepository = itemTypeRepository;
            _defaultValues = defaultValues;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemType_Find)]
        public async Task<ListResultDto<ItemTypeDetailOutput>> Find(GetItemTypeListInput input)
        {
            var @entities = await _ItemTypeRepository
                 .GetAll()
                 .AsNoTracking()
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Name.ToLower().Contains(input.Filter.ToLower())
                 )
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return new ListResultDto<ItemTypeDetailOutput>(ObjectMapper.Map<List<ItemTypeDetailOutput>>(@entities));
        }
        [AbpAuthorize(AppPermissions.Pages_Host_ItemType_GetList)]
        public async Task<ListResultDto<ItemTypeDetailOutput>> GetList(GetItemTypeListInput input)
        {
            var @entities = await _ItemTypeRepository
                  .GetAll()
                  .AsNoTracking()
                  .WhereIf(
                      !input.Filter.IsNullOrEmpty(),
                      p => p.Name.ToLower().Contains(input.Filter.ToLower())
                  )
                  .OrderBy(p => p.Name)
                  .ToListAsync();
            return new ListResultDto<ItemTypeDetailOutput>(ObjectMapper.Map<List<ItemTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_ItemType_Sync)]
        public async Task Sync()
        {
            var defaultItemTypes = _defaultValues.ItemTypes;

            var defaultCurrencyCodes = defaultItemTypes.Select(u => u.Name.ToLower());

            if (defaultCurrencyCodes.Distinct().Count() !=
                defaultCurrencyCodes.Count())
            {
                throw new UserFriendlyException(L("DuplicateItemTypeName"));
            }


            var existingItemTypes = await _ItemTypeRepository.GetAll().ToListAsync();
            var lookup = existingItemTypes.ToDictionary(u => u.Name);

            foreach (var c in defaultItemTypes)
            {
                if (lookup != null && lookup.ContainsKey(c.Name))
                {
                    //update
                    var @entity = lookup[c.Name];
                    entity.Update(null, c.Name, c.DisplayInventoryAccount, c.DisplayPurchase, c.DisplaySale, c.DisplayReorderPoint, c.DisplayTrackSerialNumber, c.DisplaySubItem,c.DisplayItemMenu);
                    CheckErrors(await _itemTypeManager.UpdateAsync(@entity, false));
                }
                else
                {
                    //create new
                    var @entity = ItemType.Create(null, c.Name, c.DisplayInventoryAccount, c.DisplayPurchase, c.DisplaySale, c.DisplayReorderPoint, c.DisplayTrackSerialNumber, c.DisplaySubItem,c.DisplayItemMenu);
                    CheckErrors(await _itemTypeManager.CreateAsync(@entity, false));
                }
            }

        }
    }
}
