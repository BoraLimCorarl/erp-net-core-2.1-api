using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.TransactionTypes;
using CorarlERP.TransactionTypes.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;

namespace CorarlERP.Transactiontypes
{
    [AbpAuthorize]
    public class TransactionTypeAppSevice : CorarlERPAppServiceBase, ITransactionTypeAppService
    {
        private readonly ITransactionTypeManager _transactionTypeManager;
        private readonly IRepository<TransactionType, long> _transactionTypeRepository;

        public TransactionTypeAppSevice(ITransactionTypeManager transactionTypeManager,
                             IRepository<TransactionType, long> transactionTypeRepository)
        {
            _transactionTypeManager = transactionTypeManager;
            _transactionTypeRepository = transactionTypeRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_Create)]
        public async Task<TransactionTypeDetailOutput> Create(CreateTransactionTypeInput input)
        {

            var validate = await _transactionTypeRepository.GetAll().Where(t => t.IsPOS == true && input.IsPOS == true).ToListAsync();
            if(validate.Count > 0)
            {
                throw new UserFriendlyException(L("SaleTypePOSHadAlready"));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = TransactionType.Create(tenantId, userId, input.TransactionTypeName);
                entity.SetIsPOS(input.IsPOS);
            CheckErrors(await _transactionTypeManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<TransactionTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _transactionTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _transactionTypeManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _transactionTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _transactionTypeManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _transactionTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _transactionTypeManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find)]
        public async Task<PagedResultDto<TransactionTypeDetailOutput>> Find(GetTransactionTypeListInput input)
        {
            var @query = _transactionTypeRepository
               .GetAll()
               .AsNoTracking()
               .Where(s => input.InclusePOS || s.IsPOS == false)
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.TransactionTypeName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<TransactionTypeDetailOutput>(resultCount, ObjectMapper.Map<List<TransactionTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_GetDetail)]
        public async Task<TransactionTypeDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _transactionTypeManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<TransactionTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_GetList)]
        public async Task<PagedResultDto<TransactionTypeDetailOutput>> GetList(GetTransactionTypeListInput input)
        {
            var @query = _transactionTypeRepository
               .GetAll()
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.TransactionTypeName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<TransactionTypeDetailOutput>(resultCount, ObjectMapper.Map<List<TransactionTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TransactionTypes_Update)]
        public async Task<TransactionTypeDetailOutput> Update(UpdateTransactionTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _transactionTypeManager.GetAsync(input.Id, true);

            var validate = await _transactionTypeRepository.GetAll().Where(t => t.IsPOS == true && t.Id != entity.Id && input.IsPOS == true).ToListAsync();
            if (validate.Count > 0)
            {
                throw new UserFriendlyException(L("SaleTypePOSHadAlready"));
            }

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.TransactionTypeName);
            entity.SetIsPOS(input.IsPOS);

            CheckErrors(await _transactionTypeManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<TransactionTypeDetailOutput>(@entity);
        }

    }
}
