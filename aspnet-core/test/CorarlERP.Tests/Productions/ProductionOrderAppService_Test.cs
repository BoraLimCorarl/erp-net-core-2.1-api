using Abp.Application.Services.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.Productions;
using CorarlERP.Productions.Dto;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.Productions
{
  public class ProductionOrderAppService_Test : AppTestBase
    {
        private readonly IProductionAppService _ProductionOrderAppService;
        private readonly Currency _currency;
        private readonly Location _location;
        private readonly Class _class;
        private readonly Item _item;
        private readonly ItemType _itemType;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public ProductionOrderAppService_Test()
        {
            _ProductionOrderAppService = Resolve<IProductionAppService>();
            _tax = CreateTax("Tax Exempt", 0);
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                        displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: _chartOfAccount.Id, purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _class = CreateClass(null, null, "Default", false, null);

        }
        private CreateProductionInput CreateHelper()
        {
            var ProductionOrderExemptInput = new CreateProductionInput()
            {
                Status = TransactionStatus.Draft,
                Memo = "Product from UAS",
                Date = DateTime.Now,
                ProductionNo = "T001",
                Reference = "T001",
                FromLocationId = _location.Id,
                ToLocationId = _location.Id,
                FromClassId = _class.Id,
                ToClassId = _class.Id,
                ItemIssueDate = DateTime.Now,
                ItemReceiptDate = DateTime.Now,
                ConvertToIssueAndReceipt = false,
                RawMaterialItems = new List<CreateOrUpdateRawMaterialItemsInput>()
                {
                      new CreateOrUpdateRawMaterialItemsInput() {
                          ItemId = _item.Id,
                          Description ="Product From UAS",
                          Unit = 1,
                          DiscountRate = 0,
                          InventoryAccountId = _chartOfAccount.Id,


            },
                new CreateOrUpdateRawMaterialItemsInput()
                {
                    ItemId = _item.Id,
                    Description = "Product From UAS",
                    Unit = 10
                }
            },
                FinishItems = new List<CreateOrUpdateFinishItemInput>()
                {
                      new CreateOrUpdateFinishItemInput() {
                          ItemId = _item.Id,
                          Description ="Product From UAS",
                          Unit = 1
                      }                      
                }
            };
            return ProductionOrderExemptInput;

        }
        private void CheckProductionOrder(CreateProductionInput ProductionOrderInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var ProductionEntity = context.Productions.FirstOrDefault(p => p.ProductionNo == ProductionOrderInput.ProductionNo);
                ProductionEntity.ShouldNotBeNull();
                if (Id != null) ProductionEntity.Id.ShouldBe(Id.Value);

                ProductionEntity.Date.ShouldBe(ProductionOrderInput.Date);
                ProductionEntity.ProductionNo.ShouldBe(ProductionOrderInput.ProductionNo);
                ProductionEntity.Memo.ShouldBe(ProductionOrderInput.Memo);

                var rawItems = context.RawMaterialItems.Where(u => u.ProductionId == ProductionEntity.Id).OrderBy(u => u.ItemId).ToList();
                var InputrawItems = ProductionOrderInput.RawMaterialItems.OrderBy(u => u.ItemId).ToList();
                rawItems.ShouldNotBeNull();
                rawItems.Count().ShouldBe(InputrawItems.Count());
                for (var i = 0; i < rawItems.Count; i++)
                {
                    var rawItem = rawItems[i];
                    var inpuchaseOrderItem = InputrawItems[i];
                    ShouldBeTestExtensions.ShouldBe(rawItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(rawItem.Qty, inpuchaseOrderItem.Unit);
                    ShouldBeTestExtensions.ShouldBe(rawItem.Description, inpuchaseOrderItem.Description);
                }

                var finishItems = context.FinishItems.Where(u => u.ProductionId == ProductionEntity.Id).OrderBy(u => u.ItemId).ToList();
                var InputfinishItems = ProductionOrderInput.FinishItems.OrderBy(u => u.ItemId).ToList();
                finishItems.ShouldNotBeNull();
                finishItems.Count().ShouldBe(InputfinishItems.Count());
                for (var i = 0; i < finishItems.Count; i++)
                {
                    var finhsItem = finishItems[i];
                    var inpuchaseOrderItem = InputfinishItems[i];
                    ShouldBeTestExtensions.ShouldBe(finhsItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(finhsItem.Qty, inpuchaseOrderItem.Unit);
                    ShouldBeTestExtensions.ShouldBe(finhsItem.Description, inpuchaseOrderItem.Description);
                }

            });

        }

        [Fact]
        public async Task Test_CreateProductionOrder()
        {
            var ProductionOrderExemptInput = CreateHelper();
            var result = await _ProductionOrderAppService.Create(ProductionOrderExemptInput);
            result.ShouldNotBeNull();
            CheckProductionOrder(ProductionOrderExemptInput);

        }

        [Fact]
        public async Task Test_FindProductionOrder()
        {

            Guid test = Guid.NewGuid();
            var ProductionOrderExemptInput = CreateHelper();
            var result = await _ProductionOrderAppService.Create(ProductionOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _ProductionOrderAppService.Find(new GetListProductionInput()
            {
                Items = null,
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
            }
        }


        [Fact]
        public async Task Test_UpdateProductionOrder()
        {
            Guid PurhcaseOrderItemId = Guid.Empty;
            var orderInput = CreateHelper();
            var result = await _ProductionOrderAppService.Create(orderInput);
            result.ShouldNotBeNull();
            CheckProductionOrder(orderInput);

            UsingDbContext(context =>
            {
                var purchaseOrderItem = context.RawMaterialItems.FirstOrDefault(u => u.ItemId == orderInput.RawMaterialItems[0].ItemId);
                purchaseOrderItem.ShouldNotBeNull();
                PurhcaseOrderItemId = purchaseOrderItem.Id;
            });
            PurhcaseOrderItemId.ShouldNotBe(Guid.Empty);

            var updateInput = new UpdateProductionInput()
            {
                FromClassId = _class.Id,
                FromLocationId = _location.Id,
                ToClassId = _class.Id,
                ToLocationId = _location.Id,
                Status = TransactionStatus.Publish,
                Id = result.Id.Value,
                Memo = "Product from Japan",
                Date = Convert.ToDateTime("2018-01-18"),
                ProductionNo = "P002",
                Reference = "P003",
                FinishItems = new List<CreateOrUpdateFinishItemInput>()
                {
                  new CreateOrUpdateFinishItemInput() {
                      ItemId =_item.Id,
                      Description ="Product From Japan",
                      Unit=100
                  }
                },
                RawMaterialItems = new List<CreateOrUpdateRawMaterialItemsInput>()
                {
                  new CreateOrUpdateRawMaterialItemsInput() {
                      ItemId =_item.Id,
                      Description ="Product From Japan",
                      Unit=100
                  }
                }
            };
            var updatedResult = await _ProductionOrderAppService.Update(updateInput);
            updatedResult.ShouldNotBeNull();
            CheckProductionOrder(updateInput, result.Id.Value);

        }

        [Fact]      
        public async Task Test_DeleteProductionOrder()
        {
            var input = CreateHelper();
            var result = await _ProductionOrderAppService.Create(input);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var ProductionOrderEntity = context.Productions.FirstOrDefault(p => p.ProductionNo == input.ProductionNo);
            });
            await _ProductionOrderAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.Productions.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListProductionOrder()
        {
            Guid test = Guid.NewGuid();
            var input = CreateHelper();
            var result = await _ProductionOrderAppService.Create(input);

            result.Id.ShouldNotBeNull();
            var output = await _ProductionOrderAppService.GetList(new GetListProductionInput()
            {
                // Items = new List<Guid> {_item.Id },
                FromDate = Convert.ToDateTime("2017-01-25T09:00:14.508+07:00"),
                ToDate = Convert.ToDateTime("2019-02-25T09:00:14.508+07:00"),
                // FromDate = DateTime.Now.AddDays(-10),
                // ToDate = DateTime.Now.AddDays(10),
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,


            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
            }
        }
    }
}
