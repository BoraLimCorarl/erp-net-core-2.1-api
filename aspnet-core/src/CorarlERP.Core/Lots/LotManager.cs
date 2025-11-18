using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Lots
{
   public class LotManager : CorarlERPDomainServiceBase, ILotManager
    {
        private readonly IRepository<Lot, long> _LotRepository;

        public LotManager(IRepository<Lot, long> LotRepository)
        {
            _LotRepository = LotRepository;
        }
        private async Task CheckDuplicateLot(Lot @entity)
        {
            var @old = await _LotRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.LotName.ToLower() == entity.LotName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateLotName", entity.LotName));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(Lot entity)
        {
            await CheckDuplicateLot(entity);

            await _LotRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Lot entity)
        {
            @entity.Enable(false);
            await _LotRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Lot entity)
        {
            @entity.Enable(true);
            await _LotRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Lot> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _LotRepository.GetAll().Include(u => u.Location) :
                _LotRepository.GetAll().Include(u => u.Location).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Lot entity)
        {
            await _LotRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Lot entity)
        {
            await CheckDuplicateLot(entity);

            await _LotRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

    }
}
