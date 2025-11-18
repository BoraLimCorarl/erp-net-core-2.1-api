using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Locations
{
    public class LocationManager : CorarlERPDomainServiceBase, ILocationManager
    {
        private readonly IRepository<Location, long> _locationRepository;

        public LocationManager(IRepository<Location, long> locationRepository)
        {
            _locationRepository = locationRepository;
        }
        private async Task CheckDuplicateLocation(Location @entity)
        {
            var @old = await _locationRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.LocationName.ToLower() == entity.LocationName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateLocationName", entity.LocationName));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(Location entity)
        {
            await CheckDuplicateLocation(entity);

            await _locationRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Location entity)
        {
            @entity.Enable(false);
            await _locationRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Location entity)
        {
            @entity.Enable(true);
            await _locationRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Location> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _locationRepository.GetAll().Include(u=>u.ParentLocation) :
                _locationRepository.GetAll().Include(u => u.ParentLocation).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Location entity)
        {
            await _locationRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Location entity)
        {
            await CheckDuplicateLocation(entity);

            await _locationRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
