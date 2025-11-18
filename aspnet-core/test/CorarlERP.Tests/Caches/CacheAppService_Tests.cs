using Abp.Application.Services.Dto;
using CorarlERP.Caches;
using CorarlERP.Caches.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.Caches
{
    public class CacheAppService_Tests : AppTestBase
    {
        private readonly ICacheAppService _cacheAppService;
        public CacheAppService_Tests()
        {
            _cacheAppService = Resolve<ICacheAppService>();
        }

        private CreateOrUpdateCache CreateHelper()
        {
            var input = new CreateOrUpdateCache()
            {
                KeyName = "transfer-order",
                KeyValue = "[{Name: 'testing'}]"
            };
            return input;
        }

        private void CheckCache(CreateOrUpdateCache input, long? Id = null)
        {
            UsingDbContext(context =>
            {
                var entity = context.Caches.FirstOrDefault(p => p.KeyName == input.KeyName);
                entity.ShouldNotBeNull();
                if (Id != null) entity.Id.ShouldBe(Id.Value);
                entity.KeyName.ShouldBe(input.KeyName);
                entity.KeyValue.ShouldBe(input.KeyValue);
            });
        }

        [Fact]
        public async Task TestCreate()
        {
            var input = CreateHelper();
            var result = await _cacheAppService.CreateOrUpdate(input);
            result.ShouldNotBeNull();
            CheckCache(input, result.Id);
        }

        [Fact]
        public async Task TestGetDetail()
        {
            var input = CreateHelper();
            var result = await _cacheAppService.CreateOrUpdate(input);
            result.ShouldNotBeNull();

            CheckCache(input, result.Id);


            //Act
            var output = await _cacheAppService.GetDetail(result.KeyName);

            //Assert
            output.Id.ShouldBe(result.Id);
            output.KeyName.ShouldBe(result.KeyName);
            output.KeyValue.ShouldBe(result.KeyValue);
        }
    }

}
