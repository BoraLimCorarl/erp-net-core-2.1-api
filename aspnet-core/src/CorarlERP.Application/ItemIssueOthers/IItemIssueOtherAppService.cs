using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ItemIssueOthers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueOthers
{
   public interface IItemIssueOtherAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemIssueOtherInput input);
        Task<ItemIssueOtherDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemIssueOtherInput input);
        //Task Delete(EntityDto<Guid> input);
    }
}
