using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.ItemReceiptCustomerCredits.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    public interface IItemReceiptCustomerCreditAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemReceiptCustomerCreditInput input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<ItemReceiptCustomerCreditDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<NullableIdDto<Guid>> Update(UpdateItemReceiptCustomerCreditInput input);

        //Task<PagedResultDto<ItemReceiptCusotmerCreditItemListOutput>> GetNewItemIssueForCustomerCredit(GetItemReceiptCustomerCreditListInput input);

    }
}
