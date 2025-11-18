using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization;
using CorarlERP.Inventories;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CorarlERP.CheckCalculations
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ViewTemplate)]
    public class CheckCalculationAppService : ReportBaseClass, ICheckCalculationAppService
    {
        private readonly IInventoryManager _inventoryManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<InventoryCalculationItem, Guid> _inventoryCalculationItemRepository;
        public CheckCalculationAppService(
            ICorarlRepository<InventoryCalculationItem, Guid> inventoryCalculationItemRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IInventoryManager inventoryManager,
            IRepository<AccountCycle, long> accountCycleRepository,
            AppFolders appFolders)
            : base(accountCycleRepository, appFolders, null, null)
        {
            _inventoryManager = inventoryManager;
            _unitOfWorkManager = unitOfWorkManager;
            _inventoryCalculationItemRepository = inventoryCalculationItemRepository;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ViewTemplate)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<bool> CheckAndCalculate(CheckCalculationInput input)
        {
            var result = true;
            if (input.FromDate == null) input.FromDate = DateTime.MinValue;
            try
            {
                var itemIds = new List<Guid>();
                var tenantId = AbpSession.TenantId.Value;

                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        itemIds = await _inventoryCalculationItemRepository.GetAll()
                                         .Where(s => s.Date.Date <= input.ToDate.Date)
                                         .AsNoTracking()
                                         .Select(s => s.ItemId)
                                         .ToListAsync();
                    }
                }

                if (itemIds.Any())
                {
                    await _inventoryManager.RecalculationCostHelper(tenantId, input.FromDate, input.ToDate, itemIds);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            return result;
        }
    }
}
