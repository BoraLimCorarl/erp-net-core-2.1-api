using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Locations
{
   public interface ILocationAppService : IApplicationService
    {
        Task<LocationDetailOutput> Create(CreateLocationInput input);
        Task<PagedResultDto<LocationDetailOutput>> GetList(GetLocationListInput input);
        Task<PagedResultDto<LocationDetailOutput>> Find(GetLocationListInput input);
        Task<LocationDetailOutput> GetDetail(EntityDto<long> input);
        Task<LocationDetailOutput> Update(UpdateLocationInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task<ValidationCountOutput> GetMaxLocationCount();
    }
}
