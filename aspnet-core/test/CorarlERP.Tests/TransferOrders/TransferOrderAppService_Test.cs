using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.Taxes;
using CorarlERP.TransferOrders;
using CorarlERP.TransferOrders.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.TransferOrders
{
    public class TransferOrderAppService_Test : AppTestBase
    {


        private readonly ITransferOrderAppService _TransferOrderAppService;
        private readonly Currency _currency;
        private readonly Location _location;
        private readonly Class _class;
        private readonly Item _item;
        private readonly ItemType _itemType;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public TransferOrderAppService_Test()
        {
            _TransferOrderAppService = Resolve<ITransferOrderAppService>();
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

        private CreateTransferOrderInput CreateHelper()
        {
            var TransferOrderExemptInput = new CreateTransferOrderInput()
            {
                Status = TransactionStatus.Draft,
                Memo = "Product from UAS",
                TransferDate = DateTime.Now,
                TransferNo = "T001",
                Reference = "T001",
                TransferFromLocationId = _location.Id,
                TransferToLocationId = _location.Id,
                TransferFromClassId = _class.Id,
                TransferToClassId = _class.Id,
                TransferOrderItems = new List<CreateOrUpdateTransferOrderItemInput>()
                {
                      new CreateOrUpdateTransferOrderItemInput() {
                          ItemId = _item.Id,
                          Description ="Product From UAS",
                          Unit = 1
                      },
                      new CreateOrUpdateTransferOrderItemInput() {
                          ItemId =_item.Id,
                          Description ="Product From UAS",
                          Unit =10
                      }
                }
            };
            return TransferOrderExemptInput;

        }

        [Fact]
        public async Task Test_FindTransferOrder()
        {

            Guid test = Guid.NewGuid();
            var TransferOrderExemptInput = CreateHelper();
            var result = await _TransferOrderAppService.Create(TransferOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _TransferOrderAppService.Find(new GetTransferOrderListInput()
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
        public async Task Test_CreateTransferOrder()
        {
            var TransferOrderExemptInput = CreateHelper();
            var result = await _TransferOrderAppService.Create(TransferOrderExemptInput);
            result.ShouldNotBeNull();
            CheckTransferOrder(TransferOrderExemptInput);

        }

        private void CheckTransferOrder(CreateTransferOrderInput transferOrderInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var transferEntity = context.TransferOrders.FirstOrDefault(p => p.TransferNo == transferOrderInput.TransferNo);
                transferEntity.ShouldNotBeNull();
                if (Id != null) transferEntity.Id.ShouldBe(Id.Value);                

                transferEntity.TransferDate.ShouldBe(transferOrderInput.TransferDate);
                transferEntity.TransferNo.ShouldBe(transferOrderInput.TransferNo);
                transferEntity.Memo.ShouldBe(transferOrderInput.Memo);

                var transferOrderItems = context.TransferOrderItems.Where(u => u.TransferOrderId == transferEntity.Id).OrderBy(u => u.ItemId).ToList();
                var InputTransferOrderItems = transferOrderInput.TransferOrderItems.OrderBy(u => u.ItemId).ToList();
                transferOrderItems.ShouldNotBeNull();
                transferOrderItems.Count().ShouldBe(InputTransferOrderItems.Count());
                for (var i = 0; i < transferOrderItems.Count; i++)
                {
                    var purchaseOrderItem = transferOrderItems[i];
                    var inpuchaseOrderItem = InputTransferOrderItems[i];
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Qty, inpuchaseOrderItem.Unit);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Description, inpuchaseOrderItem.Description);
                }
            });

        }


        [Fact]
        public async Task Test_UpdateTransferOrder()
        {
            Guid PurhcaseOrderItemId = Guid.Empty;
            var orderInput = CreateHelper();
            var result = await _TransferOrderAppService.Create(orderInput);
            result.ShouldNotBeNull();
            CheckTransferOrder(orderInput);

            UsingDbContext(context =>
            {
                var purchaseOrderItem = context.PurchaseOrderItems.FirstOrDefault(u => u.ItemId == orderInput.TransferOrderItems[0].ItemId);
                purchaseOrderItem.ShouldNotBeNull();
                PurhcaseOrderItemId = purchaseOrderItem.Id;
            });
            PurhcaseOrderItemId.ShouldNotBe(Guid.Empty);

            var updateInput = new UpdateTransferOrderInput()
            {
                Status = TransactionStatus.Publish,
                Id = result.Id.Value,
                Memo = "Product from Japan",
                TransferDate = Convert.ToDateTime("2018-01-18"),
                TransferNo = "P002",
                Reference = "P003",
                TransferOrderItems = new List<CreateOrUpdateTransferOrderItemInput>()
                {
                  new CreateOrUpdateTransferOrderItemInput() {
                      ItemId =_item.Id,
                      Description ="Product From Japan",
                      Unit=100
                  }
                }

            };
            var updatedResult = await _TransferOrderAppService.Update(updateInput);
            updatedResult.ShouldNotBeNull();

            CheckTransferOrder(updateInput, result.Id.Value);
            
        }

        [Fact]
        public async Task Test_DeleteTransferOrder()
        {
            var input = CreateHelper();
            var result = await _TransferOrderAppService.Create(input);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var transferOrderEntity = context.TransferOrders.FirstOrDefault(p => p.TransferNo == input.TransferNo);
            });
            await _TransferOrderAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.PurchaseOrders.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListTransferOrder()
        {
            Guid test = Guid.NewGuid();
            var input = CreateHelper();
            var result = await _TransferOrderAppService.Create(input);

            result.Id.ShouldNotBeNull();
            var output = await _TransferOrderAppService.GetList(new GetTransferOrderListInput()
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
