using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Currencies;
using CorarlERP.Dto;
using CorarlERP.FileStorages;
using CorarlERP.Items;
using CorarlERP.Locations.Dto;
using CorarlERP.PurchasePrices.Dto;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.UserGroups;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PurchasePrices
{
    [AbpAuthorize]
    public class PurchasePriceAppService : ReportBaseClass, IPurchasePriceAppService
    {

        private readonly IPurchasePriceManager _purchasePriceManager;
        private readonly IRepository<PurchasePrice, Guid> _purchasePriceRepository;

        private readonly IPurchasePriceItemManager _purchasePriceItemManager;
        private readonly IRepository<PurchasePriceItem, Guid> _purchasePriceItemRepository;

        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;

        private readonly IFileStorageManager _fileStorageManager;

        public PurchasePriceAppService(IPurchasePriceManager purchasePriceManager,
                                   IRepository<PurchasePrice, Guid> purchasePriceRepository,
                                   IPurchasePriceItemManager purchasePriceItemManager,
                                   IRepository<PurchasePriceItem, Guid> purchasePriceItemRepository,
                                   IRepository<Currency, long> currencyRepository,
                                   IRepository<Item, Guid> itemRepository,
                                   AppFolders appFolders,
                                   IUnitOfWorkManager unitOfWorkManager,
                                   IFileStorageManager fileStorageManager) : base(null, appFolders, null, null)
        {
            _purchasePriceManager = purchasePriceManager;
            _purchasePriceRepository = purchasePriceRepository;
            _purchasePriceItemManager = purchasePriceItemManager;
            _purchasePriceItemRepository = purchasePriceItemRepository;
            _itemRepository = itemRepository;
            _currencyRepository = currencyRepository;
            _fileStorageManager = fileStorageManager;
            _userGroupMemberRepository = IocManager.Instance.Resolve<IRepository<UserGroupMember, Guid>>();
            _unitOfWorkManager = unitOfWorkManager;
            _appFolders = appFolders;
        }

        
        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreatePurchasePriceInput input)
        {
            ValidateInput(input);

            var vendorIds = input.PurchasePriceItems.Where(s => s.VendorId.HasValue).Select(s => s.VendorId).Distinct().ToList();

            // validate sale type and location
            var validate = await _purchasePriceRepository.GetAll()
                                .AsNoTracking()
                                .Where(t => 
                                    !t.SpecificVendor || !input.SpecificVendor ||
                                    (t.LocationId == input.LocationId && t.PurchasePriceItems.Any(r => vendorIds.Contains(r.VendorId))))
                                .AnyAsync();
            if (validate)
            {
                throw new UserFriendlyException(L("CanNotCreateTheSameVendorInLocation"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = PurchasePrice.Create(tenantId, userId, input.LocationId, input.SpecificVendor);

            #region Items


            foreach (var ip in input.PurchasePriceItems)
            {
                if (ip.Prices.Count() == 0)
                {

                    throw new UserFriendlyException(L("CurrencyIsRequired"));
                }

                foreach (var p in ip.Prices)
                {
                    if (p.CurrencyId == 0)
                    {
                        throw new UserFriendlyException(L("CurrencyIsRequired"));
                    }
                    var @purchasePrice = PurchasePriceItem.Create(tenantId, userId, p.CurrencyId, ip.ItemId, ip.VendorId, p.Price, entity);
                    CheckErrors(await _purchasePriceItemManager.CreateAsync(@purchasePrice));
                }
            }
            #endregion

            CheckErrors(await _purchasePriceManager.CreateAsync(@entity));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _purchasePriceManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var PurchasePriceItems = await _purchasePriceItemRepository.GetAll().Where(u => u.PurchasePriceId == entity.Id).ToListAsync();

            foreach (var s in PurchasePriceItems)
            {
                CheckErrors(await _purchasePriceItemManager.RemoveAsync(s));
            }

            CheckErrors(await _purchasePriceManager.RemoveAsync(@entity));

        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_GetList)]
        public async Task<PagedResultDto<GetSummanryPurchasePriceOutput>> GetList(GetPurchasePriceInput input)
        {
            var userId = AbpSession.GetUserId();
            var groupLocations = await _userGroupMemberRepository.GetAll()
                                      .Where(x => x.UserGroup.LocationId.HasValue && (x.UserGroup.Location.Member == Member.All || x.MemberId == userId))
                                      .AsNoTracking()
                                      .Select(x => x.UserGroup.LocationId)
                                      .ToListAsync();

            var @query = _purchasePriceRepository.GetAll().AsNoTracking()
                        .WhereIf(groupLocations.Any(), u => u.LocationId == null || groupLocations.Contains(u.LocationId))
                        .WhereIf(!input.Locations.IsNullOrEmpty(), p => input.Locations.Contains(p.LocationId.Value))
                        .WhereIf(!input.Vendors.IsNullOrEmpty(), u => u.PurchasePriceItems.Any(t => input.Vendors.Contains(t.VendorId)))
                        .WhereIf(!input.Items.IsNullOrEmpty(), u => u.PurchasePriceItems.Any(t => input.Items.Contains(t.ItemId))) 
                        .WhereIf(!input.Curencys.IsNullOrEmpty(), u => u.PurchasePriceItems.Any(t => input.Curencys.Contains(t.CurrencyId)))
                        .Select (s => new GetSummanryPurchasePriceOutput
                         {
                             Id = s.Id,
                             Location = new LocationSummaryOutput
                             {
                                 Id = s.LocationId.Value,
                                 LocationName = s.Location.LocationName,
                             },
                             LocationId = s.LocationId,
                             SpecificVendor = s.SpecificVendor,
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetSummanryPurchasePriceOutput>(resultCount, ObjectMapper.Map<List<GetSummanryPurchasePriceOutput>>(@entities));
        }

        private void ValidateInput(CreatePurchasePriceInput input)
        {
            if (input.PurchasePriceItems.IsNullOrEmpty()) throw new UserFriendlyException(L("ListItemIsRequired"));
            
            var itemHas = new List<string>();

            foreach (var item in input.PurchasePriceItems)
            {
                if (input.SpecificVendor) 
                {
                    if(item.VendorId == null || item.VendorId == Guid.Empty) throw new UserFriendlyException(L("VendorIsRequired"));
                }
                else
                {
                    item.VendorId = null;
                }

                var key = input.SpecificVendor ? $"{item.ItemId}-{item.VendorId}" : item.ItemId.ToString();

                if(itemHas.Contains(key)) throw new UserFriendlyException((input.SpecificVendor ? L("DuplicateVendorItem") : L("DuplicateItem")) + $" {item.ItemName} - {item.VendorName}");
                    
                itemHas.Add(key);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdatePurchasePriceInput input)
        {
            ValidateInput(input);

            var vendorIds = input.PurchasePriceItems.Where(s => s.VendorId.HasValue).Select(s => s.VendorId).Distinct().ToList();

            // validate sale type and location
            var validate = await _purchasePriceRepository.GetAll()
                                .AsNoTracking()
                                .Where(s => s.Id != input.Id)
                                .Where(t =>
                                    !t.SpecificVendor || !input.SpecificVendor ||
                                    (t.LocationId == input.LocationId && t.PurchasePriceItems.Any(r => vendorIds.Contains(r.VendorId))))
                                .AnyAsync();

            if (validate)
            {
                throw new UserFriendlyException(L("CanNotCreateTheSameVendorInLocation"));
            }


            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _purchasePriceManager.GetAsync(input.Id, true); //this is journal

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }


            entity.Update(userId, input.LocationId, input.SpecificVendor);


            #region update purchasePriceItem
            var PurchasePriceItems = await _purchasePriceItemRepository.GetAll().Where(u => u.PurchasePriceId == entity.Id).ToListAsync();

            var inputItems = input.PurchasePriceItems.SelectMany(s => s.Prices.Select(r => new { s.ItemId, s.VendorId, r.Id, r.CurrencyId, r.Price })).ToList();

            var addItems = inputItems.Where(s => s.Id == null).ToList();

            var updateItems = inputItems.Where(s => s.Id != null && s.Id != Guid.Empty).ToList();

            var deleteItems = PurchasePriceItems.Where(s => !updateItems.Select(r => r.Id).Contains(s.Id)).ToList();

            if (addItems.Count() == 0 && updateItems.Count() == 0)
            {

                throw new UserFriendlyException(L("ListItemIsRequired"));
            }


            //add item prices
            foreach (var addItem in addItems)
            {
                var priceItem = PurchasePriceItem.Create(tenantId, userId, addItem.CurrencyId, addItem.ItemId, addItem.VendorId, addItem.Price, entity);
                CheckErrors(await _purchasePriceItemManager.CreateAsync(priceItem));
            }

            //update item prices
            foreach (var updateItem in updateItems)
            {
                var purchasePriceItem = PurchasePriceItems.FirstOrDefault(s => s.Id == updateItem.Id);
                purchasePriceItem.Update(userId, updateItem.CurrencyId, updateItem.ItemId, updateItem.VendorId, updateItem.Price, entity.Id);
                CheckErrors(await _purchasePriceItemManager.UpdateAsync(purchasePriceItem));
            }

            //remove item prices
            foreach (var removeItem in deleteItems)
            {
                CheckErrors(await _purchasePriceItemManager.RemoveAsync(removeItem));
            }

            #endregion

            CheckErrors(await _purchasePriceManager.UpdateAsync(entity));

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_Find)]
        public async Task<PagedResultDto<GetSummanryPurchasePriceOutput>> Find(GetPurchasePriceInput input)
        {
            var userId = AbpSession.GetUserId();
            var groupLocations = await _userGroupMemberRepository.GetAll()
                          .Where(x => x.MemberId == userId)
                          .Where(x => x.UserGroup.LocationId != null)
                          .AsNoTracking()
                          .Select(x => x.UserGroup.LocationId)
                          .ToListAsync();

            var @query = _purchasePriceRepository.GetAll().AsNoTracking()
                        .WhereIf(groupLocations.Any(), u => u.LocationId == null || groupLocations.Contains(u.LocationId))
                        .WhereIf(!input.Locations.IsNullOrEmpty(), p => input.Locations.Contains(p.LocationId.Value))
                        .WhereIf(!input.Vendors.IsNullOrEmpty(), u => u.PurchasePriceItems.Any(t => input.Vendors.Contains(t.VendorId)))
                        .WhereIf(!input.Items.IsNullOrEmpty(), u => u.PurchasePriceItems.Any(t => input.Items.Contains(t.ItemId)))
                        .WhereIf(!input.Curencys.IsNullOrEmpty(), u => u.PurchasePriceItems.Any(t => input.Curencys.Contains(t.CurrencyId)))
                        .Select(s => new GetSummanryPurchasePriceOutput
                        {
                            Id = s.Id,
                            Location = new LocationSummaryOutput
                            {
                                Id = s.LocationId.Value,
                                LocationName = s.Location.LocationName,
                            },
                            LocationId = s.LocationId,
                            SpecificVendor = s.SpecificVendor,
                        });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetSummanryPurchasePriceOutput>(resultCount, ObjectMapper.Map<List<GetSummanryPurchasePriceOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_GetDetail)]
        public async Task<GetDetailPurchasePriceOutput> GetDetail(EntityDto<Guid> input)
        {

            var result = await HelperGetDetail(input.Id);
            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchasePrices_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcel(Guid Id)
        {
            var query = new GetDetailPurchasePriceOutput();
            var tenantId = AbpSession.TenantId;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    query = await HelperGetDetail(Id);

                }
            }
            var currencyHeaders = query.PurchasePriceItems[0].GetPriceDetail.Select(t => t.CurrencyCode);
            var result = new FileDto();
            var sheetName = "PurchasePrice";
            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.

            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateItemPirces();

                foreach(var i in currencyHeaders)
                {
                    headerList.ColumnInfo.Add(new CollumnOutput
                    {
                        ColumnName = i,
                        ColumnLength = 100,
                        ColumnType = ColumnType.Money,
                        ColumnTitle = i,
                    });
                }
               
                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                // write body
                foreach (var i in query.PurchasePriceItems)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        WriteBodyPurchasePrices(ws, rowBody, collumnCellBody, h, i, count);
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion
                var fileName = query.Location != null ? $"PurchasePrice.{query.Location?.LocationName}.xlsx" : $"PurchasePrice_Report.xlsx";
                result.FileName = fileName;
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }
        #region Export and Import Excel
        private ReportOutput GetReportTemplateItemPirces()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                  
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 150,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 300,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "VendorName",
                        ColumnLength = 200,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    }
                },
                Groupby = "",
                HeaderTitle = "PurchasePrice",
                Sortby = "",
            };
            return result;
        }


        private async Task<GetDetailPurchasePriceOutput> HelperGetDetail(Guid Id)
        {
            var result = await _purchasePriceRepository.GetAll()
                        .Where(t => t.Id == Id)
                        .Include(u => u.Location)
                        .AsNoTracking()
                        .Select(g => new GetDetailPurchasePriceOutput
                        {
                            Id = g.Id,
                            Location = ObjectMapper.Map<LocationSummaryOutput>(g.Location),
                            LocationId = g.LocationId,
                            SpecificVendor = g.SpecificVendor
                        })
                        .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var priceItems = await _purchasePriceItemRepository.GetAll().AsNoTracking()
                                .Where(t => t.PurchasePriceId == Id)
                                .GroupBy(s => new { s.ItemId, s.Item.ItemCode, s.Item.ItemName, s.VendorId, VendorName = s.VendorId.HasValue ? s.Vendor.VendorName : "" })
                                .Select(g => new GetDetailPurchasePriceItemOutput
                                {
                                    ItemId = g.Key.ItemId,
                                    ItemName = g.Key.ItemName,
                                    ItemCode = g.Key.ItemCode,
                                    VendorId = g.Key.VendorId,
                                    VendorName = g.Key.VendorName,
                                    GetPriceDetail = g.Select(t => 
                                    new GetPurchasePriceDetail
                                    {
                                        Id = t.Id,
                                        CurrencyId = t.CurrencyId,
                                        CurrencyCode = t.Currency.Code,
                                        Price = t.Price
                                    })
                                    .OrderBy(t => t.CurrencyCode)
                                    .ToList()
                                })
                                .OrderBy(s => s.ItemCode)
                                .ThenBy(s => s.VendorName)
                                .ToListAsync();

            result.PurchasePriceItems = priceItems;

            return result;
        }

        #endregion


    }
}
