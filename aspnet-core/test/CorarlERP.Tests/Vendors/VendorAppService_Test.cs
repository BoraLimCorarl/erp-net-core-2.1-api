using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Taxes;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.Vendors
{
    public class CustomerAppService_Test : AppTestBase
    {
        private readonly IVendorAppService _vendorAppService;
        public readonly ChartOfAccount _chartOfAccount;       
        public readonly Tax _tax;
        public readonly AccountType _accountType;
       

        public CustomerAppService_Test()
        {
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _vendorAppService = Resolve<IVendorAppService>();
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
        }

       private CreateVendorInput CreateHelper()
        {
            var VendorExemptInput = new CreateVendorInput()
            {
                VendorCode = "001",
                AccountId = _chartOfAccount.Id,
                VendorName = "IOne Co.,Ltd",
                Email = "ione@gmail.com",
                PhoneNumber = "+85599898887",
                Website = "www.iOne.com",
                Remark = "Testing",
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                SameAsShippngAddress = true,
                ShippingAddress = new CAddress("", "", "", "", ""),
                ContactPersons = new List<CreateOrUpdateContactPersonInput>(){
                     new CreateOrUpdateContactPersonInput() {FirstName="Dara",LastName="Chan",Title="Mr.",IsPrimary=false,PhoneNumber="+8559876543", DisplayNameAs="Mr. Chan Dara",Email="thoun.vandy@gmail.com"},
                     new CreateOrUpdateContactPersonInput() {FirstName="Vandy",LastName="Thoun",Title="Mr.",IsPrimary=true,PhoneNumber="+8559876543", DisplayNameAs="Mr. Thoun Vandy",Email="thoun.vandy@gmail.com"},

                }


            };
            return VendorExemptInput;

        }

        private void CheckVendor(CreateVendorInput VendorExemptInput, Guid? Id = null)
        {
            //Assert
            UsingDbContext(context =>
            {
                var VendorEntity = context.Vendors.FirstOrDefault(p => p.VendorName == VendorExemptInput.VendorName);
                VendorEntity.ShouldNotBeNull();

                if (Id != null) VendorEntity.Id.ShouldBe(Id.Value);

                VendorEntity.VendorName.ShouldBe(VendorExemptInput.VendorName);
                VendorEntity.VendorCode.ShouldBe(VendorExemptInput.VendorCode);
                VendorEntity.Email.ShouldBe(VendorExemptInput.Email);
                VendorEntity.PhoneNumber.ShouldBe(VendorExemptInput.PhoneNumber);
                VendorEntity.Remark.ShouldBe(VendorExemptInput.Remark);
                VendorEntity.Websit.ShouldBe(VendorExemptInput.Website);

                VendorEntity.BillingAddress.ShouldBe(VendorExemptInput.BillingAddress);
                VendorEntity.ShippingAddress.ShouldBe(VendorEntity.SameAsShippngAddress ?
                                                      VendorExemptInput.BillingAddress :
                                                      VendorExemptInput.ShippingAddress);

                VendorEntity.SameAsShippngAddress.ShouldBe(VendorExemptInput.SameAsShippngAddress);
                VendorEntity.IsActive.ShouldBe(true);

                var @contactPersons = context.ContactPersons.Where(u => u.VenderId == VendorEntity.Id).OrderBy(u => u.FirstName).ToList();
                var inputContactPersons = VendorExemptInput.ContactPersons.OrderBy(u => u.FirstName).ToList();

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
            var VendorExemptInput = CreateHelper();
            
            var result = await _vendorAppService.Create(VendorExemptInput);
            result.Id.ShouldNotBeNull();

            CheckVendor(VendorExemptInput);
        }

        [Fact]
        public async Task Test_DelectVendor()
        {
            var VendorExemptInput = CreateHelper();           
            var result = await _vendorAppService.Create(VendorExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var VendorEntity = context.Vendors.FirstOrDefault(p => p.VendorName == VendorExemptInput.VendorName);
            });
            await _vendorAppService.Delete(new EntityDto<Guid> { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.Vendors.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListVendor()
        {
            var VendorExemptInput = CreateHelper();
            var result = await _vendorAppService.Create(VendorExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _vendorAppService.GetList(new GetVendorListInput()
            {
                Filter = ""
            });

           foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.VendorCode.ShouldBe(VendorExemptInput.VendorCode);
                v.VendorName.ShouldBe(VendorExemptInput.VendorName);
                v.VendorCode.ShouldBe(VendorExemptInput.VendorCode);
                v.Email.ShouldBe(VendorExemptInput.Email);
                v.PhoneNumber.ShouldBe(VendorExemptInput.PhoneNumber);

            }
        }

        [Fact]
        public async Task Test_FindVendor()
        {
            var VendorExemptInput = CreateHelper();           
            var result = await _vendorAppService.Create(VendorExemptInput);
            result.Id.ShouldNotBeNull();
            var output = await _vendorAppService.Find(new GetVendorListInput()
            {
                Filter = ""
            });
            foreach (var v in output.Items)
            {
                v.VendorCode.ShouldBe(VendorExemptInput.VendorCode);
                v.VendorName.ShouldBe(VendorExemptInput.VendorName);
                v.VendorCode.ShouldBe(VendorExemptInput.VendorCode);
                v.Email.ShouldBe(VendorExemptInput.Email);
                v.PhoneNumber.ShouldBe(VendorExemptInput.PhoneNumber);

            }
        }
        [Fact]
        public async Task Test_UpdateVendor()
        {
            Guid vendorId = Guid.Empty;
            Guid ContactPersonId = Guid.Empty;

            //you create a vendor named: VendorName
            var CreateVendorExemptInput = CreateHelper();

            var result = await _vendorAppService.Create(CreateVendorExemptInput);
            result.Id.ShouldNotBeNull();
            
            CheckVendor(CreateVendorExemptInput);
            
            UsingDbContext(context =>
            {
                var contactPerson = context.ContactPersons.FirstOrDefault(u => u.FirstName == CreateVendorExemptInput.ContactPersons[0].FirstName);
                contactPerson.ShouldNotBeNull();
                ContactPersonId = contactPerson.Id;
            });

            ContactPersonId.ShouldNotBe(Guid.Empty);

            //now you update the vendor whose name was: VendorName to UpdateVendorName
            var UpdateVendorExemptInput = new UpdateVendorInput() {
                Id =result.Id.Value,
                VendorCode = "002",
                VendorName = "IOne",
                Email = "ione.@gmail.com",
                PhoneNumber = "+85599898887",
                Website = "www.ione.com",
                Remark = "RemarkUpdate",
                AccountId = _chartOfAccount.Id,
                BillingAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
                SameAsShippngAddress = true,
                ShippingAddress = new CAddress("", "", "", "", ""),
                ContactPersons = new List<CreateOrUpdateContactPersonInput>(){
                     new CreateOrUpdateContactPersonInput() {Id =ContactPersonId, FirstName="Van",LastName="dara",Title="Mr",IsPrimary=false,PhoneNumber="+8559876543",DisplayNameAs="Van dara"},
                     new CreateOrUpdateContactPersonInput() {FirstName="Chan",LastName="Sombath",Title="Mr",IsPrimary=true,PhoneNumber="09876543",DisplayNameAs="Thoun Vandy"},

                }
            };

            var updatedResult = await _vendorAppService.Update(UpdateVendorExemptInput);
            updatedResult.ShouldNotBeNull();

            CheckVendor(UpdateVendorExemptInput, result.Id.Value);

           

        }

        [Fact]
        public async Task Test_GetDetailVendor()
        {

            var JournalExemptInput = CreateHelper();
            var result = await _vendorAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _vendorAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
           
        }


    }

}
