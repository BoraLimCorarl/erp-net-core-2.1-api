using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public  class QCTestTemplateManager : CorarlERPDomainServiceBase, IQCTestTemplateManager
    {
        private readonly IRepository<QCTestTemplate, long> _vendorTypeRepository;

        public QCTestTemplateManager(IRepository<QCTestTemplate, long> vendorTypeRepository)
        {
            _vendorTypeRepository = vendorTypeRepository;
        }
        private async Task CheckDuplicateClass(QCTestTemplate @entity)
        {
            var find = await _vendorTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.Name.ToLower() == entity.Name.ToLower())
                           .Where(u => u.Id != entity.Id)
                           .AnyAsync();

            if (find)
            {
                throw new UserFriendlyException(L("Duplicated", L("QCTestTemplateName") + " " + entity.Name));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(QCTestTemplate entity)
        {
            await CheckDuplicateClass(entity);

            await _vendorTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(QCTestTemplate entity)
        {
            @entity.Enable(false);
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(QCTestTemplate entity)
        {
            @entity.Enable(true);
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<QCTestTemplate> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _vendorTypeRepository.GetAll() : _vendorTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(QCTestTemplate entity)
        {
            await _vendorTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(QCTestTemplate entity)
        {
            await CheckDuplicateClass(entity);

            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
