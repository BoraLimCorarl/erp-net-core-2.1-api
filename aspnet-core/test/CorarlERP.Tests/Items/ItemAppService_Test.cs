using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.ItemTypes;
using CorarlERP.PropertyValues;
using CorarlERP.SubItems.Dto;
using CorarlERP.Taxes;
using Shouldly;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.Items
{
    public class ItemAppService_Test : AppTestBase
    {

     
        private readonly IItemAppService _itemAppService;
        private readonly Tax _taxExempt;
        private readonly ItemType _itemType;
        private readonly Currency _currency;
        private readonly Property _property;
        private readonly PropertyValue _propertyValue;
        public readonly ChartOfAccount _chartofAccount;
        public readonly AccountType _accountType;
        public ItemAppService_Test()
        {
            _itemAppService = Resolve<IItemAppService>();
            _taxExempt = CreateTax("Tax Exempt", 0);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false, displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _property = CreateProperty("Category",true);
            _propertyValue = CreatePropertyValue("Computer", _property.Id,0,0);
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);

            _chartofAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _taxExempt.Id);
        }
        private CreateItemInput CreateHelper()
        {
            var ItemExemptInput = new CreateItemInput()
            {

                ItemCode = "T001",
                ItemName = "Technology",
                PurchaseCost = 100,
                ItemTypeId = _itemType.Id,
                SalePrice = 110,
                ReorderPoint = 200,
                TrackSerial = false,
                SaleCurrenyId = _currency.Id,
                PurchaseCurrencyId = _currency.Id,
                PurchaseTaxId = _taxExempt.Id,
                InventoryAccountId = _chartofAccount.Id,
                PurchaseAccountId = _chartofAccount.Id,
                SaleAccountId = _chartofAccount.Id,
                SaleTaxId = _taxExempt.Id,
                Description ="Product from USA",

                Properties = new List<CreateOrItemPropertyInput>()
                {
                      new CreateOrItemPropertyInput(){PropertyId= _property.Id,PropertyValueId=1 }
                    
                },
                //SubItems = new List<CreateSubItemInput>()
                //{
                //    // new CreateSubItemInput() {Quantity= 5, Cost = 3,Total = 15,IsActive= true,},
                //    // new CreateSubItemInput() {Quantity= 4, Cost = 4,Total = 16,IsActive= true,},
                //    // new CreateSubItemInput() {Quantity= 7, Cost = 5,Total = 35,IsActive= true,},
                //}
            };
            return ItemExemptInput;
        }
        private CreateItemInput CreateHelper2()
        {
           
            var ItemExemptInput = new CreateItemInput()
            {

                ItemCode = "T002",
                ItemName = "Technology2",
                PurchaseCost = 100,
                ItemTypeId = _itemType.Id,
                SalePrice = 110,
                ReorderPoint = 200,
                TrackSerial = false,
                SaleCurrenyId = _currency.Id,
                PurchaseCurrencyId = _currency.Id,
                PurchaseTaxId = _taxExempt.Id,
                InventoryAccountId = _chartofAccount.Id,
                PurchaseAccountId = _chartofAccount.Id,
                SaleAccountId = _chartofAccount.Id,
                SaleTaxId = _taxExempt.Id,

                Properties = new List<CreateOrItemPropertyInput>()
                {
                      new CreateOrItemPropertyInput(){PropertyId= _property.Id,PropertyValueId = 2 }

                },
                //SubItems = new List<CreateSubItemInput>()
                //{
                //    // new CreateSubItemInput() {Quantity= 5, Cost = 3,Total = 15,IsActive= true,},
                //    // new CreateSubItemInput() {Quantity= 4, Cost = 4,Total = 16,IsActive= true,},
                //    // new CreateSubItemInput() {Quantity= 7, Cost = 5,Total = 35,IsActive= true,},
                //}
            };
            return ItemExemptInput;
        }


        private void CheckItem(CreateItemInput ItemExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var ItemEntity = context.Items.FirstOrDefault(p => p.ItemName == ItemExemptInput.ItemName);
                ItemEntity.ShouldNotBeNull();
                if (Id != null) ItemEntity.Id.ShouldBe(Id.Value);
                ItemEntity.Description.ShouldBe(ItemExemptInput.Description);
                ItemEntity.ItemName.ShouldBe(ItemExemptInput.ItemName);
                ItemEntity.ItemCode.ShouldBe(ItemExemptInput.ItemCode);
                ItemEntity.InventoryAccountId.ShouldBe(ItemExemptInput.InventoryAccountId);
                ItemEntity.SaleCurrenyId.ShouldBe(ItemExemptInput.SaleCurrenyId);
                ItemEntity.SaleTaxId.ShouldBe(ItemExemptInput.SaleTaxId);
                ItemEntity.SaleAccountId.ShouldBe(ItemExemptInput.SaleAccountId);
                ItemEntity.PurchaseAccountId.ShouldBe(ItemExemptInput.PurchaseAccountId);
                ItemEntity.PurchaseCurrencyId.ShouldBe(ItemExemptInput.PurchaseCurrencyId);
                ItemEntity.PurchaseTaxId.ShouldBe(ItemExemptInput.PurchaseTaxId);
                ItemEntity.IsActive.ShouldBe(true);
                var @subItems = context.SubItems.Where(u => u.ItemId == ItemEntity.Id).OrderBy(u => u.ItemId).ToList();
              //  var InputSubItems = ItemExemptInput.SubItems.OrderBy(u => u.ItemId).ToList();
                subItems.ShouldNotBeNull();
                //subItems.Count().ShouldBe(InputSubItems.Count());
                for (var i = 0; i < subItems.Count; i++)
                {
                    var subItem = subItems[i];
                 //   var inputsubItem = InputSubItems[i];
                    //ShouldBeTestExtensions.ShouldBe(subItem.Cost, inputsubItem.Cost);
                    //ShouldBeTestExtensions.ShouldBe(subItem.Quantity, inputsubItem.Quantity);
                    //ShouldBeTestExtensions.ShouldBe(subItem.Total, inputsubItem.Total);
                    //ShouldBeTestExtensions.ShouldBe(subItem.ItemId, inputsubItem.ItemId);

                }
                var Itemproperty = context.ItemProperties.ToList();
                foreach (var p in Itemproperty)
                {
                    p.PropertyId.ShouldBe(_property.Id);
                    p.PropertyValueId.ShouldBe(_propertyValue.Id);
                }
            });
        }

     

        [Fact]
        public async Task Test_CreateSingleGetList()
        {
            var item = CreateHelper();
            var result = await _itemAppService.Create(item);
           // var item2 = CreateHelper2();
          //  var result2 = await _itemAppService.Create(item2);
            var output = await _itemAppService.GetList(new GetItemListInput()
            {
                Filter = ""
                
              
            });
            foreach (var i in output.Items)
            {

                i.ItemCode.ShouldBe(item.ItemCode);
                i.ItemName.ShouldBe(item.ItemName);
                i.ItemType.Id.ShouldBe(item.ItemTypeId);
                i.SalePrice.ShouldBe(item.SalePrice);
            }
        }

        [Fact]
        public async Task Test_CreateSingleItem()
        {
            var ItemExemptInput = CreateHelper();
            var result = await _itemAppService.Create(ItemExemptInput);
            result.ShouldNotBeNull();
            CheckItem(ItemExemptInput);
        }

        [Fact]
        public async Task Test_CreateSingleFind()
        {
            var item = CreateHelper();
            var result = await _itemAppService.Create(item);

            var output = await _itemAppService.Find(new GetItemListInputFind()
            {
                Filter = ""
            });
            foreach (var i in output.Items)
            {

                i.ItemCode.ShouldBe(item.ItemCode);
                i.ItemName.ShouldBe(item.ItemName);
                i.ItemType.Id.ShouldBe(item.ItemTypeId);
                i.SalePrice.ShouldBe(item.SalePrice);
            }
        }

        [Fact]
        public async Task Test_DeleteItem()
        {
            var item = CreateHelper();

            var result = await _itemAppService.Create(item);
            result.ShouldNotBeNull("");
            await _itemAppService.Delete(new EntityDto<Guid> { Id = result.Id });

            UsingDbContext(context =>
            {
                context.Items.Count().ShouldBe(0);
            });
        }


        [Fact]
        public async Task Test_UpdateItem()
        {
            var ItemExemptInput = CreateHelper();

            var result = await _itemAppService.Create(ItemExemptInput);
            result.ShouldNotBeNull();
            //Assert
            CheckItem(ItemExemptInput);

            var UpdateItemExemptInput = new UpdateItemInput()
            {

                Id = result.Id,
                ItemCode = "T0001",
                ItemName = "Technology01",
                PurchaseCost = 10000,
                ItemTypeId = _itemType.Id,
                SalePrice = 1110,
                ReorderPoint = 1200,
                TrackSerial = false,
                SaleCurrenyId = _currency.Id,
                PurchaseCurrencyId = _currency.Id,
                PurchaseTaxId = _taxExempt.Id,
                SaleTaxId = _taxExempt.Id,
                InventoryAccountId = _chartofAccount.Id,
                PurchaseAccountId = _chartofAccount.Id,
                SaleAccountId = _chartofAccount.Id,
                //SubItems = new List<CreateSubItemInput>()
                //{
                //    // new CreateSubItemInput(){Id=subItemId,Quantity= 5,Cost = 3,ItemId =result.Id.Value,Total = 15, IsActive= true},
                //    // new CreateSubItemInput(){Quantity= 15,Cost = 2,Total = 30, IsActive= true}
                //},
                Properties = new List<CreateOrItemPropertyInput>()
                {

                    new CreateOrItemPropertyInput(){Id=Guid.NewGuid(),PropertyId = _property.Id,PropertyValueId=_propertyValue.Id },
                    new CreateOrItemPropertyInput(){PropertyId = _property.Id,PropertyValueId=_propertyValue.Id },
                },


            };


            var updatedResult = await _itemAppService.UpdateAsync(UpdateItemExemptInput);
            updatedResult.ShouldNotBeNull();
            CheckItem(UpdateItemExemptInput, result.Id);

            //Assert



        }
    }
}
