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
    public class PropertyManager : CorarlERPDomainServiceBase, IPropertyManager
    {
        private readonly IRepository<Property, long> _propertyRepository;

        public PropertyManager(IRepository<Property, long> propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }
        public async virtual Task<IdentityResult> CreateAsync(Property entity)
        {
            await CheckDuplicateProperty(@entity);

            await _propertyRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Property entity)
        {
            @entity.Enable(false);
            await _propertyRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> EnableAsync(Property entity)
        {
            @entity.Enable(true);
            await _propertyRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Property> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _propertyRepository.GetAll() : _propertyRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(Property entity)
        {
            await _propertyRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Property entity)
        {
            await CheckDuplicateProperty(@entity);
            await _propertyRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateProperty(Property @entity)
        {
            var @old = await _propertyRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.Name.ToLower() == entity.Name.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateTitle", entity.Name));
            }
        }
    }
}
