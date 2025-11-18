using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.PaymentMethods.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.Dependency;

namespace CorarlERP.PaymentMethods
{
    public class PaymentMethodBaseAppService : CorarlERPAppServiceBase, IPaymentMethodBaseAppService
    {
        private readonly IPaymentMethodBaseManager _paymentMethodBaseManager;
        private readonly IRepository<PaymentMethodBase, Guid> _paymentMethodBaseRepository;

        public PaymentMethodBaseAppService()
        {
            _paymentMethodBaseManager = IocManager.Instance.Resolve<IPaymentMethodBaseManager>();
            _paymentMethodBaseRepository = IocManager.Instance.Resolve <IRepository<PaymentMethodBase, Guid>>(); 
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_Create)]
        public async Task<PaymentMethodBaseDetailOutput> Create(CreatePaymentMethodBaseInput input)
        {
            var userId = AbpSession.GetUserId();
            var @entity = PaymentMethodBase.Create(userId, input.Name, input.Icon, input.IsDefault);

            CheckErrors(await _paymentMethodBaseManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PaymentMethodBaseDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodBaseManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _paymentMethodBaseManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodBaseManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _paymentMethodBaseManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodBaseManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _paymentMethodBaseManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_GetDetail)]
        public async Task<PaymentMethodBaseDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodBaseManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<PaymentMethodBaseDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_GetList)]
        public async Task<PagedResultDto<PaymentMethodBaseDetailOutput>> GetList(GetPaymentMethodBaseListInput input)
        {
            var @query = _paymentMethodBaseRepository
               .GetAll()
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PaymentMethodBaseDetailOutput>(resultCount, ObjectMapper.Map<List<PaymentMethodBaseDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PaymentMethods_Update)]
        public async Task<PaymentMethodBaseDetailOutput> Update(CreatePaymentMethodBaseInput input)
        {
            // var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _paymentMethodBaseManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.Name, input.Icon, input.IsDefault);

            CheckErrors(await _paymentMethodBaseManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PaymentMethodBaseDetailOutput>(@entity);
        }
    }
}
