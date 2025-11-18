using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Authorization.Users;
using CorarlERP.MultiTenancy;
using CorarlERP.MultiTenancy.Dto;
using CorarlERP.Notifications;
using Shouldly;
using CorarlERP.Addresses;
using CorarlERP.Currencies;
using CorarlERP.Formats;
using Xunit;
using static CorarlERP.Authorization.Roles.StaticRoleNames;
using CorarlERP.AccountCycles;
using System;
using Abp.Timing;
using CorarlERP.Classes;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Locations;
using CorarlERP.ItemTypes;
using CorarlERP.Taxes;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.MultiTenancy
{
    public class TenantAppService_Tests : AppTestBase
    {

        //private readonly ICompanyProfileAppService _companyAppService;
        private readonly ITenantAppService _tenantAppService;
        private readonly Format _format;
        private readonly Currency _currency;
        private readonly AccountCycle _accouncycle;
        private readonly Class _class;
        private readonly ChartOfAccount _transitAccount;
        private readonly ChartOfAccount _billPaymentAccount;
        private readonly Location _location;
        private readonly Tax _taxExempt;
        public readonly AccountType _accountType;
        public TenantAppService_Tests()
        {
            LoginAsHostAdmin();
           // LoginAsTenant();
            _tenantAppService = Resolve<ITenantAppService>();
          //  _companyAppService = Resolve<ICompanyProfileAppService>();
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _format = CreateFormat(null, "dd/MM/yyyyy", "Date", "dd/MM/yyyy");
            _class = CreateClass(null, null, "IOneII", false, null);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _taxExempt = CreateTax("Tax Exempt", 0);
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _billPaymentAccount = CreateChartOfAccount("0001", "Debit", "accountdescrition", _accountType.Id, null, _taxExempt.Id);
            _transitAccount = CreateChartOfAccount("0002", "Credit", "accountdescrition", _accountType.Id, null, _taxExempt.Id);
            int roundingDigit = 2;
            _accouncycle = CreateAccountCycle(null, null,startDate:Convert.ToDateTime("2018-01-25T09:00:14.508+07:00"), endDate: Convert.ToDateTime("2018-02-25T09:00:14.508+07:00"), roundingDigit: roundingDigit, roundingDigitUnitCost: roundingDigit);
        }
       
        [MultiTenantFact]
        public async Task GetTenants_Test()
        {
            //Act
            var output = await _tenantAppService.GetTenants(new GetTenantsInput());

            //Assert
            output.TotalCount.ShouldBe(1);
            output.Items.Count.ShouldBe(1);
            output.Items[0].TenancyName.ShouldBe(Tenant.DefaultTenantName);
        }

        [MultiTenantFact]
        public async Task Create_Update_And_Delete_Tenant_Test()
        {
            //CREATE --------------------------------

            //Act
            await _tenantAppService.CreateTenant(
                new CreateTenantInput
                {
                    TenancyName = "testTenant",
                    Name = "Tenant for test purpose",
                    AdminEmailAddress = "admin@testtenant.com",
                    AdminPassword = "123qwe",
                    IsActive = true
                });

            //Assert
            var tenant = await GetTenantOrNullAsync("testTenant");
            tenant.ShouldNotBe(null);
            tenant.Name.ShouldBe("Tenant for test purpose");
            tenant.IsActive.ShouldBe(true);

            await UsingDbContextAsync(tenant.Id, async context =>
            {
                //Check static roles
                var staticRoleNames = Resolve<IRoleManagementConfig>().StaticRoles.Where(r => r.Side == MultiTenancySides.Tenant).Select(role => role.RoleName).ToList();
                foreach (var staticRoleName in staticRoleNames)
                {
                    (await context.Roles.CountAsync(r => r.TenantId == tenant.Id && r.Name == staticRoleName)).ShouldBe(1);
                }

                //Check default admin user
                var adminUser = await context.Users.FirstOrDefaultAsync(u => u.TenantId == tenant.Id && u.UserName == User.AdminUserName);
                adminUser.ShouldNotBeNull();

                //Check notification registration
                (await context.NotificationSubscriptions.FirstOrDefaultAsync(ns => ns.UserId == adminUser.Id && ns.NotificationName == AppNotificationNames.NewUserRegistered)).ShouldNotBeNull();
            });

            //GET FOR EDIT -----------------------------

            //Act
            var editDto = await _tenantAppService.GetTenantForEdit(new EntityDto(tenant.Id));

            //Assert
            editDto.TenancyName.ShouldBe("testTenant");
            editDto.Name.ShouldBe("Tenant for test purpose");
            editDto.IsActive.ShouldBe(true);

            // UPDATE ----------------------------------

            editDto.Name = "iOne";
            editDto.IsActive = false;


            await _tenantAppService.UpdateTenant(editDto);

            //Assert
            tenant = await GetTenantAsync("testTenant");
            tenant.Name.ShouldBe("iOne");
            tenant.IsActive.ShouldBe(false);
            var feditDto = await _tenantAppService.GetTenantForEdit(new EntityDto(tenant.Id));
            // DELETE ----------------------------------

            //Act
            await _tenantAppService.DeleteTenant(new EntityDto((await GetTenantAsync("testTenant")).Id));

            //Assert
            (await GetTenantOrNullAsync("testTenant")).IsDeleted.ShouldBe(true);
        }
#if command
            private UpdateTenantInput CreateHelper()
        {

            var PurchaseOrderExemptInput = new UpdateTenantInput()
            {
           
            LegalName = "iOne.Coltd",
            LegalAddress = new Address("Khan ChamkarmournII", "KH", "13104", "Phnom Penh", "#185E0 Monivong Blvd"),
            CompanyAddress = new Address("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
            CurrencyId = _currency.Id,
            FormatDateId = _format.Id,
            FormatNumberId = _format.Id,
            BusinessId = "P0001",
            AccountCycleId = null,         
            SameAsCompanyAddress = false,
            PhoneNumber = "098655876876",
            Website = "www.iOne.com.kh",
        };
            return PurchaseOrderExemptInput;

        }
#endif

        int Tid = 0;

    //     [Fact]
    //    public async Task Test_UpdateTenatAddGetDetail()
    //{

    //        //login as host
    //        var createTenant = new CreateTenantInput
    //        {

    //            TenancyName = "testTenant",
    //            Name = "Tenant for test purpose",
    //            AdminEmailAddress = "admin@testtenant.com",
    //            AdminPassword = "123qwe",
    //            IsActive = true,
    //            EditionId = 1

    //        };

    //        await _tenantAppService.CreateTenant(createTenant);
            

    //        var result = _tenantAppService.GetTenantForEdit(new EntityDto());
            
    //        //Assert
    //        UsingDbContext(context =>
    //        {
    //            var ProfileCompany = context.Tenants.FirstOrDefault(u=>u.TenancyName == createTenant.TenancyName);
    //            ProfileCompany.ShouldNotBeNull();
    //            Tid = ProfileCompany.Id;
    //        });

    //        //login as company that we created above
    //        LoginAsTenant(createTenant.TenancyName, User.AdminUserName);

    //        var PurchaseOrderExemptInput = new UpdateTenantInput()
    //        {
    //            Id = Tid,
    //            LegalName = "iOne.Coltd",
    //            LegalAddress = new CAddress("Khan ChamkarmournII", "KH", "13104", "Phnom Penh", "#185E0 Monivong Blvd"),
    //            CompanyAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
    //            CurrencyId = _currency.Id,
    //            FormatDateId = _format.Id,
    //            FormatNumberId = _format.Id,
    //            BusinessId = "P0001",
    //            AccountCycleId = null,
    //            SameAsCompanyAddress = false,
    //            PhoneNumber = "098655876876",
    //            Website = "www.iOne.com.kh",
    //            Name = "Profile",
    //            AccountCycle = AccountCycle.Create(null, null, Clock.Now.AddDays(10), Clock.Now.AddDays(-10)),
    //            Email = "iOne.@gmail.com",
    //            BillPaymentAccountId = _billPaymentAccount.Id,
    //            TransitAccountId= _transitAccount.Id,
    //            LocationId= _location.Id,
    //            ClassId= _class.Id,
                

    //        };
    //        var updatedResult = await _companyAppService.Update(PurchaseOrderExemptInput);
    //        UsingDbContext(context =>
    //        {
    //            var ProfileCompany = context.Tenants.Include(u => u.AccountCycle)
    //                                                .FirstOrDefault(u => u.Id == 2);
    //            ProfileCompany.ShouldNotBeNull();

    //        });

    //        var resultId = updatedResult;
    //        var input = new EntityDto<int>();
    //        input.Id = resultId.Id.Value;
    //        var resultFirstSave = await _companyAppService.GetDetail(input);
    //        var secondInput = new UpdateTenantInput()
    //        {
    //            Id = Tid,
    //            LegalName = "iOne.Coltd",
    //            LegalAddress = new CAddress("Khan ChamkarmournIII", "KH", "13104", "Phnom Penh", "#185E0 Monivong Blvd"),
    //            CompanyAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
    //            CurrencyId = _currency.Id,
    //            FormatDateId = _format.Id,
    //            FormatNumberId = _format.Id,
    //            BusinessId = "P00011",
    //            AccountCycleId = resultFirstSave.AccountCycleId.Value,
    //            AccountCycle = resultFirstSave.AccountCycle,
    //            SameAsCompanyAddress = false,
    //            PhoneNumber = "098655876876",
    //            Website = "www.iOne.com.kh",
    //            Name = "Profile11",
    //            Email = "iOne.@gmail.com.kh",
    //            BillPaymentAccountId = _billPaymentAccount.Id,
    //            TransitAccountId = _transitAccount.Id,
    //            LocationId = _location.Id,
    //            ClassId = _class.Id,
    //        };

    //        var updatedResult1 = await _companyAppService.Update(secondInput);
    //        var output = await _companyAppService.GetDetail(new EntityDto<int>()
    //        {

    //            Id = Tid,

    //        });
    //    }

    //    [Fact]
    //    public async Task Test_GetTenantDetail()
    //    {
    //        var createTenant = new CreateTenantInput
    //        {

    //            TenancyName = "testTenant",
    //            Name = "Tenant for test purpose",
    //            AdminEmailAddress = "admin@testtenant.com",
    //            AdminPassword = "123qwe",
    //            IsActive = true,
    //            EditionId = 1,
                

    //        };

    //        await _tenantAppService.CreateTenant(createTenant);

    //        var PurchaseOrderExemptInput = new UpdateTenantInput()
    //        {
    //            Id = Tid,
    //            LegalName = "iOne.Coltd",
    //            LegalAddress = new CAddress("Khan ChamkarmournII", "KH", "13104", "Phnom Penh", "#185E0 Monivong Blvd"),
    //            CompanyAddress = new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"),
    //            CurrencyId = _currency.Id,
    //            FormatDateId = _format.Id,
    //            FormatNumberId = _format.Id,
    //            BusinessId = "P0001",
    //            AccountCycleId = null,
    //            SameAsCompanyAddress = false,
    //            PhoneNumber = "098655876876",
    //            Website = "www.iOne.com.kh",
    //            Name = "Profile",
    //            AccountCycle = AccountCycle.Create(null, null, Clock.Now.AddDays(10), Clock.Now.AddDays(-10)),
    //            Email = "iOne.@gmail.com",
    //            BillPaymentAccountId = _billPaymentAccount.Id,
    //            TransitAccountId = _transitAccount.Id,
    //            LocationId = _location.Id,
    //            ClassId = _class.Id,


    //        };

    //        LoginAsTenant(createTenant.TenancyName, User.AdminUserName);

    //        //var updatedResult = await _companyAppService.Update(PurchaseOrderExemptInput);
    //        //UsingDbContext(context =>
    //        //{
    //        //    var ProfileCompany = context.Tenants.FirstOrDefault(u => u.TenancyName == createTenant.TenancyName);
    //        //    ProfileCompany.ShouldNotBeNull();
    //        //    ProfileCompany.UpdateTenant("", "", "", "", null, null, false, null, null, 1, 1, "", "", null, ProfileCompany.BillPaymentAccount?.Id, null, null);

    //        //    ProfileCompany.BillPaymentAccount.ShouldNotBeNull();
    //        //    ProfileCompany.BillPaymentAccount?.Id.ShouldNotBeNull();
    //        //    Tid = ProfileCompany.Id;
    //        //});

            


    //        var output = await _companyAppService.GetDetail(new EntityDto<int>()
    //        {

    //            Id = Tid,

    //        });
    //    }
    }
}
