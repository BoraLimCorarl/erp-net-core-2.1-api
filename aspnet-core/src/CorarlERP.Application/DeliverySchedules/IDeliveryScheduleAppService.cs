using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.DeliverySchedules
{
    public interface IDeliveryScheduleAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateDeliveryScheduleInput input);
        Task<PagedResultDto<GetListDeliveryScheduleOutput>> GetList(GetListDeliveryScheduleInput input);
        Task<PagedResultDto<GetListDeliveryScheduleOutput>> Find(GetListDeliveryScheduleInput input);
        Task<GetDetailDeliveryScheduleOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateDeliveryScheduleInput input);
        Task Delete(CarlEntityDto input);
        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToClose(UpdateStatus input);
        Task<PagedResultDto<DeliveryScheduleHeaderOutput>> GetDeliveryShcedules(GetDeliveryScheduleHeaderListInput input);
        Task<GetListDeliveryItemDetail> GetItemDeliverySchedules(EntityDto<Guid> input);
        Task<GetListDeliveryItemDetail> GetDeliveryScheduleForView(EntityDto<Guid> input);
    }
}
