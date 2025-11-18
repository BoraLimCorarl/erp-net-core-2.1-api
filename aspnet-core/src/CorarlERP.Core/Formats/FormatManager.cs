using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Formats
{
    public class FormatManager : CorarlERPDomainServiceBase, IFormatManager
    {
        private readonly IRepository<Format, long> _formatRepository;

        public FormatManager(IRepository<Format, long> formatRepository)
        {
            _formatRepository = formatRepository;
        }

        public async Task<IdentityResult> CreateAsync(Format entity, bool checkDuplicate = true)
        {
            if (checkDuplicate)
                await CheckDuplicateCurrency(@entity);
            await _formatRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Format> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _formatRepository.GetAll() : _formatRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(Format entity, bool checkDuplicate = true)
        {
            if (checkDuplicate) await CheckDuplicateCurrency(@entity);

            await _formatRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
        private async Task CheckDuplicateCurrency(Format @entity)
        {
            var @old = await _formatRepository
                             .GetAll().AsNoTracking()
                             .Where(u =>
                                       u.Id != entity.Id &&
                                       u.Name.ToLower() == entity.Name.ToLower())
                              .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateFormatName", entity.Name));
            }
        }
    }
}
