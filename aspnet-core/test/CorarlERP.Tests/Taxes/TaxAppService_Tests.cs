using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Organizations;
using CorarlERP.Authorization.Users;
using CorarlERP.Organizations;
using CorarlERP.Organizations.Dto;
using Shouldly;
using Xunit;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;

namespace CorarlERP.Tests.Taxes
{
    public class TaxAppService_Tests : AppTestBase
    {
        private readonly ITaxAppService _taxAppService;

        public TaxAppService_Tests()
        {
            _taxAppService = Resolve<ITaxAppService>();
        }

        [Fact]
        public async Task Test_CreateSingleTax()
        {
            var taxExemptInput = new CreateTaxInput()
            {
                TaxName = "Tax Exempt",
                TaxRate = 0
            };

            var result = await _taxAppService.Create(taxExemptInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.TaxName.ShouldBe(taxExemptInput.TaxName);
            result.TaxRate.ShouldBe(taxExemptInput.TaxRate);
            result.IsActive.ShouldBe(true);

            //Assert
            UsingDbContext(context =>
            {
                var taxEntity = context.Taxes.FirstOrDefault(p => p.TaxName == taxExemptInput.TaxName);
                taxEntity.ShouldNotBeNull();
                taxEntity.Id.ShouldBe(result.Id);
                taxEntity.TaxName.ShouldBe(taxExemptInput.TaxName);
                taxEntity.TaxRate.ShouldBe(taxExemptInput.TaxRate);
                taxEntity.IsActive.ShouldBe(result.IsActive);
            });
        }

        [Fact]
        public async Task Test_GetTaxes()
        {
            var taxExemptInput = new CreateTaxInput()
            {
                TaxName = "Tax Exempt",
                TaxRate = 0
            };

            var result = await _taxAppService.Create(taxExemptInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.TaxName.ShouldBe(taxExemptInput.TaxName);
            result.TaxRate.ShouldBe(taxExemptInput.TaxRate);
            result.IsActive.ShouldBe(true);

            //Act
            var output = await _taxAppService.GetList(new GetTaxListInput() {
                Filter = ""
            });

            //Assert
            output.Items.Count.ShouldBe(1);
            output.Items[0].Id.ShouldBe(result.Id);
            output.Items[0].TaxName.ShouldBe(result.TaxName);
            output.Items[0].TaxRate.ShouldBe(result.TaxRate);
            output.Items[0].IsActive.ShouldBe(result.IsActive);

        }

        [Fact]
        public async Task Test_UpdateTax()
        {
            var taxExemptInput = new CreateTaxInput()
            {
                TaxName = "Tax Exempt",
                TaxRate = 0
            };

            var result = await _taxAppService.Create(taxExemptInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.TaxName.ShouldBe(taxExemptInput.TaxName);
            result.TaxRate.ShouldBe(taxExemptInput.TaxRate);

            //Assert
            UsingDbContext(context =>
            {
                var taxEntity = context.Taxes.FirstOrDefault(p => p.TaxName == taxExemptInput.TaxName);
                taxEntity.ShouldNotBeNull();
                taxEntity.Id.ShouldBe(result.Id);
                taxEntity.TaxName.ShouldBe(taxExemptInput.TaxName);
                taxEntity.TaxRate.ShouldBe(taxExemptInput.TaxRate);
            });


            var taxPurchaseInput = new UpateTaxInput()
            {
                Id = result.Id,
                TaxName = "Purchase Tax",
                TaxRate = 0.1M
            };

            var updatedResult = await _taxAppService.Update(taxPurchaseInput);
            updatedResult.ShouldNotBeNull();
            updatedResult.Id.ShouldBe(taxPurchaseInput.Id);
            updatedResult.TaxName.ShouldBe(taxPurchaseInput.TaxName);
            updatedResult.TaxRate.ShouldBe(taxPurchaseInput.TaxRate);

            //Assert
            UsingDbContext(context =>
            {
                var updateTaxEntity = context.Taxes.FirstOrDefault(p => p.Id == taxPurchaseInput.Id);
                updateTaxEntity.ShouldNotBeNull();
                updateTaxEntity.Id.ShouldBe(updatedResult.Id);
                updateTaxEntity.TaxName.ShouldBe(updatedResult.TaxName);
                updateTaxEntity.TaxRate.ShouldBe(updatedResult.TaxRate);
            });

        }



        [Fact]
        public async Task Test_DeleteTax()
        {
            var taxExemptInput = new CreateTaxInput()
            {
                TaxName = "Tax Exempt",
                TaxRate = 0
            };

            var result = await _taxAppService.Create(taxExemptInput);
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.TaxName.ShouldBe(taxExemptInput.TaxName);
            result.TaxRate.ShouldBe(taxExemptInput.TaxRate);

            //Assert
            UsingDbContext(context =>
            {
                var taxEntity = context.Taxes.FirstOrDefault(p => p.TaxName == taxExemptInput.TaxName);
                taxEntity.ShouldNotBeNull();
                taxEntity.Id.ShouldBe(result.Id);
                taxEntity.TaxName.ShouldBe(taxExemptInput.TaxName);
                taxEntity.TaxRate.ShouldBe(taxExemptInput.TaxRate);
            });

            await _taxAppService.Delete(new EntityDto<long> { Id = result.Id });

            UsingDbContext(context =>
            {
                context.Taxes.Count().ShouldBe(0);
            });
        }

    
    }
}
