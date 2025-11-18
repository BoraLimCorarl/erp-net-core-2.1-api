using Abp.Application.Services;
using CorarlERP.Settings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Settings
{
    public interface IBillInvoiceSettingAppService: IApplicationService
    {
        Task<BillInvoiceSettingInputOutput> CreateOrUpdate(BillInvoiceSettingInputOutput input);
        Task<BillInvoiceSettingInputOutput> GetDetail(BillInvoiceSettingType settingType);
    }
}
