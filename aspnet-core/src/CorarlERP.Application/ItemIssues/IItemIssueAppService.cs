using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemIssues
{
   public interface IItemIssueAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemIssueInput input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<ItemIssueDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<PagedResultDto<GetListItemIssueOutput>> GetList(GetListItemIssueInput input);

        Task<PagedResultDto<GetListItemIssueOutput>> Find(GetListItemIssueInput input);

        Task<NullableIdDto<Guid>> Update(UpdateItemIssueInput input);

        Task<PagedResultDto<ItemIssueSummaryOutput>> GetItemIssues(GetItemIssueInput input);
        Task<ItemIssueSummaryOutputForItemIssueItem> GetItemIssueItems(EntityDto<Guid> input);


        Task<PagedResultDto<ItemIssueSummaryOutput>> GetItemIssueForCutomerCredit(GetItemIssueInput input, Guid? exceptId);
        Task<ItemIssueSummaryOutputForItemIssueItem> GetItemIssueItemForCustomerCredit(EntityDto<Guid> input, Guid? exceptId);
        Task<PagedResultDto<ItemIssueForCustomerCreditOutput>> GetNewItemIssueForCutomerCredit(GetForCustomerCreditItemIssueInput input, Guid? exceptId);
        Task<PagedResultDto<ItemIssueSummaryOutput>> GetNewItemInvoiceForCutomerCredit(GetForCustomerCreditItemIssueInput input, Guid? exceptId);
        Task<ItemIssueSummaryOutputForItemIssueItem> GetItemInvoiceItemForCustomerCredit(EntityDto<Guid> input, Guid? exceptId);
       
    }
}
