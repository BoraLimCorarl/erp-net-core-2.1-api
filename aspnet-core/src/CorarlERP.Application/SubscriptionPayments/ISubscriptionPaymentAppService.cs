using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.SubscriptionPayments.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SubscriptionPayments
{
    public interface ISubscriptionPaymentAppService: IApplicationService
    {
        Task SendTelegram(SubscriptionPaymentInput input);
        Task SendTelegramService(ServiceSubscriptionPaymentInput input);
        Task<PackageSubscriptionOutput> GetPackageSubscriptionDetail();
        //Task<SubscriptionPaymentOutput> GetLatestSubscription(EntityDto input);
    }
}
