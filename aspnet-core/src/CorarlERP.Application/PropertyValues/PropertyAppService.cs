using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using CorarlERP.PropertyValues.Dto;
using Abp.Runtime.Session;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Collections.Extensions;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.Authorization;
using Abp.UI;
using System.Linq.Dynamic.Core;
using CorarlERP.Authorization;
using System.Xml.Linq;
using IdentityModel;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Dto;
using OfficeOpenXml;
using System;
using System.IO;
using CorarlERP.MultiTenancy;
using Abp.Dependency;
using CorarlERP.FileStorages;
using Abp.Domain.Uow;
using System.Transactions;
using IdentityServer4.Test;
using Org.BouncyCastle.Crypto.Engines;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static CorarlERP.Authorization.Roles.StaticRoleNames;
using Telegram.Bot.Types;

namespace CorarlERP.PropertyValues
{
    [AbpAuthorize]
    public class PropertyAppService : ReportBaseClass, IPropertyAppService
    {
        private readonly IPropertyManager _propertyManager;
        private readonly IPropertyValueManager _propertyValueManager;
        private readonly ICorarlRepository<Property, long> _propertyRepository;
        private readonly ICorarlRepository<PropertyValue, long> _propertyValueRepository;
        private readonly AppFolders _appFolders;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public PropertyAppService(IPropertyManager propertyManager,
                                  IPropertyValueManager propertyValueManager,
                                  AppFolders appFolders,
                                  IFileStorageManager fileStorageManager,
                                  ICorarlRepository<Property, long> propertyRepository,
                                  ICorarlRepository<PropertyValue, long> propertyValueRepository,
                                  IUnitOfWorkManager unitOfWorkManager)
            : base(null, appFolders, null, null)
        {
            _propertyManager = propertyManager;
            _propertyRepository = propertyRepository;
            _propertyValueManager = propertyValueManager;
            _propertyValueRepository = propertyValueRepository;
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = IocManager.Instance.Resolve<IRepository<Tenant, int>>();
        }

        #region Property Methods
        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Create)]
        public async Task Create(CreatePropertyInput input)
        {
            if ((input.IsItemGroup && input.IsUnit) ||
                (input.IsItemGroup && input.IsStandardCostGroup) ||
                (input.IsStandardCostGroup && input.IsUnit) ||
                (input.IsItemGroup && input.IsUnit && input.IsStandardCostGroup))
            {
                throw new UserFriendlyException(L("DuplicateIsUnitAndItemGroup"));
            }

            if (input.IsUnit)
            {
                var @isUnit = await _propertyRepository.GetAll().Where(t => t.IsUnit).AnyAsync();

                if (@isUnit) throw new UserFriendlyException(L("DuplicateIsUnit"));

            }

            if (input.IsItemGroup)
            {
                var @isItemGroup = await _propertyRepository.GetAll().Where(t => t.IsItemGroup).AnyAsync();

                if (isItemGroup) throw new UserFriendlyException(L("DuplicateIsItemGroup"));
            }

            if (input.IsStandardCostGroup)
            {
                var @isStandardCostGroup = await _propertyRepository.GetAll().Where(t => t.IsStandardCostGroup).AnyAsync();

                if (isStandardCostGroup) throw new UserFriendlyException(L("DuplicateIsStandardCostGroup"));
            }

            var dublicateDefault = input.Values.Where(t => t.IsDefault).Count();
            if (dublicateDefault > 1) throw new UserFriendlyException(L("CannotCheckMultiDefault"));
            //validate isUnit have only one 

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var pentity = Property.Create(tenantId, userId, input.Name, input.IsUnit, input.IsRequired, input.IsStatic, input.IsItemGroup, input.IsStandardCostGroup);

            CheckErrors(await _propertyManager.CreateAsync(pentity));
#if checkinputvalue

#endif
            if (input.Values.Count() == 0)
            {
                throw new UserFriendlyException(L("PropertyValueIsRequired") + $": {input.Name}");
            }
            foreach (var v in input.Values)
            {
                var @vEntity = PropertyValue.Create(tenantId, userId, pentity, v.Value, v.NetWeight, v.IsDefault, v.Code, v.IsBaseUnit, v.BaseUnitId, v.Factor);
                CheckErrors(await _propertyValueManager.CreateAsync(vEntity));
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _propertyManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.IsStatic) throw new UserFriendlyException(L("CanNotDeletePropertyFromStaticValue"));
            var @query = _propertyValueRepository
                 .GetAll()
                 .Include(u => u.Property)
                 .AsNoTracking()
                 .Where(u => u.PropertyId == input.Id);

            var resultCount = await query.CountAsync();
            var @entities = await query
                .ToListAsync();

            for (var propertyValue = 0; propertyValue < entities.Count; propertyValue++)
            {

                await _propertyValueRepository.DeleteAsync(entities[propertyValue].Id);
            }

            CheckErrors(await _propertyManager.RemoveAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _propertyManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _propertyManager.DisableAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _propertyManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _propertyManager.EnableAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Find)]
        public async Task<ListResultDto<PropertyOutput>> Find(GetPropertyListInput input)
        {

            var result = new List<PropertyOutput>();
            result = await _propertyRepository.GetAll()
                                            .Select(s => new PropertyOutput
                                            {
                                                Id = s.Id,
                                                IsActive = s.IsActive,
                                                Name = s.Name,
                                                IsRequired = s.IsRequired
                                            }).ToListAsync();
            foreach (var item in result)
            {
                var valueList = await _propertyValueRepository.GetAll().Select(
                                           x => new FindPropertyValueDetailOutput
                                           {
                                               Id = x.Id,
                                               IsActive = x.IsActive,
                                               Value = string.IsNullOrWhiteSpace(x.Code) ? x.Value : $"{x.Code + " - " + x.Value}",
                                               PropertyId = x.PropertyId,
                                               PropertyValueId = x.Id,
                                               IsDefault = x.IsDefault,
                                               Code = x.Code,

                                           }).Where(w => item.Id == w.PropertyId).ToListAsync();
                item.Value = valueList;
            }
            //var @entities = await _propertyRepository
            //    .GetAll()
            //    .AsNoTracking()
            //    .WhereIf(
            //        !input.Filter.IsNullOrEmpty(),
            //        p => p.Name.ToLower().Contains(input.Filter.ToLower())
            //    )
            //    .OrderBy(p => p.Name)
            //    .ToListAsync();

            return new ListResultDto<PropertyOutput>(ObjectMapper.Map<List<PropertyOutput>>(result));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_GetList, AppPermissions.Pages_Tenant_Properties_Find)]
        public async Task<ListResultDto<PropertyDetailOutput>> GetList(GetPropertyListInput input)
        {
            var @entities = await _propertyRepository
                 .GetAll()
                 .AsNoTracking()
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Name.ToLower().Contains(input.Filter.ToLower())
                 )
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return new ListResultDto<PropertyDetailOutput>(ObjectMapper.Map<List<PropertyDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Update)]
        public async Task<PropertyDetailOutput> Update(UpdatePropertyInput input)
        {
            if ((input.IsItemGroup && input.IsUnit) ||
                (input.IsItemGroup && input.IsStandardCostGroup) ||
                (input.IsStandardCostGroup && input.IsUnit) ||
                (input.IsItemGroup && input.IsUnit && input.IsStandardCostGroup))
            {
                throw new UserFriendlyException(L("DuplicateIsUnitAndItemGroup"));
            }

            if (input.IsUnit)
            {
                var @isUnit = await _propertyRepository.GetAll().Where(t => t.IsUnit && input.Id != t.Id).AnyAsync();

                if (@isUnit) throw new UserFriendlyException(L("DuplicateIsUnit"));

            }

            if (input.IsItemGroup)
            {
                var @isItemGroup = await _propertyRepository.GetAll().Where(t => t.IsItemGroup && input.Id != t.Id).AnyAsync();

                if (isItemGroup) throw new UserFriendlyException(L("DuplicateIsItemGroup"));
            }

            if (input.IsStandardCostGroup)
            {
                var @isStandardCostGroup = await _propertyRepository.GetAll().Where(t => t.IsStandardCostGroup && input.Id != t.Id).AnyAsync();

                if (isStandardCostGroup) throw new UserFriendlyException(L("DuplicateIsStandardCostGroup"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _propertyManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.Name, input.IsUnit, input.IsRequired, input.IsStatic, input.IsItemGroup, input.IsStandardCostGroup);

            CheckErrors(await _propertyManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PropertyDetailOutput>(@entity);
        }
        #endregion

        #region Property Value Methods
        [AbpAuthorize(AppPermissions.Pages_Tenant_PropertyValue_Delete)]
        public async Task DeleteValue(EntityDto<long> input)
        {
            var @entity = await _propertyValueManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            CheckErrors(await _propertyValueManager.RemoveAsync(@entity));

        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_PropertyValue_AddValue)]
        public async Task<PropertyValueDetailOutput> AddValue(AddPropertyValueInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            if (input.IsDefault)
            {
                var checkIsDefault = await _propertyValueRepository.GetAll().AsNoTracking().Where(t => t.IsDefault && t.PropertyId == input.PropertyId).AnyAsync();
                if (checkIsDefault) throw new UserFriendlyException(L("CannotCheckMultiDefault"));
            }
            var @entity = PropertyValue.Create(tenantId, userId, input.PropertyId, input.Value, input.NetWeight, input.IsDefault, input.Code, input.IsBaseUnit, input.BaseUnitId, input.Factor);

            CheckErrors(await _propertyValueManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PropertyValueDetailOutput>(@entity);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_PropertyValue_EditValue)]
        public async Task<PropertyValueDetailOutput> EditValue(EditPropertyValueInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.IsDefault)
            {
                var checkIsDefault = await _propertyValueRepository.GetAll().AsNoTracking().Where(t => t.IsDefault && t.PropertyId == input.PropertyId && t.Id != input.Id).AnyAsync();
                if (checkIsDefault) throw new UserFriendlyException(L("CannotCheckMultiDefault"));
            }
            var @entity = await _propertyValueManager.GetAsync(input.Id, true);
            @entity.Update(userId, input.Value, input.NetWeight, input.IsDefault, input.Code, input.IsBaseUnit, input.BaseUnitId, input.Factor);

            CheckErrors(await _propertyValueManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PropertyValueDetailOutput>(@entity);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_PropertyValue_GetValue)]
        public async Task<PagedResultDto<PropertyValueDetailOutput>> GetValues(GetPropertyValueListInput input)
        {
            var @query = _propertyValueRepository
                 .GetAll()
                 .Include(u => u.Property)
                 .AsNoTracking()
                 .Where(u => u.PropertyId == input.PropertyId)
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Value.ToLower().Contains(input.Filter.ToLower())
                     ||
                     p.Code.ToLower().Contains(input.Filter.ToLower())
                 )
                 .Select(s => new PropertyValueDetailOutput
                 {
                     Id = s.Id,
                     IsActive = s.IsActive,
                     Value = s.Value,
                     Property = ObjectMapper.Map<PropertyDetailOutput>(s.Property),
                     NetWeight = s.NetWeight,
                     IsDefault = s.IsDefault,
                     Code = s.Code,
                     IsBaseUnit = s.IsBaseUnit,
                     BaseUnitId = s.BaseUnitId,
                     BaseUnitName = s.BaseUnitId.HasValue ? s.BaseUnit.Value : "",
                     Factor = s.Factor
                 });
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(s => s.Code)
                .ThenBy(s => s.Value)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PropertyValueDetailOutput>(resultCount, @entities);

        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_PropertyValue_FindValue)]
        public async Task<PagedResultDto<PropertyValueDetailOutput>> FindValues(GetPropertyValueListInput input)
        {
            var @query = _propertyValueRepository
                 .GetAll()
                 .Include(u => u.Property)
                 .AsNoTracking()
                 .Where(u => u.PropertyId == input.PropertyId)
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                      p => p.Value.ToLower().Contains(input.Filter.ToLower()) ||
                      (p.Code != null && p.Code.ToLower().Contains(input.Filter.ToLower())));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PropertyValueDetailOutput>(resultCount, ObjectMapper.Map<List<PropertyValueDetailOutput>>(@entities));

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PropertyValue_FindValue)]
        public async Task<PagedResultDto<PropertyValueDetailOutput>> FindPOSFilterCategories(GetPOSCategoryListInput input)
        {
            if (input.Sorting.IsNullOrWhiteSpace()) input.Sorting = "Value";

            var propertyId = (await GetCurrentTenantAsync()).PropertyId;
            if (propertyId == null)
            {
                var property = await _propertyRepository.GetAll().FirstOrDefaultAsync();
                if (property != null) propertyId = property.Id;
            }

            var @query = _propertyValueRepository
                 .GetAll()
                 .Include(u => u.Property)
                 .AsNoTracking()
                 .Where(u => u.PropertyId == propertyId)
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Value.ToLower().Contains(input.Filter.ToLower())
                 );
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PropertyValueDetailOutput>(resultCount, ObjectMapper.Map<List<PropertyValueDetailOutput>>(@entities));

        }
        #endregion

        #region import and export to excel

        private ReportOutput GetReportTemplateProperty()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Name",
                        ColumnLength = 100,
                        ColumnTitle = "Name",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                        IsRequired = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsUnit",
                        ColumnLength = 100,
                        ColumnTitle = "IsUnit",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                          IsRequired = true,
                    },

                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsRequired",
                        ColumnLength = 100,
                        ColumnTitle = "Is Required",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                          IsRequired = true,
                    },
                           new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsItemGroup",
                        ColumnLength = 100,
                        ColumnTitle = "Is Item Group",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                        IsRequired = true,
                    }
                     ,
                        new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsStandardGroup",
                        ColumnLength = 100,
                        ColumnTitle = "Is Standard Cost Group",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false,
                        IsRequired = true,
                    }
                },
                Groupby = "",
                HeaderTitle = "Property",
                Sortby = "",
            };
            return result;
        }

        private ReportOutput GetReportTemplateSubItem()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                    
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Value",
                        ColumnLength = 150,
                        ColumnTitle = "Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                          IsRequired = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Code",
                        ColumnLength = 100,
                        ColumnTitle = "Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        IsRequired = false,
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PropertyName",
                        ColumnLength = 150,
                        ColumnTitle = "Property Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                          IsRequired = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 120,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.Number,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Default",
                        ColumnLength = 120,
                        ColumnTitle = "Default",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                       new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsBaseUnit",
                        ColumnLength = 120,
                        ColumnTitle = "Is Base Unit",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = false,
                    },
                        new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BaseUnitName",
                        ColumnLength = 150,
                        ColumnTitle = "Base Unit",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = false,
                    },
                         new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Factor",
                        ColumnLength = 100,
                        ColumnTitle = "Factor",
                        ColumnType = ColumnType.Number,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = false,
                    },
                },
                Groupby = "",
                HeaderTitle = "PropertyValue",
                Sortby = "",
            };
            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Export_Excel)]
        public async Task<FileDto> ExportExcel()
        {
            //var inputItem = new GetPropertyListInput()
            //{
            //    UsePagination = false
            //};

            var @itemData = await _propertyRepository.GetAll()
                                  .AsNoTracking()
                                  .OrderBy(s => s.Name)
                                  .ToListAsync();

            var subitems = await _propertyValueRepository
                                 .GetAll()
                                 .AsNoTracking()
                                 .Select(s => new CreatePropertyValueInputExcel
                                 {
                                      Value = s.Value,
                                      Id = s.Id,
                                      PropertyName = s.Property.Name,
                                      NetWeight = s.NetWeight,
                                      IsDefault = s.IsDefault,
                                      Code = s.Code,
                                      IsBaseUnit = s.IsBaseUnit,
                                      BaseUnitId = s.BaseUnitId,
                                      BaseUnitName = s.BaseUnitId.HasValue ? s.BaseUnit.Value : "",
                                      Factor = s.Factor
                                  })
                                 .OrderBy(s => s.PropertyName)
                                 .ThenBy(s => s.Code)
                                 .ThenBy(s => s.Value)
                                 .ToListAsync();

            var result = new FileDto();
            var sheetName = "Property";
            var seetNameProPertyValue = "PropertyValue";
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
                var headerList = GetReportTemplateProperty();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 1
                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "IsUnit")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(ws, rowBody, collumnCellBody, strList, i.IsUnit.ToString());
                        }
                        else if (h.ColumnName == "IsRequired")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(ws, rowBody, collumnCellBody, strList, i.IsRequired.ToString());
                        }
                        else if (h.ColumnName == "IsItemGroup")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(ws, rowBody, collumnCellBody, strList, i.IsItemGroup.ToString());
                        }
                        else if (h.ColumnName == "IsStandardGroup")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(ws, rowBody, collumnCellBody, strList, i.IsStandardCostGroup.ToString());
                        }
                        else
                        {
                            WriteBodyPropertys(ws, rowBody, collumnCellBody, h, i, count);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body             
                //A workbook must have at least on cell, so lets add one... 
                var subitem = p.Workbook.Worksheets.Add(seetNameProPertyValue);
                subitem.PrinterSettings.Orientation = eOrientation.Landscape;
                subitem.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                subitem.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                subitem.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowSubTableHeader = 1;
                int colSubHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerSubList = GetReportTemplateSubItem();

                foreach (var i in headerSubList.ColumnInfo)
                {

                    AddTextToCell(subitem, rowSubTableHeader, colSubHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);

                    subitem.Column(colSubHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colSubHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowSubBody = rowSubTableHeader + 1;//start from row header of spreadsheet
                int countsub = 1;
                // write body
                foreach (var i in subitems)
                {
                    int collumnSubCellBody = 1;
                    foreach (var h in headerSubList.ColumnInfo)
                    {
                        if (h.ColumnName == "Default")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(subitem, rowSubBody, collumnSubCellBody, strList, i.IsDefault.ToString());
                        }
                        else if (h.ColumnName == "IsBaseUnit")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(subitem, rowSubBody, collumnSubCellBody, strList, i.IsBaseUnit.ToString());
                        }
                        else
                        {
                            WriteBodyPropertyValues(subitem, rowSubBody, collumnSubCellBody, h, i, count);
                        }

                        collumnSubCellBody += 1;
                    }
                    rowSubBody += 1;
                    countsub += 1;
                }
                #endregion Row Body    

                result.FileName = $"Property_Value.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Import_Excel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var properties = new List<Property>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    properties = await _propertyRepository.GetAll().AsNoTracking().ToListAsync();
                }
            }


            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var addProperties = new List<Property>();
            var addPropertyValueDic = new Dictionary<string, List<PropertyValue>>();
            var updateBaseUnitDic = new Dictionary<string, List<PropertyValue>>();
            var unitProperty = string.Empty;

            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //   loop all rows
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {  
                        if (i > 1)
                        {
                            var name = worksheet.Cells[i, 1].Value?.ToString();
                            if(name.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Name") + $", Row = {i}"));

                            if (properties.Any(t => t.Name == name)) continue;

                            var isUnit = bool.TryParse(worksheet.Cells[i, 2].Value?.ToString(), out bool bUnit) ? bUnit : false;
                            var isRequired = bool.TryParse(worksheet.Cells[i, 3].Value?.ToString(), out bool bRequired) ? bRequired : false;
                            var isItemGroup = bool.TryParse(worksheet.Cells[i, 4].Value?.ToString(), out bool bItemGroup) ? bItemGroup : false;
                            var isStandardCostGroup = bool.TryParse(worksheet.Cells[i, 5].Value?.ToString(), out bool bStandardCostGroup) ? bStandardCostGroup : false;

                            if ((isItemGroup && isUnit) ||
                               (isItemGroup && isStandardCostGroup) ||
                               (isStandardCostGroup && isUnit) ||
                               (isItemGroup && isUnit && isStandardCostGroup))
                            {
                                throw new UserFriendlyException(L("DuplicateIsUnitAndItemGroup") + $", Row = {i}");
                            }

                            if (isUnit && properties.Any(t => t.IsUnit)) throw new UserFriendlyException(L("DuplicateIsUnit"));  
                            if (isItemGroup && properties.Any(t => t.IsItemGroup)) throw new UserFriendlyException(L("DuplicateIsItemGroup"));  
                            if (isStandardCostGroup && properties.Any(t => t.IsStandardCostGroup)) throw new UserFriendlyException(L("DuplicateIsStandardCostGroup"));  

                            var property = Property.Create(tenantId, userId, name, isUnit, isRequired, false, isItemGroup, isStandardCostGroup);
                            addProperties.Add(property);
                            properties.Add(property);

                            if(isUnit) unitProperty = name;
                        }
                    }

                    var subWorkSeet = excelPackage.Workbook.Worksheets[1];
                    for (int s = subWorkSeet.Dimension.Start.Row; s <= subWorkSeet.Dimension.End.Row; s++)
                    {
                        if (s > 1)
                        {
                            var propertyName = subWorkSeet.Cells[s, 3].Value?.ToString();
                            if(propertyName.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("PropertyName") + $", Row = {s}"));

                            var property = properties.FirstOrDefault(t => t.Name == propertyName);
                            if (property == null) throw new UserFriendlyException(L("IsNotValid", L("Property")) + $", Row = {s}");

                            var value = subWorkSeet.Cells[s, 1].Value?.ToString();
                            if(value.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Value") + $", Row = {s}"));
                            if(addPropertyValueDic.ContainsKey(propertyName) && 
                               addPropertyValueDic[propertyName].Any(r => r.Value == value)) throw new UserFriendlyException(L("DuplicateValue") + $", Row = {s}");

                            var code = subWorkSeet.Cells[s, 2].Value?.ToString();
                            if(!code.IsNullOrEmpty() && 
                                addPropertyValueDic.ContainsKey(propertyName) && 
                                addPropertyValueDic[propertyName].Any(r => r.Code == code)) throw new UserFriendlyException(L("DuplicateCode") + $", Row = {s}");

                            var netWeight = decimal.TryParse(subWorkSeet.Cells[s, 4].Value?.ToString(), out decimal dNetWeight) ? dNetWeight : 0;
                            var isDefault = bool.TryParse(subWorkSeet.Cells[s, 5].Value?.ToString(), out bool bDefault) ? bDefault : false;

                            if(isDefault && 
                               addPropertyValueDic.ContainsKey(propertyName) &&
                               addPropertyValueDic[propertyName].Any(r => r.IsDefault)) throw new UserFriendlyException(L("CannotCheckMultiDefault") + $", Row = {s}");

                            var isBaseUnit = bool.TryParse(subWorkSeet.Cells[s, 6].Value?.ToString(), out bool bBaseUnit) ? bBaseUnit : false;
                            var baseUnitName = subWorkSeet.Cells[s, 7].Value?.ToString();
                            var factor = decimal.TryParse(subWorkSeet.Cells[s, 8].Value?.ToString(), out decimal dFactor) ? dFactor : 0;

                            var propertyValue = PropertyValue.Create(tenantId, userId, property.Id, value, netWeight, isDefault, code, isBaseUnit, null, factor);

                            if (property.IsUnit && !isBaseUnit)
                            {
                                if(baseUnitName.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("BaseUnit") + $", Row = {s}"));

                                var baseUnit = addPropertyValueDic.ContainsKey(propertyName) ?
                                               addPropertyValueDic[propertyName].FirstOrDefault(t => t.Value == baseUnitName && t.IsBaseUnit) : null;

                                if (baseUnit == null) throw new UserFriendlyException(L("IsNotValid", L("BaseUnit") + $", Row = {s}"));


                                if (updateBaseUnitDic.ContainsKey(baseUnitName))
                                {
                                    updateBaseUnitDic[baseUnitName].Add(propertyValue);
                                }
                                else
                                {
                                    updateBaseUnitDic.Add(baseUnitName, new List<PropertyValue> { propertyValue });
                                }
                            }

                            if(addPropertyValueDic.ContainsKey(propertyName))
                            {
                                addPropertyValueDic[propertyName].Add(propertyValue);
                            }
                            else
                            {
                                addPropertyValueDic.Add(propertyName, new List<PropertyValue> { propertyValue });
                            }
                        }
                    }

                }
            }

            if (addProperties.Any())
            {
                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await _propertyRepository.BulkInsertAsync(addProperties);

                        foreach (var p in addProperties)
                        {
                            if (addPropertyValueDic.ContainsKey(p.Name))
                            {
                                foreach (var v in addPropertyValueDic[p.Name])
                                {
                                    v.SetProperty(p.Id);
                                }
                            }
                            else
                            {
                                throw new UserFriendlyException(L("IsRequired", $"{p.Name} " + L("Value")));
                            }
                        }

                        await _propertyValueRepository.BulkInsertAsync(addPropertyValueDic.Values.SelectMany(t => t).ToList());

                        if (addPropertyValueDic.ContainsKey(unitProperty) && updateBaseUnitDic.Any())
                        {
                            foreach(var item in updateBaseUnitDic)
                            {
                                var baseUnit = addPropertyValueDic[unitProperty].FirstOrDefault(t => t.Value == item.Key);
                                if (baseUnit != null)
                                {
                                    foreach (var v in item.Value)
                                    {
                                        v.SetBaseUnit(baseUnit.Id);
                                    }
                                }
                            }

                            await _propertyValueRepository.BulkUpdateAsync(updateBaseUnitDic.Values.SelectMany(t => t).ToList());
                        }

                    }

                    await uow.CompleteAsync();
                }
            }

        }

        #endregion

        #region import Update 


        [AbpAuthorize(AppPermissions.Pages_Tenant_Properties_Import_Excel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportUpdateProperty(FileDto input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var properties = new List<Property>();
            var propertyValues = new List<PropertyValue>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    properties = await _propertyRepository.GetAll().AsNoTracking().ToListAsync();
                    propertyValues = await _propertyValueRepository.GetAll().AsNoTracking().ToListAsync();
                }
            }


            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var updateProperties = new List<Property>();
            var addPropertyValues = new List<PropertyValue>();
            var updatePropertyValues = new List<PropertyValue>();
            var deletePropertyValues = new List<PropertyValue>();
            var updateBaseUnitDic = new Dictionary<string, List<PropertyValue>>();
            Property unitProperty = null;

            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //   loop all rows
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var name = worksheet.Cells[i, 1].Value?.ToString();
                            if (name.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Name") + $", Row = {i}"));

                            var property = properties.FirstOrDefault(t => t.Name == name);
                            if (property == null) throw new UserFriendlyException(L("IsNotValid", L("Property")) + $", Row = {i}");

                            var isUnit = bool.TryParse(worksheet.Cells[i, 2].Value?.ToString(), out bool bUnit) ? bUnit : false;
                            var isRequired = bool.TryParse(worksheet.Cells[i, 3].Value?.ToString(), out bool bRequired) ? bRequired : false;
                            var isItemGroup = bool.TryParse(worksheet.Cells[i, 4].Value?.ToString(), out bool bItemGroup) ? bItemGroup : false;
                            var isStandardCostGroup = bool.TryParse(worksheet.Cells[i, 5].Value?.ToString(), out bool bStandardCostGroup) ? bStandardCostGroup : false;

                            if ((isItemGroup && isUnit) ||
                               (isItemGroup && isStandardCostGroup) ||
                               (isStandardCostGroup && isUnit) ||
                               (isItemGroup && isUnit && isStandardCostGroup))
                            {
                                throw new UserFriendlyException(L("DuplicateIsUnitAndItemGroup") + $", Row = {i}");
                            }

                            if (isUnit && properties.Any(t => t.IsUnit && t.Id != property.Id)) throw new UserFriendlyException(L("DuplicateIsUnit"));
                            if (isItemGroup && properties.Any(t => t.IsItemGroup && t.Id != property.Id)) throw new UserFriendlyException(L("DuplicateIsItemGroup"));
                            if (isStandardCostGroup && properties.Any(t => t.IsStandardCostGroup && t.Id != property.Id)) throw new UserFriendlyException(L("DuplicateIsStandardCostGroup"));

                            property.Update(userId, name, isUnit, isRequired, property.IsStatic, isItemGroup, isStandardCostGroup);
                            updateProperties.Add(property);
                          
                            if (isUnit) unitProperty = property;
                        }
                    }

                    var subWorkSeet = excelPackage.Workbook.Worksheets[1];
                    for (int s = subWorkSeet.Dimension.Start.Row; s <= subWorkSeet.Dimension.End.Row; s++)
                    {
                        if (s > 1)
                        {
                            var propertyName = subWorkSeet.Cells[s, 3].Value?.ToString();
                            if (propertyName.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("PropertyName") + $", Row = {s}"));

                            var property = properties.FirstOrDefault(t => t.Name == propertyName);
                            if (property == null) throw new UserFriendlyException(L("IsNotValid", L("Property")) + $", Row = {s}");

                            var value = subWorkSeet.Cells[s, 1].Value?.ToString();
                            if (value.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Value") + $", Row = {s}"));

                            var propertyValue = propertyValues.FirstOrDefault(t => t.Value == value && t.PropertyId == property.Id);

                            var code = subWorkSeet.Cells[s, 2].Value?.ToString();
                            var findCode = propertyValues.Any(t => t.Code == code && t.PropertyId == property.Id && (propertyValue == null || t.Id != propertyValue.Id));
                            if (!code.IsNullOrEmpty() && findCode) throw new UserFriendlyException(L("DuplicateCode") + $", Row = {s}");

                            var netWeight = decimal.TryParse(subWorkSeet.Cells[s, 4].Value?.ToString(), out decimal dNetWeight) ? dNetWeight : 0;
                            var isDefault = bool.TryParse(subWorkSeet.Cells[s, 5].Value?.ToString(), out bool bDefault) ? bDefault : false;

                            var findDefault = propertyValues.Any(t => t.IsDefault && t.PropertyId == property.Id && (propertyValue == null || t.Id != propertyValue.Id));
                            if (isDefault && findDefault) throw new UserFriendlyException(L("CannotCheckMultiDefault") + $", Row = {s}");

                            var isBaseUnit = bool.TryParse(subWorkSeet.Cells[s, 6].Value?.ToString(), out bool bBaseUnit) ? bBaseUnit : false;
                            var baseUnitName = subWorkSeet.Cells[s, 7].Value?.ToString();
                            var factor = decimal.TryParse(subWorkSeet.Cells[s, 8].Value?.ToString(), out decimal dFactor) ? dFactor : 0;


                            if (propertyValue == null)
                            {
                                propertyValue = PropertyValue.Create(tenantId, userId, property.Id, value, netWeight, isDefault, code, isBaseUnit, null, factor);
                                addPropertyValues.Add(propertyValue);
                                propertyValues.Add(propertyValue);
                            }
                            else
                            {
                                if(propertyValue.Id == 0) throw new UserFriendlyException(L("DuplicateValue") + $", Row = {s}");

                                propertyValue.Update(userId, value, netWeight, isDefault, code, isBaseUnit, isBaseUnit ? null : propertyValue.BaseUnitId, factor);
                                updatePropertyValues.Add(propertyValue);
                            }


                            if (property.IsUnit && !isBaseUnit)
                            {
                                if (baseUnitName.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("BaseUnit") + $", Row = {s}"));

                                var baseUnit = propertyValues.FirstOrDefault(t => t.Value == baseUnitName && t.IsBaseUnit) ?? null;

                                if (baseUnit == null) throw new UserFriendlyException(L("IsNotValid", L("BaseUnit") + $", Row = {s}"));


                                if (baseUnit.Id > 0)
                                {
                                    propertyValue.SetBaseUnit(baseUnit.Id);
                                }
                                else if(updateBaseUnitDic.ContainsKey(baseUnitName))
                                {
                                    updateBaseUnitDic[baseUnitName].Add(propertyValue);
                                }
                                else
                                {
                                    updateBaseUnitDic.Add(baseUnitName, new List<PropertyValue> { propertyValue });
                                }
                            }

                        }
                    }

                }
            }

            foreach (var p in updateProperties)
            {
                var addValue = addPropertyValues.Any(t => t.PropertyId == p.Id);
                var updateValues = updatePropertyValues.Where(t => t.PropertyId == p.Id).ToList();

                if (!addValue && !updateValues.Any()) throw new UserFriendlyException(L("IsRequired", $"{p.Name} " + L("Value")));

                var deleteValues = propertyValues.Where(t => t.PropertyId == p.Id && t.Id > 0 && !updateValues.Any(r => r.Id == t.Id)).ToList();
                if(deleteValues.Any()) deletePropertyValues.AddRange(deleteValues);
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {     
                    await _propertyRepository.BulkUpdateAsync(updateProperties);

                    if (addPropertyValues.Any()) await _propertyValueRepository.BulkInsertAsync(addPropertyValues);
                    if (updatePropertyValues.Any()) await _propertyValueRepository.BulkUpdateAsync(updatePropertyValues);
                    if (deletePropertyValues.Any()) await _propertyValueRepository.BulkDeleteAsync(deletePropertyValues);

                    if (updateBaseUnitDic.Any())
                    {
                        foreach (var item in updateBaseUnitDic)
                        {
                            var baseUnit = addPropertyValues.FirstOrDefault(t => t.PropertyId == unitProperty.Id && t.Value == item.Key);
                            if (baseUnit != null)
                            {
                                foreach (var v in item.Value)
                                {
                                    v.SetBaseUnit(baseUnit.Id);
                                }
                            }
                        }

                        await _propertyValueRepository.BulkUpdateAsync(updateBaseUnitDic.Values.SelectMany(t => t).ToList());
                    }

                }

                await uow.CompleteAsync();
            }
        }
        #endregion
    }
}
