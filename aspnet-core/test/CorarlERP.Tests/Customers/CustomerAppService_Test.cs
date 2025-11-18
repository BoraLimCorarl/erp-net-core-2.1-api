using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.Customers
{
   public class CustomerAppService_Test : AppTestBase
    {
        private readonly ICustomerAppService _customerAppService;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;

        public CustomerAppService_Test()
        {
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _customerAppService = Resolve<ICustomerAppService>();
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
        }


        private CreateCustomerInput CreateHelper()
        {
            var VendorExemptInput = new CreateCustomerInput()
            {
                CustomerCode = "001",
                AccountId = _chartOfAccount.Id,
                CustomerName = "IOne Co.,Ltd",
                Email = "ione@gmail.com",
                PhoneNumber = "+85599898887",
                Website = "www.iOne.com",
                Remark = "Testing",
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                SameAsShippngAddress = true,
                ShippingAddress = new CAddress("", "", "", "", ""),
                ContactPersons = new List<CreateOrUpdateCustomerContactPersonInput>(){
                     new CreateOrUpdateCustomerContactPersonInput() {FirstName="Dara",LastName="Chan",Title="Mr.",IsPrimary=false,PhoneNumber="+8559876543", DisplayNameAs="Mr. Chan Dara",Email="thoun.vandy@gmail.com"},
                     new CreateOrUpdateCustomerContactPersonInput() {FirstName="Vandy",LastName="Thoun",Title="Mr.",IsPrimary=true,PhoneNumber="+8559876543", DisplayNameAs="Mr. Thoun Vandy",Email="thoun.vandy@gmail.com"},

                }


            };
            return VendorExemptInput;

        }

        private void CheckCustomer(CreateCustomerInput CustomerExemptInput, Guid? Id = null)
        {
            //Assert
            UsingDbContext(context =>
            {
                var CustomerEntity = context.Customers.FirstOrDefault(p => p.CustomerName == CustomerExemptInput.CustomerName);
                CustomerEntity.ShouldNotBeNull();

                if (Id != null) CustomerEntity.Id.ShouldBe(Id.Value);

                CustomerEntity.CustomerName.ShouldBe(CustomerExemptInput.CustomerName);
                CustomerEntity.CustomerCode.ShouldBe(CustomerExemptInput.CustomerCode);
                CustomerEntity.Email.ShouldBe(CustomerExemptInput.Email);
                CustomerEntity.PhoneNumber.ShouldBe(CustomerExemptInput.PhoneNumber);
                CustomerEntity.Remark.ShouldBe(CustomerExemptInput.Remark);
                CustomerEntity.Website.ShouldBe(CustomerExemptInput.Website);

                CustomerEntity.BillingAddress.ShouldBe(CustomerExemptInput.BillingAddress);
                CustomerEntity.ShippingAddress.ShouldBe(CustomerEntity.SameAsShippngAddress ?
                                                      CustomerExemptInput.BillingAddress :
                                                      CustomerExemptInput.ShippingAddress);

                CustomerEntity.SameAsShippngAddress.ShouldBe(CustomerExemptInput.SameAsShippngAddress);
                CustomerEntity.IsActive.ShouldBe(true);

                var @contactPersons = context.CustomerContactPersons.Where(u => u.CustomerId == CustomerEntity.Id).OrderBy(u => u.FirstName).ToList();
                var inputContactPersons = CustomerExemptInput.ContactPersons.OrderBy(u => u.FirstName).ToList();

                contactPersons.ShouldNotBeNull();
                contactPersons.Count().ShouldBe(inputContactPersons.Count());

                for (var i = 0; i < contactPersons.Count(); i++)
                {
                    var @contactPerson = @contactPersons[i];
                    var inputContactPerson = inputContactPersons[i];

                    @contactPerson.Title.ShouldBe(inputContactPerson.Title);
                    @contactPerson.FirstName.ShouldBe(inputContactPerson.FirstName);
                    @contactPerson.LastName.ShouldBe(inputContactPerson.LastName);
                    @contactPerson.PhoneNumber.ShouldBe(inputContactPerson.PhoneNumber);
                    @contactPerson.DisplayNameAs.ShouldBe(inputContactPerson.DisplayNameAs);
                }


            });

        }

        [Fact]
        public async Task Test_CreateSingleVendor()
        {
            var CustomerExemptInput = CreateHelper();

            var result = await _customerAppService.Create(CustomerExemptInput);
            result.Id.ShouldNotBeNull();

            CheckCustomer(CustomerExemptInput);
        }

        [Fact]
        public async Task Test_DelectCustomer()
        {
            var VendorExemptInput = CreateHelper();
            var result = await _customerAppService.Create(VendorExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var VendorEntity = context.Customers.FirstOrDefault(p => p.CustomerName == VendorExemptInput.CustomerName);
            });
            await _customerAppService.Delete(new EntityDto<Guid> { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.Vendors.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListCustomer()
        {
            var VendorExemptInput = CreateHelper();
            var result = await _customerAppService.Create(VendorExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _customerAppService.GetList(new GetCustomerListInput()
            {
                Filter = ""
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.CustomerCode.ShouldBe(VendorExemptInput.CustomerCode);
                v.CustomerName.ShouldBe(VendorExemptInput.CustomerName);
                v.Email.ShouldBe(VendorExemptInput.Email);
                v.PhoneNumber.ShouldBe(VendorExemptInput.PhoneNumber);

            }
        }

        [Fact]
        public async Task Test_GetDetailCustomer()
        {

            var JournalExemptInput = CreateHelper();
            var result = await _customerAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _customerAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);

        }

        [Fact]
        public async Task Test_UpdateVendor()
        {
            Guid vendorId = Guid.Empty;
            Guid ContactPersonId = Guid.Empty;

            //you create a vendor named: VendorName
            var CreateCustomerExemptInput = CreateHelper();

            var result = await _customerAppService.Create(CreateCustomerExemptInput);
            result.Id.ShouldNotBeNull();

            CheckCustomer(CreateCustomerExemptInput);

            UsingDbContext(context =>
            {
                var contactPerson = context.CustomerContactPersons.FirstOrDefault(u => u.FirstName == CreateCustomerExemptInput.ContactPersons[0].FirstName);
                contactPerson.ShouldNotBeNull();
                ContactPersonId = contactPerson.Id;
            });

            ContactPersonId.ShouldNotBe(Guid.Empty);

            //now you update the vendor whose name was: VendorName to UpdateVendorName
            var UpdateCustomerExemptInput = new UpdateCustomerInput()
            {
                Id = result.Id.Value,
                CustomerCode = "002",
                CustomerName = "IOne",
                Email = "ione.@gmail.com",
                PhoneNumber = "+85599898887",
                Website = "www.ione.com",
                Remark = "RemarkUpdate",
                AccountId = _chartOfAccount.Id,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                SameAsShippngAddress = true,
                ShippingAddress = new CAddress("", "", "", "", ""),
                ContactPersons = new List<CreateOrUpdateCustomerContactPersonInput>(){
                     new CreateOrUpdateCustomerContactPersonInput() {Id =ContactPersonId, FirstName="Van",LastName="dara",Title="Mr",IsPrimary=false,PhoneNumber="+8559876543",DisplayNameAs="Van dara"},
                     new CreateOrUpdateCustomerContactPersonInput() {FirstName="Chan",LastName="Sombath",Title="Mr",IsPrimary=true,PhoneNumber="09876543",DisplayNameAs="Thoun Vandy"},

                }
            };

            var updatedResult = await _customerAppService.Update(UpdateCustomerExemptInput);
            updatedResult.ShouldNotBeNull();

            CheckCustomer(UpdateCustomerExemptInput, result.Id.Value);



        }

    }

}
