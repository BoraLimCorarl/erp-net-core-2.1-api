using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.IssueKitchenOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.IssueKitchenOrders
{
    public interface IIssueKitchenOrderAppService : IApplicationService
    {
        Task<GetDetailIssueKitchenOrderDetailOutput> GetDetail(EntityDto<Guid> input);
    }
}
