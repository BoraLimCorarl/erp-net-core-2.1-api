using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.TransactionTypes;
using CorarlERP.TransactionTypes.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using System;
using CorarlERP.InvoiceTemplates.Dto;
using System.IO;
using System.Text;
using Abp.Timing;
using Abp.Domain.Uow;
using System.Transactions;
using CorarlERP.FileUploads;
using CorarlERP.FileStorages;

namespace CorarlERP.InvoiceTemplates
{
    [AbpAuthorize]
    public class InvoiceTemplateAppService : CorarlERPAppServiceBase, IInvoiceTemplateAppService
    {
        private readonly IRepository<TransactionType, long> _transactionTypeRepository;
        private readonly IInvoiceTemplateManager _invoiceTemplateManager;
        private readonly IRepository<InvoiceTemplate, Guid> _invoiceTemplateRepository;
        private readonly IInvoiceTemplateMapManager _invoiceTemplateMapManager;
        private readonly ICorarlRepository<InvoiceTemplateMap, Guid> _invoiceTemplateMapRepository;
        protected readonly AppFolders _appFolders;
        private readonly IFileUploadManager _fileUploadManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IFileStorageManager _fileStorageManager;

        public InvoiceTemplateAppService(
            IRepository<TransactionType, long> transactionTypeRepository,
            IUnitOfWorkManager unitOfWorkManager,
            AppFolders appFolders,
            IFileUploadManager fileUploadManager,
            IFileStorageManager fileStorageManager,
            IInvoiceTemplateManager invoiceTemplateManager,
            IRepository<InvoiceTemplate, Guid> invoiceTemplateRepository,
            IInvoiceTemplateMapManager invoiceTemplateMapManager,
            ICorarlRepository<InvoiceTemplateMap, Guid> invoiceTemplateMapRepository)
        {
            _transactionTypeRepository = transactionTypeRepository;
            _appFolders = appFolders;
            _fileUploadManager = fileUploadManager;
            _invoiceTemplateManager = invoiceTemplateManager;
            _invoiceTemplateMapManager = invoiceTemplateMapManager;
            _invoiceTemplateRepository = invoiceTemplateRepository;
            _invoiceTemplateMapRepository = invoiceTemplateMapRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _fileStorageManager = fileStorageManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Create)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<InvoiceTemplateDto> Create(InvoiceTemplateDto input)
        {
            try
            {
                var result = await CreateHelper(input);
                return result;
            }
            catch (UserFriendlyException ex)
            {
                await DeleteGallery(input.GalleryId.Value);
                throw ex;
            }
            catch (Exception ex)
            {
                await DeleteGallery(input.GalleryId.Value);
                throw new UserFriendlyException(L("CannotCreate"));
            }
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<InvoiceTemplateDto> CreateHelper(InvoiceTemplateDto input)
        {
            await CheckTemplateMap(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = InvoiceTemplate.Create(tenantId, userId, input.Name, input.GalleryId.Value, input.TemplateOption, input.TemplateType, input.ShowDetail, input.ShowSummary);

            var templateMaps = new List<InvoiceTemplateMap>();
            if (input.SaleTypes != null && input.SaleTypes.Any())
            {
                foreach (var t in input.SaleTypes)
                {
                    var map = InvoiceTemplateMap.Create(tenantId, userId, input.TemplateType, t.Id, entity.Id);
                    templateMaps.Add(map);
                }
            }
            else
            {
                var map = InvoiceTemplateMap.Create(tenantId, userId, input.TemplateType, null, entity.Id);
                templateMaps.Add(map);
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    CheckErrors(await _invoiceTemplateManager.CreateAsync(@entity));
                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    await _invoiceTemplateMapRepository.BulkInsertAsync(templateMaps);
                }
                await uow.CompleteAsync();
            }

            return ObjectMapper.Map<InvoiceTemplateDto>(@entity);
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task CheckTemplateMap(InvoiceTemplateDto input)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    var mapList = await _invoiceTemplateMapRepository.GetAll()
                               .Where(s => s.TemplateId != input.Id)
                               .Where(s => s.TemplateType == input.TemplateType)
                               .Where(s =>
                                    (!s.SaleTypeId.HasValue && (input.SaleTypes == null || !input.SaleTypes.Any())) ||
                                    (s.SaleTypeId.HasValue && (input.SaleTypes != null && input.SaleTypes.Any(t => t.Id == s.SaleTypeId))))
                               .AsNoTracking()
                               .ToListAsync();

                    if (mapList.Any()) throw new UserFriendlyException(L("DuplicateTemplate"));
                }
            }
        }


        [UnitOfWork(IsDisabled = true)]
        private async Task DeleteGallery(Guid galleryId)
        {
            try
            {
                await _fileUploadManager.Delete(AbpSession.TenantId, galleryId);
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(L(ex.Message));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(L("CannotDelete"));
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Update)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<InvoiceTemplateDto> Update(InvoiceTemplateDto input)
        {
            InvoiceTemplate entity = null;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    entity = await _invoiceTemplateManager.GetAsync(input.Id.Value, true);                    
                }
            }

            if (entity == null)
            {
                if (input.GalleryId.HasValue) await DeleteGallery(input.GalleryId.Value);
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var oldGalleryId = entity.GalleryId;
            var success = false;

            try
            {
                entity = await UpdateHelper(input, entity);
                success = true;
            }
            catch (UserFriendlyException ex)
            {
                if (input.GalleryId.HasValue) await DeleteGallery(input.GalleryId.Value);
                throw ex;
            }
            catch (Exception ex)
            {
                if (input.GalleryId.HasValue) await DeleteGallery(input.GalleryId.Value);
                throw new UserFriendlyException("CannotUpdate");
            }
            finally
            {
                if (success) await DeleteGallery(oldGalleryId);
            }

            return ObjectMapper.Map<InvoiceTemplateDto>(@entity);
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<InvoiceTemplate> UpdateHelper(InvoiceTemplateDto input, InvoiceTemplate entity)
        {
            await CheckTemplateMap(input);

            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            entity.Update(userId, input.Name, input.GalleryId.Value, input.TemplateOption, input.TemplateType, input.ShowDetail, input.ShowSummary);

            var mapList = new List<InvoiceTemplateMap>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    mapList = await _invoiceTemplateMapRepository.GetAll()
                                .Where(s => s.TemplateId == input.Id)
                                .ToListAsync();
                }
            }

            var addTemplateMaps = new List<InvoiceTemplateMap>();
            var deleteTemplateMaps = new List<InvoiceTemplateMap>();

            if (input.SaleTypes != null && input.SaleTypes.Any())
            {
                deleteTemplateMaps = mapList.Where(s => !s.SaleTypeId.HasValue || !input.SaleTypes.Any(t => t.Id == s.SaleTypeId)).ToList();

                var addList = input.SaleTypes.Where(t => !mapList.Any(m => m.SaleTypeId == t.Id)).ToList();
                foreach (var t in addList)
                {
                    var map = InvoiceTemplateMap.Create(tenantId, userId, input.TemplateType, t.Id, entity.Id);
                }
            }
            else
            {
                var map = mapList.Where(s => !s.SaleTypeId.HasValue).FirstOrDefault();
                if (map == null)
                {
                    map = InvoiceTemplateMap.Create(tenantId, userId, input.TemplateType, null, entity.Id);
                }

                deleteTemplateMaps = mapList.Where(s => s.SaleTypeId.HasValue).ToList();
            }


            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    CheckErrors(await _invoiceTemplateManager.UpdateAsync(entity));
                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    if (addTemplateMaps.Any()) await _invoiceTemplateMapRepository.BulkInsertAsync(addTemplateMaps);
                    if (deleteTemplateMaps.Any()) await _invoiceTemplateMapRepository.BulkDeleteAsync(deleteTemplateMaps);
                }
                await uow.CompleteAsync();
            }

            return entity;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Delete)]
        [UnitOfWork(IsDisabled = true)]
        public async Task Delete(EntityDto<Guid> input)
        {
            Guid? galleryId = null;
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    var entity = await _invoiceTemplateManager.GetAsync(input.Id, true);
                    if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

                    var templateMaps = await _invoiceTemplateMapRepository.GetAll().Where(s => s.TemplateId == input.Id).ToListAsync();
                    if (templateMaps.Any()) await _invoiceTemplateMapRepository.BulkDeleteAsync(templateMaps);

                    galleryId = entity.GalleryId;

                    CheckErrors(await _invoiceTemplateManager.DeleteAsync(@entity));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
                await uow.CompleteAsync();
            }

            if (galleryId.HasValue) await DeleteGallery(galleryId.Value);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _invoiceTemplateManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _invoiceTemplateManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _invoiceTemplateManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _invoiceTemplateManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates, AppPermissions.Pages_Tenant_Customer_SaleOrder)]
        public async Task<PagedResultDto<InvoiceTemplateDto>> Find(GetInvoiceTemplateListInput input)
        {
            var @query = _invoiceTemplateRepository
               .GetAll()
               .WhereIf(input.TemplateType.HasValue, p => p.TemplateType == input.TemplateType)
               .Where(p => p.IsActive)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.Name.ToLower().Contains(input.Filter.ToLower()))
               .AsNoTracking();

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<InvoiceTemplateDto>(resultCount, ObjectMapper.Map<List<InvoiceTemplateDto>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_GetDetail)]
        public async Task<InvoiceTemplateDto> GetDetail(EntityDto<Guid> input)
        {
            var entity = await _invoiceTemplateManager.GetAsync(input.Id, true);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var result = ObjectMapper.Map<InvoiceTemplateDto>(@entity);

            var saleTypes = await _invoiceTemplateMapRepository.GetAll()
                                    .Where(s => s.TemplateId == input.Id && s.SaleTypeId.HasValue)
                                    .AsNoTracking()
                                    .Select(s => new TransactionTypeSummaryOutput
                                    {
                                        Id = s.SaleTypeId.Value,
                                        TransactionTypeName = s.SaleType.TransactionTypeName,
                                    })
                                    .ToListAsync();

            result.SaleTypes = saleTypes;

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_GetList)]
        public async Task<PagedResultDto<InvoiceTemplateDto>> GetList(GetInvoiceTemplateListInput input)
        {
            var @query = from t in _invoiceTemplateRepository
                                   .GetAll()
                                   .WhereIf(input.TemplateType.HasValue, p => p.TemplateType == input.TemplateType)
                                   .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                   .WhereIf(
                                       !input.Filter.IsNullOrEmpty(),
                                       p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                                   .AsNoTracking()
                         join m in _invoiceTemplateMapRepository.GetAll()
                                   .Where(s => s.SaleTypeId.HasValue)
                                   .AsNoTracking()
                         on t.Id equals m.TemplateId
                         into maps
                         select new InvoiceTemplateDto
                         {
                             Id = t.Id,
                             Name = t.Name,
                             GalleryId = t.GalleryId,
                             IsActive = t.IsActive,
                             TemplateOption = t.TemplateOption,
                             ShowSummary = t.ShowSummary,
                             ShowDetail = t.ShowDetail,
                             TemplateType = t.TemplateType,
                             SaleTypes = maps == null ? new List<TransactionTypeSummaryOutput> () : 
                                         maps.Select(s => new TransactionTypeSummaryOutput
                                         {
                                             Id = s.SaleTypeId.Value,
                                             TransactionTypeName = s.SaleType.TransactionTypeName
                                         }).ToList()
                         };


            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<InvoiceTemplateDto>(resultCount, @entities);
        }

        public async Task<InvoiceTemplateResultOutput> GetDetailTemplateHtml(InvoiceTemplateInput input)
        {
            return await Task.Run(() =>
            {
                var templateHtml =
                @"<table id='detail-table' width='100%'>
                    <thead id='detail-header' class='repeat-header'>
                        <tr id='detail-header-row' class='bg-gray'>
                            <th id='column1' style='text-align:left;'>No</th>
                            <th id='column2' style='text-align:left;'>ItemCode</th>
                            <th id='column3' style='text-align:left;'>ItemName</th>
                            <th id='column4' style='text-align:left; display:none;'>Description</th>
                            <th id='column5' style='text-align:right;'>Qty</th>
                            <th id='column6' style='text-align:right;'>UnitPrice</th>
                            <th id='column7' style='text-align:right;'>Discount</th>
                            <th id='column8' style='text-align:right;'>Total</th>
                        </tr>
                    </thead>
                    <tbody id='detail-body'>    
                        <tr detail-body-row class='border-b-dot'>
                            <td column1 style='text-align:left;'>@No</td>
                            <td column2 style='text-align:left;'>@ItemCode</td>
                            <td column3 style='text-align:left;'>@ItemName</td>
                            <td column4 style='text-align:left;display:none;'>@Description</td>
                            <td column5 style='text-align:right;'>@Qty</td>
                            <td column6 style='text-align:right;'>@UnitPrice</td>
                            <td column7 style='text-align:right;'>@DiscountRate</td>
                            <td column8 style='text-align:right;'>@LineTotal</td>
                        </tr>
                    </tbody>
                    <tfoot id='datail-footer' style='display: table-row-group;'>
                        <tr id='detail-footer-row'>
                            <td id='footer-column1' colspan='4' class='border-b-none border-l-none' style='vertical-align: top;'>សម្គាល់: @Memo</td>
                            <td id='footer-column6' style='text-align:right; font-weight:bold; padding:0;'>
                                <span class='p-td' style='display:block;'>សរុប :</span>    
                                <span class='p-td' style='display:block;'>ប្រាក់ទទួល : </span>
                                <span class='p-td border-t bg-gray' style='display:block;'> សមតុល្យ : </span>
                            </td>
                            <td id='footer-column7' colspan='2' style='text-align:right; font-weight:bold; padding:0;'>
                                <span class='p-td' style='display:block;'>@SubTotal</span>
                                <span class='p-td' style='display:block;'>@PaymentAmount</span>
                                <span class='p-td border-t bg-gray' style='display:block;'>@Balance</span>
                            </td>
                        </tr>
                    </tfoot>
                </table>";

                return new InvoiceTemplateResultOutput
                {
                    Html = templateHtml
                };
            });
        }

        public async Task<InvoiceTemplateResultOutput> GetPaymentSummaryTemplateHtml(InvoiceTemplateInput input)
        {
            return await Task.Run(() =>
            {
                var templateHtml =
                @"<div id='payment-summary'>
                    <h5 id='payemtn-summary-title' style='font-weight: bold; font-size: 14px; margin-bottom: 0;'>របាយការណ៍ជំពាក់</h5>
                    <table id='payment-summary-table' width='100%'>
                        <tr id='payment-summary-header-row' class='bg-gray border-t-dot border-x-dot' style='font-weight:bold;'>
                            <th style='text-align:left; width: 100px;'>ថ្ងៃខែ</th>
                            <th style='text-align:left;'>អធិប្បាយ</th>
                            <th style='text-align:right;'>តម្លៃសរុប</th>
                        </tr>
                        <tbody payment-summary-body>
                            <tr payment-summary-item-row class='border-x-dot'>
                                <td>@Date</td>
                                <td>@Description</td>
                                <td style='text-align:right;'>@SummaryTotal</td>
                            </tr>
                            <tr payment-summary-group-row class='bg-gray-2 border-b-dot border-x-dot' style='font-weight:bold;'>
                                <td>សរុបជំពាក់</td>
                                <td>@GroupCurrency</td>
                                <td style='text-align:right;'>@GroupTotal</td>
                            </tr>
                        </tbody>
                    </table>
                </div>";

                return new InvoiceTemplateResultOutput
                {
                    Html = templateHtml
                };
            });
        }

        public async Task<InvoiceTemplateResultOutput> GetTemplateHtml(InvoiceTemplateInput input)
        {
            var templateHtml = await _fileStorageManager.GetTemplate(input.TemplateName);

            return new InvoiceTemplateResultOutput
            {
                Html = templateHtml
            };
        }

        public async Task<ListResultDto<DefaultTemplateOutput>> GetDefaultTemplateList(DefaultTemplateInput input)
        {
            return await Task.Run(() =>
            {
                var list = new List<DefaultTemplateOutput>
                {
                    new DefaultTemplateOutput { Name = "Standard Invoice", FileName = "invoiceTemplate.html", TemplateType = InvoiceTemplateType.Invoice },
                    new DefaultTemplateOutput { Name = "Standard Sale Order", FileName = "saleOrderTemplate.html", TemplateType = InvoiceTemplateType.SaleOrder }
                }
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                .WhereIf(input.TemplateType.HasValue, s => s.TemplateType == input.TemplateType.Value)
                .ToList();

                return new ListResultDto<DefaultTemplateOutput>
                {
                    Items = list
                };
            });
        }

        public async Task<ListResultDto<InvoiceTemplateMapDto>> GetTemplateMapList(TemplateMapListInput input)
        {
            var mapList = await _invoiceTemplateMapRepository.GetAll()
                                 .AsNoTracking()
                                 .Select(s => new InvoiceTemplateMapDto
                                 {
                                     Id = s.Id,
                                     TemplateTypeName = s.TemplateType.ToString(),
                                     TemplateType = s.TemplateType,
                                     SaleTypeId = s.SaleTypeId,
                                     SaleTypeName = s.SaleType == null ? "" : s.SaleType.TransactionTypeName,
                                     TemplateId = s.TemplateId,
                                     TemplateName = s.InvoiceTemplate.Name,
                                 })
                                 .ToListAsync();

            var defaultInoiceTemplate = mapList.Where(s => !s.SaleTypeId.HasValue && s.TemplateType == InvoiceTemplateType.Invoice).FirstOrDefault();
            if (defaultInoiceTemplate == null) mapList.Add(new InvoiceTemplateMapDto { TemplateType = InvoiceTemplateType.Invoice, TemplateTypeName = InvoiceTemplateType.Invoice.ToString() });

            var defaultSaleOrderTemplate = mapList.Where(s => !s.SaleTypeId.HasValue && s.TemplateType == InvoiceTemplateType.SaleOrder).FirstOrDefault();
            if (defaultSaleOrderTemplate == null) mapList.Add(new InvoiceTemplateMapDto { TemplateType = InvoiceTemplateType.SaleOrder, TemplateTypeName = InvoiceTemplateType.SaleOrder.ToString() });

            var transactionTypes = await _transactionTypeRepository.GetAll().Where(s => !s.IsPOS).AsNoTracking().ToListAsync();

            if (transactionTypes.Any())
            {
                foreach (var type in transactionTypes)
                {
                    var inoiceTemplate = mapList.Where(s => s.SaleTypeId == type.Id && s.TemplateType == InvoiceTemplateType.Invoice).FirstOrDefault();
                    if (inoiceTemplate == null) mapList.Add(new InvoiceTemplateMapDto { TemplateType = InvoiceTemplateType.Invoice, TemplateTypeName = InvoiceTemplateType.Invoice.ToString(), SaleTypeId = type.Id, SaleTypeName = type.TransactionTypeName });

                    var saleOrderTemplate = mapList.Where(s => s.SaleTypeId == type.Id && s.TemplateType == InvoiceTemplateType.SaleOrder).FirstOrDefault();
                    if (saleOrderTemplate == null) mapList.Add(new InvoiceTemplateMapDto { TemplateType = InvoiceTemplateType.SaleOrder, TemplateTypeName = InvoiceTemplateType.SaleOrder.ToString(), SaleTypeId = type.Id, SaleTypeName = type.TransactionTypeName });
                }
            }

            var resultList = mapList
                             .WhereIf(input.TemplateType.HasValue, s => s.TemplateType == input.TemplateType.Value)
                             .OrderBy(s => s.TemplateType)
                             .ToList();

            return new ListResultDto<InvoiceTemplateMapDto> { Items = resultList };

        }

        public async Task UnMapTemplate(EntityDto<Guid> input)
        {
            var entity = await _invoiceTemplateMapManager.GetAsync(input.Id, true);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));
            CheckErrors(await _invoiceTemplateMapManager.DeleteAsync(entity));
        }

        public async Task<InvoiceTemplateMapDto> MapTemplate(InvoiceTemplateMapDto input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.Id.HasValue)
            {
                var entity = await _invoiceTemplateMapManager.GetAsync(input.Id.Value, true);
                if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));
                entity.Update(userId, input.TemplateType, input.SaleTypeId, input.TemplateId);
                await _invoiceTemplateMapManager.UpdateAsync(entity);
            }
            else
            {
                var entity = InvoiceTemplateMap.Create(tenantId, userId, input.TemplateType, input.SaleTypeId, input.TemplateId);
                await _invoiceTemplateMapManager.CreateAsync(entity);
                input.Id = entity.Id;
            }

            return input;
        }

        public async Task<ListResultDto<VarriableDto>> GetTemplateVarriables()
        {
            return await Task.Run(() =>
            {
                var list = new List<VarriableDto> {
                    new VarriableDto{
                        Group = "Header & Footer",
                        Items = new List<string> {
                            "@Logo",
                            "@CompanyName",
                            "@CompanyAddress",
                            "@CustomerName",
                            "@CustomerPhone",
                            "@InvoiceDate",
                            "@ETADate",
                            "@DueDate",
                            "@IssueDate",
                            "@InvoiceNo",
                            "@Reference",
                            "@Currency",
                            "@Memo",
                            "@UserName",
                            "@SubTotal",
                            "@PaymentAmount",
                            "@Balance",
                            "@ShippingAddress"
                        }
                    },
                    new VarriableDto {
                        Group = "Detail Repeat",
                        Items = new List<string> {
                            "@No",
                            "@ItemCode",
                            "@ItemName",
                            "@Qty",
                            "@UnitPrice",
                            "@DiscountRate",
                            "@LineTotal",
                            "@Description",
                        }
                    },
                    new VarriableDto {
                        Group = "Summary Item Repeat",
                        Items = new List<string> {
                            "@Date",
                            "@Description",
                            "@SummaryTotal",
                        }
                    },
                    new VarriableDto {
                        Group = "Summary Group Footer",
                        Items = new List<string>{
                            "@GroupTotal",
                            "@GroupCurrency",
                        }
                    }
                };

                return new ListResultDto<VarriableDto> { Items = list };
            });

        }

    }
}
