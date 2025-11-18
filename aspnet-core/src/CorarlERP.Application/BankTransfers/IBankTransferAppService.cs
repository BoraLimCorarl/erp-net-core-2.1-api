using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.BankTransfers.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.BankTransfers
{
   public interface IBankTransferAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateBankTransferInput input);
        Task<PagedResultDto<GetListBankTransferOutput>> GetList(GetListBankTrasferInput input);
        Task<PagedResultDto<GetListBankTransferOutput>> Find(GetListBankTrasferInput input);
        Task<GetDetailBankTransferOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateBankTransferInput input);
        Task Delete(CarlEntityDto input);

        
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(BankStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
    }
}
