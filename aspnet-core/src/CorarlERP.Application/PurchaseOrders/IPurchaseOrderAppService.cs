using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.PurchaseOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PurchaseOrders
{
   public interface IPurchaseOrderAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreatePurchaseOrderInput input);
        Task<PagedResultDto<PurchaseOrderGetListOutput>> GetList(GetPurchaseOrderListInput input);
        Task<PagedResultDto<PurchaseOrderGetListOutput>> Find(GetPurchaseOrderListInput input);
        Task<PurchaseOrderDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdatePurchaseOrderInput input);
        Task Delete(CarlEntityDto input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToClose(UpdateStatus input);

        //Task<GetlistPuchaseOrderItemDetail> GetlistPuchaseOrderForItemReceipts(EntityDto<Guid> input);
        // Task<PagedResultDto<PurchaserOrderSummaryOutput>> GetTotalPurchaseOrder(GetTotalPurchaseOrderListInput input);

        Task<GetlistPuchaseOrderItemDetail> GetItemPuchaseOrders(EntityDto<Guid> input);
        Task<PagedResultDto<PurchaserOrderSummaryOutput>> GetPurchaseOrders(GetTotalPurchaseOrderListInput input);

        Task<ListResultDto<PurchaseOrderBillDetailDto>> GetBillDetailForOrder(EntityDto<Guid> input);


    }
}
