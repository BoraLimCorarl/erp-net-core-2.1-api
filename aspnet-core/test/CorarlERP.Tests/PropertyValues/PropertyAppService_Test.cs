using Abp.Application.Services.Dto;
using CorarlERP.PropertyValues;
using CorarlERP.PropertyValues.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.PropertyValues
{
   public class PropertyAppService_Test: AppTestBase
    {
        private readonly IPropertyAppService _propertyAppService;
     
        public PropertyAppService_Test()
        {
            _propertyAppService = Resolve<IPropertyAppService>();
           
        }
        [Fact]
        public async Task Test_CreateSingleProperty()
        {
            var PropertyInput = new CreatePropertyInput()
            {
                Name = "Product Category",
                Values = new List<CreatePropertyValueInput>()
                {
                    new CreatePropertyValueInput()  { Value = "Computer"},
                    new CreatePropertyValueInput()  { Value = "Phone"},
                    new CreatePropertyValueInput() { Value="Accessories"}
                }
            };

            await _propertyAppService.Create(PropertyInput);        
            //Assert
            UsingDbContext(context =>
            {
                var proEntity = context.Properties.FirstOrDefault(p => p.Name == PropertyInput.Name);
                proEntity.ShouldNotBeNull();
                proEntity.Id.ShouldNotBe(0);
                proEntity.Name.ShouldBe(PropertyInput.Name);
                proEntity.IsActive.ShouldBe(true);

                var proValueEntity = context.PropertyValues.Where(p => p.PropertyId == proEntity.Id).OrderBy(u=>u.Value).ToList();
                var inputValues = PropertyInput.Values.OrderBy(u => u.Value).ToList();
                proValueEntity.ShouldNotBeNull();
                proValueEntity.Count.ShouldBe(inputValues.Count);

                for (var i= 0 ; i< proValueEntity.Count; i++)
                {
                    proValueEntity[i].Value.ShouldBe(inputValues[i].Value);
                   
                }

            });
        }

        [Fact]
        public async Task Test_GetProperties()
        {
            var PropertyInput = new CreatePropertyInput()
            {
                Name = "Product Category",
                Values = new List<CreatePropertyValueInput>()
                {
                    new CreatePropertyValueInput()  { Value = "Computer"},
                    new CreatePropertyValueInput()  { Value = "Phone"},
                    new CreatePropertyValueInput() { Value="Accessories"}
                }
            };
            await  _propertyAppService.Create(PropertyInput);

            //Act
            var output = await _propertyAppService.GetList(new GetPropertyListInput()
            {
                Filter = ""
            });       
            for (var i= 0; i < output.Items.Count; i++)
            {
                output.Items[i].Name.ShouldBe(PropertyInput.Name);
            }
                     
        }
     
        [Fact]
        public async Task Test_UpdateProperties()
        {
           var PropertyInput = new CreatePropertyInput()
            {
                Name = "Product Category",
                Values = new List<CreatePropertyValueInput>()
                {
                    new CreatePropertyValueInput()  { Value = "Computer"},
                    new CreatePropertyValueInput()  { Value = "Phone"},
                    new CreatePropertyValueInput() { Value="Accessories"}
                }
            };
          await _propertyAppService.Create(PropertyInput);          
          long Pid = 0;
            UsingDbContext(context =>
            {
                var proEntity = context.Properties.FirstOrDefault(p => p.Name == PropertyInput.Name);
                proEntity.ShouldNotBeNull();
                proEntity.Id.ShouldNotBe(0);
                proEntity.Name.ShouldBe(PropertyInput.Name);
                proEntity.IsActive.ShouldBe(true);
                Pid = proEntity.Id;
                var proValueEntity = context.PropertyValues.Where(p => p.PropertyId == proEntity.Id).OrderBy(u => u.Value).ToList();
                var inputValues = PropertyInput.Values.OrderBy(u => u.Value).ToList();
                proValueEntity.ShouldNotBeNull();
                proValueEntity.Count.ShouldBe(inputValues.Count);
              
                for (var i = 0; i < proValueEntity.Count; i++)
                {
                    proValueEntity[i].Value.ShouldBe(inputValues[i].Value);

                }

            });
             

            var PropertyInputUpdate = new UpdatePropertyInput()
            {
                
                Id= Pid ,
                Name = "Update Property Input."

            };
            
            var updatedResult = await _propertyAppService.Update(PropertyInputUpdate);
            updatedResult.ShouldNotBeNull();
            updatedResult.Id.ShouldBe(PropertyInputUpdate.Id);
            updatedResult.Name.ShouldBe(PropertyInputUpdate.Name);

            //Assert
            UsingDbContext(context =>
            {
                var updatePropertyEntity = context.Properties.FirstOrDefault(p => p.Id == PropertyInputUpdate.Id);
                updatePropertyEntity.ShouldNotBeNull();
                updatePropertyEntity.Id.ShouldBe(updatedResult.Id);
                updatePropertyEntity.Name.ShouldBe(updatedResult.Name);

            });

        }

        [Fact]
        public async Task Test_Delete()
        {
            var PropertyInput = new CreatePropertyInput()
            {
                Name = "Product Category",
                Values = new List<CreatePropertyValueInput>()
                {
                    new CreatePropertyValueInput()  { Value = "Computer"},
                    new CreatePropertyValueInput()  { Value = "Phone"},
                    new CreatePropertyValueInput() { Value="Accessories"}
                }
            };

            await _propertyAppService.Create(PropertyInput);
            long Pid = 0;
            //Assert
            UsingDbContext(context =>
            {

                var PropertyEntity = context.Properties.FirstOrDefault(p => p.Name == PropertyInput.Name);
                PropertyEntity.ShouldNotBeNull();
                Pid = PropertyEntity.Id;
                var proValueEntity = context.PropertyValues.Where(p => p.PropertyId == Pid).OrderBy(u => u.Value).ToList();
                //for (var pv = 0; pv < proValueEntity.Count; pv++)
                //{
                //    context.PropertyValues.Remove(proValueEntity[pv]);
                //    context.SaveChanges();
                //}

            });

            await _propertyAppService.Delete(new EntityDto<long> { Id = Pid });

            UsingDbContext(context =>
            {
                context.Properties.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetFind()
        {
            var PropertyInput = new CreatePropertyInput()
            {
                Name = "Product Category",
                Values = new List<CreatePropertyValueInput>()
                {
                    new CreatePropertyValueInput()  { Value = "Computer"},
                    new CreatePropertyValueInput()  { Value = "Phone"},
                    new CreatePropertyValueInput() { Value="Accessories"}
                }
            };
            await _propertyAppService.Create(PropertyInput);

            //Act
            var output = await _propertyAppService.Find(new GetPropertyListInput()
            {
                Filter = ""
            });
            for (var i = 0; i < output.Items.Count; i++)
            {
                output.Items[i].Name.ShouldBe(PropertyInput.Name);
            }

        }
    }
}
