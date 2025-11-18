using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.ReportTemplates
{
    public class ReportTemplateManager : CorarlERPDomainServiceBase, IReportTemplateManager
    {
        //private IRepository<Report, long> _reportRepository;
        private IRepository<ReportTemplate, long> _reportTemplateRepository;
        private IRepository<ReportFilterTemplate, Guid> _reportFilterTemplateRepository;
        private IRepository<ReportColumnTemplate, Guid> _reportColumnTemplateRepository;
        public ReportTemplateManager(
            //IRepository<Report, long> reportRepository,
            IRepository<ReportTemplate, long> reportTemplateRepository,
            IRepository<ReportFilterTemplate, Guid> reportFilterTemplateRepository,
            IRepository<ReportColumnTemplate, Guid> reportColumnTemplateRepository)
        {
            //_reportRepository = reportRepository;
            _reportTemplateRepository = reportTemplateRepository;
            _reportFilterTemplateRepository = reportFilterTemplateRepository;
            _reportColumnTemplateRepository = reportColumnTemplateRepository;
        }

        public async virtual Task<ReportTemplate> GetAsync(long id)
        {
            var @entity =
                  await _reportTemplateRepository
                    .GetAll()
                    //.Include(i => i.Report)
                    .Include(i => i.Columns)
                    .Include(i => i.Filters)
                    .Where(u => u.Id == id)
                    //.Where(u => u.ReportType == reportType)
                    .FirstOrDefaultAsync();

            return @entity;
        }

        public async virtual Task<IdentityResult> CreateAsync(ReportTemplate @entity)
        {
            await CheckDuplicate(entity);
            await _reportTemplateRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> RemoveAsync(ReportTemplate @entity)
        {
            await _reportTemplateRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }


        public async virtual Task<IdentityResult> RemoveFilterAsync(ReportTemplate @entity, Guid filterId)
        {

            var @filter = @entity.RemoveFilter(filterId);
            if (@filter != null)
                await _reportFilterTemplateRepository.DeleteAsync(@filter);

            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> CreateFilterAsync(ReportFilterTemplate @entity)
        {
            await _reportFilterTemplateRepository.InsertAsync(@entity);

            return IdentityResult.Success;            
        }

        public async virtual Task<IdentityResult> CreateColumnAsync(ReportColumnTemplate @entity)
        {

            await _reportColumnTemplateRepository.InsertAsync(@entity);

            return IdentityResult.Success;
        }
        public async virtual Task<IdentityResult> RemoveColumnAsync(ReportTemplate @entity, Guid columnId)
        {

            var column = @entity.RemoveColumn(columnId);
            if (column != null)
                await _reportColumnTemplateRepository.DeleteAsync(column);

            return IdentityResult.Success;
        }
        
        public async virtual Task<IdentityResult> UpdateAsync(ReportTemplate @entity)
        {
            await CheckDuplicate(entity);
            await _reportTemplateRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicate(ReportTemplate @entity)
        {
            var categories = Enum.GetValues(typeof(ReportCategory)).Cast<ReportCategory>().ToList();

            var find = await _reportTemplateRepository.GetAll().AsNoTracking()
                           .WhereIf(entity?.ReportCategory != null, u => u.ReportCategory == entity.ReportCategory)
                           .Where(s => categories.Contains(s.ReportCategory))
                           .Where(u => u.IsActive &&
                                u.TemplateName.ToLower() == entity.TemplateName.ToLower() &&
                                u.Id != entity.Id && u.TenantId == entity.TenantId
                            ).AnyAsync();

            if(find) throw new UserFriendlyException(L("DuplicateTemplateName", entity.TemplateName));
        }
    }
}
