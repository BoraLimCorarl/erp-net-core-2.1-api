using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.ItemReceiptTransfers;
using CorarlERP.ItemReceiptTransfers.Dto;
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
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceipts.ItemReceipt;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Tests.ItemReceipts
{
    public class ItemReceiptAppService_Test : AppTestBase
    {
        private readonly IItemReceiptAppService _ItemReceiptAppService;
       
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
        private readonly PurchaseOrder _purchaseOrder1;
        private readonly PurchaseOrderItem _purchaseOrderItem1;
        public ItemReceiptAppService_Test()
        {
          
            _ItemReceiptAppService = Resolve<IItemReceiptAppService>();
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
             
              ShippingAddress: new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), accountId: _chartOfAccount.Id ,vendorType:null);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null, purchaseTaxId: _tax.Id, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: null, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            _purchaseOrder = CreatePurchaseOrder(_vendor.Id, new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), true, "Order from Thai", _currency.Id, "001", DateTime.Now, "To KH", 10, 10, 100, TransactionStatus.Publish,DateTime.Now);
            _purchaseOrderItem = CreatePurchaseOrderItem(_purchaseOrder.Id, _item.Id, _tax.Id, 10, "Description", 100, 1000, 10, 1000);

            _item1 = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null, purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: null, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);


            _purchaseOrder1 = CreatePurchaseOrder(_vendor.Id, new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
               new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), true, "Order from Thai", _currency.Id, "0011", DateTime.Now, "To KH", 10, 10, 100, TransactionStatus.Publish,DateTime.Now);
            _purchaseOrderItem1 = CreatePurchaseOrderItem(_purchaseOrder1.Id, _item.Id, _tax.Id, 10, "Description", 100, 1000, 10, 1000);
        }
        
        private CreateItemReceiptInput CreateHelper()
        {

            var ItemReceiptExemptInput = new CreateItemReceiptInput()
            {
                CurrencyId = _currency.Id,
                Memo = "Product from UAS",
                Date = DateTime.Now,
                ClearanceAccountId = _chartOfAccount.Id,
                SameAsShippingAddress = false,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#180E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                ReceiptNo = "P000001",
                ReceiveFrom = ReceiveFromStatus.PO,
                Reference = "P000001",
                VendorId = _vendor.Id,               
                Total = 90,                
                Status = TransactionStatus.Publish,
                Items = new List<CreateOrUpdateItemReceiptItemInput>()
                {
                    new CreateOrUpdateItemReceiptItemInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",BillItemId = null,ItemId =_item.Id,
                        OrderItemId = _purchaseOrderItem1.Id,Qty = 0,Total = 100,UnitCost = 10
                    },
                     new CreateOrUpdateItemReceiptItemInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",BillItemId = null,ItemId = _item1.Id,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,Total=100,UnitCost = 100,
                    },
                }
                
            };
            return ItemReceiptExemptInput;
        }


        private void CheckItemReceipt(CreateItemReceiptInput JournalExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.ReceiptNo);
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
                var InputJournalItems = JournalExemptInput.Items.OrderBy(u => u.InventoryAccountId).ToList();
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
                var ItemReceipt = context.ItemReceipts.FirstOrDefault(p => p.Total == JournalExemptInput.Total);
                var jItemId = context.JournalItems.FirstOrDefault(u => u.Debit == 0);
               // if (Id != null) JournalEntity.Id.ShouldBe(Id.Value);
           
                ItemReceipt.SameAsShippingAddress.ShouldBe(JournalExemptInput.SameAsShippingAddress);
                ItemReceipt.BillingAddress.ShouldBe(JournalExemptInput.BillingAddress);
                ItemReceipt.ShippingAddress.ShouldBe(JournalExemptInput.SameAsShippingAddress ?
                                                      JournalExemptInput.BillingAddress :
                                                      JournalExemptInput.ShippingAddress);             
                ItemReceipt.Total.ShouldBe(JournalExemptInput.Total);
                ItemReceipt.VendorId.ShouldBe(JournalExemptInput.VendorId);

                var itemReceiptItems = context.ItemReceiptItems.Where(u => u.ItemReceiptId == ItemReceipt.Id).OrderBy(u => u.ItemId).ToList();
                var InputItemReceiptItem = JournalExemptInput.Items.OrderBy(u => u.ItemId).ToList();
                itemReceiptItems.ShouldNotBeNull();
                itemReceiptItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < itemReceiptItems.Count; i++)
                {
                    var itemReceiptItem = itemReceiptItems[i];
                    var initemReceiptItem = InputItemReceiptItem[i];
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemId, itemReceiptItem.ItemId);
                   // ShouldBeTestExtensions.ShouldBe(itemReceiptItem.BillItemId, itemReceiptItem.BillItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Description, itemReceiptItem.Description);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.DiscountRate, itemReceiptItem.DiscountRate);
                    // ShouldBeTestExtensions.ShouldBe(itemReceiptItem.InventoryJournalId, itemReceiptItem.InventoryJournalId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemReceiptId, itemReceiptItem.ItemReceiptId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.OrderItemId, itemReceiptItem.OrderItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Qty, itemReceiptItem.Qty);
                   // ShouldBeTestExtensions.ShouldBe(itemReceiptItem.TaxId, itemReceiptItem.TaxId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Total, itemReceiptItem.Total);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.UnitCost, itemReceiptItem.UnitCost);

                };

            });
        }

         
        [Fact]
        public async Task Test_CreateItemReceipt()
         {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);


        }

        [Fact]
        public async Task Test_GetDetail()
        {

            var JournalExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _ItemReceiptAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);          
            output.BillingAddress.ShouldBe(JournalExemptInput.BillingAddress);
            output.LocationId.ShouldBe(JournalExemptInput.LocationId);            
            output.Total.ShouldBe(JournalExemptInput.Total);
            output.VendorId.ShouldBe(JournalExemptInput.VendorId);           
            CheckItemReceipt(JournalExemptInput);
        }

        [Fact]
        public async Task Test_Delete()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(JournalExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var itemReceipEntity = context.ItemReceipts.FirstOrDefault(p => p.VendorId == JournalExemptInput.VendorId);               
            });
            await _ItemReceiptAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.ItemReceipts.Count().ShouldBe(0);
            });
        }

          [Fact]
        public async Task Test_Find()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _ItemReceiptAppService.Find(new GetListItemReceiptInput()
            {
                FromDate = Convert.ToDateTime("1/04/2018 12:00:00 AM"),
                ToDate = Convert.ToDateTime("30/04/2018 12:00:00 AM"),
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
               // Items = new List<Guid> { _item.Id },
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(ItemReceiptExemptInput.ReceiptNo);
                v.Date.ShouldBe(ItemReceiptExemptInput.Date);
               // v.Status.ShouldBe(ItemReceiptExemptInput.Status);
                v.VendorId.ShouldBe(ItemReceiptExemptInput.VendorId);
                v.Total.ShouldBe(ItemReceiptExemptInput.Total);

            }

        }
         [Fact]
        public async Task Test_GetList()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _ItemReceiptAppService.GetList(new GetListItemReceiptInput()
            {
                //Status = new List<JournalStatus>{JournalStatus.Publish},
                FromDate = Convert.ToDateTime("2018-04-01T09:00:14.508+07:00"),
                ToDate = Convert.ToDateTime("2018-04-30T09:00:14.508+07:00"),
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
 
               
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(ItemReceiptExemptInput.ReceiptNo);
                v.Date.ShouldBe(ItemReceiptExemptInput.Date);             
                v.VendorId.ShouldBe(ItemReceiptExemptInput.VendorId);
                v.Total.ShouldBe(ItemReceiptExemptInput.Total);

            }

        }
               

        [Fact]
        public async Task Test_UpdateStatus()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {
                
                var ItemReceipt = context.ItemReceiptItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.Items[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,            
            };
            var updatedResult = await _ItemReceiptAppService.UpdateStatusToPublish(UpdateStatusInput);

        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {
                var ItemReceipt = context.ItemReceiptItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.Items[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = ItemReceipt.Id;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateItemReceiptExemptInput = new UpdateItemReceiptInput()
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
                ReceiptNo = "P000001",
                ReceiveFrom = ReceiveFromStatus.PO,
                Reference = "P000001",
                VendorId = _vendor.Id,              
                Total = 90,
                Items = new List<CreateOrUpdateItemReceiptItemInput>()
                {
                    new CreateOrUpdateItemReceiptItemInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",BillItemId = null,ItemId =_item.Id,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,Total = 100,UnitCost = 10
                    },
                     new CreateOrUpdateItemReceiptItemInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",BillItemId = null,ItemId = _item.Id,
                        OrderItemId = _purchaseOrderItem.Id,Qty = 10,Total=100,UnitCost = 100,
                    },
                }

            };
            var updatedResult = await _ItemReceiptAppService.Update(UpdateItemReceiptExemptInput);
            updatedResult.ShouldNotBeNull();

          //  CheckItemReceipt(UpdateItemReceiptExemptInput, result.Id.Value);


        }

        [Fact]
        public async Task Test_GetItemReceipt()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _ItemReceiptAppService.GetItemReceipts(new GetItemReceiptInput()
            {
              
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,


            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.ItemReceiptNo.ShouldBe(ItemReceiptExemptInput.ReceiptNo);
                v.Date.ShouldBe(ItemReceiptExemptInput.Date);
                v.Total.ShouldBe(ItemReceiptExemptInput.Total);

            }

        }


        [Fact]
        public async Task Test_GetItemReceiptItems()
        {
            var itemreceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(itemreceiptExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _ItemReceiptAppService.GetItemReceiptItems(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);          
            CheckItemReceipt(itemreceiptExemptInput);
        }


        [Fact]
        public async Task Test_UpdateStatusToVoid()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _ItemReceiptAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.ItemReceiptItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.Items[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _ItemReceiptAppService.UpdateStatusToVoid(UpdateStatusInput);

        }

    }
}
