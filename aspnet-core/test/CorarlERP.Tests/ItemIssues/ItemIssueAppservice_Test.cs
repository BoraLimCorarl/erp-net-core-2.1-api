using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.SaleOrders;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.ItemIssues
{
    public class ItemIssueAppsevice_Test : AppTestBase
    {
        private readonly IItemIssueAppService _itemIssueAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public readonly Class _class;
        public readonly Location _location;
        private readonly Customer _customer;
        private readonly Item _item;
        private readonly ItemType _itemType;
        private readonly SaleOrder _saleOrder;
        private readonly SaleOrderItem _saleOrderItem;

        public ItemIssueAppsevice_Test()
        {
            _itemIssueAppService = Resolve<IItemIssueAppService>();
            _class = CreateClass(null, null, "Ione II", false, null);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                       displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _customer = CreateCustomer(Name: "Ione", Code: "001", Email: "ione@gmail.com", PhoneNumber: "012334455", SameAsShippingAddress: true, Websit: "ione.com.kh",
              Remark: "Remark", BillingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),

              ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), accountId: _chartOfAccount.Id, customerTypeId: 1);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null,
                   purchaseTaxId: _tax.Id, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: null, itemTypeId: _itemType.Id, description: "", 
                   barcode:"", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            _saleOrder = CreateSaleOrder(_customer.Id, new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), true, "Order from Thai", _currency.Id, "001", DateTime.Now, "To KH", 10, 10, 100, TransactionStatus.Publish,DateTime.Now);
            _saleOrderItem = CreateSaleOrderItem(_saleOrder.Id, _item.Id, _tax.Id, 10, "Description", 100, 1000, 10, 1000);

        }

        private CreateItemIssueInput CreateHelper()
        {

            var ItemIssueExemptInput = new CreateItemIssueInput()
            {
                CurrencyId = _currency.Id,
                Memo = "Product from UAS",
                Date = DateTime.Now,
                SameAsShippingAddress = false,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#180E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                IssueNo = "P000001",
                ReceiveFrom = ReceiveFrom.SaleOrder,
                Reference = "P000001",
                CustomerId = _customer.Id,
                Total = 90,
                Status = TransactionStatus.Publish,
                Items = new List<CreateOrUpdateItemIssueItemInput>()
                {
                    new CreateOrUpdateItemIssueItemInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",ItemId = _item.Id,
                        SaleOrderItemId = _saleOrderItem.Id,Qty = 10,Total=100,UnitCost = 100
                    },
                     new CreateOrUpdateItemIssueItemInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",ItemId = _item.Id,
                        SaleOrderItemId = _saleOrderItem.Id,Qty = 10,Total=100,UnitCost = 100,
                    },
                }

            };
            return ItemIssueExemptInput;
        }

        [Fact]
        public async Task Test_CreateItemIssue()
        {
            var ItemIssueExemptInput = CreateHelper();
            var result = await _itemIssueAppService.Create(ItemIssueExemptInput);
            result.ShouldNotBeNull();

        }
        [Fact]
        public async Task Test_GetDetail()
        {

            var JournalExemptInput = CreateHelper();
            var result = await _itemIssueAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _itemIssueAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.BillingAddress.ShouldBe(JournalExemptInput.BillingAddress);
            output.LocationId.ShouldBe(JournalExemptInput.LocationId);
            output.Total.ShouldBe(JournalExemptInput.Total);
            output.CustomerId.ShouldBe(JournalExemptInput.CustomerId);
          //  CheckItemReceipt(JournalExemptInput);
        }


        [Fact]
        public async Task Test_Delete()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _itemIssueAppService.Create(JournalExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var itemReceipEntity = context.ItemIssues.FirstOrDefault(p => p.CustomerId == JournalExemptInput.CustomerId);
            });
            await _itemIssueAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.ItemIssues.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemIssueId = Guid.Empty;
            var ItemIssueExemptInput = CreateHelper();
            var result = await _itemIssueAppService.Create(ItemIssueExemptInput);
            result.ShouldNotBeNull();          
            UsingDbContext(context =>
            {
                var ItemIssue = context.ItemIssueItems.FirstOrDefault(u => u.ItemId == ItemIssueExemptInput.Items[0].ItemId);
                ItemIssue.ShouldNotBeNull();
                ItemIssueId = ItemIssue.Id;
            });
            ItemIssueId.ShouldNotBe(Guid.Empty);

            var UpdateItemReceiptExemptInput = new UpdateItemIssueInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Product from England",
                Date = DateTime.Now,
                SameAsShippingAddress = true,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                // ShippingAddress = new Address("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#185E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                IssueNo = "P000001",
                ReceiveFrom = ReceiveFrom.Invoice,
                Reference = "P000001",
                CustomerId = _customer.Id,
                Total = 90,
                Items = new List<CreateOrUpdateItemIssueItemInput>()
                {
                    new CreateOrUpdateItemIssueItemInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",ItemId =_item.Id,
                        SaleOrderItemId = _saleOrderItem.Id,Qty = 10,Total = 100,UnitCost = 10
                    },
                     new CreateOrUpdateItemIssueItemInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",ItemId = _item.Id,
                        SaleOrderItemId = _saleOrderItem.Id,Qty = 10,Total=100,UnitCost = 100,
                    },
                }

            };
            var updatedResult = await _itemIssueAppService.Update(UpdateItemReceiptExemptInput);
            updatedResult.ShouldNotBeNull();
        }

    }
}
