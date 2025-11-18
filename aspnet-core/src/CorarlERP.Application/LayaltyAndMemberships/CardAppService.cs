using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.FileStorages;
using CorarlERP.LayaltyAndMemberships.Dto;
using CorarlERP.Reports;
using CorarlERP.UserGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Lots.Dto;
using CorarlERP.Authorization;
using Abp.Authorization;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using OfficeOpenXml;
using CorarlERP.Customers.Dto;
using CorarlERP.Customers;

namespace CorarlERP.LayaltyAndMemberships
{
    public class CardAppService : ReportBaseClass, ICardAppService
    {
        private readonly ICardManager _cardManager;
        private readonly ICorarlRepository<Card, Guid> _cardRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly AppFolders _appFolders;
        private readonly ICorarlRepository<Customer, Guid> _customerRepository;
        public CardAppService(ICardManager cardManager,
            ICorarlRepository<Card, Guid> cardRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IUnitOfWorkManager unitOfWorkManager, AppFolders appFolders,
            ICorarlRepository<Customer, Guid> customerRepository,
            IFileStorageManager fileStorageManager) : base(null, appFolders, null, null)
        {
            _cardManager = cardManager;
            _cardRepository = cardRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _fileStorageManager = fileStorageManager;
            _appFolders = appFolders;
            _customerRepository = customerRepository;
            _appFolders = appFolders;       
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Create)]
        public async Task<Guid> Create(CreateCardInput input)
        {

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = Card.Create(tenantId, userId, input.CardId, input.CustomerId, input.Remark, input.CardNumber, input.SerialNumber);
            CheckErrors(await _cardManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _cardManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _cardManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var entity = await _cardRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _cardManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var entity = await _cardRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _cardManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Deactivate)]
        public async Task Deactivate(EntityDto<Guid> input)
        {
            var entity = await _cardRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _cardManager.DeactivateAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Find)]
        public async Task<PagedResultDto<GetListCardOutput>> Find(GetListCardInput input)
        {
            var @query = _cardRepository
                  .GetAll()
                   .Include(u => u.Customer)
                  .AsNoTracking()
                  .WhereIf(input.CardStatus != null && input.CardStatus.Count > 0, p => input.CardStatus.Contains(p.CardStatus))
                  .WhereIf(
                        !input.Filter.IsNullOrEmpty(),
                        p =>
                        p.CardId.ToLower().Contains(input.Filter.ToLower()) ||
                        p.SerialNumber.ToLower().Contains(input.Filter.ToLower()) ||
                        p.CardNumber.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .Select(s => new GetListCardOutput
                {

                    CustomerId = s.CustomerId,
                    CustomerName = s.Customer != null ? s.Customer.CustomerName : null,
                    Id = s.Id,
                    CardStatus = s.CardStatus,
                    CardStatusName = s.CardStatus.ToString(),
                    Remark = s.Remark,                 
                    CardId = s.CardId,
                    CardNumber = s.CardNumber,
                    SerialNumber = s.SerialNumber

                }).ToListAsync();
            return new PagedResultDto<GetListCardOutput>(resultCount, ObjectMapper.Map<List<GetListCardOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_View)]
        public async Task<GetListCardOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _cardRepository.GetAll().AsNoTracking().Where(s => s.Id == input.Id)
                 .Select(s => new GetListCardOutput
                 {

                     CustomerId = s.CustomerId,
                     CustomerName = s.Customer != null ? s.Customer.CustomerName : null,
                     Id = s.Id,
                     CardNumber = s.CardNumber,
                     CardStatus = s.CardStatus,
                     CardStatusName = s.CardStatus.ToString(),
                     SerialNumber = s.SerialNumber,
                     Remark = s.Remark,
                     CardId = s.CardId
                 }).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return @entity;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_GetList)]
        public async Task<PagedResultDto<GetListCardOutput>> GetList(GetListCardInput input)
        {
            var @query = _cardRepository
                 .GetAll()
                  .Include(u => u.Customer)
                 .AsNoTracking()
                 .WhereIf(input.CardStatus != null && input.CardStatus.Count > 0, p => input.CardStatus.Contains(p.CardStatus))
                 .WhereIf(input.Customers != null && input.Customers.Count > 0, p => p.CustomerId != null && input.Customers.Contains(p.CustomerId.Value))
                 .WhereIf(
                       !input.Filter.IsNullOrEmpty(),
                       p =>
                       p.CardId.ToLower().Contains(input.Filter.ToLower()) ||
                       p.SerialNumber.ToLower().Contains(input.Filter.ToLower()) ||
                       p.CardNumber.ToLower().Contains(input.Filter.ToLower())
                     );
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .Select(s => new GetListCardOutput
                {

                    CustomerId = s.CustomerId,
                    CustomerName = s.Customer != null ? s.Customer.CustomerName : null,
                    Id = s.Id,
                    CardStatus = s.CardStatus,
                    CardStatusName = s.CardStatus.ToString(),
                    Remark = s.Remark,                  
                    CardId = s.CardId,
                    CardNumber = s.CardNumber,
                    SerialNumber = s.SerialNumber

                }).ToListAsync();
            return new PagedResultDto<GetListCardOutput>(resultCount, ObjectMapper.Map<List<GetListCardOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Update)]
        public async Task<Guid> Update(UpdateCardInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _cardRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.Update(userId, input.CardId, input.CustomerId, input.Remark, input.CardNumber, input.SerialNumber);

            CheckErrors(await _cardManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Export)]
        public async Task<FileDto> ExportExcel(GetListCardExcelInput input)
        {
            var tenantId = AbpSession.TenantId.Value;
            var @query = new List<GetListCardOutput>();           
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                @query = await _cardRepository
                 .GetAll()
                 .Include(u => u.Customer)
                 .AsNoTracking()
                 .WhereIf(input.CardStatus != null && input.CardStatus.Count > 0, p => input.CardStatus.Contains(p.CardStatus))
                  .WhereIf(input.Customers != null && input.Customers.Count > 0, p => p.CustomerId != null && input.Customers.Contains(p.CustomerId.Value))
                 .WhereIf(
                       !input.Filter.IsNullOrEmpty(),
                       p =>
                       p.CardId.ToLower().Contains(input.Filter.ToLower()) ||
                       p.SerialNumber.ToLower().Contains(input.Filter.ToLower()) ||
                       p.CardNumber.ToLower().Contains(input.Filter.ToLower())
                    
                     ).Select(s => new GetListCardOutput
                     {

                         CustomerId = s.CustomerId,
                         CustomerName = s.Customer != null ? s.Customer.CustomerName : null,
                         Id = s.Id,
                         CardStatus = s.CardStatus,
                         CardStatusName = s.CardStatus.ToString(),
                         Remark = s.Remark,                        
                         CardId = s.CardId,
                         CardNumber = s.CardNumber,
                         SerialNumber = s.SerialNumber,
                         CustomerCode = s.Customer != null ? s.Customer.CustomerCode : null,

                     }).ToListAsync();



                }
            }

            var result = new FileDto();
            var sheetName = "Card";
            
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
                var headerList = GetReportTemplateItem();
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true,isRequired : i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1          
                #region write body item 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                foreach (var i in @query)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                        if (h.ColumnName == "CardNumber")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.CardNumber.ToString(), false);
                        }                       
                        else if (h.ColumnName == "SerialNumber")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.SerialNumber, false);
                        }

                        else if (h.ColumnName == "CardId")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.CardId, false);
                        }

                        
                        else if (h.ColumnName == "CustomerCode")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.CustomerCode, false);
                        }
                        else if (h.ColumnName == "Remark")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.Remark, false);
                        }
                        else if (h.ColumnName == "Status")
                        {
                            AddTextToCell(ws, rowBody, collumnCellBody, i.CardStatusName, false);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion
                result.FileName = $"CardTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
           
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;
            var customers = new List<GetCustomerWithCodeOutput>();
            var cards = new List<GetListCardOutput>();
            var createLists = new List<Card>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    customers = await _customerRepository.GetAll().AsNoTracking()
                                        .Select(s => new GetCustomerWithCodeOutput
                                        {
                                            Id = s.Id,
                                            CustomerCode =s.CustomerCode,
                                            CustomerName =s.CustomerName,
                                        }).ToListAsync();  
                    cards = await _cardRepository.GetAll().AsNoTracking().Select(s => new GetListCardOutput
                    {

                        CustomerId = s.CustomerId,
                        CustomerName = s.Customer != null ? s.Customer.CustomerName : null,
                        Id = s.Id,
                        CardStatus = s.CardStatus,
                        CardStatusName = s.CardStatus.ToString(),
                        Remark = s.Remark,                     
                        CardId = s.CardId,
                        CardNumber = s.CardNumber,
                        SerialNumber = s.SerialNumber

                    }).ToListAsync();

                }
            }

         
            var excelPackage = await Read(input);
            var headerList = GetReportTemplateItem();
            var colIndex = headerList.ColumnInfo.Count();


            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets["Card"];

                    //loop all rows
                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var cardNumber = worksheet.Cells[i, 2].Value?.ToString();
                        var serialNumber = worksheet.Cells[i, 3].Value?.ToString();
                        var cardId = worksheet.Cells[i, 1].Value?.ToString();
                      
                        var customerCode = worksheet.Cells[i, 4].Value?.ToString();
                        var remark   = worksheet.Cells[i,5].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(cardNumber)) throw new UserFriendlyException(L("CardNumberIsRequired") + $" Row {i}");
                        if (string.IsNullOrWhiteSpace(serialNumber)) throw new UserFriendlyException(L("SerialNumberIsRequired") + $" Row {i}");
                        if (string.IsNullOrWhiteSpace(cardId)) throw new UserFriendlyException(L("CardIdIsRequired") + $" Row {i}");                     
                        var customerId = string.IsNullOrWhiteSpace(customerCode) ? (Guid?)null : customers.Where(s => s.CustomerCode == customerCode).Select(s => s.Id).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(customerCode) && (customerId == null || customerId == Guid.Empty)) throw new UserFriendlyException(L("InvalidCustomerCode")  + $" Row {i}");
                        var @entity = Card.Create(tenantId, userId, cardId,customerId, remark,cardNumber,serialNumber);
                        
                        var @oldTagId = cards.Where(u => u.CardStatus == CardStatus.Enable  && u.CardId.ToLower() == entity.CardId.ToLower() && u.Id != entity.Id).FirstOrDefault();
                        var @oldCardNumber = cards.Where(u => u.CardStatus == CardStatus.Enable && u.CardNumber.ToLower() == entity.CardNumber.ToLower() && u.Id != entity.Id).FirstOrDefault();
                        var @oldSerialNumber = cards.Where(u => u.CardStatus == CardStatus.Enable && u.SerialNumber.ToLower() == entity.SerialNumber.ToLower() && u.Id != entity.Id).FirstOrDefault();
                      
                        var duplicateCardNumber = createLists.Where(u => u.CardNumber.ToLower() == entity.CardNumber.ToLower() && u.Id != entity.Id).Any();
                        var duplicateTagId = createLists.Where(u => u.CardId.ToLower() == entity.CardId.ToLower() && u.Id != entity.Id).Any();
                        var duplicateSerialNumber = createLists.Where(u => u.SerialNumber.ToLower() == entity.SerialNumber.ToLower() && u.Id != entity.Id).Any();
                      

                        if (@oldTagId != null || duplicateTagId)
                        {
                            throw new UserFriendlyException(L("DuplicateCardId", entity.CardId));
                        }
                        else if (@oldCardNumber != null || duplicateCardNumber )
                        {
                            throw new UserFriendlyException(L("DuplicateCardNumber", entity.CardNumber));
                        }
                        else if (@oldSerialNumber != null|| duplicateSerialNumber)
                        {
                            throw new UserFriendlyException(L("DuplicateSerialNumber", entity.SerialNumber));
                        }

                        

                        createLists.Add(entity);
                    }
                }
            }

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _cardRepository.BulkInsertAsync(createLists);                  
                }
                await uow.CompleteAsync();
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Cards_GetCustomerIdByCardId)]
        public async Task<GetCustomerByCardId> GetCustomerIdByCardId(GetCustomerByCardIdInput input)
        {
            var customer = await _cardRepository.GetAll().AsNoTracking().Where(c => c.CardId == input.CardId).Include(u => u.Customer)
                .Select(c => new GetCustomerByCardId
                {
                    AccountId = c.Customer != null ? c.Customer.AccountId : (Guid?)null,
                    CustomerCode = c.Customer != null ? c.Customer.CustomerCode : null,
                    CustomerName = c.Customer != null ? c.Customer.CustomerName : null,
                    CustomerId = c.Customer != null ? c.CustomerId : (Guid?)null,
                    CustomerTypeId = c.Customer != null ? c.Customer.CustomerTypeId : (long?)null,
                    IsWalkIn = c.Customer != null ? c.Customer.IsWalkIn : false,
                    CardStatus = c.CardStatus,

                }).FirstOrDefaultAsync();
            if (customer == null)
            {
                throw new UserFriendlyException(L("InvalidCard"));
            }
            if (customer != null && customer.CardStatus != CardStatus.Enable)
            {
                var status = L(customer.CardStatus.ToString());
                throw new UserFriendlyException(L("CardAlready", status));
            }
            else if(customer != null && (customer.CustomerId == null || customer.CustomerId == Guid.Empty))
            {
                throw new UserFriendlyException(L("InvalidCard"));
            }
           
            return customer;
        }

        #region helper 

        private ReportOutput GetReportTemplateItem()
        {
            var columns = new List<CollumnOutput>()
            {
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "CardId",
                    ColumnLength = 250,
                    ColumnTitle = "Card Id",
                    ColumnType = ColumnType.Bool,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false,
                    IsRequired =true,
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "CardNumber",
                    ColumnLength = 180,
                    ColumnTitle = "Card Number",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false,
                    IsRequired = true,
                },

                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "SerialNumber",
                    ColumnLength = 250,
                    ColumnTitle = "Serial Number",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false,
                    IsRequired = true,
                },

                  new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "CustomerCode",
                    ColumnLength = 250,
                    ColumnTitle = "Customer Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false,
                    IsRequired = false,
                },
                   new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Remark",
                    ColumnLength = 250,
                    ColumnTitle = "Remark",
                    ColumnType = ColumnType.String,
                    SortOrder = 4,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false,
                   
                },
                    new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Status",
                    ColumnLength = 250,
                    ColumnTitle = "Status",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false,                    
                },

            };
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.ToList(),
                Groupby = "",
                HeaderTitle = "Card",
                Sortby = "",
            };

            return result;
        }


        #endregion
    }
}
