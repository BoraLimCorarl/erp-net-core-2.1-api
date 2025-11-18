using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Journals.Dto;
using CorarlERP.PurchaseOrders;
using CorarlERP.PurchaseOrders.Dto;
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

namespace CorarlERP.Tests.PurchaseOrders
{
  public  class PurchaseOrderAppSevice_Test : AppTestBase
    {
        private readonly IPurchaseOrderAppService _purchaseOrderAppService;
        private readonly Currency _currency;
        private readonly Vendor _vendor;
        private readonly Item _item;
        private readonly ItemType _itemType;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public PurchaseOrderAppSevice_Test()
        {
            _purchaseOrderAppService = Resolve<IPurchaseOrderAppService>();
            _tax = CreateTax("Tax Exempt", 0);
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                        displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: _chartOfAccount.Id, purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);           
            _vendor = CreateVendor(VendorName: "Ione", VendorCode: "001", Email: "ione@gmail.com", PhoneNumber: "012334455",SameAsShippingAddress:true, Websit: "ione.com.kh",
                Remark: "Remark", BillingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),accountId:_chartOfAccount.Id, vendorType: null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
           
           
        }
        private CreatePurchaseOrderInput CreateHelper()
        {
            var PurchaseOrderExemptInput = new CreatePurchaseOrderInput()
            {
                Status = TransactionStatus.Draft,
                CurrencyId= _currency.Id,
                Memo = "Product from UAS",
                OrderDate= DateTime.Now,
                IsActive=true,
                Tax= 0,
                SubTotal=100,
                Total=100,
                OrderNumber ="P001",
                Reference = "P001",
                VendorId= _vendor.Id,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                SameAsShippingAddress = false,
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1214", "Phnom Penh", "#186E0 Monivong Blvd"),
                PurchaseOrderItems = new List<CreateOrUpdatePurchaseOrderItemInput>()
                {
                      new CreateOrUpdatePurchaseOrderItemInput() {ItemId=_item.Id,Description="Product From UAS",DiscountRate =10,TaxId=_tax.Id,TaxRate=10,Total=1000 ,Unit=10,UnitCost=10,TotalReceiptQty = 100,TotalBillQty=100} ,
                      new CreateOrUpdatePurchaseOrderItemInput() {ItemId=_item.Id,Description="Product From UAS",DiscountRate =10,TaxId=_tax.Id,TaxRate=10,Total=1000 ,Unit=10,UnitCost=10,TotalReceiptQty = 1,TotalBillQty=1}
                }
            };
            return PurchaseOrderExemptInput;

        }

        [Fact]
        public async Task Test_FindPurchaseOrder()
        {

            Guid test = Guid.NewGuid();
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _purchaseOrderAppService.Find(new GetPurchaseOrderListInput()
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
                v.VendorId.ShouldBe(PurchaseOrderExemptInput.VendorId);
                v.Reference.ShouldBe(PurchaseOrderExemptInput.Reference);
                v.Tax.ShouldBe(PurchaseOrderExemptInput.Tax);
                v.Total.ShouldBe(PurchaseOrderExemptInput.Total);
                v.OrderDate.ShouldBe(PurchaseOrderExemptInput.OrderDate);

            }
        }


        [Fact]
        public async Task Test_CreatePurchaseOrder()
        {
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);
            result.ShouldNotBeNull();
            CheckPurchaseOrder(PurchaseOrderExemptInput);

        }
        [Fact]
        public async Task Test_UpdatePurchaseOrder()
        {
            Guid PurhcaseOrderItemId = Guid.Empty;
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);
            result.ShouldNotBeNull();
            CheckPurchaseOrder(PurchaseOrderExemptInput);

            UsingDbContext(context =>
            {
                var purchaseOrderItem = context.PurchaseOrderItems.FirstOrDefault(u => u.ItemId == PurchaseOrderExemptInput.PurchaseOrderItems[0].ItemId);
                purchaseOrderItem.ShouldNotBeNull();
                PurhcaseOrderItemId = purchaseOrderItem.Id;
            });
            PurhcaseOrderItemId.ShouldNotBe(Guid.Empty);

            var UpdatePurchaseOrderExemptInput = new UpdatePurchaseOrderInput()
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
                VendorId = _vendor.Id,
                BillingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1012", "Phnom Penh", "#184E0 Monivong Blvd"),
                SameAsShippingAddress = false,
                ShippingAddress = new CAddress("Khan Chamkarmourn III", "KH", "10112", "Phnom Penh", "#14E0 Monivong Blvd"),
                PurchaseOrderItems = new List<CreateOrUpdatePurchaseOrderItemInput>()
                {
                  new CreateOrUpdatePurchaseOrderItemInput() {ItemId=_item.Id,Description="Product From Japan",DiscountRate =10,TaxId=_tax.Id,TaxRate=100,Total=10000 ,Unit=100,UnitCost=100}
                }

            };
            var updatedResult = await _purchaseOrderAppService.Update(UpdatePurchaseOrderExemptInput);
            updatedResult.ShouldNotBeNull();

            CheckPurchaseOrder(UpdatePurchaseOrderExemptInput, result.Id.Value);


        }
    
        [Fact]
        public async Task Test_DelectPurchaseOrder()
        {
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var PurchaseOrderEntity = context.PurchaseOrders.FirstOrDefault(p => p.OrderNumber == PurchaseOrderExemptInput.OrderNumber);
            });
            await _purchaseOrderAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.PurchaseOrders.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListPurchaseOrder()
        {
            Guid test = Guid.NewGuid();
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _purchaseOrderAppService.GetList(new GetPurchaseOrderListInput()
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
                v.VendorId.ShouldBe(PurchaseOrderExemptInput.VendorId);
                v.Reference.ShouldBe(PurchaseOrderExemptInput.Reference);
                v.Tax.ShouldBe(PurchaseOrderExemptInput.Tax);
                v.Total.ShouldBe(PurchaseOrderExemptInput.Total);
                v.OrderDate.ShouldBe(PurchaseOrderExemptInput.OrderDate);

            }
        }
        [Fact]
        public async Task Test_GetDetailPurchaseOrder()
        {

            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _purchaseOrderAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.VendorId.ShouldBe(PurchaseOrderExemptInput.VendorId);
            output.Reference.ShouldBe(PurchaseOrderExemptInput.Reference);
            output.Tax.ShouldBe(PurchaseOrderExemptInput.Tax);
            output.Total.ShouldBe(PurchaseOrderExemptInput.Total);
            output.OrderDate.ShouldBe(PurchaseOrderExemptInput.OrderDate);
            CheckPurchaseOrder(PurchaseOrderExemptInput);
        }
     


        private void CheckPurchaseOrder(CreatePurchaseOrderInput ProchaseOrderExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var PurchaseEntity = context.PurchaseOrders.FirstOrDefault(p => p.OrderNumber == ProchaseOrderExemptInput.OrderNumber);
                PurchaseEntity.ShouldNotBeNull();
                if (Id != null) PurchaseEntity.Id.ShouldBe(Id.Value);
                PurchaseEntity.VendorId.ShouldBe(ProchaseOrderExemptInput.VendorId);
                PurchaseEntity.Tax.ShouldBe(ProchaseOrderExemptInput.Tax);
                PurchaseEntity.SubTotal.ShouldBe(ProchaseOrderExemptInput.SubTotal);
                PurchaseEntity.Total.ShouldBe(ProchaseOrderExemptInput.Total);
                PurchaseEntity.SameAsShippingAddress.ShouldBe(ProchaseOrderExemptInput.SameAsShippingAddress);


                PurchaseEntity.BillingAddress.ShouldBe(ProchaseOrderExemptInput.BillingAddress);
                PurchaseEntity.ShippingAddress.ShouldBe(ProchaseOrderExemptInput.SameAsShippingAddress ?
                                                      ProchaseOrderExemptInput.BillingAddress :
                                                      ProchaseOrderExemptInput.ShippingAddress);

                PurchaseEntity.OrderDate.ShouldBe(ProchaseOrderExemptInput.OrderDate);
                PurchaseEntity.OrderNumber.ShouldBe(ProchaseOrderExemptInput.OrderNumber);
                PurchaseEntity.Memo.ShouldBe(ProchaseOrderExemptInput.Memo);
                PurchaseEntity.IsActive.ShouldBe(true);
                var @purchaseOrderItems = context.PurchaseOrderItems.Where(u => u.PurchaseOrderId == PurchaseEntity.Id).OrderBy(u => u.ItemId).ToList();
                var InputPurchaseOrderItems = ProchaseOrderExemptInput.PurchaseOrderItems.OrderBy(u => u.ItemId).ToList();
                purchaseOrderItems.ShouldNotBeNull();
                purchaseOrderItems.Count().ShouldBe(InputPurchaseOrderItems.Count());
                for (var i = 0; i < purchaseOrderItems.Count; i++)
                {
                    var purchaseOrderItem = purchaseOrderItems[i];
                    var inpuchaseOrderItem = InputPurchaseOrderItems[i];
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.TaxId, inpuchaseOrderItem.TaxId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Total, inpuchaseOrderItem.Total);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.ItemId, inpuchaseOrderItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Unit, inpuchaseOrderItem.Unit);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.UnitCost, inpuchaseOrderItem.UnitCost);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.DiscountRate, inpuchaseOrderItem.DiscountRate);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.Description, inpuchaseOrderItem.Description);
                    ShouldBeTestExtensions.ShouldBe(purchaseOrderItem.TaxRate, inpuchaseOrderItem.TaxRate);

                }
            });
        }

        [Fact]
        public async Task Test_GetTotalPurchaseOrder()
        {
            Guid test = Guid.NewGuid();
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _purchaseOrderAppService.GetPurchaseOrders(new GetTotalPurchaseOrderListInput()
            {
               Filter= ""
            });
            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.Total.ShouldBe(PurchaseOrderExemptInput.Total);
                v.OrderDate.ShouldBe(PurchaseOrderExemptInput.OrderDate);

            }
        }

        [Fact]
        public async Task Test_GetTotalPurchaseOrderForBill()
        {
            Guid test = Guid.NewGuid();
            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _purchaseOrderAppService.GetPurchaseOrders(new GetTotalPurchaseOrderListInput()
            {
                Filter = ""               
            });
            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.Total.ShouldBe(PurchaseOrderExemptInput.Total);
                v.OrderDate.ShouldBe(PurchaseOrderExemptInput.OrderDate);

            }
        }


        [Fact]
        public async Task Test_GetDetail()
        {

            var PurchaseOrderExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(PurchaseOrderExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _purchaseOrderAppService.GetItemPuchaseOrders(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);          
            output.OrderDate.ShouldBe(PurchaseOrderExemptInput.OrderDate);
           // CheckPurchaseOrder(PurchaseOrderExemptInput);
        }
        [Fact]
        public async Task Test_UpdateStatus()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckPurchaseOrder(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.PurchaseOrderItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.PurchaseOrderItems[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _purchaseOrderAppService.UpdateStatusToPublish(UpdateStatusInput);

        }

        [Fact]
        public async Task Test_UpdateStatusToDraft()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckPurchaseOrder(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.PurchaseOrderItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.PurchaseOrderItems[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _purchaseOrderAppService.UpdateStatusToDraft(UpdateStatusInput);

        }


        [Fact]
        public async Task Test_UpdateStatusToVoid()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckPurchaseOrder(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.PurchaseOrderItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.PurchaseOrderItems[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _purchaseOrderAppService.UpdateStatusToVoid(UpdateStatusInput);

        }


        [Fact]
        public async Task Test_UpdateStatusToClose()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _purchaseOrderAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckPurchaseOrder(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.PurchaseOrderItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.PurchaseOrderItems[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _purchaseOrderAppService.UpdateStatusToClose(UpdateStatusInput);

        }

    }
}
