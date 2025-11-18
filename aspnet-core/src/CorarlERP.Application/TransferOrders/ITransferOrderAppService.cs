using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.TransferOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.TransferOrders
{
    public interface ITransferOrderAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateTransferOrderInput input);
        Task<PagedResultDto<TransferOrderGetListOutput>> GetList(GetTransferOrderListInput input);
        Task<PagedResultDto<TransferOrderGetListOutput>> Find(GetTransferOrderListInput input);
        Task<TransferOrderDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateTransferOrderInput input);
        Task<TransferOrderDetailOutput> GetListTransferOrderForItemReceipt(EntityDto<Guid> input);
        Task<TransferOrderDetailOutput> GetListTransferOrderForItemIssue(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);
        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);
    }
}
