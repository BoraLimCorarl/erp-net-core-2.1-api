using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.KitchenOrders.Dto;
using CorarlERP.SaleOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.KitchenOrders.Dto.GetDetailKitchenOrder;

namespace CorarlERP.KitchenOrders
{
   public interface IKitchenOrderAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateKitchenOrderInput input);
        Task<PagedResultDto<GetListKitchenOrderOutput>> GetList(GetListKitchenOrderInput input);
        Task<KitchendOrderDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<GetListKitchenOrderOutput>> Find(GetListKitchenOrderInput input);
        Task Remove(CarlEntityDto input);
        Task<NullableIdDto<Guid>> Update(UpdateKitchenOrderInput input);
        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
    }
}
