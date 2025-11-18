using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemIssueVendorCredits.Dto;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.Taxes;
using CorarlERP.Vendors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.ItemIssueVendorCredits
{
    public class ItemIssueVendorCreditAppService_Test : AppTestBase
    {
        private readonly IItemIssueVendorCreditAppService _itemIssueVendorCreditAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public readonly Class _class;
        public readonly Location _location;
        public readonly VendorCredit.VendorCredit _vendorCredit;
        public readonly VendorCredit.VendorCreditDetail _vendorCreditItem;
        private readonly Vendor _vendor;
        private readonly Item _item;
        private readonly ItemType _itemType;

        public ItemIssueVendorCreditAppService_Test()
        {
            _itemIssueVendorCreditAppService = Resolve<IItemIssueVendorCreditAppService>();
            _class = CreateClass(null, null, "Ione II", false, null);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");

            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false,
                        displayPurchase: true, displaySale: true, displayReorderPoint: false,
                        displayTrackSerialNumber: false,
                        displaySubItem: false, displayUOM: false,
                        displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null, purchaseTaxId: _tax.Id, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: null, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);

            _vendor = CreateVendor(VendorName: "Ione", VendorCode: "001", Email: "ione@gmail.com",
                    PhoneNumber: "012334455",
                    SameAsShippingAddress: true,
                    Websit: "ione.com.kh",
                    Remark: "Remark",
                    BillingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),

                    ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                    accountId: _chartOfAccount.Id, vendorType: null);

            _vendorCredit = CreateVendorCredit(
                    _vendor.Id,
                    new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                    new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                    true,
                    "",
                  
                    "CC-001",
                    DateTime.Now,
                    "",
                    0,
                    10,
                    100,
                    TransactionStatus.Publish,
                    DateTime.Now,DateTime.Now,false);
            _vendorCreditItem = CreateVendorCreditItem(
                    _vendorCredit.Id,
                    _item.Id,
                    _tax.Id,
                    0,
                    "Description",
                    10,
                    10,
                    0,
                    100);

        }

        private CreateItemIssueVendorCreditInput CreateHelper()
        {
            var ItemReceiptExemptInput = new CreateItemIssueVendorCreditInput()
            {
                CurrencyId = _currency.Id,
                Memo = "",
                Date = DateTime.Now,
               // AccountId = _chartOfAccount.Id,
                SameAsShippingAddress = false,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#180E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                IssueNo = "Aj-01",
                Reference = "",
                Total = 100,
                Status = TransactionStatus.Publish,
                Items = new List<CreateOrUpdateItemIssueVendorCreditItemInput>()
                {
                    new CreateOrUpdateItemIssueVendorCreditItemInput(){
                        InventoryAccountId =_chartOfAccount.Id,
                        PurchaseAccountId =_chartOfAccount.Id,
                        DiscountRate = 0,
                        Description = "",
                        //TaxId = _tax.Id,
                        ItemId =_item.Id,
                        Qty = 1,
                        UnitCost = 100,
                        Total = 100,
                        VendorCreditItemId = _vendorCreditItem.Id
                    }
                },
                VendorCreditId = _vendorCredit.Id,
                VendorId = _vendor.Id
            };
            return ItemReceiptExemptInput;
        }

        private void CheckItemIssue(CreateItemIssueVendorCreditInput input, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == input.IssueNo);
                JournalEntity.ShouldNotBeNull();
                JournalEntity.ItemReceiptId.ShouldBe(JournalEntity.ItemReceiptId);
                JournalEntity.Credit.ShouldBe(input.Total);
                JournalEntity.Debit.ShouldBe(0);
                JournalEntity.CurrencyId.ShouldBe(input.CurrencyId);
                JournalEntity.Date.ShouldBe(input.Date);
                JournalEntity.Memo.ShouldBe(input.Memo);
                JournalEntity.IsActive.ShouldBe(true);
                var journalItems = context.JournalItems.Where(u => u.JournalId == JournalEntity.Id && u.Debit != 0).OrderBy(u => u.AccountId).ToList();
                var InputJournalItems = input.Items.OrderBy(u => u.InventoryAccountId).ToList();
                journalItems.ShouldNotBeNull();
                journalItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < journalItems.Count; i++)
                {
                    var JournalItem = journalItems[i];
                    var inJournalItem = InputJournalItems[i];
                    ShouldBeTestExtensions.ShouldBe(JournalItem.JournalId, JournalEntity.Id);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.AccountId, inJournalItem.InventoryAccountId);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Credit, 0);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Debit, inJournalItem.Total);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Description, inJournalItem.Description);
                };
                var ItemIssue = context.ItemIssueVendorCredit.FirstOrDefault(p => p.VendorId == input.VendorId);
                var jItemId = context.JournalItems.FirstOrDefault(u => u.Debit == 0);              
                ItemIssue.SameAsShippingAddress.ShouldBe(input.SameAsShippingAddress);
                ItemIssue.BillingAddress.ShouldBe(input.BillingAddress);
                ItemIssue.ShippingAddress.ShouldBe(input.SameAsShippingAddress ?
                                                      input.BillingAddress :
                                                      input.ShippingAddress);
                ItemIssue.Total.ShouldBe(input.Total);
                var itemIssueItems = context.ItemIssueVendorCreditItem.Where(u => u.ItemIssueVendorCreditId == ItemIssue.Id).OrderBy(u => u.ItemId).ToList();
                var InputItemReceiptItem = input.Items.OrderBy(u => u.ItemId).ToList();
                itemIssueItems.ShouldNotBeNull();
                itemIssueItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < itemIssueItems.Count; i++)
                {
                    var itemIssueItem = itemIssueItems[i];
                    var initemReceiptItem = InputItemReceiptItem[i];
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.ItemId, itemIssueItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.Description, itemIssueItem.Description);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.DiscountRate, itemIssueItem.DiscountRate);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.ItemIssueVendorCreditId, itemIssueItem.ItemIssueVendorCreditId);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.VendorCreditItemId, itemIssueItem.VendorCreditItemId);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.Qty, itemIssueItem.Qty);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.Total, itemIssueItem.Total);
                    ShouldBeTestExtensions.ShouldBe(itemIssueItem.UnitCost, itemIssueItem.UnitCost);

                };

            });
        }

        [Fact]
        public async Task Test_CreateItemIssueVendorCredit()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _itemIssueVendorCreditAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemIssue(ItemReceiptExemptInput);
        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _itemIssueVendorCreditAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemIssue(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {
                var ItemReceipt = context.ItemIssueVendorCreditItem.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.Items[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = ItemReceipt.Id;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateItemReceiptExemptInput = new UpdateItemIssueVendorCreditInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Memo",
                Date = DateTime.Now,
               // AccountId = _chartOfAccount.Id,
                SameAsShippingAddress = true,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                IssueNo = "ITCC-02",
                Reference = "",
                Total = 100,
                VendorCreditId = _vendorCredit.Id,
                VendorId = _vendor.Id,
                Items = new List<CreateOrUpdateItemIssueVendorCreditItemInput>()
                {
                   new CreateOrUpdateItemIssueVendorCreditItemInput(){
                        InventoryAccountId =_chartOfAccount.Id,
                        PurchaseAccountId = _chartOfAccount.Id,
                        DiscountRate = 0,
                        Description = "",
                        ItemId =_item.Id,
                        Qty = 1,
                        //TaxId = _tax.Id,
                        UnitCost = 100,
                        Total = 100,
                        VendorCreditItemId = _vendorCreditItem.Id
                    }
                }

            };
            var updatedResult = await _itemIssueVendorCreditAppService.Update(UpdateItemReceiptExemptInput);
            updatedResult.ShouldNotBeNull();

        }


        [Fact]
        public async Task Test_GetDetail()
        {
            var JournalInput = CreateHelper();
            var result = await _itemIssueVendorCreditAppService.Create(JournalInput);

            result.Id.ShouldNotBeNull();
            var output = await _itemIssueVendorCreditAppService.GetDetail(new EntityDto<Guid>()
            {
                Id = result.Id.Value,
            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.BillingAddress.ShouldBe(JournalInput.BillingAddress);
            output.LocationId.ShouldBe(JournalInput.LocationId);
            output.Total.ShouldBe(JournalInput.Total);
            CheckItemIssue(JournalInput);
        }

    }
}
