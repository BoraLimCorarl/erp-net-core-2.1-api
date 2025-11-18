using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.PaymentMethods.Dto;

namespace CorarlERP.PaymentMethods
{
    public interface IPaymentMethodAppService : IApplicationService
    {
        Task<PaymentMethodDetailOutput> Create(CreatePaymentMethodInput input);
        Task<PaymentMethodDetailOutput> Update(CreatePaymentMethodInput input);
        Task<PagedResultDto<PaymentMethodDetailOutput>> GetList(GetPaymentMethodListInput input);
        Task<PagedResultDto<PaymentMethodDetailOutput>> Find(GetPaymentMethodListInput input);
        Task<PagedResultDto<PaymentMethodBaseDetailOutput>> FindBase(GetPaymentMethodBaseListInput input);
        Task<PaymentMethodDetailOutput> GetDetail(EntityDto<Guid> input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
    }
        
}
