using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using CorarlERP.AccountTypes.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.UI;
using CorarlERP.ChartOfAccounts;
using static CorarlERP.enumStatus.EnumStatus;
using System;
using System.Linq.Dynamic.Core;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using OfficeOpenXml;
using CorarlERP.Reports;
using System.IO;
using CorarlERP.FileStorages;

namespace CorarlERP.AccountTypes
{
    [AbpAuthorize]
    public class AccountTypeAppService : ReportBaseClass, IAccountTypeAppService
    {
        private readonly IAccountTypeManager _accountTypeManager;
        private readonly IRepository<AccountType, long> _accountTypeRepository;
        private readonly AppFolders _appFolders;
        private readonly IFileStorageManager _fileStorageManager;

        public AccountTypeAppService(IAccountTypeManager accountTypeManager, 
                             AppFolders appFolders,
                             IRepository<AccountType, long> accountTypeRepository,
                             IFileStorageManager fileStorageManager) : base(fileStorageManager)
        {
            _accountTypeManager = accountTypeManager;
            _accountTypeRepository = accountTypeRepository;
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes_Create)]
        public async Task<AccountTypeDetailOutput> Create(CreateAccountTypeInput input)
        {
            var userId = AbpSession.GetUserId();
            var @entity = AccountType.Create(userId, input.AccountTypeName, input.Type, input.Description);

            CheckErrors(await _accountTypeManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            
            return ObjectMapper.Map<AccountTypeDetailOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes)]
        public async Task<PagedResultDto<AccountTypeDetailOutput>> GetList(GetAccountTypeListInput input)
        {
            var query = _accountTypeRepository
                .GetAll()
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.AccountTypeName.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Description.ToLower().Contains(input.Filter.ToLower())
                )
                .Select( t => new AccountTypeDetailOutput
                {
                    Id = t.Id,
                    Type = Enum.GetName(typeof(TypeOfAccount), t.Type),
                    TypeCode = t.Type,
                    AccountTypeName = t.AccountTypeName,
                    Description = t.Description,
                    IsActive = t.IsActive
                });

            var resultCount = await query.CountAsync();
            var @entities = new List<AccountTypeDetailOutput>();
            if (input.UsePagination)
            {
                entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                entities = await query.ToListAsync();
            }
            return new PagedResultDto<AccountTypeDetailOutput>(resultCount, ObjectMapper.Map<List<AccountTypeDetailOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes)]
        public async Task<ListResultDto<AccountTypeDetailOutput>> Find(GetAccountTypeListInput input)
        {
            var @entities = await _accountTypeRepository
               .GetAll()
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.AccountTypeName.ToLower().Contains(input.Filter.ToLower()) ||
                        p.Description.ToLower().Contains(input.Filter.ToLower())
               )
               .OrderBy(p => p.AccountTypeName)
               .PageBy(input)
               .ToListAsync();

           return new ListResultDto<AccountTypeDetailOutput>(ObjectMapper.Map<List<AccountTypeDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes)]
        public async Task<ListResultDto<NameValueDto<SubAccountType>>> GetSubAccountTypes(long? accountTypeId)
        {
            var accountType = await _accountTypeRepository.FirstOrDefaultAsync(s => s.Id == accountTypeId);
            if(accountType != null)
            {
                
                return new ListResultDto<NameValueDto<SubAccountType>> { Items = accountType.SubTypes().Select(s => s.NameValue()).ToList() };
            }

            return new ListResultDto<NameValueDto<SubAccountType>> { Items = SubAccountType.AccumulatedDepreciation.ToList().Select(s => s.NameValue()).ToList() };
        }



        private async Task<AccountTypeDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _accountTypeManager.GetAsync(input.Id);

            if (entity == null){
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            return ObjectMapper.Map<AccountTypeDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes_Edit)]
        public async Task<AccountTypeDetailOutput> Update(UpateAccountTypeInput input)
        {
            var userId = AbpSession.GetUserId();

            var @entity = await _accountTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.AccountTypeName, input.Type, input.Description);

            CheckErrors(await _accountTypeManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<AccountTypeDetailOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _accountTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _accountTypeManager.RemoveAsync(@entity));   
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _accountTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _accountTypeManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _accountTypeManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _accountTypeManager.DisableAsync(@entity));
        }


        #region import and export Excel 

        private ReportOutput GetReportTemplateAccountType()
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
                        ColumnName = "TypeOfAccount",
                        ColumnLength = 230,
                        ColumnTitle = "Type Of Account",
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
                        ColumnName = "Description",
                        ColumnLength = 250,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },                
                },
                Groupby = "",
                HeaderTitle = "Account Type",
                Sortby = "",
            };
            return result;
        }

        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var typeOfAccounts = await _accountTypeRepository.GetAll().ToListAsync();
            var accounttypes = Enum.GetValues(typeof(TypeOfAccount)).Cast<TypeOfAccount>().Select(v => v).ToList();
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    //loop all rows
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var parentId = typeOfAccounts.Where(s => s.AccountTypeName == worksheet.Cells[i, 1].Value.ToString()).Select(s => s.Id).Count();
                            var accountTypeId = accounttypes.Where(s => s.ToString() == worksheet.Cells[i, 2].Value?.ToString()).Select(s => s).FirstOrDefault();                           
                            var createInput = new CreateAccountTypeInput()
                            {
                                AccountTypeName = worksheet.Cells[i, 1].Value.ToString(),
                                Type = accountTypeId,                           
                                Description = worksheet.Cells[i, 3].Value.ToString(),
                            };

                            if (parentId == 0)
                            {
                                await Create(createInput);
                            }
                        }
                    }
                }
            }

            //RemoveFile(input, _appFolders);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_AccountTypes_ExportExcel)]
        public async Task<FileDto> ExportExcel()
        {
         
            var inputTypeOfAccount = new GetAccountTypeListInput()
            {
                UsePagination = false
            };
            var typeOfAccountData = (await GetList(inputTypeOfAccount)).Items;
            //var accountype = await (from a in  TypeOfAccount select a);
            var result = new FileDto();
            var sheetName = "Type Of Accounts";
            var accounttypes = Enum.GetValues(typeof(TypeOfAccount)).Cast<TypeOfAccount>().Select(v => v.ToString()).ToList();
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
                var headerList = GetReportTemplateAccountType();

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
                foreach (var i in typeOfAccountData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "TypeOfAccount")
                        {
                            AddDropdownList(ws, rowBody, collumnCellBody, accounttypes, i.Type);
                        }
                        else
                        {
                            WriteBodyTypeOfAccount(ws, rowBody, collumnCellBody, h, i, count);
                        }
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion Row Body
                result.FileName = $"TypeOfAccount_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion
    }
}
