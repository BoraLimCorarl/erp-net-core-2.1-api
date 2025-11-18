using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Bills.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.ReceivePayments.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReceivePayments
{
   public interface IReceivePaymentAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateReceivePaymentInput input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<ReceivePaymentDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<PagedResultDto<ReceivePaymentHeader>> GetList(GetLIstReceivePaymentInput input);

        Task<PagedResultDto<GetListReceivPaymentOutput>> Find(GetLIstReceivePaymentInput input);

        Task<NullableIdDto<Guid>> Update(UpdateReceivePaymentInput input);

        Task<PagedResultDto<GetListReivePaymentHistoryOutput>> ViewInvoiceHistory(GetListHistoryInput input);

        Task<PagedResultDto<GetListReivePaymentHistoryOutput>> ViewCustomerCreditHistory(GetListHistoryInput input);

        Task<MultiCurrencyPagedResultDto<GetListCustomerBalanceOutput>> GetCustomerBalance(GetListCustomerBalanceInput input);
        Task MultiDelete(GetListDeleteInput input);
        Task<FileDto> ExportExcelReceivePayment(GetLIstReceivePaymentInput input);
    }
}
