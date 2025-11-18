using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public interface ILabTestRequestManager : IDomainService
    {
        Task<LabTestRequest> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(LabTestRequest @entity);
        Task<IdentityResult> RemoveAsync(LabTestRequest @entity);
        Task<IdentityResult> UpdateAsync(LabTestRequest @entity);
    }
}
