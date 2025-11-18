using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.CustomerCredits;
using CorarlERP.Customers;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceiptCustomerCredits.Dto;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.ItemReceiptCustomerCredits
{
    public class ItemReceiptCustomerCreditAppService_Test : AppTestBase
    {

        private readonly IItemReceiptCustomerCreditAppService _itemReceiptCustomerCreditAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public readonly Class _class;
        public readonly Location _location;
        public readonly CustomerCredit _customerCredit;
        public readonly CustomerCreditDetail _customerCreditItem;
        private readonly Customer _customer;
        private readonly Item _item;
        private readonly ItemType _itemType;

        public ItemReceiptCustomerCreditAppService_Test()
        {
            _itemReceiptCustomerCreditAppService = Resolve<IItemReceiptCustomerCreditAppService>();
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

            _customer = CreateCustomer(Name: "Ione", Code: "001", Email: "ione@gmail.com",
                    PhoneNumber: "012334455",
                    SameAsShippingAddress: true,
                    Websit: "ione.com.kh",
                    Remark: "Remark",
                    BillingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),

                    ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                    accountId: _chartOfAccount.Id, customerTypeId:1);

            _customerCredit = CreateCustomerCredit(
                    _customer.Id, 
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
                    DateTime.Now,
                    false,
                    null);
            _customerCreditItem = CreateCustomerCreditItem(
                    _customerCredit.Id, 
                    _item.Id, 
                    _tax.Id, 
                    0, 
                    "Description",
                    10, 
                    10, 
                    0, 
                    100);

        }

        private CreateItemReceiptCustomerCreditInput CreateHelper()
        {
            var ItemReceiptExemptInput = new CreateItemReceiptCustomerCreditInput()
            {
                CurrencyId = _currency.Id,
                Memo = "",
                Date = DateTime.Now,
               // ClearanceAccountId = _chartOfAccount.Id,
                SameAsShippingAddress = false,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#180E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                ReceiptNo = "Aj-01",
                Reference = "",
                Total = 100,
                Status = TransactionStatus.Publish,
                Items = new List<CreateOrUpdateItemReceiptCustomerCreditItemInput>()
                {
                    new CreateOrUpdateItemReceiptCustomerCreditItemInput(){
                        InventoryAccountId =_chartOfAccount.Id,
                        DiscountRate = 0,
                        Description = "",
                        ItemId =_item.Id,
                        Qty = 1,
                        UnitCost = 100,
                        Total = 100,
                        CustomerCreditItemId = _customerCreditItem.Id
                    }
                },
                CustomerCreditId = _customerCredit.Id,
                CustomerId = _customer.Id
            };
            return ItemReceiptExemptInput;
        }

        private void CheckItemReceipt(CreateItemReceiptCustomerCreditInput input, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == input.ReceiptNo);
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
                var ItemReceipt = context.ItemReceiptCustomerCredit.FirstOrDefault(p => p.CustomerId == input.CustomerId);
                var jItemId = context.JournalItems.FirstOrDefault(u => u.Debit == 0);               
                ItemReceipt.SameAsShippingAddress.ShouldBe(input.SameAsShippingAddress);
                ItemReceipt.BillingAddress.ShouldBe(input.BillingAddress);
                ItemReceipt.ShippingAddress.ShouldBe(input.SameAsShippingAddress ?
                                                      input.BillingAddress :
                                                      input.ShippingAddress);
                ItemReceipt.Total.ShouldBe(input.Total);
                var itemReceiptItems = context.ItemReceiptCustomerCreditItem.Where(u => u.ItemReceiptCustomerCreditId == ItemReceipt.Id).OrderBy(u => u.ItemId).ToList();
                var InputItemReceiptItem = input.Items.OrderBy(u => u.ItemId).ToList();
                itemReceiptItems.ShouldNotBeNull();
                itemReceiptItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < itemReceiptItems.Count; i++)
                {
                    var itemReceiptItem = itemReceiptItems[i];
                    var initemReceiptItem = InputItemReceiptItem[i];
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemId, itemReceiptItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Description, itemReceiptItem.Description);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.DiscountRate, itemReceiptItem.DiscountRate);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemReceiptCustomerCreditId, itemReceiptItem.ItemReceiptCustomerCreditId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.CustomerCreditItemId, itemReceiptItem.CustomerCreditItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Qty, itemReceiptItem.Qty);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Total, itemReceiptItem.Total);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.UnitCost, itemReceiptItem.UnitCost);

                };

            });
        }

        [Fact]
        public async Task Test_CreateItemReceiptCustomerCredit()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _itemReceiptCustomerCreditAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);
        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _itemReceiptCustomerCreditAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {
                var ItemReceipt = context.ItemReceiptCustomerCreditItem.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.Items[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = ItemReceipt.Id;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateItemReceiptExemptInput = new UpdateItemReceiptCustomerCreditInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Memo",
                Date = DateTime.Now,
               // ClearanceAccountId = _chartOfAccount.Id,
                SameAsShippingAddress = true,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                ReceiptNo = "ITCC-02",
                Reference = "",
                Total = 100,
                CustomerCreditId = _customerCredit.Id,
                CustomerId = _customer.Id,
                Items = new List<CreateOrUpdateItemReceiptCustomerCreditItemInput>()
                {
                   new CreateOrUpdateItemReceiptCustomerCreditItemInput(){
                        InventoryAccountId =_chartOfAccount.Id,
                        DiscountRate = 0,
                        Description = "",
                        ItemId =_item.Id,
                        Qty = 1,
                        UnitCost = 100,
                        Total = 100,
                        CustomerCreditItemId = _customerCreditItem.Id
                    }
                }
                
            };
            var updatedResult = await _itemReceiptCustomerCreditAppService.Update(UpdateItemReceiptExemptInput);
            updatedResult.ShouldNotBeNull();

        }


        [Fact]
        public async Task Test_GetDetail()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _itemReceiptCustomerCreditAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _itemReceiptCustomerCreditAppService.GetDetail(new EntityDto<Guid>()
            {
                Id = result.Id.Value,
            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.BillingAddress.ShouldBe(JournalExemptInput.BillingAddress);
            output.LocationId.ShouldBe(JournalExemptInput.LocationId);
            output.Total.ShouldBe(JournalExemptInput.Total);
            CheckItemReceipt(JournalExemptInput);
        }

    }
}
