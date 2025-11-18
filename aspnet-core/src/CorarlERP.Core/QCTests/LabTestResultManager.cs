using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public  class LabTestResultManager : CorarlERPDomainServiceBase, ILabTestResultManager
    {
        private readonly IRepository<LabTestResult, Guid> _labTestResultRepository;

        public LabTestResultManager(IRepository<LabTestResult, Guid> labTestResultRepository)
        {
            _labTestResultRepository = labTestResultRepository;
        }
       
        public async virtual Task<IdentityResult> CreateAsync(LabTestResult entity)
        {
            await _labTestResultRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<LabTestResult> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _labTestResultRepository.GetAll().Include(s => s.LabTestRequest) : _labTestResultRepository.GetAll().Include(s => s.LabTestRequest).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(LabTestResult entity)
        {
            await _labTestResultRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(LabTestResult entity)
        {
            await _labTestResultRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
