using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Classes.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Authorization;

namespace CorarlERP.Classes
{
    [AbpAuthorize]
    public class ClassAppService : CorarlERPAppServiceBase, IClassAppService
    {
        private readonly IClassManager _classManager;
        private readonly IRepository<Class, long> _classRepository;

        public ClassAppService(IClassManager classManager,
                             IRepository<Class,long> classRepository)
        {
            _classManager = classManager;
            _classRepository = classRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_Create)]
        public async Task<ClassDetailOutput> Create(CreateClassInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = Class.Create(tenantId, userId,input.ClassName,input.ClassParent,input.ParentClassId);

            CheckErrors(await _classManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<ClassDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _classManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _classManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _classManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _classManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _classManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _classManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_Find)]
        public async Task<PagedResultDto<ClassDetailOutput>> Find(GetClassListInput input)
        {
            var @query = _classRepository
               .GetAll()                
               .Include(u => u.ParentClass)
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)              
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.ClassName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ClassDetailOutput>(resultCount, ObjectMapper.Map<List<ClassDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_GetDetail)]
        public async Task<ClassDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _classManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<ClassDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_GetList)]
        public async Task<PagedResultDto<ClassDetailOutput>> GetList(GetClassListInput input)
        {
            var @query = _classRepository
               .GetAll()
               .Include(u => u.ParentClass)
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.ClassName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ClassDetailOutput>(resultCount, ObjectMapper.Map<List<ClassDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Classes_Update)]
        public async Task<ClassDetailOutput> Update(UpdateClassInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _classManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId,input.ClassName,input.ClassParent,input.ParentClassId);

            CheckErrors(await _classManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<ClassDetailOutput>(@entity);
        }
    }
}
