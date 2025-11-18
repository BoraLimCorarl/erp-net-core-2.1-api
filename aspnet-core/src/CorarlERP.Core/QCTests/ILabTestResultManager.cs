using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public interface ILabTestResultManager : IDomainService
    {
        Task<LabTestResult> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(LabTestResult @entity);
        Task<IdentityResult> RemoveAsync(LabTestResult @entity);
        Task<IdentityResult> UpdateAsync(LabTestResult @entity);
    }
}
