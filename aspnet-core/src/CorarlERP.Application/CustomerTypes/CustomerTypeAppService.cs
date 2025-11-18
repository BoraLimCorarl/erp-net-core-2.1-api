using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.CustomerTypes.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;


namespace CorarlERP.CustomerTypes
{
    [AbpAuthorize]
    public class CustomerTypeAppService : CorarlERPAppServiceBase, ICustomerTypeAppService
    {
        private readonly ICustomerTypeManager _customerTypeManager;
        private readonly IRepository<CustomerType, long> _customerTypeRepository;

        public CustomerTypeAppService(ICustomerTypeManager customerTypeManager,
                             IRepository<CustomerType, long> customerTypeRepository)
        {
            _customerTypeManager = customerTypeManager;
            _customerTypeRepository = customerTypeRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_Create)]
        public async Task<CustomerTypeDetailOutput> Create(CreateCustomertTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = CustomerType.Create(tenantId, userId, input.CustomerTypeName);

            CheckErrors(await _customerTypeManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<CustomerTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _customerTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _customerTypeManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _customerTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _customerTypeManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _customerTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _customerTypeManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_Find)]
        public async Task<PagedResultDto<CustomerTypeDetailOutput>> Find(GetCustomerTypeListInput input)
        {
            var @query = _customerTypeRepository
               .GetAll()             
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.CustomerTypeName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<CustomerTypeDetailOutput>(resultCount, ObjectMapper.Map<List<CustomerTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetDetail)]
        public async Task<CustomerTypeDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _customerTypeManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<CustomerTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetList)]
        public async Task<PagedResultDto<CustomerTypeDetailOutput>> GetList(GetCustomerTypeListInput input)
        {
            var @query = _customerTypeRepository
               .GetAll()             
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.CustomerTypeName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<CustomerTypeDetailOutput>(resultCount, ObjectMapper.Map<List<CustomerTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_CustomerTypes_Update)]
        public async Task<CustomerTypeDetailOutput> Update(UpdateCustomerTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _customerTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.CustomerTypeName);

            CheckErrors(await _customerTypeManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<CustomerTypeDetailOutput>(@entity);
        }

    }
}
