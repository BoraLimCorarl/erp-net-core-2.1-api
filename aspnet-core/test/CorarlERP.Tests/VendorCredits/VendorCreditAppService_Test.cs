using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.Taxes;
using CorarlERP.VendorCredit;
using CorarlERP.VendorCredit.Dto;
using CorarlERP.Vendors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Tests.VendorCredits
{
    public class VendorCreditAppService_Test : AppTestBase
    {

        public readonly Class _class;
        private readonly IVendorCreditAppService _vendorCreditAppService;
        
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        private readonly Item _item;
        public readonly Location _location;
        public readonly AccountType _accountType;

        private readonly ItemType _itemType;
        public readonly Vendor _vendor;

        public VendorCreditAppService_Test()
        {
            _vendorCreditAppService = Resolve<IVendorCreditAppService>();
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _class = CreateClass(null, null, "Default", false, null);
            _accountType = CreateAccountType("Bank", "", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                       displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _tax = CreateTax("Tax Exempt", 0);
            _chartOfAccount = CreateChartOfAccount("111524", "Cash", "cash description", _accountType.Id, null, _tax.Id);
            _vendor = CreateVendor("Nika Phone Shop", "001", "nika@gmail.com", "070 955 102", true, "www.nika.com", "", new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), _chartOfAccount.Id, vendorType: null);

            _item = CreateItem(ItemName: "Iphone X", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null, purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            
        }

        public CreateVendorCreditInput CreateHelper()
        {
            var result = new CreateVendorCreditInput()
            {
                CurrencyId = _currency.Id,
                Memo = "Product from UAS",
                CreditDate = DateTime.Now,
                AccountId = _chartOfAccount.Id,
                SameAsShippingAddress = false,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                ShippingAddress = new CAddress("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#180E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                VendorCreditNo = "P000001",
                Reference = "P000001",
                VendorId = _vendor.Id,
                SubTotal = 100,
                Tax = 10,
                Total = 90,
                Status = TransactionStatus.Publish,
                VendorCreditDetail = new List<VendorCreditDetailInput>()
                {
                    new VendorCreditDetailInput(){
                        AccountId =_chartOfAccount.Id,
                        DiscountRate =10,
                        Description = "Order From Japen",
                        Qty = 10,
                        TaxId = _tax.Id,
                        Total = 100,
                        UnitCost = 10
                    },
                     new VendorCreditDetailInput(){
                         AccountId = _chartOfAccount.Id,
                         DiscountRate = 100,
                         Description ="Order From Thai",
                         Qty = 10,
                         TaxId = _tax.Id,Total=100,
                         UnitCost = 100,
                         ItemId = _item.Id
                    },
                }
            };
            return result;

        }


        [Fact]
        public async Task Test_CreateVendorCredit()
        {
            var Input = CreateHelper();
            var result = await _vendorCreditAppService.Create(Input);
            result.ShouldNotBeNull();
            CheckVendorCredit(Input, result.Id);
        }

        private void CheckVendorCredit(CreateVendorCreditInput JournalExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.VendorCreditNo);
                JournalEntity.ShouldNotBeNull();
                JournalEntity.Credit.ShouldBe(JournalExemptInput.Total);
                JournalEntity.Debit.ShouldBe(0);
                JournalEntity.CurrencyId.ShouldBe(JournalExemptInput.CurrencyId);
                JournalEntity.Date.ShouldBe(JournalExemptInput.CreditDate);
                JournalEntity.Memo.ShouldBe(JournalExemptInput.Memo);
                JournalEntity.IsActive.ShouldBe(true);
            });
        }

    }
}
