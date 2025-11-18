using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using CorarlERP.ChartOfAccounts.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.UI;
using System;
using System.Linq.Dynamic.Core;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.AccountTransactions;
using CorarlERP.Dto;
using OfficeOpenXml;
using System.IO;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Taxes;
using Abp.AspNetZeroCore.Net;
using CorarlERP.AccountCycles;
using CorarlERP.FileStorages;

namespace CorarlERP.ChartOfAccounts
{
    [AbpAuthorize]
    public class ChartOfAccountAppService : ReportBaseClass, IChartOfAccountAppService
    {
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IRepository<AccountType, long> _accountTypeReposity;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IAccountTransactionManager _accountTransactionManager;
        private readonly AppFolders _appFolders;
        private readonly IFileStorageManager _fileStorageManager;
        public ChartOfAccountAppService(
            IChartOfAccountManager chartOfAccountManager,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IAccountTransactionManager accountTransactionManager,
            AppFolders appFolders,
            IFileStorageManager fileStorageManager,
            IRepository<Tax, long> taxRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<AccountType, long> accountTypeReposity) : base(null, appFolders, null, null)
        {
            _accountTransactionManager = accountTransactionManager;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _appFolders = appFolders;
            _accountTypeReposity = accountTypeReposity;
            _taxRepository = taxRepository;
            _accountCycleRepository = accountCycleRepository;
            _fileStorageManager = fileStorageManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Create)]
        public async Task<ChartAccountDetailOutput> Create(CreateChartAccountInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = ChartOfAccount.Create(tenantId, userId, input.AccountCode, input.AccountName, input.Description, input.AccountTypeId, input.ParentAccountId, input.TaxId);
            entity.SetSubAccountingType(input.SubAccountType);

            CheckErrors(await _chartOfAccountManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<ChartAccountDetailOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts)]
        public async Task<PagedResultDto<ChartAccountDetailOutput>> GetList(GetChartAccountListInput input)
        {
            var query = (from chart in _chartOfAccountRepository.GetAll()
                         .Include(u => u.AccountType)
                         .Include(u => u.Tax)
                         .Include(u => u.ParentAccount)
                            .AsNoTracking()
                            .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                            .WhereIf(input.AccountTypes != null, p => input.AccountTypes.Contains(p.AccountTypeId))
                            .WhereIf(
                            !input.Filter.IsNullOrEmpty(),
                            p => p.AccountCode.ToLower().Contains(input.Filter.ToLower()) ||
                            p.AccountName.ToLower().Contains(input.Filter.ToLower()) ||
                            p.Description.ToLower().Contains(input.Filter.ToLower()))

                             //join account in _accountTransactionManager.GetAccountQuery(null, DateTime.Now) on chart.Id equals account.AccountId into a
                             //from account in a.DefaultIfEmpty()
                         select new ChartAccountDetailOutput
                         {
                             AccountCode = chart.AccountCode,
                             AccountName = chart.AccountName,
                             AccountType = ObjectMapper.Map<AccountTypeDetailOutput>(chart.AccountType),
                             Description = chart.Description,
                             Id = chart.Id,
                             IsActive = chart.IsActive,
                             ParentAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(chart.ParentAccount),
                             Tax = ObjectMapper.Map<TaxDetailOutput>(chart.Tax),
                             //  Balance = account.Balance.ToString() == null ? 0 : account.Balance,
                             Balance = 0,
                             AccountTypeId = chart.AccountTypeId,
                             TaxId = chart.TaxId,
                             SubAccountType = chart.SubAccountType
                         });
            var resultCount = await query.CountAsync();

            var @entities = new List<ChartAccountDetailOutput>();
            if (input.UsePagination)
            {
                entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                entities = await query.ToListAsync();
            }
            return new PagedResultDto<ChartAccountDetailOutput>(resultCount, ObjectMapper.Map<List<ChartAccountDetailOutput>>(@entities));

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts)]
        public async Task<PagedResultDto<ChartAccountDetailOutput>> Find(GetChartAccountListInput input)
        {
            var @query = _chartOfAccountRepository
                .GetAll()
                .Include(u => u.Tax)
                .AsNoTracking()
                .WhereIf(input.ListChartOfAccounts != null && input.ListChartOfAccounts.Count > 0, p => input.ListChartOfAccounts.Contains(p.Id))
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(input.AccountTypes != null && input.AccountTypes.Any(), p => input.AccountTypes.Contains(p.AccountTypeId))
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.AccountCode.ToLower().Contains(input.Filter.ToLower()) ||
                         p.AccountName.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Description.ToLower().Contains(input.Filter.ToLower())
                );


            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ChartAccountDetailOutput>(resultCount, new List<ChartAccountDetailOutput>());

            var @entities = await query
                //.OrderBy(input.Sorting)
                .OrderBy(s => s.AccountCode)
                .PageBy(input)
                .ToListAsync();


            return new PagedResultDto<ChartAccountDetailOutput>(resultCount, ObjectMapper.Map<List<ChartAccountDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Detail)]
        public async Task<ChartAccountDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _chartOfAccountManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            return ObjectMapper.Map<ChartAccountDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Edit)]
        public async Task<ChartAccountDetailOutput> Update(UpateChartAccountInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _chartOfAccountManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.AccountCode, input.AccountName, input.Description,
                          input.AccountTypeId, input.ParentAccountId, input.TaxId);
            entity.SetSubAccountingType(input.SubAccountType);

            CheckErrors(await _chartOfAccountManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<ChartAccountDetailOutput>(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _chartOfAccountManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _chartOfAccountManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _chartOfAccountManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _chartOfAccountManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _chartOfAccountManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _chartOfAccountManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ImportExcel)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var @accountTypes = _accountTypeReposity.GetAll().AsNoTracking().ToList();
            var @parentAccounts = _chartOfAccountRepository.GetAll().AsNoTracking().ToList();
            var @taxs = _taxRepository.GetAll().AsNoTracking().ToList();
            var subAccountTypes = SubAccountType.AccumulatedAmortization.ToList();

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
                            var accountTypeId = accountTypes.Where(s => s.AccountTypeName == worksheet.Cells[i, 3].Value?.ToString()).Select(s => s.Id).FirstOrDefault();
                            var taxId = taxs.Where(s => s.TaxName == worksheet.Cells[i, 5].Value?.ToString()).Select(s => s.Id).FirstOrDefault();
                            Guid? parentId = parentAccounts.Where(s => s.AccountName == worksheet.Cells[i, 4].Value?.ToString()).Select(s => s.Id).FirstOrDefault();

                            SubAccountType? subAccountType = null;
                            var subType = worksheet.Cells[i, 7].Value;
                            if (subType != null && !string.IsNullOrWhiteSpace(subType?.ToString()))
                            {
                                try
                                {
                                    subAccountType = Enum.Parse<SubAccountType>(subType.ToString());
                                }
                                catch (Exception ex)
                                {
                                    throw new UserFriendlyException(L("IsNotValid", L("SubAccountType")));
                                }
                            }


                            var createInput = new CreateChartAccountInput()
                            {
                                AccountCode = worksheet.Cells[i, 1].Value?.ToString(),
                                AccountName = worksheet.Cells[i, 2].Value?.ToString(),
                                AccountTypeId = accountTypeId,
                                ParentAccountId = parentId == Guid.Empty ? null : parentId,
                                TaxId = taxId,
                                Description = worksheet.Cells[i, 6].Value?.ToString(),
                                SubAccountType = subAccountType
                            };

                            if (parentAccounts.Where(t => t.AccountCode == createInput.AccountCode).Count() == 0)
                            {
                                await Create(createInput);
                            }
                        }
                    }
                }
            }
            //RemoveFile(input, _appFolders);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ExportExcel)]
        public async Task<FileDto> ExportExcel()
        {
            var inputChartOfAccount = new GetChartAccountListInput()
            {
                UsePagination = false
            };
            var chartOfAccountData = (await GetList(inputChartOfAccount)).Items;
            var result = new FileDto();
            var sheetName = "Chart Of Accounts";
            var accountype = await _accountTypeReposity.GetAll().Select(t => t.AccountTypeName).ToListAsync();
            var subAccountTypes = SubAccountType.AccumulatedAmortization.ToList().Select(s => s.ToString()).ToList();
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
                var headerList = GetReportTemplateIncome();

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
                foreach (var i in chartOfAccountData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "AccountType")
                        {
                            AddDropdownList(ws, rowBody, collumnCellBody, accountype, i.AccountType.AccountTypeName);
                        }
                        else if (h.ColumnName == "SubAccountType")
                        {
                            AddDropdownList(ws, rowBody, collumnCellBody, subAccountTypes, i.SubAccountType.ToString());
                        }
                        else
                        {
                            WriteBodyChartOfAccount(ws, rowBody, collumnCellBody, h, i, count);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion Row Body

                result.FileName = $"ChartOfAccount_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        private ReportOutput GetReportTemplateIncome()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountCode",
                        ColumnLength = 180,
                        ColumnTitle = "Account Code",
                        ColumnType = ColumnType.Language,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountName",
                        ColumnLength = 250,
                        ColumnTitle = "Account Name",
                        ColumnType = ColumnType.Array,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountType",
                        ColumnLength = 230,
                        ColumnTitle = "Account Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ParentAccount",
                        ColumnLength = 250,
                        ColumnTitle = "Is Sub Account Of",
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
                        ColumnName = "Tax",
                        ColumnLength = 200,
                        ColumnTitle = "Tax",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 250,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SubAccountType",
                        ColumnLength = 250,
                        ColumnTitle = "Sub Account Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Balance",
                        ColumnLength = 130,
                        ColumnTitle = "Balance",
                        ColumnType = ColumnType.Money,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Chart Of Account",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ExportPdf)]
        public async Task<FileDto> ExportPDF()
        {
            var tenantId = AbpSession.GetTenantId();

            var tenant = await GetCurrentTenantAsync();

            int digit = await _accountCycleRepository.GetAll()
                .Where(t => t.TenantId == tenant.Id && t.EndDate == null)
                .OrderByDescending(t => t.StartDate)
                .Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var input = new GetChartAccountListInput();
            var chartOfAccounts = await GetList(input);
            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "chat-of-account.pdf",
                    FileToken = "chat-of-account.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did

                exportHtml = templateHtml;

                //@todo replace our variable 
                exportHtml = exportHtml.Replace("{{companyName}}", tenant.LegalName);
                exportHtml = exportHtml.Replace("{{headerTitle}}", L("ChartOfAccount"));
                exportHtml = exportHtml.Replace("{{No}}", L("No"));
                exportHtml = exportHtml.Replace("{{AccountCode}}", L("AccountCode"));
                exportHtml = exportHtml.Replace("{{AccountName}}", L("AccountName"));
                exportHtml = exportHtml.Replace("{{AccountType}}", L("AccountType"));
                exportHtml = exportHtml.Replace("{{Tax}}", L("Tax"));
                exportHtml = exportHtml.Replace("{{Description}}", L("Description"));
                exportHtml = exportHtml.Replace("{{Balance}}", L("Balance"));
                exportHtml = exportHtml.Replace("{{Status}}", L("Status"));
                var contentBody = string.Empty;
                var count = 1;

                foreach (var row in chartOfAccounts.Items)
                {
                    var IsActive = "";
                    if (row.IsActive == true)
                    {
                        IsActive = L("Active");
                    }
                    else
                    {
                        IsActive = L("DisActive");
                    }


                    var rowItem = $"<tr>" +
                    $"<td> {count}</td>" +
                    $"<td>{row.AccountCode}</td>" +
                    $"<td>{row.AccountName}</td>" +
                    $"<td>{row.AccountType.AccountTypeName}</td>" +
                    $"<td>{row.Tax.TaxName}</td>" +
                    $"<td>{row.Description}</td>" +
                    $"<td>{Math.Round(row.Balance.Value, digit, MidpointRounding.ToEven)}</td>" +
                    $"<td>{IsActive}</td>" +
                    $"</tr>";
                    contentBody += rowItem;
                    count++;
                }

                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{companyName}}", tenant.LegalName);

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                //var path = Path.Combine(AppFolders.TempFileDownloadFolder, tokenName);

                var result = new FileDto();
                result.FileName = $"ChartOfAccount_Report.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;
            });


        }



    }
}
