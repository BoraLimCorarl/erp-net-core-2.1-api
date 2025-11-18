using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Bills.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.PayBills.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PayBills
{
    public interface IPayBillAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreatePayBillInput input);

        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<PayBillDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<PagedResultDto<PayBillHeader>> GetList(GetListPayBillInput input);

        Task<PagedResultDto<GetListPayBillOutput>> Find(GetListPayBillInput input);

        Task<NullableIdDto<Guid>> Update(UpdatePayBillInput input);
        Task<PagedResultDto<GetListPayBillHistoryOutput>> ViewBillHistory(GetListViewHistoryInput input);
        Task<PagedResultDto<GetListPayBillHistoryOutput>> ViewVendorCreditHistory(GetListViewHistoryInput input);

        Task<MultiCurrencyPagedResultDto<GetListVendorBalanceOutput>> GetVendorBalance(GetListVendorBalanceInput input);
        Task MultiDelete(GetListDeleteInput input);
    }
}
