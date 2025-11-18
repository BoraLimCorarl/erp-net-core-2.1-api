using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Bills.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.PurchaseOrders;
using CorarlERP.PurchaseOrders.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Currencies.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies;
using CorarlERP.ItemReceipts;
using System.Collections.Generic;
using CorarlERP.Items;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Invoices.Dto;
using CorarlERP.PayBills;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.MultiTenancy;
using CorarlERP.AutoSequences;
using CorarlERP.Dto;
using CorarlERP.Authorization.Users;
using CorarlERP.Lots.Dto;
using CorarlERP.MultiCurrencys.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using OfficeOpenXml;
using CorarlERP.Reports;
using System.IO;
using CorarlERP.Locations;
using CorarlERP.Classes;
using CorarlERP.Addresses;
using CorarlERP.Taxes;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.Settings;
using Abp.Dependency;
using CorarlERP.FileStorages;
using CorarlERP.ItemTypes;
using CorarlERP.InventoryCalculationJobSchedules;
using static CorarlERP.Authorization.Roles.StaticRoleNames;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.TransferOrders;
using CorarlERP.Productions.Dto;
using CorarlERP.Productions;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using CorarlERP.Features;
using Abp.Domain.Uow;
using CorarlERP.Lots;
using CorarlERP.Migrations;
using Telegram.Bot.Types.Payments;
using Abp.Domain.Entities;
using CorarlERP.AccountCycles;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.EntityFrameworkCore.Repositories;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using CorarlERP.Customers;
using CorarlERP.Invoices;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Exchanges.Dto;
using CorarlERP.VendorCredit;

namespace CorarlERP.Bills
{
    [AbpAuthorize]
    public class BillAppService : ReportBaseClass, IBillAppService
    {
        private readonly AppFolders _appFolders;
        private readonly ICorarlRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IItemReceiptItemManager _itemReceiptItemManager;

        private readonly ICorarlRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IItemReceiptManager _itemReceiptManager;

        private readonly ICurrencyManager _currencyManager;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly IVendorManager _vendorItemManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IPurchaseOrderItemManager _purhcaserOrderItemManager;
        private readonly ICorarlRepository<PurchaseOrderItem, Guid> _purhcaseOrderItemRepository;

        private readonly IPurchaseOrderManager _purhcaserOrderManager;
        private readonly ICorarlRepository<PurchaseOrder, Guid> _purhcaseOrderRepository;

        private readonly IBillItemManager _billItemManager;
        private readonly IBillManager _billManager;

        private readonly ICorarlRepository<Bill, Guid> _billRepository;
        private readonly ICorarlRepository<BillItem, Guid> _billItemRepository;

        private readonly ICorarlRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        private readonly ICorarlRepository<VendorCredit.VendorCreditDetail, Guid> _vendorCreditDetailRepository;

        private readonly IJournalManager _journalManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IItemManager _itemManager;

        private readonly IRepository<PayBillDetail, Guid> _payBillDetailRepository;

        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<Class, long> _classRepository;

        //paybill validate 
        private readonly IRepository<PayBill, Guid> _payBillRepository;
        private readonly ICorarlRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<VendorCreditItemBatchNo, Guid> _vendorCreditItemBatchNoRepository; 
        private readonly ICorarlRepository<BillExchangeRate, Guid> _exchangeRateRepository;
        private readonly ICorarlRepository<VendorCreditExchangeRate, Guid> _vendorCreditExchangeRateRepository;

        private readonly ITenantManager _tenantManager;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly ICorarlRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IRepository<User, long> _userRepository;

        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;

        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public BillAppService(
            ICorarlRepository<BillExchangeRate, Guid> exchangeRateRepository,
            ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            ICorarlRepository<VendorCreditItemBatchNo, Guid> vendorCreditItemBatchNoRepository,
            ICorarlRepository<VendorCreditExchangeRate, Guid> vendorCreditExchangeRateRepository,
            IJournalManager journalManager, 
            IJournalItemManager journalItemManager,
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Lot, long> lotRepository,
            ICurrencyManager currencyManager,
            IChartOfAccountManager chartOfAccountManager, BillManager billManager,
            BillItemManager billItemManager,
            ItemReceiptItemManager itemreceiptitemManager,
            ItemReceiptManager itemreceiptManager,
            ItemManager itemManager,
            IFileStorageManager fileStorageManager,
            PurchaseOrderItemManager purchaseOrderItemManager,
            PurchaseOrderManager purchaseOrderManager,
            VendorManager vendorManager,
            IRepository<Vendor, Guid> vendorRepository,
            ICorarlRepository<Journal, Guid> journalRepository,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            ICorarlRepository<Bill, Guid> billRepository,
            ICorarlRepository<BillItem, Guid> billItemRepository,
            ICorarlRepository<PurchaseOrderItem, Guid> purchaseOrderItemRepository,
            ICorarlRepository<PurchaseOrder, Guid> purchaseOrderRepository,
            IRepository<Currency, long> currencyRepository,
            ICorarlRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            ICorarlRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<PayBillDetail, Guid> payBillDetailRepository,
            ICorarlRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            ICorarlRepository<VendorCredit.VendorCreditDetail, Guid> vendorCreditDetailRepository,
            IRepository<PayBill, Guid> payBillRepository,
            ICorarlRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCreditRepository,
            ITenantManager tenantManager,
            IAutoSequenceManager autoSequenceManger,
            ICorarlRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<User, long> userRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            ICorarlRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<Lock, long> lockRepository,
            IRepository<Location, long> locationRepository,
            IRepository<Class, long> classRepository,
            IRepository<Tax, long> taxRepository,
            IRepository<AccountCycles.AccountCycle, long> accountCycleRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            AppFolders appFolders
            ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _appFolders = appFolders;
            _userRepository = userRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptPurchase);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _billItemManager = billItemManager;
            _billItemRepository = billItemRepository;
            _billManager = billManager;
            _billRepository = billRepository;
            _purhcaseOrderItemRepository = purchaseOrderItemRepository;
            _purhcaserOrderItemManager = purchaseOrderItemManager;
            _purhcaseOrderRepository = purchaseOrderRepository;
            _purhcaserOrderManager = purchaseOrderManager;
            _vendorRepository = vendorRepository;
            _vendorItemManager = vendorManager;
            _currencyRepository = currencyRepository;
            _currencyManager = currencyManager;
            _itemReceiptItemManager = itemreceiptitemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptManager = itemreceiptManager;
            _itemReceiptRepository = itemReceiptRepository;

            _itemManager = itemManager;
            _itemRepository = itemRepository;
            _vendorCreditDetailRepository = vendorCreditDetailRepository;
            _vendorCreditRepository = vendorCreditRepository;
            _payBillDetailRepository = payBillDetailRepository;
            _payBillRepository = payBillRepository;
            _itemIssueVendorCreditRepository = itemIssueVendorCreditRepository;

            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _taxRepository = taxRepository;
            _lockRepository = lockRepository;
            _fileStorageManager = fileStorageManager;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;

            _settingRepository = IocManager.Instance.Resolve<IRepository<BillInvoiceSetting, long>>();
            _unitOfWorkManager = unitOfWorkManager;
            _lotRepository = lotRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
            _vendorCreditItemBatchNoRepository = vendorCreditItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _exchangeRateRepository = exchangeRateRepository;
            _vendorCreditExchangeRateRepository = vendorCreditExchangeRateRepository;
        }

        private async Task ValidateAddBatchNo(CreateBillInput input)
        {
            if (!input.BillItems.Any(r => r.ItemId.HasValue && r.ItemId != Guid.Empty)) return;

            var validateItems = await _itemRepository.GetAll()
                            .Include(s => s.BatchNoFormula)
                            .Where(s => input.BillItems.Any(i => i.ItemId == s.Id))
                            .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                            .AsNoTracking()
                            .ToListAsync();

            if (!validateItems.Any()) return;

            var useBatchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var useBatchItems = input.BillItems.Where(s => useBatchItemDic.ContainsKey(s.ItemId.Value)).ToList();

            var emptyManualBatchItem = useBatchItems.FirstOrDefault(s => !useBatchItemDic[s.ItemId.Value].AutoBatchNo && (s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNumber.IsNullOrWhiteSpace())));
            if (emptyManualBatchItem != null) throw new UserFriendlyException(L("PleaseEnter", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {useBatchItemDic[emptyManualBatchItem.ItemId.Value].ItemCode}-{useBatchItemDic[emptyManualBatchItem.ItemId.Value].ItemName}");

            var expriationItemHash = validateItems.Where(s => s.TrackExpiration).Select(s => s.Id).ToHashSet();
            var expirationItems = input.BillItems.Where(s => expriationItemHash.Contains(s.ItemId.Value)).ToList();
            var emptyExpiration = expirationItems.FirstOrDefault(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => !r.ExpirationDate.HasValue));
            if (emptyExpiration != null) throw new UserFriendlyException(L("PleaseEnter", L("ExpiratioinDate")) + $", Item: {useBatchItemDic[emptyExpiration.ItemId.Value].ItemCode}-{useBatchItemDic[emptyExpiration.ItemId.Value].ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.BillItems.Where(s => serialItemHash.Contains(s.ItemId.Value)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Item: {useBatchItemDic[serialQty.ItemId.Value].ItemCode}-{useBatchItemDic[serialQty.ItemId.Value].ItemName}");

            var autoBatchItems = useBatchItems.Where(s => useBatchItemDic[s.ItemId.Value].AutoBatchNo).ToList();
            if (autoBatchItems.Any())
            {
                foreach (var item in autoBatchItems)
                {
                    var formula = useBatchItemDic[item.ItemId.Value].BatchNoFormula;
                    if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $", Item: {useBatchItemDic[item.ItemId.Value].ItemCode}-{useBatchItemDic[item.ItemId.Value].ItemName}");

                    var field = useBatchItemDic[item.ItemId.Value].GetType().GetProperty(formula.FieldName).GetValue(useBatchItemDic[item.ItemId.Value], null);
                    if (field == null) throw new UserFriendlyException(L("IsNotValid", formula.FieldName));

                    var code = field.ToString();

                    if (item.ItemBatchNos == null) item.ItemBatchNos = new List<BatchNoItemOutput> { new BatchNoItemOutput { Qty = item.Qty } };

                    foreach (var batch in item.ItemBatchNos)
                    {
                        batch.BatchNumber = $"{code}-{input.RecieptDate.ToString(formula.DateFormat)}";
                    }
                }
            }

            var duplicate = useBatchItems.FirstOrDefault(s => s.ItemBatchNos.GroupBy(g => g.BatchNumber).Any(r => r.Count() > 1));
            if (duplicate != null) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $" , Item: {useBatchItemDic[duplicate.ItemId.Value].ItemCode}-{useBatchItemDic[duplicate.ItemId.Value].ItemName}"));

            var zeroQty = useBatchItems.FirstOrDefault(s => s.ItemBatchNos.Any(r => r.Qty <= 0));
            if (zeroQty != null) throw new UserFriendlyException(L("MustBeGreaterThen", L("Qty"), 0));

            var validateQty = useBatchItems.FirstOrDefault(s => s.Qty != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", {useBatchItemDic[validateQty.ItemId.Value].ItemCode}-{useBatchItemDic[validateQty.ItemId.Value].ItemName}"));

            var batchNos = useBatchItems.SelectMany(s => s.ItemBatchNos).GroupBy(s => s.BatchNumber).Select(s => s.Key).ToHashSet();
            var batchNoDic = await _batchNoRepository.GetAll().Where(s => batchNos.Contains(s.Code)).ToDictionaryAsync(k => k.Code, v => v);

            var addBatchNos = new List<BatchNo>();
            foreach (var item in useBatchItems)
            {
                foreach (var i in item.ItemBatchNos)
                {
                    if (batchNoDic.ContainsKey(i.BatchNumber))
                    {
                        if (batchNoDic[i.BatchNumber].ItemId != item.ItemId) throw new UserFriendlyException(L("BatchNoAlreadyUseByOtherItem") + $" {i.BatchNumber}");
                        if (expriationItemHash.Contains(item.ItemId.Value))
                        {
                            if (batchNoDic[i.BatchNumber].ExpirationDate.Value.Date != i.ExpirationDate.Value.Date) throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {i.ExpirationDate.Value.ToString("dd-MM-yyyy")}");
                        }

                        i.BatchNoId = batchNoDic[i.BatchNumber].Id;
                    }
                    else
                    {
                        var batchNo = BatchNo.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, i.BatchNumber, item.ItemId.Value, i.IsStandard, serialItemHash.Contains(item.ItemId.Value), i.ExpirationDate);
                        i.BatchNoId = batchNo.Id;
                        addBatchNos.Add(batchNo);
                        batchNoDic.Add(i.BatchNumber, batchNo);
                    }
                }
            }

            var duplicateBatch = useBatchItems
                             .SelectMany(s => s.ItemBatchNos.Select(r => new { r.BatchNumber, s.ItemId }))
                             .GroupBy(s => s.BatchNumber)
                             .Select(g => g.Select(s => new { s.ItemId, s.BatchNumber }).Distinct())
                             .Where(s => s.Count() > 1)
                             .FirstOrDefault();
            if (duplicateBatch != null && duplicateBatch.Any()) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $": {duplicateBatch.First().BatchNumber}, Item : {useBatchItemDic[duplicateBatch.First().ItemId.Value].ItemCode}-{useBatchItemDic[duplicateBatch.First().ItemId.Value].ItemName}"));

            if (addBatchNos.Any()) await _batchNoRepository.BulkInsertAsync(addBatchNos);
        }

        private async Task ValidateOrderQtyForReceipt(Guid OrderItemId, decimal qty, Guid? itemReceiptId)
        {
            var saleOrderItem = await _purhcaseOrderItemRepository.GetAll()
                                       .Include(s => s.Item)
                                       .Include(s => s.ItemReceiptItems)
                                       .Include(s => s.BillItems)
                                       .Where(s => s.Id == OrderItemId)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync();

            var invoiceDeliveryQty = saleOrderItem.BillItems.Sum(s => s.Qty);
            var issueDeliveryQty = saleOrderItem.ItemReceiptItems.Where(s => s.ItemReceiptId != itemReceiptId).Sum(s => s.Qty);
            if (saleOrderItem.Unit - invoiceDeliveryQty - issueDeliveryQty + qty < 0)
                throw new UserFriendlyException(L("InvoiceMessageWarning", saleOrderItem.Item.ItemName));
        }

        private async Task CalculateTotal(CreateBillInput input)
        {
            #region Calculation Formular from Front End
            //this.model.subTotal = 0;
            //this.model.tax = 0;
            //for (let i of this.itemList)
            //{
            //    let subTotal = this.trimRoundValue(i.qty * i.unitCost);
            //    let totaldiscount = this.trimRoundValue((subTotal * i.discountRate) / 100);

            //    i.total = this.trimRoundValue(subTotal - totaldiscount);// get total of item
            //    let taxRate = i.tax ? i.tax.taxRate : 0;
            //    let totalTaxRate = this.trimRoundValue(i.total * taxRate);//get total tax of item

            //    this.model.subTotal += i.total;
            //    this.model.tax += totalTaxRate;
            //}
            //this.model.total = this.model.subTotal + this.model.tax;
            #endregion

            var round = await GetCurrentCycleAsync();
            var taxs = await _taxRepository.GetAll().AsNoTracking().ToListAsync();

            decimal subTotal = 0;
            decimal totalTax = 0;
            foreach (var item in input.BillItems)
            {
                item.UnitCost = Math.Round(item.UnitCost, round.RoundingDigitUnitCost);

                var lineCost = Math.Round(item.Qty * item.UnitCost, round.RoundingDigit);
                var discount = Math.Round(lineCost * item.DiscountRate / 100, round.RoundingDigit);

                item.Total = Math.Round(lineCost - discount, round.RoundingDigit);

                var tax = taxs.FirstOrDefault(s => s.Id == item.TaxId);

                var taxRate = tax == null ? 0 : tax.TaxRate;
                var taxAmount = Math.Round(item.Total * taxRate, round.RoundingDigit);

                subTotal += item.Total;
                totalTax += taxAmount;
            }

            input.SubTotal = subTotal;
            input.Tax = totalTax;
            input.Total = input.SubTotal + input.Tax;
        }

        void ValidateExchangeRate(CreateBillInput input)
        {
            if (!input.UseExchangeRate || input.CurrencyId == input.MultiCurrencyId) return;

            if (input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));
        }

        /* Functionality for check if item or can check it a list of array is account tap from Frontend */

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateBillInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                .Where(t => (t.LockKey == TransactionLockType.Bill)
                                && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (input.BillItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.ConvertToItemReceipt || input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt) await ValidateAddBatchNo(input);

            //validation Qty on hand form PO
            if (input.ReceiveFrom == Bill.ReceiveFromStatus.PO)
            {
                var validates = input.BillItems.Where(t => t.OrderItemId != null).Select(t => new { ItemName = t.ItemName, OriginalQty = t.OrginalQtyFromPurchase, OrderItemId = t.OrderItemId }).GroupBy(t => t.OrderItemId).ToList();
                foreach (var v in validates)
                {
                    var sumQtyByOrderItems = input.BillItems.Where(g => g.OrderItemId == v.Key).Sum(t => t.Qty);
                    if (sumQtyByOrderItems > v.FirstOrDefault().OriginalQty)
                    {
                        throw new UserFriendlyException(L("BillMessageWarning", v.First().ItemName));
                    }
                }
            }
            else if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt)
            {
                foreach (var v in input.BillItems.Where(s => s.OrderItemId.HasValue))
                {
                    var sumQtyByOrderItems = input.BillItems.Where(g => g.OrderItemId == v.OrderItemId).Sum(t => t.Qty);
                    await ValidateOrderQtyForReceipt(v.OrderItemId.Value, sumQtyByOrderItems, v.ItemReceiptId);
                }
            }

            int indexlot = 0;
            foreach (var i in input.BillItems)
            {
                indexlot++;
                if (i.LotId == null && i.ItemId != null && i.ItemTypeId != null && i.DisplayInventoryAccount)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexlot.ToString());
                }
            }

            ValidateExchangeRate(input);
            await CalculateTotal(input);

            //validation Qty From PO

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            #region Update Sequence
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.BillNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            #endregion

            var billItemIds = input.BillItems.Where(u => u.ItemId != null).Select(u => u.ItemId.Value);

            #region insert to journal for bill

            //create journal for bill
            var @entity = Journal.Create(tenantId, userId, input.BillNo, input.Date,
                                      input.Memo, input.Total, input.Total, input.CurrencyId,
                                      input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            entity.UpdateMultiCurrency(input.MultiCurrencyId);

            //insert clearance journal item into credit (header)
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo, 0, input.Total, PostingKey.AP, null);

            var isItem = input.BillItems.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            //create bill          
            var bill = Bill.Create(tenantId, userId, input.ReceiveFrom, input.DueDate, input.VendorId,
                                        // input.LocationId
                                        input.SameAsShippingAddress, input.ShippingAddress,
                                        input.BillingAddress, input.SubTotal, input.Tax, input.Total,
                                        input.ItemReceiptId, input.ETA, input.ConvertToItemReceipt, input.RecieptDate,
                                        input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal, isItem, input.UseExchangeRate);


            #endregion

            var itemReceiptItems = new List<ItemReceiptItem>();
            var journalItemForReceiptItems = new List<JournalItem>();

            //update journal of itemReceipt 
            if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt)
            {
                if (input.ItemReceiptId == null)
                {
                    throw new UserFriendlyException(L("PleaseAddItemReceipt"));
                }
                // check receive from is item receipt set automatically received status to full
                bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);

                itemReceiptItems = await
                    _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem).Where(u => u.ItemReceiptId == input.ItemReceiptId).ToListAsync();


                journalItemForReceiptItems = await _journalItemRepository.GetAll()
                                       .Include(u => u.Journal.ItemReceipt)
                                       .Where(t => t.Key == PostingKey.Inventory && t.Identifier != null)
                                       //.Where(u => u.Identifier == i.ItemReceiptId)
                                       .Where(u => u.Journal.ItemReceiptId == input.ItemReceiptId).ToListAsync();

            }
            else if (input.ReceiveFrom == Bill.ReceiveFromStatus.None &&
                billItemIds.Count() == 0) //check for when receive from none and from tab account 
            {
                //Check if bill list item has no item ID so it mean no receive status 
                bill.UpdateReceivedStatus(DeliveryStatus.NoReceive);
            }
            @entity.UpdateBill(@bill);
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _billManager.CreateAsync(@bill));


            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = BillExchangeRate.Create(tenantId, userId, bill.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }

            var subBillItems = new List<KeyValuePair<CreateOrUpdateBillItemInput, BillItem>>();

            foreach (var i in input.BillItems)
            {

                if (i.LotId == 0)
                {
                    i.LotId = null;
                }

                //validate when Currency as Multi Currrency

                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    i.MultiCurrencyTotal = i.Total;
                    i.MultiCurrencyUnitCost = i.UnitCost;
                }

                //insert to bill item
                var @billItem = BillItem.Create(tenantId, userId, bill,
                        i.TaxId, i.ItemId, i.Description, i.Qty,
                        i.UnitCost, i.DiscountRate, i.Total, i.MultiCurrencyUnitCost, i.MultiCurrencyTotal);

                //Update lotId  in table bill Item
                billItem.UpdateLot(i.LotId);

                // Assign bill item id to input of bill item 
                i.Id = billItem.Id;
                if (i.ItemReceiptId != null)
                {
                    billItem.UpdateIsReceipt(true);
                }
                if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt && i.ItemReceiptId != null)
                {
                    billItem.UpdateReceiptItemId(i.ItemReceiptId);
                }
                //Update billItemid when click on ItemReceipt             
                //if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt && input.ItemReceiptId != null)
                //{
                //    if (i.ItemReceiptId != null)
                //    {
                //        if(i.OrderItemId != null)
                //        {
                //            billItem.UpdateOrderItemId(i.OrderItemId);
                //        }
                //        billItem.UpdateReceiptItemId(i.ItemReceiptId.Value);
                //        //validate qty 
                //        var itemReceiptItem = itemReceiptItems.FirstOrDefault(u => u.Id == i.ItemReceiptId);
                //        itemReceiptItem.UpdateItemReceiptItem(i.UnitCost, i.DiscountRate, i.Total);
                //        itemReceiptItem.UpdateQty(i.Qty);                       
                //        itemReceiptItem.UpdateLot(i.LotId);                        
                //        var ji = journalItemForReceiptItems.FirstOrDefault(u => u.Identifier == i.ItemReceiptId);
                //        ji.UpdateJournalItemDebitCredit(i.Total, 0);

                //    }
                //}
                if (input.ReceiveFrom == Bill.ReceiveFromStatus.PO && i.OrderItemId != null)
                {
                    billItem.UpdateOrderItemId(i.OrderItemId);
                }
                CheckErrors(await _billItemManager.CreateAsync(billItem));

                if (i.Key.HasValue || i.ParentId.HasValue) subBillItems.Add(new KeyValuePair<CreateOrUpdateBillItemInput, BillItem>(i, billItem));

                //insert inventory journal item into debit
                var inventoryJournalItem =
                    JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId, i.Description, i.Total, 0, PostingKey.Clearance, billItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

            }

            var keys = input.BillItems.Where(s => s.Key.HasValue).GroupBy(s => s.Key.Value).Select(s => s.Key).ToHashSet();
            foreach (var key in keys)
            {
                var parent = subBillItems.FirstOrDefault(s => s.Key.Key == key);
                var subitems = subBillItems.Where(s => s.Key.ParentId == key).Select(s => s.Value);
                foreach (var sub in subitems)
                {
                    sub.SetParent(parent.Value.Id);
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Bill };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.ConvertToItemReceipt == true && input.Status == TransactionStatus.Publish)
            {

                var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.TransitAccountId).FirstOrDefaultAsync();

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                }
                var inventoryAccount = await _itemRepository.GetAll()
                                            .Where(t => billItemIds.Contains(t.Id) &&
                                                        t.InventoryAccountId != null &&
                                                        t.ItemType.DisplayInventoryAccount)
                                            .AsNoTracking()
                                            .Select(u => new { u.Id, InventoryAccountId = u.InventoryAccountId.Value })
                                            .ToDictionaryAsync(u => u.Id);

                var itemLists = input.BillItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId.Value))
                    .Select(t => new CreateOrUpdateItemReceiptItemInput
                    {
                        InventoryAccountId = inventoryAccount[t.ItemId.Value].InventoryAccountId,
                        Description = t.Description,
                        DiscountRate = t.DiscountRate,
                        ItemId = t.ItemId.Value,
                        //OrderItemId = t.OrderItemId,
                        //PurchaseOrderId = t.PurchaseOrderId,
                        Qty = t.Qty,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        BillItemId = t.Id,
                        Id = t.ItemReceiptId,
                        LotId = t.LotId,
                        UseBatchNo = t.UseBatchNo,
                        ItemBatchNos = t.ItemBatchNos,
                    }).ToList();
                var createInputItemREceipt = new CreateItemReceiptInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.BillNo,
                    BillId = bill.Id,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.RecieptDate,
                    LocationId = input.LocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.Bill,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    Items = itemLists,

                };
                await CreateItemReceipt(createInputItemREceipt);
            }

            else if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt && input.ItemReceiptId != null)
            {

                var inventoryAccount = await _itemRepository.GetAll()
                                            .Where(t => billItemIds.Contains(t.Id) &&
                                                        t.InventoryAccountId != null &&
                                                        t.ItemType.DisplayInventoryAccount)
                                            .AsNoTracking()
                                            .Select(u => new { u.Id, InventoryAccountId = u.InventoryAccountId.Value })
                                            .ToDictionaryAsync(u => u.Id);

                var itemLists = input.BillItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId.Value))
                    .Select(t => new CreateOrUpdateItemReceiptItemInput
                    {
                        InventoryAccountId = inventoryAccount[t.ItemId.Value].InventoryAccountId,
                        Description = t.Description,
                        DiscountRate = t.DiscountRate,
                        ItemId = t.ItemId.Value,
                        OrderItemId = t.OrderItemId,
                        PurchaseOrderId = t.PurchaseOrderId,
                        Qty = t.Qty,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        BillItemId = t.Id,
                        Id = t.ItemReceiptId,
                        LotId = t.LotId,
                        UseBatchNo = t.UseBatchNo,
                        ItemBatchNos = t.ItemBatchNos,
                    }).ToList();

                var itemreceipt = await _itemReceiptRepository.GetAll().Where(t => t.Id == input.ItemReceiptId).FirstOrDefaultAsync();
                var inputItemReceipt = new UpdateItemReceiptInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.BillNo,
                    BillId = bill.Id,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.RecieptDate,
                    LocationId = input.LocationId,
                    ReceiveFrom = itemreceipt.ReceiveFrom,
                    Reference = input.Reference,
                    Status = input.Status,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    Items = itemLists,
                    Id = input.ItemReceiptId.Value
                };

                var itemReceiptJournal = journalItemForReceiptItems.Select(u => u.Journal).FirstOrDefault();
                await UpdateItemReceipt(tenantId, userId, @bill.Id,
                    inputItemReceipt, false, itemReceiptItems, itemReceiptJournal, journalItemForReceiptItems);
            }


            var orders = input.BillItems.Where(s => s.PurchaseOrderId.HasValue).GroupBy(s => s.PurchaseOrderId).Select(s => s.Key.Value);

            if (orders.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var orderId in orders)
                {

                    await UpdatePurhaseOrderInventoryStatus(orderId);
                }
            }


            return new NullableIdDto<Guid>() { Id = bill.Id };
        }


        private async Task CreateItemReceipt(CreateItemReceiptInput input)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                   && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.ReceiptNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            input.Total = input.Items.Sum(t => t.Total);
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity =
                Journal.Create(tenantId, userId, input.ReceiptNo, input.Date,
                input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);

            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptPurchase);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion

            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo, 0, input.Total, PostingKey.Clearance, null);

            //insert to item Receipt
            var @itemReceipt = ItemReceipt.Create(tenantId, userId, input.ReceiveFrom, input.VendorId,
                input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress,
                input.Total, null);

            itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptPurchase);
            @entity.UpdateItemReceipt(@itemReceipt);
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && input.BillId != null)
            {
                var bill = await _billRepository.GetAll().Where(u => u.Id == input.BillId).FirstOrDefaultAsync();
                bill.UpdateItemReceiptid(itemReceipt.Id);

                // update received status in bill to full when receive from bill 
                if (input.Status == TransactionStatus.Publish)
                {
                    bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
                }
                CheckErrors(await _billManager.UpdateAsync(bill));
            }
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemReceiptManager.CreateAsync(@itemReceipt));

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.None && input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            await UpdatePOReceiptStautus(null, input.ReceiveFrom, input.BillId, input.Items);

            var ItemReceiptItems = (from ItemReceiptitem in input.Items where (ItemReceiptitem.OrderItemId != null) select ItemReceiptitem.OrderItemId);
            var count = ItemReceiptItems.Count();
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && count <= 0)
            {
                throw new UserFriendlyException(L("PleaseAddPuchaseOrder"));
            }

            ////select POId from Bill 
            //var billItems = await _billItemRepository.GetAll()
            //   .Include(u => u.OrderItem)
            //   .Include(u => u.OrderItem.PurchaseOrder)
            //   .Where(u => u.BillId == input.BillId && u.OrderItemId != null
            //               && u.OrderItem.PurchaseOrderId == u.OrderItem.PurchaseOrder.Id)
            //               .Select(u => new
            //               {
            //                   orderItemId = u.OrderItemId,
            //                   soId = u.OrderItem.PurchaseOrderId,
            //                  // sumTotalQty = u.OrderItem.TotalReceiptBillQty + u.OrderItem.TotalReceiptQty,
            //                   OriginalTotalQty = u.OrderItem.Unit,
            //                   qty = u.Qty
            //               }).ToListAsync();   

            foreach (var i in input.Items)
            {

                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, i.ItemId,
                                                                             i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);

                if (i.OrderItemId.HasValue) itemReceiptItem.UpdateOrderItemId(i.OrderItemId);

                //Update lotId in table item Receipt item 
                itemReceiptItem.UpdateLot(i.LotId);
                if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && i.OrderItemId != null)
                {
                    itemReceiptItem.UpdateOrderItemId(i.OrderItemId);
                }
                CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));

                //Update billItemid when click on BIll in table billItem
                if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
                {
                    //if(i.OrderItemId != null)
                    //{
                    //    itemReceiptItem.UpdateOrderItemId(i.OrderItemId);
                    //}
                    var billItem = await _billItemRepository.GetAll()
                         .Where(u => u.Id == i.BillItemId).FirstOrDefaultAsync();
                    billItem.UpdateReceiptItemId(itemReceiptItem.Id);
                    //update lot Id in table bill item 
                    billItem.UpdateLot(i.LotId);
                    CheckErrors(await _billItemManager.UpdateAsync(billItem));

                    if (i.Qty > billItem.Qty)
                    {
                        throw new UserFriendlyException(L("PleaseCheckYourQty"));
                    }
                }

                //insert inventory journal item into debit
                var inventoryJournalItem =
                    JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId, i.Description, i.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceipt(itemReceipt.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, input.Date, scheduleItems);
            }

        }

        private async Task UpdateItemReceipt(
                                            int tenantId,
                                            long userId,
                                            Guid billId,
                                            UpdateItemReceiptInput input, bool autoConvert,
                                            List<ItemReceiptItem> itemReceipItems,
                                            Journal @journal,
                                            List<JournalItem> inventoryJournalItems)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                var isChangeItems = input.Items.Count() != itemReceipItems.Count() || input.Items.Any(s => itemReceipItems.Any(r => r.Id == s.Id && (r.ItemId != s.ItemId || r.LotId != s.LotId || r.Qty != s.Qty)));

                if (validateLockDate > 0 && (input.Date.Date != journal.Date.Date || isChangeItems || input.LocationId != journal.LocationId))
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            input.Total = input.Items.Sum(t => t.Total);

            // update Journal 

            if (@journal == null || @journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }


            var itemReceiptSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            if (itemReceiptSequence.CustomFormat == true)
            {
                input.ReceiptNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.UpdateStatus(input.Status);


            var billItems = await _billItemRepository.GetAll().Where(u => u.BillId == billId).ToListAsync();

            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                        .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Clearance && u.Identifier == null))
                                        .FirstOrDefaultAsync();
            if (autoConvert == true)
            {

                clearanceAccountItem.UpdateJournalItem(userId, input.ClearanceAccountId, input.Memo, 0, input.Total);
                CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
                journal.Update(userId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total,
                                input.CurrencyId, input.ClassId, input.Reference, input.Status, input.LocationId);
            }
            else
            {

                var setting = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == BillInvoiceSettingType.Bill);

                if (setting == null || setting.ReferenceSameAsGoodsMovement)
                {
                    journal.Update(userId, input.Total, input.Total, input.Date,
                           input.ClassId, input.Reference, input.LocationId);
                }
                else
                {
                    journal.Update(userId, input.Total, input.Total, input.Date,
                           input.ClassId, input.LocationId);
                }

                clearanceAccountItem.SetDebitCredit(0, input.Total);
                CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            }

            //update item receipt 
            var @itemReceipt = @journal.ItemReceipt;// await _itemReceiptManager.GetAsync(input.Id, true);

            await UpdatePOReceiptStautus(input.Id, @itemReceipt.ReceiveFrom, input.BillId, input.Items);

            @itemReceipt.Update(userId, input.ReceiveFrom, input.VendorId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null);



            CheckErrors(await _journalManager.UpdateAsync(@journal, itemReceiptSequence.DocumentType));

            CheckErrors(await _itemReceiptManager.UpdateAsync(@itemReceipt));

            var toDeleteItemReceiptItem = itemReceipItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            //delete validate when use in vendorcredit and itemIssueVendorCredit
            var vendorCredit = await _vendorCreditDetailRepository.GetAll()
                .Include(u => u.ItemReceiptItem)
                .Where(t => (toDeleteItemReceiptItem.Any(g => g.Id == t.ItemReceiptItemId)) ||
                (input.Items.Any(g => g.Id != null && g.Id == t.ItemReceiptItemId && t.ItemReceiptItem != null && (g.Qty != t.ItemReceiptItem.Qty || g.LotId != t.ItemReceiptItem.LotId)))).AsNoTracking().CountAsync();
            var itemIssueVendorCredit = await _itemIssueVendorCreditItemRepository.GetAll()
                .Include(u => u.ItemReceiptPurchaseItem)
                .Where(t => (toDeleteItemReceiptItem.Any(g => g.Id == t.ItemReceiptPurchaseItemId)) ||
                (input.Items.Any(g => g.Id != null && g.Id == t.ItemReceiptPurchaseItemId && t.ItemReceiptPurchaseItem != null && (g.Qty != t.ItemReceiptPurchaseItem.Qty || g.LotId != t.ItemReceiptPurchaseItem.LotId)))).AsNoTracking().CountAsync();

            if (vendorCredit > 0 || itemIssueVendorCredit > 0)
            {
                throw new UserFriendlyException(L("ItemAlreadyReturnCannotBeModified"));
            }

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.None && input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var itemBatchs = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptItemId)).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchs.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptItemId)).ToList();
                if (deleteItemBatchNos.Any())
                {
                    var batchs = deleteItemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                    if (batchs.Any()) batchNos.AddRange(batchs);

                    await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                }
            }


            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemReceipItem = itemReceipItems.FirstOrDefault(u => u.Id == c.Id);

                    if (input.ReceiveFrom != ItemReceipt.ReceiveFromStatus.None && c.OrderItemId != null)
                    {
                        itemReceipItem.UpdateOrderItemId(c.OrderItemId);
                    }
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(userId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        //update lot id in table item Receipt Item 
                        itemReceipItem.UpdateLot(c.LotId);

                        if (c.OrderItemId.HasValue)
                        {
                            itemReceipItem.UpdateOrderItemId(c.OrderItemId);
                        }
                        else
                        {
                            itemReceipItem.UpdateOrderItemId(null);
                        }


                        CheckErrors(await _itemReceiptItemManager.UpdateAsync(itemReceipItem));

                    }

                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, c.Total, 0);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchs.Where(s => s.ItemReceiptItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newItemBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            //in case of change date and item is auto generate it will change to use new batch so delete old one if not use
                            if (oldItemBatch.BatchNoId != newItemBatch.BatchNoId && !batchNos.Any(b => b.Id == oldItemBatch.BatchNoId)) batchNos.Add(oldItemBatch.BatchNo);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newItemBatch.BatchNoId, newItemBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            var batchs = deleteItemBatchNos.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                            if (batchs.Any()) batchNos.AddRange(batchs);
                            await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        var batchs = oldItemBatchs.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                        if (batchs.Any()) batchNos.AddRange(batchs);

                        await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }
                }
                else if (c.Id == null) //create
                {

                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, c.ItemId,
                                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);

                    if (input.ReceiveFrom != ItemReceipt.ReceiveFromStatus.None && c.OrderItemId != null)
                    {
                        @itemReceiptItem.UpdateOrderItemId(c.OrderItemId);
                    }
                    // update lot id 
                    itemReceiptItem.UpdateLot(c.LotId);

                    if (c.OrderItemId.HasValue) @itemReceiptItem.UpdateOrderItemId(c.OrderItemId);


                    //Update table bill and bill item

                    var billItem = billItems.Where(u => u.Id == c.BillItemId).FirstOrDefault();
                    billItem.UpdateIsReceipt(true);
                    billItem.UpdateReceiptItemId(@itemReceiptItem.Id);
                    billItem.UpdateLot(c.LotId);
                    itemReceiptItem.UpdateLot(c.LotId);

                    CheckErrors(await _billItemManager.UpdateAsync(billItem));
                    CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));
                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.InventoryAccountId, c.Description, c.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                        }
                    }

                }

            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemReceiptItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            //need save change before performing bellow task
            await CurrentUnitOfWork.SaveChangesAsync();

            if (batchNos.Any())
            {
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }

            await SyncItemReceipt(itemReceipt.Id);

            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(tenantId, userId, scheduleDate, scheduleItems);
            }

        }

        private async Task DeleteItemReceipt(CarlEntityDto input)
        {

            //validate when use in vendorcredit and itemIssueVendorCredit
            var vendorCredit = await _vendorCreditRepository.GetAll().Where(t => t.ItemReceiptId == input.Id).CountAsync();
            var itemIssueVendorCredit = await _itemIssueVendorCreditRepository.GetAll().Where(t => t.ItemReceiptPurchaseId == input.Id).CountAsync();

            if (vendorCredit > 0 || itemIssueVendorCredit > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChangeInVendorCredit"));
            }
            var journal = await _journalRepository
                                 .GetAll()
                                 .Include(u => u.ItemReceipt)
                                 .Include(u => u.ItemReceipt.BillingAddress)
                                 .Include(u => u.ItemReceipt.ShippingAddress)
                                 .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                     u.JournalType == JournalType.ItemReceiptAdjustment ||
                                     u.JournalType == JournalType.ItemReceiptOther ||
                                     u.JournalType == JournalType.ItemReceiptTransfer ||
                                     u.JournalType == JournalType.ItemReceiptProduction)
                                     && u.ItemReceipt.Id == input.Id)
                                 .FirstOrDefaultAsync();

            //query get item Receipt

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                  && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = journal.ItemReceipt;

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != journal.Id &&
                                     (t.JournalType == JournalType.ItemReceiptPurchase ||
                                     t.JournalType == JournalType.ItemReceiptAdjustment ||
                                     t.JournalType == JournalType.ItemReceiptOther ||
                                     t.JournalType == JournalType.ItemReceiptTransfer ||
                                     t.JournalType == JournalType.ItemReceiptProduction))
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (jo != null)
                {
                    auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
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
            //qurey check creationTime Item Receipt and Bill
            var UpdateItemReceiptIdOnBill = await _billRepository.GetAll()
                                          .Include(u => u.ItemReceipt)
                                          .Where(u => u.ItemReceiptId == input.Id).FirstOrDefaultAsync();

            if (UpdateItemReceiptIdOnBill != null)
            {
                UpdateItemReceiptIdOnBill.UpdateItemReceiptid(null);
                UpdateItemReceiptIdOnBill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                CheckErrors(await _billManager.UpdateAsync(UpdateItemReceiptIdOnBill));
            }
            var @itemReceipts = (from itemReceipt in _itemReceiptRepository.GetAll()
                                 .Where(u => u.Id == input.Id)
                                 join bill in _billRepository.GetAll() on itemReceipt.Id equals bill.ItemReceiptId
                                 where (itemReceipt.CreationTime < bill.CreationTime)
                                 select itemReceipt);




            if (itemReceipts != null && itemReceipts.Count() > 0)
            {
                throw new UserFriendlyException(L("ItemReceiptMessage"));
            }

            Guid? billId = null;
            if (UpdateItemReceiptIdOnBill != null)
            {
                billId = UpdateItemReceiptIdOnBill.Id;
            }

            await UpdatePOReceiptStautus(input.Id, entity.ReceiveFrom, billId, null);

            //query get journal and delete

            journal.UpdateItemReceipt(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = journal.Date;

            CheckErrors(await _journalManager.RemoveAsync(journal));

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => s.ItemReceiptItem.ItemReceiptId == input.Id).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (itemBatchNos.Any())
            {
                batchNos = itemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt item and delete 
            var @itemReceiptItems = await _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem)
                .Where(u => u.ItemReceiptId == entity.Id).ToListAsync();

            //Update itemReceiptitemId on table billitem
            var updateitemReceiptitemId = (from billitem in _billItemRepository.GetAll()
                                           join itemReceiptItem in _itemReceiptItemRepository.GetAll()
                                           .Include(u => u.ItemReceipt)
                                           on billitem.ItemReceiptItemId equals itemReceiptItem.Id
                                           where (itemReceiptItem.ItemReceiptId == entity.Id)
                                           select billitem);
            foreach (var u in updateitemReceiptitemId)
            {
                u.UpdateReceiptItemId(null);
                CheckErrors(await _billItemManager.UpdateAsync(u));
            }

            var scheduleItems = itemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in @itemReceiptItems)
            {

                CheckErrors(await _itemReceiptItemManager.RemoveAsync(iri));

            }

            CheckErrors(await _itemReceiptManager.RemoveAsync(entity));

            if (batchNos.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            // validate from Paybill
            var validatefrompayBill = await _payBillDetailRepository.GetAll().AsNoTracking()
                                       .Where(t => t.BillId == input.Id).CountAsync();
            if (validatefrompayBill > 0)
            {
                throw new UserFriendlyException(L("RecordHasPayBill"));
            }

            var journal = await _journalRepository.GetAll()
                                .Include(u => u.Bill)
                                .Include(u => u.Bill.ItemReceipt)
                                .Include(u => u.Bill.ShippingAddress)
                                .Include(u => u.Bill.BillingAddress)
                                .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
                                .FirstOrDefaultAsync();

            if (journal == null || journal.Bill == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.Bill)
                   && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            //query get bill
            var @entity = journal.Bill;


            if (entity.ItemReceiptId != null && @entity.ConvertToItemReceipt == true)
            {
                var inputItemReceipt = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = entity.ItemReceiptId.Value };
                await DeleteItemReceipt(inputItemReceipt);
                //update item issue id to null after delete item issue from invoice 
                entity.UpdateItemReceiptid(null);
            }
            else if (entity.ItemReceipt != null && entity.CreationTime < entity.ItemReceipt.CreationTime)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.Bill)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (jo != null)
                {
                    auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            journal.UpdateBill(null);

            //if (entity.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt)
            //{
            //    // start query to delete item receipt item table if receive from itemReceipt which has add more record into it 
            //    var @itemReceiptQuery = (from billItem in _billItemRepository.GetAll()
            //                            .Where(u => u.BillId == input.Id && u.IsItemReceipt == false)
            //                             join itemReceiptItem in _itemReceiptItemRepository.GetAll()
            //                             on billItem.ItemReceiptItemId equals itemReceiptItem.Id
            //                             select itemReceiptItem);
            //    foreach (var ir in @itemReceiptQuery)
            //    {
            //        //query item receipt to delete by match with identifier with item receipt item id
            //        var @jounalItemReceipt = await _journalItemRepository.GetAll().Where(u => u.Identifier == ir.Id).FirstOrDefaultAsync();
            //        CheckErrors(await _journalItemManager.RemoveAsync(jounalItemReceipt));
            //        CheckErrors(await _itemReceiptItemManager.RemoveAsync(ir));
            //    }
            //}

            //query get bill item and delete 
            var billItems = await _billItemRepository.GetAll()
                .Include(u => u.OrderItem)
                .Include(s => s.ItemReceiptItem.OrderItem)
                .Include(s => s.ItemReceiptItem.ItemReceipt)
                .Where(u => u.BillId == entity.Id).ToListAsync();

            var billOrderIds = billItems
                               .Where(s => s.OrderItemId.HasValue)
                               .GroupBy(s => s.OrderItem.PurchaseOrderId)
                               .Select(s => s.Key)
            .ToList();


            var receiveOrderIds = new List<Guid>();
            var receiveLinkItems = billItems
                                .Where(s => s.ItemReceiptItem != null && s.ItemReceiptItem.OrderItem != null)
                                .Select(s => s.ItemReceiptItem)
                                .ToList();

            receiveOrderIds = receiveLinkItems
                        .GroupBy(s => s.OrderItem.PurchaseOrderId)
                        .Select(s => s.Key)
                        .ToList();

            foreach (var link in receiveLinkItems)
            {
                if (link.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.None)
                {
                    link.UpdateOrderItemId(null);
                    await _itemReceiptItemManager.UpdateAsync(link);
                }
            }

            foreach (var bi in billItems)
            {
                if (bi.OrderItemId != null && bi.OrderItem != null)
                {
                    //Update Purchase Order item when delete Item Receipt 
                    bi.UpdateOrderItemId(null);
                    //CheckErrors(await _purhcaserOrderItemManager.UpdateAsync(bi.OrderItem));
                }

                CheckErrors(await _billItemManager.RemoveAsync(bi));

            }

            var exchangeRates = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.BillId == input.Id).ToListAsync();
            if (exchangeRates.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchangeRates);
            
            CheckErrors(await _billManager.RemoveAsync(entity));

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(journal));


            var orderIds = billOrderIds.Union(receiveOrderIds).ToList();
            if (orderIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var orderId in orderIds)
                {
                    await UpdatePurhaseOrderInventoryStatus(orderId);
                }
            }


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Bill };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_MultiDelete)]
        [UnitOfWork(IsDisabled = true)]
        public async Task MultiDelete(GetListDeleteInput input)
        {
            var tenantId = AbpSession.TenantId;
         
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var journals = await _journalRepository.GetAll()
                                         .Include(s => s.Bill.ItemReceipt)
                                         .Include(s => s.VendorCredit)
                                         .AsNoTracking()
                                         .Where(u => u.JournalType == JournalType.Bill || u.JournalType == JournalType.VendorCredit)
                                         .Where(u => input.Ids.Contains(u.BillId.Value) || input.Ids.Contains(u.VendorCreditId.Value))
                                         .OrderByDescending(s => s.JournalNo)
                                         .ToListAsync();

                    if (journals.IsNullOrEmpty()) throw new UserFriendlyException(L("RecordNotFound"));
                 
                    var lockDate = journals.OrderByDescending(t => t.Date).Select(t => t.Date).FirstOrDefault();
                    var locktransaction = await _lockRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(t => t.LockKey == TransactionLockType.Bill && t.IsLock == true && t.LockDate.Value.Date >= lockDate.Date)
                                                .AnyAsync();
                    if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                
                    var @closePeroid = await _accountCycleRepository.GetAll().AsNoTracking().AnyAsync(t => t.EndDate != null && lockDate.Date > t.EndDate.Value.Date);
                    if (closePeroid) throw new UserFriendlyException(L("PeriodIsClose"));
                  
                    // validate from Paybill
                    var hasPayment = await _payBillDetailRepository.GetAll()
                                           .AsNoTracking()
                                           .AnyAsync(t => input.Ids.Contains(t.PayToId.Value));


                    var validateBill = journals.Where(t => t.Bill != null && t.Bill.ItemReceipt != null && t.Bill.CreationTime < t.Bill.ItemReceipt.CreationTime && !t.Bill.ConvertToItemReceipt).Any();                      
                    if (validateBill)
                    {
                        throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                    }

                    var _vendorCreditIds = journals.Where(t => t.VendorCredit != null && !t.VendorCredit.ConvertToItemIssueVendor).Select(t => t.VendorCreditId).ToList();

                    if (_vendorCreditIds != null && _vendorCreditIds.Count() > 0)
                    {
                       
                        var validateVendorCredit = await _itemIssueVendorCreditRepository.GetAll()
                                                   .AsNoTracking()
                                                   .Where(t => t.VendorCreditId != null && t.ReceiveFrom == ReceiveFrom.VendorCredit)
                                                   .Where(t => _vendorCreditIds.Contains(t.VendorCreditId)).AnyAsync();
                        if (validateVendorCredit)
                        {
                            throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                        }
                    }
                  


                    if (hasPayment) throw new UserFriendlyException(L("RecordHasPayBill"));

                    var journalItems = await _journalItemRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(s => journals.Any(j => j.Id == s.JournalId))
                                            .ToListAsync();

                    if (journalItems.Any()) await _journalItemRepository.BulkDeleteAsync(journalItems);

                    var deleteItemIssueVenderCreditIds = new List<Guid>();
                   
                    var vendorCreditJournals = journals.Where(s => s.VendorCredit != null).ToList();
                    if (vendorCreditJournals.Any())
                    {
                        var autoVendorCredit = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.VendorCredit);

                        var lastVnedorCreditJournal = vendorCreditJournals.FirstOrDefault();

                        if (lastVnedorCreditJournal.JournalNo == autoVendorCredit.LastAutoSequenceNumber)
                        {
                            var jo = await _journalRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(t => t.JournalType == JournalType.VendorCredit)
                                            .Where(t => !vendorCreditJournals.Any(r => r.Id == t.Id))
                                            .OrderByDescending(t => t.CreationTime)
                                            .FirstOrDefaultAsync();
                            if (jo != null)
                            {
                                autoVendorCredit.UpdateLastAutoSequenceNumber(jo.JournalNo);
                            }
                            else
                            {
                                autoVendorCredit.UpdateLastAutoSequenceNumber(null);
                            }
                            CheckErrors(await _autoSequenceManager.UpdateAsync(autoVendorCredit));
                        }

                        var vendorCredits = vendorCreditJournals.Select(s => s.VendorCredit).ToList();

                        var vIds = vendorCredits.Select(s => s.Id).ToList();
                        var exchanges = await _vendorCreditExchangeRateRepository.GetAll().AsNoTracking().Where(s => vIds.Contains(s.VendorCreditId)).ToListAsync();
                        if (exchanges.Any()) await _vendorCreditExchangeRateRepository.BulkDeleteAsync(exchanges);


                        var autoConvertIssues = vendorCredits.Where(s => s.ConvertToItemIssueVendor).Select(s => s.Id).ToList();
                        
                        if (autoConvertIssues.Any())
                        {
                            var itemIssueJournals = await _journalRepository.GetAll()
                                                            .Include(s => s.ItemIssueVendorCredit)
                                                            .AsNoTracking()
                                                            .Where(s => s.JournalType == JournalType.ItemIssueVendorCredit)
                                                            .Where(s => autoConvertIssues.Contains(s.ItemIssueVendorCredit.VendorCreditId.Value))
                                                            .OrderByDescending(s => s.JournalNo)
                                                            .ToListAsync();

                            var itemIssueJournalItems = await _journalItemRepository.GetAll().AsNoTracking()
                                                                .Where(s => itemIssueJournals.Any(r => r.Id == s.JournalId))
                                                                .ToListAsync();

                            if (itemIssueJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(itemIssueJournalItems);


                            var autoItemIssue = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);

                            var lastItemIssueJournal = itemIssueJournals.FirstOrDefault();

                            if (lastItemIssueJournal.JournalNo == autoItemIssue.LastAutoSequenceNumber)
                            {
                                var jo = await _journalRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(t => t.JournalType == JournalType.ItemIssueVendorCredit)
                                                .Where(t => !itemIssueJournals.Any(r => r.Id == t.Id))
                                                .OrderByDescending(t => t.CreationTime)
                                                .FirstOrDefaultAsync();
                                if (jo != null)
                                {
                                    autoItemIssue.UpdateLastAutoSequenceNumber(jo.JournalNo);
                                }
                                else
                                {
                                    autoItemIssue.UpdateLastAutoSequenceNumber(null);
                                }
                                CheckErrors(await _autoSequenceManager.UpdateAsync(autoItemIssue));
                            }

                            var itemIssues = itemIssueJournals.Select(s => s.ItemIssueVendorCredit).ToList();

                            if (itemIssueJournals.Any()) await _journalRepository.BulkDeleteAsync(itemIssueJournals);


                            var itemIssueItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                                         .AsNoTracking()
                                                         .Where(s => itemIssues.Any(r => r.Id == s.ItemIssueVendorCreditId))
                                                         .ToListAsync();

                            var itemIssueBatchNos = await _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                                            .AsNoTracking()
                                                            .Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueVendorCreditItemId))
                                                            .ToListAsync();

                            var inventoryTransactionItems = await _inventoryTransactionItemRepository.GetAll()
                                                                  .AsNoTracking()
                                                                  .Where(s => itemIssueItems.Any(r => r.Id == s.Id))
                                                                  .ToListAsync();


                            deleteItemIssueVenderCreditIds = itemIssues.Select(s => s.Id).ToList();

                            if (itemIssueBatchNos.Any()) await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(itemIssueBatchNos);
                            if (inventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(inventoryTransactionItems);
                            if (itemIssueItems.Any()) await _itemIssueVendorCreditItemRepository.BulkDeleteAsync(itemIssueItems);
                            if (itemIssues.Any()) await _itemIssueVendorCreditRepository.BulkDeleteAsync(itemIssues);
                        }

                        var notAutoConvertIssues = vendorCredits.Where(s => !s.ConvertToItemIssueVendor).Select(s => s.Id).ToList();
                        if (notAutoConvertIssues.Any())
                        {
                            var itemIssueVenderCredits = await _itemIssueVendorCreditRepository.GetAll()
                                                               .AsNoTracking()
                                                               .Where(s => s.VendorCreditId.HasValue && notAutoConvertIssues.Contains(s.VendorCreditId.Value))
                                                               .ToListAsync();

                            if (itemIssueVenderCredits.Any())
                            {
                                var itemIssueVendorCreditItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                                                       .AsNoTracking()
                                                                       .Where(s => itemIssueVenderCredits.Any(r => r.Id == s.ItemIssueVendorCreditId))
                                                                       .ToListAsync();

                                foreach(var i in itemIssueVendorCreditItems)
                                {
                                    i.UpdateVendorCreditItemId(null);
                                }

                                foreach(var i in itemIssueVenderCredits)
                                {
                                    i.UpdateVendorCreditId(null);
                                }


                                await _itemIssueVendorCreditItemRepository.BulkUpdateAsync(itemIssueVendorCreditItems);
                                await _itemIssueVendorCreditRepository.BulkUpdateAsync(itemIssueVenderCredits);
                            }
                        }


                        var vendorCreditItems = await _vendorCreditDetailRepository.GetAll()
                                                    .AsNoTracking()
                                                    .Where(s => vendorCredits.Any(r => r.Id == s.VendorCreditId))
                                                    .ToListAsync();

                        var vendorCreditItemBatchNos = await _vendorCreditItemBatchNoRepository.GetAll()
                                                             .AsNoTracking()
                                                             .Where(s => vendorCreditItems.Any(r => r.Id == s.VendorCreditItemId))
                                                             .ToListAsync();
                        if (vendorCreditItemBatchNos.Any()) await _vendorCreditItemBatchNoRepository.BulkDeleteAsync(vendorCreditItemBatchNos);
                        if (vendorCreditItems.Any()) await _vendorCreditDetailRepository.BulkDeleteAsync(vendorCreditItems);

                        await _journalRepository.BulkDeleteAsync(vendorCreditJournals);
                        await _vendorCreditRepository.BulkDeleteAsync(vendorCredits);

                    }


                    var billJournals = journals.Where(s => s.Bill != null).ToList();
                    var purchaseOrderIds = new List<Guid>();
                    if (billJournals.Any())
                    {
                        var autoBill = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);

                        var lastBillJournal = billJournals.FirstOrDefault();
                        
                        if (lastBillJournal != null && lastBillJournal.JournalNo == autoBill.LastAutoSequenceNumber)
                        {
                            var jo = await _journalRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(t => t.JournalType == JournalType.Bill)
                                            .Where(t => !billJournals.Any(r => r.Id == t.Id))
                                            .OrderByDescending(t => t.CreationTime)
                                            .FirstOrDefaultAsync();
                            if (jo != null)
                            {
                                autoBill.UpdateLastAutoSequenceNumber(jo.JournalNo);
                            }
                            else
                            {
                                autoBill.UpdateLastAutoSequenceNumber(null);
                            }
                            CheckErrors(await _autoSequenceManager.UpdateAsync(autoBill));
                        }

                        var billItems = await _billItemRepository.GetAll()
                                       .Include(s => s.OrderItem)
                                       .Include(s => s.ItemReceiptItem.OrderItem)
                                       .AsNoTracking()
                                       .Where(s => billJournals.Any(r => r.BillId.Value == s.BillId))
                                       .ToListAsync();

                        purchaseOrderIds = billItems.Where(s => s.OrderItem != null).GroupBy(s => s.OrderItem.PurchaseOrderId).Select(s => s.Key).ToList();

                        var bills = billJournals.Select(s => s.Bill).ToList();


                        var bIds = bills.Select(s => s.Id).ToList();
                        var exchanges = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => bIds.Contains(s.BillId)).ToListAsync();
                        if (exchanges.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchanges);


                        var autoConvertReceips = bills.Where(s => s.ConvertToItemReceipt && s.ItemReceiptId.HasValue).Select(s => s.ItemReceiptId.Value).ToList();
                        var notAutoConvertReceipts = bills.Where(s => !s.ConvertToItemReceipt && s.ItemReceiptId.HasValue).Select(s => s.ItemReceiptId.Value).ToList();

                        if (notAutoConvertReceipts.Any())
                        {
                            var itemReceiptItemLinkOrders = billItems.Where(s => s.ItemReceiptItem != null)
                                                                      .Where(s => notAutoConvertReceipts.Contains(s.ItemReceiptItem.ItemReceiptId))
                                                                      .Where(s => s.ItemReceiptItem.OrderItem != null)
                                                                      .Select(s => s.ItemReceiptItem);

                            if (itemReceiptItemLinkOrders.Any())
                            {
                                var orderLinkIds = itemReceiptItemLinkOrders.Where(s => !purchaseOrderIds.Contains(s.OrderItem.PurchaseOrderId))
                                                                          .GroupBy(s => s.OrderItem.PurchaseOrderId)
                                                                          .Select(s => s.Key).ToList();
                                if (orderLinkIds.Any()) purchaseOrderIds.AddRange(orderLinkIds);

                                foreach (var link in itemReceiptItemLinkOrders)
                                {
                                    link.UpdateOrderItemId(null);
                                }

                                await _itemReceiptItemRepository.BulkUpdateAsync(itemReceiptItemLinkOrders);
                            }
                        }


                        if (billItems.Any()) await _billItemRepository.BulkDeleteAsync(billItems);

                        await _journalRepository.BulkDeleteAsync(billJournals);
                        await _billRepository.BulkDeleteAsync(bills);

                        if (autoConvertReceips.Any())
                        {
                            var itemReceiptJournals = await _journalRepository.GetAll()
                                                         .Include(s => s.ItemReceipt)
                                                         .AsNoTracking()
                                                         .Where(s => autoConvertReceips.Contains(s.ItemReceiptId.Value))
                                                         .OrderByDescending(s => s.JournalNo)
                                                         .ToListAsync();

                            var itemReceipts = itemReceiptJournals.Select(s => s.ItemReceipt).ToList();
                            var itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                                                        .AsNoTracking()
                                                        .Where(s => itemReceipts.Any(r => r.Id == s.ItemReceiptId))
                                                        .ToListAsync();

                            var itemReceiptHasVendorCredits = await _vendorCreditDetailRepository.GetAll()
                                                                    .AsNoTracking()
                                                                    .WhereIf(!vendorCreditJournals.IsNullOrEmpty(), s => !vendorCreditJournals.Any(r => r.VendorCreditId == s.VendorCreditId))
                                                                    .Where(s => s.ItemReceiptItemId.HasValue && itemReceiptItems.Any(r => r.Id == s.ItemReceiptItemId))
                                                                    .AnyAsync();
                            if (itemReceiptHasVendorCredits) throw new UserFriendlyException(L("InvoiceAlreadyHasReturn"));

                            var itemReceiptHasReutrn = await _itemIssueVendorCreditItemRepository.GetAll()
                                                            .AsNoTracking()
                                                            .WhereIf(!deleteItemIssueVenderCreditIds.IsNullOrEmpty(), s => !deleteItemIssueVenderCreditIds.Contains(s.ItemIssueVendorCreditId))
                                                            .Where(s => s.ItemReceiptPurchaseItemId.HasValue && itemReceiptItems.Any(r => r.Id == s.ItemReceiptPurchaseItemId))
                                                            .AnyAsync();
                            if (itemReceiptHasReutrn) throw new UserFriendlyException(L("InvoiceAlreadyHasReturn"));


                            var autoItemReceipt = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

                            var lastItemReceiptJournal = itemReceiptJournals.FirstOrDefault();

                            if (lastItemReceiptJournal != null && lastItemReceiptJournal.JournalNo == autoItemReceipt.LastAutoSequenceNumber)
                            {
                                var jo = await _journalRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(t => t.JournalType == JournalType.ItemReceiptPurchase ||
                                                            t.JournalType == JournalType.ItemReceiptAdjustment ||
                                                            t.JournalType == JournalType.ItemReceiptOther ||
                                                            t.JournalType == JournalType.ItemReceiptTransfer ||
                                                            t.JournalType == JournalType.ItemReceiptProduction)
                                                .Where(t => !itemReceiptJournals.Any(r => r.Id == t.Id))
                                                .OrderByDescending(t => t.CreationTime)
                                                .FirstOrDefaultAsync();
                                if (jo != null)
                                {
                                    autoItemReceipt.UpdateLastAutoSequenceNumber(jo.JournalNo);
                                }
                                else
                                {
                                    autoItemReceipt.UpdateLastAutoSequenceNumber(null);
                                }
                                CheckErrors(await _autoSequenceManager.UpdateAsync(autoItemReceipt));
                            }

                            var itemReceiptJournalItems = await _journalItemRepository.GetAll().AsNoTracking()
                                                               .Where(s => itemReceiptJournals.Any(r => r.Id == s.JournalId))
                                                               .ToListAsync();

                            if (itemReceiptJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(itemReceiptJournalItems);
                            if (itemReceiptJournals.Any()) await _journalRepository.BulkDeleteAsync(itemReceiptJournals);


                            var itemReceiptBatchNos = await _itemReceiptItemBatchNoRepository.GetAll()
                                                            .AsNoTracking()
                                                            .Where(s => itemReceiptItems.Any(r => r.Id == s.ItemReceiptItemId))
                                                            .ToListAsync();

                            var inventoryTransactionItems = await _inventoryTransactionItemRepository.GetAll()
                                                                  .AsNoTracking()
                                                                  .Where(s => itemReceiptItems.Any(r => r.Id == s.Id)) 
                                                                  .ToListAsync();

                            if (itemReceiptBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(itemReceiptBatchNos);
                            if (inventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(inventoryTransactionItems);
                            if (itemReceiptItems.Any()) await _itemReceiptItemRepository.BulkDeleteAsync(itemReceiptItems);
                            if (itemReceipts.Any()) await _itemReceiptRepository.BulkDeleteAsync(itemReceipts);
                        }

                    }


                    if (purchaseOrderIds.Any()) await UpdatePurhaseOrderInventoryStatus(purchaseOrderIds);

                }

                await uow.CompleteAsync();
            }

        }




        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_Find)]
        public async Task<PagedResultDto<GetListBillOutput>> Find(GetListBillInput input)
        {
            var accountCycle = await GetCurrentCycleAsync();
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.Bill)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.LocationId != null && input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))
                         on jItem.JournalId equals j.Id

                         join bi in _billItemRepository.GetAll().Include(u => u.OrderItem.PurchaseOrder).AsNoTracking()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         on jItem.Identifier equals bi.Id

                         join b in _billRepository.GetAll().Include(t => t.Vendor).AsNoTracking()
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                         on j.BillId equals b.Id

                         group bi by new
                         {
                             Memo = j.Memo,
                             BillId = b.Id,
                             JournalDate = j.Date,
                             JournalNo = j.JournalNo,
                             JournalStatus = j.Status,
                             BillTotal = b.Total,
                             BillDueDate = b.DueDate,
                             BillOpenBalance = b.OpenBalance,
                             BillPaidStatus = b.PaidStatus,
                             BillReceivedStatus = b.ReceivedStatus,
                             VendorId = b.VendorId,
                             VendorCode = b.Vendor.VendorCode,
                             VendorName = b.Vendor.VendorName,
                             VendorAccountId = b.Vendor.AccountId
                             //journal = j, bill = b, vendor = v
                         } into u

                         select new GetListBillOutput
                         {

                             Memo = u.Key.Memo,
                             Id = u.Key.BillId,
                             Date = u.Key.JournalDate,
                             JournalNo = u.Key.JournalNo,
                             Status = u.Key.JournalStatus,
                             Total = u.Key.BillTotal,
                             Vendor = new VendorSummaryOutput
                             {
                                 AccountId = u.Key.VendorAccountId.Value,
                                 Id = u.Key.VendorId,
                                 VendorCode = u.Key.VendorCode,
                                 VendorName = u.Key.VendorName
                             },
                             VendorId = u.Key.VendorId,
                             DueDate = u.Key.BillDueDate,
                             OpenBalance = Math.Round(u.Key.BillOpenBalance, accountCycle.RoundingDigit),
                             PaidStatus = u.Key.BillPaidStatus,
                             ReceivedStatus = u.Key.BillReceivedStatus,
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.PageBy(input).ToListAsync();//.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListBillOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetBills)]
        public async Task<PagedResultDto<BillSummaryOutput>> GetBills(GetBillListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var @query = (from b in _billRepository.GetAll()
                          //.WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                          .WhereIf(vendorTypeMemberIds.Any(), s => vendorTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                          .Where(u => u.ItemReceiptId == null)
                          .AsNoTracking()
                          join bi in _billItemRepository.GetAll()
                          .Include(u => u.Item)
                          .Include(u => u.Tax)
                          .Include(u => u.ItemReceiptItem)
                          .Where(u => u.ItemId.Value != null)
                          .AsNoTracking()
                          on b.Id equals bi.BillId into p
                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.LocationId.Value))
                          //.WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking()
                          on b.Id equals j.BillId
                          select new BillSummaryOutput
                          {
                              Vendor = ObjectMapper.Map<VendorSummaryOutput>(b.Vendor),
                              VendorId = b.VendorId,
                              Memo = j.Memo,
                              Id = b.Id,
                              BillNo = j.JournalNo,
                              CurrencyId = j.CurrencyId,
                              Currency = ObjectMapper.Map<CurrencyDetailOutput>(j.Currency),
                              Date = j.Date,
                              Reference = j.Reference,
                              Total = b.Total,
                              CountItems = p.Where(r => r.Item.InventoryAccountId != null).Count(),
                              ETA = b.ETA,
                              CreationTimeIndex = j.CreationTimeIndex,
                          }).Where(u => u.CountItems > 0);
            var resultCount = await query.CountAsync();

            var @entities = await query.OrderByDescending(t => t.ETA).ThenByDescending(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<BillSummaryOutput>(resultCount, @entities);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetDetail)]
        public async Task<BillDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Bill)
                                .Include(u => u.Class)
                                .Include(u => u.Currency)
                                .Include(u => u.MultiCurrency)
                                .Include(u => u.Location)
                                .Include(u => u.Bill.Vendor)
                                .Include(u => u.Bill.ShippingAddress)
                                .Include(u => u.Bill.BillingAddress)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
                                .FirstOrDefaultAsync();



            if (@journal == null || @journal.Bill == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.AP && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var billItems = await _billItemRepository.GetAll()
                                 .Include(u => u.Bill.ItemReceipt)
                                .Include(u => u.Tax)
                                .Include(u => u.ItemReceiptItem.OrderItem.PurchaseOrder)
                                .Include(u => u.Item)
                                .Include(u => u.Lot)
                                .Include(u => u.OrderItem.PurchaseOrder)
                                .Include(u => u.Tax)
                                .Where(u => u.BillId == input.Id)
                                .Join(_journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(), u => u.Id, s => s.Identifier,
                                (bItem, jItem) =>
                                new BillItemDetailOutput()
                                {
                                    OriginalQtyFromPurchase = bItem.OrderItemId != null ? (bItem.OrderItem != null ? bItem.OrderItem.Unit : 0) - (bItem.ItemReceiptItem != null && bItem.Bill.ItemReceiptId == null ? bItem.ItemReceiptItem.Qty : 0) - (bItem != null ? bItem.Qty : 0) : 0,
                                    CreationTime = bItem.CreationTime,
                                    OrderNumber = bItem.Bill.ReceiveFrom == Bill.ReceiveFromStatus.PO ?
                                                  bItem.OrderItem.PurchaseOrder.OrderNumber :
                                                  bItem.ItemReceiptItem != null && bItem.ItemReceiptItem.OrderItem != null ? bItem.ItemReceiptItem.OrderItem.PurchaseOrder.OrderNumber : "",
                                    Id = bItem.Id,
                                    Item = ObjectMapper.Map<ItemSummaryOutput>(bItem.Item),
                                    ItemId = bItem.ItemId,
                                    PurchaseOrderItem = ObjectMapper.Map<PurchaseOrderItemSummaryOut>(bItem.OrderItem),
                                    InventoryAccountId = jItem.AccountId,
                                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                    Description = bItem.Description,
                                    DiscountRate = bItem.DiscountRate,
                                    Qty = bItem.Qty,
                                    Tax = ObjectMapper.Map<TaxSummaryOutput>(bItem.Tax),
                                    Total = bItem.Total,
                                    UnitCost = bItem.UnitCost,
                                    OrderItemId = bItem.Bill.ReceiveFrom == Bill.ReceiveFromStatus.PO ? bItem.OrderItemId :
                                                  bItem.ItemReceiptItem != null ? bItem.ItemReceiptItem.OrderItemId : (Guid?)null,
                                    TaxId = bItem.TaxId,
                                    ItemReceiptId = bItem.ItemReceiptItemId,
                                    IsItemReceipt = bItem.IsItemReceipt,
                                    LotId = bItem.LotId,
                                    LotDetail = ObjectMapper.Map<LotSummaryOutput>(bItem.Lot),
                                    MultiCurrencyTotal = bItem.MultiCurrencyTotal,
                                    MultiCurrencyUnitCost = bItem.MultiCurrencyUnitCost,
                                    PurchaseOrderId = bItem.Bill.ReceiveFrom == Bill.ReceiveFromStatus.PO ?
                                        bItem.OrderItem.PurchaseOrderId :
                                        bItem.ItemReceiptItem != null && bItem.ItemReceiptItem.OrderItem != null ? bItem.OrderItem.PurchaseOrderId : (Guid?)null,
                                    ParentId = bItem.ParentId,
                                    DisplayInventoryAccount = bItem.Item == null ? false : bItem.Item.ItemType.DisplayInventoryAccount,
                                    UseBatchNo = bItem.Item != null && bItem.Item.UseBatchNo,
                                    AutoBatchNo = bItem.Item != null && bItem.Item.AutoBatchNo,
                                    TrackSerial = bItem.Item != null && bItem.Item.TrackSerial,
                                    TrackExpiration = bItem.Item != null && bItem.Item.TrackExpiration,
                                }).OrderBy(u => u.CreationTime)
                                .ToListAsync();

            //Map Key for assembly item
            var parentItemIds = billItems.Where(s => s.ParentId.HasValue).GroupBy(s => s.ParentId.Value).Select(s => s.Key).ToList();
            if (parentItemIds.Any())
            {
                var keyItems = billItems.Where(s => parentItemIds.Contains(s.Id)).ToList();
                foreach (var k in keyItems)
                {
                    k.Key = k.Id;

                    var subItems = billItems.Where(s => s.ParentId == k.Id).ToList();
                    foreach (var sub in subItems)
                    {
                        sub.Display = k.Item.ShowSubItems;
                    }
                }
            }


            var itemReceiptItemIds = billItems.Where(s => s.ItemReceiptId.HasValue).Select(s => s.ItemReceiptId.Value).ToList();
            if (itemReceiptItemIds.Any())
            {
                var batchDic = await _itemReceiptItemBatchNoRepository.GetAll()
                              .AsNoTracking()
                              .Where(s => itemReceiptItemIds.Contains(s.ItemReceiptItemId))
                              .Select(s => new BatchNoItemOutput
                              {
                                  Id = s.Id,
                                  BatchNoId = s.BatchNoId,
                                  BatchNumber = s.BatchNo.Code,
                                  ExpirationDate = s.BatchNo.ExpirationDate,
                                  Qty = s.Qty,
                                  TransactionItemId = s.ItemReceiptItemId
                              })
                              .GroupBy(s => s.TransactionItemId)
                              .ToDictionaryAsync(k => k.Key, v => v.ToList());
                if (batchDic.Any())
                {
                    foreach (var i in billItems)
                    {
                        if (i.ItemReceiptId.HasValue && batchDic.ContainsKey(i.ItemReceiptId.Value)) i.ItemBatchNos = batchDic[i.ItemReceiptId.Value];
                    }
                }
            }

            var result = ObjectMapper.Map<BillDetailOutput>(journal.Bill);


            var itemReceipt = (from inv in _billRepository.GetAll().Where(t => t.Id == input.Id)
                               join iis in _itemReceiptRepository.GetAll() on inv.ItemReceiptId equals iis.Id
                               join j in _journalRepository.GetAll() on iis.Id equals j.ItemReceiptId
                               select new
                               {
                                   Id = iis.Id,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   j.Reference,
                               }).FirstOrDefault();
            if (itemReceipt == null)
            {
                result.ItemReceiptId = null;
            }
            else
            {
                result.ItemReceiptId = itemReceipt.Id;
                result.ItemReceiptNo = itemReceipt.JournalNo;
                result.ItemReceiptDate = itemReceipt.Date;
                result.ItemReceiptReference = itemReceipt.Reference;
            }

            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.BillId == input.Id)
                                            .Select(s => new GetExchangeRateDto
                                            {
                                                Id = s.Id,
                                                FromCurrencyCode = s.FromCurrency.Code,
                                                ToCurrencyCode = s.ToCurrency.Code,
                                                FromCurrencyId = s.FromCurrencyId,
                                                ToCurrencyId = s.ToCurrencyId,
                                                Bid = s.Bid,
                                                Ask = s.Ask,
                                                IsInves = s.FromCurrencyId == journal.CurrencyId
                                            })
                                            .FirstOrDefaultAsync();
            }

            result.BillNo = journal.JournalNo;
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.BillItems = billItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.ItemReceiptId = journal.Bill.ItemReceiptId;
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetList)]
        public async Task<PagedResultDto<BillHeader>> GetList(GetListBillInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var jQuery = _journalRepository.GetAll()
                        .Where(s => s.JournalType == JournalType.VendorCredit || s.JournalType == JournalType.Bill)
                        .WhereIf(input.ClassIds != null && input.ClassIds.Count() > 0, s => input.ClassIds.Contains(s.ClassId.Value))
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .WhereIf(input.Status != null && input.Status.Count > 0, t => input.Status.Contains(t.Status))
                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                        .WhereIf(input.Type != null && input.Type.Count > 0, u => input.Type.Contains(u.JournalType))
                        .WhereIf(input.FromDate != null && input.ToDate != null,
                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                            u.Reference.ToLower().Contains(input.Filter.ToLower()))
                        .AsNoTracking()
                        .Select(j => new
                        {
                            Id = j.Id,
                            CreationTimeIndex = j.CreationTimeIndex,
                            CreationTime = j.CreationTime,
                            JournalType = j.JournalType,
                            Memo = j.Memo,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            BillId = j.BillId,
                            VenderCreditId = j.VendorCreditId,
                            CurrencyId = j.CurrencyId,
                            LocationId = j.LocationId,
                            CreatorUserId = j.CreatorUserId,
                            Reference = j.Reference,
                        });

            var currencyQuery = GetCurrencies();
            var locationQuery = GetLocations(null, input.Locations);
            var userQuery = GetUsers(input.Users);

            var journalQuery = from j in jQuery
                               join c in currencyQuery
                               on j.CurrencyId equals c.Id
                               join l in locationQuery
                               on j.LocationId equals l.Id
                               join u in userQuery
                               on j.CreatorUserId equals u.Id
                               select new
                               {
                                   Id = j.Id,
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   CreationTime = j.CreationTime,
                                   JournalType = j.JournalType,
                                   Memo = j.Memo,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Status = j.Status,
                                   BillId = j.BillId,
                                   VendorCreditId = j.VenderCreditId,
                                   CurrencyId = j.CurrencyId,
                                   LocationId = j.LocationId,
                                   CreatorUserId = j.CreatorUserId,
                                   CurrencyCode = c.Code,
                                   LocationName = l.LocationName,
                                   UserName = u.UserName,
                                   Reference = j.Reference
                               };

            var journalItemQuery = _journalItemRepository.GetAll()
                                   .Where(t => t.Key == PostingKey.AP && t.Identifier == null)
                                   .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                                   .AsNoTracking()
                                   .Select(s => new
                                   {
                                       Id = s.Id,
                                       AccountId = s.AccountId,
                                       JournalId = s.JournalId
                                   });

            var accountQuery = GetAccounts(input.Accounts);
            var jiaQuery = from ji in journalItemQuery
                           join a in accountQuery
                           on ji.AccountId equals a.Id
                           select new
                           {
                               Id = ji.Id,
                               AccountId = ji.AccountId,
                               JournalId = ji.JournalId,
                               AccountName = a.AccountName
                           };

            var jiQuery = from j in journalQuery
                          join ji in jiaQuery
                          on j.Id equals ji.JournalId
                          select new
                          {
                              Id = j.Id,
                              CreationTimeIndex = j.CreationTimeIndex,
                              CreationTime = j.CreationTime,
                              JournalType = j.JournalType,
                              Memo = j.Memo,
                              Date = j.Date,
                              JournalNo = j.JournalNo,
                              Status = j.Status,
                              BillId = j.BillId,
                              VendorCreditId = j.VendorCreditId,
                              CurrencyId = j.CurrencyId,
                              LocationId = j.LocationId,
                              CreatorUserId = j.CreatorUserId,
                              CurrencyCode = j.CurrencyCode,
                              LocationName = j.LocationName,
                              UserName = j.UserName,
                              AccountId = ji.AccountId,
                              AccountName = ji.AccountName,
                              Reference = j.Reference
                          };


            var billQuery = _billRepository.GetAll()
                               .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => input.PaidStatus.Contains(u.PaidStatus))
                               .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                               .AsNoTracking()
                               .Select(s => new
                               {
                                   Id = s.Id,
                                   Total = s.Total,
                                   TotalPaid = s.TotalPaid,
                                   DueDate = s.DueDate,
                                   OpenBalance = s.OpenBalance,
                                   PaidStatus = s.PaidStatus,
                                   ReceivedStatus = s.ReceivedStatus,
                                   VendorId = s.VendorId
                               });

            var billItemQuery = _billItemRepository.GetAll()
                                .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                .AsNoTracking()
                                .Select(s => new
                                {
                                    BillId = s.BillId
                                });

            var iQuery = from i in billQuery
                         join ii in billItemQuery
                         on i.Id equals ii.BillId
                         into iis
                         where iis.Count() > 0
                         select i;


            var vendorQuery = GetVendors(null, input.Vendors, input.VendorTypes, vendorTypeMemberIds);
            var icQuery = from i in iQuery
                          join c in vendorQuery
                          on i.VendorId equals c.Id
                          select new
                          {
                              Id = i.Id,
                              Total = i.Total,
                              TotalPaid = i.TotalPaid,
                              DueDate = i.DueDate,
                              OpenBalance = i.OpenBalance,
                              PaidStatus = i.PaidStatus,
                              ReceivedStatus = i.ReceivedStatus,
                              VendorId = i.VendorId,
                              VendorName = c.VendorName
                          };

            var vendorCreditQuery = _vendorCreditRepository.GetAll()
                                    .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => input.PaidStatus.Contains(u.PaidStatus))
                                    .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        Id = s.Id,
                                        Total = s.Total,
                                        TotalPaid = s.TotalPaid,
                                        DueDate = s.DueDate,
                                        OpenBalance = s.OpenBalance,
                                        PaidStatus = s.PaidStatus,
                                        ShipedStatus = s.ShipedStatus,
                                        VendorId = s.VendorId
                                    });

            var vendorCreditItemQuery = _vendorCreditDetailRepository.GetAll()
                                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .AsNoTracking()
                                        .Select(s => new
                                        {
                                            VendorCreditId = s.VendorCreditId
                                        });

            var cQuery = from c in vendorCreditQuery
                         join ci in vendorCreditItemQuery
                         on c.Id equals ci.VendorCreditId
                         into cis
                         where cis.Count() > 0
                         select c;

            var ccQuery = from cc in cQuery
                          join c in vendorQuery
                          on cc.VendorId equals c.Id
                          select new
                          {
                              Id = cc.Id,
                              Total = cc.Total,
                              TotalPaid = cc.TotalPaid,
                              DueDate = cc.DueDate,
                              OpenBalance = cc.OpenBalance,
                              PaidStatus = cc.PaidStatus,
                              ShipedStatus = cc.ShipedStatus,
                              VendorId = cc.VendorId,
                              VendorName = c.VendorName
                          };

            var union = from j in jiQuery

                        join i in icQuery
                        on j.BillId equals i.Id
                        into invs
                        from inv in invs.DefaultIfEmpty()

                        join c in ccQuery
                        on j.VendorCreditId equals c.Id
                        into ccs
                        from cc in ccs.DefaultIfEmpty()

                        where inv != null || cc != null
                        select new GetListBillOutput
                        {
                            Currency = new CurrencyDetailOutput
                            {
                                Code = j.CurrencyCode,
                                Id = j.CurrencyId
                            },
                            CreationTimeIndex = j.CreationTimeIndex,
                            CreationTime = j.CreationTime,
                            CurrencyId = j.CurrencyId,
                            TypeName = j.JournalType.ToString(),
                            TypeCode = j.JournalType,
                            Memo = j.Memo,
                            Id = j.BillId.HasValue ? j.BillId.Value : j.VendorCreditId.Value,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            Total = inv != null ? inv.Total : cc.Total,
                            Vendor = inv != null ?
                            new VendorSummaryOutput
                            {
                                Id = inv.VendorId,
                                VendorName = inv.VendorName
                            } :
                            new VendorSummaryOutput
                            {
                                Id = cc.VendorId,
                                VendorName = cc.VendorName
                            },
                            User = new UserDto
                            {
                                Id = j.CreatorUserId.Value,
                                UserName = j.UserName
                            },
                            AccountId = j.AccountId,
                            AccountName = j.AccountName,
                            LocationName = j.LocationName,

                            VendorId = inv != null ? inv.VendorId : cc.VendorId,
                            TotalPaid = inv != null ? inv.TotalPaid : cc.TotalPaid,
                            DueDate = inv != null ? inv.DueDate : cc.DueDate,
                            OpenBalance = inv != null ? inv.OpenBalance : cc.OpenBalance,
                            PaidStatus = inv != null ? inv.PaidStatus : cc.PaidStatus,
                            ReceivedStatus = inv != null ? inv.ReceivedStatus : cc.ShipedStatus,
                            Reference = j.Reference
                        };

            var resultCount = await union.CountAsync();
            if (resultCount == 0) return new PagedResultDto<BillHeader>(resultCount, new List<BillHeader>());


            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    var sortQeury = union.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    var sortQeury = union.OrderByDescending(s => s.TypeName).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    var sortQeury = union.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    var sortQeury = union.OrderByDescending(s => s.Total).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("openbalance"))
                {
                    var sortQeury = union.OrderByDescending(s => s.OpenBalance).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("duedate"))
                {
                    var sortQeury = union.OrderByDescending(s => s.DueDate.Date).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("paidstatus"))
                {
                    var sortQeury = union.OrderByDescending(s => s.PaidStatus).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("receivedstatus"))
                {
                    var sortQeury = union.OrderByDescending(s => s.ReceivedStatus).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    var sortQeury = union.OrderByDescending(s => s.Status).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    var sortQeury = union.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    var sortQeury = union.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    var sortQeury = union.OrderBy(s => s.TypeName).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    var sortQeury = union.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    var sortQeury = union.OrderBy(s => s.Total).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("openbalance"))
                {
                    var sortQeury = union.OrderBy(s => s.OpenBalance).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("duedate"))
                {
                    var sortQeury = union.OrderBy(s => s.DueDate.Date).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("paidstatus"))
                {
                    var sortQeury = union.OrderBy(s => s.PaidStatus).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("receivedstatus"))
                {
                    var sortQeury = union.OrderBy(s => s.ReceivedStatus).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    var sortQeury = union.OrderBy(s => s.Status).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    var sortQeury = union.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
            }

            var query = await union.PageBy(input).ToListAsync();


            var summary = new List<BalanceSummary>();

            if (query != null && query.Count > 0)
            {
                // get total record of summary for first index
                summary.Add(
                    new BalanceSummary
                    {
                        Balance = query.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.OpenBalance * -1 : t.OpenBalance),
                        chartOfAccount = null,
                        Total = query.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.Total * -1 : t.Total),
                        TotalPaid = query.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.TotalPaid : t.TotalPaid * -1)
                    }
                );
                var allAcc = query.GroupBy(ct => new { ct.AccountName }).Select(a => new BalanceSummary
                {
                    Balance = a.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.OpenBalance * -1 : t.OpenBalance),
                    chartOfAccount = a.Key.AccountName,
                    Total = a.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.Total * -1 : t.Total),
                    TotalPaid = a.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.TotalPaid : t.TotalPaid * -1)
                }).ToList();

                summary = summary.Concat(allAcc).ToList();
            }

            var headersQuery = new List<BillHeader> {
                new BillHeader
                {
                    BalanceSummary = summary,
                    BillList = query.ToList()
               }
            };

            var @entities = headersQuery;


            return new PagedResultDto<BillHeader>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetList)]
        public async Task<PagedResultDto<BillHeader>> GetListOld(GetListBillInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

#if oldCode
            //var billQuery = (from jItem in _journalItemRepository.GetAll().Include(t => t.Account).AsNoTracking()
            //                 join j in _journalRepository.GetAll()
            //                 .Where(u => u.JournalType == JournalType.Bill || u.JournalType == JournalType.VendorCredit)
            //                 .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
            //                 .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
            //                 .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
            //                 .WhereIf(input.Type != null && input.Type.Count > 0, u => input.Type.Contains(u.JournalType))
            //                 .WhereIf(input.FromDate != null && input.ToDate != null,
            //                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
            //                 .WhereIf(!input.Filter.IsNullOrEmpty(),
            //                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
            //                            u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
            //                            u.Memo.ToLower().Contains(input.Filter.ToLower()))
            //                 .AsNoTracking()
            //                 on jItem.JournalId equals j.Id


            //                 //into bi
            //                 //from bitem in bi.DefaultIfEmpty()

            //                 join b in _billRepository.GetAll().Include(t => t.Vendor).AsNoTracking()
            //                 .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => input.PaidStatus.Contains(u.PaidStatus))
            //                 .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
            //                 .WhereIf(input.VendorTypes != null && input.VendorTypes.Count > 0, u => input.VendorTypes.Contains(u.Vendor.VendorTypeId.Value))
            //                 on j.BillId equals b.Id

            //                 join bi in _billItemRepository.GetAll().AsNoTracking()
            //                .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
            //                on b.Id equals bi.BillId

            //                 join c in _currencyRepository.GetAll().AsNoTracking()
            //                 on j.CurrencyId equals c.Id

            //                 join u in _userRepository.GetAll().AsNoTracking()
            //                 on j.CreatorUserId equals u.Id

            //                 group jItem by new
            //                 {
            //                     CreationTimeIndex = j.CreationTimeIndex,
            //                     JournalNo = j.JournalNo,
            //                     JournalId = j.Id,
            //                     JournalDate = j.Date,
            //                     JournalStatus = j.Status,
            //                     JournalType = j.JournalType,
            //                     JournalMemo = j.Memo,
            //                     VendorId = b.VendorId,
            //                     VendorName = b.Vendor.VendorName,
            //                     VendorCode = b.Vendor.VendorCode,
            //                     VendorAccountId = b.Vendor.AccountId,
            //                     BillId = b.Id,
            //                     BillTotal = b.Total,
            //                     BillDueDate = b.DueDate,
            //                     BillTotalPaid = b.TotalPaid,
            //                     BillOpenBalance = b.OpenBalance,
            //                     BillPaidStatus = b.PaidStatus,
            //                     BillReceivedStatus = b.ReceivedStatus,
            //                     CurrencyId = j.CurrencyId,
            //                     CurrencyCode = c.Code,
            //                     CurrencyName = c.Name,
            //                     CurrencyPluralName = c.PluralName,
            //                     CurrencySymbol = c.Symbol,
            //                     CreatorId = u.Id,
            //                     CreatorName = u.UserName
            //                     //journal = j, bill = b, vendor = v, currency = c
            //                 } into u
            //                 select new GetListBillOutput
            //                 {
            //                     //IsCanVoidOrDraftOrClose =
            //                     //        (_billRepository.GetAll().Include(t => t.ItemReceipt)
            //                     //        .Where(v => v.Id == u.Key.BillId && v.ItemReceiptId != null &&
            //                     //               v.CreationTime < v.ItemReceipt.CreationTime && v.ConvertToItemReceipt == false)).Count() > 0
            //                     //           ||
            //                     //        _payBillDetailRepository.GetAll().Where(v => v.BillId == u.Key.BillId).Count() > 0
            //                     //           ? false : true,
            //                     Currency = new CurrencyDetailOutput
            //                     {
            //                         Code = u.Key.CurrencyCode,
            //                         Id = u.Key.CurrencyId,
            //                         Name = u.Key.CurrencyName,
            //                         PluralName = u.Key.CurrencyPluralName,
            //                         Symbol = u.Key.CurrencySymbol
            //                     },
            //                     CreationTimeIndex = u.Key.CreationTimeIndex,
            //                     CurrencyId = u.Key.CurrencyId,
            //                     TypeName = u.Key.JournalType.ToString(),
            //                     TypeCode = u.Key.JournalType,
            //                     Memo = u.Key.JournalMemo,
            //                     Id = u.Key.BillId,
            //                     Date = u.Key.JournalDate,
            //                     JournalNo = u.Key.JournalNo,
            //                     Status = u.Key.JournalStatus,
            //                     Total = u.Key.BillTotal,
            //                     Vendor = new VendorSummaryOutput
            //                     {
            //                         Id = u.Key.VendorId,
            //                         // AccountId = u.Key.VendorAccountId.Value,
            //                         VendorCode = u.Key.VendorCode,
            //                         VendorName = u.Key.VendorName
            //                     },
            //                     User = new UserDto
            //                     {
            //                         Id = u.Key.CreatorId,
            //                         UserName = u.Key.CreatorName
            //                     },
            //                     VendorId = u.Key.VendorId,
            //                     TotalPaid = u.Key.BillTotalPaid,
            //                     DueDate = u.Key.BillDueDate,
            //                     OpenBalance = u.Key.BillOpenBalance,
            //                     PaidStatus = u.Key.BillPaidStatus,
            //                     ReceivedStatus = u.Key.BillReceivedStatus,
            //                     AccountName = u.Where(t => t.JournalId == u.Key.JournalId && t.Key == PostingKey.AP && t.Identifier == null)
            //                             .Select(t => t.Account.AccountName).FirstOrDefault(),
            //                     AccountId = u.Where(t => t.JournalId == u.Key.JournalId && t.Key == PostingKey.AP && t.Identifier == null)
            //                             .Select(t => t.AccountId).FirstOrDefault(),
            //                 })
            //                 .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId));

            //var creditQuery = (from jItem in _journalItemRepository.GetAll().Include(t => t.Account).AsNoTracking()
            //                   join j in _journalRepository.GetAll().AsNoTracking()
            //                   .Where(u => u.JournalType == JournalType.Bill || u.JournalType == JournalType.VendorCredit)
            //                   .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))
            //                   .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
            //                   .WhereIf(input.Type != null && input.Type.Count > 0, u => input.Type.Contains(u.JournalType))
            //                   .WhereIf(input.FromDate != null && input.ToDate != null,
            //                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
            //                   .WhereIf(!input.Filter.IsNullOrEmpty(),
            //                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
            //                            u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
            //                            u.Memo.ToLower().Contains(input.Filter.ToLower()))
            //                    on jItem.JournalId equals j.Id


            //                   //into vcd
            //                   //from vCredit in vcd.DefaultIfEmpty()

            //                   join vc in _vendorCreditRepository.GetAll().Include(t => t.Vendor).AsNoTracking()
            //                   .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => input.PaidStatus.Contains(u.PaidStatus))
            //                   .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId)) 
            //                   .WhereIf(input.VendorTypes != null && input.VendorTypes.Count > 0, u => input.VendorTypes.Contains(u.Vendor.VendorTypeId.Value)) on jItem.Journal.VendorCreditId equals vc.Id

            //                   join vcd in _vendorCreditDetailRepository.GetAll().AsNoTracking()
            //                   .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
            //                   on vc.Id equals vcd.VendorCreditId

            //                   //join v in _vendorRepository.GetAll().AsNoTracking() 
            //                   //on vc.VendorId equals v.Id

            //                   join c in _currencyRepository.GetAll().AsNoTracking()
            //                   on j.CurrencyId equals c.Id

            //                   join u in _userRepository.GetAll().AsNoTracking()
            //                   on j.CreatorUserId equals u.Id

            //                   group jItem by new
            //                   {
            //                       CreationTimeIndex = j.CreationTimeIndex,
            //                       JournalNo = j.JournalNo,
            //                       JournalId = j.Id,
            //                       JournalDate = j.Date,
            //                       JournalStatus = j.Status,
            //                       JournalType = j.JournalType,
            //                       JournalMemo = j.Memo,
            //                       VendorId = vc.VendorId,
            //                       VendorName = vc.Vendor.VendorName,
            //                       VendorCode = vc.Vendor.VendorCode,
            //                       VendorAccountId = vc.Vendor.AccountId,
            //                       VendorCreditId = vc.Id,
            //                       VendorCreditTotal = vc.Total,
            //                       VendorCreditDueDate = vc.DueDate,
            //                       VendorCreditTotalPaid = vc.TotalPaid,
            //                       VendorCreditOpenBalance = vc.OpenBalance,
            //                       VendorCreditPaidStatus = vc.PaidStatus,
            //                       VendorCreditShipedStatus = vc.ShipedStatus,
            //                       CurrencyId = j.CurrencyId,
            //                       CurrencyCode = c.Code,
            //                       CurrencyName = c.Name,
            //                       CurrencyPluralName = c.PluralName,
            //                       CurrencySymbol = c.Symbol,
            //                       CreatorId = u.Id,
            //                       CreatorName = u.UserName
            //                       //journal = j, vendorCredit = vc, vendor = v, currency = c
            //                   } into u

            //                   select new GetListBillOutput
            //                   {
            //                       CreationTimeIndex = u.Key.CreationTimeIndex,
            //                       Currency = new CurrencyDetailOutput
            //                       {
            //                           Code = u.Key.CurrencyCode,
            //                           Id = u.Key.CurrencyId,
            //                           Name = u.Key.CurrencyName,
            //                           PluralName = u.Key.CurrencyPluralName,
            //                           Symbol = u.Key.CurrencySymbol
            //                       },
            //                       User = new UserDto
            //                       {
            //                           Id = u.Key.CreatorId,
            //                           UserName = u.Key.CreatorName
            //                       },
            //                       CurrencyId = u.Key.CurrencyId,
            //                       TypeName = u.Key.JournalType.ToString(),
            //                       TypeCode = u.Key.JournalType,
            //                       Memo = u.Key.JournalMemo,
            //                       Id = u.Key.VendorCreditId,
            //                       Date = u.Key.JournalDate,
            //                       TotalPaid = u.Key.VendorCreditTotalPaid,
            //                       DueDate = u.Key.VendorCreditDueDate,
            //                       OpenBalance = u.Key.VendorCreditOpenBalance,
            //                       JournalNo = u.Key.JournalNo,
            //                       Status = u.Key.JournalStatus,
            //                       Total = u.Key.VendorCreditTotal,
            //                       Vendor = new VendorSummaryOutput
            //                       {
            //                           Id = u.Key.VendorId,
            //                           AccountId = u.Key.VendorAccountId.Value,
            //                           VendorCode = u.Key.VendorCode,
            //                           VendorName = u.Key.VendorName
            //                       },
            //                       VendorId = u.Key.VendorId,
            //                       PaidStatus = u.Key.VendorCreditPaidStatus,
            //                       ReceivedStatus = u.Key.VendorCreditShipedStatus,
            //                       AccountName = u.Where(t => t.JournalId == u.Key.JournalId && t.Key == PostingKey.AP && t.Identifier == null)
            //                             .Select(t => t.Account.AccountName).FirstOrDefault(),
            //                       AccountId = u.Where(t => t.JournalId == u.Key.JournalId && t.Key == PostingKey.AP && t.Identifier == null)
            //                             .Select(t => t.AccountId).FirstOrDefault(),
            //                       //IsCanVoidOrDraftOrClose = (_payBillRepository.GetAll()
            //                       //                 .Where(v => v.VendorCreditId == u.Key.VendorCreditId && v.VendorCreditId != null)).Count() > 0 ||
            //                       //                  (_itemIssueVendorCreditRepository.GetAll().Include(c => c.VendorCredit)
            //                       //                  .Where(v => v.VendorCreditId == u.Key.VendorCreditId && v.VendorCreditId != null && v.VendorCredit.ConvertToItemIssueVendor == false)).Count() > 0 ? false : true,
            //                   })
            //                   .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId));

            //var union = billQuery.Union(creditQuery);
#endif

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var union = from j in _journalRepository.GetAll()
                                      .Include(u => u.Bill.Vendor)
                                      .Include(u => u.VendorCredit.Vendor)
                                      .Include(u => u.Currency)
                                      .Include(u => u.Location)
                                      .Where(s => s.JournalType == JournalType.Bill || s.JournalType == JournalType.VendorCredit)
                                      .WhereIf(input.ClassIds != null && input.ClassIds.Count() > 0, s => input.ClassIds.Contains(s.ClassId.Value))
                                      .WhereIf(input.VendorTypes != null && input.VendorTypes.Count > 0, u =>
                                       (u.Bill != null && u.Bill.Vendor != null && input.VendorTypes.Contains(u.Bill.Vendor.VendorTypeId.Value)) ||
                                      (u.VendorCredit != null && u.VendorCredit.Vendor != null && input.VendorTypes.Contains(u.VendorCredit.Vendor.VendorTypeId.Value)))

                                      .WhereIf(vendorTypeMemberIds.Any(), s => (s.JournalType == JournalType.Bill && vendorTypeMemberIds.Contains(s.Bill.Vendor.VendorTypeId.Value)) ||
                                                                               (s.JournalType == JournalType.VendorCredit && vendorTypeMemberIds.Contains(s.VendorCredit.Vendor.VendorTypeId.Value)))

                                      .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                      .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                      .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                      .WhereIf(input.Type != null && input.Type.Count > 0, u => input.Type.Contains(u.JournalType))
                                      .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => (u.Bill != null && input.PaidStatus.Contains(u.Bill.PaidStatus)) || (u.VendorCredit != null && input.PaidStatus.Contains(u.VendorCredit.PaidStatus)))
                                      .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => (u.Bill != null && input.Vendors.Contains(u.Bill.VendorId)) || (u.VendorCredit != null && input.Vendors.Contains(u.VendorCredit.VendorId)))
                                      .WhereIf(input.FromDate != null && input.ToDate != null,
                                       (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                      .WhereIf(!input.Filter.IsNullOrEmpty(),
                                       u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                       u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                       u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                       .AsNoTracking()
                        join ji in _journalItemRepository.GetAll()
                       .Where(t => t.Key == PostingKey.AP && t.Identifier == null)
                       .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                       .AsNoTracking()
                       on j.Id equals ji.JournalId
                       into jis
                        join bi in _billItemRepository.GetAll()
                                      .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                      .AsNoTracking()
                                      on j.BillId equals bi.BillId
                         into bitems

                        join vcd in _vendorCreditDetailRepository.GetAll()
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .AsNoTracking()
                       on j.VendorCreditId equals vcd.VendorCreditId
                        into vcditems
                        where ((bitems != null && bitems.Count() > 0) || (vcditems != null && vcditems.Count() > 0)) && jis != null && jis.Count() > 0

                        select new GetListBillOutput
                        {
                            Currency = new CurrencyDetailOutput
                            {
                                Code = j.Currency.Code,
                                Id = j.Currency.Id,
                                Name = j.Currency.Name,
                                PluralName = j.Currency.PluralName,
                                Symbol = j.Currency.Symbol
                            },
                            CreationTimeIndex = j.CreationTimeIndex,
                            CurrencyId = j.CurrencyId,
                            TypeName = j.JournalType.ToString(),
                            TypeCode = j.JournalType,
                            Memo = j.Memo,
                            Id = j.BillId != null ? j.BillId.Value : j.VendorCreditId.Value,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            Total = j.Bill != null ? j.Bill.Total : j.VendorCredit.Total,
                            Vendor = j.Bill != null ? new VendorSummaryOutput
                            {
                                Id = j.Bill.VendorId,

                                VendorCode = j.Bill.Vendor.VendorCode,
                                VendorName = j.Bill.Vendor.VendorName
                            } :

                                                   new VendorSummaryOutput
                                                   {
                                                       Id = j.VendorCredit.VendorId,
                                                       VendorCode = j.VendorCredit.Vendor.VendorCode,
                                                       VendorName = j.VendorCredit.Vendor.VendorName
                                                   },
                            User = new UserDto
                            {
                                Id = j.CreatorUserId.Value,
                                UserName = j.CreatorUser.Name
                            },
                            VendorId = j.Bill != null ? j.Bill.VendorId : j.VendorCredit.VendorId,
                            TotalPaid = j.Bill != null ? j.Bill.TotalPaid : j.VendorCredit.TotalPaid,
                            DueDate = j.Bill != null ? j.Bill.DueDate : j.VendorCredit.DueDate,
                            OpenBalance = j.Bill != null ? j.Bill.OpenBalance : j.VendorCredit.OpenBalance,
                            PaidStatus = j.Bill != null ? j.Bill.PaidStatus : j.VendorCredit.PaidStatus,
                            ReceivedStatus = j.Bill != null ? j.Bill.ReceivedStatus : j.VendorCredit.ShipedStatus,
                            AccountId = jis.FirstOrDefault().AccountId,
                            AccountName = jis.FirstOrDefault().Account.AccountName,
                            LocationName = j.Location.LocationName
                        };

            List<GetListBillOutput> query;
            if (input.Sorting.Contains("date") && !input.Sorting.Contains(".")) input.Sorting = input.Sorting.Replace("date", "Date.Date");

            if (input.Sorting.Contains("DESC"))
            {
                query = await union.OrderBy(input.Sorting).ThenByDescending(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            }
            else
            {
                query = await union.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            }

            var summary = new List<BalanceSummary>();

            if (query != null && query.Count > 0)
            {
                // get total record of summary for first index
                summary.Add(
                    new BalanceSummary
                    {
                        Balance = query.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.OpenBalance * -1 : t.OpenBalance),
                        chartOfAccount = null,
                        Total = query.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.Total * -1 : t.Total),
                        TotalPaid = query.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.TotalPaid : t.TotalPaid * -1)
                    }
                );
                var allAcc = query.GroupBy(ct => new { ct.AccountName }).Select(a => new BalanceSummary
                {
                    Balance = a.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.OpenBalance * -1 : t.OpenBalance),
                    chartOfAccount = a.Key.AccountName,
                    Total = a.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.Total * -1 : t.Total),
                    TotalPaid = a.Sum(t => t.TypeCode == JournalType.VendorCredit ? t.TotalPaid : t.TotalPaid * -1)
                }).ToList();

                summary = summary.Concat(allAcc).ToList();
            }

            var headersQuery = new List<BillHeader> {
                new BillHeader
                {
                    BalanceSummary = summary,
                    BillList = query.ToList()
               }
            };
            var resultCount = await union.CountAsync();
            var @entities = headersQuery;


            return new PagedResultDto<BillHeader>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetListForPayBill)]
        public async Task<PagedResultDto<getBillListOutput>> GetListBillForPayBill(GetListBillForPaybillInput input)
        {
            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var accountCycle = await GetCurrentCycleAsync();
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var jQuery = _journalRepository.GetAll()
                        .Where(u => u.JournalType == JournalType.Bill)
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .Where(u => u.Status == TransactionStatus.Publish)
                        .WhereIf(
                        input.FromDate != null && input.ToDate != null,
                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date))
                        )
                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            s.Id,
                            s.BillId,
                            s.Date,
                            s.JournalNo,
                            s.Reference,
                            s.Memo,
                            s.CurrencyId,
                            s.MultiCurrencyId
                        });

            var currencyQuery = GetCurrencies();
            var journalQuery = from j in jQuery
                               join c in currencyQuery
                               on j.MultiCurrencyId equals c.Id
                               select new
                               {
                                   j.Id,
                                   j.BillId,
                                   j.Date,
                                   j.JournalNo,
                                   j.Reference,
                                   j.Memo,
                                   j.MultiCurrencyId,
                                   MultiCurrencyCode = c.Code
                               };


            var journalItemQuery = _journalItemRepository.GetAll()
                                   .Where(s => s.Identifier == null && s.Key == PostingKey.AP)
                                   .AsNoTracking()
                                   .Select(s => new
                                   {
                                       s.JournalId,
                                       s.AccountId
                                   });

            var jiQuery = from j in journalQuery
                          join ji in journalItemQuery
                          on j.Id equals ji.JournalId
                          select new
                          {
                              j.Id,
                              j.BillId,
                              j.Date,
                              j.JournalNo,
                              j.Reference,
                              j.Memo,
                              j.MultiCurrencyId,
                              j.MultiCurrencyCode,
                              ji.AccountId
                          };

            var iQuery = _billRepository.GetAll()
                        .Where(x => x.OpenBalance != 0)
                        .WhereIf(input.BillNo != null && input.BillNo.Count > 0, u => input.BillNo.Contains(u.Id))
                        .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            Id = s.Id,
                            Total = s.Total,
                            VendorId = s.VendorId,
                            DueDate = s.DueDate,
                            OpenBalance = Math.Round(s.OpenBalance, accountCycle.RoundingDigit),
                            TotalPaid = s.TotalPaid,
                            MultiCurrencyOpenBalance = Math.Round(s.MultiCurrencyOpenBalance, accountCycle.RoundingDigit),
                            MultiCurrencyTotalPaid = s.MultiCurrencyTotalPaid,
                        });

            var vendorQuery = GetVendors(null, input.Vendors, null, vendorTypeMemberIds);
            var billQuery = from i in iQuery
                            join c in vendorQuery
                            on i.VendorId equals c.Id
                            select new
                            {
                                Id = i.Id,
                                Total = i.Total,
                                VendorId = i.VendorId,
                                DueDate = i.DueDate,
                                OpenBalance = i.OpenBalance,
                                TotalPaid = i.TotalPaid,
                                MultiCurrencyOpenBalance = i.MultiCurrencyOpenBalance,
                                MultiCurrencyTotalPaid = i.MultiCurrencyTotalPaid,
                                VendorName = c.VendorName,
                            };


            var query = from i in billQuery
                        join j in jiQuery
                        on i.Id equals j.BillId
                        select new getBillListOutput
                        {
                            Memo = j.Memo,
                            Id = i.Id,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Reference = j.Reference,
                            AccountId = j.AccountId,
                            Total = i.Total,
                            Vendor = new VendorSummaryOutput
                            {
                                Id = i.VendorId,
                                VendorName = i.VendorName,
                            },
                            VendorId = i.VendorId,
                            DueDate = i.DueDate,
                            OpenBalance = i.OpenBalance,
                            TotalPaid = i.TotalPaid,
                            MultiCurrencyOpenBalance = i.MultiCurrencyOpenBalance,
                            MultiCurrencyTotalPaid = i.MultiCurrencyTotalPaid,
                            MultiCurrencyCode = j.MultiCurrencyCode,
                            MultiCurrencyId = j.MultiCurrencyId
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<getBillListOutput>(resultCount, new List<getBillListOutput>());

            var @entities = await query.OrderBy(t => t.DueDate).ToListAsync();


            return new PagedResultDto<getBillListOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetListForPayBill)]
        public async Task<PagedResultDto<getBillListOutput>> GetListBillForPayBillOld(GetListBillForPaybillInput input)
        {
            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var accountCycle = await GetCurrentCycleAsync();
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.Bill)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .Where(u => u.Status == TransactionStatus.Publish)
                         .WhereIf(
                            input.FromDate != null && input.ToDate != null,
                            (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date))
                         )
                          .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))

                         //.WhereIf(
                         //    !input.Filter.IsNullOrEmpty(),
                         //    u => u.JournalNo.ToLower().Contains(input.Filter.ToLower())) 
                         on jItem.JournalId equals j.Id
                         where (jItem.Identifier == null)
                         //join bi in _billItemRepository.GetAll().AsNoTracking()
                         //on jItem.Identifier equals bi.Id
                         join mc in _currencyRepository.GetAll() on j.MultiCurrencyId equals mc.Id
                         join b in _billRepository.GetAll()
                         .Where(x => x.OpenBalance > 0)
                         .WhereIf(input.BillNo != null && input.BillNo.Count > 0, u => input.BillNo.Contains(u.Id))
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                         .WhereIf(vendorTypeMemberIds.Any(), s => vendorTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                         .AsNoTracking()
                         on j.BillId equals b.Id

                         join v in _vendorRepository.GetAll().AsNoTracking() on b.VendorId equals v.Id

                         group jItem by new { multiCurrency = mc, journal = j, journalItem = jItem, bill = b, vendor = v } into u

                         select new getBillListOutput
                         {
                             Memo = u.Key.journal.Memo,
                             Id = u.Key.bill.Id,
                             Date = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             AccountId = u.Key.journalItem.AccountId,
                             Total = u.Key.bill.Total,
                             Vendor = ObjectMapper.Map<VendorSummaryOutput>(u.Key.vendor),
                             VendorId = u.Key.bill.VendorId,
                             DueDate = u.Key.bill.DueDate,
                             OpenBalance = Math.Round(u.Key.bill.OpenBalance, accountCycle.RoundingDigit),
                             TotalPaid = u.Key.bill.TotalPaid,
                             MultiCurrencyOpenBalance = Math.Round(u.Key.bill.MultiCurrencyOpenBalance, accountCycle.RoundingDigit),
                             MultiCurrencyTotal = u.Key.bill.MultiCurrencyTotal,
                             MultiCurrencyTotalPaid = u.Key.bill.MultiCurrencyTotalPaid,
                             MultiCurrencyId = u.Key.journal.MultiCurrencyId,
                             MultiCurrencyCode = u.Key.multiCurrency.Code,
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(t => t.DueDate).ToListAsync();


            return new PagedResultDto<getBillListOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateBillInput input)
        {
            //validate billItem when create by none
            if (input.BillItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            // validate from Paybill

            var validatefrompayBill = await (from paybillDetail in _payBillDetailRepository.GetAll()
                                                .Where(t => t.BillId == input.Id).AsNoTracking()
                                             select paybillDetail).CountAsync();

            if (validatefrompayBill > 0)
            {
                throw new UserFriendlyException(L("RecordHasPayBill"));
            }

            if (input.IsConfirm == false)
            {

                var validateLockDate = await _lockRepository.GetAll()
                                       .Where(t => t.IsLock == true &&
                                       (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                       && t.LockKey == TransactionLockType.Bill).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (input.ConvertToItemReceipt || input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt) await ValidateAddBatchNo(input);

            //validation Qty on hand form PO
            //validation Qty on hand form PO
            if (input.ReceiveFrom == Bill.ReceiveFromStatus.PO)
            {
                var validates = input.BillItems.Where(t => t.OrderItemId != null).Select(t => new { ItemName = t.ItemName, OriginalQty = t.OrginalQtyFromPurchase, OrderItemId = t.OrderItemId }).GroupBy(t => t.OrderItemId).ToList();
                foreach (var v in validates)
                {
                    var sumQtyByOrderItems = input.BillItems.Where(g => g.OrderItemId == v.Key).Sum(t => t.Qty);
                    if (sumQtyByOrderItems > v.FirstOrDefault().OriginalQty)
                    {
                        throw new UserFriendlyException(L("BillMessageWarning", v.First().ItemName));
                    }
                }
            }
            else if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt)
            {
                foreach (var v in input.BillItems.Where(s => s.OrderItemId.HasValue))
                {
                    var sumQtyByOrderItems = input.BillItems.Where(g => g.OrderItemId == v.OrderItemId).Sum(t => t.Qty);
                    await ValidateOrderQtyForReceipt(v.OrderItemId.Value, sumQtyByOrderItems, v.ItemReceiptId);
                }
            }

            int index = 0;
            foreach (var c in input.BillItems)
            {
                index++;
                if (c.LotId == null && c.ItemId != null && c.ItemTypeId != null && c.DisplayInventoryAccount)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }
            }

            ValidateExchangeRate(input);
            await CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository.GetAll().AsNoTracking().Include(u => u.Bill).Include(u => u.Bill.ItemReceipt)
                              .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
                              .FirstOrDefaultAsync();
            if (@journal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (journal.Bill.ItemReceipt != null && journal.Bill.CreationTime < journal.Bill.ItemReceipt.CreationTime && journal.Bill.ConvertToItemReceipt == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            await CheckClosePeriod(journal.Date, input.Date);
            journal.Update(tenantId, input.BillNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;

            }
            journal.UpdateMultiCurrency(input.MultiCurrencyId);


            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.AP && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);

            //update bill 
            var @bill = journal.Bill;// await _billManager.GetAsync(input.Id, true);
            var oldConvertItemReceipt = bill.ConvertToItemReceipt;
            var itemReceiptId = bill.ItemReceiptId;
            var originalReceiveFrom = bill.ReceiveFrom;
            // calculate balance and update 
            decimal amount = input.Total - bill.Total;
            @bill.UpdateOpenBalance(amount);
            //calculate balnce and Update multi currency
            var multiamount = input.MultiCurrencyTotal - bill.MultiCurrencyTotal;
            bill.UpdateMultiCurrencyOpenBalance(multiamount);


            var isItem = input.BillItems.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            @bill.Update(tenantId, input.ReceiveFrom, input.VendorId, input.DueDate,
                          // input.LocationId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.SubTotal, input.Tax, input.Total,
                          input.ItemReceiptId, input.ETA, input.ConvertToItemReceipt, input.RecieptDate,
                          input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal, isItem);

            var billItemIds = input.BillItems.Where(u => u.ItemId != null).Select(u => u.ItemId.Value);


            var itemReceiptItems = new List<ItemReceiptItem>();
            var journalItemForReceiptItems = new List<JournalItem>();

            //update journal of itemReceipt when receive from item receipt 
            if ((input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt && input.ItemReceiptId != null) ||
                (oldConvertItemReceipt == true && input.ConvertToItemReceipt == true)
            )
            {
                journalItemForReceiptItems = await _journalItemRepository.GetAll()
                                        .Include(u => u.Journal.ItemReceipt)
                                        .Where(u => u.Journal.ItemReceiptId == input.ItemReceiptId &&
                                                   u.Key == PostingKey.Inventory && u.Identifier != null)
                                        .ToListAsync();

                itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                                    .Where(u => u.ItemReceiptId == input.ItemReceiptId).ToListAsync();
            }

            //Check if bill list item has no item ID so it mean no receive status 
            //var isAccountTap = CheckIfItemExist(input.BillItems, null);
            if (input.ReceiveFrom == Bill.ReceiveFromStatus.None && billItemIds.Count() == 0)
            {
                bill.UpdateReceivedStatus(DeliveryStatus.NoReceive);
            }
            else if (input.ReceiveFrom == Bill.ReceiveFromStatus.None && billItemIds.Count() == 0 && bill.ReceivedStatus == DeliveryStatus.NoReceive)
            {
                bill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);// in some case user switch from tap account info to item info
            }
            else if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt || (input.ConvertToItemReceipt && billItemIds.Count() > 0))
            {
                bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
            }
            else if (input.ReceiveFrom != Bill.ReceiveFromStatus.ItemReceipt)
            {
                bill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
            }

            // update item receipt id to null if the orginal receive from receipt then user change to other item receipt must null
            if (originalReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt && input.ReceiveFrom != Bill.ReceiveFromStatus.ItemReceipt)
            {
                bill.UpdateItemReceiptid(null);
            }
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _billManager.UpdateAsync(@bill));


            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.BillId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if(exchange == null)
                {
                    exchange = BillExchangeRate.Create(tenantId, userId, bill.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, bill.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if(exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }


            //Update bill Item and Journal Item
            var billItems = await _billItemRepository.GetAll().Include(s => s.ItemReceiptItem.OrderItem).Where(u => u.BillId == input.Id).ToListAsync();

            var oldOrderIds = billItems
                           .Where(s => s.ItemReceiptItem != null && s.ItemReceiptItem.OrderItem != null)
                           .GroupBy(s => s.ItemReceiptItem.OrderItem.PurchaseOrderId)
                           .Select(s => s.Key).ToList();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Clearance && u.Identifier != null)).ToListAsync();

            var toDeleteBillItem = billItems.Where(u => !input.BillItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.BillItems.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            var subBillItems = new List<KeyValuePair<CreateOrUpdateBillItemInput, BillItem>>();

            //Update Item Receipt Item and Journal Item            
            foreach (var c in input.BillItems)
            {

                if (c.LotId == 0)
                {
                    c.LotId = null;
                }
                //validate when Currency as Multi Currrency

                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    c.MultiCurrencyTotal = c.Total;
                    c.MultiCurrencyUnitCost = c.UnitCost;
                }

                if (c.Id != null) //update
                {
                    var billItem = billItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (billItem != null)
                    {
                        //validate when currency as multi currency
                        if (input.CurrencyId == input.MultiCurrencyId)
                        {
                            c.MultiCurrencyTotal = c.Total;
                            c.MultiCurrencyUnitCost = c.MultiCurrencyUnitCost;
                        }
                        //new
                        billItem.Update(tenantId, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal);
                        billItem.UpdateLot(c.LotId);

                        if (input.ReceiveFrom == Bill.ReceiveFromStatus.PO && c.OrderItemId != null)
                        {
                            billItem.UpdateOrderItemId(c.OrderItemId);
                        }
                        CheckErrors(await _billItemManager.UpdateAsync(billItem));

                    }

                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, c.Total, 0);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to bill item
                    var @billItem = BillItem.Create(tenantId, userId, bill, c.TaxId, c.ItemId,
                                                    c.Description, c.Qty, c.UnitCost, c.DiscountRate,
                                                    c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal);
                    billItem.UpdateLot(c.LotId);
                    if (input.ReceiveFrom == Bill.ReceiveFromStatus.PO && c.OrderItemId != null)
                    {
                        if (c.Qty > c.OrginalQtyFromPurchase)
                        {
                            throw new UserFriendlyException(L("BillMessageWarning", c.ItemName) + L("Row") + " " + index.ToString());
                        }
                        billItem.UpdateOrderItemId(c.OrderItemId);
                    }
                    CheckErrors(await _billItemManager.CreateAsync(billItem));

                    if (c.Key.HasValue || c.ParentId.HasValue) subBillItems.Add(new KeyValuePair<CreateOrUpdateBillItemInput, BillItem>(c, billItem));

                    //insert inventory journal item into debit
                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.InventoryAccountId,
                                                            c.Description, c.Total, 0, PostingKey.Clearance, billItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    c.Id = billItem.Id;

                }

            }


            var keys = input.BillItems.Where(s => s.Key.HasValue).GroupBy(s => s.Key.Value).Select(s => s.Key).ToHashSet();
            foreach (var key in keys)
            {
                var parent = subBillItems.FirstOrDefault(s => s.Key.Key == key);
                var subitems = subBillItems.Where(s => s.Key.ParentId == key).Select(s => s.Value);
                foreach (var sub in subitems)
                {
                    sub.SetParent(parent.Value.Id);
                }
            }


            //delete itemReceiptItem 

            foreach (var t in toDeleteBillItem)
            {
                CheckErrors(await _billItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Bill };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            //update item receipt 
            if (oldConvertItemReceipt == true && input.ConvertToItemReceipt == true && input.Status == TransactionStatus.Publish)
            {

                var tenant = await GetCurrentTenantAsync();
                var ClearanceAccountId = tenant.TransitAccountId;
                if (ClearanceAccountId == null)
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                var inventoryAccount = await _itemRepository.GetAll()
                                            .Where(t => billItemIds.Contains(t.Id) &&
                                                        t.InventoryAccountId != null &&
                                                        t.ItemType.DisplayInventoryAccount)
                                            .AsNoTracking()
                                            .Select(u => new { u.Id, InventoryAccountId = u.InventoryAccountId.Value })
                                            .ToDictionaryAsync(u => u.Id);

                var itemLists = input.BillItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId.Value))
                                .Select(t => new CreateOrUpdateItemReceiptItemInput
                                {
                                    InventoryAccountId = inventoryAccount[t.ItemId.Value].InventoryAccountId,
                                    Description = t.Description,
                                    DiscountRate = t.DiscountRate,
                                    ItemId = t.ItemId.Value,
                                    //OrderItemId = t.OrderItemId,
                                    //PurchaseOrderId = t.PurchaseOrderId,
                                    Qty = t.Qty,
                                    Total = t.Total,
                                    UnitCost = t.UnitCost,
                                    BillItemId = t.Id,
                                    Id = t.ItemReceiptId,
                                    LotId = t.LotId,
                                    UseBatchNo = t.UseBatchNo,
                                    ItemBatchNos = t.ItemBatchNos,
                                }).ToList();

                var UpdateInputItemReceipt = new UpdateItemReceiptInput()
                {
                    Id = itemReceiptId.Value,
                    Memo = input.Memo,
                    ReceiptNo = input.BillNo,
                    BillId = bill.Id,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.RecieptDate,
                    LocationId = input.LocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.Bill,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    DateCompare = input.DateCompare,
                    Items = itemLists,
                    IsConfirm = input.IsConfirm,

                };
                var itemReceiptJournal = journalItemForReceiptItems.Select(u => u.Journal).FirstOrDefault();
                await UpdateItemReceipt(tenantId, userId, input.Id, UpdateInputItemReceipt, input.ConvertToItemReceipt, itemReceiptItems, itemReceiptJournal, journalItemForReceiptItems);
            }

            //delete item receipt when old data bill before true but now user change false then delete item receipt
            else if (oldConvertItemReceipt == true && input.ConvertToItemReceipt == false && input.Status == TransactionStatus.Publish)
            {
                var ReceiptId = new CarlEntityDto { IsConfirm = input.IsConfirm, Id = itemReceiptId.Value };
                await DeleteItemReceipt(ReceiptId);
            }

            // create item receipt while bill data before auto convert is false then user change to true so automatically create item receipt
            else if (oldConvertItemReceipt == false && input.ConvertToItemReceipt == true && input.Status == TransactionStatus.Publish)
            {

                var tenant = await GetCurrentTenantAsync();
                var ClearanceAccountId = tenant.TransitAccountId;
                if (ClearanceAccountId == null)
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                var inventoryAccount = await _itemRepository.GetAll()
                                            .Where(t => billItemIds.Contains(t.Id) &&
                                                        t.InventoryAccountId != null &&
                                                        t.ItemType.DisplayInventoryAccount)
                                            .AsNoTracking()
                                            .Select(u => new { u.Id, InventoryAccountId = u.InventoryAccountId.Value })
                                            .ToDictionaryAsync(u => u.Id);

                var itemLists = input.BillItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId.Value))
                                .Select(t => new CreateOrUpdateItemReceiptItemInput
                                {
                                    InventoryAccountId = inventoryAccount[t.ItemId.Value].InventoryAccountId,
                                    Description = t.Description,
                                    DiscountRate = t.DiscountRate,
                                    ItemId = t.ItemId.Value,
                                    //OrderItemId = t.OrderItemId,
                                    //PurchaseOrderId = t.PurchaseOrderId,
                                    Qty = t.Qty,
                                    Total = t.Total,
                                    UnitCost = t.UnitCost,
                                    BillItemId = t.Id,
                                    Id = t.ItemReceiptId,
                                    LotId = t.LotId,
                                    UseBatchNo = t.UseBatchNo,
                                    ItemBatchNos = t.ItemBatchNos,
                                }).ToList();
                var createInputItemREceipt = new CreateItemReceiptInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.BillNo,
                    BillId = bill.Id,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.RecieptDate,
                    LocationId = input.LocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.Bill,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    Items = itemLists,
                };
                await CreateItemReceipt(createInputItemREceipt);
            }


            if (input.ReceiveFrom == Bill.ReceiveFromStatus.ItemReceipt && input.ItemReceiptId != null)
            {
                var itemreceipt = await _itemReceiptRepository.GetAll().Where(t => t.Id == input.ItemReceiptId).FirstOrDefaultAsync();
                var tenant = await GetCurrentTenantAsync();
                var ClearanceAccountId = tenant.TransitAccountId;
                if (ClearanceAccountId == null)
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                var inventoryAccount = await _itemRepository.GetAll()
                                            .Where(t => billItemIds.Contains(t.Id) &&
                                                        t.InventoryAccountId != null &&
                                                        t.ItemTypeId != Item_Type_Service)
                                            .AsNoTracking()
                                            .Select(u => new { u.Id, InventoryAccountId = u.InventoryAccountId.Value })
                                            .ToDictionaryAsync(u => u.Id);

                var itemLists = input.BillItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId.Value))
                                .Select(t => new CreateOrUpdateItemReceiptItemInput
                                {
                                    InventoryAccountId = inventoryAccount[t.ItemId.Value].InventoryAccountId,
                                    Description = t.Description,
                                    DiscountRate = t.DiscountRate,
                                    ItemId = t.ItemId.Value,
                                    OrderItemId = t.OrderItemId,
                                    PurchaseOrderId = t.PurchaseOrderId,
                                    Qty = t.Qty,
                                    Total = t.Total,
                                    UnitCost = t.UnitCost,
                                    BillItemId = t.Id,
                                    Id = t.ItemReceiptId,
                                    LotId = t.LotId,
                                    UseBatchNo = t.UseBatchNo,
                                    ItemBatchNos = t.ItemBatchNos,
                                }).ToList();
                var inputItemReceipt = new UpdateItemReceiptInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.BillNo,
                    BillId = bill.Id,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.RecieptDate,
                    LocationId = input.LocationId,
                    ReceiveFrom = itemreceipt.ReceiveFrom,
                    Reference = input.Reference,
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    Items = itemLists,
                    Id = input.ItemReceiptId.Value
                };
                var itemReceiptJournal = journalItemForReceiptItems.Select(u => u.Journal).FirstOrDefault();
                await UpdateItemReceipt(tenantId, userId, input.Id, inputItemReceipt, input.ConvertToItemReceipt,
                            itemReceiptItems, itemReceiptJournal, journalItemForReceiptItems);
            }


            var newOrderIds = input.BillItems.Where(s => s.PurchaseOrderId.HasValue).GroupBy(s => s.PurchaseOrderId).Select(s => s.Key.Value);

            var orders = oldOrderIds.Union(newOrderIds).ToList();

            if (orders.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();

                foreach (var orderId in orders)
                {
                    await UpdatePurhaseOrderInventoryStatus(orderId);
                }
            }

            return new NullableIdDto<Guid>() { Id = bill.Id };
        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToDraft)]
        //public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        //{
        //    //query select convertitemReceipt             
        //    var @entity = await _journalRepository
        //                       .GetAll()
        //                       .Include(u => u.Bill)
        //                       .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
        //                       .FirstOrDefaultAsync();

        //    if (@entity.Bill.ConvertToItemReceipt == true)
        //    {
        //        var itemReceiptId = await _billRepository.GetAll().Where(i => i.Id == input.Id).Select(t => t.ItemReceiptId).FirstOrDefaultAsync();
        //        var @jounal = await _journalRepository
        //                                        .GetAll()
        //                                        .Include(u => u.ItemReceipt)
        //                                        .Include(u => u.ItemReceipt.BillingAddress)
        //                                        .Include(u => u.ItemReceipt.ShippingAddress)
        //                                        .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
        //                                            u.JournalType == JournalType.ItemReceiptAdjustment ||
        //                                            u.JournalType == JournalType.ItemReceiptOther ||
        //                                            u.JournalType == JournalType.ItemReceiptTransfer)
        //                                            && u.ItemReceipt.Id == itemReceiptId)
        //                                        .FirstOrDefaultAsync();

        //        if (@jounal == null)
        //        {
        //            throw new UserFriendlyException(L("RecordNotFound"));
        //        }
        //        var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

        //        if (jounal.JournalNo == auto.LastAutoSequenceNumber)
        //        {
        //            var jo = await _journalRepository.GetAll()
        //                .Where(t => t.Id != jounal.Id &&
        //                                 (t.JournalType == JournalType.ItemReceiptPurchase ||
        //                                 t.JournalType == JournalType.ItemReceiptAdjustment ||
        //                                 t.JournalType == JournalType.ItemReceiptOther ||
        //                                 t.JournalType == JournalType.ItemReceiptTransfer ||
        //                                 t.JournalType == JournalType.ItemReceiptProduction))
        //                .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
        //            if (jo != null)
        //            {
        //                auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
        //            }
        //            else
        //            {
        //                auto.UpdateLastAutoSequenceNumber(null);
        //            }
        //            CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
        //        }
        //        //query get item Receipt 
        //        var @entityItem = @jounal.ItemReceipt;

        //        //qurey check creationTime Item Receipt and Bill
        //        var UpdateItemReceiptIdOnBill = await _billRepository.GetAll()
        //          .Include(u => u.ItemReceipt)
        //          .Where(u => u.ItemReceiptId == itemReceiptId).FirstOrDefaultAsync();

        //        if (UpdateItemReceiptIdOnBill != null)
        //        {
        //            UpdateItemReceiptIdOnBill.UpdateItemReceiptid(null);
        //            UpdateItemReceiptIdOnBill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
        //            CheckErrors(await _billManager.UpdateAsync(UpdateItemReceiptIdOnBill));
        //        }
        //        var @itemReceipts = (from itemReceipt in _itemReceiptRepository.GetAll()
        //                             .Where(u => u.Id == itemReceiptId)
        //                             join bill in _billRepository.GetAll() on itemReceipt.Id equals bill.ItemReceiptId
        //                             where (itemReceipt.CreationTime < bill.CreationTime)
        //                             select itemReceipt);


        //        if (itemReceipts != null && itemReceipts.Count() > 0)
        //        {
        //            throw new UserFriendlyException(L("ItemReceiptMessage"));
        //        }

        //        //query get journal and delete

        //        @jounal.UpdateItemReceipt(null);

        //        //query get journal item and delete
        //        var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
        //        foreach (var ji in jounalItems)
        //        {
        //            CheckErrors(await _journalItemManager.RemoveAsync(ji));
        //        }


        //        CheckErrors(await _journalManager.RemoveAsync(@jounal));

        //        //query get item receipt item and delete 
        //        var @itemReceiptItems = await _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem)
        //            .Where(u => u.ItemReceiptId == @entityItem.Id).ToListAsync();

        //        var queryPOHeader = (from po in _purhcaseOrderRepository.GetAll()
        //                             join pi in _purhcaseOrderItemRepository.GetAll() on po.Id equals pi.PurchaseOrderId
        //                             where (@itemReceiptItems.Any(t => t.OrderItemId == pi.Id))
        //                             group pi by po into u
        //                             select new { poId = u.Key.Id }
        //             );

        //        // temp of po id header 
        //        var listOfPoHeader = new List<CreateOrUpdateItemReceiptItemInput>();

        //        foreach (var i in queryPOHeader)
        //        {
        //            listOfPoHeader.Add(new CreateOrUpdateItemReceiptItemInput
        //            {
        //                PurchaseOrderId = i.poId
        //            });
        //        }

        //        //Update itemReceiptitemId on table billitem
        //        var updateitemReceiptitemId = (from billitem in _billItemRepository.GetAll()
        //                                       join itemReceiptItem in _itemReceiptItemRepository.GetAll()
        //                                       .Include(u => u.ItemReceipt)
        //                                       on billitem.ItemReceiptItemId equals itemReceiptItem.Id
        //                                       where (itemReceiptItem.ItemReceiptId == @entityItem.Id)
        //                                       select billitem);
        //        foreach (var u in updateitemReceiptitemId)
        //        {
        //            u.UpdateReceiptItemId(null);
        //            CheckErrors(await _billItemManager.UpdateAsync(u));
        //        }

        //        // query to get list of item which is receive form status == bill and bill item is has order id 
        //        var getPOIdInBillItem = (from iri in _itemReceiptItemRepository.GetAll()
        //                                 .Where(u => u.ItemReceiptId == @entityItem.Id)
        //                                 .Include(u => u.OrderItem)
        //                                 join bir in _billItemRepository.GetAll()
        //                                 .Include(u => u.Bill)
        //                                 .Include(u => u.OrderItem)
        //                                 .Include(u => u.OrderItem.PurchaseOrder)
        //                                 on iri.Id equals bir.ItemReceiptItemId
        //                                 where (listOfPoHeader.Any(t => t.PurchaseOrderId == bir.OrderItem.PurchaseOrderId))
        //                                 select new { iri, bir }
        //                                 ).ToList();

        //        foreach (var iri in @itemReceiptItems)
        //        {

        //            CheckErrors(await _itemReceiptItemManager.RemoveAsync(iri));

        //        }

        //        CheckErrors(await _itemReceiptManager.RemoveAsync(@entityItem));
        //    }

        //    //old code delete not yet convert 

        //    // validate from Paybill
        //    var validatefrompayBill = (from paybillDetail in _payBillDetailRepository.GetAll().Where(t => t.BillId == input.Id) select paybillDetail).ToList().Count();
        //    if (validatefrompayBill > 0)
        //    {
        //        throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
        //    }

        //    //validate bill 
        //    var validatBill = (from billItemIssue in _billRepository.GetAll().Include(t => t.ItemReceipt)
        //                       .Where(t => t.Id == input.Id
        //                       && t.ConvertToItemReceipt == false
        //                       && t.CreationTime < t.ItemReceipt.CreationTime
        //                       && t.ItemReceiptId != null)
        //                       select billItemIssue.ItemReceiptId).ToList().Count();
        //    if (validatBill > 0 && @entity.Bill.ConvertToItemReceipt == false)
        //    {
        //        throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
        //    }



        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }

        //    entity.UpdateStatusToDraft();

        //    var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);
        //    CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));
        //    return new NullableIdDto<Guid>() { Id = entity.Id };
        //}

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {

            //validate bill 
            var validatbill = (from billItemIssue in _billRepository.GetAll().Include(t => t.ItemReceipt)
                               .Where(t => t.Id == input.Id
                                    && t.ItemReceiptId != null
                                    && t.CreationTime < t.ItemReceipt.CreationTime)
                               select billItemIssue.ItemReceiptId).ToList().Count();
            if (validatbill > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            // validate from Paybill
            var validatefrompayBill = (from paybillDetail in _payBillDetailRepository.GetAll().Where(t => t.BillId == input.Id) select paybillDetail).ToList().Count();
            if (validatefrompayBill > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.Bill)
                                .Include(u => u.Bill.BillingAddress)
                                .Include(u => u.Bill.ShippingAddress)
                                .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdatePublish();


            if (entity.Bill.ConvertToItemReceipt == true)
            {
                var tenantId = AbpSession.GetTenantId();
                var userId = AbpSession.GetUserId();

                var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.TransitAccountId).FirstOrDefaultAsync();

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                }
                //var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.BillItems.Any(i => i.ItemId == g.Id)).ToListAsync();

                var createInputItemREceipt = new CreateItemReceiptInput()
                {
                    Memo = entity.Memo,
                    ReceiptNo = entity.JournalNo,
                    BillId = entity.Bill.Id,
                    BillingAddress = entity.Bill.BillingAddress,
                    SameAsShippingAddress = entity.Bill.SameAsShippingAddress,
                    ShippingAddress = entity.Bill.ShippingAddress,
                    ClassId = entity.ClassId,
                    CurrencyId = entity.CurrencyId,
                    Date = entity.Bill.ItemReceiptDate.Value,
                    LocationId = entity.LocationId.Value,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.Bill,
                    Reference = entity.Reference,
                    Status = entity.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    Total = entity.Bill.Total,
                    VendorId = entity.Bill.VendorId,
                    Items = _billItemRepository.GetAll()
                               .Include(u => u.OrderItem)
                               .Include(u => u.OrderItem.PurchaseOrder)
                               .Include(u => u.Item)
                            .AsNoTracking()
                        .Where(t => t.BillId == entity.BillId && t.Item.ItemTypeId != 3).Select(t => new CreateOrUpdateItemReceiptItemInput
                        {
                            InventoryAccountId = t.Item.InventoryAccountId.Value,
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            ItemId = t.ItemId.Value,
                            //OrderItemId = t.OrderItemId,
                            //PurchaseOrderId = t.OrderItem.PurchaseOrderId,
                            Qty = t.Qty,
                            Total = t.Total,
                            UnitCost = t.UnitCost,
                            BillItemId = t.Id,

                        }).ToList()
                };
                await CreateItemReceipt(createInputItemREceipt);
            }

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            //validate bill 
            var validatBill = (from billItemIssue in _billRepository.GetAll().Include(t => t.ItemReceipt)
                                .Where(t => t.Id == input.Id
                                && t.CreationTime < t.ItemReceipt.CreationTime
                                && t.ItemReceiptId != null && t.ConvertToItemReceipt == false)
                               select billItemIssue.ItemReceiptId).ToList().Count();
            if (validatBill > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            // validate from Paybill
            var validatefrompayBill = (from paybillDetail in _payBillDetailRepository.GetAll()
                                       .Where(t => t.BillId == input.Id)
                                       select paybillDetail).ToList().Count();
            if (validatefrompayBill > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }

            var @jounal = await _journalRepository.GetAll()
                .Include(u => u.ItemReceipt)
                .Include(u => u.Bill)
                .Include(u => u.Bill.ShippingAddress)
                .Include(u => u.Bill.BillingAddress)
                .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
                .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var billItems = await _billItemRepository.GetAll().Include(u => u.OrderItem)
                .Where(u => u.BillId == jounal.BillId).ToListAsync();

            //update status Item receipt tov void by auto convert
            if (jounal.Bill.ConvertToItemReceipt == true)
            {
                var @jounalItemReceipt = await _journalRepository
               .GetAll()
               .Include(u => u.ItemReceipt)
               .Include(u => u.Bill)
               .Include(u => u.ItemReceipt.ShippingAddress)
               .Include(u => u.ItemReceipt.BillingAddress)
                            .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                u.JournalType == JournalType.ItemReceiptAdjustment ||
                                u.JournalType == JournalType.ItemReceiptOther ||
                                u.JournalType == JournalType.ItemReceiptTransfer)
                                && u.ItemReceipt.Id == jounal.Bill.ItemReceiptId)
                            .FirstOrDefaultAsync();


                // validate item reciept type sale by bill
                var validate = (from bill in _billRepository.GetAll().Include(t => t.ItemReceipt)
                                .Where(t => t.ItemReceiptId == jounal.Bill.ItemReceiptId
                                && t.ItemReceiptId != null
                                && t.CreationTime > t.ItemReceipt.CreationTime
                                && jounal.JournalType == JournalType.ItemReceiptPurchase)
                                select bill).ToList().Count();
                if (validate > 0)
                {
                    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                }

                if (@jounalItemReceipt.JournalType == JournalType.ItemReceiptPurchase)
                {
                    //query get item receipt item and update totalbillQty 
                    var bill = (_billRepository.GetAll().Where(u => u.ItemReceiptId == jounal.ItemReceiptId)).FirstOrDefault();

                    if (@jounalItemReceipt.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
                    {
                        bill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                        CheckErrors(await _billManager.UpdateAsync(bill));
                    }

                    //query get item receipt item and update totalItemReceipt 
                    var @itemReceiptItems = await _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem)
                        .Where(u => u.ItemReceiptId == jounalItemReceipt.ItemReceiptId).ToListAsync();
                    var queryPOHeader = (from po in _purhcaseOrderRepository.GetAll()
                                         join pi in _purhcaseOrderItemRepository.GetAll().Include(u => u.Item) on po.Id equals pi.PurchaseOrderId
                                         where (@itemReceiptItems.Any(t => t.OrderItemId == pi.Id))
                                         group pi by po into u
                                         select new { poId = u.Key.Id }
                        );

                    if (jounalItemReceipt.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && jounalItemReceipt.ItemReceipt != null)
                    {

                    }
                }

                jounalItemReceipt.UpdateVoid();
            }
            jounal.UpdateVoid();

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);
            CheckErrors(await _journalManager.UpdateAsync(@jounal, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = jounal.ItemReceiptId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetBillItems)]
        public async Task<BillSummaryOutputForGetBillItem> GetBillItems(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                  .GetAll()
                                  .Include(u => u.Currency)
                                  .Include(u => u.Bill)
                                  .Include(u => u.Currency)
                                  .Include(u => u.Location)
                                  .Include(u => u.Bill.Vendor)
                                  .Include(u => u.Bill.Vendor.ChartOfAccount)
                                  .Include(u => u.Class)
                                  .Include(u => u.Bill.ShippingAddress)
                                  .Include(u => u.Bill.BillingAddress)
                                  .AsNoTracking()
                                  .Where(u => u.JournalType == JournalType.Bill && u.BillId == input.Id)
                                  .FirstOrDefaultAsync();



            if (@journal == null || @journal.Bill == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.AP && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var billItems = await _billItemRepository.GetAll()
                .Include(u => u.Tax)
                .Include(u => u.ItemReceiptItem)
                .Include(u => u.Item)
                .Include(u => u.Item.InventoryAccount)
                .Include(u => u.OrderItem)
                .Include(u => u.Tax)
                .Include(u => u.Lot)
                .Where(u => u.BillId == input.Id && u.Item.InventoryAccountId != null)
                .Join(_journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(), u => u.Id, s => s.Identifier,
                (bItem, jItem) =>
                new BillItemSummaryOutput()
                {
                    CreationTime = bItem.CreationTime,
                    Id = bItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryDetailOutput>(bItem.Item),
                    PurchaseOrderId = bItem.OrderItem != null ? bItem.OrderItem.PurchaseOrderId : (Guid?)null,
                    ItemId = bItem.ItemId,
                    OrderItemId = bItem.OrderItemId,
                    Description = bItem.Description,
                    Qty = bItem.Qty,
                    Tax = ObjectMapper.Map<TaxSummaryOutput>(bItem.Tax),
                    Total = bItem.Total,
                    UnitCost = bItem.UnitCost,
                    TaxId = bItem.TaxId,
                    InventoryAccountId = jItem.AccountId,
                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                    LotId = bItem.LotId,
                    LotDetail = ObjectMapper.Map<LotSummaryOutput>(bItem.Lot),
                    UseBatchNo = bItem.Item != null && bItem.Item.UseBatchNo,
                    AutoBatchNo = bItem.Item != null && bItem.Item.AutoBatchNo,
                    TrackSerial = bItem.Item != null && bItem.Item.TrackSerial,
                    TrackExpiration = bItem.Item != null && bItem.Item.TrackExpiration,
                }).OrderBy(u => u.CreationTime)
                .ToListAsync();
            var result = ObjectMapper.Map<BillSummaryOutputForGetBillItem>(journal.Bill);
            result.BillNo = journal.JournalNo;
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.BillItems = billItems;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            return result;
        }

        #region import export excel

        private ReportOutput GetReportTemplateItemReceiptOther(bool hasClassFeature)
        {

            var columns = new List<CollumnOutput>() {
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "BillGroup",
                    ColumnLength = 180,
                    ColumnTitle = "Bill Group",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Date",
                    ColumnLength = 180,
                    ColumnTitle = "Date",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "VendorCode",
                    ColumnLength = 230,
                    ColumnTitle = "Vendor Code",
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
                    ColumnName = "APAccount",
                    ColumnLength = 130,
                    ColumnTitle = "AP Account",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Location",
                    ColumnLength = 250,
                    ColumnTitle = "Location",
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
                    ColumnName = "Class",
                    ColumnLength = 200,
                    ColumnTitle = "Class",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = hasClassFeature,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Memo",
                    ColumnLength = 250,
                    ColumnTitle = "Memo",
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
                    ColumnName = "Reference",
                    ColumnLength = 250,
                    ColumnTitle = "Reference",
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
                    ColumnName = "BillNo",
                    ColumnLength = 250,
                    ColumnTitle = "BillNo",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Currency",
                    ColumnLength = 130,
                    ColumnTitle = "Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Amount",
                    ColumnLength = 250,
                    ColumnTitle = "Amount",
                    ColumnType = ColumnType.Money,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "AmountInAccounting",
                    ColumnLength = 250,
                    ColumnTitle = "Amount In Accounting",
                    ColumnType = ColumnType.String,
                    SortOrder =11,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemAccount",
                    ColumnLength = 130,
                    ColumnTitle = "Item Account",
                    ColumnType = ColumnType.String,
                    SortOrder = 12,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemDescription",
                    ColumnLength = 130,
                    ColumnTitle = "Item Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 13,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

            };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.WhereIf(!hasClassFeature, s => s.ColumnName != "Class").ToList(),
                Groupby = "",
                HeaderTitle = "Bill",
                Sortby = "",
            };

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_ExportExcelBillTamplate)]
        public async Task<FileDto> ExportExcelTamplate()
        {
            var result = new FileDto();
            var sheetName = "Bill";
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
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
                var headerList = GetReportTemplateItemReceiptOther(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"BillTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_ImportExcel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            if (excelPackage == null) return;

            AccountCycle currentCycle = null;
            Tenant tenant = null;
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
            var indexHasClassFeature = hasClassFeature ? 0 : -1;
            var @accounts = new List<ChartOfAccountSummaryWithTax>();
            var @locations = new List<NameValueDto<long>>();
            var classes = new List<NameValueDto<long>>();
            var currencies = new List<NameValueDto<long>>();
            var vendors = new List<NameValueDto<Guid>>();
            var billJournalHash = new HashSet<string>();
            AutoSequence billAuto = null;
            bool useExchangeRate = false;
            

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    currentCycle = await GetCurrentCycleAsync();
                    tenant = await GetCurrentTenantAsync();

                    @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().Select(s => new ChartOfAccountSummaryWithTax { AccountCode = s.AccountCode, Id = s.Id, TaxId = s.TaxId }).ToListAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.LocationName, Value = s.Id }).ToListAsync();
                    classes = await _classRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.ClassName, Value = s.Id }).ToListAsync();
                    currencies = await _currencyRepository.GetAll().Select(s => new NameValueDto<long> { Name = s.Code, Value = s.Id }).AsNoTracking().ToListAsync();
                    vendors = await _vendorRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<Guid> { Name = s.VendorCode, Value = s.Id }).ToListAsync();
                    billJournalHash = (await _journalRepository.GetAll().Where(s => s.JournalType == JournalType.Bill).Select(s => s.JournalNo).ToListAsync()).ToHashSet();

                    billAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);
                }
            }

            var addBillItems = new List<BillItem>();
            var addBills = new List<Bill>();
            var addBillJournalItems = new List<JournalItem>();
            var addBillJournals = new List<Journal>();

            var billDic = new Dictionary<string, Bill>();

            // Get the work book in the file
            var workBook = excelPackage.Workbook;
            if (workBook != null)
            {
                // retrive first worksheets
                var worksheet = excelPackage.Workbook.Worksheets[0];

                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var billGroup = worksheet.Cells[i, 1].Value?.ToString();
                    var date = worksheet.Cells[i, 2].Value?.ToString();
                    var vendorCode = worksheet.Cells[i, 3].Value?.ToString();
                    var aPAccountCode = worksheet.Cells[i, 4].Value?.ToString();
                    var locationName = worksheet.Cells[i, 5].Value?.ToString();
                    var className = hasClassFeature ? worksheet.Cells[i, 6].Value?.ToString() : "";
                    var memo = worksheet.Cells[i, 7 + indexHasClassFeature].Value?.ToString();
                    var reference = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                    var billNo = worksheet.Cells[i, 9 + indexHasClassFeature].Value?.ToString();
                    var currencyCode = worksheet.Cells[i, 10 + indexHasClassFeature].Value?.ToString();
                    var amount = worksheet.Cells[i, 11 + indexHasClassFeature].Value.ToString();
                    var amountInAccount = worksheet.Cells[i, 12 + indexHasClassFeature].Value.ToString();
                    var itemAccountCode = worksheet.Cells[i, 13 + indexHasClassFeature].Value?.ToString();
                    var itemDescription = worksheet.Cells[i, 14 + indexHasClassFeature].Value?.ToString();

                    var currency = currencies.Where(s => s.Name == currencyCode).FirstOrDefault();
                    var defaultClass = hasClassFeature ? classes.Where(s => s.Name == className).Select(t => t.Value).FirstOrDefault() : tenant.ClassId;
                    var location = locations.Where(s => s.Name == locationName).FirstOrDefault();
                    var vendor = vendors.Where(s => s.Name == vendorCode).FirstOrDefault();
                    var apAccount = accounts.Where(s => s.AccountCode == aPAccountCode).FirstOrDefault();
                    var itemAccount = accounts.Where(s => s.AccountCode == itemAccountCode).FirstOrDefault();

                    if (billAuto.CustomFormat == false && billNo == null)
                    {
                        throw new UserFriendlyException(L("IsRequired", L("BillNo")) + $", Row = {i}");
                    }

                    if (billAuto.RequireReference && reference.IsNullOrWhiteSpace())
                    {
                        throw new UserFriendlyException(L("IsRequired", L("Reference")) + $", Row = {i}");
                    }
                    if (string.IsNullOrWhiteSpace(date))
                    {
                        throw new UserFriendlyException(L("IsRequired", L("Date")) + $", Row = {i}");
                    }
                    if (currency == null)
                    {
                        throw new UserFriendlyException(L("NoCurrencyFound") + $", Row = {i}");
                    }
                    if (defaultClass == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound") + $", Row = {i}");
                    }
                    if (vendor == null)
                    {
                        throw new UserFriendlyException(L("NoVendorFound") + $", Row = {i}");
                    }

                    if (apAccount == null)
                    {
                        throw new UserFriendlyException(L("NoAPAccountFound") + $", Row = {i}");
                    }

                    if (itemAccount == null)
                    {
                        throw new UserFriendlyException(L("NoItemAccountFound") + $", Row = {i}");
                    }

                    if (location == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound") + $", Row = {i}");
                    }

                    var qty = 1;
                    var unitPrice = Math.Round(Convert.ToDecimal(amount), currentCycle.RoundingDigitUnitCost);
                    var unitPriceInAccount = Math.Round(Convert.ToDecimal(amountInAccount), currentCycle.RoundingDigitUnitCost);
                    var totalInAcc = Math.Round(qty * unitPriceInAccount, currentCycle.RoundingDigit);
                    var totalInTran = Math.Round(qty * unitPrice, currentCycle.RoundingDigit);

                    var addBillItem = BillItem.Create(tenantId, userId, Guid.Empty, itemAccount.TaxId, null, itemDescription, qty, unitPriceInAccount, 0, totalInAcc, unitPrice, totalInTran);
                    var addBillJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, itemAccount.Id, itemDescription, totalInAcc, 0, PostingKey.Clearance, addBillItem.Id);

                    addBillItems.Add(addBillItem);
                    addBillJournalItems.Add(addBillJournalItem);

                    if (billDic.ContainsKey(billGroup))
                    {
                        var addBill = billDic[billGroup];
                        addBill.SetSubTotal(addBill.SubTotal + totalInAcc);
                        addBill.SetTotal(addBill.Total + totalInAcc);
                        addBill.SetMultiCurrencySubTotal(addBill.MultiCurrencySubTotal + totalInTran);
                        addBill.SetMultiCurrencyTotal(addBill.MultiCurrencyTotal + totalInTran);
                        addBill.SetOpenBalance(addBill.Total);
                        addBill.SetMultiCurrencyOpenBalance(addBill.MultiCurrencyTotal);

                        var addBillJournal = addBillJournals.FirstOrDefault(s => s.BillId == addBill.Id);
                        addBillJournal.SetDebitCredit(addBill.Total);

                        var apAccountItem = addBillJournalItems.FirstOrDefault(s => s.JournalId == addBillJournal.Id && s.Key == PostingKey.AP);
                        apAccountItem.SetDebitCredit(0, addBill.Total);

                        addBillItem.SetBill(addBill.Id);
                        addBillJournalItem.SetJournal(addBillJournal.Id);
                    }
                    else
                    {

                        if (billAuto.CustomFormat == true)
                        {
                            var newAuto = _autoSequenceManager.GetNewReferenceNumber(billAuto.DefaultPrefix, billAuto.YearFormat.Value,
                                           billAuto.SymbolFormat, billAuto.NumberFormat, billAuto.LastAutoSequenceNumber, DateTime.Now);

                            billNo = newAuto;
                            billAuto.UpdateLastAutoSequenceNumber(newAuto);
                        }
                        else if (billJournalHash.Contains(billNo) || addBillJournals.Any(s => s.JournalNo == billNo))
                        {
                            throw new UserFriendlyException(L("DuplicateBillNo", billNo) + $", Row = {i}");
                        }

                        CAddress billAddress = new CAddress("", "", "", "", "");
                        CAddress shipAddress = new CAddress("", "", "", "", "");

                        var addBill = Bill.Create(tenantId, userId, Bill.ReceiveFromStatus.None, Convert.ToDateTime(date), vendor.Value, true, shipAddress, billAddress, totalInAcc, 0, totalInAcc, null, Convert.ToDateTime(date), false, Convert.ToDateTime(date), totalInTran, 0, totalInTran, false, useExchangeRate);
                        var addBillJournal = Journal.Create(tenantId, userId, billNo, Convert.ToDateTime(date), memo, totalInAcc, totalInAcc, tenant.CurrencyId.Value, defaultClass.Value, reference, location.Value);
                        var apJournalItem = JournalItem.CreateJournalItem(tenantId, userId, addBillJournal.Id, apAccount.Id, memo, 0, totalInAcc, PostingKey.AP, null);

                        addBillJournal.UpdateMultiCurrency(currency.Value);
                        addBillJournal.UpdateBill(addBill.Id);
                        addBillJournal.UpdateStatus(TransactionStatus.Publish);

                        addBills.Add(addBill);
                        addBillJournals.Add(addBillJournal);
                        addBillJournalItems.Add(apJournalItem);

                        addBillItem.SetBill(addBill.Id);
                        addBillJournalItem.SetJournal(addBillJournal.Id);

                        billDic.Add(billGroup, addBill);
                    }
                }
            }


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (addBills.Any()) await _billRepository.BulkInsertAsync(addBills);
                    if (addBillItems.Any()) await _billItemRepository.BulkInsertAsync(addBillItems);
                    if (addBillJournals.Any()) await _journalRepository.BulkInsertAsync(addBillJournals);
                    if (addBillJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addBillJournalItems);

                    if (billAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(billAuto));
                }

                await uow.CompleteAsync();
            }

        }

        #endregion


        #region Import Excel Items

        private ReportOutput GetReportTemplateBillItems(bool hasClassFeature)
        {
            var columns = new List<CollumnOutput>()
            {
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "BillGroup",
                    ColumnLength = 180,
                    ColumnTitle = "Bill Group",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Date",
                    ColumnLength = 180,
                    ColumnTitle = "Bill Date",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ItemReceiptDate",
                    ColumnLength = 180,
                    ColumnTitle = "Receipt Date",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "VendorCode",
                    ColumnLength = 230,
                    ColumnTitle = "Vendor Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "APAccount",
                    ColumnLength = 130,
                    ColumnTitle = "AP Account",
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
                    ColumnName = "Location",
                    ColumnLength = 250,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Class",
                    ColumnLength = 200,
                    ColumnTitle = "Class",
                    ColumnType = ColumnType.String,
                    SortOrder = 6,
                    Visible = hasClassFeature,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Currency",
                    ColumnLength = 130,
                    ColumnTitle = "Currency",
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
                    ColumnName = "Memo",
                    ColumnLength = 250,
                    ColumnTitle = "Memo",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Reference",
                    ColumnLength = 250,
                    ColumnTitle = "Reference",
                    ColumnType = ColumnType.String,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "BillNo",
                    ColumnLength = 250,
                    ColumnTitle = "Bill No",
                    ColumnType = ColumnType.String,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemCode",
                    ColumnLength = 250,
                    ColumnTitle = "Item Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 11,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Zone",
                    ColumnLength = 250,
                    ColumnTitle = "Zone",
                    ColumnType = ColumnType.String,
                    SortOrder = 12,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Qty",
                    ColumnLength = 250,
                    ColumnTitle = "Qty",
                    ColumnType = ColumnType.Number,
                    SortOrder = 13,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "UnitPriceInTranCurrency",
                    ColumnLength = 250,
                    ColumnTitle = "Unit Price in Tran. Currency",
                    ColumnType = ColumnType.Money,
                    SortOrder = 14,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "UnitPriceInAccCurrency",
                    ColumnLength = 250,
                    ColumnTitle = "Unit Price In Acc. Currency",
                    ColumnType = ColumnType.String,
                    SortOrder =15,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemDescription",
                    ColumnLength = 130,
                    ColumnTitle = "Item Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 16,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "BatchSerial",
                    ColumnLength = 130,
                    ColumnTitle = "Batch/Serial",
                    ColumnType = ColumnType.String,
                    SortOrder = 17,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Expiration",
                    ColumnLength = 130,
                    ColumnTitle = "Expiration",
                    ColumnType = ColumnType.Date,
                    SortOrder = 18,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                }
            };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.WhereIf(!hasClassFeature, s => s.ColumnName != "Class").ToList(),
                Groupby = "",
                HeaderTitle = "Bill",
                Sortby = "",
            };

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_ExportExcelBillTamplate)]
        public async Task<FileDto> ExportExcelItemsTamplate()
        {
            var result = new FileDto();
            var sheetName = "Bill";
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
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
                var headerList = GetReportTemplateBillItems(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"BillTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_ImportExcel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelItems(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            if (excelPackage == null) return;

            AccountCycle currentCycle = null;
            Tenant tenant = null;
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
            var indexHasClassFeature = hasClassFeature ? 0 : -1;
            var @accounts = new List<ChartOfAccountSummaryWithTax>();
            var @locations = new List<NameValueDto<long>>();
            var @lots = new List<ItemLotDto>();
            var classes = new List<NameValueDto<long>>();
            var currencies = new List<NameValueDto<long>>();
            var vendors = new List<NameValueDto<Guid>>();
            var billJournalHash = new List<JournalRefWithPartnerDto>();
            var itemReceiptJournalHash = new HashSet<string>();
            var items = new List<ItemSummaryWithAccount>();
            var batchNos = new List<BatchNoWithExpiration>();
            AutoSequence billAuto = null;
            AutoSequence receiptAuto = null;
            Guid? transactionTypeId = null;
            bool useExchangeRate = false;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    currentCycle = await GetCurrentCycleAsync();
                    tenant = await GetCurrentTenantAsync();

                    @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().Select(s => new ChartOfAccountSummaryWithTax { AccountCode = s.AccountCode, Id = s.Id, TaxId = s.TaxId }).ToListAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.LocationName, Value = s.Id }).ToListAsync();
                    @lots = await _lotRepository.GetAll().AsNoTracking().Select(s => new ItemLotDto { Id = s.Id, LotName = s.LotName, LocationId = s.LocationId }).ToListAsync();
                    classes = await _classRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.ClassName, Value = s.Id }).ToListAsync();
                    currencies = await _currencyRepository.GetAll().Select(s => new NameValueDto<long> { Name = s.Code, Value = s.Id }).AsNoTracking().ToListAsync();
                    vendors = await _vendorRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<Guid> { Name = s.VendorCode, Value = s.Id }).ToListAsync();

                    billJournalHash = await _journalRepository.GetAll().AsNoTracking()
                                               .Where(t => t.JournalType == JournalType.Bill)
                                               .Select(s => new JournalRefWithPartnerDto
                                               {
                                                   JournalNo = s.JournalNo,
                                                   Reference = s.Reference,
                                                   PartnerId = s.Bill.VendorId
                                               })
                                               .ToListAsync();

                    itemReceiptJournalHash = (await _journalRepository.GetAll().Where(s => s.ItemReceiptId.HasValue).Select(s => s.JournalNo).ToListAsync()).ToHashSet();
                    batchNos = await _batchNoRepository.GetAll().AsNoTracking().Select(s => new BatchNoWithExpiration { Id = s.Id, BatchNumber = s.Code, ExpirationDate = s.ExpirationDate, ItemId = s.ItemId }).ToListAsync();
                    items = await _itemRepository.GetAll().AsNoTracking().Select(s => new ItemSummaryWithAccount
                    {
                        Id = s.Id,
                        ItemCode = s.ItemCode,
                        PurchaseAccountId = s.PurchaseAccountId,
                        PurchaseTaxId = s.PurchaseTaxId,
                        SaleAccountId = s.SaleAccountId,
                        SaleTaxId = s.SaleTaxId,
                        InventoryAccountId = s.InventoryAccountId,
                        UseBatchNo = s.UseBatchNo,
                        TrackExpiration = s.TrackExpiration,
                        TrackSerial = s.TrackSerial,
                        ItemTypeId = s.ItemTypeId,
                        ManageInventory = s.ItemType.DisplayInventoryAccount,
                        ShowSubItems = s.ShowSubItems
                    }).ToListAsync();

                    billAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);
                    receiptAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
                    #region journal transaction type 
                    transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptPurchase);
                    #endregion
                }
            }

            var addBatchNos = new List<BatchNo>();
            var addItemReceiptItemBatchNos = new List<ItemReceiptItemBatchNo>();
            var addItemReceiptItems = new List<ItemReceiptItem>();
            var addItemReceipts = new List<ItemReceipt>();
            var addBillItems = new List<BillItem>();
            var addBills = new List<Bill>();
            var addBillJournalItems = new List<JournalItem>();
            var addBillJournals = new List<Journal>();
            var addItemReceiptJournalItems = new List<JournalItem>();
            var addItemReceiptJournals = new List<Journal>();
            var addInventoryTransactionItems = new List<InventoryTransactionItem>();

            var billDic = new Dictionary<string, Bill>();
            var itemReceiptDic = new Dictionary<string, ItemReceipt>();

            // Get the work book in the file
            var workBook = excelPackage.Workbook;
            if (workBook != null)
            {
                // retrive first worksheets
                var worksheet = excelPackage.Workbook.Worksheets[0];



                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var billGroup = worksheet.Cells[i, 1].Value?.ToString();
                    var date = worksheet.Cells[i, 2].Value?.ToString();
                    var receiptDate = worksheet.Cells[i, 3].Value?.ToString();
                    var vendorCode = worksheet.Cells[i, 4].Value?.ToString();
                    var aPAccountCode = worksheet.Cells[i, 5].Value?.ToString();
                    var locationName = worksheet.Cells[i, 6].Value?.ToString();
                    var className = hasClassFeature ? worksheet.Cells[i, 7].Value?.ToString() : "";
                    var currencyCode = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                    var memo = worksheet.Cells[i, 9 + indexHasClassFeature].Value?.ToString();
                    var reference = worksheet.Cells[i, 10 + indexHasClassFeature].Value?.ToString();
                    var billNo = worksheet.Cells[i, 11 + indexHasClassFeature].Value?.ToString();

                    var itemCode = worksheet.Cells[i, 12 + indexHasClassFeature].Value.ToString();
                    var zone = worksheet.Cells[i, 13 + indexHasClassFeature].Value.ToString();
                    var qtyText = worksheet.Cells[i, 14 + indexHasClassFeature].Value.ToString();
                    var unitPriceText = worksheet.Cells[i, 15 + indexHasClassFeature].Value.ToString();
                    var unitPriceInAccountText = worksheet.Cells[i, 16 + indexHasClassFeature].Value.ToString();
                    var itemDescription = worksheet.Cells[i, 17 + indexHasClassFeature].Value?.ToString();
                    var batchSerial = worksheet.Cells[i, 18 + indexHasClassFeature].Value?.ToString();
                    var expiration = worksheet.Cells[i, 19 + indexHasClassFeature].Value?.ToString();

                    var currency = currencies.Where(s => s.Name == currencyCode).FirstOrDefault();
                    var defaultClass = hasClassFeature ? classes.Where(s => s.Name == className).Select(t => t.Value).FirstOrDefault() : tenant.ClassId;
                    var location = locations.Where(s => s.Name == locationName).FirstOrDefault();
                    var vendor = vendors.Where(s => s.Name == vendorCode).FirstOrDefault();
                    var apAccount = accounts.Where(s => s.AccountCode == aPAccountCode).FirstOrDefault();
                    var lot = lots.FirstOrDefault(s => s.LotName == zone);

                    if (billAuto.CustomFormat == false && billNo == null)
                    {
                        throw new UserFriendlyException(L("IsRequired", L("BillNo")) + $", Row = {i}");
                    }


                    if (currency == null)
                    {
                        throw new UserFriendlyException(L("NoCurrencyFound") + $", Row = {i}");
                    }
                    if (defaultClass == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound") + $", Row = {i}");
                    }
                    if (vendor == null)
                    {
                        throw new UserFriendlyException(L("NoVendorFound") + $", Row = {i}");
                    }

                    if (apAccount == null)
                    {
                        throw new UserFriendlyException(L("NoAPAccountFound") + $", Row = {i}");
                    }
                    if (location == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound") + $", Row = {i}");
                    }

                    if (billAuto.RequireReference)
                    {
                        if (reference.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Reference")) + $", Row = {i}");

                        var find = billJournalHash.Any(s => s.Reference != null && s.Reference.ToLower() == reference.ToLower() && s.PartnerId == vendor.Value);
                        if (find) throw new UserFriendlyException(L("DuplicateReferenceNo") + $", Row = {i}");
                    }

                    var item = items.FirstOrDefault(s => s.ItemCode == itemCode);
                    if (item == null) throw new UserFriendlyException(L("InvalidItemCode") + $", Row = {i}");

                    if (item.ManageInventory)
                    {
                        if (lot == null) throw new UserFriendlyException(L("IsRequired", L("Zone")) + $", Row = {i}");
                        if (lot.LocationId != location.Value) throw new UserFriendlyException(L("InvalidLocationZone") + $", {locationName}, {zone}, Row = {i}");
                    }

                    var autoReceipt = !receiptDate.IsNullOrWhiteSpace();
                    if (autoReceipt)
                    {
                        if ((item.UseBatchNo || item.TrackSerial || item.TrackExpiration) && batchSerial.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("Batch/Serial")) + $", Row = {i}");
                        if (item.TrackExpiration && expiration.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("Expiration")) + $", Row = {i}");
                    }

                    var qty = Convert.ToDecimal(qtyText);
                    if (autoReceipt && item.TrackSerial && qty != 1) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Row = {i}");

                    var unitPrice = Math.Round(Convert.ToDecimal(unitPriceText), currentCycle.RoundingDigitUnitCost);
                    var unitPriceInAccount = Math.Round(Convert.ToDecimal(unitPriceInAccountText), currentCycle.RoundingDigitUnitCost);
                    var totalInAcc = Math.Round(qty * unitPriceInAccount, currentCycle.RoundingDigit);
                    var totalInTran = Math.Round(qty * unitPrice, currentCycle.RoundingDigit);

                    var addBillItem = BillItem.Create(tenantId, userId, Guid.Empty, item.PurchaseTaxId.Value, item.Id, itemDescription, qty, unitPriceInAccount, 0, totalInAcc, unitPrice, totalInTran);
                    var purchaseAccount = item.ManageInventory ? tenant.TransitAccountId.Value : item.PurchaseAccountId.Value;
                    var addBillJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, purchaseAccount, itemDescription, totalInAcc, 0, PostingKey.Clearance, addBillItem.Id);

                    if (item.ManageInventory) addBillItem.UpdateLot(lot.Id);
                    addBillItems.Add(addBillItem);
                    addBillJournalItems.Add(addBillJournalItem);

                    if (billDic.ContainsKey(billGroup))
                    {
                        var addBill = billDic[billGroup];
                        addBill.SetSubTotal(addBill.SubTotal + totalInAcc);
                        addBill.SetTotal(addBill.Total + totalInAcc);
                        addBill.SetMultiCurrencySubTotal(addBill.MultiCurrencySubTotal + totalInTran);
                        addBill.SetMultiCurrencyTotal(addBill.MultiCurrencyTotal + totalInTran);
                        addBill.SetOpenBalance(addBill.Total);
                        addBill.SetMultiCurrencyOpenBalance(addBill.MultiCurrencyTotal);

                        var addBillJournal = addBillJournals.FirstOrDefault(s => s.BillId == addBill.Id);
                        addBillJournal.SetDebitCredit(addBill.Total);

                        var apAccountItem = addBillJournalItems.FirstOrDefault(s => s.JournalId == addBillJournal.Id && s.Key == PostingKey.AP);
                        apAccountItem.SetDebitCredit(0, addBill.Total);

                        addBillItem.SetBill(addBill.Id);
                        addBillJournalItem.SetJournal(addBillJournal.Id);
                    }
                    else
                    {

                        if (billAuto.CustomFormat == true)
                        {
                            var newAuto = _autoSequenceManager.GetNewReferenceNumber(billAuto.DefaultPrefix, billAuto.YearFormat.Value,
                                           billAuto.SymbolFormat, billAuto.NumberFormat, billAuto.LastAutoSequenceNumber, DateTime.Now);

                            billNo = newAuto;
                            billAuto.UpdateLastAutoSequenceNumber(newAuto);
                        }
                        else if (billJournalHash.Any(s => s.JournalNo == billNo) || addBillJournals.Any(s => s.JournalNo == billNo))
                        {
                            throw new UserFriendlyException(L("DuplicateBillNo", billNo) + $", Row = {i}");
                        }

                        if (billAuto.RequireReference)
                        {
                            var find = from j in addBillJournals
                                       join ii in addBills
                                       on j.BillId equals ii.Id
                                       where j.Reference != null && j.Reference.ToLower() == reference.ToLower() && ii.VendorId == vendor.Value
                                       select j;

                            if (find.Any()) throw new UserFriendlyException(L("DuplicateReferenceNo") + $", Row = {i}");
                        }

                        CAddress billAddress = new CAddress("", "", "", "", "");
                        CAddress shipAddress = new CAddress("", "", "", "", "");

                        var addBill = Bill.Create(tenantId, userId, Bill.ReceiveFromStatus.None, Convert.ToDateTime(date), vendor.Value, true, shipAddress, billAddress, totalInAcc, 0, totalInAcc, null, Convert.ToDateTime(date), autoReceipt, autoReceipt ? Convert.ToDateTime(receiptDate) : Convert.ToDateTime(date), totalInTran, 0, totalInTran, true, useExchangeRate);
                        var addBillJournal = Journal.Create(tenantId, userId, billNo, Convert.ToDateTime(date), memo, totalInAcc, totalInAcc, tenant.CurrencyId.Value, defaultClass.Value, reference, location.Value);
                        var apJournalItem = JournalItem.CreateJournalItem(tenantId, userId, addBillJournal.Id, apAccount.Id, memo, 0, totalInAcc, PostingKey.AP, null);

                        addBillJournal.UpdateMultiCurrency(currency.Value);
                        addBillJournal.UpdateBill(addBill.Id);
                        addBillJournal.UpdateStatus(TransactionStatus.Publish);

                        addBills.Add(addBill);
                        addBillJournals.Add(addBillJournal);
                        addBillJournalItems.Add(apJournalItem);

                        addBillItem.SetBill(addBill.Id);
                        addBillJournalItem.SetJournal(addBillJournal.Id);

                        billDic.Add(billGroup, addBill);
                    }

                    if (autoReceipt && item.ManageInventory)
                    {
                        var addItemReceiptItem = ItemReceiptItem.Create(tenantId, userId, Guid.Empty, item.Id, itemDescription, qty, unitPriceInAccount, 0, totalInAcc);
                        var addItemReceiptJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, item.InventoryAccountId.Value, itemDescription, totalInAcc, 0, PostingKey.Inventory, addItemReceiptItem.Id);

                        addItemReceiptItem.UpdateLot(lot.Id);
                        addItemReceiptItems.Add(addItemReceiptItem);
                        addItemReceiptJournalItems.Add(addItemReceiptJournalItem);

                        //Set reference to bill item
                        addBillItem.UpdateReceiptItemId(addItemReceiptItem.Id);

                        Journal addItemReceiptJournal = null;

                        if (itemReceiptDic.ContainsKey(billGroup))
                        {
                            var addItemReceipt = itemReceiptDic[billGroup];
                            addItemReceipt.SetTotal(addItemReceipt.Total + totalInAcc);

                            addItemReceiptJournal = addItemReceiptJournals.FirstOrDefault(s => s.ItemReceiptId == addItemReceipt.Id);
                            addItemReceiptJournal.SetDebitCredit(addItemReceipt.Total);

                            var clearanceJournalItem = addItemReceiptJournalItems.FirstOrDefault(s => s.JournalId == addItemReceiptJournal.Id && s.Key == PostingKey.Clearance);
                            clearanceJournalItem.SetDebitCredit(0, addItemReceipt.Total);

                            addItemReceiptItem.SetItemReceipt(addItemReceipt.Id);
                            addItemReceiptJournalItem.SetJournal(addItemReceiptJournal.Id);
                        }
                        else
                        {
                            var itemReceiptNo = billNo;
                            if (autoReceipt && receiptAuto.CustomFormat == true)
                            {
                                var newAuto = _autoSequenceManager.GetNewReferenceNumber(receiptAuto.DefaultPrefix, receiptAuto.YearFormat.Value,
                                                receiptAuto.SymbolFormat, receiptAuto.NumberFormat, receiptAuto.LastAutoSequenceNumber, DateTime.Now);
                                itemReceiptNo = newAuto;
                                receiptAuto.UpdateLastAutoSequenceNumber(newAuto);
                            }
                            else if (itemReceiptJournalHash.Contains(itemReceiptNo) || addItemReceiptJournals.Any(s => s.JournalNo == itemReceiptNo))
                            {
                                throw new UserFriendlyException(L("DuplicateItemReceiptNo", itemReceiptNo) + $", Row = {i}");
                            }

                            var addBill = billDic[billGroup];

                            var addItemReceipt = ItemReceipt.Create(tenantId, userId, ItemReceipt.ReceiveFromStatus.Bill, vendor.Value, true, addBill.ShippingAddress, addBill.BillingAddress, totalInAcc, null);

                            addItemReceiptJournal = Journal.Create(tenantId, userId, itemReceiptNo, Convert.ToDateTime(receiptDate), memo, totalInAcc, totalInAcc, currency.Value, defaultClass.Value, reference, location.Value);
                            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, addItemReceiptJournal.Id, tenant.TransitAccountId.Value, memo, 0, totalInAcc, PostingKey.Clearance, null);

                            addItemReceiptJournal.UpdateItemReceipt(addItemReceipt.Id);
                            addItemReceiptJournal.UpdateStatus(TransactionStatus.Publish);
                            addItemReceiptJournal.SetJournalTransactionTypeId(transactionTypeId);
                            addItemReceipts.Add(addItemReceipt);
                            addItemReceiptJournals.Add(addItemReceiptJournal);
                            addItemReceiptJournalItems.Add(clearanceJournalItem);

                            addItemReceiptItem.SetItemReceipt(addItemReceipt.Id);
                            addItemReceiptJournalItem.SetJournal(addItemReceiptJournal.Id);

                            //Set reference to bill
                            addBill.UpdateItemReceiptid(addItemReceipt.Id);
                            addBill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);

                            itemReceiptDic.Add(billGroup, addItemReceipt);
                        }


                        //manage batch serial and expiration
                        if (item.TrackSerial || item.UseBatchNo || item.TrackExpiration)
                        {

                            var batchNoId = Guid.Empty;
                            var expiredDate = item.TrackExpiration ? Convert.ToDateTime(expiration) : (DateTime?)null;

                            var findBatch = batchNos.FirstOrDefault(s => s.BatchNumber == batchSerial);
                            if (findBatch != null)
                            {
                                if (findBatch.ItemId != item.Id) throw new UserFriendlyException(L("BatchNoAlreadyUseByOtherItem") + $" {batchSerial}, Row = {i}");
                                if (item.TrackExpiration && findBatch.ExpirationDate.Value.Date != expiredDate.Value.Date)
                                {
                                    throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {expiredDate.Value.ToString("dd-MM-yyyy")}, Row = {i}");
                                }
                                batchNoId = findBatch.Id;
                            }
                            else
                            {
                                var find = addBatchNos.FirstOrDefault(s => s.Code == batchSerial);
                                if (find != null)
                                {
                                    if (find.ItemId != item.Id) throw new UserFriendlyException(L("BatchNoAlreadyUseByOtherItem") + $" {batchSerial}, Row = {i}");
                                    if (item.TrackExpiration && find.ExpirationDate.Value.Date != expiredDate.Value.Date)
                                    {
                                        throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {expiredDate.Value.ToString("dd-MM-yyyy")}, Row = {i}");
                                    }
                                    batchNoId = find.Id;
                                }
                                else
                                {
                                    var addBatchNo = BatchNo.Create(tenantId.Value, userId, batchSerial, item.Id, false, item.TrackSerial, expiredDate);
                                    addBatchNos.Add(addBatchNo);
                                    batchNoId = addBatchNo.Id;
                                }
                            }

                            var addItemReceiptItemBatchNo = ItemReceiptItemBatchNo.Create(tenantId.Value, userId, addItemReceiptItem.Id, batchNoId, addItemReceiptItem.Qty);
                            addItemReceiptItemBatchNos.Add(addItemReceiptItemBatchNo);
                        }


                        //Sync inventory transaction items
                        var addInventoryTransactionItem = InventoryTransactionItem.Create(
                                                      tenantId.Value,
                                                      userId,
                                                      addItemReceiptItem.CreationTime,
                                                      addItemReceiptItem.LastModifierUserId,
                                                      addItemReceiptItem.LastModificationTime,
                                                      addItemReceiptItem.Id,
                                                      addItemReceiptItem.ItemReceiptId,
                                                      null,
                                                      null,
                                                      addItemReceiptJournal.Id,
                                                      addItemReceiptJournal.Date,
                                                      addItemReceiptJournal.CreationTimeIndex.Value,
                                                      addItemReceiptJournal.JournalType,
                                                      addItemReceiptJournal.JournalNo,
                                                      addItemReceiptJournal.Reference,
                                                      addItemReceiptItem.ItemId,
                                                      addItemReceiptJournalItem.AccountId,
                                                      location.Value,
                                                      addItemReceiptItem.LotId.Value,
                                                      addItemReceiptItem.Qty,
                                                      addItemReceiptItem.UnitCost,
                                                      addItemReceiptItem.Total,
                                                      true,
                                                      addItemReceiptItem.Description);

                        addInventoryTransactionItems.Add(addInventoryTransactionItem);

                    }

                }
            }


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (addBatchNos.Any()) await _batchNoRepository.BulkInsertAsync(addBatchNos);
                    if (addItemReceipts.Any()) await _itemReceiptRepository.BulkInsertAsync(addItemReceipts);
                    if (addItemReceiptItems.Any()) await _itemReceiptItemRepository.BulkInsertAsync(addItemReceiptItems);
                    if (addItemReceiptItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkInsertAsync(addItemReceiptItemBatchNos);
                    if (addItemReceiptJournals.Any()) await _journalRepository.BulkInsertAsync(addItemReceiptJournals);
                    if (addItemReceiptJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addItemReceiptJournalItems);

                    if (addBills.Any()) await _billRepository.BulkInsertAsync(addBills);
                    if (addBillItems.Any()) await _billItemRepository.BulkInsertAsync(addBillItems);
                    if (addBillJournals.Any()) await _journalRepository.BulkInsertAsync(addBillJournals);
                    if (addBillJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addBillJournalItems);

                    //Sync Inventroy Transaction Item
                    if (addInventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(addInventoryTransactionItems);

                    if (billAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(billAuto));
                    if (addItemReceipts.Any() && receiptAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(receiptAuto));
                }

                await uow.CompleteAsync();
            }

        }

        #endregion

        [AbpAuthorize(AppPermissions.Pages_Tenant_CleanRounding)]
        public async Task CleanRoundingPaidStatus(CarlEntityDto input)
        {
            var currenctPeriod = await GetCurrentCycleAsync();
            var roundDigit = currenctPeriod == null ? 2 : currenctPeriod.RoundingDigit;

            var bill = await _billManager.GetAsync(input.Id);
            if (bill == null) throw new UserFriendlyException(L("BillNoNoFound"));

            var journal = await _journalRepository.FirstOrDefaultAsync(s => s.BillId == input.Id);
            if (journal == null) throw new UserFriendlyException(L("BillNoNoFound"));

            var billItems = await _billItemRepository.GetAll().Where(s => s.BillId == input.Id).ToListAsync();
            var journalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id).ToListAsync();

            decimal subTotal = 0;
            foreach (var billItem in billItems)
            {
                var lineTotal = Math.Round(billItem.Total, roundDigit);
                billItem.SetTotal(lineTotal);
                await _billItemManager.UpdateAsync(billItem);

                var journalItem = journalItems.FirstOrDefault(s => s.Identifier == billItem.Id);
                if (journalItem == null) throw new UserFriendlyException(L("BillNoNoFound"));

                journalItem.SetDebitCredit(lineTotal, 0);
                await _journalItemManager.UpdateAsync(journalItem);

                subTotal += lineTotal;
            }

            decimal tax = Math.Round(bill.Tax, roundDigit);
            decimal total = subTotal + tax;
            decimal openBalance = total - bill.TotalPaid;

            bill.SetSubTotal(subTotal);
            bill.SetTax(tax);
            bill.SetTotal(total);
            bill.SetOpenBalance(openBalance);
            bill.UpdatePaidStatus(bill.TotalPaid == 0 ? PaidStatuse.Pending : openBalance == 0 ? PaidStatuse.Paid : PaidStatuse.Partial);
            await _billManager.UpdateAsync(bill);

            journal.SetDebitCredit(total);
            await _journalManager.UpdateAsync(journal, DocumentType.Bill, false);

            var headerJournalItem = journalItems.FirstOrDefault(s => s.Identifier == null && s.Key == PostingKey.AP);
            if (headerJournalItem == null) throw new UserFriendlyException(L("BillNoNoFound"));

            headerJournalItem.SetDebitCredit(0, total);
            await _journalItemManager.UpdateAsync(headerJournalItem);


            if (bill.ItemReceiptId == null) return;

            var itemReceipt = await _itemReceiptRepository.FirstOrDefaultAsync(s => s.Id == bill.ItemReceiptId);
            if (itemReceipt == null) throw new UserFriendlyException(L("BillNoNoFound"));

            var journalItemReceipt = await _journalRepository.FirstOrDefaultAsync(s => s.ItemReceiptId == itemReceipt.Id);
            if (journalItemReceipt == null) throw new UserFriendlyException(L("BillNoNoFound"));

            var itemReceiptItems = await _itemReceiptItemRepository.GetAll().Where(s => s.ItemReceiptId == itemReceipt.Id).ToListAsync();
            var journalItemReceiptItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journalItemReceipt.Id).ToListAsync();

            decimal itemReceiptTotal = 0;
            foreach (var itemReceiptItem in itemReceiptItems)
            {
                var lineTotal = Math.Round(itemReceiptItem.Total, roundDigit);
                itemReceiptItem.SetTotal(lineTotal);
                await _itemReceiptItemManager.UpdateAsync(itemReceiptItem);

                var journalItem = journalItemReceiptItems.FirstOrDefault(s => s.Identifier == itemReceiptItem.Id && s.Key == PostingKey.Inventory);
                if (journalItem == null) throw new UserFriendlyException(L("BillNoNoFound"));

                journalItem.SetDebitCredit(lineTotal, 0);
                await _journalItemManager.UpdateAsync(journalItem);

                itemReceiptTotal += lineTotal;
            }

            itemReceipt.UpdateTotal(itemReceiptTotal);
            await _itemReceiptManager.UpdateAsync(itemReceipt);

            journalItemReceipt.SetDebitCredit(itemReceiptTotal);
            await _journalManager.UpdateAsync(journalItemReceipt, DocumentType.ItemReceipt, false);

            var headerItemReceiptJournalItem = journalItemReceiptItems.FirstOrDefault(s => s.Identifier == null && s.Key == PostingKey.Clearance);
            if (headerItemReceiptJournalItem == null) throw new UserFriendlyException(L("BillNoNoFound"));

            headerItemReceiptJournalItem.SetDebitCredit(0, itemReceiptTotal);
            await _journalItemManager.UpdateAsync(headerItemReceiptJournalItem);
        }

    }
}
