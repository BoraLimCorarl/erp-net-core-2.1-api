using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.ItemIssueVendorCredits.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssueVendorCredits
{
    public interface IItemIssueVendorCreditAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemIssueVendorCreditInput input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<ItemIssueVendorCreditDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<ItemIssueVendorCreditDetailOutput> GetItemIssueVendorItems(EntityDto<Guid> input);
        Task Delete(CarlEntityDto input);

        Task<NullableIdDto<Guid>> Update(UpdateItemIssueVendorCreditInput input);

        Task<PagedResultDto<ItemIssueVendorCreditSummaryOutput>> GetItemIssueForVendorCredit(GetItemIssueVendorCreditInput input);
        Task<PagedResultDto<ItemIssueVendorCreditItemFromVendorCreditOutput>> GetNewItemIssueForVendorCredit(GetItemIssueVendorCreditInput input);
    }
}
