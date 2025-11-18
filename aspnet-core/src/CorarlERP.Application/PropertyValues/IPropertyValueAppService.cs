using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.PropertyValues.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PropertyValues
{
   public interface IPropertyValueAppService : IApplicationService
    {
        Task<PropertyValueDetailOutput> Create(CreatePropertyValueDto input);
        Task<ListResultDto<FindPropertyValueDetailOutput>> GetBaseUnits();
    }
}
