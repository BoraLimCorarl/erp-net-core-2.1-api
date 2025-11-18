using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.POS.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.CustomerCredits
{
    public interface ICustomerCreditAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateCustomerCreditInput input);

        //Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<CustomerCreditDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<NullableIdDto<Guid>> Update(UpdateCustomerCreditInput input);

        Task<PagedResultDto<GetListCustomerCreditOutput>> Find(ListCustomerCreditInput input);

        Task<PagedResultDto<CustomerCreditSummaryOutput>> GetCustomerCreditHeader(GetCustomerCreditInput input);
        Task<CustomerCreditDetailOutput> GetCustomerCreditItems(EntityDto<Guid> input);

        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
        Task<PagedResultDto<GetListSaleReturn>> FindSaleReturn(ListCustomerCreditInput input);
        Task<NullableIdDto<Guid>> RefundMore(POSPaymoreInput input);
    }
}
