using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.PaymentMethods.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PaymentMethods
{
    public interface IPaymentMethodBaseAppService : IApplicationService
    {
        Task<PaymentMethodBaseDetailOutput> Create(CreatePaymentMethodBaseInput input);
        Task<PagedResultDto<PaymentMethodBaseDetailOutput>> GetList(GetPaymentMethodBaseListInput input);
        Task<PaymentMethodBaseDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<PaymentMethodBaseDetailOutput> Update(CreatePaymentMethodBaseInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
    }
}
