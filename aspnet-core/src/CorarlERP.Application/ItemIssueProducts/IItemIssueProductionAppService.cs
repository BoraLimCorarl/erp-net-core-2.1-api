using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ItemIssueProducts.Dto;
using CorarlERP.ItemIssueTransfers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueProducts
{
  public interface IItemIssueProductionAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemIssueProductInput input);
        Task<ItemIssueProductDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemIssueProductInput input);
        //Task Delete(EntityDto<Guid> input);
    }
}
