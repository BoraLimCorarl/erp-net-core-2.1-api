using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Settings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Settings
{

    [AbpAuthorize]
    public class BillInvoiceSettingAppService : CorarlERPAppServiceBase, IBillInvoiceSettingAppService
    {
        private readonly IBillInvoiceSettingManager _settingManager; //domain event
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;//repository

        public BillInvoiceSettingAppService(IBillInvoiceSettingManager settingManager,
                             IRepository<BillInvoiceSetting, long> settingRepository)
        {
            _settingManager = settingManager;
            _settingRepository = settingRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Setting, AppPermissions.Pages_Tenant_Vendors_Bills_Setting)]
        public async Task<BillInvoiceSettingInputOutput> CreateOrUpdate(BillInvoiceSettingInputOutput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            BillInvoiceSetting @entity ;

            if (input.Id > 0)
            {
                entity = await _settingManager.GetAsync(input.Id);
                if(entity == null) throw new UserFriendlyException(L("RecordNotFound"));
                entity.Update(userId, input.SettingType, input.ReferenceSameAsGoodsMovement, input.TurnOffStockValidationForImportExcel);

                CheckErrors(await _settingManager.UpdateAsync(@entity));
            }
            else
            {
                @entity = BillInvoiceSetting.Create(tenantId, userId, input.SettingType, input.ReferenceSameAsGoodsMovement, input.TurnOffStockValidationForImportExcel);

                CheckErrors(await _settingManager.CreateAsync(@entity));
                await CurrentUnitOfWork.SaveChangesAsync();
            }


            return ObjectMapper.Map<BillInvoiceSettingInputOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Setting, AppPermissions.Pages_Tenant_Vendors_Bills_Setting)]

        public async Task<BillInvoiceSettingInputOutput> GetDetail(BillInvoiceSettingType settingType)
        {
            var @entity = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == settingType);

            if (entity == null) return null;          

            return ObjectMapper.Map<BillInvoiceSettingInputOutput>(@entity);
        }

      
    }
}
