using CorarlERP.Formats;
using CorarlERP.Formats.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.Formats
{
   public class FormatAppService_Test : AppTestBase
    {
        private readonly IFormatAppService _formatAppService;
        private readonly IDefaultValues _defaultValues;

        public FormatAppService_Test()
        {
            _formatAppService = Resolve<IFormatAppService>();
            _defaultValues = Resolve<IDefaultValues>();
            LoginAsHostAdmin();
        }


        [Fact]
        public async Task Test_Sync()
        {
            await _formatAppService.Sync();

            var defaultCurrencies = _defaultValues.Formats.OrderBy(u => u.Key).ToList();

            // Assert
            UsingDbContext(context =>
            {
                var formatEntities = context.Formats.OrderBy(u => u.Key).ToList();
                formatEntities.Count.ShouldBe(defaultCurrencies.Count());

                for (var i = 0; i < formatEntities.Count; i++)
                {

                    formatEntities[i].Name.ShouldBe(defaultCurrencies[i].Name);
                    formatEntities[i].Key.ShouldBe(defaultCurrencies[i].Key);
                    formatEntities[i].Web.ShouldBe(defaultCurrencies[i].Web);
                }

            });

            var khr = _defaultValues.Formats.OrderBy(u => u.Name).FirstOrDefault(u => u.Name == "MM/DD/yyyy");
            khr.Name = "123,456,78.00";
            khr.Key = "Number";

            await _formatAppService.Sync();

            UsingDbContext(context =>
            {
                var khrEntity = context.Formats.FirstOrDefault(u => u.Name == khr.Name);
                khrEntity.Name.ShouldBe(khr.Name);
                khrEntity.Web.ShouldBe(khr.Web);
                khrEntity.Key.ShouldBe(khr.Key);

            });
        }


        [Fact]
        public async Task Test_GetFormat()
        {
            await _formatAppService.Sync();
          
            var defaultFormats = _defaultValues.Formats.OrderBy(u => u.Key).ToList();
            
            var output = await _formatAppService.GetList(new GetFormatListInput()
            {
                Filter = ""
            });
            UsingDbContext(context =>
            {
                var formatEntities = context.Formats.OrderBy(u => u.Key).ToList();
                formatEntities.Count.ShouldBe(defaultFormats.Count());

                for (var i = 0; i < formatEntities.Count; i++)
                {
                    formatEntities[i].Name.ShouldBe(defaultFormats[i].Name);
                    formatEntities[i].Key.ShouldBe(defaultFormats[i].Key);
                    formatEntities[i].Web.ShouldBe(defaultFormats[i].Web);
                }

            });
        }

        [Fact]
        public async Task Test_FindNumberFormat()
        {
            await _formatAppService.Sync();
            LoginAsDefaultTenantAdmin();
            var defaultFormat = _defaultValues.Formats.Where(u => u.Key == "Number").OrderBy(u => u.Key).ToList();
            LoginAsDefaultTenantAdmin();
            var output = await _formatAppService.FindNumber(new GetFormatListInput()
            {
                Filter = ""
            });
            UsingDbContext(context =>
            {
                var formatEntities = context.Formats.Where(u=>u.Key=="Number").OrderBy(u => u.Key).ToList();
                formatEntities.Count.ShouldBe(defaultFormat.Count());

                for (var i = 0; i < formatEntities.Count; i++)
                {
                    formatEntities[i].Name.ShouldBe(defaultFormat[i].Name);
                    formatEntities[i].Key.ShouldBe(defaultFormat[i].Key);
                    formatEntities[i].Web.ShouldBe(defaultFormat[i].Web);
                }

            });
        }

        [Fact]
        public async Task Test_FindDateFormat()
        {
            await _formatAppService.Sync();
            LoginAsDefaultTenantAdmin();
            var defaultFormats = _defaultValues.Formats.Where(u => u.Key == "Date").OrderBy(u => u.Key).ToList();
            var output = await _formatAppService.FindDate(new GetFormatListInput()
            {
                Filter = ""
            });
            UsingDbContext(context =>
            {
                var formatEntities = context.Formats.Where(u => u.Key == "Date").OrderBy(u => u.Key).ToList();
                formatEntities.Count.ShouldBe(defaultFormats.Count());

                for (var i = 0; i < formatEntities.Count; i++)
                {
                    formatEntities[i].Name.ShouldBe(defaultFormats[i].Name);
                    formatEntities[i].Key.ShouldBe(defaultFormats[i].Key);
                    formatEntities[i].Web.ShouldBe(defaultFormats[i].Web);
                }

            });
        }
    }
}
