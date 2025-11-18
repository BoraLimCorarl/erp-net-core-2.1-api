using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.ItemReceiptTransfers;
using CorarlERP.ItemReceiptTransfers.Dto;
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

namespace CorarlERP.Tests.ItemReceiptTransfers
{
   public class ItemReceiptTransferAppService_Test : AppTestBase
    {
        private readonly IItemReceiptTransferAppService _itemReceiptTransferAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public readonly Class _class;
        public readonly Location _location;       
        private readonly Item _item;       
        private readonly ItemType _itemType;
        public ItemReceiptTransferAppService_Test()
        {
            _itemReceiptTransferAppService = Resolve<IItemReceiptTransferAppService>();
            _class = CreateClass(null, null, "Ione II", false, null);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                       displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);            
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null, purchaseTaxId: _tax.Id, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: null, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            
        }

        private CreateItemReceiptTransferInput CreateHelper()
        {
            var ItemReceiptExemptInput = new CreateItemReceiptTransferInput()
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
                Reference = "P000001",              
                Total = 90,
                
                Status = TransactionStatus.Publish,
                Items = new List<CreateOrUpdateItemReceiptItemTransferInput>()
                {
                    new CreateOrUpdateItemReceiptItemTransferInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",ItemId =_item.Id,Qty = 0,Total = 100,UnitCost = 10
                    }                    
                }

            };

            return ItemReceiptExemptInput;
        }

        private void CheckItemReceipt(CreateItemReceiptTransferInput JournalExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.ReceiptNo);
                JournalEntity.ShouldNotBeNull();
                JournalEntity.ItemReceiptId.ShouldBe(JournalEntity.ItemReceiptId);
                JournalEntity.Credit.ShouldBe(JournalExemptInput.Total);
                JournalEntity.Debit.ShouldBe(JournalExemptInput.Total);
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
              
                ItemReceipt.SameAsShippingAddress.ShouldBe(JournalExemptInput.SameAsShippingAddress);
                ItemReceipt.BillingAddress.ShouldBe(JournalExemptInput.BillingAddress);
                ItemReceipt.ShippingAddress.ShouldBe(JournalExemptInput.SameAsShippingAddress ?
                                                      JournalExemptInput.BillingAddress :
                                                      JournalExemptInput.ShippingAddress);
                ItemReceipt.Total.ShouldBe(JournalExemptInput.Total);
                var itemReceiptItems = context.ItemReceiptItems.Where(u => u.ItemReceiptId == ItemReceipt.Id).OrderBy(u => u.ItemId).ToList();
                var InputItemReceiptItem = JournalExemptInput.Items.OrderBy(u => u.ItemId).ToList();
                itemReceiptItems.ShouldNotBeNull();
                itemReceiptItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < itemReceiptItems.Count; i++)
                {
                    var itemReceiptItem = itemReceiptItems[i];
                    var initemReceiptItem = InputItemReceiptItem[i];
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemId, itemReceiptItem.ItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Description, itemReceiptItem.Description);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.DiscountRate, itemReceiptItem.DiscountRate);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.ItemReceiptId, itemReceiptItem.ItemReceiptId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.OrderItemId, itemReceiptItem.OrderItemId);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Qty, itemReceiptItem.Qty);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.Total, itemReceiptItem.Total);
                    ShouldBeTestExtensions.ShouldBe(itemReceiptItem.UnitCost, itemReceiptItem.UnitCost);

                };

            });
        }

        [Fact]
        public async Task Test_CreateItemReceipt()
        {
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _itemReceiptTransferAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);
        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _itemReceiptTransferAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckItemReceipt(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {
                var ItemReceipt = context.ItemReceiptItems.FirstOrDefault(u => u.ItemId == ItemReceiptExemptInput.Items[0].ItemId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = ItemReceipt.Id;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateItemReceiptExemptInput = new UpdateItemReceiptTransferInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Product from England",
                Date = DateTime.Now,
                ClearanceAccountId = _chartOfAccount.Id,
                SameAsShippingAddress = true,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),               
                ClassId = _class.Id,
                LocationId = _location.Id,
                ReceiptNo = "P000001",               
                Reference = "P000001",             
                Total = 90,
                Items = new List<CreateOrUpdateItemReceiptItemTransferInput>()
                {
                    new CreateOrUpdateItemReceiptItemTransferInput(){InventoryAccountId=_chartOfAccount.Id,DiscountRate=10,
                        Description = "Order From Japen",ItemId =_item.Id, Qty = 10,Total = 100,UnitCost = 10
                    },
                     new CreateOrUpdateItemReceiptItemTransferInput(){InventoryAccountId = _chartOfAccount.Id,DiscountRate = 100,
                        Description ="Order From Thai",ItemId = _item.Id, Qty = 10,Total=100,UnitCost = 100,
                    },
                }

            };
            var updatedResult = await _itemReceiptTransferAppService.Update(UpdateItemReceiptExemptInput);
            updatedResult.ShouldNotBeNull(); 

        }

        [Fact]
        public async Task Test_GetDetail()
        {

            var JournalExemptInput = CreateHelper();
            var result = await _itemReceiptTransferAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _itemReceiptTransferAppService.GetDetail(new EntityDto<Guid>()
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
