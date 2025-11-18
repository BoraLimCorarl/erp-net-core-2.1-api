using Abp.Application.Services.Dto;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Common.Dto;
using CorarlERP.InventoryTransactionTypes;
using CorarlERP.JournalTransactionTypes.Dto;
using CorarlERP.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Authorization;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Application.Features;
using CorarlERP.Features;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.JournalTransactionTypes
{
    public class JournalTransactionTypeAppservice : ReportBaseClass, IJournalTransactionTypeAppService
    {

        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        private ICorarlRepository<JournalTransactionType, Guid> _journalTransactionTransactionRepository;
        private readonly IFeatureChecker _featureChecker;
        public JournalTransactionTypeAppservice(IJournalTransactionTypeManager journalTransactionTypeManager,
                                                ICorarlRepository<JournalTransactionType, Guid> journalTransactionTransactionRepository,
         AppFolders appFolders,
         IFeatureChecker featureChecker

       ) : base(null, appFolders, null, null)
        {
            _journalTransactionTransactionRepository = journalTransactionTransactionRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _featureChecker = featureChecker;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Create)]
        public async Task<Guid> Create(CreateJournalTransactionTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = JournalTransactionType.Create(tenantId, userId, input.Name, input.IsIssue, input.IsDefault, input.InventoryTransactionType);

            CheckErrors(await _journalTransactionTypeManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _journalTransactionTransactionRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            if (entity.IsDefault) throw new UserFriendlyException(L("CanNotDeleteDefaultValue"));

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _journalTransactionTypeManager.RemoveAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _journalTransactionTransactionRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            if (entity.IsDefault) throw new UserFriendlyException(L("CanNotDisableDefaultValue"));
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _journalTransactionTypeManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _journalTransactionTransactionRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _journalTransactionTypeManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find)]
        public async Task<PagedResultDto<GetJournalTransactionTypeDetail>> Find(GetListJournalTransactionTypeInput input)
        {
            var hasPoductionFeature = IsEnabled(AppFeatures.ProductionFeature);
            var hasInventoryPhysicalCountsFeature = IsEnabled(AppFeatures.InventoryFeaturePhysicalCounts);
            var hasTransferOrderFeature = IsEnabled(AppFeatures.InventoryFeatureTransferOrders);
            var @query = _journalTransactionTransactionRepository
               .GetAll()
               .AsNoTracking()
               .WhereIf(input.InventoryTransactionType != null && input.InventoryTransactionType.Count > 0, s => input.InventoryTransactionType.Contains(s.InventoryTransactionType))
               .WhereIf(input.IsActive != null, p => p.Active == input.IsActive.Value)
               .WhereIf(!hasPoductionFeature, s => s.InventoryTransactionType != InventoryTransactionType.ItemIssueProduction && s.InventoryTransactionType != InventoryTransactionType.ItemReceiptProduction)
               .WhereIf(!hasInventoryPhysicalCountsFeature, s => s.InventoryTransactionType != InventoryTransactionType.ItemIssuePhysicalCount && s.InventoryTransactionType != InventoryTransactionType.ItemReceiptPhysicalCount)
               .WhereIf(!hasTransferOrderFeature, s => s.InventoryTransactionType != InventoryTransactionType.ItemIssueTransfer && s.InventoryTransactionType != InventoryTransactionType.ItemReceiptTransfer)
               .WhereIf(input.InventoryTypes != null && input.InventoryTypes.Count > 0, s => 
                                     ((input.InventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.IsIssue) ||
                                     (input.InventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.IsIssue)))
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetJournalTransactionTypeDetail>(resultCount, ObjectMapper.Map<List<GetJournalTransactionTypeDetail>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_GetDetail)]
        public async Task<GetJournalTransactionTypeOutput> GetDetail(EntityDto<Guid> input)
        {
            var query = await _journalTransactionTransactionRepository.GetAll().AsNoTracking().Where(t => t.Id == input.Id).Select(j => new GetJournalTransactionTypeOutput
            {
                Active = j.Active,
                Id = j.Id,
                InventoryTransactionType = j.InventoryTransactionType,
                IsDefault = j.IsDefault,
                IsIssue = j.IsIssue,
                Name = j.Name,
                InventoryTransactionTypeName = j.InventoryTransactionType.ToString(),
            }).FirstOrDefaultAsync();
            return query;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_GetList)]
        public async Task<PagedResultDto<GetJournalTransactionTypeDetail>> GetList(GetListJournalTransactionTypeInput input)
        {
            var @query = _journalTransactionTransactionRepository
              .GetAll()
              .AsNoTracking()
              .WhereIf(input.InventoryTransactionType != null && input.InventoryTransactionType.Count > 0, s => input.InventoryTransactionType.Contains(s.InventoryTransactionType))
              .WhereIf(input.IsActive != null, p => p.Active == input.IsActive.Value)
              .WhereIf(
                  !input.Filter.IsNullOrEmpty(),
                  p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetJournalTransactionTypeDetail>(resultCount, ObjectMapper.Map<List<GetJournalTransactionTypeDetail>>(@entities));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Update)]

        public async Task<Guid> Update(CreateJournalTransactionTypeInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = await _journalTransactionTransactionRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            input.InventoryTransactionType = entity.InventoryTransactionType;
            entity.Update(userId, input.Name, input.IsIssue, input.IsDefault, input.InventoryTransactionType);
            await CurrentUnitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventorys)]
        public async Task<PagedResultDto<GetListTypeForJournalTransactionOutput>> FindType(GetListTypeForJournalTransactionInput input)
        {
            var lst = Enum.GetValues(typeof(InventoryTransactionType))
                       .Cast<InventoryTransactionType>()
                       .Select(d => new { Id = (int)d, Name = d.ToString() })
                        .Where(d => d.Id != 6 && d.Id != 12)
                       .ToList();
            var results = lst.WhereIf(input.Filter != null && !string.IsNullOrWhiteSpace(input.Filter), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                         .Select(s => new GetListTypeForJournalTransactionOutput
                         {
                             Id = s.Id.ToString(),
                             Name = s.Id == 1 ? $"{L(s.Name)} ({L("Purchase")})" : s.Id == 3 || s.Id == 9 ? $"{L(s.Name)} ({L("Transfer")})" : s.Id == 15 || s.Id == 16 ? $"{L(s.Name)} ({L("Production")})" :
                             s.Id == 4 || s.Id == 10 ? $"{L(s.Name)} ({L("Adjustment")})" : s.Id == 5 || s.Id == 11 ? $"{L(s.Name)} ({L("Other")})" : s.Id == 2 ? $"{L(s.Name)} ({L("SaleReturn")})" :
                             s.Id == 14 || s.Id == 13 ? $"{L(s.Name)} ({L("PhysicalCount")})" : s.Id == 7 ? $"{L(s.Name)} ({L("Sale")})" : s.Id == 17  ?  $"{L(s.Name)} ({L("KitchenOrder")})" : $"{L(s.Name)} ({L("PurchaseReturn")})"
                         })
                         .ToList();
            return new PagedResultDto<GetListTypeForJournalTransactionOutput>(lst.Count(), results);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventorys)]
        public async Task<PagedResultDto<GetListInentoryTypeOutput>> FindInventoryType()
        {
            var results = Enum.GetValues(typeof(InventoryType))
                            .Cast<InventoryType>()
                            .Select(v => new GetListInentoryTypeOutput
                            { 
                             Id = (int)v,
                             Name = v.ToString(),
                            })
                            .ToList();
            return new PagedResultDto<GetListInentoryTypeOutput>(results.Count(), results);
        }

     [AbpAuthorize(AppPermissions.Pages_Tenant_Inventorys)]
      public async Task<GetJournalTransactionTypeOutput> GetTransactionType(GetInventoryTypeNameInput input)
        {
            var query = await _journalTransactionTransactionRepository.GetAll().AsNoTracking().Where(t => t.InventoryTransactionType == input.InventoryTransactionType && t.IsDefault).Select(j => new GetJournalTransactionTypeOutput
            {
                Active = j.Active,
                Id = j.Id,
                InventoryTransactionType = j.InventoryTransactionType,
                IsDefault = j.IsDefault,
                IsIssue = j.IsIssue,
                Name = j.Name,
                InventoryTransactionTypeName = j.InventoryTransactionType.ToString(),
            }).FirstOrDefaultAsync();
            return query;
        }
    }
}
