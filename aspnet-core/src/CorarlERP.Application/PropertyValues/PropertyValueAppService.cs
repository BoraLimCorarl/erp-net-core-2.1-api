using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using CorarlERP.PropertyValues.Dto;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;
using System.Linq;
using Abp.Extensions;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.UI;
using CorarlERP.Authorization;

namespace CorarlERP.PropertyValues
{
    [AbpAuthorize]
    public class PropertyValueAppService : CorarlERPAppServiceBase, IPropertyValueAppService
    {
        private readonly IPropertyValueManager _propertyValuetManager;
        private readonly IRepository<PropertyValue, long> _propertyValueRepository;

        public PropertyValueAppService(IPropertyValueManager propertyValueManager,
                             IRepository<PropertyValue, long> propertyValueRepository)
        {
            _propertyValuetManager = propertyValueManager;
            _propertyValueRepository = propertyValueRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Create)]
        public async Task<PropertyValueDetailOutput> Create(CreatePropertyValueDto input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            if (input.IsDefault)
            {
                var checkIsDefault = await _propertyValueRepository.GetAll().AsNoTracking().Where(t => t.IsDefault && t.PropertyId == input.PropertyId).AnyAsync();
                if (checkIsDefault) throw new UserFriendlyException(L("CannotCheckMultiDefault"));
            }

            var @entity = PropertyValue.Create(tenantId, userId, input.PropertyId, input.Value, input.NetWeight, input.IsDefault,input.Code, input.IsBaseUnit, input.BaseUnitId, input.Factor);

            CheckErrors(await _propertyValuetManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PropertyValueDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Find)]
        public async Task<ListResultDto<FindPropertyValueDetailOutput>> GetBaseUnits()
        {
            var @query = _propertyValueRepository
                 .GetAll()
                 .AsNoTracking()
                 .Where(s => s.Property.IsUnit)
                 .Where(s => s.IsBaseUnit && s.IsActive)
                 .Select(s => new FindPropertyValueDetailOutput
                 {
                     Id = s.Id,
                     Value = s.Value,
                     Code = s.Code
                 })
                 .OrderBy(s => s.Code).ThenBy(s => s.Value);

            var @entities = await query.ToListAsync();

            return new ListResultDto<FindPropertyValueDetailOutput>(@entities);

        }

    }
}
