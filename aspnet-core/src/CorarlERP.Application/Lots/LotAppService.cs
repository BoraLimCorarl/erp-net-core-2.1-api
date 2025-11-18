using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Lots.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Locations.Dto;
using CorarlERP.Locations;
using CorarlERP.UserGroups;
using System;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using Abp.Domain.Uow;
using CorarlERP.MultiTenancy;
using CorarlERP.Dto;
using CorarlERP.Reports;
using System.Transactions;
using OfficeOpenXml;
using CorarlERP.FileStorages;

namespace CorarlERP.Lots
{
    public class LotAppService : ReportBaseClass, ILotAppService
    {
        private readonly ILotManager _lotManager;
        private readonly ICorarlRepository<Lot, long> _lotRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly AppFolders _appFolders;
        public LotAppService(ILotManager lotManager,
            ICorarlRepository<Lot, long> lotRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Location, long> locationRepository,
             AppFolders appFolders,
            IFileStorageManager fileStorageManager) : base(null, appFolders, userGroupMemberRepository, null)
        {
            _lotManager = lotManager;
            _lotRepository = lotRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _locationRepository = locationRepository;
            _fileStorageManager = fileStorageManager;
            _appFolders = appFolders;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Create)]
        public async Task<LotDetailOutput> Create(CreateLotInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = Lot.Create(tenantId, userId, input.LotName, input.LocationId);

            CheckErrors(await _lotManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<LotDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _lotManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _lotManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _lotManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _lotManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _lotManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _lotManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Find)]
        public async Task<PagedResultDto<LotDetailOutput>> Find(FindLotInput input)
        {
            var selectedValues = new List<Lot>();
            if (input.SelectedValues != null && input.SelectedValues.Any())
            {
                selectedValues = await _lotRepository.GetAll().AsNoTracking().Where(s => input.SelectedValues.Contains(s.Id)).ToListAsync();
            }


            if (input.IsExcept == null || input.IsExcept == false)
            {
                #region Get LocationIDS by group user
                var userId = AbpSession.GetUserId();
                // get user by group member
                var userGroups = await _userGroupMemberRepository.GetAll()
                                .Where(x => x.MemberId == userId)
                                .Where(x => x.UserGroup.LocationId != null)
                                .AsNoTracking()
                                .Select(x => x.UserGroup.LocationId)
                                .ToListAsync();

                #endregion

                var @query = _lotRepository
                                    .GetAll()
                                    .Include(u => u.Location)
                                    .AsNoTracking()
                                    .Where(s => s.Location.Member == Member.All || userGroups.Contains(s.LocationId))
                                    .WhereIf(input.Locations != null && input.Locations.Count > 0, p => input.Locations.Contains(p.LocationId))
                                    .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                    .WhereIf(
                                        !input.Filter.IsNullOrEmpty(),
                                        p => p.LotName.ToLower().Contains(input.Filter.ToLower()));

                var resultCount = await query.CountAsync();
                var @entities = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();
                return new PagedResultDto<LotDetailOutput>(resultCount, ObjectMapper.Map<List<LotDetailOutput>>(selectedValues.Union(@entities)));
            }
            else
            {
                var @query = _lotRepository
                            .GetAll()
                            .Include(u => u.Location)
                            .AsNoTracking()
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, p => input.Locations.Contains(p.LocationId))
                            .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.LotName.ToLower().Contains(input.Filter.ToLower()));
                var resultCount = await query.CountAsync();
                var @entities = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();
                return new PagedResultDto<LotDetailOutput>(resultCount, ObjectMapper.Map<List<LotDetailOutput>>(selectedValues.Union(@entities)));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_GetDetail)]
        public async Task<LotDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _lotManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<LotDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_GetList)]
        public async Task<PagedResultDto<LotDetailOutput>> GetList(GetLotListInput input)
        {
            var @query = _lotRepository
                .GetAll()
                .Include(u => u.Location)
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(input.Locations != null, p => input.Locations.Contains(p.LocationId))
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.LotName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<LotDetailOutput>(resultCount, ObjectMapper.Map<List<LotDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Update)]
        public async Task<LotDetailOutput> Update(UpdateLotInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _lotManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.LotName, input.LocationId);

            CheckErrors(await _lotManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<LotDetailOutput>(@entity);
        }


        private ReportOutput ExportHeaderTempate()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "zoneName",
                        ColumnLength = 130,
                        ColumnTitle = "Zone Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsRequired = true,
                        DisableDefault = false
                    },
                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Location",
                        ColumnLength = 130,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsRequired = true,
                        DisableDefault = false
                    },


                },
                Groupby = "",
                HeaderTitle = "Zone",
                Sortby = "",
            };
            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            var userId = AbpSession.GetUserId();
            var tenantId = AbpSession.TenantId;
            var locations = new List<Location>();
            var zones = new List<Lot>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
                }
            }

            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var zoneName = worksheet.Cells[i, 1].Value?.ToString();
                            var locationName = worksheet.Cells[i, 2].Value?.ToString();
                            var locationId = locations.Where(s => s.LocationName == locationName).Select(t => t.Id).FirstOrDefault();

                            if (locationId == 0)
                            {
                                throw new UserFriendlyException(L("NoLocationFound"));
                            }
                            var create = Lot.Create(tenantId, userId, zoneName, locationId);
                            zones.Add(create);
                        }

                    }
                    var query = zones.GroupBy(x => x.LotName)
                              .Where(g => g.Count() > 1)
                              .Select(y => y.Key)
                              .ToList();
                    if (query.Count > 0)
                    {
                        foreach (var i in query)
                        {
                            throw new UserFriendlyException(L("DuplicateLotName", i));
                        }
                    }
                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                        {
                            await CheckDuplocate(zones.Select(t => t.LotName.ToLower()).ToList());
                            await _lotRepository.BulkInsertAsync(zones);
                        }
                        await uow.CompleteAsync();
                    }
                }
            }

        }


        private async Task CheckDuplocate(List<string> entities)
        {
            var @old = await _lotRepository.GetAll().AsNoTracking()
                          .Where(u => entities.Contains(u.LotName.ToLower()))
                          .Select(t => t.LotName).ToListAsync();

            if (old != null && old.Count > 0)
            {
                foreach (var i in old)
                {
                    throw new UserFriendlyException(L("DuplicateLotName", i));
                }

            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Lots_Export)]
        [UnitOfWork(IsDisabled = true)]
        public async Task<FileDto> ExportExcelTamplate()
        {

            var tenantId = AbpSession.TenantId;
            var lots = new List<ItemLotDto>();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    lots = await _lotRepository.GetAll().Include(u=>u.Location).AsNoTracking().Select(t => new ItemLotDto
                    {
                        Id = t.Id,
                        LocationId = t.LocationId,
                        LotName = t.LotName,
                        LocationName = t.Location.LocationName,
                    }).ToListAsync();
                }
            }
            var result = new FileDto();
            var sheetName = "zone";
            using (var p = new ExcelPackage())
            {
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;

                ws.Cells.Style.Font.Size = DefaultFontSize;
                ws.Cells.Style.Font.Name = DefaultFontName;

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;
                var headerList = ExportHeaderTempate();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                foreach (var i in lots)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo.Where(t => t.Visible).ToList())
                    {                     
                        WriteBodyLot(ws, rowBody, collumnCellBody, h, i, count);                       
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row 1
                result.FileName = $"zoneTemplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

    }
}
