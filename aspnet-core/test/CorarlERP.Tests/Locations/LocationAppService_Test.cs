using Abp.Application.Services.Dto;
using CorarlERP.Classes;
using CorarlERP.Classes.Dto;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.Localization
{
   public class LocationAppService_Test : AppTestBase
    {
        private readonly ILocationAppService _locationAppService;


        public LocationAppService_Test()
        {
            LoginAsDefaultTenantAdmin();
              _locationAppService = Resolve<ILocationAppService>();           
        }
        private CreateLocationInput CreateHelper()
        {
            var ClassExemptInput = new CreateLocationInput()
            {
                LocationName = "AEON Shop KPC",
                LocationParent = false,
                ParentLocationId = 1, 
            };
            return ClassExemptInput;
        }
        private void CheckLocation(CreateLocationInput LocationExemptInput, long? Id = null)
        {
            UsingDbContext(context =>
            {
                var ClassEntity = context.Locations.FirstOrDefault(p => p.LocationName == LocationExemptInput.LocationName);
                ClassEntity.ShouldNotBeNull();
                if (Id != null) ClassEntity.Id.ShouldBe(Id.Value);               
                ClassEntity.LocationName.ShouldBe(LocationExemptInput.LocationName);
                ClassEntity.ParentLocationId.ShouldBe(LocationExemptInput.ParentLocationId);
                ClassEntity.LocationParent.ShouldBe(LocationExemptInput.LocationParent);
                ClassEntity.IsActive.ShouldBe(true);                                                                       
            });
        }
        [Fact]
        public async Task Test_CreateSingleLocation()
        {
            var LocationExemptInput = CreateHelper();
            var result = await _locationAppService.Create(LocationExemptInput);
            result.ShouldNotBeNull();
            CheckLocation(LocationExemptInput);
        }

        [Fact]
        public async Task Test_CreateSingleGetList()
        {
            var classes = CreateHelper();
            var result = await _locationAppService.Create(classes);
            
            var output = await _locationAppService.GetList(new GetLocationListInput()
            {
                Filter = ""


            });
            foreach (var i in output.Items)
            {

                i.LocationName.ShouldBe(classes.LocationName);
                i.ParentLocationId.Equals(classes.ParentLocationId);
                i.LocationParent.ShouldBe(classes.LocationParent);
                
            }
        }

        [Fact]
        public async Task Test_CreateSingleFind()
        {
            var classes = CreateHelper();
            var result = await _locationAppService.Create(classes);

            var output = await _locationAppService.Find(new GetLocationListInput()
            {
                Filter = ""


            });
            foreach (var i in output.Items)
            {

                i.LocationName.ShouldBe(classes.LocationName);
                i.ParentLocationId.Equals(classes.ParentLocationId);
                i.LocationParent.ShouldBe(classes.LocationParent);

            }
        }

        [Fact]
        public async Task Test_GetDetailClass()
        {

            var CleassExemptInput = CreateHelper();
            var result = await _locationAppService.Create(CleassExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _locationAppService.GetDetail(new EntityDto<long>()
            {

                Id = result.Id,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id);
            output.LocationName.ShouldBe(CleassExemptInput.LocationName);
            output.LocationParent.ShouldBe(CleassExemptInput.LocationParent);
            output.ParentLocationId.ShouldBe(CleassExemptInput.ParentLocationId);
            CheckLocation(CleassExemptInput);
        }

        [Fact]
        public async Task Test_DelectClass()
        {
            var ClassExemptInput = CreateHelper();
            var result = await _locationAppService.Create(ClassExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var ClassEntity = context.Locations.FirstOrDefault(p => p.LocationName == ClassExemptInput.LocationName);
            });
            await _locationAppService.Delete(new EntityDto<long> { Id = result.Id });

            UsingDbContext(context =>
            {
                context.PurchaseOrders.Count().ShouldBe(0);
            });
        }

    }
}
