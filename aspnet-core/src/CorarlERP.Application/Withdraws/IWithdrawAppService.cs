using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.Withdraws.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.Withdraws
{
    public interface IWithdrawAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateWithdrawInput input);

        Task Delete(CarlEntityDto input);

        Task<NullableIdDto<Guid>> Update(UpdateWithdrawInput input);

        Task<WithdrawDetailOutput> GetDetail(EntityDto<Guid> input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

       

       

       
    }
}
