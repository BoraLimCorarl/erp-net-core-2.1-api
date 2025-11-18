using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ProductionProcesses
{
    public class ProductionProcessManager : CorarlERPDomainServiceBase, IProductionProcessManager
    {

        private readonly IRepository<ProductionProcess, long> _productionProcessRepository;

        public ProductionProcessManager(IRepository<ProductionProcess, long> productionProcessRepository)
        {
            _productionProcessRepository = productionProcessRepository;
        }
        private async Task CheckDuplicateName(ProductionProcess @entity)
        {
            var @old = await _productionProcessRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.ProcessName.ToLower() == entity.ProcessName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateProcessName", entity.ProcessName));
            }
        }

        public async Task<IdentityResult> CreateAsync(ProductionProcess entity)
        {
            await CheckDuplicateName(entity);

            await _productionProcessRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(ProductionProcess entity)
        {
            @entity.Enable(false);
            await _productionProcessRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(ProductionProcess entity)
        {
            @entity.Enable(true);
            await _productionProcessRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ProductionProcess> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _productionProcessRepository.GetAll().Include(u => u.Account) :
                         _productionProcessRepository.GetAll().Include(u => u.Account).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }
        

        public async Task<IdentityResult> RemoveAsync(ProductionProcess entity)
        {
            await _productionProcessRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ProductionProcess entity)
        {
            await CheckDuplicateName(entity);

            await _productionProcessRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }

}
