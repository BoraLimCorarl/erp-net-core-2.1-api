using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.ItemPriceAppServices.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Authorization;
using Abp.Authorization;
using CorarlERP.ItemPrices;
using System;
using CorarlERP.Currencies.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.TransactionTypes.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.CustomerTypes.Dto;
using Abp.Dependency;
using CorarlERP.UserGroups;
using CorarlERP.Dto;
using OfficeOpenXml;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.FileStorages;
using CorarlERP.ReportTemplates;
using Abp.Domain.Uow;

namespace CorarlERP.ItemPriceAppServices
{
    [AbpAuthorize]
    public class ItemPriceAppService : ReportBaseClass, IItemPriceService
    {

        private readonly IItemPriceManager _itemPriceManager;
        private readonly IRepository<ItemPrice, Guid> _itemPriceRepository;

        private readonly IItemPriceItemManager _itemPriceItemManager;
        private readonly IRepository<ItemPriceItem, Guid> _itemPriceItemRepository;

        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;

        private readonly IFileStorageManager _fileStorageManager;

        public ItemPriceAppService(IItemPriceManager itemPriceManager,
                                   IRepository<ItemPrice, Guid> itemPriceRepository,
                                   IItemPriceItemManager itemPriceItemManager,
                                   IRepository<ItemPriceItem, Guid> itemPriceItemRepository,
                                   IRepository<Currency, long> currencyRepository,
                                   IRepository<Item, Guid> itemRepository,
                                   AppFolders appFolders,
                                   IUnitOfWorkManager unitOfWorkManager,
                                   IFileStorageManager fileStorageManager) : base(null, appFolders, null, null)
        {
            _itemPriceManager = itemPriceManager;
            _itemPriceRepository = itemPriceRepository;
            _itemPriceItemManager = itemPriceItemManager;
            _itemPriceItemRepository = itemPriceItemRepository;
            _itemRepository = itemRepository;
            _currencyRepository = currencyRepository;
            _fileStorageManager = fileStorageManager;
            _userGroupMemberRepository = IocManager.Instance.Resolve<IRepository<UserGroupMember, Guid>>();
            _unitOfWorkManager = unitOfWorkManager;
            _appFolders = appFolders;
        }
    

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemPriceInput input)
        {
            // validate sale type and location
            var validate = await _itemPriceRepository.GetAll().Where(t => t.TransactionTypeSaleId == input.SaleTypeId && t.LocationId == input.LocationId && t.CustomerTypeId == input.CustomerTypeId).CountAsync();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("CanNotCreateTheSameSaleTypeAndLocation"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = ItemPrice.Create(tenantId, userId,input.LocationId, input.SaleTypeId, input.CustomerTypeId);          


            #region Items

            if(input.ItemPriceItems.Count() == 0)
            {
                throw new UserFriendlyException(L("ListItemIsRequired"));
            }

            foreach (var ip in input.ItemPriceItems)
            {
                if(ip.Prices.Count() == 0) {

                    throw new UserFriendlyException(L("CurrencyIsRequired"));
                }

                foreach (var p in ip.Prices)
                {
                    if (p.CurrencyId == 0)
                    {
                        throw new UserFriendlyException(L("CurrencyIsRequired"));
                    }
                    var @itemPrice = ItemPriceItem.Create(tenantId, userId, p.CurrencyId, ip.ItemId, p.Price, entity);
                    CheckErrors(await _itemPriceItemManager.CreateAsync(@itemPrice));
                }
            }
            #endregion

            CheckErrors(await _itemPriceManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_Delete)]
        public  async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _itemPriceManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var ItemPriceItems = await _itemPriceItemRepository.GetAll().Where(u => u.ItemPriceId == entity.Id).ToListAsync();

            foreach (var s in ItemPriceItems)
            {
                CheckErrors(await _itemPriceItemManager.RemoveAsync(s));
            }

            CheckErrors(await _itemPriceManager.RemoveAsync(@entity));

        }

       

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_GetList)]
        public async Task<PagedResultDto<GetSummanryItemPriceOutput>> GetList(GetItemPriceInput input)
        {
            var userId = AbpSession.GetUserId();
            var groupLocations = await _userGroupMemberRepository.GetAll()
                          .Where(x => x.MemberId == userId)
                          .Where(x => x.UserGroup.LocationId != null)
                          .AsNoTracking()
                          .Select(x => x.UserGroup.LocationId)
                          .ToListAsync();

            var @query = from
                pi in _itemPriceRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.TransactionTypeSale)
                .Include(u => u.CustomerType)
                .WhereIf(groupLocations.Any(), u => u.LocationId == null || groupLocations.Contains(u.LocationId))
                .WhereIf(input.Locations != null, p => input.Locations.Contains(p.LocationId.Value))
                .WhereIf(input.SaleTypes != null, p => input.SaleTypes.Contains(p.TransactionTypeSaleId.Value))
                .WhereIf(input.CustomerTypes != null, p => input.CustomerTypes.Contains(p.CustomerTypeId.Value))
                .AsNoTracking()

                join ipi in  _itemPriceItemRepository.GetAll()
                .Include(u => u.Currency)
                .Include(u => u.Item)
                .AsNoTracking()
                .WhereIf(input.Items != null, p => input.Items.Contains(p.ItemId))
                .WhereIf(input.Curencys != null, p => input.Curencys.Contains(p.CurrencyId))
              
                on pi.Id equals ipi.ItemPriceId into u

                where u != null && u.Count() > 0 

                select (new GetSummanryItemPriceOutput
                {
                    Id = pi.Id,
                    Location = ObjectMapper.Map<LocationSummaryOutput>(pi.Location),
                    LocationId = pi.LocationId,
                    SaleType = ObjectMapper.Map<TransactionTypeSummaryOutput>(pi.TransactionTypeSale),
                    SaleTypeId = pi.TransactionTypeSaleId,
                    CustomerTypeId = pi.CustomerTypeId,
                    CustomerType = ObjectMapper.Map<CustomerTypeSummaryOutput>(pi.CustomerType),
                });


            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetSummanryItemPriceOutput>(resultCount, ObjectMapper.Map<List<GetSummanryItemPriceOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemPirceInput input)
        {
            // validate sale type and location
            var validate = await _itemPriceRepository.GetAll().Where(t => t.TransactionTypeSaleId == input.SaleTypeId && t.LocationId == input.LocationId && t.CustomerTypeId == input.CustomerTypeId && t.Id != input.Id).CountAsync();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("CanNotCreateTheSameSaleTypeAndLocation"));
            }


            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _itemPriceManager.GetAsync(input.Id, true); //this is journal

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.ItemPriceItems.Count() == 0)
            {
                throw new UserFriendlyException(L("ListItemIsRequired"));
            }

            entity.Update(userId,input.LocationId,input.SaleTypeId, input.CustomerTypeId);


            #region update itemPriceItem
            var ItemPriceItems = await _itemPriceItemRepository.GetAll().Where(u => u.ItemPriceId == entity.Id).ToListAsync();

            var inputItems = input.ItemPriceItems.SelectMany(s => s.Prices.Select(r => new { s.ItemId, r.Id, r.CurrencyId, r.Price})).ToList();

            var addItems = inputItems.Where(s => s.Id == null).ToList();

            var updateItems = inputItems.Where(s => s.Id != null && s.Id != Guid.Empty).ToList();

            var deleteItems = ItemPriceItems.Where(s => !updateItems.Select(r => r.Id).Contains(s.Id)).ToList();

            if(addItems.Count() == 0 && updateItems.Count() == 0) {

                throw new UserFriendlyException(L("ListItemIsRequired"));
            }


            //add item prices
            foreach(var addItem in addItems)
            {
                var priceItem = ItemPriceItem.Create(tenantId, userId, addItem.CurrencyId, addItem.ItemId, addItem.Price, entity);
                            CheckErrors(await _itemPriceItemManager.CreateAsync(priceItem));
            }

            //update item prices
            foreach (var updateItem in updateItems)
            {
                var itemPriceItem = ItemPriceItems.FirstOrDefault(s => s.Id == updateItem.Id);
                itemPriceItem.Update(userId, updateItem.CurrencyId, updateItem.ItemId, updateItem.Price, entity.Id);
                                CheckErrors(await _itemPriceItemManager.UpdateAsync(itemPriceItem));
            }

            //remove item prices
            foreach (var removeItem in deleteItems)
            {
                CheckErrors(await _itemPriceItemManager.RemoveAsync(removeItem));
            }

            #endregion

            CheckErrors(await _itemPriceManager.UpdateAsync(entity));
           
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_Find)]
        public async Task<PagedResultDto<GetSummanryItemPriceOutput>> Find(GetItemPriceInput input)
        {
            var @query = from
                pi in _itemPriceRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.TransactionTypeSale)
                .Include(u => u.CustomerType)
                .WhereIf(input.Locations != null, p => input.Locations.Contains(p.LocationId.Value))
                .WhereIf(input.SaleTypes != null, p => input.SaleTypes.Contains(p.TransactionTypeSaleId.Value))
                .WhereIf(input.CustomerTypes != null, p => input.CustomerTypes.Contains(p.CustomerTypeId.Value))
                .AsNoTracking()

                         join ipi in _itemPriceItemRepository.GetAll()
                         .Include(u => u.Currency)
                         .Include(u => u.Item)
                         .AsNoTracking()
                         .WhereIf(input.Items != null, p => input.Items.Contains(p.ItemId))
                         .WhereIf(input.Curencys != null, p => input.Curencys.Contains(p.CurrencyId))

                         on pi.Id equals ipi.ItemPriceId into u

                         where u != null && u.Count() > 0

                         select (new GetSummanryItemPriceOutput
                         {
                             Id = pi.Id,
                             Location = ObjectMapper.Map<LocationSummaryOutput>(pi.Location),
                             LocationId = pi.LocationId,
                             SaleType = ObjectMapper.Map<TransactionTypeSummaryOutput>(pi.TransactionTypeSale),
                             SaleTypeId = pi.TransactionTypeSaleId,
                             CustomerTypeId = pi.CustomerTypeId,
                             CustomerType = ObjectMapper.Map<CustomerTypeSummaryOutput>(pi.CustomerType),
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetSummanryItemPriceOutput>(resultCount, ObjectMapper.Map<List<GetSummanryItemPriceOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_GetDetail)]
        public async Task<GetDetailItemPriceOutput> GetDetail(EntityDto<Guid> input)
        {

            var result = await HelperGetDetail(input.Id);
            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_ItemPrices_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcel(Guid Id)
        {
            var query = new GetDetailItemPriceOutput();
            var tenantId = AbpSession.TenantId;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    query = await HelperGetDetail(Id);

                }
            }
            var currencyHeaders = query.ItemPriceItems[0].GetPriceDetail.Select(t => t.CurrencyCode);
            var result = new FileDto();
            var sheetName = "ItemPrice";
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
                foreach (var i in query.ItemPriceItems)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        WriteBodyItemPrices(ws, rowBody, collumnCellBody, h, i, count);
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion
                var fileName = query.Location != null ? $"ItemPrice.{query.Location?.LocationName}.xlsx" : $"ItemPrice_Report.xlsx";
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
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 250,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    }                  
                },
                Groupby = "",
                HeaderTitle = "ItemPrice",
                Sortby = "",
            };
            return result;
        }


        private async Task<GetDetailItemPriceOutput> HelperGetDetail(Guid Id)
        {
            var @entity = await _itemPriceRepository.GetAll()
                        .Where(t => t.Id == Id)
                        .Include(u => u.Location)
                        .Include(u => u.TransactionTypeSale)
                        .Include(u => u.CustomerType)
                        .AsNoTracking()
                        .Select(g => new GetDetailItemPriceOutput
                        {
                            Id = g.Id,
                            Location = ObjectMapper.Map<LocationSummaryOutput>(g.Location),
                            LocationId = g.LocationId,
                            SaleTypeId = g.TransactionTypeSaleId,
                            SaleType = new TransactionTypeSummaryOutput
                            {
                                Id = g.TransactionTypeSaleId != null ? g.TransactionTypeSaleId.Value : 0,
                                TransactionTypeName = g.TransactionTypeSale != null ? g.TransactionTypeSale.TransactionTypeName : null
                            },
                            CustomerTypeId = g.CustomerTypeId,
                            CustomerType = ObjectMapper.Map<CustomerTypeSummaryOutput>(g.CustomerType),

                        }).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var itemPriceItems = await(from ipi in _itemPriceItemRepository.GetAll()
                                       .Include(u => u.Currency)
                                       .AsNoTracking()

                                       join i in _itemRepository
                                        .GetAll()
                                        .AsNoTracking()
                                        on ipi.ItemId equals i.Id
                                       // orderby ipi.CreationTime descending
                                       where (ipi.ItemPriceId == entity.Id)
                                       group ipi by new { item = i } into u

                                       select new GetDetailItemPriceItemOutput
                                       {

                                           ItemName = u.Key.item.ItemName,
                                           ItemCode = u.Key.item.ItemCode,
                                           ItemId = u.Key.item.Id,
                                           GetPriceDetail = u.OrderBy(s => s.Currency.Code).Select(t => new GetItemPriceDetail
                                           {

                                               Id = t.Id,
                                               CurrencyCode = t.Currency.Code,
                                               CurrencyId = t.CurrencyId,
                                               Price = t.Price,
                                           }).ToList(),

                                       }).ToListAsync();

            var result = ObjectMapper.Map<GetDetailItemPriceOutput>(@entity);
            result.ItemPriceItems = ObjectMapper.Map<List<GetDetailItemPriceItemOutput>>(itemPriceItems);
            return result;

        }

        #endregion


    }
}
