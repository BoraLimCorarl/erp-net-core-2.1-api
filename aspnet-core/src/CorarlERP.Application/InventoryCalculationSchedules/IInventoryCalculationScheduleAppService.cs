using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.InventoryCalculationSchedules.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.TransactionTypes.Dto;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryCalculationSchedules
{
    public interface IInventoryCalculationScheduleAppService : IApplicationService
    {
        Task<PagedResultDto<GetListInventoryCalculationScheduleOutput>> GetList(GetListInventoryCalculationScheduleInput input);
        Task<InventoryCalculationScheduleDetailOutput> GetDetail(EntityDto<Guid> input);
        Task Create(InventoryCalculationScheduleDto input);
        Task Update(InventoryCalculationScheduleDto input);
        Task Execute(EntityDto<Guid> input);
        Task Delete(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
    }
}
