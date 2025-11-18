using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReportTemplates
{
    public interface IReportTemplateManager : IDomainService
    {
        Task<ReportTemplate> GetAsync(long id);
        Task<IdentityResult> CreateAsync(ReportTemplate @entity);
        Task<IdentityResult> UpdateAsync(ReportTemplate @entity);
        Task<IdentityResult> RemoveAsync(ReportTemplate @entity);

        Task<IdentityResult> CreateFilterAsync(ReportFilterTemplate @entity);
        Task<IdentityResult> RemoveFilterAsync(ReportTemplate @entity, Guid filterId);
        //Task<IdentityResult> RemoveGroupbyAsync(ReportTemplate @entity, Guid filterId);
        Task<IdentityResult> CreateColumnAsync(ReportColumnTemplate @entity);
        Task<IdentityResult> RemoveColumnAsync(ReportTemplate @entity, Guid filterId);
    }
}
