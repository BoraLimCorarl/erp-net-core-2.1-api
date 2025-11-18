using Abp.Authorization;
using Abp.Domain.Repositories;
using CorarlERP.ChartOfAccounts;
using CorarlERP.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.AutoSequences
{
    [AbpAuthorize]
    public class AutoSequenceAppService: CorarlERPAppServiceBase, IAutoSequenceAppService
    {

        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<Tenant, int> _tenantRepository;
        public AutoSequenceAppService(
            IRepository<Tenant, int> tenantRepository,         
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager
            ) 
        { 
            _autoSequenceRepository = autoSequenceRepository;
            _tenantRepository = tenantRepository;
            _autoSequenceManager = autoSequenceManager;
        
        }

        [AbpAuthorize]
        public async Task Sync()
        {
            var activeTenants = await _tenantRepository.GetAll().AsNoTracking().Where(s => s.IsActive).ToListAsync();

            var docTyps = _autoSequenceManager.GetDocumentTypes();

            foreach ( var tenant in activeTenants )
            {
                using(CurrentUnitOfWork.SetTenantId(tenant.Id))
                {
                    var autoSequences = await _autoSequenceRepository.GetAll().AsNoTracking().ToListAsync();
                    var userId = autoSequences.FirstOrDefault()?.CreatorUserId;

                    var addDocTypes = docTyps.Where(s => !autoSequences.Any(a => a.DocumentType == s.Type)).ToList();

                    if(addDocTypes.Any() ) 
                    { 
                        foreach ( var auto in addDocTypes )
                        {
                            var docType = AutoSequence.Create(
                                                      tenant.Id, userId, auto.Type, auto.AutoSequenceTitle,
                                                      auto.SybolFormat, auto.NumberFormat, auto.CustomFormat,
                                                      auto.YearFormat, auto.LastAutoSequence);

                            await _autoSequenceManager.CreateAsync(docType);
                        }
                    
                    }
                }
            }
        }

    }
}
