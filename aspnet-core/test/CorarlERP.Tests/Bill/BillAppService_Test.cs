using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.Bills;
using CorarlERP.Bills.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.PurchaseOrders;
using CorarlERP.Taxes;
using CorarlERP.Vendors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.Bills.Bill;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Tests.Bill
{
   public class BillAppService_Test : AppTestBase
    {
        private readonly IBillAppService _billAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public readonly Class _class;
        public readonly Location _location;
        private readonly Vendor _vendor;
        private readonly Item _item;
        private readonly Item _item1;
        private readonly ItemType _itemType;
        private readonly PurchaseOrder _purchaseOrder;
        private readonly PurchaseOrderItem _purchaseOrderItem;
        public BillAppService_Test()
        {
            _billAppService = Resolve<IBillAppService>();
            _class = CreateClass(null, null, "Ione II", false, null);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                       displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _vendor = CreateVendor(VendorName: "Ione", VendorCode: "001", Email: "ione@gmail.com", PhoneNumber: "012334455", SameAsShippingAddress: true, Websit: "ione.com.kh",
              Remark: "Remark", BillingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),

              ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), accountId: _chartOfAccount.Id, vendorType: null);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null, purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            _purchaseOrder = CreatePurchaseOrder(_vendor.Id, new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), true, "Order from Thai", _currency.Id, "001", DateTime.Now, "To KH", 10, 10, 100, TransactionStatus.Publish,DateTime.Now);
            _purchaseOrderItem = CreatePurchaseOrderItem(_purchaseOrder.Id, _item.Id, _tax.Id, 10, "Description", 100, 1000, 10, 1000);
            _item1 = CreateItem(ItemName: "Computer Laptop", ItemCode: "0002", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null,
                 purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: null, itemTypeId: _itemType.Id, description: "",
                 barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
        }

        private CreateBillInput CreateHelper()
        {

            var BillExemptInput = new CreateBillInput()
            {
                ConvertToItemReceipt = true,
                RecieptDate = DateTime.Now,
                CurrencyId = _currency.Id,
                Memo = "Product from UAS",
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
                ClearanceAccountId = _chartOfAccount.Id,
                SameAsShippingAddress = false,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#180E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                BillNo = "P000001",
                ReceiveFrom = ReceiveFromStatus.PO,
                Reference = "P000001",
                VendorId = _vendor.Id,
                SubTotal = 100,
                Tax = 10,
                Total = 90,
                Status = TransactionStatus.Publish,
                BillItems = new List<CreateOrUpdateBillItemInput>()
                {
                    new CreateOrUpdateBillItemInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",ItemReceiptId = null,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,TaxId = _tax.Id,Total = 100,UnitCost = 10,ItemId = _item1.Id
                    },
                     new CreateOrUpdateBillItemInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",ItemReceiptId = null,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,TaxId = _tax.Id,Total=100,UnitCost = 100,ItemId = _item.Id
                    },
                }

            };
            return BillExemptInput;

        }

        private void CheckBill(CreateBillInput JournalExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.BillNo);
                JournalEntity.ShouldNotBeNull();
                // if (Id != null) JournalEntity.Id.ShouldBe(Id.Value);
                JournalEntity.ItemReceiptId.ShouldBe(JournalEntity.ItemReceiptId);
                JournalEntity.Credit.ShouldBe(JournalExemptInput.Total);
                JournalEntity.Debit.ShouldBe(0);
                JournalEntity.CurrencyId.ShouldBe(JournalExemptInput.CurrencyId);
                JournalEntity.Date.ShouldBe(JournalExemptInput.Date);
                JournalEntity.Memo.ShouldBe(JournalExemptInput.Memo);
                JournalEntity.IsActive.ShouldBe(true);
                var journalItems = context.JournalItems.Where(u => u.JournalId == JournalEntity.Id && u.Debit != 0).OrderBy(u => u.AccountId).ToList();
                var InputJournalItems = JournalExemptInput.BillItems.OrderBy(u => u.InventoryAccountId).ToList();
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
                var bill = context.Bills.FirstOrDefault(p => p.VendorId == JournalExemptInput.VendorId);
                var jItemId = context.JournalItems.FirstOrDefault(u => u.Debit == 0);
                // if (Id != null) JournalEntity.Id.ShouldBe(Id.Value);
              //  bill.LocationId.ShouldBe(JournalExemptInput.LocationId);
                bill.SameAsShippingAddress.ShouldBe(JournalExemptInput.SameAsShippingAddress);
                bill.BillingAddress.ShouldBe(JournalExemptInput.BillingAddress);
                bill.ShippingAddress.ShouldBe(JournalExemptInput.SameAsShippingAddress ?
                                                      JournalExemptInput.BillingAddress :
                                                      JournalExemptInput.ShippingAddress);
                bill.SubTotal.ShouldBe(JournalExemptInput.SubTotal);
                bill.Tax.ShouldBe(JournalExemptInput.Tax);
                bill.Total.ShouldBe(JournalExemptInput.Total);
                bill.VendorId.ShouldBe(JournalExemptInput.VendorId);

                var billItems = context.BillItems.Where(u => u.BillId == bill.Id).OrderBy(u => u.ItemId).ToList();
                var InputItemReceiptItem = JournalExemptInput.BillItems.OrderBy(u => u.ItemId).ToList();
                billItems.ShouldNotBeNull();
                billItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < billItems.Count; i++)
                {
                    var itemReceiptItem = billItems[i];
                    var initemReceiptItem = InputItemReceiptItem[i];
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemId, itemReceiptItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemReceiptItemId, itemReceiptItem.ItemReceiptItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Description, itemReceiptItem.Description);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.DiscountRate, itemReceiptItem.DiscountRate);
                    // ShouldBeTestExtensions.ShouldBe(itemReceiptItem.InventoryJournalId, itemReceiptItem.InventoryJournalId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemReceiptItemId, itemReceiptItem.ItemReceiptItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.OrderItemId, itemReceiptItem.OrderItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Qty, itemReceiptItem.Qty);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.TaxId, itemReceiptItem.TaxId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Total, itemReceiptItem.Total);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.UnitCost, itemReceiptItem.UnitCost);

                };

            });
        }

        [Fact]
        public async Task Test_CreateBill()
        {
            var BillExemptInput = CreateHelper();
            var result = await _billAppService.Create(BillExemptInput);
            result.ShouldNotBeNull();
            CheckBill(BillExemptInput);


        }

        [Fact]
        public async Task Test_GetDetail()
        {

            var billExemptInput = CreateHelper();
            var result = await _billAppService.Create(billExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _billAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.BillingAddress.ShouldBe(billExemptInput.BillingAddress);
            output.LocationId.ShouldBe(billExemptInput.LocationId);
            output.SubTotal.ShouldBe(billExemptInput.SubTotal);
            output.Tax.ShouldBe(billExemptInput.Tax);
            output.Total.ShouldBe(billExemptInput.Total);
            output.VendorId.ShouldBe(billExemptInput.VendorId);
            CheckBill(billExemptInput);
        }

        [Fact]
        public async Task Test_Delete()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _billAppService.Create(JournalExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var itemReceipEntity = context.Bills.FirstOrDefault(p => p.VendorId == JournalExemptInput.VendorId);
            });
            await _billAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.Bills.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_Find()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _billAppService.Create(ItemReceiptExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _billAppService.Find(new GetListBillInput()
            {
                //FromDate = DateTime.Now.AddDays(-10),
                //ToDate = DateTime.Now.AddDays(10),
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
               // Items = new List<Guid?> { _item.Id,_item1.Id },
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(ItemReceiptExemptInput.BillNo);
                v.Date.ShouldBe(ItemReceiptExemptInput.Date);
                v.Status.ShouldBe(ItemReceiptExemptInput.Status);
                v.VendorId.ShouldBe(ItemReceiptExemptInput.VendorId);
                v.Total.ShouldBe(ItemReceiptExemptInput.Total);

            }

        }

        //[Fact]
        //public async Task Test_GetList()
        //{
        //    var billInput = CreateHelper();
        //    var result = await _billAppService.Create(billInput);

        //    result.Id.ShouldNotBeNull();
        //    var output = await _billAppService.GetList(new GetListBillInput()
        //    {
        //        FromDate = DateTime.Now.AddDays(-10),
        //        ToDate = DateTime.Now.AddDays(10),
        //        Filter = null,
        //        Sorting = null,
        //        MaxResultCount = 10,
        //        SkipCount = 0,
        //        Items = new List<Guid?> { _item.Id, _item1.Id },
        //    });

        //    foreach (var v in output.Items)
        //    {
        //        v.Id.ShouldNotBeNull();
        //        v.Id.ShouldBe(result.Id.Value);
        //        v.JournalNo.ShouldBe(billInput.BillNo);
        //        v.Date.ShouldBe(billInput.Date);
        //        v.Status.ShouldBe(billInput.Status);
        //        v.VendorId.ShouldBe(billInput.VendorId);
        //        v.Total.ShouldBe(billInput.Total);
        //        v.DueDate.ShouldBe(billInput.DueDate);
        //        v.ReceivedStatus.ShouldBe(DeliveryStatus.ReceivePending);
        //    }

        //}

        [Fact]
        public async Task Test_GetListForPayBillList()
        {
            var billInput = CreateHelper();
            var result = await _billAppService.Create(billInput);

            result.Id.ShouldNotBeNull();
            var output = await _billAppService.GetListBillForPayBill(new GetListBillForPaybillInput()
            {
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(10),
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(billInput.BillNo);
                v.Date.ShouldBe(billInput.Date);
                v.AccountId.ShouldBe(billInput.ClearanceAccountId);
                v.VendorId.ShouldBe(billInput.VendorId);
                v.Total.ShouldBe(billInput.Total);
                v.DueDate.ShouldBe(billInput.DueDate);
            }

        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _billAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckBill(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {
                var ItemReceipt = context.BillItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.BillItems[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = ItemReceipt.Id;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateItemReceiptExemptInput = new UpdateBillInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Product from England",
                Date = DateTime.Now,
                ClearanceAccountId = _chartOfAccount.Id,
                SameAsShippingAddress = true,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                // ShippingAddress = new Address("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#185E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                BillNo = "P000001",
                ReceiveFrom = ReceiveFromStatus.PO,
                Reference = "P000001",
                VendorId = _vendor.Id,
                SubTotal = 100,
                Tax = 10,
                Total = 90,
                Status = TransactionStatus.Publish,
                BillItems = new List<CreateOrUpdateBillItemInput>()
                {
                    new CreateOrUpdateBillItemInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",ItemReceiptId = null,ItemId =_item.Id,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,TaxId = _tax.Id,Total = 100,UnitCost = 10
                    },
                     new CreateOrUpdateBillItemInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",ItemReceiptId = null,ItemId = _item.Id,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,TaxId = _tax.Id,Total=100,UnitCost = 100,
                    },
                }

            };
            var updatedResult = await _billAppService.Update(UpdateItemReceiptExemptInput);
            updatedResult.ShouldNotBeNull();

          //  CheckBill(UpdateItemReceiptExemptInput, result.Id.Value);


        }


        [Fact]
        public async Task Test_GetBill()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _billAppService.Create(ItemReceiptExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _billAppService.GetBills(new GetBillListInput()
            {
              
                //Filter = null,
                //Sorting = null,
                //MaxResultCount = 10,
                //SkipCount = 0,
              
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.BillNo.ShouldBe(ItemReceiptExemptInput.BillNo);
                v.Date.ShouldBe(ItemReceiptExemptInput.Date);
                v.Total.ShouldBe(ItemReceiptExemptInput.Total);

            }

        }


        [Fact]
        public async Task Test_GetBillItems()
        {
            var billExemptInput = CreateHelper();
            var result = await _billAppService.Create(billExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _billAppService.GetBillItems(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);       
            CheckBill(billExemptInput);
        }

        [Fact]
        public async Task Test_UpdateStatusToVoid()
        {
            Guid BillId = Guid.Empty;
            var BillExemptInput = CreateHelper();
            var result = await _billAppService.Create(BillExemptInput);
            result.ShouldNotBeNull();
            CheckBill(BillExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.BillItems.FirstOrDefault(u => u.ItemId == BillExemptInput.BillItems[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                BillId = result.Id.Value;
            });
            BillId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = BillId,
            };
            var updatedResult = await _billAppService.UpdateStatusToVoid(UpdateStatusInput);

        }
    }
}
