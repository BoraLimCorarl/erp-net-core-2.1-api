using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.CustomerCredits;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Customers;
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

namespace CorarlERP.Tests.CustomerCredits
{
    public class CustomerCreditAppService_Test : AppTestBase
    {

        public readonly Class _class;
        private readonly ICustomerCreditAppService _customerCreditAppService;

        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        private readonly Item _item;
        public readonly Location _location;
        public readonly AccountType _accountType;

        private readonly ItemType _itemType;
        public readonly Customer _customer;

        public CustomerCreditAppService_Test()
        {
            _customerCreditAppService = Resolve<ICustomerCreditAppService>();
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _class = CreateClass(null, null, "Default", false, null);
            _accountType = CreateAccountType("Bank", "", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                       displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _tax = CreateTax("Tax Exempt", 0);
            _chartOfAccount = CreateChartOfAccount("111524", "Cash", "cash description", _accountType.Id, null, _tax.Id);
            _customer = CreateCustomer("Nika Phone Shop", "001", "nika@gmail.com", "070 955 102", true, "www.nika.com", "", new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), _chartOfAccount.Id,1);

            _item = CreateItem(ItemName: "Iphone X", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null,
                   purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", 
                   barcode: "", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);

        }
        public CreateCustomerCreditInput CreateHelper()
        {
            var result = new CreateCustomerCreditInput()
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
                CustomerCreditNo = "P000001",
                Reference = "P000001",
                CustomerId = _customer.Id,
                SubTotal = 100,
                Tax = 10,
                Total = 90,
                Status = TransactionStatus.Publish,
                CustomerCreditDetail = new List<CustomerCreditDetailInput>()
                {
                    new CustomerCreditDetailInput(){
                        AccountId =_chartOfAccount.Id,
                        DiscountRate =10,
                        Description = "Order From Japen",
                        Qty = 10,
                        TaxId = _tax.Id,
                        Total = 100,
                        UnitCost = 10
                    },
                     new CustomerCreditDetailInput(){
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
        public async Task Test_CreateCustomerCredit()
        {
            var Input = CreateHelper();
            var result = await _customerCreditAppService.Create(Input);
            result.ShouldNotBeNull();
            CheckCustomerCredit(Input, result.Id);
        }

        private void CheckCustomerCredit(CreateCustomerCreditInput JournalExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.CustomerCreditNo);
                JournalEntity.ShouldNotBeNull();
                JournalEntity.Credit.ShouldBe(JournalExemptInput.Total);
                JournalEntity.Debit.ShouldBe(0);
                JournalEntity.CurrencyId.ShouldBe(JournalExemptInput.CurrencyId);
                JournalEntity.Date.ShouldBe(JournalExemptInput.CreditDate);
                JournalEntity.Memo.ShouldBe(JournalExemptInput.Memo);
                JournalEntity.IsActive.ShouldBe(true);
            });
        }


        [Fact]
        public async Task Test_FindCustomerCredit()
        {
            var input = CreateHelper();
            var result = await _customerCreditAppService.Create(input);
            result.ShouldNotBeNull();
            CheckCustomerCredit(input, result.Id);
            result.Id.ShouldNotBeNull();
            var filter = new ListCustomerCreditInput()
            {
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,
            };
            var output = await _customerCreditAppService.Find(filter);

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(input.CustomerCreditNo);
                v.Date.ShouldBe(input.CreditDate);
                v.Status.ShouldBe(input.Status);
                //v.CustomerId.ShouldBe(input.CustomerId);
                v.Total.ShouldBe(input.Total);
                v.DueDate.ShouldBe(input.DueDate);
            }
        }

        [Fact]
        public async Task Test_Update()
        {
            Guid creditId = Guid.Empty;
            var createInput = CreateHelper();
            var result = await _customerCreditAppService.Create(createInput);
            result.ShouldNotBeNull();
            CheckCustomerCredit(createInput, result.Id);

            UsingDbContext(context =>
            {
                var customerCredit = context.CustomerCredit.FirstOrDefault(u => u.Id == result.Id);
                customerCredit.ShouldNotBeNull();
                creditId = customerCredit.Id;
            });
            creditId.ShouldNotBe(Guid.Empty);

            var updateInput = new UpdateCustomerCreditInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                Memo = "Update my Testing",
                CreditDate = DateTime.Now,
                AccountId = _chartOfAccount.Id,
                SameAsShippingAddress = true,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                // ShippingAddress = new Address("Khan Chamkarmourn II", "KH", "1204", "Phnom Penh", "#185E0 Monivong Blvd"),
                ClassId = _class.Id,
                LocationId = _location.Id,
                CustomerCreditNo = "CUS-001",
                Reference = "CUS-001",
                CustomerId = _customer.Id,
                SubTotal = 100,
                Tax = 10,
                Total = 90,
                Status = TransactionStatus.Publish,
                CustomerCreditDetail = new List<CustomerCreditDetailInput>()
                {
                    new CustomerCreditDetailInput(){
                        AccountId = _chartOfAccount.Id,
                         DiscountRate = 0,
                         Description ="update item",
                         Qty = 20,
                         TaxId = _tax.Id,
                         Total = 3000,
                         UnitCost = 150,
                         ItemId = _item.Id
                    },
                     new CustomerCreditDetailInput(){
                         AccountId = _chartOfAccount.Id,
                         DiscountRate = 0,
                         Description ="update item",
                         Qty = 20,
                         TaxId = _tax.Id,
                         Total = 3000,
                         UnitCost = 150,
                         ItemId = _item.Id
                    },
                }

            };
            var updatedResult = await _customerCreditAppService.Update(updateInput);
            updatedResult.ShouldNotBeNull();

            CheckCustomerCredit(updateInput, result.Id);
        }

        [Fact]
        public async Task Test_GetDetailCustomerCredit()
        {
            var input = CreateHelper();
            var result = await _customerCreditAppService.Create(input);
            result.ShouldNotBeNull();
            CheckCustomerCredit(input, result.Id);
            result.Id.ShouldNotBeNull();
            var filter = new EntityDto<Guid>()
            {
                Id = result.Id.Value,
            };
            UsingDbContext(context =>
            {
                var customerCredit = context.ChartOfAccounts.FirstOrDefault(u => u.Id == _chartOfAccount.Id);
                customerCredit.ShouldNotBeNull();
                
            });
            //var output = await _customerCreditAppService.GetDetail(filter);
            //output.ShouldNotBeNull();
            
        }

    }
}
