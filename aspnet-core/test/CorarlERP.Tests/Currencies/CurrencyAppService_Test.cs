using CorarlERP.Currencies;
using Shouldly;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CorarlERP.Currencies.Dto;

namespace CorarlERP.Tests.Currencies
{
    public class CurrencyAppService_Test : AppTestBase
    {
        private readonly ICurrencyAppService _currencyAppService;
        private readonly IDefaultValues _defaultValues;

        public CurrencyAppService_Test()
        {
            _currencyAppService = Resolve<ICurrencyAppService>();
            _defaultValues = Resolve<IDefaultValues>();
            base.LoginAsHostAdmin();
           
        }

        [Fact]
        public async Task Test_Sync()
        {
            await _currencyAppService.Sync();

            var defaultCurrencies = _defaultValues.Currencies.OrderBy(u => u.Code).ToList();

            // Assert
            UsingDbContext(context =>
            {
                var currencyEntities = context.Currencies.OrderBy(u=>u.Code).ToList();
                currencyEntities.Count.ShouldBe(defaultCurrencies.Count());

                for (var i=0;i<currencyEntities.Count; i++)
                {
                    currencyEntities[i].Code.ShouldBe(defaultCurrencies[i].Code);
                    currencyEntities[i].Name.ShouldBe(defaultCurrencies[i].Name);
                    currencyEntities[i].Symbol.ShouldBe(defaultCurrencies[i].Symbol);
                    currencyEntities[i].PluralName.ShouldBe(defaultCurrencies[i].PluralName);
                }
                
            });

            var khr = _defaultValues.Currencies.OrderBy(u=>u.Code).FirstOrDefault(u => u.Code == "KHR");
            khr.Name = "Change KHR";
            khr.PluralName = "Change KHR";

            await _currencyAppService.Sync();

            UsingDbContext(context =>
            {
                var khrEntity = context.Currencies.FirstOrDefault(u => u.Code == khr.Code);
                khrEntity.Name.ShouldBe(khr.Name);
                khrEntity.Symbol.ShouldBe(khr.Symbol);
                khrEntity.PluralName.ShouldBe(khr.PluralName);

            });
        }

        [Fact]
        public async Task Test_GetCurrency()
        {
            await _currencyAppService.Sync();
            var defaultCurrencies = _defaultValues.Currencies.OrderBy(u => u.Code).ToList();
            //base.LoginAsDefaultTenantAdmin();
            var output = await _currencyAppService.GetList(new GetCurrencyListInput()
            {
                Filter = ""
            });
            UsingDbContext(context =>
            {
                var currencyEntities = context.Currencies.OrderBy(u => u.Code).ToList();
                currencyEntities.Count.ShouldBe(defaultCurrencies.Count());

                for (var i = 0; i < currencyEntities.Count; i++)
                {   
                    output.Items[i].Code.ShouldBe(defaultCurrencies[i].Code);
                    output.Items[i].Name.ShouldBe(defaultCurrencies[i].Name);
                    output.Items[i].Symbol.ShouldBe(defaultCurrencies[i].Symbol);
                    output.Items[i].PluralName.ShouldBe(defaultCurrencies[i].PluralName);
                }

            });    
        }

    }
}
