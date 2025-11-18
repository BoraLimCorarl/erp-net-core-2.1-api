using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.UI;
using Abp.Collections.Extensions;
using System.Linq.Dynamic.Core;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using CorarlERP.PackageEditions.Dot;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.Reports;
using Microsoft.CodeAnalysis.CSharp;
using CorarlERP.FileStorages;
using OfficeOpenXml;
using CorarlERP.Items;

namespace CorarlERP.PackageEditions
{
    [AbpAuthorize]
    public class FeatureAppService : ReportBaseClass, IFeatureAppService
    {
        private readonly ICorarlRepository<Feature, Guid> _featureRepository;//repository
        private readonly IFileStorageManager _fileStorageManager;
        private readonly AppFolders _appFolders;

        public FeatureAppService(
            AppFolders appFolders,
            IFileStorageManager fileStorageManager,
            ICorarlRepository<Feature, Guid> featureRepository
        )
        :base(fileStorageManager)
        {
            _appFolders = appFolders;
            _featureRepository = featureRepository;
            _fileStorageManager = fileStorageManager;
        }

        private async Task CheckDuplicate(FeatureDto input)
        {
            if (input.Name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Name")));

            var duplicate = await _featureRepository.GetAll().AsNoTracking().AnyAsync(s => s.Id != input.Id && s.Name.ToLower() == input.Name.ToLower());
            if (duplicate) throw new UserFriendlyException(L("Duplicated", L("PromotionName")));
        }


        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Create)]
        public async Task Create(FeatureDto input)
        {
            await CheckDuplicate(input);

            var userId = AbpSession.GetUserId();

            var @entity = Feature.Create(userId, input.Name, input.Description, input.SortOrder, input.IsDefault);
            await _featureRepository.BulkInsertAsync(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Find)]
        public async Task<ListResultDto<GetFeatureListDto>> Find(FindFeatureInput input)
        {
            var @query = _featureRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p =>
                    p.IsActive == input.IsActive.Value ||
                    input.SelectedFeatures.IsNullOrEmpty() ||
                    input.SelectedFeatures.Contains(p.Id))
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                .Select(s => new GetFeatureListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    IsDefault = s.IsDefault,
                    IsActive = s.IsActive
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            return new ListResultDto<GetFeatureListDto> { Items = entities };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Find)]
        public async Task<ListResultDto<GetFeatureListDto>> GetDefaultFeatures()
        {
            var @query = _featureRepository
                .GetAll()
                .AsNoTracking()
                .Where(p => p.IsDefault)
                .Select(s => new GetFeatureListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    IsDefault = s.IsDefault,
                    IsActive = s.IsActive
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return new ListResultDto<GetFeatureListDto> { Items = entities };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Update, AppPermissions.Pages_Host_Client_Packages_GetDetail)]
        public async Task<FeatureDto> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _featureRepository.GetAll().AsNoTracking()
                                .Where(s => s.Id == input.Id)
                                .Select(s => new FeatureDto
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                    Description = s.Description,
                                    SortOrder = s.SortOrder,
                                    IsDefault = s.IsDefault,
                                    IsActive = s.IsActive
                                })
                                .FirstOrDefaultAsync();

            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));


            return entity;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Update)]
        public async Task Update(FeatureDto input)
        {
            await CheckDuplicate(input);

            var userId = AbpSession.GetUserId();

            var @entity = await _featureRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id.Value);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            @entity.Update(userId, input.Name, input.Description, input.SortOrder, input.IsDefault);

            await _featureRepository.BulkUpdateAsync(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _featureRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            await _featureRepository.BulkDeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _featureRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, true);

            await _featureRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _featureRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, false);

            await _featureRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_GetList)]
        public async Task<PagedResultDto<GetFeatureListDto>> GetList(GetFeatureListInput input)
        {
            var @query = _featureRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                .Select(s => new GetFeatureListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    IsDefault = s.IsDefault,
                    IsActive = s.IsActive
                });

            var resultCount = await query.CountAsync();
            var @entities = new List<GetFeatureListDto>();

            if (input.UsePagination)
            {
                entities =  await query
                           .OrderBy(input.Sorting)
                           .PageBy(input)
                           .ToListAsync();
            }
            else
            {
                entities = await query
                          .OrderBy(input.Sorting)
                          .ToListAsync();
            }           

            return new PagedResultDto<GetFeatureListDto>(resultCount, @entities);
        }


        private ReportOutput GetTemplateColumns()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                 
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Name",
                        ColumnLength = 250,
                        ColumnTitle = "Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 230,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SortOrder",
                        ColumnLength = 250,
                        ColumnTitle = "Sort Order",
                        ColumnType = ColumnType.Number,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsDefault",
                        ColumnLength = 250,
                        ColumnTitle = "Is Default",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Features",
                Sortby = "",
            };
            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Create)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var featuers = await _featureRepository.GetAll().AsNoTracking().ToListAsync();
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    var addFeatures = new List<Feature>();

                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var name = worksheet.Cells[i, 1].Value.ToString();
                        if (name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Name") + $", Row = {i}"));

                        var duplicate = featuers.Any(s =>  s.Name == name);
                        if (duplicate) continue;
                        
                        var description = worksheet.Cells[i, 2].Value?.ToString();
                        var sortOrder = Convert.ToInt32(worksheet.Cells[i, 3].Value?.ToString());
                        var isDefault = Convert.ToBoolean(worksheet.Cells[i, 4].Value?.ToString());

                        var feature = Feature.Create(AbpSession.UserId.Value, name, description, sortOrder, isDefault);
                        addFeatures.Add(feature);
                    }

                    if (addFeatures.Any()) await _featureRepository.BulkInsertAsync(addFeatures);
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_GetList, AppPermissions.Pages_Host_Client_Packages_Create)]
        public async Task<FileDto> ExportExcel()
        {
            var input = new GetFeatureListInput()
            {
                Sorting = "SortOrder",
                UsePagination = false
            };

            var resultData = (await GetList(input)).Items;
            //var accountype = await (from a in  TypeOfAccount select a);
            var result = new FileDto();
            var sheetName = "Features";
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
                var headerList = GetTemplateColumns().ColumnInfo.OrderBy(s => s.SortOrder);

                foreach (var i in headerList)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var trueFalseList = new List<string> { "True", "False" };

                // write body
                foreach (var i in resultData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList)
                    {
                        if(h.ColumnName == "IsDefault")
                        {
                            AddDropdownList(ws, rowBody, collumnCellBody, trueFalseList, i.IsDefault.ToString());
                        }
                        else 
                        {
                            var value = i.GetType().GetProperty(h.ColumnName).GetValue(i, null);

                            AddTextToCell(ws, rowBody, collumnCellBody, value?.ToString(), true);
                        }                           
                     
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion Row Body
                result.FileName = $"Features.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }
    }
}
