using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ItemIssueTransfers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueTransfers
{
  public interface IItemIssueTransferAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemIssueTransferInput input);
        Task<ItemIssueTransferDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemIssueTransferInput input);
        //Task Delete(EntityDto<Guid> input);
    }
}
