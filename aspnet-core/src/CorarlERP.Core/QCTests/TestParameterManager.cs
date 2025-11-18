using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public  class TestParameterManager : CorarlERPDomainServiceBase, ITestParameterManager
    {
        private readonly IRepository<TestParameter, long> _vendorTypeRepository;

        public TestParameterManager(IRepository<TestParameter, long> vendorTypeRepository)
        {
            _vendorTypeRepository = vendorTypeRepository;
        }
        private async Task CheckDuplicateClass(TestParameter @entity)
        {
            var find = await _vendorTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.Name.ToLower() == entity.Name.ToLower())
                           .Where(u => u.Id != entity.Id)
                           .AnyAsync();

            if (find)
            {
                throw new UserFriendlyException(L("Duplicated", L("TestParameterName") + " " + entity.Name));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(TestParameter entity)
        {
            await CheckDuplicateClass(entity);

            await _vendorTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(TestParameter entity)
        {
            @entity.Enable(false);
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(TestParameter entity)
        {
            @entity.Enable(true);
            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<TestParameter> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _vendorTypeRepository.GetAll() : _vendorTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(TestParameter entity)
        {
            await _vendorTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(TestParameter entity)
        {
            await CheckDuplicateClass(entity);

            await _vendorTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
