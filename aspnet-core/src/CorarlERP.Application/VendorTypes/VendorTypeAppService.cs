using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.VendorTypes.Dto;

namespace CorarlERP.VendorTypes
{
    [AbpAuthorize]
    public class VendorTypeAppService : CorarlERPAppServiceBase, IVendorTypeAppService
    {
        private readonly IVendorTypeManager _vendorTypeManager;
        private readonly IRepository<VendorType, long> _vendorTypeRepository;

        public VendorTypeAppService(IVendorTypeManager vendorTypeManager,
                             IRepository<VendorType, long> vendorTypeRepository)
        {
            _vendorTypeManager = vendorTypeManager;
            _vendorTypeRepository = vendorTypeRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_Create)]
        public async Task<VendorTypeDetailOutput> Create(CreateVendorTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = VendorType.Create(tenantId, userId, input.VendorTypeName);

            CheckErrors(await _vendorTypeManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<VendorTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _vendorTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _vendorTypeManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _vendorTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _vendorTypeManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _vendorTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _vendorTypeManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_Find)]
        public async Task<PagedResultDto<VendorTypeDetailOutput>> Find(GetVendorTypeListInput input)
        {
            var @query = _vendorTypeRepository
               .GetAll()             
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.VendorTypeName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<VendorTypeDetailOutput>(resultCount, ObjectMapper.Map<List<VendorTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_GetDetail)]
        public async Task<VendorTypeDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _vendorTypeManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<VendorTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_GetList)]
        public async Task<PagedResultDto<VendorTypeDetailOutput>> GetList(GetVendorTypeListInput input)
        {
            var @query = _vendorTypeRepository
               .GetAll()             
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.VendorTypeName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<VendorTypeDetailOutput>(resultCount, ObjectMapper.Map<List<VendorTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_VendorTypes_Update)]
        public async Task<VendorTypeDetailOutput> Update(UpdateVendorTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _vendorTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.VendorTypeName);

            CheckErrors(await _vendorTypeManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<VendorTypeDetailOutput>(@entity);
        }

    }
}
