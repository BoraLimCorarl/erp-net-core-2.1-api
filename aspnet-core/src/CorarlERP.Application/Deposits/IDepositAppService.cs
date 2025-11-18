using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Deposits.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Deposits
{
    public interface IDepositAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateDepositInput input);        
        //Task<PagedResultDto<GetListDepositOutput>> GetList(GetListDepositInput input);
        //Task<PagedResultDto<GetListDepositOutput>> Find(GetListDepositInput input);
        Task<DepositDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateDepositInput input);
        Task Delete(CarlEntityDto input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

    }
}
