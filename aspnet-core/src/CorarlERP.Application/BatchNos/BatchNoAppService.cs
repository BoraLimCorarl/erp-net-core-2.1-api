using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Internal;
using Abp.Collections.Extensions;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.Extensions;
using CorarlERP.BatchNos.Dto;
using Abp.AutoMapper;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.Productions;
using Nito.AsyncEx;
using CorarlERP.InventoryCostCloses;
using IdentityServer4.Configuration;

namespace CorarlERP.BatchNos
{
    public class BatchNoAppService : CorarlERPAppServiceBase, IBatchNoAppService
    {

        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<BatchNo, Guid> _batchNoRepository;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly IRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly IRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        private readonly IRepository<InventoryCostCloseItemBatchNo, Guid> _inventoryCostCloseItemBatchNoRepository;
        public BatchNoAppService(
            IRepository<Item, Guid> itemRepository,
            IRepository<BatchNo, Guid> batchNoRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            IRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            IRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            IRepository<InventoryCostCloseItemBatchNo, Guid> inventoryCostCloseItemBatchNoRepository
            )
        {
            _itemRepository = itemRepository;
            _batchNoRepository = batchNoRepository;
            _journalRepository = journalRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _inventoryCostCloseItemBatchNoRepository = inventoryCostCloseItemBatchNoRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Common_BatchNo_Find)]
        public async Task<PagedResultDto<BatchNoDetailOutput>> Find(GetBatchNoListInput input)
        {
            var query = _batchNoRepository.GetAll()
                        .Where(s => s.ItemId == input.ItemId)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Code.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(input.isStandard.HasValue, s => s.IsStandard == input.isStandard)
                        .AsNoTracking()
                        .Select(s => new BatchNoDetailOutput
                        {
                            Id = s.Id,
                            Code = s.Code,
                            ItemId = s.ItemId,
                            ExpirationDate = s.ExpirationDate
                        });

            var count = await query.CountAsync();
            var items = new List<BatchNoDetailOutput>();

            if (count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }



            if (input.SelectedValue.HasValue)
            {
                var find = items.FirstOrDefault(s => s.Id == input.SelectedValue.Value);
                if (find == null) {
                    var selected = await _batchNoRepository.GetAll()
                               .AsNoTracking()
                               .Where(s => s.Id == input.SelectedValue.Value)
                               .Select(s => new BatchNoDetailOutput
                               {
                                   Id = s.Id,
                                   Code = s.Code,
                                   ItemId = s.ItemId,
                               })
                               .FirstOrDefaultAsync();
                    if (selected != null) items.Add(selected);
                }
            }

            return new PagedResultDto<BatchNoDetailOutput> { Items = items, TotalCount = count };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Common_BatchNo_Find)]
        public async Task<ListResultDto<GenerateBatchNoOutput>> GenerateBatchNo(GenerateBatchNoInput input)
        {
            if (input.ItemIds == null || !input.ItemIds.Any()) return new ListResultDto<GenerateBatchNoOutput>();

            var itemUseBatchs = await _itemRepository.GetAll().Include(s => s.BatchNoFormula).AsNoTracking().Where(s => s.UseBatchNo && s.AutoBatchNo && input.ItemIds.Contains(s.Id)).ToListAsync();

            if (!itemUseBatchs.Any()) return new ListResultDto<GenerateBatchNoOutput>();

            var items = itemUseBatchs.Select(s => {

                var formula = s.BatchNoFormula;
                if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $", Item: {s.ItemCode}-{s.ItemName}");

                var field = s.GetType().GetProperty(formula.FieldName).GetValue(s, null);
                if (field == null) throw new UserFriendlyException(L("IsNotValid", formula.FieldName));

                var code = field.ToString();

                var batchNumber = $"{code}-{input.Date.ToString(formula.DateFormat)}";

                if (input.Standard)
                {
                    if (formula.StandardPrePos == BatchNoFormulaPrePos.Prefix)
                    {
                        batchNumber = $"{formula.PrePosCode}-{code}-{input.Date.ToString(formula.DateFormat)}";
                    }
                    else if (formula.StandardPrePos == BatchNoFormulaPrePos.Postfix)
                    {
                        batchNumber = $"{code}-{input.Date.ToString(formula.DateFormat)}-{formula.PrePosCode}";
                    }
                }

                var item = new GenerateBatchNoOutput {
                    ItemId = s.Id,
                    BatchNumber = batchNumber
                };

                return item;

            }).ToList();

            return new ListResultDto<GenerateBatchNoOutput> { Items = items };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Common_BatchNo_Find)]
        public async Task<PagedResultDto<FindBatchNoOutput>> FindBatchNoForIssue(FindBatchNoInput input)
        {
            var batchNos = new List<string>();
            if(!input.Filter.IsNullOrWhiteSpace() && input.Filter.Contains(","))
            {
                batchNos = input.Filter.ToLower()
                                .Split(',')
                                .Select(s => s.Trim())
                                .Where(s => !s.IsNullOrWhiteSpace())
                                .ToList();
                input.Filter = string.Empty;
            }

            var tenant = await GetCurrentTenantAsync();
            var fromDate = tenant.AccountCycle != null && tenant.AccountCycle.StartDate != null ? tenant.AccountCycle.StartDate : DateTime.MinValue;

            IQueryable<FindBatchNoOutput> closeQuery = null;

            if (!input.ProductionPlanId.HasValue || input.ProductionPlanId == Guid.Empty)
            {
                var closeAccount = await GetPreviousCloseCyleAsync(input.ToDate);
                if (closeAccount != null)
                {
                    fromDate = closeAccount.EndDate.Value.AddDays(1);

                    closeQuery = _inventoryCostCloseItemBatchNoRepository.GetAll()
                                                   .Where(t => t.InventoryCostClose.CloseDate.Date == closeAccount.EndDate.Value.Date)
                                                   .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Lot.LocationId))
                                                   .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.LotId))
                                                   .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.InventoryCostClose.ItemId))
                                                   .WhereIf(input.IsStandard.HasValue, s => s.BatchNo.IsStandard == input.IsStandard.Value)
                                                   .WhereIf(batchNos.Any(), s => batchNos.Contains(s.BatchNo.Code.ToLower()))
                                                   .WhereIf(input.IsSerial.HasValue, s => s.BatchNo.IsSerial == input.IsSerial.Value)
                                                   .WhereIf(input.TrackExpiration.HasValue, s => s.InventoryCostClose.Item.TrackExpiration ==  input.TrackExpiration.Value)
                                                   .WhereIf(input.ExpiredFrom.HasValue, s => s.BatchNo.ExpirationDate.HasValue && input.ExpiredFrom.Value.Date <= s.BatchNo.ExpirationDate)
                                                   .WhereIf(input.ExpiredTo.HasValue, s => s.BatchNo.ExpirationDate.HasValue && s.BatchNo.ExpirationDate <= input.ExpiredTo.Value.Date)
                                                   .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                                       s.InventoryCostClose.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                                       s.InventoryCostClose.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                                       (!s.InventoryCostClose.Item.Barcode.IsNullOrEmpty() && s.InventoryCostClose.Item.Barcode.ToLower().Contains(input.Filter.ToLower())) ||
                                                       s.BatchNo.Code.ToLower().Contains(input.Filter.ToLower())
                                                   )
                                                   .AsNoTracking()
                                                   .Select(s => new FindBatchNoOutput
                                                   {
                                                       BatchNoId = s.BatchNoId,
                                                       BatchNumber = s.BatchNo.Code,
                                                       ExpirationDate = s.BatchNo.ExpirationDate,
                                                       ItemId = s.InventoryCostClose.ItemId,
                                                       ItemCode = s.InventoryCostClose.Item.ItemCode,
                                                       ItemName = s.InventoryCostClose.Item.ItemName,
                                                       ItemTypeId = s.InventoryCostClose.Item.ItemTypeId,
                                                       LotId = s.LotId,
                                                       LotName = s.Lot.LotName,
                                                       ReceiptQty = s.Qty,
                                                       IssueQty = 0,
                                                       BalanceQty = s.Qty,

                                                       SalePrice = s.InventoryCostClose.Item.SalePrice,
                                                       SaleTaxId = s.InventoryCostClose.Item.SaleTaxId,
                                                       SaleTaxRate = s.InventoryCostClose.Item.SaleTaxId.HasValue ? s.InventoryCostClose.Item.SaleTax.TaxRate : 0,
                                                       SaleAccountId = s.InventoryCostClose.Item.SaleAccountId,
                                                       PurchaseTaxId = s.InventoryCostClose.Item.PurchaseTaxId,
                                                       PurchaseAccountId = s.InventoryCostClose.Item.PurchaseAccountId,
                                                       InventoryAccountId = s.InventoryCostClose.Item.InventoryAccountId,
                                                       InventoryAccountCode = s.InventoryCostClose.Item.InventoryAccount.AccountCode,
                                                       InventoryAccountName = s.InventoryCostClose.Item.InventoryAccount.AccountName,
                                                       SaleAccountCode = s.InventoryCostClose.Item.SaleAccount.AccountName,
                                                       SaleAccountName = s.InventoryCostClose.Item.SaleAccount.AccountName,
                                                       PurchaseAccountCode = s.InventoryCostClose.Item.PurchaseAccount.AccountCode,
                                                       PurchaseAccountName = s.InventoryCostClose.Item.PurchaseAccount.AccountName
                                                   });

                }
            }

            var itemReceiptBatch = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                             .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.BatchNo.ItemId))
                                             .WhereIf(input.IsStandard.HasValue, s => s.BatchNo.IsStandard == input.IsStandard.Value)
                                             .WhereIf(batchNos.Any(), s => batchNos.Contains(s.BatchNo.Code.ToLower()))
                                             .WhereIf(input.IsSerial.HasValue, s => s.BatchNo.IsSerial == input.IsSerial.Value)
                                             .WhereIf(input.ExpiredFrom.HasValue, s => s.BatchNo.ExpirationDate.HasValue && input.ExpiredFrom.Value.Date <= s.BatchNo.ExpirationDate)
                                             .WhereIf(input.ExpiredTo.HasValue, s => s.BatchNo.ExpirationDate.HasValue && s.BatchNo.ExpirationDate <= input.ExpiredTo.Value.Date)
                                             .AsNoTracking()

                                   join ri in _itemReceiptItemRepository.GetAll()
                                              .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Lot.LocationId))
                                              .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.LotId.Value))
                                              .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemId))
                                              .WhereIf(input.TrackExpiration.HasValue, s => s.Item.TrackExpiration ==  input.TrackExpiration.Value)
                                              .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemReceiptId))
                                              .WhereIf(input.ProductionPlanId.HasValue && input.ProductionPlanId != Guid.Empty, s => s.ItemReceipt.ProductionOrderId.HasValue && s.ItemReceipt.ProductionOrder.ProductionPlanId == input.ProductionPlanId)
                                              .AsNoTracking()
                                   on b.ItemReceiptItemId equals ri.Id

                                   join j in _journalRepository.GetAll()
                                            .Where(s => s.ItemReceiptId.HasValue)
                                            .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                            .Where(s => s.Date.Date <= input.ToDate.Date)
                                            .Where(s => fromDate.Date <= s.Date.Date)
                                            .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemReceiptId.Value))
                                            .AsNoTracking()
                                   on ri.ItemReceiptId equals j.ItemReceiptId

                                   where input.Filter.IsNullOrWhiteSpace() || (
                                         ri.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                         ri.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                         (!ri.Item.Barcode.IsNullOrEmpty() && ri.Item.Barcode.ToLower().Contains(input.Filter.ToLower())) ||
                                         b.BatchNo.Code.ToLower().Contains(input.Filter.ToLower()))

                                   select new FindBatchNoOutput
                                   {
                                       BatchNoId = b.BatchNoId,
                                       BatchNumber = b.BatchNo.Code,
                                       ExpirationDate = b.BatchNo.ExpirationDate,
                                       ItemId = ri.ItemId,
                                       ItemCode = ri.Item.ItemCode,
                                       ItemName = ri.Item.ItemName,
                                       ItemTypeId = ri.Item.ItemTypeId,
                                       LotId = ri.LotId.Value,
                                       LotName = ri.Lot.LotName,
                                       ReceiptQty = b.Qty,
                                       IssueQty = 0,
                                       BalanceQty = b.Qty,


                                       SalePrice = ri.Item.SalePrice,
                                       SaleTaxId = ri.Item.SaleTaxId,
                                       SaleTaxRate = ri.Item.SaleTaxId.HasValue ? ri.Item.SaleTax.TaxRate : 0,
                                       SaleAccountId = ri.Item.SaleAccountId,
                                       PurchaseTaxId = ri.Item.PurchaseTaxId,
                                       PurchaseAccountId = ri.Item.PurchaseAccountId,
                                       InventoryAccountId = ri.Item.InventoryAccountId,
                                       InventoryAccountCode = ri.Item.InventoryAccount.AccountCode,
                                       InventoryAccountName = ri.Item.InventoryAccount.AccountName,
                                       SaleAccountCode = ri.Item.SaleAccount.AccountName,
                                       SaleAccountName = ri.Item.SaleAccount.AccountName,
                                       PurchaseAccountCode = ri.Item.PurchaseAccount.AccountCode,
                                       PurchaseAccountName = ri.Item.PurchaseAccount.AccountName
                                   };

            var itemIssueBatch = from b in _itemIssueItemBatchNoRepository.GetAll()
                                           .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.BatchNo.ItemId))
                                           .WhereIf(input.IsStandard.HasValue, s => s.BatchNo.IsStandard == input.IsStandard.Value)
                                           .WhereIf(batchNos.Any(), s => batchNos.Contains(s.BatchNo.Code.ToLower()))
                                           .WhereIf(input.IsSerial.HasValue, s => s.BatchNo.IsSerial == input.IsSerial.Value)
                                           .WhereIf(input.ExpiredFrom.HasValue, s => s.BatchNo.ExpirationDate.HasValue && input.ExpiredFrom.Value.Date <= s.BatchNo.ExpirationDate)
                                           .WhereIf(input.ExpiredTo.HasValue, s => s.BatchNo.ExpirationDate.HasValue && s.BatchNo.ExpirationDate <= input.ExpiredTo.Value.Date)
                                           .AsNoTracking()

                                 join ri in _itemIssueItemRepository.GetAll()
                                            .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Lot.LocationId))
                                            .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.LotId.Value))
                                            .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemId))
                                            .WhereIf(input.TrackExpiration.HasValue, s => s.Item.TrackExpiration ==  input.TrackExpiration.Value)
                                            .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemIssueId))
                                            .WhereIf(input.ProductionPlanId.HasValue && input.ProductionPlanId != Guid.Empty, s => s.ItemIssue.ProductionOrderId.HasValue && s.ItemIssue.ProductionOrder.ProductionPlanId == input.ProductionPlanId)
                                            .AsNoTracking()
                                 on b.ItemIssueItemId equals ri.Id

                                 join j in _journalRepository.GetAll()
                                          .Where(s => s.ItemIssueId.HasValue)
                                          .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                          .Where(s => s.Date.Date <= input.ToDate.Date)
                                          .Where(s => fromDate.Date <= s.Date.Date)
                                          .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemIssueId.Value))
                                          .AsNoTracking()
                                 on ri.ItemIssueId equals j.ItemIssueId

                                 where input.Filter.IsNullOrWhiteSpace() || (
                                       ri.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                       ri.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                       (!ri.Item.Barcode.IsNullOrEmpty() && ri.Item.Barcode.ToLower().Contains(input.Filter.ToLower())) ||
                                       b.BatchNo.Code.ToLower().Contains(input.Filter.ToLower()))

                                 select new FindBatchNoOutput
                                 {
                                     BatchNoId = b.BatchNoId,
                                     BatchNumber = b.BatchNo.Code,
                                     ExpirationDate = b.BatchNo.ExpirationDate,
                                     ItemId = ri.ItemId,
                                     ItemCode = ri.Item.ItemCode,
                                     ItemName = ri.Item.ItemName,
                                     ItemTypeId = ri.Item.ItemTypeId,
                                     LotId = ri.LotId.Value,
                                     LotName = ri.Lot.LotName,
                                     ReceiptQty = 0,
                                     IssueQty = b.Qty,
                                     BalanceQty = -b.Qty,

                                     SalePrice = ri.Item.SalePrice,
                                     SaleTaxId = ri.Item.SaleTaxId,
                                     SaleTaxRate = ri.Item.SaleTaxId.HasValue ? ri.Item.SaleTax.TaxRate : 0,
                                     SaleAccountId = ri.Item.SaleAccountId,
                                     PurchaseTaxId = ri.Item.PurchaseTaxId,
                                     PurchaseAccountId = ri.Item.PurchaseAccountId,
                                     InventoryAccountId = ri.Item.InventoryAccountId,
                                     InventoryAccountCode = ri.Item.InventoryAccount.AccountCode,
                                     InventoryAccountName = ri.Item.InventoryAccount.AccountName,
                                     SaleAccountCode = ri.Item.SaleAccount.AccountName,
                                     SaleAccountName = ri.Item.SaleAccount.AccountName,
                                     PurchaseAccountCode = ri.Item.PurchaseAccount.AccountCode,
                                     PurchaseAccountName = ri.Item.PurchaseAccount.AccountName
                                 };

            var allQuery = itemReceiptBatch
                           .Concat(itemIssueBatch);

            if (!input.ProductionPlanId.HasValue || input.ProductionPlanId == Guid.Empty)
            {
                var itemIssueReturnBatch = from b in _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                                .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.BatchNo.ItemId))
                                                .WhereIf(input.IsStandard.HasValue, s => s.BatchNo.IsStandard == input.IsStandard.Value)
                                                .WhereIf(batchNos.Any(), s => batchNos.Contains(s.BatchNo.Code.ToLower()))
                                                .WhereIf(input.IsSerial.HasValue, s => s.BatchNo.IsSerial == input.IsSerial.Value)
                                                .WhereIf(input.ExpiredFrom.HasValue, s => s.BatchNo.ExpirationDate.HasValue && input.ExpiredFrom.Value.Date <= s.BatchNo.ExpirationDate)
                                                .WhereIf(input.ExpiredTo.HasValue, s => s.BatchNo.ExpirationDate.HasValue && s.BatchNo.ExpirationDate <= input.ExpiredTo.Value.Date)
                                                .AsNoTracking()

                                           join ri in _itemIssueVendorCreditItemRepository.GetAll()
                                                      .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Lot.LocationId))
                                                      .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.LotId.Value))
                                                      .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemId))
                                                      .WhereIf(input.TrackExpiration.HasValue, s => s.Item.TrackExpiration ==  input.TrackExpiration.Value)
                                                      .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemIssueVendorCreditId))
                                                      .AsNoTracking()
                                           on b.ItemIssueVendorCreditItemId equals ri.Id

                                           join j in _journalRepository.GetAll()
                                                    .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                    .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                    .Where(s => s.Date.Date <= input.ToDate.Date)
                                                    .Where(s => fromDate.Date <= s.Date.Date)
                                                    .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemIssueVendorCreditId.Value))
                                                    .AsNoTracking()
                                           on ri.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId

                                           where input.Filter.IsNullOrWhiteSpace() || (
                                           ri.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                           ri.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                           (!ri.Item.Barcode.IsNullOrEmpty() && ri.Item.Barcode.ToLower().Contains(input.Filter.ToLower())) ||
                                           b.BatchNo.Code.ToLower().Contains(input.Filter.ToLower()))

                                           select new FindBatchNoOutput
                                           {
                                               BatchNoId = b.BatchNoId,
                                               BatchNumber = b.BatchNo.Code,
                                               ExpirationDate = b.BatchNo.ExpirationDate,
                                               ItemId = ri.ItemId,
                                               ItemCode = ri.Item.ItemCode,
                                               ItemName = ri.Item.ItemName,
                                               ItemTypeId = ri.Item.ItemTypeId,
                                               LotId = ri.LotId.Value,
                                               LotName = ri.Lot.LotName,
                                               ReceiptQty = 0,
                                               IssueQty = b.Qty,
                                               BalanceQty = -b.Qty,

                                               SalePrice = ri.Item.SalePrice,
                                               SaleTaxId = ri.Item.SaleTaxId,
                                               SaleTaxRate = ri.Item.SaleTaxId.HasValue ? ri.Item.SaleTax.TaxRate : 0,
                                               SaleAccountId = ri.Item.SaleAccountId,
                                               PurchaseTaxId = ri.Item.PurchaseTaxId,
                                               PurchaseAccountId = ri.Item.PurchaseAccountId,
                                               InventoryAccountId = ri.Item.InventoryAccountId,
                                               InventoryAccountCode = ri.Item.InventoryAccount.AccountCode,
                                               InventoryAccountName = ri.Item.InventoryAccount.AccountName,
                                               SaleAccountCode = ri.Item.SaleAccount.AccountName,
                                               SaleAccountName = ri.Item.SaleAccount.AccountName,
                                               PurchaseAccountCode = ri.Item.PurchaseAccount.AccountCode,
                                               PurchaseAccountName = ri.Item.PurchaseAccount.AccountName
                                           };

                var itemReciptReturnBatch = from b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.BatchNo.ItemId))
                                                .WhereIf(input.IsStandard.HasValue, s => s.BatchNo.IsStandard == input.IsStandard.Value)
                                                .WhereIf(batchNos.Any(), s => batchNos.Contains(s.BatchNo.Code.ToLower()))
                                                .WhereIf(input.IsSerial.HasValue, s => s.BatchNo.IsSerial == input.IsSerial.Value)
                                                .WhereIf(input.ExpiredFrom.HasValue, s => s.BatchNo.ExpirationDate.HasValue && input.ExpiredFrom.Value.Date <= s.BatchNo.ExpirationDate)
                                                .WhereIf(input.ExpiredTo.HasValue, s => s.BatchNo.ExpirationDate.HasValue && s.BatchNo.ExpirationDate <= input.ExpiredTo.Value.Date)
                                                .AsNoTracking()

                                            join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                       .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.Lot.LocationId))
                                                       .WhereIf(input.Lots != null && input.Lots.Any(), s => input.Lots.Contains(s.LotId.Value))
                                                       .WhereIf(input.Items != null && input.Items.Any(), s => input.Items.Contains(s.ItemId))
                                                       .WhereIf(input.TrackExpiration.HasValue, s => s.Item.TrackExpiration ==  input.TrackExpiration.Value)
                                                       .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemReceiptCustomerCreditId))
                                                       .AsNoTracking()
                                            on b.ItemReceiptCustomerCreditItemId equals ri.Id

                                            join j in _journalRepository.GetAll()
                                                     .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                     .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                     .Where(s => s.Date.Date <= input.ToDate.Date)
                                                     .Where(s => fromDate.Date <= s.Date.Date)
                                                     .WhereIf(input.ExceptIds != null && input.ExceptIds.Any(), s => !input.ExceptIds.Contains(s.ItemReceiptCustomerCreditId.Value))
                                                     .AsNoTracking()
                                            on ri.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId

                                            where input.Filter.IsNullOrWhiteSpace() || (
                                            ri.Item.ItemCode.ToLower().Contains(input.Filter.ToLower()) ||
                                            ri.Item.ItemName.ToLower().Contains(input.Filter.ToLower()) ||
                                            (!ri.Item.Barcode.IsNullOrEmpty() && ri.Item.Barcode.ToLower().Contains(input.Filter.ToLower())) ||
                                            b.BatchNo.Code.ToLower().Contains(input.Filter.ToLower()))

                                            select new FindBatchNoOutput
                                            {
                                                BatchNoId = b.BatchNoId,
                                                BatchNumber = b.BatchNo.Code,
                                                ExpirationDate = b.BatchNo.ExpirationDate,
                                                ItemId = ri.ItemId,
                                                ItemCode = ri.Item.ItemCode,
                                                ItemName = ri.Item.ItemName,
                                                ItemTypeId = ri.Item.ItemTypeId,
                                                LotId = ri.LotId.Value,
                                                LotName = ri.Lot.LotName,
                                                ReceiptQty = b.Qty,
                                                IssueQty = 0,
                                                BalanceQty = b.Qty,

                                                SalePrice = ri.Item.SalePrice,
                                                SaleTaxId = ri.Item.SaleTaxId,
                                                SaleTaxRate = ri.Item.SaleTaxId.HasValue ? ri.Item.SaleTax.TaxRate : 0,
                                                SaleAccountId = ri.Item.SaleAccountId,
                                                PurchaseTaxId = ri.Item.PurchaseTaxId,
                                                PurchaseAccountId = ri.Item.PurchaseAccountId,
                                                InventoryAccountId = ri.Item.InventoryAccountId,
                                                InventoryAccountCode = ri.Item.InventoryAccount.AccountCode,
                                                InventoryAccountName = ri.Item.InventoryAccount.AccountName,
                                                SaleAccountCode = ri.Item.SaleAccount.AccountName,
                                                SaleAccountName = ri.Item.SaleAccount.AccountName,
                                                PurchaseAccountCode = ri.Item.PurchaseAccount.AccountCode,
                                                PurchaseAccountName = ri.Item.PurchaseAccount.AccountName
                                            };

                allQuery = itemReceiptBatch
                           .Concat(itemIssueBatch)
                           .Concat(itemIssueReturnBatch)
                           .Concat(itemReciptReturnBatch);

            }


            if (closeQuery != null) allQuery = closeQuery.Concat(allQuery);

            var query = allQuery
                        .OrderBy(input.Sorting)
                        .GroupBy(g => new { 
                            g.BatchNoId,
                            g.BatchNumber,
                            g.ExpirationDate,
                            g.ItemId,
                            g.ItemCode,
                            g.ItemName,
                            g.ItemTypeId,
                            g.LotId,
                            g.LotName,
                            g.SaleAccountId,
                            g.SaleTaxId,
                            g.SaleTaxRate,
                            g.SalePrice,
                            g.PurchaseTaxId,
                            g.PurchaseAccountId,
                            g.InventoryAccountId,
                            g.InventoryAccountCode,
                            g.InventoryAccountName,
                            g.PurchaseAccountCode,
                            g.PurchaseAccountName,
                            g.SaleAccountCode, 
                            g.SaleAccountName,
                        })
                        .Select(s => new FindBatchNoOutput
                        {
                            BatchNoId = s.Key.BatchNoId,
                            BatchNumber = s.Key.BatchNumber,
                            ExpirationDate = s.Key.ExpirationDate,
                            ItemId = s.Key.ItemId,
                            ItemCode = s.Key.ItemCode,
                            ItemName = s.Key.ItemName,
                            ItemTypeId = s.Key.ItemTypeId,
                            LotId = s.Key.LotId,
                            LotName = s.Key.LotName,
                            SaleAccountId = s.Key.SaleAccountId,
                            SaleTaxId = s.Key.SaleTaxId,
                            SaleTaxRate = s.Key.SaleTaxRate,
                            SalePrice = s.Key.SalePrice,
                            PurchaseTaxId = s.Key.PurchaseTaxId,
                            PurchaseAccountId = s.Key.PurchaseAccountId,
                            InventoryAccountId = s.Key.InventoryAccountId,
                            InventoryAccountCode = s.Key.InventoryAccountCode,
                            InventoryAccountName = s.Key.InventoryAccountName,
                            SaleAccountCode = s.Key.SaleAccountCode,
                            SaleAccountName = s.Key.SaleAccountName,
                            PurchaseAccountCode = s.Key.PurchaseAccountCode,
                            PurchaseAccountName = s.Key.PurchaseAccountName,

                            ReceiptQty = s.Sum(t => t.ReceiptQty),
                            IssueQty = s.Sum(t => t.IssueQty),
                            BalanceQty = s.Sum(t => t.ReceiptQty - t.IssueQty)
                        })
                        .WhereIf(input.State.HasValue, s =>
                            (input.State == BatchNoState.NotInstock && s.BalanceQty == 0) ||
                            (input.State == BatchNoState.Available && s.BalanceQty > 0) ||
                            (input.State == BatchNoState.NegativeStock && s.BalanceQty < 0));

            var count = await query.CountAsync();
            var items = new List<FindBatchNoOutput>();
            if(count > 0)
            {
                items = await query.PageBy(input).ToListAsync();
            }

            return new PagedResultDto<FindBatchNoOutput> { TotalCount = count, Items = items };
        }
    }
}
