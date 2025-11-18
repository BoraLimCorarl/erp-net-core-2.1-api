using Abp.Application.Services.Dto;
using Abp.AspNetZeroCore.Net;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization;
using CorarlERP.Authorization.Users;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerCredits;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.DeliverySchedules;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.FileStorages;
using CorarlERP.FileUploads;
using CorarlERP.Galleries;
using CorarlERP.Inventories;
using CorarlERP.Invoices;
using CorarlERP.InvoiceTemplates;
using CorarlERP.InvoiceTemplates.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locks;
using CorarlERP.MultiTenancy;
using CorarlERP.Reports;
using CorarlERP.SaleOrders.Dto;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using CorarlERP.TransactionTypes.Dto;
using CorarlERP.Url;
using CorarlERP.UserGroups;
using EvoPdf.HtmlToPdfClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders
{
    [AbpAuthorize]
    public class SaleOrderAppService : ReportBaseClass, ISaleOrderAppService
    {
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<CustomerCreditDetail, Guid> _customerCreditItemRepository;
        private readonly ISaleOrderManager _saleOrderManager;
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly ISaleOrderItemManager _saleOrderItemManager;
        private readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<InvoiceItem, Guid> _invoiceRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly AppFolders _appFolders;

        private readonly IRepository<InvoiceTemplate, Guid> _invoiceTemplateRepository;
        private readonly IRepository<InvoiceTemplateMap, Guid> _invoiceTemplateMapRepository;
        private readonly IRepository<Gallery, Guid> _galleryRepository;
        private readonly string _baseUrl;
        private readonly ITenantManager _tenantManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IFileUploadManager _fileUploadManager;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<SaleOrderExchangeRate, Guid> _exchangeRateRepository;

        private readonly ICorarlRepository<DeliverySchedule, Guid> _deliveryScheduleRepository;
        private readonly ICorarlRepository<DeliveryScheduleItem, Guid> _deliveryScheduleItemRepository;
        public SaleOrderAppService(
            ICorarlRepository<SaleOrderExchangeRate, Guid> exchangeRateRepository,
            ITenantManager tenantManager,
            IRepository<InvoiceTemplate, Guid> invoiceTemplateRepository,
            IRepository<InvoiceTemplateMap, Guid> invoiceTemplateMapRepository,
            IRepository<Gallery, Guid> galleryRepository,
            IFileUploadManager fileUploadManager,
            IFileStorageManager fileStorageManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            ISaleOrderManager saleOrderManager,
            ISaleOrderItemManager saleOrderItemManager,
            ICurrencyManager currencyManager,
            ITaxManager taxManager,
            IRepository<SaleOrder, Guid> saleOrderRepository,
            IRepository<SaleOrderItem, Guid> saleOrderItemRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Customer, Guid> customerRepository,
            IRepository<Currency, long> currencyRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<CustomerCreditDetail, Guid> customerCreditItemRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<InvoiceItem, Guid> invoiceRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<Tax, long> taxRepository,
            IInventoryManager inventoryManager,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Locations.Location, long> locationRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<InvoiceItem, Guid> invoiceItemRepository,
            IRepository<Lock, long> lockRepository,
            AppFolders appFolders,
            IRepository<AccountCycles.AccountCycle, long> accountCyclesRepository,
            ICorarlRepository<DeliverySchedule, Guid> deliveryScheduleRepository,
            ICorarlRepository<DeliveryScheduleItem, Guid> deliveryScheduleItemRepository
        ) : base(accountCyclesRepository, appFolders, userGroupMemberRepository, locationRepository) //: base(userGroupMemberRepository, locationRepository)
        {
            _invoiceItemRepository = invoiceItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _saleOrderManager = saleOrderManager;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderItemManager = saleOrderItemManager;
            _saleOrderItemRepository = saleOrderItemRepository;
            _itemRepository = itemRepository;
            _customerRepository = customerRepository;
            _currencyRepository = currencyRepository;
            _taxRepository = taxRepository;
            _journalRepository = journalRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _customerCreditItemRepository = customerCreditItemRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _invoiceRepository = invoiceRepository;
            _inventoryManager = inventoryManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _appFolders = appFolders;
            _lockRepository = lockRepository;

            _invoiceTemplateRepository = invoiceTemplateRepository;
            _invoiceTemplateMapRepository = invoiceTemplateMapRepository;
            _tenantManager = tenantManager;
            _baseUrl = (IocManager.Instance.Resolve<IWebUrlService>()).GetServerRootAddress().EnsureEndsWith('/');
            _userRepository = userRepository;
            _galleryRepository = galleryRepository;
            _fileUploadManager = fileUploadManager;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _exchangeRateRepository = exchangeRateRepository;
            _deliveryScheduleItemRepository = deliveryScheduleItemRepository;
            _deliveryScheduleRepository = deliveryScheduleRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateSaleOrderInput input)
        {
            var result = await SaveSaleOrder(input);
            return new NullableIdDto<Guid>() { Id = result.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Create)]
        public async Task<FileDto> CreateAndPrint(CreateSaleOrderInput input)
        {
            var saveSaleOrder = await SaveSaleOrder(input);
            await CurrentUnitOfWork.SaveChangesAsync();
            var result = new EntityDto<Guid>() { Id = saveSaleOrder.Id.Value };
            var print = await Print(result);
            return print;
        }

        private string AddBlankRows(int rows, int range = 9)
        {
            var item = "";
            if (rows <= range)
            {
                var add = range - rows;
                for (var i = 1; i <= add; i++)
                {
                    item += "<tr>";
                    item += "<td align='center'>" + (rows + i) + "</td>" +
                            "<td></td><td></td><td></td><td></td><td></td>";
                    item += "</tr>";
                }
            }
            else
            {
                return AddBlankRows(rows, range + 14);
            }
            return item;
        }

        public async Task<FileDto> Print(EntityDto<Guid> input)
        {
            return await PrintTemplate(input);

            //var detail = await GetDetail(input);
            //var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(detail.OrderDate);
            //var t = accountCyclePeriod;
            //return await Task.Run(() =>
            //{

            //    var exportHtml = string.Empty;
            //    var templateHtml = string.Empty;
            //    FileDto fileDto = new FileDto()
            //    {
            //        SubFolder = null,
            //        FileName = "saleOrderPrint.pdf",
            //        FileToken = "saleOrderPrint.html",
            //        FileType = MimeTypeNames.TextHtml
            //    };
            //    try
            //    {
            //        templateHtml = ReadTemplateFile(fileDto);//retrive template from path
            //        templateHtml = templateHtml.Trim();
            //    }
            //    catch (FileNotFoundException)
            //    {
            //        throw new UserFriendlyException("FileNotFound");
            //    }
            //    exportHtml = templateHtml;
            //    var dateFormat = FormatDate(detail.OrderDate, "dd/MM/yyyy");//@todo take date format from tenant setup               
            //    var No = 1;
            //    var contentBody = string.Empty;

            //        exportHtml = exportHtml.Replace("{{itemHeaderKey}}", "លេខកូដទំនិញ");
            //        exportHtml = exportHtml.Replace("{{itemCodeKey}}", "Item Code");

            //        exportHtml = exportHtml.Replace("{{itemHeaderKeyName}}", "ឈ្មោះទំនិញ");
            //        exportHtml = exportHtml.Replace("{{itemCodeKeyName}}", "Item Name");

            //    foreach (var i in detail.SaleOrderItems)
            //    {                   
            //        var item = "<tr>";                    
            //        item += $"<td align='center'>" + No + "</td>";
            //        if (i.Item != null)
            //        {
            //            item += $"<td align='center'>" + i.Item.ItemCode + "</td>";
            //            item += $"<td>" + i.Item.ItemName + "</td>";

            //        }

            //        item += $"<td align='center'>" + FormatNumberCurrency(i.Qty, accountCyclePeriod.RoundingDigit) + "</td>";
            //        item += $"<td align='right'>" + FormatNumberCurrency(i.MultiCurrencyUnitCost, accountCyclePeriod.RoundingDigit) + "</td>";
            //        item += $"<td align='right'>" + FormatNumberCurrency(i.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit) + "</td>";
            //        item += "</tr>";
            //        contentBody += item;
            //        No++;
            //    }
            //    var length = detail.SaleOrderItems.Count.ToString();
            //    int lastNumber = Int16.Parse(length.Substring(length.Length - 1, 1));
            //    contentBody += AddBlankRows(detail.SaleOrderItems.Count);

            //    exportHtml = exportHtml.Replace("{{saleOrderDate}}", dateFormat);
            //    exportHtml = exportHtml.Replace("{{saleOrderNo}}", detail.OrderNumber);
            //    // exportHtml = exportHtml.Replace("{{saleOrderNo}}", saleOrderNo);
            //    // exportHtml = exportHtml.Replace("{{issueNo}}", issueNo);
            //    var address = detail.Customer.shippingAddress.PostalCode + " " + detail.Customer.shippingAddress.Street + " " +
            //                detail.Customer.shippingAddress.CityTown + " " + detail.Customer.shippingAddress.Province + " " +
            //                detail.Customer.shippingAddress.Country;
            //    exportHtml = exportHtml.Replace("{{customerAddress}}", address);
            //    exportHtml = exportHtml.Replace("{{customerName}}", detail.Customer.CustomerName);
            //    exportHtml = exportHtml.Replace("{{customerTelephone}}", detail.Customer.PhoneNumber);
            //    exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
            //    exportHtml = exportHtml.Replace("{{subTotal}}", FormatNumberCurrency(detail.MultiCurrencySubTotal, accountCyclePeriod.RoundingDigit));
            //    exportHtml = exportHtml.Replace("{{deposit}}", FormatNumberCurrency(0, accountCyclePeriod.RoundingDigit));
            //    exportHtml = exportHtml.Replace("{{balance}}", FormatNumberCurrency(detail.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit));
            //    HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
            //    // Set if the fonts are embedded in the generated PDF document
            //    // Leave it not set to embed the fonts in the generated PDF document
            //    htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;
            //    htmlToPdfConverter.PdfDocumentOptions.FitWidth = true;
            //    htmlToPdfConverter.PdfDocumentOptions.TableHeaderRepeatEnabled = true;
            //    htmlToPdfConverter.PdfDocumentOptions.TableFooterRepeatEnabled = false;
            //    htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
            //    htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A5;
            //    htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 10;
            //    htmlToPdfConverter.PdfDocumentOptions.RightMargin = 10;

            //    htmlToPdfConverter.PdfDocumentOptions.TopMargin = 10;
            //    htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 10;


            //    byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");


            //    var tokenName = $"{Guid.NewGuid()}.pdf";
            //    var result = new FileDto();
            //    result.FileName = $"saleOrder_{dateFormat}.pdf";
            //    result.FileToken = Guid.NewGuid().ToString();
            //    result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
            //    result.FileType = MimeTypeNames.ApplicationPdf;
            //    var path = Path.Combine(_appFolders.TempFileDownloadFolder, result.FileToken);
            //    File.WriteAllBytes(path, outPdfBuffer);
            //    return result;

            //});
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<FileDto> PrintTemplate(EntityDto<Guid> input)
        {

            var companyInfo = await _tenantManager.GetAsync(AbpSession.GetTenantId());

            var invoice = await GetDetail(input);
            var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(invoice.OrderDate);
            var template = await this.GetInovieTemplateHtml(invoice.SaleTransactionTypeId);
            var exportHtml = EmbedBase64Fonts(template.Html);

            return await Task.Run(async () =>
            {
                var dateFormat = FormatDate(invoice.OrderDate, "dd-MM-yyyy");
                var canSeePrice = IsGranted(AppPermissions.Pages_Tenant_Customer_SaleOrder_CanSeePrice);

                if (template.ShowDetail)
                {
                    var rowStart = FindTag(exportHtml, "<tr", " detail-body-row");
                    if (!string.IsNullOrWhiteSpace(rowStart.Key))
                    {
                        var detialRow = GetOuterHtml(rowStart.Key, "</tr>", exportHtml.Substring(rowStart.Value));
                        if (!string.IsNullOrEmpty(detialRow))
                        {
                            int n = 1;
                            var detail = string.Empty;
                            foreach (var i in invoice.SaleOrderItems)
                            {
                                detail += detialRow.Replace("@No", $"{n}")
                                                   .Replace("@ItemCode", i.Item.ItemCode)
                                                   .Replace("@ItemName", i.Item.ItemName)
                                                   .Replace("@Description", i.Description)
                                                   .Replace("@Qty", i.Qty.ToString("#,##0.########"))
                                                   .Replace("@UnitPrice", !canSeePrice ? "..." : FormatNumberCurrency(i.MultiCurrencyUnitCost, accountCyclePeriod.RoundingDigitUnitCost))
                                                   .Replace("@DiscountRate", !canSeePrice ? "..." : $"{FormatNumberCurrency(i.DiscountRate * 100, accountCyclePeriod.RoundingDigit)}%")
                                                   .Replace("@LineTotal", !canSeePrice ? "..." : FormatNumberCurrency(i.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit));
                                n++;
                            }

                            exportHtml = exportHtml.Replace(detialRow, detail);
                        }
                    }
                }


                var logo = "";
                if (companyInfo.LogoId.HasValue)
                {
                    var image = await _fileUploadManager.DownLoad(companyInfo.Id, companyInfo.LogoId.Value);

                    if (image != null)
                    {
                        var base64Str = StreamToBase64(image.Stream);
                        logo = $"<img src=\"data:{image.ContentType};base64, {base64Str}\" alt=\"logo\" style=\"max-height: 90px; max-width: 150px; display: block;\"/>";
                    }
                }

                var shippingAddress = "";
                if (invoice.ShippingAddress != null)
                {
                    shippingAddress = $"{(invoice.ShippingAddress.Street.IsNullOrWhiteSpace()?"....." : invoice.ShippingAddress.Street)}, " +
                    $"{(invoice.ShippingAddress.PostalCode.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.PostalCode) }, " +
                    $"{(invoice.ShippingAddress.CityTown.IsNullOrWhiteSpace()? ".....": invoice.ShippingAddress.CityTown)}, " +
                    $"{(invoice.ShippingAddress.Province.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Province)}, " +
                    $"{(invoice.ShippingAddress.Country.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Country)}";
                }

                exportHtml = exportHtml.Replace("@Logo", logo)
                                       .Replace("@CompanyName", companyInfo.Name)
                                       .Replace("@CompanyAddress", $"{companyInfo.LegalAddress?.Street} {companyInfo.LegalAddress?.CityTown} {companyInfo.LegalAddress?.Province}")
                                       .Replace("@CustomerName", invoice.Customer?.CustomerName)
                                       .Replace("@CustomerPhone", invoice.Customer?.PhoneNumber)
                                       .Replace("@ShippingAddress", shippingAddress)
                                       .Replace("@InvoiceDate", dateFormat)
                                       .Replace("@ETADate", FormatDate(invoice.ETD, "dd-MM-yyyy"))
                                       .Replace("@InvoiceNo", invoice.OrderNumber)
                                       .Replace("@Reference", invoice.Reference)
                                       .Replace("@Currency", invoice.MultiCurrency?.Code)
                                       .Replace("@Memo", invoice.Memo == null ? "" : invoice.Memo.Replace(Environment.NewLine, "<br>").Replace("\n", "<br>"))
                                       .Replace("@UserName", invoice.UserName)
                                       .Replace("@SubTotal", !canSeePrice ? "..." : FormatNumberCurrency(invoice.MultiCurrencySubTotal, accountCyclePeriod.RoundingDigit));


                HtmlToPdfConverter htmlToPdfConverter = GetInitPDF();
                // Set if the fonts are embedded in the generated PDF document
                // Leave it not set to embed the fonts in the generated PDF document    
                htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
                htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 20;
                htmlToPdfConverter.PdfDocumentOptions.RightMargin = 20;
                htmlToPdfConverter.PdfDocumentOptions.TopMargin = 20;
                htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 20;


                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");


                var result = new FileDto();
                result.FileName = $"Invoice_{dateFormat}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<InvoiceTemplateWithOptionResultOutput> GetInovieTemplateHtml(long? saleTypeId)
        {
            InvoiceTemplateMap templateMap = null;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    templateMap = await _invoiceTemplateMapRepository
                                    .GetAll()
                                    .Include(s => s.InvoiceTemplate)
                                    .AsNoTracking()
                                    .Where(s => s.TemplateType == InvoiceTemplateType.SaleOrder)
                                    .Where(s => s.SaleTypeId == saleTypeId || !s.SaleTypeId.HasValue)
                                    .OrderBy(s => s.SaleTypeId.HasValue ? 0 : 1)
                                    .FirstOrDefaultAsync();
                }
            }


            if (templateMap == null) return await GetDefaultTemplateHtml("saleOrderTemplate.html");

            GalleryDownloadOutput fileDownload = await _fileUploadManager.DownLoad(AbpSession.TenantId, templateMap.InvoiceTemplate.GalleryId);

            var templateHtml = string.Empty;
            using (StreamReader r = new StreamReader(fileDownload.Stream))
            {
                templateHtml = r.ReadToEnd();
            }

            return new InvoiceTemplateWithOptionResultOutput
            {
                Html = templateHtml,
                ShowDetail = templateMap.InvoiceTemplate.ShowDetail,
                ShowSummary = templateMap.InvoiceTemplate.ShowSummary
            };

        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<InvoiceTemplateWithOptionResultOutput> GetDefaultTemplateHtml(string templateFileName)
        {
            var templateHtml = await _fileStorageManager.GetTemplate(templateFileName);

            return new InvoiceTemplateWithOptionResultOutput { Html = templateHtml, ShowDetail = true };

        }

        void ValidateExchangeRate(CreateSaleOrderInput input)
        {
            if (!input.UseExchangeRate || input.CurrencyId == input.MultiCurrencyId) return;

            if (input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));
        }

        private void ValidateDeliverySchedule(CreateSaleOrderInput input)
        {
            if (input.DeliverySchedules.IsNullOrEmpty()) return;

            if (input.DeliverySchedules.Any(s => s.Items.IsNullOrEmpty())) throw new UserFriendlyException(L("IsRequired", L("ScheduleItems")));

            var find = input.SaleOrderItems.Any(s => s.Qty < input.DeliverySchedules.SelectMany(r => r.Items).Where(r => r.SaleOrderItemId == s.Id).Sum(t => t.Qty));
            if (find) throw new UserFriendlyException(L("CannotBeMoreThan", L("DeliveryScheduleQty"), L("SaleOrderQty")));

            var link = input.DeliverySchedules.SelectMany(s => s.Items).Any(f => !input.SaleOrderItems.Any(r => r.Id == f.SaleOrderItemId));
            if (link) throw new UserFriendlyException(L("IsNotValid", L("ScheduleItems")));
        }

        private async Task<NullableIdDto<Guid>> SaveSaleOrder(CreateSaleOrderInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => t.LockKey == TransactionLockType.SaleOrder && t.IsLock == true && t.LockDate.Value.Date >= input.OrderDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateDeliverySchedule(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.SaleOrder);

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            ValidateExchangeRate(input);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.OrderNumber = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == input.CurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTotal = input.Total;
            }

            var @entity = SaleOrder.Create(
                tenantId, userId,
                input.CustomerId,
                input.ShippingAddress,
                input.BillingAddress,
                input.SameAsShippingAddress,
                input.Reference,
                input.CurrencyId,
                input.OrderNumber,
                input.OrderDate,
                input.Memo,
                input.Tax,
                input.Total,
                input.SubTotal,
                input.Status,
                input.ETD,
                input.LocationId,
                input.MultiCurrencyId,
                input.MultiCurrencySubTotal,
                input.MultiCurrencyTotal,
                input.MultiCurrencyTax, input.UseExchangeRate);
            entity.UpdateSaleType(input.SaleTypeId);
            //add status and update receivestatus to pending
            entity.UpdateReceiveStatusToPending();
            entity.SetApprovalStatus(ApprovalStatus.Recorded);

            CheckErrors(await _saleOrderManager.CreateAsync(@entity, auto.RequireReference));

            #region saleOrderItem   

            foreach (var item in input.SaleOrderItems)
            {
                if (input.MultiCurrencyId == null || input.MultiCurrencyId == input.CurrencyId)
                {
                    item.MultiCurrencyTotal = item.Total;
                    item.MultiCurrencyUnitCost = item.UnitCost;
                }
                var @saleOrderItem = SaleOrderItem.Create(
                    tenantId,
                    userId,
                    item.ItemId,
                    entity.Id,
                    item.TaxId,
                    item.TaxRate,
                    item.Description,
                    item.Qty,
                    item.UnitCost,
                    item.DiscountRate,
                    item.Total,
                    item.MultiCurrencyUnitCost,
                    item.MultiCurrencyTotal);
                base.CheckErrors(await _saleOrderItemManager.CreateAsync(@saleOrderItem));

                //update linke
                if (!input.DeliverySchedules.IsNullOrEmpty())
                {
                    var scheduleItems = input.DeliverySchedules.SelectMany(s => s.Items).Where(s => s.SaleOrderItemId == item.Id);
                    foreach(var  scheduleItem in scheduleItems)
                    {
                        scheduleItem.SaleOrderItemId = saleOrderItem.Id;
                    }
                }

            }
            #endregion

            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = SaleOrderExchangeRate.Create(tenantId, userId, entity.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }

            if (!input.DeliverySchedules.IsNullOrEmpty())
            {
                var addSchedules = new List<DeliverySchedule>();
                var addScheduleItems = new List<DeliveryScheduleItem>();
                DeliverySchedule latestDelivery = null;

                foreach (var schedule in input.DeliverySchedules)
                {
                    if (latestDelivery != null)
                    {
                        var index = latestDelivery.DeliveryNo.Replace($"{entity.OrderNumber}/", "");
                        var nexIndex = Convert.ToInt32(index) + 1;
                        schedule.DeliveryNo = $"{entity.OrderNumber}/{nexIndex.ToString().PadLeft(2, '0')}";
                    }
                    else
                    {
                        schedule.DeliveryNo = $"{entity.OrderNumber}/01";
                    }

                    var addSchedule = DeliverySchedule.Create(tenantId, userId, input.CustomerId, schedule.Reference, schedule.DeliveryNo, schedule.InitialDeliveryDate, schedule.FinalDeliveryDate, schedule.Memo, input.LocationId.Value, entity.Id);
                    addSchedule.UpdateReceiveStatusToPending();
                    addSchedules.Add(addSchedule);

                    var addItems = schedule.Items.Select(s => DeliveryScheduleItem.Create(tenantId, userId, addSchedule.Id, s.Description, s.Qty, s.SaleOrderItemId, s.ItemId));
                    addScheduleItems.AddRange(addItems);

                    latestDelivery = addSchedule;
                }

                await CurrentUnitOfWork.SaveChangesAsync();
                await _deliveryScheduleRepository.BulkInsertAsync(addSchedules);
                await _deliveryScheduleItemRepository.BulkInsertAsync(addScheduleItems);
            }


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.SaleOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Delete)]
        public async Task Delete(CarlEntityDto input)
        {

            await ValidateIssueLink(input.Id);
            await ValidateInvoiceLink(input.Id);
            await ValidateDeliverySchedule(input.Id);

            var @entity = await _saleOrderManager.GetAsync(input.Id, true);

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
             .Where(t => t.LockKey == TransactionLockType.SaleOrder && t.IsLock == true && t.LockDate.Value.Date >= entity.OrderDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.SaleOrder);

            if (entity.OrderNumber == auto.LastAutoSequenceNumber)
            {
                var so = await _saleOrderRepository.GetAll().Where(t => t.Id != entity.Id)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (so != null)
                {
                    auto.UpdateLastAutoSequenceNumber(so.OrderNumber);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var exchangeRates = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.SaleOrderId == input.Id).ToListAsync();
            if (exchangeRates.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchangeRates);

            var saleOrderItem = await _saleOrderItemRepository.GetAll().Where(u => u.SaleOrderId == entity.Id).ToListAsync();

            foreach (var s in saleOrderItem)
            {
                CheckErrors(await _saleOrderItemManager.RemoveAsync(s));
            }
            CheckErrors(await _saleOrderManager.RemoveAsync(@entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.SaleOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _saleOrderManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            CheckErrors(await _saleOrderManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Find)]
        public async Task<PagedResultDto<SaleOrderGetListOutput>> Find(GetSaleOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from p in _saleOrderItemRepository.GetAll()
                         join i in _itemRepository.GetAll()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.Id))
                         on p.ItemId equals i.Id

                         join o in _saleOrderRepository.GetAll()
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                         p => p.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Reference.ToLower().Contains(input.Filter.ToLower())) on p.SaleOrderId equals o.Id
                         join c in _customerRepository.GetAll().WhereIf(input.Customers != null && input.Customers.Count > 0, p => input.Customers.Contains(p.Id))
                         on o.CustomerId equals c.Id
                         group c by new
                         {
                             Id = o.Id,
                             IsActive = o.IsActive,
                             OrderDate = o.OrderDate,
                             OrderNumber = o.OrderNumber,
                             Reference = o.Reference,
                             Tax = o.Tax,
                             Total = o.Total,
                             CustomerId = o.CustomerId,
                             CustomerName = c.CustomerName,
                             CustomerCode = c.CustomerCode
                         } into u

                         select new SaleOrderGetListOutput
                         {
                             Id = u.Key.Id,
                             IsActive = u.Key.IsActive,
                             OrderDate = u.Key.OrderDate,
                             OrderNumber = u.Key.OrderNumber,
                             Reference = u.Key.Reference,
                             Tax = u.Key.Tax,
                             Total = u.Key.Total,
                             CustomerId = u.Key.Id,
                             CountItem = u.Count(),
                             Customer = new CustomerSummaryOutput
                             {
                                 CustomerName = u.Key.CustomerName,
                                 Id = u.Key.CustomerId,
                                 CustomerCode = u.Key.CustomerCode
                             }
                         });


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<SaleOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetDetail)]
        public async Task<SaleOrderDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItemQty = from i in _invoiceItemRepository.GetAll()
                                           .Where(s => s.OrderItemId.HasValue)
                                           .AsNoTracking()
                                 join j in _journalRepository.GetAll()
                                           .Where(s => s.InvoiceId.HasValue)
                                           .Where(s => s.Status == TransactionStatus.Publish)
                                           .AsNoTracking()
                                 on i.InvoiceId equals j.InvoiceId
                                 select new
                                 {
                                     i.Qty,
                                     i.OrderItemId
                                 };

            var issueItemQty = from i in _itemIssueItemRepository.GetAll()
                                         .Where(s => s.SaleOrderItemId.HasValue)
                                         .AsNoTracking()
                               join j in _journalRepository.GetAll()
                                         .Where(s => s.ItemIssueId.HasValue)
                                         .Where(s => s.Status == TransactionStatus.Publish)
                                         .AsNoTracking()
                               on i.ItemIssueId equals j.ItemIssueId
                               select new
                               {
                                   i.Qty,
                                   i.SaleOrderItemId
                               };

            // delivery schedule item
            var delivaryItemQuery = from di in _deliveryScheduleItemRepository.GetAll()
                                     .Where(s => s.SaleOrderItemId.HasValue)
                                     .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                     .AsNoTracking()
                                    select new
                                    {
                                        SaleOrderItemId = di.SaleOrderItemId,
                                        Qty = di.Qty,                                     
                                    };

            var query = from oi in _saleOrderItemRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include(u => u.Tax)
                                    .Where(u => u.SaleOrderId == entity.Id)
                                    .AsNoTracking()
                        join ii in invoiceItemQty
                        on oi.Id equals ii.OrderItemId
                        into iiItems

                        join si in issueItemQty
                        on oi.Id equals si.SaleOrderItemId
                        into siItems

                        join di in delivaryItemQuery on oi.Id equals di.SaleOrderItemId
                        into diitems
                        let remainQty = oi.Qty - iiItems.Sum(s => s.Qty) - siItems.Sum(s => s.Qty) - diitems.Sum(s => s.Qty)

                        select new CreateOrUpdateSaleOrderItemInput
                        {
                            MultiCurrencyTotal = oi.MultiCurrencyTotal,
                            MultiCurrencyUnitCost = oi.MultiCurrencyUnitCost,
                            Qty = oi.Qty,
                            Description = oi.Description,
                            DiscountRate = oi.DiscountRate,
                            Id = oi.Id,
                            Item = ObjectMapper.Map<ItemSummaryOutput>(oi.Item),
                            ItemId = oi.ItemId,
                            Tax = ObjectMapper.Map<TaxDetailOutput>(oi.Tax),
                            TaxId = oi.TaxId,
                            TaxRate = oi.TaxRate,
                            Total = oi.Total,
                            UnitCost = oi.UnitCost,
                            Remain = remainQty
                        };


            var @saleOrderItem = await query.ToListAsync();

            var result = ObjectMapper.Map<SaleOrderDetailOutput>(@entity);
            result.UserName = entity.CreatorUser?.FullName;
            result.Location = entity.Location?.LocationName;
            result.LocationId = entity.LocationId;
            result.SaleOrderItems = @saleOrderItem;

            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.SaleOrderId == input.Id)
                                            .Select(s => new GetExchangeRateDto
                                            {
                                                Id = s.Id,
                                                FromCurrencyCode = s.FromCurrency.Code,
                                                ToCurrencyCode = s.ToCurrency.Code,
                                                FromCurrencyId = s.FromCurrencyId,
                                                ToCurrencyId = s.ToCurrencyId,
                                                Bid = s.Bid,
                                                Ask = s.Ask,
                                                IsInves = s.FromCurrencyId == entity.CurrencyId
                                            })
                                            .FirstOrDefaultAsync();
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList)]
        public async Task<PagedResultDto<SaleOrderGetListOutput>> GetList(GetSaleOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, input.Locations);

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();
         
            var customerQuery = GetCustomers(null, input.Customers, input.CustomerTypes, customerTypeMemberIds);

            var oQuery = _saleOrderRepository.GetAll()
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                            (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(input.DeliveryStatuses != null && input.DeliveryStatuses.Count > 0, u => input.DeliveryStatuses.Contains(u.ReceiveStatus))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                p => p.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                p.Reference.ToLower().Contains(input.Filter.ToLower())
                         )
                         .AsNoTracking()
                         .Select(o => new
                         {
                             Id = o.Id,
                             IsActive = o.IsActive,
                             OrderDate = o.OrderDate,
                             OrderNumber = o.OrderNumber,
                             Reference = o.Reference,
                             Tax = o.Tax,
                             Total = o.Total,
                             Status = o.Status,
                             ReceiveStatus = o.ReceiveStatus,
                             IssueCount = o.IssueCount,
                             CreatorId = o.CreatorUser.Id,
                             CustomerId = o.CustomerId,
                             LocationId = o.LocationId
                         });

            var oiQuery = _saleOrderItemRepository.GetAll()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         .AsNoTracking()
                         .Select(s => s.SaleOrderId);


            var query = from o in oQuery
                        join c in customerQuery
                        on o.CustomerId equals c.Id
                        join l in locationQuery
                        on o.LocationId equals l.Id
                        join u in userQuery
                        on o.CreatorId equals u.Id
                        join oi in oiQuery
                        on o.Id equals oi
                        into ois
                        where ois.Count() > 0

                        select new SaleOrderGetListOutput
                        {
                            Id = o.Id,
                            IsActive = o.IsActive,
                            OrderDate = o.OrderDate,
                            OrderNumber = o.OrderNumber,
                            Reference = o.Reference,
                            Tax = o.Tax,
                            Total = o.Total,
                            CustomerId = o.CustomerId,
                            CountItem = ois.Count(),
                            StatusCode = o.Status,
                            StatusName = o.Status.ToString(),
                            ReceiveStatus = o.ReceiveStatus,
                            TotalIssueCount = o.IssueCount,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = o.CustomerId,
                                CustomerName = c.CustomerName
                            },
                            User = new UserDto
                            {
                                Id = o.CreatorId,
                                UserName = u.UserName
                            },
                            LocationName = l.LocationName
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<SaleOrderGetListOutput>(resultCount, new List<SaleOrderGetListOutput>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("orderdate"))
                {
                    query = query.OrderByDescending(s => s.OrderDate);
                }
                else if (input.Sorting.ToLower().StartsWith("ordernumber"))
                {
                    query = query.OrderByDescending(s => s.OrderNumber);
                }
                else if (input.Sorting.ToLower().StartsWith("reference"))
                {
                    query = query.OrderByDescending(s => s.Reference);
                }
                else if (input.Sorting.ToLower().StartsWith("countitem"))
                {
                    query = query.OrderByDescending(s => s.CountItem);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderByDescending(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderByDescending(s => s.StatusName);
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    query = query.OrderByDescending(s => s.Total);
                }
                else if (input.Sorting.ToLower().StartsWith("customer"))
                {
                    query = query.OrderByDescending(s => s.Customer.CustomerName);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("orderdate"))
                {
                    query = query.OrderBy(s => s.OrderDate);
                }
                else if (input.Sorting.ToLower().StartsWith("ordernumber"))
                {
                    query = query.OrderBy(s => s.OrderNumber);
                }
                else if (input.Sorting.ToLower().StartsWith("reference"))
                {
                    query = query.OrderBy(s => s.Reference);
                }
                else if (input.Sorting.ToLower().StartsWith("countitem"))
                {
                    query = query.OrderBy(s => s.CountItem);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderBy(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderBy(s => s.StatusName);
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    query = query.OrderBy(s => s.Total);
                }
                else if (input.Sorting.ToLower().StartsWith("customer"))
                {
                    query = query.OrderBy(s => s.Customer.CustomerName);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }


            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<SaleOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList)]
        public async Task<PagedResultDto<SaleOrderGetListOutput>> GetListOld(GetSaleOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from p in _saleOrderItemRepository.GetAll().AsNoTracking()
                         join i in _itemRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.Id))
                         on p.ItemId equals i.Id

                         join o in _saleOrderRepository.GetAll().Include(t => t.CreatorUser).Include(t => t.Location)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                            (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                            p => p.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                p.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                p.Memo.ToLower().Contains(input.Filter.ToLower())
                         ).AsNoTracking()

                         on p.SaleOrderId equals o.Id
                         join c in _customerRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, p => input.Customers.Contains(p.Id))
                         on o.CustomerId equals c.Id
                         group p by new
                         {
                             Id = o.Id,
                             IsActive = o.IsActive,
                             OrderDate = o.OrderDate,
                             OrderNumber = o.OrderNumber,
                             LocationName = o.Location.LocationName,
                             Reference = o.Reference,
                             Tax = o.Tax,
                             Total = o.Total,
                             CustomerId = c.Id,
                             Status = o.Status,
                             ReceiveStatus = o.ReceiveStatus,
                             CustomerCode = c.CustomerCode,
                             CustomerName = c.CustomerName,
                             CreatorUserName = o.CreatorUser.UserName,
                             CreatorId = o.CreatorUser.Id,
                             TotalIssueCount = o.IssueCount,
                             //saleOrder = o, customer = c
                         } into u
                         select new SaleOrderGetListOutput
                         {
                             Id = u.Key.Id,
                             IsActive = u.Key.IsActive,
                             OrderDate = u.Key.OrderDate,
                             OrderNumber = u.Key.OrderNumber,
                             Reference = u.Key.Reference,
                             Tax = u.Key.Tax,
                             Total = u.Key.Total,
                             CustomerId = u.Key.CustomerId,
                             CountItem = u.Count(),
                             StatusCode = u.Key.Status,
                             StatusName = u.Key.Status.ToString(),
                             ReceiveStatus = u.Key.ReceiveStatus,
                             TotalIssueCount = u.Key.TotalIssueCount,
                             Customer = new CustomerSummaryOutput
                             {
                                 Id = u.Key.CustomerId,
                                 CustomerName = u.Key.CustomerName,
                                 CustomerCode = u.Key.CustomerCode
                             },
                             User = new UserDto
                             {
                                 Id = u.Key.CreatorId,
                                 UserName = u.Key.CreatorUserName
                             },
                             LocationName = u.Key.LocationName
                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<SaleOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateSaleOrderInput input)
        {

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();


            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                    .Where(t => t.IsLock == true &&
                                    (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.OrderDate.Date)
                                    && (t.LockKey == TransactionLockType.SaleOrder)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateExchangeRate(input);

            var @entity = await _saleOrderManager.GetAsync(input.Id, true); //this is vendor

            await CheckClosePeriod(entity.OrderDate, input.OrderDate);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            #region update saleOrderItem           
            var saleOrderItems = await _saleOrderItemRepository.GetAll()
                                       .Include(s => s.ItemIssueItems)
                                       .Include(s => s.InvoiceItems)
                                       .Where(u => u.SaleOrderId == entity.Id)
                                       .ToListAsync();

            var addOrderItems = input.SaleOrderItems.Where(s => !s.Id.HasValue);
            var updateOrderItems = input.SaleOrderItems.Where(s => s.Id.HasValue);
            var deleteOrderItems = saleOrderItems.Where(u => !updateOrderItems.Any(i => i.Id == u.Id)).ToList();


            //validate delete items           
            if (deleteOrderItems.Any(s => s.ItemIssueItems.Any())) throw new UserFriendlyException(L("OrderAlreadyIssuedItems"));
            if (deleteOrderItems.Any(s => s.InvoiceItems.Any())) throw new UserFriendlyException(L("OrderAlreadyConvertToInvoice"));
            if(deleteOrderItems.Any()) await this.ValidateDeliveryScheduleBySaleItem(deleteOrderItems.Select(s=>s.Id).ToList());


            //validate modify items
            var updateItemsHaveLink = saleOrderItems
                                      .Where(s => updateOrderItems.Any(d => d.Id == s.Id));
                                      // new code
                                      //.Where(s => s.ItemIssueItems.Any() || s.InvoiceItems.Any());

            if (updateItemsHaveLink.Any())
            {

                var returnList = new Dictionary<Guid?, decimal>();

                var itemIssueIds = updateItemsHaveLink.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                if (itemIssueIds.Any())
                {
                    var list = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                        .Where(s => itemIssueIds.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIds.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                        .AsNoTracking()
                                        .Select(s => new
                                        {
                                            IssueId = s.ItemIssueSaleItemId.HasValue ? s.ItemIssueSaleItemId : s.CustomerCreditItem.ItemIssueSaleItemId,
                                            s.Qty
                                        })
                                        .ToListAsync();

                    returnList = list.GroupBy(g => g.IssueId).ToDictionary(k => k.Key, v => v.Sum(s => s.Qty));
                }
                var linkDeliverys = await _deliveryScheduleItemRepository.GetAll().AsNoTracking().Where(s => s.DeliverySchedule.SaleOrderId == entity.Id).ToListAsync();
                foreach (var link in updateItemsHaveLink)
                {
                    var linkIssueItemIds = link.ItemIssueItems.Select(r => r.Id).Concat(link.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value)).ToList();
                    var linkReturnQty = returnList.Where(s => linkIssueItemIds.Contains(s.Key.Value)).Sum(v => v.Value);
                    var linkDeliveryQty = linkDeliverys.Where(s => s.SaleOrderItemId == link.Id).Sum(s => s.Qty);
                    
                    var applyQty = link.ItemIssueItems.Sum(s => s.Qty) + linkDeliveryQty + link.InvoiceItems.Sum(s => s.Qty) - linkReturnQty;

                    var updateItem = updateOrderItems.First(s => s.Id == link.Id);
                    if (updateItem.Qty < applyQty) throw new UserFriendlyException(L("OrderItemCannotChangeQtyLessThanIssueQty", applyQty));
                }
            }


            foreach (var p in addOrderItems)
            {
                if (input.CurrencyId == input.MultiCurrencyId || input.MultiCurrencyId == null)
                {
                    p.MultiCurrencyTotal = p.Total;
                    p.MultiCurrencyUnitCost = p.UnitCost;
                }

                var saleOrderItem = SaleOrderItem.Create(tenantId, userId, p.ItemId, entity.Id, p.TaxId, p.TaxRate, p.Description, p.Qty, p.UnitCost, p.DiscountRate, p.Total, p.MultiCurrencyUnitCost, p.MultiCurrencyTotal);
                base.CheckErrors(await _saleOrderItemManager.CreateAsync(saleOrderItem));
            }

            foreach (var p in updateOrderItems)
            {
                if (input.CurrencyId == input.MultiCurrencyId || input.MultiCurrencyId == null)
                {
                    p.MultiCurrencyTotal = p.Total;
                    p.MultiCurrencyUnitCost = p.UnitCost;
                }

                var saleOrder = saleOrderItems.FirstOrDefault(u => u.Id == p.Id);
                if (saleOrder == null) throw new UserFriendlyException(L("RecordNotFound"));

                //here is in only same purchaseOrder so no need to update purchaseOrder
                saleOrder.Update(
                    userId,
                    p.ItemId,
                    p.TaxId,
                    p.TaxRate,
                    p.Description,
                    p.Qty,
                    p.UnitCost,
                    p.DiscountRate,
                    p.Total,
                    p.MultiCurrencyUnitCost,
                    p.MultiCurrencyTotal);

                CheckErrors(await _saleOrderItemManager.UpdateAsync(saleOrder));
            }


            foreach (var t in deleteOrderItems)
            {
                CheckErrors(await _saleOrderItemManager.RemoveAsync(t));
            }
            #endregion

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == input.CurrencyId)
            {
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencyId = input.CurrencyId;
            }
            entity.Update(userId, input.CustomerId, input.Reference, input.CurrencyId, input.OrderNumber,
                    input.OrderDate, input.Memo, input.ShippingAddress, input.BillingAddress, input.SameAsShippingAddress,
                    input.SubTotal, input.Tax, input.Total, input.Status, input.ETD, input.LocationId, input.MultiCurrencyId,
                    input.MultiCurrencySubTotal, input.MultiCurrencyTotal, input.MultiCurrencyTax, input.UseExchangeRate);
            entity.UpdateSaleType(input.SaleTypeId);

            CheckErrors(await _saleOrderManager.UpdateAsync(@entity));

            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.SaleOrderId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = SaleOrderExchangeRate.Create(tenantId, userId, entity.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, entity.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if (exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }


            if (updateItemsHaveLink.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                await UpdateOrderInventoryStatus(input.Id);
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.SaleOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToClose)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToClose(UpdateStatus input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToClose();
            CheckErrors(await _saleOrderManager.UpdateAsync(@entity));

            var deliverySchedules = await _deliveryScheduleRepository.GetAll().Where(s => s.SaleOrderId == input.Id).ToListAsync();

            if (deliverySchedules.Any())
            {
                foreach(var s in deliverySchedules)
                {
                    s.UpdateStatusToClose();
                }

                await _deliveryScheduleRepository.BulkUpdateAsync(deliverySchedules);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        private async Task ValidateIssueLink(Guid orderId)
        {
            
            var validateByIssue = await _saleOrderItemRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.SaleOrderId == orderId)
                                        .Where(s => s.ItemIssueItems.Any())
                                        .AnyAsync();

            if (validateByIssue)
            {
                throw new UserFriendlyException(L("OrderAlreadyIssuedItems"));
            }
        }

        private async Task ValidateDeliverySchedule(Guid orderId)
        {
            var validateByIssue = await _deliveryScheduleRepository.GetAll()
                                       .AsNoTracking()
                                       .Where(s => s.SaleOrderId == orderId)
                                       .AnyAsync();

            if (validateByIssue)
            {
                throw new UserFriendlyException(L("OrderAlreadyDeliverySchedule"));
            }
        }

        private async Task ValidateDeliveryScheduleBySaleItem(List<Guid> Ids)
        {
            if (Ids.Count == 0) return;
            var validateByIssue = await _deliveryScheduleItemRepository.GetAll()
                                      .AsNoTracking()
                                      .Where(s => s.SaleOrderItemId.HasValue && Ids.Contains(s.SaleOrderItemId.Value))
                                      .AnyAsync();
            if (validateByIssue)
            {
                throw new UserFriendlyException(L("OrderAlreadyDeliverySchedule"));
            }

        }
        private async Task ValidateInvoiceLink(Guid orderId)
        {
           
            var validateByIssue = await _saleOrderItemRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.SaleOrderId == orderId)
                                        .Where(s => s.InvoiceItems.Any())
                                        .AnyAsync();

            if (validateByIssue)
            {
                throw new UserFriendlyException(L("OrderAlreadyConvertToInvoice"));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateIssueLink(input.Id);
            await ValidateInvoiceLink(input.Id);
            await ValidateDeliverySchedule(input.Id);
            entity.UpdateStatusToDraft();
            CheckErrors(await _saleOrderManager.UpdateAsync(@entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.UpdateStatusToPublish();
            CheckErrors(await _saleOrderManager.UpdateAsync(@entity));

            var deliverySchedules = await _deliveryScheduleRepository.GetAll().Where(s => s.SaleOrderId == input.Id).ToListAsync();

            if (deliverySchedules.Any())
            {
                foreach (var s in deliverySchedules)
                {
                    s.UpdateStatusToPublish();
                }

                await _deliveryScheduleRepository.BulkUpdateAsync(deliverySchedules);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _saleOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateIssueLink(input.Id);
            await ValidateInvoiceLink(input.Id);
            //await ValidateDeliverySchedule(input.Id);

            entity.UpdateStatusToVoid();
            CheckErrors(await _saleOrderManager.UpdateAsync(@entity));

            var deliverySchedules = await _deliveryScheduleRepository.GetAll().Where(s => s.SaleOrderId == input.Id).ToListAsync();

            if (deliverySchedules.Any())
            {
                foreach (var s in deliverySchedules)
                {
                    s.UpdateStatusToVoid();
                }

                await _deliveryScheduleRepository.BulkUpdateAsync(deliverySchedules);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrderForInvoice,
                      AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder)]
        public async Task<PagedResultDto<SaleOrderHeaderOutput>> GetSaleOrders(GetSaleOrderHeaderListInput input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();
            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                              .Where(s => s.OrderItemId.HasValue)
                                              .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.SaleOrderItem.SaleOrderId == input.SaleOrderId)
                                              .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       OrderItemId = iv.OrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.SaleOrderItemId.HasValue)
                                             .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.SaleOrderItem.SaleOrderId == input.SaleOrderId)
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     SaleOrderItemId = iv.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };

            var dinvoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                          .Where(s => s.DeliverySchedulItemId.HasValue)
                                          .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.DeliverySchedulItemId.HasValue && s.DeliveryScheduleItem.SaleOrderItem.SaleOrderId == input.SaleOrderId)
                                          .AsNoTracking()
                                    join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => s.ItemIssueSaleItemId.HasValue)
                                               .AsNoTracking()
                                    on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                    into r1

                                    join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => s.CustomerCreditItemId.HasValue)
                                               .AsNoTracking()
                                    on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                    into r2

                                    let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                    let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                    select new
                                    {
                                        iv.DeliverySchedulItemId,
                                        Qty = iv.Qty,
                                        ReturnQty = rq1 + rq2
                                    };

            var dissueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.DeliverySchedulItemId.HasValue && s.DeliveryScheduleItem.SaleOrderItem.SaleOrderId == input.SaleOrderId)
                                             .AsNoTracking()
                                  join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                             .Where(s => s.ItemIssueSaleItemId.HasValue)
                                             .AsNoTracking()
                                  on iv.Id equals ri.ItemIssueSaleItemId
                                  into r1

                                  join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                             .Where(s => s.CustomerCreditItemId.HasValue)
                                             .AsNoTracking()
                                  on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                  into r2

                                  let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                  let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                  select new
                                  {
                                      iv.DeliverySchedulItemId,
                                      Qty = iv.Qty,
                                      ReturnQty = rq1 + rq2
                                  };

            // delivery schedule item
            var delivaryItemQuery = from di in _deliveryScheduleItemRepository.GetAll()
                                         .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish || s.DeliverySchedule.Status == TransactionStatus.Close)
                                         .Where(s => s.SaleOrderItemId.HasValue)
                                         .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.SaleOrderItem.SaleOrderId == input.SaleOrderId)
                                         .AsNoTracking()

                                    join isi in dissueItemQuery
                                    on di.Id equals isi.DeliverySchedulItemId
                                    into iiItems

                                    join ivi in dinvoiceItemQuery
                                    on di.Id equals ivi.DeliverySchedulItemId
                                    into ivItems

                                    let issueQty = iiItems.Sum(s => s.Qty - s.ReturnQty) + ivItems.Sum(s => s.Qty - s.ReturnQty)
                                    let remainQty = di.Qty - issueQty
                                    where remainQty > 0

                                    select new
                                    {
                                        SaleOrderItemId = di.SaleOrderItemId,
                                        Qty = remainQty,
                                        ReturnQty = 0
                                    };

            var orderItemQuery = from soi in _saleOrderItemRepository.GetAll()
                                             .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.SaleOrderId == input.SaleOrderId)
                                             .AsNoTracking()

                                 join ivi in invoiceItemQuery
                                 on soi.Id equals ivi.OrderItemId into ivItems

                                 join iii in issueItemQuery
                                 on soi.Id equals iii.SaleOrderItemId into iItems

                                 join di in delivaryItemQuery on soi.Id equals di.SaleOrderItemId

                                 into diItems

                                 let diQty = diItems == null ? 0 : diItems.Sum(s=> s.Qty - s.ReturnQty)
                                 let issueQty = iItems == null ? 0 : iItems.Sum(s => s.Qty - s.ReturnQty)
                                 let invoiceQty = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)

                                 select (new
                                 {
                                     SaleOrderId = soi.SaleOrderId,
                                     remainQty = soi.Qty - issueQty - invoiceQty - diQty,
                                     Qty = soi.Qty,
                                     unitcost = soi.UnitCost,
                                 });

            var orderQuery = _saleOrderRepository.GetAll()
                         .Include(u => u.Customer)
                         .Include(u => u.SaleTransactionType)
                         .Include(s => s.Currency)
                         .Where(s => s.Status == TransactionStatus.Publish &&
                            (s.ApprovalStatus == ApprovalStatus.Approved ||
                            s.ApprovalStatus == ApprovalStatus.Recorded))
                         .Where(p => p.IsActive == true)
                         .WhereIf(input.SaleOrderId.HasValue && input.SaleOrderId != Guid.Empty, s => s.Id == input.SaleOrderId)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.MultiCurrencys != null && input.MultiCurrencys.Count > 0, u => input.MultiCurrencys.Contains(u.MultiCurrencyId))
                         .WhereIf(input.Customers != null && input.Customers.Any(), c => input.Customers.Contains(c.CustomerId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.OrderNumber.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .AsNoTracking()
                         .Select(so => new
                         {
                             SaleTransactionTypeId = so.SaleTransactionTypeId,
                             SaleTransactionType = ObjectMapper.Map<TransactionTypeSummaryOutput>(so.SaleTransactionType),
                             Customer = ObjectMapper.Map<CustomerSummaryOutput>(so.Customer),
                             CustomerId = so.CustomerId,
                             Memo = so.Memo,
                             Id = so.Id,
                             OrderDate = so.OrderDate,
                             OrderNumber = so.OrderNumber,
                             CurrencyId = so.CurrencyId,
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(so.Currency),
                             ETD = so.ETD,
                             Reference = so.Reference,
                             Total = so.Total
                         });

            var query = (from so in orderQuery

                         join soi in orderItemQuery
                         on so.Id equals soi.SaleOrderId
                         into poItems

                         where poItems.Any(s => s.remainQty > 0)

                         select new SaleOrderHeaderOutput
                         {
                             SaleTransactionTypeId = so.SaleTransactionTypeId,
                             SaleTransactionType = so.SaleTransactionType,
                             Customer = so.Customer,
                             CustomerId = so.CustomerId,
                             Memo = so.Memo,
                             Id = so.Id,
                             OrderDate = so.OrderDate,
                             OrderNumber = so.OrderNumber,
                             Total = so.Total,
                             CountSaleOrderItems = poItems.Count(),
                             CurrencyId = so.CurrencyId,
                             Currency = so.Currency,
                             ETD = so.ETD,
                             Reference = so.Reference

                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(t => t.ETD).PageBy(input).ToListAsync();
            return new PagedResultDto<SaleOrderHeaderOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices,
                      AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues)]
        public async Task<GetListSaleOrderItemDetail> GetItemSaleOrders(EntityDto<Guid> input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var @entity = await _saleOrderManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                             .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       OrderItemId = iv.OrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.SaleOrderItemId.HasValue)
                                             .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     SaleOrderItemId = iv.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };

            var dinvoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                         .Where(s => s.DeliverySchedulItemId.HasValue)
                                         .Where(s => s.DeliveryScheduleItem.SaleOrderItem.SaleOrderId == input.Id)
                                         .AsNoTracking()
                                    join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => s.ItemIssueSaleItemId.HasValue)
                                               .AsNoTracking()
                                    on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                    into r1

                                    join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => s.CustomerCreditItemId.HasValue)
                                               .AsNoTracking()
                                    on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                    into r2

                                    let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                    let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                    select new
                                    {
                                        iv.DeliverySchedulItemId,
                                        Qty = iv.Qty,
                                        ReturnQty = rq1 + rq2
                                    };

            var dissueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.DeliveryScheduleItem.SaleOrderItem.SaleOrderId == input.Id)
                                             .AsNoTracking()
                                  join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                             .Where(s => s.ItemIssueSaleItemId.HasValue)
                                             .AsNoTracking()
                                  on iv.Id equals ri.ItemIssueSaleItemId
                                  into r1

                                  join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                             .Where(s => s.CustomerCreditItemId.HasValue)
                                             .AsNoTracking()
                                  on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                  into r2

                                  let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                  let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                  select new
                                  {
                                      iv.DeliverySchedulItemId,
                                      Qty = iv.Qty,
                                      ReturnQty = rq1 + rq2
                                  };

            // delivery schedule item
            var delivaryItemQuery = from di in _deliveryScheduleItemRepository.GetAll()
                                         .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish || s.DeliverySchedule.Status == TransactionStatus.Close)
                                         .Where(s => s.SaleOrderItemId.HasValue)
                                         .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                         .AsNoTracking()

                                    join isi in dissueItemQuery
                                    on di.Id equals isi.DeliverySchedulItemId
                                    into iiItems

                                    join ivi in dinvoiceItemQuery
                                    on di.Id equals ivi.DeliverySchedulItemId
                                    into ivItems

                                    let issueQty = iiItems.Sum(s => s.Qty - s.ReturnQty) + ivItems.Sum(s => s.Qty - s.ReturnQty)
                                    let remainQty = di.Qty - issueQty
                                    where remainQty > 0

                                    select new
                                    {
                                        SaleOrderItemId = di.SaleOrderItemId,
                                        Qty = remainQty,
                                        ReturnQty = 0
                                    };

            var @saleOrderItem = await (from si in _saleOrderItemRepository.GetAll()
                                             .Include(u => u.Item.InventoryAccount)
                                             .Include(u => u.Tax)
                                             .Include(u => u.Item.PurchaseAccount)
                                             .Where(s => s.SaleOrder.Status == TransactionStatus.Publish &&
                                                        (s.SaleOrder.ApprovalStatus == ApprovalStatus.Approved ||
                                                        s.SaleOrder.ApprovalStatus == ApprovalStatus.Recorded))
                                             .Where(p => p.SaleOrder.IsActive == true)
                                             .Where(u => u.SaleOrderId == entity.Id).AsNoTracking()

                                        join ivi in invoiceItemQuery
                                        on si.Id equals ivi.OrderItemId
                                        into ivItems

                                        join iii in issueItemQuery
                                        on si.Id equals iii.SaleOrderItemId
                                        into iItems

                                        join di in delivaryItemQuery 
                                        on si.Id equals di.SaleOrderItemId
                                        into diItems

                                        let ivQty = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                        let iiQty = iItems == null ? 0 : iItems.Sum(s => s.Qty - s.ReturnQty)
                                        let diQty = diItems == null ? 0 : diItems.Sum(s=>s.Qty - s.ReturnQty)
                                        let remainQty = si.Qty - ivQty - iiQty - diQty

                                        where remainQty > 0

                                        select new SaleOrderItemSummaryOut
                                        {
                                            Id = si.Id,
                                            Description = si.Description,
                                            DiscountRate = si.DiscountRate,
                                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(si.Item),
                                            ItemId = si.ItemId,
                                            Remain = remainQty,
                                            Qty = si.Qty,
                                            Tax = ObjectMapper.Map<TaxDetailOutput>(si.Tax),
                                            TaxId = si.TaxId,
                                            Total = Math.Round(si.UnitCost * remainQty, roundDigits),
                                            UnitCost = si.UnitCost,
                                            MultiCurrencyTotal = Math.Round(si.MultiCurrencyUnitCost * remainQty, roundDigits),
                                            MultiCurrencyUnitCost = si.MultiCurrencyUnitCost,
                                            UseBatchNo = si.Item.UseBatchNo,
                                            AutoBatchNo = si.Item.AutoBatchNo,
                                            TrackSerial = si.Item.TrackSerial,
                                            TrackExpiration = si.Item.TrackExpiration
                                        }).ToListAsync();
            var result = ObjectMapper.Map<GetListSaleOrderItemDetail>(@entity);
            result.Total = Math.Round(saleOrderItem.Sum(s => s.Total), roundDigits);
            result.SaleOrderItems = ObjectMapper.Map<List<SaleOrderItemSummaryOut>>(saleOrderItem);
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices,
                      AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues)]
        public async Task<GetListSaleOrderItemDetail> GetSaleOrderItemsForView(EntityDto<Guid> input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var @entity = await _saleOrderManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                              .Where(s => s.OrderItemId.HasValue)
                                              .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                              .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       OrderItemId = iv.OrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.SaleOrderItemId.HasValue)
                                             .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     SaleOrderItemId = iv.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };


            var dinvoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                          .Where(s => s.DeliverySchedulItemId.HasValue)
                                          .Where(s => s.DeliveryScheduleItem.SaleOrderItem.SaleOrderId == input.Id)
                                          .AsNoTracking()
                                    join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => s.ItemIssueSaleItemId.HasValue)
                                               .AsNoTracking()
                                    on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                    into r1

                                    join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => s.CustomerCreditItemId.HasValue)
                                               .AsNoTracking()
                                    on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                    into r2

                                    let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                    let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                    select new
                                    {
                                        iv.DeliverySchedulItemId,
                                        Qty = iv.Qty,
                                        ReturnQty = rq1 + rq2
                                    };

            var dissueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.DeliveryScheduleItem.SaleOrderItem.SaleOrderId == input.Id)
                                             .AsNoTracking()
                                  join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                             .Where(s => s.ItemIssueSaleItemId.HasValue)
                                             .AsNoTracking()
                                  on iv.Id equals ri.ItemIssueSaleItemId
                                  into r1

                                  join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                             .Where(s => s.CustomerCreditItemId.HasValue)
                                             .AsNoTracking()
                                  on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                  into r2

                                  let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                  let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                  select new
                                  {
                                      iv.DeliverySchedulItemId,
                                      Qty = iv.Qty,
                                      ReturnQty = rq1 + rq2
                                  };

            // delivery schedule item
            var delivaryItemQuery = from di in _deliveryScheduleItemRepository.GetAll()
                                         .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish || s.DeliverySchedule.Status == TransactionStatus.Close)
                                         .Where(s => s.SaleOrderItemId.HasValue)
                                         .Where(s => s.SaleOrderItem.SaleOrderId == input.Id)
                                         .AsNoTracking()

                                    join isi in dissueItemQuery
                                    on di.Id equals isi.DeliverySchedulItemId
                                    into iiItems

                                    join ivi in dinvoiceItemQuery
                                    on di.Id equals ivi.DeliverySchedulItemId
                                    into ivItems

                                    let issueQty = iiItems.Sum(s => s.Qty - s.ReturnQty) + ivItems.Sum(s => s.Qty - s.ReturnQty)
                                    let remainQty = di.Qty - issueQty
                                    where remainQty > 0

                                    select new
                                    {
                                        SaleOrderItemId = di.SaleOrderItemId,
                                        Qty = remainQty,
                                        ReturnQty = 0
                                    };

            var @saleOrderItem = await (from si in _saleOrderItemRepository.GetAll()
                                             .Include(u => u.Item.InventoryAccount)
                                             .Include(u => u.Tax)
                                             .Include(u => u.Item.PurchaseAccount)
                                             .Where(s => s.SaleOrder.Status == TransactionStatus.Publish &&
                                                        (s.SaleOrder.ApprovalStatus == ApprovalStatus.Approved ||
                                                        s.SaleOrder.ApprovalStatus == ApprovalStatus.Recorded))
                                             .Where(p => p.SaleOrder.IsActive == true)
                                             .Where(u => u.SaleOrderId == entity.Id).AsNoTracking()

                                        join ivi in invoiceItemQuery
                                        on si.Id equals ivi.OrderItemId
                                        into ivItems

                                        join iii in issueItemQuery
                                        on si.Id equals iii.SaleOrderItemId
                                        into iItems

                                        join di in delivaryItemQuery 
                                        on si.Id equals di.SaleOrderItemId
                                        into diItems

                                        let ivQty = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                        let iiQty = iItems == null ? 0 : iItems.Sum(s => s.Qty - s.ReturnQty)
                                        let diQty = diItems == null ? 0 : diItems.Sum(s=>s.Qty - s.ReturnQty)
                                        let remainQty = si.Qty - ivQty - iiQty - diQty

                                        select new SaleOrderItemSummaryOut
                                        {
                                            Id = si.Id,
                                            Description = si.Description,
                                            DiscountRate = si.DiscountRate,
                                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(si.Item),
                                            ItemId = si.ItemId,
                                            Remain = remainQty,
                                            Qty = si.Qty,
                                            Tax = ObjectMapper.Map<TaxDetailOutput>(si.Tax),
                                            TaxId = si.TaxId,
                                            Total = si.Total,
                                            UnitCost = si.UnitCost,
                                            MultiCurrencyTotal = si.MultiCurrencyTotal,
                                            MultiCurrencyUnitCost = si.MultiCurrencyUnitCost
                                        }).ToListAsync();
            var result = ObjectMapper.Map<GetListSaleOrderItemDetail>(@entity);
            result.Total = Math.Round(saleOrderItem.Sum(s => s.Total), roundDigits);
            result.SaleOrderItems = ObjectMapper.Map<List<SaleOrderItemSummaryOut>>(saleOrderItem);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices,
                      AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues)]
        public async Task<ListResultDto<SaleOrderItemFroInvoiceDto>> GetSaleOrderItemsForInvoice(GetSaleOrderItemInput input)
        {

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                            .Where(s => s.OrderItemId.HasValue)
                                            .WhereIf(!input.ItemIds.IsNullOrEmpty(), s => input.ItemIds.Contains(s.ItemId.Value))
                                            .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       OrderItemId = iv.OrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.SaleOrderItemId.HasValue)
                                             .WhereIf(!input.ItemIds.IsNullOrEmpty(), s => input.ItemIds.Contains(s.ItemId))
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     SaleOrderItemId = iv.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };

            var dinvoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                            .Where(s => s.DeliverySchedulItemId.HasValue)
                                            .WhereIf(!input.ItemIds.IsNullOrEmpty(), s => input.ItemIds.Contains(s.ItemId.Value))
                                            .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       iv.DeliveryScheduleItem.SaleOrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var dissueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .WhereIf(!input.ItemIds.IsNullOrEmpty(), s => input.ItemIds.Contains(s.ItemId))
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     iv.DeliveryScheduleItem.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };

            //// delivery schedule item
            //var delivaryItemQuery = from di in _deliveryScheduleItemRepository.GetAll()
            //                             .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish || s.DeliverySchedule.Status == TransactionStatus.Close)
            //                             .Where(s => s.SaleOrderItemId.HasValue)
            //                             .WhereIf(!input.ItemIds.IsNullOrEmpty(), s => input.ItemIds.Contains(s.ItemId))
            //                             .AsNoTracking()

            //                        join isi in dissueItemQuery
            //                        on di.Id equals isi.DeliverySchedulItemId
            //                        into iiItems

            //                        join ivi in dinvoiceItemQuery
            //                        on di.Id equals ivi.DeliverySchedulItemId
            //                        into ivItems

            //                        let issueQty = iiItems.Sum(s => s.Qty - s.ReturnQty) + ivItems.Sum(s => s.Qty - s.ReturnQty) 
            //                        let remainQty = di.Qty - issueQty
            //                        where remainQty > 0

            //                        select new
            //                        {
            //                            SaleOrderItemId = di.SaleOrderItemId,
            //                            Qty = remainQty,
            //                            ReturnQty = 0
            //                        };


            var query = from si in _saleOrderItemRepository.GetAll()
                                .Where(s => s.SaleOrder.Status == TransactionStatus.Publish &&
                                        (s.SaleOrder.ApprovalStatus == ApprovalStatus.Approved ||
                                        s.SaleOrder.ApprovalStatus == ApprovalStatus.Recorded))
                                .Where(p => p.SaleOrder.IsActive == true)
                                .WhereIf(input.LocationIds != null && input.LocationIds.Any(), s => input.LocationIds.Contains(s.SaleOrder.LocationId))
                                .WhereIf(input.ItemIds != null && input.ItemIds.Any(), s => input.ItemIds.Contains(s.ItemId))
                                .WhereIf(input.Customers != null && input.Customers.Any(), u => input.Customers.Contains(u.SaleOrder.CustomerId))
                                .WhereIf(input.MultiCurrencys != null && input.MultiCurrencys.Any(), s => input.MultiCurrencys.Contains(s.SaleOrder.MultiCurrencyId))
                                .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    u => u.SaleOrder.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                    u.SaleOrder.Reference.ToLower().Contains(input.Filter.ToLower())
                                )
                                .AsNoTracking()
                        join isi in issueItemQuery
                        on si.Id equals isi.SaleOrderItemId
                        into iiItems

                        join ivi in invoiceItemQuery
                        on si.Id equals ivi.OrderItemId
                        into ivItems

                        join disi in dissueItemQuery
                         on si.Id equals disi.SaleOrderItemId
                         into diiItems

                        join divi in dinvoiceItemQuery
                        on si.Id equals divi.SaleOrderItemId
                        into divItems

                        let issueQty = iiItems.Sum(s => s.Qty - s.ReturnQty) + ivItems.Sum(s => s.Qty - s.ReturnQty) + diiItems.Sum(s => s.Qty - s.ReturnQty) + divItems.Sum(s => s.Qty - s.ReturnQty)
                        let remainQty = si.Qty - issueQty
                        where remainQty > 0
                        select new SaleOrderItemFroInvoiceDto
                        {
                            Date = si.SaleOrder.OrderDate,
                            Id = si.Id,
                            ItemId = si.ItemId,
                            IssueQty = issueQty,
                            RemainQty = remainQty,
                            Qty = si.Qty,
                            Total = si.Total,
                            UnitCost = si.MultiCurrencyUnitCost,
                            CurrencyCode = si.SaleOrder.MultiCurrency.Code,
                            OrderId = si.SaleOrderId,
                            OrderNumber = si.SaleOrder.OrderNumber,
                            Reference = si.SaleOrder.Reference,
                            LocationName = si.SaleOrder.Location.LocationName
                        };

            var saleOrderItems = await query.ToListAsync();

            return new ListResultDto<SaleOrderItemFroInvoiceDto>(saleOrderItems);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList, AppPermissions.Pages_Tenant_Report_SaleOrder)]
        public async Task<ListResultDto<SaleOrderInvoiceDetailDto>> GetInvoiceDetailForOrder(EntityDto<Guid> input)
        {
            var invoiceItemHasOrderQuery = from bi in _invoiceItemRepository.GetAll()
                                                   .Include(s => s.Invoice.Customer)
                                                   .Include(s => s.ItemIssueItem)
                                                   .Where(s => s.ItemIssueItemId.HasValue)
                                                   .Where(s => (s.OrderItemId.HasValue && s.SaleOrderItem.SaleOrderId == input.Id)
                                                   ||(s.DeliverySchedulItemId.HasValue && s.DeliveryScheduleItem.DeliverySchedule.SaleOrderId == input.Id))
                                                   .AsNoTracking()
                                           join b in _journalRepository.GetAll()
                                                     .Include(s => s.Currency)
                                                     .Include(s => s.MultiCurrency)
                                                     .Where(s => s.JournalType == JournalType.Invoice && s.Status == TransactionStatus.Publish)
                                                     .AsNoTracking()
                                           on bi.InvoiceId equals b.InvoiceId
                                           join r in _journalRepository.GetAll()
                                                     .Where(s => s.JournalType == JournalType.ItemIssueSale && s.Status == TransactionStatus.Publish)
                                           on bi.ItemIssueItem.ItemIssueId equals r.ItemIssueId

                                           select new SaleOrderInvoiceDetailDto
                                           {
                                               IssueDate = r.Date,
                                               IssueId = r.ItemIssueId.Value,
                                               IssueNo = r.JournalNo,
                                               IssueReference = r.Reference,

                                               PaidStatus = bi.Invoice.PaidStatus,
                                               Total = bi.Invoice.Total,
                                               PaidAmount = bi.Invoice.TotalPaid,
                                               Balance = bi.Invoice.OpenBalance,
                                               InvoiceId = bi.InvoiceId,
                                               InvoiceNo = b.JournalNo,
                                               CustomerId = bi.Invoice.CustomerId,
                                               CustomerName = bi.Invoice.Customer.CustomerName,
                                               InvoiceReference = b.Reference,
                                               CurrencyCode = b.MultiCurrency != null ? b.MultiCurrency.Code : b.Currency.Code,
                                           };

            var invoiceItemHasIssueQuery = from bi in _invoiceItemRepository.GetAll()
                                                  .Where(s => s.ItemIssueItemId.HasValue && s.ItemIssueItem.SaleOrderItemId.HasValue || s.ItemIssueItem.DeliverySchedulItemId.HasValue)
                                                  .Where(s => (s.ItemIssueItem.SaleOrderItem.SaleOrderId == input.Id) || (s.DeliverySchedulItemId.HasValue && s.DeliveryScheduleItem.DeliverySchedule.SaleOrderId == input.Id))
                                                  .AsNoTracking()
                                           join b in _journalRepository.GetAll()
                                                   .Where(s => s.JournalType == JournalType.Invoice && s.Status == TransactionStatus.Publish)
                                                   .AsNoTracking()
                                         on bi.InvoiceId equals b.InvoiceId
                                           select new
                                           {
                                               ReceivedStatus = bi.Invoice.ReceivedStatus,
                                               PaidStatus = bi.Invoice.PaidStatus,
                                               InvoiceId = bi.InvoiceId,
                                               InvoiceNo = b.JournalNo,
                                               InvoiceReference = b.Reference,
                                               ItemIssueItemId = bi.ItemIssueItemId,
                                               PaidAmount = bi.Invoice.TotalPaid,
                                               Balance = bi.Invoice.OpenBalance
                                           };

            var itemReceiptItemHasOrderQuery = from ri in _itemIssueItemRepository.GetAll()
                                                           .Include(s => s.ItemIssue.Customer)
                                                           .Where(s => (s.SaleOrderItemId.HasValue && s.SaleOrderItem.SaleOrderId == input.Id) || (s.DeliverySchedulItemId.HasValue && s.DeliveryScheduleItem.DeliverySchedule.SaleOrderId == input.Id))
                                                           .AsNoTracking()
                                               join r in _journalRepository.GetAll()
                                                         .Include(s => s.Currency)
                                                         .Include(s => s.MultiCurrency)
                                                         .Where(s => s.JournalType == JournalType.ItemIssueSale && s.Status == TransactionStatus.Publish)
                                               on ri.ItemIssueId equals r.ItemIssueId

                                               select new
                                               {
                                                   IssueItemId = ri.Id,
                                                   IssueDate = r.Date,
                                                   IssueId = r.ItemIssueId.Value,
                                                   IssueNo = r.JournalNo,
                                                   IssueReference = r.Reference,
                                                   CustomerId = ri.ItemIssue.CustomerId.Value,
                                                   CustomerName = ri.ItemIssue.Customer.CustomerName,
                                                   CurrencyCode = r.MultiCurrency != null ? r.MultiCurrency.Code : r.Currency.Code,
                                                   Total = ri.ItemIssue.Total,
                                               };

            var invoiceItems = await invoiceItemHasIssueQuery.ToListAsync();
            var receiveItems = await itemReceiptItemHasOrderQuery.ToListAsync();

            var itemRceiptFromItemIssueItems = from ri in receiveItems

                                               join bi in invoiceItems
                                               on ri.IssueItemId equals bi.ItemIssueItemId
                                               into bItems
                                               from bi in bItems.DefaultIfEmpty()

                                               select new SaleOrderInvoiceDetailDto
                                               {
                                                   IssueDate = ri.IssueDate,
                                                   IssueId = ri.IssueId,
                                                   IssueNo = ri.IssueNo,
                                                   IssueReference = ri.IssueReference,
                                                   CustomerId = ri.CustomerId,
                                                   CustomerName = ri.CustomerName,
                                                   CurrencyCode = ri.CurrencyCode,
                                                   Total = ri.Total,

                                                   PaidStatus = bi == null ? (PaidStatuse?)null : bi.PaidStatus,
                                                   InvoiceId = bi == null ? (Guid?)null : bi.InvoiceId,
                                                   InvoiceNo = bi == null ? "" : bi.InvoiceNo,
                                                   InvoiceReference = bi == null ? "" : bi.InvoiceReference,
                                                   PaidAmount = bi == null ? 0 : bi.PaidAmount,
                                                   Balance = bi == null ? ri.Total : bi.Balance
                                               };


            var itemReceiptFromInvoiceItem = await invoiceItemHasOrderQuery.ToListAsync();

            var items = itemReceiptFromInvoiceItem
                        .Union(itemRceiptFromItemIssueItems)
                        .GroupBy(s => new
                        {
                            s.IssueDate,
                            s.IssueId,
                            s.IssueReference,
                            s.IssueNo,
                            s.InvoiceReference,
                            s.InvoiceNo,
                            s.InvoiceId,
                            s.CurrencyCode,
                            s.Total,
                            s.CustomerId,
                            s.CustomerName,
                            s.PaidStatus,
                            s.PaidAmount,
                            s.Balance
                        })
                        .Select(s => new SaleOrderInvoiceDetailDto
                        {
                            IssueId = s.Key.IssueId,
                            IssueDate = s.Key.IssueDate,
                            IssueNo = s.Key.IssueNo,
                            IssueReference = s.Key.IssueReference,
                            CustomerId = s.Key.CustomerId,
                            CustomerName = s.Key.CustomerName,
                            InvoiceId = s.Key.InvoiceId,
                            InvoiceNo = s.Key.InvoiceNo,
                            InvoiceReference = s.Key.InvoiceReference,
                            CurrencyCode = s.Key.CurrencyCode,
                            Total = s.Key.Total,
                            PaidStatus = s.Key.PaidStatus,
                            PaidAmount = s.Key.PaidAmount,
                            Balance = s.Key.Balance
                        }).ToList();


            return new ListResultDto<SaleOrderInvoiceDetailDto>(items);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateInventoryStatus)]
        public async Task UpdateInventoryStatus(EntityDto<Guid> input)
        {
            await UpdateOrderInventoryStatus(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_GetDetail)]
        public async Task<ListResultDto<GetListDeliveryScheduleOutput>> GetListDeliverySchedules(EntityDto<Guid> input)
        {
            var result = await _deliveryScheduleRepository.GetAll().AsNoTracking()
                               .Where(s => s.SaleOrderId == input.Id)
                               .OrderBy(s => s.FinalDeliveryDate)
                               .Select(s => new GetListDeliveryScheduleOutput
                               {
                                   Id = s.Id,
                                   InitialDeliveryDate = s.InitialDeliveryDate,
                                   FinalDeliveryDate = s.FinalDeliveryDate,
                                   DeliveryNo = s.DeliveryNo,
                                   Reference = s.Reference,
                                   ReceiveStatus = s.ReceiveStatus,
                                   LocationId = s.LocationId,
                                   LocationName = s.Location.LocationName,
                                   Customer = new CustomerSummaryOutput
                                   {
                                       CustomerCode = s.Customer.CustomerCode,
                                       CustomerName = s.Customer.CustomerName,
                                       Id = s.CustomerId,
                                   },
                                   SaleOrderId = s.SaleOrderId,
                                   SaleOrderNo = s.SaleOrder.OrderNumber,
                                   Status = s.Status,
                                   StatusName = s.Status.ToString(),
                                   ReceiveStatusName = s.ReceiveStatus.ToString(),
                               })
                               .ToListAsync();

            return new ListResultDto<GetListDeliveryScheduleOutput> { Items = result };
        }
    }
}
