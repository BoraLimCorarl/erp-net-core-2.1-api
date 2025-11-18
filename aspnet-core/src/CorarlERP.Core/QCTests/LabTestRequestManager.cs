using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public  class LabTestRequestManager : CorarlERPDomainServiceBase, ILabTestRequestManager
    {
        private readonly IRepository<LabTestRequest, Guid> _labTestRequestRepository;

        public LabTestRequestManager(IRepository<LabTestRequest, Guid> labTestRequestRepository)
        {
            _labTestRequestRepository = labTestRequestRepository;
        }
       
        public async virtual Task<IdentityResult> CreateAsync(LabTestRequest entity)
        {
            await _labTestRequestRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<LabTestRequest> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _labTestRequestRepository.GetAll().Include(s => s.QCSample).Include(s => s.QCTestTemplate).Include(s => s.Lab) : _labTestRequestRepository.GetAll().Include(s => s.QCSample).Include(s => s.QCTestTemplate).Include(s => s.Lab).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(LabTestRequest entity)
        {
            await _labTestRequestRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(LabTestRequest entity)
        {
            await _labTestRequestRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
