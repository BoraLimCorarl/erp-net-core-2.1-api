using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Shouldly;
using Xunit;
using CorarlERP.AccountTypes;
using CorarlERP.AccountTypes.Dto;

namespace CorarlERP.Tests.AccountTypes
{
    public class AccountTypeAppService_Tests : AppTestBase
    {
        private readonly IAccountTypeAppService _accountTypeAppService;

        public AccountTypeAppService_Tests()
        {
            _accountTypeAppService = Resolve<IAccountTypeAppService>();
            base.LoginAsHostAdmin();
        }

        [Fact]
        public async Task Test_CreateSingleAccountType()
        {
            var accountTypeInput = new CreateAccountTypeInput()
            {
                AccountTypeName = "Cash",
                Description = "for cash account"

            };

            var result = await _accountTypeAppService.Create(accountTypeInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
            result.Description.ShouldBe(accountTypeInput.Description);
            result.IsActive.ShouldBe(true);

            //Assert
            UsingDbContext(context =>
            {
                var taxEntity = context.AccountTypes.FirstOrDefault(p => p.AccountTypeName == accountTypeInput.AccountTypeName);
                taxEntity.ShouldNotBeNull();
                taxEntity.Id.ShouldBe(result.Id);
                taxEntity.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
                taxEntity.Description.ShouldBe(accountTypeInput.Description);
                taxEntity.IsActive.ShouldBe(result.IsActive);
            });
        }

        [Fact]
        public async Task Test_GetAccountTypes()
        {
            var accountTypeInput = new CreateAccountTypeInput()
            {
                AccountTypeName = "Cash",
                Description = "for cash account"

            };

            var result = await _accountTypeAppService.Create(accountTypeInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
            result.Description.ShouldBe(accountTypeInput.Description);
            result.IsActive.ShouldBe(true);

            //Act
            var output = await _accountTypeAppService.GetList(new GetAccountTypeListInput() {
                Filter = ""
            });

            //Assert
            output.Items.Count.ShouldBe(1);
            output.Items[0].Id.ShouldBe(result.Id);
            output.Items[0].AccountTypeName.ShouldBe(result.AccountTypeName);
            output.Items[0].Description.ShouldBe(result.Description);
            output.Items[0].IsActive.ShouldBe(result.IsActive);

        }

        [Fact]
        public async Task Test_UpdateAccountType()
        {
            var accountTypeInput = new CreateAccountTypeInput()
            {
                AccountTypeName = "Cash",
                Description = "for cash account"

            };

            var result = await _accountTypeAppService.Create(accountTypeInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
            result.Description.ShouldBe(accountTypeInput.Description);
            result.IsActive.ShouldBe(true);

            //Assert
            UsingDbContext(context =>
            {
                var accountTypeEntity = context.AccountTypes.FirstOrDefault(p => p.AccountTypeName == accountTypeInput.AccountTypeName);
                accountTypeEntity.ShouldNotBeNull();
                accountTypeEntity.Id.ShouldBe(result.Id);
                accountTypeEntity.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
                accountTypeEntity.Description.ShouldBe(accountTypeInput.Description);
                accountTypeEntity.IsActive.ShouldBe(result.IsActive);
            });


            var expenseAccountTypeInput = new UpateAccountTypeInput()
            {
                Id = result.Id,
                AccountTypeName = "Expense",
                Description = "for expense account"
            };

            var updatedResult = await _accountTypeAppService.Update(expenseAccountTypeInput);
            updatedResult.ShouldNotBeNull();
            updatedResult.Id.ShouldBe(expenseAccountTypeInput.Id);
            updatedResult.AccountTypeName.ShouldBe(expenseAccountTypeInput.AccountTypeName);
            updatedResult.Description.ShouldBe(expenseAccountTypeInput.Description);

            //Assert
            UsingDbContext(context =>
            {
                var updateAccountTypeEntity = context.AccountTypes.FirstOrDefault(p => p.Id == expenseAccountTypeInput.Id);
                updateAccountTypeEntity.ShouldNotBeNull();
                updateAccountTypeEntity.Id.ShouldBe(updatedResult.Id);
                updateAccountTypeEntity.AccountTypeName.ShouldBe(updatedResult.AccountTypeName);
                updateAccountTypeEntity.Description.ShouldBe(updatedResult.Description);
                updateAccountTypeEntity.IsActive.ShouldBe(true);
            });

        }



        [Fact]
        public async Task Test_DeleteAccountType()
        {
            var accountTypeInput = new CreateAccountTypeInput()
            {
                AccountTypeName = "Cash",
                Description = "for cash account"

            };

            var result = await _accountTypeAppService.Create(accountTypeInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
            result.Description.ShouldBe(accountTypeInput.Description);
            result.IsActive.ShouldBe(true);

            //Assert
            UsingDbContext(context =>
            {
                var taxEntity = context.AccountTypes.FirstOrDefault(p => p.AccountTypeName == accountTypeInput.AccountTypeName);
                taxEntity.ShouldNotBeNull();
                taxEntity.Id.ShouldBe(result.Id);
                taxEntity.AccountTypeName.ShouldBe(accountTypeInput.AccountTypeName);
                taxEntity.Description.ShouldBe(accountTypeInput.Description);
                taxEntity.IsActive.ShouldBe(result.IsActive);
            });

            await _accountTypeAppService.Delete(new EntityDto<long> { Id = result.Id });

            UsingDbContext(context =>
            {
                context.Taxes.Count().ShouldBe(0);
            });
        }

    
    }
}
