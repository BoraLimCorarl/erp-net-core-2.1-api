using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Journals.Dto;
using CorarlERP.SaleOrders;
using CorarlERP.SaleOrders.Dto;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.SaleOrders
{
    public class SaleOrderAppService_Test: AppTestBase
    {
        private readonly ISaleOrderAppService _saleOrderAppService;
        private readonly Currency _currency;
        private readonly Customer _customer;
        private readonly Item _item;
        private readonly ItemType _itemType;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public SaleOrderAppService_Test()
        {
            _saleOrderAppService = Resolve<ISaleOrderAppService>();
            _tax = CreateTax("Tax Exempt", 0);
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                        displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: _chartOfAccount.Id, purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            _customer = CreateCustomer(Name: "Sokhom", Code: "001", Email: "sokhom@gmail.com", PhoneNumber: "012334455", SameAsShippingAddress: true, Websit: "sokhom.com.kh",
                Remark: "Remark", BillingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), accountId: _chartOfAccount.Id, customerTypeId: 1);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
        }

        private CreateSaleOrderInput CreateHelper()
        {
            var PurchaseOrderExemptInput = new CreateSaleOrderInput()
            {
                Status = TransactionStatus.Draft,
                CurrencyId = _currency.Id,
                Memo = "Product from UAS",
                OrderDate = DateTime.Now,
                IsActive = true,
                Tax = 0,
                SubTotal = 100,
                Total = 100,
                OrderNumber = "S001",
                Reference = "S001",
                CustomerId = _customer.Id,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                SameAsShippingAddress = false,
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1214", "Phnom Penh", "#186E0 Monivong Blvd"),
                SaleOrderItems = new List<CreateOrUpdateSaleOrderItemInput>()
                {
                      new CreateOrUpdateSaleOrderItemInput() {
                          ItemId =_item.Id,
                          Description ="IPhoneX Gold",
                          DiscountRate = 10,
                          TaxId =_tax.Id,                        
                          TaxRate = 10,
                          Total = 8000,
                          Qty = 10,
                          UnitCost = 800
                      } ,
                      new CreateOrUpdateSaleOrderItemInput() {
                          ItemId =_item.Id,
                          Description ="VIVO 9",
                          DiscountRate = 10,
                          TaxId =_tax.Id,
                          TaxRate = 10,
                          Total = 2000,
                          Qty = 10,
                          UnitCost = 200
                      }
                }
            };
            return PurchaseOrderExemptInput;

        }

        [Fact]
        public async Task Test_FindSaleOrder()
        {

            Guid test = Guid.NewGuid();
            var saleOrderExemptInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _saleOrderAppService.Find(new GetSaleOrderListInput()
            {
                Items = new List<Guid> { },
                //FromDate = Convert.ToDateTime("2018-01-25T09:00:14.508+07:00"),
                //ToDate = Convert.ToDateTime("2018-01-25T09:00:14.508+07:00"),
                // IsActive = null,
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.CustomerId.ShouldBe(saleOrderExemptInput.CustomerId);
                v.Reference.ShouldBe(saleOrderExemptInput.Reference);
                v.Tax.ShouldBe(saleOrderExemptInput.Tax);
                v.Total.ShouldBe(saleOrderExemptInput.Total);
                v.OrderDate.ShouldBe(saleOrderExemptInput.OrderDate);

            }
        }

        [Fact]
        public async Task Test_CreatePurchaseOrder()
        {
            var saleOrderInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderInput);
            result.ShouldNotBeNull();
            CheckSaleOrderValid(saleOrderInput, result.Id);
        }

        private void CheckSaleOrderValid(CreateSaleOrderInput saleOrderInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var saleEntity = context.SaleOrder.FirstOrDefault(p => p.OrderNumber == saleOrderInput.OrderNumber);
                saleEntity.ShouldNotBeNull();
                if (Id != null) saleEntity.Id.ShouldBe(Id.Value);
                saleEntity.CustomerId.ShouldBe(saleOrderInput.CustomerId);
                saleEntity.Tax.ShouldBe(saleOrderInput.Tax);
                saleEntity.SubTotal.ShouldBe(saleOrderInput.SubTotal);
                saleEntity.Total.ShouldBe(saleOrderInput.Total);
                saleEntity.SameAsShippingAddress.ShouldBe(saleOrderInput.SameAsShippingAddress);


                saleEntity.BillingAddress.ShouldBe(saleOrderInput.BillingAddress);
                saleEntity.ShippingAddress.ShouldBe(saleOrderInput.SameAsShippingAddress ?
                                                      saleOrderInput.BillingAddress :
                                                      saleOrderInput.ShippingAddress);

                saleEntity.OrderDate.ShouldBe(saleOrderInput.OrderDate);
                saleEntity.OrderNumber.ShouldBe(saleOrderInput.OrderNumber);
                saleEntity.Memo.ShouldBe(saleOrderInput.Memo);
                saleEntity.IsActive.ShouldBe(true);
                var saleOrderItems = context.SaleOrderItems.Where(u => u.SaleOrderId == saleEntity.Id).OrderBy(u => u.ItemId).ToList();
                var inputSaleOrderItems = saleOrderInput.SaleOrderItems.OrderBy(u => u.ItemId).ToList();
                saleOrderItems.ShouldNotBeNull();
                saleOrderItems.Count().ShouldBe(inputSaleOrderItems.Count());
                for (var i = 0; i < saleOrderItems.Count; i++)
                {
                    var purchaseOrderItem = saleOrderItems[i];
                    var inpuchaseOrderItem = inputSaleOrderItems[i];
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.TaxId, inpuchaseOrderItem.TaxId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Total, inpuchaseOrderItem.Total);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Qty, inpuchaseOrderItem.Qty);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.UnitCost, inpuchaseOrderItem.UnitCost);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.DiscountRate, inpuchaseOrderItem.DiscountRate);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Description, inpuchaseOrderItem.Description);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.TaxRate, inpuchaseOrderItem.TaxRate);

                }
            });
        }


        [Fact]
        public async Task Test_UpdateSaleOrder()
        {
            Guid saleOrderItemId = Guid.Empty;
            var saleOrderInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderInput);
            result.ShouldNotBeNull();
            CheckSaleOrderValid(saleOrderInput);

            UsingDbContext(context =>
            {
                var saleOrderItem = context.SaleOrderItems.FirstOrDefault(u => u.ItemId == saleOrderInput.SaleOrderItems[0].ItemId);
                saleOrderItem.ShouldNotBeNull();
                saleOrderItemId = saleOrderItem.Id;
            });
            saleOrderItemId.ShouldNotBe(Guid.Empty);

            var updateSaleOrderInput = new UpdateSaleOrderInput()
            {
                Status = TransactionStatus.Publish,
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Product from Japan",
                OrderDate = Convert.ToDateTime("2018-01-18"),
                IsActive = true,
                Tax = 0,
                SubTotal = 100,
                Total = 100,
                OrderNumber = "P002",
                Reference = "P003",
                CustomerId = _customer.Id,
                BillingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1012", "Phnom Penh", "#184E0 Monivong Blvd"),
                SameAsShippingAddress = false,
                ShippingAddress = new CAddress("Khan Chamkarmourn III", "KH", "10112", "Phnom Penh", "#14E0 Monivong Blvd"),
                SaleOrderItems = new List<CreateOrUpdateSaleOrderItemInput>()
                {
                  new CreateOrUpdateSaleOrderItemInput() {
                      ItemId =_item.Id,
                      Description ="Product From Japan",
                      DiscountRate =10,
                      TaxId =_tax.Id,
                      TaxRate =100,
                      Total =10000 ,
                      Qty =100,
                      UnitCost =100
                  }
                }

            };
            var updatedResult = await _saleOrderAppService.Update(updateSaleOrderInput);
            updatedResult.ShouldNotBeNull();

            CheckSaleOrderValid(updateSaleOrderInput, result.Id.Value);


        }

        [Fact]
        public async Task Test_DeleteSaleOrder()
        {
            var saleOrderInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var saleOrderEntity = context.SaleOrder.FirstOrDefault(p => p.OrderNumber == saleOrderInput.OrderNumber);
            });
            await _saleOrderAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.PurchaseOrders.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListSaleOrder()
        {
            Guid test = Guid.NewGuid();
            var saleOrderInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderInput);

            result.Id.ShouldNotBeNull();
            var output = await _saleOrderAppService.GetList(new GetSaleOrderListInput()
            {
                // Items = new List<Guid> {_item.Id },
                FromDate = Convert.ToDateTime("2018-01-25T09:00:14.508+07:00"),
                ToDate = Convert.ToDateTime("2018-02-25T09:00:14.508+07:00"),
                // FromDate = DateTime.Now.AddDays(-10),
                // ToDate = DateTime.Now.AddDays(10),
                //IsActive = null,
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
            });
            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.CustomerId.ShouldBe(saleOrderInput.CustomerId);
                v.Reference.ShouldBe(saleOrderInput.Reference);
                v.Tax.ShouldBe(saleOrderInput.Tax);
                v.Total.ShouldBe(saleOrderInput.Total);
                v.OrderDate.ShouldBe(saleOrderInput.OrderDate);
            }
        }

        [Fact]
        public async Task Test_GetDetailSaleOrder()
        {
            var saleOrderInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderInput);
            result.Id.ShouldNotBeNull();
            var output = await _saleOrderAppService.GetDetail(new EntityDto<Guid>()
            {
                Id = result.Id.Value,
            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.CustomerId.ShouldBe(saleOrderInput.CustomerId);
            output.Reference.ShouldBe(saleOrderInput.Reference);
            output.Tax.ShouldBe(saleOrderInput.Tax);
            output.Total.ShouldBe(saleOrderInput.Total);
            output.OrderDate.ShouldBe(saleOrderInput.OrderDate);
            CheckSaleOrderValid(saleOrderInput);
        }

        [Fact]
        public async Task Test_UpdateStatusToDraft()
        {
            var saleOrderCreateInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderCreateInput);
            result.ShouldNotBeNull();
            CheckSaleOrderValid(saleOrderCreateInput);
            var UpdateStatusInput = new UpdateStatus()
            {
                Id = result.Id.Value,
            };
            var updatedResult = await _saleOrderAppService.UpdateStatusToDraft(UpdateStatusInput);

            UsingDbContext(context =>
            {
                var saleOrder = context.SaleOrder.FirstOrDefault(u => u.Id == UpdateStatusInput.Id);
                saleOrder.ShouldNotBeNull();
                saleOrder.Status.ShouldBe(TransactionStatus.Draft);
            });
        }


        [Fact]
        public async Task Test_UpdateStatusToVoid()
        {
            var saleOrderCreateInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderCreateInput);
            result.ShouldNotBeNull();
            CheckSaleOrderValid(saleOrderCreateInput);
            var UpdateStatusInput = new UpdateStatus()
            {
                Id = result.Id.Value,
            };
            var updatedResult = await _saleOrderAppService.UpdateStatusToVoid(UpdateStatusInput);

            UsingDbContext(context =>
            {
                var saleOrder = context.SaleOrder.FirstOrDefault(u => u.Id == UpdateStatusInput.Id);
                saleOrder.ShouldNotBeNull();
                saleOrder.Status.ShouldBe(TransactionStatus.Void);
            });
        }


        [Fact]
        public async Task Test_UpdateStatusToClose()
        {
            var saleOrderCreateInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderCreateInput);
            result.ShouldNotBeNull();
            CheckSaleOrderValid(saleOrderCreateInput);
            var UpdateStatusInput = new UpdateStatus()
            {
                Id = result.Id.Value,
            };
            var updatedResult = await _saleOrderAppService.UpdateStatusToClose(UpdateStatusInput);

            UsingDbContext(context =>
            {
                var saleOrder = context.SaleOrder.FirstOrDefault(u => u.Id == UpdateStatusInput.Id);
                saleOrder.ShouldNotBeNull();
                saleOrder.Status.ShouldBe(TransactionStatus.Close);
            });
        }


        [Fact]
        public async Task Test_UpdateStatusToPublish()
        {
            var saleOrderCreateInput = CreateHelper();
            var result = await _saleOrderAppService.Create(saleOrderCreateInput);
            result.ShouldNotBeNull();
            CheckSaleOrderValid(saleOrderCreateInput);
            var UpdateStatusInput = new UpdateStatus()
            {
                Id = result.Id.Value,
            };
            var updatedResult = await _saleOrderAppService.UpdateStatusToPublish(UpdateStatusInput);

            UsingDbContext(context =>
            {
                var saleOrder = context.SaleOrder.FirstOrDefault(u => u.Id == UpdateStatusInput.Id);
                saleOrder.ShouldNotBeNull();
                saleOrder.Status.ShouldBe(TransactionStatus.Publish);
            });
        }
    }
}
