using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.SaleOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SaleOrders
{
    public interface ISaleOrderAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateSaleOrderInput input);
        Task<PagedResultDto<SaleOrderGetListOutput>> GetList(GetSaleOrderListInput input);
        Task<PagedResultDto<SaleOrderGetListOutput>> Find(GetSaleOrderListInput input);
        Task<SaleOrderDetailOutput> GetDetail(EntityDto<Guid> input);

        Task<NullableIdDto<Guid>> Update(UpdateSaleOrderInput input);
        Task Delete(CarlEntityDto input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToClose(UpdateStatus input);

        Task<GetListSaleOrderItemDetail> GetItemSaleOrders(EntityDto<Guid> input);      
        Task<PagedResultDto<SaleOrderHeaderOutput>> GetSaleOrders(GetSaleOrderHeaderListInput input);

        Task<FileDto> CreateAndPrint(CreateSaleOrderInput input);

        Task<ListResultDto<SaleOrderInvoiceDetailDto>> GetInvoiceDetailForOrder(EntityDto<Guid> input);
        Task UpdateInventoryStatus(EntityDto<Guid> input);

        Task<ListResultDto<GetListDeliveryScheduleOutput>> GetListDeliverySchedules(EntityDto<Guid> input);

    }
}
