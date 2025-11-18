using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Abp.UI;


namespace CorarlERP.PropertyValues
{
    public class PropertyValueManager : CorarlERPDomainServiceBase, IPropertyValueManager
    {
        private readonly IRepository<PropertyValue, long> _propertyValueRepository;

        public PropertyValueManager(IRepository<PropertyValue, long> propertuValueRepository)
        {
            _propertyValueRepository = propertuValueRepository;
        }
        public async virtual Task<IdentityResult> CreateAsync(PropertyValue entity)
        {
            await CheckDuplicatePropertyValue(entity);
            await CheckDuplicatePropertyCode(entity);
            await _propertyValueRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(PropertyValue entity)
        {
            @entity.Enable(false);
            await _propertyValueRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(PropertyValue entity)
        {
            @entity.Enable(true);
            await _propertyValueRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PropertyValue> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _propertyValueRepository.GetAll() : _propertyValueRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PropertyValue entity)
        {
            await _propertyValueRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PropertyValue entity)
        {
            await CheckDuplicatePropertyValue(@entity);
            await CheckDuplicatePropertyCode(entity);
            await _propertyValueRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicatePropertyValue(PropertyValue @entity)
        {
            var @old = await _propertyValueRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.Value.ToLower() == entity.Value.ToLower() &&
                                       u.Id != entity.Id &&
                                       u.PropertyId == entity.PropertyId)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateValue", entity.Value));
            }
        }

        private async Task CheckDuplicatePropertyCode(PropertyValue @entity)
        {
            var @old = await _propertyValueRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive && !string.IsNullOrWhiteSpace(u.Code) &&
                                      !string.IsNullOrWhiteSpace(entity.Code) &&
                                       u.Code.ToLower() == entity.Code.ToLower() &&
                                       u.Id != entity.Id &&
                                       u.PropertyId == entity.PropertyId)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                var message = L("DuplicateCode") + " " + entity.Code;
                throw new UserFriendlyException(message);
            }
        }
    }
}
