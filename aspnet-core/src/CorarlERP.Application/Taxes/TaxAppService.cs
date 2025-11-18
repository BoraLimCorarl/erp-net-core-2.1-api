using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using CorarlERP.Taxes.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.UI;
using Abp.Collections.Extensions;
using System.Linq.Dynamic.Core;


namespace CorarlERP.Taxes
{
    [AbpAuthorize]
    public class TaxAppService : CorarlERPAppServiceBase, ITaxAppService
    {
        private readonly ITaxManager _taxManager; //domain event
        private readonly IRepository<Tax, long> _taxRepository;//repository

        public TaxAppService(ITaxManager taxManager,
                             IRepository<Tax, long> taxRepository)
        {
            _taxManager = taxManager;
            _taxRepository = taxRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Taxes_Create)]
        public async Task<TaxDetailOutput> Create(CreateTaxInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = Tax.Create(tenantId, userId, input.TaxName, input.TaxRate);

            CheckErrors(await _taxManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();


            return ObjectMapper.Map<TaxDetailOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes)]
        public async Task<ListResultDto<TaxDetailOutput>> Find(GetTaxListInput input)
        {
            var @entities = await _taxRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.TaxName.ToLower().Contains(input.Filter.ToLower())
                )
                .OrderBy(p => p.TaxName)
                .ToListAsync();

            return new ListResultDto<TaxDetailOutput>(ObjectMapper.Map<List<TaxDetailOutput>>(@entities));
        }

        private async Task<TaxDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _taxManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            return ObjectMapper.Map<TaxDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Taxes_Edit)]
        public async Task<TaxDetailOutput> Update(UpateTaxInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _taxManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.TaxName, input.TaxRate);

            CheckErrors(await _taxManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<TaxDetailOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Taxes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _taxManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _taxManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Taxes_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _taxManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _taxManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Taxes_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _taxManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _taxManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Taxes)]
        public async Task<PagedResultDto<TaxDetailOutput>> GetList(GetTaxListInput input)
        {
            var @query = _taxRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.TaxName.ToLower().Contains(input.Filter.ToLower()))
                .OrderBy(p => p.TaxName);
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<TaxDetailOutput>(resultCount, ObjectMapper.Map<List<TaxDetailOutput>>(@entities));
        }
    }
}
