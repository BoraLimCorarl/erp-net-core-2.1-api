using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public  class QCSampleManager : CorarlERPDomainServiceBase, IQCSampleManager
    {
        private readonly IRepository<QCSample, Guid> _qcSampleRepository;

        public QCSampleManager(IRepository<QCSample, Guid> qcSampleRepository)
        {
            _qcSampleRepository = qcSampleRepository;
        }
        private async Task CheckDuplicateClass(QCSample @entity)
        {
            var find = await _qcSampleRepository.GetAll().AsNoTracking()
                           .Where(u => u.SampleId.ToLower() == entity.SampleId.ToLower())
                           .Where(u => u.Id != entity.Id)
                           .AnyAsync();

            if (find)
            {
                throw new UserFriendlyException(L("Duplicated", L("QCSample", entity.SampleId)));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(QCSample entity)
        {
            await CheckDuplicateClass(entity);

            await _qcSampleRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<QCSample> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _qcSampleRepository.GetAll() : _qcSampleRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(QCSample entity)
        {
            await _qcSampleRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(QCSample entity)
        {
            await CheckDuplicateClass(entity);

            await _qcSampleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
